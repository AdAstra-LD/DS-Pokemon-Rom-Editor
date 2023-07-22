using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;


namespace DSPRE.ROMFiles
{
    //currently only used to get the abilities of pokemon for trainer editor
    public class SpeciesFile
    {
        public byte Ability1;
        public byte Ability2;

        public const int ABILITY1_BYTE_INDEX = 22;

        public SpeciesFile(FileStream pokeData)
        {
            var pokeDataReader = new BinaryReader(pokeData);
            pokeDataReader.BaseStream.Position = ABILITY1_BYTE_INDEX;

            Ability1 = pokeDataReader.ReadByte();
            Ability2 = pokeDataReader.ReadByte();

            pokeDataReader.Close();
        }
    }
}

