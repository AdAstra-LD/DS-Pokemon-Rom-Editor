using System.IO;
using System.Collections.Generic;
using LibNDSFormats.NSBMD;
using System;
using System.Windows.Forms;

namespace DSPRE
{
    /* ----------------------- MAP FILE DATA STRUCTURE (DPPtHGSS):--------------------------

    0x0  //  uint:       Length of permissions section (always 2048)
    0x4  //  uint:       Length of buildings section
    0x8  //  uint:       Length of nsbmd model section
    0xC  //  uint:       Length of BDHC section
    0x10 //  

    ************* Permissions section (1024 byte - byte pairs):
        
    0x0  //  byte:       Type value of tile (e.g. normal, grass, surfable water)
    0x1  //  byte:       Collision value of tile (00 for walkable, 80 for blocked)   

    ************* Buildings section (# of buildings equal to section length divided by 48):
    
    BUILDING FORMAT:
    0x0  //  uint:      Model ID
    0x4  //  ushort:    65535ths of X coordinate
    0x6  //  ushort:    X coordinate
    0x8  //  ushort:    65535ths of Z coordinate
    0xA  //  ushort:    Z coordinate
    0xC  //  ushort:    65535ths of Y coordinate
    0xE  //  ushort:    Y coordinate
    -------- 0x10 - 0x1C FILLER ZEROES
    0x1D //  uint:      Model width scale (usually 16)
    0x21 //  uint:      Model height scale (usually 16)
    0x25 //  uint:      Model length scale (usually 16)
    -------- 0x2A - 0x2F FILLER ZEROES

   ************* NSBMD model section
   ************* BDHC section
   
   -------------------------------------------------------------------------------------- */


    /// <summary>
    /// Class to store map data in Pokémon NDS games
    /// </summary>
    public class MapFile
    {
        #region Fields
        private string gameVersion;
        public byte[,] collisions = new byte[32, 32];
        public byte[,] types = new byte[32, 32];
        public List<Building> buildings;
        public NSBMD mapModel;
        public byte[] mapModelData;
        public byte[] bdhc;
        public byte[] bgs;
        #endregion

