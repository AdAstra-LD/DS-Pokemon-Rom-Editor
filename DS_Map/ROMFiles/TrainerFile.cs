using ScintillaNET;
using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using static DSPRE.ROMFiles.PartyPokemon;

namespace DSPRE.ROMFiles {
    public class PartyPokemon : RomFile {
        public const int MON_NUMBER_BITSIZE = 10;
        public const int MON_NUMBER_BITMASK = (1 << MON_NUMBER_BITSIZE) - 1;

        public const int MON_FORM_BITSIZE = 6; //16-MON_NUMBER_BITSIZE
        public const int MON_FORM_BITMASK = ((1 << MON_FORM_BITSIZE) - 1) << MON_NUMBER_BITSIZE;

        #region Fields
        public ushort? pokeID = null;
        public ushort formID = 0; //unused in DP
        public ushort level = 0;
        public byte difficulty = 0;
        public GenderAndAbilityFlags genderAndAbilityFlags; //only used for HGSS, filler byte for the rest of the games
        public ushort ballSeals = 0;

        public ushort? heldItem = null;
        public ushort[] moves = null;

        public enum GenderAndAbilityFlags {
            NO_FLAGS = 0,
            FORCE_MALE = 0x1,
            FORCE_FEMALE = 0x2,
            ABILITY_SLOT1 = 0x10,
            ABILITY_SLOT2 = 0x20
        }
        #endregion

        #region Constructor
        public PartyPokemon(bool chooseItems = false, bool chooseMoves = false) {
            UpdateItemsAndMoves(chooseItems, chooseMoves);
        }

        public PartyPokemon(byte difficulty, GenderAndAbilityFlags genderAndAbilityFlags, ushort Level, ushort pokeNum, ushort ballSealConfig, ushort? heldItem = null, ushort[] moves = null) {
            pokeID = pokeNum;
            level = Level;
            this.difficulty = difficulty;
            this.genderAndAbilityFlags = genderAndAbilityFlags;
            ballSeals = ballSealConfig;
            this.heldItem = heldItem;
            this.moves = moves;
        }

