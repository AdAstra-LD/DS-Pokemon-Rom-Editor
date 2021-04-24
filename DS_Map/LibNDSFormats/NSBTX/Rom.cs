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
using System.Text;
using NSMBe4.DSFileSystem;
using System.IO;
using System.Runtime.InteropServices;


/**
 * This class handles internal NSMB-specific data in the ROM.
 * Right now it can decompress the Overlay data and read
 * data from several tables contained in the ROM.
 * 
 * Data description about overlay 0: (From an old text file)
 * 76 max tilesets. Each table is 0x130 big.
 * 
 * 2F8E4: Object definition indexes (unt+hd) table
 * 2FA14: Object definitions (unt) table
 * 2FB44: Tile behaviours (chk) table
 * 2FC74: Animated tileset graphics (ncg) table
 * 2FDA4: Jyotyu tile behaviour file
 * 30D74: Background graphics (ncg) table
 * 30EA4: Tileset graphics (ncg) table
 * 30FD4: Foreground graphics (ncg) table
 * 31104: Foreground design (nsc) table
 * 31234: Background design (nsc) table
 * 31364: Background palette (ncl) table
 * 31494: Tileset palette (ncl) table
 * 315C4: Foreground palette (ncl) table
 * 316F4: Map16 (pnl) table
 * 
 **/

namespace NSMBe4
{
    public static class ROM
    {
        public static byte[] Overlay0;
        //public static NitroROMFilesystem FS;
        public static string filename;
        public static System.IO.FileInfo romfile;
        public static Dictionary<int, List<string>> descriptions;
        public static string DescriptionPath;

        /*public static void load(String filename)
        {
            ROM.filename = filename;
            FS = new NitroROMFilesystem(filename);
            romfile = new System.IO.FileInfo(filename);

            LoadDescriptions();
            LoadOverlay0();

            if (Overlay0 != null)
            {
                if (Overlay0[28] == 0x84)
                    Region = Origin.US;
                else if (Overlay0[28] == 0x64)
                    Region = Origin.EU;
                else if (Overlay0[28] == 0x04)
                    Region = Origin.JP;
                else if (Overlay0[28] == 0xC4)
                    Region = Origin.KR;
                else
                {
                    Region = Origin.US;
                    System.Windows.Forms.MessageBox.Show(LanguageManager.Get("General", "UnknownRegion"), LanguageManager.Get("General", "Warning"), System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                }
            }
            else
            {
                Region = Origin.US;
                System.Windows.Forms.MessageBox.Show(LanguageManager.Get("General", "UnknownRegion"), LanguageManager.Get("General", "Warning"), System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
            }

            //            SaveOverlay0();
        }*/

        //public static void close()
        //{
        //    if (FS is null) return;
        //    FS.close();
        //}

        /*public static void SaveOverlay0()
        {
            OverlayFile ov = FS.arm9ovs[0];
            ov.decompress();
            ov.beginEdit(FS);
            ov.replace(Overlay0, FS);
            ov.endEdit(FS);
        }*/

        /*public static void LoadOverlay0()
        {
            if (FS.arm9ovs.Length == 0)
                return;

            OverlayFile ov = FS.arm9ovs[0];
            ov.decompress();

            Overlay0 = ov.getContents();
        }*/

        public static void LoadDescriptions()
        {
            DescriptionPath = filename.Substring(0, filename.LastIndexOf('.') + 1) + "txt";
            descriptions = new Dictionary<int, List<string>>();
            int lineNum = 0;
            try
            {
                if (!System.IO.File.Exists(DescriptionPath)) return;
                System.IO.StreamReader s = new System.IO.StreamReader(DescriptionPath);
                List<string> curList = null;
                while (!s.EndOfStream)
                {
                    lineNum++;
                    string line = s.ReadLine();
                    if (line != "")
                    {
                        if (line.StartsWith("["))
                        {
                            int num = int.Parse(line.Substring(1, line.Length - 2));
                            descriptions.Add(num, new List<string>());
                            curList = descriptions[num];
                        }
                        else if (curList != null)
                        {
                            curList.Add(line);
                        }
                    }
                }
                s.Close();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("An error occurred while reading tileset descriptions on line " + lineNum.ToString() + ".\n\nThe original error message was:\n" + ex.Message);
            }
        }

