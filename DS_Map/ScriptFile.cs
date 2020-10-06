using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.IO;
using System.Linq;

namespace DS_Map
{
    /// <summary>
    /// Class to store script file data in Pokémon NDS games
    /// </summary>
    public class ScriptFile
	{
        #region Fields (3)
        public List<Script> scripts = new List<Script>();
        public List<Script> functions = new List<Script>();
        public List<Script> movements = new List<Script>();
        public bool isLevelScript = new bool();
        #endregion

        #region Constructors (1)
        public ScriptFile(Stream fs, string version)
		{
            List<uint> scriptOffsets = new List<uint>();
            List<uint> functionOffsets = new List<uint>();
            List<uint> movementOffsets = new List<uint>();
            ushort[] endCodes = new ushort[] { 0x2, 0x16, 0x1B };

            using (BinaryReader scriptFileReader = new BinaryReader(fs))
            {
                /* Read script offsets from the header */
                while (scriptFileReader.ReadUInt16() != 0xFD13)
                {
                    scriptFileReader.BaseStream.Position -= 0x2;
                    uint value = scriptFileReader.ReadUInt32();

                    if (value == 0)
                    {
                        isLevelScript = true;
                        return;
                    }
                    else
                    {
                        uint offset = value + (uint)scriptFileReader.BaseStream.Position;
                        scriptOffsets.Add(offset); // Don't change order of addition
                    }
                }

                /* Read scripts */
                for (int i = 0; i < scriptOffsets.Count; i++)
                {
                    int duplicateIndex = scriptOffsets.FindIndex(offset => offset == scriptOffsets[i]); // Check for UseScript_#
                    if (duplicateIndex != i) scripts.Add(new Script(duplicateIndex));
                    else
                    {
                        scriptFileReader.BaseStream.Position = scriptOffsets[i];

                        List<Command> commandsList = new List<Command>();
                        bool endScript = new bool();
                        while (!endScript)
                        {
                            Command command = Read_Command(scriptFileReader, ref functionOffsets, ref movementOffsets, version);
                            commandsList.Add(command);
                            if (endCodes.Contains(command.id)) endScript = true;
                        }
                        this.scripts.Add(new Script(commandsList));
                    }
                }

                /* Read functions */
                for (int i = 0; i < functionOffsets.Count; i++)
                {
                    scriptFileReader.BaseStream.Position = functionOffsets[i];

                    List<Command> commandsList = new List<Command>();
                    bool endFunction = new bool();
                    while (!endFunction)
                    {
                        Command command = Read_Command(scriptFileReader, ref functionOffsets, ref movementOffsets, version);
                        commandsList.Add(command);
                        if (endCodes.Contains(command.id)) endFunction = true;
                    }

                    this.functions.Add(new Script(commandsList));
                }

                /* Read movements */
                for (int i = 0; i < movementOffsets.Count; i++)
                {
                    scriptFileReader.BaseStream.Position = movementOffsets[i];

                    List<Command> commandsList = new List<Command>();
                    bool endMovement = new bool();
                    while (!endMovement)
                    {
                        ushort id = scriptFileReader.ReadUInt16();
                        List<byte[]> parameters = new List<byte[]>();
                        if (id != 0xFE) parameters.Add(scriptFileReader.ReadBytes(2));
                        Command command = new Command(id, parameters, version, true);

                        commandsList.Add(command);
                        if (command.id == 0xFE) endMovement = true;

                    }
                    this.movements.Add(new Script(commandsList));
                }
            }
        }
        #endregion

