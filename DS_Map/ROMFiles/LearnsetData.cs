using System.Collections.Generic;
using System.IO;
using DSPRE.ROMFiles;
using static DSPRE.RomInfo;

namespace DSPRE {
    internal class LearnsetData : RomFile {
        public static readonly int bitsMove = 9;
        public static readonly int bitsLevel = 7;

        public readonly UniqueList<(byte level, ushort move)> list;

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