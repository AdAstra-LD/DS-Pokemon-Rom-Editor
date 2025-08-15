using System;
using System.IO;
using System.Net.Sockets;
using System.Windows.Controls.Primitives;
using static DSPRE.RomInfo;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

namespace DSPRE.ROMFiles
{
    #region Enums
    public enum NaturalGiftType
    {
        Normal = 0,
        Fighting = 1,
        Flying = 2,
        Poison = 3,
        Ground = 4,
        Rock = 5,
        Bug = 6,
        Ghost = 7,
        Steel = 8,
        Fire = 10,
        Water = 11,
        Grass = 12,
        Electric = 13,
        Psychic = 14,
        Ice = 15,
        Dragon = 16,
        Dark = 17,
        NONE = 31
    }

    public enum FieldUseFunc : byte
    {
        Generic = 0,
        HealingItem = 1,
        Dummy1 = 2,
        Dummy2 = 3,
        Bicycle = 4,
        Dummy3 = 5,
        TMHM = 6,
        Mail = 7,
        Berry = 8,
        Dummy4 = 9,
        PalPad = 10,
        Dummy5 = 11,
        Dummy6 = 12,
        Dummy7 = 13,
        Honey = 14,
        Dummy8 = 15,
        OldRod = 16,
        GoodRod = 17,
        SuperRod = 18,
        Generic2 = 19,
        EvoStone = 20,
        EscapeRope = 21,
        Dummy9 = 22,
        ApricornBox = 23,
        BerryPots = 24,
        UnownReport = 25,
        DowsingMchn = 26,
        GbSounds = 27,
        Gracidea = 28,
        VSRecorder = 29
    }

    public enum BattleUseFunc : byte
    {
        No_Use = 0,
        Ball = 1,
        Healing = 2,
        Escape = 3
    }

    public enum FieldPocket
    {
        Items = 0,
        Medicine = 1,
        Balls = 2,
        TmHms = 3,
        Berries = 4,
        Mail = 5,
        BattleItems = 6,
        KeyItems = 7
    }

    [Flags]
    public enum BattlePocket
    {
        None = 0,
        PokeBalls = 1 << 0,      // 0b00001
        BattleItems = 1 << 1,    // 0b00010
        HpRestore = 1 << 2,      // 0b00100
        StatusHealers = 1 << 3,  // 0b01000
        PpRestore = 1 << 4      // 0b10000
    }

