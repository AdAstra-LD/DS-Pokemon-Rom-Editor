using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using DSPRE.Resources;
using System;

namespace DSPRE {

    /// <summary>
    /// Class to store ROM data from GEN IV Pokémon games
    /// </summary>

    public class RomInfo {
        public static string romID { get; private set; }
        public static string workDir { get; private set; }
        public static string arm9Path { get; private set; }
        public static string overlayTablePath { get; set; }
        public static string overlayPath { get; set; }
        public static string gameVersion { get; private set; }
        public static string gameLanguage { get; private set; }
        public static string gameName { get; private set; }

        public static long headerTableOffset { get; private set; }
        public static uint arm9spawnOffset { get; private set; }
        public static int initialMoneyOverlayNumber { get; private set; }
        public static uint initialMoneyOffset { get; private set; }
        public static string syntheticOverlayPath { get; private set; }
        public static string OWSpriteDirPath { get; private set; }
        public static long OWTableOffset { get; internal set; }

        private string interiorBuildingsPath;
        private string exteriorBuildingModelsPath;

        public string areaDataDirPath { get; private set; }
        public static string OWtablePath { get; private set; }
        public string mapTexturesDirPath { get; private set; }
        public string buildingTexturesDirPath { get; private set; }
        public string buildingConfigFilesPath { get; private set; }
        public static string matrixDirPath { get; private set; }
        public static string mapDirPath { get; private set; }
        public static string eventsDirPath { get; private set; }
        public static string scriptDirPath { get; private set; }
        public static string textArchivesPath { get; private set; }
        public static string encounterDirPath { get; private set; }
        public static string trainerDataDirPath { get; private set; }
        public static string[] narcPaths { get; private set; }
        public static string[] extractedNarcDirs { get; private set; }

        public static int nullEncounterID { get; private set; }
        public static int attackNamesTextNumber { get; private set; }
        public static int[] pokemonNamesTextNumbers { get; private set; }
        public static int itemNamesTextNumber { get; private set; }
        public static int itemScriptFileNumber { get; internal set; }      
        public static int trainerClassMessageNumber { get; private set; }
        public static int trainerNamesMessageNumber { get; private set; }
        public static int locationNamesTextNumber { get; private set; }


        public static readonly byte internalNameLength = 16;
        public static string InternalNamesLocation { get; private set; }

        public Dictionary<List<uint>, (Color background, Color foreground)> MapCellsColorDictionary { get; private set; }
        public static Dictionary<ushort, string> ScriptCommandNamesDict { get; private set; }
        public static Dictionary<ushort, byte[]> CommandParametersDict { get; private set; }
        public static SortedDictionary<uint, (uint spriteID, ushort properties)> OverworldTable { get; private set; }
        public static uint[] overworldTableKeys { get; private set; }
        public static Dictionary<uint, string> ow3DSpriteDict { get; private set; }

