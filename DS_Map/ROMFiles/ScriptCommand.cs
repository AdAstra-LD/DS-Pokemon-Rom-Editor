using DSPRE.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DSPRE.ROMFiles {
    public class ScriptCommand {

        public ushort? id;
        public List<byte[]> cmdParams;
        public string name;

        public ScriptCommand(ushort id, List<byte[]> parameterData)
        {
            this.id = id;
            cmdParams = parameterData;

            // Get command name
            if (!RomInfo.ScriptCommandNamesDict.TryGetValue(id, out name))
            {
                name = $"CMD_{id:X3}";
            }

            // Get parameter types
            List<ScriptParameter.ParameterType> paramTypes = null;
            switch (RomInfo.gameFamily)
            {
                case RomInfo.GameFamilies.DP:
                    ScriptDatabase.DPScrCmdParameterTypes.TryGetValue(id, out paramTypes);
                    break;
                case RomInfo.GameFamilies.Plat:
                    ScriptDatabase.PlatScrCmdParameterTypes.TryGetValue(id, out paramTypes);
                    break;
                case RomInfo.GameFamilies.HGSS:
                    ScriptDatabase.HGSSScrCmdParameterTypes.TryGetValue(id, out paramTypes);
                    break;
            }

            // Format parameters based on their types
            if (paramTypes != null && parameterData != null)
            {
                for (int i = 0; i < Math.Min(paramTypes.Count, parameterData.Count); i++)
                {
                    var param = new ScriptParameter(parameterData[i], paramTypes[i]);
                    name += " " + param.DisplayValue;
                }
            }
            else if (parameterData != null)
            {
                foreach (var param in parameterData)
                {
                    name += " " + new ScriptParameter(param, ScriptParameter.ParameterType.Integer).DisplayValue;
                }
            }
        }

        public ScriptCommand(string wholeLine, int lineNumber = 0) {
            name = wholeLine;
            cmdParams = new List<byte[]>();

            var processedLine = ProcessBracketedItems(wholeLine);
            string[] nameParts = processedLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries); // Separate command code from parameters
            /* Get command id, which is always first in the description */

            if (RomInfo.ScriptCommandNamesReverseDict.TryGetValue(nameParts[0].ToLower(), out ushort cmdID)) {
                id = cmdID;
            } else {
                try {
                    id = ushort.Parse(nameParts[0].PurgeSpecial(ScriptFile.specialChars), nameParts[0].GetNumberStyle());
                } catch {
                    string details;
                    if (wholeLine.Contains(':') && wholeLine.ContainsNumber()) {
                        details = "This probably means you forgot to \"End\" the Script or Function above it.";
                        details += Environment.NewLine + "Please, also note that only Functions can be terminated\nwith \"Return\".";
                    } else {
                        details = "Are you sure it's a proper Script Command?";
                    }

                    MessageBox.Show("This Script file could not be saved." +
                                    $"\nParser failed to interpret line {lineNumber}: \"{wholeLine}\".\n\n{details}", "Parser error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            /* Read parameters from remainder of the description */
            byte[] parametersSizeArr = RomInfo.ScriptCommandParametersDict[(ushort)id];

            int paramLength = 0;
            int paramsProcessed = 0;

            if (parametersSizeArr.Length > 0 && parametersSizeArr.First() == 0xFF) {
                int firstParamValue = int.Parse(nameParts[1].PurgeSpecial(ScriptFile.specialChars), nameParts[1].GetNumberStyle());
                byte firstParamSize = parametersSizeArr[1];

                cmdParams.Add(firstParamValue.ToByteArrayChooseSize(firstParamSize));
                paramsProcessed++;

                int i = 2;
                int optionsCount = 0;

                bool found = false;
                while (i < parametersSizeArr.Length) {
                    paramLength = parametersSizeArr[i + 1];

                    if (parametersSizeArr[i] == firstParamValue) {
                        //Firstly, build subarray of parameter sizes, starting from the chosen option [firstParamValue]
                        //FOR EXAMPLE: CMD 0x235 and firstParamValue = 5

                        // { 0xFF, 2,  
                        // 0, 1,   2,       
                        // 1, 3,   2, 2, 2, 
                        // 2, 0,            
                        // 3, 3,   2, 2, 2, 
                        // 4, 2,   2, 2,    
                        // 5, 3,   (2, 2, 2) => this will be the parameters subarray 
                        // 6, 1,   2
                        // },      
                        byte[] subParametersSize = parametersSizeArr.SubArray(i + 2, paramLength++);

                        //Create a slightly bigger temp array 
                        byte[] temp = new byte[1 + subParametersSize.Length];

                        //Store the size of the firstParamValue there
                        temp[0] = firstParamSize;

                        //Then copy the whole subarray of parameter sizes
                        Array.Copy(subParametersSize, 0, temp, 1, temp.Length - 1);

                        //Replace the original parametersSizeArr with the new array
                        parametersSizeArr = temp;
                        found = true;
                        break;
                    }

                    i += 2 + paramLength;
                    optionsCount++;
                }

                if (!found) {
                    MessageBox.Show($"Command {nameParts[0]} is a special Script Command.\n" +
                                    $"The value of the first parameter must be a number in the range [0 - {optionsCount}].\n\n" +
                                    $"Line {lineNumber}: {wholeLine}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    id = null;
                    return;
                }
            } else if (parametersSizeArr.Length == 1 && parametersSizeArr.First() == 0) {
                paramLength = 0;
            } else {
                paramLength = parametersSizeArr.Length;
            }

            if (nameParts.Length - 1 == paramLength) {
                for (int i = paramsProcessed; i < paramLength; i++) {
                    AppLogger.Debug($"Parameter #{i}: {nameParts[i + 1]}");

                    if (RomInfo.ScriptComparisonOperatorsReverseDict.TryGetValue(nameParts[i + 1].ToLower(), out cmdID)) { //Check succeeds when command is like "asdfg LESS" or "asdfg DIFFERENT"
                        cmdParams.Add(new byte[] { (byte)cmdID });
                    } else { //Not a comparison
                        /* Convert strings of parameters to the correct datatypes */
                        NumberStyles numStyle = nameParts[i + 1].GetNumberStyle();
                        if (!nameParts[i + 1].StartsWith("SEQ_") 
                            && !nameParts[i + 1].StartsWith("SPECIES_") 
                            && !nameParts[i + 1].StartsWith("ITEM_")
                            && !nameParts[i + 1].StartsWith("MOVE_")
                            && !nameParts[i + 1].StartsWith("TRAINER_")) {
                            nameParts[i + 1] = nameParts[i + 1].PurgeSpecial(ScriptFile.specialChars);
                        }

                        int result = 0;

                        try
                        {
                            result = int.Parse(nameParts[i + 1], numStyle);
                        }
                        catch (FormatException)
                        {
                            try
                            {
                                string paramToCheck = CheckAndCompareParam(nameParts[i + 1]);
                                var first = ScriptDatabase.specialOverworlds.FirstOrDefault(x => x.Value.IgnoreCaseEquals(paramToCheck));
                                if (!string.IsNullOrWhiteSpace(first.Value))
                                {
                                    result = first.Key;
                                }
                                else
                                {
                                    var direction = ScriptDatabase.overworldDirections.FirstOrDefault(x => x.Value.IgnoreCaseEquals(paramToCheck));
                                    if (!string.IsNullOrWhiteSpace(direction.Value))
                                    {
                                        result = direction.Key;
                                    }
                                    else
                                    {
                                        var pokemon = ScriptDatabase.pokemonNames.FirstOrDefault(x => x.Value.IgnoreCaseEquals(paramToCheck));
                                        if (!string.IsNullOrWhiteSpace(pokemon.Value))
                                        {
                                            result = pokemon.Key;
                                        }
                                        else
                                        {
                                            var item = ScriptDatabase.itemNames.FirstOrDefault(x => x.Value.IgnoreCaseEquals(paramToCheck));
                                            if (!string.IsNullOrWhiteSpace(item.Value))
                                            {
                                                result = item.Key;
                                            }
                                            else
                                            {
                                                var move = ScriptDatabase.moveNames.FirstOrDefault(x => x.Value.IgnoreCaseEquals(paramToCheck));
                                                if (!string.IsNullOrWhiteSpace(move.Value))
                                                {
                                                    result = move.Key;
                                                }
                                                else
                                                {
                                                    var sound = ScriptDatabase.soundNames.FirstOrDefault(x => x.Value.IgnoreCaseEquals(paramToCheck));
                                                    if (!string.IsNullOrWhiteSpace(sound.Value))
                                                    {
                                                        result = sound.Key;
                                                    }
                                                    else
                                                    {
                                                        var trainer = ScriptDatabase.trainerNames.FirstOrDefault(x => x.Value.IgnoreCaseEquals(paramToCheck));
                                                        if (!string.IsNullOrWhiteSpace(trainer.Value))
                                                        {
                                                            result = trainer.Key;
                                                        }
                                                        else
                                                        {
                                                            MessageBox.Show($"Argument {paramToCheck} couldn't be parsed as a valid Condition, Overworld ID, Direction ID, Pokemon, Item, Move, Sound, Trainer, Script, Function or Action number.\n\n" +
                                                                $"Line {lineNumber}: {wholeLine}", "Invalid identifier", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                            id = null;
                                                            return;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            catch (ArgumentException ex)
                            {
                                MessageBox.Show($"{ex.Message}\n\nLine {lineNumber}: {wholeLine}",
                                    "Invalid syntax", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                id = null;
                                return;
                            }
                        }

                        try
                        {
                            cmdParams.Add(result.ToByteArrayChooseSize(parametersSizeArr[i]));
                        }
                        catch (OverflowException)
                        {
                            MessageBox.Show($"Argument {nameParts[i + 1]} at line {lineNumber} is not in the range [0, {Math.Pow(2, 8 * parametersSizeArr[i]) - 1}].", "Argument error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            id = null;
                        }
                    }
                }
            } else {
                MessageBox.Show($"Wrong number of parameters for command {nameParts[0]} at line {lineNumber}.\n" +
                                $"Received: {nameParts.Length - 1}\n" +
                                $"Expected: {paramLength}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                id = null;
            }
        }

        private string ProcessBracketedItems(string line)
        {
            // Early exit if no brackets
            if (!line.Contains('[')) return line;

            StringBuilder result = new StringBuilder(line);
            int currentPos = 0;

            while (true)
            {
                int start = result.ToString().IndexOf('[', currentPos);
                if (start == -1) break;

                int end = result.ToString().IndexOf(']', start);
                if (end == -1) break;

                // Process only the current bracket pair
                for (int i = start + 1; i < end; i++)
                {
                    if (result[i] == ' ')
                    {
                        result[i] = '§';
                    }
                }

                currentPos = end + 1;
            }

            return result.ToString();
        }

        private int LevenshteinDistance(string s1, string s2)
        {
            int[,] d = new int[s1.Length + 1, s2.Length + 1];

            for (int i = 0; i <= s1.Length; i++)
                d[i, 0] = i;
            for (int j = 0; j <= s2.Length; j++)
                d[0, j] = j;

            for (int i = 1; i <= s1.Length; i++)
            {
                for (int j = 1; j <= s2.Length; j++)
                {
                    int cost = (s2[j - 1] == s1[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(Math.Min(
                        d[i - 1, j] + 1,
                        d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            return d[s1.Length, s2.Length];
        }

        private string FindClosestMatch(string input, IEnumerable<string> possibilities, int threshold = 3)
        {
            // Remove brackets and spaces for comparison
            input = input.Trim('[', ']').Replace(" ", "").ToLower();

            var closest = possibilities
                .Select(x => new {
                    Name = x,
                    Distance = LevenshteinDistance(
                        input,
                        x.Replace(" ", "").ToLower()
                    )
                })
                .Where(x => x.Distance <= threshold)
                .OrderBy(x => x.Distance)
                .FirstOrDefault();

            return closest?.Name;
        }

        private string CheckAndCompareParam(string parameter)
        {

            // Check for Pokemon names first
            var pokemon = ScriptDatabase.pokemonNames.FirstOrDefault(x =>
                x.Value.IgnoreCaseEquals(parameter));
            if (!string.IsNullOrWhiteSpace(pokemon.Value))
            {
                return pokemon.Value;
            }
            var item = ScriptDatabase.itemNames.FirstOrDefault(x =>
                x.Value.IgnoreCaseEquals(parameter));
            if (!string.IsNullOrWhiteSpace(item.Value))
            {
                return item.Value;
            }
            var move = ScriptDatabase.moveNames.FirstOrDefault(x =>
                x.Value.IgnoreCaseEquals(parameter));
            if (!string.IsNullOrWhiteSpace(move.Value))
            {
                return move.Value;
            }
            var sound = ScriptDatabase.soundNames.FirstOrDefault(x =>
                x.Value.IgnoreCaseEquals(parameter));
            if (!string.IsNullOrWhiteSpace(sound.Value))
            {
                return sound.Value;
            }
            var trainer = ScriptDatabase.trainerNames.FirstOrDefault(x =>
                x.Value.IgnoreCaseEquals(parameter));
            if (!string.IsNullOrWhiteSpace(trainer.Value))
            {
                return trainer.Value;
            }

            string closestItem = FindClosestMatch(parameter, ScriptDatabase.itemNames.Values);
            if (!string.IsNullOrWhiteSpace(closestItem))
            {
                throw new ArgumentException($"'{parameter}' is not a valid Item.\nDid you mean {closestItem}?");
            }
            string closestPokemon = FindClosestMatch(parameter, ScriptDatabase.pokemonNames.Values);
            if (!string.IsNullOrWhiteSpace(closestPokemon))
            {
                throw new ArgumentException($"'{parameter}' is not a valid Pokemon.\nDid you mean {closestPokemon}?");
            }
            string closestMove = FindClosestMatch(parameter, ScriptDatabase.moveNames.Values);
            if (!string.IsNullOrWhiteSpace(closestMove))
            {
                throw new ArgumentException($"'{parameter}' is not a valid Move.\nDid you mean {closestMove}?");
            }
            string closestSound = FindClosestMatch(parameter, ScriptDatabase.soundNames.Values);
            if (!string.IsNullOrWhiteSpace(closestSound))
            {
                throw new ArgumentException($"'{parameter}' is not a valid Sound name.\nDid you mean {closestSound}?");
            }
            string closestTrainer = FindClosestMatch(parameter, ScriptDatabase.trainerNames.Values);
            if (!string.IsNullOrWhiteSpace(closestTrainer))
            {
                throw new ArgumentException($"'{parameter}' is not a valid Trainer.\nDid you mean {closestTrainer}?");
            }

            return parameter;
        }
        

    }
}
