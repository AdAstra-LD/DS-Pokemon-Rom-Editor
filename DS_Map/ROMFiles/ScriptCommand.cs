using DSPRE.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using static DSPRE.ROMFiles.Event;
using static DSPRE.ROMFiles.ScriptFile;

namespace DSPRE.ROMFiles {
    public enum ParamTypeEnum { INTEGER, VARIABLE, FLEX, OW_ID, OW_MOVEMENT_TYPE, OW_DIRECTION, FUNCTION_ID, ACTION_ID, CMD_NUMBER };
    public class CommandContainer {
        public List<ScriptCommand> commands;
        public uint manualUserID;
        public int usedScript; //useScript ID referenced by this Script/Function
        public containerTypes containerType;
        internal static readonly string functionStart;

        #region Constructors (2)
        public CommandContainer(uint scriptNumber, containerTypes containerType, int useScript = -1, List<ScriptCommand> commandList = null) {
            manualUserID = scriptNumber;
            this.usedScript = useScript;
            this.containerType = containerType;
            commands = commandList;
        }
        public CommandContainer(uint newID, CommandContainer toCopy) {
            manualUserID = newID;
            usedScript = toCopy.usedScript;
            containerType = toCopy.containerType;
            commands = new List<ScriptCommand>(toCopy.commands); //command parameters need to be copied recursively
        }
        #endregion
    }
    public class ScriptCommand {
        #region Fields (4)
        public ushort? id;
        public List<byte[]> cmdParams;
        public string name;
        #endregion