    public enum HoldEffect
    {
        None = 0,
        HpRestore = 1,
        GiratinaBoost = 2,
        DialgaBoost = 3,
        PalkiaBoost = 4,
        PrzRestore = 5,
        SlpRestore = 6,
        PsnRestore = 7,
        BrnRestore = 8,
        FrzRestore = 9,
        PpRestore = 10,
        ConfuseRestore = 11,
        StatusRestore = 12,
        HpPctRestore = 13,
        HpRestoreSpicy = 14,
        HpRestoreDry = 15,
        HpRestoreSweet = 16,
        HpRestoreBitter = 17,
        HpRestoreSour = 18,
        WeakenSeFire = 19,
        WeakenSeWater = 20,
        WeakenSeElectric = 21,
        WeakenSeGrass = 22,
        WeakenSeIce = 23,
        WeakenSeFight = 24,
        WeakenSePoison = 25,
        WeakenSeGround = 26,
        WeakenSeFlying = 27,
        WeakenSePsychic = 28,
        WeakenSeBug = 29,
        WeakenSeRock = 30,
        WeakenSeGhost = 31,
        WeakenSeDragon = 32,
        WeakenSeDark = 33,
        WeakenSeSteel = 34,
        WeakenNormal = 35,
        PinchAtkUp = 36,
        PinchDefUp = 37,
        PinchSpeedUp = 38,
        PinchSpAtkUp = 39,
        PinchSpDefUp = 40,
        PinchCritrateUp = 41,
        PinchRandomUp = 42,
        HpRestoreSe = 43,
        PinchAccUp = 44,
        PinchPriority = 45,
        RecoilPhysical = 46,
        RecoilSpecial = 47,
        AccReduce = 48,
        StatdownRestore = 49,
        EvsUpSpeedDown = 50,
        ExpShare = 51,
        SometimesPriority = 52,
        FriendshipUp = 53,
        HealInfatuation = 54,
        ChoiceAtk = 55,
        SometimesFlinch = 56,
        StrengthenBug = 57,
        MoneyUp = 58,
        EncountersDown = 59,
        LatiSpecial = 60,
        ClamperlSpAtk = 61,
        ClamperlSpDef = 62,
        Flee = 63,
        NoEvolve = 64,
        MaybeEndure = 65,
        ExpUp = 66,
        CritrateUp = 67,
        StrengthenSteel = 68,
        HpRestoreGradual = 69,
        EvolveSeadra = 70,
        PikaSpAtkUp = 71,
        StrengthenGround = 72,
        StrengthenRock = 73,
        StrengthenGrass = 74,
        StrengthenDark = 75,
        StrengthenFight = 76,
        StrengthenElectric = 77,
        StrengthenWater = 78,
        StrengthenFlying = 79,
        StrengthenPoison = 80,
        StrengthenIce = 81,
        StrengthenGhost = 82,
        StrengthenPsychic = 83,
        StrengthenFire = 84,
        StrengthenDragon = 85,
        StrengthenNormal = 86,
        EvolvePorygon = 87,
        HpRestoreOnDmg = 88,
        ChanseyCritrateUp = 89,
        DittoDefUp = 90,
        CuboneAtkUp = 91,
        FarfetchdCritrateUp = 92,
        AccuracyUp = 93,
        PowerUpPhys = 94,
        PowerUpSpec = 95,
        PowerUpSe = 96,
        ExtendScreens = 97,
        HpDrainOnAtk = 98,
        ChargeSkip = 99,
        PsnUser = 100,
        BrnUser = 101,
        DittoSpeedUp = 102,
        Endure = 103,
        AccuracyUpSlower = 104,
        BoostRepeated = 105,
        SpeedDownGrounded = 106,
        PriorityDown = 107,
        ReciprocateInfat = 108,
        HpRestorePsnType = 109,
        ExtendHail = 110,
        ExtendSandstorm = 111,
        ExtendSun = 112,
        ExtendRain = 113,
        ExtendTrapping = 114,
        ChoiceSpeed = 115,
        DmgUserContactXfr = 116,
        LvlupAtkEvUp = 117,
        LvlupDefEvUp = 118,
        LvlupSpAtkEvUp = 119,
        LvlupSpDefEvUp = 120,
        LvlupSpeedEvUp = 121,
        LvlupHpEvUp = 122,
        Switch = 123,
        LeechBoost = 124,
        ChoiceSpAtk = 125,
        ArceusFire = 126,
        ArceusWater = 127,
        ArceusElectric = 128,
        ArceusGrass = 129,
        ArceusIce = 130,
        ArceusFighting = 131,
        ArceusPoison = 132,
        ArceusGround = 133,
        ArceusFlying = 134,
        ArceusPsychic = 135,
        ArceusBug = 136,
        ArceusRock = 137,
        ArceusGhost = 138,
        ArceusDragon = 139,
        ArceusDark = 140,
        ArceusSteel = 141,
        EvolveRhydon = 142,
        EvolveElectabuzz = 143,
        EvolveMagmar = 144,
        EvolvePorygon2 = 145,
        EvolveDusclops = 146,
        // These are hg-engine items, not in gen 4
        //BurnDrive = 147,
        //ChillDrive = 148,
        //DouseDrive = 149,
        //ShockDrive = 150,
        //EvolveFeebas = 151,
        //Eviolite = 152,
        //HalveWeight = 153,
        //DamageOnContact = 154,
        //UngroundDestroyedOnHit = 155,
        //ForceSwitchOnDamage = 156,
        //LoseTypeImmunities = 157,
        //TrappingDamageUp = 158,
        //BoostSpecialAttackOnWaterHit = 159,
        //BoostAtkOnElectricHit = 160,
        //SwitchOutWhenHit = 161,
        //BoostAtkAndSpAtkOnSe = 162,
        //SpDefBoostNoStatusMoves = 163,
        //ArceusFairy = 164,
        //EvolveSwirlix = 165,
        //EvolveSpritzee = 166,
        //BoostSpecialDefenseOnWaterHit = 167,
        //BoostAtkOnIceHit = 168,
        //SporePowderImmunity = 169,
        //WeakenSeFairy = 170,
        //BoostDefOnPhysicalHit = 171,
        //BoostSpDefOnSpecialHit = 172,
        //IntimidateBoostSpeed = 173,
        //ExtendTerrain = 174,
        //PreventContactEffects = 175,
        //BoostDefOnElectricTerrain = 176,
        //BoostSpDefOnPsychicTerrain = 177,
        //BoostSpDefOnMistyTerrain = 178,
        //BoostDefOnGrassyTerrain = 179,
        //FightingMemory = 180,
        //FlyingMemory = 181,
        //PoisonMemory = 182,
        //GroundMemory = 183,
        //RockMemory = 184,
        //BugMemory = 185,
        //GhostMemory = 186,
        //SteelMemory = 187,
        //FireMemory = 188,
        //WaterMemory = 189,
        //GrassMemory = 190,
        //ElectricMemory = 191,
        //PsychicMemory = 192,
        //IceMemory = 193,
        //DragonMemory = 194,
        //DarkMemory = 195,
        //FairyMemory = 196,
        //TransformZacian = 197,
        //TransformZamazenta = 198,
        //BoostSpAtkOnSoundMove = 199,
        //SwitchOutOnStatDrop = 200,
        //IgnoreEntryHazards = 201,
        //BoostSpeedOnMiss = 202,
        //DropSpeedInTrickRoom = 203,
        //UnaffectedByRainOrSun = 204,
        //DialgaBoostAndTransform = 205,
        //PalkiaBoostAndTransform = 206,
        //GiratinaBoostAndTransform = 207,
        //ArceusNormal = 208,
        //ActivateParadoxAbilities = 209,
        //PreventAbilityChanges = 210,
        //PreventStatDrops = 211,
        //CopyStatIncrease = 212,
        //IncreasePunchingMoveDmg = 213,
        //PreventSecondaryEffects = 214,
        //IncreaseMultiStrikeMinimum = 215,
        //StrengthenFairy = 216,
        //CornerstoneMask = 217,
        //WellspringMask = 218,
        //HearthflameMask = 219
    }

