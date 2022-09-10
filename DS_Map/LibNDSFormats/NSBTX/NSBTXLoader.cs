// Loader for NSBTX files/data.
// Code adapted from kiwi.ds' NSBMD Model Viewer.

using System;
using System.Collections.Generic;
using System.IO;
using LibNDSFormats.NSBMD;
using System.Linq;
using System.Windows.Forms;

namespace LibNDSFormats.NSBTX
{
    /// <summary>
    /// Loader for NSBTX files & data.
    /// </summary>
    public static class NSBTXLoader
    {
        public static object MessageBoxIcons { get; private set; }
        #region Methods (2)

        // Public Methods (2) 

        /// <summary>
        /// Load NSBTX from stream.
        /// </summary>
        /// <param name="stream">Stream to use.</param>
        /// <returns>Material definitions.</returns>
        public static IEnumerable<NSBMDMaterial> LoadNsbtx(Stream stream, out List<NSBMDTexture> texs, out List<NSBMDPalette> pals)
        {
            texs = new List<NSBMDTexture>();
            pals = new List<NSBMDPalette>();
            var materials = new List<NSBMDMaterial>();
            var reader = new BinaryReader(stream);
            int id = reader.ReadInt32();
            if (id != NSBMD.NSBMD.NDS_TYPE_BTX0) {
                //Console.WriteLine("The header of this texture file is null!!!");
                return null;
            }

            reader.BaseStream.Position += 2;
            int i = reader.ReadUInt16();
            if (i == NSBMD.NSBMD.NDS_TYPE_UNK1) {
                i = reader.ReadInt32();

                if (i == stream.Length) {
                    int numblock = reader.ReadInt32();
                    numblock >>= 16;
                    int r = reader.ReadInt32();
                    id = reader.ReadInt32();

                    if (numblock == 1 && r == 0x14) {
                        int texnum;
                        int palnum;
                        materials.AddRange(ReadTex0(stream, 0x14, out texnum, out palnum, out texs, out pals));
                    }
                }
            }
            return materials;
        }

        /// <summary>
        /// Load NSBTX from file.
        /// </summary>
        /// <param name="stream">File to use.</param>
        /// <returns>Material definitions.</returns>
        public static IEnumerable<NSBMDMaterial> LoadNsbtx(FileInfo fileInfo, out List<NSBMDTexture> texs, out List<NSBMDPalette> pals)
        {
            texs = new List<NSBMDTexture>();
            pals = new List<NSBMDPalette>();
            IEnumerable<NSBMDMaterial> result = null;
            using (var fileStream = new FileStream(fileInfo.FullName, FileMode.Open))
            {
                result = LoadNsbtx(fileStream, out texs, out pals);
            }
            return result;
        }

