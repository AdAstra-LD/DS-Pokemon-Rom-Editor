using System;
using System.IO;

namespace DSPRE.ROMFiles
{
  public class SafariZoneEncounter
  {
    public ushort pokemonID;
    public byte level;
    public SafariZoneEncounter() {
      level = 1;
      pokemonID = 0;
    }

    public SafariZoneEncounter(BinaryReader br) {
      readEncounter(br);
    }

    public void readEncounter(BinaryReader br) {
      this.pokemonID = br.ReadUInt16();
      this.level = br.ReadByte();
    }

    public void writeEncounter(BinaryWriter bw) {
      bw.Write((UInt16)pokemonID);
      bw.Write((byte)level);
    }

    public override string ToString() {
      string[] pokemonNames = RomInfo.GetPokemonNames();
      string pokemon = pokemonNames[pokemonID];
      return $"{pokemonID,4} {pokemon,10}: {level,3}";
    }
  }
}
