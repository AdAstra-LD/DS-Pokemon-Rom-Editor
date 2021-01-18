using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace DSPRE
{
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
       -----------------    7: Escape Rope
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
       -----------------    2: Allow Escape Rope
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
       -----------------    5: Allow Escape rope
       -----------------    6: ?
       -----------------    7: Allow Bicycle
       -----------------    8: ?

    ----------------------------------------------------------------------------------*/

    /// <summary>
    /// General class to store common map header data across all Gen IV Pokémon NDS games
    /// </summary>
    public abstract class Header
	{
        #region Fields (10)
        public byte areaDataID { get; set; }
        public byte camera { get; set; }
        public byte areaSettings { get; set; }
        public ushort eventID { get; set; }
        public byte flags { get; set; }
        public ushort levelScript { get; set; }
        public ushort matrix { get; set; }
        public ushort script { get; set; }
        public byte showName { get; set; }
        public ushort text { get; set; }
        public byte weather { get; set; }
        public ushort wildPokémon { get; set; }
        #endregion Fields

        #region Methods (1)
        public abstract byte[] SaveHeader();
        #endregion
    }

    /// <summary>
    /// Class to store map header data from Pokémon D and P
    /// </summary>
    public class HeaderDP: Header
    {
        #region Fields (5)
        public byte unknown1 { get; set; }
        public ushort musicDay { get; set; }
        public ushort musicNight { get; set; }
        public ushort mapName { get; set; }
        #endregion Fields

        #region Constructors (1)
        public HeaderDP(Stream data)
        {
            using (BinaryReader reader = new BinaryReader(data))
            {
                areaDataID = reader.ReadByte();
                unknown1 = reader.ReadByte();
                matrix = reader.ReadUInt16();
                script = reader.ReadUInt16();
                levelScript = reader.ReadUInt16();
                text = reader.ReadUInt16();
                musicDay = reader.ReadUInt16();
                musicNight = reader.ReadUInt16();
                wildPokémon = reader.ReadUInt16();
                eventID = reader.ReadUInt16();
                mapName = reader.ReadUInt16();
                weather = StandardizeWeather(reader.ReadByte());
                camera = reader.ReadByte();
                showName = reader.ReadByte();
                flags = reader.ReadByte();
            }
        }
        #endregion Constructors

        #region Methods (1)
        public override byte[] SaveHeader()
        {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData))
            {
                writer.Write(areaDataID);
                writer.Write(unknown1);
                writer.Write(matrix);
                writer.Write(script);
                writer.Write(levelScript);
                writer.Write(text);
                writer.Write(musicDay);
                writer.Write(musicNight);
                writer.Write(wildPokémon);
                writer.Write(eventID);
                writer.Write(mapName);
                writer.Write(weather);
                writer.Write(camera);
                writer.Write(showName);
                writer.Write(flags);
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
    public class HeaderPt : Header
    {
        #region Fields (5)
        public byte areaIcon { get; set; }
        public byte mapName { get; set; }
        public ushort musicDay { get; set; }
        public ushort musicNight { get; set; }
        public byte unknown1 { get; set; }
        #endregion Fields

        #region Constructors (1)
        public HeaderPt(Stream data)
        {
            using (BinaryReader reader = new BinaryReader(data))
            {
                try {
                    areaDataID = reader.ReadByte();
                    unknown1 = reader.ReadByte();
                    matrix = reader.ReadUInt16();
                    script = reader.ReadUInt16();
                    levelScript = reader.ReadUInt16();
                    text = reader.ReadUInt16();
                    musicDay = reader.ReadUInt16();
                    musicNight = reader.ReadUInt16();
                    wildPokémon = reader.ReadUInt16();
                    eventID = reader.ReadUInt16();
                    mapName = reader.ReadByte();
                    areaIcon = reader.ReadByte();
                    weather = StandardizeWeather(reader.ReadByte());
                    camera = reader.ReadByte();
                    showName = reader.ReadByte();
                    flags = reader.ReadByte();
                } catch (EndOfStreamException) {
                    MessageBox.Show("Error loading headers.\n", "Unexpected EOF", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion Constructors

        #region Methods(1)
        public override byte[] SaveHeader()
        {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData))
            {
                writer.Write(areaDataID);
                writer.Write(unknown1);
                writer.Write(matrix);
                writer.Write(script);
                writer.Write(levelScript);
                writer.Write(text);
                writer.Write(musicDay);
                writer.Write(musicNight);
                writer.Write(wildPokémon);
                writer.Write(eventID);
                writer.Write(mapName);
                writer.Write(areaIcon);
                writer.Write(weather);
                writer.Write(camera);
                writer.Write(showName);
                writer.Write(flags);
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
                case 26:
                case 27:
                case 28:
                case 29:
                case 30:
                case 31:
                case 33:
                    return 0; // Normal weather
                case 21:
                case 34:
                case 35:
                    return 6; // D snow
                case 36:
                    return 5; // Snowfall
                case 32:
                    return 2; // Rain
                default:
                    return weather;
            }
        }
        #endregion
    }
    
    /// <summary>
    /// Class to store map header data from Pokémon HG and SS
    /// </summary>
    public class HeaderHGSS : Header
    {
        #region Fields (7)
        public byte areaIcon { get; set; }
        public byte followMode { get; set; }
        public byte mapName { get; set; }
        public ushort musicDay { get; set; }
        public ushort musicNight { get; set; }
        public byte unknown1 { get; set; }
        public byte unknown2 { get; set; }
        #endregion

        #region Constructors (1)
        public HeaderHGSS(Stream data)
        {
            using (BinaryReader reader = new BinaryReader(data))
            {
                try {
                    wildPokémon = reader.ReadByte();
                    areaDataID = reader.ReadByte();
                    unknown1 = reader.ReadByte();
                    unknown2 = reader.ReadByte();
                    matrix = reader.ReadUInt16();
                    script = reader.ReadUInt16();
                    levelScript = reader.ReadUInt16();
                    text = reader.ReadUInt16();
                    musicDay = reader.ReadUInt16();
                    musicNight = reader.ReadUInt16();
                    eventID = reader.ReadUInt16();
                    mapName = reader.ReadByte();
                    areaIcon = StandardizeAreaIcon(reader.ReadByte());
                    weather = StandardizeWeather(reader.ReadByte());
                    
                    byte cameraAndArea = reader.ReadByte();
                    camera = (byte)(cameraAndArea / 16);
                    areaSettings = (byte)(cameraAndArea % 16);

                    followMode = reader.ReadByte();
                    flags = reader.ReadByte();
                } catch (EndOfStreamException) {
                    MessageBox.Show("Error loading headers.\n", "Unexpected EOF", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion Constructors

        #region Methods(1)
        public override byte[] SaveHeader()
        {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData))
            {
                writer.Write((byte)wildPokémon);
                writer.Write(areaDataID);
                writer.Write(unknown1);
                writer.Write(unknown2);
                writer.Write(matrix);
                writer.Write(script);
                writer.Write(levelScript);
                writer.Write(text);
                writer.Write(musicDay);
                writer.Write(musicNight);
                writer.Write(eventID);
                writer.Write(mapName);
                writer.Write(areaIcon);
                writer.Write(weather);
                writer.Write((byte)(camera*16 + areaSettings));
                writer.Write(followMode);
                writer.Write(flags);
            }
            return newData.ToArray();
        }
        public byte StandardizeAreaIcon(byte areaIcon)
        {
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
        public byte StandardizeWeather(byte weather)
        {
            /* This function was written to avoid having to account 
            for duplicate weather values , since  many share the same 
            weather conditions */

            switch (weather)
            {
                case 0:
                case 1:
                case 26:
                    return 0;
                case 22:
                case 23:
                    return 22;
                default:
                    return weather; // No name displayed
            }
        }
        #endregion
    }

    public class HeaderDatabase
    {
        public string[] PtAreaIconValues = new string[]
        {
            "[00] City",
            "[01] City",
            "[02] Town 1",
            "[03] Town 2",
            "[04] Cave",
            "[05] Forest",
            "[06] Water",
            "[07] Field",
            "[08] Island",
            "[09] Wood"
        };
        public string[] HGSSAreaIconValues = new string[]
        {
            "[002] Wood",
            "[009] Gray",
            "[017] Wall",
            "[048] Not displayed",
            "[131] Town",
            "[132] Cave",
            "[135] Field",
            "[152] Lake",
            "[165] Forest",
            "[166] Water"
        };
        public string[] DPPtCameraValues = new string[]
        {
            "[00] 3D Normal",
            "[01] 3D Top View (Higher than [12])",
            "[02] 3D Frontal Low (Wider than [15])",
            "[03] 3D Frontal",
            "[04] 2D Ortho",
            "[05] 3D Normal - Wide FOV",
            "[06] 3D Bird View",
            "[07] 3D Normal",
            "[08] 3D Bird View Far",
            "[09] 3D Frontal - Wide FOV",
            "[10] 3D Top View - Narrow",
            "[11] Normal 3D",
            "[12] 3D Top View",
            "[13] Frontal 3D",
            "[14] 3D Top View - Wide FOV",
            "[15] 3D Frontal Low"
        };
        public string[] HGSSCameraValues = new string[] {
            "[00] 3D Top",
            "[01] 3D Front High",
            "[02] 3D Lower",
            "[03] 3D Frontal",
            "[04] 2D Top View",
            "[05] Normal 3D",
            "[06] 3D Normal",
            "[07] High 3D",
            "[08] 3D Top View",
            "[09] 3D Top View",
            "[10] 3D High Wide",
            "[11] 3D Frontal Wide",
            "[12] 3D Lower Close",
            "[13] 3D Full Frontal",
            "[14] 3D Top View",
            "[15] 2D Higher"
        };
        public string[] HGSSAreaProperties = new string[] {
            "[00] Unknown",
            "[01] Unknown",
            "[02] Unknown",
            "[03] Cave Animation",
            "[04] Hide Nametag",
            "[05] Unknown",
            "[06] Unknown",
            "[07] Unknown",
            "[08] Unknown",
            "[09] Unknown",
            "[10] Unknown",
            "[11] Unknown",
            "[12] Unknown",
            "[13] Unknown",
            "[14] Unknown",
            "[15] Unknown",
            "[16] Unknown",
        };
        public string[] DPShowNameValues = new string[]
        {
            "[000] Show",
            "[001] Show",
            "[002] Show",
            "[003] Show",
            "[004] Don't show"
        };
        public string[] PtShowNameValues = new string[]
        {
            "[000] Show",
            "[001] Show",
            "[002] Show",
            "[003] Show",
            "[004] Don't show",
            "[128] Don't show",
            "[129] Show",
            "[130] Show",
            "[131] Show",
            "[132] Don't show",
            "[134] Show"
        };
        public string[] DPWeatherValues = new string[]
        {
            "[00] Normal",
            "[01] Normal, somewhat dark",
            "[02] Rain",
            "[03] Heavy rain",
            "[04] Thunderstorm",
            "[05] Snowfall, slow",
            "[06] D dust",
            "[07] Blizzard",
            "[09] Volcanic ash fall, slow",
            "[10] Sand storm",
            "[11] Hail",
            "[12] Rocks ascending (?)",
            "[14] Fog",
            "[15] Deep fog",
            "[16] Dark, Flash usable",
            "[17] Lightning, no rain",
            "[22] Volcanic ash fall, steady",
        };
        public string[] PtWeatherValues = new string[]
        {
            "[00] Normal",
            "[01] Normal, somewhat dark",
            "[02] Rain",
            "[03] Heavy rain",
            "[04] Thunderstorm",
            "[05] Snowfall, slow",
            "[06] D dust",
            "[07] Blizzard",
            "[09] Volcanic ash fall, slow",
            "[10] Sand storm",
            "[11] Hail",
            "[12] Rocks ascending (?)",
            "[14] Fog",
            "[15] Deep fog",
            "[16] Dark, Flash usable",
            "[17] Lightning, no rain",
            "[22] Volcanic ash fall, steady",
            "[23] Eterna forest weather",
            "[24] Player in circle of light",
            "[25] Player in a circle of light",
        };
        public string[] HGSSWeatherValues = new string[]
        {
            "[00] Normal",
            "[02] Heavy rain",
            "[10] D snow",
            "[16] Hail",
            "[18] Fog",
            "[22] Cave Dark",
            "[24] Cave Dark after flash",
        };

        #region Constructors (1)
        public HeaderDatabase()
        {
           
        }
        #endregion

    }
}