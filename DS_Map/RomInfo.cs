using System.IO;
using System.Collections.Generic;

namespace DS_Map
{

    /// <summary>
    /// Class to store ROM data from GEN IV Pokémon games
    /// </summary>

    public class RomInfo
    {
        private string romID;
        private string workingFolder;
        private string gameVersion;

        #region Constructors (1)
        public RomInfo(string id, string workingFolder)
        {
            romID = id;
            gameVersion = getGameVersion();
            this.workingFolder = workingFolder;
        }
        #endregion

        #region Methods (22)
        public string GetAreaDataFolderPath()
        {
            string path;
            switch (gameVersion)
            {
                case "Diamond":
                case "Pearl":
                case "Platinum":
                    path = @"data\fielddata\areadata\area_data";
                    break;
                default:
                    path = @"data\a\0\4\area_data";
                    break;
            }
            return workingFolder + path;
        }
        public int GetAreaDataCount()
        {
            return Directory.GetFiles(GetAreaDataFolderPath()).Length; ;
        }
        public string GetBuildingModelsFolderPath(bool interior)
        {
            string path;
            switch (gameVersion)
            {
                case "Diamond":
                case "Pearl":
                case "Platinum":
                    path = @"data\fielddata\build_model\build_model";
                    break;
                default:
                    if (interior) path = @"data\a\1\4\building";
                    else path = @"data\a\0\4\building";
                    break;
            }
            return workingFolder + path;
        }
        public int GetBuildingCount(bool interior)
        {
            return Directory.GetFiles(GetBuildingModelsFolderPath(interior)).Length;
        }
        public int GetHeaderTableOffset()
        {
            Dictionary<string, int> offsets = new Dictionary<string, int>()
            {
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
        public int GetHeaderCount()
        {
            return (int)new FileInfo(workingFolder + @"data\fielddata\maptable\mapname.bin").Length / 0x10;
        }
        public int GetAttackNamesMessageNumber()
        {
            int fileNumber;
            switch (gameVersion)
            {
                case "Diamond":
                case "Pearl":
                    fileNumber = 588;
                    break;
                case "Platinum":
                    fileNumber = 647;
                    break;
                default:
                    if (GetLanguage() == "JAP") fileNumber = 739;
                    else fileNumber = 750;
                    break;
            }
            return fileNumber;
        }
        public int GetItemNamesMessageNumber()
        {
            int fileNumber;
            switch (gameVersion)
            {
                case "Diamond":
                case "Pearl":
                    fileNumber = 344;
                    break;
                case "Platinum":
                    fileNumber = 392;
                    break;
                default:
                    if (GetLanguage() == "JAP") fileNumber = 219;
                    else fileNumber = 222;
                    break;
            }
            return fileNumber;
        }
        public int GetMapNamesMessageNumber()
        {
            int fileNumber;
            switch (gameVersion)
            {
                case "Diamond":
                case "Pearl":
                    fileNumber = 382;
                    break;
                case "Platinum":
                    fileNumber = 433;
                    break;
                default:
                    if (GetLanguage() == "JAP") fileNumber = 272;
                    else fileNumber = 279;
                    break;
            }
            return fileNumber;
        }
        public int GetItemScriptFileNumber()
        {
            int fileNumber;
            switch (gameVersion)
            {
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
        public int GetPokémonNamesMessageNumber()
        {
            int fileNumber;
            switch (gameVersion)
            {
                case "Diamond":
                case "Pearl":
                    fileNumber = 362;
                    break;
                case "Platinum":
                    fileNumber = 412;
                    break;
                default:
                    if (GetLanguage() == "JAP") fileNumber = 232;
                    else fileNumber = 237;
                    break;
            }
            return fileNumber;
        }
        public int GetTrainerNamesMessageNumber()
        {
            int fileNumber;
            switch (gameVersion)
            {
                case "Diamond":
                case "Pearl":
                    fileNumber = 559;
                    break;
                case "Platinum":
                    fileNumber = 618;
                    break;
                default:
                    if (GetLanguage() == "JAP") fileNumber = 719;
                    else fileNumber = 729;
                    break;
            }
            return fileNumber;
        }
        public int GetTrainerClassMessageNumber()
        {
            int fileNumber;
            switch (gameVersion)
            {
                case "Diamond":
                case "Pearl":
                    fileNumber = 560;
                    break;
                case "Platinum":
                    fileNumber = 619;
                    break;
                default:
                    if (GetLanguage() == "JAP") fileNumber = 720;
                    else fileNumber = 730;
                    break;
            }
            return fileNumber;
        }
        public string GetMapTexturesFolderPath()
        {
            string path;
            switch (gameVersion)
            {
                case "Diamond":
                case "Pearl":
                case "Platinum":
                    path = @"data\fielddata\areadata\area_map_tex\map_tex_set";
                    break;
                default:
                    path = @"data\a\0\4\texture";
                    break;
            }
            return workingFolder + path;
        }
        public int GetMapTexturesCount()
        {
            return Directory.GetFiles(GetMapTexturesFolderPath()).Length;
        }
        public string GetBuildingTexturesFolderPath()
        {
            string path;
            switch (gameVersion)
            {
                case "Diamond":
                case "Pearl":
                case "Platinum":
                    path = @"data\fielddata\areadata\area_build_model\areabm_texset";
                    break;
                default:
                    path = @"data\a\0\7\textureBld";
                    break;
            }
            return workingFolder + path;
        }
        public int GetBuildingTexturesCount()
        {
            return Directory.GetFiles(GetBuildingTexturesFolderPath()).Length;
        }
        public string GetMatrixFolderPath()
        {
            string path;
            switch (gameVersion)
            {
                case "Diamond":
                case "Pearl":
                case "Platinum":
                    path = @"data\fielddata\mapmatrix\map_matrix";
                    break;
                default:
                    path = @"data\a\0\4\matrix";
                    break;
            }
            return workingFolder + path;
        }
        public int GetMatrixCount()
        {
            return Directory.GetFiles(GetMatrixFolderPath()).Length;
        }
        public string GetMessageFolderPath()
        {
            string path;
            switch (gameVersion)
            {
                case "Diamond":
                case "Pearl":
                    path = @"data\msgdata\msg";
                    break;
                case "Platinum":
                    path = @"data\msgdata\pl_msg";
                    break;
                default:
                    path = @"data\a\0\2\text";
                    break;
            }
            return workingFolder + path;
        }
        public int GetMessageCount()
        {
            return Directory.GetFiles(GetMessageFolderPath()).Length;
        }
        public string GetTrainerDataFolderPath()
        {
            string path;
            switch (gameVersion)
            {
                case "Diamond":
                case "Pearl":
                case "Platinum":
                    path = @"data\poketool\trainer\trdata";
                    break;
                default:
                    path = @"data\a\0\5\trdata";
                    break;
            }
            return workingFolder + path;
        }
        public string GetMapFolderPath()
        {
            string path;
            switch (gameVersion)
            {
                case "Diamond":
                case "Pearl":
                    path = @"data\fielddata\land_data\land_data_release";
                    break;
                case "Platinum":
                    path = @"data\fielddata\land_data\land_data";
                    break;
                default:
                    path = @"data\a\0\6\map";
                    break;
            }
            return workingFolder + path;
        }
        public int GetMapCount()
        {
            return Directory.GetFiles(GetMapFolderPath()).Length;
        }
        public string GetOverworldFolderPath()
        {
            string path;
            switch (gameVersion)
            {
                case "Diamond":
                case "Pearl":
                case "Platinum":
                    path = @"data\data\mmodel\mmodel";
                    break;
                default:
                    path = @"data\a\0\8\overworlds";
                    break;
            }
            return workingFolder + path;
        }
        public string GetEncounterFolderPath()
        {
            string path;
            switch (gameVersion)
            {
                case "Diamond":
                case "Pearl":
                    path = @"data\fielddata\encountdata\enc_data";
                    break;
                case "Platinum":
                    path = @"data\fielddata\encountdata\pl_enc_data";
                    break;
                default:
                    path = @"data\a\0\3\wild";
                    break;
            }
            return workingFolder + path;
        }
        public int GetEventCount()
        {
            return Directory.GetFiles(GetEventFolderPath()).Length;
        }
        public string GetEventFolderPath()
        {
            string path;
            switch (gameVersion)
            {
                case "Diamond":
                case "Pearl":
                    path = @"data\fielddata\eventdata\zone_event_release";
                    break;
                case "Platinum":
                    path = @"data\fielddata\eventdata\zone_event";
                    break;
                default:
                    path = @"data\a\0\3\event";
                    break;
            }
            return workingFolder + path;
        }
        public string GetLanguage()
        {
            string language;
            switch (romID)
            {
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
        public string getGameVersion()
        {
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
            return versions[this.romID];
        }
        public int GetScriptCount()
        {
            return Directory.GetFiles(GetScriptFolderPath()).Length;
        }
        public string GetScriptFolderPath()
        {
            string path;
            switch (gameVersion)
            {
                case "Diamond":
                case "Pearl":
                    path = @"data\fielddata\script\scr_seq_release";
                    break;
                case "Platinum":
                    path = @"data\fielddata\script\scr_seq";
                    break;
                default:
                    path = @"data\a\0\1\script";
                    break;
            }
            return workingFolder + path;
        }



        #endregion

    }
}