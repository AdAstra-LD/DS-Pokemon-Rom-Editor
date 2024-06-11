using System.IO;

namespace DSPRE.ROMFiles {
  public class HeadbuttEncounter {
    public ushort pokemonID;
    public byte minLevel;
    public byte maxLevel;

    public HeadbuttEncounter() {
      maxLevel = 0;
      minLevel = 0;
      pokemonID = 0;
    }

    public HeadbuttEncounter(BinaryReader br) {
      this.pokemonID = br.ReadUInt16();
      this.minLevel = br.ReadByte();
      this.maxLevel = br.ReadByte();
    }

    public override string ToString() {
      string[] pokemonNames = RomInfo.GetPokemonNames();
      string pokemon = pokemonNames[pokemonID];
      return $"({pokemonID,4}) {pokemon,10}: Lv {minLevel,3} to Lv.{maxLevel,3}";
    }
  }
}
