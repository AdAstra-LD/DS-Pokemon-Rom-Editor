using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using static DSPRE.RomInfo;

namespace DSPRE.ROMFiles {
    /* ---------------------- WILD POKÉMON DATA STRUCTURE (DPPt):----------------------------
        
       0x0  //  byte:       Walking encounter rate
       0x4  //  byte:       Level of
       0x2  //  ushort:     Matrix number
       0x4  //  ushort:     Script file number
       0x6  //  ushort:     Level script file number
       0x8  //  ushort:     Text Archive number
       0xA  //  ushort:     Day music track number
       0xC  //  ushort:     Night music track number
       0xE  //  ushort:     Wild Pokemon file number
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

    /* ---------------------- WILD POKÉMON DATA STRUCTURE (HGSS):----------------------------
        
       0x0  //  byte:       Wild Pokemon file number
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
       0x16 //  byte:       Follow mode (for the Pokemon following hero)
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
    /// General class to store common wild Pokemon data across all Gen IV Pokemon NDS games
    /// </summary>
    public abstract class EncounterFile : RomFile {
        public const string msgFixed = " (already fixed)";
        public const string extension = "wld";
        #region Fields (19)

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
        public ushort[] goodRodPokemon = new ushort[5];
        public ushort[] oldRodPokemon = new ushort[5];
        public ushort[] superRodPokemon = new ushort[5];
        public ushort[] surfPokemon = new ushort[5];
        public ushort[] swarmPokemon { get; set; }  //2 for DPPt, 4 for HGSS
        #endregion

        #region Methods (1)
        public void SaveToFileDefaultDir(int IDtoReplace, bool showSuccessMessage = true) {
            SaveToFileDefaultDir(DirNames.encounters, IDtoReplace, showSuccessMessage);
        }

        public void ReportErrors(List<string> errorList) {
            string fullError = "The following sections of this encounter file couldn't be read correctly: " + Environment.NewLine;

            string errorSections = "";
            foreach (string elem in errorList) {
                errorSections += "- " + elem + Environment.NewLine;
            }
            fullError += errorSections;

            fullError += Environment.NewLine + "It is recommended that you check them before resaving.";
            
            if (errorSections.Contains(msgFixed)) {
                fullError += Environment.NewLine + "Fields marked as " + '\'' + msgFixed + '\'' + " have been repaired with a value of 0.";
            }

            MessageBox.Show(fullError, "Encounter File error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        #endregion
    }

    /// <summary>
    /// Class to store wild Pokemon data from Pokemon Diamond, Pearl and Platinum
    /// </summary>
    public class EncounterFileDPPt : EncounterFile {
        #region Fields (9)       
        /* Field encounters */
        public uint[] radarPokemon = new uint[4];
        public uint[] walkingPokemon = new uint[12];

        /* Time-specific encounters */
        public uint[] dayPokemon = new uint[2];
        public uint[] nightPokemon = new uint[2];

        /* Dual slot exclusives */
        public uint[] rubyPokemon = new uint[2];
        public uint[] sapphirePokemon = new uint[2];
        public uint[] emeraldPokemon = new uint[2];
        public uint[] fireRedPokemon = new uint[2];
        public uint[] leafGreenPokemon = new uint[2];

        /* Form Data */
        public uint[] regionalForms = new uint[5];
        public uint unknownTable = 0;

        #endregion