    #endregion
    public class ItemData : RomFile
    {
        public ushort price;
        public HoldEffect holdEffect;
        public byte HoldEffectParam;
        public byte PluckEffect;
        public byte FlingEffect;
        public byte FlingPower;
        public byte NaturalGiftPower;

        // Bit-packed 16-bit value
        public NaturalGiftType naturalGiftType; // 5 bits
        public bool PreventToss;     // 1 bit
        public bool Selectable;      // 1 bit
        public FieldPocket fieldPocket;     // 4 bits
        public BattlePocket battlePocket;    // 5 bits

        public FieldUseFunc fieldUseFunc;
        public BattleUseFunc battleUseFunc;
        public byte PartyUse;
        public ItemPartyUseParam PartyUseParam;

        public int ID;

        public ItemData(Stream stream, int ID)
        {
            this.ID = ID;
            using (BinaryReader reader = new BinaryReader(stream))
            {
                price = reader.ReadUInt16();
                holdEffect = (HoldEffect)reader.ReadByte();
                HoldEffectParam = reader.ReadByte();
                PluckEffect = reader.ReadByte();
                FlingEffect = reader.ReadByte();
                FlingPower = reader.ReadByte();
                NaturalGiftPower = reader.ReadByte();

                ushort bitfield = reader.ReadUInt16();
                naturalGiftType = (NaturalGiftType)(bitfield & 0b0001_1111);
                PreventToss = (bitfield & (1 << 5)) != 0;
                Selectable = (bitfield & (1 << 6)) != 0;
                fieldPocket = (FieldPocket)((bitfield >> 7) & 0b1111);
                battlePocket = (BattlePocket)((bitfield >> 11) & 0b11111);

                fieldUseFunc = (FieldUseFunc)reader.ReadByte();
                battleUseFunc = (BattleUseFunc)reader.ReadByte();
                PartyUse = reader.ReadByte();

                reader.ReadByte(); // skip 1 byte padding_0D

                PartyUseParam = new ItemPartyUseParam(reader);

                reader.ReadBytes(2); // skip padding_22
            }
        }


