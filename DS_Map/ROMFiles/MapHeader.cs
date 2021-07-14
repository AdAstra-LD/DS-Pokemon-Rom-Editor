using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace DSPRE.ROMFiles {
    /* ---------------------- HEADER DATA STRUCTURE (DPPt):----------------------------
        
       0x0  //  byte:       Area data value
       0x1  //  byte:       Unknown value
       0x2  //  ushort:     Matrix number
       0x4  //  ushort:     Script file number
       0x6  //  ushort:     Level script file number
       0x8  //  ushort:     Text Archive number
       0xA  //  ushort:     Day music track number
       0xC  //  ushort:     Night music track number
       0xE  //  ushort:     Wild Pokémon file number
       0x10 //  ushort:     Event file number

       * D/P:
       0x12 //  ushort:     Index of map name in Text Archive #382 (US version)   
       
       * Plat:
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

    /* ---------------------- HEADER DATA STRUCTURE (HGSS):----------------------------
        
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

        DPPT
       -----------------    1: Allow Fly
       -----------------    2: Allow Esc. Rope
       -----------------    3: Allow Running
       -----------------    4: Allow Bike 
       -----------------    5: Battle BG b4
       -----------------    6: Battle BG b3
       -----------------    7: Battle BG b2
       -----------------    8: Battle BG b1

        HGSS
       -----------------    1: ?
       -----------------    2: ?
       -----------------    3: ?
       -----------------    4: Allow Fly 
       -----------------    5: Allow Esc. Rope
       -----------------    6: ?
       -----------------    7: Allow Bicycle
       -----------------    8: ?

    ----------------------------------------------------------------------------------*/

    /// <summary>
    /// General class to store common map header data across all Gen IV Pokémon NDS games
    /// </summary>
    public abstract class MapHeader : RomFile {
        /*System*/
        public ushort ID { get; set; }
        public static readonly byte length = 24;
        public static readonly string nameSeparator = " -   ";

        public enum SearchableFields: byte {
            AreaDataID,
            CameraAngleID,
            EventFileID,
            InternalName,
            LevelScriptID,
            MatrixID,
            MusicDayID,
            //MusicDayName,
            MusicNightID,
            //MusicNightName,
            ScriptFileID,
            TextArchiveID,
            WeatherID,
        };
        /**/


        #region Fields (10)
        public byte areaDataID { get; set; }
        public byte cameraAngleID { get; set; }
        public ushort eventFileID { get; set; }
        public ushort levelScriptID { get; set; }
        public ushort matrixID { get; set; }
        public ushort scriptFileID { get; set; }
        public ushort musicDayID { get; set; }
        public ushort musicNightID { get; set; }
        public byte locationSpecifier { get; set; }
        public byte battleBackground { get; set; }
        public ushort textArchiveID { get; set; }
        public byte weatherID { get; set; }
        public byte flags { get; set; }
        public ushort wildPokémon { get; set; }
        #endregion Fields

        #region Methods (1)
        public static MapHeader LoadFromByteArray(byte[] headerData, ushort headerNumber) {
            /* Encapsulate header data into the class appropriate for the gameVersion */
            if (headerData.Length < MapHeader.length) {
                MessageBox.Show("File of header " + headerNumber + " is too small and can't store header data.", "Header file too small", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            switch (RomInfo.gameFamily) {
                case "DP":
                    return new HeaderDP(headerNumber, new MemoryStream(headerData));
                case "Plat":
                    return new HeaderPt(headerNumber, new MemoryStream(headerData));
                default:
                    return new HeaderHGSS(headerNumber, new MemoryStream(headerData));
            }
        }
        public static MapHeader LoadFromFile(string filename, ushort headerNumber, long offsetInFile) {
            /* Calculate header offset and load data */
            byte[] headerData = DSUtils.ReadFromFile(filename, offsetInFile, MapHeader.length);
            return LoadFromByteArray(headerData, headerNumber);
        }
        public static MapHeader LoadFromARM9(ushort headerNumber) {
            long headerOffset = Resources.PokeDatabase.System.headerOffsetsDict[RomInfo.romID] + MapHeader.length * headerNumber;
            return LoadFromFile(RomInfo.arm9Path, headerNumber, headerOffset);
        }

        
        #endregion
    }

    /// <summary>
    /// Class to store map header data from Pokémon D and P
    /// </summary>
    public class HeaderDP : MapHeader {
        #region Fields (5)
        public byte unknown1 { get; set; }
        public ushort locationName { get; set; }
        #endregion Fields

        #region Constructors (1)
        public HeaderDP(ushort headerNumber, Stream data) {
            this.ID = headerNumber;
            using (BinaryReader reader = new BinaryReader(data)) {
                areaDataID = reader.ReadByte();
                unknown1 = reader.ReadByte();
                matrixID = reader.ReadUInt16();
                scriptFileID = reader.ReadUInt16();
                levelScriptID = reader.ReadUInt16();
                textArchiveID = reader.ReadUInt16();
                musicDayID = reader.ReadUInt16();
                musicNightID = reader.ReadUInt16();
                wildPokémon = reader.ReadUInt16();
                eventFileID = reader.ReadUInt16();
                locationName = reader.ReadUInt16();
                weatherID = reader.ReadByte();
                cameraAngleID = reader.ReadByte();
                locationSpecifier = reader.ReadByte();

                byte mapSettings = reader.ReadByte();
                battleBackground = (byte)(mapSettings & 0b_1111);
                flags = (byte)(mapSettings >> 4 & 0b_1111);
            }
        }
        #endregion Constructors

        #region Methods (1)
        public override byte[] ToByteArray() {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData)) {
                writer.Write(areaDataID);
                writer.Write(unknown1);
                writer.Write(matrixID);
                writer.Write(scriptFileID);
                writer.Write(levelScriptID);
                writer.Write(textArchiveID);
                writer.Write(musicDayID);
                writer.Write(musicNightID);
                writer.Write(wildPokémon);
                writer.Write(eventFileID);
                writer.Write(locationName);
                writer.Write(weatherID);
                writer.Write(cameraAngleID);
                writer.Write(locationSpecifier);

                byte mapSettings = (byte)((battleBackground & 0b_1111) + ((flags & 0b_1111) << 4));
                writer.Write(mapSettings);
            }
            return newData.ToArray();
        }
        #endregion
    }

    /// <summary>
    /// Class to store map header data from Pokémon Plat
    /// </summary>
    public class HeaderPt : MapHeader {
        #region Fields (5)
        public byte areaIcon { get; set; }
        public byte locationName { get; set; }
        public byte unknown1 { get; set; }
        #endregion Fields

        #region Constructors (1)
        public HeaderPt(ushort headerNumber, Stream data) {
            this.ID = headerNumber;
            using (BinaryReader reader = new BinaryReader(data)) {
                try {
                    areaDataID = reader.ReadByte();
                    unknown1 = reader.ReadByte();
                    matrixID = reader.ReadUInt16();
                    scriptFileID = reader.ReadUInt16();
                    levelScriptID = reader.ReadUInt16();
                    textArchiveID = reader.ReadUInt16();
                    musicDayID = reader.ReadUInt16();
                    musicNightID = reader.ReadUInt16();
                    wildPokémon = reader.ReadUInt16();
                    eventFileID = reader.ReadUInt16();
                    locationName = reader.ReadByte();
                    areaIcon = reader.ReadByte();
                    weatherID = reader.ReadByte();
                    cameraAngleID = reader.ReadByte();

                    ushort mapSettings = reader.ReadUInt16();
                    locationSpecifier = (byte)(mapSettings & 0b_1111_111);
                    battleBackground = (byte)(mapSettings >> 7 & 0b_1111_1);
                    flags = (byte)(mapSettings >> 12 & 0b_1111);

                } catch (EndOfStreamException) {
                    MessageBox.Show("Error loading header " + ID + '.', "Unexpected EOF", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion Constructors

        #region Methods(1)
        public override byte[] ToByteArray() {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData)) {
                writer.Write(areaDataID);
                writer.Write(unknown1);
                writer.Write(matrixID);
                writer.Write(scriptFileID);
                writer.Write(levelScriptID);
                writer.Write(textArchiveID);
                writer.Write(musicDayID);
                writer.Write(musicNightID);
                writer.Write(wildPokémon);
                writer.Write(eventFileID);
                writer.Write(locationName);
                writer.Write(areaIcon);
                writer.Write(weatherID);
                writer.Write(cameraAngleID);

                ushort mapSettings = (ushort)((locationSpecifier & 0b_1111_111) + ((battleBackground & 0b_1111_1) << 7) + ((flags & 0b_1111) << 12));
                writer.Write(mapSettings);
            }
            return newData.ToArray();
        }
        #endregion
    }

    /// <summary>
    /// Class to store map header data from Pokémon HG and SS
    /// </summary>
    public class HeaderHGSS : MapHeader {
        #region Fields (7)
        public byte areaIcon { get; set; }
        public byte followMode { get; set; }
        public byte locationName { get; set; }
        public byte locationType { get; set; }  //4 bits only
        public byte unknown0 { get; set; } //4 bits only
        public byte unknown1 { get; set; } //4 bits only
        public byte worldmapX { get; set; } //6 bits only
        public byte worldmapY { get; set; } //6 bits only
        public bool kantoFlag { get; set; }
        #endregion

        #region Constructors (1)
        public HeaderHGSS(ushort headerNumber, Stream data) {
            this.ID = headerNumber;
            using (BinaryReader reader = new BinaryReader(data)) {
                try {
                    wildPokémon = reader.ReadByte();
                    areaDataID = reader.ReadByte();

                    ushort coords = reader.ReadUInt16();
                    unknown0 = (byte)(coords & 0b_1111); //get 4 bits
                    worldmapX = (byte)((coords >> 4) & 0b_1111_11); //get 6 bits after the first 4
                    worldmapY = (byte)((coords >> 10) & 0b_1111_11); //get 6 bits after the first 10

                    matrixID = reader.ReadUInt16();
                    scriptFileID = reader.ReadUInt16();
                    levelScriptID = reader.ReadUInt16();
                    textArchiveID = reader.ReadUInt16();
                    musicDayID = reader.ReadUInt16();
                    musicNightID = reader.ReadUInt16();
                    eventFileID = reader.ReadUInt16();
                    locationName = reader.ReadByte();
                    
                    byte areaProperties = reader.ReadByte();
                    areaIcon = (byte)(areaProperties & 0b_1111); //get 4 bits
                    unknown1 = (byte)((areaProperties >> 4) & 0b_1111); //get 4 bits after the first 4

                    uint last32 = reader.ReadUInt32();
                    kantoFlag = (last32 & 0b_1) == 1; //get 1 bit
                    weatherID = (byte)((last32 >> 1) & 0b_1111_111); //get 7 bits after the first one

                    locationType = (byte)((last32 >> 8) & 0b_1111); //get 4 bits after the first 8
                    cameraAngleID = (byte)((last32 >> 12) & 0b_1111_11); //get 6 bits after the first 12
                    followMode = (byte)((last32 >> 18) & 0b_11); //get 2 bits after the first 17
                    battleBackground = (byte)((last32 >> 20) & 0b_1111_1); //get 5 bits after the first 19
                    flags = (byte)(last32 >> 25 & 0b_1111_111); //get 7 bits after the first 24

                } catch (EndOfStreamException) {
                    MessageBox.Show("Error loading header " + ID + '.', "Unexpected EOF", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ID = ushort.MaxValue;
                }
            }
        }
        #endregion Constructors

        #region Methods(1)
        public override byte[] ToByteArray() {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData)) {
                writer.Write((byte)wildPokémon);
                writer.Write(areaDataID);

                ushort worldMapCoordinates = (ushort)((unknown0 & 0b_1111) + ((worldmapX & 0b_1111_11) << 4) + ((worldmapY & 0b_1111_11) << 10));
                writer.Write(worldMapCoordinates);

                writer.Write(matrixID);
                writer.Write(scriptFileID);
                writer.Write(levelScriptID);
                writer.Write(textArchiveID);
                writer.Write(musicDayID);
                writer.Write(musicNightID);
                writer.Write(eventFileID);
                writer.Write(locationName);

                byte areaProperties = (byte)((areaIcon & 0b_1111) + ((unknown1 & 0b_1111) << 4));
                writer.Write(areaProperties);

                uint last32 = (uint)(((weatherID & 0b_1111_111) << 1) +
                    ((locationType & 0b_1111) << 8) +
                    ((cameraAngleID & 0b_1111_1) << 12) +
                    ((followMode & 0b_11) << 18) +
                    ((battleBackground & 0b_1111_1) << 20) +
                    ((flags & 0b_1111_111) << 25));

                if (kantoFlag) {
                    last32++;
                }

                writer.Write(last32);
            }
            return newData.ToArray();
        }
        #endregion
    }
}