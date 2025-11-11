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

            string charmapPath = GetCharMapPath();

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
        /// Retrieves the file path of the character map XML file to be used by the application.
        /// </summary>
        /// <remarks>This method checks for the existence of a custom character map file in the user data
        /// directory. If the custom file is not found, it falls back to the default character map file in the
        /// application directory. If neither file is found, an exception is thrown.</remarks>
        /// <returns>The file path of the character map XML file. This will be either the custom file path or the default file
        /// path, depending on which file is available.</returns>
        /// <exception cref="FileNotFoundException">Thrown if neither the custom character map file nor the default character map file exists.</exception>
        public static string GetCharMapPath()
        {
            if (File.Exists(customCharmapFilePath))
            {
                AppLogger.Info("Loading custom charmap from user data directory.");
                return customCharmapFilePath;
            }
            else if (File.Exists(charmapFilePath))
            {
                AppLogger.Info("Loading default charmap from application directory.");
                return charmapFilePath;
            }
            else
            {
                throw new FileNotFoundException("No charmap XML file found in application or user data directory.");
            }
        }

        /// <summary>
        /// Retrieves the version of the character map from the specified XML file.
        /// </summary>
        /// <remarks>The method reads the XML file at the specified path and looks for a "header" element
        /// with a "version" attribute.  If the "version" attribute is present and valid, it is parsed into a <see
        /// cref="Version"/> object and returned.  If the attribute is missing or invalid, a warning is logged, and the
        /// default version 1.0 is returned.</remarks>
        /// <param name="path">The file path to the XML document containing the character map.</param>
        /// <returns>The version of the character map as specified in the XML file. If the version is not found or is invalid, 
        /// returns a default version of <see cref="Version"/> 1.0.</returns>
        public static Version GetCharMapVersion(string path)
        {
            var xml = XDocument.Load(path, LoadOptions.PreserveWhitespace);
            var header = xml.Descendants("header").FirstOrDefault();
            string version = header?.Element("version")?.Value;
            
            if (version != null && Version.TryParse(version, out Version ver))
            {
                return ver;
            }
            else
            {
                AppLogger.Warn("Charmap version not found or invalid, defaulting to 1.0");
                return new Version(1, 0);
            }
        }

        /// <summary>
        /// Determines whether the custom character map is outdated compared to the default character map.
        /// </summary>
        /// <remarks>This method compares the versions of the default and custom character maps to
        /// determine if the custom map needs to be updated.</remarks>
        /// <returns><see langword="true"/> if the version of the custom character map is older than the version of the default
        /// character map; otherwise, <see langword="false"/>.</returns>
        public static bool IsCustomMapOutdated()
        {
            // Check if custom charmap file exists
            if (!File.Exists(customCharmapFilePath))
            {
                AppLogger.Warn($"No custom charmap found at: {customCharmapFilePath}");

                try
                {
                    // Ensure directory exists
                    if (!Directory.Exists(Program.DspreDataPath))
                    {
                        Directory.CreateDirectory(Program.DspreDataPath);
                    }

                    // Copy default charmap to custom path
                    File.Copy(charmapFilePath, customCharmapFilePath, false);
                    AppLogger.Info($"Created custom charmap from default at: {customCharmapFilePath}");

                    // Since we just created it, it's not outdated
                    return false;
                }
                catch (Exception ex)
                {
                    AppLogger.Error($"Failed to create custom charmap: {ex.Message}");
                    return false; // Don't treat as outdated if creation failed
                }
            }

            // Both files exist, compare versions
            try
            {
                var defaultVersion = GetCharMapVersion(charmapFilePath);
                var customVersion = GetCharMapVersion(customCharmapFilePath);

                return customVersion < defaultVersion;
            }
            catch (Exception ex)
            {
                AppLogger.Error($"Error comparing charmap versions: {ex.Message}");
                return false; // Don't treat as outdated if comparison failed
            }
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

        /// <summary>
        /// Saves the provided custom character map to a predefined file location.
        /// </summary>
        /// <remarks>This method attempts to save the provided XML document to a predefined file path.  If
        /// the operation fails, an error is logged, and the method returns <see langword="false"/>.</remarks>
        /// <param name="xml">The <see cref="XmlDocument"/> representing the custom character map to be saved.</param>
        /// <returns><see langword="true"/> if the custom character map is successfully saved; otherwise, <see
        /// langword="false"/>.</returns>
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
