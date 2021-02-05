using System.IO;
using System.Windows.Forms;

namespace DSPRE.ROMFiles {
    /* ---------------------- WILD POKéMON DATA STRUCTURE (DPPt):----------------------------
        
       0x0  //  byte:       Walking encounter rate
       0x4  //  byte:       Level of
       0x2  //  ushort:     Matrix number
       0x4  //  ushort:     Script file number
       0x6  //  ushort:     Level script file number
       0x8  //  ushort:     Text Archive number
       0xA  //  ushort:     Day music track number
       0xC  //  ushort:     Night music track number
       0xE  //  ushort:     Wild Pokémon file number
       0x10 //  ushort:     Event file number

       * Diamond/Pearl:
       0x12 //  ushort:     Index of map name in Text Archive #382 (US version)   
       
       * Platinum:
       0x12 //  byte:       Index of map name in Text Archive #382 (US version)
       0x13 //  byte:       Map name textbox type value

       0x14 //  byte:       Weather value
       0x15 //  byte:       Camera value
       0x16 //  byte:       Boolean flag: show name when entering map
       0x17 //  byte:       Bitwise permission flags:

       -----------------    1: Allow Fly
       -----------------    2: ?
       -----------------    3: ?
       -----------------    4: Allow Bike usage
       -----------------    5: ?
       -----------------    6: ?
       -----------------    7: Esc. Rope
       -----------------    8: ?

    /* ---------------------- WILD POKéMON DATA STRUCTURE (HGSS):----------------------------
        
       0x0  //  byte:       Wild Pokémon file number
       0x1  //  byte:       Area data value
       0x2  //  byte:       ?
       0x3  //  byte:       ?
       0x4  //  ushort:     Matrix number
       0x6  //  ushort:     Script file number
       0x8  //  ushort:     Level script file
       0xA  //  ushort:     Text Archive number
       0xC  //  ushort:     Day music track number
       0xE  //  ushort:     Night music track number
       0x10 //  ushort:     Event file number
       0x12 //  byte:       Index of map name in Text Archive #382 (US version)
       0x13 //  byte:       Map name textbox type value
       0x14 //  byte:       Weather value
       0x15 //  byte:       Camera value
       0x16 //  byte:       Follow mode (for the Pokémon following hero)
       0x17 //  byte:       Bitwise permission flags:

       -----------------    1: Allow Fly
       -----------------    2: ?
       -----------------    3: ?
       -----------------    4: Allow Bike usage
       -----------------    5: ?
       -----------------    6: ?
       -----------------    7: Esc. Rope
       -----------------    8: ?

    ----------------------------------------------------------------------------------*/

    /// <summary>
    /// General class to store common wild Pokémon data across all Gen IV Pokémon NDS games
    /// </summary>
    public abstract class EncounterFile
	{
        #region Fields (19)
        
        /* Encounter rates */
        public byte goodRodRate { get; set; }
        public byte oldRodRate { get; set; }
        public byte superRodRate { get; set; }
        public byte surfRate { get; set; }
        public byte walkingRate { get; set; }

        /* Levels */
        public byte[] goodRodMaxLevels = new byte[5];
        public byte[] goodRodMinLevels = new byte[5];
        public byte[] oldRodMaxLevels = new byte[5];
        public byte[] oldRodMinLevels = new byte[5];
        public byte[] walkingLevels = new byte[12];
        public byte[] superRodMaxLevels = new byte[5];
        public byte[] superRodMinLevels = new byte[5];
        public byte[] surfMaxLevels = new byte[5];
        public byte[] surfMinLevels = new byte[5];

        /* Encounters */
        public ushort[] goodRodPokémon = new ushort[5];
        public ushort[] oldRodPokémon = new ushort[5];
        public ushort[] superRodPokémon = new ushort[5];
        public ushort[] surfPokémon = new ushort[5];
        public ushort[] swarmPokémon { get; set; }
        #endregion

        #region Methods (1)
        public abstract byte[] SaveEncounterFile();
        #endregion
    }

    /// <summary>
    /// Class to store wild Pokémon data from Pokémon Diamond, Pearl and Platinum
    /// </summary>
    public class EncounterFileDPPt: EncounterFile
    {
        #region Fields (9)       
        /* Field encounters */
        public uint[] radarPokémon = new uint[4];
        public uint[] walkingPokémon = new uint[12];

