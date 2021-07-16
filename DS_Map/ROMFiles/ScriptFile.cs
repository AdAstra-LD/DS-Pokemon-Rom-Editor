using DSPRE.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static DSPRE.ROMFiles.ScriptFile;
using static DSPRE.RomInfo;

namespace DSPRE.ROMFiles {
    /// <summary>
    /// Class to store script file data in Pokémon NDS games
    /// </summary>
    public class ScriptFile: RomFile {
        #region Constants
        //this enum doesn't really make much sense now but it will, once scripts can be called and jumped to
        public enum containerTypes { FUNCTION, MOVEMENT, SCRIPT };
        #endregion
        #region Fields (3)
        public List<CommandContainer> allScripts = new List<CommandContainer>();
        public List<CommandContainer> allFunctions = new List<CommandContainer>();
        public List<ActionContainer> allActions = new List<ActionContainer>();
        public int? fileID = null;
        public bool isLevelScript = new bool();
        #endregion

        #region Constructors (1)

        public ScriptFile(Stream fs) {
            List<int> scriptOffsets = new List<int>();
            List<int> functionOffsets = new List<int>();
            List<int> movementOffsets = new List<int>();

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
                            int offsetFromStart = (int)(value + scrReader.BaseStream.Position);
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
                for (uint current = 0; current < scriptOffsets.Count; current++) {
                    int index = scriptOffsets.FindIndex(x => x == scriptOffsets[(int)current]); // Check for UseScript

                    if (index == current) {
                        scrReader.BaseStream.Position = scriptOffsets[(int)current];

                        List<ScriptCommand> cmdList = new List<ScriptCommand>();
                        bool endScript = new bool();
                        while (!endScript) {
                            ScriptCommand cmd = ReadCommand(scrReader, ref functionOffsets, ref movementOffsets);
                            if (cmd.cmdParams is null) 
                                return;
                            
                            cmdList.Add(cmd);

                            if (PokeDatabase.ScriptEditor.endCodes.Contains((ushort)cmd.id))
                                endScript = true;
                            
                        }
                        allScripts.Add(new CommandContainer(current+1, containerTypes.SCRIPT, commandList: cmdList));
                    } else {
                        allScripts.Add(new CommandContainer(current+1, containerTypes.SCRIPT, useScript: index+1));
                    }
                }

                /* Read functions */
                for (uint current = 0; current < functionOffsets.Count; current++) {
                    scrReader.BaseStream.Position = functionOffsets[(int)current];
                    int posInList = scriptOffsets.IndexOf(functionOffsets[(int)current]); // Check for UseScript_#

                    if (posInList == -1) {
                        List<ScriptCommand> cmdList = new List<ScriptCommand>();
                        bool endFunction = new bool();
                        while (!endFunction) {
                            ScriptCommand command = ReadCommand(scrReader, ref functionOffsets, ref movementOffsets);
                            if (command.cmdParams is null)
                                return;

                            cmdList.Add(command);
                            if (PokeDatabase.ScriptEditor.endCodes.Contains((ushort)command.id))
                                endFunction = true;
                        }
                        allFunctions.Add(new CommandContainer(current+1, containerTypes.FUNCTION, commandList: cmdList));
                    } else {
                        allFunctions.Add(new CommandContainer(current+1, containerTypes.FUNCTION, useScript: posInList +1));
                    }
                }

                /* Read movements */
                for (uint current = 0; current < movementOffsets.Count; current++) {
                    scrReader.BaseStream.Position = movementOffsets[(int)current];

                    List<ScriptAction> cmdList = new List<ScriptAction>();
                    bool endMovement = new bool();
                    while (!endMovement) {
                        ushort id = scrReader.ReadUInt16();
                        if (id == 0xFE) {
                            endMovement = true;
                            cmdList.Add(new ScriptAction(id, 0));
                        } else {
                            cmdList.Add(new ScriptAction(id, scrReader.ReadUInt16()));
                        }
                    }
                    allActions.Add(new ActionContainer(current+1, actionCommandsList: cmdList));
                }
            }
        }
        public ScriptFile(int fileID) : this(new FileStream(RomInfo.gameDirs[DirNames.scripts].unpackedDir +
        "\\" + fileID.ToString("D4"), FileMode.Open)) {
            this.fileID = fileID;
        }
        public ScriptFile(List<CommandContainer> scripts, List<CommandContainer> functions, List<ActionContainer> movements, int fileID = -1) {
            allScripts = scripts;
            allFunctions = functions;
            allActions = movements;
            isLevelScript = false;
        }
        public ScriptFile(string[] scriptLines, string[] functionLines, string[] actionLines, int fileID = -1) {
            //TODO: give user the possibility to jump to/call a script
            //once it's done, this Predicate below will be the only one needed, since there will be no distinction between
            //a script and a function
            Func<List<string>, int, ushort?, bool> functionEndCondition =
                (source, x, id) => source[x].TrimEnd().Equals(RomInfo.ScriptCommandNamesDict[0x0002], StringComparison.InvariantCultureIgnoreCase)    //End
                            || source[x].IndexOf(RomInfo.ScriptCommandNamesDict[0x0016] + " Function", StringComparison.InvariantCultureIgnoreCase) >= 0 //Jump Function_#
                            || source[x].TrimEnd().Equals(RomInfo.ScriptCommandNamesDict[0x001B], StringComparison.InvariantCultureIgnoreCase)
                            || PokeDatabase.ScriptEditor.endCodes.Contains(id);  //Return

            Func<List<string>, int, ushort?, bool> scriptEndCondition =
                (source, x, id) => source[x].TrimEnd().Equals(RomInfo.ScriptCommandNamesDict[0x0002], StringComparison.InvariantCultureIgnoreCase)    //End
                            || source[x].IndexOf(RomInfo.ScriptCommandNamesDict[0x0016] + " Function") >= 0 //Jump Function_#
                            || PokeDatabase.ScriptEditor.endCodes.Contains(id);

            allScripts = ReadCommandsFromLines(scriptLines, containerTypes.SCRIPT, scriptEndCondition);  //Jump + whitespace
            if (allScripts is null || allScripts.Count <= 0)
                return;

            allFunctions = ReadCommandsFromLines(functionLines, containerTypes.FUNCTION, functionEndCondition);  //Jump + whitespace
            if (allFunctions is null)
                return;

            allActions = ReadActionsFromLines(actionLines);
            if (allActions is null)
                return;

            this.fileID = fileID;
        }
        #endregion

