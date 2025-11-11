using DSPRE;
using DSPRE.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using static DSPRE.RomInfo;

public static class ScriptDatabaseJsonLoader
{
    /// <summary>
    /// Call this once at startup (before any script lookups).
    /// Loads all script command metadata from JSON into unified objects.
    /// </summary>
    public static void InitializeFromJson(string jsonPath, GameVersions gameVersion)
    {
        string expandedPath = Environment.ExpandEnvironmentVariables(jsonPath);
        string text = File.ReadAllText(expandedPath);
        JsonDocument doc = JsonDocument.Parse(text);
        try
        {
            JsonElement root = doc.RootElement;

            ScriptDatabase.movementsDict = root
                .GetProperty("movements")
                .EnumerateObject()
                .ToDictionary(
                    prop => Convert.ToUInt16(prop.Name.Substring(2), 16),
                    prop => new MovementCommandInfo
                    {
                        CommandId = Convert.ToUInt16(prop.Name.Substring(2), 16),
                        Name = prop.Value.GetProperty("name").GetString(),
                        DecompName = prop.Value.GetProperty("decomp_name").GetString(),
                        Description = prop.Value.GetProperty("description").GetString(),
                    }
                );

            ScriptDatabase.comparisonOperatorsDict = root
                .GetProperty("comparisonOperators")
                .EnumerateObject()
                .ToDictionary(
                    prop => Convert.ToUInt16(prop.Name.Substring(2), 16),
                    prop => prop.Value.GetString()
                );

            ScriptDatabase.specialOverworlds = root
                .GetProperty("specialOverworlds")
                .EnumerateObject()
                .ToDictionary(
                    prop => Convert.ToUInt16(prop.Name.Substring(2), 16),
                    prop => prop.Value.GetString()
                );

            ScriptDatabase.overworldDirections = root
                .GetProperty("overworldDirections")
                .EnumerateObject()
                .ToDictionary(
                    prop => (byte)Convert.ToUInt16(prop.Name.Substring(2), 16),
                    prop => prop.Value.GetString()
                );

            // Determine which dictionary to populate based on game version
            Dictionary<ushort, ScriptCommandInfo> commandInfoDict;

            switch (gameVersion)
            {
                case GameVersions.Platinum:
                    commandInfoDict = ScriptDatabase.PlatScrCmdInfo;
                    break;
                case GameVersions.Diamond:
                case GameVersions.Pearl:
                    commandInfoDict = ScriptDatabase.DPScrCmdInfo;
                    break;
                case GameVersions.HeartGold:
                case GameVersions.SoulSilver:
                    commandInfoDict = ScriptDatabase.HGSSScrCmdInfo;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameVersion), gameVersion, "Unsupported game");
            }

            // Load script commands
            JsonElement scrRoot;
            if (!root.TryGetProperty("scrcmd", out scrRoot))
                throw new InvalidOperationException("JSON is missing the \"scrcmd\" key");

            foreach (JsonProperty prop in scrRoot.EnumerateObject())
            {
                ushort code = Convert.ToUInt16(prop.Name.Substring(2), 16);
                JsonElement entry = prop.Value;

                // Create unified command info object
                var cmdInfo = new ScriptCommandInfo
                {
                    CommandId = code,
                    Name = entry.GetProperty("name").GetString(),
                    DecompName = entry.TryGetProperty("decomp_name", out var decompElem) ? decompElem.GetString() : "",
                    Description = entry.TryGetProperty("description", out var descElem) ? descElem.GetString() : "",
                };

                if (entry.TryGetProperty("parameters", out var paramsElem))
                {
                    cmdInfo.ParameterSizes = paramsElem
                        .EnumerateArray()
                        .Select(x => (byte)x.GetInt32())
                        .ToArray();
                }

                // Load parameter types
                if (entry.TryGetProperty("parameter_types", out var paramTypesElem))
                {
                    cmdInfo.ParameterTypes = new List<ScriptParameter.ParameterType>();
                    foreach (JsonElement typeElement in paramTypesElem.EnumerateArray())
                    {
                        string typeStr = typeElement.GetString();
                        if (!string.IsNullOrEmpty(typeStr))
                        {
                            var paramType = ScriptParameter.ParseTypeString(typeStr);
                            cmdInfo.ParameterTypes.Add(paramType);
                        }
                    }
                }

                // Load parameter names (for display/documentation)
                if (entry.TryGetProperty("parameter_values", out var paramNamesElem))
                {
                    cmdInfo.ParameterNames = new List<string>();
                    foreach (JsonElement nameElement in paramNamesElem.EnumerateArray())
                    {
                        string paramName = nameElement.GetString();
                        cmdInfo.ParameterNames.Add(paramName ?? "");
                    }
                }

                // Store in unified dictionary
                commandInfoDict[code] = cmdInfo;
            }

