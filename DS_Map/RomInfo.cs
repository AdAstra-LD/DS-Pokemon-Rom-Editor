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
        private string areaDataFolderPath;
        private string OWtablePath;

        #region Constructors (1)
        public RomInfo(string id, string workDir) {
            romID = id;
            gameVersion = loadGameVersion();
            language = loadGameLanguage();
            this.workDir = workDir;
            SetBuildingModelsDirPath();
            SetAreaDataDirPath();
            SetOWtablePath();
        }
        #endregion

        #region Methods (22)
        public string GetWorkingFolder() {
            return workDir;
        }
        public string GetAreaDataDirPath() {
            return areaDataFolderPath;
        }
        public void SetAreaDataDirPath() {
            areaDataFolderPath = workDir + @"unpacked\area_data";
        }
        public int GetAreaDataCount() {
            return Directory.GetFiles(GetAreaDataDirPath()).Length; ;
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
            return Directory.GetFiles(GetBuildingModelsDirPath(interior)).Length;
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
                    if (GetGameLanguage() == "JAP") fileNumber = 272;
                    else fileNumber = 279;
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
        public string GetMapTexturesFolderPath() {
            return workDir + @"unpacked\maptex";
        }
        public int GetMapTexturesCount() {
            return Directory.GetFiles(GetMapTexturesFolderPath()).Length;
        }
        public string GetBuildingTexturesFolderPath() {
            return workDir + @"unpacked\TextureBLD";
        }

        public int GetBuildingTexturesCount() {
            return Directory.GetFiles(GetBuildingTexturesFolderPath()).Length;
        }
        public string GetMatrixFolderPath() {
            return workDir + @"unpacked\matrix";
        }
        public int GetMatrixCount() {
            return Directory.GetFiles(GetMatrixFolderPath()).Length;
        }
        public string GetTextArchivesPath() {
            return workDir + @"unpacked\msg";
        }
        public int GetTextArchivesCount() {
            return Directory.GetFiles(GetTextArchivesPath()).Length;
        }
        public string GetTrainerDataFolderPath() {
            return workDir + @"unpacked\trainerdata";
        }
        public string GetMapFolderPath() {
            return workDir + @"unpacked\maps";
        }
        public int GetMapCount() {
            return Directory.GetFiles(GetMapFolderPath()).Length;
        }
        public string GetOWSpriteDirPath() {
            return workDir + @"unpacked\overworlds";
        }
        public string GetEncounterFolderPath() {
            return workDir + @"unpacked\wildPokeData";
        }
        public int GetEventCount() {
            return Directory.GetFiles(GetEventFolderPath()).Length;
        }
        public string GetEventFolderPath() {
            return workDir + @"unpacked\events";
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
            return Directory.GetFiles(GetScriptFolderPath()).Length;
        }
        public string GetScriptFolderPath() {
            return workDir + @"unpacked\scripts";
        }

        internal string GetSyntheticOverlayPath() {
            return workDir + @"unpacked\syntheticOverlayNarc";
        }



        #endregion

    }
}