        #region Methods (1)
        private ScriptCommand ReadCommand(BinaryReader dataReader, ref List<int> functionOffsets, ref List<int> actionOffsets) {
            ushort id = dataReader.ReadUInt16();
            List<byte[]> parameterList = new List<byte[]>();

            /* How to read parameters for different commands for DPPt*/
            switch (RomInfo.gameFamily) {
                case "DP":
                case "Plat":
                    switch (id)  {
                        case 0x16: //Jump
                        case 0x1A: //Call 
                            ProcessRelativeJump(dataReader, ref parameterList, ref functionOffsets);
                            break;
                        case 0x17: //JumpIfObjID
                        case 0x18: //JumpIfBgID
                        case 0x19: //JumpIfPlayerDir
                        case 0x1C: //Jump-If
                        case 0x1D: //Call-If
                            //in the case of Jump-If and Call-If, the first param is a comparisonOperator
                            //for JumpIfPlayerDir it's a directionID
                            //for JumpIfObjID, it's an EventID
                            parameterList.Add(new byte[] { dataReader.ReadByte() }); 
                            ProcessRelativeJump(dataReader, ref parameterList, ref functionOffsets);
                            break;
                        case 0x5E: // Movement
                        case 0x2A1: // Movement2
                            //in the case of Movement, the first param is an overworld ID
                            parameterList.Add(BitConverter.GetBytes(dataReader.ReadUInt16())); 
                            ProcessRelativeJump(dataReader, ref parameterList, ref actionOffsets);
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
                case "HGSS":
                    switch (id) {
                        case 0x16: //Jump
                        case 0x1A: //Call 
                            ProcessRelativeJump(dataReader, ref parameterList, ref functionOffsets);
                            break;
                        case 0x17: //JumpIfObjID
                        case 0x18: //JumpIfBgID
                        case 0x19: //JumpIfPlayerDir
                        case 0x1C: //Jump-If
                        case 0x1D: //Call-If
                            parameterList.Add(new byte[] { dataReader.ReadByte() }); //in the case of Jump-If and Call-If, the first param is a comparisonOperator
                            ProcessRelativeJump(dataReader, ref parameterList, ref functionOffsets);
                            break;
                        case 0x5E: // Movement
                            parameterList.Add(BitConverter.GetBytes(dataReader.ReadUInt16())); //in the case of Movement, the first param is an overworld ID
                            ProcessRelativeJump(dataReader, ref parameterList, ref actionOffsets);
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

        private void ProcessRelativeJump(BinaryReader dataReader, ref List<byte[]> parameterList, ref List<int> offsetsList) {
            int relativeOffset = dataReader.ReadInt32();
            int offsetFromScriptFileStart = (int)(relativeOffset + dataReader.BaseStream.Position);

            if (!offsetsList.Contains(offsetFromScriptFileStart))
                offsetsList.Add(offsetFromScriptFileStart);

            int functionNumber = offsetsList.IndexOf(offsetFromScriptFileStart);
            if (functionNumber < 0)
                throw new InvalidOperationException();

            parameterList.Add(BitConverter.GetBytes(functionNumber + 1));
        }

        private void addParametersToList(ref List<byte[]> parameterList, ushort id, BinaryReader dataReader) {
            Console.WriteLine("Loaded command id: " + id.ToString("X4"));
            try {
                foreach (int bytesToRead in RomInfo.CommandParametersDict[id])
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
        private void AddReference(ref List<ScriptReference> references, ushort commandID, List<byte[]> parameterList, int pos, CommandContainer cont) {
            int parameterWithRelativeJump;
            if (!PokeDatabase.ScriptEditor.commandsWithRelativeJump.TryGetValue(commandID, out parameterWithRelativeJump))
                return;

            uint invokedID = BitConverter.ToUInt32(parameterList[parameterWithRelativeJump], 0);  // Jump, Call

            if (commandID == 0x005E)
                references.Add(new ScriptReference(cont.containerType, cont.manualUserID, containerTypes.MOVEMENT, invokedID, pos - 4));
            else {
                references.Add(new ScriptReference(cont.containerType, cont.manualUserID, containerTypes.FUNCTION, invokedID, pos - 4));
            }
        }
        private List<CommandContainer> ReadCommandsFromLines(string[] lineSource, containerTypes containerType, Func<List<string>, int, ushort?, bool> endConditions) {
            List<CommandContainer> ls = new List<CommandContainer>();

            List<string> lineSourceL = RemoveEmptyLines(lineSource);
            try {
                int i = 0;
                while (i < lineSourceL.Count) {
                    int positionOfScriptNumber;
                    if (lineSourceL[i].IndexOf(':') < 0 || (positionOfScriptNumber = lineSourceL[i].IndexOfAny("0123456789".ToCharArray())) < 0) { //Script #1
                        continue;
                    }
                    uint scriptNumber = uint.Parse(lineSourceL[i++].Substring(positionOfScriptNumber).Split()[0].Replace(":", ""));

                    if (lineSourceL[i].IndexOf("UseScript", StringComparison.InvariantCultureIgnoreCase) >= 0) {
                        int useScriptNumber = Int16.Parse(lineSourceL[i].Substring(1 + lineSourceL[i].IndexOf('#')));
                        ls.Add(new CommandContainer(scriptNumber, containerType, useScriptNumber));
                    } else {

                        /* Read script commands */
                        List<ScriptCommand> cmdList = new List<ScriptCommand>();
                        ScriptCommand lastRead;
                        do {
                            lastRead = new ScriptCommand(lineSourceL[i], i + 1);
                            if (lastRead.id is null)
                                return null;

                            cmdList.Add(lastRead);
                        } while (!endConditions(lineSourceL, i++, lastRead.id));

                        ls.Add(new CommandContainer(scriptNumber, containerType, commandList: cmdList));
                    }
                }
            } catch (IndexOutOfRangeException) { }
            return ls;
        }

        private List<ActionContainer> ReadActionsFromLines(string[] lineSource) {
            List<ActionContainer> ls = new List<ActionContainer>();

            List<string> lineSourceL = RemoveEmptyLines(lineSource);
            try {
                int i = 0;
                while (i < lineSourceL.Count) {
                    int positionOfActionNumber;
                    if (lineSourceL[i].IndexOf(':') < 0 || (positionOfActionNumber = lineSourceL[i].IndexOfAny("0123456789".ToCharArray())) < 0) { // Move on until script header is found
                        continue;
                    }
                    uint actionNumber = uint.Parse(lineSourceL[i++].Substring(positionOfActionNumber).Split()[0].Replace(":", ""));

                    List<ScriptAction> cmdList = new List<ScriptAction>();
                    /* Read script actions */
                    do {
                        ScriptAction toAdd = new ScriptAction(lineSourceL[i], i + 1);
                        if (toAdd.id is null)
                            return null;

                        cmdList.Add(toAdd);
                    } while (!lineSourceL[i++].Equals(PokeDatabase.ScriptEditor.movementsDictIDName[0x00FE], StringComparison.InvariantCultureIgnoreCase));

                    ls.Add(new ActionContainer(actionNumber, actionCommandsList: cmdList));
                }
            } catch (IndexOutOfRangeException) { }
            return ls;
        }
        private List<string> RemoveEmptyLines(string[] lineSource) {
            List<string> result = new List<string>();
            foreach (string x in lineSource) {
                string tr = x.Trim();
                if (tr.Length > 0) {
                    result.Add(tr);
                }
            }
            return result;
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
                        return " " + "Camera";
                    default:
                        return " " + "Overworld_#" + flexID.ToString("D");
                }
            }
        }

        #region Output
        public override byte[] ToByteArray() {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData)) {
                List<(uint scriptID, uint offsetInFile)> scriptOffsets = new List<(uint, uint)>(); //uint OFFSET, int Function/Script/Action ID
                List<(uint functionID, uint offsetInFile)> functionOffsets = new List<(uint, uint)>();
                List<(uint actionID, uint offsetInFile)> actionOffsets = new List<(uint, uint)>();

                List<ScriptReference> refList = new List<ScriptReference>();

                /* Allocate enough space for script pointers, which we do not know yet */
                try {
                    writer.BaseStream.Position += allScripts.Count * 0x4;
                    writer.Write((ushort)0xFD13); // Signal the end of header section

                    /* Write scripts */
                    foreach (CommandContainer currentScript in allScripts) {
                        if (currentScript.useScript == -1) {
                            scriptOffsets.Add((currentScript.manualUserID, (uint)writer.BaseStream.Position));

                            foreach (ScriptCommand currentCmd in currentScript.commands) {
                                writer.Write((ushort)currentCmd.id);
                                //System.Diagnostics.Debug.Write(BitConverter.ToString(BitConverter.GetBytes(commandID)) + " ");

                                List<byte[]> parameterList = currentCmd.cmdParams;
                                foreach (byte[] b in parameterList) {
                                    writer.Write(b);
                                    //System.Diagnostics.Debug.WriteLine(BitConverter.ToString(parameterList[k]) + " ");
                                }

                                /* If command calls a function/movement, store reference position */
                                AddReference(ref refList, (ushort)currentCmd.id, parameterList, (int)writer.BaseStream.Position, currentScript);
                            }
                        } else {
                            scriptOffsets.Add((currentScript.manualUserID, scriptOffsets[currentScript.useScript - 1].offsetInFile));  // If script has UseScript, copy offset
                        }
                    }

                    /* Write functions */
                    foreach (CommandContainer currentFunction in allFunctions) {
                        if (currentFunction.useScript == -1) {
                            functionOffsets.Add((currentFunction.manualUserID, (uint)writer.BaseStream.Position));

                            foreach (ScriptCommand currentCmd in currentFunction.commands) {
                                writer.Write((ushort)currentCmd.id);
                                //System.Diagnostics.Debug.Write(BitConverter.ToString(BitConverter.GetBytes(commandID)) + " ");

                                List<byte[]> parameterList = currentCmd.cmdParams;
                                foreach (byte[] b in parameterList) {
                                    writer.Write(b);
                                    //System.Diagnostics.Debug.Write(BitConverter.ToString(parameterList[k]) + " ");
                                }

                                /* If command calls a function/movement, store reference position */
                                
                                AddReference(ref refList, (ushort)currentCmd.id, parameterList, (int)writer.BaseStream.Position, currentFunction);
                            }
                        } else {
                            functionOffsets.Add((currentFunction.manualUserID, scriptOffsets[currentFunction.useScript - 1].offsetInFile));
                        }
                    }

                    // Movements must be halfword-aligned
                    if (writer.BaseStream.Position % 2 == 1) { //Check if the writer's head is on an odd byte
                        writer.Write((byte)0x00); //Add padding
                    }

                    /* Write movements */
                    foreach (ActionContainer currentAction in allActions) {
                        actionOffsets.Add((currentAction.manualUserID, (uint)writer.BaseStream.Position));

                        foreach (ScriptAction currentCmd in currentAction.actionCommandsList) {
                            writer.Write((ushort)currentCmd.id);
                            writer.Write((ushort)currentCmd.repetitionCount);
                        }
                    }

                    /* Write script offsets to header */
                    writer.BaseStream.Position = 0x0;
                    for (int i = 0; i < scriptOffsets.Count; i++)
                        writer.Write(scriptOffsets[i].offsetInFile - (uint)writer.BaseStream.Position - 0x4);


                    SortedSet<uint> undeclaredFuncs = new SortedSet<uint>();
                    SortedSet<uint> undeclaredActions = new SortedSet<uint>();

                    SortedSet<uint> uninvokedFuncs = new SortedSet<uint>(allFunctions.Select(x => x.manualUserID).ToArray());
                    SortedSet<uint> unreferencedActions = new SortedSet<uint>(allActions.Select(x => x.manualUserID).ToArray());

                    while (refList.Count > 0) {
                        writer.BaseStream.Position = refList[0].invokedOffset; //place seek head on parameter that is supposed to store the jump address
                        (uint actionID, uint offsetInFile) result;

                        if (refList[0].invokedType == containerTypes.MOVEMENT) { //isApplyMovement 
                            result = actionOffsets.Find(x => x.actionID == refList[0].invokedID);

                            if (result.Equals((0, 0)))
                                undeclaredActions.Add(refList[0].invokedID);
                            else {
                                int relativeOffset = (int)(result.offsetInFile - refList[0].invokedOffset - 4);
                                writer.Write(relativeOffset);
                                unreferencedActions.Remove(refList[0].invokedID);
                            }
                        } else {
                            result = functionOffsets.Find(x => x.functionID == refList[0].invokedID);

                            if (result.Equals((0, 0)))
                                undeclaredFuncs.Add(refList[0].invokedID);
                            else {
                                int relativeOffset = (int)(result.offsetInFile - refList[0].invokedOffset - 4);
                                writer.Write(relativeOffset);
                                if (refList[0].callerType != containerTypes.FUNCTION || 
                                    (refList[0].callerType == refList[0].invokedType && refList[0].callerID == refList[0].invokedID) ||
                                    !uninvokedFuncs.Contains(refList[0].callerID)) { //remove reference if caller is a script, or if caller calls itself, or if caller is a function that's been invoked already
                                    uninvokedFuncs.Remove(refList[0].invokedID);
                                }
                            }
                        }
                        refList.RemoveAt(0);
                    }

                    string errorMsg = "";
                    if (undeclaredFuncs.Count > 0) {
                        string[] errorFunctionsUndeclared = undeclaredFuncs.ToArray().Select(x => x.ToString()).ToArray();
                        errorMsg += "These Functions have been invoked but not declared: " + Environment.NewLine + string.Join(",", errorFunctionsUndeclared);
                        errorMsg += Environment.NewLine;
                    }
                    if (undeclaredActions.Count > 0) {
                        string[] errorActionsUndeclared = undeclaredActions.ToArray().Select(x => x.ToString()).ToArray();
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
                        errorMsg += "\nIn order for a Function to be saved, it must be invoked by a Script or by another used Function.";
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
        public void SaveToFileDefaultDir(int IDtoReplace, bool showSuccessMessage = true) {
            SaveToFileDefaultDir(DirNames.scripts, IDtoReplace, showSuccessMessage);
        }
        public void SaveToFileExplorePath(string suggestedFileName, bool blindmode) {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "Gen IV Script File (*.scr)|*.scr";

            if (!string.IsNullOrEmpty(suggestedFileName))
                sf.FileName = suggestedFileName;
            if (sf.ShowDialog() != DialogResult.OK)
                return;

            if (blindmode) {
                File.Copy(RomInfo.gameDirs[DirNames.scripts].unpackedDir + "\\" + ((int)fileID).ToString("D4"), sf.FileName, overwrite: true);

                string msg = "";
                if (!isLevelScript)
                    msg += "The last saved version of this ";
                MessageBox.Show(msg + GetType().Name + " has been exported successfully.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } else {
                this.SaveToFile(sf.FileName, showSuccessMessage: true);
            }
        }
        #endregion
        #endregion
    }

    internal class ScriptReference {
        public containerTypes callerType { get; private set; }
        public uint callerID { get; private set; }
        public containerTypes invokedType { get; private set; }
        public uint invokedID { get; private set; }
        public int invokedOffset { get; private set; }

        public ScriptReference(containerTypes callerType, uint callerID, containerTypes invokedType, uint invokedID, int invokedOffset) {
            this.callerType = callerType;
            this.callerID = callerID;
            this.invokedType = invokedType;
            this.invokedID = invokedID;
            this.invokedOffset = invokedOffset;
        }
    }
}