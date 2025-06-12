using System;
using System.IO;
using static DSPRE.RomInfo;

namespace DSPRE.ROMFiles
{
    public enum FieldPocket
    {
        Items = 0,
        Medicine = 1,
        Balls = 2,
        TmHms = 3,
        Berries = 4,
        Mail = 5,
        BattleItems = 6,
        KeyItems = 7,
        PocketsCount = 8
    }

    public enum BattlePocket
    {
        None = 0,
        PokeBalls = 1,
        BattleItems = 2,
        HpRestore = 4,
        StatusHealers = 8,
        PpRestore = 16
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
        public byte NaturalGiftType; // 5 bits
        public bool PreventToss;     // 1 bit
        public bool Selectable;      // 1 bit
        public FieldPocket fieldPocket;     // 4 bits
        public BattlePocket battlePocket;    // 5 bits

        public byte FieldUseFunc;
        public byte BattleUseFunc;
        public byte PartyUse;
        public ItemPartyUseParam PartyUseParam;

        public ItemData(Stream stream)
        {
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
                NaturalGiftType = (byte)(bitfield & 0b0001_1111);
                PreventToss = (bitfield & (1 << 5)) != 0;
                Selectable = (bitfield & (1 << 6)) != 0;
                fieldPocket = (FieldPocket)(byte)((bitfield >> 7) & 0b1111);
                battlePocket = (BattlePocket)(byte)((bitfield >> 11) & 0b11111);

                FieldUseFunc = reader.ReadByte();
                BattleUseFunc = reader.ReadByte();
                PartyUse = reader.ReadByte();

                reader.ReadByte(); // padding

                PartyUseParam = new ItemPartyUseParam(reader);

                reader.ReadBytes(2); // padding
            }
        }

        public ItemData(int ID) : this(new FileStream(RomInfo.gameDirs[DirNames.itemData].unpackedDir + "\\" + ID.ToString("D4"), FileMode.Open)) { }

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
                bitfield |= (ushort)(NaturalGiftType & 0b11111);
                if (PreventToss) bitfield |= (1 << 5);
                if (Selectable) bitfield |= (1 << 6);
                bitfield |= (ushort)(((byte)fieldPocket & 0b1111) << 7);
                bitfield |= (ushort)(((byte)battlePocket & 0b11111) << 11);
                writer.Write(bitfield);

                writer.Write(FieldUseFunc);
                writer.Write(BattleUseFunc);
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
            // Flags (bit-packed)
            public byte HealStatus1; // slp, psn, brn, frz, prz, cfs, inf, guard_spec
            public ushort Flags;     // revive, revive_all, level_up, evolve, etc.

            public sbyte AtkStages;
            public sbyte DefStages;
            public sbyte SpAtkStages;
            public sbyte SpDefStages;
            public sbyte SpeedStages;
            public sbyte AccuracyStages;
            public sbyte CritRateStages;

            public bool PPUps;
            public bool PPMax;
            public bool PPRestore;
            public bool PPRestoreAll;
            public bool HPRestore;

            public bool[] EVUps = new bool[6]; // hp, atk, def, speed, spatk, spdef
            public bool[] FriendshipMods = new bool[3]; // lo, med, hi

            public sbyte[] EVParams = new sbyte[6]; // hp, atk, def, speed, spatk, spdef
            public byte HPRestoreParam;
            public byte PPRestoreParam;
            public sbyte[] FriendshipParams = new sbyte[3]; // lo, med, hi

            public ItemPartyUseParam(BinaryReader reader)
            {
                HealStatus1 = reader.ReadByte();
                Flags = reader.ReadUInt16();

                AtkStages = (sbyte)reader.ReadByte();
                DefStages = (sbyte)reader.ReadByte();
                SpAtkStages = (sbyte)reader.ReadByte();
                SpDefStages = (sbyte)reader.ReadByte();
                SpeedStages = (sbyte)reader.ReadByte();
                AccuracyStages = (sbyte)reader.ReadByte();
                CritRateStages = (sbyte)reader.ReadByte();

                byte ppFlags = reader.ReadByte();
                PPUps = (ppFlags & (1 << 0)) != 0;
                PPMax = (ppFlags & (1 << 1)) != 0;
                PPRestore = (ppFlags & (1 << 2)) != 0;
                PPRestoreAll = (ppFlags & (1 << 3)) != 0;
                HPRestore = (ppFlags & (1 << 4)) != 0;

                byte evFlags = reader.ReadByte();
                for (int i = 0; i < 6; i++)
                {
                    EVUps[i] = (evFlags & (1 << i)) != 0;
                }

                byte friendFlags = reader.ReadByte();
                for (int i = 0; i < 3; i++)
                {
                    FriendshipMods[i] = (friendFlags & (1 << i)) != 0;
                }

                for (int i = 0; i < 6; i++)
                {
                    EVParams[i] = reader.ReadSByte();
                }

                HPRestoreParam = reader.ReadByte();
                PPRestoreParam = reader.ReadByte();

                for (int i = 0; i < 3; i++)
                {
                    FriendshipParams[i] = reader.ReadSByte();
                }

                reader.ReadBytes(2); // padding
            }

            public void WriteTo(BinaryWriter writer)
            {
                writer.Write(HealStatus1);
                writer.Write(Flags);
                writer.Write((byte)AtkStages);
                writer.Write((byte)DefStages);
                writer.Write((byte)SpAtkStages);
                writer.Write((byte)SpDefStages);
                writer.Write((byte)SpeedStages);
                writer.Write((byte)AccuracyStages);
                writer.Write((byte)CritRateStages);

                byte ppFlags = 0;
                if (PPUps) ppFlags |= (1 << 0);
                if (PPMax) ppFlags |= (1 << 1);
                if (PPRestore) ppFlags |= (1 << 2);
                if (PPRestoreAll) ppFlags |= (1 << 3);
                if (HPRestore) ppFlags |= (1 << 4);
                writer.Write(ppFlags);

                byte evFlags = 0;
                for (int i = 0; i < 6; i++)
                {
                    if (EVUps[i]) evFlags |= (byte)(1 << i);
                }
                writer.Write(evFlags);

                byte friendFlags = 0;
                for (int i = 0; i < 3; i++)
                {
                    if (FriendshipMods[i]) friendFlags |= (byte)(1 << i);
                }
                writer.Write(friendFlags);

                foreach (sbyte val in EVParams)
                {
                    writer.Write(val);
                }

                writer.Write(HPRestoreParam);
                writer.Write(PPRestoreParam);

                foreach (sbyte val in FriendshipParams)
                {
                    writer.Write(val);
                }

                writer.Write(new byte[2]); // padding
            }
        }
    }
}
