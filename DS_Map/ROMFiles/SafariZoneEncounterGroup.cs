using System.ComponentModel;
using System.IO;

namespace DSPRE.ROMFiles
{
  public class SafariZoneEncounterGroup
  {
    private const int EncounterSlots = 10;

    public byte ObjectSlots;
    public BindingList<SafariZoneEncounter> MorningEncounters = new BindingList<SafariZoneEncounter>();
    public BindingList<SafariZoneEncounter> DayEncounters = new BindingList<SafariZoneEncounter>();
    public BindingList<SafariZoneEncounter> NightEncounters = new BindingList<SafariZoneEncounter>();
    public BindingList<SafariZoneEncounter> MorningEncountersObject = new BindingList<SafariZoneEncounter>();
    public BindingList<SafariZoneEncounter> DayEncountersObject = new BindingList<SafariZoneEncounter>();
    public BindingList<SafariZoneEncounter> NightEncountersObject = new BindingList<SafariZoneEncounter>();
    public BindingList<SafariZoneObjectRequirement> ObjectRequirements = new BindingList<SafariZoneObjectRequirement>();
    public BindingList<SafariZoneObjectRequirement> OptionalObjectRequirements = new BindingList<SafariZoneObjectRequirement>();

    public void readObjectSlots(BinaryReader br) {
      ObjectSlots = br.ReadByte();
    }

    public void writeObjectSlots(BinaryWriter bw) {
      bw.Write((byte)ObjectSlots);
    }

    public void readGroup(BinaryReader br) {
      //#2 Section - Tall Grass Encounters
      for (int i = 0; i < EncounterSlots; i++) {
        MorningEncounters.Add(new SafariZoneEncounter(br));
        br.ReadByte();
      }

      for (int i = 0; i < EncounterSlots; i++) {
        DayEncounters.Add(new SafariZoneEncounter(br));
        br.ReadByte();
      }

      for (int i = 0; i < EncounterSlots; i++) {
        NightEncounters.Add(new SafariZoneEncounter(br));
        br.ReadByte();
      }

      //#3 Section - Tall Grass Encounters (Object Arrangement)
      for (int i = 0; i < ObjectSlots; i++) {
        MorningEncountersObject.Add(new SafariZoneEncounter(br));
        br.ReadByte();
      }

      for (int i = 0; i < ObjectSlots; i++) {
        DayEncountersObject.Add(new SafariZoneEncounter(br));
        br.ReadByte();
      }

      for (int i = 0; i < ObjectSlots; i++) {
        NightEncountersObject.Add(new SafariZoneEncounter(br));
        br.ReadByte();
      }

      //#4 Section - Object Arrangement Requirements (Tall Grass)
      for (int i = 0; i < ObjectSlots; i++) {
        ObjectRequirements.Add(new SafariZoneObjectRequirement(br));
        OptionalObjectRequirements.Add(new SafariZoneObjectRequirement(br));
      }
    }

    public void writeGroup(BinaryWriter bw) {
      //#2 Section - Tall Grass Encounters
      for (int i = 0; i < MorningEncounters.Count; i++) {
        MorningEncounters[i].writeEncounter(bw);
        bw.Write((byte)0);
      }

      for (int i = 0; i < EncounterSlots; i++) {
        DayEncounters[i].writeEncounter(bw);
        bw.Write((byte)0);
      }

      for (int i = 0; i < EncounterSlots; i++) {
        NightEncounters[i].writeEncounter(bw);
        bw.Write((byte)0);
      }

      //#3 Section - Tall Grass Encounters (Object Arrangement)
      for (int i = 0; i < ObjectSlots; i++) {
        MorningEncountersObject[i].writeEncounter(bw);
        bw.Write((byte)0);
      }

      for (int i = 0; i < ObjectSlots; i++) {
        DayEncountersObject[i].writeEncounter(bw);
        bw.Write((byte)0);
      }

      for (int i = 0; i < ObjectSlots; i++) {
        NightEncountersObject[i].writeEncounter(bw);
        bw.Write((byte)0);
      }

      //#4 Section - Object Arrangement Requirements (Tall Grass)
      for (int i = 0; i < ObjectSlots; i++) {
        ObjectRequirements[i].writeRequirement(bw);
        OptionalObjectRequirements[i].writeRequirement(bw);
      }
    }
  }
}
