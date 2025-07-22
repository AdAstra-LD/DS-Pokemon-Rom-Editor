using DSPRE.Resources;
using DSPRE.ROMFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;
using static DSPRE.MoveData;
using static DSPRE.RomInfo;

namespace DSPRE
{
    internal class DocTool
    {

        public static void ExportAll()
        {
            // Create the subfolder Docs in the executable directory and write the CSV files there
            string executablePath = AppDomain.CurrentDomain.BaseDirectory;
            string docsFolderPath = Path.Combine(executablePath, "Docs");

            string pokePersonalDataPath = Path.Combine(docsFolderPath, "PokemonPersonalData.csv");
            string learnsetDataPath = Path.Combine(docsFolderPath, "LearnsetData.csv");
            string evolutionDataPath = Path.Combine(docsFolderPath, "EvolutionData.csv");
            string trainerDataPath = Path.Combine(docsFolderPath, "TrainerData.txt");
            string moveDataPath = Path.Combine(docsFolderPath, "MoveData.csv");
            string TMHMDataPath = Path.Combine(docsFolderPath, "TMHMData.csv");

            DSUtils.TryUnpackNarcs(new List<DirNames> { DirNames.personalPokeData, DirNames.learnsets, DirNames.evolutions, DirNames.trainerParty, DirNames.trainerProperties ,DirNames.moveData, DirNames.itemData});

            string[] pokeNames = RomInfo.GetPokemonNames();
            string[] itemNames = RomInfo.GetItemNames();
            string[] abilityNames = RomInfo.GetAbilityNames();
            string[] moveNames = RomInfo.GetAttackNames();
            string[] trainerNames = RomInfo.GetSimpleTrainerNames();
            string[] trainerClassNames = RomInfo.GetTrainerClassNames();
            string[] typeNames = RomInfo.GetTypeNames();

            // Handle Forms
            int extraCount = RomInfo.GetPersonalFilesCount() - pokeNames.Length;
            string[] extraNames = new string[extraCount];

            for (int i = 0; i < extraCount; i++)
            {
                PokeDatabase.PersonalData.PersonalExtraFiles extraEntry = PokeDatabase.PersonalData.personalExtraFiles[i];
                extraNames[i] = pokeNames[extraEntry.monId] + " - " + extraEntry.description;
            }

            pokeNames = pokeNames.Concat(extraNames).ToArray();

            // Create the Docs folder if it doesn't exist
            if (!Directory.Exists(docsFolderPath))
            {
                Directory.CreateDirectory(docsFolderPath);
            }

            ExportPersonalDataToCSV(pokePersonalDataPath, pokeNames, abilityNames, typeNames);
            ExportLearnsetDataToCSV(learnsetDataPath, pokeNames, moveNames);
            ExportEvolutionDataToCSV(evolutionDataPath, pokeNames, itemNames, moveNames);
            ExportTrainersToText(trainerDataPath, trainerNames, trainerClassNames, pokeNames, itemNames, moveNames, abilityNames);
            ExportMoveDataToCSV(moveDataPath, moveNames, typeNames);
            ExportTMHMDataToCSV(TMHMDataPath, pokeNames);


            MessageBox.Show($"CSV files exported successfully to path: {docsFolderPath}");

        }

        private static void ExportPersonalDataToCSV(string pokePersonalDataPath, string[] pokeNames, string[] abilityNames, string[] typeNames)
        {
            // Write the Pokemon Personal Data to the CSV file
            PokemonPersonalData curPersonalData = null;
            StreamWriter sw = new StreamWriter(pokePersonalDataPath);

            sw.WriteLine("ID,Name,Type1,Type2,BaseHP,BaseAttack,BaseDefense,BaseSpecialAttack,BaseSpecialDefense,BaseSpeed," +
                "Ability1,Ability2");

            for (int i = 0; i < RomInfo.GetPersonalFilesCount(); i++)
            {
                curPersonalData = new PokemonPersonalData(i);

                string type1String = (int) curPersonalData.type1 < typeNames.Length ? typeNames[(int)curPersonalData.type1] : "UnknownType_" + (int)curPersonalData.type1;
                string type2String = (int) curPersonalData.type2 < typeNames.Length ? typeNames[(int)curPersonalData.type2] : "UnknownType_" + (int)curPersonalData.type2;

                sw.WriteLine($"{i},{pokeNames[i]},{type1String},{type2String}," +
                    $"{curPersonalData.baseHP},{curPersonalData.baseAtk},{curPersonalData.baseDef}, " +
                    $"{curPersonalData.baseSpAtk},{curPersonalData.baseSpDef},{curPersonalData.baseSpeed}," +
                    $"{abilityNames[curPersonalData.firstAbility]},{abilityNames[curPersonalData.secondAbility]}");
            }

            sw.Close();
        }