        #region Constructors (1)
        public RomInfo(string id, string dir) {
            romID = id;
            workDir = dir;
            LoadGameVersion();
            if (gameVersion == null)
                return;

            LoadGameName();
            LoadGameLanguage();

            arm9Path = workDir + @"arm9.bin";
            overlayTablePath = workDir + @"y9.bin";
            overlayPath = workDir + "overlay";

            InternalNamesLocation = workDir + @"data\fielddata\maptable\mapname.bin";
            mapTexturesDirPath = workDir + @"unpacked\maptex";
            buildingTexturesDirPath = workDir + @"unpacked\TextureBLD";
            buildingConfigFilesPath = workDir + @"unpacked\area_build";
            areaDataDirPath = workDir + @"unpacked\area_data";
            textArchivesPath = workDir + @"unpacked\msg";
            matrixDirPath = workDir + @"unpacked\matrix";
            trainerDataDirPath = workDir + @"unpacked\trainerdata";
            mapDirPath = workDir + @"unpacked\maps";
            encounterDirPath = workDir + @"unpacked\wildPokeData";
            eventsDirPath = workDir + @"unpacked\events";
            scriptDirPath = workDir + @"unpacked\scripts";
            syntheticOverlayPath = workDir + @"unpacked\syntheticOverlayNarc";
            OWSpriteDirPath = workDir + @"unpacked\overworlds";

            SetNullEncounterID();           
            SetBuildingModelsDirPath();
            SetOWtable();
            SetInitialMoneyOverlayAndOffset();

            SetAttackNamesTextNumber();
            SetPokémonNamesTextNumber();
            SetItemNamesTextNumber();
            SetItemScriptFileNumber();
            SetLocationNamesTextNumber();
            SetTrainerNamesMessageNumber();
            SetTrainerClassMessageNumber();
            SetSpawnPointOffset();
            Set3DOverworldsDict();

            /* System */
            SetNarcDirs();
            LoadMapCellsColorDictionary();
            ScriptCommandNamesDict = BuildCommandNamesDatabase(gameVersion);
            CommandParametersDict = BuildCommandParametersDatabase(gameVersion);
            /* * * * */
        }