        public enum Origin
        {
            US = 0, EU = 1, JP = 2, KR = 3
        }

        public static Origin Region = Origin.US;

        public enum Data : int
        {//
            Number_FileOffset = 0,
            Table_TS_UNT_HD = 1,
            Table_TS_UNT = 2,
            Table_TS_CHK = 3,
            Table_TS_ANIM_NCG = 4,
            Table_BG_NCG = 5,
            Table_TS_NCG = 6,
            Table_FG_NCG = 7,
            Table_FG_NSC = 8,
            Table_BG_NSC = 9,
            Table_BG_NCL = 10,
            Table_TS_NCL = 11,
            Table_FG_NCL = 12,
            Table_TS_PNL = 13,
            Table_Jyotyu_NCL = 14,
            File_Jyotyu_CHK = 15,
            File_Modifiers = 16,
            Table_Sprite_CLASSID = 17,
        }

        public static int[,] Offsets = {
                                           {131, 135, 131, 131}, //File Offset (Overlay Count)
                                           {0x2F8E4, 0x2F0F8, 0x2ECE4, 0x2EDA4}, //TS_UNT_HD
                                           {0x2FA14, 0x2F228, 0x2EE14, 0x2EED4}, //TS_UNT
                                           {0x2FB44, 0x2F358, 0x2EF44, 0x2F004}, //TS_CHK
                                           {0x2FC74, 0x2F488, 0x2F074, 0x2F134}, //TS_ANIM_NCG
                                           {0x30D74, 0x30588, 0x30174, 0x30234}, //BG_NCG
                                           {0x30EA4, 0x306B8, 0x302A4, 0x30364}, //TS_NCG
                                           {0x30FD4, 0x307E8, 0x303D4, 0x30494}, //FG_NCG
                                           {0x31104, 0x30918, 0x30504, 0x305C4}, //FG_NSC
                                           {0x31234, 0x30A48, 0x30634, 0x306F4}, //BG_NSC
                                           {0x31364, 0x30B78, 0x30764, 0x30824}, //BG_NCL
                                           {0x31494, 0x30CA8, 0x30894, 0x30954}, //TS_NCL
                                           {0x315C4, 0x30DD8, 0x309C4, 0x30A84}, //FG_NCL
                                           {0x316F4, 0x30F08, 0x30AF4, 0x30BB4}, //TS_PNL
                                           {0x30CD8, 0x304EC, 0x300D8, 0x30198}, //Jyotyu_NCL
                                           {0x2FDA4, 0x2F5B8, 0x2F1A4, 0x2FC74}, //Jyotyu_CHK
                                           {0x2C930, 0x2BDF0, 0x2BD30, 0x2BDF0}, //Modifiers
                                           {0x29BD8, 0x00000, 0x00000, 0x00000}, //Sprite Class IDs
                                           {0x2CBBC, 0, 0, 0}, //weird table¿?
                                       };

        public static int[] FileSizes = {
                                            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0, //Don't include tables
                                            0x400, //Jyotyu_CHK
                                            0x288, //Modifiers
                                        };

        public static ushort GetFileIDFromTable(int id, Data datatype)
        {
            return GetFileIDFromTable(id, GetOffset(datatype));
        }

        public static ushort GetFileIDFromTable(int id, int tableoffset)
        {
            int off = tableoffset + (id << 2);
            return (ushort)((Overlay0[off] | (Overlay0[off + 1] << 8)) + GetOffset(Data.Number_FileOffset));
        }

        public static ushort GetClassIDFromTable(int id)
        {
            int off = GetOffset(Data.Table_Sprite_CLASSID) + (id << 1);
            return (ushort)((Overlay0[off] | (Overlay0[off + 1] << 8)));
        }

