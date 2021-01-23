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
                try {
                    while (scrReader.ReadUInt16() != 0xFD13) {
                        scrReader.BaseStream.Position -= 0x2;
                        uint value = scrReader.ReadUInt32();

                        if (value == 0) {
                            isLevelScript = true;
                            return;
                        } else {
                            uint offsetFromStart = value + (uint)scrReader.BaseStream.Position;
                            scriptOffsets.Add(offsetFromStart); // Don't change order of addition
                        }
                    }
                } catch (EndOfStreamException) {
                    MessageBox.Show("Script File couldn't be read correctly.", "Unexpected EOF", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                /* Read scripts */
                for (int i = 0; i < scriptOffsets.Count; i++) {
                    int duplicateIndex = scriptOffsets.FindIndex(offset => offset == scriptOffsets[i]); // Check for UseScript_#
                    if (duplicateIndex != i)
                        scripts.Add(new Script(duplicateIndex));
                    else {
                        scrReader.BaseStream.Position = scriptOffsets[i];

                        List<Command> commandsList = new List<Command>();
                        bool endScript = new bool();
                        while (!endScript) {
                            Command command = Read_Command(scrReader, ref functionOffsets, ref movementOffsets, gameVersion);
                            commandsList.Add(command);

                            if (endCodes.Contains(command.id)) 
                                endScript = true;
                        }
                        this.scripts.Add(new Script(commandsList));
                    }
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
                        Command command = new Command(id, parameters, gameVersion, true);

                        commandsList.Add(command);
                        if (command.id == 0xFE) 
                            endMovement = true;

                    }
                    this.movements.Add(new Script(commandsList));
                }
            }
        }
        #endregion

        #region Methods (1)
        private Command Read_Command(BinaryReader dataReader, ref List<uint> functionOffsets, ref List<uint> movementOffsets, string gameVersion) {
            ResourceManager paramDatabase;
            switch (gameVersion) {
                case "D":
                case "P":
                case "Plat":
                    paramDatabase = new ResourceManager("DSPRE.Resources.ScriptParametersDP", Assembly.GetExecutingAssembly());
                    break;
                default:
                    paramDatabase = new ResourceManager("DSPRE.Resources.ScriptParametersHGSS", Assembly.GetExecutingAssembly());
                    break;
            }

            ushort id = dataReader.ReadUInt16();
            List<byte[]> parameters = new List<byte[]>();

            /* How to read parameters for different commands */
            switch (id) {
                case 0x16: //Jump
                case 0x1A: //Call
                    uint offset = dataReader.ReadUInt32() + (uint)dataReader.BaseStream.Position; // Do not change order of addition
                    if (!functionOffsets.Contains(offset))
                        functionOffsets.Add(offset);

                    parameters.Add(BitConverter.GetBytes(functionOffsets.IndexOf(offset)));
                    break;
                case 0x1C: //CompareLastResultJump
                case 0x1D: //CompareLastResultCall
                    byte opcode = dataReader.ReadByte();
                    offset = dataReader.ReadUInt32() + (uint)dataReader.BaseStream.Position; // Do not change order of addition
                    if (!functionOffsets.Contains(offset))
                        functionOffsets.Add(offset);

                    parameters.Add(new byte[] { opcode });
                    parameters.Add(BitConverter.GetBytes(functionOffsets.IndexOf(offset)));
                    break;
                case 0x5E: // ApplyMovement
                case 0x2A1: // ApplyMovement2
                    {
                        ushort overworld = dataReader.ReadUInt16();
                        offset = dataReader.ReadUInt32() + (uint)dataReader.BaseStream.Position; // Do not change order of addition
                        if (!movementOffsets.Contains(offset))
                            movementOffsets.Add(offset);

                        parameters.Add(BitConverter.GetBytes(overworld));
                        parameters.Add(BitConverter.GetBytes(movementOffsets.IndexOf(offset)));
                    }
                    break;
                case 0x190: {
                        if (gameVersion == "D" || gameVersion == "P" || gameVersion == "Plat")
                            goto default;
                        else {
                            byte parameter1 = dataReader.ReadByte();
                            parameters.Add(new byte[] { parameter1 });
                            if (parameter1 == 0x2)
                                parameters.Add(dataReader.ReadBytes(2));
                        }
                    }
                    break;
                case 0x1CF: {
                        byte parameter1 = dataReader.ReadByte();
                        parameters.Add(new byte[] { parameter1 });
                        if (parameter1 == 0x2)
                            parameters.Add(dataReader.ReadBytes(2));
                    }
                    break;
                case 0x1D1: {
                        if (gameVersion == "D" || gameVersion == "P" || gameVersion == "Plat")
                            goto default;
                        else {
                            short parameter1 = dataReader.ReadInt16();
                            parameters.Add(BitConverter.GetBytes(parameter1));
                            switch (parameter1) {
                                case 0x0:
                                case 0x1:
                                case 0x2:
                                case 0x3:
                                    parameters.Add(dataReader.ReadBytes(2));
                                    parameters.Add(dataReader.ReadBytes(2));
                                    break;
                                case 0x4:
                                case 0x5:
                                case 0x6:
                                case 0x7:
                                    parameters.Add(dataReader.ReadBytes(2));
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    break;
                case 0x1E9: {
                        if (gameVersion == "D" || gameVersion == "P" || gameVersion == "Plat")
                            goto default;
                        else {
                            short parameter1 = dataReader.ReadInt16();
                            parameters.Add(BitConverter.GetBytes(parameter1));

                            switch (parameter1) {
                                case 0x1:
                                case 0x2:
                                case 0x3:
                                case 0x7:
                                    parameters.Add(dataReader.ReadBytes(2));
                                    break;
                                case 0x5:
                                case 0x6:
                                    parameters.Add(dataReader.ReadBytes(2));
                                    parameters.Add(dataReader.ReadBytes(2));
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    break;
                case 0x21D: {
                        if (gameVersion == "Plat") {
                            byte parameter1 = dataReader.ReadByte();
                            parameters.Add(new byte[] { parameter1 });

                            if (parameter1 != 0x6) {
                                parameters.Add(dataReader.ReadBytes(2));
                                if (parameter1 != 0x5)
                                    parameters.Add(dataReader.ReadBytes(2));
                            }
                        } else
                            goto default;
                    }
                    break;
                case 0x235: {
                        short parameter1 = dataReader.ReadInt16();
                        parameters.Add(BitConverter.GetBytes(parameter1));

                        switch (parameter1) {
                            case 0x1:
                            case 0x3:
                                parameters.Add(dataReader.ReadBytes(2));
                                parameters.Add(dataReader.ReadBytes(2));
                                parameters.Add(dataReader.ReadBytes(2));
                                break;
                            case 0x4:
                                parameters.Add(dataReader.ReadBytes(2));
                                parameters.Add(dataReader.ReadBytes(2));
                                break;
                            case 0x0:
                            case 0x6:
                                parameters.Add(dataReader.ReadBytes(2));
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case 0x23E: {
                        short parameter1 = dataReader.ReadInt16();
                        parameters.Add(BitConverter.GetBytes(parameter1));

                        switch (parameter1) {
                            case 0x1:
                            case 0x3:
                                parameters.Add(dataReader.ReadBytes(2));
                                break;
                            case 0x5:
                            case 0x6:
                                parameters.Add(dataReader.ReadBytes(2));
                                parameters.Add(dataReader.ReadBytes(2));
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case 0x2C4: {
                        byte parameter1 = dataReader.ReadByte();
                        parameters.Add(new byte[] { parameter1 });
                        if (parameter1 == 0 || parameter1 == 1)
                            parameters.Add(dataReader.ReadBytes(2));
                    }
                    break;
                case 0x2C5: {
                        if (gameVersion == "Plat") {
                            parameters.Add(dataReader.ReadBytes(2));
                            parameters.Add(dataReader.ReadBytes(2));
                        } else
                            goto default;
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
                        parameters.Add(dataReader.ReadBytes(2));
                        parameters.Add(dataReader.ReadBytes(2));
                    } else
                        goto default;
                    break;
                default:
                    Console.WriteLine("Loaded command id: " + id.ToString("X4"));
                    try {
                        string[] databaseResult = paramDatabase.GetString(id.ToString("X4")).Split(' ');
                        int numberOfParameters = Int32.Parse(databaseResult[0]);

                        for (int i = 1; i <= numberOfParameters; i++) {
                            int parameterSize = Int32.Parse(databaseResult[i]);
                            parameters.Add(dataReader.ReadBytes(parameterSize));
                        }
                        break;
                    } catch (NullReferenceException) {
                        MessageBox.Show("Unrecognized script command " + id, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }
            }

            return new Command(id, parameters, gameVersion, false);
        }
        public byte[] Save() {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData)) {
                List<uint> scriptOffsets = new List<uint>();
                List<uint> functionOffsets = new List<uint>();
                List<uint> movementOffsets = new List<uint>();

                List<Tuple<int, int, int>> references = new List<Tuple<int, int, int>>(); // Format: [address, function/movement #, type]
                int[] referenceCodes = new int[] { 0x16, 0x1A, 0x1C, 0x1D, 0x5E };

                /* Allocate enough space for script pointers, which we do not know yet */
                writer.BaseStream.Position += scripts.Count * 0x4;
                writer.Write((ushort)0xFD13); // End of header signal

                /* Write scripts */
                for (int i = 0; i < scripts.Count; i++) {
                    if (scripts[i].useScript != -1)
                        scriptOffsets.Add(scriptOffsets[scripts[i].useScript]);  // If script is a UseScript, copy offset
                    else {
                        scriptOffsets.Add((uint)writer.BaseStream.Position);

                        for (int j = 0; j < scripts[i].commands.Count; j++) {
                            /* Write command id */
                            ushort id = scripts[i].commands[j].id;
                            Console.WriteLine("Command added: " + scripts[i].commands[j].id + " with params " + String.Join(", ", scripts[i].commands[j].parameters));
                            writer.Write(id);

                            /* Write command parameters */
                            List<byte[]> parameters = scripts[i].commands[j].parameters;
                            for (int k = 0; k < parameters.Count; k++)
                                writer.Write(parameters[k]);

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
                                references.Add(new Tuple<int, int, int>((int)writer.BaseStream.Position - 4, BitConverter.ToInt32(parameters[index], 0), type));
                            }
                        }
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
                        List<byte[]> parameters = functions[i].commands[j].parameters;
                        for (int k = 0; k < parameters.Count; k++) writer.Write(parameters[k]);

                        /* If command calls a function/movement, store reference position */
                        if (referenceCodes.Contains(id)) {
                            int index;
                            if (id == 0x16 || id == 0x1A) index = 0;
                            else index = 1;

                            int type = 0;
                            if (id == 0x5E) type = 1;
                            references.Add(new Tuple<int, int, int>((int)writer.BaseStream.Position - 4, BitConverter.ToInt32(parameters[index], 0), type));
                        }
                    }
                }

                /* Write movements */
                for (int i = 0; i < movements.Count; i++) {
                    movementOffsets.Add((uint)writer.BaseStream.Position);

                    for (int j = 0; j < movements[i].commands.Count; j++) {
                        /* Write movement command id */
                        ushort id = movements[i].commands[j].id;
                        writer.Write(id);

                        /* Write movement command parameters */
                        List<byte[]> parameters = movements[i].commands[j].parameters;
                        for (int k = 0; k < parameters.Count; k++) writer.Write(parameters[k]);
                    }
                }

                /* Write script offsets to header */
                writer.BaseStream.Position = 0x0;
                for (int i = 0; i < scriptOffsets.Count; i++) writer.Write(scriptOffsets[i] - (uint)writer.BaseStream.Position - 0x4);

                /* Fix references to functions and movements */
                for (int i = 0; i < references.Count; i++) {
                    writer.BaseStream.Position = references[i].Item1;

                    if (references[i].Item3 == 1) writer.Write((UInt32)(movementOffsets[references[i].Item2] - references[i].Item1 - 4));
                    else writer.Write((UInt32)(functionOffsets[references[i].Item2] - references[i].Item1 - 4));
                }
            }

            return newData.ToArray();
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
        public List<byte[]> parameters;
        public string cmdName;
        public bool isMovement;
        #endregion

        #region Constructors (2)
        public Command(ushort id, List<byte[]> parameters, string gameVersion, bool isMovement) {
            ResourceManager commandNamesDatabase;
            if (isMovement) {
                commandNamesDatabase = new ResourceManager("DSPRE.Resources.MovementNames", Assembly.GetExecutingAssembly());
            } else {
                if (gameVersion == "D" || gameVersion == "P" || gameVersion == "Plat")
                    commandNamesDatabase = new ResourceManager("DSPRE.Resources.ScriptNamesDP", Assembly.GetExecutingAssembly());
                else
                    commandNamesDatabase = new ResourceManager("DSPRE.Resources.ScriptNamesHGSS", Assembly.GetExecutingAssembly());
            }

            this.id = id;
            this.isMovement = isMovement;
            this.parameters = parameters;
            this.cmdName = commandNamesDatabase.GetString(id.ToString("X4"));
            if (cmdName == null)
                cmdName = id.ToString("X4");

            for (int i = 0; i < parameters.Count; i++) {
                if (parameters[i].Length < 4) {
                    byte[] temp = new byte[4];
                    parameters[i].CopyTo(temp, 0);
                    parameters[i] = temp;
                }
            }

            switch (id) {
                case 0x16:      // Jump
                case 0x1A:      // Call
                    this.cmdName += " " + "Function_#" + (1 + BitConverter.ToInt32(parameters[0], 0)).ToString("D");
                    break;
                case 0x1C:      // CompareLastResultJump
                case 0x1D:      // CompareLastResultCall
                    byte opcode = parameters[0][0];
                    this.cmdName += " " + PokeDatabase.System.Scripts.byteToComparisonOperatorDict[opcode] + " " + "Function_#" + (1 + (BitConverter.ToInt32(parameters[1], 0))).ToString("D");
                    break;
                case 0x5E:      // ApplyMovement
                    this.cmdName += " " + "Overworld_#" + (BitConverter.ToInt16(parameters[0], 0)).ToString("D") + " " + "Movement_#" + (1 + (BitConverter.ToInt32(parameters[1], 0))).ToString("D");
                    break;
                case 0x62:      // Lock
                case 0x63:      // Release
                case 0x64:      // AddPeople
                case 0x65:      // RemoveOW
                    this.cmdName += " " + "Overworld_#" + BitConverter.ToInt16(parameters[0], 0).ToString("D");
                    break;
                default:
                    for (int i = 0; i < parameters.Count; i++) {
                        if (parameters[i].Length == 1) 
                            this.cmdName += " " + "0x" + (parameters[i][0]).ToString("X1");
                        else if (parameters[i].Length == 2) 
                            this.cmdName += " " + "0x" + (BitConverter.ToInt16(parameters[i], 0)).ToString("X1");
                        else if (parameters[i].Length == 4) 
                            this.cmdName += " " + "0x" + (BitConverter.ToInt32(parameters[i], 0)).ToString("X1");
                    }
                    break;
            }
        }

        private Object GetResxNameByValue(string value, ResourceManager rm) {
            var entry =
                rm.GetResourceSet(System.Threading.Thread.CurrentThread.CurrentCulture, true, true)
                  .OfType<DictionaryEntry>()
                  .FirstOrDefault(e => e.Value.ToString() == value);

            var key = entry.Key;
            return key;
        }

        public Command(string description, string gameVersion, bool isMovement) {
            this.cmdName = description;
            this.isMovement = isMovement;
            this.parameters = new List<byte[]>();

            string[] words = description.Split(' '); // Separate command code from parameters
            Console.WriteLine(String.Join(",", words));
            Console.WriteLine(gameVersion);
            /* Get command id, which is always first in the description */
            
            ResourceManager commandDatabase; // Load the resource file containing information on parameters for each command
            switch (gameVersion) {
                case "D":
                case "P":
                case "Plat":
                    commandDatabase = new ResourceManager("DSPRE.Resources.ScriptNamesDP", Assembly.GetExecutingAssembly());
                    break;
                default:
                    commandDatabase = new ResourceManager("DSPRE.Resources.ScriptNamesHGSS", Assembly.GetExecutingAssembly());
                    break;
            }

            if (isMovement) {
                if (PokeDatabase.System.Scripts.movementsDictDPPtHGSS.ContainsKey(words[0]))
                    this.id = PokeDatabase.System.Scripts.movementsDictDPPtHGSS[words[0]];
                else
                    UInt16.TryParse(words[0], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out this.id);
            } else {
                Console.WriteLine("Command name: " + words[0]);
                Object cmd = GetResxNameByValue(words[0], commandDatabase);

                if (cmd != null) {
                    Console.WriteLine(cmd);
                    this.id = ushort.Parse(cmd.ToString(), System.Globalization.NumberStyles.AllowHexSpecifier);
                } else {
                    UInt16.TryParse(words[0], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out this.id);
                }

            }

            /* Read parameters from remainder of the description */
            Console.WriteLine("ID = " + id.ToString("X4"));
            if (words.Length > 1 && this.id != 0) {
                if (isMovement) {
                    if (words[1].Length > 4) { // Cases where movement is followed by an Overworld parameter

                        int index = 1 + words[1].IndexOf('#'); // FInd index of #
                        parameters.Add(BitConverter.GetBytes(Int32.Parse(words[1].Substring(index), NumberStyles.Integer))); // Add Overworld_#
                        parameters[0] = BitConverter.GetBytes(BitConverter.ToInt32(parameters[0], 0) - 1); // Add Overworld number

                        // TODO: Check if other cases may apply to movement parameters
                    } else {
                        parameters.Add(BitConverter.GetBytes(Int16.Parse(words[1].Substring(2), NumberStyles.HexNumber)));
                    }
                } else {
                    ResourceManager paramDatabase; // Load the resource file containing information on parameters for each command
                    switch (gameVersion) {
                        case "D":
                        case "P":
                        case "Plat":
                            paramDatabase = new ResourceManager("DSPRE.Resources.ScriptParametersDP", Assembly.GetExecutingAssembly());
                            break;
                        default:
                            paramDatabase = new ResourceManager("DSPRE.Resources.ScriptParametersHGSS", Assembly.GetExecutingAssembly());
                            break;
                    }
                    string[] indices = paramDatabase.GetString(id.ToString("X4")).Split(' ');

                    for (int i = 1; i < indices.Length; i++) {
                        Console.WriteLine("Index : " + i.ToString());
                        Console.WriteLine("Started word : " + words[i]);
                        if (PokeDatabase.System.Scripts.stringToComparisonOperatorDict.ContainsKey(words[i])) {
                            parameters.Add(new byte[] { PokeDatabase.System.Scripts.stringToComparisonOperatorDict[words[i]] });
                        } else {
                            int index = 1 + words[i].IndexOfAny(new char[] { 'x', '#' });

                            /* If number is preceded by 0x parse it as hex, otherwise as decimal */
                            NumberStyles style;
                            if (words[i][index - 1] == 'x')
                                style = NumberStyles.HexNumber;
                            else
                                style = NumberStyles.Integer;

                            /* Convert strings of parameters into the correct datatypes */
                            Console.WriteLine("started params");
                            if (indices[i] == "1")
                                parameters.Add(new byte[] { Byte.Parse(words[i].Substring(index), style) });
                            if (indices[i] == "2")
                                parameters.Add(BitConverter.GetBytes(Int16.Parse(words[i].Substring(index), style)));
                            if (indices[i] == "4")
                                parameters.Add(BitConverter.GetBytes(Int32.Parse(words[i].Substring(index), style)));
                            Console.WriteLine("finished params");
                        }
                        Console.WriteLine("Finished word : " + words[i]);
                    }

                    /* Fix function and movement references which are +1 greater than array indexes */
                    Console.WriteLine("before fix");
                    Console.WriteLine("Param length = " + parameters.Count.ToString());
                    if (id == 0x16 || id == 0x1A)
                        parameters[0] = BitConverter.GetBytes(BitConverter.ToInt32(parameters[0], 0) - 1);
                    if (id == 0x1C || id == 0x1D || id == 0x5E)
                        parameters[1] = BitConverter.GetBytes(BitConverter.ToInt32(parameters[1], 0) - 1);
                }
            }

        }
        #endregion
    }
}