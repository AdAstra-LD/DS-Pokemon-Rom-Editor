using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.IO;
using System.Linq;
using System.Collections;
using System.Windows.Forms;

namespace DSPRE {
    /// <summary>
    /// Class to store script file data in Pokémon NDS games
    /// </summary>
    public class ScriptFile {
        #region Fields (3)
        public List<Script> scripts = new List<Script>();
        public List<Script> functions = new List<Script>();
        public List<Script> movements = new List<Script>();
        public bool isLevelScript = new bool();
        #endregion

        #region Constructors (1)
        public ScriptFile(Stream fs, string gameVersion) {
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
                for (int i = 0; i < scriptOffsets.Count; i++) {
                    int duplicateIndex = scriptOffsets.FindIndex(offset => offset == scriptOffsets[i]); // Check for UseScript_#
                    if (duplicateIndex == i) {
                        scrReader.BaseStream.Position = scriptOffsets[i];

                        List<Command> commandsList = new List<Command>();
                        bool endScript = new bool();
                        while (!endScript) {
                            Command command = Read_Command(scrReader, ref functionOffsets, ref movementOffsets, gameVersion);
                            if (command == null) {
                                return;
                            } else {
                                commandsList.Add(command);

                                if (endCodes.Contains(command.id))
                                    endScript = true;
                            }
                        }
                        this.scripts.Add(new Script(commandsList));
                    } else 
                        scripts.Add(new Script(duplicateIndex));
                }

                /* Read functions */
                for (int i = 0; i < functionOffsets.Count; i++) {
                    scrReader.BaseStream.Position = functionOffsets[i];

                    List<Command> commandsList = new List<Command>();
                    bool endFunction = new bool();
                    while (!endFunction) {
                        Command command = Read_Command(scrReader, ref functionOffsets, ref movementOffsets, gameVersion);
                        commandsList.Add(command);
                        if (endCodes.Contains(command.id))
                            endFunction = true;
                    }

                    this.functions.Add(new Script(commandsList));
                }

                /* Read movements */
                for (int i = 0; i < movementOffsets.Count; i++) {
                    scrReader.BaseStream.Position = movementOffsets[i];

                    List<Command> commandsList = new List<Command>();
                    bool endMovement = new bool();
                    while (!endMovement) {
                        ushort id = scrReader.ReadUInt16();
                        List<byte[]> parameters = new List<byte[]>();
                        if (id != 0xFE)
                            parameters.Add(scrReader.ReadBytes(2));
                        else
                            endMovement = true;

                        Command command = new Command(id, parameters, true);
                        commandsList.Add(command);
                    }
                    this.movements.Add(new Script(commandsList));
                }
            }
        }
        public ScriptFile(int fileID) : this((new FileStream(RomInfo.scriptDirPath +
        "\\" + fileID.ToString("D4"), FileMode.Open)), RomInfo.gameVersion) {
        }
        public ScriptFile(List<Script> scripts, List<Script> functions, List<Script> movements) {
            this.scripts = scripts;
            this.functions = functions;
            this.movements = movements;
            isLevelScript = false;
        }
        #endregion

