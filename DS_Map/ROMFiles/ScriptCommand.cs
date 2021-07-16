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

        #region Constructors (2)
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
        #region Fields (4)
        public ushort? id;
        public List<byte[]> cmdParams;
        public string name;
        #endregion

        #region Constructors (2)
        public ScriptCommand(ushort id, List<byte[]> commandParameters) {
            if (commandParameters is null) {
                this.id = null;
                return;
            }

            this.id = id;
            this.cmdParams = commandParameters;

            if (!RomInfo.ScriptCommandNamesDict.TryGetValue(id, out name))
                name = id.ToString("X4");

            switch (id) {
                case 0x16:      // Jump
                case 0x1A:      // Call
                    name += " " + "Function_#" + (BitConverter.ToInt32(commandParameters[0], 0)).ToString("D");
                    break;
                case 0x17:      // JumpIfObjID
                case 0x18:      // JumpIfBgID
                case 0x19:      // JumpIfPlayerDir
                    byte param = commandParameters[0][0];
                    name += " " + param.ToString("X") + " " + "Function_#" + BitConverter.ToInt32(commandParameters[1], 0).ToString("D");
                    break;
                case 0x1C:      // Jump-If
                case 0x1D:      // Call-If
                    byte opcode = commandParameters[0][0];
                    name += " " + PokeDatabase.ScriptEditor.comparisonOperatorsDict[opcode] + " " + "Function_#" + BitConverter.ToInt32(commandParameters[1], 0).ToString("D");
                    break;
                case 0x5E:      // ApplyMovement
                    ushort flexID = BitConverter.ToUInt16(commandParameters[0], 0);
                    this.name += ScriptFile.OverworldFlexDecode(flexID);
                    name += " " + "Action_#" + BitConverter.ToInt32(commandParameters[1], 0).ToString("D");
                    break;
                case 0x62:      // Lock
                case 0x63:      // Release
                case 0x64:      // AddPeople
                case 0x65:      // RemoveOW
                    flexID = BitConverter.ToUInt16(commandParameters[0], 0);
                    name += ScriptFile.OverworldFlexDecode(flexID);
                    break;
                default:
                    for (int i = 0; i < commandParameters.Count; i++) {
                        if (commandParameters[i].Length == 1)
                            this.name += " " + "0x" + (commandParameters[i][0]).ToString("X1");
                        else if (commandParameters[i].Length == 2)
                            this.name += " " + "0x" + (BitConverter.ToInt16(commandParameters[i], 0)).ToString("X1");
                        else if (commandParameters[i].Length == 4)
                            this.name += " " + "0x" + (BitConverter.ToInt32(commandParameters[i], 0)).ToString("X1");
                    }
                    break;

            }
        }
        public ScriptCommand(string wholeLine, int lineNumber = 0) {
            name = wholeLine;
            cmdParams = new List<byte[]>();

            string[] nameParts = wholeLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries); // Separate command code from parameters
            /* Get command id, which is always first in the description */

            try {
                id = RomInfo.ScriptCommandNamesDict.First(x => x.Value.Equals(nameParts[0], StringComparison.InvariantCultureIgnoreCase)).Key;
            } catch (InvalidOperationException) {

                try {
                    id = ushort.Parse(nameParts[0], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                } catch {
                    string details;
                    if (wholeLine.Contains('@') && wholeLine.Contains('#')) {
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

            byte[] parametersSizeArr = RomInfo.CommandParametersDict[(ushort)id];

            int paramLength = 0;
            if (parametersSizeArr.Length == 1 && parametersSizeArr.First() == 0) {
                paramLength = 0;
            } else {
                paramLength = parametersSizeArr.Length;
            }

            if (nameParts.Length - 1 == paramLength) {
                for (int i = 0; i < paramLength; i++) {
                    Console.WriteLine("Parameter #" + i.ToString() + ": " + nameParts[i + 1]);
                    try {
                        ushort comparisonOperator = PokeDatabase.ScriptEditor.comparisonOperatorsDict.First(x => x.Value.Equals(nameParts[i + 1], StringComparison.InvariantCultureIgnoreCase)).Key;
                        cmdParams.Add(new byte[] { (byte)comparisonOperator });
                    } catch { //Not a comparison
                        int indexOfSpecialCharacter = nameParts[i + 1].IndexOfAny(new char[] { 'x', 'X', '#' });

                        /* If number is preceded by 0x parse it as hex, otherwise as decimal */
                        NumberStyles style;
                        if (nameParts[i + 1].StartsWith("0x", StringComparison.InvariantCultureIgnoreCase)) {
                            style = NumberStyles.HexNumber;
                        } else {
                            style = NumberStyles.Integer;
                        }

                        nameParts[i + 1] = nameParts[i + 1].Substring(indexOfSpecialCharacter + 1);
                        /* Convert strings of parameters to the correct datatypes */
                        try {
                            switch (parametersSizeArr[i]) {
                                case 1:
                                    cmdParams.Add(new byte[] { Byte.Parse(nameParts[i + 1], style) });
                                    break;
                                case 2:
                                    if (nameParts[i + 1].Equals("Player", StringComparison.InvariantCultureIgnoreCase)) {
                                        cmdParams.Add(BitConverter.GetBytes((ushort)255));
                                    } else if (nameParts[i + 1].Equals("Following", StringComparison.InvariantCultureIgnoreCase)) {
                                        cmdParams.Add(BitConverter.GetBytes((ushort)253));
                                    } else if (nameParts[i + 1].Equals("Camera", StringComparison.InvariantCultureIgnoreCase)) {
                                        cmdParams.Add(BitConverter.GetBytes((ushort)241));
                                    } else {
                                        cmdParams.Add(BitConverter.GetBytes(ushort.Parse(nameParts[i + 1], style)));
                                    }
                                    break;
                                case 4:
                                    cmdParams.Add(BitConverter.GetBytes(Int32.Parse(nameParts[i + 1], style)));
                                    break;
                            }
                        } catch (FormatException) {
                            MessageBox.Show("Argument " + '"' + nameParts[i + 1] + '"' + " at line " + lineNumber + " is not a valid " + style , "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            id = null;
                        } catch (OverflowException) {
                            MessageBox.Show("Argument " + '"' + nameParts[i + 1] + '"' + " at line " + lineNumber + " is not in the range [" + 0 + ", " + (Math.Pow(2, 8 * parametersSizeArr[i]) - 1) + "].", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            id = null;
                        }
                    }
                }
            } else {
                MessageBox.Show("Wrong number of parameters for command " + nameParts[0] + " at line " + lineNumber + "." + Environment.NewLine +
                    "Received: " + (nameParts.Length - 1) + Environment.NewLine + "Expected: " + paramLength, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                id = null;
            }
        }
        #endregion

        #region Utilities
        public override string ToString() {
            return name + " (" + ((ushort)id).ToString("X") + ")";
        }
        #endregion
    }
}