        public PartyPokemon(byte difficulty, ushort Level, ushort pokeNum, ushort? heldItem = null, ushort[] moves = null) {
            // Simply adding a new constructor for Diamond and Pearl since they dont have ball seal config
            pokeID = pokeNum;
            level = Level;
            this.difficulty = difficulty;
            this.heldItem = heldItem;
            this.moves = moves;
        }
        public PartyPokemon(byte difficulty, GenderAndAbilityFlags genderAndAbilityFlags, ushort Level, ushort pokeNum, ushort formNum, ushort ballSealConfig, ushort? heldItem = null, ushort[] moves = null) :
            this(difficulty, genderAndAbilityFlags, Level, pokeNum, ballSealConfig, heldItem, moves) {

            formID = formNum;
        }
        public override byte[] ToByteArray() {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData)) {
                writer.Write(difficulty);
                writer.Write((byte)genderAndAbilityFlags);
                writer.Write(level);
                writer.Write((ushort)((pokeID ?? 0) | formID << MON_NUMBER_BITSIZE));

                if (heldItem != null) {
                    writer.Write((ushort)heldItem);
                }

                if (moves != null) {
                    foreach (ushort move in moves) {
                        writer.Write(move);
                    }
                }
                if (RomInfo.gameFamily == RomInfo.GameFamilies.HGSS || RomInfo.gameFamily == RomInfo.GameFamilies.Plat)
                    writer.Write(ballSeals); // Diamond and Pearl apparently dont save ball capsule data in enemy trainer pokedata!!!
            }
            return newData.ToArray();
        }
        public void UpdateItemsAndMoves(bool chooseItems = false, bool chooseMoves = false) {
            if (chooseItems) {
                this.heldItem = 0;
            }
            if (chooseMoves) {
                this.moves = new ushort[4];
            }
        }

        public override string ToString() {
            return CheckEmpty() ? "Empty" : this.pokeID + " Lv. " + this.level;
        }
        public bool CheckEmpty() {
            return this is null || pokeID is null || level <= 0;
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
        public bool chooseMoves = false;
        public bool chooseItems = false;

        public ushort[] trainerItems = new ushort[TRAINER_ITEMS];
        public BitArray AI;
        #endregion

        #region Constructor
        public TrainerProperties(ushort ID, byte partyCount = 0) {
            trainerID = ID;
            trainerItems = new ushort[TRAINER_ITEMS];
            AI = new BitArray(new bool[AI_COUNT] { true, false, false, false, false, false, false, false, false, false, false });
            trDataUnknown = 0;
        }
        public TrainerProperties(ushort ID, Stream trainerPropertiesStream) {
            trainerID = ID;
            using (BinaryReader reader = new BinaryReader(trainerPropertiesStream)) {
                byte flags = reader.ReadByte();
                chooseMoves = (flags & 1) != 0;
                chooseItems = (flags & 2) != 0;

                trainerClass = reader.ReadByte();
                trDataUnknown = reader.ReadByte();
                partyCount = reader.ReadByte();

                for (int i = 0; i < trainerItems.Length; i++) {
                    trainerItems[i] = reader.ReadUInt16();
                }

                AI = new BitArray(BitConverter.GetBytes(reader.ReadUInt32()));
                doubleBattle = reader.ReadUInt32() == 2;
            }
        }
        #endregion

        #region Methods
        public override byte[] ToByteArray() {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData)) {
                byte flags = 0;
                flags |= (byte)(chooseMoves ? 1 : 0);
                flags |= (byte)(chooseItems ? 2 : 0);

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

        public const int MOVES_PER_POKE = 4;
        public Party(int POKE_IN_PARTY, bool init, TrainerProperties trp) {
            this.trp = trp;
            this.content = new PartyPokemon[POKE_IN_PARTY];

            if (init) {
                for (int i = 0; i < content.Length; i++) {
                    this.content[i] = new PartyPokemon();
                }
            }
        }

        public Party(bool readFirstByte, int maxPoke, Stream partyData, TrainerProperties traipr) {
            using (BinaryReader reader = new BinaryReader(partyData)) {
                try {
                    this.trp = traipr;
                    if (readFirstByte) {
                        byte flags = reader.ReadByte();

                        trp.chooseMoves = (flags & 1) != 0;
                        trp.chooseItems = (flags & 2) != 0;
                        trp.partyCount = (byte)((flags & 28) >> 2);
                    }

                    int dividend = 8;

                    if (trp.chooseMoves) {
                        dividend += Party.MOVES_PER_POKE * sizeof(ushort);
                    }
                    if (trp.chooseItems) {
                        dividend += sizeof(ushort);
                    }

                    int endval = Math.Min((int)(partyData.Length - 1 / dividend), trp.partyCount);
                    this.content = new PartyPokemon[maxPoke];
                    for (int i = 0; i < endval; i++) {
                        byte difficulty = reader.ReadByte();
                        GenderAndAbilityFlags genderAndAbilityFlags = (GenderAndAbilityFlags)reader.ReadByte();
                        ushort level = reader.ReadUInt16();

                        ushort monFull = reader.ReadUInt16();
                        ushort pokemon = (ushort)(monFull & PartyPokemon.MON_NUMBER_BITMASK);
                        ushort form_no = (ushort)((monFull & PartyPokemon.MON_FORM_BITMASK) >> PartyPokemon.MON_NUMBER_BITSIZE);

                        ushort? heldItem = null;
                        ushort[] moves = null;

                        if (trp.chooseItems) {
                            heldItem = reader.ReadUInt16();
                        }
                        if (trp.chooseMoves) {
                            moves = new ushort[MOVES_PER_POKE];
                            for (int m = 0; m < moves.Length; m++) {
                                ushort val = reader.ReadUInt16();
                                moves[m] = (ushort)(val == ushort.MaxValue ? 0 : val);
                            }
                        }


                        if (RomInfo.gameFamily == RomInfo.GameFamilies.HGSS || RomInfo.gameFamily == RomInfo.GameFamilies.Plat)
                            content[i] = new PartyPokemon(difficulty, genderAndAbilityFlags, level, pokemon, form_no, reader.ReadUInt16(), heldItem, moves);
                        else
                            content[i] = new PartyPokemon(difficulty, level, pokemon, heldItem, moves); // Diamond and Pearl apparently dont save ball capsule data in enemy trainer pokedata!!!

                    }
                    for (int i = endval; i < maxPoke; i++) {
                        content[i] = new PartyPokemon();
                    };
                } catch (EndOfStreamException) {
                    MessageBox.Show("There was a problem reading the party data of this " + this.GetType().Name + ".", "Read Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        public override string ToString() {
            if (this.content == null) {
                return "Empty";
            } else {
                string buffer = "";
                byte nonEmptyCtr = CountNonEmptyMons();
                buffer += nonEmptyCtr + " Poke ";
                if (this.trp.chooseMoves) {
                    buffer += ", moves ";
                }
                if (this.trp.chooseItems) {
                    buffer += ", items ";
                }
                return buffer;
            }
        }

        public byte CountNonEmptyMons() {
            byte nonEmptyCtr = 0;
            foreach (PartyPokemon p in this.content) {
                if (!p.CheckEmpty()) {
                    nonEmptyCtr++;
                }
            }

            return nonEmptyCtr;
        }

        public override byte[] ToByteArray() {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData)) {
                if (this.exportCondensedData && trp != null) {
                    byte condensedTrData = (byte)(((trp.chooseMoves ? 1 : 0) & 0b_1) + (((trp.chooseItems ? 1 : 0) & 0b_1) << 1) + ((trp.partyCount & 0b_1111_11) << 2));
                    writer.Write(condensedTrData);
                }

                foreach (PartyPokemon poke in this.content) {
                    if (!poke.CheckEmpty()) {
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
        public const int defaultNameLen = 10; //Does not include special \0 end character!
        public const int POKE_IN_PARTY = 6;
        public static readonly string NAME_NOT_FOUND = "NAME READ ERROR";

        #region Fields
        public string name;
        public TrainerProperties trp;
        public Party party;
        #endregion

        #region Constructor
        public TrainerFile(TrainerProperties trp, string name = "") {
            this.name = name;
            this.trp = trp;
            trp.partyCount = 1;
            this.party = new Party(1, init: true, trp);
        }
        public TrainerFile(TrainerProperties trp, Stream partyData, string name = "") {
            this.name = name;
            this.trp = trp;
            party = new Party(readFirstByte: false, POKE_IN_PARTY, partyData, this.trp);
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