        /* Time-specific encounters */
        public uint[] morningPokémon = new uint[2];
        public uint[] nightPokémon = new uint[2];

        /* Dual slot exclusives */
        public uint[] rubyPokémon = new uint[2];
        public uint[] sapphirePokémon = new uint[2];
        public uint[] emeraldPokémon = new uint[2];
        public uint[] fireRedPokémon = new uint[2];
        public uint[] leafGreenPokémon = new uint[2];
        #endregion

        #region Constructors (1)
        public EncounterFileDPPt(Stream data)
        {
            using (BinaryReader reader = new BinaryReader(data))
            {
                /* Walking encounters */
                walkingRate = (byte)reader.ReadInt32();
                for (int i = 0; i < 12; i++)
                {
                    walkingLevels[i] = (byte)reader.ReadUInt32();
                    walkingPokémon[i] = reader.ReadUInt32();
                }

                /* Swarms */
                swarmPokémon = new ushort[2];
                for (int i = 0; i < 2; i++) swarmPokémon[i] = (ushort)reader.ReadUInt32();
                
                /* Time-specific encounters */
                for (int i = 0; i < 2; i++) morningPokémon[i] = reader.ReadUInt32();
                for (int i = 0; i < 2; i++) nightPokémon[i] = reader.ReadUInt32();

                /* Poké-Radar encounters */
                for (int i = 0; i < 4; i++) radarPokémon[i] = reader.ReadUInt32();

                reader.BaseStream.Position = 0xA4;

                /* Dual-slot encounters */
                for (int i = 0; i < 2; i++) rubyPokémon[i] = reader.ReadUInt32();
                for (int i = 0; i < 2; i++) sapphirePokémon[i] = reader.ReadUInt32();
                for (int i = 0; i < 2; i++) emeraldPokémon[i] = reader.ReadUInt32();
                for (int i = 0; i < 2; i++) fireRedPokémon[i] = reader.ReadUInt32();
                for (int i = 0; i < 2; i++) leafGreenPokémon[i] = reader.ReadUInt32();

                /* Surf encounters */
                surfRate = (byte)reader.ReadInt32();
                for (int i = 0; i < 5; i++)
                {
                    surfMaxLevels[i] = reader.ReadByte();
                    surfMinLevels[i] = reader.ReadByte();
                    reader.BaseStream.Position += 0x2;
                    surfPokémon[i] = (ushort)reader.ReadUInt32();
                }

                reader.BaseStream.Position = 0x124;

                /* Old Rod encounters */
                oldRodRate = (byte)reader.ReadInt32();
                for (int i = 0; i < 5; i++)
                {
                    oldRodMaxLevels[i] = reader.ReadByte();
                    oldRodMinLevels[i] = reader.ReadByte();
                    reader.BaseStream.Position += 0x2;
                    oldRodPokémon[i] = (ushort)reader.ReadUInt32();
                }

                /* Good Rod encounters */
                goodRodRate = (byte)reader.ReadInt32();
                for (int i = 0; i < 5; i++)
                {
                    goodRodMaxLevels[i] = reader.ReadByte();
                    goodRodMinLevels[i] = reader.ReadByte();
                    reader.BaseStream.Position += 0x2;
                    goodRodPokémon[i] = (ushort)reader.ReadUInt32();
                }

                /* Super Rod encounters */
                superRodRate = (byte)reader.ReadInt32();
                for (int i = 0; i < 5; i++)
                {
                    superRodMaxLevels[i] = reader.ReadByte();
                    superRodMinLevels[i] = reader.ReadByte();
                    reader.BaseStream.Position += 0x2;
                    superRodPokémon[i] = (ushort)reader.ReadUInt32();
                }
            }
        }
        #endregion Constructors