        private static void ExportLearnsetDataToCSV(string learnsetDataPath, string[] pokeNames, string[] moveNames)
        {
            // Write the Learnset Data to the CSV file
            LearnsetData curLearnsetData = null;
            StreamWriter sw = new StreamWriter(learnsetDataPath);

            sw.WriteLine("ID,Name,[Level,Move]");

            for (int i = 0; i < RomInfo.GetLearnsetFilesCount(); i++)
            {
                curLearnsetData = new LearnsetData(i);

                sw.Write($"{i},{pokeNames[i]}");

                foreach (var entry in curLearnsetData.list)
                {
                    sw.Write($",[{entry.level},{moveNames[entry.move]}]");
                }

                sw.WriteLine();
            }

            sw.Close();
        }

        private static void ExportEvolutionDataToCSV(string evolutionDataPath, string[] pokeNames, string[] itemNames, string[] moveNames)
        {
            // Write the Evolution Data to the CSV file
            EvolutionFile curEvolutionFile = null;
            StreamWriter sw = new StreamWriter(evolutionDataPath);

            sw.WriteLine("ID,Name,[Method|Param|Target]");

            for (int i = 0; i < RomInfo.GetEvolutionFilesCount(); i++)
            {
                curEvolutionFile = new EvolutionFile(i);

                sw.Write($"{i},{pokeNames[i]}");

                foreach (var entry in curEvolutionFile.data)
                {
                    EvolutionParamMeaning meaning = EvolutionFile.evoDescriptions[entry.method];

                    string paramString = "";

                    switch (meaning)
                    {
                        case EvolutionParamMeaning.Ignored:
                            paramString = "Ignored";
                            break;
                        case EvolutionParamMeaning.FromLevel:
                            paramString = entry.param.ToString();
                            break;
                        case EvolutionParamMeaning.ItemName:
                            paramString = itemNames[entry.param];
                            break;
                        case EvolutionParamMeaning.MoveName:
                            paramString = moveNames[entry.param];
                            break;
                        case EvolutionParamMeaning.PokemonName:
                            paramString = pokeNames[entry.param];
                            break;
                        case EvolutionParamMeaning.BeautyValue:
                            paramString = entry.param.ToString();
                            break;
                    }
                    if (entry.target == 0)
                    {
                        break;
                    }
                    sw.Write($",[{entry.method}|{paramString}|{pokeNames[entry.target]}]");
                }

                sw.WriteLine();

            }

            sw.Close();
        }

        private static void ExportTrainersToText(string trainerDataPath, string[] trainerNames, string[] trainerClassNames, string[] pokeNames, string[] itemNames, string[] moveNames, string[] abilityNames)
        {
            // Write the Trainer Data to the Text file
            TrainerFile curTrainerFile = null;
            TrainerProperties curTrainerProperties = null;
            FileStream curTrainerParty = null;
            StreamWriter sw = new StreamWriter(trainerDataPath);

            int trainerCount = Directory.GetFiles(RomInfo.gameDirs[DirNames.trainerProperties].unpackedDir).Length;

            for (int i = 1; i < trainerCount; i++)
            {
                string suffix = "\\" + i.ToString("D4");

                curTrainerProperties = new TrainerProperties((ushort)i,
                    new FileStream(RomInfo.gameDirs[DirNames.trainerProperties].unpackedDir + suffix, FileMode.Open));

                curTrainerParty = new FileStream(RomInfo.gameDirs[DirNames.trainerParty].unpackedDir + suffix, FileMode.Open);

                curTrainerFile = new TrainerFile(curTrainerProperties, curTrainerParty, trainerNames[i]);

                string trainerName = trainerNames[i];
                string trainerClass = trainerClassNames[curTrainerProperties.trainerClass];
                string[] trainerItems = curTrainerProperties.trainerItems.Select(item => item != 0 ? itemNames[(int)item] : "None").ToArray();

                // Create array of party pokemon
                PartyPokemon[] partyPokemon = new PartyPokemon[curTrainerProperties.partyCount];

                // Now that we have the party pokemons, we can declare the arrays to store the data
                string[] monNames = new string[partyPokemon.Length];
                PartyPokemon.GenderAndAbilityFlags[] monFlags = new PartyPokemon.GenderAndAbilityFlags[partyPokemon.Length];
                string[] items = new string[partyPokemon.Length];
                int[] levels = new int[partyPokemon.Length];
                int[] ivs = new int[partyPokemon.Length];
                string[][] moves = new string[partyPokemon.Length][];                

                for (int j = 0; j < partyPokemon.Length; j++)
                {
                    // This assumes that the non-empty mons are at the beginning of the party array which they should be
                    // if there is some way for this not to be the case, the program will crash
                    partyPokemon[j] = curTrainerFile.party[j];
                    // Type cast can be done because CountNonEmptyMons() only returns non-empty mons i.e. mons with non-null pokeID
                    monNames[j] = pokeNames[(int)partyPokemon[j].pokeID];
                    monFlags[j] = partyPokemon[j].genderAndAbilityFlags;

                    // Need to account for the case where the mon has no held item
                    if (partyPokemon[j].heldItem != null)
                    {
                        items[j] = itemNames[(int)partyPokemon[j].heldItem];
                    }
                    else
                    {
                        items[j] = "None";
                    }

                    levels[j] = partyPokemon[j].level;
                    ivs[j] = partyPokemon[j].difficulty * 31 / 255;

                    // Need to account for the case where the mon has no moves
                    if (partyPokemon[j].moves == null)
                    {
                        LearnsetData learnset = new LearnsetData((int)partyPokemon[j].pokeID);
                        moves[j] = learnset.GetLearnsetAtLevel(levels[j]).Select(move => moveNames[move]).ToArray();
                    }
                    else
                    {
                        moves[j] = partyPokemon[j].moves.Select(move => moveNames[move]).ToArray();
                    }
                    
                }

                string[] monGenders = new string[partyPokemon.Length];
                string[] abilities = new string[partyPokemon.Length];
                string[] natures = new string[partyPokemon.Length];

                // This function sets the monGenders, abilities and natures arrays
                // We hide this away in a function because it's a bit complex
                // and we don't want to clutter the main function more than it already is
                SetMonGendersAndAbilitiesAndNature(i, curTrainerProperties.trainerClass, partyPokemon, monFlags, ref abilityNames, ref monGenders, ref abilities, ref natures);


                sw.Write(TrainerToDocFormat(i, trainerName, trainerClass, trainerItems, monNames, monGenders, items, abilities, levels, natures, ivs, moves));
            }

            sw.Close();

        }

