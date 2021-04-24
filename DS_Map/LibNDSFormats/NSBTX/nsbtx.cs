/*
*   This file is part of NSMB Editor 5.
*
*   NSMB Editor 5 is free software: you can redistribute it and/or modify
*   it under the terms of the GNU General Public License as published by
*   the Free Software Foundation, either version 3 of the License, or
*   (at your option) any later version.
*
*   NSMB Editor 5 is distributed in the hope that it will be useful,
*   but WITHOUT ANY WARRANTY; without even the implied warranty of
*   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*   GNU General Public License for more details.
*
*   You should have received a copy of the GNU General Public License
*   along with NSMB Editor 5.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using NSMBe4.DSFileSystem;

namespace NSMBe4.NSBMD
{
    public class NSBTX
    {
        DSFileSystem.File f;
        byte[] data;
        int texDataOffset;
        int palDataOffset;
        int palDefOffset;
        int palDataSize;
        int f5texDataOffset;
        int f5dataOffset;
        public List<Palette> pal = new List<Palette>();
        public PalettedImage[] textures;
        public PaletteDef[] palettes;
        public ByteArrayInputStream str;
        public int startoffset = 0;

        public NSBTX(DSFileSystem.File f)
        {
            this.f = f;
            data = f.getContents();
            str = new ByteArrayInputStream(data);

            bool LZd = false;
            if (str.readUInt() == 0x37375A4C) //LZ77
            {
                byte[] ndata = new byte[data.Length - 4];
                Array.Copy(data, 4, ndata, 0, ndata.Length);

                data = ROM.LZ77_Decompress(ndata);
                str = new ByteArrayInputStream(data);
                LZd = true;
            }

            //look for TEX0 block
            //ugly, but i'm lazy to implement it properly.
            bool found = false;
            int blockStart = 0;
            while (str.lengthAvailable(4))
            {
                uint v = str.readUInt();
                if (v == 0x30584554) // "TEX0"
                {
                    str.setOrigin(str.getPos() - 4);
                    blockStart = (int)(str.getPos() - 4);
                    found = true;
                    startoffset = (int)str.getPos() - 4;
                    break;
                }
                //                else
                //                    str.skipback(3); //just in case its not word-aligned
            }
            str.seek(0);
            if (!found)
            {
                textures = new PalettedImage[0];
                palettes = new PaletteDef[0];
                return;
            }

            Console.Out.WriteLine("\n");
            //Read stuff
            str.seek(0x14);
            texDataOffset = str.readInt() + blockStart;
            Console.Out.WriteLine("Texdata " + texDataOffset.ToString("X8"));

            str.seek(0x24);
            f5texDataOffset = str.readInt() + blockStart;
            Console.Out.WriteLine("f5Texdata " + f5texDataOffset.ToString("X8"));
            f5dataOffset = str.readInt() + blockStart;
            Console.Out.WriteLine("f5data " + f5dataOffset.ToString("X8"));

            str.seek(0x30);
            palDataSize = str.readInt() * 8;
            Console.Out.WriteLine("paldata size " + palDataSize.ToString("X8"));
            str.seek(0x34);
            palDefOffset = str.readInt();
            Console.Out.WriteLine("paldef " + palDefOffset.ToString("X8"));
            palDataOffset = str.readInt();
            Console.Out.WriteLine("paldata " + palDataOffset.ToString("X8"));

            //Read texture definitions
            str.seek(0x3D);
            textures = new PalettedImage[str.readByte()];
            str.skip((uint)(0xE + textures.Length * 4));

            //ImageManagerWindow mgr = new ImageManagerWindow();
            //mgr.Text = f.name + " - Texture Editor";

            bool hasFormat5 = false;
            for (int i = 0; i < textures.Length; i++)
            {
                int offset = 8 * str.readUShort();
                ushort param = str.readUShort();
                int format = (param >> 10) & 7;

                if (format == 5)
                    offset += f5texDataOffset;
                else
                    offset += texDataOffset;

                int width = 8 << ((param >> 4) & 7);
                int height = 8 << ((param >> 7) & 7);
                bool color0 = ((param >> 13) & 1) != 0;
                str.readUInt(); // unused

                int size = width * height * Image3D.bpps[format] / 8;
                Console.Out.WriteLine(offset.ToString("X8") + " " + format + " " + width + "x" + height + " " + color0 + " LZ" + LZd);

                InlineFile mainfile = new InlineFile(f, offset, size, Image3D.formatNames[format], null, LZd ? InlineFile.CompressionType.LZWithHeaderComp : InlineFile.CompressionType.NoComp);
                if (format == 5)
                {
                    hasFormat5 = true;
                    int f5size = (width * height) / 16 * 2;
                    InlineFile f5file = new InlineFile(f, f5dataOffset, f5size, Image3D.formatNames[format], null, LZd ? InlineFile.CompressionType.LZWithHeaderComp : InlineFile.CompressionType.NoComp);

                    f5dataOffset += f5size;
                    textures[i] = new Image3Dformat5(mainfile, f5file, width, height);
                }
                else
                {
                    textures[i] = new Image3D(mainfile, color0, width, height, format, offset);
                }

                //                textures[i] = new Texture(this, color0, width, height, format, offset, "");
                /*                if (format == 5)
                                {
                                    textures[i].f5DataOffset = f5dataOffset;
                                    f5dataOffset += (uint)(width * height) / 16 * 2;
                                }*/
            }

            for (int i = 0; i < textures.Length; i++)
            {
                if (textures[i] is null) continue;
                textures[i].name = str.ReadString(16);
                //mgr.m.addImage(textures[i]);
            }

            //Read palette definitions
            str.seek(palDefOffset + 1);
            palettes = new PaletteDef[str.readByte()];
            str.skip((uint)(0xE + palettes.Length * 4));

            for (int i = 0; i < palettes.Length; i++)
            {
                int offset = 8 * str.readUShort() + palDataOffset + blockStart;
                str.readUShort();
                palettes[i] = new PaletteDef();
                palettes[i].offs = offset;
            }

            Array.Sort(palettes);

            for (int i = 0; i < palettes.Length; i++) {
                palettes[i].name = str.ReadString(16);
                if (i != palettes.Length - 1)
                    palettes[i].size = palettes[i + 1].offs - palettes[i].offs;

            }
            palettes[palettes.Length - 1].size = blockStart + palDataOffset + palDataSize - palettes[palettes.Length - 1].offs;

            for (int i = 0; i < palettes.Length; i++) {
                if (hasFormat5) {
                    FilePalette pa = new FilePalette(new InlineFile(f, palettes[i].offs, palettes[i].size, palettes[i].name, null, LZd ? InlineFile.CompressionType.LZWithHeaderComp : InlineFile.CompressionType.NoComp));
                    pal.Add((NSMBe4.Palette)pa);
                    //mgr.m.addPalette(pa);
                } else {
                    int extrapalcount = (palettes[i].size) / 512;
                    for (int j = 0; j < extrapalcount; j++) {
                        FilePalette pa = new FilePalette(new InlineFile(f, palettes[i].offs + j * 512, 512, palettes[i].name + ":" + j, null, LZd ? InlineFile.CompressionType.LZWithHeaderComp : InlineFile.CompressionType.NoComp));
                        pal.Add((NSMBe4.Palette)pa);
                        //mgr.m.addPalette(pa);
                    }
                    int lastsize = palettes[i].size % 512;
                    if (lastsize != 0) {
                        FilePalette pa = new FilePalette(new InlineFile(f, palettes[i].offs + extrapalcount * 512, lastsize, palettes[i].name + ":" + extrapalcount, null, LZd ? InlineFile.CompressionType.LZWithHeaderComp : InlineFile.CompressionType.NoComp));
                        pal.Add((NSMBe4.Palette)pa);
                        //mgr.m.addPalette(pa);
                    }
                }
            }

            //mgr.Show();

            //            new ImagePreviewer(textures[0].render(palettes[0])).Show();
        }

        public void close() {
            f.endEdit(this);
        }

        public byte[] save() {
            f.replace(str.getData(), this);
            return str.getData();
        }

        public class PaletteDef : IComparable<PaletteDef> {
            public int offs, size;
            public string name;

            public int CompareTo(PaletteDef b) {
                return offs.CompareTo(b.offs);
            }
        }
    }
    public class NSBTX_File
    {
        public byte[] before;
        public byte[] after;
        public header Header;
        public struct header {
            public string ID;
            public byte[] Magic;
            public Int32 file_size;
            public Int16 header_size;
            public Int16 nSection;
            public Int32[] Section_Offset;
        }
        public tex0 TEX0;
        public struct tex0 {
            public string ID;
            public Int32 Section_size;
            public Int32 Padding1;
            public Int32 Texture_Data_Size; //shift << 3
            public Int16 Texture_Info_Offset;
            public Int32 Padding2;
            public Int32 Texture_Data_Offset;
            public Int32 Padding3;
            public Int32 Compressed_Texture_Data_Size; //shift << 3
            public Int16 Compressed_Texture_Info_Offset;
            public Int32 Padding4;
            public Int32 Compressed_Texture_Data_Offset;
            public Int32 Compressed_Texture_Info_Data_Offset;
            public Int32 Padding5;
            public Int32 Palette_Data_Size; //shift << 3
            public Int32 Palette_Info_Offset;
            public Int32 Palette_Data_Offset;
        }
        public texInfo TexInfo;
        public struct texInfo
        {
            public byte dummy;
            public byte num_objs;
            public short section_size;
            public UnknownBlock unknownBlock;
            public Info infoBlock;
            public List<string> names;

            public struct UnknownBlock
            {
                public short header_size;
                public short section_size;
                public int constant; // 0x017F

                public List<short> unknown1;
                public List<short> unknown2;
            }
            public struct Info {
                public short header_size;
                public short data_size;

                public texInfo[] TexInfo;

                public struct texInfo {
                    public Int32 Texture_Offset; //shift << 3, relative to start of Texture Data
                    public Int16 Parameters;
                    public byte Width;
                    public byte Unknown1;
                    public byte Height;
                    public byte Unknown2;

                    public byte[] Image;
                    public byte[] spData;
                    // Parameters
                    public byte repeat_X;   // 0 = freeze; 1 = repeat
                    public byte repeat_Y;   // 0 = freeze; 1 = repeat
                    public byte flip_X;     // 0 = no; 1 = flip each 2nd texture (requires repeat)
                    public byte flip_Y;     // 0 = no; 1 = flip each 2nd texture (requires repeat)
                    public ushort width;      // 8 << width
                    public ushort height;     // 8 << height
                    public byte format;     // Texture format
                    public byte color0; // 0 = displayed; 1 = transparent
                    public byte coord_transf; // Texture coordination transformation mode

                    public byte depth;
                    public uint compressedDataStart;
                }
            }
        }
        public palInfo PalInfo;
        public struct palInfo {
            public byte dummy;
            public byte num_objs;
            public short section_size;
            public UnknownBlock unknownBlock;
            public Info infoBlock;
            public List<string> names;

            public struct UnknownBlock {
                public short header_size;
                public short section_size;
                public int constant; // 0x017F

                public List<short> unknown1;
                public List<short> unknown2;
            }
            public struct Info {
                public short header_size;
                public short data_size;

                public palInfo[] PalInfo;

                public struct palInfo
                {
                    public Int32 Palette_Offset; //shift << 3, relative to start of Palette Data
                    public Int16 Color0;
                    public Color[] pal;
                }
            }
        }

        int[] bpp = { 0, 8, 2, 4, 8, 2, 8, 16 };

        public NSBTX_File(FileStream f) {
            EndianBinaryReader er = new EndianBinaryReader(f, Endianness.LittleEndian);
            if (f.Length <= 4) {
                MessageBox.Show("Error: Texture file is too small.", null, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                er.Close();
                return;
            }
            if (er.ReadString(Encoding.ASCII, 4) != "BTX0") {
                MessageBox.Show("Error: BTX header is wrong.", null, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                er.Close();
                return;
            } else {
                Header.ID = "BTX0";
                Header.Magic = er.ReadBytes(4);
                Header.file_size = er.ReadInt32();
                Header.header_size = er.ReadInt16();
                Header.nSection = er.ReadInt16();
                Header.Section_Offset = new Int32[Header.nSection];
                for (int i = 0; i < Header.nSection; i++) {
                    Header.Section_Offset[i] = er.ReadInt32();
                }
                
                TEX0.ID = er.ReadString(Encoding.ASCII, 4);
                if (TEX0.ID != "TEX0") {
                    MessageBox.Show("Error", "Error");
                    er.Close();
                    return;
                }
            }
            TEX0.Section_size = er.ReadInt32();
            TEX0.Padding1 = er.ReadInt32();
            TEX0.Texture_Data_Size = (er.ReadInt16() << 3);
            TEX0.Texture_Info_Offset = er.ReadInt16();
            TEX0.Padding2 = er.ReadInt32();
            TEX0.Texture_Data_Offset = er.ReadInt32();
            TEX0.Padding3 = er.ReadInt32();
            TEX0.Compressed_Texture_Data_Size = (er.ReadInt16() << 3);
            TEX0.Compressed_Texture_Info_Offset = er.ReadInt16();
            TEX0.Padding4 = er.ReadInt32();
            TEX0.Compressed_Texture_Data_Offset = er.ReadInt32();
            TEX0.Compressed_Texture_Info_Data_Offset = er.ReadInt32();
            TEX0.Padding5 = er.ReadInt32();
            TEX0.Palette_Data_Size = (er.ReadInt32() << 3);
            TEX0.Palette_Info_Offset = er.ReadInt32();
            TEX0.Palette_Data_Offset = er.ReadInt32();

            TexInfo.dummy = er.ReadByte();
            TexInfo.num_objs = er.ReadByte();
            TexInfo.section_size = er.ReadInt16();

            TexInfo.unknownBlock.header_size = er.ReadInt16();
            TexInfo.unknownBlock.section_size = er.ReadInt16();
            TexInfo.unknownBlock.constant = er.ReadInt32();
            TexInfo.unknownBlock.unknown1 = new List<short>();
            TexInfo.unknownBlock.unknown2 = new List<short>();
            for (int i = 0; i < TexInfo.num_objs; i++)
            {
                TexInfo.unknownBlock.unknown1.Add(er.ReadInt16());
                TexInfo.unknownBlock.unknown2.Add(er.ReadInt16());
            }

            TexInfo.infoBlock.header_size = er.ReadInt16();
            TexInfo.infoBlock.data_size = er.ReadInt16();
            TexInfo.infoBlock.TexInfo = new NSBTX_File.texInfo.Info.texInfo[TexInfo.num_objs];
            for (int i = 0; i < TexInfo.num_objs; i++)
            {
                TexInfo.infoBlock.TexInfo[i].Texture_Offset = (er.ReadInt16() << 3);
                TexInfo.infoBlock.TexInfo[i].Parameters = er.ReadInt16();
                TexInfo.infoBlock.TexInfo[i].Width = er.ReadByte();
                TexInfo.infoBlock.TexInfo[i].Unknown1 = er.ReadByte();
                TexInfo.infoBlock.TexInfo[i].Height = er.ReadByte();
                TexInfo.infoBlock.TexInfo[i].Unknown2 = er.ReadByte();
                TexInfo.infoBlock.TexInfo[i].coord_transf = (byte)(TexInfo.infoBlock.TexInfo[i].Parameters & 14);
                TexInfo.infoBlock.TexInfo[i].color0 = (byte)((TexInfo.infoBlock.TexInfo[i].Parameters >> 13) & 1);
                TexInfo.infoBlock.TexInfo[i].format = (byte)((TexInfo.infoBlock.TexInfo[i].Parameters >> 10) & 7);
                TexInfo.infoBlock.TexInfo[i].height = (byte)(8 << ((TexInfo.infoBlock.TexInfo[i].Parameters >> 7) & 7));
                TexInfo.infoBlock.TexInfo[i].width = (byte)(8 << ((TexInfo.infoBlock.TexInfo[i].Parameters >> 4) & 7));
                TexInfo.infoBlock.TexInfo[i].flip_Y = (byte)((TexInfo.infoBlock.TexInfo[i].Parameters >> 3) & 1);
                TexInfo.infoBlock.TexInfo[i].flip_X = (byte)((TexInfo.infoBlock.TexInfo[i].Parameters >> 2) & 1);
                TexInfo.infoBlock.TexInfo[i].repeat_Y = (byte)((TexInfo.infoBlock.TexInfo[i].Parameters >> 1) & 1);
                TexInfo.infoBlock.TexInfo[i].repeat_X = (byte)(TexInfo.infoBlock.TexInfo[i].Parameters & 1);
                TexInfo.infoBlock.TexInfo[i].depth = (byte)bpp[TexInfo.infoBlock.TexInfo[i].format];
                if (TexInfo.infoBlock.TexInfo[i].width == 0x00)
                    switch (TexInfo.infoBlock.TexInfo[i].Unknown1 & 0x3)
                    {
                        case 2:
                            TexInfo.infoBlock.TexInfo[i].width = 0x200;
                            break;
                        default:
                            TexInfo.infoBlock.TexInfo[i].width = 0x100;
                            break;
                    }
                if (TexInfo.infoBlock.TexInfo[i].height == 0x00)
                    switch ((TexInfo.infoBlock.TexInfo[i].Height >> 3) & 0x3)
                    {
                        case 2:
                            TexInfo.infoBlock.TexInfo[i].height = 0x200;
                            break;
                        default:
                            TexInfo.infoBlock.TexInfo[i].height = 0x100;
                            break;
                    }
                int imgsize = (TexInfo.infoBlock.TexInfo[i].width * TexInfo.infoBlock.TexInfo[i].height * TexInfo.infoBlock.TexInfo[i].depth) / 8;
                long curpos = er.BaseStream.Position;
                if (TexInfo.infoBlock.TexInfo[i].format != 5)
                {
                    er.BaseStream.Seek(TexInfo.infoBlock.TexInfo[i].Texture_Offset + Header.Section_Offset[0] + TEX0.Texture_Data_Offset, SeekOrigin.Begin);
                }
                else
                {
                    er.BaseStream.Seek(Header.Section_Offset[0] + TEX0.Compressed_Texture_Data_Offset + TexInfo.infoBlock.TexInfo[i].Texture_Offset, SeekOrigin.Begin);
                }
                TexInfo.infoBlock.TexInfo[i].Image = er.ReadBytes(imgsize);
                er.BaseStream.Seek(curpos, SeekOrigin.Begin);

                if (TexInfo.infoBlock.TexInfo[i].format == 5)
                {
                    curpos = er.BaseStream.Position;
                    er.BaseStream.Seek(Header.Section_Offset[0] + TEX0.Compressed_Texture_Info_Data_Offset + TexInfo.infoBlock.TexInfo[i].Texture_Offset / 2, SeekOrigin.Begin);
                    TexInfo.infoBlock.TexInfo[i].spData = er.ReadBytes(imgsize / 2);
                    er.BaseStream.Seek(curpos, SeekOrigin.Begin);
                }
            }
            TexInfo.names = new List<string>();
            for (int i = 0; i < TexInfo.num_objs; i++)  TexInfo.names.Add(er.ReadString(Encoding.ASCII, 16).Replace("\0", ""));

            PalInfo.dummy = er.ReadByte();
            PalInfo.num_objs = er.ReadByte();
            PalInfo.section_size = er.ReadInt16();

            PalInfo.unknownBlock.header_size = er.ReadInt16();
            PalInfo.unknownBlock.section_size = er.ReadInt16();
            PalInfo.unknownBlock.constant = er.ReadInt32();
            PalInfo.unknownBlock.unknown1 = new List<short>();
            PalInfo.unknownBlock.unknown2 = new List<short>();
            for (int i = 0; i < PalInfo.num_objs; i++)
            {
                PalInfo.unknownBlock.unknown1.Add(er.ReadInt16());
                PalInfo.unknownBlock.unknown2.Add(er.ReadInt16());
            }

            PalInfo.infoBlock.header_size = er.ReadInt16();
            PalInfo.infoBlock.data_size = er.ReadInt16();
            PalInfo.infoBlock.PalInfo = new NSBTX_File.palInfo.Info.palInfo[PalInfo.num_objs];
            for (int i = 0; i < PalInfo.num_objs; i++)
            {
                PalInfo.infoBlock.PalInfo[i].Palette_Offset = (er.ReadInt16() << 3);
                PalInfo.infoBlock.PalInfo[i].Color0 = er.ReadInt16();
                er.BaseStream.Position -= 4;
                int palBase = er.ReadInt32();
                int palAddr = palBase & 0xfff;
                long curpos = er.BaseStream.Position;
                er.BaseStream.Seek(Header.Section_Offset[0] + TEX0.Palette_Data_Offset, SeekOrigin.Begin);
                er.BaseStream.Seek(PalInfo.infoBlock.PalInfo[i].Palette_Offset, SeekOrigin.Current);
                PalInfo.infoBlock.PalInfo[i].pal = Tinke.Convertir.BGR555(er.ReadBytes(TEX0.Palette_Data_Size - PalInfo.infoBlock.PalInfo[i].Palette_Offset));
                er.BaseStream.Seek(curpos, SeekOrigin.Begin);
            }
            PalInfo.names = new List<string>();
            for (int i = 0; i < PalInfo.num_objs; i++) PalInfo.names.Add(er.ReadString(Encoding.ASCII, 16).Replace("\0", ""));

            List<int> offsets = new List<int>();
            for (int i = 0; i < PalInfo.num_objs; i++) offsets.Add(PalInfo.infoBlock.PalInfo[i].Palette_Offset);

            offsets.Add((int)er.BaseStream.Length);
            offsets.Sort();
            for (int i = 0; i < PalInfo.num_objs; i++)
            {
                int pallength;
                int j = -1;
                do
                {
                    j++;
                }
                while (offsets[j] - PalInfo.infoBlock.PalInfo[i].Palette_Offset <= 0);
                pallength = offsets[j] - PalInfo.infoBlock.PalInfo[i].Palette_Offset;
                Color[] c_ = PalInfo.infoBlock.PalInfo[i].pal;
                List<Color> c = new List<Color>();
                c.AddRange(PalInfo.infoBlock.PalInfo[i].pal.Take(pallength / 2));
                PalInfo.infoBlock.PalInfo[i].pal = c.ToArray();
            }
            er.Close();
        }
        public bool convert_4x4texel(uint[] tex, int width, int height, UInt16[] data, Color[] pal, ImageTexeler.LockBitmap rgbaOut)
        {
            int w = width / 4;
            int h = height / 4;

            // traverse 'w x h blocks' of 4x4-texel
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    int index = y * w + x;
                    UInt32 t = tex[index];
                    UInt16 d = data[index];
                    UInt16 addr = (ushort)(d & 0x3fff);
                    UInt16 mode = (ushort)((d >> 14) & 3);

                    // traverse every texel in the 4x4 texels
                    for (int r = 0; r < 4; r++)
                        for (int c = 0; c < 4; c++)
                        {
                            int texel = (int)((t >> ((r * 4 + c) * 2)) & 3);
                            Color pixel = rgbaOut.GetPixel((x * 4 + c), (y * 4 + r));

                            switch (mode)
                            {
                                case 0:
                                    pixel = pal[(addr << 1) + texel];
                                    if (texel == 3) pixel = Color.Transparent; // make it transparent, alpha = 0
                                    break;
                                case 2:
                                    pixel = pal[(addr << 1) + texel];
                                    break;
                                case 1:
                                    switch (texel)
                                    {
                                        case 0:
                                        case 1:
                                            pixel = pal[(addr << 1) + texel];
                                            break;
                                        case 2:
                                            byte R = (byte)((pal[(addr << 1)].R + pal[(addr << 1) + 1].R) / 2L);
                                            byte G = (byte)((pal[(addr << 1)].G + pal[(addr << 1) + 1].G) / 2L);
                                            byte B = (byte)((pal[(addr << 1)].B + pal[(addr << 1) + 1].B) / 2L);
                                            byte A = 0xff;
                                            pixel = Color.FromArgb(A, R, G, B);
                                            break;
                                        case 3:
                                            pixel = Color.Transparent; // make it transparent, alpha = 0
                                            break;
                                    }
                                    break;
                                case 3:
                                    switch (texel)
                                    {
                                        case 0:
                                        case 1:
                                            pixel = pal[(addr << 1) + texel];
                                            break;
                                        case 2:
                                            {
                                                byte R = (byte)((pal[(addr << 1)].R * 5L + pal[(addr << 1) + 1].R * 3L) / 8);
                                                byte G = (byte)((pal[(addr << 1)].G * 5L + pal[(addr << 1) + 1].G * 3L) / 8);
                                                byte B = (byte)((pal[(addr << 1)].B * 5L + pal[(addr << 1) + 1].B * 3L) / 8);
                                                byte A = 0xff;
                                                pixel = Color.FromArgb(A, R, G, B);
                                                break;
                                            }
                                        case 3:
                                            {
                                                byte R = (byte)((pal[(addr << 1)].R * 3L + pal[(addr << 1) + 1].R * 5L) / 8);
                                                byte G = (byte)((pal[(addr << 1)].G * 3L + pal[(addr << 1) + 1].G * 5L) / 8);
                                                byte B = (byte)((pal[(addr << 1)].B * 3L + pal[(addr << 1) + 1].B * 5L) / 8);
                                                byte A = 0xff;
                                                pixel = Color.FromArgb(A, R, G, B);
                                                break;
                                            }
                                    }
                                    break;
                            }
                            rgbaOut.SetPixel((x * 4 + c), (y * 4 + r), pixel);
                            //rgbaOut[(y * 4 + r) * width + (x * 4 + c)] = pixel;
                        }
                }
            return true;
        }
        public void convert_4x4texel_b(byte[] tex, int width, int height, byte[] data, Color[] pal, ImageTexeler.LockBitmap rgbaOut)
        {
            var list1 = new List<uint>();
            for (int i = 0; i < (tex.Length + 1) / 4; ++i)
                list1.Add(LibNDSFormats.Utils.Read4BytesAsUInt32(tex, i * 4));

            var list2 = new List<UInt16>();
            for (int i = 0; i < (data.Length + 1) / 2; ++i)
                list2.Add(LibNDSFormats.Utils.Read2BytesAsUInt16(data, i * 2));
            var b = convert_4x4texel(list1.ToArray(), width, height, list2.ToArray(), pal, rgbaOut);
        }
    }

}
