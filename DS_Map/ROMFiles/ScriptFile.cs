using DSPRE.Resources;
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
        public List<CommandContainer> allScripts = new List<CommandContainer>();
        public List<CommandContainer> allFunctions = new List<CommandContainer>();
        public List<ActionContainer> allActions = new List<ActionContainer>();
        int fileID = -1;
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
                for (int current = 0; current < scriptOffsets.Count; current++) {
                    int index = scriptOffsets.FindIndex(x => x == scriptOffsets[current]); // Check for UseScript

                    if (index == current) {
                        scrReader.BaseStream.Position = scriptOffsets[current];

                        List<ScriptCommand> cmdList = new List<ScriptCommand>();
                        bool endScript = new bool();
                        while (!endScript) {
                            ScriptCommand cmd = ReadCommand(scrReader, ref functionOffsets, ref movementOffsets);
                            if (cmd.commandParameters == null) 
                                return;
                            
                            cmdList.Add(cmd);

                            if (endCodes.Contains(cmd.id))
                                endScript = true;
                            
                        }
                        allScripts.Add(new CommandContainer(current, commandList: cmdList));
                    } else {
                        allScripts.Add(new CommandContainer(current, useScript: index+1));
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
                            if (command.commandParameters == null)
                                return;

                            cmdList.Add(command);
                            if (endCodes.Contains(command.id))
                                endFunction = true;
                        }
                        allFunctions.Add(new CommandContainer(i, commandList: cmdList));
                    } else {
                        allFunctions.Add(new CommandContainer(i, useScript: posInList+1));
                    }
                }

                /* Read movements */
                for (int i = 0; i < movementOffsets.Count; i++) {
                    scrReader.BaseStream.Position = movementOffsets[i];

                    List<ScriptAction> cmdList = new List<ScriptAction>();
                    bool endMovement = new bool();
                    while (!endMovement) {
                        ushort id = scrReader.ReadUInt16();
                        if (id == 0xFE) {
                            endMovement = true;
                            cmdList.Add(new ScriptAction(id));
                        } else {
                            cmdList.Add(new ScriptAction(id, scrReader.ReadUInt16()));
                        }
                    }
                    allActions.Add(new ActionContainer(i, actionList: cmdList));
                }
            }
        }
        public ScriptFile(int fileID) : this(new FileStream(RomInfo.scriptDirPath +
        "\\" + fileID.ToString("D4"), FileMode.Open)) {
            this.fileID = fileID;
        }
        public ScriptFile(List<CommandContainer> scripts, List<CommandContainer> functions, List<ActionContainer> movements) {
            allScripts = scripts;
            allFunctions = functions;
            allActions = movements;
            isLevelScript = false;
        }
        public ScriptFile(string[] scriptLines, string[] functionLines, string[] actionLines, int ID = -1) {
            allScripts = readCommandsFromLines(scriptLines,
                (source, x) => x < source.Length - 1
                            && !source[x].Equals(RomInfo.scriptCommandNamesDict[0x0002])        //End
                            && !source[x].Contains(RomInfo.scriptCommandNamesDict[0x0016] + " Function"));  //Jump
            if (allScripts == null)
                return;

            allFunctions = readCommandsFromLines(functionLines,
                (source, x) => !source[x].Equals(RomInfo.scriptCommandNamesDict[0x0002])        //End
                            && !source[x].Contains(RomInfo.scriptCommandNamesDict[0x001B])      //Return
                            && !source[x].Contains(RomInfo.scriptCommandNamesDict[0x0016] + " Function"));  //Jump
            if (allFunctions == null)
                return;

            allActions = readActionsFromLines(actionLines);
            if (allActions == null)
                return;

            this.fileID = ID;
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
                foreach (int bytesToRead in RomInfo.commandParametersDict[id])
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
                List<(uint offset, int ID)> scriptOffsets = new List<(uint, int)>(); //uint OFFSET, int Function/Script/Action ID
                List<(uint offset, int ID)> functionOffsets = new List<(uint, int)>();
                List<(uint offset, int ID)> actionOffsets = new List<(uint, int)>();

                List<(int address, int destID, bool isMovement, int manualUserID)> references = new List<(int, int, bool, int)>(); 

                /* Allocate enough space for script pointers, which we do not know yet */
                try {
                    writer.BaseStream.Position += allScripts.Count * 0x4;
                    writer.Write((ushort)0xFD13); // Signal the end of header section

                    /* Write scripts */
                    for (int i = 0; i < allScripts.Count; i++) {
                        if (allScripts[i].useScript == -1) {
                            scriptOffsets.Add(((uint)writer.BaseStream.Position, i));

                            for (int j = 0; j < allScripts[i].commands.Count; j++) {
                                ushort commandID = allScripts[i].commands[j].id;
                                writer.Write(commandID);
                                //System.Diagnostics.Debug.Write(BitConverter.ToString(BitConverter.GetBytes(commandID)) + " ");

                                /* Get command parameters */
                                List<byte[]> parameterList = allScripts[i].commands[j].commandParameters;
                                for (int k = 0; k < parameterList.Count; k++) {
                                    writer.Write(parameterList[k]);
                                    //System.Diagnostics.Debug.WriteLine(BitConverter.ToString(parameterList[k]) + " ");
                                }

                                /* If command calls a function/movement, store reference position */
                                AddReference(ref references, commandID, parameterList, (int)writer.BaseStream.Position, i);
                            }
                        } else {
                            scriptOffsets.Add(scriptOffsets[allScripts[i].useScript - 1]);  // If script has UseScript, copy offset
                        }
                    }

                    /* Write functions */
                    for (int i = 0; i < allFunctions.Count; i++) {
                        if (allFunctions[i].useScript == -1) {
                            functionOffsets.Add(((uint)writer.BaseStream.Position, allFunctions[i].manualUserID));

                            for (int j = 0; j < allFunctions[i].commands.Count; j++) {
                                ushort commandID = allFunctions[i].commands[j].id;
                                writer.Write(commandID);
                                //System.Diagnostics.Debug.Write(BitConverter.ToString(BitConverter.GetBytes(commandID)) + " ");

                                /* Write command parameters */
                                List<byte[]> parameterList = allFunctions[i].commands[j].commandParameters;
                                for (int k = 0; k < parameterList.Count; k++) {
                                    writer.Write(parameterList[k]);
                                    //System.Diagnostics.Debug.Write(BitConverter.ToString(parameterList[k]) + " ");
                                }

                                /* If command calls a function/movement, store reference position */
                                AddReference(ref references, commandID, parameterList, (int)writer.BaseStream.Position, i);
                            }
                        } else {
                            functionOffsets.Add((scriptOffsets[allFunctions[i].useScript - 1].offset, allFunctions[i].manualUserID));
                        }
                    }

                    // Movements must be halfword-aligned
                    if (writer.BaseStream.Position % 2 == 1) { //Check if the writer's head is on an odd byte
                        writer.Write((byte)0x00); //Add padding
                    }

                    /* Write movements */
                    for (int i = 0; i < allActions.Count; i++) {
                        actionOffsets.Add(((uint)writer.BaseStream.Position, allActions[i].manualUserID));

                        for (int j = 0; j < allActions[i].actions.Count; j++) {
                            /* Write movement command id */
                            writer.Write(allActions[i].actions[j].id);

                            /* Write movement command parameters */
                            if (allActions[i].actions[j].id != 0x00FE)
                                writer.Write(allActions[i].actions[j].repetitionCount);
                        }
                    }

                    /* Write script offsets to header */
                    writer.BaseStream.Position = 0x0;
                    for (int i = 0; i < scriptOffsets.Count; i++) 
                        writer.Write(scriptOffsets[i].Item1 - (uint)writer.BaseStream.Position - 0x4);

                    /* Fix references to functions and movements */
                    List<int> undeclaredFuncs = new List<int>();
                    List<int> undeclaredActions = new List<int>();

                    List<int> uninvokedFuncs = new List<int>(allFunctions.Select( x => x.manualUserID).ToArray());
                    List<int> unreferencedActions = new List<int>(allActions.Select(x => x.manualUserID).ToArray());

                    while (references.Count > 0) {
                        writer.BaseStream.Position = references[0].address; //place seek head on parameter that is supposed to store the jump address

                        if (references[0].isMovement) { //isApplyMovement 
                            (uint offset, int id) result = actionOffsets.Find(x => x.ID == references[0].destID);
                            
                            if (result == (0, 0))
                                undeclaredActions.Add(references[0].destID);
                            else { 
                                writer.Write((uint)(result.offset - references[0].address - 4)); ////////////////BROKEN
                                unreferencedActions.Remove(references[0].destID);                                
                            }
                        } else {
                            (uint offset, int id) result = functionOffsets.Find(x => x.ID == references[0].destID);
                            
                            if (result == (0, 0))
                                undeclaredFuncs.Add(references[0].destID);
                            else { 
                                writer.Write((uint)(result.offset - references[0].address - 4));
                                uninvokedFuncs.Remove(references[0].destID);
                            }
                        }
                        references.RemoveAt(0);
                    }

                    string errorMsg = "";
                    if (undeclaredFuncs.Count > 0) {
                        string[] errorFunctionsUndeclared = undeclaredFuncs.ToArray().Select( x => x.ToString() ).ToArray();
                        errorMsg += "These Functions have been invoked but not declared: " + Environment.NewLine + string.Join(",", errorFunctionsUndeclared);
                        errorMsg += Environment.NewLine;
                    }
                    if (undeclaredActions.Count > 0) {
                        string[] errorActionsUndeclared = undeclaredActions.ToArray().Select( x => x.ToString() ).ToArray();
                        errorMsg += "These Actions have been referenced but not declared: " + Environment.NewLine + string.Join(",", errorActionsUndeclared);
                        errorMsg += Environment.NewLine;
                    }
                    if (!string.IsNullOrEmpty(errorMsg)) {
                        MessageBox.Show(errorMsg + Environment.NewLine + "This Script File has not been overwritten since it can not be saved.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        errorMsg = "";
                        return null;
                    }
                    
                    if (uninvokedFuncs.Count > 0) {
                        string[] orphanedFunctions = uninvokedFuncs.ToArray().Select(x => x.ToString()).ToArray();
                        errorMsg += "Unused Function IDs detected: " + Environment.NewLine + string.Join(",", orphanedFunctions);
                        errorMsg += Environment.NewLine;
                    }
                    if (unreferencedActions.Count > 0) {
                        string[] orphanedActions = unreferencedActions.ToArray().Select(x => x.ToString()).ToArray();
                        errorMsg += "Unused Action IDs detected: " + Environment.NewLine + string.Join(",", orphanedActions);
                        errorMsg += Environment.NewLine;
                    }
                    if (!string.IsNullOrEmpty(errorMsg)) {
                        MessageBox.Show(errorMsg + Environment.NewLine + "Remember that every unused Function or Action is always lost upon reloading the Script File.", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        errorMsg = "";
                    }
                } catch (NullReferenceException nre) {
                    Console.WriteLine(nre);
                    return null;
                }
            }

            return newData.ToArray();
            
        }
        private void AddReference(ref List<(int, int, bool, int)> references, ushort commandID, List<byte[]> parameterList, int pos, int callerID) {
            try {
                if (Resources.PokeDatabase.ScriptEditor.commandsWithRelativeJump[commandID] == true) {
                    byte[] parameterWithReferenceID;
                    if (commandID == 0x16 || commandID == 0x1A)
                        parameterWithReferenceID = parameterList[0]; // Jump, Call
                    else
                        parameterWithReferenceID = parameterList[1];

                    int referenceID = BitConverter.ToInt32(parameterWithReferenceID, 0);
                    references.Add((pos - 4, referenceID, commandID == 0x5E, callerID));
                }
            } catch (KeyNotFoundException) { }
        }
        private void SaveToFile(string path) {
            byte[] thisScript = ToByteArray();
            if (thisScript == null) {
                Console.WriteLine(GetType().Name + " couldn't be saved!");
                return;
            }

            using (BinaryWriter writer = new BinaryWriter(new FileStream(path, FileMode.Create))) {
                writer.Write(thisScript);
            }

            MessageBox.Show(GetType().Name + " saved successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public void SaveToFileDefaultDir(int IDtoReplace) {
            string path = RomInfo.scriptDirPath + "\\" + IDtoReplace.ToString("D4");
            this.SaveToFile(path);  
        }
        public void SaveToFileExplorePath(string suggestedFileName, bool blindmode) {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "Gen IV Script File (*.scr)|*.scr";

            if (!string.IsNullOrEmpty(suggestedFileName))
                sf.FileName = suggestedFileName;
            if (sf.ShowDialog() != DialogResult.OK)
                return;

            if (blindmode) {
                File.Copy(RomInfo.scriptDirPath + "\\" + fileID.ToString("D4"), sf.FileName, overwrite: true);
                
                string msg = "";
                if (!isLevelScript)
                    msg += "The last saved version of this ";
                MessageBox.Show(msg + GetType().Name + " has been exported successfully.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } else {
                this.SaveToFile(sf.FileName);
            }
        }
        private List<CommandContainer> readCommandsFromLines(string[] lineSource, Func<string[], int, bool> endConditions) {
            List<CommandContainer> ls = new List<CommandContainer>();

            for (int i = 0; i < lineSource.Length; i++) {
                if (lineSource[i].Contains('@')) { // Move on until script header is found
                    int positionOfScriptNumber = lineSource[i].IndexOf('#');
                    int scriptNumber = Int32.Parse(lineSource[i].Substring(positionOfScriptNumber + 1).Split()[0].Replace("-", ""));

                    i++;
                    while (lineSource[i].Length <= 0)
                        i++; //Skip all empty lines 

                    if (lineSource[i].Contains("UseScript")) {
                        int useScriptNumber = Int16.Parse(lineSource[i].Substring(1 + lineSource[i].IndexOf('#')));
                        ls.Add(new CommandContainer(scriptNumber, useScriptNumber));
                    } else {

                        /* Read script commands */
                        List<ScriptCommand> cmdList = new List<ScriptCommand>();
                        while (endConditions(lineSource, i)) {
                            ScriptCommand toAdd = new ScriptCommand(lineSource[i], i+1);
                            if (toAdd.id == UInt16.MaxValue)
                                return null;

                            cmdList.Add(toAdd);
                            i++;
                        }
                        cmdList.Add(new ScriptCommand(lineSource[i], i+1)); // Add end or jump/call command
                        ls.Add(new CommandContainer(scriptNumber, commandList: cmdList));
                    }
                }
            }
            return ls;
        }
        private List<ActionContainer> readActionsFromLines(string[] lineSource) {
            List<ActionContainer> ls = new List<ActionContainer>();

            for (int i = 0; i < lineSource.Length; i++) {

                if (lineSource[i].Contains('@')) {  // Move on until script header is found
                    int positionOfActionNumber = lineSource[i].IndexOf('#');
                    int actionNumber = Int32.Parse(lineSource[i].Substring(positionOfActionNumber + 1).Split()[0].Replace("-", ""));

                    i++;
                    while (lineSource[i].Length <= 0)
                        i++; //Skip all empty lines 

                    List<ScriptAction> cmdList = new List<ScriptAction>();
                    /* Read script commands */
                    while (!lineSource[i].Equals(PokeDatabase.ScriptEditor.movementsDictIDName[0x00FE])) { //End
                        ScriptAction toAdd = new ScriptAction(lineSource[i], i+1);
                        if (toAdd.id == UInt16.MaxValue)
                            return null;

                        cmdList.Add(toAdd);
                        i++;
                    }
                    cmdList.Add(new ScriptAction(lineSource[i], i+1)); // Add end command

                    ls.Add(new ActionContainer(actionNumber, actionList: cmdList));
                }
            }
            return ls;
        }

        public static string OverworldFlexDecode(ushort flexID) {
            if (flexID > 255) {
                return " " + "0x" + flexID.ToString("X4");
            } else {
                switch (flexID) {
                    case 255:
                        return " " + "Player";
                    case 253:
                        return " " + "Following";
                    case 241:
                        return " " + "Cam";
                    default:
                        return " " + "Overworld_#" + flexID.ToString("D");
                }
            }
        }
        #endregion
    }    
}