using System.IO;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;

namespace DSPRE {

    /// <summary>
    /// Class to store ROM data from GEN IV Pokémon games
    /// </summary>

    public class RomInfo {
        private string romID;
        private string workDir;
        private string gameVersion;
        private string language;

        private string interiorBuildingsPath;
        private string exteriorBuildingsPath;
        private string areaDataDirPath;
        private string OWtablePath;
        private string mapTexturesDirPath;
        private string buildingTexturesDirPath;
        private string matrixDirPath;
        private string mapDirPath;
        private string eventsDirPath;
        private string scriptDirPath;
        private string textArchivesPath;
        private string syntheticOverlayPath;

        private string[] narcPaths;
        private string[] extractedNarcDirs;


        #region Constructors (1)
        public RomInfo(string id, string workDir) {
            romID = id;
            gameVersion = loadGameVersion();
            language = loadGameLanguage();
            this.workDir = workDir;

            SetSyntheticOverlayPath();

            SetBuildingModelsDirPath();
            SetAreaDataDirPath();
            SetOWtablePath();
            SetMapTexturesDirPath();
            SetBuildingTexturesDirPath();
            SetEventsDirPath();
            SetMatrixDirPath();
            SetTextArchivesPath();
            SetMapDirPath();
            SetScriptDirPath();

            SetNarcDirs();
        }
        #endregion

        #region Methods (22)
        public string GetWorkingFolder() {
            return workDir;
        }
        public string loadGameLanguage() {
            string language;

            switch (romID) {
                case "ADAE":
                case "APAE":
                case "CPUE":
                case "IPKE":
                case "IPGE":
                    language = "USA";
                    break;

                case "ADAS":
                case "APAS":
                case "CPUS":
                case "IPKS":
                case "IPGS":
                case "LATA":
                    language = "ESP";
                    break;

                case "ADAI":
                case "APAI":
                case "CPUI":
                case "IPKI":
                case "IPGI":
                    language = "ITA";
                    break;

                case "ADAF":
                case "APAF":
                case "CPUF":
                case "IPKF":
                case "IPGF":
                    language = "FRA";
                    break;

                case "ADAD":
                case "APAD":
                case "CPUD":
                case "IPKD":
                case "IPGD":
                    language = "GER";
                    break;

                default:
                    language = "JAP";
                    break;
            }
            return language;
        }

        public string GetGameLanguage() {
            return this.language;
        }

