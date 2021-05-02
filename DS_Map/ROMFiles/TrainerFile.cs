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

    public class PartyPokemon
    {
        #region Fields
        public UInt16 pokemon = 0;
        public UInt16 level = 0;
        public UInt16 heldItem = 0;
        public UInt16 unknown1 = 0;
        public UInt16 unknown2 = 0;
        public UInt16[] moves = new UInt16[4] { 0, 0, 0, 0 };
        #endregion

        #region Constructor
        public PartyPokemon() {
        }
        public PartyPokemon(UInt16 Unk1, UInt16 Level, UInt16 Pokemon, UInt16 Unk2) {
            pokemon = Pokemon;
            level = Level;
            unknown1 = Unk1;
            unknown2 = Unk2;
        }
        public PartyPokemon(UInt16 Unk1, UInt16 Level, UInt16 Pokemon, UInt16 Item, UInt16 Unk2) {
            pokemon = Pokemon;
            level = Level;
            heldItem = Item;
            unknown1 = Unk1;
            unknown2 = Unk2;
        }
        public PartyPokemon(UInt16 Unk1, UInt16 Level, UInt16 Pokemon, UInt16 Move1, UInt16 Move2, UInt16 Move3, UInt16 Move4, UInt16 Unk2) {
            pokemon = Pokemon;
            level = Level;
            unknown1 = Unk1;
            unknown2 = Unk2;
            moves = new UInt16[4] { Move1 == 0xFFFF ? (UInt16)(0) : Move1, Move2 == 0xFFFF ? (UInt16)(0) : Move2, Move3 == 0xFFFF ? (UInt16)(0) : Move3, Move4 == 0xFFFF ? (UInt16)(0) : Move4 };
        }
        public PartyPokemon(UInt16 Unk1, UInt16 Level, UInt16 Pokemon, UInt16 Item, UInt16 Move1, UInt16 Move2, UInt16 Move3, UInt16 Move4, UInt16 Unk2) {
            pokemon = Pokemon;
            level = Level;
            heldItem = Item;
            unknown1 = Unk1;
            unknown2 = Unk2;
            moves = new UInt16[4] { Move1 == 0xFFFF ? (UInt16)(0) : Move1, Move2 == 0xFFFF ? (UInt16)(0) : Move2, Move3 == 0xFFFF ? (UInt16)(0) : Move3, Move4 == 0xFFFF ? (UInt16)(0) : Move4 };
        }
        #endregion
    }

    public class Trainer
    {
        #region Fields
        public UInt16 trainerID;
        public byte trainerClass;
        public byte partyCount;
        public bool doubleBattle;
        public bool hasMoves;
        public bool hasItems;
        public UInt16[] trainerItems = new UInt16[4];
        public bool[] IA = new bool[11];
        public PartyPokemon[] trainerParty = new PartyPokemon[6];
        public byte trdataunknown;
        #endregion

        #region Constructor
        public Trainer(UInt16 ID) {
            trainerID = ID;
            trainerClass = 0;
            partyCount = 0;
            doubleBattle = false;
            hasMoves = false;
            hasItems = false;
            trainerItems = new UInt16[4] { 0, 0, 0, 0 };
            IA = new bool[11] { true, true, true, false, false, false, false, false, false, false, false };
            trainerParty = new PartyPokemon[0];
            trdataunknown = 0;
        }
        public Trainer(UInt16 ID, Stream trainerData, Stream partyData) {
            using (BinaryReader reader = new BinaryReader(trainerData)) {
                trainerID = ID;
                byte flags = reader.ReadByte();
                hasMoves = (flags & 1) != 0;
                hasItems = (flags & 2) != 0;
                trainerClass = reader.ReadByte();
                trdataunknown = reader.ReadByte();
                partyCount = reader.ReadByte();
                trainerItems[0] = reader.ReadUInt16();
                trainerItems[1] = reader.ReadUInt16();
                trainerItems[2] = reader.ReadUInt16();
                trainerItems[3] = reader.ReadUInt16();
                UInt32 IAflags = reader.ReadUInt32();
                for (int i = 0; i < 11; i++) {
                    IA[i] = (IAflags & (1<<i)) != 0;
                }
                doubleBattle = reader.ReadUInt32() == 2;
            }
            using (BinaryReader reader = new BinaryReader(partyData)) {
                for (int i = 0; i < 6; i++)
                    trainerParty[i] = new PartyPokemon();
                if (!hasMoves && !hasItems) {
                    for (int i = 0; i < Math.Min((int)(partyData.Length / 8), partyCount); i++) {
                        trainerParty[i] = new PartyPokemon(reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt16());
                    }
                } else if (!hasMoves && hasItems) {
                    for (int i = 0; i < Math.Min((int)(partyData.Length / 10), partyCount); i++) {
                        trainerParty[i] = new PartyPokemon(reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt16());
                    }
                } else if (hasMoves && !hasItems) {
                    for (int i = 0; i < Math.Min((int)(partyData.Length / 16), partyCount); i++) {
                        trainerParty[i] = new PartyPokemon(reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt16());
                    }
                } else if (hasMoves && hasItems) {
                    for (int i = 0; i < Math.Min((int)(partyData.Length / 18), partyCount); i++) {
                        trainerParty[i] = new PartyPokemon(reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt16());
                    }
                }
            }
        }
        #endregion

        #region Methods
        public byte[] TrainerDataToByteArray() {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData)) {
                byte flags = 0;
                if (hasMoves) flags |= 1;
                if (hasItems) flags |= 2;
                writer.Write(flags);
                writer.Write(trainerClass);
                writer.Write(trdataunknown);
                writer.Write(partyCount);
                writer.Write(trainerItems[0]);
                writer.Write(trainerItems[1]);
                writer.Write(trainerItems[2]);
                writer.Write(trainerItems[3]);
                UInt32 IAflags = 0;
                for (int i = 0; i < 11; i++)
                    if (IA[i]) IAflags |= ((UInt32)(1) << i);
                writer.Write(IAflags);
                if (doubleBattle) writer.Write((UInt32)(2)); else writer.Write((UInt32)(0));
            }
            return newData.ToArray();
        }

        public byte[] PartyDataToByteArray() {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData)) {
                if (!hasMoves && !hasItems) {
                    for (int i = 0; i < partyCount; i++) {
                        writer.Write(trainerParty[i].unknown1);
                        writer.Write(trainerParty[i].level);
                        writer.Write(trainerParty[i].pokemon);
                        writer.Write(trainerParty[i].unknown2);
                    }
                } else if (!hasMoves && hasItems) {
                    for (int i = 0; i < partyCount; i++) {
                        writer.Write(trainerParty[i].unknown1);
                        writer.Write(trainerParty[i].level);
                        writer.Write(trainerParty[i].pokemon);
                        writer.Write(trainerParty[i].heldItem);
                        writer.Write(trainerParty[i].unknown2);
                    }
                } else if (hasMoves && !hasItems) {
                    for (int i = 0; i < partyCount; i++) {
                        writer.Write(trainerParty[i].unknown1);
                        writer.Write(trainerParty[i].level);
                        writer.Write(trainerParty[i].pokemon);
                        writer.Write(trainerParty[i].moves[0]);
                        writer.Write(trainerParty[i].moves[1]);
                        writer.Write(trainerParty[i].moves[2]);
                        writer.Write(trainerParty[i].moves[3]);
                        writer.Write(trainerParty[i].unknown2);
                    }
                } else if (hasMoves && hasItems) {
                    for (int i = 0; i < partyCount; i++) {
                        writer.Write(trainerParty[i].unknown1);
                        writer.Write(trainerParty[i].level);
                        writer.Write(trainerParty[i].pokemon);
                        writer.Write(trainerParty[i].heldItem);
                        writer.Write(trainerParty[i].moves[0]);
                        writer.Write(trainerParty[i].moves[1]);
                        writer.Write(trainerParty[i].moves[2]);
                        writer.Write(trainerParty[i].moves[3]);
                        writer.Write(trainerParty[i].unknown2);
                    }
                }
            }
            return newData.ToArray();
        }

        #endregion

    }

}