        #region Constructors (1)
        public MapFile(Stream data, string gameVersion) {
            this.gameVersion = gameVersion;
            using (BinaryReader reader = new BinaryReader(data))
            {
                /* Read sections lengths */
                int permissionsSectionLength = reader.ReadInt32();
                int buildingsSectionLength = reader.ReadInt32();
                int nsbmdSectionLength = reader.ReadInt32();
                int bdhcSectionLength = reader.ReadInt32();

                /* Read background sounds section */
                if (gameVersion == "HG" || gameVersion == "SS") {
                    ushort signature = reader.ReadUInt16();
                    if (signature != 0x1234) {
                        MessageBox.Show("The header section of this map's BackGround Sound data is corrupted.",
                            "BGS Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    ushort bgsSectionLength = reader.ReadUInt16();

                    reader.BaseStream.Position -= 4;
                    ImportSoundPlates(new MemoryStream(reader.ReadBytes(bgsSectionLength + 4)));
                }

                /* Read permission data */
                ImportPermissions(new MemoryStream(reader.ReadBytes(permissionsSectionLength)));

                /* Read buildings data */
                ImportBuildings(new MemoryStream(reader.ReadBytes(buildingsSectionLength)));

                /* Read nsbmd model */
                ImportMapModel(new MemoryStream(reader.ReadBytes(nsbmdSectionLength)));

                /* Read bdhc data */
                ImportTerrain(new MemoryStream(reader.ReadBytes(bdhcSectionLength)));
            }
        }
        #endregion

        #region Methods (2)
        /* Creates new standard building object, adds it to the current map and returns it*/
        public Building AddNewBuilding() {
            Building b = new Building();
            buildings.Add(b);
            return b;
        }
        public void AddBuilding(Building b) {
            buildings.Add(b);
        }
        public byte[] BuildingsToByteArray() {
            MemoryStream newData = new MemoryStream(0x30 * buildings.Count);
            using (BinaryWriter writer = new BinaryWriter(newData)) {
                for (int i = 0; i < buildings.Count; i++) {
                    writer.Write(buildings[i].modelID);
                    writer.Write(buildings[i].xFraction);
                    writer.Write(buildings[i].xPosition);
                    writer.Write(buildings[i].zFraction);
                    writer.Write(buildings[i].zPosition);
                    writer.Write(buildings[i].yFraction);
                    writer.Write(buildings[i].yPosition);

                    writer.BaseStream.Position += 0xD; // First filler section

                    writer.Write(buildings[i].width);
                    writer.Write(buildings[i].height);
                    writer.Write(buildings[i].length);

                    writer.Write(new byte[0x7]); // Second filler section
                }
            }
            return newData.ToArray();
        }
        public byte[] ExportMapModel() {
            return mapModelData;
        }
        public byte[] CollisionsToByteArray() {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData)) {
                for (int i = 0; i < 32; i++) {
                    for (int j = 0; j < 32; j++) {
                        writer.Write(types[i, j]);
                        writer.Write(collisions[i, j]);
                    }
                }
            }
            return newData.ToArray();
        }       
        public byte[] GetTerrain() {
            return bdhc;
        }
        public byte[] GetSoundPlates() {
            return bgs;
        }
        public void ImportBuildings(Stream newData) {
            buildings = new List<Building>();
            using (BinaryReader reader = new BinaryReader(newData)) {
                for (int i = 0; i < newData.Length / 0x30; i++) {
                    buildings.Add(new Building(new MemoryStream(reader.ReadBytes(0x30))));
                }
            }
        }
        public void ImportMapModel(Stream newData) {
            using (BinaryReader reader = new BinaryReader(newData)) {
                reader.BaseStream.Position = 0xE;
                if (reader.ReadInt16() > 1) {// If there is more than one file, it means there are embedded textures we must remove
                    using (BinaryWriter writer = new BinaryWriter(new MemoryStream())) {
                        reader.BaseStream.Position = 0x1C;
                        uint mdl0Size = reader.ReadUInt32(); // Read mdl0 file size

                        reader.BaseStream.Position = 0x0;
                        writer.Write(reader.ReadBytes(0x8)); // Write firt header bytes, same for all NSBMD.
                        writer.Write(mdl0Size + 0x14);
                        writer.Write((short)0x10); // Writes BMD0 header size (always 16)
                        writer.Write((short)0x1); // Write new number of sub-files, since embedded textures are removed
                        writer.Write(0x14); // Writes new start offset of MDL0
                        reader.BaseStream.Position = 0x18;
                        writer.Write(reader.ReadBytes((int)mdl0Size)); // Writes MDL0;

                        mapModelData = ((MemoryStream)writer.BaseStream).ToArray();
                    }
                } else {
                    reader.BaseStream.Position = 0x0;
                    mapModelData = reader.ReadBytes((int)newData.Length);
                }

                mapModel = NSBMDLoader.LoadNSBMD(new MemoryStream(mapModelData));
            }
        }
        public void ImportPermissions(Stream newData) {
            using (BinaryReader reader = new BinaryReader(newData)) {
                for (int i = 0; i < 32; i++) {
                    for (int j = 0; j < 32; j++) {
                        types[i, j] = reader.ReadByte(); // Read permission type (e.g. surfing water, grass, sand etc.)
                        collisions[i, j] = reader.ReadByte(); // Read walkability (00 for walkable and 80 for blocked)                        
                    }
                }
            }
        }
        public void ImportSoundPlates(Stream newData) {
            using (BinaryReader reader = new BinaryReader(newData)) {
                bgs = reader.ReadBytes((int)newData.Length);
            }
        }
        public void ImportTerrain(Stream newData) {
            using (BinaryReader reader = new BinaryReader(newData)) {
                bdhc = reader.ReadBytes((int)newData.Length);
            }
        }
        public byte[] ToByteArray() {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData)) {
                /* Write section lengths */
                writer.Write(0x800);
                writer.Write(buildings.Count * 0x30);
                writer.Write(mapModelData.Length);
                writer.Write(bdhc.Length);

                /* Write soundplate section for HG/SS */
                if (gameVersion == "HG" || gameVersion == "SS") { 
                    writer.Write((ushort)0x1234);
                    writer.Write((ushort)bgs.Length);
                    writer.Write(bgs);
                }

                /* Write sections */
                writer.Write(CollisionsToByteArray());
                writer.Write(BuildingsToByteArray());
                writer.Write(ExportMapModel());
                writer.Write(GetTerrain());
            }
            return newData.ToArray();
        }
        #endregion
    }

    /// <summary>
    /// Class to store building data from Pokémon NDS games
    /// </summary>
    public class Building
	{
        #region Fields (11)
        public NSBMD NSBMDFile;
        public uint modelID { get; set; }
        public short xPosition { get; set; }
        public short yPosition { get; set; }
        public short zPosition { get; set; }
        public ushort xFraction { get; set; }
        public ushort yFraction { get; set; }
        public ushort zFraction { get; set; }
        public uint width { get; set; }
        public uint height { get; set; }
        public uint length { get; set; }
        #endregion Fields

        #region Constructors (2)
        public Building(Stream data)
		{
            using (BinaryReader reader = new BinaryReader(data))
            {
                modelID = reader.ReadUInt32();
                xFraction = reader.ReadUInt16();
                xPosition = reader.ReadInt16();
                zFraction = reader.ReadUInt16();
                zPosition = reader.ReadInt16();
                yFraction = reader.ReadUInt16();
                yPosition = reader.ReadInt16();

                reader.BaseStream.Position += 0xD; // Skip first filler section

                width = reader.ReadUInt16();
                reader.BaseStream.Position += 0x2;
                height = reader.ReadUInt16();
                reader.BaseStream.Position += 0x2;
                length = reader.ReadUInt16();
                reader.BaseStream.Position += 0x2;
            }
        }
        public Building()
        {
           modelID = 0;
           xFraction = 0;
           xPosition = 0;
           zFraction = 0;
           zPosition = 1;
           yFraction = 0;
           yPosition = 0;
           width = 16;
           height = 16;
           length = 16;
        }
        
        public Building(Building toCopy) {
            modelID = toCopy.modelID;
            xFraction = toCopy.xFraction;
            xPosition = toCopy.xPosition;
            zFraction = toCopy.zFraction;
            zPosition = (short)(toCopy.zPosition + 1);
            yFraction = toCopy.yFraction;
            yPosition = toCopy.yPosition;
            width = toCopy.width;
            height = toCopy.height;
            length = toCopy.length;
        }
        #endregion Constructors
    }
}