        public string GetGameVersion() {
            return gameVersion;
        }
        public void SetNarcDirs () {
            switch (gameVersion) {
                case "Diamond":
                case "Pearl":
                    extractedNarcDirs = new string[] {
                        syntheticOverlayPath,
                        GetBuildingTexturesDirPath(),
                        workDir + @"unpacked\area_build",
                        areaDataDirPath,
                        mapTexturesDirPath,
                        exteriorBuildingsPath,
                        GetEncounterDirPath(),
                        eventsDirPath,
                        mapDirPath,
                        matrixDirPath,
                        textArchivesPath,
                        GetScriptDirPath(),
                        GetOWSpriteDirPath(),
                        GetTrainerDataDirPath(),
                    };
                    narcPaths = new string[] {
                        @"data\data\weather_sys.narc",
                        @"data\fielddata\areadata\area_build_model\areabm_texset.narc",
                        @"data\fielddata\areadata\area_build_model\area_build.narc",
                        @"data\fielddata\areadata\area_data.narc",
                        @"data\fielddata\areadata\area_map_tex\map_tex_set.narc",
                        @"data\fielddata\build_model\build_model.narc",
                        @"data\fielddata\encountdata\" + char.ToLower(gameVersion[0]) + "_enc_data.narc",
                        @"data\fielddata\eventdata\zone_event_release.narc",
                        @"data\fielddata\land_data\land_data_release.narc",
                        @"data\fielddata\mapmatrix\map_matrix.narc",
                        @"data\msgdata\msg.narc",
                        @"data\fielddata\script\scr_seq_release.narc",
                        @"data\data\mmodel\mmodel.narc",
                        @"data\poketool\trainer\trdata.narc"
                    };
                    break;
                case "Platinum":
                    extractedNarcDirs = new string[] {
                        syntheticOverlayPath,
                        GetBuildingTexturesDirPath(),
                        workDir + @"unpacked\area_build",
                        areaDataDirPath,
                        mapTexturesDirPath,
                        exteriorBuildingsPath,
                        GetEncounterDirPath(),
                        eventsDirPath,
                        mapDirPath,
                        matrixDirPath,
                        textArchivesPath,
                        scriptDirPath,
                        GetOWSpriteDirPath(),
                        GetTrainerDataDirPath()
                    };
                    narcPaths = new string[] {
                        @"data\data\weather_sys.narc",
                        @"data\fielddata\areadata\area_build_model\areabm_texset.narc",
                        @"data\fielddata\areadata\area_build_model\area_build.narc",
                        @"data\fielddata\areadata\area_data.narc",
                        @"data\fielddata\areadata\area_map_tex\map_tex_set.narc",
                        @"data\fielddata\build_model\build_model.narc",
                        @"data\fielddata\encountdata\pl_enc_data.narc",
                        @"data\fielddata\eventdata\zone_event.narc",
                        @"data\fielddata\land_data\land_data.narc",
                        @"data\fielddata\mapmatrix\map_matrix.narc",
                        @"data\msgdata\pl_msg.narc",
                        @"data\fielddata\script\scr_seq.narc",
                        @"data\data\mmodel\mmodel.narc",
                        @"data\poketool\trainer\trdata.narc"
                    };
                    break;
                case "HeartGold":
                case "SoulSilver":
                    extractedNarcDirs = new string[] {
                        syntheticOverlayPath,
                        buildingTexturesDirPath,
                        workDir + @"unpacked\area_build",
                        areaDataDirPath,
                        mapTexturesDirPath,
                        exteriorBuildingsPath,
                        eventsDirPath,
                        mapDirPath,
                        matrixDirPath,
                        textArchivesPath,
                        scriptDirPath,
                        GetOWSpriteDirPath(),
                        GetTrainerDataDirPath(),
                        GetEncounterDirPath(),
                        interiorBuildingsPath
                    };
                    narcPaths = new string[] {
                        @"data\a\0\2\8",
                        @"data\a\0\7\0",
                        @"data\a\0\4\3",
                        @"data\a\0\4\2",
                        @"data\a\0\4\4",
                        @"data\a\0\4\0",
                        @"data\a\0\3\2",
                        @"data\a\0\6\5",
                        @"data\a\0\4\1",
                        @"data\a\0\2\7",
                        @"data\a\0\1\2",
                        @"data\a\0\8\1",
                        @"data\a\0\5\5",
                        @"data\a\0\3\7",
                        @"data\a\1\4\8"
                    };
                    break;
                default:
                    narcPaths = new string[] {
                           @"data\a\0\7\0",
                           @"data\a\0\4\3",
                           @"data\a\0\4\2",
                           @"data\a\0\4\4",
                           @"data\a\0\4\0",
                           @"data\a\0\3\2",
                           @"data\a\0\6\5",
                           @"data\a\0\4\1",
                           @"data\a\0\2\7",
                           @"data\a\0\1\2",
                           @"data\a\0\8\1",
                           @"data\a\0\5\5",
                           @"data\a\0\3\7",
                           @"data\a\1\4\8"
                        };
                    extractedNarcDirs = new string[] {
                       GetBuildingTexturesDirPath(),
                       workDir + @"unpacked\area_build",
                       areaDataDirPath,
                       mapTexturesDirPath,
                       eventsDirPath,
                       mapDirPath,
                       matrixDirPath,
                       textArchivesPath,
                       scriptDirPath,
                       GetOWSpriteDirPath(),
                       GetTrainerDataDirPath(),
                       GetEncounterDirPath(),
                    };
                    break;
            }
        }

        public string[] GetNarcPaths() {
            return narcPaths;
        }
        public string[] GetExtractedNarcDirs() {
            return extractedNarcDirs;
        }

        public string GetAreaDataDirPath() {
            return areaDataDirPath;
        }
        public void SetAreaDataDirPath() {
            areaDataDirPath = workDir + @"unpacked\area_data";
        }

        public int GetAreaDataCount() {
            return Directory.GetFiles(areaDataDirPath).Length; ;
        }
        public void SetBuildingModelsDirPath() {
            switch (gameVersion) {
                case "Diamond":
                case "Pearl":
                case "Platinum":
                    exteriorBuildingsPath = workDir + @"unpacked\DPPtBuildings";
                    break;
                default:
                    interiorBuildingsPath = workDir + @"unpacked\HGSSBuildingsIN";
                    exteriorBuildingsPath = workDir + @"unpacked\HGSSBuildingsOUT";
                    break;
            }
        }
        public string GetBuildingModelsDirPath(bool interior) {
            if (interior)
                return interiorBuildingsPath;
            else
                return exteriorBuildingsPath;
        }
        public int GetBuildingCount(bool interior) {
            if (interior)
                return Directory.GetFiles(interiorBuildingsPath).Length;
            else
                return Directory.GetFiles(exteriorBuildingsPath).Length;
        }