        public static void SetFileIDFromTable(int id, Data datatype, ushort fid)
        {
            SetFileIDFromTable(id, GetOffset(datatype), fid);
        }

        public static string getDataForSprite(int id)
        {
            int offs = 0x2CBBC + id * 20;
            string s = "";

            for (int i = 0; i < 20; i++)
                s += String.Format("{0:X2}", Overlay0[offs++]) + " ";

            return s;
        }

        public static void SetFileIDFromTable(int id, int tableoffset, ushort fid)
        {
            int off = tableoffset + (id << 2);
            fid -= (ushort)GetOffset(Data.Number_FileOffset);
            Overlay0[off] = (byte)(fid & 0xFF);
            Overlay0[off + 1] = (byte)(fid >> 8);
        }

        public static int GetOffset(Data datatype)
        {
            return Offsets[(int)datatype, (int)Region];
        }

        public static byte[] GetInlineFile(Data datatype)
        {
            byte[] output = new byte[FileSizes[(int)datatype]];
            Array.Copy(Overlay0, GetOffset(datatype), output, 0, output.Length);
            return output;
        }

        //public static void ReplaceInlineFile(Data datatype, byte[] NewFile)
        //{
        //    Array.Copy(NewFile, 0, Overlay0, GetOffset(datatype), NewFile.Length);
        //    SaveOverlay0();
        //}

        public static byte[] decompressOverlay(byte[] sourcedata)
        {
            uint DataVar1, DataVar2;
            //uint last 8-5 bytes
            DataVar1 = (uint)(sourcedata[sourcedata.Length - 8] | (sourcedata[sourcedata.Length - 7] << 8) | (sourcedata[sourcedata.Length - 6] << 16) | (sourcedata[sourcedata.Length - 5] << 24));
            //uint last 4 bytes
            DataVar2 = (uint)(sourcedata[sourcedata.Length - 4] | (sourcedata[sourcedata.Length - 3] << 8) | (sourcedata[sourcedata.Length - 2] << 16) | (sourcedata[sourcedata.Length - 1] << 24));

            byte[] memory = new byte[sourcedata.Length + DataVar2];
            sourcedata.CopyTo(memory, 0);

            uint r0, r1, r2, r3, r5, r6, r7, r12;
            bool N, V;
            r0 = (uint)sourcedata.Length;

            if (r0 == 0)
            {
                return null;
            }
            r1 = DataVar1;
            r2 = DataVar2;
            r2 = r0 + r2; //length + datavar2 -> decompressed length
            r3 = r0 - (r1 >> 0x18); //delete the latest 3 bits??
            r1 &= 0xFFFFFF; //save the latest 3 bits
            r1 = r0 - r1;
        a958:
            if (r3 <= r1)
            { //if r1 is 0 they will be equal
                goto a9B8; //return the memory buffer
            }
            r3 -= 1;
            r5 = memory[r3];
            r6 = 8;
        a968:
            SubS(out r6, r6, 1, out N, out V);
            if (N != V)
            {
                goto a958;
            }
            if ((r5 & 0x80) != 0)
            {
                goto a984;
            }
            r3 -= 1;
            r0 = memory[r3];
            r2 -= 1;
            memory[r2] = (byte)r0;
            goto a9AC;
        a984:
            r3 -= 1;
            r12 = memory[r3];
            r3 -= 1;
            r7 = memory[r3];
            r7 |= (r12 << 8);
            r7 &= 0xFFF;
            r7 += 2;
            r12 += 0x20;
        a99C:
            r0 = memory[r2 + r7];
            r2 -= 1;
            memory[r2] = (byte)r0;
            SubS(out r12, r12, 0x10, out N, out V);
            if (N == V)
            {
                goto a99C;
            }
        a9AC:
            r5 <<= 1;
            if (r3 > r1)
            {
                goto a968;
            }
        a9B8:
            return memory;
        }

        private static void SubS(out uint dest, uint v1, uint v2, out bool N, out bool V)
        {
            dest = v1 - v2;
            N = (dest & 2147483648) != 0;
            V = ((((v1 & 2147483648) != 0) && ((v2 & 2147483648) == 0) && ((dest & 2147483648) == 0)) || ((v1 & 2147483648) == 0) && ((v2 & 2147483648) != 0) && ((dest & 2147483648) != 0));
        }

