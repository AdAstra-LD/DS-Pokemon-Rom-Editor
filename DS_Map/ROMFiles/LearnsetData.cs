using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DSPRE.ROMFiles;
using static DSPRE.RomInfo;

namespace DSPRE {
    internal class LearnsetData : RomFile {
        public static readonly int bitsMove = 9;
        public static readonly int bitsLevel = 7;
        public static readonly int VanillaLimit = 20;

        public readonly UniqueList<(byte level, ushort move)> list;

        public ushort[] GetLearnsetAtLevel(int atLevel)
        {
            List<ushort> moves = new List<ushort>();
            
            foreach ((byte level, ushort move) elem in list) {

                if (elem.level > atLevel) {
                    continue; // the moves should be sorted by level so we can probably break here but better safe than sorry
                }

                if (elem.move == 0) {
                    continue; // skip empty moves
                }

                if (!moves.Contains(elem.move)) {
                    
                    if (moves.Count >= 4)
                    {
                        moves.RemoveAt(0);
                    }                    
                    moves.Add(elem.move); // add the new move to the end of the list
                }
            }

            ushort[] learnset = moves.ToArray();

            // Ensure we have exactly 4 moves, filling with 0 if necessary
            if (learnset.Length < 4) {
                Array.Resize(ref learnset, 4);
            }

            return learnset;
        }

        public LearnsetData(Stream stream) {
            int numEntries = (int)(stream.Length / sizeof(ushort));
            list = new UniqueList<(byte level, ushort move)>(numEntries - 1);

            using (BinaryReader reader = new BinaryReader(stream)) {
                for (int i = 0; i < numEntries; i++) {
                    ushort entry = reader.ReadUInt16();
                    if (entry == 0xFFFF) {
                        return;
                    }

                    int maskMove = (1 << (bitsMove)) - 1;
                    int move = entry & maskMove;
                    entry >>= bitsMove;
                    
                    int maskLevel = (1 << (bitsLevel)) - 1;
                    int lv = entry & maskLevel;
                    
                    list.Add(((byte)lv, (ushort)move));
                }
            }
        }


        public LearnsetData(int ID) : this(new FileStream(RomInfo.gameDirs[DirNames.learnsets].unpackedDir + "\\" + ID.ToString("D4"), FileMode.Open)) { }

        public override byte[] ToByteArray() {
            using (MemoryStream memoryStream = new MemoryStream()) {
                using (BinaryWriter writer = new BinaryWriter(memoryStream)) {
                    foreach ((byte level, ushort move) elem in list) {
                        ushort move = (ushort)(elem.move & ((1 << bitsMove) - 1));
                        byte level = (byte)(elem.level & ((1 << bitsLevel) - 1));

                        ushort entry = (ushort)(move | (level << bitsMove));
                        writer.Write(entry);
                    }
                    // Add the termination entry
                    writer.Write((ushort)0xFFFF);
                    writer.Write((ushort)0x0000);
                }
                return memoryStream.ToArray();
            }
        }

        public void SaveToFileDefaultDir(int IDtoReplace, bool showSuccessMessage = true) {
            SaveToFileDefaultDir(DirNames.learnsets, IDtoReplace, showSuccessMessage);
        }
        public void SaveToFileExplorePath(string suggestedFileName, bool showSuccessMessage = true) {
            SaveToFileExplorePath("Gen IV Pokémon Learnset data", "bin", suggestedFileName, showSuccessMessage);
        }
    }
}