using System.IO;
using static DSPRE.RomInfo;

namespace DSPRE.ROMFiles {
    /// <summary>
    /// Class to store area data in Pokémon NDS games
    /// </summary>
    public class AreaData : RomFile
	{
        internal static readonly byte TYPE_INDOOR = 0;
        internal static readonly byte TYPE_OUTDOOR = 1;

        #region Fields (2)
        public ushort buildingsTileset;
        public ushort mapTileset;
        public ushort dynamicTextureType;
        public ushort unknown1;
        public byte areaType = TYPE_OUTDOOR;
        public byte lightType;
        #endregion

        #region Constructors (1)
        public AreaData(Stream data, string gameVersion)
		{
            using (BinaryReader reader = new BinaryReader(data))
            {
                buildingsTileset = reader.ReadUInt16();
                mapTileset = reader.ReadUInt16();

                if (gameVersion == "D" || gameVersion == "P" || gameVersion == "Plat") {
                    unknown1 = reader.ReadUInt16();
                    lightType = reader.ReadByte();
                } else {
                    dynamicTextureType = reader.ReadUInt16();
                    areaType = reader.ReadByte();
                    lightType = reader.ReadByte();
                }
            }
        }
        #endregion

        #region Methods (1)
        public override byte[] ToByteArray() {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData))
            {
                writer.Write(buildingsTileset);
                writer.Write(mapTileset);

                if (RomInfo.gameFamily.Equals("DP") || RomInfo.gameFamily.Equals("Plat")) {
                    writer.Write(unknown1);
                    writer.Write(lightType);
                } else {
                    writer.Write(dynamicTextureType);
                    writer.Write(areaType);
                    writer.Write(lightType);
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