        static private unsafe int[] Search(byte* source, int position, int lenght) //Function taken from Nintenlord's compressor. All credits for this code goes to Nintenlord!
        {
            int SlidingWindowSize = 4096;
            int ReadAheadBufferSize = 18;

            if (position >= lenght)
                return new int[2] { -1, 0 };
            if ((position < 2) || ((lenght - position) < 2))
                return new int[2] { 0, 0 };

            List<int> results = new List<int>();

            for (int i = 1; (i < SlidingWindowSize) && (i < position); i++)
            {
                if (source[position - (i + 1)] == source[position])
                {
                    results.Add(i + 1);
                }
            }
            if (results.Count == 0)
                return new int[2] { 0, 0 };

            int amountOfBytes = 0;

            bool Continue = true;
            while (amountOfBytes < ReadAheadBufferSize && Continue)
            {
                amountOfBytes++;
                for (int i = results.Count - 1; i >= 0; i--)
                {
                    if (source[position + amountOfBytes] != source[position - results[i] + (amountOfBytes % results[i])])
                    {
                        if (results.Count > 1)
                            results.RemoveAt(i);
                        else
                            Continue = false;
                    }
                }
            }
            return new int[2] { amountOfBytes, results[0] }; //lenght of data is first, then position
        }

        static public unsafe byte[] LZ77_Compress(byte[] source, bool header = false)//Function taken from Nintenlord's compressor. All credits for this code goes to Nintenlord!
        {
            fixed (byte* pointer = &source[0])
            {
                return Compress(pointer, source.Length, header);
            }
        }

        static public unsafe byte[] Compress(byte* source, int lenght, bool header = false) //Function taken from Nintenlord's compressor. All credits for this code goes to Nintenlord!
        {
            int position = 0;
            int BlockSize = 8;

            List<byte> CompressedData = new List<byte>();
            if (header) //0x37375A4C
            {
                CompressedData.Add(0x4C);
                CompressedData.Add(0x5A);
                CompressedData.Add(0x37);
                CompressedData.Add(0x37);
            }
            CompressedData.Add(0x10);

            {
                byte* pointer = (byte*)&lenght;
                for (int i = 0; i < 3; i++)
                    CompressedData.Add(*(pointer++));
            }

            while (position < lenght)
            {
                byte isCompressed = 0;
                List<byte> tempList = new List<byte>();

                for (int i = 0; i < BlockSize; i++)
                {
                    int[] searchResult = Search(source, position, lenght);

                    if (searchResult[0] > 2)
                    {
                        byte add = (byte)((((searchResult[0] - 3) & 0xF) << 4) + (((searchResult[1] - 1) >> 8) & 0xF));
                        tempList.Add(add);
                        add = (byte)((searchResult[1] - 1) & 0xFF);
                        tempList.Add(add);
                        position += searchResult[0];
                        isCompressed |= (byte)(1 << (BlockSize - (i + 1)));
                    }
                    else if (searchResult[0] >= 0)
                        tempList.Add(source[position++]);
                    else
                        break;
                }
                CompressedData.Add(isCompressed);
                CompressedData.AddRange(tempList);
            }
            while (CompressedData.Count % 4 != 0)
                CompressedData.Add(0);

            return CompressedData.ToArray();
        }

        public static byte[] LZ77_FastCompress(byte[] source)
        {
            int DataLen = 4;
            DataLen += source.Length;
            DataLen += (int)Math.Ceiling((double)source.Length / 8);
            byte[] dest = new byte[DataLen];

            dest[0] = 0;
            dest[1] = (byte)(source.Length & 0xFF);
            dest[2] = (byte)((source.Length >> 8) & 0xFF);
            dest[3] = (byte)((source.Length >> 16) & 0xFF);

            int FilePos = 4;
            int UntilNext = 0;

            for (int SrcPos = 0; SrcPos < source.Length; SrcPos++)
            {
                if (UntilNext == 0)
                {
                    dest[FilePos] = 0;
                    FilePos++;
                    UntilNext = 8;
                }
                dest[FilePos] = source[SrcPos];
                FilePos++;
                UntilNext -= 1;
            }

            return dest;
        }

