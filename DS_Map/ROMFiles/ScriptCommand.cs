using DSPRE.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using static DSPRE.ROMFiles.ScriptFile;

namespace DSPRE.ROMFiles {
    public class CommandContainer {
        public List<ScriptCommand> commands;
        public uint manualUserID;
        public int useScript;
        public containerTypes containerType;
        internal static readonly string functionStart;

        #region Constructors (2)
        public CommandContainer(uint scriptNumber, containerTypes containerType, int useScript = -1, List<ScriptCommand> commandList = null) {
            manualUserID = scriptNumber;
            this.useScript = useScript;
            this.containerType = containerType;
            commands = commandList;
        }
        public CommandContainer(uint newID, CommandContainer toCopy) {
            manualUserID = newID;
            useScript = toCopy.useScript;
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

            this.id = id;
            this.cmdParams = parametersList;

            if (!RomInfo.ScriptCommandNamesDict.TryGetValue(id, out name)) {
                name = id.ToString("X4");
            }

            switch (id) {
                case 0x16:      // Jump
                case 0x1A:      // Call
                    name += " " + FunctionKW + "#" + BitConverter.ToInt32(parametersList[0], 0).ToString("D");
                    break;
                case 0x17:      // JumpIfObjID
                case 0x18:      // JumpIfEventID
                    byte owid = parametersList[0][0];
                    name += " " + ScriptFile.OverworldFlexDecode(owid);
                    name += " " + FunctionKW + "#" + BitConverter.ToInt32(parametersList[1], 0).ToString("D");
                    break;
                case 0x19:      // JumpIfPlayerDir
                    byte param = parametersList[0][0];
                    name += " " + param.ToString("X") + " " + FunctionKW + "#" + BitConverter.ToInt32(parametersList[1], 0).ToString("D");
                    break;
                case 0x1C:      // JumpIf
                case 0x1D:      // CallIf
                    byte opcode = parametersList[0][0];
                    name += " " + RomInfo.ScriptComparisonOperatorsDict[opcode] + " " + FunctionKW + "#" + BitConverter.ToInt32(parametersList[1], 0).ToString("D");
                    break;
                case 0x5E:      // Movement
                    ushort flexID = BitConverter.ToUInt16(parametersList[0], 0);
                    name += " " + ScriptFile.OverworldFlexDecode(flexID);
                    name += " " + ActionKW + "#" + BitConverter.ToInt32(parametersList[1], 0).ToString("D");
                    break;
                case 0x6A:      // CheckOverworldPosition
                    flexID = BitConverter.ToUInt16(parametersList[0], 0);
                    name += " " + ScriptFile.OverworldFlexDecode(flexID) + " " + "0x" + BitConverter.ToInt16(parametersList[1], 0).ToString("X") + " " + "0x" + BitConverter.ToInt16(parametersList[2], 0).ToString("X");
                    break;
                case 0x62:      // Lock
                case 0x63:      // Release
                case 0x64:      // AddOW
                case 0x65:      // RemoveOW
                    flexID = BitConverter.ToUInt16(parametersList[0], 0);
                    name += " " + ScriptFile.OverworldFlexDecode(flexID);
                    break;
                default:
                    for (int i = 0; i < parametersList.Count; i++) {
                        if (parametersList[i].Length == 1)
                            this.name += " " + "0x" + (parametersList[i][0]).ToString("X1");
                        else if (parametersList[i].Length == 2)
                            this.name += " " + "0x" + BitConverter.ToInt16(parametersList[i], 0).ToString("X1");
                        else if (parametersList[i].Length == 4)
                            this.name += " " + "0x" + BitConverter.ToInt32(parametersList[i], 0).ToString("X1");
                    }
                    break;

            }
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
                    id = ushort.Parse(nameParts[0], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
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
        private string GetStringWithoutSpecialCharacters(string s) {
            int indexOfSpecialCharacter = s.IndexOfAny(new char[] { 'x', 'X', '#', '.' });
            return s.Substring(indexOfSpecialCharacter + 1);
        }

        public NumberStyles GetNumberStyleFromString(string s) {
            if (s.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase)) {
                foreach (char c in s.Substring(2)) {
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