        public void SetOWtablePath () {
            switch (gameVersion) {
                case "Diamond":
                case "Pearl":
                case "Platinum":
                    OWtablePath = workDir + "overlay" + "\\" + "overlay_0005.bin";
                    break;
                default:
                    OWtablePath = workDir + "overlay" + "\\" + "overlay_0001.bin";
                    break;
            }
        }
        public string GetOWtablePath () {
            return OWtablePath;
        }
        public int GetHeaderTableOffset() {
            Dictionary<string, int> offsets = new Dictionary<string, int>() {
                ["ADAE"] = 0xEEDBC,
                ["ADAS"] = 0xEEE08,
                ["ADAI"] = 0xEED70,
                ["ADAF"] = 0xEEDFC,
                ["ADAD"] = 0xEEDCC,
                ["ADAJ"] = 0xF0C28,

                ["APAE"] = 0xEEDBC,
                ["APAS"] = 0xEEE08,
                ["APAI"] = 0xEED70,
                ["APAF"] = 0xEEDFC,
                ["APAD"] = 0xEEDCC,
                ["APAJ"] = 0xF0C28,

                ["CPUE"] = 0xE601C,
                ["CPUS"] = 0xE60B0,
                ["CPUI"] = 0xE6038,
                ["CPUF"] = 0xE60A4,
                ["CPUD"] = 0xE6074,
                ["CPUJ"] = 0xE56F0,

                ["IPKE"] = 0xF6BE0,
                ["IPKS"] = 0xF6BC8,
                ["IPKI"] = 0xF6B58,
                ["IPKF"] = 0xF6BC4,
                ["IPKD"] = 0xF6B94,
                ["IPKJ"] = 0xF6390,

                ["IPGE"] = 0xF6BE0,
                ["IPGS"] = 0xF6BD0,
                ["IPGI"] = 0xF6B58,
                ["IPGF"] = 0xF6BC4,
                ["IPGD"] = 0xF6B94,
                ["IPGJ"] = 0xF6390,
            };
            return offsets[this.romID];
        }
        public int GetHeaderCount() {
            return (int)new FileInfo(workDir + @"data\fielddata\maptable\mapname.bin").Length / 0x10;
        }
        public int GetAttackNamesMessageNumber() {
            int fileNumber;
            switch (gameVersion) {
                case "Diamond":
                case "Pearl":
                    fileNumber = 588;
                    break;
                case "Platinum":
                    fileNumber = 647;
                    break;
                default:
                    if (GetGameLanguage() == "JAP")
                        fileNumber = 739;
                    else
                        fileNumber = 750;
                    break;
            }
            return fileNumber;
        }
        public int GetItemNamesMessageNumber() {
            int fileNumber;
            switch (gameVersion) {
                case "Diamond":
                case "Pearl":
                    fileNumber = 344;
                    break;
                case "Platinum":
                    fileNumber = 392;
                    break;
                default:
                    if (GetGameLanguage() == "JAP") fileNumber = 219;
                    else fileNumber = 222;
                    break;
            }
            return fileNumber;
        }
        public int GetMapNamesMessageNumber() {
            int fileNumber;
            switch (gameVersion) {
                case "Diamond":
                case "Pearl":
                    fileNumber = 382;
                    break;
                case "Platinum":
                    fileNumber = 433;
                    break;
                default:
                    if (GetGameLanguage() == "JAP") 
                        fileNumber = 272;
                    else 
                        fileNumber = 279;
                    break;
            }
            return fileNumber;
        }
        public int GetItemScriptFileNumber() {
            int fileNumber;
            switch (gameVersion) {
                case "Diamond":
                case "Pearl":
                    fileNumber = 370;
                    break;
                case "Platinum":
                    fileNumber = 404;
                    break;
                default:
                    fileNumber = 141;
                    break;
            }
            return fileNumber;
        }
        public int GetPokémonNamesMessageNumber() {
            int fileNumber;
            switch (gameVersion) {
                case "Diamond":
                case "Pearl":
                    fileNumber = 362;
                    break;
                case "Platinum":
                    fileNumber = 412;
                    break;
                default:
                    if (GetGameLanguage() == "JAP")
                        fileNumber = 232;
                    else
                        fileNumber = 237;
                    break;
            }
            return fileNumber;
        }
        public int GetTrainerNamesMessageNumber() {
            int fileNumber;
            switch (gameVersion) {
                case "Diamond":
                case "Pearl":
                    fileNumber = 559;
                    break;
                case "Platinum":
                    fileNumber = 618;
                    break;
                default:
                    if (GetGameLanguage() == "JAP")
                        fileNumber = 719;
                    else
                        fileNumber = 729;
                    break;
            }
            return fileNumber;
        }
        public int GetTrainerClassMessageNumber() {
            int fileNumber;
            switch (gameVersion) {
                case "Diamond":
                case "Pearl":
                    fileNumber = 560;
                    break;
                case "Platinum":
                    fileNumber = 619;
                    break;
                default:
                    if (GetGameLanguage() == "JAP")
                        fileNumber = 720;
                    else
                        fileNumber = 730;
                    break;
            }
            return fileNumber;
        }
        public void SetMapTexturesDirPath() {
            mapTexturesDirPath = workDir + @"unpacked\maptex";
        }
        public string GetMapTexturesDirPath() {
            return mapTexturesDirPath;
        }
        public int GetMapTexturesCount() {
            return Directory.GetFiles(GetMapTexturesDirPath()).Length;
        }
        public string GetBuildingTexturesDirPath() {
            return buildingTexturesDirPath;
        }
        public void SetBuildingTexturesDirPath() {
            buildingTexturesDirPath = workDir + @"unpacked\TextureBLD";
        }

