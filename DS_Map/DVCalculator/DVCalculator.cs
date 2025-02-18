using System.Collections.Generic;

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

        public static List<string> Natures = new List<string>
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


    }
}