        private static void ExportMoveDataToCSV(string moveDataPath, string[] moveNames, string[] typeNames)
        {
            StreamWriter sw = new StreamWriter(moveDataPath);

            string[] moveFlags = Enum.GetNames(typeof(MoveData.MoveFlags));
            string[] attackRange = Enum.GetNames(typeof(MoveData.AttackRange));
            string[] battleSeqDesc = PokeDatabase.MoveData.battleSequenceDescriptions;

            sw.WriteLine("Move ID,Move Name,Move Type,Move Split,Power,Accuracy,Priority,Side Effect Probability,PP,Ranges,Flags,Effect Description");

            for (int i = 0; i < moveNames.Length; i++)
            {
                MoveData curMoveDataFile = new MoveData(i);

                // Lambda magic to select the flags that are set
                string moveFlagsString = string.Join("|", moveFlags.Select((flag, index) 
                    => (curMoveDataFile.flagField & (1 << index)) != 0 ? flag : "").Where(flag => !string.IsNullOrEmpty(flag)));
                
                string attackRangeString = string.Join("|", attackRange.Select((range, index) 
                    => (curMoveDataFile.target & (1 << index)) != 0 ? range : "").Where(range => !string.IsNullOrEmpty(range)));

                string battleSeqDescString = "";

                if (curMoveDataFile.battleeffect < battleSeqDesc.Length)
                {
                    battleSeqDescString = battleSeqDesc[curMoveDataFile.battleeffect];
                }

                string typeString = (int)curMoveDataFile.movetype < typeNames.Length ? typeNames[(int)curMoveDataFile.movetype] : "UnknownType_" + (int)curMoveDataFile.movetype;

                sw.WriteLine($"{i},{moveNames[i]},{curMoveDataFile.movetype},{curMoveDataFile.split}," +
                             $"{curMoveDataFile.damage},{curMoveDataFile.accuracy},{curMoveDataFile.priority}," +
                             $"{curMoveDataFile.sideEffectProbability},{curMoveDataFile.pp}," +
                             $"[{attackRangeString}],[{moveFlagsString}],{battleSeqDescString}");
            }
            
            sw.Close();
        }

        private static void ExportTMHMDataToCSV(string THHMDataPath, string[] pokeNames)
        {
            // Write the TM/HM Data to the CSV file
            PokemonPersonalData curPersonalData = null;
            StreamWriter sw = new StreamWriter(THHMDataPath);

            sw.Write("ID,Name");

            int totalTMs = PokemonPersonalData.tmsCount + PokemonPersonalData.hmsCount;

            // Write Header (List of all TMs/HMs)
            for (int count = 0; count < totalTMs; count++)
            {
                string currentItem = MachineNameFromIndex(count);
                sw.Write($",{currentItem}");

            }

            sw.WriteLine();

            for (int i = 0; i < RomInfo.GetPersonalFilesCount(); i++)
            {
                curPersonalData = new PokemonPersonalData(i);
                sw.Write($"{i},{pokeNames[i]},[");

                // Slight code duplication to PersonalDataEditor here
                for (byte b = 0; b < totalTMs; b++)
                {
                    sw.Write(b == 0 ? "" : ",");
                    sw.Write(curPersonalData.machines.Contains(b) ? "true" : "false");
                }

                sw.WriteLine("]");

            }
            sw.Close();
        }

