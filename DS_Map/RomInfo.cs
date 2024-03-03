using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using DSPRE.Resources;
using System;
using DSPRE.ROMFiles;

namespace DSPRE {

    /// <summary>
    /// Class to store ROM data from GEN IV Pokémon games
    /// </summary>

    public class RomInfo {
        public static readonly byte internalNameLength = 16;
        public static uint synthOverlayLoadAddress = 0x023C8000;
        public Dictionary<List<uint>, (Color background, Color foreground)> MapCellsColorDictionary;
        public static string folderSuffix = "_DSPRE_contents";
        const string dataFolderName = @"data";

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
        public static int nullEncounterID { get; private set; }
        public static int attackNamesTextNumber { get; private set; }
        public static int[] pokemonNamesTextNumbers { get; private set; }
        public static int itemNamesTextNumber { get; private set; }
        public static int itemScriptFileNumber { get; internal set; }
        public static int trainerClassMessageNumber { get; private set; }
        public static int trainerNamesMessageNumber { get; private set; }
        public static int locationNamesTextNumber { get; private set; }
        public static string internalNamesPath { get; private set; }
        public static int cameraSize { get; private set; }
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

        public enum GameVersions : byte {
            Diamond, Pearl, Platinum,
            HeartGold, SoulSilver,
            Black, White,
            Black2, White2
        }
        public enum GameFamilies : byte {
            NULL,
            DP,
            Plat,
            HGSS,
            BW,
            BW2
        }
        public enum GameLanguages : byte {
            English,
            Japanese,

            Italian,
            Spanish,
            French,
            German
        }
        public enum DirNames : byte {
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

            monIcons,

            interiorBuildingModels
        };
        public static Dictionary<DirNames, (string packedDir, string unpackedDir)> gameDirs { get; private set; }

