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
        private static long seed;

        public static void setSeed(long seed)
        {
            DVCalculator.seed = seed;
        }

        public static long getSeed()
        {
            return DVCalculator.seed;
        }


        // This function is lifted from turtleisaac's Pokeditor (with permission)
        // See https://github.com/turtleisaac/PokEditor-v2/blob/72ca6ab641f616b8be9a87624b81896baa45f947/src/com/turtleisaac/pokeditor/utilities/TrainerPersonalityCalculator.java
        public static long getNextRandom()
        {
            long random = 0x41c64e6d * seed + 0x6073;

            //last 32 bits is new seed
            seed = random & 0xFFFFFFFFL;

            return random;
        }

        public static int findHighestDV(int trainerIdx, int trainerClassIdx, bool trainerClassMale, int pokeIdx, int pokeLevel, uint nature)
        {
            int DV;

            // Iterate over all possible PIDs and return highest DV that yields the desired nature
            for (DV = 255; DV >= 0; DV--)
            {
                if (getNatureFromPID(generatePID(trainerIdx, trainerClassIdx, trainerClassMale, pokeIdx, pokeLevel, DV)) == nature)
                { return DV; }
            }

            return -1;

        }

        // this function is lifted from turtleisaac's Pokeditor (with permission)
        // See https://github.com/turtleisaac/PokEditor-v2/blob/72ca6ab641f616b8be9a87624b81896baa45f947/src/com/turtleisaac/pokeditor/utilities/TrainerPersonalityCalculator.java
        public static uint generatePID(int trainerIdx, int trainerClassIdx, bool trainerClassMale, int pokeIdx, int pokeLevel, int difficultyValue)
        {
            long newSeed = trainerIdx + pokeIdx + pokeLevel + difficultyValue;

            long random = 0;

            setSeed(newSeed);

            while (trainerClassIdx > 0)
            {
                trainerClassIdx--;
                random = getNextRandom();
            }

            // Don't really get this part? Why are we shifting to the right then left again?
            long PID = (random >> 16) & 0xffff;
            PID = PID * 256;

            // This seems super arbitrary (wtf GameFreak?)
            PID += trainerClassMale ? 136 : 120;

            return (uint)PID;
        }

        public static uint getNatureFromPID(uint PID)
        {
            return (PID % 100) % 25;
        }

        public static List<DVIVNatureTriplet> getAllNatures(int trainerIdx, int trainerClassIdx, bool trainerClassMale, int pokeIdx, int pokeLevel)
        {
            List<DVIVNatureTriplet> natureList = new List<DVIVNatureTriplet>();

            int DV;
            uint natureIdx;

            // Iterate over all possible PIDs and store the DV IV and Nature String in the custom data type
            for (DV = 255; DV >= 0; DV--)
            {
                natureIdx = getNatureFromPID(generatePID(trainerIdx, trainerClassIdx, trainerClassMale, pokeIdx, pokeLevel, DV));

                natureList.Add(new DVIVNatureTriplet(DV, DV*31/255 , Natures[(int)natureIdx]));

            }

            return natureList;
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


    }
}