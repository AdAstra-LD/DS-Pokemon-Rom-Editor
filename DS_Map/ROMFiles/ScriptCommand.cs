using DSPRE.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace DSPRE.ROMFiles {
    public class ScriptCommand {
        enum ParamTypeEnum {
            INTEGER,
            VARIABLE,
            FLEX,
            OW_ID,
            OW_MOVEMENT_TYPE,
            OW_DIRECTION,
            FUNCTION_ID,
            ACTION_ID,
            CMD_NUMBER
        };

        public ushort? id;
        public List<byte[]> cmdParams;
        public string name;

        public ScriptCommand(ushort id, List<byte[]> parametersList) {
            if (parametersList is null) {
                this.id = null;
                return;
            }

            if (!RomInfo.ScriptCommandNamesDict.TryGetValue(id, out name)) {
                name = FormatNumber(id, ParamTypeEnum.CMD_NUMBER);
            }

            switch (id) {
                case 0x0016: // Jump
                case 0x001A: // Call
                    name += $" {FormatNumber(parametersList[0], ParamTypeEnum.FUNCTION_ID)}";
                    break;
                case 0x0017: // JumpIfObjID
                case 0x0018: // JumpIfEventID
                    name += $" {FormatNumber(parametersList[0], ParamTypeEnum.OW_ID)} {FormatNumber(parametersList[1])}";
                    break;
                case 0x0019: // JumpIfPlayerDir
                    name += $" {FormatNumber(parametersList[0], ParamTypeEnum.OW_DIRECTION)} {FormatNumber(parametersList[1], ParamTypeEnum.FUNCTION_ID)}";
                    break;
                case 0x001C: // JumpIf
                case 0x001D: // CallIf
                {
                        string number = FormatNumber(parametersList[1], ParamTypeEnum.FUNCTION_ID);

                        if (RomInfo.ScriptComparisonOperatorsDict.TryGetValue(parametersList[0][0], out string v)) {
                            name += $" {v} {number}";
                        } else {
                            name += $" {parametersList[0][0]} {number}";
                        }

                        break;
                    }
                case 0x005E: // Movement
                    name += $" {FormatNumber(parametersList[0], ParamTypeEnum.OW_ID)} {FormatNumber(parametersList[1], ParamTypeEnum.ACTION_ID)}";
                    break;
                case 0x006A: // GetOverworldPosition
                    name += FormatCmd_Overworld_TwoParams(parametersList);
                    break;
                case 0x0062: // Lock
                case 0x0063: // Release
                case 0x0064: // AddOW
                case 0x0065: // RemoveOW
                    name += $" {FormatNumber(parametersList[0], ParamTypeEnum.OW_ID)}";
                    break;
                case 0x006D: // SetOverworldMovement
                    name += FormatCmd_Overworld_Move(parametersList);
                    break;

                case 0x00B0: // Warp [HGSS]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.HGSS)) {
                        name += FormatCmd_Warp(parametersList);
                    } else {
                        goto default;
                    }

                    break;
                case 0x0152: // SetOverworldDefaultPosition [HGSS]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.HGSS)) {
                        name += FormatCmd_Overworld_TwoParams(parametersList);
                    } else {
                        goto default;
                    }

                    break;
                case 0x0153: // SetOverworldPosition [HGSS]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.HGSS)) {
                        name += FormatCmd_Overworld_3Coords_Dir(parametersList);
                    } else {
                        goto default;
                    }

                    break;
                case 0x0154: // SetOverworldDefaultMovement [HGSS]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.HGSS)) {
                        name += FormatCmd_Overworld_Move(parametersList);
                    } else {
                        goto default;
                    }

                    break;
                case 0x0155: // SetOverworldDefaultDirection [DPPt]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.HGSS)) {
                        name += FormatCmd_Overworld_Dir(parametersList);
                    } else {
                        goto default;
                    }

                    break;
                case 0x0158: // SetOverworldDirection [DPPt]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.HGSS)) {
                        name += FormatCmd_Overworld_Dir(parametersList);
                    } else {
                        goto default;
                    }

                    break;

                case 0x00BE: // Warp [DPPt]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.DP) || RomInfo.gameFamily.Equals(RomInfo.GameFamilies.Plat)) {
                        name += FormatCmd_Warp(parametersList);
                    } else {
                        goto default;
                    }

                    break;
                case 0x0186: // SetOverworldDefaultPosition [DPPt]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.DP) || RomInfo.gameFamily.Equals(RomInfo.GameFamilies.Plat)) {
                        name += FormatCmd_Overworld_TwoParams(parametersList);
                    } else {
                        goto default;
                    }

                    break;
                case 0x0187: // SetOverworldPosition [DPPt]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.DP) || RomInfo.gameFamily.Equals(RomInfo.GameFamilies.Plat)) {
                        name += FormatCmd_Overworld_3Coords_Dir(parametersList);
                    } else {
                        goto default;
                    }

                    break;
                case 0x0188: // SetOverworldDefaultMovement [DPPt]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.DP) || RomInfo.gameFamily.Equals(RomInfo.GameFamilies.Plat)) {
                        name += FormatCmd_Overworld_Move(parametersList);
                    } else {
                        goto default;
                    }

                    break;
                case 0x0189: // SetOverworldDefaultDirection [DPPt]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.DP) || RomInfo.gameFamily.Equals(RomInfo.GameFamilies.Plat)) {
                        name += FormatCmd_Overworld_Dir(parametersList);
                    } else {
                        goto default;
                    }

                    break;
                case 0x018C: // SetOverworldDirection [DPPt]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.DP) || RomInfo.gameFamily.Equals(RomInfo.GameFamilies.Plat)) {
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
            name = wholeLine;
            cmdParams = new List<byte[]>();

            string[] nameParts = wholeLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries); // Separate command code from parameters
            /* Get command id, which is always first in the description */

            if (RomInfo.ScriptCommandNamesReverseDict.TryGetValue(nameParts[0].ToLower(), out ushort cmdID)) {
                id = cmdID;
            } else {
                try {
                    id = ushort.Parse(nameParts[0].PurgeSpecial(ScriptFile.specialChars), nameParts[0].GetNumberStyle());
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

            byte[] parametersSizeArr = RomInfo.ScriptCommandParametersDict[(ushort)id];

            int paramLength = 0;
            int paramsProcessed = 0;

            if (parametersSizeArr.First() == 0xFF) {
                int firstParamValue = int.Parse(nameParts[1].PurgeSpecial(ScriptFile.specialChars), nameParts[1].GetNumberStyle());
                byte firstParamSize = parametersSizeArr[1];

                cmdParams.Add(firstParamValue.ToByteArrayChooseSize(firstParamSize));
                paramsProcessed++;

                int i = 2;
                int optionsCount = 0;

                bool found = false;
                while (i < parametersSizeArr.Length) {
                    paramLength = parametersSizeArr[i + 1];

                    if (parametersSizeArr[i] == firstParamValue) {
                        //Firstly, build subarray of parameter sizes, starting from the chosen option [firstParamValue]
                        //FOR EXAMPLE: CMD 0x235 and firstParamValue = 5

                        // { 0xFF, 2,  
                        // 0, 1,   2,       
                        // 1, 3,   2, 2, 2, 
                        // 2, 0,            
                        // 3, 3,   2, 2, 2, 
                        // 4, 2,   2, 2,    
                        // 5, 3,   (2, 2, 2) => this will be the parameters subarray 
                        // 6, 1,   2
                        // },      
                        byte[] subParametersSize = parametersSizeArr.SubArray(i + 2, paramLength++);

                        //Create a slightly bigger temp array 
                        byte[] temp = new byte[1 + subParametersSize.Length];

                        //Store the size of the firstParamValue there
                        temp[0] = firstParamSize;

                        //Then copy the whole subarray of parameter sizes
                        Array.Copy(subParametersSize, 0, temp, 1, temp.Length - 1);

                        //Replace the original parametersSizeArr with the new array
                        parametersSizeArr = temp;
                        found = true;
                        break;
                    }

                    i += 2 + paramLength;
                    optionsCount++;
                }

                if (!found) {
                    MessageBox.Show($"Command {nameParts[0]} is a special Script Command.\n" +
                                    $"The value of the first parameter must be a number in the range [0 - {optionsCount}].\n\n" +
                                    $"Line {lineNumber}: {wholeLine}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    id = null;
                    return;
                }
            } else if (parametersSizeArr.Length == 1 && parametersSizeArr.First() == 0) {
                paramLength = 0;
            } else {
                paramLength = parametersSizeArr.Length;
            }

            if (nameParts.Length - 1 == paramLength) {
                for (int i = paramsProcessed; i < paramLength; i++) {
                    Console.WriteLine($"Parameter #{i}: {nameParts[i + 1]}");

                    if (RomInfo.ScriptComparisonOperatorsReverseDict.TryGetValue(nameParts[i + 1].ToLower(), out cmdID)) { //Check succeeds when command is like "asdfg LESS" or "asdfg DIFFERENT"
                        cmdParams.Add(new byte[] { (byte)cmdID });
                    } else { //Not a comparison
                        /* Convert strings of parameters to the correct datatypes */
                        NumberStyles numStyle = nameParts[i + 1].GetNumberStyle();
                        nameParts[i + 1] = nameParts[i + 1].PurgeSpecial(ScriptFile.specialChars);

                        int result = 0;

                        try {
                            result = int.Parse(nameParts[i + 1], numStyle);
                        } catch {
                            if (string.IsNullOrWhiteSpace(nameParts[i + 1])) {
                                MessageBox.Show($"You must specify an Overworld ID, Script, Function or Action number.\n\n" +
                                                $"Line {lineNumber}: {wholeLine}", "Unspecified identifier", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                id = null;
                            } else {
                                var first = ScriptDatabase.specialOverworlds.FirstOrDefault(x => x.Value.IgnoreCaseEquals(nameParts[i + 1]));

                                if (string.IsNullOrWhiteSpace(first.Value)) {
                                    var res = ScriptDatabase.overworldDirections.FirstOrDefault(x => x.Value.IgnoreCaseEquals(nameParts[i + 1]));

                                    if (string.IsNullOrWhiteSpace(res.Value)) {
                                        MessageBox.Show($"Argument {nameParts[i + 1]} couldn't be parsed as a valid Condition, Overworld ID, Direction ID, Script, Function or Action number.\n\n" +
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
                            cmdParams.Add(result.ToByteArrayChooseSize(parametersSizeArr[i]));
                        } catch (OverflowException) {
                            MessageBox.Show($"Argument {nameParts[i + 1]} at line {lineNumber} is not in the range [0, {Math.Pow(2, 8 * parametersSizeArr[i]) - 1}].", "Argument error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            id = null;
                        }
                    }
                }
            } else {
                MessageBox.Show($"Wrong number of parameters for command {nameParts[0]} at line {lineNumber}.\n" +
                                $"Received: {nameParts.Length - 1}\n" +
                                $"Expected: {paramLength}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                id = null;
            }
        }

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
                    return ScriptFile.ContainerTypes.Function.ToString() + "#" + num;

                case ParamTypeEnum.ACTION_ID:
                    return ScriptFile.ContainerTypes.Action.ToString() + "#" + num;

                case ParamTypeEnum.OW_MOVEMENT_TYPE:
                    if (num < 4000) {
                        outp += "Move.";
                    }

                    goto default;

                case ParamTypeEnum.OW_ID: {
                        if (ScriptDatabase.specialOverworlds.TryGetValue((ushort)num, out string output)) {
                            return output;
                        } else {
                            if (num < 4000) {
                                outp += $"{Event.EventType.Overworld}.";
                            }

                            goto default;
                        }
                    }
                case ParamTypeEnum.OW_DIRECTION: {
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
    }
}
