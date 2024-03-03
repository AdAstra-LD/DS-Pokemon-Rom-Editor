using System.Collections.Generic;
using System.IO;

namespace DSPRE.ROMFiles {
  public class SafariZoneObjectRequirement {
    public static Dictionary<int, string> ObjectTypes = new Dictionary<int, string>() {
      { 0, "No Requirement" },
      { 1, "Plains" },
      { 2, "Forest" },
      { 3, "Peak" },
      { 4, "Waterside" },
    };

    public byte typeID;
    public byte quantity;

    public SafariZoneObjectRequirement(byte typeID = 0, byte quantity = 0) {
      this.typeID = typeID;
      this.quantity = quantity;
    }

    public SafariZoneObjectRequirement(BinaryReader br) {
      readRequirement(br);
    }

    public void readRequirement(BinaryReader br) {
      typeID = br.ReadByte();
      quantity = br.ReadByte();
    }

    public void writeRequirement(BinaryWriter bw) {
      bw.Write((byte)typeID);
      bw.Write((byte)quantity);
    }

    public override string ToString() {
      return $"{typeID} {ObjectTypes[typeID]}: {quantity}";
    }
  }
}