        public ItemData(int ID) : this(new FileStream(RomInfo.gameDirs[DirNames.itemData].unpackedDir + "\\" + ID.ToString("D4"), FileMode.Open), ID) { }

        public override byte[] ToByteArray()
        {
            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(price);
                writer.Write((byte)holdEffect);
                writer.Write(HoldEffectParam);
                writer.Write(PluckEffect);
                writer.Write(FlingEffect);
                writer.Write(FlingPower);
                writer.Write(NaturalGiftPower);

                ushort bitfield = 0;
                bitfield |= (ushort)((byte)naturalGiftType & 0b11111);
                if (PreventToss) bitfield |= (1 << 5);
                if (Selectable) bitfield |= (1 << 6);
                bitfield |= (ushort)(((byte)fieldPocket & 0b1111) << 7);
                bitfield |= (ushort)((bitfield & ~(0b11111 << 11)) | (((byte)battlePocket & 0b11111) << 11));
                writer.Write(bitfield);

                writer.Write((byte)fieldUseFunc);
                writer.Write((byte)battleUseFunc);
                writer.Write(PartyUse);
                writer.Write((byte)0); // padding

                PartyUseParam.WriteTo(writer);

                writer.Write(new byte[2]); // padding

                return stream.ToArray();
            }
        }

        public void SaveToFileDefaultDir(int IDtoReplace, bool showSuccessMessage = true)
        {
            SaveToFileDefaultDir(DirNames.itemData, IDtoReplace, showSuccessMessage);
        }
        public void SaveToFileExplorePath(string suggestedFileName, bool showSuccessMessage = true)
        {
            SaveToFileExplorePath("Gen IV Item data", "bin", suggestedFileName, showSuccessMessage);
        }

        public class ItemPartyUseParam
        {
            public bool SlpHeal, PsnHeal, BrnHeal, FrzHeal, PrzHeal, CfsHeal, InfHeal, GuardSpec;
            public bool Revive, ReviveAll, LevelUp, Evolve;
            public int AtkStages, DefStages, SpAtkStages, SpDefStages, SpeedStages, AccuracyStages, CritRateStages;
            public bool PPUps, PPMax, PPRestore, PPRestoreAll, HPRestore;
            public bool EVHp, EVAtk, EVDef, EVSpeed, EVSpAtk, EVSpDef;
            public bool FriendshipLow, FriendshipMid, FriendshipHigh;
            public sbyte EVHpValue, EVAtkValue, EVDefValue, EVSpeedValue, EVSpAtkValue, EVSpDefValue;
            public byte HPRestoreParam, PPRestoreParam;
            public sbyte FriendshipLowValue, FriendshipMidValue, FriendshipHighValue;

