using System;
using System.IO;
using static DSPRE.RomInfo;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

namespace DSPRE.ROMFiles
{
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
        public NaturalGiftType naturalGiftType; // 5 bits
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
                naturalGiftType = (NaturalGiftType)(bitfield & 0b0001_1111);
                PreventToss = (bitfield & (1 << 5)) != 0;
                Selectable = (bitfield & (1 << 6)) != 0;
                fieldPocket = (FieldPocket)((bitfield >> 7) & 0b1111);
                battlePocket = (BattlePocket)((bitfield >> 11) & 0b11111);

                FieldUseFunc = reader.ReadByte();
                BattleUseFunc = reader.ReadByte();
                PartyUse = reader.ReadByte();

                //reader.ReadByte(); // skip 1 byte padding_0D

                PartyUseParam = new ItemPartyUseParam(reader);

                reader.ReadBytes(2); // skip padding_22
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
                bitfield |= (ushort)((byte)naturalGiftType & 0b11111);
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
            public bool SlpHeal; 
            public bool PsnHeal; 
            public bool BrnHeal; 
            public bool FrzHeal; 
            public bool PrzHeal; 
            public bool CfsHeal;
            public bool InfHeal;

            public bool GuardSpec;

            public bool Revive;
            public bool ReviveAll;

            public bool LevelUp;

            public bool Evolve;

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
                byte[] data = reader.ReadBytes(19); // Total size of struct

                // Byte 0: HealStatus1 bits
                byte healStatus1 = data[0];
                SlpHeal = (healStatus1 & (1 << 0)) != 0;
                PsnHeal = (healStatus1 & (1 << 1)) != 0;
                BrnHeal = (healStatus1 & (1 << 2)) != 0;
                FrzHeal = (healStatus1 & (1 << 3)) != 0;
                PrzHeal = (healStatus1 & (1 << 4)) != 0;
                CfsHeal = (healStatus1 & (1 << 5)) != 0;
                InfHeal = (healStatus1 & (1 << 6)) != 0;
                GuardSpec = (healStatus1 & (1 << 7)) != 0;

                // Byte 1: revive, revive_all, level_up, evolve, + 4 bits padding
                byte flags1 = data[1];
                Revive = (flags1 & (1 << 0)) != 0;
                ReviveAll = (flags1 & (1 << 1)) != 0;
                LevelUp = (flags1 & (1 << 2)) != 0;
                Evolve = (flags1 & (1 << 3)) != 0;

                // Byte 2: atk_stages (4 bits), def_stages (4 bits)
                byte stages1 = data[2];
                AtkStages = (sbyte)(stages1 & 0x0F);
                DefStages = (sbyte)((stages1 >> 4) & 0x0F);

                // Byte 3: spatk_stages (4 bits), spdef_stages (4 bits)
                byte stages2 = data[3];
                SpAtkStages = (sbyte)(stages2 & 0x0F);
                SpDefStages = (sbyte)((stages2 >> 4) & 0x0F);

                // Byte 4: speed_stages (4 bits), accuracy_stages (4 bits)
                byte stages3 = data[4];
                SpeedStages = (sbyte)(stages3 & 0x0F);
                AccuracyStages = (sbyte)((stages3 >> 4) & 0x0F);

                // Byte 5: critrate_stages (2 bits), pp_up, pp_max, pp_restore, pp_restore_all, hp_restore (5 bits)
                byte flags2 = data[5];
                CritRateStages = (sbyte)(flags2 & 0x03);
                PPUps = (flags2 & (1 << 2)) != 0;
                PPMax = (flags2 & (1 << 3)) != 0;
                PPRestore = (flags2 & (1 << 4)) != 0;
                PPRestoreAll = (flags2 & (1 << 5)) != 0;
                HPRestore = (flags2 & (1 << 6)) != 0;

                // Byte 6: ev flags
                byte evFlags = data[6];
                EVUps = new bool[6];
                for (int i = 0; i < 6; i++)
                    EVUps[i] = (evFlags & (1 << i)) != 0;

                // Byte 7: friendship flags
                byte friendFlags = data[7];
                FriendshipMods = new bool[3];
                for (int i = 0; i < 3; i++)
                    FriendshipMods[i] = (friendFlags & (1 << i)) != 0;

                // Bytes 8-13: EVParams
                EVParams = new sbyte[6];
                for (int i = 0; i < 6; i++)
                    EVParams[i] = (sbyte)data[8 + i];

                // Byte 14: HPRestoreParam
                HPRestoreParam = data[14];

                // Byte 15: PPRestoreParam
                PPRestoreParam = data[15];

                // Bytes 16–18: FriendshipParams
                FriendshipParams = new sbyte[3];
                for (int i = 0; i < 3; i++)
                    FriendshipParams[i] = (sbyte)data[16 + i];

                // Padding is ignored (2 bytes assumed to follow)
            }

            public void WriteTo(BinaryWriter writer)
            {
                byte[] data = new byte[18];

                // Byte 0: HealStatus1
                data[0] = 0;
                if (SlpHeal) data[0] |= (1 << 0);
                if (PsnHeal) data[0] |= (1 << 1);
                if (BrnHeal) data[0] |= (1 << 2);
                if (FrzHeal) data[0] |= (1 << 3);
                if (PrzHeal) data[0] |= (1 << 4);
                if (CfsHeal) data[0] |= (1 << 5);
                if (InfHeal) data[0] |= (1 << 6);
                if (GuardSpec) data[0] |= (1 << 7);

                // Byte 1: revive, revive_all, level_up, evolve
                data[1] = 0;
                if (Revive) data[1] |= (1 << 0);
                if (ReviveAll) data[1] |= (1 << 1);
                if (LevelUp) data[1] |= (1 << 2);
                if (Evolve) data[1] |= (1 << 3);

                // Byte 2: atk + def stages
                data[2] = (byte)((DefStages & 0x0F) << 4 | (AtkStages & 0x0F));

                // Byte 3: spatk + spdef stages
                data[3] = (byte)((SpDefStages & 0x0F) << 4 | (SpAtkStages & 0x0F));

                // Byte 4: speed + accuracy stages
                data[4] = (byte)((AccuracyStages & 0x0F) << 4 | (SpeedStages & 0x0F));

                // Byte 5: crit rate + pp/hp flags
                data[5] = (byte)(CritRateStages & 0x03);
                if (PPUps) data[5] |= (1 << 2);
                if (PPMax) data[5] |= (1 << 3);
                if (PPRestore) data[5] |= (1 << 4);
                if (PPRestoreAll) data[5] |= (1 << 5);
                if (HPRestore) data[5] |= (1 << 6);

                // Byte 6: EV flags
                data[6] = 0;
                for (int i = 0; i < 6; i++)
                    if (EVUps[i]) data[6] |= (byte)(1 << i);

                // Byte 7: Friendship flags
                data[7] = 0;
                for (int i = 0; i < 3; i++)
                    if (FriendshipMods[i]) data[7] |= (byte)(1 << i);

                // Bytes 8-13: EVParams
                for (int i = 0; i < 6; i++)
                    data[8 + i] = (byte)EVParams[i];

                // Byte 14: HPRestoreParam
                data[14] = HPRestoreParam;

                // Byte 15: PPRestoreParam
                data[15] = PPRestoreParam;

                // Bytes 16–18: FriendshipParams
                for (int i = 0; i < 3; i++)
                    data[16 + i] = (byte)FriendshipParams[i];

                // Write all bytes
                writer.Write(data);

                // Write padding
                writer.Write(new byte[2]);
            }

        }
    }
}
