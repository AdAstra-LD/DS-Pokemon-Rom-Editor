using System;
using System.IO;
using System.Threading;

namespace DSPRE.ROMFiles {
    public class PartyPokemon : RomFile {
        #region Fields
        public ushort pokemon = 0;
        public ushort level = 0;
        public ushort unknown1_DATASTART = 0;
        public ushort unknown2_DATAEND = 0;

        public ushort? heldItem = null;
        public ushort[] moves = null;
        #endregion

        #region Constructor
        public PartyPokemon() {
        }
        public PartyPokemon(ushort Unk1, ushort Level, ushort Pokemon, ushort Unk2, ushort? heldItem = null, ushort[] moves = null) {
            pokemon = Pokemon;
            level = Level;
            unknown1_DATASTART = Unk1;
            unknown2_DATAEND = Unk2;
            this.heldItem = heldItem;
            this.moves = moves;
        }
        public override byte[] ToByteArray() {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData)) {
                writer.Write(unknown1_DATASTART);
                writer.Write(level);
                writer.Write(pokemon);

                if (heldItem != null) {
                    writer.Write((ushort)heldItem);
                }

                if (moves != null) {
                    foreach (ushort move in moves) {
                        writer.Write(move);
                    }
                }
                writer.Write(unknown2_DATAEND);
            }
            return newData.ToArray();
        }
        #endregion
    }

    public class Trainer {
        public const int POKE_IN_PARTY = 6;
        public const int AI_COUNT = 11;
        public const int TRAINER_ITEMS = 4;

        #region Fields
        public ushort trainerID;
        public byte trDataUnknown;

        public byte trainerClass = 0;
        public byte partyCount = 0;

        public bool doubleBattle = false;
        public bool hasMoves = false;
        public bool hasItems = false;

        public ushort[] trainerItems = new ushort[TRAINER_ITEMS];
        public bool[] AI = new bool[AI_COUNT];
        public PartyPokemon[] party = new PartyPokemon[POKE_IN_PARTY];
        #endregion

        #region Constructor
        public Trainer(ushort ID) {
            trainerID = ID;        
            trainerItems = new ushort[TRAINER_ITEMS];
            AI = new bool[AI_COUNT] { true, true, true, false, false, false, false, false, false, false, false };
            party = new PartyPokemon[1] { new PartyPokemon() };
            trDataUnknown = 0;
        }
        public Trainer(ushort ID, Stream trainerData, Stream partyData) {
            trainerID = ID;
            Thread t1 = new Thread(() => {
                using (BinaryReader reader = new BinaryReader(trainerData)) {
                    byte flags = reader.ReadByte();
                    hasMoves = (flags & 1) != 0;
                    hasItems = (flags & 2) != 0;

                    trainerClass = reader.ReadByte();
                    trDataUnknown = reader.ReadByte();
                    partyCount = reader.ReadByte();

                    for (int i = 0; i < trainerItems.Length; i++) {
                        trainerItems[i] = reader.ReadUInt16();
                    }

                    uint AIflags = reader.ReadUInt32();
                    for (int i = 0; i < AI_COUNT; i++) {
                        AI[i] = (AIflags & (1 << i)) != 0;
                    }
                    doubleBattle = reader.ReadUInt32() == 2;
                }
            });
            Thread t2 = new Thread(() => {
                using (BinaryReader reader = new BinaryReader(partyData)) {
                    int dividend = 8;
                    int nMoves = 4;

                    if (hasMoves) {
                        dividend += nMoves * sizeof(ushort);
                    }
                    if (hasItems) {
                        dividend += sizeof(ushort);
                    }

                    int endval = Math.Min((int)(partyData.Length / dividend), partyCount);
                    for (int i = 0; i < endval; i++) {
                        ushort unknown1 = reader.ReadUInt16();
                        ushort level = reader.ReadUInt16();
                        ushort pokemon = reader.ReadUInt16();

                        ushort item = 0;
                        ushort[] moves = new ushort[4];

                        if (hasItems) {
                            item = reader.ReadUInt16();
                        }
                        if (hasMoves) {
                            for (int m = 0; m < moves.Length; m++) {
                                ushort val = reader.ReadUInt16();
                                moves[m] = (ushort)(val == 0xFFFF ? 0 : val);
                            }
                        }

                        party[i] = new PartyPokemon(unknown1, level, pokemon, reader.ReadUInt16(), item, moves);
                    }
                    for (int i = endval; i < POKE_IN_PARTY; i++) {
                        party[i] = new PartyPokemon();
                    }
                }
            });

            t1.Start();
            t2.Start();

            t1.Join();
            t2.Join();
        }
        #endregion

        #region Methods
        public byte[] TrainerDataToByteArray() {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData)) {
                byte flags = 0;
                flags |= (byte)(hasMoves ? 1 : 0);
                flags |= (byte)(hasItems ? 2 : 0);

                writer.Write(flags);
                writer.Write(trainerClass);
                writer.Write(trDataUnknown);
                writer.Write(partyCount);

                foreach (ushort trItem in trainerItems) {
                    writer.Write(trItem);
                }

                UInt32 AIflags = 0;
                for (int i = 0; i < AI.Length; i++) {
                    if (AI[i]) {
                        AIflags |= (uint)1 << i;
                    }
                }

                writer.Write(AIflags);
                writer.Write((uint)(doubleBattle ? 2 : 0));
            }
            return newData.ToArray();
        }
        public byte[] PartyDataToByteArray() {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData)) {
                foreach (PartyPokemon poke in party) {
                    writer.Write(poke.ToByteArray());
                }
            }
            return newData.ToArray();
        }

        #endregion

    }

}