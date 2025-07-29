using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSPRE
{

    public class DspreSettings
    {
        public byte menuLayout { get; set; } = 2;
        public string lastColorTablePath { get; set; } = "";
        public bool textEditorPreferHex { get; set; } = false;
        public int scriptEditorFormatPreference { get; set; } = 0;
        public bool renderSpawnables { get; set; } = true;
        public bool renderOverworlds { get; set; } = true;
        public bool renderWarps { get; set; } = true;
        public bool renderTriggers { get; set; } = true;
        public string exportPath { get; set; } = "";
        public string mapImportStarterPoint { get; set; } = "";
        public string openDefaultRom { get; set; } = "";
        public bool neverAskForOpening { get; set; } = false;
    }

    public static class SettingsManager
    {
        public static DspreSettings Settings { get; private set; }

        private static readonly string SettingsFile = Path.Combine(Program.DspreDataPath, "userSettings.json");

        public static void Load()
        {
            AppLogger.Info("Loading app settings");
            if (File.Exists(SettingsFile))
            {
                string json = File.ReadAllText(SettingsFile);
                Settings = JsonConvert.DeserializeObject<DspreSettings>(json);
            }
            else
            {
                Settings = new DspreSettings();
                Save();
            }
        }

        public static void Save()
        {
            string json = JsonConvert.SerializeObject(Settings, Formatting.Indented);
            File.WriteAllText(SettingsFile, json);
        }
    }
}
