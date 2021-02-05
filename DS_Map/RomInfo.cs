using System.IO;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using System;
using System.Drawing;
using System.Resources;
using System.Reflection;
using System.Linq;
using DSPRE.Resources;

namespace DSPRE {

    /// <summary>
    /// Class to store ROM data from GEN IV Pokémon games
    /// </summary>

    public class RomInfo {
        public static string romID { get; private set; }
        public string workDir { get; private set; }
        public static string arm9Path { get; private set; }
        public static string overlayTablePath { get; private set; }
        public static string gameVersion { get; private set; }
        public static string gameLanguage { get; private set; }
        public string gameName { get; private set; }

        public long headerTableOffset { get; private set; }
        public string syntheticOverlayPath { get; private set; }
        public string OWSpriteDirPath { get; private set; }

        private string interiorBuildingsPath;
        private string exteriorBuildingModelsPath;

        public string areaDataDirPath { get; private set; }
        public string OWtablePath { get; private set; }
        public string mapTexturesDirPath { get; private set; }
        public string buildingTexturesDirPath { get; private set; }
        public string buildingConfigFilesPath { get; private set; }
        public static string matrixDirPath { get; private set; }
        public static string mapDirPath { get; private set; }
        public static string eventsDirPath { get; private set; }
        public static string scriptDirPath { get; private set; }
        public static string textArchivesPath { get; private set; }
        public string encounterDirPath { get; private set; }
        public string trainerDataDirPath { get; private set; }
        public static string[] narcPaths { get; private set; }
        public static string[] extractedNarcDirs { get; private set; }

        public static int nullEncounterID { get; private set; }
        public static int attackNamesTextNumber { get; private set; }
        public static int[] pokémonNamesTextNumbers { get; private set; }
        public static int itemNamesTextNumber { get; private set; }
        public static int itemScriptFileNumber { get; internal set; }


        public static readonly byte internalNameLength = 16;
        public string internalNamesLocation { get; private set; }
        public Dictionary<List<uint>, Tuple<Color, Color>> mapCellsColorDictionary { get; private set; }
        public static Dictionary<ushort, string> scriptCommandNamesDict { get; private set; }
        public static Dictionary<ushort, byte[]> scriptParametersDict { get; private set; }
        