        #region Constructors (1)
        public EncounterFileDPPt(Stream data) {
            using (BinaryReader reader = new BinaryReader(data)) {
                List<string> fieldsWithErrors = new List<string>();

                /* Walking encounters */
                try {
                    walkingRate = (byte)reader.ReadInt32();
                    for (int i = 0; i < 12; i++) {
                        walkingLevels[i] = (byte)reader.ReadUInt32();
                        walkingPokemon[i] = reader.ReadUInt32();
                    }
                } catch {
                    fieldsWithErrors.Add("Regular encounters");
                }

                /* Swarms */
                swarmPokemon = new ushort[2];
                for (int i = 0; i < 2; i++) {
                    try {
                        swarmPokemon[i] = (ushort)reader.ReadUInt32();
                    } catch (EndOfStreamException) {
                        swarmPokemon[i] = 0x00;
                        fieldsWithErrors.Add("Swarms" + ' ' + '[' + i + ']' + msgFixed);
                    }
                }

                /* Time-specific encounters */
                for (int i = 0; i < 2; i++) {
                    try {
                        dayPokemon[i] = reader.ReadUInt32();
                    } catch {
                        dayPokemon[i] = 0x00;
                        fieldsWithErrors.Add("Morning encounters" + ' ' + '[' + i + ']' + msgFixed);
                    }
                }

                for (int i = 0; i < 2; i++) {
                    try {
                        nightPokemon[i] = reader.ReadUInt32();
                    } catch {
                        nightPokemon[i] = 0x00;
                        fieldsWithErrors.Add("Night encounters" + ' ' + '[' + i + ']' + msgFixed);
                    }
                }

                /* Poké-Radar encounters */
                for (int i = 0; i < 4; i++) {
                    try {
                        radarPokemon[i] = reader.ReadUInt32();
                    } catch {
                        radarPokemon[i] = 0x00;
                        fieldsWithErrors.Add("PokéRadar" + ' ' + '[' + i + ']' + msgFixed);
                    }
                }

                /* Form data */
                for (int i = 0; i < 5; i++) {
                    try {
                        regionalForms[i] = reader.ReadUInt32();
                    } catch {
                        regionalForms[i] = 0x00;
                        fieldsWithErrors.Add("Form data" + ' ' + '[' + i + ']' + msgFixed);
                    }
                }

                try
                {
                    unknownTable = reader.ReadUInt32();
                }
                catch
                {
                    unknownTable = 0x00;
                    fieldsWithErrors.Add("Unknown table" + msgFixed);
                }

                /* Dual-slot encounters */
                for (int i = 0; i < 2; i++) {
                    try {
                        rubyPokemon[i] = reader.ReadUInt32();
                    } catch {
                        rubyPokemon[i] = 0x00;
                        fieldsWithErrors.Add("Dual-Slot Ruby" + ' ' + '[' + i + ']' + msgFixed);
                    }
                }

                for (int i = 0; i < 2; i++) {
                    try {
                        sapphirePokemon[i] = reader.ReadUInt32();
                    } catch {
                        sapphirePokemon[i] = 0x00;
                        fieldsWithErrors.Add("Dual-Slot Sapphire" + ' ' + '[' + i + ']' + msgFixed);
                    }
                }

                for (int i = 0; i < 2; i++) {
                    try {
                        emeraldPokemon[i] = reader.ReadUInt32();
                    } catch {
                        emeraldPokemon[i] = 0x00;
                        fieldsWithErrors.Add("Dual-Slot Emerald" + ' ' + '[' + i + ']' + msgFixed);
                    }
                }

                for (int i = 0; i < 2; i++) {
                    try {
                        fireRedPokemon[i] = reader.ReadUInt32();
                    } catch {
                        fireRedPokemon[i] = 0x00;
                        fieldsWithErrors.Add("Dual-Slot FireRed" + ' ' + '[' + i + ']' + msgFixed);
                    }
                }

                for (int i = 0; i < 2; i++) {
                    try {
                        leafGreenPokemon[i] = reader.ReadUInt32();
                    } catch {
                        leafGreenPokemon[i] = 0x00;
                        fieldsWithErrors.Add("Dual-Slot LeafGreen" + ' ' + '[' + i + ']' + msgFixed);
                    }
                }

                /* Surf encounters */
                try {
                    surfRate = (byte)reader.ReadInt32();
                    for (int i = 0; i < 5; i++) {
                        surfMaxLevels[i] = reader.ReadByte();
                        surfMinLevels[i] = reader.ReadByte();
                        reader.BaseStream.Position += 0x2;
                        surfPokemon[i] = (ushort)reader.ReadUInt32();
                    }
                } catch {
                    fieldsWithErrors.Add("Surf");
                }

                reader.BaseStream.Position = 0x124;

                /* Old Rod encounters */
                try {
                    oldRodRate = (byte)reader.ReadInt32();
                    for (int i = 0; i < 5; i++) {
                        oldRodMaxLevels[i] = reader.ReadByte();
                        oldRodMinLevels[i] = reader.ReadByte();

                        reader.BaseStream.Position += 0x2;
                        oldRodPokemon[i] = (ushort)reader.ReadUInt32();
                    }
                } catch {
                    fieldsWithErrors.Add("Old Rod");
                }

                /* Good Rod encounters */
                try {
                    goodRodRate = (byte)reader.ReadInt32();
                    for (int i = 0; i < 5; i++) {
                        goodRodMaxLevels[i] = reader.ReadByte();
                        goodRodMinLevels[i] = reader.ReadByte();

                        reader.BaseStream.Position += 0x2;
                        goodRodPokemon[i] = (ushort)reader.ReadUInt32();
                    }
                } catch {
                    fieldsWithErrors.Add("Good Rod");
                }

                /* Super Rod encounters */
                try {
                    superRodRate = (byte)reader.ReadInt32();
                    for (int i = 0; i < 5; i++) {
                        superRodMaxLevels[i] = reader.ReadByte();
                        superRodMinLevels[i] = reader.ReadByte();

                        reader.BaseStream.Position += 0x2;
                        superRodPokemon[i] = (ushort)reader.ReadUInt32();
                    }
                } catch {
                    fieldsWithErrors.Add("Super Rod");
                }

                if (fieldsWithErrors.Count > 0) {
                    ReportErrors(fieldsWithErrors);
                }
            }
        }