            public ItemPartyUseParam(BinaryReader reader)
            {
                byte b0 = reader.ReadByte(); // byte 0
                SlpHeal = (b0 & (1 << 0)) != 0;
                PsnHeal = (b0 & (1 << 1)) != 0;
                BrnHeal = (b0 & (1 << 2)) != 0;
                FrzHeal = (b0 & (1 << 3)) != 0;
                PrzHeal = (b0 & (1 << 4)) != 0;
                CfsHeal = (b0 & (1 << 5)) != 0;
                InfHeal = (b0 & (1 << 6)) != 0;
                GuardSpec = (b0 & (1 << 7)) != 0;

                byte b1 = reader.ReadByte(); // byte 1
                Revive = (b1 & (1 << 0)) != 0;
                ReviveAll = (b1 & (1 << 1)) != 0;
                LevelUp = (b1 & (1 << 2)) != 0;
                Evolve = (b1 & (1 << 3)) != 0;
                AtkStages = (sbyte)((b1 >> 4) & 0xF); // signed 4-bit from high nibble

                byte b2 = reader.ReadByte(); // byte 2
                DefStages = (sbyte)(b2 & 0xF);
                SpAtkStages = (sbyte)((b2 >> 4) & 0xF);

                byte b3 = reader.ReadByte(); // byte 3
                SpDefStages = (sbyte)(b3 & 0xF);
                SpeedStages = (sbyte)((b3 >> 4) & 0xF);

                byte b4 = reader.ReadByte(); // byte 4
                AccuracyStages = (sbyte)(b4 & 0xF);
                CritRateStages = (sbyte)((b4 >> 4) & 0x3);
                PPUps = (b4 & (1 << 6)) != 0;
                PPMax = (b4 & (1 << 7)) != 0;

                byte b5 = reader.ReadByte(); // byte 5
                PPRestore = (b5 & (1 << 0)) != 0;
                PPRestoreAll = (b5 & (1 << 1)) != 0;
                HPRestore = (b5 & (1 << 2)) != 0;
                EVHp = (b5 & (1 << 3)) != 0;
                EVAtk = (b5 & (1 << 4)) != 0;
                EVDef = (b5 & (1 << 5)) != 0;
                EVSpeed = (b5 & (1 << 6)) != 0;
                EVSpAtk = (b5 & (1 << 7)) != 0;

                byte b6 = reader.ReadByte(); // byte 6
                EVSpDef = (b6 & (1 << 0)) != 0;
                FriendshipLow = (b6 & (1 << 1)) != 0;
                FriendshipMid = (b6 & (1 << 2)) != 0;
                FriendshipHigh = (b6 & (1 << 3)) != 0;
                // bits 4-7 unused

                // Remaining bytes (7–18): values
                EVHpValue = reader.ReadSByte();
                EVAtkValue = reader.ReadSByte();
                EVDefValue = reader.ReadSByte();
                EVSpeedValue = reader.ReadSByte();
                EVSpAtkValue = reader.ReadSByte();
                EVSpDefValue = reader.ReadSByte();
                HPRestoreParam = reader.ReadByte();
                PPRestoreParam = reader.ReadByte();
                FriendshipLowValue = reader.ReadSByte();
                FriendshipMidValue = reader.ReadSByte();
                FriendshipHighValue = reader.ReadSByte();

                reader.BaseStream.Seek(2, SeekOrigin.Current); // skip padding
            }