        #region Constructors (2)
        public ScriptCommand(ushort id, List<byte[]> parametersList) {
            if (parametersList is null) {
                this.id = null;
                return;
            }

            if (!RomInfo.ScriptCommandNamesDict.TryGetValue(id, out name)) {
                name = FormatNumber(id, ParamTypeEnum.CMD_NUMBER);
            }

            switch (id) {
                case 0x0016:      // Jump
                case 0x001A:      // Call
                    name += $" {FormatNumber(parametersList[0], ParamTypeEnum.FUNCTION_ID)}";
                    break;
                case 0x0017:      // JumpIfObjID
                case 0x0018:      // JumpIfEventID
                    name += $" {FormatNumber(parametersList[0], ParamTypeEnum.OW_ID)} {FormatNumber(parametersList[1])}";
                    break;
                case 0x0019:      // JumpIfPlayerDir
                    name += $" {FormatNumber(parametersList[0], ParamTypeEnum.OW_DIRECTION)} {FormatNumber(parametersList[1], ParamTypeEnum.ACTION_ID)}";
                    break;
                case 0x001C:      // JumpIf
                case 0x001D:      // CallIf
                    {
                        string number = FormatNumber(parametersList[1], ParamTypeEnum.FUNCTION_ID);

                        if (RomInfo.ScriptComparisonOperatorsDict.TryGetValue(parametersList[0][0], out string v)) {
                            name += $" {v} {number}";
                        } else {
                            name += $" {parametersList[0][0]} {number}";
                        }
                        break;
                    }
                case 0x005E:      // Movement
                    name += $" {FormatNumber(parametersList[0], ParamTypeEnum.OW_ID)} {FormatNumber(parametersList[1], ParamTypeEnum.ACTION_ID)}";
                    break;
                case 0x006A:      // GetOverworldPosition
                    name += FormatCmd_Overworld_TwoParams(parametersList);
                    break;
                case 0x0062:      // Lock
                case 0x0063:      // Release
                case 0x0064:      // AddOW
                case 0x0065:      // RemoveOW
                    name += $" {FormatNumber(parametersList[0], ParamTypeEnum.OW_ID)}";
                    break;
                case 0x006D:      // SetOverworldMovement
                    name += FormatCmd_Overworld_Move(parametersList);
                    break;

                case 0x00B0:      // Warp [HGSS]
                    if (RomInfo.gameFamily.Equals(RomInfo.gFamEnum.HGSS)) {
                        name += FormatCmd_Warp(parametersList);
                    } else {
                        goto default;
                    }
                    break;
                case 0x0152:      // SetOverworldDefaultPosition [HGSS]
                    if (RomInfo.gameFamily.Equals(RomInfo.gFamEnum.HGSS)) {
                        name += FormatCmd_Overworld_TwoParams(parametersList);
                    } else {
                        goto default;
                    }
                    break;
                case 0x0153:      // SetOverworldPosition [HGSS]
                    if (RomInfo.gameFamily.Equals(RomInfo.gFamEnum.HGSS)) {
                        name += FormatCmd_Overworld_3Coords_Dir(parametersList);
                    } else {
                        goto default;
                    }
                    break;
                case 0x0154:      // SetOverworldDefaultMovement [HGSS]
                    if (RomInfo.gameFamily.Equals(RomInfo.gFamEnum.HGSS)) {
                        name += FormatCmd_Overworld_Move(parametersList);
                    } else {
                        goto default;
                    }
                    break;
                case 0x0155:      // SetOverworldDefaultDirection [DPPt]
                    if (RomInfo.gameFamily.Equals(RomInfo.gFamEnum.HGSS)) {
                        name += FormatCmd_Overworld_Dir(parametersList);
                    } else {
                        goto default;
                    }
                    break;
                case 0x0158:      // SetOverworldDirection [DPPt]
                    if (RomInfo.gameFamily.Equals(RomInfo.gFamEnum.HGSS)) {
                        name += FormatCmd_Overworld_Dir(parametersList);
                    } else {
                        goto default;
                    }
                    break;

                case 0x00BE:      // Warp [DPPt]
                    if (RomInfo.gameFamily.Equals(RomInfo.gFamEnum.DP) || RomInfo.gameFamily.Equals(RomInfo.gFamEnum.Plat)) {
                        name += FormatCmd_Warp(parametersList);
                    } else {
                        goto default;
                    }
                    break;
                case 0x0186:      // SetOverworldDefaultPosition [DPPt]
                    if (RomInfo.gameFamily.Equals(RomInfo.gFamEnum.DP) || RomInfo.gameFamily.Equals(RomInfo.gFamEnum.Plat)) {
                        name += FormatCmd_Overworld_TwoParams(parametersList);
                    } else {
                        goto default;
                    }
                    break;
                case 0x0187:      // SetOverworldPosition [DPPt]
                    if (RomInfo.gameFamily.Equals(RomInfo.gFamEnum.DP) || RomInfo.gameFamily.Equals(RomInfo.gFamEnum.Plat)) {
                        name += FormatCmd_Overworld_3Coords_Dir(parametersList);
                    } else {
                        goto default;
                    }
                    break;
                case 0x0188:      // SetOverworldDefaultMovement [DPPt]
                    if (RomInfo.gameFamily.Equals(RomInfo.gFamEnum.DP) || RomInfo.gameFamily.Equals(RomInfo.gFamEnum.Plat)) {
                        name += FormatCmd_Overworld_Move(parametersList);
                    } else {
                        goto default;
                    }
                    break;
                case 0x0189:      // SetOverworldDefaultDirection [DPPt]
                    if (RomInfo.gameFamily.Equals(RomInfo.gFamEnum.DP) || RomInfo.gameFamily.Equals(RomInfo.gFamEnum.Plat)) {
                        name += FormatCmd_Overworld_Dir(parametersList);
                    } else {
                        goto default;
                    }
                    break;
                case 0x018C:      // SetOverworldDirection [DPPt]
                    if (RomInfo.gameFamily.Equals(RomInfo.gFamEnum.DP) || RomInfo.gameFamily.Equals(RomInfo.gFamEnum.Plat)) {
                        name += FormatCmd_Overworld_Dir(parametersList);
                    } else {
                        goto default;
                    }
                    break;
                default:
                    for (int i = 0; i < parametersList.Count; i++) {
                        name += $" {FormatNumber(parametersList[i])}";
                    }
                    break;

            }
            this.id = id;
            this.cmdParams = parametersList;
        }
        private string FormatCmd_Warp(List<byte[]> parametersList) {
            return $" {FormatNumber(parametersList[0])} {FormatNumber(parametersList[1])} {FormatNumber(parametersList[2])} {FormatNumber(parametersList[3])} {FormatNumber(parametersList[4], ParamTypeEnum.OW_DIRECTION)}";
        }
        private string FormatCmd_Overworld_TwoParams(List<byte[]> parametersList) {
            return $" {FormatNumber(parametersList[0], ParamTypeEnum.OW_ID)} {FormatNumber(parametersList[1])} {FormatNumber(parametersList[2])}";
        }
        private string FormatCmd_Overworld_Move(List<byte[]> parametersList) {
            return $" {FormatNumber(parametersList[0], ParamTypeEnum.OW_ID)} {FormatNumber(parametersList[1], ParamTypeEnum.OW_MOVEMENT_TYPE)}";
        }
        private string FormatCmd_Overworld_3Coords_Dir(List<byte[]> parametersList) {
            return $" {FormatNumber(parametersList[0], ParamTypeEnum.OW_ID)} {FormatNumber(parametersList[1])} {FormatNumber(parametersList[2])} {FormatNumber(parametersList[3])} {FormatNumber(parametersList[4], ParamTypeEnum.OW_DIRECTION)}";
        }
        private string FormatCmd_Overworld_Dir(List<byte[]> parametersList) {
            return $" {FormatNumber(parametersList[0], ParamTypeEnum.OW_ID)} {FormatNumber(parametersList[1], ParamTypeEnum.OW_DIRECTION)}";
        }

