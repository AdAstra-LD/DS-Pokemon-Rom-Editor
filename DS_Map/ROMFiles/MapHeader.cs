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
    public abstract class MapHeader {
        /*System*/
        public short ID { get; set; }
        public static readonly byte length = 24;
        public static readonly string nameSeparator = " -   ";
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
        public byte showName { get; set; }
        public byte battleBackground { get; set; }
        public ushort textArchiveID { get; set; }
        public byte weatherID { get; set; }
        public byte flags { get; set; }
        public ushort wildPokémon { get; set; }
        #endregion Fields

        #region Methods (1)
        public abstract byte[] toByteArray();
        public static MapHeader BuildFromFile(string filename, short headerNumber, long offsetInFile) {
            /* Calculate header offset and load data */
            byte[] headerData = DSUtils.ReadFromFile(filename, offsetInFile, MapHeader.length);

            /* Encapsulate header data into the class appropriate for the gameVersion */
            if (headerData.Length < MapHeader.length)
                return null;

            switch (RomInfo.gameVersion) {
                case "D":
                case "P":
                    return new HeaderDP(headerNumber, new MemoryStream(headerData));
                case "Plat":
                    return new HeaderPt(headerNumber, new MemoryStream(headerData));
                default:
                    return new HeaderHGSS(headerNumber, new MemoryStream(headerData));
            }
        }
        public static MapHeader LoadFromARM9(short headerNumber) {
            long headerOffset = Resources.PokeDatabase.System.headerOffsetsDict[RomInfo.romID] + MapHeader.length * headerNumber;
            return BuildFromFile(RomInfo.arm9Path, headerNumber, headerOffset);
        }
        #endregion
    }

    /// <summary>
    /// Class to store map header data from Pokémon D and P
    /// </summary>
    public class HeaderDP: MapHeader
    {
        #region Fields (5)
        public byte unknown1 { get; set; }
        public ushort locationName { get; set; }
        #endregion Fields

        #region Constructors (1)
        public HeaderDP(short headerNumber, Stream data)
        {
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
                weatherID = StandardizeWeather(reader.ReadByte());
                cameraAngleID = reader.ReadByte();
                showName = reader.ReadByte();

                byte mapSettings = reader.ReadByte();
                battleBackground = (byte)(mapSettings & 0b_1111);
                flags = (byte)(mapSettings >> 4 & 0b_1111);
            }
        }
        #endregion Constructors

        #region Methods (1)
        public override byte[] toByteArray()
        {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData))
            {
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
                writer.Write(showName);
                
                byte mapSettings = (byte) ((battleBackground & 0b_1111) + ((flags & 0b_1111) << 4));
                writer.Write(mapSettings);
            }
            return newData.ToArray();
        }
        public byte StandardizeWeather(byte weather)
        {
            /* This function was written to avoid having to account 
            for duplicate weather values , since  many share the same 
            weather conditions */

            switch (weather)
            {
                case 8:
                case 13:
                case 18:
                case 19:
                case 20:
                case 23:
                case 25:
                    return 0; // Normal weather
                case 21:
                case 26:
                case 27:
                    return 6; // D snow
                case 28:
                    return 5; // Snowfall
                case 24:
                    return 4; // Thunderstorm
                default:
                    return weather;
            }
        }
        #endregion
    }
    
    /// <summary>
    /// Class to store map header data from Pokémon Plat
    /// </summary>
    public class HeaderPt : MapHeader
    {
        #region Fields (5)
        public byte areaIcon { get; set; }
        public byte locationName { get; set; }
        public byte unknown1 { get; set; }
        #endregion Fields

        #region Constructors (1)
        public HeaderPt(short headerNumber, Stream data)
        {
            this.ID = headerNumber;
            using (BinaryReader reader = new BinaryReader(data))
            {
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
                    showName = (byte)(mapSettings & 0b_1111_111);
                    battleBackground = (byte)(mapSettings >> 7 & 0b_1111_1);
                    flags = (byte)(mapSettings >> 12 & 0b_1111);

                } catch (EndOfStreamException) {
                    MessageBox.Show("Error loading header " + ID + '.', "Unexpected EOF", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion Constructors

        #region Methods(1)
        public override byte[] toByteArray()
        {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData))
            {
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

                ushort mapSettings = (ushort)((showName & 0b_1111_111) + ((battleBackground & 0b_1111_1) << 7) + ((flags & 0b_1111) << 12));
                writer.Write(mapSettings);
            }
            return newData.ToArray();
        }
        #endregion
    }
    
    /// <summary>
    /// Class to store map header data from Pokémon HG and SS
    /// </summary>
    public class HeaderHGSS : MapHeader
    {
        #region Fields (7)
        public byte areaIcon { get; set; }
        public byte followMode { get; set; }
        public byte locationName { get; set; }
        public byte areaSettings { get; set; } // 4 bits only [4 bits for the camera as well]
        public byte unknown0 { get; set; } //4 bits only
        public byte worldmapX { get; set; } //6 bits only
        public byte worldmapY { get; set; } //6 bits only
        #endregion

        #region Constructors (1)
        public HeaderHGSS(short headerNumber, Stream data)
        {
            this.ID = headerNumber;
            using (BinaryReader reader = new BinaryReader(data))
            {
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
                    areaIcon = StandardizeAreaIcon(reader.ReadByte());
                    weatherID = reader.ReadByte();
                    
                    byte cameraAndArea = reader.ReadByte();
                    areaSettings = (byte)(cameraAndArea & 0b_1111); //get 4 bits 
                    cameraAngleID = (byte)(cameraAndArea >> 4 & 0b_1111); //get 4 bits after the first 4
                    

                    followMode = reader.ReadByte();
                    flags = reader.ReadByte();
                } catch (EndOfStreamException) {
                    MessageBox.Show("Error loading header " + ID + '.', "Unexpected EOF", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ID = -1;
                }
            }
        }
        #endregion Constructors

        #region Methods(1)
        public override byte[] toByteArray()
        {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData))
            {
                writer.Write((byte)wildPokémon);
                writer.Write(areaDataID);

                ushort worldMapCoordinates = (ushort) ( unknown0 & 0b_1111 + ((worldmapX & 0b_1111_11) << 4) + ((worldmapY & 0b_1111_11) << 10) );
                writer.Write(worldMapCoordinates);

                writer.Write(matrixID);
                writer.Write(scriptFileID);
                writer.Write(levelScriptID);
                writer.Write(textArchiveID);
                writer.Write(musicDayID);
                writer.Write(musicNightID);
                writer.Write(eventFileID);
                writer.Write(locationName);
                writer.Write(areaIcon);
                writer.Write(weatherID);

                byte cameraAndArea = (byte)  ((areaSettings & 0b_1111) + ((cameraAngleID & 0b_1111) << 4));
                writer.Write(cameraAndArea);

                writer.Write(followMode);
                writer.Write(flags);
            }
            return newData.ToArray();
        }
        public byte StandardizeAreaIcon(byte areaIcon)
        {
            //TO DO: improve this
            //The AreaIcon byte is probably split into bits by the game engine,
            //each with a specific purpose...
            //there is a very interesting pattern here


            /* This function was written to avoid having to account 
            for duplicate values of the map name textbox types, since 
            many share the same textbox image */
             
            switch (areaIcon)
            {
                /* Water textbox values */
                case 182:
                case 198:
                    return 166;
                /* Town textbox values*/
                case 147:
                case 163:
                case 179:
                case 195:
                    return 131;
                /* Wall textbox values */
                case 33:
                case 65:
                case 81:
                case 97:
                case 113:
                case 145:
                case 161:
                case 193:
                case 209:
                    return 17;
                /* Gray textbox values */
                case 25:
                case 41:
                case 57:
                case 73:
                case 89:
                case 105:
                case 121:
                case 137:
                case 153:
                case 169:
                case 185:
                case 201:
                case 217:
                    return 9;
                /* Cave textbox values */
                case 148:
                case 164:
                case 180:
                case 196:
                    return 132;
                /* Field textbox values */
                case 151:
                    return 135;
                /* Wooden textbox values */
                case 50:
                case 162:
                case 194:
                    return 2;
                /* Forest textbox values */
                case 181:
                    return 165;
                default:
                    return areaIcon;
            }
        }
        #endregion
    }
}