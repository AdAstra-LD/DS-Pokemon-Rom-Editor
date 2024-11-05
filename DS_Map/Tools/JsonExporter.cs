using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Linq;
using DSPRE.Resources;
using System.Collections;
using System;

namespace DSPRE.Tools
{
    public static class JsonExporter
    {
        private const string ExportDirectory = "Exports/ScriptDatabase/";

        public static void ExportDictionaries()
        {
            // Ensure the directory exists
            Directory.CreateDirectory(ExportDirectory);

            ExportWithMetadata(Path.Combine(ExportDirectory, "comparisonOperatorsDict.json"), ScriptDatabase.comparisonOperatorsDict);
            ExportWithMetadata(Path.Combine(ExportDirectory, "comparisonOperatorsGenVappendix.json"), ScriptDatabase.comparisonOperatorsGenVappendix);
            ExportWithMetadata(Path.Combine(ExportDirectory, "specialOverworlds.json"), ScriptDatabase.specialOverworlds);
            ExportWithMetadata(Path.Combine(ExportDirectory, "overworldDirections.json"), ScriptDatabase.overworldDirections);
            ExportWithMetadata(Path.Combine(ExportDirectory, "commandsWithRelativeJump.json"), ScriptDatabase.commandsWithRelativeJump);
            ExportWithMetadata(Path.Combine(ExportDirectory, "endCodes.json"), ScriptDatabase.endCodes);
            ExportWithMetadata(Path.Combine(ExportDirectory, "movementsDictIDName.json"), ScriptDatabase.movementsDictIDName);
            ExportWithMetadata(Path.Combine(ExportDirectory, "movementEndCodes.json"), ScriptDatabase.movementEndCodes);
            ExportCommandJson(Path.Combine(ExportDirectory, "DPPtCommands.json"), ScriptDatabase.DPPtScrCmdNames, ScriptDatabase.DPPtScrCmdParameters);
            ExportCommandJson(Path.Combine(ExportDirectory, "HGSSCommands.json"), ScriptDatabase.HGSSScrCmdNames, ScriptDatabase.HGSSScrCmdParameters);
        }

        private static void ExportWithMetadata<T>(string filePath, T data)
        {
            var output = new
            {
                Type = typeof(T).Name,
                Data = data
            };
            string json = JsonSerializer.Serialize(output, new JsonSerializerOptions
            {
                WriteIndented = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            });
            File.WriteAllText(filePath, json);
        }

        private static void ExportCommandJson(string filePath, Dictionary<ushort, string> commandNames, Dictionary<ushort, byte[]> commandParameters)
        {
            var commands = commandNames.ToDictionary(
                entry => entry.Key,
                entry => new {
                    Name = entry.Value,
                    Parameters = commandParameters.ContainsKey(entry.Key) ? Array.ConvertAll(commandParameters[entry.Key], b => (int)b) : null
                }
            );

            var output = new
            {
                Type = "Dictionary<ushort, CommandData>",
                Data = commands
            };

            string json = JsonSerializer.Serialize(output, new JsonSerializerOptions
            {
                WriteIndented = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            });
            File.WriteAllText(filePath, json);
        }
    }
}
