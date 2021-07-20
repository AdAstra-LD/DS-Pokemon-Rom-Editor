using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Resources;
using System.Reflection;
using System.Windows.Forms;
using DSPRE.Resources;
using static DSPRE.RomInfo;

namespace DSPRE.ROMFiles {
    public class PartyPokemon {
        #region Fields
        public ushort pokemon = 0;
        public ushort level = 0;
        public ushort heldItem = 0;
        public ushort unknown1_DATASTART = 0;
        public ushort unknown2_DATAEND = 0;
        public ushort[] moves = new ushort[4] { 0, 0, 0, 0 };
        #endregion

        #region Constructor
        public PartyPokemon() {
        }
        public PartyPokemon(ushort Unk1, ushort Level, ushort Pokemon, ushort Unk2, ushort Item = 0, ushort[] Moves = null) {
            pokemon = Pokemon;
            level = Level;
            heldItem = Item;
            unknown1_DATASTART = Unk1;
            unknown2_DATAEND = Unk2;

            if (Moves != null) {
                moves = Moves;
            }
        }
        #endregion
    }

    public class Trainer {
        public static int POKE_IN_PARTY = 6;
        public static int AI_COUNT = 11;

        #region Fields
        public ushort trainerID;
        public byte trainerClass;
        public byte trDataUnknown;
        public byte partyCount;
        public bool doubleBattle;
        public bool hasMoves;
        public bool hasItems;

        public ushort[] trainerItems = new ushort[4];
        public bool[] AI = new bool[11];
        public PartyPokemon[] party = new PartyPokemon[6];
        #endregion

        #region Constructor
        public Trainer(ushort ID) {
            trainerID = ID;
            trainerClass = 0;
            partyCount = 0;
            doubleBattle = false;
            hasMoves = false;
            hasItems = false;
            trainerItems = new ushort[4] { 0, 0, 0, 0 };
            AI = new bool[11] { true, true, true, false, false, false, false, false, false, false, false };
            party = new PartyPokemon[0];
            trDataUnknown = 0;
        }
        public Trainer(ushort ID, Stream trainerData, Stream partyData) {
            trainerID = ID;
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
            using (BinaryReader reader = new BinaryReader(partyData)) {
                int dividend = 8;
                int nMoves = 4;

                for (int i = 0; i < POKE_IN_PARTY; i++) {
                    party[i] = new PartyPokemon();
                }

                if (hasMoves) {
                    dividend += nMoves * sizeof(ushort);
                }
                if (hasItems) {
                    dividend += sizeof(ushort);
                }

                for (int i = 0; i < Math.Min((int)(partyData.Length / dividend), partyCount); i++) {
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
                            moves[m] = val == 0xFFFF ? (ushort)0 : val;
                        }
                    }

                    party[i] = new PartyPokemon(unknown1, level, pokemon, reader.ReadUInt16(), item, moves);
                }
            }
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
                if (!hasMoves && !hasItems) {
                    for (int i = 0; i < partyCount; i++) {
                        writer.Write(party[i].unknown1_DATASTART);
                        writer.Write(party[i].level);
                        writer.Write(party[i].pokemon);
                        writer.Write(party[i].unknown2_DATAEND);
                    }
                } else if (!hasMoves && hasItems) {
                    for (int i = 0; i < partyCount; i++) {
                        writer.Write(party[i].unknown1_DATASTART);
                        writer.Write(party[i].level);
                        writer.Write(party[i].pokemon);
                        writer.Write(party[i].heldItem);
                        writer.Write(party[i].unknown2_DATAEND);
                    }
                } else if (hasMoves && !hasItems) {
                    for (int i = 0; i < partyCount; i++) {
                        writer.Write(party[i].unknown1_DATASTART);
                        writer.Write(party[i].level);
                        writer.Write(party[i].pokemon);
                        foreach (ushort move in party[i].moves) {
                            writer.Write(move);
                        }
                        writer.Write(party[i].unknown2_DATAEND);
                    }
                } else if (hasMoves && hasItems) {
                    for (int i = 0; i < partyCount; i++) {
                        writer.Write(party[i].unknown1_DATASTART);
                        writer.Write(party[i].level);
                        writer.Write(party[i].pokemon);
                        writer.Write(party[i].heldItem);
                        foreach (ushort move in party[i].moves) {
                            writer.Write(move);
                        }
                        writer.Write(party[i].unknown2_DATAEND);
                    }
                }
            }
            return newData.ToArray();
        }

        #endregion

    }

}