        #region Constructors (1)
        public RomInfo(string id, string workDir) {
            romID = id;
            this.workDir = workDir;
            LoadGameVersion();
            if (gameVersion == null)
                return;

            LoadGameName();
            LoadGameLanguage();

            arm9Path = workDir + @"arm9.bin";
            overlayTablePath = workDir + @"y9.bin";

            internalNamesLocation = this.workDir + @"data\fielddata\maptable\mapname.bin";
            mapTexturesDirPath = this.workDir + @"unpacked\maptex";
            buildingTexturesDirPath = this.workDir + @"unpacked\TextureBLD";
            buildingConfigFilesPath = this.workDir + @"unpacked\area_build";
            areaDataDirPath = this.workDir + @"unpacked\area_data";
            textArchivesPath = this.workDir + @"unpacked\msg";
            matrixDirPath = this.workDir + @"unpacked\matrix";
            trainerDataDirPath = this.workDir + @"unpacked\trainerdata";
            mapDirPath = this.workDir + @"unpacked\maps";
            encounterDirPath = this.workDir + @"unpacked\wildPokeData";
            eventsDirPath = this.workDir + @"unpacked\events";
            scriptDirPath = this.workDir + @"unpacked\scripts";
            syntheticOverlayPath = this.workDir + @"unpacked\syntheticOverlayNarc";
            OWSpriteDirPath = this.workDir + @"unpacked\overworlds";

            SetNullEncounterID();           
            SetBuildingModelsDirPath();
            SetOWtablePath();

            SetAttackNamesTextNumber();
            SetPokémonNamesTextNumber();
            SetItemNamesTextNumber();
            SetItemScriptFileNumber();

            /* System */
            SetNarcDirs();
            LoadMapCellsColorDictionary();
            SetScriptDatabases();
            /* * * * */
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

        public void SetOWtablePath () {
            switch (gameVersion) {
                case "D":
                case "P":
                case "Plat":
                    OWtablePath = workDir + "overlay" + "\\" + "overlay_0005.bin";
                    break;
                default:
                    OWtablePath = workDir + "overlay" + "\\" + "overlay_0001.bin";
                    break;
            }
        }

        public void SetScriptDatabases() {
            switch (gameVersion) {
                case "D":
                case "P":
                    var commonDictionaryNames = PokeDatabase.ScriptEditor.DPPtScrCmdNames;
                    var specificDictionaryNames = PokeDatabase.ScriptEditor.DPScrCmdNames;
                    scriptCommandNamesDict = commonDictionaryNames.Concat(specificDictionaryNames).ToLookup(x => x.Key, x => x.Value).ToDictionary(x => x.Key, g => g.First());

                    var commonDictionaryParams = PokeDatabase.ScriptEditor.DPPtScrCmdParameters;
                    var specificDictionaryParams = PokeDatabase.ScriptEditor.DPScrCmdParameters;
                    scriptParametersDict = commonDictionaryParams.Concat(specificDictionaryParams).ToLookup(x => x.Key, x => x.Value).ToDictionary(x => x.Key, g => g.First());
                    break;
                case "Plat":
                    commonDictionaryNames = PokeDatabase.ScriptEditor.DPPtScrCmdNames;
                    specificDictionaryNames = PokeDatabase.ScriptEditor.PlatScrCmdNames;
                    scriptCommandNamesDict = commonDictionaryNames.Concat(specificDictionaryNames).ToLookup(x => x.Key, x => x.Value).ToDictionary(x => x.Key, g => g.First());

                    commonDictionaryParams = PokeDatabase.ScriptEditor.DPPtScrCmdParameters;
                    specificDictionaryParams = PokeDatabase.ScriptEditor.PlatScrCmdParameters;
                    scriptParametersDict = commonDictionaryParams.Concat(specificDictionaryParams).ToLookup(x => x.Key, x => x.Value).ToDictionary(x => x.Key, g => g.First()); break;
                    break;
                default:
                    scriptCommandNamesDict = PokeDatabase.ScriptEditor.HGSSScrCmdNames;
                    scriptParametersDict = PokeDatabase.ScriptEditor.HGSSScrCmdParameters;
                    break;
            }
        }
        public void LoadGameVersion() {
            try {
                gameVersion = PokeDatabase.System.versionsDict[romID];
            } catch (KeyNotFoundException) {
                System.Windows.Forms.MessageBox.Show("The ROM you attempted to load is not supported.\nYou can only load Gen IV Pokémon ROMS, for now.", "Unsupported ROM",
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
            return (int)new FileInfo(internalNamesLocation).Length / internalNameLength;
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
        public int GetLocationNamesTextNumber() {
            int fileNumber;
            switch (gameVersion) {
                case "D":
                case "P":
                    fileNumber = 382;
                    break;
                case "Plat":
                    fileNumber = 433;
                    break;
                default:
                    if (gameLanguage == "JAP") {
                        fileNumber = 272;
                    } else {
                        fileNumber = 279;
                    }
                    break;
            }
            return fileNumber;
        }
        public static void SetPokémonNamesTextNumber() {
            switch (gameVersion) {
                case "D":
                case "P":
                    pokémonNamesTextNumbers = new int[2] { 362, 363 };
                    break;
                case "Plat":
                    pokémonNamesTextNumbers = new int[6] { 412, 712, 713, 714, 715, 716}; //also 413?
                    break;
                case "HG":
                case "SS":
                    pokémonNamesTextNumbers = new int[6] { 237, 817, 818, 819, 820, 821}; //also 238?
                    break;
            }
        }
        public int GetTrainerNamesMessageNumber() {
            int fileNumber;
            switch (gameVersion) {
                case "D":
                case "P":
                    fileNumber = 559;
                    break;
                case "Plat":
                    fileNumber = 618;
                    break;
                default:
                    if (gameLanguage == "JAP") {
                        fileNumber = 719;
                    } else {
                        fileNumber = 729;
                    }
                    break;
            }
            return fileNumber;
        }
        public int GetTrainerClassMessageNumber() {
            int fileNumber;
            switch (gameVersion) {
                case "D":
                case "P":
                    fileNumber = 560;
                    break;
                case "Plat":
                    fileNumber = 619;
                    break;
                default:
                    if (gameLanguage == "JAP") {
                        fileNumber = 720;
                    } else {
                        fileNumber = 730;
                    }
                    break;
            }
            return fileNumber;
        }
        public int GetAreaDataCount() {
            return Directory.GetFiles(areaDataDirPath).Length; ;
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
                    mapCellsColorDictionary = PokeDatabase.System.MatrixCellColors.DPPtmatrixColorsDict;
                    break;
                case "HG":
                case "SS":
                    mapCellsColorDictionary = PokeDatabase.System.MatrixCellColors.HGSSmatrixColorsDict;
                    break;
            }
        }
        public void SetMapCellsColorDictionary(Dictionary<List<uint>, Tuple<Color, Color>> dict) {
            mapCellsColorDictionary = dict;
        }
        #endregion
    }
}