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
            // Default Trainer Genders and Classes taken from pokeplatinum and pokeheartgold projects
            // Should probably read the actual game data instead of hardcoding this
            public static readonly bool[] IsMalePlatinum = new bool[]
                {
                    true,  // TRAINER_CLASS_PLAYER_MALE
                    false, // TRAINER_CLASS_PLAYER_FEMALE
                    true,  // TRAINER_CLASS_YOUNGSTER
                    false, // TRAINER_CLASS_LASS
                    true,  // TRAINER_CLASS_CAMPER
                    false, // TRAINER_CLASS_PICNICKER
                    true,  // TRAINER_CLASS_BUG_CATCHER
                    false, // TRAINER_CLASS_AROMA_LADY
                    false, // TRAINER_CLASS_TWINS
                    true,  // TRAINER_CLASS_HIKER
                    false, // TRAINER_CLASS_BATTLE_GIRL
                    true,  // TRAINER_CLASS_FISHERMAN
                    true,  // TRAINER_CLASS_CYCLIST_MALE
                    false, // TRAINER_CLASS_CYCLIST_FEMALE
                    true,  // TRAINER_CLASS_BLACK_BELT
                    true,  // TRAINER_CLASS_ARTIST
                    true,  // TRAINER_CLASS_BREEDER_MALE
                    false, // TRAINER_CLASS_BREEDER_FEMALE
                    false, // TRAINER_CLASS_COWGIRL
                    true,  // TRAINER_CLASS_JOGGER
                    true,  // TRAINER_CLASS_POKEFAN_MALE
                    false, // TRAINER_CLASS_POKEFAN_FEMALE
                    false, // TRAINER_CLASS_POKE_KID
                    false, // TRAINER_CLASS_YOUNG_COUPLE
                    true,  // TRAINER_CLASS_ACE_TRAINER_MALE
                    false, // TRAINER_CLASS_ACE_TRAINER_FEMALE
                    false, // TRAINER_CLASS_WAITRESS
                    true,  // TRAINER_CLASS_VETERAN
                    true,  // TRAINER_CLASS_NINJA_BOY
                    true,  // TRAINER_CLASS_DRAGON_TAMER
                    false, // TRAINER_CLASS_BIRD_KEEPER
                    false, // TRAINER_CLASS_DOUBLE_TEAM
                    true,  // TRAINER_CLASS_RICH_BOY
                    false, // TRAINER_CLASS_LADY
                    true,  // TRAINER_CLASS_GENTLEMAN
                    false, // TRAINER_CLASS_SOCIALITE
                    false, // TRAINER_CLASS_BEAUTY
                    true,  // TRAINER_CLASS_COLLECTOR
                    true,  // TRAINER_CLASS_POLICEMAN
                    true,  // TRAINER_CLASS_RANGER_MALE
                    false, // TRAINER_CLASS_RANGER_FEMALE
                    true,  // TRAINER_CLASS_SCIENTIST
                    true,  // TRAINER_CLASS_SWIMMER_MALE
                    false, // TRAINER_CLASS_SWIMMER_FEMALE
                    true,  // TRAINER_CLASS_TUBER_MALE
                    false, // TRAINER_CLASS_TUBER_FEMALE
                    true,  // TRAINER_CLASS_SAILOR
                    false, // TRAINER_CLASS_SIS_AND_BRO
                    true,  // TRAINER_CLASS_RUIN_MANIAC
                    true,  // TRAINER_CLASS_PSYCHIC_MALE
                    false, // TRAINER_CLASS_PSYCHIC_FEMALE
                    true,  // TRAINER_CLASS_PI
                    true,  // TRAINER_CLASS_GUITARIST
                    true,  // TRAINER_CLASS_ACE_TRAINER_SNOW_MALE
                    false, // TRAINER_CLASS_ACE_TRAINER_SNOW_FEMALE
                    true,  // TRAINER_CLASS_SKIER_MALE
                    false, // TRAINER_CLASS_SKIER_FEMALE
                    true,  // TRAINER_CLASS_ROUGHNECK
                    true,  // TRAINER_CLASS_CLOWN
                    true,  // TRAINER_CLASS_WORKER
                    true,  // TRAINER_CLASS_SCHOOL_KID_MALE
                    false, // TRAINER_CLASS_SCHOOL_KID_FEMALE
                    true,  // TRAINER_CLASS_LEADER_ROARK
                    true,  // TRAINER_CLASS_RIVAL
                    true,  // TRAINER_CLASS_LEADER_BYRON
                    true,  // TRAINER_CLASS_ELITE_FOUR_AARON
                    false, // TRAINER_CLASS_ELITE_FOUR_BERTHA
                    true,  // TRAINER_CLASS_ELITE_FOUR_FLINT
                    true,  // TRAINER_CLASS_ELITE_FOUR_LUCIAN
                    false, // TRAINER_CLASS_CHAMPION_CYNTHIA
                    false, // TRAINER_CLASS_BELLE_AND_PA
                    true,  // TRAINER_CLASS_RANCHER
                    false, // TRAINER_CLASS_COMMANDER_MARS
                    true,  // TRAINER_CLASS_GALACTIC_GRUNT_MALE
                    false, // TRAINER_CLASS_LEADER_GARDENIA
                    true,  // TRAINER_CLASS_LEADER_WAKE
                    false, // TRAINER_CLASS_LEADER_MAYLENE
                    false, // TRAINER_CLASS_LEADER_FANTINA
                    false, // TRAINER_CLASS_LEADER_CANDICE
                    true,  // TRAINER_CLASS_LEADER_VOLKNER
                    false, // TRAINER_CLASS_PARASOL_LADY
                    true,  // TRAINER_CLASS_WAITER
                    false, // TRAINER_CLASS_INTERVIEWERS
                    true,  // TRAINER_CLASS_CAMERAMAN
                    false, // TRAINER_CLASS_REPORTER
                    false, // TRAINER_CLASS_IDOL
                    true,  // TRAINER_CLASS_GALACTIC_BOSS
                    false, // TRAINER_CLASS_COMMANDER_JUPITER
                    false, // TRAINER_CLASS_COMMANDER_SATURN
                    false, // TRAINER_CLASS_GALACTIC_GRUNT_FEMALE
                    false, // TRAINER_CLASS_TRAINER_CHERYL
                    true,  // TRAINER_CLASS_TRAINER_RILEY
                    false, // TRAINER_CLASS_TRAINER_MARLEY
                    true,  // TRAINER_CLASS_TRAINER_BUCK
                    false, // TRAINER_CLASS_TRAINER_MIRA
                    true,  // TRAINER_CLASS_DP_PLAYER_MALE
                    false, // TRAINER_CLASS_DP_PLAYER_FEMALE
                    true,  // TRAINER_CLASS_TOWER_TYCOON
                    false, // TRAINER_CLASS_MAID
                    false, // TRAINER_CLASS_HALL_MATRON
                    true,  // TRAINER_CLASS_FACTORY_HEAD
                    false, // TRAINER_CLASS_ARCADE_STAR
                    true,  // TRAINER_CLASS_CASTLE_VALET
                    true,  // TRAINER_CLASS_DP_PLAYER_MALE_2
                    false  // TRAINER_CLASS_DP_PLAYER_FEMALE_2
                };

                public static readonly bool[] IsMaleHGSS = new bool[]
                {
                    true, // TRAINERCLASS_PKMN_TRAINER_ETHAN
                    false, // TRAINERCLASS_PKMN_TRAINER_LYRA
                    true, // TRAINERCLASS_YOUNGSTER
                    false, // TRAINERCLASS_LASS
                    true, // TRAINERCLASS_CAMPER
                    false, // TRAINERCLASS_PICNICKER
                    true, // TRAINERCLASS_BUG_CATCHER
                    false, // TRAINERCLASS_AROMA_LADY
                    false, // TRAINERCLASS_TWINS
                    true, // TRAINERCLASS_HIKER
                    false, // TRAINERCLASS_BATTLE_GIRL
                    true, // TRAINERCLASS_FISHERMAN
                    true, // TRAINERCLASS_CYCLIST_M
                    false, // TRAINERCLASS_CYCLIST_F
                    true, // TRAINERCLASS_BLACK_BELT
                    true, // TRAINERCLASS_ARTIST
                    true, // TRAINERCLASS_PKMN_BREEDER_M
                    false, // TRAINERCLASS_PKMN_BREEDER_F
                    false, // TRAINERCLASS_COWGIRL
                    true, // TRAINERCLASS_JOGGER
                    true, // TRAINERCLASS_POKEFAN_M
                    false, // TRAINERCLASS_POKEFAN
                    false, // TRAINERCLASS_POKE_KID
                    true, // TRAINERCLASS_RIVAL
                    true, // TRAINERCLASS_ACE_TRAINER_M
                    false, // TRAINERCLASS_ACE_TRAINER_F
                    false, // TRAINERCLASS_WAITRESS
                    true, // TRAINERCLASS_VETERAN
                    true, // TRAINERCLASS_NINJA_BOY
                    true, // TRAINERCLASS_DRAGON_TAMER
                    false, // TRAINERCLASS_BIRD_KEEPER
                    true, // TRAINERCLASS_JUGGLER
                    true, // TRAINERCLASS_RICH_BOY
                    false, // TRAINERCLASS_LADY
                    true, // TRAINERCLASS_GENTLEMAN
                    false, // TRAINERCLASS_SOCIALITE
                    false, // TRAINERCLASS_BEAUTY
                    true, // TRAINERCLASS_COLLECTOR
                    true, // TRAINERCLASS_POLICEMAN
                    true, // TRAINERCLASS_PKMN_RANGER_M
                    false, // TRAINERCLASS_PKMN_RANGER_F
                    true, // TRAINERCLASS_SCIENTIST
                    true, // TRAINERCLASS_SWIMMER_M
                    false, // TRAINERCLASS_SWIMMER_F
                    true, // TRAINERCLASS_TUBER_M
                    false, // TRAINERCLASS_TUBER_F
                    true, // TRAINERCLASS_SAILOR
                    false, // TRAINERCLASS_KIMONO_GIRL
                    true, // TRAINERCLASS_RUIN_MANIAC
                    true, // TRAINERCLASS_PSYCHIC_M
                    false, // TRAINERCLASS_PSYCHIC_F
                    true, // TRAINERCLASS_PI
                    true, // TRAINERCLASS_GUITARIST
                    true, // TRAINERCLASS_ACE_TRAINER_M_GS
                    false, // TRAINERCLASS_ACE_TRAINER_F_GS
                    true, // TRAINERCLASS_TEAM_ROCKET
                    false, // TRAINERCLASS_SKIER
                    true, // TRAINERCLASS_ROUGHNECK
                    true, // TRAINERCLASS_CLOWN
                    true, // TRAINERCLASS_WORKER
                    true, // TRAINERCLASS_SCHOOL_KID_M
                    false, // TRAINERCLASS_SCHOOL_KID_F
                    false, // TRAINERCLASS_TEAM_ROCKET_F
                    true, // TRAINERCLASS_BURGLAR
                    true, // TRAINERCLASS_FIREBREATHER
                    true, // TRAINERCLASS_BIKER
                    true, // TRAINERCLASS_LEADER_FALKNER
                    false, // TRAINERCLASS_LEADER_BUGSY
                    true, // TRAINERCLASS_POKE_MANIAC
                    true, // TRAINERCLASS_BIRD_KEEPER_GS
                    false, // TRAINERCLASS_LEADER_WHITNEY
                    true, // TRAINERCLASS_RANCHER
                    true, // TRAINERCLASS_LEADER_MORTY
                    true, // TRAINERCLASS_LEADER_PRYCE
                    false, // TRAINERCLASS_LEADER_JASMINE
                    true, // TRAINERCLASS_LEADER_CHUCK
                    false, // TRAINERCLASS_LEADER_CLAIR
                    false, // TRAINERCLASS_TEACHER
                    true, // TRAINERCLASS_SUPER_NERD
                    true, // TRAINERCLASS_SAGE
                    false, // TRAINERCLASS_PARASOL_LADY
                    true, // TRAINERCLASS_WAITER
                    false, // TRAINERCLASS_MEDIUM
                    true, // TRAINERCLASS_CAMERAMAN
                    false, // TRAINERCLASS_REPORTER
                    false, // TRAINERCLASS_IDOL
                    true, // TRAINERCLASS_CHAMPION
                    false, // TRAINERCLASS_ELITE_FOUR_WILL
                    false, // TRAINERCLASS_ELITE_FOUR_KAREN
                    true, // TRAINERCLASS_ELITE_FOUR_KOGA
                    false, // TRAINERCLASS_PKMN_TRAINER_CHERYL
                    true, // TRAINERCLASS_PKMN_TRAINER_RILEY
                    false, // TRAINERCLASS_PKMN_TRAINER_BUCK
                    true, // TRAINERCLASS_PKMN_TRAINER_MIRA
                    false, // TRAINERCLASS_PKMN_TRAINER_MARLEY
                    true, // TRAINERCLASS_PKMN_TRAINER_FTR_LUCAS
                    false, // TRAINERCLASS_PKMN_TRAINER_FTR_DAWN
                    true, // TRAINERCLASS_TOWER_TYCOON
                    true, // TRAINERCLASS_LEADER_BROCK
                    false, // TRAINERCLASS_HALL_MATRON
                    true, // TRAINERCLASS_FACTORY_HEAD
                    false, // TRAINERCLASS_ARCADE_STAR
                    true, // TRAINERCLASS_CASTLE_VALET
                    false, // TRAINERCLASS_LEADER_MISTY
                    true, // TRAINERCLASS_LEADER_LT_SURGE
                    false, // TRAINERCLASS_LEADER_ERIKA
                    false, // TRAINERCLASS_LEADER_JANINE
                    false, // TRAINERCLASS_LEADER_SABRINA
                    true, // TRAINERCLASS_LEADER_BLAINE
                    true, // TRAINERCLASS_PKMN_TRAINER_RED
                    true, // TRAINERCLASS_LEADER_BLUE
                    true, // TRAINERCLASS_ELDER
                    true, // TRAINERCLASS_ELITE_FOUR_BRUNO
                    true, // TRAINERCLASS_SCIENTIST_GS
                    false, // TRAINERCLASS_EXECUTIVE_ARIANA
                    true, // TRAINERCLASS_BOARDER
                    true, // TRAINERCLASS_EXECUTIVE_ARCHER
                    true, // TRAINERCLASS_EXECUTIVE_PROTON
                    true, // TRAINERCLASS_EXECUTIVE_PETREL
                    true, // TRAINERCLASS_PASSERBY
                    true, // TRAINERCLASS_MYSTERY_MAN
                    true, // TRAINERCLASS_DOUBLE_TEAM
                    true, // TRAINERCLASS_YOUNG_COUPLE
                    true, // TRAINERCLASS_PKMN_TRAINER_LANCE
                    true, // TRAINERCLASS_ROCKET_BOSS
                    true, // TRAINERCLASS_PKMN_TRAINER_LUCAS_DP
                    false, // TRAINERCLASS_PKMN_TRAINER_DAWN_DP
                    true, // TRAINERCLASS_PKMN_TRAINER_LUCAS_PT
                };

            }

            

    }
}