            public void WriteTo(BinaryWriter writer)
            {
                // Byte 0
                byte b0 = 0;
                b0 |= (byte)(SlpHeal ? 1 << 0 : 0);
                b0 |= (byte)(PsnHeal ? 1 << 1 : 0);
                b0 |= (byte)(BrnHeal ? 1 << 2 : 0);
                b0 |= (byte)(FrzHeal ? 1 << 3 : 0);
                b0 |= (byte)(PrzHeal ? 1 << 4 : 0);
                b0 |= (byte)(CfsHeal ? 1 << 5 : 0);
                b0 |= (byte)(InfHeal ? 1 << 6 : 0);
                b0 |= (byte)(GuardSpec ? 1 << 7 : 0);
                writer.Write(b0);

                // Byte 1
                byte b1 = 0;
                b1 |= (byte)(Revive ? 1 << 0 : 0);
                b1 |= (byte)(ReviveAll ? 1 << 1 : 0);
                b1 |= (byte)(LevelUp ? 1 << 2 : 0);
                b1 |= (byte)(Evolve ? 1 << 3 : 0);
                b1 |= (byte)((AtkStages & 0x0F) << 4); // signed 4-bit
                writer.Write(b1);

                // Byte 2
                byte b2 = 0;
                b2 |= (byte)((DefStages & 0x0F));
                b2 |= (byte)((SpAtkStages & 0x0F) << 4);
                writer.Write(b2);

                // Byte 3
                byte b3 = 0;
                b3 |= (byte)((SpDefStages & 0x0F));
                b3 |= (byte)((SpeedStages & 0x0F) << 4);
                writer.Write(b3);

                // Byte 4
                byte b4 = 0;
                b4 |= (byte)((AccuracyStages & 0x0F));
                b4 |= (byte)((CritRateStages & 0x03) << 4);
                b4 |= (byte)(PPUps ? 1 << 6 : 0);
                b4 |= (byte)(PPMax ? 1 << 7 : 0);
                writer.Write(b4);

                // Byte 5
                byte b5 = 0;
                b5 |= (byte)(PPRestore ? 1 << 0 : 0);
                b5 |= (byte)(PPRestoreAll ? 1 << 1 : 0);
                b5 |= (byte)(HPRestore ? 1 << 2 : 0);
                b5 |= (byte)(EVHp ? 1 << 3 : 0);
                b5 |= (byte)(EVAtk ? 1 << 4 : 0);
                b5 |= (byte)(EVDef ? 1 << 5 : 0);
                b5 |= (byte)(EVSpeed ? 1 << 6 : 0);
                b5 |= (byte)(EVSpAtk ? 1 << 7 : 0);
                writer.Write(b5);

                // Byte 6
                byte b6 = 0;
                b6 |= (byte)(EVSpDef ? 1 << 0 : 0);
                b6 |= (byte)(FriendshipLow ? 1 << 1 : 0);
                b6 |= (byte)(FriendshipMid ? 1 << 2 : 0);
                b6 |= (byte)(FriendshipHigh ? 1 << 3 : 0);
                // bits 4-7 unused
                writer.Write(b6);

                // Bytes 7–18: raw values
                writer.Write(EVHpValue);
                writer.Write(EVAtkValue);
                writer.Write(EVDefValue);
                writer.Write(EVSpeedValue);
                writer.Write(EVSpAtkValue);
                writer.Write(EVSpDefValue);
                writer.Write(HPRestoreParam);
                writer.Write(PPRestoreParam);
                writer.Write(FriendshipLowValue);
                writer.Write(FriendshipMidValue);
                writer.Write(FriendshipHighValue);

                // Bytes 19–20: padding
                writer.Write((byte)0);
                writer.Write((byte)0);
            }

            private static bool GetBit(ulong val, int bit) => ((val >> bit) & 1) != 0;
            private static void SetBit(ref ulong val, int bit, bool on)
            {
                if (on) val |= 1UL << bit;
                else val &= ~(1UL << bit);
            }

            private static byte EncodeStage(sbyte val) => (byte)((val + 6 < 0) ? 0 : (val + 6 > 15) ? 15 : val + 6);
            private static sbyte DecodeStage(byte val) => (sbyte)(val - 6);

            public override string ToString()
            {
                return $"ItemPartyUseParam: SlpHeal={SlpHeal}, PsnHeal={PsnHeal}, BrnHeal={BrnHeal}, FrzHeal={FrzHeal}, PrzHeal={PrzHeal}, CfsHeal={CfsHeal}, InfHeal={InfHeal}, GuardSpec={GuardSpec},\nRevive={Revive}, ReviveAll={ReviveAll}, LevelUp={LevelUp}, Evolve={Evolve},\nAtkStages={AtkStages}, DefStages={DefStages}, SpAtkStages={SpAtkStages}, SpDefStages={SpDefStages}, SpeedStages={SpeedStages}, AccuracyStages={AccuracyStages}, CritRateStages={CritRateStages},\nPPUps={PPUps}, PPMax={PPMax}, PPRestore={PPRestore}, PPRestoreAll={PPRestoreAll}, HPRestore={HPRestore},\nEVHp={EVHp}, EVAtk={EVAtk}, EVDef={EVDef}, EVSpeed={EVSpeed}, EVSpAtk={EVSpAtk}, EVSpDef={EVSpDef}, \nFriendshipLow={FriendshipLow}, FriendshipMid={FriendshipMid}, FriendshipHigh={FriendshipHigh}";
            }
        }

    }
}
