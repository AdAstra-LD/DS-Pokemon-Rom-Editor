using System.IO;
namespace DSPRE
{
	/// <summary>
	/// Class to store area data in Pokémon NDS games
	/// </summary>
	public class AreaData
	{
        #region Fields (2)
        public ushort buildingsTileset;
        public ushort mapTileset;
        public ushort dynamicTextureType;
        public ushort unknown1;
        public byte areaType;
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
        public byte[] Save(string gameVersion)
        {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData))
            {
                writer.Write(buildingsTileset);
                writer.Write(mapTileset);

                if (gameVersion == "D" || gameVersion == "P" || gameVersion == "Plat") {
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
        #endregion
    }
}