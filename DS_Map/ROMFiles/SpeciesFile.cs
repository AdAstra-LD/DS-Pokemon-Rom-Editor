using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;


namespace DSPRE.ROMFiles
{
    public class SpeciesFile
    {
        public byte Ability1;
        public byte Ability2;
        public byte GenderRatioMaleToFemale;

        public const int GENDER_RATIO_BYTE_OFFSET = 16;
        public const int ABILITY1_BYTE_OFFSET = 22;

        public const int GENDER_RATIO_MALE = 0;
        public const int GENDER_RATIO_FEMALE = 254;
        public const int GENDER_RATIO_GENDERLESS = 255;

        public const int PICHU_ID_NUM = 172;
        public const int UNOWN_ID_NUM = 201;
        public const int CASTFORM_ID_NUM = 351;
        public const int DEOXYS_ID_NUM = 386;
        public const int BURMY_ID_NUM = 412;
        public const int WORMADAM_ID_NUM = 413;
        public const int SHELLOS_ID_NUM = 422;
        public const int GASTRODON_ID_NUM = 423;
        public const int ROTOM_ID_NUM = 479;
        public const int GIRATINA_ID_NUM = 487;
        public const int SHAYMIN_ID_NUM = 492;

        public SpeciesFile(FileStream pokeData)
        {
            var pokeDataReader = new BinaryReader(pokeData);
            pokeDataReader.BaseStream.Position = GENDER_RATIO_BYTE_OFFSET;
            GenderRatioMaleToFemale = pokeDataReader.ReadByte();

            pokeDataReader.BaseStream.Position = ABILITY1_BYTE_OFFSET;
            Ability1 = pokeDataReader.ReadByte();
            Ability2 = pokeDataReader.ReadByte();

            pokeDataReader.Close();
        }

        public static bool hasMoreThanOneGender(int pokemonID, SpeciesFile[] pokemonSpecies)
        {
            switch (pokemonSpecies[pokemonID].GenderRatioMaleToFemale)
            {
                case GENDER_RATIO_MALE:
                case GENDER_RATIO_FEMALE:
                case GENDER_RATIO_GENDERLESS:
                    return false;
                default:
                    return true;
            }
        }
    }
}

