using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static DSPRE.RomInfo;

namespace DSPRE.ROMFiles {
    public enum PokemonGender {
        Male = 0,
        Female = 254,
        Unknown = 255
    }
    public enum PokemonType {
        Normal = 0,
        Fighting,
        Flying,
        Poison,
        Ground,
        Rock,
        Bug,
        Ghost,
        Steel,
        Unknown,
        Fire,
        Water,
        Grass,
        Electric,
        Psychic,
        Ice,
        Dragon,
        Dark
    }
    public enum PokemonGrowthCurve {
        MediumFast = 0,
        Erratic,
        Fluctuating,
        MediumSlow,
        Fast,
        Slow
    }

    public enum PokemonEggGroup {
        Unassigned = 0,
        Monster,
        Water1,
        Bug,
        Flying,
        Field,
        Fairy,
        Grass,
        Humanoid,
        Water3,
        Mineral,
        Amorphous,
        Water2,
        Ditto,
        Dragon,
        NoBreed,
    };
    public enum PokemonDexColor {
        Red = 0,
        Blue, 
        Yellow,
        Green,
        Black,
        Brown,
        Purple,
        Gray,
        White,
        Pink,
        Unspecified
    }

    public class PokemonPersonalData : RomFile {
        public static readonly int tmsCount = 92;
        public static readonly int hmsCount = 8;

        public byte baseHP;   
        public byte baseAtk;
        public byte baseDef;
        public byte baseSpeed;
        public byte baseSpAtk;
        public byte baseSpDef;

        public PokemonType type1;
        public PokemonType type2;

        public byte catchRate;
        public byte givenExp;

        //Part of a u16 bitfield, 2 bits each.
        public byte evHP;   
        public byte evAtk;
        public byte evDef;
        public byte evSpeed;
        public byte evSpAtk;
        public byte evSpDef;
        //public byte dummy;

        public ushort item1;             // First item that the mon may hold when caught
        public ushort item2;             // Second item that the mon may hold when caught

        public byte genderVec;        
        public byte eggSteps;      
        public byte baseFriendship;
        public PokemonGrowthCurve growthCurve;

        public byte eggGroup1;    
        public byte eggGroup2;    
        public byte firstAbility; 
        public byte secondAbility;

        public byte escapeRate;
        public PokemonDexColor color;// : 7;           // Color (used in Pokedex)
        public bool flip;// : 1;         // Flip Flag

        public SortedSet<byte> machines;            

        public PokemonPersonalData(Stream stream) {
            using (BinaryReader reader = new BinaryReader(stream)) {
                // Deserialize the object from binary
                baseHP = reader.ReadByte();
                baseAtk = reader.ReadByte();
                baseDef = reader.ReadByte();
                baseSpeed = reader.ReadByte();
                baseSpAtk = reader.ReadByte();
                baseSpDef = reader.ReadByte();
                type1 = (PokemonType)reader.ReadByte();
                type2 = (PokemonType)reader.ReadByte();
                catchRate = reader.ReadByte();
                givenExp = reader.ReadByte();
                ushort evData = reader.ReadUInt16();
                evHP = (byte)(evData & 0b11);
                evAtk = (byte)((evData >> 2) & 0b11);
                evDef = (byte)((evData >> 4) & 0b11);
                evSpeed = (byte)((evData >> 6) & 0b11);
                evSpAtk = (byte)((evData >> 8) & 0b11);
                evSpDef = (byte)((evData >> 10) & 0b11);
                item1 = reader.ReadUInt16();
                item2 = reader.ReadUInt16();
                genderVec = reader.ReadByte();
                eggSteps = reader.ReadByte();
                baseFriendship = reader.ReadByte();
                growthCurve = (PokemonGrowthCurve)reader.ReadByte();
                eggGroup1 = reader.ReadByte();
                eggGroup2 = reader.ReadByte();
                firstAbility = reader.ReadByte();
                secondAbility = reader.ReadByte();
                escapeRate = reader.ReadByte();

                byte colorAndFlip = reader.ReadByte();
                color = (PokemonDexColor)(colorAndFlip & 0b01111111);
                flip = ((colorAndFlip >> 7) & 0b00000001) == 1;
                //reader.BaseStream.Position += 2; //alignment

                reader.BaseStream.Position += 2; //Alignment

                uint tm1 = reader.ReadUInt32();
                uint tm2 = reader.ReadUInt32();
                uint tm3 = reader.ReadUInt32();
                uint tm4 = reader.ReadUInt32();
                machines = BitFieldToSet(new uint[4] { tm1, tm2, tm3, tm4 });
            }
        }