        #region Methods (1)
        private Command Read_Command(BinaryReader dataReader, ref List<uint> functionOffsets, ref List<uint> movementOffsets, string gameVersion)
        {
            ResourceManager getCommandParameters;
            switch (gameVersion)
            {
                case "Diamond":
                case "Pearl":
                    getCommandParameters = new ResourceManager("DS_Map.Resources.ScriptParametersDP", Assembly.GetExecutingAssembly());
                    break;
                case "HeartGold":
                case "SoulSilver":
                    getCommandParameters = new ResourceManager("DS_Map.Resources.ScriptParametersHGSS", Assembly.GetExecutingAssembly());
                    break;
                default: // Platinum
                    getCommandParameters = new ResourceManager("DS_Map.Resources.ScriptParametersDP", Assembly.GetExecutingAssembly());
                    break;
            }

            ushort id = dataReader.ReadUInt16();
            List<byte[]> parameters = new List<byte[]>();

            /* How to read parameters for different commands */
            switch (id)
            {
                case 0x16: // Jump
                case 0x1A: // Call
                case 0x1C: // CompareLastResultJump
                case 0x1D: // CompareLastResultCall
                    {
                        if (id == 0x1C || id == 0x1D)
                        {
                            byte opcode = dataReader.ReadByte();
                            uint offset = dataReader.ReadUInt32() + (uint)dataReader.BaseStream.Position; // Do not change order of addition
                            if (!functionOffsets.Contains(offset)) functionOffsets.Add(offset);

                            parameters.Add(new byte[] { opcode });
                            parameters.Add(BitConverter.GetBytes(functionOffsets.IndexOf(offset)));
                        }
                        else
                        {
                            uint offset = dataReader.ReadUInt32() + (uint)dataReader.BaseStream.Position; // Do not change order of addition
                            if (!functionOffsets.Contains(offset)) functionOffsets.Add(offset);

                            parameters.Add(BitConverter.GetBytes(functionOffsets.IndexOf(offset)));
                        }
                    }
                    break;
                case 0x5E: // ApplyMovement
                case 0x2A1: // ApplyMovement2
                    {
                        ushort overworld = dataReader.ReadUInt16();
                        uint offset = dataReader.ReadUInt32() + (uint)dataReader.BaseStream.Position; // Do not change order of addition
                        if (!movementOffsets.Contains(offset)) movementOffsets.Add(offset);

                        parameters.Add(BitConverter.GetBytes(overworld));
                        parameters.Add(BitConverter.GetBytes(movementOffsets.IndexOf(offset)));
                    }
                    break;
                case 0x190:
                    {
                        if (gameVersion != "HeartGold" && gameVersion != "SoulSilver") goto default;
                        else
                        {
                            byte parameter1 = dataReader.ReadByte();
                            parameters.Add(new byte[] { parameter1 });
                            if (parameter1 == 0x2) parameters.Add(dataReader.ReadBytes(2));
                        }
                    }
                    break;
                case 0x1CF:
                    {
                        byte parameter1 = dataReader.ReadByte();
                        parameters.Add(new byte[] { parameter1 });
                        if (parameter1 == 0x2) parameters.Add(dataReader.ReadBytes(2));
                    }
                    break;
                case 0x1D1:
                    {
                        if (gameVersion != "HeartGold" && gameVersion != "SoulSilver") goto default;
                        else
                        {
                            short parameter1 = dataReader.ReadInt16();
                            parameters.Add(BitConverter.GetBytes(parameter1));
                            switch (parameter1)
                            {
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
                case 0x1E9:
                    {
                        if (gameVersion != "HeartGold" || gameVersion != "SoulSilver") goto default;
                        else
                        {
                            short parameter1 = dataReader.ReadInt16();
                            parameters.Add(BitConverter.GetBytes(parameter1));

                            switch (parameter1)
                            {
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
                case 0x21D:
                    {
                        if (gameVersion != "Platinum") goto default;
                        else
                        {
                            byte parameter1 = dataReader.ReadByte();
                            parameters.Add(new byte[] { parameter1 });
                            if (parameter1 != 0x6)
                            {
                                parameters.Add(dataReader.ReadBytes(2));
                                if (parameter1 != 0x5) parameters.Add(dataReader.ReadBytes(2));
                            }
                        }
                    }
                    break;
                case 0x235:
                    {
                        short parameter1 = dataReader.ReadInt16();
                        parameters.Add(BitConverter.GetBytes(parameter1));

                        switch (parameter1)
                        {
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
                case 0x23E:
                    {
                        short parameter1 = dataReader.ReadInt16();
                        parameters.Add(BitConverter.GetBytes(parameter1));

                        switch (parameter1)
                        {
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
                case 0x2C4:
                    {
                        byte parameter1 = dataReader.ReadByte();
                        parameters.Add(new byte[] { parameter1 });
                        if (parameter1 == 0 || parameter1 == 1) parameters.Add(dataReader.ReadBytes(2));
                    }
                    break;
                case 0x2C5:
                    {
                        if (gameVersion != "Platinum") goto default;
                        else
                        {
                            parameters.Add(dataReader.ReadBytes(2));
                            parameters.Add(dataReader.ReadBytes(2));
                        }
                    }
                    break;
                case 0x2C6:
                case 0x2C9:
                case 0x2CA:
                case 0x2CD:
                    if (gameVersion != "Platinum") goto default;
                    break;
                case 0x2CF:
                    if (gameVersion != "Platinum") goto default;
                    else
                    {
                        parameters.Add(dataReader.ReadBytes(2));
                        parameters.Add(dataReader.ReadBytes(2));
                    }
                    break;
                default:
                    string[] indexes = getCommandParameters.GetString(id.ToString("X4")).Split(' ');
                    for (int i = 1; i < indexes.Length; i++) 
                    {
                        int length = Convert.ToInt32(indexes[i]);
                        parameters.Add(dataReader.ReadBytes(length));
                    }
                    break;

            }

            return new Command(id, parameters, gameVersion, false);
        }
        public byte[] Save()
        {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData))
            {
                List<uint> scriptOffsets = new List<uint>();
                List<uint> functionOffsets = new List<uint>();
                List<uint> movementOffsets = new List<uint>();

                List<Tuple<int, int, int>> references = new List<Tuple<int, int, int>>(); // Format: [address, function/movement #, type]
                int[] referenceCodes = new int[] { 0x16, 0x1A, 0x1C, 0x1D, 0x5E };

                /* Allocate enough space for script pointers, which we do not know yet */
                writer.BaseStream.Position += scripts.Count * 0x4;
                writer.Write((ushort)0xFD13); // End of header signal

                /* Write scripts */
                for (int i = 0; i < scripts.Count; i++)
                {
                    if (scripts[i].useScript != -1) scriptOffsets.Add(scriptOffsets[scripts[i].useScript]);  // If script is a UseScript, copy offset
                    else
                    {
                        scriptOffsets.Add((uint)writer.BaseStream.Position);

                        for (int j = 0; j < scripts[i].commands.Count; j++)
                        {
                            /* Write command id */
                            ushort id = scripts[i].commands[j].id;
                            writer.Write(id);

                            /* Write command parameters */
                            List<byte[]> parameters = scripts[i].commands[j].parameters;
                            for (int k = 0; k < parameters.Count; k++) writer.Write(parameters[k]);

                            /* If command calls a function/movement, store reference position */
                            if (referenceCodes.Contains(id))
                            {
                                int index;
                                if (id == 0x16 || id == 0x1A) index = 0; // Jump, Call
                                else index = 1;

                                int type = 0;
                                if (id == 0x5E) type = 1; // ApplyMovement
                                references.Add(new Tuple<int, int, int>((int)writer.BaseStream.Position - 4, BitConverter.ToInt32(parameters[index], 0), type));
                            }
                        }
                    }
                }

                /* Write functions */
                for (int i = 0; i < functions.Count; i++)
                {
                    functionOffsets.Add((uint)writer.BaseStream.Position);

                    for (int j = 0; j < functions[i].commands.Count; j++)
                    {
                        /* Write command id */
                        ushort id = functions[i].commands[j].id;
                        writer.Write(id);

                        /* Write command parameters */
                        List<byte[]> parameters = functions[i].commands[j].parameters;
                        for (int k = 0; k < parameters.Count; k++) writer.Write(parameters[k]);

                        /* If command calls a function/movement, store reference position */
                        if (referenceCodes.Contains(id))
                        {
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
                for (int i = 0; i < movements.Count; i++)
                {
                    movementOffsets.Add((uint)writer.BaseStream.Position);

                    for (int j = 0; j < movements[i].commands.Count; j++)
                    {
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
                for (int i = 0; i < references.Count; i++)
                {
                    writer.BaseStream.Position = references[i].Item1;
                    
                    if (references[i].Item3 == 1) writer.Write((UInt32)(movementOffsets[references[i].Item2] - references[i].Item1 - 4));
                    else writer.Write((UInt32)(functionOffsets[references[i].Item2] - references[i].Item1 - 4));
                }
            }

            return newData.ToArray();
        }
        #endregion
    }
    public class Script
    {
        #region Fields (1)
        public List<Command> commands;
        public int useScript = -1;
        #endregion Fields

        #region Constructors (2)
        public Script(List<Command> commandsList)
        {
            commands = commandsList;
        }
        public Script(int useScript)
        {
            this.useScript = useScript;
        }
        #endregion
    }
    public class Command
    {
        #region Fields (4)
        public ushort id;
        public List<byte[]> parameters;
        public string description;
        public bool isMovement;
        #endregion

        #region Constructors (2)
        public Command(ushort id, List<byte[]> parameters, string version, bool isMovement)
        {
            Dictionary<byte, string> operatorsDict = new Dictionary<byte, string>()
            {
                [0x0] = "LOWER",
                [0x1] = "EQUAL",
                [0x2] = "LARGER",
                [0x3] = "LOWER/EQUAL",
                [0x4] = "LARGER/EQUAL",
                [0x5] = "DIFFERENT",
                [0x6] = "OR",
                [0x7] = "AND",
                [0xFF] = "TRUEUP"
            };
            ResourceManager getCommandName;
            if (!isMovement)
            {
                if (version == "Diamond" || version == "Pearl" || version == "Platinum") getCommandName = new ResourceManager("DS_Map.Resources.ScriptNamesDP", Assembly.GetExecutingAssembly());
                else getCommandName = new ResourceManager("DS_Map.Resources.ScriptNamesHGSS", Assembly.GetExecutingAssembly());
            }
            else getCommandName = new ResourceManager("DS_Map.Resources.MovementNames", Assembly.GetExecutingAssembly());

            this.id = id;
            this.parameters = parameters;
            this.description = getCommandName.GetString(id.ToString("X4"));
            if (description == null) description = id.ToString("X4");
            this.isMovement = isMovement;

            switch (id)
            {
                case 0x16:      // Jump
                case 0x1A:      // Call
                    this.description += " " + "Function_#" + (1 + BitConverter.ToInt32(parameters[0], 0)).ToString("D");
                    break;
                case 0x1C:      // CompareLastResultJump
                case 0x1D:      // CompareLastResultCall
                    byte opcode = parameters[0][0];
                    this.description += " " + operatorsDict[opcode] + " " + "Function_#" + (1 + (BitConverter.ToInt32(parameters[1], 0))).ToString("D");
                    break;
                case 0x5E:      // ApplyMovement
                    this.description += " " + "Overworld_#" + (BitConverter.ToInt16(parameters[0], 0)).ToString("D") + " " + "Movement_#" + (1 + (BitConverter.ToInt32(parameters[1], 0))).ToString("D");
                    break;
                case 0x62:      // Lock
                case 0x63:      // Release
                case 0x64:      // AddPeople
                case 0x65:      // RemovePeople
                    this.description += " " + "Overworld_#" + BitConverter.ToInt16(parameters[0], 0).ToString("D");
                    break;
                default:
                    for (int i = 0; i < parameters.Count; i++)
                    {
                        if (parameters[i].Length == 1) this.description += " " + "0x" + (parameters[i][0]).ToString("X1");
                        else if (parameters[i].Length == 2) this.description += " " + "0x" + (BitConverter.ToInt16(parameters[i], 0)).ToString("X1");
                        else if (parameters[i].Length == 4) this.description += " " + "0x" + (BitConverter.ToInt32(parameters[i], 0)).ToString("X1");
                    }                    
                    break;
            }           
        }
        public Command(string description, string version, bool isMovement)
        {
            this.description = description;
            this.isMovement = isMovement;
            this.parameters = new List<byte[]>();

            string[] words = description.Split(' '); // Separate command code from parameters

            /* Get command id, which is always first in the description */
            Dictionary<string, ushort> commandsDictDPPt = new Dictionary<string, ushort>()
            {
                ["Nop"] = 0x0000,
                ["Nop1"] = 0x0001,
                ["End"] = 0x0002,
                ["Return2"] = 0x0003,
                ["If"] = 0x0011,
                ["If2"] = 0x0012,
                ["CallStandard"] = 0x0014,
                ["Jump"] = 0x0016 ,
                ["Call"] = 0x001A,
                ["Return"] = 0x001B,
                ["CompareLastResultJump"] = 0x001C,
                ["CompareLastResultCall"] = 0x001D,
                ["ClearFlag"] = 0x001E,
                ["SetFlag"] = 0x001F,
                ["CheckFlag"] = 0x0020,
                ["SetValue"] = 0x0023,
                ["CompareVarsToByte"] = 0x0026,
                ["SetVar"] = 0x0028,
                ["CopyVar"] = 0x0029,
                ["Message"] = 0x002C,
                ["Message2"] = 0x002D,
                ["Message3"] = 0x002F,
                ["WaitButton"] = 0x0031 ,
                ["CloseMessageOnKeyPress"] = 0x0034,
                ["FreezeMessageBox"] = 0x0035,
                ["CallMessageBox"] = 0x0036,
                ["ColorMessageBox"] = 0x0037,
                ["TypeMessageBox"] = 0x0038,
                ["NoMapMessageBox"] = 0x0039,
                ["CallMessageBoxText"] = 0x003A,
                ["Menu"] = 0x003C ,
                ["YesNoBox"] = 0x003E,
                ["WaitFor"] = 0x003F,
                ["Multi"] = 0x0040,
                ["Multi2"] = 0x0041,
                ["TextScriptMulti"] = 0x0042,
                ["CloseMulti"] = 0x0043,
                ["Multi3"] = 0x0044,
                ["TextMessageScript"] = 0x0046,
                ["MultiRow"] = 0x0048,
                ["PlayFanfare"] = 0x0049,
                ["PlayFanfare2"] = 0x004A,
                ["WaitFanfare"] = 0x004B,
                ["PlayCry"] = 0x004C,
                ["WaitCry"] = 0x004D,
                ["PlaySound"] = 0x004E,
                ["FadeDefaultMusic"] = 0x004F,
                ["PlayMusic"] = 0x0050,
                ["StopMusic"] = 0x0051,
                ["RestartMusic"] = 0x0052,
                ["SwitchMusic"] = 0x0054,
                ["SwitchMusic2"] = 0x005A,
                ["ApplyMovement"] = 0x005E,
                ["WaitMovement"] = 0x005F,
                ["LockAll"] = 0x0060,
                ["ReleaseAll"] = 0x0061,
                ["Lock"] = 0x0062,
                ["Release"] = 0x0063,
                ["AddPeople"] = 0x0064,
                ["RemovePeople"] = 0x0065,
                ["LockCam"] = 0x0066,
                ["FacePlayer"] = 0x0068,
                ["CheckSpritePosition"] = 0x0069,
                ["CheckPersonPosition"] = 0x006B,
                ["ContinueFollow"] = 0x006C,
                ["FollowHero"] = 0x006D,
                ["StopFollowHero"] = 0x006E,
                ["GiveMoney"] = 0x006F,
                ["TakeMoney"] = 0x0070,
                ["CheckMoney"] = 0x0071,
                ["ShowMoney"] = 0x0072,
                ["HideMoney"] = 0x0073,
                ["UpdateMoney"] = 0x0074,
                ["ShowCoins"] = 0x0075,
                ["HideCoins"] = 0x0076,
                ["UpdateCoins"] = 0x0077,
                ["CheckCoins"] = 0x0078,
                ["GiveCoins"] = 0x0079,
                ["TakeCoins"] = 0x007A,
                ["TakeItem"] = 0x007B,
                ["CheckStoreItem"] = 0x007C,
                ["CheckItem"] = 0x007D,
                ["CheckUndergroundPCStatus"] = 0x0085,
                ["CheckPokémonParty"] = 0x0093,
                ["StorePokémonParty"] = 0x0094,
                ["SetPokémonPartyStored"] = 0x0095,
                ["GivePokémon"] = 0x0096,
                ["GiveEgg"] = 0x0097,
                ["CheckMove"] = 0x0099,
                ["CheckPlaceStored"] = 0x009A,
                ["CallEnd"] = 0x00A1,
                ["WFC"] = 0x00A3,
                ["Interview"] = 0x00A5,
                ["DressPokémon"] = 0x00A6,
                ["DisplayDressedPokémon"] = 0x00A7,
                ["DisplayContestPokémon"] = 0x00A8,
                ["CapsuleEditor"] = 0x00A9,
                ["SinnohMaps"] = 0x00AA,
                ["BoxPokémon"] = 0x00AB,
                ["DrawUnion"] = 0x00AC,
                ["TrainerCaseUnion"] = 0x00AD,
                ["TradeUnion"] = 0x00AE,
                ["RecordMixingUnion"] = 0x00AF,
                ["EndGame"] = 0x00B0,
                ["HallOfFameData"] = 0x00B1,
                ["WFC1"] = 0x00B3,
                ["ChooseStarter"] = 0x00B4,
                ["ChoosePlayerName"] = 0x00BA,
                ["ChoosePokémonName"] = 0x00BB,
                ["FadeScreen"] = 0x00BC,
                ["ResetScreen"] = 0x00BD,
                ["Warp"] = 0x00BE,
                ["RockClimbAnimation"] = 0x00BF,
                ["SurfAnimation"] = 0x00C0,
                ["WaterfallAnimation"] = 0x00C1,
                ["FlyAnimation"] = 0x00C2,
                ["Tuxedo"] = 0x00C6,
                ["CheckBike"] = 0x00C7,
                ["RideBike"] = 0x00C8,
                ["BerryHeroAnimation"] = 0x00CB,
                ["StopBerryHeroAnimation"] = 0x00CC,
                ["SetVariableHero"] = 0x00CD,
                ["SetVariableRival"] = 0x00CE,
                ["SetVariableAlter"] = 0x00CF,
                ["SetVariablePokémon"] = 0x00D0,
                ["SetVariableItem"] = 0x00D1,
                ["SetVariableAttackItem"] = 0x00D3,
                ["SetVariableAttack"] = 0x00D4,
                ["SetVariableNumber"] = 0x00D5,
                ["SetVariableNickname"] = 0x00D6,
                ["SetVariableObject"] = 0x00D7,
                ["SetVariableTrainer"] = 0x00D8,
                ["SetVarPokémonStored"] = 0x00DA,
                ["SetVarHeroStored"] = 0x00DB,
                ["SetVarRivalStored"] = 0x00DC,
                ["SetVarAlterStored"] = 0x00DD,
                ["StoreStarter"] = 0x00DE,
                ["TrainerBattle"] = 0x00E5,
                ["EndTrainerBattle"] = 0x00E6,
                ["ActLeagueBattlers"] = 0x00EA,
                ["LostGoToPokémonCenter"] = 0x00EB,
                ["CheckLost"] = 0x00EC,
                ["ChooseFriend"] = 0x00F2,
                ["WirelessBattleWait"] = 0x00F3,
                ["PokémonContest"] = 0x00F7,
                ["FlashContest"] = 0x0111,
                ["EndFlash"] = 0x0112,
                ["ShowLinkCountRecord"] = 0x0116,
                ["WarpMapElevator"] = 0x011B,
                ["CheckFloor"] = 0x011C,
                ["SetPositionAfterShip"] = 0x0120,
                ["WildBattle"] = 0x0124,
                ["StarterBattle"] = 0x0125,
                ["ExplanationBattle"] = 0x0126,
                ["HoneyTreeBattle"] = 0x0127,
                ["RandomBattle"] = 0x0129,
                ["WriteAutograph"] = 0x012B,
                ["CheckDress"] = 0x012E,
                ["GivePokétch"] = 0x0131,
                ["ActivatePokétchApp"] = 0x0133,
                ["StorePokétchApp"] = 0x0134,
                ["ExpectDecisionOther"] = 0x0143,
                ["PokéMart"] = 0x0147,
                ["PokéMart1"] = 0x0148,
                ["PokéMart2"] = 0x0149,
                ["PokéMart3"] = 0x014A,
                ["DefeatGoToPokéCenter"] = 0x014B,
                ["CheckGender"] = 0x014D,
                ["HealPokémon"] = 0x014E,
                ["UnionRoom"] = 0x0153,
                ["ActivatePokédex"] = 0x0158,
                ["GiveRunningShoes"] = 0x015A,
                ["CheckBadge"] = 0x015B,
                ["EnableBadge"] = 0x015C,
                ["DisableBadge"] = 0x015D,
                ["PrepareDoorAnimation"] = 0x0168,
                ["WaitAction"] = 0x0169,
                ["WaitClose"] = 0x016A,
                ["OpenDoor"] = 0x016B,
                ["CloseDoor"] = 0x016C,
                ["CheckPartyNumber"] = 0x0177,
                ["OpenBerryPouch"] = 0x0178,
                ["SetOverworldPosition"] = 0x0186,
                ["SetOverworldMovement"] = 0x0188,
                ["ReleaseOverworld"] = 0x0189,
                ["SetDoorPassable"] = 0x018A,
                ["SetDoorLocked"] = 0x018B,
                ["ShowSavingClock"] = 0x018D,
                ["HideSavingClock"] = 0x018E,
                ["ChoosePokémonMenu"] = 0x0191,
                ["ChoosePokémonMenu2"] = 0x0192,
                ["StorePokémonMenu2"] = 0x0193,
                ["PokémonInfo"] = 0x0195,
                ["StorePokémonNumber"] = 0x0198,
                ["CheckPartyNumber2"] = 0x019A,
                ["EggAnimation"] = 0x01AC,
                ["MailBox"] = 0x01B3,
                ["RecordList"] = 0x01B5,
                ["CheckHappiness"] = 0x01B9,
                ["CheckPosition"] = 0x01BD,
                ["CheckPokémonParty2"] = 0x01C0,
                ["CopyPokémonHeight"] = 0x01C1,
                ["SetVariablePokémonHeight"] = 0x01C2,
                ["ComparePokémonHeight"] = 0x01C3,
                ["CheckPokémonHeight"] = 0x01C4,
                ["MoveInfo"] = 0x01C6,
                ["StoreMove"] = 0x01C7,
                ["DeleteMove"] = 0x01C9,
                ["BerryPoffin"] = 0x01D7,
                ["BattleRoomResult"] = 0x01D9,
                ["CheckSinnohPokédex"] = 0x01E8,
                ["CheckNationalPokédex"] = 0x01E9,
                ["ShowSinnohCertificate"] = 0x01EA,
                ["ShowNationalCertificate"] = 0x01EB,
                ["CheckFossil"] = 0x01F1,
                ["CheckPokémonLevel"] = 0x01F6,
                ["WarpLastElevator"] = 0x0204,
                ["GeoNet"] = 0x0205,
                ["GreatMarshBinoculars"] = 0x0206,
                ["PokémonPicture"] = 0x0208,
                ["HidePicture"] = 0x0209,
                ["RememberMove"] = 0x0221,
                ["TeachMove"] = 0x0224,
                ["CheckTeachMove"] = 0x0225,
                ["CheckPokémonTrade"] = 0x0228,
                ["TradeChosenPokémon"] = 0x0229,
                ["StopTrade"] = 0x022A,
                ["DecideRules"] = 0x0239,
                ["HealPokémonAnimation"] = 0x023B,
                ["ShipAnimation"] = 0x023D,
                ["PhraseBox1W"] = 0x0243,
                ["PhraseBox2W"] = 0x0244,
                ["CheckPhraseBoxInput"] = 0x0249,
                ["PreparePcAnimation"] = 0x024B,
                ["OpenPcAnimation"] = 0x024C,
                ["ClosePcAnimation"] = 0x024D,
                ["CheckLotteryNumber"] = 0x024E,
                ["CompareLotteryNumber"] = 0x024F,
                ["CheckBoxesNumber"] = 0x0252,
                ["SprtSave"] = 0x0257,
                ["RetSprtSave"] = 0x0258,
                ["ElevLgAnimation"] = 0x0259,
                ["CheckAccessories"] = 0x0261,
                ["PokéCasino"] = 0x0267,
                ["UnownMessageBox"] = 0x026D,
                ["ThankNameInsert"] = 0x0271,
                ["LeagueCastleView"] = 0x027A,
                ["PokémonPartyPicture"] = 0x028C,
                ["CheckFirstTimeChampion"] = 0x028F,
                ["ShowBattlePointsBox"] = 0x0294,
                ["HideBattlePointsBox"] = 0x0295,
                ["ChoiceMulti"] = 0x029D,
                ["HiddenMachineEffect"] = 0x029E,
                ["CameraBumpEffect"] = 0x029F,
                ["DoubleBattle"] = 0x02A0,
                ["ApplyMovement2"] = 0x02A1,
                ["ChooseTradePokémon"] = 0x02A5,
                ["ComparePhraseBoxInput"] = 0x02AA,
                ["ActivateMysteryGift"] = 0x02AC,
                ["CheckWildBattle2"] = 0x02BC,
                ["WildBattle2"] = 0x02BD,
                ["BikeRide"] = 0x02BF,
                ["ShowSaveBox"] = 0x02C1,
                ["HideSaveBox"] = 0x02C2,
                ["SpinTradeUnion"] = 0x02C6,
                ["CheckGameVersion"] = 0x02C7,
                ["FloralClockAnimation"] = 0x02CA,
                ["PortalEffect"] = 0x0328,
                ["DisplayFloor"] = 0x0347
            };
            Dictionary<string, ushort> movementsDictDPPtHGSS = new Dictionary<string, ushort>()
            {
                ["SeeUp"] = 0x0000,
                ["SeeDown"] = 0x0001,
                ["SeeLeft"] = 0x0002,
                ["SeeRight"] = 0x0003,
                ["WalkUpSlow"] = 0x0004,
                ["WalkDownSlow"] = 0x0005,
                ["WalkLeftSlow"] = 0x0006,
                ["WalkRightSlow"] = 0x0007,
                ["WalkUp"] = 0x0008,
                ["WalkDown"] = 0x0009,
                ["WalkLeft"] = 0x000A,
                ["WalkRight"] = 0x000B,
                ["WalkUpFast"] = 0x000C,
                ["WalkDownFast"] = 0x000D,
                ["WalkLeftFast"] = 0x000E,
                ["WalkRightFast"] = 0x000F,
                ["WalkUpVeryFast"] = 0x0010,
                ["WalkDownVeryFast"] = 0x0011,
                ["WalkLeftVeryFast"] = 0x0012,
                ["WalkRightVeryFast"] = 0x0013,
                ["RunUp"] = 0x0014,
                ["RunDown"] = 0x0015,
                ["RunLeft"] = 0x0016,
                ["RunRight"] = 0x0017,
                ["WalkUpSlowSite"] = 0x0018,
                ["WalkDownSlowSite"] = 0x0019,
                ["WalkLeftSlowSite"] = 0x001A,
                ["WalkRightSlowSite"] = 0x001B,
                ["WalkUpSite"] = 0x001C,
                ["WalkDownSite"] = 0x001D,
                ["WalkLeftSite"] = 0x001E,
                ["WalkRightSite"] = 0x001F,
                ["WalkUpFastSite"] = 0x0020,
                ["WalkDownFastSite"] = 0x0021,
                ["WalkLeftFastSite"] = 0x0022,
                ["WalkRightFastSite"] = 0x0023,
                ["WalkUpVeryFastSite"] = 0x0024,
                ["WalkDownVeryFastSite"] = 0x0025,
                ["WalkLeftVeryFastSite"] = 0x0026,
                ["WalkRightVeryFastSite"] = 0x0027,
                ["RunUpSite"] = 0x0028,
                ["RunDownSite"] = 0x0029,
                ["RunLeftSite"] = 0x002A,
                ["RunRightSite"] = 0x002B,
                ["JumpUpSlow"] = 0x002C,
                ["JumpDownSlow"] = 0x002D,
                ["JumpLeftSlow"] = 0x002E,
                ["JumpRightSlow"] = 0x002F,
                ["JumpUp"] = 0x0030,
                ["JumpDown"] = 0x0031,
                ["JumpLeft"] = 0x0032,
                ["JumpRight"] = 0x0033,
                ["JumpUp1"] = 0x0034,
                ["JumpDown1"] = 0x0035,
                ["JumpLeft1"] = 0x0036,
                ["JumpRight1"] = 0x0037,
                ["JumpUp2"] = 0x0038,
                ["JumpDown2"] = 0x0039,
                ["JumpLeft2"] = 0x003A,
                ["JumpRight2"] = 0x003B,
                ["WaitDisappear"] = 0x0045,
                ["Exclamation"] = 0x004B,
                ["WaitWalkUpSlow"] = 0x004C,
                ["WaitWalkDownSlow"] = 0x004D,
                ["WaitWalkLeftSlow"] = 0x004E,
                ["WaitWalkRightSlow"] = 0x004F,
                ["WaitWalkUp"] = 0x0050,
                ["WaitWalkDown"] = 0x0051,
                ["WaitWalkLeft"] = 0x0052,
                ["WaitWalkRight"] = 0x0053,
                ["WaitMoveUp"] = 0x0054,
                ["WaitMoveDown"] = 0x0055,
                ["WaitMoveLeft"] = 0x0056,
                ["WaitMoveRight"] = 0x0057,
                ["WaitWalkBackUp"] = 0x0058,
                ["WaitWalkBackDown"] = 0x0059,
                ["WaitWalkBackLeft"] = 0x005A,
                ["WaitWalkBackRight"] = 0x005B,
                ["WaitJumpLeft1"] = 0x005C,
                ["WaitJumpRight1"] = 0x005D,
                ["WaitJumpLeft2"] = 0x005E,
                ["WaitJumpRight2"] = 0x005F,
                ["WaitMoveSite"] = 0x0064,
                ["WaitJumpSite"] = 0x0065,
                ["WaitDoubleExclamation"] = 0x0067,
                ["WaitMoveForever"] = 0x0068,
                ["End"] = 0x00FE
            };

            if (!isMovement)
            {
                if (commandsDictDPPt.ContainsKey(words[0])) this.id = commandsDictDPPt[words[0]];
                else UInt16.TryParse(words[0], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out this.id);
            }
            else
            {
                if (movementsDictDPPtHGSS.ContainsKey(words[0])) this.id = movementsDictDPPtHGSS[words[0]];
                else UInt16.TryParse(words[0], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out this.id);
            }

            /* Read parameters from remainder of the description */
            if (words.Length > 1 && this.id != 0)
            {
                if(!isMovement)
                {
                    Dictionary<string, byte> operatorsDict = new Dictionary<string, byte>()
                    {
                        ["LOWER"] = 0x0,
                        ["EQUAL"] = 0x1,
                        ["LARGER"] = 0x2,
                        ["LOWER/EQUAL"] = 0x3,
                        ["LARGER/EQUAL"] = 0x4,
                        ["DIFFERENT"] = 0x5,
                        ["OR"] = 0x6,
                        ["AND"] = 0x7,
                        ["TRUEUP"] = 0xFF
                    };
                    ResourceManager getCommandParameters; // Load the resource file containing information on parameters for each command
                    switch (version)
                    {
                        case "Diamond":
                        case "Pearl":
                            getCommandParameters = new ResourceManager("DS_Map.Resources.ScriptParametersDP", Assembly.GetExecutingAssembly());
                            break;
                        case "HeartGold":
                        case "SoulSilver":
                            getCommandParameters = new ResourceManager("DS_Map.Resources.ScriptParametersHGSS", Assembly.GetExecutingAssembly());
                            break;
                        default: // Platinum
                            getCommandParameters = new ResourceManager("DS_Map.Resources.ScriptParametersDP", Assembly.GetExecutingAssembly());
                            break;
                    }

                    string[] indexes = getCommandParameters.GetString(id.ToString("X4")).Split(' ');

                    for (int i = 1; i < indexes.Length; i++)
                    {
                        if (operatorsDict.ContainsKey(words[i])) parameters.Add(new byte[] { operatorsDict[words[i]] });
                        else
                        {
                            int index = 1 + words[i].IndexOfAny(new char[] { 'x', '#' });

                            /* If number is preceded by 0x parse it as hex, otherwise as decimal */
                            NumberStyles style;
                            if (words[i][index - 1] == 'x') style = NumberStyles.HexNumber;
                            else style = NumberStyles.Integer;

                            /* Convert strings of parameters into the correct datatypes */
                            if (indexes[i] == "1") parameters.Add(new byte[] { Byte.Parse(words[i].Substring(index), style) });
                            if (indexes[i] == "2") parameters.Add(BitConverter.GetBytes(Int16.Parse(words[i].Substring(index), style)));
                            if (indexes[i] == "4") parameters.Add(BitConverter.GetBytes(Int32.Parse(words[i].Substring(index), style)));
                        }
                    }

                    /* Fix function and movement references which are +1 greater than array indexes */
                    if (id == 0x16 || id == 0x1A) parameters[0] = BitConverter.GetBytes(BitConverter.ToInt32(parameters[0], 0) - 1);
                    if (id == 0x1C || id == 0x1D || id == 0x5E) parameters[1] = BitConverter.GetBytes(BitConverter.ToInt32(parameters[1], 0) - 1);
                }
                else parameters.Add(BitConverter.GetBytes(Int16.Parse(words[1].Substring(2), NumberStyles.HexNumber)));
            }
            
        }
        #endregion
    }
}