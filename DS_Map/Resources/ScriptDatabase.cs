using DSPRE;
using DSPRE.Resources;
using DSPRE.ROMFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using static DSPRE.RomInfo;

public static class ScriptDatabaseJsonLoader
{
    /// <summary>
    /// Call this once at startup (before any script‐lookups).
    /// </summary>
    public static void InitializeFromJson(string jsonPath, GameVersions gameVersion)
    {
        string expandedPath = Environment.ExpandEnvironmentVariables(jsonPath);
        string text = File.ReadAllText(expandedPath);
        JsonDocument doc = JsonDocument.Parse(text);
        try
        {
            JsonElement root = doc.RootElement;

            ScriptDatabase.movementsDictIDName = root
                .GetProperty("movements")
                .EnumerateObject()
                .ToDictionary(
                    prop => Convert.ToUInt16(prop.Name.Substring(2), 16),
                    prop => prop.Value.GetString()
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

            Dictionary<ushort, string> namesDict;
            Dictionary<ushort, byte[]> paramsDict;
            Dictionary<ushort, string> soundsDict = ScriptDatabase.soundNames;

            switch (gameVersion)
            {
                case GameVersions.Platinum:
                    namesDict = ScriptDatabase.PlatScrCmdNames;
                    paramsDict = ScriptDatabase.PlatScrCmdParameters;
                    break;
                case GameVersions.Diamond:
                case GameVersions.Pearl:
                    namesDict = ScriptDatabase.DPScrCmdNames;
                    paramsDict = ScriptDatabase.DPScrCmdParameters;
                    break;
                case GameVersions.HeartGold:
                case GameVersions.SoulSilver:
                    namesDict = ScriptDatabase.HGSSScrCmdNames;
                    paramsDict = ScriptDatabase.HGSSScrCmdParameters;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameVersion), gameVersion, "Unsupported game");
            }

            JsonElement scrRoot;
            if (!root.TryGetProperty("scrcmd", out scrRoot))
                throw new InvalidOperationException("JSON is missing the \"scrcmd\" key");

            Console.WriteLine("About to load scrcmd entries:");
            foreach (JsonProperty prop in scrRoot.EnumerateObject())
            {
                ushort code = Convert.ToUInt16(prop.Name.Substring(2), 16);
                JsonElement entry = prop.Value;

                string name = entry.GetProperty("name").GetString();
                namesDict[code] = name;

                var arr = entry.GetProperty("parameters");
                byte[] bytes = arr
                    .EnumerateArray()
                    .Select(x => (byte)x.GetInt32())
                    .ToArray();
                paramsDict[code] = bytes;
            }

            JsonElement soundsRoot;
            if (!root.TryGetProperty("sounds", out soundsRoot))
                throw new InvalidOperationException("JSON is missing the \"sounds\" key");

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
        finally
        {
            doc.Dispose();
        }
    }

    public static void LoadParameterTypes(string jsonPath, GameVersions gameVersion)
    {
        Dictionary<ushort, List<ScriptParameter.ParameterType>> paramtypesDict;
        switch (gameVersion)
        {
            case GameVersions.Platinum:
                paramtypesDict = ScriptDatabase.PlatScrCmdParameterTypes;
                break;
            case GameVersions.Diamond:
            case GameVersions.Pearl:
                paramtypesDict = ScriptDatabase.DPScrCmdParameterTypes;
                break;
            case GameVersions.HeartGold:
            case GameVersions.SoulSilver:
                paramtypesDict = ScriptDatabase.HGSSScrCmdParameterTypes;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gameVersion));
        }

        string text = File.ReadAllText(jsonPath);
        JsonDocument doc = JsonDocument.Parse(text);

        try
        {
            JsonElement root = doc.RootElement;
            if (!root.TryGetProperty("scrcmd", out JsonElement scrRoot))
            {
                throw new InvalidOperationException("JSON is missing the \"scrcmd\" key");
            }

            foreach (JsonProperty prop in scrRoot.EnumerateObject())
            {
                ushort code = Convert.ToUInt16(prop.Name.Substring(2), 16);
                JsonElement entry = prop.Value;

                if (entry.TryGetProperty("parameter_types", out JsonElement paramTypesElement))
                {
                    List<ScriptParameter.ParameterType> paramTypes = new List<ScriptParameter.ParameterType>();
                    foreach (JsonElement typeElement in paramTypesElement.EnumerateArray())
                    {
                        string typeStr = typeElement.GetString();
                        if (!string.IsNullOrEmpty(typeStr))
                        {
                            // Debug output to see what types we're getting
                            Console.WriteLine($"Command 0x{code:X3} parameter type: {typeStr}");
                            var paramType = ScriptParameter.ParseTypeString(typeStr);
                            paramTypes.Add(paramType);
                        }
                    }

                    if (paramTypes.Count > 0)
                    {
                        paramtypesDict[code] = paramTypes;
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

namespace DSPRE.Resources {
    public static class ScriptDatabase {  
        public static Dictionary<ushort, string> comparisonOperatorsGenVappendix = new Dictionary<ushort, string>() {
            /* GEN V ONLY */
            [6] = "OR",
            [7] = "AND",
            [0xFF] = "TRUEUP"
        };

        // will all be populated from json at runtime
        public static Dictionary<ushort, string> comparisonOperatorsDict = new Dictionary<ushort, string>();
        public static Dictionary<ushort, string> specialOverworlds = new Dictionary<ushort, string>();
        public static Dictionary<byte, string> overworldDirections = new Dictionary<byte, string>();
        public static Dictionary<ushort, string> DPScrCmdNames = new Dictionary<ushort, string>();
        public static Dictionary<ushort, byte[]> DPScrCmdParameters = new Dictionary<ushort, byte[]>();
        public static Dictionary<ushort, List<ScriptParameter.ParameterType>> DPScrCmdParameterTypes = new Dictionary<ushort, List<ScriptParameter.ParameterType>>();
        public static Dictionary<ushort, string> PlatScrCmdNames = new Dictionary<ushort, string>();
        public static Dictionary<ushort, byte[]> PlatScrCmdParameters = new Dictionary<ushort, byte[]>();
        public static Dictionary<ushort, List<ScriptParameter.ParameterType>> PlatScrCmdParameterTypes = new Dictionary<ushort, List<ScriptParameter.ParameterType>>();
        public static Dictionary<ushort, string> HGSSScrCmdNames = new Dictionary<ushort, string>();
        public static Dictionary<ushort, byte[]> HGSSScrCmdParameters = new Dictionary<ushort, byte[]>();
        public static Dictionary<ushort, List<ScriptParameter.ParameterType>> HGSSScrCmdParameterTypes = new Dictionary<ushort, List<ScriptParameter.ParameterType>>();
        public static Dictionary<ushort, string> pokemonNames = new Dictionary<ushort, string>();
        public static Dictionary<ushort, string> itemNames = new Dictionary<ushort, string>();
        public static Dictionary<ushort, string> moveNames = new Dictionary<ushort, string>();
        public static Dictionary<ushort, string> soundNames = new Dictionary<ushort, string>();
        public static Dictionary<ushort, string> trainerNames = new Dictionary<ushort, string>();
        public static Dictionary<ushort, string> movementsDictIDName = new Dictionary<ushort, string>();

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
    }
}