        private void Set3DOverworldsDict() {
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
        private void SetInitialMoneyOverlayAndOffset() {
            switch (gameVersion) {
                case "D":
                case "P":
                    initialMoneyOverlayNumber = 52;
                    initialMoneyOffset = 0x1E4;
                    break;
                case "Plat":
                    initialMoneyOverlayNumber = 57;
                    initialMoneyOffset = 0x1EC;
                    break;
                case "HG":
                case "SS":
                    initialMoneyOverlayNumber = 36;
                    initialMoneyOffset = 0x2FC;
                    break;
            }
        }
        private void SetSpawnPointOffset() {
            switch (gameVersion) {
                case "D":
                case "P":
                    switch (gameLanguage) {
                        case "ENG":
                            arm9spawnOffset = 0xF2B9C;
                            break;
                        case "ESP":
                            arm9spawnOffset = 0xF2BE8;
                            break;
                        case "ITA":
                            arm9spawnOffset = 0xF2B50;
                            break;
                        case "FRA":
                            arm9spawnOffset = 0xF2BDC;
                            break;
                        case "GER":
                            arm9spawnOffset = 0xF2BAC;
                            break;
                        case "JAP":
                            arm9spawnOffset = 0xF4B48;
                            break;
                    }
                    break;
                case "Plat":
                    switch (gameLanguage) {
                        case "ENG":
                            arm9spawnOffset = 0xEA12C;
                            break;
                        case "ESP":
                            arm9spawnOffset = 0xEA1C0;
                            break;
                        case "ITA":
                            arm9spawnOffset = 0xEA148;
                            break;
                        case "FRA":
                            arm9spawnOffset = 0xEA1B4;
                            break;
                        case "GER":
                            arm9spawnOffset = 0xEA184;
                            break;
                        case "JAP":
                            arm9spawnOffset = 0xE9800;
                            break;
                    }
                    break;
                case "HG":
                case "SS":
                    switch (gameLanguage) {
                        case "ENG":
                            arm9spawnOffset = 0xFA17C;
                            break;
                        case "ESP":
                            if (gameVersion == "HG") {
                                arm9spawnOffset = 0xFA164;
                            } else {
                                arm9spawnOffset = 0xFA16C;
                            }
                            break;
                        case "ITA":
                            arm9spawnOffset = 0xFA0F4;
                            break;
                        case "FRA":
                            arm9spawnOffset = 0xFA160;
                            break;
                        case "GER":
                            arm9spawnOffset = 0xFA130;
                            break;
                        case "JAP":
                            arm9spawnOffset = 0xF992C;
                            break;
                    }
                    break;
            }
        }
        #endregion

        #region Methods (22)
        private void SetItemScriptFileNumber() {
            switch (gameVersion) {
                case "D":
                case "P":
                    itemScriptFileNumber = 370;
                    break;
                case "Plat":
                    itemScriptFileNumber = 404;
                    break;
                default:
                    itemScriptFileNumber = 141;
                    break;
            }
        }
        private void SetNullEncounterID() {
            switch (gameVersion) {
                case "D":
                case "P":
                case "Plat":
                    nullEncounterID = 65535;
                    break;
                case "HG":
                case "SS":
                    nullEncounterID = 255;
                    break;
            }
        }
        public void SetNarcDirs () {
            extractedNarcDirs = new string[] {
                syntheticOverlayPath,
                textArchivesPath,

                matrixDirPath,

                mapDirPath,
                exteriorBuildingModelsPath,
                buildingConfigFilesPath,
                buildingTexturesDirPath,
                mapTexturesDirPath,
                areaDataDirPath,

                eventsDirPath,
                trainerDataDirPath,
                OWSpriteDirPath,

                scriptDirPath,
                encounterDirPath,

                interiorBuildingsPath
            };
            
            switch (gameVersion) {
                case "D":
                case "P":
                    narcPaths = new string[] {
                        @"data\data\weather_sys.narc",
                        @"data\msgdata\msg.narc",

                        @"data\fielddata\mapmatrix\map_matrix.narc",

                        @"data\fielddata\land_data\land_data_release.narc",
                        @"data\fielddata\build_model\build_model.narc",
                        @"data\fielddata\areadata\area_build_model\area_build.narc",
                        @"data\fielddata\areadata\area_build_model\areabm_texset.narc",
                        @"data\fielddata\areadata\area_map_tex\map_tex_set.narc",
                        @"data\fielddata\areadata\area_data.narc",

                        @"data\fielddata\eventdata\zone_event_release.narc",
                        @"data\poketool\trainer\trdata.narc",
                        @"data\data\mmodel\mmodel.narc",

                        @"data\fielddata\script\scr_seq_release.narc",
                        @"data\fielddata\encountdata\" + char.ToLower(gameVersion[0]) + '_' + "enc_data.narc"

                    };
                    break;
                case "Plat":
                    narcPaths = new string[] {
                        @"data\data\weather_sys.narc",
                        @"data\msgdata\" + gameVersion.Substring(0,2).ToLower() + '_' + "msg.narc",

                        @"data\fielddata\mapmatrix\map_matrix.narc",

                        @"data\fielddata\land_data\land_data.narc",
                        @"data\fielddata\build_model\build_model.narc",
                        @"data\fielddata\areadata\area_build_model\area_build.narc",
                        @"data\fielddata\areadata\area_build_model\areabm_texset.narc",
                        @"data\fielddata\areadata\area_map_tex\map_tex_set.narc",
                        @"data\fielddata\areadata\area_data.narc",

                        @"data\fielddata\eventdata\zone_event.narc",
                        @"data\poketool\trainer\trdata.narc",
                        @"data\data\mmodel\mmodel.narc",

                        @"data\fielddata\script\scr_seq.narc",
                        @"data\fielddata\encountdata\" + gameVersion.Substring(0,2).ToLower() + '_' + "enc_data.narc"
                    };
                    break;
                case "HG":
                case "SS":
                    narcPaths = new string[] {
                        @"data\a\0\2\8",
                        @"data\a\0\2\7",

                        @"data\a\0\4\1",

                        @"data\a\0\6\5",
                        @"data\a\0\4\0",
                        @"data\a\0\4\3",
                        @"data\a\0\7\0",
                        @"data\a\0\4\4",
                        @"data\a\0\4\2",

                        @"data\a\0\3\2",
                        @"data\a\0\5\5",
                        @"data\a\0\8\1",

                        @"data\a\0\1\2",
                        @"data\a\0\3\7",

                        @"data\a\1\4\8"
                    };
                    if (gameVersion == "SS")
                        narcPaths[narcPaths.Length - 2] = @"data\a\1\3\6"; //Fix SS encounters
                    break;
            }
        }
        public void SetBuildingModelsDirPath() {
            switch (gameVersion) {
                case "D":
                case "P":
                case "Plat":
                    exteriorBuildingModelsPath = workDir + @"unpacked\DPPtBuildings";
                    break;
                default:
                    interiorBuildingsPath = workDir + @"unpacked\HGSSBuildingsIN";
                    exteriorBuildingModelsPath = workDir + @"unpacked\HGSSBuildingsOUT";
                    break;
            }
        }
        public string GetBuildingModelsDirPath(bool interior) {
            if (interior)
                return interiorBuildingsPath;
            else
                return exteriorBuildingModelsPath;
        }
        public int GetBuildingCount(bool interior) {
            if (interior)
                return Directory.GetFiles(interiorBuildingsPath).Length;
            else
                return Directory.GetFiles(exteriorBuildingModelsPath).Length;
        }

        public void SetOWtable () {
            switch (gameVersion) {
                case "D":
                case "P":
                    OWtablePath = workDir + "overlay" + "\\" + "overlay_0005.bin";
                    switch (gameLanguage) { // Go to the beginning of the overworld table
                        case "ENG":
                            OWTableOffset = 0x22BCC;
                            break;
                        case "JAP":
                            OWTableOffset = 0x23BB8;
                            break;
                        default:
                            OWTableOffset = 0x22B84;
                            break;
                    }
                    break;
                case "Plat":
                    OWtablePath = workDir + "overlay" + "\\" + "overlay_0005.bin";
                    switch (gameLanguage) { // Go to the beginning of the overworld table
                        case "ITA":
                            OWTableOffset = 0x2BC44;
                            break;
                        case "FRA":
                        case "ESP":
                            OWTableOffset = 0x2BC3C;
                            break;
                        case "GER":
                            OWTableOffset = 0x2BC50;
                            break;
                        case "JAP":
                            OWTableOffset = 0x2BA24;
                            break;
                        default:
                            OWTableOffset = 0x2BC34;
                            break;
                    }
                    break;
                case "HG":
                case "SS":
                    OWtablePath = workDir + "overlay" + "\\" + "overlay_0001.bin";
                    OWTableOffset = 0x21BA8;
                    break;
            }
        }
        public static Dictionary<ushort, string> BuildCommandNamesDatabase(string gameVer) {
            switch (gameVer) {
                case "D":
                case "P":
                    var commonDictionaryNames = PokeDatabase.ScriptEditor.DPPtScrCmdNames;
                    var specificDictionaryNames = PokeDatabase.ScriptEditor.DPScrCmdNames;
                    return commonDictionaryNames.Concat(specificDictionaryNames).ToLookup(x => x.Key, x => x.Value).ToDictionary(x => x.Key, g => g.First());
                case "Plat":
                    commonDictionaryNames = PokeDatabase.ScriptEditor.DPPtScrCmdNames;
                    specificDictionaryNames = PokeDatabase.ScriptEditor.PlatScrCmdNames;
                    return commonDictionaryNames.Concat(specificDictionaryNames).ToLookup(x => x.Key, x => x.Value).ToDictionary(x => x.Key, g => g.First());
                default:
                    commonDictionaryNames = PokeDatabase.ScriptEditor.HGSSScrCmdNames;
                    var customDictionaryNames = PokeDatabase.ScriptEditor.CustomScrCmdNames;
                    return commonDictionaryNames.Concat(customDictionaryNames).ToLookup(x => x.Key, x => x.Value).ToDictionary(x => x.Key, g => g.First());
            }
        }        
        public static Dictionary<ushort, byte[]> BuildCommandParametersDatabase(string gameVer) {
            switch (gameVer) {
                case "D":
                case "P":
                    var commonDictionaryParams = PokeDatabase.ScriptEditor.DPPtScrCmdParameters;
                    var specificDictionaryParams = PokeDatabase.ScriptEditor.DPScrCmdParameters;
                    return commonDictionaryParams.Concat(specificDictionaryParams).ToLookup(x => x.Key, x => x.Value).ToDictionary(x => x.Key, g => g.First());
                case "Plat":
                    commonDictionaryParams = PokeDatabase.ScriptEditor.DPPtScrCmdParameters;
                    specificDictionaryParams = PokeDatabase.ScriptEditor.PlatScrCmdParameters;
                    return commonDictionaryParams.Concat(specificDictionaryParams).ToLookup(x => x.Key, x => x.Value).ToDictionary(x => x.Key, g => g.First());                 
                default:
                    commonDictionaryParams = PokeDatabase.ScriptEditor.HGSSScrCmdParameters;
                    var customDictionaryParams = PokeDatabase.ScriptEditor.CustomScrCmdParameters;
                    return commonDictionaryParams.Concat(customDictionaryParams).ToLookup(x => x.Key, x => x.Value).ToDictionary(x => x.Key, g => g.First());
            }
        }
        public void LoadGameVersion() {
            try {
                gameVersion = PokeDatabase.System.versionsDict[romID];
            } catch (KeyNotFoundException) {
                MessageBox.Show("The ROM you attempted to load is not supported.\nYou can only load Gen IV Pokémon ROMS, for now.", "Unsupported ROM",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadGameName() {
            switch (gameVersion) {
                case "D":
                    gameName = "Diamond";
                    break;
                case "P":
                    gameName = "Pearl";
                    break;
                case "Plat":
                    gameName = "Platinum";
                    break;
                case "HG":
                    gameName = "HeartGold";
                    break;
                case "SS":
                    gameName = "SoulSilver";
                    break;
            }
        }
        public void LoadGameLanguage() {
            switch (romID) {
                case "ADAE":
                case "APAE":
                case "CPUE":
                case "IPKE":
                case "IPGE":
                    gameLanguage = "ENG";
                    break;

                case "ADAS":
                case "APAS":
                case "CPUS":
                case "IPKS":
                case "IPGS":
                case "LATA":
                    gameLanguage = "ESP";
                    break;

                case "ADAI":
                case "APAI":
                case "CPUI":
                case "IPKI":
                case "IPGI":
                    gameLanguage = "ITA";
                    break;

                case "ADAF":
                case "APAF":
                case "CPUF":
                case "IPKF":
                case "IPGF":
                    gameLanguage = "FRA";
                    break;

                case "ADAD":
                case "APAD":
                case "CPUD":
                case "IPKD":
                case "IPGD":
                    gameLanguage = "GER";
                    break;

                default:
                    gameLanguage = "JAP";
                    break;
            }
        }
        public int GetHeaderCount() {
            return (int)new FileInfo(InternalNamesLocation).Length / internalNameLength;
        }
        public void SetAttackNamesTextNumber() {
            switch (gameVersion) {
                case "D":
                case "P":
                    attackNamesTextNumber = 588;
                    break;
                case "Plat":
                    attackNamesTextNumber = 647;
                    break;
                default:
                    if (gameLanguage == "JAP") {
                        attackNamesTextNumber = 739;
                    } else {
                        attackNamesTextNumber = 750;
                    }
                    break;
            }
        }
        public void SetItemNamesTextNumber() {
            switch (gameVersion) {
                case "D":
                case "P":
                    itemNamesTextNumber = 344;
                    break;
                case "Plat":
                    itemNamesTextNumber = 392;
                    break;
                default:
                    if (gameLanguage == "JAP") {
                        itemNamesTextNumber = 219;
                    } else {
                        itemNamesTextNumber = 222;
                    }
                    break;
            }
        }
        public void SetLocationNamesTextNumber() {;
            switch (gameVersion) {
                case "D":
                case "P":
                    locationNamesTextNumber = 382;
                    break;
                case "Plat":
                    locationNamesTextNumber = 433;
                    break;
                default:
                    if (gameLanguage == "JAP") {
                        locationNamesTextNumber = 272;
                    } else {
                        locationNamesTextNumber = 279;
                    }
                    break;
            }
        }
        public static void SetPokémonNamesTextNumber() {
            switch (gameVersion) {
                case "D":
                case "P":
                    pokemonNamesTextNumbers = new int[2] { 362, 363 };
                    break;
                case "Plat":
                    pokemonNamesTextNumbers = new int[7] { 412, 413, 712, 713, 714, 715, 716 }; //413?
                    break;
                case "HG":
                case "SS":
                    pokemonNamesTextNumbers = new int[7] { 237, 238, 817, 818, 819, 820, 821 }; //238?
                    break;
            }
        }
        public void SetTrainerNamesMessageNumber() {
            switch (gameVersion) {
                case "D":
                case "P":
                    trainerNamesMessageNumber = 559;
                    break;
                case "Plat":
                    trainerNamesMessageNumber = 618;
                    break;
                default:
                    if (gameLanguage == "JAP") {
                        trainerNamesMessageNumber = 719;
                    } else {
                        trainerNamesMessageNumber = 729;
                    }
                    break;
            }
        }
        public void SetTrainerClassMessageNumber() {
            switch (gameVersion) {
                case "D":
                case "P":
                    trainerClassMessageNumber = 560;
                    break;
                case "Plat":
                    trainerClassMessageNumber = 619;
                    break;
                default:
                    if (gameLanguage == "JAP") {
                        trainerClassMessageNumber = 720;
                    } else {
                        trainerClassMessageNumber = 730;
                    }
                    break;
            }
        }
        public int GetAreaDataCount() {
            return Directory.GetFiles(areaDataDirPath).Length;
        }
        public int GetMapTexturesCount() {
            return Directory.GetFiles(mapTexturesDirPath).Length;
        }
        public int GetBuildingTexturesCount() {
            return Directory.GetFiles(buildingTexturesDirPath).Length;
        }
        public int GetMatrixCount() {
            return Directory.GetFiles(matrixDirPath).Length;
        }
        public int GetTextArchivesCount() {
            return Directory.GetFiles(textArchivesPath).Length;
        }
        public int GetMapCount() {
            return Directory.GetFiles(mapDirPath).Length;
        }
        public int GetEventCount() {
            return Directory.GetFiles(eventsDirPath).Length;
        }
        public int GetScriptCount() {
            return Directory.GetFiles(scriptDirPath).Length;
        }
        #endregion

        #region System Methods
        public void LoadMapCellsColorDictionary() {
            switch (gameVersion) {
                case "D":
                case "P":
                case "Plat":
                    MapCellsColorDictionary = PokeDatabase.System.MatrixCellColors.DPPtmatrixColorsDict;
                    break;
                case "HG":
                case "SS":
                    MapCellsColorDictionary = PokeDatabase.System.MatrixCellColors.HGSSmatrixColorsDict;
                    break;
            }
        }
        public void SetMapCellsColorDictionary(Dictionary<List<uint>, (Color background, Color foreground)> dict) {
            MapCellsColorDictionary = dict;
        }
        public static void ReadOWTable () {
            OverworldTable = new SortedDictionary<uint, (uint spriteID, ushort properties)>();
            switch (gameVersion) {
                case "D":
                case "P":
                case "Plat":
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
                case "HG":
                case "SS":
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
            foreach(uint k in ow3DSpriteDict.Keys) {
                OverworldTable.Add(k, (0x3D3D, 0x3D3D)); //ADD 3D overworld data (spriteID and properties are dummy values)
            }
            overworldTableKeys = OverworldTable.Keys.ToArray();
        }
        #endregion
    }
}