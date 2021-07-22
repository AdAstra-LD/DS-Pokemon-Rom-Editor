using System.IO;
using static DSPRE.RomInfo;

namespace DSPRE.ROMFiles {
    /// <summary>
    /// Class to store area data in Pokémon NDS games
    /// </summary>
    public class AreaData : RomFile {
        internal static readonly byte TYPE_INDOOR = 0;
        internal static readonly byte TYPE_OUTDOOR = 1;

        #region Fields (2)
        public ushort buildingsTileset;
        public ushort mapTileset;
        public ushort dynamicTextureType;
        public ushort unknown1;
        public byte areaType = TYPE_OUTDOOR;
        public ushort lightType; //using an overabundant size. HGSS only needs a byte
        #endregion

        #region Constructors (1)
        public AreaData(Stream data) {
            using (BinaryReader reader = new BinaryReader(data)) {
                buildingsTileset = reader.ReadUInt16();
                mapTileset = reader.ReadUInt16();

                if (RomInfo.gameFamily.Equals("HGSS")) {
                    dynamicTextureType = reader.ReadUInt16();
                    areaType = reader.ReadByte();
                    lightType = reader.ReadByte();
                } else {
                    unknown1 = reader.ReadUInt16();
                    lightType = reader.ReadUInt16();
                }
            }
        }
        public AreaData (byte ID) : this(new FileStream(RomInfo.gameDirs[DirNames.areaData].unpackedDir + "//" + ID.ToString("D4"), FileMode.Open)) {}
        #endregion

        #region Methods (1)
        public override byte[] ToByteArray() {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData)) {
                writer.Write(buildingsTileset);
                writer.Write(mapTileset);

                if (RomInfo.gameFamily.Equals("HGSS")) {
                    writer.Write(dynamicTextureType);
                    writer.Write(areaType);
                    writer.Write((byte)lightType);
                } else {
                    writer.Write(unknown1);
                    writer.Write((ushort)lightType);
                }
            }
            return newData.ToArray();

        }

        public void SaveToFileDefaultDir(int IDtoReplace, bool showSuccessMessage = true) {
            SaveToFileDefaultDir(DirNames.areaData, IDtoReplace, showSuccessMessage);
        }

        public void SaveToFileExplorePath(string suggestedFileName, bool showSuccessMessage = true) {
            SaveToFileExplorePath("Gen IV Area Data File", "bin", suggestedFileName, showSuccessMessage);
        }
        #endregion
    }
}