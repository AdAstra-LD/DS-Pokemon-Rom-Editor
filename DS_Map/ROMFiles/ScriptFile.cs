using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DSPRE.ROMFiles {
    /// <summary>
    /// Class to store script file data in Pokémon NDS games
    /// </summary>
    public class ScriptFile {
        #region Fields (3)
        public List<Script> scripts = new List<Script>();
        public List<Script> functions = new List<Script>();
        public List<Script> movements = new List<Script>();
        public bool isLevelScript = new bool();

        private readonly bool debug = true;
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
                for (int current = 0; current < scriptOffsets.Count; current++) {
                    int index = scriptOffsets.FindIndex(x => x == scriptOffsets[current]); // Check for UseScript

                    if (index == current) {
                        scrReader.BaseStream.Position = scriptOffsets[current];

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
                        scripts.Add(new Script(useScript: index+1));
                    }
                }

                /* Read functions */
                for (int i = 0; i < functionOffsets.Count; i++) {
                    scrReader.BaseStream.Position = functionOffsets[i];
                    int posInList = scriptOffsets.IndexOf(functionOffsets[i]); // Check for UseScript_#

                    if (posInList == -1) {
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
                        functions.Add(new Script(commandList: cmdList));
                    } else {
                        functions.Add(new Script(useScript: posInList+1));
                    }
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

                List<(int, int, bool)> references = new List<(int, int, bool)>(); // Format: [address, function/movement #, isApplyMovement]

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
                            System.Diagnostics.Debug.Write(BitConverter.ToString(BitConverter.GetBytes(commandID)) + " ");

                            /* Get command parameters */
                            List<byte[]> parameterList = scripts[i].commands[j].parameterList;
                            for (int k = 0; k < parameterList.Count; k++) {
                                writer.Write(parameterList[k]);
                                System.Diagnostics.Debug.WriteLine(BitConverter.ToString(parameterList[k]) + " ");
                            }

                            /* If command calls a function/movement, store reference position */
                            AddReference(ref references, commandID, parameterList, (int)writer.BaseStream.Position);
                        }
                    } else {
                        scriptOffsets.Add(scriptOffsets[scripts[i].useScript - 1]);  // If script has UseScript, copy offset
                    }
                }

                /* Write functions */
                for (int i = 0; i < functions.Count; i++) {
                    if (functions[i].useScript == -1) {
                        functionOffsets.Add((uint)writer.BaseStream.Position);

                        for (int j = 0; j < functions[i].commands.Count; j++) {
                            ushort commandID = functions[i].commands[j].id;
                            writer.Write(commandID);
                            System.Diagnostics.Debug.Write(BitConverter.ToString(BitConverter.GetBytes(commandID)) + " ");

                            /* Write command parameters */
                            List<byte[]> parameterList = functions[i].commands[j].parameterList;
                            for (int k = 0; k < parameterList.Count; k++) {
                                writer.Write(parameterList[k]);
                                System.Diagnostics.Debug.Write(BitConverter.ToString(parameterList[k]) + " ");
                            }

                            /* If command calls a function/movement, store reference position */
                            AddReference(ref references, commandID, parameterList, (int)writer.BaseStream.Position);
                        }
                    } else {
                        functionOffsets.Add(scriptOffsets[functions[i].useScript - 1]);
                    }
                }

                // Movements must be halfword-aligned
                if (writer.BaseStream.Position % 2 == 1) { //Check if the writer's head is on an odd byte
                    writer.Write((byte)0x00); //Add padding
                }

                /* Write movements */
                for (int i = 0; i < movements.Count; i++) {
                    movementOffsets.Add((uint)writer.BaseStream.Position);

                    for (int j = 0; j < movements[i].commands.Count; j++) {
                        /* Write movement command id */
                        writer.Write(movements[i].commands[j].id);

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
                List<int> undeclaredFuncs = new List<int>();
                List<int> undeclaredMovs = new List<int>();
                for (int i = 0; i < references.Count; i++) {
                    writer.BaseStream.Position = references[i].Item1; //go to parameter that must store the jump address

                    if (references[i].Item3 == true) { //isApplyMovement 
                        try {
                            writer.Write((uint)(movementOffsets[references[i].Item2 - 1] - references[i].Item1 - 4));
                        } catch (ArgumentOutOfRangeException) {
                            undeclaredMovs.Add(1 + references[i].Item2);
                        }
                    } else {
                        try {
                            writer.Write((uint)(functionOffsets[references[i].Item2 - 1] - references[i].Item1 - 4)); //
                        } catch ( ArgumentOutOfRangeException) {
                            undeclaredFuncs.Add(1 + references[i].Item2);
                        }
                    }
                }
                
                if (undeclaredFuncs.Count > 0) {
                    string[] result = undeclaredFuncs.ToArray().Select( x => x.ToString() ).ToArray();
                    string funcs = string.Join(", ", result);
                    MessageBox.Show("You must declare the function(s) below:" + Environment.NewLine + funcs, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                if (undeclaredMovs.Count > 0) {
                    string[] result = undeclaredMovs.ToArray().Select(x => x.ToString()).ToArray();
                    string movs = string.Join(",", result);
                    MessageBox.Show("You must declare the movement(s) below:" + Environment.NewLine + movs, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            return newData.ToArray();
        }

        private void AddReference(ref List<(int, int, bool)> references, ushort commandID, List<byte[]> parameterList, int pos) {
            try {
                if (Resources.PokeDatabase.ScriptEditor.commandsWithRelativeJump[commandID] == true) {
                    byte[] parameterWithReferenceID;
                    if (commandID == 0x16 || commandID == 0x1A)
                        parameterWithReferenceID = parameterList[0]; // Jump, Call
                    else
                        parameterWithReferenceID = parameterList[1];

                    int referenceID = BitConverter.ToInt32(parameterWithReferenceID, 0);
                    references.Add((pos - 4, referenceID, commandID == 0x5E));
                }
            } catch (KeyNotFoundException) { }
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