        public static byte[] LZ77_Decompress(byte[] source)
        {
            // This code converted from Elitemap 
            int DataLen;
            DataLen = source[1] | (source[2] << 8) | (source[3] << 16);
            byte[] dest = new byte[DataLen];
            int i, j, xin, xout;
            xin = 4;
            xout = 0;
            int length, offset, windowOffset, data;
            byte d;
            while (DataLen > 0)
            {
                d = source[xin++];
                if (d != 0)
                {
                    for (i = 0; i < 8; i++)
                    {
                        if ((d & 0x80) != 0)
                        {
                            data = ((source[xin] << 8) | source[xin + 1]);
                            xin += 2;
                            length = (data >> 12) + 3;
                            offset = data & 0xFFF;
                            windowOffset = xout - offset - 1;
                            for (j = 0; j < length; j++)
                            {
                                dest[xout++] = dest[windowOffset++];
                                DataLen--;
                                if (DataLen == 0)
                                {
                                    return dest;
                                }
                            }
                        }
                        else
                        {
                            dest[xout++] = source[xin++];
                            DataLen--;
                            if (DataLen == 0)
                            {
                                return dest;
                            }
                        }
                        d <<= 1;
                    }
                }
                else
                {
                    for (i = 0; i < 8; i++)
                    {
                        dest[xout++] = source[xin++];
                        DataLen--;
                        if (DataLen == 0)
                        {
                            return dest;
                        }
                    }
                }
            }
            return dest;
        }

        public static byte[] LZ77_DecompressWithHeader(byte[] source)
        {
            // This code converted from Elitemap 
            int DataLen;
            DataLen = source[5] | (source[6] << 8) | (source[7] << 16);
            byte[] dest = new byte[DataLen];
            int i, j, xin, xout;
            xin = 8;
            xout = 0;
            int length, offset, windowOffset, data;
            byte d;
            while (DataLen > 0)
            {
                d = source[xin++];
                if (d != 0)
                {
                    for (i = 0; i < 8; i++)
                    {
                        if ((d & 0x80) != 0)
                        {
                            data = ((source[xin] << 8) | source[xin + 1]);
                            xin += 2;
                            length = (data >> 12) + 3;
                            offset = data & 0xFFF;
                            windowOffset = xout - offset - 1;
                            for (j = 0; j < length; j++)
                            {
                                dest[xout++] = dest[windowOffset++];
                                DataLen--;
                                if (DataLen == 0)
                                {
                                    return dest;
                                }
                            }
                        }
                        else
                        {
                            dest[xout++] = source[xin++];
                            DataLen--;
                            if (DataLen == 0)
                            {
                                return dest;
                            }
                        }
                        d <<= 1;
                    }
                }
                else
                {
                    for (i = 0; i < 8; i++)
                    {
                        dest[xout++] = source[xin++];
                        DataLen--;
                        if (DataLen == 0)
                        {
                            return dest;
                        }
                    }
                }
            }
            return dest;
        }

