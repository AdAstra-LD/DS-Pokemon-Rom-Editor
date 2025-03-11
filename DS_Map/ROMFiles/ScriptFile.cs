using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static DSPRE.RomInfo;

namespace DSPRE.ROMFiles {
    /// <summary>
    /// Class to store script file data in Pokémon NDS games
    /// </summary>
    public class ScriptFile : RomFile {
        public List<ScriptCommandPosition> CommandSequence { get; set; } = new List<ScriptCommandPosition>();
        public Dictionary<int, string> OffsetToLabelMap { get; set; } = new Dictionary<int, string>();
        public enum ContainerTypes {
            Action,
            Script,
            Label
        };

        public struct ContainerReference {
            public uint ID;
            public uint offsetInFile;
        }

        public int fileID = -1;
        public bool isLevelScript = new bool();

        public bool HasNoScripts { get { return fileID == int.MaxValue; } }

        public static readonly char[] specialChars = { 'x', 'X', '#', '.', '_' };

        public ScriptFile(Stream fs) {
            // Initialize collections
            CommandSequence = new List<ScriptCommandPosition>();
            OffsetToLabelMap = new Dictionary<int, string>();

            using (BinaryReader br = new BinaryReader(fs)) {
                // Read header to find entry points
                List<int> entryPointOffsets = new List<int>();
                isLevelScript = true; // Is Level Script until proved otherwise

                try {
                    int entryPointIndex = 0;
                    while (true) {
                        long headerPos = br.BaseStream.Position;
                        uint checker = br.ReadUInt16();
                        br.BaseStream.Position -= 0x2;
                        uint value = br.ReadUInt32();

                        if (value == 0 && entryPointOffsets.Count == 0) {
                            isLevelScript = true;
                            break;
                        }

                        if (checker == 0xFD13) {
                            br.BaseStream.Position -= 0x4;
                            isLevelScript = false;
                            break;
                        }

                        int offsetFromStart = (int)(value + headerPos + 4);
                        entryPointOffsets.Add(offsetFromStart);

                        // Create entry point label
                        string entryLabel = $"script_{entryPointIndex}";
                        OffsetToLabelMap[offsetFromStart] = entryLabel;
                        entryPointIndex++;
                    }
                } catch (EndOfStreamException) {
                    if (!isLevelScript) {
                        MessageBox.Show("Script File couldn't be read correctly.", "Unexpected EOF", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                if (isLevelScript) {
                    return;
                }

                // Skip the 0xFD13 marker
                br.ReadUInt16();

                // Process commands until end of file
                while (br.BaseStream.Position < br.BaseStream.Length) {
                    int currentOffset = (int)br.BaseStream.Position;

                    // Sometimes we might hit padding bytes - try to detect and skip
                    if (br.BaseStream.Position + 2 <= br.BaseStream.Length) {
                        byte[] peekBytes = br.ReadBytes(2);
                        br.BaseStream.Position -= 2;

                        // Check if these look like padding
                        if (peekBytes[0] == 0 && peekBytes[1] == 0) {
                            // Skip padding byte
                            br.ReadByte();
                            continue;
                        }
                    }

                    ScriptCommand cmd = ReadCommand(br);
                    if (cmd == null || cmd.id == null) {
                        break; // End of file or error
                    }

                    // Check if this is an entry point or jump target
                    bool isEntryPoint = false;
                    int entryPointIndex = -1;
                    string label = null;

                    if (OffsetToLabelMap.TryGetValue(currentOffset, out string existingLabel)) {
                        label = existingLabel;
                        isEntryPoint = existingLabel.StartsWith("script_");
                        if (isEntryPoint) {
                            entryPointIndex = int.Parse(existingLabel.Substring("script_".Length));
                        }
                    }

                    // Add to command sequence
                    CommandSequence.Add(new ScriptCommandPosition(
                        cmd, currentOffset, label, isEntryPoint, entryPointIndex));
                }
            }
        }

        public ScriptFile(int fileID) : this(getFileStream(fileID)) {
            this.fileID = fileID;
        }

        public static FileStream getFileStream(int fileID) {
            string path = Filesystem.GetScriptPath(fileID);
            return new FileStream(path, FileMode.OpenOrCreate);
        }

        public override string ToString() {
            string prefix = isLevelScript ? "Level " : "";
            return $"{prefix}Script File " + this.fileID;
        }

        public ScriptFile(IEnumerable<string> lines, int fileID = -1) {
            CommandSequence = new List<ScriptCommandPosition>();
            OffsetToLabelMap = new Dictionary<int, string>();
            this.fileID = fileID;

            int currentOffset = 0;
            string currentLabel = null;
            int entryPointIndex = 0;
            bool isCurrentLabelEntryPoint = false;  // Track this instead

            // Parse each line
            foreach (var line in lines) {
                string trimmedLine = line.Trim();

                // Skip empty lines
                if (string.IsNullOrWhiteSpace(trimmedLine)) {
                    continue;
                }

                // Check if this is a label
                if (trimmedLine.EndsWith(":")) {
                    currentLabel = trimmedLine.Substring(0, trimmedLine.Length - 1);

                    // Track if it's an entry point
                    isCurrentLabelEntryPoint = currentLabel.StartsWith("script_");
                    if (isCurrentLabelEntryPoint) {
                        if (int.TryParse(currentLabel.Substring("script_".Length), out int index)) {
                            entryPointIndex = index;
                        }
                    }

                    continue;
                }

                // Parse the command
                ScriptCommand cmd = new ScriptCommand(trimmedLine);
                if (cmd.id == null) {
                    continue; // Skip invalid commands
                }

                // Calculate the size of this command for offset tracking
                int cmdSize = 2; // Command ID (2 bytes)
                foreach (var param in cmd.Parameters) {
                    cmdSize += param.RawData.Length;
                }

                // Use the tracked entry point flag
                int epIndex = isCurrentLabelEntryPoint ? entryPointIndex : -1;

                CommandSequence.Add(new ScriptCommandPosition(
                    cmd, currentOffset, currentLabel, isCurrentLabelEntryPoint, epIndex));

                // Map the offset to the label for jump targets
                if (currentLabel != null) {
                    OffsetToLabelMap[currentOffset] = currentLabel;
                    currentLabel = null; // Reset label
                    isCurrentLabelEntryPoint = false; // Reset entry point flag
                }

                // Update offset for next command
                currentOffset += cmdSize;
            }
        }

        private ScriptCommand ReadCommand(BinaryReader br) {
            // Check if we've reached the end of the file
            if (br.BaseStream.Position >= br.BaseStream.Length) {
                return null;
            }

            try {
                ushort id = br.ReadUInt16();
                List<ScriptParameter> parameters = new List<ScriptParameter>();

                // Track the original position for jump calculations
                long commandStartPos = br.BaseStream.Position - 2;

                switch (gameFamily) {
                    case GameFamilies.Plat:
                        switch (id) {
                            case 0x16: // Jump
                            case 0x1A: // Call
                                ProcessRelativeJumpLinear(br, parameters);
                                break;

                            case 0x17: // JumpIfObjID
                            case 0x18: // JumpIfEventID
                            case 0x19: // JumpIfPlayerDir
                            case 0x1C: // JumpIf
                            case 0x1D: // CallIf
                                       // First parameter (condition)
                                parameters.Add(new ScriptParameter(new byte[] { br.ReadByte() }));
                                // Then jump target
                                ProcessRelativeJumpLinear(br, parameters);
                                break;

                            case 0x5E: // Movement
                                parameters.Add(new ScriptParameter(BitConverter.GetBytes(br.ReadUInt16())));
                                ProcessRelativeJumpLinear(br, parameters);
                                break;

                            case 0x1CF:
                            case 0x1D0:
                            case 0x1D1: {
                                    byte parameter1 = br.ReadByte();
                                    parameters.Add(new ScriptParameter(new byte[] { parameter1 }));
                                    if (parameter1 == 0x2) {
                                        parameters.Add(new ScriptParameter(br.ReadBytes(2))); //Read additional u16 if first param read is 2
                                    }
                                }
                                break;
                            case 0x21D: {
                                    ushort parameter1 = br.ReadUInt16();
                                    parameters.Add(new ScriptParameter(BitConverter.GetBytes(parameter1)));

                                    switch (parameter1) {
                                        case 0:
                                        case 1:
                                        case 2:
                                        case 3:
                                            parameters.Add(new ScriptParameter(br.ReadBytes(2)));
                                            parameters.Add(new ScriptParameter(br.ReadBytes(2)));
                                            break;
                                        case 4:
                                        case 5:
                                            parameters.Add(new ScriptParameter(br.ReadBytes(2)));
                                            break;
                                        case 6:
                                            break;
                                    }
                                }
                                break;
                            case 0x235: {
                                    short parameter1 = br.ReadInt16();
                                    parameters.Add(new ScriptParameter(BitConverter.GetBytes(parameter1)));

                                    switch (parameter1) {
                                        case 0x1:
                                        case 0x3:
                                            parameters.Add(new ScriptParameter(br.ReadBytes(2)));
                                            parameters.Add(new ScriptParameter(br.ReadBytes(2)));
                                            parameters.Add(new ScriptParameter(br.ReadBytes(2)));
                                            break;
                                        case 0x4:
                                            parameters.Add(new ScriptParameter(br.ReadBytes(2)));
                                            parameters.Add(new ScriptParameter(br.ReadBytes(2)));
                                            break;
                                        case 0x0:
                                        case 0x6:
                                            parameters.Add(new ScriptParameter(br.ReadBytes(2)));
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            case 0x23E: {
                                    short parameter1 = br.ReadInt16();
                                    parameters.Add(new ScriptParameter(BitConverter.GetBytes(parameter1)));

                                    switch (parameter1) {
                                        case 0x1:
                                        case 0x3:
                                            parameters.Add(new ScriptParameter(br.ReadBytes(2)));
                                            break;
                                        case 0x5:
                                        case 0x6:
                                            parameters.Add(new ScriptParameter(br.ReadBytes(2)));
                                            parameters.Add(new ScriptParameter(br.ReadBytes(2)));
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            case 0x2C4: {
                                    byte parameter1 = br.ReadByte();
                                    parameters.Add(new ScriptParameter(new byte[] { parameter1 }));
                                    if (parameter1 == 0 || parameter1 == 1) {
                                        parameters.Add(new ScriptParameter(br.ReadBytes(2)));
                                    }
                                }
                                break;
                            case 0x2C5: {
                                    if (RomInfo.gameVersion == GameVersions.Platinum) {
                                        parameters.Add(new ScriptParameter(br.ReadBytes(2)));
                                        parameters.Add(new ScriptParameter(br.ReadBytes(2)));
                                    } else {
                                        goto default;
                                    }
                                }
                                break;
                            case 0x2C6:
                            case 0x2C9:
                            case 0x2CA:
                            case 0x2CD:
                                if (RomInfo.gameVersion == GameVersions.Platinum) {
                                    break;
                                } else {
                                    goto default;
                                }
                            case 0x2CF:
                                if (RomInfo.gameVersion == GameVersions.Platinum) {
                                    parameters.Add(new ScriptParameter(br.ReadBytes(2)));
                                    parameters.Add(new ScriptParameter(br.ReadBytes(2)));
                                } else {
                                    goto default;
                                }

                                break;
                            default:
                                // Standard command handling
                                if (RomInfo.ScriptCommandParametersDict.TryGetValue(id, out byte[] paramSizes)) {
                                    foreach (int size in paramSizes) {
                                        parameters.Add(new ScriptParameter(br.ReadBytes(size)));
                                    }
                                }
                                break;
                        }
                    break;

                    case GameFamilies.HGSS:
                        switch (id) {
                            case 0x16: //Jump
                            case 0x1A: //Call 
                                ProcessRelativeJumpLinear(br, parameters);
                                break;

                            case 0x17: //JumpIfObjID
                            case 0x18: //JumpIfBgID
                            case 0x19: //JumpIfPlayerDir
                            case 0x1C: //JumpIf
                            case 0x1D: //CallIf
                                parameters.Add(new ScriptParameter(new byte[] { br.ReadByte() })); //in the case of JumpIf and CallIf, the first param is a comparisonOperator
                                ProcessRelativeJumpLinear(br, parameters);
                                break;

                            case 0x5E: // Movement
                                parameters.Add(new ScriptParameter(BitConverter.GetBytes(br.ReadUInt16()))); //in the case of Movement, the first param is an overworld ID
                                ProcessRelativeJumpLinear(br, parameters);
                                break;

                            case 0x190:
                            case 0x191:
                            case 0x192: {
                                    byte parameter1 = br.ReadByte();
                                    parameters.Add(new ScriptParameter(new byte[] { parameter1 }));
                                    if (parameter1 == 0x2) {
                                        parameters.Add(new ScriptParameter(br.ReadBytes(2)));
                                    }
                                }
                                break;

                            case 0x1D1: // Number of parameters differ depending on the first parameter value
                            {
                                    short parameter1 = br.ReadInt16();
                                    parameters.Add(new ScriptParameter(BitConverter.GetBytes(parameter1)));
                                    switch (parameter1) {
                                        case 0x0:
                                        case 0x1:
                                        case 0x2:
                                        case 0x3:
                                            parameters.Add(new ScriptParameter(br.ReadBytes(2)));
                                            parameters.Add(new ScriptParameter(br.ReadBytes(2)));
                                            break;
                                        case 0x4:
                                        case 0x5:
                                            parameters.Add(new ScriptParameter(br.ReadBytes(2)));
                                            break;
                                        case 0x6:
                                            break;
                                        case 0x7:
                                            parameters.Add(new ScriptParameter(br.ReadBytes(2)));
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;

                            case 0x1E9: // Number of parameters differ depending on the first parameter value
                                {
                                    short parameter1 = br.ReadInt16();
                                    parameters.Add(new ScriptParameter(BitConverter.GetBytes(parameter1)));
                                    switch (parameter1) {
                                        case 0x0:
                                            break;
                                        case 0x1:
                                        case 0x2:
                                        case 0x3:
                                            parameters.Add(new ScriptParameter(br.ReadBytes(2)));
                                            break;
                                        case 0x4:
                                            break;
                                        case 0x5:
                                        case 0x6:
                                            parameters.Add(new ScriptParameter(br.ReadBytes(2)));
                                            parameters.Add(new ScriptParameter(br.ReadBytes(2)));
                                            break;
                                        case 0x7:
                                        case 0x8:
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            break;

                            default:
                                // For standard commands, read parameters based on definition
                                if (RomInfo.ScriptCommandParametersDict.TryGetValue(id, out byte[] paramSizes)) {
                                    foreach (int size in paramSizes) {
                                        parameters.Add(new ScriptParameter(br.ReadBytes(size)));
                                    }
                                }
                                break;
                        }

                    break;
                }

                return new ScriptCommand(id, parameters);
            } catch (Exception ex) {
                Console.WriteLine($"Error reading command at offset {br.BaseStream.Position}: {ex.Message}");
                return null;
            }
        }

        private void ProcessRelativeJumpLinear(BinaryReader br, List<ScriptParameter> parameters) {
            // Read the relative offset
            int relativeOffset = br.ReadInt32();

            // Calculate absolute target
            int targetOffset = (int)(relativeOffset + br.BaseStream.Position);

            // Add to label map if not already there
            if (!OffsetToLabelMap.ContainsKey(targetOffset)) {
                string labelName = $"label_0x{targetOffset:X}";
                OffsetToLabelMap[targetOffset] = labelName;
            }

            // Create jump parameter
            string targetLabel = OffsetToLabelMap[targetOffset];
            ScriptParameter jumpParam = new ScriptParameter(relativeOffset, targetLabel) {
                Type = ScriptParameter.ParameterType.RelativeJump
            };
            parameters.Add(jumpParam);
        }

        // Convert to text - outputs commands in the order they appear in the binary
        public string ToText() {
            StringBuilder sb = new StringBuilder();

            foreach (var cmdPos in CommandSequence) {
                // If this command needs a label, output it
                if (!string.IsNullOrEmpty(cmdPos.Label)) {
                    sb.AppendLine($"\n{cmdPos.Label}:");
                }

                // Output the command (indented if not an entry point)
                string indent = "\t";
                sb.AppendLine($"{indent}{cmdPos.Command.name}");
            }

            return sb.ToString();
        }

        public override byte[] ToByteArray() {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData)) {
                // First, find all entry points and their positions in the command sequence
                var entryPoints = CommandSequence
                    .Where(c => c.IsEntryPoint)
                    .OrderBy(c => c.EntryPointIndex)
                    .ToList();

                // Allocate space for header
                long headerStart = writer.BaseStream.Position;
                writer.BaseStream.Position += entryPoints.Count * 4;
                writer.Write((ushort)0xFD13); // End of header marker

                // Keep track of command offsets in the new file
                Dictionary<int, long> oldOffsetToNewOffset = new Dictionary<int, long>();

                            functionOffsets.Add(new ContainerReference() {
                                ID = currentFunction.manualUserID,
                                offsetInFile = scriptOffsets.Find(x => x.ID == currentFunction.usedScriptID).offsetInFile
                            });
                        }
                    }
                // Write all commands sequentially
                foreach (var cmdPos in CommandSequence) {
                    // Record the position of this command
                    oldOffsetToNewOffset[cmdPos.Offset] = writer.BaseStream.Position;

                    // Write the command
                    writer.Write((ushort)cmdPos.Command.id);

                    // Write parameters, handling jumps specially
                    foreach (var param in cmdPos.Command.Parameters) {
                        if (param.Type == ScriptParameter.ParameterType.RelativeJump) {
                            // For jump targets, we need to recalculate the relative offset
                            // based on the new positions of commands

                            // Find the target offset in the original file
                            int targetOffset = -1;
                            foreach (var kvp in OffsetToLabelMap) {
                                if (kvp.Value == param.TargetLabel) {
                                    targetOffset = kvp.Key;
                                    break;
                                }
                            }

                            if (targetOffset != -1 && oldOffsetToNewOffset.TryGetValue(targetOffset, out long newTargetOffset)) {
                                // Calculate new relative offset
                                int relativeOffset = (int)(newTargetOffset - (writer.BaseStream.Position + 4));
                                writer.Write(relativeOffset);
                            } else {
                                // Fallback - write original offset
                                writer.Write(param.TargetOffset);
                            }
                        } else {
                            // Regular parameter
                            writer.Write(param.RawData);
                        }
                    }
                }

                // Update header with entry point offsets
                writer.BaseStream.Position = headerStart;
                foreach (var entryPoint in entryPoints) {
                    if (oldOffsetToNewOffset.TryGetValue(entryPoint.Offset, out long newOffset)) {
                        uint relativeOffset = (uint)(newOffset - (headerStart + 4));
                        writer.Write(relativeOffset);
                    }
                }
            }

            return newData.ToArray();
        }

        public bool SaveToFileDefaultDir(int IDtoReplace, bool showSuccessMessage = true) {
            return SaveToFileDefaultDir(RomInfo.DirNames.scripts, IDtoReplace, showSuccessMessage);
        }

        public void SaveToFileExplorePath(string suggestedFileName, bool blindmode) {
            SaveFileDialog sf = new SaveFileDialog {
                Filter = "Gen IV Script File (*.scr)|*.scr"
            };

            if (!string.IsNullOrEmpty(suggestedFileName)) {
                sf.FileName = suggestedFileName;
            }

            if (sf.ShowDialog() != DialogResult.OK) {
                return;
            }

            if (blindmode) {
                string path = Filesystem.GetScriptPath(fileID);
                File.Copy(path, sf.FileName, overwrite: true);

                string msg = "";
                if (!isLevelScript) {
                    msg += "The last saved version of this ";
                }

                MessageBox.Show(msg + GetType().Name + " has been exported successfully.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } else {
                this.SaveToFile(sf.FileName, showSuccessMessage: true);
            }
        }
    }
}