        #region Methods (1)
        public override byte[] SaveEncounterFile()
        {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData))
            {
                writer.Write((uint)walkingRate);
                
                /* Walking encounters */
                for (int i = 0; i < 12; i++)
                {
                    writer.Write((uint)walkingLevels[i]);
                    writer.Write(walkingPokémon[i]);
                }

                /* Swarms */
                for (int i = 0; i < 2; i++) writer.Write((uint)swarmPokémon[i]);

                /* Time-specific encounters */
                for (int i = 0; i < 2; i++) writer.Write(morningPokémon[i]);
                for (int i = 0; i < 2; i++) writer.Write(nightPokémon[i]);

                /* Poké-Radar encounters */
                for (int i = 0; i < 4; i++) writer.Write(radarPokémon[i]);

                writer.BaseStream.Position = 0xA4;

                /* Dual-slot encounters */
                for (int i = 0; i < 2; i++) writer.Write(rubyPokémon[i]);
                for (int i = 0; i < 2; i++) writer.Write(sapphirePokémon[i]);
                for (int i = 0; i < 2; i++) writer.Write(emeraldPokémon[i]);
                for (int i = 0; i < 2; i++) writer.Write(fireRedPokémon[i]);
                for (int i = 0; i < 2; i++) writer.Write(leafGreenPokémon[i]);

                /* Surf encounters */
                writer.Write((uint)surfRate);
                for (int i = 0; i < 5; i++)
                {
                    writer.Write(surfMaxLevels[i]);
                    writer.Write(surfMinLevels[i]);
                    writer.BaseStream.Position += 0x2;
                    writer.Write((uint)surfPokémon[i]);
                }

                writer.BaseStream.Position = 0x124;

                /* Old Rod encounters */
                writer.Write((uint)oldRodRate);
                for (int i = 0; i < 5; i++)
                {
                    writer.Write(oldRodMaxLevels[i]);
                    writer.Write(oldRodMinLevels[i]);
                    writer.BaseStream.Position += 0x2;
                    writer.Write((uint)oldRodPokémon[i]);
                }

                /* Good Rod encounters */
                writer.Write((uint)goodRodRate);
                for (int i = 0; i < 5; i++)
                {
                    writer.Write(goodRodMaxLevels[i]);
                    writer.Write(goodRodMinLevels[i]);
                    writer.BaseStream.Position += 0x2;
                    writer.Write((uint)goodRodPokémon[i]);
                }

                /* Super Rod encounters */
                writer.Write((uint)superRodRate);
                for (int i = 0; i < 5; i++)
                {
                    writer.Write(superRodMaxLevels[i]);
                    writer.Write(superRodMinLevels[i]);
                    writer.BaseStream.Position += 0x2;
                    writer.Write((uint)superRodPokémon[i]);
                }
            }
            return newData.ToArray();
        }
        #endregion
    }
   
    /// <summary>
    /// Class to store wild Pokémon data from Pokémon HeartGold and SoulSilver
    /// </summary>
    public class EncounterFileHGSS : EncounterFile
    {
        #region Fields (9)
        public byte rockSmashRate;
        public ushort[] morningPokémon = new ushort[12];
        public ushort[] dayPokémon = new ushort[12];
        public ushort[] nightPokémon = new ushort[12];
        public ushort[] hoennMusicPokémon = new ushort[2];
        public ushort[] sinnohMusicPokémon = new ushort[2];
        public ushort[] rockSmashPokémon = new ushort[2];
        public byte[] rockSmashMinLevels = new byte[2];
        public byte[] rockSmashMaxLevels = new byte[2];
        #endregion

        #region Constructors (1)
        public EncounterFileHGSS(Stream data)
        {
            using (BinaryReader reader = new BinaryReader(data))
            {
                bool error = false;

                /* Encounter rates */
                walkingRate = reader.ReadByte();
                surfRate = reader.ReadByte();
                rockSmashRate = reader.ReadByte();
                oldRodRate = reader.ReadByte();
                goodRodRate = reader.ReadByte();
                superRodRate = reader.ReadByte();

                reader.BaseStream.Position += 0x2;

                /* Walking encounters levels */
                for (int i = 0; i < 12; i++) 
                    walkingLevels[i] = reader.ReadByte();

                /* Morning walking encounters */
                for (int i = 0; i < 12; i++) 
                    morningPokémon[i] = reader.ReadUInt16();

                /* Day walking encounters */
                for (int i = 0; i < 12; i++) 
                    dayPokémon[i] = reader.ReadUInt16();

                /* Night walking encounters */
                for (int i = 0; i < 12; i++) 
                    nightPokémon[i] = reader.ReadUInt16();

                /* PokéGear music encounters */
                for (int i = 0; i < 2; i++) 
                    hoennMusicPokémon[i] = reader.ReadUInt16();
                for (int i = 0; i < 2; i++) 
                    sinnohMusicPokémon[i] = reader.ReadUInt16();

                /* Surf encounters */
                for (int i = 0; i < 5; i++) {
                    surfMinLevels[i] = reader.ReadByte();
                    surfMaxLevels[i] = reader.ReadByte();
                    surfPokémon[i] = reader.ReadUInt16();
                }

                /* Rock Smash encounters */
                for (int i = 0; i < 2; i++) {
                    rockSmashMinLevels[i] = reader.ReadByte();
                    rockSmashMaxLevels[i] = reader.ReadByte();
                    rockSmashPokémon[i] = reader.ReadUInt16();
                }

                /* Old Rod encounters */
                for (int i = 0; i < 5; i++) {
                    oldRodMinLevels[i] = reader.ReadByte();
                    oldRodMaxLevels[i] = reader.ReadByte();
                    oldRodPokémon[i] = reader.ReadUInt16();
                }

                /* Good Rod encounters */
                for (int i = 0; i < 5; i++) {
                    goodRodMinLevels[i] = reader.ReadByte();
                    goodRodMaxLevels[i] = reader.ReadByte();
                    goodRodPokémon[i] = reader.ReadUInt16();
                }

                /* Super Rod encounters */
                for (int i = 0; i < 5; i++) {
                    superRodMinLevels[i] = reader.ReadByte();
                    superRodMaxLevels[i] = reader.ReadByte();
                    superRodPokémon[i] = reader.ReadUInt16();
                }

                /* Swarm encounters */
                swarmPokémon = new ushort[4];
                for (int i = 0; i < 4; i++) {
                    try {
                        swarmPokémon[i] = reader.ReadUInt16();
                    } catch (EndOfStreamException) {
                        error = true;
                        swarmPokémon[i] = 0x00;
                    } 
                }

                if (error) {
                    MessageBox.Show("The Swarm Encounters section of this Encounters File" +
                        "is partially corrupted.\n" + "Assuming a value of 0 to repair the " +
                        "unreadable fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
        #endregion

        #region Methods(1)
        public override byte[] SaveEncounterFile()
        {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData))
            {
                /* Encounter rates */
                writer.Write(walkingRate);
                writer.Write(surfRate);
                writer.Write(rockSmashRate);
                writer.Write(oldRodRate);
                writer.Write(goodRodRate);
                writer.Write(superRodRate);

                writer.BaseStream.Position += 0x2;

                /* Walking encounters levels */
                for (int i = 0; i < 12; i++) writer.Write(walkingLevels[i]);

                /* Morning walking encounters */
                for (int i = 0; i < 12; i++) writer.Write(morningPokémon[i]);

                /* Day walking encounters */
                for (int i = 0; i < 12; i++) writer.Write(dayPokémon[i]);

                /* Night walking encounters */
                for (int i = 0; i < 12; i++) writer.Write(nightPokémon[i]);

                /* PokéGear music encounters */
                for (int i = 0; i < 2; i++) writer.Write(hoennMusicPokémon[i]);
                for (int i = 0; i < 2; i++) writer.Write(sinnohMusicPokémon[i]);

                /* Surf encounters */
                for (int i = 0; i < 5; i++)
                {
                    writer.Write(surfMinLevels[i]);
                    writer.Write(surfMaxLevels[i]);
                    writer.Write(surfPokémon[i]);
                }

                /* Rock Smash encounters */
                for (int i = 0; i < 2; i++)
                {
                    writer.Write(rockSmashMinLevels[i]);
                    writer.Write(rockSmashMaxLevels[i]);
                    writer.Write(rockSmashPokémon[i]);
                }

                /* Old Rod encounters */
                for (int i = 0; i < 5; i++)
                {
                    writer.Write(oldRodMinLevels[i]);
                    writer.Write(oldRodMaxLevels[i]);
                    writer.Write(oldRodPokémon[i]);
                }

                /* Good Rod encounters */
                for (int i = 0; i < 5; i++)
                {
                    writer.Write(goodRodMinLevels[i]);
                    writer.Write(goodRodMaxLevels[i]);
                    writer.Write(goodRodPokémon[i]);
                }

                /* Super Rod encounters */
                for (int i = 0; i < 5; i++)
                {
                    writer.Write(superRodMinLevels[i]);
                    writer.Write(superRodMaxLevels[i]);
                    writer.Write(superRodPokémon[i]);
                }

                /* Swarm encounters */
                for (int i = 0; i < 4; i++) writer.Write(swarmPokémon[i]);
            }
            return newData.ToArray();
        }
        #endregion
    }
}