        private static string TrainerToDocFormat(int index, string trainerName, string trainerClass, string[] trainerItems, string[] monNames, string[] monGenders, string[] items, string[] abilities,
                       int[] levels, string[] natures, int[] ivs, string[][] moves)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"[{index}] {trainerClass} {trainerName}");

            // If trainer has at least one item then list all non-zero id items behind the trainer name
            if (trainerItems.Length > 0 && trainerItems[0] != "None")
            {
                sb.Append(" @ (");
                sb.Append(string.Join(", ", trainerItems.Where(item => item != "None")));
                sb.Append(")");
            }

            sb.Append(":\n\n");

            for (int i = 0; i < monNames.Length; i++)
            {
                sb.Append(MonToShowdownFormat(monNames[i], monGenders[i], items[i], abilities[i], levels[i], natures[i], ivs[i], moves[i]));
                sb.Append("\n\n");
            }

            sb.Append("\n\n\n");

            return sb.ToString();
        }

        private static string MonToShowdownFormat(string monName, string gender, string itemName, string ability,
            int level, string nature, int ivs, string[] moves)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"{monName}");

            if (gender != "random")
            {
                sb.Append($" ({gender})");
            }

            if (itemName != "None")
            {
                sb.Append($" @ {itemName}");
            }

            sb.Append("\nAbility: " + ability);
            sb.Append("\nLevel: " + level);
            sb.Append("\n"+ nature + " Nature");

            sb.Append("\nIVs: " + string.Join(" / ", Enumerable.Repeat(ivs.ToString(), 6)));

            moves = moves.Where(move => (move != "None" && move != "-")).ToArray();

            sb.Append("\n- " + string.Join("\n- ", moves));

            return sb.ToString();

        }

        // Slight code duplication to PersonalDataEditor here
        private static string MachineNameFromIndex(int n)
        {
            //0-91 --> TMs
            //>=92 --> HM
            n += 1;
            int diff = n - PokemonPersonalData.tmsCount;
            string type = diff > 0 ? "HM" : "TM";
            string item = $"{type}{(diff > 0 ? diff : n):D2}";
            return item;
        }


        private static void SetMonGendersAndAbilitiesAndNature(int trainerID, int trainerClassID, PartyPokemon[] partyPokemon,
            PartyPokemon.GenderAndAbilityFlags[] monFlags, ref string[] abilityNames,
            ref string[] monGenders, ref string[] abilities, ref string[] natures)
        { 
            bool trainerMale = false;

            trainerMale = DVCalculator.TrainerClassGender.GetTrainerClassGender(trainerClassID);
            DVCalculator.ResetGenderMod(trainerMale);

            // Get Pokemon Genders and Abilities from flags
            for (int j = 0; j < partyPokemon.Length; j++)
            {

                byte baseGenderRatio = new PokemonPersonalData((int)partyPokemon[j].pokeID).genderVec;
                byte genderOverride = (byte)((byte) monFlags[j] & 0x0F); // Get the lower 4 bits
                byte abilityOverride = (byte)((byte)monFlags[j] >> 4); // Get the upper 4 bits
                
                uint PID = DVCalculator.generatePID((uint)trainerID, (uint)trainerClassID, (uint)partyPokemon[j].pokeID, (byte)partyPokemon[j].level, baseGenderRatio, genderOverride, abilityOverride, partyPokemon[j].difficulty);
                natures[j] = DVCalculator.Natures[DVCalculator.getNatureFromPID(PID)].Split(':')[0];

                switch (genderOverride)
                {
                    case 0: // Random
                        monGenders[j] = "random";
                        break;
                    case 1: // Male
                        monGenders[j] = "M";
                        break;
                    case 2: // Female
                        monGenders[j] = "F";
                        break;
                }

                switch (PID % 2) // Lowest bit of PID determines the ability
                {
                    case 0:
                        abilities[j] = abilityNames[new PokemonPersonalData((int)partyPokemon[j].pokeID).firstAbility];
                        break;
                    case 1:
                        abilities[j] = abilityNames[new PokemonPersonalData((int)partyPokemon[j].pokeID).secondAbility];
                        break;
                }
            }
        }
    }
}