        public int GetBuildingTexturesCount() {
            return Directory.GetFiles(GetBuildingTexturesDirPath()).Length;
        }
        public string GetMatrixDirPath() {
            return matrixDirPath;
        }
        public void SetMatrixDirPath() {
            matrixDirPath = workDir + @"unpacked\matrix";
        }
        public int GetMatrixCount() {
            return Directory.GetFiles(matrixDirPath).Length;
        }
        public string GetTextArchivesPath() {
            return textArchivesPath;
        }
        public void SetTextArchivesPath() {
            textArchivesPath = workDir + @"unpacked\msg";
        }
        public int GetTextArchivesCount() {
            return Directory.GetFiles(textArchivesPath).Length;
        }
        public string GetTrainerDataDirPath() {
            return workDir + @"unpacked\trainerdata";
        }
        public string GetMapDirPath() {
            return mapDirPath;
        }
        public void SetMapDirPath () {
            mapDirPath = workDir + @"unpacked\maps";
        }
        public int GetMapCount() {
            return Directory.GetFiles(mapDirPath).Length;
        }
        public string GetOWSpriteDirPath() {
            return workDir + @"unpacked\overworlds";
        }
        public string GetEncounterDirPath() {
            return workDir + @"unpacked\wildPokeData";
        }
        public int GetEventCount() {
            return Directory.GetFiles(eventsDirPath).Length;
        }
        public string GetEventsDirPath() {
            return eventsDirPath;
        }
        public void SetEventsDirPath() {
            eventsDirPath = workDir + @"unpacked\events";
        }
        public string loadGameVersion() {
            Dictionary<string, string> versions = new Dictionary<string, string>() {
                ["ADAE"] = "Diamond",
                ["ADAS"] = "Diamond",
                ["ADAI"] = "Diamond",
                ["ADAF"] = "Diamond",
                ["ADAD"] = "Diamond",
                ["ADAJ"] = "Diamond",
                ["APAE"] = "Pearl",
                ["APAS"] = "Pearl",
                ["APAI"] = "Pearl",
                ["APAF"] = "Pearl",
                ["APAD"] = "Pearl",
                ["APAJ"] = "Pearl",
                ["CPUE"] = "Platinum",
                ["CPUS"] = "Platinum",
                ["CPUI"] = "Platinum",
                ["CPUF"] = "Platinum",
                ["CPUD"] = "Platinum",
                ["CPUJ"] = "Platinum",
                ["IPKE"] = "HeartGold",
                ["IPKS"] = "HeartGold",
                ["IPKI"] = "HeartGold",
                ["IPKF"] = "HeartGold",
                ["IPKD"] = "HeartGold",
                ["IPKJ"] = "HeartGold",
                ["IPGE"] = "SoulSilver",
                ["IPGS"] = "SoulSilver",
                ["IPGI"] = "SoulSilver",
                ["IPGF"] = "SoulSilver",
                ["IPGD"] = "SoulSilver",
                ["IPGJ"] = "SoulSilver",
                ["LATA"] = "LATA",
            };
            try {
                return versions[this.romID];
            } catch (KeyNotFoundException) {
                System.Windows.Forms.MessageBox.Show("The ROM you attempted to load is not supported.\nYou can only load Gen IV Pokémon ROMS, for now.", "Unsupported ROM",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        public int GetScriptCount() {
            return Directory.GetFiles(GetScriptDirPath()).Length;
        }
        public string GetScriptDirPath() {
            return scriptDirPath;
        }
        public void SetScriptDirPath() {
            scriptDirPath = workDir + @"unpacked\scripts";
        }

        public string GetSyntheticOverlayPath() {
            return syntheticOverlayPath;
        }

        public void SetSyntheticOverlayPath() {
            syntheticOverlayPath = workDir + @"unpacked\syntheticOverlayNarc";
        }



        #endregion

    }
}