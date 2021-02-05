using System.IO;
using System.Windows.Forms;

namespace DSPRE.ROMFiles {
    /* ---------------------- WILD POK�MON DATA STRUCTURE (DPPt):----------------------------
        
       0x0  //  byte:       Walking encounter rate
       0x4  //  byte:       Level of
       0x2  //  ushort:     Matrix number
       0x4  //  ushort:     Script file number
       0x6  //  ushort:     Level script file number
       0x8  //  ushort:     Text Archive number
       0xA  //  ushort:     Day music track number
       0xC  //  ushort:     Night music track number
       0xE  //  ushort:     Wild Pok�mon file number
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

    /* ---------------------- WILD POK�MON DATA STRUCTURE (HGSS):----------------------------
        
       0x0  //  byte:       Wild Pok�mon file number
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
       0x16 //  byte:       Follow mode (for the Pok�mon following hero)
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
    /// General class to store common wild Pok�mon data across all Gen IV Pok�mon NDS games
    /// </summary>
    public abstract class EncounterFile
	{
        #region�Fields�(19)
        
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
        public ushort[] goodRodPok�mon = new ushort[5];
        public ushort[] oldRodPok�mon = new ushort[5];
        public ushort[] superRodPok�mon = new ushort[5];
        public ushort[] surfPok�mon = new ushort[5];
        public ushort[] swarmPok�mon { get; set; }
        #endregion

        #region Methods (1)
        public abstract byte[] SaveEncounterFile();
        #endregion
    }

    /// <summary>
    /// Class to store wild Pok�mon data from Pok�mon Diamond, Pearl and Platinum
    /// </summary>
    public class EncounterFileDPPt: EncounterFile
    {
        #region�Fields�(9)       
        /* Field encounters */
        public uint[] radarPok�mon = new uint[4];
        public uint[] walkingPok�mon = new uint[12];

        /* Time-specific encounters */
        public uint[] morningPok�mon = new uint[2];
        public uint[] nightPok�mon = new uint[2];

        /* Dual slot exclusives */
        public uint[] rubyPok�mon = new uint[2];
        public uint[] sapphirePok�mon = new uint[2];
        public uint[] emeraldPok�mon = new uint[2];
        public uint[] fireRedPok�mon = new uint[2];
        public uint[] leafGreenPok�mon = new uint[2];
        #endregion

