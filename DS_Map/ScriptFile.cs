using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DSPRE {
    /// <summary>
    /// Class to store script file data in Pokémon NDS games
    /// </summary>
    public class ScriptFile {
        #region Fields (3)
        public List<Script> scripts = new List<Script>();
        public List<Script> functions = new List<Script>();
        public List<Script> movements = new List<Script>();
        public bool isLevelScript = new bool();
        #endregion

        #region Constructors (1)
        public ScriptFile(Stream fs) {
            List<uint> scriptOffsets = new List<uint>();
            List<uint> functionOffsets = new List<uint>();
            List<uint> movementOffsets = new List<uint>();
            ushort[] endCodes = new ushort[] { 0x2, 0x16, 0x1B };

            using (BinaryReader scrReader = new BinaryReader(fs)) {
                /* Read script offsets from the header */
                isLevelScript = true; // Is Level Script as long as magic number FD13 doesn't exist
                try {
                    while (true) {
                        uint checker = scrReader.ReadUInt16();
                        scrReader.BaseStream.Position -= 0x2;
                        uint value = scrReader.ReadUInt32();

                        if (value == 0) {
                            isLevelScript = true;
                            break;
                        } else if (checker == 0xFD13) {
                            scrReader.BaseStream.Position -= 0x4;
                            isLevelScript = false;
                            break;
                        } else {
                            uint offsetFromStart = value + (uint)scrReader.BaseStream.Position;
                            scriptOffsets.Add(offsetFromStart); // Don't change order of addition
                        }
                    }
                } catch (EndOfStreamException) {
                    if (!isLevelScript)
                        MessageBox.Show("Script File couldn't be read correctly.", "Unexpected EOF", MessageBoxButtons.OK, MessageBoxIcon.Error); // Now this may appear in a few level scripts that don't have a 4-byte aligned "00 00 00 00"
                }

                if (isLevelScript) {
                    return;
                }

                /* Read scripts */
                for (int i = 0; i < scriptOffsets.Count; i++) {
                    int duplicateIndex = scriptOffsets.FindIndex(offset => offset == scriptOffsets[i]); // Check for UseScript_#

                    if (duplicateIndex == i) {
                        scrReader.BaseStream.Position = scriptOffsets[i];

                        List<ScriptCommand> commandsList = new List<ScriptCommand>();
                        bool endScript = new bool();
                        while (!endScript) {
                            ScriptCommand command = ReadCommand(scrReader, ref functionOffsets, ref movementOffsets);
                            if (command.parameterList == null) 
                                return;
                            
                            commandsList.Add(command);

                            if (endCodes.Contains(command.id))
                                endScript = true;
                            
                        }
                        scripts.Add(new Script(commandList: commandsList));
                    } else {
                        scripts.Add(new Script(useScript: duplicateIndex));
                    }
                }

                /* Read functions */
                for (int i = 0; i < functionOffsets.Count; i++) {
                    scrReader.BaseStream.Position = functionOffsets[i];

                    List<ScriptCommand> cmdList = new List<ScriptCommand>();
                    bool endFunction = new bool();
                    while (!endFunction) {
                        ScriptCommand command = ReadCommand(scrReader, ref functionOffsets, ref movementOffsets);
                        if (command.parameterList == null)
                            return;

                        cmdList.Add(command);
                        if (endCodes.Contains(command.id))
                            endFunction = true;
                    }

                    this.functions.Add(new Script(commandList: cmdList));
                }

                /* Read movements */
                for (int i = 0; i < movementOffsets.Count; i++) {
                    scrReader.BaseStream.Position = movementOffsets[i];

                    List<ScriptCommand> cmdList = new List<ScriptCommand>();
                    bool endMovement = new bool();
                    while (!endMovement) {
                        ushort id = scrReader.ReadUInt16();
                        List<byte[]> parameters = new List<byte[]>();
                        if (id != 0xFE)
                            parameters.Add(scrReader.ReadBytes(2));
                        else
                            endMovement = true;

                        ScriptCommand command = new ScriptCommand(id, parameters, isMovement: true);
                        cmdList.Add(command);
                    }
                    this.movements.Add(new Script(commandList: cmdList));
                }
            }
        }
        public ScriptFile(int fileID) : this((new FileStream(RomInfo.scriptDirPath +
        "\\" + fileID.ToString("D4"), FileMode.Open))) {
        }
        public ScriptFile(List<Script> scripts, List<Script> functions, List<Script> movements) {
            this.scripts = scripts;
            this.functions = functions;
            this.movements = movements;
            isLevelScript = false;
        }
        #endregion

        #region Methods (1)
        private ScriptCommand ReadCommand(BinaryReader dataReader, ref List<uint> functionOffsets, ref List<uint> movementOffsets) {
            ushort id = dataReader.ReadUInt16();
            List<byte[]> parameterList = new List<byte[]>();

            /* How to read parameters for different commands for DPPt*/
            switch (RomInfo.gameVersion) {
                case "D":
                case "P":
                case "Plat":
                    switch (id)  {
                        case 0x16: //Jump
                        case 0x1A: //Call
                            uint offset = dataReader.ReadUInt32() + (uint)dataReader.BaseStream.Position; // Do not change order of addition
                            if (!functionOffsets.Contains(offset))
                                functionOffsets.Add(offset);

                            parameterList.Add(BitConverter.GetBytes(functionOffsets.IndexOf(offset)));
                            break;
                        case 0x1C: //CompareLastResultJump
                        case 0x1D: //CompareLastResultCall
                            byte opcode = dataReader.ReadByte();
                            offset = dataReader.ReadUInt32() + (uint)dataReader.BaseStream.Position; // Do not change order of addition
                            if (!functionOffsets.Contains(offset))
                                functionOffsets.Add(offset);

                            parameterList.Add(new byte[] { opcode });
                            parameterList.Add(BitConverter.GetBytes(functionOffsets.IndexOf(offset)));
                            break;
                        case 0x5E: // ApplyMovement
                        case 0x2A1: // ApplyMovement2
                            {
                                ushort overworld = dataReader.ReadUInt16();
                                offset = dataReader.ReadUInt32() + (uint)dataReader.BaseStream.Position; // Do not change order of addition
                                if (!movementOffsets.Contains(offset))
                                    movementOffsets.Add(offset);

                                parameterList.Add(BitConverter.GetBytes(overworld));
                                parameterList.Add(BitConverter.GetBytes(movementOffsets.IndexOf(offset)));
                            }
                            break;
                        case 0x1CF:
                        case 0x1D0:
                        case 0x1D1: 
                            {
                                byte parameter1 = dataReader.ReadByte();
                                parameterList.Add(new byte[] { parameter1 });
                                if (parameter1 == 0x2)
                                    parameterList.Add(dataReader.ReadBytes(2)); //Read additional u16 if first param read is 2
                            }
                            break;
                        case 0x21D: 
                            {
                                ushort parameter1 = dataReader.ReadUInt16();
                                parameterList.Add(BitConverter.GetBytes( parameter1 ));

                                switch (parameter1) {
                                    case 0:
                                    case 1:
                                    case 2:
                                    case 3:
                                        parameterList.Add(dataReader.ReadBytes(2));
                                        parameterList.Add(dataReader.ReadBytes(2));
                                        break;
                                    case 4:
                                    case 5:
                                        parameterList.Add(dataReader.ReadBytes(2));
                                        break;
                                    case 6:
                                        break;
                                }
                            }
                            break;
                        case 0x235: 
                            {
                                short parameter1 = dataReader.ReadInt16();
                                parameterList.Add(BitConverter.GetBytes(parameter1));

                                switch (parameter1) {
                                    case 0x1:
                                    case 0x3:
                                        parameterList.Add(dataReader.ReadBytes(2));
                                        parameterList.Add(dataReader.ReadBytes(2));
                                        parameterList.Add(dataReader.ReadBytes(2));
                                        break;
                                    case 0x4:
                                        parameterList.Add(dataReader.ReadBytes(2));
                                        parameterList.Add(dataReader.ReadBytes(2));
                                        break;
                                    case 0x0:
                                    case 0x6:
                                        parameterList.Add(dataReader.ReadBytes(2));
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;
                        case 0x23E: 
                            {
                                short parameter1 = dataReader.ReadInt16();
                                parameterList.Add(BitConverter.GetBytes(parameter1));

                                switch (parameter1) {
                                    case 0x1:
                                    case 0x3:
                                        parameterList.Add(dataReader.ReadBytes(2));
                                        break;
                                    case 0x5:
                                    case 0x6:
                                        parameterList.Add(dataReader.ReadBytes(2));
                                        parameterList.Add(dataReader.ReadBytes(2));
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;
                        case 0x2C4: 
                            {
                                byte parameter1 = dataReader.ReadByte();
                                parameterList.Add(new byte[] { parameter1 });
                                if (parameter1 == 0 || parameter1 == 1)
                                    parameterList.Add(dataReader.ReadBytes(2));
                            }
                            break;
                        case 0x2C5: 
                            {
                                if (RomInfo.gameVersion == "Plat") {
                                    parameterList.Add(dataReader.ReadBytes(2));
                                    parameterList.Add(dataReader.ReadBytes(2));
                                } else {
                                    goto default;
                                }
                            }
                            break;
                        case 0x2C6:
                        case 0x2C9:
                        case 0x2CA:
                        case 0x2CD:
                            if (RomInfo.gameVersion == "Plat")
                                break;
                            else
                                goto default;
                        case 0x2CF:
                            if (RomInfo.gameVersion == "Plat") {
                                parameterList.Add(dataReader.ReadBytes(2));
                                parameterList.Add(dataReader.ReadBytes(2));
                            } else {
                                goto default;
                            }
                            break;
                        default:
                            addParametersToList(ref parameterList, id, dataReader);
                            break;
                    }
                    break;
                case "HG":
                case "SS":
                    switch (id) {
                        case 0x16: //Jump
                        case 0x1A: //Call
                            uint offset = dataReader.ReadUInt32() + (uint)dataReader.BaseStream.Position; // Do not change order of addition
                            if (!functionOffsets.Contains(offset))
                                functionOffsets.Add(offset);

                            parameterList.Add(BitConverter.GetBytes(functionOffsets.IndexOf(offset)));
                            break;
                        case 0x1C: //CompareLastResultJump
                        case 0x1D: //CompareLastResultCall
                            byte opcode = dataReader.ReadByte();
                            offset = dataReader.ReadUInt32() + (uint)dataReader.BaseStream.Position; // Do not change order of addition
                            if (!functionOffsets.Contains(offset))
                                functionOffsets.Add(offset);

                            parameterList.Add(new byte[] { opcode });
                            parameterList.Add(BitConverter.GetBytes(functionOffsets.IndexOf(offset)));
                            break;
                        case 0x5E: // ApplyMovement
                            {
                                ushort overworld = dataReader.ReadUInt16();
                                offset = dataReader.ReadUInt32() + (uint)dataReader.BaseStream.Position; // Do not change order of addition
                                if (!movementOffsets.Contains(offset))
                                    movementOffsets.Add(offset);

                                parameterList.Add(BitConverter.GetBytes(overworld));
                                parameterList.Add(BitConverter.GetBytes(movementOffsets.IndexOf(offset)));
                            }
                            break;
                        case 0x190:
                        case 0x191:
                        case 0x192: 
                            {
                                byte parameter1 = dataReader.ReadByte();
                                parameterList.Add(new byte[] { parameter1 });
                                if (parameter1 == 0x2)
                                    parameterList.Add(dataReader.ReadBytes(2));

                            }
                            break;
                        case 0x1D1: // Number of parameters differ depending on the first parameter value
                            {
                                short parameter1 = dataReader.ReadInt16();
                                parameterList.Add(BitConverter.GetBytes(parameter1));
                                switch (parameter1) {
                                    case 0x0:
                                    case 0x1:
                                    case 0x2:
                                    case 0x3:
                                        parameterList.Add(dataReader.ReadBytes(2));
                                        parameterList.Add(dataReader.ReadBytes(2));
                                        break;
                                    case 0x4:
                                    case 0x5:
                                        parameterList.Add(dataReader.ReadBytes(2));
                                        break;
                                    case 0x6:
                                        break;
                                    case 0x7:
                                        parameterList.Add(dataReader.ReadBytes(2));
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;
                        case 0x1E9: // Number of parameters differ depending on the first parameter value
                            {
                                short parameter1 = dataReader.ReadInt16();
                                parameterList.Add(BitConverter.GetBytes(parameter1));
                                switch (parameter1) {
                                    case 0x0:
                                        break;
                                    case 0x1:
                                    case 0x2:
                                    case 0x3:
                                        parameterList.Add(dataReader.ReadBytes(2));
                                        break;
                                    case 0x4:
                                        break;
                                    case 0x5:
                                    case 0x6:
                                        parameterList.Add(dataReader.ReadBytes(2));
                                        parameterList.Add(dataReader.ReadBytes(2));
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
                            addParametersToList(ref parameterList, id, dataReader);
                            break;
                    }
                    break;
            }
            return new ScriptCommand(id, parameterList);
        }
        private void addParametersToList(ref List<byte[]> parameterList, ushort id, BinaryReader dataReader) {
            Console.WriteLine("Loaded command id: " + id.ToString("X4"));
            try {
                foreach (int bytesToRead in RomInfo.scriptParametersDict[id])
                    parameterList.Add(dataReader.ReadBytes(bytesToRead));
            } catch (NullReferenceException) {
                MessageBox.Show("Script command " + id + "can't be handled for now." +
                    Environment.NewLine + "Reference offset 0x" + dataReader.BaseStream.Position.ToString("X"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                parameterList = null;
                return;
            } catch {
                MessageBox.Show("Error: ID Read - " + id +
                    Environment.NewLine + "Reference offset 0x" + dataReader.BaseStream.Position.ToString("X"), "Unrecognized script command", MessageBoxButtons.OK, MessageBoxIcon.Error);
                parameterList = null;
                return;
            }
        }
        public byte[] ToByteArray() {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData)) {
                List<uint> scriptOffsets = new List<uint>();
                List<uint> functionOffsets = new List<uint>();
                List<uint> movementOffsets = new List<uint>();

                List<Tuple<int, int, bool>> references = new List<Tuple<int, int, bool>>(); // Format: [address, function/movement #, isApplyMovement]
                int[] referenceCodes = new int[] { 0x16, 0x1A, 0x1C, 0x1D, 0x5E };

                /* Allocate enough space for script pointers, which we do not know yet */
                writer.BaseStream.Position += scripts.Count * 0x4;
                writer.Write((ushort)0xFD13); // Signal the end of header section

                /* Write scripts */
                for (int i = 0; i < scripts.Count; i++) {
                    if (scripts[i].useScript == -1) {
                        scriptOffsets.Add((uint)writer.BaseStream.Position);

                        for (int j = 0; j < scripts[i].commands.Count; j++) {
                            ushort commandID = scripts[i].commands[j].id;
                            writer.Write(commandID);

                            /* Get command parameters */
                            List<byte[]> parameterList = scripts[i].commands[j].parameterList;
                            for (int k = 0; k < parameterList.Count; k++)
                                writer.Write(parameterList[k]);

                            Console.Write("\nCommand added: " + scripts[i].commands[j]);

                            /* If command calls a function/movement, store reference position */
                            if (referenceCodes.Contains(commandID)) {
                                int positionOfJumpAddress;
                                if (commandID == 0x16 || commandID == 0x1A)
                                    positionOfJumpAddress = 0; // Jump, Call
                                else
                                    positionOfJumpAddress = 1;

                                references.Add(new Tuple<int, int, bool>((int)(writer.BaseStream.Position - 4), BitConverter.ToInt32(parameterList[positionOfJumpAddress], 0)-1, commandID == 0x5E));
                            }
                        }
                    } else {
                        scriptOffsets.Add(scriptOffsets[scripts[i].useScript]);  // If script has UseScript, copy offset
                    }
                }

                /* Write functions */
                for (int i = 0; i < functions.Count; i++) {
                    functionOffsets.Add((uint)writer.BaseStream.Position);

                    for (int j = 0; j < functions[i].commands.Count; j++) {
                        ushort commandID = functions[i].commands[j].id;
                        writer.Write(commandID);

                        /* Write command parameters */
                        List<byte[]> parameterList = functions[i].commands[j].parameterList;
                        for (int k = 0; k < parameterList.Count; k++) 
                            writer.Write(parameterList[k]);

                        /* If command calls a function/movement, store reference position */
                        if (referenceCodes.Contains(commandID)) {
                            int index;
                            if (commandID == 0x16 || commandID == 0x1A) 
                                index = 0;
                            else 
                                index = 1;

                            references.Add(new Tuple<int, int, bool>((int)(writer.BaseStream.Position - 4), BitConverter.ToInt32(parameterList[index], 0)-1, commandID == 0x5E));
                        }
                    }
                }

                // Movements must be halfword-aligned
                if (writer.BaseStream.Position % 2 == 1) { //Check if the writer's head is on an odd byte
                    writer.Write((byte)0); //Add padding
                }

                /* Write movements */
                for (int i = 0; i < movements.Count; i++) {
                    movementOffsets.Add((uint)writer.BaseStream.Position);

                    for (int j = 0; j < movements[i].commands.Count; j++) {
                        /* Write movement command id */
                        ushort id = movements[i].commands[j].id;
                        writer.Write(id);

                        /* Write movement command parameters */
                        List<byte[]> parameterLists = movements[i].commands[j].parameterList;
                        for (int k = 0; k < parameterLists.Count; k++) 
                            writer.Write(parameterLists[k]);
                    }
                }

                /* Write script offsets to header */
                writer.BaseStream.Position = 0x0;
                for (int i = 0; i < scriptOffsets.Count; i++) 
                    writer.Write(scriptOffsets[i] - (uint)writer.BaseStream.Position - 0x4);

                /* Fix references to functions and movements */
                for (int i = 0; i < references.Count; i++) {
                    writer.BaseStream.Position = references[i].Item1;

                    if (references[i].Item3 == true) { //isApplyMovement 
                        try {
                            writer.Write((UInt32)(movementOffsets[references[i].Item2] - references[i].Item1 - 4));
                        } catch (ArgumentOutOfRangeException) {
                            MessageBox.Show("Movement #" + (1+references[i].Item2) + " undeclared.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    } else {
                        try {
                            writer.Write((UInt32)(functionOffsets[references[i].Item2] - references[i].Item1 - 4));
                        } catch ( ArgumentOutOfRangeException) {
                            MessageBox.Show("Function #" + (1+references[i].Item2) + " undeclared.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }

            return newData.ToArray();
        }
        private void SaveToFile(string path) {
            using (BinaryWriter writer = new BinaryWriter(new FileStream(path, FileMode.Create)))
                writer.Write(this.ToByteArray());

            MessageBox.Show(GetType().Name + " saved successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public void SaveToFileDefaultDir(int IDtoReplace) {
            string path = RomInfo.scriptDirPath + "\\" + IDtoReplace.ToString("D4");
            this.SaveToFile(path);  
        }
        internal void SaveToFileExplorePath(string suggestedFileName) {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "Gen IV Script File (*.scr)|*.scr";

            if (!string.IsNullOrEmpty(suggestedFileName))
                sf.FileName = suggestedFileName;
            if (sf.ShowDialog() != DialogResult.OK)
                return;

            this.SaveToFile(sf.FileName);
        }
        #endregion
    }    
}