using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace DSPRE
{
   /* ---------------------- MATRIX DATA STRUCTURE (DPPtHGSS):-----------------------------

   0x0  //  byte:       Matrix width (a.k.a row length) (x)
   0x1  //  byte:       Matrix height (a.k.a number of rows per section) (y)
   0x2  //  byte:       Headers section boolean (0 = not present, 1 = present)
   0x3  //  byte:       Altitudes section flag (0 = not present, 1 = present)
   0x4  //  byte:       Length of matrix name string
   0x5  //  string:     Matrix name string (UTF-8 Encoded)
   --   //              [Header section if applicable: y blocks of length x]
   --   //              [Altitudes section if applicable: y blocks of length x]
   --   //              [Map files section: y blocks of length x]

   -------------------------------------------------------------------------------------- */

    /// <summary>
    /// Class to store map matrix data from Pokémon NDS games
    /// </summary>
    public class Matrix
	{
        #region Fields (8)
        public bool hasHeadersSection { get; set; }
        public bool hasHeightsSection { get; set; }
        public string name { get; set; }
        public byte width { get; set; }
        public byte height { get; set; }
        public ushort[,] headers;
        public byte[,] altitudes;
        public ushort[,] maps;

        public static readonly ushort EMPTY = 65535;
        #endregion Fields

        #region Constructors(1)
        public Matrix(Stream data) {
            using (BinaryReader reader = new BinaryReader(data)) {
                /* Read matrix size and sections included */
                width = reader.ReadByte();
                height = reader.ReadByte();
                if (reader.ReadBoolean()) hasHeadersSection = true;
                if (reader.ReadBoolean()) hasHeightsSection = true;
                
                /* Read matrix's name */
                byte nameLength = reader.ReadByte();
                name = Encoding.UTF8.GetString(reader.ReadBytes(nameLength));

                /* Initialize section arrays */
                headers = new ushort[height, width];
                altitudes = new byte[height, width];
                maps = new ushort[height, width];

                /* Read sections */
                if (hasHeadersSection) 
                    for (int i = 0; i < height; i++) 
                        for (int j = 0; j < width; j++) 
                            headers[i, j] = reader.ReadUInt16();

                if (hasHeightsSection) 
                    for (int i = 0; i < height; i++) 
                        for (int j = 0; j < width; j++) 
                            altitudes[i, j] = reader.ReadByte();     
                
                for (int i = 0; i < height; i++) 
                    for (int j = 0; j < width; j++) 
                        maps[i, j] = reader.ReadUInt16();
            }
        }
        public Matrix(int ID) : this (new FileStream(RomInfo.matrixDirPath + "\\" + ID.ToString("D4"), FileMode.Open)) { }
        #endregion

        #region Methods (6)
        public void ResizeMatrix(int newHeight, int newWidth)
        {
            /*  Initialize new arrays   */
            ushort[,] newHeaders = new ushort[newHeight, newWidth];
            byte[,] newAltitudes = new byte[newHeight, newWidth];
            ushort[,] newMaps = new ushort[newHeight, newWidth];

            /* Copy existing headers and altitudes rows into new arrays. If new matrix is greater in any dimension, new entries will be zero */
            if (hasHeadersSection) 
                for (int i = 0; i < Math.Min(height, newHeight); i++) 
                    for (int j = 0; j < Math.Min(width, newWidth); j++) 
                        newHeaders[i, j] = headers[i, j];
            if (hasHeightsSection) 
                for (int i = 0; i < Math.Min(height, newHeight); i++) 
                    for (int j = 0; j < Math.Min(width, newWidth); j++) 
                        newAltitudes[i, j] = altitudes[i, j];

            /* Copy existing map rows into new array, and fill eventual new ones with Matrix.EMPTY (FF FF) */
            for (int i = 0; i < Math.Min(height, newHeight); i++) 
                for (int j = 0; j < Math.Min(width, newWidth); j++) 
                    newMaps[i, j] = maps[i, j];

            if (newHeight > height) 
                for (int i = height; i < newHeight; i++) 
                    for (int j = 0; j < newWidth; j++) 
                        newMaps[i, j] = Matrix.EMPTY;

            if (newWidth > width)   
                for (int j = width; j < newWidth; j++) 
                    for (int i = 0; i < newHeight; i++) 
                        newMaps[i, j] = Matrix.EMPTY;

            /* Substitute old arrays with new arrays */
            headers = newHeaders;
            altitudes = newAltitudes;
            maps = newMaps;

            /* Set new width and height */
            height = (byte)newHeight;
            width = (byte)newWidth;
        }
        public byte[] ToByteArray() {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData)) {
                writer.Write(width);
                writer.Write(height);
                writer.Write(hasHeadersSection);
                writer.Write(hasHeightsSection);
                writer.Write(name);

                if (hasHeadersSection) {
                    for (int i = 0; i < height; i++)
                        for (int j = 0; j < width; j++)
                            writer.Write(headers[i, j]);
                }

                if (hasHeightsSection) {
                    for (int i = 0; i < height; i++)
                        for (int j = 0; j < width; j++)
                            writer.Write(altitudes[i, j]);
                }

                for (int i = 0; i < height; i++) {
                    for (int j = 0; j < width; j++)
                        writer.Write(maps[i, j]);
                }

            }
            return newData.ToArray();
        }
        public void SaveToFile(string path) {
            using (BinaryWriter writer = new BinaryWriter(new FileStream(path, FileMode.Create)))
                writer.Write(this.ToByteArray());
        }
        public void SaveToFileDefaultDir(int IDtoReplace) {
            string path = RomInfo.matrixDirPath + "\\" + IDtoReplace.ToString("D4");
            this.SaveToFile(path);
        }
        public void SaveToFileExplorePath(string suggestedFileName) {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "Gen IV Matrix File (*.mtx)|*.mtx";

            if (suggestedFileName != null && suggestedFileName != "")
                sf.FileName = suggestedFileName;
            if (sf.ShowDialog() != DialogResult.OK)
                return;

            this.SaveToFile(sf.FileName);
        }
        #endregion

    }
}