        #region Methods (1)
        private Command Read_Command(BinaryReader dataReader, ref List<uint> functionOffsets, ref List<uint> movementOffsets, string gameVersion) {
            ushort id = dataReader.ReadUInt16();
            List<byte[]> parameterList = new List<byte[]>();

            /* How to read parameters for different commands for DPPt*/
            switch (gameVersion) {
                case "D":
                case "P":
                case "Plat":
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
                                if (gameVersion == "Plat") {
                                    byte parameter1 = dataReader.ReadByte();
                                    parameterList.Add(new byte[] { parameter1 });

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
                                } else {
                                    goto default;
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
                                if (gameVersion == "Plat") {
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
                            if (gameVersion == "Plat")
                                break;
                            else
                                goto default;
                        case 0x2CF:
                            if (gameVersion == "Plat") {
                                parameterList.Add(dataReader.ReadBytes(2));
                                parameterList.Add(dataReader.ReadBytes(2));
                            } else {
                                goto default;
                            }
                            break;
                        default:
                            addParametersToList(parameterList, id, dataReader);
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
                            addParametersToList(parameterList, id, dataReader);
                            break;
                    }
                    break;
            }
            return new Command(id, parameterList, false);
        }
        private void addParametersToList(List<byte[]> parameterList, ushort id, BinaryReader dataReader) {
            Console.WriteLine("Loaded command id: " + id.ToString("X4"));
            try {
                foreach (int bytesToRead in RomInfo.scriptParametersDict[id])
                    parameterList.Add(dataReader.ReadBytes(bytesToRead));
            } catch (NullReferenceException) {
                MessageBox.Show("Script command " + id + "can't be handled for now." +
                    Environment.NewLine + "Reference offset 0x" + dataReader.BaseStream.Position.ToString("X"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                parameterList = null;
            } catch {
                MessageBox.Show("Error: ID Read - " + id +
                    Environment.NewLine + "Reference offset 0x" + dataReader.BaseStream.Position.ToString("X"), "Unrecognized script command", MessageBoxButtons.OK, MessageBoxIcon.Error);
                parameterList = null;
            }
        }
        public byte[] ToByteArray() {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData)) {
                List<uint> scriptOffsets = new List<uint>();
                List<uint> functionOffsets = new List<uint>();
                List<uint> movementOffsets = new List<uint>();

                List<Tuple<int, int, int>> references = new List<Tuple<int, int, int>>(); // Format: [address, function/movement #, type]
                int[] referenceCodes = new int[] { 0x16, 0x1A, 0x1C, 0x1D, 0x5E };

                /* Allocate enough space for script pointers, which we do not know yet */
                writer.BaseStream.Position += scripts.Count * 0x4;
                writer.Write((ushort)0xFD13); // Signal the end of header section

                /* Write scripts */
                for (int i = 0; i < scripts.Count; i++) {
                    if (scripts[i].useScript == -1) {
                        scriptOffsets.Add((uint)writer.BaseStream.Position);

                        for (int j = 0; j < scripts[i].commands.Count; j++) {
                            /* Get command id */
                            ushort id = scripts[i].commands[j].id;
                            /* Write ID and Parameters*/
                            writer.Write(id);

                            /* Get command parameters */
                            List<byte[]> parameterList = scripts[i].commands[j].parameterList;
                            for (int k = 0; k < parameterList.Count; k++)
                                writer.Write(parameterList[k]);

                            Console.Write("\nCommand added: " + scripts[i].commands[j]);

                            /* If command calls a function/movement, store reference position */
                            if (referenceCodes.Contains(id)) {
                                int index;
                                if (id == 0x16 || id == 0x1A)
                                    index = 0; // Jump, Call
                                else
                                    index = 1;

                                int type = 0;
                                if (id == 0x5E)
                                    type = 1; // ApplyMovement
                                references.Add(new Tuple<int, int, int>((int)writer.BaseStream.Position - 4, BitConverter.ToInt32(parameterList[index], 0), type));
                            }
                        }
                    } else {
                        scriptOffsets.Add(scriptOffsets[scripts[i].useScript]);  // If script has UseScript, copy offset
                    }
                }

                /* Write functions */
                for (int i = 0; i < functions.Count; i++) {
                    functionOffsets.Add((uint)writer.BaseStream.Position);

                    for (int j = 0; j < functions[i].commands.Count; j++) {
                        /* Write command id */
                        ushort id = functions[i].commands[j].id;
                        writer.Write(id);

                        /* Write command parameters */
                        List<byte[]> parameterList = functions[i].commands[j].parameterList;
                        for (int k = 0; k < parameterList.Count; k++) 
                            writer.Write(parameterList[k]);

                        /* If command calls a function/movement, store reference position */
                        if (referenceCodes.Contains(id)) {
                            int index;
                            if (id == 0x16 || id == 0x1A) 
                                index = 0;
                            else 
                                index = 1;

                            int type = 0;
                            if (id == 0x5E) 
                                type = 1;
                            references.Add(new Tuple<int, int, int>((int)writer.BaseStream.Position - 4, BitConverter.ToInt32(parameterList[index], 0), type));
                        }
                    }
                }

                // Movements must be halfword-aligned
                if (writer.BaseStream.Position % 2 == 1) { //Check if the writer's head is on an odd byte
                    writer.Write((byte)0); //Add padding
                }

                /* Write movements */
                for (int i = 0; i < movements.Count; i++) {
                    movementOffsets.Add((uint)writer.BaseStream.Position);

                    for (int j = 0; j < movements[i].commands.Count; j++) {
                        /* Write movement command id */
                        ushort id = movements[i].commands[j].id;
                        writer.Write(id);

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
                for (int i = 0; i < references.Count; i++) {
                    writer.BaseStream.Position = references[i].Item1;

                    if (references[i].Item3 == 1) 
                        writer.Write((UInt32)(movementOffsets[references[i].Item2] - references[i].Item1 - 4));
                    else 
                        writer.Write((UInt32)(functionOffsets[references[i].Item2] - references[i].Item1 - 4));
                }
            }

            return newData.ToArray();
        }

        public void SaveToFile(int fileID) {
            using (BinaryWriter writer = new BinaryWriter((new FileStream(RomInfo.scriptDirPath +
                "\\" + fileID.ToString("D4"), FileMode.Create))))
                writer.Write(this.ToByteArray());
        }
        #endregion
    }
    public class Script {
        #region Fields (1)
        public List<Command> commands;
        public int useScript = -1;
        #endregion Fields

        #region Constructors (2)
        public Script(List<Command> commandsList) {
            commands = commandsList;
        }
        public Script(int useScript) {
            this.useScript = useScript;
        }
        #endregion
    }
    public class Command {
        #region Fields (4)
        public ushort id;
        public List<byte[]> parameterList;
        public string cmdName;
        public bool isMovement;
        #endregion

        #region Constructors (2)
        public Command(ushort id, List<byte[]> parameterList, bool isMovement) {
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
                        string owToMove = BitConverter.ToUInt16(parameterList[0], 0).ToString("D");
                        switch (owToMove) {
                            case "255":
                                owToMove = "Player";
                                break;
                            case "253":
                                owToMove = "Following";
                                break;
                            case "241":
                                owToMove = "Cam";
                                break;
                            default:
                                owToMove = "Overworld_#" + owToMove;
                                break;
                        }
                        this.cmdName += " " + owToMove + " " + "Movement_#" + (1 + (BitConverter.ToInt32(parameterList[1], 0))).ToString("D");
                        break;
                    case 0x62:      // Lock
                    case 0x63:      // Release
                    case 0x64:      // AddPeople
                    case 0x65:      // RemoveOW
                        owToMove = BitConverter.ToUInt16(parameterList[0], 0).ToString("D");
                        switch (owToMove) {
                            case "255":
                                owToMove = "Player";
                                break;
                            case "253":
                                owToMove = "Following";
                                break;
                            case "241":
                                owToMove = "Cam";
                                break;
                            default:
                                owToMove = "Overworld_#" + owToMove;
                                break;
                        }
                        this.cmdName += " " + owToMove;
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
        public Command(string description, bool isMovement = false) {
            this.cmdName = description;
            this.isMovement = isMovement;
            this.parameterList = new List<byte[]>();

            string[] words = description.Split(' '); // Separate command code from parameters
            /* Get command id, which is always first in the description */

            if (isMovement) {
                id = PokeDatabase.ScriptEditor.movementsDictIDName.First(x => x.Value == words[0]).Key;
            } else {
                id = RomInfo.scriptCommandNamesDict.First(x => x.Value == words[0]).Key;
            }

            if (id == null) {
                UInt16.TryParse(words[0], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out this.id);
            }

            /* Read parameters from remainder of the description */
            Console.WriteLine("ID = " + id.ToString("X4"));
            if (words.Length > 1) {
                if (isMovement) {
                    if (words[1].Length > 4) { // Cases where movement is followed by an Overworld parameter

                        int positionOfOverworldID = 1 + words[1].IndexOf('#'); // Find position of #
                        parameterList.Add(BitConverter.GetBytes(Int32.Parse(words[1].Substring(positionOfOverworldID), NumberStyles.Integer))); // Add Overworld_#
                        parameterList[0] = BitConverter.GetBytes(BitConverter.ToInt32(parameterList[0], 0) - 1); // Add Overworld number

                        // TODO: Check if other cases may apply to movement parameters
                    } else {
                        parameterList.Add(BitConverter.GetBytes(Int16.Parse(words[1].Substring(2), NumberStyles.HexNumber)));
                    }
                } else {
                    byte[] parametersArr = RomInfo.scriptParametersDict[id];
                    for (int i = 1; i < parametersArr.Length; i++) {
                        Console.WriteLine("Parameter #" + i.ToString() + ": " + words[i]);
                        try {
                            ushort comparisonOperator = PokeDatabase.ScriptEditor.comparisonOperators.First(x => x.Value == words[i]).Key;
                            parameterList.Add(new byte[] { (byte)comparisonOperator });
                        }  catch (KeyNotFoundException) {
                            int indexOfSpecialCharacter = 1 + words[i].IndexOfAny(new char[] { 'x', '#' });

                            /* If number is preceded by 0x parse it as hex, otherwise as decimal */
                            NumberStyles style;
                            if (words[i].Contains("0x"))
                                style = NumberStyles.HexNumber;
                            else
                                style = NumberStyles.Integer;

                            /* Convert strings of parameters to the correct datatypes */
                            switch (parametersArr[i]) {
                                case 1:
                                    parameterList.Add(new byte[] { Byte.Parse(words[i].Substring(indexOfSpecialCharacter), style) });
                                    break;
                                case 2:
                                    switch (words[i]) {
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
                                            parameterList.Add(BitConverter.GetBytes(Int16.Parse(words[i].Substring(indexOfSpecialCharacter), style)));
                                            break;
                                    }
                                    break;
                                case 4:
                                    parameterList.Add(BitConverter.GetBytes(Int32.Parse(words[i].Substring(indexOfSpecialCharacter), style)));
                                    break;
                            }
                        }
                    }

                    /* Fix function and movement references which are +1 greater than array indexes */
                    Console.WriteLine("before fix Param length = " + parameterList.Count.ToString());
                    if (id == 0x16 || id == 0x1A)
                        parameterList[0] = BitConverter.GetBytes(BitConverter.ToInt32(parameterList[0], 0) - 1);
                    if (id == 0x1C || id == 0x1D || id == 0x5E)
                        parameterList[1] = BitConverter.GetBytes(BitConverter.ToInt32(parameterList[1], 0) - 1);
                }
            }
        }
        #endregion

        #region Utilities

        public override string ToString() {
            return cmdName + " (" + id.ToString("X") + ")";
        }
        #endregion
    }
}