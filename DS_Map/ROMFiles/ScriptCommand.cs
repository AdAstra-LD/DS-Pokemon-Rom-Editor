using DSPRE.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DSPRE.ROMFiles {
    public class Script {
        public List<ScriptCommand> commands;
        public int useScript;

        #region Constructors (2)
        public Script(int useScript = -1, List<ScriptCommand> commandList = null) {
            commands = commandList;
            this.useScript = useScript;
        }
        #endregion
    }
    public class ScriptCommand {
        #region Fields (4)
        public ushort id;
        public List<byte[]> parameterList;
        public string cmdName;
        public bool isMovement;
        #endregion

        #region Constructors (2)
        public ScriptCommand(ushort id, List<byte[]> parameterList, bool isMovement = false) {
            this.id = id;
            this.isMovement = isMovement;
            this.parameterList = parameterList;

            Dictionary<ushort, string> commandNamesDatabase;
            if (isMovement) {
                commandNamesDatabase = PokeDatabase.ScriptEditor.movementsDictIDName;
            } else {
                commandNamesDatabase = RomInfo.scriptCommandNamesDict;
            }

            try {
                cmdName = commandNamesDatabase[id];
            } catch (KeyNotFoundException) {
                cmdName = id.ToString("X4");
            }

            if (isMovement) {
                for (int i = 0; i < parameterList.Count; i++) {
                    if (parameterList[i].Length == 1)
                        this.cmdName += " " + "0x" + (parameterList[i][0]).ToString("X1");
                    else if (parameterList[i].Length == 2)
                        this.cmdName += " " + "0x" + (BitConverter.ToInt16(parameterList[i], 0)).ToString("X1");
                    else if (parameterList[i].Length == 4)
                        this.cmdName += " " + "0x" + (BitConverter.ToInt32(parameterList[i], 0)).ToString("X1");
                }
            } else {
                switch (id) {
                    case 0x16:      // Jump
                    case 0x1A:      // Call
                        this.cmdName += " " + "Function_#" + (1 + BitConverter.ToInt32(parameterList[0], 0)).ToString("D");
                        break;
                    case 0x17:      // JumpIfObjID
                    case 0x18:      // JumpIfBgID
                    case 0x19:      // JumpIfPlayerDir
                        this.cmdName += " " + (BitConverter.ToInt32(parameterList[0], 0)).ToString("D") + " " + "Function_#" + (1 + (BitConverter.ToInt32(parameterList[1], 0))).ToString("D");
                        break;
                    case 0x1C:      // CompareLastResultJump
                    case 0x1D:      // CompareLastResultCall
                        byte opcode = parameterList[0][0];
                        this.cmdName += " " + PokeDatabase.ScriptEditor.comparisonOperators[opcode] + " " + "Function_#" + (1 + (BitConverter.ToInt32(parameterList[1], 0))).ToString("D");
                        break;
                    case 0x5E:      // ApplyMovement
                        ushort flexID = BitConverter.ToUInt16(parameterList[0], 0);
                        this.cmdName += OverworldFlexDecode(flexID);
                        this.cmdName += " " + "Movement_#" + (1 + (BitConverter.ToInt32(parameterList[1], 0))).ToString("D");
                        break;
                    case 0x62:      // Lock
                    case 0x63:      // Release
                    case 0x64:      // AddPeople
                    case 0x65:      // RemoveOW
                        flexID = BitConverter.ToUInt16(parameterList[0], 0);
                        cmdName += OverworldFlexDecode(flexID);
                        break;
                    default:
                        for (int i = 0; i < parameterList.Count; i++) {
                            if (parameterList[i].Length == 1)
                                this.cmdName += " " + "0x" + (parameterList[i][0]).ToString("X1");
                            else if (parameterList[i].Length == 2)
                                this.cmdName += " " + "0x" + (BitConverter.ToInt16(parameterList[i], 0)).ToString("X1");
                            else if (parameterList[i].Length == 4)
                                this.cmdName += " " + "0x" + (BitConverter.ToInt32(parameterList[i], 0)).ToString("X1");
                        }
                        break;
                }
            }
        }
        public ScriptCommand(string wholeLine, bool isMovement = false) {
            this.cmdName = wholeLine;
            this.isMovement = isMovement;
            this.parameterList = new List<byte[]>();

            string[] nameParts = wholeLine.Split(' '); // Separate command code from parameters
            /* Get command id, which is always first in the description */

            try {
                if (isMovement) {
                    id = PokeDatabase.ScriptEditor.movementsDictIDName.First(x => x.Value == nameParts[0]).Key;
                } else {
                    id = RomInfo.scriptCommandNamesDict.First(x => x.Value == nameParts[0]).Key;
                }
            } catch (InvalidOperationException) {
                UInt16.TryParse(nameParts[0], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out this.id);
            }

            /* Read parameters from remainder of the description */
            Console.WriteLine("ID = " + id.ToString("X4"));
            if (nameParts.Length > 1) {
                if (isMovement) {
                    if (nameParts[1].Length > 4) { // Cases where movement is followed by an Overworld parameter

                        int positionOfOverworldID = nameParts[1].IndexOf('#'); // Find position of #
                        parameterList.Add(BitConverter.GetBytes(Int32.Parse(nameParts[1].Substring(positionOfOverworldID + 1), NumberStyles.Integer))); // Add Overworld_#
                        parameterList[0] = BitConverter.GetBytes(BitConverter.ToInt32(parameterList[0], 0) - 1); // Add Overworld number

                        // TODO: Check if other cases may apply to movement parameters
                    } else {
                        parameterList.Add(BitConverter.GetBytes(Int16.Parse(nameParts[1].Substring(2), NumberStyles.HexNumber)));
                    }
                } else {
                    byte[] parametersArr = RomInfo.scriptParametersDict[id];
                    for (int i = 0; i < parametersArr.Length; i++) {
                        Console.WriteLine("Parameter #" + i.ToString() + ": " + nameParts[i + 1]);
                        try {
                            ushort comparisonOperator = PokeDatabase.ScriptEditor.comparisonOperators.First(x => x.Value == nameParts[i + 1]).Key;
                            parameterList.Add(new byte[] { (byte)comparisonOperator });
                        } catch { //Not a comparison
                            int indexOfSpecialCharacter = nameParts[i + 1].IndexOfAny(new char[] { 'x', '#' });

                            /* If number is preceded by 0x parse it as hex, otherwise as decimal */
                            NumberStyles style;
                            if (nameParts[i + 1].Contains("0x"))
                                style = NumberStyles.HexNumber;
                            else
                                style = NumberStyles.Integer;

                            /* Convert strings of parameters to the correct datatypes */
                            switch (parametersArr[i]) {
                                case 1:
                                    parameterList.Add(new byte[] { Byte.Parse(nameParts[i + 1].Substring(indexOfSpecialCharacter + 1), style) });
                                    break;
                                case 2:
                                    switch (nameParts[i + 1]) {
                                        case "Player":
                                            parameterList.Add(BitConverter.GetBytes((ushort)255));
                                            break;
                                        case "Following":
                                            parameterList.Add(BitConverter.GetBytes((ushort)253));
                                            break;
                                        case "Cam":
                                            parameterList.Add(BitConverter.GetBytes((ushort)241));
                                            break;
                                        default:
                                            parameterList.Add(BitConverter.GetBytes(Int16.Parse(nameParts[i + 1].Substring(indexOfSpecialCharacter + 1), style)));
                                            break;
                                    }
                                    break;
                                case 4:
                                    parameterList.Add(BitConverter.GetBytes(Int32.Parse(nameParts[i + 1].Substring(indexOfSpecialCharacter + 1), style)));
                                    break;
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Utilities
        private string OverworldFlexDecode(ushort flexID) {
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
        public override string ToString() {
            return cmdName + " (" + id.ToString("X") + ")";
        }
        #endregion
    }
}
