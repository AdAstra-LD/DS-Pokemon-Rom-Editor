using DSPRE.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static DSPRE.RomInfo;

namespace DSPRE.ROMFiles
{
    /// <summary>
    /// Class to store script file data in Pokémon NDS games
    /// </summary>
    public class ScriptFile : RomFile
    {
        public enum ContainerTypes
        {
            Function,
            Action,
            Script
        };

        public struct ContainerReference
        {
            public uint ID;
            public uint offsetInFile;
        }

        public List<ScriptCommandContainer> allScripts = new List<ScriptCommandContainer>();
        public List<ScriptCommandContainer> allFunctions = new List<ScriptCommandContainer>();
        public List<ScriptActionContainer> allActions = new List<ScriptActionContainer>();
        public int fileID = -1;
        public bool isLevelScript = new bool();

        public bool hasNoScripts { get { return fileID == int.MaxValue; } }

        public static readonly char[] specialChars = { 'x', 'X', '#', '.', '_' };

        public ScriptFile(Stream fs, bool readFunctions = true, bool readActions = true)
        {
            List<int> scriptOffsets = new List<int>();
            List<int> functionOffsets = new List<int>();
            List<int> movementOffsets = new List<int>();

            using (BinaryReader br = new BinaryReader(fs))
            {
                /* Read script offsets from the header */
                isLevelScript = true; // Is Level Script as long as magic number FD13 doesn't exist

                try
                {
                    while (true)
                    {
                        long headerPos = br.BaseStream.Position;
                        uint checker = br.ReadUInt16();
                        br.BaseStream.Position -= 0x2;
                        uint value = br.ReadUInt32();

                        if (value == 0 && scriptOffsets.Count == 0)
                        {
                            isLevelScript = true;
                            break;
                        }

                        if (checker == 0xFD13)
                        {
                            br.BaseStream.Position -= 0x4;
                            isLevelScript = false;
                            break;
                        }

                        int offsetFromStart = (int)(value + br.BaseStream.Position); // Don't change order of addition
                        scriptOffsets.Add(offsetFromStart);
                    }
                }
                catch (EndOfStreamException)
                {
                    if (!isLevelScript)
                    {
                        MessageBox.Show("Script File couldn't be read correctly.", "Unexpected EOF", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                if (isLevelScript)
                {
                    return;
                }

                // Skip the 0xFD13 marker
                br.ReadUInt16();
                /* Read scripts */
                for (uint current = 0; current < scriptOffsets.Count; current++)
                {
                    int index = scriptOffsets.FindIndex(x => x == scriptOffsets[(int)current]); // Check for UseScript

                    if (index == current)
                    {
                        br.BaseStream.Position = scriptOffsets[(int)current];

                        List<ScriptCommand> cmdList = new List<ScriptCommand>();
                        bool endScript = new bool();
                        while (!endScript)
                        {
                            ScriptCommand cmd = ReadCommand(br, ref functionOffsets, ref movementOffsets);
                            if (cmd.cmdParams is null)
                            {
                                return;
                            }

                            cmdList.Add(cmd);

                            if (ScriptDatabase.endCodes.Contains((ushort)cmd.id))
                            {
                                endScript = true;
                            }
                        }

                        allScripts.Add(new ScriptCommandContainer(current + 1, ContainerTypes.Script, commandList: cmdList));
                    }
                    else
                    {
                        allScripts.Add(new ScriptCommandContainer(current + 1, ContainerTypes.Script, usedScriptID: index + 1));
                    }
                }

                /* Read functions */
                if (readFunctions)
                {
                    for (uint current = 0; current < functionOffsets.Count; current++)
                    {
                        br.BaseStream.Position = functionOffsets[(int)current];
                        int posInList = scriptOffsets.IndexOf(functionOffsets[(int)current]); // Check for UseScript

                        if (posInList == -1)
                        {
                            List<ScriptCommand> cmdList = new List<ScriptCommand>();
                            bool endFunction = new bool();
                            while (!endFunction)
                            {
                                ScriptCommand command = ReadCommand(br, ref functionOffsets, ref movementOffsets);
                                if (command.cmdParams is null)
                                {
                                    return;
                                }

                                cmdList.Add(command);
                                if (ScriptDatabase.endCodes.Contains((ushort)command.id))
                                {
                                    endFunction = true;
                                }
                            }

                            allFunctions.Add(new ScriptCommandContainer(current + 1, ContainerTypes.Function, commandList: cmdList));
                        }
                        else
                        {
                            allFunctions.Add(new ScriptCommandContainer(current + 1, ContainerTypes.Function, usedScriptID: posInList + 1));
                        }
                    }
                }

                if (readActions)
                {
                    /* Read movements */
                    for (uint current = 0; current < movementOffsets.Count; current++)
                    {
                        br.BaseStream.Position = movementOffsets[(int)current];

                        List<ScriptAction> cmdList = new List<ScriptAction>();
                        bool endMovement = new bool();
                        while (!endMovement)
                        {
                            ushort id = br.ReadUInt16();
                            if (id == 0xFE)
                            {
                                endMovement = true;
                                cmdList.Add(new ScriptAction(id, 0));
                            }
                            else
                            {
                                cmdList.Add(new ScriptAction(id, br.ReadUInt16()));
                            }
                        }

                        allActions.Add(new ScriptActionContainer(current + 1, commands: cmdList));
                    }
                }
            }
        }

        public ScriptFile(int fileID, bool readFunctions = true, bool readActions = true) : this(getFileStream(fileID), readFunctions, readActions)
        {
            this.fileID = fileID;
        }

        static FileStream getFileStream(int fileID)
        {
            string path = Filesystem.GetScriptPath(fileID);
            return new FileStream(path, FileMode.OpenOrCreate);
        }
        public override string ToString()
        {
            string prefix = isLevelScript ? "Level " : "";
            return $"{prefix}Script File " + this.fileID;
        }

        public ScriptFile(List<ScriptCommandContainer> scripts, List<ScriptCommandContainer> functions, List<ScriptActionContainer> movements, int fileID = -1)
        {
            allScripts = scripts;
            allFunctions = functions;
            allActions = movements;
            isLevelScript = false;
        }

        public ScriptFile(IEnumerable<string> scriptLines, IEnumerable<string> functionLines, IEnumerable<string> actionLines, int fileID = -1)
        {
            //TODO: give user the possibility to jump to/call a script
            //once it's done, this Predicate below will be the only one needed, since there will be no distinction between
            //a script and a function
            bool functionEndCondition(List<(int linenum, string text)> source, int x, ushort? id)
            {
                return source[x].text.TrimEnd().IgnoreCaseEquals(RomInfo.ScriptCommandNamesDict[0x0002]) //End
                       || source[x].text.IndexOf(RomInfo.ScriptCommandNamesDict[0x0016] + ' ' + ContainerTypes.Function.ToString(), StringComparison.InvariantCultureIgnoreCase) >= 0 //Jump Function_#
                       || source[x].text.TrimEnd().IgnoreCaseEquals(RomInfo.ScriptCommandNamesDict[0x001B])
                       || ScriptDatabase.endCodes.Contains(id);
            } //Return

            bool scriptEndCondition(List<(int linenum, string text)> source, int x, ushort? id)
            {
                return source[x].text.TrimEnd().IgnoreCaseEquals(RomInfo.ScriptCommandNamesDict[0x0002]) //End
                       || source[x].text.IndexOf(RomInfo.ScriptCommandNamesDict[0x0016] + ' ' + ContainerTypes.Function.ToString()) >= 0 //Jump Function_#
                       || ScriptDatabase.endCodes.Contains(id);
            }

            allScripts = ReadCommandsFromLines(scriptLines.ToList(), ContainerTypes.Script, scriptEndCondition); //Jump + whitespace
            if (allScripts is null)
            {
                return;
            }

            if (allScripts.Count <= 0)
            {
                this.fileID = int.MaxValue;
                return;
            }

            if (functionLines != null)
            {
                allFunctions = ReadCommandsFromLines(functionLines.ToList(), ContainerTypes.Function, functionEndCondition); //Jump + whitespace
                if (allFunctions is null)
                {
                    return;
                }
            }

            if (actionLines != null)
            {
                allActions = ReadActionsFromLines(actionLines.ToList());
                if (allActions is null)
                {
                    return;
                }
            }

            this.fileID = fileID;
        }

        private ScriptCommand ReadCommand(BinaryReader dataReader, ref List<int> functionOffsets, ref List<int> actionOffsets)
        {
            ushort id = dataReader.ReadUInt16();
            List<byte[]> parameterList = new List<byte[]>();

            /* How to read parameters for different commands for DPPt*/
            switch (RomInfo.gameFamily)
            {
                case GameFamilies.DP:
                case GameFamilies.Plat:
                    switch (id)
                    {
                        case 0x16: //Jump
                        case 0x1A: //Call 
                            ProcessRelativeJump(dataReader, ref parameterList, ref functionOffsets);
                            break;
                        case 0x17: //JumpIfObjID
                        case 0x18: //JumpIfBgID
                        case 0x19: //JumpIfPlayerDir
                        case 0x1C: //JumpIf
                        case 0x1D: //CallIf
                                   //in the case of JumpIf and CallIf, the first param is a comparisonOperator
                                   //for JumpIfPlayerDir it's a directionID
                                   //for JumpIfObjID, it's an EventID
                            parameterList.Add(new byte[] { dataReader.ReadByte() });
                            ProcessRelativeJump(dataReader, ref parameterList, ref functionOffsets);
                            break;
                        case 0x5E: // Movement
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
                                {
                                    parameterList.Add(dataReader.ReadBytes(2)); //Read additional u16 if first param read is 2
                                }
                            }
                            break;
                        case 0x21D:
                            {
                                ushort parameter1 = dataReader.ReadUInt16();
                                parameterList.Add(BitConverter.GetBytes(parameter1));

                                switch (parameter1)
                                {
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

                                switch (parameter1)
                                {
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

                                switch (parameter1)
                                {
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
                                {
                                    parameterList.Add(dataReader.ReadBytes(2));
                                }
                            }
                            break;
                        case 0x2C5:
                            {
                                if (RomInfo.gameVersion == GameVersions.Platinum)
                                {
                                    parameterList.Add(dataReader.ReadBytes(2));
                                    parameterList.Add(dataReader.ReadBytes(2));
                                }
                                else
                                {
                                    goto default;
                                }
                            }
                            break;
                        case 0x2C6:
                        case 0x2C9:
                        case 0x2CA:
                        case 0x2CD:
                            if (RomInfo.gameVersion == GameVersions.Platinum)
                            {
                                break;
                            }
                            else
                            {
                                goto default;
                            }
                        case 0x2CF:
                            if (RomInfo.gameVersion == GameVersions.Platinum)
                            {
                                parameterList.Add(dataReader.ReadBytes(2));
                                parameterList.Add(dataReader.ReadBytes(2));
                            }
                            else
                            {
                                goto default;
                            }

                            break;
                        default:
                            addParametersToList(ref parameterList, id, dataReader);
                            break;
                    }

                    break;
                case GameFamilies.HGSS:
                    switch (id)
                    {
                        case 0x16: //Jump
                        case 0x1A: //Call 
                            ProcessRelativeJump(dataReader, ref parameterList, ref functionOffsets);
                            break;
                        case 0x17: //JumpIfObjID
                        case 0x18: //JumpIfBgID
                        case 0x19: //JumpIfPlayerDir
                        case 0x1C: //JumpIf
                        case 0x1D: //CallIf
                            parameterList.Add(new byte[] { dataReader.ReadByte() }); //in the case of JumpIf and CallIf, the first param is a comparisonOperator
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
                                {
                                    parameterList.Add(dataReader.ReadBytes(2));
                                }
                            }
                            break;
                        case 0x1D1: // Number of parameters differ depending on the first parameter value
                            {
                                short parameter1 = dataReader.ReadInt16();
                                parameterList.Add(BitConverter.GetBytes(parameter1));
                                switch (parameter1)
                                {
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
                                switch (parameter1)
                                {
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

        private void ProcessRelativeJump(BinaryReader dataReader, ref List<byte[]> parameterList, ref List<int> offsetsList)
        {
            int relativeOffset = dataReader.ReadInt32();
            int offsetFromScriptFileStart = (int)(relativeOffset + dataReader.BaseStream.Position);

            if (!offsetsList.Contains(offsetFromScriptFileStart))
            {
                offsetsList.Add(offsetFromScriptFileStart);
            }

            int functionNumber = offsetsList.IndexOf(offsetFromScriptFileStart);
            if (functionNumber < 0)
            {
                throw new InvalidOperationException();
            }

            parameterList.Add(BitConverter.GetBytes(functionNumber + 1));
        }

        private void addParametersToList(ref List<byte[]> parameterList, ushort id, BinaryReader dataReader)
        {
            Console.WriteLine("Loaded command id: " + id.ToString("X4"));
            try
            {
                foreach (int bytesToRead in RomInfo.ScriptCommandParametersDict[id])
                {
                    parameterList.Add(dataReader.ReadBytes(bytesToRead));
                }
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Script command " + id + "can't be handled for now." +
                                Environment.NewLine + "Reference offset 0x" + dataReader.BaseStream.Position.ToString("X"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                parameterList = null;
                return;
            }
            catch
            {
                MessageBox.Show("Error: ID Read - " + id +
                                Environment.NewLine + "Reference offset 0x" + dataReader.BaseStream.Position.ToString("X"), "Unrecognized script command", MessageBoxButtons.OK, MessageBoxIcon.Error);
                parameterList = null;
                return;
            }
        }

        private void AddReference(ref List<ScriptReference> references, ushort commandID, List<byte[]> parameterList, int pos, ScriptCommandContainer cont)
        {
            if (ScriptDatabase.commandsWithRelativeJump.TryGetValue(commandID, out int parameterWithRelativeJump))
            {
                uint invokedID = BitConverter.ToUInt32(parameterList[parameterWithRelativeJump], 0); // Jump, Call

                if (commandID == 0x005E)
                    references.Add(new ScriptReference(cont.containerType, cont.manualUserID, ContainerTypes.Action, invokedID, pos - 4));
                else
                {
                    references.Add(new ScriptReference(cont.containerType, cont.manualUserID, ContainerTypes.Function, invokedID, pos - 4));
                }
            }
        }

        private List<ScriptCommandContainer> ReadCommandsFromLines(List<string> linelist, ContainerTypes containerType, Func<List<(int linenum, string text)>, int, ushort?, bool> endConditions)
        {
            List<(int linenum, string text)> lineSource = new List<(int linenum, string text)>();

            for (int l = 0; l < linelist.Count; l++)
            {
                string cur = linelist[l];
                if (!string.IsNullOrWhiteSpace(cur))
                {
                    lineSource.Add((l, cur));
                }
            }

            List<ScriptCommandContainer> ls = new List<ScriptCommandContainer>();
            int i = 0;

            try
            {
                uint scriptNumber = 0;

                while (i < lineSource.Count)
                {
                    if (scriptNumber == 0)
                    {
                        int positionOfScriptNumber;
                        int positionOfScriptKeyword = lineSource[i].text.IndexOf(containerType.ToString(), StringComparison.InvariantCultureIgnoreCase);

                        if (positionOfScriptKeyword > 0)
                        {
                            MessageBox.Show("Unrecognized container keyword: \"" + lineSource[i] + '"', "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return null;
                        }
                        else if (positionOfScriptKeyword < 0)
                        {
                            i++;
                            continue;
                        }
                        else
                        {
                            if ((positionOfScriptNumber = lineSource[i].text.IndexOfFirstNumber()) < positionOfScriptKeyword)
                            {
                                MessageBox.Show("Unspecified Script/Function label.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return null;
                            }
                        }

                        scriptNumber = uint.Parse(lineSource[i++].text.Substring(positionOfScriptNumber).Split()[0].Replace(":", ""));
                    }

                    if (lineSource[i].text.IndexOf("UseScript", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    {
                        int useScriptNumber = short.Parse(lineSource[i].text.Substring(1 + lineSource[i].text.IndexOf('#')));
                        ls.Add(new ScriptCommandContainer(scriptNumber, containerType, useScriptNumber));
                        i++;
                    }
                    else
                    {
                        /* Read script commands */
                        List<ScriptCommand> cmdList = new List<ScriptCommand>();
                        ScriptCommand lastRead;

                        do
                        {
                            lastRead = new ScriptCommand(lineSource[i].text, lineSource[i].linenum + 1);
                            if (lastRead.id is null)
                            {
                                return null;
                            }

                            cmdList.Add(lastRead);
                        }
                        while (!endConditions(lineSource, i++, lastRead.id));

                        ls.Add(new ScriptCommandContainer(scriptNumber, containerType, commandList: cmdList));
                    }

                    scriptNumber = 0;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show($"Unexpectedly reached end of lines.\n\n" +
                                $"Last line index: {lineSource[i].linenum}.\n" +
                                $"Managed to parse {ls.Count} Command Containers.", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            return ls;
        }

        private List<ScriptActionContainer> ReadActionsFromLines(List<string> linelist)
        {
            List<(int linenum, string text)> lineSource = new List<(int linenum, string text)>();

            for (int l = 0; l < linelist.Count; l++)
            {
                string cur = linelist[l];
                if (!string.IsNullOrWhiteSpace(cur))
                {
                    lineSource.Add((l, cur));
                }
            }

            List<ScriptActionContainer> ls = new List<ScriptActionContainer>();
            int i = 0;

            try
            {
                uint actionNumber = 0;

                while (i < lineSource.Count)
                {
                    if (actionNumber == 0)
                    {
                        int positionOfActionNumber;
                        int positionOfActionKeyword = lineSource[i].text.IndexOf(ContainerTypes.Action.ToString(), StringComparison.InvariantCultureIgnoreCase);

                        if (positionOfActionKeyword > 0)
                        {
                            MessageBox.Show("Unrecognized container keyword: \"" + lineSource[i] + '"', "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return null;
                        }
                        else if (positionOfActionKeyword < 0)
                        {
                            i++;
                            continue;
                        }
                        else
                        {
                            if ((positionOfActionNumber = lineSource[i].text.IndexOfFirstNumber()) < positionOfActionKeyword)
                            {
                                MessageBox.Show("Unspecified Action label.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return null;
                            }
                        }

                        actionNumber = uint.Parse(lineSource[i].text.Substring(positionOfActionNumber).Split()[0].Replace(":", ""));
                        i++;
                    }

                    List<ScriptAction> cmdList = new List<ScriptAction>();
                    /* Read script actions */
                    do
                    {
                        ScriptAction toAdd = new ScriptAction(lineSource[i].text, lineSource[i].linenum + 1);
                        if (toAdd.id is null)
                        {
                            return null;
                        }

                        cmdList.Add(toAdd);
                    }
                    while (!lineSource[i++].text.IgnoreCaseEquals(RomInfo.ScriptActionNamesDict[0x00FE]));

                    ls.Add(new ScriptActionContainer(actionNumber, commands: cmdList));
                    actionNumber = 0;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show($"Unexpectedly reached end of lines.\n\n" +
                                $"Last line index: {i}.\n" +
                                $"Managed to parse {ls.Count} Command Containers.", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            return ls;
        }

        public override byte[] ToByteArray()
        {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData))
            {
                List<ContainerReference> scriptOffsets = new List<ContainerReference>(); //uint OFFSET, int Function/Script/Action ID
                List<ContainerReference> functionOffsets = new List<ContainerReference>();
                List<ContainerReference> actionOffsets = new List<ContainerReference>();

                List<ScriptReference> refList = new List<ScriptReference>();

                /* Allocate enough space for script pointers, which we do not know yet */
                try
                {
                    writer.BaseStream.Position += allScripts.Count * 0x4;
                    writer.Write((ushort)0xFD13); // Signal the end of header section
                    List<ScriptCommandContainer> useScriptCallers = new List<ScriptCommandContainer>();

                    /* Write scripts */
                    foreach (ScriptCommandContainer currentScript in allScripts)
                    {
                        if (currentScript.usedScriptID == -1)
                        {
                            scriptOffsets.Add(new ContainerReference()
                            {
                                ID = currentScript.manualUserID,
                                offsetInFile = (uint)writer.BaseStream.Position
                            }
                            );

                            foreach (ScriptCommand currentCmd in currentScript.commands)
                            {
                                writer.Write((ushort)currentCmd.id);
                                //System.Diagnostics.Debug.Write(BitConverter.ToString(BitConverter.GetBytes(commandID)) + " ");

                                List<byte[]> parameterList = currentCmd.cmdParams;
                                foreach (byte[] b in parameterList)
                                {
                                    writer.Write(b);
                                    //System.Diagnostics.Debug.WriteLine(BitConverter.ToString(parameterList[k]) + " ");
                                }

                                /* If command calls a function/movement, store reference position */
                                AddReference(ref refList, (ushort)currentCmd.id, parameterList, (int)writer.BaseStream.Position, currentScript);
                            }
                        }
                        else
                        {
                            useScriptCallers.Add(currentScript);
                        }
                    }

                    int scriptsCount = scriptOffsets.Count;
                    foreach (ScriptCommandContainer caller in useScriptCallers)
                    {
                        for (int i = 0; i < scriptsCount; i++)
                        {
                            ContainerReference scriptReference = scriptOffsets[i];

                            if (scriptReference.ID == caller.usedScriptID)
                            {
                                scriptOffsets.Add(new ContainerReference()
                                {
                                    ID = caller.manualUserID,
                                    offsetInFile = scriptReference.offsetInFile
                                }); // If script has UseScript, copy offset
                            }
                        }
                    }

                    /* Write functions */
                    foreach (ScriptCommandContainer currentFunction in allFunctions)
                    {
                        if (currentFunction.usedScriptID == -1)
                        {
                            functionOffsets.Add(new ContainerReference()
                            {
                                ID = currentFunction.manualUserID,
                                offsetInFile = (uint)writer.BaseStream.Position
                            }
                            );

                            foreach (ScriptCommand currentCmd in currentFunction.commands)
                            {
                                writer.Write((ushort)currentCmd.id);
                                //System.Diagnostics.Debug.Write(BitConverter.ToString(BitConverter.GetBytes(commandID)) + " ");

                                List<byte[]> parameterList = currentCmd.cmdParams;
                                foreach (byte[] b in parameterList)
                                {
                                    writer.Write(b);
                                    //System.Diagnostics.Debug.Write(BitConverter.ToString(parameterList[k]) + " ");
                                }

                                /* If command calls a function/movement, store reference position */

                                AddReference(ref refList, (ushort)currentCmd.id, parameterList, (int)writer.BaseStream.Position, currentFunction);
                            }
                        }
                        else
                        {
                            int functionUsescript = currentFunction.usedScriptID - 1;
                            if (functionUsescript >= scriptOffsets.Count)
                            {
                                MessageBox.Show($"Function #{currentFunction.manualUserID} refers to Script {currentFunction.usedScriptID}, which does not exist.\n" +
                                                $"This Script File can't be saved.", "Can't resolve UseScript reference", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return null;
                            }

                            functionOffsets.Add(new ContainerReference()
                            {
                                ID = currentFunction.manualUserID,
                                offsetInFile = scriptOffsets.Find(x => x.ID == currentFunction.usedScriptID).offsetInFile
                            });
                        }
                    }

                    // Movements must be halfword-aligned
                    if (writer.BaseStream.Position % 2 == 1)
                    { //Check if the writer's head is on an odd byte
                        writer.Write((byte)0x00); //Add padding
                    }

                    /* Write movements */
                    foreach (ScriptActionContainer currentAction in allActions)
                    {
                        actionOffsets.Add(new ContainerReference()
                        {
                            ID = currentAction.manualUserID,
                            offsetInFile = (uint)writer.BaseStream.Position
                        });

                        foreach (ScriptAction currentCmd in currentAction.commands)
                        {
                            writer.Write((ushort)currentCmd.id);
                            writer.Write((ushort)currentCmd.repetitionCount);
                        }
                    }

                    /* Write script offsets to header */
                    writer.BaseStream.Position = 0x0;

                    scriptOffsets = scriptOffsets.OrderBy(x => x.ID).ToList(); //Write script offsets to header in the correct order
                    for (int i = 0; i < scriptOffsets.Count; i++)
                    {
                        writer.Write(scriptOffsets[i].offsetInFile - (uint)writer.BaseStream.Position - 0x4);
                    }

                    SortedSet<uint> undeclaredFuncs = new SortedSet<uint>();
                    SortedSet<uint> undeclaredActions = new SortedSet<uint>();

                    SortedSet<uint> uninvokedFuncs = new SortedSet<uint>(allFunctions.Select(x => x.manualUserID).ToArray());
                    SortedSet<uint> unreferencedActions = new SortedSet<uint>(allActions.Select(x => x.manualUserID).ToArray());

                    //refList = refList.OrderBy(x => x.invokedID).ToList(); //Sorting is not necessary, after all...

                    for (int i = 0; i < refList.Count; i++)
                    {
                        writer.BaseStream.Position = refList[i].invokedAt; //place seek head on parameter that is supposed to store the jump address
                        ContainerReference result;

                        if (refList[i].typeOfInvoked is ContainerTypes.Action)
                        { //isApplyMovement 
                            result = actionOffsets.Find(entry => entry.ID == refList[i].invokedID);

                            if (result.Equals(default(ContainerReference)))
                            {
                                undeclaredActions.Add(refList[i].invokedID);
                            }
                            else
                            {
                                int relativeOffset = (int)(result.offsetInFile - refList[i].invokedAt - 4);
                                writer.Write(relativeOffset);
                                unreferencedActions.Remove(refList[i].invokedID);
                            }
                        }
                        else
                        {
                            result = functionOffsets.Find(entry => entry.ID == refList[i].invokedID);

                            if (result.Equals(default(ContainerReference)))
                            {
                                undeclaredFuncs.Add(refList[i].invokedID);
                            }
                            else
                            {
                                int relativeOffset = (int)(result.offsetInFile - refList[i].invokedAt - 4);
                                writer.Write(relativeOffset);

                                if (FunctionIsInvoked(refList, uninvokedFuncs, refList[i].invokedID, 0))
                                {
                                    uninvokedFuncs.Remove(refList[i].invokedID);
                                }

                                //if (refList[i].callerType != containerTypes.Function || 
                                //    (refList[i].callerType == refList[i].invokedType && refList[i].callerID == refList[i].invokedID) ||
                                //    !uninvokedFuncs.Contains(refList[i].callerID)) { //remove reference if caller is a script, or if caller calls itself, or if caller is a function that's been invoked already
                                //    uninvokedFuncs.Remove(refList[i].invokedID);
                                //}
                            }
                        }
                    }

                    //Error check
                    string errorMsg = "";
                    if (undeclaredFuncs.Count > 0)
                    {
                        string[] errorFunctionsUndeclared = undeclaredFuncs.ToArray().Select(x => x.ToString()).ToArray();
                        errorMsg += "These Functions have been invoked but not declared: " + Environment.NewLine + string.Join(separator: ",", errorFunctionsUndeclared);
                        errorMsg += Environment.NewLine;
                    }

                    if (undeclaredActions.Count > 0)
                    {
                        string[] errorActionsUndeclared = undeclaredActions.ToArray().Select(x => x.ToString()).ToArray();
                        errorMsg += "These Actions have been referenced but not declared: " + Environment.NewLine + string.Join(separator: ",", errorActionsUndeclared);
                        errorMsg += Environment.NewLine;
                    }

                    if (!string.IsNullOrEmpty(errorMsg))
                    {
                        MessageBox.Show(errorMsg + Environment.NewLine + "This Script File has not been overwritten since it can not be saved.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        errorMsg = "";
                        return null;
                    }

                    if (uninvokedFuncs.Count > 0)
                    {
                        string[] orphanedFunctions = uninvokedFuncs.ToArray().Select(x => x.ToString()).ToArray();
                        errorMsg += "Unused Function IDs detected: " + Environment.NewLine + string.Join(", ", orphanedFunctions);
                        errorMsg += Environment.NewLine;
                        errorMsg += "\nIn order for a Function to be saved, it must be invoked by a Script or by another used Function.";
                        errorMsg += Environment.NewLine;
                        errorMsg += Environment.NewLine;
                    }

                    if (unreferencedActions.Count > 0)
                    {
                        string[] orphanedActions = unreferencedActions.ToArray().Select(x => x.ToString()).ToArray();
                        errorMsg += "Unused Action IDs detected: " + Environment.NewLine + string.Join(", ", orphanedActions);
                        errorMsg += Environment.NewLine;
                        errorMsg += "\nIn order for an Action to be saved, it must be called by a Script or by a used Function.";
                        errorMsg += Environment.NewLine;
                        errorMsg += Environment.NewLine;
                    }

                    if (!string.IsNullOrEmpty(errorMsg))
                    {
                        MessageBox.Show(errorMsg + Environment.NewLine + "Remember that every unused Function or Action is always lost upon reloading the Script File.", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        errorMsg = "";
                    }
                }
                catch (NullReferenceException nre)
                {
                    Console.WriteLine(nre);
                    return null;
                }
            }

            return newData.ToArray();
        }

        private bool FunctionIsInvoked(List<ScriptReference> refList, SortedSet<uint> uninvokedFuncsSet, uint funcID, int callCount = 0, uint? excludedCaller = null)
        {
            if (callCount >= 30)
            {
                MessageBox.Show("Something went very wrong saving this Script File!" +
                                "\nIt is recommended that you backup its code somewhere, to avoid losing progress.",
                  "Fatal error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            Console.WriteLine("Checking calls of function " + funcID + (excludedCaller == null ? "" : " excluding Function " + excludedCaller + " as the caller."));

            if (!uninvokedFuncsSet.Contains(funcID))
            {
                Console.WriteLine("Function " + funcID + " has already been invoked before. Nothing to check.");
                return true; //Abort 
            }

            if (refList is null || refList.Count <= 0)
            {
                return false;
            }

            //Find the first instance of funcID being called, excluding calls coming from an excludedCaller
            //if excludedCaller is null, there's nothing to exclude: a normal search is performed.
            ScriptReference sr = refList.Find(x => x.invokedID == funcID && (excludedCaller == null || x.callerID != excludedCaller));

            if (sr is null)
            {
                Console.WriteLine("No reference found!!!");
                return false;
            }

            if (sr.typeOfCaller is ContainerTypes.Script)
            {
                Console.WriteLine("Function " + funcID + " is directly called by Script " + sr.callerID);
                return true;
            }

            if (sr.typeOfCaller is ContainerTypes.Function)
            {
                if (FunctionIsInvoked(refList, uninvokedFuncsSet, sr.callerID, ++callCount, excludedCaller: sr.invokedID))
                { //check if caller function is invoked as well
                    Console.WriteLine("Function " + funcID + " is called by Function " + sr.callerID);
                    return true;
                }
            }

            Console.WriteLine("Function " + funcID + " is unused");
            return false;
        }

        public bool SaveToFileDefaultDir(int IDtoReplace, bool showSuccessMessage = true)
        {
            return SaveToFileDefaultDir(RomInfo.DirNames.scripts, IDtoReplace, showSuccessMessage);
        }

        public void SaveToFileExplorePath(string suggestedFileName, bool blindmode)
        {
            SaveFileDialog sf = new SaveFileDialog
            {
                Filter = "Gen IV Script File (*.scr)|*.scr"
            };

            if (!string.IsNullOrEmpty(suggestedFileName))
            {
                sf.FileName = suggestedFileName;
            }

            if (sf.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            if (blindmode)
            {
                string path = Filesystem.GetScriptPath(fileID);
                File.Copy(path, sf.FileName, overwrite: true);

                string msg = "";
                if (!isLevelScript)
                {
                    msg += "The last saved version of this ";
                }

                MessageBox.Show(msg + GetType().Name + " has been exported successfully.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                this.SaveToFile(sf.FileName, showSuccessMessage: true);
            }
        }
    }
}