using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace DSPRE.ROMFiles {
    public class PartyPokemon : RomFile {
        #region Fields
        public ushort? pokeID = null;
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
            pokeID = Pokemon;
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
                writer.Write((ushort)pokeID);

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

    public class TrainerProperties : RomFile {
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
        #endregion

        #region Constructor
        public TrainerProperties(ushort ID, byte partyCount = 0) {
            trainerID = ID;
            trainerItems = new ushort[TRAINER_ITEMS];
            AI = new bool[AI_COUNT] { true, true, true, false, false, false, false, false, false, false, false };
            trDataUnknown = 0;
        }
        public TrainerProperties(ushort ID, Stream trainerPropertiesStream) {
            trainerID = ID;
            using (BinaryReader reader = new BinaryReader(trainerPropertiesStream)) {
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
        }
        #endregion

        #region Methods
        public override byte[] ToByteArray() {
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

                uint AIflags = 0;
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

        public void SaveToFileExplorePath(string suggestedFileName, bool showSuccessMessage = true) {
            SaveToFileExplorePath("Gen IV Trainer Properties", "trp", suggestedFileName, showSuccessMessage);
        }
        #endregion

    }

    public class Party : RomFile {
        private PartyPokemon[] content;
        private TrainerProperties trp;
        public bool exportCondensedData;
        public Party(int POKE_IN_PARTY, bool init, TrainerProperties trp) {
            this.trp = trp;
            this.content = new PartyPokemon[POKE_IN_PARTY];

            if (init) {
                for (int i = 0; i < content.Length; i++) {
                    this.content[i] = new PartyPokemon();
                }
            }
        }
        public PartyPokemon this[int index] {
            get {
                return content[index];
            }
            set {
                content[index] = value;
            }
        }
        public override byte[] ToByteArray() {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData)) {
                if (this.exportCondensedData && trp != null) {
                    byte condensedTrData = (byte)(((trp.hasMoves ? 1 : 0) & 0b_1) + (((trp.hasItems ? 1 : 0) & 0b_1) << 1) + ((trp.partyCount & 0b_1111_11) << 2));
                    writer.Write(condensedTrData);
                }

                foreach (PartyPokemon poke in this.content) {
                    if (poke.pokeID != null && poke.level > 0) {
                        writer.Write(poke.ToByteArray());
                    }
                }
            }
            return newData.ToArray();
        }
        public void SaveToFileExplorePath(string suggestedFileName, bool showSuccessMessage = true) {
            SaveToFileExplorePath("Gen IV Party Data", "pdat", suggestedFileName, showSuccessMessage);
        }
    }
    public class TrainerFile : RomFile {
        public const int POKE_IN_PARTY = 6;

        #region Fields
        public string name;
        public TrainerProperties trp;
        public Party party;
        #endregion

        #region Constructor
        public TrainerFile(TrainerProperties trp, string name = ""){
            this.name = name;
            this.trp = trp;
            trp.partyCount = 1;
            this.party = new Party(1, init: true, trp);
        }
        public TrainerFile(TrainerProperties trp, Stream partyData, string name = "") {
            this.name = name;
            this.trp = trp;
            party = new Party(POKE_IN_PARTY, init: false, trp);
            using (BinaryReader reader = new BinaryReader(partyData)) {
                int dividend = 8;
                int nMoves = 4;

                if (trp.hasMoves) {
                    dividend += nMoves * sizeof(ushort);
                }
                if (trp.hasItems) {
                    dividend += sizeof(ushort);
                }

                int endval = Math.Min((int)(partyData.Length / dividend), trp.partyCount);
                for (int i = 0; i < endval; i++) {
                    ushort unknown1 = reader.ReadUInt16();
                    ushort level = reader.ReadUInt16();
                    ushort pokemon = reader.ReadUInt16();

                    ushort? heldItem = null;
                    ushort[] moves = null;

                    if (trp.hasItems) {
                        heldItem = reader.ReadUInt16();
                    }
                    if (trp.hasMoves) {
                        moves = new ushort[4];
                        for (int m = 0; m < moves.Length; m++) {
                            ushort val = reader.ReadUInt16();
                            moves[m] = (ushort)(val == 0xFFFF ? 0 : val);
                        }
                    }

                    party[i] = new PartyPokemon(unknown1, level, pokemon, reader.ReadUInt16(), heldItem, moves);
                }
                for (int i = endval; i < POKE_IN_PARTY; i++) {
                    party[i] = new PartyPokemon();
                }
            }
        }
        #endregion

        #region Methods
        public override byte[] ToByteArray() {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData)) {
                writer.Write(name);

                byte[] trDat = trp.ToByteArray();
                writer.Write((byte)trDat.Length);
                writer.Write(trDat);

                byte[] pDat = party.ToByteArray();
                writer.Write((byte)pDat.Length);
                writer.Write(pDat);
            }
            return newData.ToArray();
        }

        public void SaveToFileExplorePath(string suggestedFileName, bool showSuccessMessage = true) {
            SaveToFileExplorePath("Gen IV Trainer File", "trf", suggestedFileName, showSuccessMessage);
        }
        #endregion

    }

}