        /// <summary>
        /// Load materials in stream.
        /// </summary>
        /// <param name="stream">Stream to use.</param>
        /// <returns>Material definitions.</returns>
        public static IEnumerable<NSBMDMaterial> ReadTex0(Stream stream, int blockoffset, out int ptexnum, out int ppalnum, out List<NSBMDTexture> texs, out List<NSBMDPalette> pals)
        {
            EndianBinaryReader reader = new EndianBinaryReader(stream, Endianness.LittleEndian);
            UInt32 blocksize, blockptr, blocklimit;
            int texnum;
            UInt32 texdataoffset;
            int texdatasize;
            UInt32 sptexoffset; // for 4x4 compressed texels only
            int sptexsize; // for 4x4 compressed texels only
            UInt32 spdataoffset; // for 4x4 compressed texels only
            int palnum;
            UInt32 paldefoffset;
            UInt32 paldataoffset;
            int paldatasize;
            NSBMDMaterial[] material = null;
            int i, j;
            texs = new List<NSBMDTexture>();
            pals = new List<NSBMDPalette>();

            blockptr = (uint)(blockoffset + 4); // already read the block ID, so skip 4 bytes
            blocksize = reader.ReadUInt32(); // block size
            blocklimit = (uint)(blocksize + blockoffset);
            //Console.WriteLine("DEBUG: blockoffset = {0}, blocksize = {1}", blockoffset, blocksize);

            stream.Skip(4); // skip 4 padding 0s
            texdatasize = reader.ReadUInt16() << 3; // total texture data size div8
            stream.Skip(6); // skip 6 bytes
            texdataoffset = (uint)(reader.ReadUInt32() + blockoffset);

            stream.Skip(4); // skip 4 padding 0s
            sptexsize = reader.ReadUInt16() << 3; // for format 5 4x4-texel, data size div8
            stream.Skip(6); // skip 6 bytesmhm
            sptexoffset = (uint)(reader.ReadUInt32() + blockoffset); // for format 5 4x4-texel, data offset
            spdataoffset = (uint)(reader.ReadUInt32() + blockoffset); // for format 5 4x4-texel, palette info

            stream.Skip(4); // skip 4 bytes
            paldatasize = reader.ReadUInt16() << 3; // total palette data size div8
            stream.Skip(2); // skip 2 bytes
            paldefoffset = (uint)(reader.ReadUInt32() + blockoffset);
            paldataoffset = (uint)(reader.ReadUInt32() + blockoffset);

            //	printf( "texdataoffset = %08x texdatasize = %08x\n", texdataoffset, texdatasize );
            //	printf( "sptexoffset = %08x sptexsize = %08x spdataoffset = %08x\n", sptexoffset, sptexsize, spdataoffset );
            //	printf( "paldataoffset = %08x paldatasize = %08x\n", paldataoffset, paldatasize );

            ////////////////////////////////////////////////
            // texture definition

            stream.Skip(1); // skip dummy '0'
            texnum = reader.ReadByte(); // no of texture
            blockptr = (uint)stream.Position;
            stream.Seek(paldefoffset, SeekOrigin.Begin);
            stream.Skip(1); // skip dummy '0'
            palnum = reader.ReadByte(); // no of palette
            stream.Seek(blockptr, SeekOrigin.Begin);

            //Console.WriteLine("texnum = {0}, palnum = {1}", texnum, palnum);

            // allocate memory for material, great enough to hold all texture and palette
            material = new NSBMDMaterial[(texnum > palnum ? texnum : palnum)];
            for (i = 0; i < material.Length; i++)
                material[i] = new NSBMDMaterial();


            stream.Skip(14 + (texnum * 4)); // go straight to texture info

            for (i = 0; i < texnum; i++)
            {
                UInt32 offset;
                int param;
                int format;
                int width;
                int height;

                var mat = material[i];

                offset = (uint)(reader.ReadUInt16() << 3);
                param = reader.ReadUInt16(); // texture parameter
                stream.Skip(4); // skip 4 bytes

                format = (param >> 10) & 7; // format 0..7, see DSTek
                width = 8 << ((param >> 4) & 7);
                height = 8 << ((param >> 7) & 7);
                mat.color0 = (param >> 13) & 1;

                if (format == 5)
                    mat.texoffset = offset + sptexoffset; // 4x4-Texel Compressed Texture
                else
                    mat.texoffset = offset + texdataoffset;

                mat.format = format;
                mat.width = width;
                mat.height = height;
                NSBMDTexture t = new NSBMDTexture();
                t.format = format;
                t.width = width;
                t.height = height;
                t.color0 = (param >> 13) & 1;
                texs.Add(t);
            }

            ////////////////////////////////////////////
            // copy texture names
            for (i = 0; i < texnum; i++)
            {
                material[i].texname = Utils.ReadNSBMDString(reader);
                reader.BaseStream.Position -= 16;
                texs[i].texname = Utils.ReadNSBMDString(reader);
            }

            ////////////////////////////////////////////////
            // calculate each texture's size
            for (i = 0; i < texnum; i++)
            {
                int[] bpp = { 0, 8, 2, 4, 8, 2, 8, 16 };

                var mat = material[i];
                mat.texsize = (uint)(mat.width * mat.height * bpp[mat.format] / 8);
                //Console.WriteLine("tex {0} '{1}': offset = {2} size = {3} [W,H] = [{4}, {5}]",
                //i, mat.texname, mat.texoffset, mat.texsize, mat.width, mat.height);
                texs[i].texsize = (uint)(mat.width * mat.height * bpp[mat.format] / 8);
            }

            ////////////////////////////////////////////////
            // palette definition
            stream.Seek(paldefoffset + 2, SeekOrigin.Begin); // skip palnum, already read
            stream.Seek(14 + (palnum * 4), SeekOrigin.Current); // go straight to palette info
            for (i = 0; i < palnum; i++)
            {
                uint curOffset = (uint)((reader.ReadUInt16() << 3) + paldataoffset);
                stream.Seek(2, SeekOrigin.Current); // skip 2 bytes
                material[i].paloffset = curOffset;
                NSBMDPalette t = new NSBMDPalette();
                t.paloffset = curOffset;
                pals.Add(t);
            }

            ////////////////////////////////////////////////
            // copy palette names
            for (i = 0; i < palnum; i++)
            {
                var mat = material[i];
                mat.palname = Utils.ReadNSBMDString(reader);
                reader.BaseStream.Position -= 16;
                pals[i].palname = Utils.ReadNSBMDString(reader);
            }

            ////////////////////////////////////////////////
            // calculate each palette's size
            // assume the palettes are stored sequentially
            /*for (i = 0; i < palnum - 1; i++)
            {
                int r;
                var mat = material[i];
                r = i;
                try { while (material[r].paloffset == mat.paloffset) r++; }
                catch { }
                // below is RotA stupid way to calculate the size of palette: next's offset - current's offset
                // it works most of the time
                if (r != palnum)
                {
                    mat.palsize = material[r].paloffset - mat.paloffset;
                    pals[i].palsize = material[r].paloffset - mat.paloffset;
                }
                else
                {
                    mat.palsize = blocklimit - mat.paloffset;
                    pals[i].palsize = blocklimit - mat.paloffset;
                }
                //printf("pal '%s' size = %d\n", mat->palname, mat->palsize);
            }
            material[i].palsize = blocklimit - material[i].paloffset;
            pals[i].palsize = blocklimit - material[i].paloffset;*/
            List<int> offsets = new List<int>();
            for (int k = 0; k < pals.Count; k++)
            {
                if (!offsets.Contains((int)pals[k].paloffset))
                {
                    offsets.Add((int)pals[k].paloffset);
                }
            }
            offsets.Add((int)blocklimit);
            offsets.Sort();
            for (int k = 0; k < pals.Count; k++)
            {
                int pallength;
                int l = -1;
                do
                {
                    l++;
                }
                while (offsets[l] - pals[k].paloffset <= 0);//nsbtx.PalInfo.infoBlock.PalInfo[i + j].Palette_Offset - nsbtx.PalInfo.infoBlock.PalInfo[i].Palette_Offset == 0)
                pallength = offsets[l] - (int)pals[k].paloffset;
                //RGBA[] c_ = pals[k].paldata;
                //List<RGBA> c = new List<RGBA>();
                //c.AddRange(pals[k].paldata.Take(pallength / 2));
                //pals[k].paldata = c.ToArray();
                pals[k].palsize = (uint)pallength;
                material[k].palsize = (uint)pallength;
            }

            ////////////////////////////////////////////////
            // traverse each texture
            for (i = 0; i < texnum; i++)
            {
                var mat = material[i];
                stream.Seek(mat.texoffset, SeekOrigin.Begin);

                ////////////////////////////////////////////////
                // read texture into memory
                byte[] by = reader.ReadBytes((int)mat.texsize);
                mat.texdata = by;
                texs[i].texdata = by;

                //Console.WriteLine("DEBUG: texoffset = {0}, texsize = {1}", mat.texoffset, mat.texsize);

                ////////////////////////////////////////////////
                // additional data for format 5 4x4 compressed texels
                if (mat.format == 5)
                {
                    UInt32 r = mat.texsize / 2;//>> 1;
                    stream.Seek(spdataoffset + (mat.texoffset - sptexoffset) / 2, SeekOrigin.Begin);

                    by = reader.ReadBytes((int)r);
                    mat.spdata = by;
                    texs[i].spdata = by;
                    //Console.WriteLine("DEBUG: 4x4-texel spdataoffset = {0}, spdatasize = {1}", spdataoffset, r);

                    //spdataoffset += r;
                }
            }


            ////////////////////////////////////////////////
            // traverse each palette
            for (i = 0; i < palnum; i++)
            {
                try
                {
                    NSBMDMaterial mat = material[i];
                    var palentry = mat.palsize >> 1;

                    RGBA[] rgbq = new RGBA[palentry];

                    //Console.WriteLine("DEBUG: converting pal '{0}', palentry = {1}", mat.palname, palentry);

                    stream.Seek(mat.paloffset, SeekOrigin.Begin);
                    for (j = 0; j < palentry; j++)
                    {
                        ushort p = reader.ReadUInt16();
                        rgbq[j].R = (byte)(((p >> 0) & 0x1f) << 3); // red
                        rgbq[j].G = (byte)(((p >> 5) & 0x1f) << 3); // green
                        rgbq[j].B = (byte)(((p >> 10) & 0x1f) << 3); // blue
                        //rgbq[j].RotA = (p&0x8000) ? 0xff : 0;
                        rgbq[j].A = (p & 0x8000) == 0 ? (byte)0xff : (byte)0;//0xff; // alpha
                    }
                    mat.paldata = rgbq;
                    pals[i].paldata = rgbq;
                }
                catch
                {
                }
            }

            ptexnum = texnum;
            ppalnum = palnum;

            return material;
        }

        #endregion Methods
    }
}