        public ScriptCommand(string wholeLine, int lineNumber = 0) {
            const int NAMEPARTS_PARAMS_FIRST = 1;

            name = wholeLine;
            cmdParams = new List<byte[]>();

            string[] nameParts = wholeLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries); // Separate command code from parameters
            /* Get command id, which is always first in the description */

            if (RomInfo.ScriptCommandNamesReverseDict.TryGetValue(nameParts[0].ToLower(), out ushort cmdID)) {
                id = cmdID;
            } else {
                try {
                    id = ushort.Parse(nameParts[0].PurgeSpecial(ScriptFile.specialChars), nameParts[0].GetNumberStyle());
                    //id = ushort.Parse(nameParts[0].Substring(nameParts[0].("_")+1), Properties.Settings.Default.scriptEditorFormatPreference, CultureInfo.InvariantCulture);
                } catch {
                    string details;
                    if (wholeLine.Contains(':') && wholeLine.ContainsNumber()) {
                        details = "This probably means you forgot to \"End\" the Script or Function above it.";
                        details += Environment.NewLine + "Please, also note that only Functions can be terminated\nwith \"Return\".";
                    } else {
                        details = "Are you sure it's a proper Script Command?";
                    }

                    MessageBox.Show("This Script file could not be saved." +
                    $"\nParser failed to interpret line {lineNumber}: \"{wholeLine}\".\n\n{details}", "Parser error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            /* Read parameters from remainder of the description */
            //Console.WriteLine("ID = " + ((ushort)id).ToString("X4"));

            byte[] parametersDescriptorArr = RomInfo.ScriptCommandParametersDict[(ushort)id];

            Dictionary<int, byte[]> configurations = new Dictionary<int, byte[]>();

            int firstByte = parametersDescriptorArr.First();
            int sizeKeyParam = -1; //In bytes. Unspecified by default.
            int posKeyParam = -1; //Relative to the start of the descriptor [database] array. Unspecified by default.

            if (parametersDescriptorArr.Length == 1 && firstByte == 0) {
                parametersDescriptorArr = new byte[0];
            }

            if (firstByte == 0xFF) { //When first byte of the descriptor [database] array is 0xFF, the command takes a variable number of parameters
                int i = 1;

                //The 2nd byte of the descriptor array is the number of fixed parameters
                //(which must be read in all possible configurations of parameter count/size), since they're in common
                int numFixedParams = parametersDescriptorArr[i++];


                //The key parameter is a switcher value (as in, switch-case)
                //It selects a particular parameter configuration.
                //It is included in the number of fixed params (numFixedParams).
                posKeyParam = i;
                sizeKeyParam = parametersDescriptorArr[i];
                //In most [if not all cases], the number of fixed params is 1, meaning that
                //there is just the key (switcher).


                byte[] sizesOfFixedParams = null; 
                //do not instantiate an array of fixed parameter sizes until it's absolutely sure
                //that it will contain more than 0 elements.


                if (numFixedParams > 0) {
                    sizesOfFixedParams = new byte[numFixedParams];

                    //Copy the fixed [common] parameters from the descriptor to the new fixedParams array
                    Array.Copy(parametersDescriptorArr, i, sizesOfFixedParams, 0, numFixedParams);
                    i += numFixedParams;
                }

                while (i < parametersDescriptorArr.Length) {
                    byte keyValToCheck = parametersDescriptorArr[i++];
                    byte numParamsToRead = parametersDescriptorArr[i++];

                    List<byte> paramsSizesInOption = new List<byte>(sizesOfFixedParams);
                    for (int opt = 0; opt < numParamsToRead; opt++, i++) {
                        paramsSizesInOption.Add(parametersDescriptorArr[i + opt]);
                    }
                    configurations.Add(keyValToCheck, paramsSizesInOption.ToArray());
                    //Each configuration will contain at least the size of the params, since those are in common (fixed)
                }

                if (configurations.Count > 0) {
                    int offs = NAMEPARTS_PARAMS_FIRST;
                    NumberStyles numStyle = nameParts[offs].GetNumberStyle();
                    nameParts[offs] = nameParts[offs].PurgeSpecial(ScriptFile.specialChars);
                    int chosenConfiguration = int.Parse(nameParts[offs], numStyle);

                    if (!configurations.TryGetValue(chosenConfiguration, out parametersDescriptorArr)) {
                        parametersDescriptorArr = configurations[-1];
                    }
                } else {
                    MessageBox.Show($"The parameter database is corrupted.\nFailed to retrieve param configurations for command {cmdID} at line {lineNumber}.\n" +
                        $"Line: '{wholeLine}'", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    id = null;
                    return;
                }
            }

            int lenParams = nameParts.Length - NAMEPARTS_PARAMS_FIRST;

            if (lenParams < sizeKeyParam || lenParams < parametersDescriptorArr.Length) {
                string appendix = "";
                if (configurations.Count > 0) {
                    appendix += "\n\nThis command only accepts parameters in the following formats:\n";
                    foreach (var kvp in configurations) {
                        for (int par = 0; par < kvp.Value.Length; par++) {
                            if (par == sizeKeyParam) {
                                appendix += "{" + kvp.Value[par] + "B " + "} ";
                            } else {
                                appendix += kvp.Value[par] + "B ";
                            }
                        }
                        appendix += '\n';
                    }
                }

                MessageBox.Show($"Wrong number of parameters for command {nameParts[0]} at line {lineNumber}.\n" +
                    $"Received: {nameParts.Length - 1}\n" +
                    $"Expected: {parametersDescriptorArr.Length}" + appendix, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                id = null;
                return;
            }

            int paramsProcessed = 0;

            for (int i = paramsProcessed; i < parametersDescriptorArr.Length; i++) {
                int offs = i + NAMEPARTS_PARAMS_FIRST;
                Console.WriteLine($"Parameter #{i}: {nameParts[offs]}");

                if (RomInfo.ScriptComparisonOperatorsReverseDict.TryGetValue(nameParts[offs].ToLower(), out cmdID)) { //Check succeeds when command is like "asdfg LESS" or "asdfg DIFFERENT"
                    cmdParams.Add(new byte[] { (byte)cmdID });
                } else { //Not a comparison
                    /* Convert strings of parameters to the correct datatypes */
                    NumberStyles numStyle = nameParts[offs].GetNumberStyle();
                    nameParts[offs] = nameParts[offs].PurgeSpecial(ScriptFile.specialChars);
                        
                    int result = 0;

                    try {
                        result = int.Parse(nameParts[offs], numStyle);
                    } catch {
                        if (string.IsNullOrWhiteSpace(nameParts[offs])) {
                            MessageBox.Show($"You must specify an Overworld ID, Script, Function or Action number.\n\n" +
                                $"Line {lineNumber}: {wholeLine}", "Unspecified identifier", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            id = null;
                        } else { 
                            var first = ScriptDatabase.specialOverworlds.FirstOrDefault(x => x.Value.IgnoreCaseEquals(nameParts[offs]));

                            if (string.IsNullOrWhiteSpace(first.Value)) {
                                var res = ScriptDatabase.overworldDirections.FirstOrDefault(x => x.Value.IgnoreCaseEquals(nameParts[offs]));

                                if (string.IsNullOrWhiteSpace(res.Value)) {
                                    MessageBox.Show($"Argument {nameParts[offs]} couldn't be parsed as a valid Condition, Overworld ID, Direction ID, Script, Function or Action number.\n\n" +
                                    $"Line {lineNumber}: {wholeLine}", "Invalid identifier", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    id = null;
                                } else {
                                    result = res.Key;
                                }
                            } else {
                                result = first.Key;
                            }
                        }
                    }

                    try {
                        cmdParams.Add(result.ToByteArrayChooseSize(parametersDescriptorArr[i]));
                    } catch (OverflowException) {
                        MessageBox.Show($"Argument {nameParts[offs]} at line {lineNumber} is not in the range [0, {Math.Pow(2, 8 * parametersDescriptorArr[i]) - 1}].", "Argument error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        id = null;
                    }
                }
            }
        }
        #endregion

        #region Utilities
        private string FormatNumber(byte[] par, ParamTypeEnum paramType = ParamTypeEnum.INTEGER) {
            //number acquisition
            uint num;
            if (par.Length == 0) {
                return "";
            } else if (par.Length == 1) {
                num = par[0];
            } else if (par.Length == 2) {
                num = BitConverter.ToUInt16(par, 0);
            } else if (par.Length == 4) {
                num = BitConverter.ToUInt32(par, 0);
            } else {
                throw new InvalidOperationException();
            }
            return FormatNumber(num, paramType);
        }
        private string FormatNumber(uint num, ParamTypeEnum paramType = ParamTypeEnum.INTEGER) {
            //differentiate depending on param type
            string formatOverride;
            string prefix;

            if (Properties.Settings.Default.scriptEditorFormatPreference == (int)NumberStyles.HexNumber) {
                formatOverride = "X";
                prefix = "0x";
            } else { //(Properties.Settings.Default.scriptEditorFormatPreference == NumberStyles.Integer)
                formatOverride = "D";
                prefix = "";
            }

            string outp = "";

            switch (paramType) {
                case ParamTypeEnum.CMD_NUMBER:
                    return "CMD_" + prefix + num.ToString(formatOverride + '3');

                case ParamTypeEnum.FUNCTION_ID:
                    return containerTypes.Function.ToString() + "#" + num;

                case ParamTypeEnum.ACTION_ID:
                    return containerTypes.Action.ToString() + "#" + num;

                case ParamTypeEnum.OW_MOVEMENT_TYPE:
                    if (num < 4000) {
                        outp += "Move.";
                    }
                    goto default;

                case ParamTypeEnum.OW_ID: 
                    {
                        if (ScriptDatabase.specialOverworlds.TryGetValue((ushort)num, out string output)) {
                            return output;
                        } else {
                            if (num < 4000) {
                                outp += $"{EventType.Overworld}.";
                            }
                            goto default;
                        }
                    }
                case ParamTypeEnum.OW_DIRECTION: 
                    {
                        if (ScriptDatabase.overworldDirections.TryGetValue((byte)num, out string output)) {
                            return output;
                        } else {
                            if (num < 4000) {
                                outp += $"Direction.";
                            }
                            goto default;
                        }
                    }
                default:
                    if (Properties.Settings.Default.scriptEditorFormatPreference == (int)NumberStyles.None) {
                        if (num >= 4000) {
                            formatOverride = "X";
                            prefix = "0x";
                        }
                    }
                    outp += prefix + num.ToString(formatOverride);
                    break;
            }
            
            return outp;
        }
        public override string ToString() {
            return name + " (" + ((ushort)id).ToString("X") + ")";
        }
        #endregion
    }
}