        private static ushort[] CRC16Table = {
            0x0000, 0xC0C1, 0xC181, 0x0140, 0xC301, 0x03C0, 0x0280, 0xC241,
            0xC601, 0x06C0, 0x0780, 0xC741, 0x0500, 0xC5C1, 0xC481, 0x0440,
            0xCC01, 0x0CC0, 0x0D80, 0xCD41, 0x0F00, 0xCFC1, 0xCE81, 0x0E40,
            0x0A00, 0xCAC1, 0xCB81, 0x0B40, 0xC901, 0x09C0, 0x0880, 0xC841,
            0xD801, 0x18C0, 0x1980, 0xD941, 0x1B00, 0xDBC1, 0xDA81, 0x1A40,
            0x1E00, 0xDEC1, 0xDF81, 0x1F40, 0xDD01, 0x1DC0, 0x1C80, 0xDC41,
            0x1400, 0xD4C1, 0xD581, 0x1540, 0xD701, 0x17C0, 0x1680, 0xD641,
            0xD201, 0x12C0, 0x1380, 0xD341, 0x1100, 0xD1C1, 0xD081, 0x1040,
            0xF001, 0x30C0, 0x3180, 0xF141, 0x3300, 0xF3C1, 0xF281, 0x3240,
            0x3600, 0xF6C1, 0xF781, 0x3740, 0xF501, 0x35C0, 0x3480, 0xF441,
            0x3C00, 0xFCC1, 0xFD81, 0x3D40, 0xFF01, 0x3FC0, 0x3E80, 0xFE41,
            0xFA01, 0x3AC0, 0x3B80, 0xFB41, 0x3900, 0xF9C1, 0xF881, 0x3840,
            0x2800, 0xE8C1, 0xE981, 0x2940, 0xEB01, 0x2BC0, 0x2A80, 0xEA41,
            0xEE01, 0x2EC0, 0x2F80, 0xEF41, 0x2D00, 0xEDC1, 0xEC81, 0x2C40,
            0xE401, 0x24C0, 0x2580, 0xE541, 0x2700, 0xE7C1, 0xE681, 0x2640,
            0x2200, 0xE2C1, 0xE381, 0x2340, 0xE101, 0x21C0, 0x2080, 0xE041,
            0xA001, 0x60C0, 0x6180, 0xA141, 0x6300, 0xA3C1, 0xA281, 0x6240,
            0x6600, 0xA6C1, 0xA781, 0x6740, 0xA501, 0x65C0, 0x6480, 0xA441,
            0x6C00, 0xACC1, 0xAD81, 0x6D40, 0xAF01, 0x6FC0, 0x6E80, 0xAE41,
            0xAA01, 0x6AC0, 0x6B80, 0xAB41, 0x6900, 0xA9C1, 0xA881, 0x6840,
            0x7800, 0xB8C1, 0xB981, 0x7940, 0xBB01, 0x7BC0, 0x7A80, 0xBA41,
            0xBE01, 0x7EC0, 0x7F80, 0xBF41, 0x7D00, 0xBDC1, 0xBC81, 0x7C40,
            0xB401, 0x74C0, 0x7580, 0xB541, 0x7700, 0xB7C1, 0xB681, 0x7640,
            0x7200, 0xB2C1, 0xB381, 0x7340, 0xB101, 0x71C0, 0x7080, 0xB041,
            0x5000, 0x90C1, 0x9181, 0x5140, 0x9301, 0x53C0, 0x5280, 0x9241,
            0x9601, 0x56C0, 0x5780, 0x9741, 0x5500, 0x95C1, 0x9481, 0x5440,
            0x9C01, 0x5CC0, 0x5D80, 0x9D41, 0x5F00, 0x9FC1, 0x9E81, 0x5E40,
            0x5A00, 0x9AC1, 0x9B81, 0x5B40, 0x9901, 0x59C0, 0x5880, 0x9841,
            0x8801, 0x48C0, 0x4980, 0x8941, 0x4B00, 0x8BC1, 0x8A81, 0x4A40,
            0x4E00, 0x8EC1, 0x8F81, 0x4F40, 0x8D01, 0x4DC0, 0x4C80, 0x8C41,
            0x4400, 0x84C1, 0x8581, 0x4540, 0x8701, 0x47C0, 0x4680, 0x8641,
            0x8201, 0x42C0, 0x4380, 0x8341, 0x4100, 0x81C1, 0x8081, 0x4040
        };

        // from ndstool
        public static ushort CalcCRC16(byte[] data)
        {
            ushort crc = 0xFFFF;

            for (int i = 0; i < data.Length; i++)
            {
                crc = (ushort)((crc >> 8) ^ CRC16Table[(crc ^ data[i]) & 0xFF]);
            }

            return crc;
        }
    }
}
