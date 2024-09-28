using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace DSPRE.ROMFiles {
    //https://hirotdk.neocities.org/FileSpecs.html#Headbutt
    public class HeadbuttEncounterFile {
        public ushort ID;

        //get encounter tables, 12 normal pokemon definitions, 6 special pokemon definitions, 4 bytes per definition
        const int normalEncountersCount = 12;
        const int specialEncountersCount = 6;

        public byte normalTreeGroupsCount;
        public byte specialTreeGroupsCount;
        public List<HeadbuttEncounter> normalEncounters;
        public List<HeadbuttEncounter> specialEncounters;
        public BindingList<HeadbuttTreeGroup> normalTreeGroups;
        public BindingList<HeadbuttTreeGroup> specialTreeGroups;

        public HeadbuttEncounterFile(ushort id) {
            this.ID = id;
            string path = Filesystem.GetHeadbuttPath(id);
            parse_file(path);
        }

        public HeadbuttEncounterFile(string path) {
            parse_file(path);
        }

        public void parse_file(string path) {
            FileStream fs = new FileStream(path, FileMode.Open);
            using (BinaryReader br = new BinaryReader(fs)) {
                //get the number of tree group definitions
                normalTreeGroupsCount = br.ReadByte();
                br.ReadByte(); //padding
                specialTreeGroupsCount = br.ReadByte();
                br.ReadByte(); //padding

                normalEncounters = new List<HeadbuttEncounter>();
                specialEncounters = new List<HeadbuttEncounter>();

                normalTreeGroups = new BindingList<HeadbuttTreeGroup>();
                specialTreeGroups = new BindingList<HeadbuttTreeGroup>();

                //if there are no trees defined in either section, there are no encounters or trees defined
                bool hasTrees = normalTreeGroupsCount > 0 || specialTreeGroupsCount > 0;
                if (!hasTrees) {
                    for (int i = 0; i < normalEncountersCount; i++) {
                        normalEncounters.Add(new HeadbuttEncounter());
                    }

                    for (int i = 0; i < specialEncountersCount; i++) {
                        specialEncounters.Add(new HeadbuttEncounter());
                    }

                    return;
                }

                for (int i = 0; i < normalEncountersCount; i++) {
                    normalEncounters.Add(new HeadbuttEncounter(br));
                }

                for (int i = 0; i < specialEncountersCount; i++) {
                    specialEncounters.Add(new HeadbuttEncounter(br));
                }

                //tree groups have 6 sets of xy global coordinates x treeGroupsCount
                //coordinates need to be converted to matrix and local coordinates to be useful
                for (int i = 0; i < normalTreeGroupsCount; i++) {
                    normalTreeGroups.Add(new HeadbuttTreeGroup(br));
                }

                for (int i = 0; i < specialTreeGroupsCount; i++) {
                    specialTreeGroups.Add(new HeadbuttTreeGroup(br));
                }
            }
        }

        public byte[] ToByteArray() {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData)) {
                writer.Write((ushort)normalTreeGroups.Count);
                writer.Write((ushort)specialTreeGroups.Count);

                foreach (HeadbuttEncounter encounter in normalEncounters) {
                    writer.Write((ushort)encounter.pokemonID);
                    writer.Write((byte)encounter.minLevel);
                    writer.Write((byte)encounter.maxLevel);
                }

                foreach (HeadbuttEncounter encounter in specialEncounters) {
                    writer.Write((UInt16)encounter.pokemonID);
                    writer.Write((byte)encounter.minLevel);
                    writer.Write((byte)encounter.maxLevel);
                }

                foreach (var treeGroup in normalTreeGroups) {
                    foreach (var tree in treeGroup.trees) {
                        writer.Write((UInt16)tree.globalX);
                        writer.Write((UInt16)tree.globalY);
                    }
                }

                foreach (var treeGroup in specialTreeGroups) {
                    foreach (var tree in treeGroup.trees) {
                        writer.Write((UInt16)tree.globalX);
                        writer.Write((UInt16)tree.globalY);
                    }
                }
            }

            return newData.ToArray();
        }

        public bool SaveToFile() {
            string path = Filesystem.GetHeadbuttPath(ID);
            return SaveToFile(path);
        }

        public bool SaveToFile(int id) {
            string path = Filesystem.GetHeadbuttPath(id);
            return SaveToFile(path);
        }


        public bool SaveToFile(string path, bool showSuccessMessage = true) {
            byte[] romFileToByteArray = ToByteArray();
            File.WriteAllBytes(path, romFileToByteArray);
            return true;
        }
    }
}
