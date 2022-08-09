using System.IO;
using System.Collections.Generic;
using LibNDSFormats.NSBMD;
using System.Windows.Forms;
using static DSPRE.RomInfo;
using System;
using System.Drawing;

namespace DSPRE.ROMFiles {
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
    public class MapFile : RomFile {
        #region Fields

        public static readonly string NSBMDFilter = "NSBMD File (*.nsbmd)|*.nsbmd";
        public static readonly string TexturedNSBMDFilter = "Textured" + NSBMDFilter;
        public static readonly string UntexturedNSBMDFilter = "Untextured" + NSBMDFilter;

        public static readonly string MovepermsFilter = "Permissions File (*.per)|*.per";

        public static readonly string BuildingsFilter = "Buildings File (*.bld)|*.bld";

        public static readonly string BDHCFilter = "Terrain File (*.bdhc)|*.bdhc";
        public static readonly string BDHCamFilter = "Terrain File (*.bdhc, *.bdhcam)|*.bdhc;*.bdhcam";
        
        public static readonly string BGSFilter = "BackGround Sound File (*.bgs)|*.bgs";

        public bool correctnessFlag = true;
        public static readonly byte mapSize = 32;
        public static readonly byte buildingHeaderSize = 48;
        public static readonly byte[] blankBGS = new byte[] { 0x34, 0x12, 0x00, 0x00 };

        public List<Building> buildings;
        public NSBMD mapModel;
        public byte[,] collisions = new byte[mapSize, mapSize];
        public byte[,] types = new byte[mapSize, mapSize];
        public byte[] mapModelData;
        public byte[] bdhc;
        public byte[] bgs = blankBGS;
        #endregion

