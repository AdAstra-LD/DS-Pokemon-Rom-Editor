using DSPRE.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using static DSPRE.ROMFiles.ScriptFile;

namespace DSPRE.ROMFiles {
    public enum ParamTypeEnum { INTEGER, VARIABLE, FLEX, OW_ID, FUNCTION_ID, ACTION_ID, CMD_NUMBER };
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
                    name += " " + FormatNumber(parametersList[0], ParamTypeEnum.FUNCTION_ID);
                    break;
                case 0x0017:      // JumpIfObjID
                case 0x0018:      // JumpIfEventID
                    name += " " + FormatNumber(parametersList[0], ParamTypeEnum.OW_ID) + " " + FormatNumber(parametersList[1]);
                    break;
                case 0x0019:      // JumpIfPlayerDir
                    name += " " + FormatNumber(parametersList[0]) + " " + FormatNumber(parametersList[1], ParamTypeEnum.ACTION_ID);
                    break;
                case 0x001C:      // JumpIf
                case 0x001D:      // CallIf
                    name += " " + RomInfo.ScriptComparisonOperatorsDict[ parametersList[0][0] ] + " " + FormatNumber(parametersList[1], ParamTypeEnum.FUNCTION_ID);
                    break;
                case 0x005E:      // Movement
                    name += " " + FormatNumber(parametersList[0], ParamTypeEnum.OW_ID) + " " + FormatNumber(parametersList[1], ParamTypeEnum.ACTION_ID);
                    break;
                case 0x006A:      // CheckOverworldPosition
                    name += " " + FormatNumber(parametersList[0], ParamTypeEnum.OW_ID) + " " + FormatNumber(parametersList[1]) + " " + FormatNumber(parametersList[2]);
                    break;
                case 0x0062:      // Lock
                case 0x0063:      // Release
                case 0x0064:      // AddOW
                case 0x0065:      // RemoveOW
                    name += " " + FormatNumber(parametersList[0], ParamTypeEnum.OW_ID);
                    break;
                default:
                    for (int i = 0; i < parametersList.Count; i++) {
                        this.name += " " + FormatNumber(parametersList[i]);
                    }
                    break;

            }
            this.id = id;
            this.cmdParams = parametersList;
        }

        public ScriptCommand(string wholeLine, int lineNumber = 0) {
            name = wholeLine;
            cmdParams = new List<byte[]>();

            string[] nameParts = wholeLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries); // Separate command code from parameters
            /* Get command id, which is always first in the description */

            ushort cmdID;
            if (RomInfo.ScriptCommandNamesReverseDict.TryGetValue(nameParts[0].ToLower(), out cmdID)) {
                id = cmdID;
            } else {
                try {
                    id = ushort.Parse(GetStringWithoutSpecialCharacters(nameParts[0]), GetNumberStyleFromString(nameParts[0]));
                    //id = ushort.Parse(nameParts[0].Substring(nameParts[0].("_")+1), ScriptFile.numFormatSpecifier, CultureInfo.InvariantCulture);
                } catch {
                    string details;
                    if (wholeLine.Contains(':') && wholeLine.ContainsNumber()) {
                        details = "This probably means you forgot to \"End\" the Script or Function above it.";
                        details += Environment.NewLine + "Please, also note that only Functions can be terminated\nwith \"Return\".";
                    } else {
                        details = "Are you sure it's a proper Script Command?";
                    }
                    MessageBox.Show("This Script file could not be saved." +
                        Environment.NewLine + "Parser failed to interpret line " + lineNumber + ": \"" + wholeLine + "\"." +
                        Environment.NewLine + "\n" + details, "Parser error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            /* Read parameters from remainder of the description */
            //Console.WriteLine("ID = " + ((ushort)id).ToString("X4"));

            byte[] parametersSizeArr = RomInfo.ScriptCommandParametersDict[(ushort)id];

            int paramLength = 0;
            int paramsProcessed = 0;

            if (parametersSizeArr.First() == 0xFF) { 
                int firstParamValue = int.Parse(GetStringWithoutSpecialCharacters(nameParts[1]), GetNumberStyleFromString(nameParts[1]));
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
                        Array.Copy(subParametersSize, 0, temp, 1, temp.Length-1);

                        //Replace the original parametersSizeArr with the new array
                        parametersSizeArr = temp;
                        found = true;
                        break;
                    }
                    i += 2 + paramLength;
                    optionsCount++;
                }
                if (!found) {
                    MessageBox.Show("Command " + '"' + nameParts[0] + '"' + " at line " + lineNumber + " is a special Script command." + Environment.NewLine +
                    "The value of the first parameter must be a number in the range [0 - " + optionsCount + "].", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    Console.WriteLine("Parameter #" + i.ToString() + ": " + nameParts[i + 1]);

                    if (RomInfo.ScriptComparisonOperatorsReverseDict.TryGetValue(nameParts[i + 1].ToLower(), out cmdID)) {
                        cmdParams.Add(new byte[] { (byte)cmdID });
                    } else { //Not a comparison
                        /* Convert strings of parameters to the correct datatypes */
                        NumberStyles style = GetNumberStyleFromString(nameParts[i + 1]);
                        nameParts[i + 1] = GetStringWithoutSpecialCharacters(nameParts[i + 1]);
                        
                        int result = 0;
                        try {
                            result = int.Parse(nameParts[i + 1], style);
                        } catch {
                            try {
                                result = ScriptDatabase.specialOverworlds.First(x => x.Value.Equals(nameParts[i + 1])).Key;
                            } catch (InvalidOperationException) {
                                MessageBox.Show("Argument " + '"' + nameParts[i + 1] + '"' + " at line " + lineNumber + " is not " + "a valid " + "Overworld number or identifier.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                id = null;
                            } catch (FormatException) {
                                MessageBox.Show("Argument " + '"' + nameParts[i + 1] + '"' + " at line " + lineNumber + " is not " + "a valid " + style, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                id = null;
                            }
                        }

                        try {
                            cmdParams.Add(result.ToByteArrayChooseSize(parametersSizeArr[i]));
                        } catch (OverflowException) {
                            MessageBox.Show("Argument " + '"' + nameParts[i + 1] + '"' + " at line " + lineNumber + " is not " + "in the range [" + 0 + ", " + (Math.Pow(2, 8 * parametersSizeArr[i]) - 1) + "].", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            id = null;
                        }
                    }
                }
            } else {
                MessageBox.Show("Wrong number of parameters for command " + '"' + nameParts[0] + '"' + " at line " + lineNumber + "." + Environment.NewLine +
                    "Received: " + (nameParts.Length - 1) + Environment.NewLine + "Expected: " + paramLength, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                id = null;
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
            if (ScriptFile.numFormatSpecifier == NumberStyles.HexNumber) {
                formatOverride = "X";
                prefix = "0x";
            } else { //(ScriptFile.numFormatSpecifier == NumberStyles.Integer)
                formatOverride = "D";
                prefix = "";
            }

            switch (paramType) {
                case ParamTypeEnum.CMD_NUMBER:
                    return "CMD_" + prefix + num.ToString(formatOverride + '3');
                case ParamTypeEnum.OW_ID:
                    if (ScriptDatabase.specialOverworlds.TryGetValue((ushort)num, out string output)) {
                        return output;
                    } else {
                        return "Overworld." + prefix + num.ToString(formatOverride);
                    }
                case ParamTypeEnum.FUNCTION_ID:
                    return containerTypes.Function.ToString() + "#" + num;
                case ParamTypeEnum.ACTION_ID:
                    return containerTypes.Action.ToString() + "#" + num;
                default:
                    if (ScriptFile.numFormatSpecifier == NumberStyles.None) {
                        if (num >= 4000) {
                            return "0x" + num.ToString("X");
                        }
                    }
                    return prefix + num.ToString(formatOverride);
            }
        }

        private string GetStringWithoutSpecialCharacters(string s) {
            foreach (char c in ScriptFile.specialChars) {
                int pos = s.IndexOf(c);
                if (pos >= 0) {
                    return s.Substring(pos + 1);
                }
            }
            return s;
        }

        public NumberStyles GetNumberStyleFromString(string s) {
            int posOfPrefix = s.IndexOf("0x", StringComparison.InvariantCultureIgnoreCase);
            if (posOfPrefix >= 0) {
                foreach (char c in s.Substring(posOfPrefix+2)) {
                    if (!Char.IsDigit(c) && Char.ToUpper(c) > 'F') {
                        return NumberStyles.None;
                    }
                }
                return NumberStyles.HexNumber;
            } else {
                foreach (char c in s) {
                    if (!Char.IsDigit(c)) {
                        return NumberStyles.None;
                    }
                }
                return NumberStyles.Integer;
            }
        }

        public override string ToString() {
            return name + " (" + ((ushort)id).ToString("X") + ")";
        }
        #endregion
    }
}
