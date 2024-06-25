using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using DSPRE.Resources;
using System;
using DSPRE.ROMFiles;
using static DSPRE.RomInfo;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace DSPRE
{
    /// <summary>
    /// Class to store ROM data from GEN IV Pokémon games
    /// </summary>

    public class RomInfo
    {
        public static string folderSuffix = "_DSPRE_contents";
        private const string dataFolderName = @"data";

        public static string romID { get; private set; }
        public static string fileName { get; private set; }
        public static string workDir { get; private set; }
        public static string arm9Path { get; private set; }
        public static string arm7Path { get; private set; }
        public static string overlayTablePath { get; set; }
        public static string y7Path { get; set; }
        public static string dataPath { get; set; }
        public static string overlayPath { get; set; }
        public static string unpackedPath { get; set; }
        public static string bannerPath { get; set; }
        public static string headerPath { get; set; }

        public static GameLanguages gameLanguage { get; private set; }
        public static GameVersions gameVersion { get; private set; }
        public static GameFamilies gameFamily { get; private set; }

        public static uint synthOverlayLoadAddress = 0x023C8000;
        public static uint arm9spawnOffset { get; private set; }

        public static int initialMoneyOverlayNumber { get; private set; }
        public static uint initialMoneyOverlayOffset { get; private set; }

        public static int cameraTblOverlayNumber { get; private set; }
        public static uint[] cameraTblOffsetsToRAMaddress { get; private set; }

        public static uint headerTableOffset { get; private set; }

        public static uint conditionalMusicTableOffsetToRAMAddress { get; internal set; }
        public static uint encounterMusicTableOffsetToRAMAddress { get; internal set; }

        public static uint vsTrainerEntryTableOffsetToRAMAddress { get; internal set; }
        public static uint vsPokemonEntryTableOffsetToRAMAddress { get; internal set; }
        public static uint effectsComboTableOffsetToRAMAddress { get; internal set; }

        public static uint vsTrainerEntryTableOffsetToSizeLimiter { get; internal set; }
        public static uint vsPokemonEntryTableOffsetToSizeLimiter { get; internal set; }
        public static uint effectsComboTableOffsetToSizeLimiter { get; internal set; }

        public static uint OWTableOffset { get; internal set; }
        public static string OWtablePath { get; private set; }

        public static uint monIconPalTableAddress { get; private set; }

        public static int nullEncounterID { get; private set; }
        public static int abilityNamesTextNumber { get; private set; }
        public static int attackNamesTextNumber { get; private set; }
        public static int[] pokemonNamesTextNumbers { get; private set; }
        public static int itemNamesTextNumber { get; private set; }
        public static int itemScriptFileNumber { get; internal set; }
        public static int trainerClassMessageNumber { get; private set; }
        public static int trainerNamesMessageNumber { get; private set; }
        public static int moveDescriptionsTextNumbers { get; private set; }
        public static int moveNamesTextNumbers { get; private set; }
        public static int locationNamesTextNumber { get; private set; }
        public static int trainerNameLenOffset { get; private set; }
        public static int trainerNameMaxLen => SetTrainerNameMaxLen();
        public static int trainerFunnyScriptNumber { get; private set; }

        public static string internalNamesLocation { get; private set; }
        public static readonly byte internalNameLength = 16;
        public static string internalNamesPath { get; private set; }

        public static int cameraSize { get; private set; }

        public Dictionary<List<uint>, (Color background, Color foreground)> MapCellsColorDictionary;
        public static SortedDictionary<uint, (uint spriteID, ushort properties)> OverworldTable { get; private set; }
        public static uint[] overworldTableKeys { get; private set; }
        public static Dictionary<uint, string> ow3DSpriteDict { get; private set; }

        public static Dictionary<ushort, string> ScriptCommandNamesDict { get; private set; }
        public static Dictionary<string, ushort> ScriptCommandNamesReverseDict { get; private set; }

        public static Dictionary<ushort, string> ScriptActionNamesDict { get; private set; }
        public static Dictionary<string, ushort> ScriptActionNamesReverseDict { get; private set; }

        public static Dictionary<ushort, byte[]> ScriptCommandParametersDict { get; private set; }

        public static Dictionary<ushort, string> ScriptComparisonOperatorsDict { get; private set; }
        public static Dictionary<string, ushort> ScriptComparisonOperatorsReverseDict { get; private set; }

        public enum GameVersions : byte
        {
            Diamond, Pearl, Platinum,
            HeartGold, SoulSilver,
            Black, White,
            Black2, White2
        }

        public enum GameFamilies : byte
        {
            NULL,
            DP,
            Plat,
            HGSS,
            BW,
            BW2
        }

        public enum GameLanguages : byte
        {
            English,
            Japanese,

            Italian,
            Spanish,
            French,
            German
        }

        public enum DirNames : byte
        {
            personalPokeData,

            synthOverlay,
            dynamicHeaders,

            textArchives,
            matrices,

            maps,
            exteriorBuildingModels,
            buildingConfigFiles,
            buildingTextures,
            mapTextures,
            areaData,

            eventFiles,
            OWSprites,

            scripts,

            encounters,
            headbutt,
            safariZone,

            trainerProperties,
            trainerParty,
            trainerGraphics,
            moveData,

            monIcons,

            interiorBuildingModels,
            learnsets,
            evolutions
        };

        public static Dictionary<DirNames, (string packedDir, string unpackedDir)> gameDirs { get; private set; }

        #region Constructors (1)

        public RomInfo(string id, string romName, bool useSuffix = true)
        {
            if (!useSuffix)
            {
                folderSuffix = "";
            }

            string path = System.IO.Path.GetDirectoryName(romName) + "\\" + Path.GetFileNameWithoutExtension(romName) + folderSuffix + "\\";

            workDir = path;
            arm9Path = Path.Combine(workDir, @"arm9.bin");
            arm7Path = Path.Combine(workDir, @"arm7.bin");
            overlayTablePath = Path.Combine(workDir, @"y9.bin");
            y7Path = Path.Combine(workDir, @"y7.bin");
            dataPath = Path.Combine(workDir, dataFolderName);
            overlayPath = Path.Combine(workDir, @"overlay");
            bannerPath = Path.Combine(workDir, @"banner.bin");
            headerPath = Path.Combine(workDir, @"header.bin");
            unpackedPath = Path.Combine(workDir, @"unpacked");
            internalNamesPath = Path.Combine(workDir, $@"{dataFolderName}\fielddata\maptable\mapname.bin");

            try
            {
                gameVersion = PokeDatabase.System.versionsDict[id];
            }
            catch (KeyNotFoundException)
            {
                MessageBox.Show("The ROM you attempted to load is not supported.\nYou can only load Gen IV Pokémon ROMS, for now.", "Unsupported ROM",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            romID = id;
            fileName = romName;

            LoadGameFamily();
            LoadGameLanguage();

            SetNarcDirs();
            SetHeaderTableOffset();
            SetNullEncounterID();

            SetAbilityNamesTextNumber();
            SetAttackNamesTextNumber();
            SetPokemonNamesTextNumber();
            SetItemNamesTextNumber();
            SetItemScriptFileNumber();
            SetLocationNamesTextNumber();
            SetTrainerNamesMessageNumber();
            SetTrainerClassMessageNumber();
            SetTrainerFunnyScriptNumber();
            SetTrainerNameLenOffset();
            SetMoveTextNumbers();

            /* System */
            ScriptCommandParametersDict = BuildCommandParametersDatabase(gameFamily);

            ScriptCommandNamesDict = BuildCommandNamesDatabase(gameFamily);
            ScriptActionNamesDict = BuildActionNamesDatabase(gameFamily);
            ScriptComparisonOperatorsDict = BuildComparisonOperatorsDatabase(gameFamily);

            ScriptCommandNamesReverseDict = ScriptCommandNamesDict.Reverse();
            ScriptActionNamesReverseDict = ScriptActionNamesDict.Reverse();
            ScriptComparisonOperatorsReverseDict = ScriptComparisonOperatorsDict.Reverse();
        }

        #endregion Constructors (1)

        #region Methods (22)

        public static Dictionary<ushort, string> BuildCommandNamesDatabase(GameFamilies gameFam)
        {
            Dictionary<ushort, string> commonDictionaryNames;
            Dictionary<ushort, string> specificDictionaryNames;

            switch (gameFam)
            {
                case GameFamilies.DP:
                    commonDictionaryNames = ScriptDatabase.DPPtScrCmdNames;
                    specificDictionaryNames = ScriptDatabase.DPScrCmdNames;
                    break;

                case GameFamilies.Plat:
                    commonDictionaryNames = ScriptDatabase.DPPtScrCmdNames;
                    specificDictionaryNames = ScriptDatabase.PlatScrCmdNames;
                    break;

                default:
                    commonDictionaryNames = ScriptDatabase.HGSSScrCmdNames;
#if true
                    specificDictionaryNames = new Dictionary<ushort, string>();
#else
                        specificDictionaryNames = ScriptDatabase.CustomScrCmdNames;
#endif
                    break;
            }
            return commonDictionaryNames.Concat(specificDictionaryNames).ToLookup(x => x.Key, x => x.Value).ToDictionary(x => x.Key, g => g.First());
        }

        public static Dictionary<ushort, byte[]> BuildCommandParametersDatabase(GameFamilies gameFam)
        {
            Dictionary<ushort, byte[]> commonDictionaryParams;
            Dictionary<ushort, byte[]> specificDictionaryParams;

            switch (gameFam)
            {
                case GameFamilies.DP:
                    commonDictionaryParams = ScriptDatabase.DPPtScrCmdParameters;
                    specificDictionaryParams = ScriptDatabase.DPScrCmdParameters;
                    break;

                case GameFamilies.Plat:
                    commonDictionaryParams = ScriptDatabase.DPPtScrCmdParameters;
                    specificDictionaryParams = ScriptDatabase.PlatScrCmdParameters;
                    break;

                default:
                    commonDictionaryParams = ScriptDatabase.HGSSScrCmdParameters;
#if true
                    specificDictionaryParams = new Dictionary<ushort, byte[]>();
#else
                        specificDictionaryParams = ScriptDatabase.CustomScrCmdParameters;
#endif
                    break;
            }
            return commonDictionaryParams.Concat(specificDictionaryParams).ToLookup(x => x.Key, x => x.Value).ToDictionary(x => x.Key, g => g.First());
        }

        public static Dictionary<ushort, string> BuildActionNamesDatabase(GameFamilies gameFam)
        {
            switch (gameFam)
            {
                case GameFamilies.DP:
                case GameFamilies.Plat:
                    return ScriptDatabase.movementsDictIDName;

                default:
#if false
                    var commonDictionaryParams = ScriptDatabase.movementsDictIDName;
                    var customDictionaryParams = ScriptDatabase.customMovementsDictIDName;
                    return commonDictionaryParams.Concat(customDictionaryParams).ToLookup(x => x.Key, x => x.Value).ToDictionary(x => x.Key, g => g.First());
#else
                    return ScriptDatabase.movementsDictIDName;
#endif
            }
        }

        public static Dictionary<ushort, string> BuildComparisonOperatorsDatabase(GameFamilies gameFam)
        {
            switch (gameFam)
            {
                case GameFamilies.DP:
                case GameFamilies.Plat:
                case GameFamilies.HGSS:
                    return ScriptDatabase.comparisonOperatorsDict;

                default:
                    var commonDict = ScriptDatabase.comparisonOperatorsDict;
                    var appendixDict = ScriptDatabase.comparisonOperatorsGenVappendix;
                    return commonDict.Concat(appendixDict).ToLookup(x => x.Key, x => x.Value).ToDictionary(x => x.Key, g => g.First());
            }
        }

        public static void Set3DOverworldsDict()
        {
            ow3DSpriteDict = new Dictionary<uint, string>()
            {
                [91] = "brown_sign",
                [92] = "red_sign",
                [93] = "gray_sign",
                [94] = "route_sign",
                [95] = "blue_sign", //to fix this one (gym_sign)
                [96] = "blue_sign",
                [101] = "dawn_platinum",
                //[174] = "dppt_suitcase",
            };
        }

        public static void SetHeaderTableOffset()
        {
            switch (gameFamily)
            {
                case GameFamilies.DP:
                    switch (gameLanguage)
                    {
                        case GameLanguages.English:
                            headerTableOffset = 0xEEDBC;
                            break;

                        case GameLanguages.Spanish:
                            headerTableOffset = 0xEEE08;
                            break;

                        case GameLanguages.Italian:
                            headerTableOffset = 0xEED70;
                            break;

                        case GameLanguages.French:
                            headerTableOffset = 0xEEDFC;
                            break;

                        case GameLanguages.German:
                            headerTableOffset = 0xEEDCC;
                            break;

                        case GameLanguages.Japanese:
                            headerTableOffset = gameVersion == GameVersions.Diamond ? (uint)0xF0D68 : 0xF0D6C;
                            break;
                    }
                    break;

                case GameFamilies.Plat:
                    switch (gameLanguage)
                    {
                        case GameLanguages.English:
                            headerTableOffset = 0xE601C;
                            break;

                        case GameLanguages.Spanish:
                            headerTableOffset = 0xE60B0;
                            break;

                        case GameLanguages.Italian:
                            headerTableOffset = 0xE6038;
                            break;

                        case GameLanguages.French:
                            headerTableOffset = 0xE60A4;
                            break;

                        case GameLanguages.German:
                            headerTableOffset = 0xE6074;
                            break;

                        case GameLanguages.Japanese:
                            headerTableOffset = 0xE56F0;
                            break;
                    }
                    break;

                case GameFamilies.HGSS:
                    switch (gameLanguage)
                    {
                        case GameLanguages.English:
                            headerTableOffset = 0xF6BE0;
                            break;

                        case GameLanguages.Spanish:
                            headerTableOffset = gameVersion == GameVersions.HeartGold ? 0xF6BC8 : (uint)0xF6BD0;
                            break;

                        case GameLanguages.Italian:
                            headerTableOffset = 0xF6B58;
                            break;

                        case GameLanguages.French:
                            headerTableOffset = 0xF6BC4;
                            break;

                        case GameLanguages.German:
                            headerTableOffset = 0xF6B94;
                            break;

                        case GameLanguages.Japanese:
                            headerTableOffset = 0xF6390;
                            break;
                    }
                    break;
            }
        }

        public static void SetupSpawnSettings()
        {
            switch (gameFamily)
            {
                case GameFamilies.DP:
                    initialMoneyOverlayNumber = 52;
                    initialMoneyOverlayOffset = 0x1E4;
                    switch (gameLanguage)
                    {
                        case GameLanguages.English:
                            arm9spawnOffset = 0xF2B9C;
                            break;

                        case GameLanguages.Spanish:
                            arm9spawnOffset = 0xF2BE8;
                            break;

                        case GameLanguages.Italian:
                            arm9spawnOffset = 0xF2B50;
                            break;

                        case GameLanguages.French:
                            arm9spawnOffset = 0xF2BDC;
                            break;

                        case GameLanguages.German:
                            arm9spawnOffset = 0xF2BAC;
                            break;

                        case GameLanguages.Japanese:
                            arm9spawnOffset = 0xF4B48;
                            break;
                    }
                    break;

                case GameFamilies.Plat:
                    initialMoneyOverlayNumber = 57;
                    initialMoneyOverlayOffset = 0x1EC;
                    switch (gameLanguage)
                    {
                        case GameLanguages.English:
                            arm9spawnOffset = 0xEA12C;
                            break;

                        case GameLanguages.Spanish:
                            arm9spawnOffset = 0xEA1C0;
                            break;

                        case GameLanguages.Italian:
                            arm9spawnOffset = 0xEA148;
                            break;

                        case GameLanguages.French:
                            arm9spawnOffset = 0xEA1B4;
                            break;

                        case GameLanguages.German:
                            arm9spawnOffset = 0xEA184;
                            break;

                        case GameLanguages.Japanese:
                            arm9spawnOffset = 0xE9800;
                            break;
                    }
                    break;

                case GameFamilies.HGSS:
                    initialMoneyOverlayNumber = 36;
                    initialMoneyOverlayOffset = 0x2FC;
                    switch (gameLanguage)
                    {
                        case GameLanguages.English:
                            arm9spawnOffset = 0xFA17C;
                            break;

                        case GameLanguages.Spanish:
                            arm9spawnOffset = gameVersion == GameVersions.HeartGold ? 0xFA164 : (uint)0xFA16C;
                            break;

                        case GameLanguages.Italian:
                            arm9spawnOffset = 0xFA0F4;
                            break;

                        case GameLanguages.French:
                            arm9spawnOffset = 0xFA160;
                            break;

                        case GameLanguages.German:
                            arm9spawnOffset = 0xFA130;
                            break;

                        case GameLanguages.Japanese:
                            arm9spawnOffset = 0xF992C;
                            break;
                    }
                    break;
            }
        }

        public static void PrepareCameraData()
        {
            switch (gameFamily)
            {
                case GameFamilies.DP:
                    cameraTblOverlayNumber = 5;
                    cameraTblOffsetsToRAMaddress = gameLanguage.Equals(GameLanguages.Japanese) ? (new uint[] { 0x4C50 }) : (new uint[] { 0x4908 });
                    cameraSize = 24;
                    break;

                case GameFamilies.Plat:
                    cameraTblOverlayNumber = 5;
                    cameraTblOffsetsToRAMaddress = new uint[] { 0x4E24 };
                    cameraSize = 24;
                    break;

                case GameFamilies.HGSS:
                    cameraTblOverlayNumber = 1;
                    cameraSize = 36;
                    switch (gameLanguage)
                    {
                        case GameLanguages.English:
                        case GameLanguages.Spanish:
                        case GameLanguages.French:
                        case GameLanguages.German:
                        case GameLanguages.Italian:
                            cameraTblOffsetsToRAMaddress = new uint[] { 0x532C, 0x547C };
                            break;

                        case GameLanguages.Japanese:
                            cameraTblOffsetsToRAMaddress = new uint[] { 0x5324, 0x5474 };
                            break;
                    }
                    break;
            }
        }

        public static void SetOWtable()
        {
            switch (gameFamily)
            {
                case GameFamilies.DP:
                    OWtablePath = OverlayUtils.GetPath(5);
                    switch (gameLanguage)
                    { // Go to the beginning of the overworld table
                        case GameLanguages.English:
                            OWTableOffset = 0x22BCC;
                            break;

                        case GameLanguages.Japanese:
                            OWTableOffset = 0x23BB8;
                            break;

                        default:
                            OWTableOffset = 0x22B84;
                            break;
                    }
                    break;

                case GameFamilies.Plat:
                    OWtablePath = OverlayUtils.GetPath(5);
                    switch (gameLanguage)
                    { // Go to the beginning of the overworld table
                        case GameLanguages.Italian:
                            OWTableOffset = 0x2BC44;
                            break;

                        case GameLanguages.French:
                        case GameLanguages.Spanish:
                            OWTableOffset = 0x2BC3C;
                            break;

                        case GameLanguages.German:
                            OWTableOffset = 0x2BC50;
                            break;

                        case GameLanguages.Japanese:
                            OWTableOffset = 0x2BA24;
                            break;

                        default:
                            OWTableOffset = 0x2BC34;
                            break;
                    }
                    break;

                case GameFamilies.HGSS:
                    if (OverlayUtils.OverlayTable.IsDefaultCompressed(1))
                    {
                        if (OverlayUtils.IsCompressed(1))
                        {
                            if (OverlayUtils.Decompress(1) < 0)
                            {
                                MessageBox.Show("Overlay 1 couldn't be decompressed.\nOverworld sprites in the Event Editor will be " +
                                "displayed incorrectly or not displayed at all.", "Decompression error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }

                    string ov1Path = OverlayUtils.GetPath(1);
                    uint ov1Address = OverlayUtils.OverlayTable.GetRAMAddress(1);

                    int ramAddrOfPointer;
                    switch (gameLanguage)
                    {
                        case GameLanguages.Italian:
                            ramAddrOfPointer = 0x021F929C;
                            break;

                        case GameLanguages.French:
                        case GameLanguages.Spanish:
                            ramAddrOfPointer = 0x021F931C;
                            break;

                        case GameLanguages.German:
                            ramAddrOfPointer = 0x021F92DC;
                            break;

                        case GameLanguages.Japanese:
                            ramAddrOfPointer = 0x021F86C4;
                            break;

                        default:
                            ramAddrOfPointer = 0x021F92FC;
                            break;
                    }

                    using (DSUtils.EasyReader bReader = new DSUtils.EasyReader(ov1Path, ramAddrOfPointer - ov1Address))
                    { // read the pointer at the specified ram address and adjust accordingly below
                        uint ramAddressOfTable = bReader.ReadUInt32();
                        if ((ramAddressOfTable >> 0x18) != 0x02)
                        {
                            MessageBox.Show("Something went wrong reading the Overworld configuration table.\nOverworld sprites in the Event Editor will be " +
                                "displayed incorrectly or not displayed at all.", "Decompression error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        string ov131path = OverlayUtils.GetPath(131);
                        if (File.Exists(ov131path))
                        {
                            // if HGE field extension overlay exists
                            OWTableOffset = ramAddressOfTable - OverlayUtils.OverlayTable.GetRAMAddress(131);
                            OWtablePath = ov131path;
                        }
                        else if (ramAddressOfTable >= RomInfo.synthOverlayLoadAddress)
                        {
                            // if the pointer shows the table was moved to the synthetic overlay
                            OWTableOffset = ramAddressOfTable - RomInfo.synthOverlayLoadAddress;
                            OWtablePath = gameDirs[DirNames.synthOverlay].unpackedDir + "\\" + PatchToolboxDialog.expandedARMfileID.ToString("D4");
                        }
                        else
                        {
                            OWTableOffset = ramAddressOfTable - ov1Address;
                            OWtablePath = ov1Path;
                        }
                    }
                    break;
            }
        }

        public static void SetConditionalMusicTableOffsetToRAMAddress()
        {
            switch (gameFamily)
            {
                case GameFamilies.HGSS:
                    switch (gameLanguage)
                    {
                        case GameLanguages.Spanish:
                            conditionalMusicTableOffsetToRAMAddress = gameVersion == GameVersions.HeartGold ? (uint)0x667D0 : 0x667D8;
                            break;

                        case GameLanguages.English:
                        case GameLanguages.Italian:
                        case GameLanguages.French:
                        case GameLanguages.German:
                            conditionalMusicTableOffsetToRAMAddress = 0x667D8;
                            break;

                        case GameLanguages.Japanese:
                            conditionalMusicTableOffsetToRAMAddress = 0x66238;
                            break;
                    }
                    break;
            }
        }

        public static void SetBattleEffectsData()
        {
            switch (gameFamily)
            {
                case GameFamilies.HGSS:
                    switch (gameLanguage)
                    {
                        case GameLanguages.Spanish:
                            vsPokemonEntryTableOffsetToRAMAddress = gameVersion == GameVersions.HeartGold ? (uint)0x518CC : 0x518D4;
                            vsTrainerEntryTableOffsetToRAMAddress = gameVersion == GameVersions.HeartGold ? (uint)0x51888 : 0x51890;
                            effectsComboTableOffsetToRAMAddress = gameVersion == GameVersions.HeartGold ? (uint)0x517C0 : 0x517C8;
                            break;

                        case GameLanguages.English:
                        case GameLanguages.Italian:
                        case GameLanguages.French:
                        case GameLanguages.German:
                            vsPokemonEntryTableOffsetToRAMAddress = 0x518D4;
                            vsTrainerEntryTableOffsetToRAMAddress = 0x51890;
                            effectsComboTableOffsetToRAMAddress = 0x517C8;
                            break;

                        case GameLanguages.Japanese:
                            vsPokemonEntryTableOffsetToRAMAddress = 0x5136C;
                            vsTrainerEntryTableOffsetToRAMAddress = 0x51328;
                            effectsComboTableOffsetToRAMAddress = 0x51260;
                            break;
                    }
                    vsPokemonEntryTableOffsetToSizeLimiter = vsPokemonEntryTableOffsetToRAMAddress - 0xA;
                    vsTrainerEntryTableOffsetToSizeLimiter = vsTrainerEntryTableOffsetToRAMAddress - 0xA;
                    effectsComboTableOffsetToSizeLimiter = effectsComboTableOffsetToRAMAddress - 0x1E;
                    break;

                case GameFamilies.Plat:
                    switch (gameLanguage)
                    {
                        case GameLanguages.English:
                            effectsComboTableOffsetToRAMAddress = 0x51BE0;
                            break;

                        case GameLanguages.Italian:
                        case GameLanguages.French:
                        case GameLanguages.Spanish:
                        case GameLanguages.German:
                            effectsComboTableOffsetToRAMAddress = 0x51C84;
                            break;

                        case GameLanguages.Japanese:
                            effectsComboTableOffsetToRAMAddress = 0x514C0;
                            break;
                    }
                    break;
            }
        }

        public static void SetEncounterMusicTableOffsetToRAMAddress()
        {
            switch (gameFamily)
            {
                case GameFamilies.HGSS:
                    switch (gameLanguage)
                    {
                        case GameLanguages.Spanish:
                            encounterMusicTableOffsetToRAMAddress = gameVersion == GameVersions.HeartGold ? (uint)0x550D8 : 0x550E0;
                            break;

                        case GameLanguages.English:
                        case GameLanguages.Italian:
                        case GameLanguages.French:
                        case GameLanguages.German:
                            encounterMusicTableOffsetToRAMAddress = 0x550E0;
                            break;

                        case GameLanguages.Japanese:
                            encounterMusicTableOffsetToRAMAddress = 0x54B44;
                            break;
                    }
                    break;

                case GameFamilies.Plat:
                    switch (gameLanguage)
                    {
                        case GameLanguages.English:
                            encounterMusicTableOffsetToRAMAddress = 0x5563C;
                            break;

                        case GameLanguages.Italian:
                        case GameLanguages.French:
                        case GameLanguages.Spanish:
                        case GameLanguages.German:
                            encounterMusicTableOffsetToRAMAddress = 0x556E0;
                            break;

                        case GameLanguages.Japanese:
                            encounterMusicTableOffsetToRAMAddress = 0x54F04;
                            break;
                    }
                    break;

                case GameFamilies.DP:
                    switch (gameLanguage)
                    {
                        case GameLanguages.English:
                            encounterMusicTableOffsetToRAMAddress = 0x4AD3C;
                            break;

                        case GameLanguages.Italian:
                        case GameLanguages.French:
                        case GameLanguages.Spanish:
                        case GameLanguages.German:
                            encounterMusicTableOffsetToRAMAddress = 0x4ADAC;
                            break;

                        case GameLanguages.Japanese:
                            encounterMusicTableOffsetToRAMAddress = 0x4D9AC;
                            break;
                    }
                    break;
            }
        }

        public static void SetMonIconsPalTableAddress()
        {
            switch (RomInfo.gameFamily)
            {
                case GameFamilies.DP:
                    switch (gameLanguage)
                    {
                        case GameLanguages.English:
                            monIconPalTableAddress = BitConverter.ToUInt32(ARM9.ReadBytes(0x6B838, 4), 0);
                            break;

                        case GameLanguages.Italian:
                            monIconPalTableAddress = BitConverter.ToUInt32(ARM9.ReadBytes(0x6B874, 4), 0);
                            break;

                        case GameLanguages.German:
                        case GameLanguages.French:
                        case GameLanguages.Spanish:
                            monIconPalTableAddress = BitConverter.ToUInt32(ARM9.ReadBytes(0x6B894, 4), 0);
                            break;

                        case GameLanguages.Japanese:
                            monIconPalTableAddress = BitConverter.ToUInt32(ARM9.ReadBytes(0x6FDEC, 4), 0);
                            break;
                    }
                    break;

                case GameFamilies.Plat:
                    switch (gameLanguage)
                    {
                        case GameLanguages.English:
                            monIconPalTableAddress = BitConverter.ToUInt32(ARM9.ReadBytes(0x79F80, 4), 0);
                            break;

                        case GameLanguages.Italian:
                        case GameLanguages.German:
                        case GameLanguages.French:
                        case GameLanguages.Spanish:
                            monIconPalTableAddress = BitConverter.ToUInt32(ARM9.ReadBytes(0x7A020, 4), 0);
                            break;

                        case GameLanguages.Japanese:
                            monIconPalTableAddress = BitConverter.ToUInt32(ARM9.ReadBytes(0x79858, 4), 0);
                            break;
                    }
                    break;

                case GameFamilies.HGSS:
                default:
                    switch (gameLanguage)
                    {
                        case GameLanguages.English:
                        case GameLanguages.Italian:
                            monIconPalTableAddress = BitConverter.ToUInt32(ARM9.ReadBytes(0x74408, 4), 0);
                            break;

                        case GameLanguages.German:
                            if (gameVersion == GameVersions.HeartGold)
                            {
                                monIconPalTableAddress = BitConverter.ToUInt32(ARM9.ReadBytes(0x74408, 4), 0);
                            }
                            else
                            {
                                monIconPalTableAddress = BitConverter.ToUInt32(ARM9.ReadBytes(0x74400, 4), 0);
                            }
                            break;

                        case GameLanguages.French:
                        case GameLanguages.Spanish:
                            if (gameVersion == GameVersions.HeartGold)
                            {
                                monIconPalTableAddress = BitConverter.ToUInt32(ARM9.ReadBytes(0x74400, 4), 0);
                            }
                            else
                            {
                                monIconPalTableAddress = BitConverter.ToUInt32(ARM9.ReadBytes(0x74408, 4), 0);
                            }
                            break;

                        case GameLanguages.Japanese:
                            monIconPalTableAddress = BitConverter.ToUInt32(ARM9.ReadBytes(0x73EA0, 4), 0);
                            break;
                    }
                    break;
            }
        }

        private static void SetItemScriptFileNumber()
        {
            switch (gameFamily)
            {
                case GameFamilies.DP:
                    itemScriptFileNumber = 370;
                    break;

                case GameFamilies.Plat:
                    itemScriptFileNumber = 404;
                    break;

                default:
                    itemScriptFileNumber = 141;
                    break;
            }
        }

        private static void SetNullEncounterID()
        {
            switch (gameFamily)
            {
                case GameFamilies.DP:
                case GameFamilies.Plat:
                    nullEncounterID = ushort.MaxValue;
                    break;

                case GameFamilies.HGSS:
                    nullEncounterID = Byte.MaxValue;
                    break;
            }
        }

        private static void SetAbilityNamesTextNumber()
        {
            switch (gameFamily)
            {
                case GameFamilies.DP:
                    abilityNamesTextNumber = 552;
                    break;

                case GameFamilies.Plat:
                    abilityNamesTextNumber = 610;
                    break;

                case GameFamilies.HGSS:
                    abilityNamesTextNumber = 720;
                    break;

                default:
                    break;
            }
        }

        private static void SetAttackNamesTextNumber()
        {
            switch (gameFamily)
            {
                case GameFamilies.DP:
                    attackNamesTextNumber = 588;
                    break;

                case GameFamilies.Plat:
                    attackNamesTextNumber = 647;
                    break;

                default:
                    attackNamesTextNumber = gameLanguage == GameLanguages.Japanese ? 739 : 750;
                    break;
            }
        }

        private static void SetItemNamesTextNumber()
        {
            switch (gameFamily)
            {
                case GameFamilies.DP:
                    itemNamesTextNumber = 344;
                    break;

                case GameFamilies.Plat:
                    itemNamesTextNumber = 392;
                    break;

                default:
                    itemNamesTextNumber = gameLanguage == GameLanguages.Japanese ? 219 : 222;
                    break;
            }
        }

        private static void SetLocationNamesTextNumber()
        {
            switch (gameFamily)
            {
                case GameFamilies.DP:
                    locationNamesTextNumber = 382;
                    break;

                case GameFamilies.Plat:
                    locationNamesTextNumber = 433;
                    break;

                default:
                    locationNamesTextNumber = gameLanguage == GameLanguages.Japanese ? 272 : 279;
                    break;
            }
        }

        private static void SetPokemonNamesTextNumber()
        {
            switch (gameFamily)
            {
                case GameFamilies.DP:
                    pokemonNamesTextNumbers = new int[2] { 362, 363 };
                    break;

                case GameFamilies.Plat:
                    pokemonNamesTextNumbers = new int[7] { 412, 413, 712, 713, 714, 715, 716 }; //413?
                    break;

                case GameFamilies.HGSS:
                    pokemonNamesTextNumbers = gameLanguage.Equals(GameLanguages.Japanese) ? new int[1] { 232 } : new int[7] { 237, 238, 817, 818, 819, 820, 821 }; //238?
                    break;
            }
        }

        private static void SetTrainerNamesMessageNumber()
        {
            switch (gameFamily)
            {
                case GameFamilies.DP:
                    trainerNamesMessageNumber = 559;
                    if (gameLanguage.Equals(GameLanguages.Japanese))
                    {
                        trainerNamesMessageNumber -= 9;
                    }
                    break;

                case GameFamilies.Plat:
                    trainerNamesMessageNumber = 618;
                    break;

                default:
                    trainerNamesMessageNumber = 729;
                    if (gameLanguage == GameLanguages.Japanese)
                    {
                        trainerNamesMessageNumber -= 10;
                    }
                    break;
            }
        }

        private static void SetTrainerClassMessageNumber()
        {
            switch (gameFamily)
            {
                case GameFamilies.DP:
                    trainerClassMessageNumber = 560;
                    if (gameLanguage.Equals(GameLanguages.Japanese))
                    {
                        trainerClassMessageNumber -= 9;
                    }
                    break;

                case GameFamilies.Plat:
                    trainerClassMessageNumber = 619;
                    break;

                default:
                    trainerClassMessageNumber = 730;
                    if (gameLanguage.Equals(GameLanguages.Japanese))
                    {
                        trainerClassMessageNumber -= 10;
                    }
                    break;
            }
        }
        private static void SetMoveTextNumbers() {
            switch (gameFamily) {
                case GameFamilies.DP:
                case GameFamilies.Plat:
                    moveDescriptionsTextNumbers = 646;
                    moveNamesTextNumbers = 647;
                    break;

                case GameFamilies.HGSS:
                    moveDescriptionsTextNumbers = 749;
                    moveNamesTextNumbers = 750;
                    break;
            }
        }

        private static void SetTrainerFunnyScriptNumber() {
            switch (gameFamily) {
                case GameFamilies.DP:
                    trainerFunnyScriptNumber = 851;
                    break;

                case GameFamilies.Plat:
                    trainerFunnyScriptNumber = 929;
                    break;

                default: // HGSS
                    trainerFunnyScriptNumber = 740;
                    break;
            }
        }        

        private static void SetTrainerNameLenOffset()
        {
            switch (RomInfo.gameFamily)
            {
                case GameFamilies.DP:
                    switch (RomInfo.gameLanguage)
                    {
                        case GameLanguages.English:
                            trainerNameLenOffset = 0x6AC32;
                            break;

                        case GameLanguages.Italian:
                            trainerNameLenOffset = 0x6AC6E;
                            break;

                        case GameLanguages.Spanish:
                        case GameLanguages.German:
                        case GameLanguages.French:
                            trainerNameLenOffset = 0x6AC8E;
                            break;

                        case GameLanguages.Japanese: //?
                        default:
                            trainerNameLenOffset = -1;
                            break;
                    }
                    break;

                case GameFamilies.Plat:
                    switch (RomInfo.gameLanguage)
                    {
                        case GameLanguages.English:
                            trainerNameLenOffset = 0x791DE;
                            break;

                        case GameLanguages.Spanish:
                        case GameLanguages.Italian:
                        case GameLanguages.German:
                        case GameLanguages.French:
                            trainerNameLenOffset = 0x7927E;
                            break;

                        case GameLanguages.Japanese:
                            trainerNameLenOffset = 0x78AB6;
                            break;

                        default:
                            trainerNameLenOffset = -1;
                            break;
                    }
                    break;

                case GameFamilies.HGSS:
                    if (RomInfo.gameLanguage.Equals(GameLanguages.Japanese))
                    {
                        //Jap HGSS
                        trainerNameLenOffset = 0x7342E;
                    }
                    else if (gameVersion.Equals(GameVersions.SoulSilver))
                    {
                        //All SS languages except Jap
                        trainerNameLenOffset = 0x72EC2;
                    }
                    else
                    {
                        //All HG languages except Jap
                        switch (RomInfo.gameLanguage)
                        {
                            case GameLanguages.English:
                            case GameLanguages.Italian:
                            case GameLanguages.German:
                            case GameLanguages.French:
                                trainerNameLenOffset = 0x7342E;
                                break;

                            case GameLanguages.Spanish:
                                trainerNameLenOffset = 0x73426;
                                break;
                        }
                    }
                    break;
            }
        }

        public static int SetTrainerNameMaxLen()
        {
            int maxLength = TrainerFile.defaultNameLen;
            if (trainerNameLenOffset > 0)
            {
                using (ARM9.Reader ar = new ARM9.Reader(trainerNameLenOffset))
                {
                    maxLength = ar.ReadByte();
                }
                maxLength += ((maxLength - 4) / 2);
            }
            return maxLength;
        }

        public string GetBuildingModelsDirPath(bool interior) => interior ? gameDirs[DirNames.interiorBuildingModels].unpackedDir : gameDirs[DirNames.exteriorBuildingModels].unpackedDir;

        public string GetRomNameFromWorkdir() => workDir.Substring(0, workDir.Length - folderSuffix.Length - 1);

        public static int GetHeaderCount() => (int)new FileInfo(internalNamesPath).Length / internalNameLength;

        public static List<string> GetLocationNames() => new TextArchive(locationNamesTextNumber).messages;

        public static string[] GetSimpleTrainerNames() => new TextArchive(trainerNamesMessageNumber).messages.ToArray();

        public static string[] GetTrainerClassNames() => new TextArchive(trainerClassMessageNumber).messages.ToArray();

        public static string[] GetItemNames() => new TextArchive(itemNamesTextNumber).messages.ToArray();

        public static string[] GetItemNames(int startIndex = 0, int? count = null)
        {
            TextArchive itemNames = new TextArchive(itemNamesTextNumber);
            return itemNames.messages.GetRange(startIndex, count == null ? itemNames.messages.Count - 1 : (int)count).ToArray();
        }

        public static string[] GetPokemonNames() => new TextArchive(pokemonNamesTextNumbers[0]).messages.ToArray();

        public static string[] GetAbilityNames() => new TextArchive(abilityNamesTextNumber).messages.ToArray();

        public static string[] GetAttackNames() => new TextArchive(attackNamesTextNumber).messages.ToArray();

        public static int GetLearnsetFilesCount() => Directory.GetFiles(gameDirs[DirNames.learnsets].unpackedDir).Length;

        public static int GetPersonalFilesCount() => Directory.GetFiles(gameDirs[DirNames.personalPokeData].unpackedDir).Length;

        public static string[] GetEvolutionFilesList() => Directory.GetFiles(gameDirs[DirNames.evolutions].unpackedDir);

        public static int GetEvolutionFilesCount() => GetEvolutionFilesList().Length;
        public static string[] GetBattleEffectSequenceFiles() => Directory.GetFiles(gameDirs[DirNames.moveData].unpackedDir);
        public static int GetBattleEffectSequenceFilesCount() => GetBattleEffectSequenceFiles().Length;

        public int GetAreaDataCount() => Directory.GetFiles(gameDirs[DirNames.areaData].unpackedDir).Length;

        public int GetMapTexturesCount() => Directory.GetFiles(gameDirs[DirNames.mapTextures].unpackedDir).Length;

        public int GetBuildingTexturesCount() => Directory.GetFiles(gameDirs[DirNames.buildingTextures].unpackedDir).Length;

        public int GetMatrixCount() => Directory.GetFiles(gameDirs[DirNames.matrices].unpackedDir).Length;

        public int GetTextArchivesCount() => Directory.GetFiles(gameDirs[DirNames.textArchives].unpackedDir).Length;

        public int GetMapCount() => Directory.GetFiles(gameDirs[DirNames.maps].unpackedDir).Length;

        public int GetEventCount() => Directory.GetFiles(gameDirs[DirNames.eventFiles].unpackedDir).Length;

        public int GetScriptCount() => Directory.GetFiles(gameDirs[DirNames.scripts].unpackedDir).Length;

        public int GetBuildingCount(bool interior) => Directory.GetFiles(GetBuildingModelsDirPath(interior)).Length;

        public static int GetEventFileCount() => Directory.GetFiles(RomInfo.gameDirs[DirNames.eventFiles].unpackedDir).Length;

        #endregion Methods (22)

        #region System Methods

        private void LoadGameLanguage()
        {
            switch (romID)
            {
                case "ADAE":
                case "APAE":
                case "CPUE":
                case "IPKE":
                case "IPGE":
                    gameLanguage = GameLanguages.English;
                    break;

                case "ADAS":
                case "APAS":
                case "CPUS":
                case "IPKS":
                case "IPGS":
                case "LATA":
                    gameLanguage = GameLanguages.Spanish;
                    break;

                case "ADAI":
                case "APAI":
                case "CPUI":
                case "IPKI":
                case "IPGI":
                    gameLanguage = GameLanguages.Italian;
                    break;

                case "ADAF":
                case "APAF":
                case "CPUF":
                case "IPKF":
                case "IPGF":
                    gameLanguage = GameLanguages.French;
                    break;

                case "ADAD":
                case "APAD":
                case "CPUD":
                case "IPKD":
                case "IPGD":
                    gameLanguage = GameLanguages.German;
                    break;

                default:
                    gameLanguage = GameLanguages.Japanese;
                    break;
            }
        }

        private void LoadGameFamily()
        {
            switch (gameVersion)
            {
                case GameVersions.Diamond:
                case GameVersions.Pearl:
                    gameFamily = GameFamilies.DP;
                    break;

                case GameVersions.Platinum:
                    gameFamily = GameFamilies.Plat;
                    break;

                case GameVersions.HeartGold:
                case GameVersions.SoulSilver:
                    gameFamily = GameFamilies.HGSS;
                    break;
            }
        }

        private void SetNarcDirs()
        {
            Dictionary<DirNames, string> packedDirsDict = null;
            switch (gameFamily)
            {
                case GameFamilies.DP:
                    string suffix = "";
                    if (!gameLanguage.Equals(GameLanguages.Japanese))
                    {
                        suffix = "_release";
                    }

                    packedDirsDict = new Dictionary<DirNames, string>()
                    {
                        [DirNames.synthOverlay] = @"data\data\weather_sys.narc",
                        [DirNames.textArchives] = @"data\msgdata\msg.narc",

                        [DirNames.matrices] = @"data\fielddata\mapmatrix\map_matrix.narc",

                        [DirNames.maps] = @"data\fielddata\land_data\land_data" + suffix + ".narc",
                        [DirNames.exteriorBuildingModels] = @"data\fielddata\build_model\build_model.narc",
                        [DirNames.buildingConfigFiles] = @"data\fielddata\areadata\area_build_model\area_build.narc",
                        [DirNames.buildingTextures] = @"data\fielddata\areadata\area_build_model\areabm_texset.narc",
                        [DirNames.mapTextures] = @"data\fielddata\areadata\area_map_tex\map_tex_set.narc",
                        [DirNames.areaData] = @"data\fielddata\areadata\area_data.narc",

                        [DirNames.eventFiles] = @"data\fielddata\eventdata\zone_event" + suffix + ".narc",
                        [DirNames.OWSprites] = @"data\data\mmodel\mmodel.narc",

                        [DirNames.scripts] = @"data\fielddata\script\scr_seq" + suffix + ".narc",

                        [DirNames.trainerProperties] = @"data\poketool\trainer\trdata.narc",
                        [DirNames.trainerParty] = @"data\poketool\trainer\trpoke.narc",
                        [DirNames.trainerGraphics] = @"data\poketool\trgra\trfgra.narc",

                        [DirNames.monIcons] = @"data\poketool\icongra\poke_icon.narc",

                        [DirNames.encounters] = @"data\fielddata\encountdata\" + char.ToLower(gameVersion.ToString()[0]) + '_' + "enc_data.narc",
                        [DirNames.learnsets] = @"data\poketool\personal\wotbl.narc",
                        [DirNames.evolutions] = @"data\poketool\personal\evo.narc",
                    };

                    //Personal Data archive is different for Pearl
                    string personal = @"data\poketool\personal";
                    if (gameVersion == GameVersions.Pearl)
                    {
                        personal += ("_" + gameVersion.ToString().ToLower());
                    }
                    personal += @"\personal.narc";
                    packedDirsDict[DirNames.personalPokeData] = personal;

                    break;

                case GameFamilies.Plat:
                    suffix = gameVersion.ToString().Substring(0, 2).ToLower();

                    packedDirsDict = new Dictionary<DirNames, string>()
                    {
                        [DirNames.personalPokeData] = @"data\poketool\personal\pl_personal.narc",
                        [DirNames.synthOverlay] = @"data\data\weather_sys.narc",
                        [DirNames.dynamicHeaders] = @"data\debug\cb_edit\d_test.narc",

                        [DirNames.textArchives] = @"data\msgdata\" + suffix + '_' + "msg.narc",

                        [DirNames.matrices] = @"data\fielddata\mapmatrix\map_matrix.narc",

                        [DirNames.maps] = @"data\fielddata\land_data\land_data.narc",
                        [DirNames.exteriorBuildingModels] = @"data\fielddata\build_model\build_model.narc",
                        [DirNames.buildingConfigFiles] = @"data\fielddata\areadata\area_build_model\area_build.narc",
                        [DirNames.buildingTextures] = @"data\fielddata\areadata\area_build_model\areabm_texset.narc",
                        [DirNames.mapTextures] = @"data\fielddata\areadata\area_map_tex\map_tex_set.narc",
                        [DirNames.areaData] = @"data\fielddata\areadata\area_data.narc",

                        [DirNames.eventFiles] = @"data\fielddata\eventdata\zone_event.narc",
                        [DirNames.OWSprites] = @"data\data\mmodel\mmodel.narc",

                        [DirNames.scripts] = @"data\fielddata\script\scr_seq.narc",

                        [DirNames.trainerProperties] = @"data\poketool\trainer\trdata.narc",
                        [DirNames.trainerParty] = @"data\poketool\trainer\trpoke.narc",
                        [DirNames.trainerGraphics] = @"data\poketool\trgra\trfgra.narc",
                        [DirNames.moveData] = @"data\poketool\waza\pl_waza_tbl.narc",

                        [DirNames.monIcons] = @"data\poketool\icongra\pl_poke_icon.narc",

                        [DirNames.encounters] = @"data\fielddata\encountdata\" + suffix + '_' + "enc_data.narc",
                        [DirNames.learnsets] = @"data\poketool\personal\wotbl.narc",
                        [DirNames.evolutions] = @"data\poketool\personal\evo.narc",
                    };
                    break;

                case GameFamilies.HGSS:
                    packedDirsDict = new Dictionary<DirNames, string>()
                    {
                        [DirNames.personalPokeData] = @"data\a\0\0\2",
                        [DirNames.synthOverlay] = @"data\a\0\2\8",
                        [DirNames.dynamicHeaders] = @"data\a\0\5\0",

                        [DirNames.textArchives] = @"data\a\0\2\7",

                        [DirNames.matrices] = @"data\a\0\4\1",

                        [DirNames.maps] = @"data\a\0\6\5",
                        [DirNames.exteriorBuildingModels] = @"data\a\0\4\0",
                        [DirNames.buildingConfigFiles] = @"data\a\0\4\3",
                        [DirNames.buildingTextures] = @"data\a\0\7\0",
                        [DirNames.mapTextures] = @"data\a\0\4\4",
                        [DirNames.areaData] = @"data\a\0\4\2",

                        [DirNames.eventFiles] = @"data\a\0\3\2",
                        [DirNames.OWSprites] = @"data\a\0\8\1",

                        [DirNames.scripts] = @"data\a\0\1\2",
                        //ENCOUNTERS FOLDER DEPENDS ON VERSION
                        [DirNames.trainerProperties] = @"data\a\0\5\5",
                        [DirNames.trainerParty] = @"data\a\0\5\6",
                        [DirNames.trainerGraphics] = @"data\a\0\5\8",
                        [DirNames.moveData] = @"data\a\0\1\1",

                        [DirNames.monIcons] = @"data\a\0\2\0",

                        [DirNames.interiorBuildingModels] = @"data\a\1\4\8",
                        [DirNames.learnsets] = @"data\a\0\3\3",
                        [DirNames.evolutions] = @"data\a\0\3\4",

                        [DirNames.safariZone] = @"data\a\2\3\0",
                        [DirNames.headbutt] = @"data\a\2\5\2", //both versions use the same folder with different data
                    };

                    //Encounter archive is different for SS
                    packedDirsDict[DirNames.encounters] = gameVersion == GameVersions.HeartGold ? @"data\a\0\3\7" : @"data\a\1\3\6";
                    break;
            }

            gameDirs = new Dictionary<DirNames, (string packedDir, string unpackedDir)>();
            foreach (KeyValuePair<DirNames, string> kvp in packedDirsDict)
            {
                gameDirs.Add(kvp.Key, (workDir + kvp.Value, workDir + @"unpacked" + '\\' + kvp.Key.ToString()));
            }
        }

        public void ResetMapCellsColorDictionary()
        {
            switch (gameFamily)
            {
                case GameFamilies.DP:
                case GameFamilies.Plat:
                    MapCellsColorDictionary = PokeDatabase.System.MatrixCellColors.DPPtmatrixColorsDict;
                    break;

                case GameFamilies.HGSS:
                    MapCellsColorDictionary = PokeDatabase.System.MatrixCellColors.HGSSmatrixColorsDict;
                    break;
            }
        }

        public static void ReadOWTable()
        {
            OverworldTable = new SortedDictionary<uint, (uint spriteID, ushort properties)>();
            switch (gameFamily)
            {
                case GameFamilies.DP:
                case GameFamilies.Plat:
                    using (BinaryReader idReader = new BinaryReader(new FileStream(OWtablePath, FileMode.Open)))
                    {
                        idReader.BaseStream.Position = OWTableOffset;

                        uint entryID = idReader.ReadUInt32();
                        idReader.BaseStream.Position -= 4;
                        while ((entryID = idReader.ReadUInt32()) != 0xFFFF)
                        {
                            uint spriteID = idReader.ReadUInt32();
                            (uint spriteID, ushort properties) tup = (spriteID, 0x0000);
                            OverworldTable.Add(entryID, tup);
                        }
                    }
                    break;

                case GameFamilies.HGSS:
                    using (BinaryReader idReader = new BinaryReader(new FileStream(OWtablePath, FileMode.Open)))
                    {
                        idReader.BaseStream.Position = OWTableOffset;

                        ushort entryID = idReader.ReadUInt16();
                        idReader.BaseStream.Position -= 2;
                        while ((entryID = idReader.ReadUInt16()) != 0xFFFF)
                        {
                            uint spriteID = idReader.ReadUInt16();
                            ushort properties = idReader.ReadUInt16();
                            (uint spriteID, ushort properties) tup = (spriteID, properties);
                            OverworldTable.Add(entryID, tup);
                        }
                    }
                    break;
            }
            foreach (uint k in ow3DSpriteDict.Keys)
            {
                OverworldTable.Add(k, (0x3D3D, 0x3D3D)); //ADD 3D overworld data (spriteID and properties are dummy values)
            }
            overworldTableKeys = OverworldTable.Keys.ToArray();
        }

        #endregion System Methods
    }
}