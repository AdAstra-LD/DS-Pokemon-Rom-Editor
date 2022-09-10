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
using static NSMBe4.NSBMD.NSBTX_File.PalInfo_Master.Info;

namespace NSMBe4.NSBMD
{
    public class NSBTX {
        readonly DSFileSystem.File f;
        readonly byte[] data;
        readonly int texDataOffset;
        readonly int palDataOffset;
        readonly int palDefOffset;
        readonly int palDataSize;
        readonly int f5texDataOffset;
        readonly int f5dataOffset;
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
            if (str.readUInt() == 0x37375A4C) { //LZ77
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
            while (str.lengthAvailable(4)) {
                if (BitConverter.GetBytes(str.readUInt()).SequenceEqual(Encoding.ASCII.GetBytes("TEX0"))){ // "TEX0"
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
            if (!found) {
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
            for (int i = 0; i < textures.Length; i++) {
                int offset = 8 * str.ReadUInt16();
                ushort param = str.ReadUInt16();
                int format = (param >> 10) & 7;

                if (format == 5) {
                    offset += f5texDataOffset;
                } else {
                    offset += texDataOffset;
                }

                int width = 8 << ((param >> 4) & 7);
                int height = 8 << ((param >> 7) & 7);
                bool color0 = ((param >> 13) & 1) != 0;
                str.readUInt(); // unused

                int size = width * height * Image3D.bpps[format] / 8;
                Console.Out.WriteLine(offset.ToString("X8") + " " + format + " " + width + "x" + height + " " + color0 + " LZ" + LZd);

                InlineFile mainfile = new InlineFile(f, offset, size, Image3D.formatNames[format], null, LZd ? InlineFile.CompressionType.LZWithHeaderComp : InlineFile.CompressionType.NoComp);
                if (format == 5) {
                    hasFormat5 = true;
                    int f5size = (width * height) / 16 * 2;
                    InlineFile f5file = new InlineFile(f, f5dataOffset, f5size, Image3D.formatNames[format], null, LZd ? InlineFile.CompressionType.LZWithHeaderComp : InlineFile.CompressionType.NoComp);

                    f5dataOffset += f5size;
                    textures[i] = new Image3Dformat5(mainfile, f5file, width, height);
                } else {
                    textures[i] = new Image3D(mainfile, color0, width, height, format, offset);
                }

                //                textures[i] = new Texture(this, color0, width, height, format, offset, "");
                /*                if (format == 5)
                                {
                                    textures[i].f5DataOffset = f5dataOffset;
                                    f5dataOffset += (uint)(width * height) / 16 * 2;
                                }*/
            }

            for (int i = 0; i < textures.Length; i++) {
                if (textures[i] is null) {
                    continue;
                }

                textures[i].name = str.ReadString(16);
                //mgr.m.addImage(textures[i]);
            }

            //Read palette definitions
            str.seek(palDefOffset + 1);
            palettes = new PaletteDef[str.readByte()];
            str.skip((uint)(0xE + palettes.Length * 4));

            for (int i = 0; i < palettes.Length; i++) {
                int offset = 8 * str.ReadUInt16() + palDataOffset + blockStart;
                str.ReadUInt16();
                palettes[i] = new PaletteDef {
                    offs = offset
                };
            }

            Array.Sort(palettes);

            for (int i = 0; i < palettes.Length; i++) {
                palettes[i].name = str.ReadString(16);
                if (i != palettes.Length - 1) {
                    palettes[i].size = palettes[i + 1].offs - palettes[i].offs;
                }
            }
            palettes[palettes.Length - 1].size = blockStart + palDataOffset + palDataSize - palettes[palettes.Length - 1].offs;

            for (int i = 0; i < palettes.Length; i++) {
                if (hasFormat5) {
                    pal.Add(new FilePalette(new InlineFile(f, palettes[i].offs, palettes[i].size, palettes[i].name, null, LZd ? InlineFile.CompressionType.LZWithHeaderComp : InlineFile.CompressionType.NoComp)));
                    //mgr.m.addPalette(pa);
                } else {
                    int extrapalcount = (palettes[i].size) / 512;
                    for (int j = 0; j < extrapalcount; j++) {
                        pal.Add(new FilePalette(new InlineFile(f, palettes[i].offs + j * 512, 512, palettes[i].name + ":" + j, null, LZd ? InlineFile.CompressionType.LZWithHeaderComp : InlineFile.CompressionType.NoComp)));
                        //mgr.m.addPalette(pa);
                    }
                    int lastsize = palettes[i].size % 512;
                    if (lastsize != 0) {
                        pal.Add(new FilePalette(new InlineFile(f, palettes[i].offs + extrapalcount * 512, lastsize, palettes[i].name + ":" + extrapalcount, null, LZd ? InlineFile.CompressionType.LZWithHeaderComp : InlineFile.CompressionType.NoComp)));
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
    public class NSBTX_File {
        public enum TextureFormat : byte {
            FORMAT_NO_TEXTURE = 0,
            A3I5 = 1,
            COLOR_4 = 2,
            COLOR_16 = 3,
            COLOR_256 = 4,
            TEXEL_4X4 = 5,
            A5I3 = 6,
            DIRECT = 7
        }

        public byte[] before;
        public byte[] after;
        public Header header;
        public struct Header {
            public string ID;
            public ushort unk1;
            public ushort unk2;
            public int file_size;
            public short header_size;
            public short nSection;
            public long[] Section_Offset;
        }
        public struct TextureInformation {
            public int unk;
            public int datasize; //shift << 3
            public short dictionary_Offset;
            public short unk2;
            public int data_Offset;
        }
        public struct CompressedTextureInformation {
            public int unk;
            public int datasize; //shift << 3
            public short dictionary_Offset;
            public short unk2;
            public int data_Offset;
            public int palIndex;
        }
        public struct PaletteInformation {
            public int unk;
            public int datasize; //shift << 3
            public short unk2;
            public short dictionary_Offset;
            public int data_Offset;
        }
        public TEX0 tex0;
        public struct TEX0 {
            public int Section_size;
            public TextureInformation textureInformation;
            public CompressedTextureInformation compTextureInformation;
            public PaletteInformation paletteInformation;
        }
        public struct UnknownBlock {
            public short header_size;
            public short section_size;
            public int constant; // 0x017F

            public List<(short unk1, short unk2)> data;
        }
        public TexInfo_Master texInfo;
        public struct TexInfo_Master {
            public byte dummy;
            public byte num_objs;
            public short section_size;
            public UnknownBlock unknownBlock;
            public Info infoBlock;
            public List<string> names;

            public struct Info {
                public struct TexInfo {
                    public int Texture_Offset; //shift << 3, relative to start of Texture Data
                    public short Parameters;
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
                    public TextureFormat format;     // Texture format
                    public byte color0; // 0 = displayed; 1 = transparent
                    public byte coord_transf; // Texture coordination transformation mode

                    public byte depth;
                    public uint compressedDataStart;
                }

                public short header_size;
                public short data_size;

                public TexInfo[] tInfoArr;
            }
        }
        public PalInfo_Master palInfo;
        public struct PalInfo_Master {
            public byte dummy;
            public byte num_objs;
            public short section_size;
            public UnknownBlock unknownBlock;
            public Info infoBlock;
            public List<string> names;
            public struct Info {
                public struct PalInfo {
                    public int Palette_Offset; //shift << 3, relative to start of Palette Data
                    public short Color0;
                    public Color[] pal;
                }

                public short header_size;
                public short data_size;

                public PalInfo[] pInfoArr;
            }
        }

        readonly int[] bitDepth = { 0, 8, 2, 4, 8, 2, 8, 16 };

        public NSBTX_File(FileStream f) {
            using (EndianBinaryReader er = new EndianBinaryReader(f, Endianness.LittleEndian)) {
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
                    header.ID = "BTX0";
                    header.unk1 = er.ReadUInt16(); //4
                    header.unk2 = er.ReadUInt16(); //6

                    header.file_size = er.ReadInt32(); //8
                    header.header_size = er.ReadInt16(); //10
                    header.nSection = er.ReadInt16(); //12
                    header.Section_Offset = new long[header.nSection];

                    for (int i = 0; i < header.nSection; i++) {
                        header.Section_Offset[i] = er.ReadUInt32();
                    }

                    //==================================================
                    long backup = f.Position;
                    string ID = er.ReadString(Encoding.ASCII, 4);
                    if (ID != "TEX0") {
                        MessageBox.Show("Error: TEX0 header is wrong.", null, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        er.Close();
                        return;
                    }
                }

                tex0.Section_size = er.ReadInt32();
                
                tex0.textureInformation.unk  = er.ReadInt32();
                tex0.textureInformation.datasize = er.ReadInt16() << 3;
                tex0.textureInformation.dictionary_Offset = er.ReadInt16();
                tex0.textureInformation.unk2 = er.ReadInt16();
                er.BaseStream.Position += 2; //Skip padding
                tex0.textureInformation.data_Offset = er.ReadInt32();

                tex0.compTextureInformation.unk = er.ReadInt32();
                tex0.compTextureInformation.datasize = er.ReadInt16() << 3;
                tex0.compTextureInformation.dictionary_Offset = er.ReadInt16();
                tex0.compTextureInformation.unk2 = er.ReadInt16();
                er.BaseStream.Position += 2; //Skip padding
                tex0.compTextureInformation.data_Offset = er.ReadInt32();
                tex0.compTextureInformation.palIndex = er.ReadInt32();

                tex0.paletteInformation.unk = er.ReadInt32();
                tex0.paletteInformation.datasize = er.ReadInt16() << 3;
                tex0.paletteInformation.unk2 = er.ReadInt16();
                tex0.paletteInformation.dictionary_Offset = er.ReadInt16();
                er.BaseStream.Position += 2; //Skip padding
                tex0.paletteInformation.data_Offset = er.ReadInt32();

                texInfo.dummy = er.ReadByte();
                texInfo.num_objs = er.ReadByte();
                texInfo.section_size = er.ReadInt16();

                texInfo.unknownBlock.header_size = er.ReadInt16();
                texInfo.unknownBlock.section_size = er.ReadInt16();
                texInfo.unknownBlock.constant = er.ReadInt32();
                texInfo.unknownBlock.data = new List<(short unk1, short unk2)>(texInfo.num_objs);

                for (int i = 0; i < texInfo.num_objs; i++) {
                    texInfo.unknownBlock.data.Add((er.ReadInt16(), er.ReadInt16()));
                }

                texInfo.infoBlock.header_size = er.ReadInt16();
                texInfo.infoBlock.data_size = er.ReadInt16();
                texInfo.infoBlock.tInfoArr = new TexInfo_Master.Info.TexInfo[texInfo.num_objs];

                for (int i = 0; i < texInfo.num_objs; i++) {
                    texInfo.infoBlock.tInfoArr[i].Texture_Offset = er.ReadInt16() << 3;
                    texInfo.infoBlock.tInfoArr[i].Parameters = er.ReadInt16();
                    texInfo.infoBlock.tInfoArr[i].Width = er.ReadByte();
                    texInfo.infoBlock.tInfoArr[i].Unknown1 = er.ReadByte();
                    texInfo.infoBlock.tInfoArr[i].Height = er.ReadByte();
                    texInfo.infoBlock.tInfoArr[i].Unknown2 = er.ReadByte();
                    texInfo.infoBlock.tInfoArr[i].coord_transf = (byte)(texInfo.infoBlock.tInfoArr[i].Parameters & 14);
                    texInfo.infoBlock.tInfoArr[i].color0 = (byte)((texInfo.infoBlock.tInfoArr[i].Parameters >> 13) & 1);
                    texInfo.infoBlock.tInfoArr[i].format = (TextureFormat)((texInfo.infoBlock.tInfoArr[i].Parameters >> 10) & 7);
                    texInfo.infoBlock.tInfoArr[i].height = (byte)(8 << ((texInfo.infoBlock.tInfoArr[i].Parameters >> 7) & 7));
                    texInfo.infoBlock.tInfoArr[i].width = (byte)(8 << ((texInfo.infoBlock.tInfoArr[i].Parameters >> 4) & 7));
                    texInfo.infoBlock.tInfoArr[i].flip_Y = (byte)((texInfo.infoBlock.tInfoArr[i].Parameters >> 3) & 1);
                    texInfo.infoBlock.tInfoArr[i].flip_X = (byte)((texInfo.infoBlock.tInfoArr[i].Parameters >> 2) & 1);
                    texInfo.infoBlock.tInfoArr[i].repeat_Y = (byte)((texInfo.infoBlock.tInfoArr[i].Parameters >> 1) & 1);
                    texInfo.infoBlock.tInfoArr[i].repeat_X = (byte)(texInfo.infoBlock.tInfoArr[i].Parameters & 1);
                    texInfo.infoBlock.tInfoArr[i].depth = (byte)bitDepth[(byte)texInfo.infoBlock.tInfoArr[i].format];

                    if (texInfo.infoBlock.tInfoArr[i].width == 0x00) {
                        switch (texInfo.infoBlock.tInfoArr[i].Unknown1 & 0x3) {
                            case 2:
                                texInfo.infoBlock.tInfoArr[i].width = 0x200;
                                break;
                            default:
                                texInfo.infoBlock.tInfoArr[i].width = 0x100;
                                break;
                        }
                    }

                    if (texInfo.infoBlock.tInfoArr[i].height == 0x00) {
                        switch ((texInfo.infoBlock.tInfoArr[i].Height >> 3) & 0x3) {
                            case 2:
                                texInfo.infoBlock.tInfoArr[i].height = 0x200;
                                break;
                            default:
                                texInfo.infoBlock.tInfoArr[i].height = 0x100;
                                break;
                        }
                    }

                    int imgsize = texInfo.infoBlock.tInfoArr[i].width * texInfo.infoBlock.tInfoArr[i].height * texInfo.infoBlock.tInfoArr[i].depth / 8;
                    long curpos = er.BaseStream.Position;

                    if (texInfo.infoBlock.tInfoArr[i].format == TextureFormat.TEXEL_4X4) {
                        er.BaseStream.Seek(header.Section_Offset[0] + tex0.compTextureInformation.data_Offset + texInfo.infoBlock.tInfoArr[i].Texture_Offset, SeekOrigin.Begin);
                    } else {
                        er.BaseStream.Seek(texInfo.infoBlock.tInfoArr[i].Texture_Offset + header.Section_Offset[0] + tex0.textureInformation.data_Offset, SeekOrigin.Begin);
                    }

                    texInfo.infoBlock.tInfoArr[i].Image = er.ReadBytes(imgsize);
                    er.BaseStream.Seek(curpos, SeekOrigin.Begin);

                    if (texInfo.infoBlock.tInfoArr[i].format == TextureFormat.TEXEL_4X4) {
                        curpos = er.BaseStream.Position;
                        er.BaseStream.Seek(header.Section_Offset[0] + tex0.compTextureInformation.palIndex + texInfo.infoBlock.tInfoArr[i].Texture_Offset / 2, SeekOrigin.Begin);
                        texInfo.infoBlock.tInfoArr[i].spData = er.ReadBytes(imgsize / 2);
                        er.BaseStream.Seek(curpos, SeekOrigin.Begin);
                    }
                }

                texInfo.names = new List<string>(texInfo.num_objs);
                for (int i = 0; i < texInfo.num_objs; i++) {
                    texInfo.names.Add(er.ReadString(Encoding.ASCII, 16).Replace("\0", ""));
                }

                palInfo.dummy = er.ReadByte();
                palInfo.num_objs = er.ReadByte();
                palInfo.section_size = er.ReadInt16();

                palInfo.unknownBlock.header_size = er.ReadInt16();
                palInfo.unknownBlock.section_size = er.ReadInt16();
                palInfo.unknownBlock.constant = er.ReadInt32();
                palInfo.unknownBlock.data = new List<(short unk1, short unk2)>(palInfo.num_objs);

                for (int i = 0; i < palInfo.num_objs; i++) {
                    palInfo.unknownBlock.data.Add((er.ReadInt16(), er.ReadInt16()));
                }

                palInfo.infoBlock.header_size = er.ReadInt16();
                palInfo.infoBlock.data_size = er.ReadInt16();
                palInfo.infoBlock.pInfoArr = new PalInfo_Master.Info.PalInfo[palInfo.num_objs];

                for (int i = 0; i < palInfo.num_objs; i++) {
                    palInfo.infoBlock.pInfoArr[i].Palette_Offset = er.ReadInt16() << 3;
                    palInfo.infoBlock.pInfoArr[i].Color0 = er.ReadInt16();
                    //er.BaseStream.Position -= 4;
                    //int palBase = er.ReadInt32();
                    //int palAddr = palBase & 0xfff;
                    long curpos = er.BaseStream.Position;

                    er.BaseStream.Seek(header.Section_Offset[0] + tex0.paletteInformation.data_Offset, SeekOrigin.Begin);
                    er.BaseStream.Seek(palInfo.infoBlock.pInfoArr[i].Palette_Offset, SeekOrigin.Current);
                    palInfo.infoBlock.pInfoArr[i].pal = Tinke.Convertir.BGR555ToColorArray(er.ReadBytes(tex0.paletteInformation.datasize - palInfo.infoBlock.pInfoArr[i].Palette_Offset));
                    er.BaseStream.Seek(curpos, SeekOrigin.Begin);
                }

                palInfo.names = new List<string>(palInfo.num_objs);
                for (int i = 0; i < palInfo.num_objs; i++) {
                    palInfo.names.Add(er.ReadString(Encoding.ASCII, 16).Replace("\0", ""));
                }

                List<int> offsets = new List<int>(palInfo.num_objs);
                for (int i = 0; i < palInfo.num_objs; i++) {
                    offsets.Add(palInfo.infoBlock.pInfoArr[i].Palette_Offset);
                }

                offsets.Add((int)er.BaseStream.Length);
                offsets.Sort();

                for (int i = 0; i < palInfo.num_objs; i++) {
                    int pallength;
                    int j = -1;

                    do {
                        j++;
                    } while (offsets[j] - palInfo.infoBlock.pInfoArr[i].Palette_Offset <= 0);

                    pallength = offsets[j] - palInfo.infoBlock.pInfoArr[i].Palette_Offset;
                    palInfo.infoBlock.pInfoArr[i].pal = palInfo.infoBlock.pInfoArr[i].pal.Take(pallength / 2).ToArray();
                }
            }
        }
        public (Bitmap bmp, int ctrlCode) GetBitmap(int imageIndex, int palIndex) {
            NSBTX_File overworldFrames = null;

            int ctrlCode = 0;
            Bitmap b_ = new Bitmap(this.texInfo.infoBlock.tInfoArr[imageIndex].width, this.texInfo.infoBlock.tInfoArr[imageIndex].height);
            ImageTexeler.LockBitmap b = new ImageTexeler.LockBitmap(b_);
            b.LockBits();
            int pixelnum = b.Height * b.Width;

            try {
                switch (this.texInfo.infoBlock.tInfoArr[imageIndex].format) {
                    case TextureFormat.A3I5:
                        for (int j = 0; j < pixelnum; j++) {
                            int index = this.texInfo.infoBlock.tInfoArr[imageIndex].Image[j] & 0x1f;
                            int alpha = this.texInfo.infoBlock.tInfoArr[imageIndex].Image[j] >> 5;
                            alpha = alpha == 0 ? 0 : (alpha + 1) * 32 - 1;
                            Color c = Color.FromArgb(alpha, this.palInfo.infoBlock.pInfoArr[palIndex].pal[index]);
                            b.SetPixel(j - (j / b.Width * b.Width), j / b.Width, c);
                        }
                        break;
                    case TextureFormat.COLOR_4:
                        for (int j = 0; j < pixelnum; j++) {
                            uint index = this.texInfo.infoBlock.tInfoArr[imageIndex].Image[j / 4];
                            index = (index >> ((j % 4) << 1)) & 3;

                            PalInfo[] pinfoArr = this.palInfo.infoBlock.pInfoArr;

                            Color c;
                            if (index == 0 && this.texInfo.infoBlock.tInfoArr[imageIndex].color0 == 1) {
                                c = Color.Transparent;
                            } else {
                                if (index >= pinfoArr[palIndex].pal.Length) {
                                    c = Color.Black;
                                    ctrlCode = 4;
                                } else {
                                    c = pinfoArr[palIndex].pal[index];
                                }
                            }

                            b.SetPixel(j - (j / b.Width * b.Width), j / b.Width, c);
                        }
                        break;
                    case TextureFormat.COLOR_16:
                        for (int j = 0; j < pixelnum; j++) {
                            uint index = this.texInfo.infoBlock.tInfoArr[imageIndex].Image[j / 2];
                            index = (index >> ((j % 2) << 2)) & 0x0f;

                            PalInfo[] pinfoArr = this.palInfo.infoBlock.pInfoArr;
                            
                            Color c;
                            if (index == 0 && this.texInfo.infoBlock.tInfoArr[imageIndex].color0 == 1) {
                                c = Color.Transparent;
                            } else {
                                if (index >= pinfoArr[palIndex].pal.Length) {
                                    ctrlCode = 16;
                                    c = Color.Black;
                                } else {
                                    c = pinfoArr[palIndex].pal[index];
                                }
                            }

                            b.SetPixel(j - (j / b.Width * b.Width), j / b.Width, c);
                        }
                        break;
                    case TextureFormat.COLOR_256:
                        for (int j = 0; j < pixelnum; j++) {
                            byte index = this.texInfo.infoBlock.tInfoArr[imageIndex].Image[j];

                            PalInfo[] pinfoArr = this.palInfo.infoBlock.pInfoArr;

                            Color c;
                            if (index == 0 && this.texInfo.infoBlock.tInfoArr[imageIndex].color0 == 1) {
                                c = Color.Transparent;
                            } else {
                                if (index >= pinfoArr[palIndex].pal.Length) {
                                    ctrlCode = 256;
                                    c = Color.Black;
                                } else {
                                    c = pinfoArr[palIndex].pal[index];
                                }
                            }

                            b.SetPixel(j - (j / b.Width * b.Width), j / b.Width, c);
                        }
                        break;
                    case TextureFormat.TEXEL_4X4:
                        overworldFrames.convert_4x4texel_b(this.texInfo.infoBlock.tInfoArr[imageIndex].Image, b.Width, b.Height, this.texInfo.infoBlock.tInfoArr[imageIndex].spData, this.palInfo.infoBlock.pInfoArr[palIndex].pal, b);
                        b.UnlockBits();
                        break;
                    case TextureFormat.A5I3:
                        for (int j = 0; j < pixelnum; j++) {
                            int index = this.texInfo.infoBlock.tInfoArr[imageIndex].Image[j] & 0x7;
                            int alpha = this.texInfo.infoBlock.tInfoArr[imageIndex].Image[j] >> 3;// & 0x1f;
                            alpha = alpha == 0 ? 0 : (alpha + 1) * 8 - 1;
                            Color c = Color.FromArgb(alpha, this.palInfo.infoBlock.pInfoArr[palIndex].pal[index]);
                            b.SetPixel(j - (j / b.Width * b.Width), j / b.Width, c);
                        }
                        break;
                    case TextureFormat.DIRECT:
                        for (int j = 0; j < pixelnum; j++) {
                            ushort p = (ushort)(this.texInfo.infoBlock.tInfoArr[imageIndex].Image[j * 2] + (this.texInfo.infoBlock.tInfoArr[imageIndex].Image[j * 2 + 1] << 8));
                            Color c = Color.FromArgb(
                                ((p & 0x8000) != 0) ? 0xff : 0, 
                                ((p >> 0) & 0x1f) << 3, 
                                ((p >> 5) & 0x1f) << 3, 
                                ((p >> 10) & 0x1f) << 3);
                            b.SetPixel(j - (j / b.Width * b.Width), j / b.Width, c);
                        }
                        break;
                }
            } finally {
                b.UnlockBits();
            }

            return (b_, ctrlCode);
        }
        public byte[] ToByteArray() {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData)) {
                long filesizeOffset;

                writer.Write(Encoding.ASCII.GetBytes("BTX0"));
                writer.Write(header.unk1);
                writer.Write(header.unk2);

                filesizeOffset = writer.BaseStream.Position;
                writer.BaseStream.Position += 4; //Size will be written once we know it

                writer.Write(header.header_size);
                writer.Write(header.nSection);
                
                for (int i = 0; i < header.nSection; i++) {
                    writer.Write(header.Section_Offset[i]);
                }

                //=============//
                writer.Write(Encoding.ASCII.GetBytes("TEX0"));

                writer.Write(tex0.Section_size);
                
                writer.Write(tex0.textureInformation.unk);
                writer.Write((short)(tex0.textureInformation.datasize >> 3));
                writer.Write(tex0.textureInformation.dictionary_Offset);
                writer.Write(tex0.textureInformation.unk2);
                writer.BaseStream.Position += 2; //Skip padding
                writer.Write(tex0.textureInformation.data_Offset);

                writer.Write(tex0.compTextureInformation.unk);
                writer.Write((short)(tex0.compTextureInformation.datasize >> 3));
                writer.Write(tex0.compTextureInformation.dictionary_Offset);
                writer.Write(tex0.compTextureInformation.unk2);
                writer.BaseStream.Position += 2; //Skip padding
                writer.Write(tex0.compTextureInformation.data_Offset);
                writer.Write(tex0.compTextureInformation.palIndex);

                writer.Write(tex0.paletteInformation.unk);
                writer.Write((short)(tex0.paletteInformation.datasize >> 3)); //this should actually be serialized as 4B int
                writer.Write(tex0.paletteInformation.unk2);
                writer.Write(tex0.paletteInformation.dictionary_Offset);
                writer.BaseStream.Position += 2; //Skip padding
                writer.Write(tex0.paletteInformation.data_Offset);

                writer.Write(texInfo.dummy);
                writer.Write(texInfo.num_objs);
                writer.Write(texInfo.section_size);

                writer.Write(texInfo.unknownBlock.header_size);
                writer.Write(texInfo.unknownBlock.section_size);
                writer.Write(texInfo.unknownBlock.constant);

                for (int i = 0; i < texInfo.num_objs; i++) {
                    (short unk1, short unk2) = texInfo.unknownBlock.data[i];
                    writer.Write(unk1);
                    writer.Write(unk2);
                }

                writer.Write(texInfo.infoBlock.header_size);
                writer.Write(texInfo.infoBlock.data_size);

                for (int i = 0; i < texInfo.num_objs; i++) {
                    writer.Write((ushort)(texInfo.infoBlock.tInfoArr[i].Texture_Offset >> 3));
                    writer.Write(texInfo.infoBlock.tInfoArr[i].Parameters);
                    writer.Write(texInfo.infoBlock.tInfoArr[i].Width);
                    writer.Write(texInfo.infoBlock.tInfoArr[i].Unknown1);
                    writer.Write(texInfo.infoBlock.tInfoArr[i].Height);
                    writer.Write(texInfo.infoBlock.tInfoArr[i].Unknown2);

                    long curpos = writer.BaseStream.Position;

                    if (texInfo.infoBlock.tInfoArr[i].format == TextureFormat.TEXEL_4X4) {
                        writer.BaseStream.Seek(header.Section_Offset[0] + tex0.compTextureInformation.data_Offset + texInfo.infoBlock.tInfoArr[i].Texture_Offset, SeekOrigin.Begin);
                    } else {
                        writer.BaseStream.Seek(texInfo.infoBlock.tInfoArr[i].Texture_Offset + header.Section_Offset[0] + tex0.textureInformation.data_Offset, SeekOrigin.Begin);
                    }

                    writer.Write(texInfo.infoBlock.tInfoArr[i].Image);
                    writer.BaseStream.Seek(curpos, SeekOrigin.Begin);

                    if (texInfo.infoBlock.tInfoArr[i].format == TextureFormat.TEXEL_4X4) {
                        curpos = writer.BaseStream.Position;
                        writer.BaseStream.Seek(header.Section_Offset[0] + tex0.compTextureInformation.palIndex + texInfo.infoBlock.tInfoArr[i].Texture_Offset / 2, SeekOrigin.Begin);
                        writer.Write(texInfo.infoBlock.tInfoArr[i].spData);
                        writer.BaseStream.Seek(curpos, SeekOrigin.Begin);
                    }
                }

                for (int i = 0; i < texInfo.num_objs; i++) {
                    writer.Write(Encoding.ASCII.GetBytes(texInfo.names[i].PadRight(16, '\0')));
                }

                writer.Write(palInfo.dummy);
                writer.Write(palInfo.num_objs);
                writer.Write(palInfo.section_size);

                writer.Write(palInfo.unknownBlock.header_size);
                writer.Write(palInfo.unknownBlock.section_size);
                writer.Write(palInfo.unknownBlock.constant);

                for (int i = 0; i < palInfo.num_objs; i++) {
                    (short unk1, short unk2) = palInfo.unknownBlock.data[i];
                    writer.Write(unk1);
                    writer.Write(unk2);
                }

                writer.Write(palInfo.infoBlock.header_size);
                writer.Write(palInfo.infoBlock.data_size);

                for (int i = 0; i < palInfo.num_objs; i++) {
                    writer.Write((ushort)(palInfo.infoBlock.pInfoArr[i].Palette_Offset >> 3));
                    writer.Write(palInfo.infoBlock.pInfoArr[i].Color0);
                    //int palAddr = palBase & 0xfff;

                    long curpos = writer.BaseStream.Position;
                    writer.BaseStream.Seek(header.Section_Offset[0] + tex0.paletteInformation.data_Offset, SeekOrigin.Begin);
                    writer.BaseStream.Seek(palInfo.infoBlock.pInfoArr[i].Palette_Offset, SeekOrigin.Current);
                    writer.Write(Tinke.Convertir.ColorArrayToBGR555(palInfo.infoBlock.pInfoArr[i].pal));
                    writer.BaseStream.Seek(curpos, SeekOrigin.Begin);
                }


                for (int i = 0; i < palInfo.num_objs; i++) {
                    writer.Write(Encoding.ASCII.GetBytes(palInfo.names[i].PadRight(16, '\0')));
                }

                for (int i = 0; i < palInfo.num_objs; i++) {
                    writer.Write(palInfo.infoBlock.pInfoArr[i].Palette_Offset);
                }

                writer.BaseStream.Position = filesizeOffset; //go back and write file size
                writer.Write((uint)writer.BaseStream.Length);
            }

            return newData.ToArray();
        }
        public bool convert_4x4texel(uint[] tex, int width, int height, ushort[] data, Color[] pal, ImageTexeler.LockBitmap rgbaOut)
        {
            int w = width / 4;
            int h = height / 4;

            // traverse 'w x h blocks' of 4x4-texel
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    int index = y * w + x;
                    UInt32 t = tex[index];
                    ushort d = data[index];
                    ushort addr = (ushort)(d & 0x3fff);
                    ushort mode = (ushort)((d >> 14) & 3);

                    // traverse every texel in the 4x4 texels
                    for (int r = 0; r < 4; r++) {
                        for (int c = 0; c < 4; c++) {
                            int texel = (int)((t >> ((r * 4 + c) * 2)) & 3);
                            Color pixel = rgbaOut.GetPixel((x * 4 + c), (y * 4 + r));

                            switch (mode) {
                                case 0:
                                    pixel = pal[(addr << 1) + texel];
                                    if (texel == 3) pixel = Color.Transparent; // make it transparent, alpha = 0
                                    break;
                                case 2:
                                    pixel = pal[(addr << 1) + texel];
                                    break;
                                case 1:
                                    switch (texel) {
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
                                    switch (texel) {
                                        case 0:
                                        case 1:
                                            pixel = pal[(addr << 1) + texel];
                                            break;
                                        case 2: {
                                                byte R = (byte)((pal[(addr << 1)].R * 5L + pal[(addr << 1) + 1].R * 3L) / 8);
                                                byte G = (byte)((pal[(addr << 1)].G * 5L + pal[(addr << 1) + 1].G * 3L) / 8);
                                                byte B = (byte)((pal[(addr << 1)].B * 5L + pal[(addr << 1) + 1].B * 3L) / 8);
                                                byte A = 0xff;
                                                pixel = Color.FromArgb(A, R, G, B);
                                                break;
                                            }
                                        case 3: {
                                                byte R = (byte)((pal[(addr << 1)].R * 3L + pal[(addr << 1) + 1].R * 5L) / 8);
                                                byte G = (byte)((pal[(addr << 1)].G * 3L + pal[(addr << 1) + 1].G * 5L) / 8);
                                                byte B = (byte)((pal[(addr << 1)].B * 3L + pal[(addr << 1) + 1].B * 5L) / 8);
                                                byte A = 0xff;
                                                pixel = Color.FromArgb(A, R, G, B);
                                                break;
                                            }
                                    }
                                    break;
                                default:
                                    break;
                            }
                            rgbaOut.SetPixel((x * 4 + c), (y * 4 + r), pixel);
                            //rgbaOut[(y * 4 + r) * width + (x * 4 + c)] = pixel;
                        }
                    }
                }
            return true;
        }
        public void convert_4x4texel_b(byte[] tex, int width, int height, byte[] data, Color[] pal, ImageTexeler.LockBitmap rgbaOut)
        {
            var list1 = new List<uint>();
            for (int i = 0; i < (tex.Length + 1) / 4; ++i) {
                list1.Add(LibNDSFormats.Utils.Read4BytesAsUInt32(tex, i * 4));
            }

            var list2 = new List<ushort>();
            for (int i = 0; i < (data.Length + 1) / 2; ++i) {
                list2.Add(LibNDSFormats.Utils.Read2BytesAsushort(data, i * 2));
            }

            convert_4x4texel(list1.ToArray(), width, height, list2.ToArray(), pal, rgbaOut);
        }

        internal HashSet<(byte f1, byte f2)> AnalyzeRepetitions() {
            HashSet<(byte f1, byte f2)> equality = new HashSet<(byte f1, byte f2)>();
            HashSet<byte> equalY = new HashSet<byte>();

            int texInfoCount = this.texInfo.infoBlock.tInfoArr.Length;
            for (byte x = 0; x < texInfoCount; x++) {
                for (byte y = (byte)(x + 1); y < texInfoCount; y++) {
                    if (!equalY.Contains(x) && !equalY.Contains(y)) {
                        if (this.texInfo.infoBlock.tInfoArr[x].Image.SequenceEqual(this.texInfo.infoBlock.tInfoArr[y].Image)) {
                            equality.Add((x, y));
                            equalY.Add(y);
                        }
                    }
                }
            }

            return equality;
        }

        public class DupFrameHashSetComparer : IEqualityComparer<HashSet<(byte f1, byte f2)>> {
            #region IEqualityComparer<Customer> Members

            public bool Equals(HashSet<(byte f1, byte f2)> x, HashSet<(byte f1, byte f2)> y) {
                return x.SetEquals(y);
            }

            public int GetHashCode(HashSet<(byte f1, byte f2)> obj) {
                int ret = 0;
                foreach(var key in obj) {
                    ret += key.GetHashCode();
                }
                return ret;
            }

            #endregion
        }

    }

}