        public EncounterFileDPPt(int ID) : this(new FileStream(RomInfo.gameDirs[DirNames.encounters].unpackedDir + "\\" + ID.ToString("D4"), FileMode.Open)) { }
        public EncounterFileDPPt() {
            swarmPokemon = new ushort[2];
        }
        #endregion Constructors

        #region Methods (1)
        public override byte[] ToByteArray() {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData)) {
                writer.Write((uint)walkingRate);

                /* Walking encounters */
                for (int i = 0; i < 12; i++) {
                    writer.Write((uint)walkingLevels[i]);
                    writer.Write(walkingPokemon[i]);
                }

                /* Swarms */
                for (int i = 0; i < 2; i++) {
                    writer.Write((uint)swarmPokemon[i]);
                }

                /* Time-specific encounters */
                for (int i = 0; i < 2; i++) {
                    writer.Write(dayPokemon[i]);
                }

                for (int i = 0; i < 2; i++) {
                    writer.Write(nightPokemon[i]);
                }

                /* Poké-Radar encounters */
                for (int i = 0; i < 4; i++) {
                    writer.Write(radarPokemon[i]);
                }

                writer.BaseStream.Position = 0xA4;

                /* Dual-slot encounters */
                for (int i = 0; i < 2; i++) {
                    writer.Write(rubyPokemon[i]);
                }

                for (int i = 0; i < 2; i++) {
                    writer.Write(sapphirePokemon[i]);
                }

                for (int i = 0; i < 2; i++) {
                    writer.Write(emeraldPokemon[i]);
                }

                for (int i = 0; i < 2; i++) {
                    writer.Write(fireRedPokemon[i]);
                }

                for (int i = 0; i < 2; i++) {
                    writer.Write(leafGreenPokemon[i]);
                }

                /* Surf encounters */
                writer.Write((uint)surfRate);
                for (int i = 0; i < 5; i++) {
                    writer.Write(surfMaxLevels[i]);
                    writer.Write(surfMinLevels[i]);
                    writer.BaseStream.Position += 0x2;
                    writer.Write((uint)surfPokemon[i]);
                }

                writer.BaseStream.Position = 0x124;

                /* Old Rod encounters */
                writer.Write((uint)oldRodRate);
                for (int i = 0; i < 5; i++) {
                    writer.Write(oldRodMaxLevels[i]);
                    writer.Write(oldRodMinLevels[i]);
                    writer.BaseStream.Position += 0x2;
                    writer.Write((uint)oldRodPokemon[i]);
                }

                /* Good Rod encounters */
                writer.Write((uint)goodRodRate);
                for (int i = 0; i < 5; i++) {
                    writer.Write(goodRodMaxLevels[i]);
                    writer.Write(goodRodMinLevels[i]);
                    writer.BaseStream.Position += 0x2;
                    writer.Write((uint)goodRodPokemon[i]);
                }

                /* Super Rod encounters */
                writer.Write((uint)superRodRate);
                for (int i = 0; i < 5; i++) {
                    writer.Write(superRodMaxLevels[i]);
                    writer.Write(superRodMinLevels[i]);
                    writer.BaseStream.Position += 0x2;
                    writer.Write((uint)superRodPokemon[i]);
                }
            }
            return newData.ToArray();
        }
        public void SaveToFileExplorePath(string suggestedFileName, bool showSuccessMessage = true) {
            SaveToFileExplorePath("DPPt Encounter File", EncounterFile.extension, suggestedFileName, showSuccessMessage);
        }
        #endregion
    }

    /// <summary>
    /// Class to store wild Pokemon data from Pokemon HeartGold and SoulSilver
    /// </summary>
    public class EncounterFileHGSS : EncounterFile {
        #region Fields (9)
        public byte rockSmashRate;
        public ushort[] morningPokemon = new ushort[12];
        public ushort[] dayPokemon = new ushort[12];
        public ushort[] nightPokemon = new ushort[12];
        public ushort[] hoennMusicPokemon = new ushort[2];
        public ushort[] sinnohMusicPokemon = new ushort[2];
        public ushort[] rockSmashPokemon = new ushort[2];
        public byte[] rockSmashMinLevels = new byte[2];
        public byte[] rockSmashMaxLevels = new byte[2];
        #endregion

        #region Constructors
        public EncounterFileHGSS(Stream data) {
            using (BinaryReader reader = new BinaryReader(data)) {
                List<string> fieldsWithErrors = new List<string>();

                /* Encounter rates */
                try {
                    walkingRate = reader.ReadByte();
                } catch {
                    walkingRate = 0x00;
                    fieldsWithErrors.Add("Regular Encounters rate" + msgFixed);
                }

                try {
                    surfRate = reader.ReadByte();
                } catch {
                    surfRate = 0x00;
                    fieldsWithErrors.Add("Surf rate" + msgFixed);
                }

                try { 
                    rockSmashRate = reader.ReadByte();
                } catch {
                    rockSmashRate = 0x00;
                    fieldsWithErrors.Add("Rock Smash rate" + msgFixed);
                }

                try { 
                    oldRodRate = reader.ReadByte();
                } catch {
                    oldRodRate = 0x00;
                    fieldsWithErrors.Add("Old Rod rate" + msgFixed);
                }

                try { 
                    goodRodRate = reader.ReadByte();
                } catch {
                    goodRodRate = 0x00;
                    fieldsWithErrors.Add("Good Rod rate" + msgFixed);
                }

                try { 
                    superRodRate = reader.ReadByte();
                } catch {
                    superRodRate = 0x00;
                    fieldsWithErrors.Add("Super Rod rate" + msgFixed);
                }

                reader.BaseStream.Position += 0x2;

                /* Walking encounters levels */
                for (int i = 0; i < 12; i++) {
                    try { 
                        walkingLevels[i] = reader.ReadByte();
                    } catch {
                        walkingLevels[i] = 0x00;
                        fieldsWithErrors.Add("Regular Encounters" + ' ' + '[' + i + ']' + msgFixed);
                    }
                }

                /* Morning walking encounters */
                for (int i = 0; i < 12; i++) {
                    try {
                        morningPokemon[i] = reader.ReadUInt16();
                    } catch {
                        morningPokemon[i] = 0x00;
                        fieldsWithErrors.Add("Morning Encounters" + ' ' + '[' + i + ']' + msgFixed);
                    }
                }

                /* Day walking encounters */
                for (int i = 0; i < 12; i++) {
                    try { 
                        dayPokemon[i] = reader.ReadUInt16();
                    } catch {
                        dayPokemon[i] = 0x00;
                        fieldsWithErrors.Add("Day Encounters" + ' ' + '[' + i + ']' + msgFixed);
                    }
                }

                /* Night walking encounters */
                for (int i = 0; i < 12; i++) {
                    try { 
                        nightPokemon[i] = reader.ReadUInt16();
                    } catch {
                        nightPokemon[i] = 0x00;
                        fieldsWithErrors.Add("Night Encounters" + ' ' + '[' + i + ']' + msgFixed);
                    }
                }

                /* PokéGear music encounters */
                for (int i = 0; i < 2; i++) {
                    try {
                        hoennMusicPokemon[i] = reader.ReadUInt16();
                    } catch {
                        hoennMusicPokemon[i] = 0x00;
                        fieldsWithErrors.Add("Hoenn Music Encounters" + ' ' + '[' + i + ']' + msgFixed);
                    }
                }

                for (int i = 0; i < 2; i++) {
                    try {  
                        sinnohMusicPokemon[i] = reader.ReadUInt16();
                    } catch {
                        sinnohMusicPokemon[i] = 0x00;
                        fieldsWithErrors.Add("Sinnoh Music Encounters" + ' ' + '[' + i + ']' + msgFixed);
                    }
                }

                /* Surf encounters */
                for (int i = 0; i < 5; i++) {
                    try {
                        surfMinLevels[i] = reader.ReadByte();
                    } catch {
                        surfMinLevels[i] = 0x01;
                        fieldsWithErrors.Add("Surf Encounters" + ' ' + '[' + i + ']' + " min. level" + msgFixed);
                    }

                    try {
                        surfMaxLevels[i] = reader.ReadByte();
                    } catch {
                        surfMaxLevels[i] = 0x01;
                        fieldsWithErrors.Add("Surf Encounters" + ' ' + '[' + i + ']' + " max. level" + msgFixed);
                    }

                    try {
                        surfPokemon[i] = reader.ReadUInt16();
                    } catch {
                        surfMinLevels[i] = 0x00;
                        fieldsWithErrors.Add("Surf Encounters" + ' ' + '[' + i + ']' + " Pokémon" + msgFixed);
                    }
                }

                /* Rock Smash encounters */
                for (int i = 0; i < 2; i++) {
                    try {
                        rockSmashMinLevels[i] = reader.ReadByte();
                    } catch {
                        rockSmashMinLevels[i] = 0x01;
                        fieldsWithErrors.Add("Rock Smash Encounters" + ' ' + '[' + i + ']' + " min. level" + msgFixed);
                    }

                    try {
                        rockSmashMaxLevels[i] = reader.ReadByte();
                    } catch {
                        rockSmashMaxLevels[i] = 0x01;
                        fieldsWithErrors.Add("Rock Smash Encounters" + ' ' + '[' + i + ']' + " max. level" + msgFixed);
                    }

                    try {
                        rockSmashPokemon[i] = reader.ReadUInt16();
                    } catch {
                        rockSmashPokemon[i] = 0x00;
                        fieldsWithErrors.Add("Rock Smash Encounters" + ' ' + '[' + i + ']' + " Pokémon" + msgFixed);
                    }
                }

                /* Old Rod encounters */
                for (int i = 0; i < 5; i++) {
                    try {
                        oldRodMinLevels[i] = reader.ReadByte();
                    } catch {
                        oldRodMinLevels[i] = 0x01;
                        fieldsWithErrors.Add("Old Rod Encounters" + ' ' + '[' + i + ']' + " min. level" + msgFixed);
                    }

                    try {
                        oldRodMaxLevels[i] = reader.ReadByte();
                    } catch {
                        oldRodMaxLevels[i] = 0x01;
                        fieldsWithErrors.Add("Old Rod Encounters" + ' ' + '[' + i + ']' + " max. level" + msgFixed);
                    }

                    try {
                        oldRodPokemon[i] = reader.ReadUInt16();
                    } catch {
                        oldRodPokemon[i] = 0x00;
                        fieldsWithErrors.Add("Old Rod Encounters" + ' ' + '[' + i + ']' + " Pokémon" + msgFixed);
                    }
                }

                /* Good Rod encounters */
                for (int i = 0; i < 5; i++) {
                    try {
                        goodRodMinLevels[i] = reader.ReadByte();
                    } catch {
                        goodRodMinLevels[i] = 0x01;
                        fieldsWithErrors.Add("Good Rod Encounters" + ' ' + '[' + i + ']' + " min. level" + msgFixed);
                    }

                    try {
                        goodRodMaxLevels[i] = reader.ReadByte();
                    } catch {
                        goodRodMaxLevels[i] = 0x01;
                        fieldsWithErrors.Add("Good Rod Encounters" + ' ' + '[' + i + ']' + " max. level" + msgFixed);
                    }

                    try {
                        goodRodPokemon[i] = reader.ReadUInt16();
                    } catch {
                        goodRodPokemon[i] = 0x00;
                        fieldsWithErrors.Add("Good Rod Encounters" + ' ' + '[' + i + ']' + " Pokémon" + msgFixed);
                    }
                }

                /* Super Rod encounters */
                for (int i = 0; i < 5; i++) {
                    try {
                        superRodMinLevels[i] = reader.ReadByte();
                    } catch {
                        superRodMinLevels[i] = 0x01;
                        fieldsWithErrors.Add("Super Rod Encounters" + ' ' + '[' + i + ']' + " min. level" + msgFixed);
                    }

                    try {
                        superRodMaxLevels[i] = reader.ReadByte();
                    } catch {
                        superRodMaxLevels[i] = 0x01;
                        fieldsWithErrors.Add("Super Rod Encounters" + ' ' + '[' + i + ']' + " max. level" + msgFixed);
                    }

                    try {
                        superRodPokemon[i] = reader.ReadUInt16();
                    } catch {
                        superRodPokemon[i] = 0x00;
                        fieldsWithErrors.Add("Super Rod Encounters" + ' ' + '[' + i + ']' + " Pokémon" + msgFixed);
                    }
                }

                /* Swarm encounters */
                swarmPokemon = new ushort[4];
                for (int i = 0; i < 4; i++) {
                    try {
                        swarmPokemon[i] = reader.ReadUInt16();
                    } catch (EndOfStreamException) {
                        swarmPokemon[i] = 0x00;
                        fieldsWithErrors.Add("Swarms" + '[' + i + ']' + msgFixed);
                    }
                }

                if (fieldsWithErrors.Count > 0) {
                    ReportErrors(fieldsWithErrors);
                }
            }
        }
        public EncounterFileHGSS(int ID) : this(new FileStream(RomInfo.gameDirs[DirNames.encounters].unpackedDir + "\\" + ID.ToString("D4"), FileMode.Open)) { }
        public EncounterFileHGSS() {
            swarmPokemon = new ushort[4];
        }
        #endregion

        #region Methods(1)
        public override byte[] ToByteArray() {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData)) {
                /* Encounter rates */
                writer.Write(walkingRate);
                writer.Write(surfRate);
                writer.Write(rockSmashRate);
                writer.Write(oldRodRate);
                writer.Write(goodRodRate);
                writer.Write(superRodRate);

                writer.BaseStream.Position += 0x2;

                /* Walking encounters levels */
                for (int i = 0; i < 12; i++) {
                    writer.Write(walkingLevels[i]);
                }

                /* Morning walking encounters */
                for (int i = 0; i < 12; i++) { 
                    writer.Write(morningPokemon[i]);
                }

                /* Day walking encounters */
                for (int i = 0; i < 12; i++) {
                    writer.Write(dayPokemon[i]);
                }

                /* Night walking encounters */
                for (int i = 0; i < 12; i++) {
                    writer.Write(nightPokemon[i]);
                }

                /* PokéGear music encounters */
                for (int i = 0; i < 2; i++) {
                    writer.Write(hoennMusicPokemon[i]);
                }

                for (int i = 0; i < 2; i++) {
                    writer.Write(sinnohMusicPokemon[i]);
                }

                /* Surf encounters */
                for (int i = 0; i < 5; i++) {
                    writer.Write(surfMinLevels[i]);
                    writer.Write(surfMaxLevels[i]);
                    writer.Write(surfPokemon[i]);
                }

                /* Rock Smash encounters */
                for (int i = 0; i < 2; i++) {
                    writer.Write(rockSmashMinLevels[i]);
                    writer.Write(rockSmashMaxLevels[i]);
                    writer.Write(rockSmashPokemon[i]);
                }

                /* Old Rod encounters */
                for (int i = 0; i < 5; i++) {
                    writer.Write(oldRodMinLevels[i]);
                    writer.Write(oldRodMaxLevels[i]);
                    writer.Write(oldRodPokemon[i]);
                }

                /* Good Rod encounters */
                for (int i = 0; i < 5; i++) {
                    writer.Write(goodRodMinLevels[i]);
                    writer.Write(goodRodMaxLevels[i]);
                    writer.Write(goodRodPokemon[i]);
                }

                /* Super Rod encounters */
                for (int i = 0; i < 5; i++) {
                    writer.Write(superRodMinLevels[i]);
                    writer.Write(superRodMaxLevels[i]);
                    writer.Write(superRodPokemon[i]);
                }

                /* Swarm encounters */
                for (int i = 0; i < 4; i++) {
                    writer.Write(swarmPokemon[i]);
                }
            }
            return newData.ToArray();
        }

        public void SaveToFileExplorePath(string suggestedFileName, bool showSuccessMessage = true) {
            SaveToFileExplorePath("HGSS Encounter File", EncounterFile.extension, suggestedFileName, showSuccessMessage);
        }
        #endregion
    }
}