using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace DSPRE.CharMaps
{
    internal static class CharMapManager
    {

        private static Dictionary<ushort, string> decodeMap;
        private static Dictionary<string, ushort> encodeMap;
        private static Dictionary<ushort, string> commandMap;

        public static readonly string charmapFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tools", "charmap.xml");
        public static readonly string customCharmapFilePath = Path.Combine(Program.DspreDataPath, "charmap.xml");

        private static bool mapsInitialized = false;

        public static Dictionary<ushort, string> GetDecodingMap()
        {
            if (!mapsInitialized)
            {
                InitializeCharMaps();
            }
            return decodeMap;
        }

        public static Dictionary<string, ushort> GetEncodingMap()
        {
            if (!mapsInitialized)
            {
                InitializeCharMaps();
            }
            return encodeMap;
        }

        public static Dictionary<ushort, string> GetCommandMap()
        {
            if (!mapsInitialized)
            {
                InitializeCharMaps();
            }
            return commandMap;
        }

        public static void InvalidateMaps()
        {
            mapsInitialized = false;
        }

        /// <summary>
        /// Initializes the character maps by loading data from the charmap XML file.
        /// </summary>
        /// <remarks> 
        /// Loads the custom charmap file if it exists; otherwise, loads the default charmap file. 
        /// If neither file is found, a FileNotFoundException is thrown.
        /// </remarks>
        /// <exception cref="FileNotFoundException"></exception>
        private static void InitializeCharMaps()
        {
            decodeMap = new Dictionary<ushort, string>();
            encodeMap = new Dictionary<string, ushort>();
            commandMap = new Dictionary<ushort, string>();

            string charmapPath = "";

            if (File.Exists(customCharmapFilePath))
            {
                AppLogger.Info("Loading custom charmap from user data directory.");
                charmapPath = customCharmapFilePath;
            }
            else if (File.Exists(charmapFilePath))
            {
                AppLogger.Info("Loading default charmap from application directory.");
                charmapPath = charmapFilePath;
            }
            else
            {
                throw new FileNotFoundException("No charmap XML file found in application or user data directory.");
            }

            var xml = XDocument.Load(charmapPath, LoadOptions.PreserveWhitespace);

            foreach (var entry in xml.Descendants("entry"))
            {
                var code = entry.Attribute("code")?.Value;
                var kind = entry.Attribute("kind")?.Value;
                var value = entry.Value;

                if (code == null || kind == null || value == null)
                {
                    AppLogger.Error("Found charmap entry with null value.");
                    continue;
                }

                ushort codeValue;
                string codeKind = kind.ToLower();
                string chars = value.ToString();

                if (!ushort.TryParse(code, System.Globalization.NumberStyles.HexNumber, null, out codeValue))
                {
                    AppLogger.Error($"Invalid code value in charmap: {code}");
                    continue;
                }

                if (codeKind == "char" || codeKind == "escape")
                {
                    decodeMap[codeValue] = chars;
                    encodeMap[chars] = codeValue;
                }
                else if (codeKind == "alias")
                {
                    encodeMap[chars] = codeValue;
                }
                else if (codeKind == "command")
                {
                    commandMap[codeValue] = chars;
                }
                else
                {
                    AppLogger.Error($"Unknown kind '{kind}' in charmap entry.");
                }
            }

            mapsInitialized = true;
        }

        /// <summary>
        /// Creates a custom character map file by copying the default character map file to a specified location.
        /// </summary>
        /// <remarks>This method ensures that the directory for the custom character map file exists
        /// before attempting to copy the file. If the operation succeeds, the custom character map file will overwrite
        /// any existing file at the target location.</remarks>
        /// <returns><see langword="true"/> if the custom character map file is successfully created; otherwise, <see
        /// langword="false"/>.</returns>
        public static bool CreateCustomCharMapFile()
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(customCharmapFilePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(customCharmapFilePath));
                }
                File.Copy(charmapFilePath, customCharmapFilePath, overwrite: true);
                AppLogger.Info("Custom charmap file created successfully.");
                return true;
            }
            catch (Exception ex)
            {
                AppLogger.Error($"Failed to create custom charmap file: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Deletes the custom character map file if it exists.
        /// </summary>
        /// <remarks>This method checks for the existence of the custom character map file at the
        /// predefined path and deletes it if found.</remarks>
        /// <returns><see langword="true"/> if the operation completes successfully, regardless of whether the file existed;
        /// otherwise, <see langword="false"/> if an error occurs during the deletion process.</returns>
        public static bool DeleteCustomCharMapFile()
        {
            try
            {
                if (File.Exists(customCharmapFilePath))
                {
                    File.Delete(customCharmapFilePath);
                    AppLogger.Info("Custom charmap file deleted successfully.");
                }
                else
                {
                    AppLogger.Info("No custom charmap file to delete.");
                }
                return true;
            }
            catch (Exception ex)
            {
                AppLogger.Error($"Failed to delete custom charmap file: {ex.Message}");
                return false;
            }
        }

        public static bool SaveCustomCharMap(XmlDocument xml)
        {
            try
            {
                xml.Save(customCharmapFilePath);
                AppLogger.Info("Custom charmap XML saved successfully.");
                InvalidateMaps();
                return true;
            }
            catch (Exception ex)
            {
                AppLogger.Error($"Failed to save custom charmap XML: {ex.Message}");
                return false;
            }
        }

    }
}
