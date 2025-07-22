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
            HashSet<ushort> seen = new HashSet<ushort>();

            foreach ((byte level, ushort move) elem in list)
            {
                // If the level is lower than the mon level and the move hasn't been seen yet, add it to the list
                if (elem.level <= atLevel && seen.Add(elem.move))
                {
                    moves.Add(elem.move);
                }
            }

            ushort[] learnset = new ushort[4];
            int start = Math.Max(0, moves.Count - 4);

            // Fill the learnset with the last 4 moves, or 0 if there are not enough moves
            for (int i = 0; i < 4; i++)
            {
                int idx = start + i;
                learnset[i] = (idx < moves.Count) ? moves[idx] : (ushort)0;
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