        #region Constructors (1)
        public RomInfo(string id, string romName, bool useSuffix = true) {
            if (!useSuffix) {
                folderSuffix = "";
            }

            string path = Path.GetDirectoryName(romName) + "\\" + Path.GetFileNameWithoutExtension(romName) + folderSuffix + "\\";

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

            try {
                gameVersion = PokeDatabase.System.versionsDict[id];
            } catch (KeyNotFoundException) {
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

            SetAttackNamesTextNumber();
            SetPokemonNamesTextNumber();
            SetItemNamesTextNumber();
            SetItemScriptFileNumber();
            SetLocationNamesTextNumber();
            SetTrainerNamesMessageNumber();
            SetTrainerClassMessageNumber();

            /* System */
            ScriptCommandParametersDict = BuildCommandParametersDatabase(gameFamily);

            ScriptCommandNamesDict = BuildCommandNamesDatabase(gameFamily);
            ScriptActionNamesDict = BuildActionNamesDatabase(gameFamily);
            ScriptComparisonOperatorsDict = BuildComparisonOperatorsDatabase(gameFamily);

            ScriptCommandNamesReverseDict = ScriptCommandNamesDict.Reverse();
            ScriptActionNamesReverseDict = ScriptActionNamesDict.Reverse();
            ScriptComparisonOperatorsReverseDict = ScriptComparisonOperatorsDict.Reverse();
        }
        #endregion

        #region Methods (22)
        public static Dictionary<ushort, string> BuildCommandNamesDatabase(GameFamilies gameFam) {
            Dictionary<ushort, string> commonDictionaryNames;
            Dictionary<ushort, string> specificDictionaryNames;

            switch (gameFam) {
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
        public static Dictionary<ushort, byte[]> BuildCommandParametersDatabase(GameFamilies gameFam) {
            Dictionary<ushort, byte[]> commonDictionaryParams;
            Dictionary<ushort, byte[]> specificDictionaryParams;

            switch (gameFam) {
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
        public static Dictionary<ushort, string> BuildActionNamesDatabase(GameFamilies gameFam) {
            switch (gameFam) {
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
        public static Dictionary<ushort, string> BuildComparisonOperatorsDatabase(GameFamilies gameFam) {
            switch (gameFam) {
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
        public static void Set3DOverworldsDict() {
            ow3DSpriteDict = new Dictionary<uint, string>() {
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
        public static void SetHeaderTableOffset() {
            switch (gameFamily) {
                case GameFamilies.DP:
                    switch (gameLanguage) {
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
                    switch (gameLanguage) {
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
                    switch (gameLanguage) {
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
        public static void SetupSpawnSettings() {
            switch (gameFamily) {
                case GameFamilies.DP:
                    initialMoneyOverlayNumber = 52;
                    initialMoneyOverlayOffset = 0x1E4;
                    switch (gameLanguage) {
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
                    switch (gameLanguage) {
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
                    switch (gameLanguage) {
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
        public static void PrepareCameraData() {
            switch (gameFamily) {
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
                    switch (gameLanguage) {
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
        public static void SetOWtable() {
            switch (gameFamily) {
                case GameFamilies.DP:
                    OWtablePath = DSUtils.GetOverlayPath(5);
                    switch (gameLanguage) { // Go to the beginning of the overworld table
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
                    OWtablePath = DSUtils.GetOverlayPath(5);
                    switch (gameLanguage) { // Go to the beginning of the overworld table
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
                    if (DSUtils.CheckOverlayHasCompressionFlag(1)) {
                        if (DSUtils.OverlayIsCompressed(1)) {
                            if (DSUtils.DecompressOverlay(1) < 0) {
                                MessageBox.Show("Overlay 1 couldn't be decompressed.\nOverworld sprites in the Event Editor will be " +
                                "displayed incorrectly or not displayed at all.", "Decompression error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }

                    string ov1Path = DSUtils.GetOverlayPath(1);
                    uint ov1Address = DSUtils.GetOverlayRAMAddress(1);

                    int ramAddrOfPointer;
                    switch (gameLanguage) {
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

                    using (DSUtils.EasyReader bReader = new DSUtils.EasyReader(ov1Path, ramAddrOfPointer - ov1Address)) { // read the pointer at the specified ram address and adjust accordingly below
                        uint ramAddressOfTable = bReader.ReadUInt32();
                        if (ramAddressOfTable >= 0x03000000) {
                            MessageBox.Show("Something went wrong reading the Overworld configuration table.\nOverworld sprites in the Event Editor will be " +
                                "displayed incorrectly or not displayed at all.", "Decompression error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        if (ramAddressOfTable >= RomInfo.synthOverlayLoadAddress) { // if the pointer shows the table was moved to the synthetic overlay
                            OWTableOffset = ramAddressOfTable - RomInfo.synthOverlayLoadAddress;
                            OWtablePath = Filesystem.expArmPath;
                        } else {
                            OWTableOffset = ramAddressOfTable - ov1Address;
                            OWtablePath = ov1Path;
                        }
                    }
                    break;
            }
        }
        public static void SetConditionalMusicTableOffsetToRAMAddress() {
            switch (gameFamily) {
                case GameFamilies.HGSS:
                    switch (gameLanguage) {
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
        public static void SetBattleEffectsData() {
            switch (gameFamily) {
                case GameFamilies.HGSS:
                    switch (gameLanguage) {
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
                    switch (gameLanguage) {
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
        public static void SetEncounterMusicTableOffsetToRAMAddress() {
            switch (gameFamily) {
                case GameFamilies.HGSS:
                    switch (gameLanguage) {
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
                    switch (gameLanguage) {
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
                    switch (gameLanguage) {
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
        private void SetItemScriptFileNumber() {
            switch (gameFamily) {
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
        private void SetNullEncounterID() {
            switch (gameFamily) {
                case GameFamilies.DP:
                case GameFamilies.Plat:
                    nullEncounterID = ushort.MaxValue;
                    break;
                case GameFamilies.HGSS:
                    nullEncounterID = Byte.MaxValue;
                    break;
            }
        }
        private void SetAttackNamesTextNumber() {
            switch (gameFamily) {
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
        private void SetItemNamesTextNumber() {
            switch (gameFamily) {
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
        private void SetLocationNamesTextNumber() {
            switch (gameFamily) {
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
        private void SetPokemonNamesTextNumber() {
            switch (gameFamily) {
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
        private void SetTrainerNamesMessageNumber() {
            switch (gameFamily) {
                case GameFamilies.DP:
                    trainerNamesMessageNumber = 559;
                    if (gameLanguage.Equals(GameLanguages.Japanese)) {
                        trainerNamesMessageNumber -= 9;
                    }
                    break;
                case GameFamilies.Plat:
                    trainerNamesMessageNumber = 618;
                    break;
                default:
                    trainerNamesMessageNumber = 729;
                    if (gameLanguage == GameLanguages.Japanese) {
                        trainerNamesMessageNumber -= 10;
                    }
                    break;
            }
        }
        private void SetTrainerClassMessageNumber() {
            switch (gameFamily) {
                case GameFamilies.DP:
                    trainerClassMessageNumber = 560;
                    if (gameLanguage.Equals(GameLanguages.Japanese)) {
                        trainerClassMessageNumber -= 9;
                    }
                    break;
                case GameFamilies.Plat:
                    trainerClassMessageNumber = 619;
                    break;
                default:
                    trainerClassMessageNumber = 730;
                    if (gameLanguage.Equals(GameLanguages.Japanese)) {
                        trainerClassMessageNumber -= 10;
                    }
                    break;
            }
        }

        public string GetRomNameFromWorkdir() => workDir.Substring(0, workDir.Length - folderSuffix.Length - 1);
        public static int GetHeaderCount() => (int)new FileInfo(internalNamesPath).Length / internalNameLength;
        public static List<string> GetLocationNames() => new TextArchive(locationNamesTextNumber).messages;
        public static string[] GetSimpleTrainerNames() => new TextArchive(trainerNamesMessageNumber).messages.ToArray();
        public static string[] GetTrainerClassNames() => new TextArchive(trainerClassMessageNumber).messages.ToArray();
        public static string[] GetItemNames() => new TextArchive(itemNamesTextNumber).messages.ToArray();
        public static string[] GetItemNames(int startIndex = 0, int? count = null) {
            TextArchive itemNames = new TextArchive(itemNamesTextNumber);
            return itemNames.messages.GetRange(startIndex, count == null ? itemNames.messages.Count - 1 : (int)count).ToArray();
        }
        public static string[] GetPokemonNames() => new TextArchive(pokemonNamesTextNumbers[0]).messages.ToArray();
        public static string[] GetAttackNames() => new TextArchive(attackNamesTextNumber).messages.ToArray();
        #endregion

        #region System Methods
        private void LoadGameLanguage() {
            switch (romID) {
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
        private void LoadGameFamily() {
            switch (gameVersion) {
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
        private void SetNarcDirs() {
            Dictionary<DirNames, string> packedDirsDict = null;
            switch (gameFamily) {
                case GameFamilies.DP:
                    string suffix = "";
                    if (!gameLanguage.Equals(GameLanguages.Japanese))
                        suffix = "_release";

                    packedDirsDict = new Dictionary<DirNames, string>() {
                        [DirNames.synthOverlay] = @"data\weather_sys.narc",
                        [DirNames.textArchives] = @"msgdata\msg.narc",

                        [DirNames.matrices] = @"fielddata\mapmatrix\map_matrix.narc",

                        [DirNames.maps] = @"fielddata\land_data\land_data" + suffix + ".narc",
                        [DirNames.exteriorBuildingModels] = @"fielddata\build_model\build_model.narc",
                        [DirNames.buildingConfigFiles] = @"fielddata\areadata\area_build_model\area_build.narc",
                        [DirNames.buildingTextures] = @"fielddata\areadata\area_build_model\areabm_texset.narc",
                        [DirNames.mapTextures] = @"fielddata\areadata\area_map_tex\map_tex_set.narc",
                        [DirNames.areaData] = @"fielddata\areadata\area_data.narc",

                        [DirNames.eventFiles] = @"fielddata\eventdata\zone_event" + suffix + ".narc",
                        [DirNames.OWSprites] = @"data\mmodel\mmodel.narc",

                        [DirNames.scripts] = @"fielddata\script\scr_seq" + suffix + ".narc",

                        [DirNames.trainerProperties] = @"poketool\trainer\trdata.narc",
                        [DirNames.trainerParty] = @"poketool\trainer\trpoke.narc",
                        [DirNames.trainerGraphics] = @"poketool\trgra\trfgra.narc",

                        [DirNames.monIcons] = @"poketool\icongra\poke_icon.narc",

                        [DirNames.encounters] = @"fielddata\encountdata\" + char.ToLower(gameVersion.ToString()[0]) + '_' + "enc_data.narc"
                    };
                    break;
                case GameFamilies.Plat:
                    packedDirsDict = new Dictionary<DirNames, string>() {
                        [DirNames.synthOverlay] = @"data\weather_sys.narc",
                        [DirNames.dynamicHeaders] = @"debug\cb_edit\d_test.narc",

                        [DirNames.textArchives] = @"msgdata\" + gameVersion.ToString().Substring(0, 2).ToLower() + '_' + "msg.narc",

                        [DirNames.matrices] = @"fielddata\mapmatrix\map_matrix.narc",

                        [DirNames.maps] = @"fielddata\land_data\land_data.narc",
                        [DirNames.exteriorBuildingModels] = @"fielddata\build_model\build_model.narc",
                        [DirNames.buildingConfigFiles] = @"fielddata\areadata\area_build_model\area_build.narc",
                        [DirNames.buildingTextures] = @"fielddata\areadata\area_build_model\areabm_texset.narc",
                        [DirNames.mapTextures] = @"fielddata\areadata\area_map_tex\map_tex_set.narc",
                        [DirNames.areaData] = @"fielddata\areadata\area_data.narc",

                        [DirNames.eventFiles] = @"fielddata\eventdata\zone_event.narc",
                        [DirNames.OWSprites] = @"data\mmodel\mmodel.narc",

                        [DirNames.scripts] = @"fielddata\script\scr_seq.narc",

                        [DirNames.trainerProperties] = @"poketool\trainer\trdata.narc",
                        [DirNames.trainerParty] = @"poketool\trainer\trpoke.narc",
                        [DirNames.trainerGraphics] = @"poketool\trgra\trfgra.narc",

                        [DirNames.monIcons] = @"poketool\icongra\pl_poke_icon.narc",

                        [DirNames.encounters] = @"fielddata\encountdata\" + gameVersion.ToString().Substring(0, 2).ToLower() + '_' + "enc_data.narc"
                    };
                    break;
                case GameFamilies.HGSS:
                    packedDirsDict = new Dictionary<DirNames, string>() {
                        [DirNames.synthOverlay] = @"a\0\2\8",
                        [DirNames.dynamicHeaders] = @"a\0\5\0",

                        [DirNames.textArchives] = @"a\0\2\7",

                        [DirNames.matrices] = @"a\0\4\1",

                        [DirNames.maps] = @"a\0\6\5",
                        [DirNames.exteriorBuildingModels] = @"a\0\4\0",
                        [DirNames.buildingConfigFiles] = @"a\0\4\3",
                        [DirNames.buildingTextures] = @"a\0\7\0",
                        [DirNames.mapTextures] = @"a\0\4\4",
                        [DirNames.areaData] = @"a\0\4\2",

                        [DirNames.eventFiles] = @"a\0\3\2",
                        [DirNames.OWSprites] = @"a\0\8\1",

                        [DirNames.scripts] = @"a\0\1\2",

                        //ENCOUNTERS FOLDER DEPENDS ON VERSION
                        [DirNames.trainerProperties] = @"a\0\5\5",
                        [DirNames.trainerParty] = @"a\0\5\6",
                        [DirNames.trainerGraphics] = @"a\0\5\8",

                        [DirNames.monIcons] = @"a\0\2\0",

                        [DirNames.interiorBuildingModels] = @"a\1\4\8",

                        [DirNames.safariZone] = @"a\2\3\0",
                        [DirNames.headbutt] = @"a\2\5\2", //both versions use the same folder with different data
                    };

                    //Encounter archive is different for SS 
                    if (gameVersion == GameVersions.HeartGold) {
                        packedDirsDict[DirNames.encounters] = @"a\0\3\7";
                    } else {
                        packedDirsDict[DirNames.encounters] = @"a\1\3\6";
                    }

                    break;
            }

            gameDirs = new Dictionary<DirNames, (string packedDir, string unpackedDir)>();
            foreach (KeyValuePair<DirNames, string> kvp in packedDirsDict) {
                string _packedPath = Path.Combine(workDir, Path.Combine(dataFolderName, kvp.Value));
                string _unpackedPath = Path.Combine(unpackedPath, kvp.Key.ToString());
                gameDirs.Add(kvp.Key, (_packedPath, _unpackedPath));
            }
        }
        public void ResetMapCellsColorDictionary() {
            switch (gameFamily) {
                case GameFamilies.DP:
                case GameFamilies.Plat:
                    MapCellsColorDictionary = PokeDatabase.System.MatrixCellColors.DPPtmatrixColorsDict;
                    break;
                case GameFamilies.HGSS:
                    MapCellsColorDictionary = PokeDatabase.System.MatrixCellColors.HGSSmatrixColorsDict;
                    break;
            }
        }
        public static void ReadOWTable() {
            OverworldTable = new SortedDictionary<uint, (uint spriteID, ushort properties)>();
            switch (gameFamily) {
                case GameFamilies.DP:
                case GameFamilies.Plat:
                    using (BinaryReader idReader = new BinaryReader(new FileStream(OWtablePath, FileMode.Open))) {
                        idReader.BaseStream.Position = OWTableOffset;

                        uint entryID = idReader.ReadUInt32();
                        idReader.BaseStream.Position -= 4;
                        while ((entryID = idReader.ReadUInt32()) != 0xFFFF) {
                            uint spriteID = idReader.ReadUInt32();
                            (uint spriteID, ushort properties) tup = (spriteID, 0x0000);
                            OverworldTable.Add(entryID, tup);
                        }
                    }
                    break;
                case GameFamilies.HGSS:
                    using (BinaryReader idReader = new BinaryReader(new FileStream(OWtablePath, FileMode.Open))) {
                        idReader.BaseStream.Position = OWTableOffset;

                        ushort entryID = idReader.ReadUInt16();
                        idReader.BaseStream.Position -= 2;
                        while ((entryID = idReader.ReadUInt16()) != 0xFFFF) {
                            uint spriteID = idReader.ReadUInt16();
                            ushort properties = idReader.ReadUInt16();
                            (uint spriteID, ushort properties) tup = (spriteID, properties);
                            OverworldTable.Add(entryID, tup);
                        }
                    }
                    break;
            }
            foreach (uint k in ow3DSpriteDict.Keys) {
                OverworldTable.Add(k, (0x3D3D, 0x3D3D)); //ADD 3D overworld data (spriteID and properties are dummy values)
            }
            overworldTableKeys = OverworldTable.Keys.ToArray();
        }
        #endregion
    }
}