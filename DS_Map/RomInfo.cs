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
        public static string folderSuffix = "_DSPRE_contents";
        public static string romID { get; private set; }
        public static string fileName { get; private set; }
        public static string workDir { get; private set; }
        public static string arm9Path { get; private set; }
        public static string overlayTablePath { get; set; }
        public static string overlayPath { get; set; }

        public static gLangEnum gameLanguage { get; private set; }
        public static gVerEnum gameVersion { get; private set; }
        public static gFamEnum gameFamily { get; private set; }

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

        public static string internalNamesLocation { get; private set; }
        public static readonly byte internalNameLength = 16;
        public static int cameraSize { get; private set; }

        public static Dictionary<List<uint>, (Color background, Color foreground)> MapCellsColorDictionary { get; private set; }
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

        public enum gVerEnum : byte {
            Diamond, Pearl, Platinum,
            HeartGold, SoulSilver,
            Black, White,
            Black2, White2
        }
        public enum gFamEnum : byte {
            NULL,
            DP,
            Plat,
            HGSS,
            BW,
            BW2
        }
        public enum gLangEnum : byte {
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

            trainerProperties,
            trainerParty,
            trainerGraphics,

            interiorBuildingModels
        };
        public static Dictionary<DirNames, (string packedDir, string unpackedDir)> gameDirs { get; private set; }


        #region Constructors (1)
        public RomInfo(string id, string romName, bool useSuffix = true) {
            if (!useSuffix) {
                folderSuffix = "";
            }

            workDir = Path.GetDirectoryName(romName) + "\\" + Path.GetFileNameWithoutExtension(romName) + folderSuffix + "\\";
            arm9Path = workDir + @"arm9.bin";
            overlayTablePath = workDir + @"y9.bin";
            overlayPath = workDir + "overlay";
            internalNamesLocation = workDir + @"data\fielddata\maptable\mapname.bin";

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
            SetPokémonNamesTextNumber();
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
        public static Dictionary<ushort, string> BuildCommandNamesDatabase(gFamEnum gameFam) {
            Dictionary<ushort, string> commonDictionaryNames;
            Dictionary<ushort, string> specificDictionaryNames;

            switch (gameFam) {
                case gFamEnum.DP:
                    commonDictionaryNames = ScriptDatabase.DPPtScrCmdNames;
                    specificDictionaryNames = ScriptDatabase.DPScrCmdNames;
                    break;
                case gFamEnum.Plat:
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
        public static Dictionary<ushort, byte[]> BuildCommandParametersDatabase(gFamEnum gameFam) {
            Dictionary<ushort, byte[]> commonDictionaryParams;
            Dictionary<ushort, byte[]> specificDictionaryParams;

            switch (gameFam) {
                case gFamEnum.DP:
                    commonDictionaryParams = ScriptDatabase.DPPtScrCmdParameters;
                    specificDictionaryParams = ScriptDatabase.DPScrCmdParameters;
                    break;
                case gFamEnum.Plat:
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
        public static Dictionary<ushort, string> BuildActionNamesDatabase(gFamEnum gameFam) {
            switch (gameFam) {
                case gFamEnum.DP:
                case gFamEnum.Plat:
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
        public static Dictionary<ushort, string> BuildComparisonOperatorsDatabase(gFamEnum gameFam) {
            switch (gameFam) {
                case gFamEnum.DP:
                case gFamEnum.Plat:
                case gFamEnum.HGSS:
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
                case gFamEnum.DP:
                    switch (gameLanguage) {
                        case gLangEnum.English:
                            headerTableOffset = 0xEEDBC;
                            break;
                        case gLangEnum.Spanish:
                            headerTableOffset = 0xEEE08;
                            break;
                        case gLangEnum.Italian:
                            headerTableOffset = 0xEED70;
                            break;
                        case gLangEnum.French:
                            headerTableOffset = 0xEEDFC;
                            break;
                        case gLangEnum.German:
                            headerTableOffset = 0xEEDCC;
                            break;
                        case gLangEnum.Japanese:
                            headerTableOffset = gameVersion == gVerEnum.Diamond ? (uint)0xF0D68 : 0xF0D6C;
                            break;
                    }
                    break;
                case gFamEnum.Plat:
                    switch (gameLanguage) {
                        case gLangEnum.English:
                            headerTableOffset = 0xE601C;
                            break;
                        case gLangEnum.Spanish:
                            headerTableOffset = 0xE60B0;
                            break;
                        case gLangEnum.Italian:
                            headerTableOffset = 0xE6038;
                            break;
                        case gLangEnum.French:
                            headerTableOffset = 0xE60A4;
                            break;
                        case gLangEnum.German:
                            headerTableOffset = 0xE6074;
                            break;
                        case gLangEnum.Japanese:
                            headerTableOffset = 0xE56F0;
                            break;
                    }
                    break;
                case gFamEnum.HGSS:
                    switch (gameLanguage) {
                        case gLangEnum.English:
                            headerTableOffset = 0xF6BE0;
                            break;
                        case gLangEnum.Spanish:
                            headerTableOffset = gameVersion == gVerEnum.HeartGold ? 0xF6BC8 : (uint)0xF6BD0;
                            break;
                        case gLangEnum.Italian:
                            headerTableOffset = 0xF6B58;
                            break;
                        case gLangEnum.French:
                            headerTableOffset = 0xF6BC4;
                            break;
                        case gLangEnum.German:
                            headerTableOffset = 0xF6B94;
                            break;
                        case gLangEnum.Japanese:
                            headerTableOffset = 0xF6390;
                            break;
                    }
                    break;
            }
        }
        public static void SetupSpawnSettings() {
            switch (gameFamily) {
                case gFamEnum.DP:
                    initialMoneyOverlayNumber = 52;
                    initialMoneyOverlayOffset = 0x1E4;
                    switch (gameLanguage) {
                        case gLangEnum.English:
                            arm9spawnOffset = 0xF2B9C;
                            break;
                        case gLangEnum.Spanish:
                            arm9spawnOffset = 0xF2BE8;
                            break;
                        case gLangEnum.Italian:
                            arm9spawnOffset = 0xF2B50;
                            break;
                        case gLangEnum.French:
                            arm9spawnOffset = 0xF2BDC;
                            break;
                        case gLangEnum.German:
                            arm9spawnOffset = 0xF2BAC;
                            break;
                        case gLangEnum.Japanese:
                            arm9spawnOffset = 0xF4B48;
                            break;
                    }
                    break;
                case gFamEnum.Plat:
                    initialMoneyOverlayNumber = 57;
                    initialMoneyOverlayOffset = 0x1EC;
                    switch (gameLanguage) {
                        case gLangEnum.English:
                            arm9spawnOffset = 0xEA12C;
                            break;
                        case gLangEnum.Spanish:
                            arm9spawnOffset = 0xEA1C0;
                            break;
                        case gLangEnum.Italian:
                            arm9spawnOffset = 0xEA148;
                            break;
                        case gLangEnum.French:
                            arm9spawnOffset = 0xEA1B4;
                            break;
                        case gLangEnum.German:
                            arm9spawnOffset = 0xEA184;
                            break;
                        case gLangEnum.Japanese:
                            arm9spawnOffset = 0xE9800;
                            break;
                    }
                    break;
                case gFamEnum.HGSS:
                    initialMoneyOverlayNumber = 36;
                    initialMoneyOverlayOffset = 0x2FC;
                    switch (gameLanguage) {
                        case gLangEnum.English:
                            arm9spawnOffset = 0xFA17C;
                            break;
                        case gLangEnum.Spanish:
                            arm9spawnOffset = gameVersion == gVerEnum.HeartGold ? 0xFA164 : (uint)0xFA16C;
                            break;
                        case gLangEnum.Italian:
                            arm9spawnOffset = 0xFA0F4;
                            break;
                        case gLangEnum.French:
                            arm9spawnOffset = 0xFA160;
                            break;
                        case gLangEnum.German:
                            arm9spawnOffset = 0xFA130;
                            break;
                        case gLangEnum.Japanese:
                            arm9spawnOffset = 0xF992C;
                            break;
                    }
                    break;
            }
        }
        public static void PrepareCameraData() {
            switch (gameFamily) {
                case gFamEnum.DP:
                    cameraTblOverlayNumber = 5;
                    cameraTblOffsetsToRAMaddress = gameLanguage.Equals(gLangEnum.Japanese) ? (new uint[] { 0x4C50 }) : (new uint[] { 0x4908 });
                    cameraSize = 24;
                    break;
                case gFamEnum.Plat:
                    cameraTblOverlayNumber = 5;
                    cameraTblOffsetsToRAMaddress = new uint[] { 0x4E24 };
                    cameraSize = 24;
                    break;
                case gFamEnum.HGSS:
                    cameraTblOverlayNumber = 1;
                    cameraSize = 36;
                    switch (gameLanguage) {
                        case gLangEnum.English:
                        case gLangEnum.Spanish:
                        case gLangEnum.French:
                        case gLangEnum.German:
                        case gLangEnum.Italian:
                            cameraTblOffsetsToRAMaddress = new uint[] { 0x532C, 0x547C };
                            break;
                        case gLangEnum.Japanese:
                            cameraTblOffsetsToRAMaddress = new uint[] { 0x5324, 0x5474 };
                            break;
                    }
                    break;
            }
        }
        public static void SetOWtable() {
            switch (gameFamily) {
                case gFamEnum.DP:
                    OWtablePath = DSUtils.GetOverlayPath(5);
                    switch (gameLanguage) { // Go to the beginning of the overworld table
                        case gLangEnum.English:
                            OWTableOffset = 0x22BCC;
                            break;
                        case gLangEnum.Japanese:
                            OWTableOffset = 0x23BB8;
                            break;
                        default:
                            OWTableOffset = 0x22B84;
                            break;
                    }
                    break;
                case gFamEnum.Plat:
                    OWtablePath = DSUtils.GetOverlayPath(5);
                    switch (gameLanguage) { // Go to the beginning of the overworld table
                        case gLangEnum.Italian:
                            OWTableOffset = 0x2BC44;
                            break;
                        case gLangEnum.French:
                        case gLangEnum.Spanish:
                            OWTableOffset = 0x2BC3C;
                            break;
                        case gLangEnum.German:
                            OWTableOffset = 0x2BC50;
                            break;
                        case gLangEnum.Japanese:
                            OWTableOffset = 0x2BA24;
                            break;
                        default:
                            OWTableOffset = 0x2BC34;
                            break;
                    }
                    break;
                case gFamEnum.HGSS:
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

                    using (DSUtils.EasyReader bReader = new DSUtils.EasyReader(ov1Path, 0x1F92FC - (ov1Address - DSUtils.ARM9.address))) { // read the pointer at 0x021F92FC and adjust accordingly below
                        uint ramAddress = bReader.ReadUInt32();
                        
                        if (ramAddress >= ROMToolboxDialog.synthOverlayLoadAddress) { // if the pointer shows the table was moved to the synthetic overlay
                            OWTableOffset = ramAddress - ROMToolboxDialog.synthOverlayLoadAddress;
                            OWtablePath = gameDirs[DirNames.synthOverlay].unpackedDir + "\\" + ROMToolboxDialog.expandedARMfileID.ToString("D4");
                        } else {
                            OWTableOffset = ramAddress - ov1Address;
                            OWtablePath = ov1Path;
                        }
                    }
                    break;
            }
        }
        public static void SetConditionalMusicTableOffsetToRAMAddress() {
            switch (gameFamily) {
                case gFamEnum.HGSS:
                    switch (gameLanguage) {                            
                        case gLangEnum.Spanish:
                            conditionalMusicTableOffsetToRAMAddress = gameVersion == gVerEnum.HeartGold ? (uint)0x667D0 : 0x667D8;
                            break;
                        case gLangEnum.English:
                        case gLangEnum.Italian:
                        case gLangEnum.French:
                        case gLangEnum.German:
                            conditionalMusicTableOffsetToRAMAddress = 0x667D8;
                            break;
                        case gLangEnum.Japanese:
                            conditionalMusicTableOffsetToRAMAddress = 0x66238;
                            break;
                    }
                break;
            }
        }
        public static void SetBattleEffectsData() {
            switch (gameFamily) {
                case gFamEnum.HGSS:
                    switch (gameLanguage) {
                        case gLangEnum.Spanish:
                            vsPokemonEntryTableOffsetToRAMAddress = gameVersion == gVerEnum.HeartGold ? (uint)0x518CC : 0x518D4;
                            vsTrainerEntryTableOffsetToRAMAddress = gameVersion == gVerEnum.HeartGold ? (uint)0x51888 : 0x51890;
                            effectsComboTableOffsetToRAMAddress = gameVersion == gVerEnum.HeartGold ? (uint)0x517C0 : 0x517C8;
                            break;
                        case gLangEnum.English:
                        case gLangEnum.Italian:
                        case gLangEnum.French:
                        case gLangEnum.German:
                            vsPokemonEntryTableOffsetToRAMAddress = 0x518D4;
                            vsTrainerEntryTableOffsetToRAMAddress = 0x51890;
                            effectsComboTableOffsetToRAMAddress = 0x517C8;
                            break;
                        case gLangEnum.Japanese:
                            vsPokemonEntryTableOffsetToRAMAddress = 0x5136C;
                            vsTrainerEntryTableOffsetToRAMAddress = 0x51328;
                            effectsComboTableOffsetToRAMAddress = 0x51260;
                            break;
                    }
                    vsPokemonEntryTableOffsetToSizeLimiter = vsPokemonEntryTableOffsetToRAMAddress - 0xA;
                    vsTrainerEntryTableOffsetToSizeLimiter = vsTrainerEntryTableOffsetToRAMAddress - 0xA;
                    effectsComboTableOffsetToSizeLimiter = effectsComboTableOffsetToRAMAddress - 0x1E;
                    break;

                case gFamEnum.Plat:
                    switch (gameLanguage) {
                        case gLangEnum.English:
                            effectsComboTableOffsetToRAMAddress = 0x51BE0;
                            break;
                        case gLangEnum.Italian:
                        case gLangEnum.French:
                        case gLangEnum.Spanish:
                        case gLangEnum.German:
                            effectsComboTableOffsetToRAMAddress = 0x51C84;
                            break;
                        case gLangEnum.Japanese:
                            effectsComboTableOffsetToRAMAddress = 0x514C0;
                            break;
                    }
                    break;
            }
        }
        public static void SetEncounterMusicTableOffsetToRAMAddress() {
            switch (gameFamily) {
                case gFamEnum.HGSS:
                    switch (gameLanguage) {
                        case gLangEnum.Spanish:
                            encounterMusicTableOffsetToRAMAddress = gameVersion == gVerEnum.HeartGold ? (uint)0x550D8 : 0x550E0;
                            break;
                        case gLangEnum.English:
                        case gLangEnum.Italian:
                        case gLangEnum.French:
                        case gLangEnum.German:
                            encounterMusicTableOffsetToRAMAddress = 0x550E0;
                            break;
                        case gLangEnum.Japanese:
                            encounterMusicTableOffsetToRAMAddress = 0x54B44;
                            break;
                    }
                break;

                case gFamEnum.Plat:
                    switch (gameLanguage) {
                        case gLangEnum.English:
                            encounterMusicTableOffsetToRAMAddress = 0x5563C;
                            break;
                        case gLangEnum.Italian:
                        case gLangEnum.French:
                        case gLangEnum.Spanish:
                        case gLangEnum.German:
                            encounterMusicTableOffsetToRAMAddress = 0x556E0;
                            break;
                        case gLangEnum.Japanese:
                            encounterMusicTableOffsetToRAMAddress = 0x54F04; 
                            break;
                    }
                    break;

                case gFamEnum.DP:
                    switch (gameLanguage) {
                        case gLangEnum.English:
                            encounterMusicTableOffsetToRAMAddress = 0x4AD3C;
                            break;
                        case gLangEnum.Italian:
                        case gLangEnum.French:
                        case gLangEnum.Spanish:
                        case gLangEnum.German:
                            encounterMusicTableOffsetToRAMAddress = 0x4ADAC;
                            break;
                        case gLangEnum.Japanese:
                            encounterMusicTableOffsetToRAMAddress = 0x4D9AC;
                            break;
                    } 
                break;
            }
        }
        private void SetItemScriptFileNumber() {
            switch (gameFamily) {
                case gFamEnum.DP:
                    itemScriptFileNumber = 370;
                    break;
                case gFamEnum.Plat:
                    itemScriptFileNumber = 404;
                    break;
                default:
                    itemScriptFileNumber = 141;
                    break;
            }
        }
        private void SetNullEncounterID() {
            switch (gameFamily) {
                case gFamEnum.DP:
                case gFamEnum.Plat:
                    nullEncounterID = ushort.MaxValue;
                    break;
                case gFamEnum.HGSS:
                    nullEncounterID = Byte.MaxValue;
                    break;
            }
        }
        private void SetAttackNamesTextNumber() {
            switch (gameFamily) {
                case gFamEnum.DP:
                    attackNamesTextNumber = 588;
                    break;
                case gFamEnum.Plat:
                    attackNamesTextNumber = 647;
                    break;
                default:
                    attackNamesTextNumber = gameLanguage == gLangEnum.Japanese ? 739 : 750;
                    break;
            }
        }
        private void SetItemNamesTextNumber() {
            switch (gameFamily) {
                case gFamEnum.DP:
                    itemNamesTextNumber = 344;
                    break;
                case gFamEnum.Plat:
                    itemNamesTextNumber = 392;
                    break;
                default:
                    itemNamesTextNumber = gameLanguage == gLangEnum.Japanese ? 219 : 222;
                    break;
            }
        }
        private void SetLocationNamesTextNumber() {
            switch (gameFamily) {
                case gFamEnum.DP:
                    locationNamesTextNumber = 382;
                    break;
                case gFamEnum.Plat:
                    locationNamesTextNumber = 433;
                    break;
                default:
                    locationNamesTextNumber = gameLanguage == gLangEnum.Japanese ? 272 : 279;
                    break;
            }
        }
        private void SetPokémonNamesTextNumber() {
            switch (gameFamily) {
                case gFamEnum.DP:
                    pokemonNamesTextNumbers = new int[2] { 362, 363 };
                    break;
                case gFamEnum.Plat:
                    pokemonNamesTextNumbers = new int[7] { 412, 413, 712, 713, 714, 715, 716 }; //413?
                    break;
                case gFamEnum.HGSS:
                    pokemonNamesTextNumbers = gameLanguage.Equals(gLangEnum.Japanese) ? new int[1] { 232 } : new int[7] { 237, 238, 817, 818, 819, 820, 821 }; //238?
                    break;
            }
        }
        private void SetTrainerNamesMessageNumber() {
            switch (gameFamily) {
                case gFamEnum.DP:
                    trainerNamesMessageNumber = 559;
                    if (gameLanguage.Equals(gLangEnum.Japanese)) {
                        trainerNamesMessageNumber -= 9;
                    }
                    break;
                case gFamEnum.Plat:
                    trainerNamesMessageNumber = 618;
                    break;
                default:
                    trainerNamesMessageNumber = 729;
                    if (gameLanguage == gLangEnum.Japanese) {
                        trainerNamesMessageNumber -= 10;
                    }
                    break;
            }
        }
        private void SetTrainerClassMessageNumber() {
            switch (gameFamily) {
                case gFamEnum.DP:
                    trainerClassMessageNumber = 560;
                    if (gameLanguage.Equals(gLangEnum.Japanese)) {
                        trainerClassMessageNumber -= 9;
                    }
                    break;
                case gFamEnum.Plat:
                    trainerClassMessageNumber = 619;
                    break;
                default:
                    trainerClassMessageNumber = 730;
                    if (gameLanguage.Equals(gLangEnum.Japanese)) {
                        trainerClassMessageNumber -= 10;
                    }
                    break;
            }
        }

        public string GetBuildingModelsDirPath(bool interior) => interior ? gameDirs[DirNames.interiorBuildingModels].unpackedDir : gameDirs[DirNames.exteriorBuildingModels].unpackedDir;
        public string GetRomNameFromWorkdir() => workDir.Substring(0, workDir.Length - folderSuffix.Length - 1);
        public static int GetHeaderCount() => (int)new FileInfo(internalNamesLocation).Length / internalNameLength;
        public static List<string> GetLocationNames() => new TextArchive(locationNamesTextNumber).messages;
        public static string[] GetSimpleTrainerNames() => new TextArchive(trainerNamesMessageNumber).messages.ToArray();
        public static string[] GetTrainerClassNames() => new TextArchive(trainerClassMessageNumber).messages.ToArray();
        public static string[] GetItemNames() => new TextArchive(itemNamesTextNumber).messages.ToArray();
        public static string[] GetItemNames(int startIndex = 0, int? count = null) {
            TextArchive itemNames = new TextArchive(itemNamesTextNumber);
            return itemNames.messages.GetRange(startIndex, count == null ? itemNames.messages.Count-1 : (int)count).ToArray();
        }
        public static string[] GetPokémonNames() => new TextArchive(pokemonNamesTextNumbers[0]).messages.ToArray();
        public static string[] GetAttackNames() => new TextArchive(attackNamesTextNumber).messages.ToArray();
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
#endregion

#region System Methods
        private void LoadGameLanguage() {
            switch (romID) {
                case "ADAE":
                case "APAE":
                case "CPUE":
                case "IPKE":
                case "IPGE":
                    gameLanguage = gLangEnum.English;
                    break;

                case "ADAS":
                case "APAS":
                case "CPUS":
                case "IPKS":
                case "IPGS":
                case "LATA":
                    gameLanguage = gLangEnum.Spanish;
                    break;

                case "ADAI":
                case "APAI":
                case "CPUI":
                case "IPKI":
                case "IPGI":
                    gameLanguage = gLangEnum.Italian;
                    break;

                case "ADAF":
                case "APAF":
                case "CPUF":
                case "IPKF":
                case "IPGF":
                    gameLanguage = gLangEnum.French;
                    break;

                case "ADAD":
                case "APAD":
                case "CPUD":
                case "IPKD":
                case "IPGD":
                    gameLanguage = gLangEnum.German;
                    break;

                default:
                    gameLanguage = gLangEnum.Japanese;
                    break;
            }
        }
        private void LoadGameFamily() {
            switch (gameVersion) {
                case gVerEnum.Diamond:
                case gVerEnum.Pearl:
                    gameFamily = gFamEnum.DP;
                    break;
                case gVerEnum.Platinum:
                    gameFamily = gFamEnum.Plat;
                    break;
                case gVerEnum.HeartGold:
                case gVerEnum.SoulSilver:
                    gameFamily = gFamEnum.HGSS;
                    break;
            }
        } 
        private void SetNarcDirs() {
            Dictionary<DirNames, string> packedDirsDict = null;
            switch (gameFamily) {
                case gFamEnum.DP:
                    string suffix = "";
                    if (!gameLanguage.Equals(gLangEnum.Japanese))
                        suffix = "_release";

                    packedDirsDict = new Dictionary<DirNames, string>() {
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

                        [DirNames.encounters] = @"data\fielddata\encountdata\" + char.ToLower(gameVersion.ToString()[0]) + '_' + "enc_data.narc"
                    };
                    break;
                case gFamEnum.Plat:
                    packedDirsDict = new Dictionary<DirNames, string>() {
                        [DirNames.synthOverlay] = @"data\data\weather_sys.narc",
                        [DirNames.dynamicHeaders] = @"data\debug\cb_edit\d_test.narc",

                        [DirNames.textArchives] = @"data\msgdata\" + gameVersion.ToString().Substring(0, 2).ToLower() + '_' + "msg.narc",

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

                        [DirNames.encounters] = @"data\fielddata\encountdata\" + gameVersion.ToString().Substring(0, 2).ToLower() + '_' + "enc_data.narc"
                    };
                    break;
                case gFamEnum.HGSS:
                    packedDirsDict = new Dictionary<DirNames, string>() {
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

                        [DirNames.interiorBuildingModels] = @"data\a\1\4\8"
                    };

                    //Encounter archive is different for SS 
                    packedDirsDict[DirNames.encounters] = gameVersion == gVerEnum.HeartGold ? @"data\a\0\3\7" : @"data\a\1\3\6";
                    break;
            }

            gameDirs = new Dictionary<DirNames, (string packedDir, string unpackedDir)>();
            foreach (KeyValuePair<DirNames, string> kvp in packedDirsDict) {
                gameDirs.Add(kvp.Key, (workDir + kvp.Value, workDir + @"unpacked" + '\\' + kvp.Key.ToString()));
            }
        }
        public static void LoadMapCellsColorDictionary() {
            switch (gameFamily) {
                case gFamEnum.DP:
                case gFamEnum.Plat:
                    MapCellsColorDictionary = PokeDatabase.System.MatrixCellColors.DPPtmatrixColorsDict;
                    break;
                case gFamEnum.HGSS:
                    MapCellsColorDictionary = PokeDatabase.System.MatrixCellColors.HGSSmatrixColorsDict;
                    break;
            }
        }
        public void SetMapCellsColorDictionary(Dictionary<List<uint>, (Color background, Color foreground)> dict) {
            MapCellsColorDictionary = dict;
        }
        public static void ReadOWTable() {
            OverworldTable = new SortedDictionary<uint, (uint spriteID, ushort properties)>();
            switch (gameFamily) {
                case gFamEnum.DP:
                case gFamEnum.Plat:
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
                case gFamEnum.HGSS:
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