        public PokemonPersonalData(int ID) : this(new FileStream(RomInfo.gameDirs[DirNames.personalPokeData].unpackedDir + "\\" + ID.ToString("D4"), FileMode.Open)) { }

        public override byte[] ToByteArray() {
            using (MemoryStream stream = new MemoryStream()) {
                using (BinaryWriter writer = new BinaryWriter(stream)) {
                    // Serialize the object to binary
                    writer.Write(baseHP);
                    writer.Write(baseAtk);
                    writer.Write(baseDef);
                    writer.Write(baseSpeed);
                    writer.Write(baseSpAtk);
                    writer.Write(baseSpDef);
                    writer.Write((byte)type1);
                    writer.Write((byte)type2);
                    writer.Write(catchRate);
                    writer.Write(givenExp);

                    ushort evData = (ushort)((evHP & 0b11) |
                                             ((evAtk & 0b11) << 2) |
                                             ((evDef & 0b11) << 4) |
                                             ((evSpeed & 0b11) << 6) |
                                             ((evSpAtk & 0b11) << 8) |
                                             ((evSpDef & 0b11) << 10));
                    writer.Write(evData);

                    writer.Write(item1);
                    writer.Write(item2);
                    writer.Write(genderVec);
                    writer.Write(eggSteps);
                    writer.Write(baseFriendship);
                    writer.Write((byte)growthCurve);
                    writer.Write(eggGroup1);
                    writer.Write(eggGroup2);
                    writer.Write(firstAbility);
                    writer.Write(secondAbility);
                    writer.Write(escapeRate);
                    byte colorAndFlipflag = (byte)(((byte)color & 0b01111111) |
                                                  (((flip ? 1 : 0) & 0b00000001) << 7));
                    writer.Write(colorAndFlipflag);

                    writer.BaseStream.Position += 2; //Alignment

                    uint[] bfs = SetToBitField(machines);
                    int l = Math.Min(bfs.Length, 4);
                    int i;
                    for (i = 0; i < l; i++) {
                        writer.Write(bfs[i]);
                    }
                    while (i < 4) {
                        writer.Write((uint)0);
                        i++;
                    }
                }
                return stream.ToArray();
            }
        }

        public SortedSet<byte> BitFieldToSet(uint[] bitfield) {
            var result = new SortedSet<byte>();

            for (uint i = 0; i < bitfield.Length; i++) {
                uint currentBitfield = bitfield[i];

                for (int j = 0; j < 32; j++) {
                    if ((currentBitfield & (1 << j)) != 0) {
                        result.Add((byte)(i * 32 + j));
                    }
                }
            }

            return result;
        }

        public uint[] SetToBitField(SortedSet<byte> set) {
            if (set == null) {
                return null;
            } 
            if (set.Count == 0) {
                return new uint[0];
            }

            int maxBit = set.Max();

            uint[] bitfield = new uint[(maxBit / 32) + 1];

            foreach (byte bit in set) {
                int index = bit / 32;
                int offset = bit % 32;
                bitfield[index] |= (uint)(1 << offset);
            }

            return bitfield;
        }
        public void SaveToFileDefaultDir(int IDtoReplace, bool showSuccessMessage = true) {
            SaveToFileDefaultDir(DirNames.personalPokeData, IDtoReplace, showSuccessMessage);
        }
        public void SaveToFileExplorePath(string suggestedFileName, bool showSuccessMessage = true) {
            SaveToFileExplorePath("Gen IV Personal Pokémon data", "bin", suggestedFileName, showSuccessMessage);
        }

    }
}
