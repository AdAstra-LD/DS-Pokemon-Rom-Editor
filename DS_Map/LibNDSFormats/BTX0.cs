using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSPRE.LibNDSFormats
{
    internal class BTX0
    {
        public static uint PaletteIndex;

        public static uint PaletteCount;

        public static uint PaletteSize;

        public static uint ColorCount;

        public static uint ImageOffset;

        public static uint PaletteOffset;

        public static uint ImageWidth;

        public static uint ImageHeight;
        public static Bitmap Read(byte[] BTXFile)
        {
            if (BitConverter.ToUInt32(BTXFile, 0) != 811095106)
            {
                return null;
            }
            uint num = BitConverter.ToUInt32(BTXFile, 16);
            if (BitConverter.ToUInt32(BTXFile, (int)num) != 811091284)
            {
                return null;
            }
            uint num2 = num + BitConverter.ToUInt16(BTXFile, (int)(num + 14));
            uint num3 = (ImageOffset = num + BitConverter.ToUInt32(BTXFile, (int)(num + 20)));
            uint num4 = BitConverter.ToUInt32(BTXFile, (int)(num + 48)) << 3;
            uint num5 = num + BitConverter.ToUInt32(BTXFile, (int)(num + 52));
            uint num6 = (PaletteOffset = num + BitConverter.ToUInt32(BTXFile, (int)(num + 56)));
            uint num7 = BTXFile[num2 + 1];
            uint num8 = BitConverter.ToUInt16(BTXFile, (int)(num2 + 12 + num7 * 4 + 6));
            uint num9 = (uint)(8 << (((int)num8 >> 4) & 7));
            uint num10 = (num8 >> 10) & 7;
            uint num11 = (PaletteCount = BTXFile[num5 + 1]);
            PaletteSize = num4;
            if (num10 == 3)
            {
                Color[] array = new Color[num4 / num11 / 2];
                if (num4 < 64 && num11 >= 2)
                {
                    array = new Color[(BTXFile.Length - num6) / 2];
                }
                ColorCount = (uint)array.Length;
                for (int i = 0; i < array.Length; i++)
                {
                    ushort num12 = BitConverter.ToUInt16(BTXFile, (int)(num6 + PaletteIndex * (ColorCount * 2)) + i * 2);
                    uint red = (uint)((num12 & 0x1F) << 3);
                    uint green = (uint)(num12 & 0x3E0) >> 2;
                    uint blue = (uint)(num12 & 0x7C00) >> 7;
                    array[i] = Color.FromArgb(255, (int)red, (int)green, (int)blue);
                }
                ImageWidth = num9;
                ImageHeight = (num6 - num3) * 2 / num9;
                Bitmap bitmap = new Bitmap((int)ImageWidth, (int)ImageHeight);
                uint num13 = 0u;
                uint num14 = 0u;
                for (int j = (int)num3; j < num6; j++)
                {
                    uint num15 = BTXFile[j];
                    uint[] array2 = new uint[2]
                    {
                    num15 & 0xF,
                    num15 >> 4
                    };
                    for (int k = 0; k < array2.Length; k++)
                    {
                        bitmap.SetPixel((int)num13, (int)num14, array[array2[k]]);
                        num13++;
                    }
                    if (num13 >= num9)
                    {
                        num13 = 0u;
                        num14++;
                    }
                }
                return bitmap;
            }
            return null;
        }

        public static byte[] Write(byte[] BTXFile, Bitmap bm)
        {
            HashSet<Color> hashSet = new HashSet<Color>();
            uint num = 0u;
            uint num2 = 0u;
            for (int i = 0; i < bm.Width * bm.Height; i++)
            {
                hashSet.Add(bm.GetPixel((int)num, (int)num2));
                num++;
                if (num >= bm.Width)
                {
                    num = 0u;
                    num2++;
                }
            }
            Color[] array = hashSet.ToArray();
            num = 0u;
            num2 = 0u;
            for (int j = (int)ImageOffset; j < PaletteOffset; j++)
            {
                Color pixel = bm.GetPixel((int)num, (int)num2);
                num++;
                uint num3 = 0u;
                for (int k = 0; k < array.Length; k++)
                {
                    if (array[k] == pixel)
                    {
                        num3 = (uint)k;
                        break;
                    }
                }
                pixel = bm.GetPixel((int)num, (int)num2);
                num++;
                for (int l = 0; l < array.Length; l++)
                {
                    if (array[l] == pixel)
                    {
                        num3 += (uint)(l << 4);
                        break;
                    }
                }
                BTXFile[j] = (byte)num3;
                if (num >= ImageWidth)
                {
                    num = 0u;
                    num2++;
                }
            }
            for (int m = 0; m < array.Length; m++)
            {
                uint num4 = (uint)Math.Round((double)(int)array[m].R / 8.0);
                uint num5 = (uint)Math.Round((double)(int)array[m].G / 8.0);
                uint num6 = (uint)Math.Round((double)(int)array[m].B / 8.0);
                if (num4 > 31)
                {
                    num4 = 31u;
                }
                if (num5 > 31)
                {
                    num5 = 31u;
                }
                if (num6 > 31)
                {
                    num6 = 31u;
                }
                uint num7 = num4 + (num5 << 5) + (num6 << 10);
                BTXFile[PaletteOffset + PaletteIndex * (ColorCount * 2) + m * 2] = (byte)num7;
                BTXFile[PaletteOffset + PaletteIndex * (ColorCount * 2) + m * 2 + 1] = (byte)(num7 >> 8);
            }
            return BTXFile;
        }
    }

}
