using System.Collections.Generic;
using System.IO;

namespace DSPRE.ROMFiles {
  public class SafariZoneEncounterFile {
    public static Dictionary<int, string> Names = new Dictionary<int, string>() {
      {0, "Plains"},
      {1, "Meadow"},
      {2, "Savannah"},
      {3, "Peak"},
      {4, "Rocky Beach"},
      {5, "Wetland"},
      {6, "Forest"},
      {7, "Swamp"},
      {8, "Marshland"},
      {9, "Wasteland"},
      {10, "Mountain"},
      {11, "Desert"},
    };

    public int ID;
    
    public SafariZoneEncounterGroup grassEncounterGroup = new SafariZoneEncounterGroup();
    public SafariZoneEncounterGroup surfEncounterGroup = new SafariZoneEncounterGroup();
    public SafariZoneEncounterGroup oldRodEncounterGroup = new SafariZoneEncounterGroup();
    public SafariZoneEncounterGroup goodRodEncounterGroup = new SafariZoneEncounterGroup();
    public SafariZoneEncounterGroup superRodEncounterGroup = new SafariZoneEncounterGroup();

    public SafariZoneEncounterFile(int id) {
      this.ID = id;
      string path = Filesystem.GetSafariZonePath(id);
      parse_file(path);
    }

    public SafariZoneEncounterFile(string path) {
      parse_file(path);
    }

    public void parse_file(string path) {
      FileStream fs = new FileStream(path, FileMode.Open);
      using (BinaryReader br = new BinaryReader(fs)) {
        if (br.BaseStream.Length < 5){ return; }
        //#1 Section - Object Arrangement Allocation
        grassEncounterGroup.readObjectSlots(br);
        surfEncounterGroup.readObjectSlots(br);
        oldRodEncounterGroup.readObjectSlots(br);
        goodRodEncounterGroup.readObjectSlots(br);
        superRodEncounterGroup.readObjectSlots(br);

        br.ReadByte();
        br.ReadByte();
        br.ReadByte();

        grassEncounterGroup.readGroup(br);
        surfEncounterGroup.readGroup(br);
        oldRodEncounterGroup.readGroup(br);
        goodRodEncounterGroup.readGroup(br);
        superRodEncounterGroup.readGroup(br);
      }
    }

    public byte[] ToByteArray() {
      MemoryStream newData = new MemoryStream();
      using (BinaryWriter bw = new BinaryWriter(newData)) {
        grassEncounterGroup.writeObjectSlots(bw);
        surfEncounterGroup.writeObjectSlots(bw);
        oldRodEncounterGroup.writeObjectSlots(bw);
        goodRodEncounterGroup.writeObjectSlots(bw);
        superRodEncounterGroup.writeObjectSlots(bw);

        bw.Write((byte)0);
        bw.Write((byte)0);
        bw.Write((byte)0);

        grassEncounterGroup.writeGroup(bw);
        surfEncounterGroup.writeGroup(bw);
        oldRodEncounterGroup.writeGroup(bw);
        goodRodEncounterGroup.writeGroup(bw);
        superRodEncounterGroup.writeGroup(bw);
      }
      return newData.ToArray();
    }

    public bool SaveToFile() {
      string path = Filesystem.GetSafariZonePath(ID);
      return SaveToFile(path);
    }

    public bool SaveToFile(int id) {
      string path = Filesystem.GetSafariZonePath(id);
      return SaveToFile(path);
    }


    public bool SaveToFile(string path, bool showSuccessMessage = true) {
      byte[] romFileToByteArray = ToByteArray();
      File.WriteAllBytes(path, romFileToByteArray);
      return true;
    }
  }
}
