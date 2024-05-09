using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSPRE.ROMFiles {
    public enum MoveCategory {
        Physical,
        Special,
        Status
    }

    public enum ContestCategories {
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Eleven = 11,
        Twelve = 12
    }

    [Flags]
    public enum MoveTargets {
        OneOpponent = 1,
        Automatic = 2,
        Random = 4,
        BothOpponents = 16,
        BothOpponentsAndAlly = 8,
        User = 32,
        UsersSideOfField = 64,
        EntireField = 128,
        OpponentsSideOfField = 256,
        AutomaticWithAlly = 512,
        UserOrAlly = 1024,
        OneOpponentFailsIfTargetFaints = 2048
    }


    public struct MoveData {
        public ushort Effect; // u16
        public byte Category; // u8
        public byte Power; // u8

        public byte Type; // u8
        public byte Accuracy; // u8
        public byte PP; // u8
        public byte EffectChance; // u8

        public ushort Target; // u16
        public sbyte Priority; // s8
        public bool[] Flags; // u8

        public byte ContestEffect; // u8
        public byte ContestType; // u8
    }

    public class MoveFile : RomFile{

        public MoveData moveData;

        public MoveFile(Stream stream) {
            moveData = new MoveData();
            using (BinaryReader reader = new BinaryReader(stream)) {
                moveData.Effect = reader.ReadUInt16();
                moveData.Category = reader.ReadByte();
                moveData.Power = reader.ReadByte();
                moveData.Type = reader.ReadByte();
                moveData.Accuracy = reader.ReadByte();
                moveData.PP = reader.ReadByte();
                moveData.EffectChance = reader.ReadByte();
                moveData.Target = reader.ReadUInt16();
                moveData.Priority = reader.ReadSByte();

                // Reading flags
                byte flagsByte = reader.ReadByte();
                moveData.Flags = new bool[8];
                for (int i = 0; i < 8; i++) {
                    moveData.Flags[i] = (flagsByte & (1 << i)) != 0;
                }

                moveData.ContestEffect = reader.ReadByte();
                moveData.ContestType = reader.ReadByte();
            }
        }

        public override byte[] ToByteArray() {
            using (MemoryStream stream = new MemoryStream()) {
                using (BinaryWriter writer = new BinaryWriter(stream)) {
                    writer.Write(moveData.Effect);
                    writer.Write(moveData.Category);
                    writer.Write(moveData.Power);
                    writer.Write(moveData.Type);
                    writer.Write(moveData.Accuracy);
                    writer.Write(moveData.PP);
                    writer.Write(moveData.EffectChance);
                    writer.Write(moveData.Target);
                    writer.Write(moveData.Priority);

                    // Writing flags
                    byte flagsByte = 0;
                    for (int i = 0; i < 8; i++) {
                        if (moveData.Flags[i])
                            flagsByte |= (byte)(1 << i);
                    }
                    writer.Write(flagsByte);

                    writer.Write(moveData.ContestEffect);
                    writer.Write(moveData.ContestType);
                }

                return stream.ToArray();
            }
        }
    }
}
