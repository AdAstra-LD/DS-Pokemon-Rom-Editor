using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace DSPRE
{
    // Putting this class here is bad practice, however it's just a really small utility class
    public class DVIVNatureTriplet
    {
        public int DV { get; set; }
        public int IV { get; set; }
        public string Nature { get; set; }

        public DVIVNatureTriplet(int DV, int IV, string Nature)
        {
            this.DV = DV;
            this.IV = IV;
            this.Nature = Nature;
        }
    }

    internal static class DVCalculator
    {
        private static uint state;
        public static void setSeed(uint seed)
        {

            DVCalculator.state = seed;

        }

        // this code is partially lifted from turtleisaac's Pokeditor
        // See https://github.com/turtleisaac/PokEditor-v2/blob/72ca6ab641f616b8be9a87624b81896baa45f947/src/com/turtleisaac/pokeditor/utilities/TrainerPersonalityCalculator.java
        // and the pokeplatinum / pokeheartgold projects See: https://github.com/pret
        public static uint getNextRandom()
        {
            state = 1103515245 * state + 24691;

            // Upper 16 bit are random value
            return state >> 16;
        }

        public static int findHighestDV(uint trainerIdx, uint trainerClassIdx, bool trainerClassMale, uint pokeIdx, byte pokeLevel, byte baseGenderRatio, int genderOverride, int abilityOverride, uint nature)
        {
            byte DV;

            for (DV = 255; DV > 0; DV--)
            {
                if (getNatureFromPID(generatePID(trainerIdx, trainerClassIdx, trainerClassMale, pokeIdx, pokeLevel, baseGenderRatio, genderOverride, abilityOverride, DV)) == nature)
                { return DV; }
            }

            return -1;

        }

        public static uint generatePID(uint trainerIdx, uint trainerClassIdx, bool trainerClassMale, uint pokeIdx, byte pokeLevel, byte baseGenderRatio, int genderOverride, int abilityOverride, byte difficultyValue)
        {
            uint newSeed = (uint)(trainerIdx + pokeIdx + pokeLevel + difficultyValue);

            setSeed(newSeed);

            uint random = 0;

            while (trainerClassIdx > 0)
            {
                trainerClassIdx--;
                random = getNextRandom();
            }

            uint genderMod = 0;

            // this is always the case in platinum
            if (genderOverride == 0)
            {
                genderMod = trainerClassMale ? 136u : 120u;
            }

            // Code from here in is HGSS exclusive
            if (genderOverride == 1)
            {
                genderMod = baseGenderRatio + 2u;
            }

            else if (genderOverride == 2)
            {
                genderMod = baseGenderRatio - 2u;
            }

            // Force Ability 1 --> Force lowest bit to 0
            if (abilityOverride == 1)
            {
                genderMod = (uint)(genderMod & ~1);
            }

            // Force Ability 2 --> Force lowest bit to 1
            else if (abilityOverride == 2)
            {
                genderMod = (uint)(genderMod | 1);
            }

            uint PID = (random << 8) + genderMod;

            return (uint)PID;
        }

        public static uint getNatureFromPID(uint PID)
        {
            return (PID % 100) % 25;
        }

        public static List<DVIVNatureTriplet> getAllNatures(uint trainerIdx, uint trainerClassIdx, bool trainerClassMale, uint pokeIdx, byte pokeLevel, byte baseGenderRatio, int genderOverride, int abilityOverride)
        {
            List<DVIVNatureTriplet> natureDict = new List<DVIVNatureTriplet>();

            byte DV;
            uint natureIdx;

            for (DV = 255; DV > 0; DV--)
            {
                natureIdx = getNatureFromPID(generatePID(trainerIdx, trainerClassIdx, trainerClassMale, pokeIdx, pokeLevel, baseGenderRatio, genderOverride, abilityOverride, DV));

                natureDict.Add(new DVIVNatureTriplet(DV, DV * 31 / 255, Natures[(int)natureIdx]));

            }

            return natureDict;
        }

        public static readonly List<string> Natures = new List<string>
            {
            "Hardy: Neutral",
            "Lonely: +Atk, -Def",
            "Brave: +Atk, -Spe",
            "Adamant: +Atk, -SpA",
            "Naughty: +Atk, -SpD",
            "Bold: +Def, -Atk",
            "Docile: Neutral",
            "Relaxed: +Def, -Spe",
            "Impish: +Def, -SpA",
            "Lax: +Def, -SpD",
            "Timid: +Spe, -Atk",
            "Hasty: +Spe, -Def",
            "Serious: Neutral",
            "Jolly: +Spe, -SpA",
            "Naive: +Spe, -SpD",
            "Modest: +SpA, -Atk",
            "Mild: +SpA, -Def",
            "Quiet: +SpA, -Spe",
            "Bashful: Neutral",
            "Rash: +SpA, -SpD",
            "Calm: +SpD, -Atk",
            "Gentle: +SpD, -Def",
            "Sassy: +SpD, -Spe",
            "Careful: +SpD, -SpA",
            "Quirky: Neutral"
            };

        public static class TrainerClassGender
        {

            // true represents male, false represents female
            private static List<bool> trainerClassGenders = new List<bool>();

            private static bool tableLoaded = false;

            public static bool GetTrainerClassGender(int trainerClassID)
            {
                if (!tableLoaded)
                {
                    ReadTrainerClassGenderTable();
                }
                return trainerClassGenders[trainerClassID];
            }
                
            public static void ReadTrainerClassGenderTable()
            {
                uint offset = GetTableOffset();
                uint length = GetTableLength();
                if (offset == 0 || length == 0)
                {
                    MessageBox.Show("Couldn't load trainer class gender table from arm9." +
                        "\nTrainers will default to male when calculating natures.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tableLoaded = true;
                    return;
                }

                byte[] table = ARM9.ReadBytes(offset, length);

                for (int i = 0; i < table.Length; i++)
                {
                    trainerClassGenders.Add(table[i] == 1 ? false : true);
                }
                tableLoaded = true;

            }

            private static uint GetTableLength()
            {
                switch (RomInfo.gameFamily)
                {
                    case RomInfo.GameFamilies.Plat:
                        return 105; // Platinum has 105 trainer classes
                    case RomInfo.GameFamilies.HGSS:
                        return 128;
                    case RomInfo.GameFamilies.DP:
                        return 0;
                    default:
                        // Unknown game family
                        return 0;
                }
                    
            }

            private static uint GetTableOffset()
            {
                switch (RomInfo.gameFamily)
                {
                    case RomInfo.GameFamilies.Plat:
                        switch(RomInfo.gameLanguage)
                        {
                            case RomInfo.GameLanguages.English:
                                return 0xF0714;
                            case RomInfo.GameLanguages.Japanese:
                                return 0xEFDA4;
                            case RomInfo.GameLanguages.Spanish:
                                return 0xF078A;
                            case RomInfo.GameLanguages.German:
                                return 0xF076C;
                            case RomInfo.GameLanguages.French:
                                return 0xF079C;
                            case RomInfo.GameLanguages.Italian:
                                return 0xF0730;
                            default:
                                // Unknown game language
                                return 0;
                        }
                    case RomInfo.GameFamilies.HGSS:
                        switch (RomInfo.gameLanguage)
                        {
                            case RomInfo.GameLanguages.English:
                                return 0xFFB90;
                            case RomInfo.GameLanguages.Japanese:
                                return 0xFF310;
                            case RomInfo.GameLanguages.Spanish:
                                return 0xFFB90;
                            case RomInfo.GameLanguages.German:
                                return 0xFFB44;
                            case RomInfo.GameLanguages.French:
                                return 0xFFB74;
                            case RomInfo.GameLanguages.Italian:
                                return 0xFFB08;
                            default:
                                // Unknown game language
                                return 0;
                        }
                    case RomInfo.GameFamilies.DP:
                        // Dummy offset for DP
                        return 0;
                    default:
                        // Unknown game family
                        return 0;
                }
            }

        }

            

    }
}