        #region�Constructors�(1)
        public EncounterFileDPPt(Stream data)
        {
            using (BinaryReader reader = new BinaryReader(data))
            {
                /* Walking encounters */
                walkingRate = (byte)reader.ReadInt32();
                for (int i = 0; i < 12; i++)
                {
                    walkingLevels[i] = (byte)reader.ReadUInt32();
                    walkingPok�mon[i] = reader.ReadUInt32();
                }

                /* Swarms */
                swarmPok�mon = new ushort[2];
                for (int i = 0; i < 2; i++) swarmPok�mon[i] = (ushort)reader.ReadUInt32();
                
                /* Time-specific encounters */
                for (int i = 0; i < 2; i++) morningPok�mon[i] = reader.ReadUInt32();
                for (int i = 0; i < 2; i++) nightPok�mon[i] = reader.ReadUInt32();

                /* Pok�-Radar encounters */
                for (int i = 0; i < 4; i++) radarPok�mon[i] = reader.ReadUInt32();

                reader.BaseStream.Position = 0xA4;

                /* Dual-slot encounters */
                for (int i = 0; i < 2; i++) rubyPok�mon[i] = reader.ReadUInt32();
                for (int i = 0; i < 2; i++) sapphirePok�mon[i] = reader.ReadUInt32();
                for (int i = 0; i < 2; i++) emeraldPok�mon[i] = reader.ReadUInt32();
                for (int i = 0; i < 2; i++) fireRedPok�mon[i] = reader.ReadUInt32();
                for (int i = 0; i < 2; i++) leafGreenPok�mon[i] = reader.ReadUInt32();

                /* Surf encounters */
                surfRate = (byte)reader.ReadInt32();
                for (int i = 0; i < 5; i++)
                {
                    surfMaxLevels[i] = reader.ReadByte();
                    surfMinLevels[i] = reader.ReadByte();
                    reader.BaseStream.Position += 0x2;
                    surfPok�mon[i] = (ushort)reader.ReadUInt32();
                }

                reader.BaseStream.Position = 0x124;

                /* Old Rod encounters */
                oldRodRate = (byte)reader.ReadInt32();
                for (int i = 0; i < 5; i++)
                {
                    oldRodMaxLevels[i] = reader.ReadByte();
                    oldRodMinLevels[i] = reader.ReadByte();
                    reader.BaseStream.Position += 0x2;
                    oldRodPok�mon[i] = (ushort)reader.ReadUInt32();
                }

                /* Good Rod encounters */
                goodRodRate = (byte)reader.ReadInt32();
                for (int i = 0; i < 5; i++)
                {
                    goodRodMaxLevels[i] = reader.ReadByte();
                    goodRodMinLevels[i] = reader.ReadByte();
                    reader.BaseStream.Position += 0x2;
                    goodRodPok�mon[i] = (ushort)reader.ReadUInt32();
                }

                /* Super Rod encounters */
                superRodRate = (byte)reader.ReadInt32();
                for (int i = 0; i < 5; i++)
                {
                    superRodMaxLevels[i] = reader.ReadByte();
                    superRodMinLevels[i] = reader.ReadByte();
                    reader.BaseStream.Position += 0x2;
                    superRodPok�mon[i] = (ushort)reader.ReadUInt32();
                }
            }
        }
        #endregion�Constructors

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
                    writer.Write(walkingPok�mon[i]);
                }

                /* Swarms */
                for (int i = 0; i < 2; i++) writer.Write((uint)swarmPok�mon[i]);

                /* Time-specific encounters */
                for (int i = 0; i < 2; i++) writer.Write(morningPok�mon[i]);
                for (int i = 0; i < 2; i++) writer.Write(nightPok�mon[i]);

                /* Pok�-Radar encounters */
                for (int i = 0; i < 4; i++) writer.Write(radarPok�mon[i]);

                writer.BaseStream.Position = 0xA4;

                /* Dual-slot encounters */
                for (int i = 0; i < 2; i++) writer.Write(rubyPok�mon[i]);
                for (int i = 0; i < 2; i++) writer.Write(sapphirePok�mon[i]);
                for (int i = 0; i < 2; i++) writer.Write(emeraldPok�mon[i]);
                for (int i = 0; i < 2; i++) writer.Write(fireRedPok�mon[i]);
                for (int i = 0; i < 2; i++) writer.Write(leafGreenPok�mon[i]);

                /* Surf encounters */
                writer.Write((uint)surfRate);
                for (int i = 0; i < 5; i++)
                {
                    writer.Write(surfMaxLevels[i]);
                    writer.Write(surfMinLevels[i]);
                    writer.BaseStream.Position += 0x2;
                    writer.Write((uint)surfPok�mon[i]);
                }

                writer.BaseStream.Position = 0x124;

                /* Old Rod encounters */
                writer.Write((uint)oldRodRate);
                for (int i = 0; i < 5; i++)
                {
                    writer.Write(oldRodMaxLevels[i]);
                    writer.Write(oldRodMinLevels[i]);
                    writer.BaseStream.Position += 0x2;
                    writer.Write((uint)oldRodPok�mon[i]);
                }

                /* Good Rod encounters */
                writer.Write((uint)goodRodRate);
                for (int i = 0; i < 5; i++)
                {
                    writer.Write(goodRodMaxLevels[i]);
                    writer.Write(goodRodMinLevels[i]);
                    writer.BaseStream.Position += 0x2;
                    writer.Write((uint)goodRodPok�mon[i]);
                }

                /* Super Rod encounters */
                writer.Write((uint)superRodRate);
                for (int i = 0; i < 5; i++)
                {
                    writer.Write(superRodMaxLevels[i]);
                    writer.Write(superRodMinLevels[i]);
                    writer.BaseStream.Position += 0x2;
                    writer.Write((uint)superRodPok�mon[i]);
                }
            }
            return newData.ToArray();
        }
        #endregion
    }
   
    /// <summary>
    /// Class to store wild Pok�mon data from Pok�mon HeartGold and SoulSilver
    /// </summary>
    public class EncounterFileHGSS : EncounterFile
    {
        #region�Fields�(9)
        public byte rockSmashRate;
        public ushort[] morningPok�mon = new ushort[12];
        public ushort[] dayPok�mon = new ushort[12];
        public ushort[] nightPok�mon = new ushort[12];
        public ushort[] hoennMusicPok�mon = new ushort[2];
        public ushort[] sinnohMusicPok�mon = new ushort[2];
        public ushort[] rockSmashPok�mon = new ushort[2];
        public byte[] rockSmashMinLevels = new byte[2];
        public byte[] rockSmashMaxLevels = new byte[2];
        #endregion

        #region�Constructors�(1)
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
                    morningPok�mon[i] = reader.ReadUInt16();

                /* Day walking encounters */
                for (int i = 0; i < 12; i++) 
                    dayPok�mon[i] = reader.ReadUInt16();

                /* Night walking encounters */
                for (int i = 0; i < 12; i++) 
                    nightPok�mon[i] = reader.ReadUInt16();

                /* Pok�Gear music encounters */
                for (int i = 0; i < 2; i++) 
                    hoennMusicPok�mon[i] = reader.ReadUInt16();
                for (int i = 0; i < 2; i++) 
                    sinnohMusicPok�mon[i] = reader.ReadUInt16();

                /* Surf encounters */
                for (int i = 0; i < 5; i++) {
                    surfMinLevels[i] = reader.ReadByte();
                    surfMaxLevels[i] = reader.ReadByte();
                    surfPok�mon[i] = reader.ReadUInt16();
                }

                /* Rock Smash encounters */
                for (int i = 0; i < 2; i++) {
                    rockSmashMinLevels[i] = reader.ReadByte();
                    rockSmashMaxLevels[i] = reader.ReadByte();
                    rockSmashPok�mon[i] = reader.ReadUInt16();
                }

                /* Old Rod encounters */
                for (int i = 0; i < 5; i++) {
                    oldRodMinLevels[i] = reader.ReadByte();
                    oldRodMaxLevels[i] = reader.ReadByte();
                    oldRodPok�mon[i] = reader.ReadUInt16();
                }

                /* Good Rod encounters */
                for (int i = 0; i < 5; i++) {
                    goodRodMinLevels[i] = reader.ReadByte();
                    goodRodMaxLevels[i] = reader.ReadByte();
                    goodRodPok�mon[i] = reader.ReadUInt16();
                }

                /* Super Rod encounters */
                for (int i = 0; i < 5; i++) {
                    superRodMinLevels[i] = reader.ReadByte();
                    superRodMaxLevels[i] = reader.ReadByte();
                    superRodPok�mon[i] = reader.ReadUInt16();
                }

                /* Swarm encounters */
                swarmPok�mon = new ushort[4];
                for (int i = 0; i < 4; i++) {
                    try {
                        swarmPok�mon[i] = reader.ReadUInt16();
                    } catch (EndOfStreamException) {
                        error = true;
                        swarmPok�mon[i] = 0x00;
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
                for (int i = 0; i < 12; i++) writer.Write(morningPok�mon[i]);

                /* Day walking encounters */
                for (int i = 0; i < 12; i++) writer.Write(dayPok�mon[i]);

                /* Night walking encounters */
                for (int i = 0; i < 12; i++) writer.Write(nightPok�mon[i]);

                /* Pok�Gear music encounters */
                for (int i = 0; i < 2; i++) writer.Write(hoennMusicPok�mon[i]);
                for (int i = 0; i < 2; i++) writer.Write(sinnohMusicPok�mon[i]);

                /* Surf encounters */
                for (int i = 0; i < 5; i++)
                {
                    writer.Write(surfMinLevels[i]);
                    writer.Write(surfMaxLevels[i]);
                    writer.Write(surfPok�mon[i]);
                }

                /* Rock Smash encounters */
                for (int i = 0; i < 2; i++)
                {
                    writer.Write(rockSmashMinLevels[i]);
                    writer.Write(rockSmashMaxLevels[i]);
                    writer.Write(rockSmashPok�mon[i]);
                }

                /* Old Rod encounters */
                for (int i = 0; i < 5; i++)
                {
                    writer.Write(oldRodMinLevels[i]);
                    writer.Write(oldRodMaxLevels[i]);
                    writer.Write(oldRodPok�mon[i]);
                }

                /* Good Rod encounters */
                for (int i = 0; i < 5; i++)
                {
                    writer.Write(goodRodMinLevels[i]);
                    writer.Write(goodRodMaxLevels[i]);
                    writer.Write(goodRodPok�mon[i]);
                }

                /* Super Rod encounters */
                for (int i = 0; i < 5; i++)
                {
                    writer.Write(superRodMinLevels[i]);
                    writer.Write(superRodMaxLevels[i]);
                    writer.Write(superRodPok�mon[i]);
                }

                /* Swarm encounters */
                for (int i = 0; i < 4; i++) writer.Write(swarmPok�mon[i]);
            }
            return newData.ToArray();
        }
        #endregion
    }
}