            // Load sounds
            if (root.TryGetProperty("sounds", out JsonElement soundsRoot))
            {
                Dictionary<ushort, string> soundsDict = ScriptDatabase.soundNames;
                foreach (JsonProperty prop in soundsRoot.EnumerateObject())
                {
                    if (ushort.TryParse(prop.Name, out ushort id))
                    {
                        JsonElement entry = prop.Value;
                        if (entry.TryGetProperty("name", out JsonElement nameElement))
                        {
                            string name = nameElement.GetString();
                            if (!string.IsNullOrEmpty(name))
                            {
                                soundsDict[id] = name;
                            }
                        }
                    }
                }
            }
        }
        finally
        {
            doc.Dispose();
        }
    }


}

namespace DSPRE.Resources
{
    public static class ScriptDatabase
    {
        // New unified object-oriented dictionaries
        public static Dictionary<ushort, ScriptCommandInfo> DPScrCmdInfo = new Dictionary<ushort, ScriptCommandInfo>();
        public static Dictionary<ushort, ScriptCommandInfo> PlatScrCmdInfo = new Dictionary<ushort, ScriptCommandInfo>();
        public static Dictionary<ushort, ScriptCommandInfo> HGSSScrCmdInfo = new Dictionary<ushort, ScriptCommandInfo>();

        public static Dictionary<ushort, string> comparisonOperatorsGenVappendix = new Dictionary<ushort, string>()
        {
            /* GEN V ONLY */
            [6] = "OR",
            [7] = "AND",
            [0xFF] = "TRUEUP"
        };

        // Reference data - populated from JSON at runtime
        public static Dictionary<ushort, string> comparisonOperatorsDict = new Dictionary<ushort, string>();
        public static Dictionary<ushort, string> specialOverworlds = new Dictionary<ushort, string>();
        public static Dictionary<byte, string> overworldDirections = new Dictionary<byte, string>();
        public static Dictionary<ushort, string> pokemonNames = new Dictionary<ushort, string>();
        public static Dictionary<ushort, string> itemNames = new Dictionary<ushort, string>();
        public static Dictionary<ushort, string> moveNames = new Dictionary<ushort, string>();
        public static Dictionary<ushort, string> soundNames = new Dictionary<ushort, string>();
        public static Dictionary<ushort, string> trainerNames = new Dictionary<ushort, string>();
        public static Dictionary<ushort, MovementCommandInfo> movementsDict = new Dictionary<ushort, MovementCommandInfo>();
        public static Dictionary<ushort, string> movementsDictIDName => movementsDict.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Name);

        public static Dictionary<ushort, int> commandsWithRelativeJump = new Dictionary<ushort, int>()
        {
            //commandID, ID of parameter With Jump Address

            [0x0016] = 0,   //Jump
            [0x0017] = 1,   //Call
            [0x0018] = 1,   //Call
            [0x0019] = 1,   //Call
            [0x001A] = 0,   //Call
            [0x001C] = 1,   //JumpIf
            [0x001D] = 1,   //CallIf
            [0x005E] = 1,   //Movement
        };

        public static HashSet<ushort?> endCodes = new HashSet<ushort?>() {
            0x2,
            0x16,
            0x1B
        };

        public static HashSet<ushort?> movementEndCodes = new HashSet<ushort?>() {
            0x00FE,
        };

        public static void InitializePokemonNames()
        {
            string[] names = GetPokemonNames();
            pokemonNames = names.Select((name, index) => new { name, index })
                             .ToDictionary(
                                 x => (ushort)x.index,
                                 x => "SPECIES_" + x.name.ToUpper().Replace(' ', '_')
                             );
        }
        public static void InitializeItemNames()
        {
            string[] names = GetItemNames();
            itemNames = names.Select((name, index) => new { name, index })
                             .ToDictionary(
                                 x => (ushort)x.index,
                                 x => "ITEM_" + x.name.ToUpper().Replace(' ', '_').Replace('É', 'E')
                             );
        }
        public static void InitializeMoveNames()
        {
            string[] names = GetAttackNames();
            moveNames = names.Select((name, index) => new { name, index })
                             .ToDictionary(
                                 x => (ushort)x.index,
                                 x => "MOVE_" + x.name.ToUpper().Replace(' ', '_')
                             );
        }
        public static void InitializeTrainerNames()
        {
            string[] names = GetSimpleTrainerNames();

            trainerNames = Enumerable.Range(0, names.Length)
                .ToDictionary(
                    index => (ushort)index,
                    index => index == 0
                        ? "TRAINER_NONE"
                        : $"TRAINER_{names[index]}_{index:D3}"
                            .ToUpper()
                            .Replace(' ', '_')
                            .Replace("&", "AND")
            );
        }

        internal static void InitializePokemonNamesIfNeeded()
        {
            if(pokemonNames.Count == 0)
            {
                InitializePokemonNames();
            }
        }

        internal static void InitializeMoveNamesIfNeeded()
        {
            if(moveNames.Count == 0)
            {
                InitializeMoveNames();
            }   
        }
    }
}