        #region Constructors (1)
        public MapFile(string path, gFamEnum gFamily, bool discardMoveperms = false, bool showMessages = true) : this (new FileStream(path, FileMode.Open), gFamily, discardMoveperms, showMessages) {}
        public MapFile(int mapNumber, gFamEnum gFamily, bool discardMoveperms = false, bool showMessages = true) : this(RomInfo.gameDirs[DirNames.maps].unpackedDir + "\\" + mapNumber.ToString("D4"), gFamily, discardMoveperms, showMessages) { }
        public MapFile(Stream data, gFamEnum gFamily, bool discardMoveperms = false, bool showMessages = true) {
            using (BinaryReader reader = new BinaryReader(data)) {
                /* Read sections lengths */
                int permissionsSectionLength = reader.ReadInt32();
                int buildingsSectionLength = reader.ReadInt32();
                int nsbmdSectionLength = reader.ReadInt32();
                int bdhcSectionLength = reader.ReadInt32();

                /* Read background sounds section */
                if (gFamily == gFamEnum.HGSS) { //Map must be loaded as HGSS
                    ushort bgsSignature = reader.ReadUInt16();
                    if (bgsSignature == 0x1234) {
                        ushort bgsDataLength = reader.ReadUInt16();

                        reader.BaseStream.Position -= 4; //go back so that the signature "0x1234" + size can be read and stored
                        ImportSoundPlates(reader.ReadBytes(bgsDataLength + 4));
                    } else {
                        correctnessFlag = false;
                        if (showMessages) {
                            MessageBox.Show("The header section of this map's BackGround Sound data is corrupted.",
                            "BGS Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }

                /* Read permission data */
                if (discardMoveperms) {
                    reader.BaseStream.Position += permissionsSectionLength;
                } else {
                    ImportPermissions(reader.ReadBytes(permissionsSectionLength));
                }

                /* Read buildings data */
                ImportBuildings(reader.ReadBytes(buildingsSectionLength));

                /* Read nsbmd model */
                if ( !LoadMapModel(reader.ReadBytes(nsbmdSectionLength), showMessages) ) { //Assign result to flag
                    correctnessFlag = false;
                    mapModel = null;
                };

                /* Read bdhc data */
                ImportTerrain(reader.ReadBytes(bdhcSectionLength));
            }
        }
        #endregion

        #region Methods
        public byte[] BuildingsToByteArray() {
            MemoryStream newData = new MemoryStream(buildingHeaderSize * buildings.Count);
            using (BinaryWriter writer = new BinaryWriter(newData)) {
                for (int i = 0; i < buildings.Count; i++) {
                    writer.Write(buildings[i].modelID);
                    writer.Write(buildings[i].xFraction);
                    writer.Write(buildings[i].xPosition);
                    writer.Write(buildings[i].yFraction);
                    writer.Write(buildings[i].yPosition);
                    writer.Write(buildings[i].zFraction);
                    writer.Write(buildings[i].zPosition);

                    writer.Write((int)buildings[i].xRotation);
                    writer.Write((int)buildings[i].yRotation);
                    writer.Write((int)buildings[i].zRotation);

                    writer.BaseStream.Position += 1;

                    writer.Write(buildings[i].width);
                    writer.Write(buildings[i].height);
                    writer.Write(buildings[i].length);

                    writer.Write(new byte[0x7]); // Second filler section
                }
            }
            return newData.ToArray();
        }
        public byte[] CollisionsToByteArray() {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData)) {
                for (int i = 0; i < mapSize; i++) {
                    for (int j = 0; j < mapSize; j++) {
                        writer.Write(types[i, j]);
                        writer.Write(collisions[i, j]);
                    }
                }
            }
            return newData.ToArray();
        }
        public void ImportBuildings(byte[] newData) {
            buildings = new List<Building>();
            using (BinaryReader reader = new BinaryReader(new MemoryStream(newData))) {
                for (int i = 0; i < newData.Length / buildingHeaderSize; i++) {
                    buildings.Add(new Building(new MemoryStream(reader.ReadBytes(buildingHeaderSize))));
                }
            }
        }
        public bool LoadMapModel(byte[] newData, bool showMessages = true) {
            using (BinaryReader modelReader = new BinaryReader(new MemoryStream(newData))) {

                if (modelReader.ReadUInt32() != NSBMD.NDS_TYPE_BMD0) {
                    if (showMessages) {
                        MessageBox.Show("Please select an NSBMD file.", "Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return false;
                }

                modelReader.BaseStream.Position = 0xE;
                if (modelReader.ReadInt16() > 1) { // If NSBMD contains more than one segment, it means there are embedded textures we must remove
                    mapModelData = DSUtils.GetModelWithoutTextures(newData);
                } else {
                    modelReader.BaseStream.Position = 0x0;
                    mapModelData = modelReader.ReadBytes((int)modelReader.BaseStream.Length);
                }

                mapModel = NSBMDLoader.LoadNSBMD(new MemoryStream(mapModelData));
            }
            return true;
        }

        public void ImportPermissions(byte[] newData) {
            using (BinaryReader reader = new BinaryReader(new MemoryStream(newData))) {
                for (int i = 0; i < 32; i++) {
                    for (int j = 0; j < 32; j++) {
                        types[i, j] = reader.ReadByte(); // Read permission type (e.g. surfing water, grass, sand etc.)
                        collisions[i, j] = reader.ReadByte(); // Read walkability (00 for walkable and 80 for blocked)                        
                    }
                }
            }
        }
        public void ImportSoundPlates(byte[] newData) {
            using (BinaryReader reader = new BinaryReader(new MemoryStream(newData))) {
                bgs = reader.ReadBytes((int)newData.Length);
            }
        }
        public void ImportTerrain(byte[] newData) {
            using (BinaryReader reader = new BinaryReader(new MemoryStream(newData))) {
                bdhc = reader.ReadBytes((int)newData.Length);
            }
        }
        public override byte[] ToByteArray() {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData)) {
                /* Write section lengths */
                writer.Write(collisions.Length + types.Length);

                writer.Write(buildings.Count * buildingHeaderSize);
                writer.Write(mapModelData.Length);
                writer.Write(bdhc.Length);

                /* Write soundplate section for HG/SS */
                if (RomInfo.gameFamily == gFamEnum.HGSS) {
                    writer.Write(bgs);
                }

                /* Write sections */
                writer.Write(CollisionsToByteArray());
                writer.Write(BuildingsToByteArray());
                writer.Write(mapModelData);
                writer.Write(bdhc);
            }
            return newData.ToArray();
        }
        public void SaveToFileDefaultDir(int IDtoReplace, bool showSuccessMessage = true) {
            SaveToFileDefaultDir(DirNames.maps, IDtoReplace, showSuccessMessage);
        }
        public void SaveToFileExplorePath(string suggestedFileName, bool showSuccessMessage = true) {
            SaveToFileExplorePath("Gen IV Map Bin", "bin", suggestedFileName, showSuccessMessage);
        }
        #endregion
    }

    /// <summary>
    /// Class to store building data from Pokémon NDS games
    /// </summary>
    public class Building {
        #region Fields (11)
        public NSBMD NSBMDFile;
        public uint modelID { get; set; }
        public short xPosition { get; set; }
        public short yPosition { get; set; }
        public short zPosition { get; set; }
        public ushort xFraction { get; set; }
        public ushort yFraction { get; set; }
        public ushort zFraction { get; set; }
        public ushort xRotation { get; set; }
        public ushort yRotation { get; set; }
        public ushort zRotation { get; set; }
        public uint width { get; set; }
        public uint height { get; set; }
        public uint length { get; set; }
        #endregion Fields

        #region Constructors (2)
        public Building(Stream data) {
            using (BinaryReader reader = new BinaryReader(data)) {
                modelID = reader.ReadUInt32();

                xFraction = reader.ReadUInt16();
                xPosition = reader.ReadInt16();
                yFraction = reader.ReadUInt16();
                yPosition = reader.ReadInt16();
                zFraction = reader.ReadUInt16();
                zPosition = reader.ReadInt16();
                
                xRotation = reader.ReadUInt16();
                reader.BaseStream.Position += 0x2;
                yRotation = reader.ReadUInt16();
                reader.BaseStream.Position += 0x2;
                zRotation = reader.ReadUInt16();
                reader.BaseStream.Position += 0x2;

                reader.BaseStream.Position += 0x1;

                width = reader.ReadUInt16();
                reader.BaseStream.Position += 0x2;
                height = reader.ReadUInt16();
                reader.BaseStream.Position += 0x2;
                length = reader.ReadUInt16();
                
                //reader.BaseStream.Position += 0x2;
            }
        }
        public Building() {
            modelID = 0;
            xFraction = 0;
            xPosition = 0;
            yFraction = 0;
            yPosition = 1;
            zFraction = 0;
            zPosition = 0;

            xRotation = yRotation = zRotation = 0;
            width = 16;
            height = 16;
            length = 16;
        }

        public Building(Building toCopy) {
            modelID = toCopy.modelID;
            xFraction = toCopy.xFraction;
            xPosition = toCopy.xPosition;
            yFraction = toCopy.yFraction;
            yPosition = (short)(toCopy.yPosition + 1);
            zFraction = toCopy.zFraction;
            zPosition = toCopy.zPosition;

            xRotation = toCopy.xRotation;
            yRotation = toCopy.yRotation;
            zRotation = toCopy.zRotation;

            width = toCopy.width;
            height = toCopy.height;
            length = toCopy.length;
        }
        #endregion Constructors
        public static ushort DegToU16(float deg) {
            return (ushort)(deg * 65536 / 360);
        }
        public static float U16ToDeg(ushort u16) {
            return (float)u16 * 360 / 65536;
        }
        public void LoadModelData(string dir) {
            LoadModelDataFromID((int)modelID, dir);
        }
        public void LoadModelDataFromID(int modelID, string bmDir) {
            string modelPath = bmDir + "\\" + modelID.ToString("D4");

            if (string.IsNullOrWhiteSpace(modelPath) || !File.Exists(modelPath)) {
                MessageBox.Show("Building " + modelID + " could not be found in\n" + '"' + bmDir + '"', "Building not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try {
                using (Stream fs = new FileStream(modelPath, FileMode.Open)) {
                    this.NSBMDFile = NSBMDLoader.LoadNSBMD(fs);
                }
            } catch (FileNotFoundException) {
                MessageBox.Show("Building " + modelID + " could not be found in\n" + '"' + bmDir + '"', "Building not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}