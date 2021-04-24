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
using System.Drawing;
using System.Windows.Forms;

namespace NSMBe4 {
    /**
     * This is the core of all the image importing.
     * Takes one or more RGB bitmaps and outputs:
     *  - The image data common to all images. (tiled or non-tiled)
     *  - One palette for each image, so that viewing the image data with 
     *    it shows the original image.
     *    
     * It could still be optimized more, I know.
     * ~Dirbaio
     */

    public class ImageIndexer {
        private List<Box> boxes;
        private Dictionary<MultiColor, int> freqTable;
        private Dictionary<MultiColor, byte> colorTable;
        public Color[][] palettes;
        private MultiColor[] multiPalette;
        private int width, height;
        private int paletteCount, boxColorCount;
        private bool useAlpha;
        private List<Bitmap> bl;
        public Byte[,] imageData;

        public ImageIndexer(List<Bitmap> bl, bool useAlpha)
            : this(bl, 256, useAlpha, 0) {
        }

        public ImageIndexer(List<Bitmap> bl)
            : this(bl, 256, true, 0) {
        }

        public ImageIndexer(List<Bitmap> bl, int paletteCount, bool useAlpha, int transpCol) {
            this.bl = bl;
            this.paletteCount = paletteCount;
            this.useAlpha = useAlpha;

            boxColorCount = bl.Count * 3;
            //COMPUTE FREQUENCY TABLE

            freqTable = new Dictionary<MultiColor, int>();

            //Quick check just in case...
            width = bl[0].Width;
            height = bl[0].Height;
            foreach (Bitmap b in bl) {
                if (b.Width != width || b.Height != height)
                    throw new Exception("Not all images have the same size!!");
            }


            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++) {
                    MultiColor c = new MultiColor(boxColorCount);
                    for (int i = 0; i < bl.Count; i++) {
                        NSMBe4.NSBMD.ImageTexeler.LockBitmap iii = new NSBMD.ImageTexeler.LockBitmap(bl[i]);
                        iii.LockBits();
                        c.setColor(i, iii.GetPixel(x, y));
                        iii.UnlockBits();
                    }
                    c.calcHash();
                    if (!c.allTransparent())
                        if (freqTable.ContainsKey(c))
                            freqTable[c]++;
                        else
                            freqTable[c] = 1;
                }

            int ct = 0;
            foreach (MultiColor c in freqTable.Keys)
                if (c.someTransparent()) ct++;
            Console.Out.WriteLine("Transparent: " + ct);


            Dictionary<MultiColor, int> newFreqTable = new Dictionary<MultiColor, int>();
            foreach (MultiColor c in freqTable.Keys) {
                if (!c.deleteFlag) {
                    int cnt = freqTable[c];
                    foreach (MultiColor c2 in freqTable.Keys) {
                        if (c2 is null) continue;
                        if (c2.deleteFlag) continue;
                        if (c2 == c) continue;

                        if (c.diff(c2) == 0) {
                            cnt += freqTable[c2];
                            c.merge(c2);
                            c2.deleteFlag = true;
                        }
                    }
                    c.deleteFlag = true;
                    c.removeAllTransparent();
                    newFreqTable.Add(c, cnt);
                }
            }
            freqTable = newFreqTable;

            ct = 0;
            foreach (MultiColor c in freqTable.Keys)
                if (c.someTransparent()) ct++;
            Console.Out.WriteLine("Transparent2: " + ct);

            // NOW CREATE THE PALETTE ZONES
            Box startBox = shrinkBox(new Box(boxColorCount));
            boxes = new List<Box>();
            boxes.Add(startBox);

            while (boxes.Count < (useAlpha ? paletteCount - 1 : paletteCount)) {
                Console.Out.WriteLine(boxes.Count);
                Box bo = getDominantBox();
                if (bo is null)
                    break;

                split(bo);
            }


            multiPalette = new MultiColor[paletteCount];
            for (int j = useAlpha ? 1 : 0; j < paletteCount; j++)
                if ((useAlpha ? j : j + 1) <= boxes.Count)
                    multiPalette[j] = boxes[useAlpha ? j - 1 : j].center();

            //NOW CREATE THE PALETTE COLORS
            palettes = new Color[bl.Count][];
            for (int i = 0; i < bl.Count; i++) {
                palettes[i] = new Color[paletteCount];
                for (int j = useAlpha ? 1 : 0; j < paletteCount; j++) {
                    if ((useAlpha ? j : j + 1) > boxes.Count)
                        palettes[i][j] = palettes[i][j - 1];//Color.Fuchsia;
                    else
                        palettes[i][j] = boxes[useAlpha ? j - 1 : j].center().getColor(i);
                    //                Console.Out.WriteLine(i + ": " + boxes[i] + ": "+ palette[i]);
                }
                if (useAlpha)
                    palettes[i][0] = Color.Transparent;

            }

            //NOW MAP ORIGINAL COLORS TO PALETTE ENTRIES
            colorTable = new Dictionary<MultiColor, byte>();
            foreach (MultiColor c in freqTable.Keys)
                colorTable[c] = closestMultiColor(c);

            //NOW INDEX THE WHOLE IMAGES
            imageData = new byte[width, height];
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    MultiColor c = new MultiColor(boxColorCount);
                    for (int i = 0; i < bl.Count; i++) {
                        NSMBe4.NSBMD.ImageTexeler.LockBitmap iii = new NSBMD.ImageTexeler.LockBitmap(bl[i]);
                        iii.LockBits();
                        c.setColor(i, iii.GetPixel(x, y));
                        iii.UnlockBits();
                    }
                    c.calcHash();
                    if (c.allTransparent())
                        imageData[x, y] = (byte)transpCol;
                    else
                        imageData[x, y] = closestMultiColor(c);
                }
            }

            Console.Out.WriteLine("DONE");
            /*

                }*/
        }





        //PUBLIC DATA-RETRIEVING FUNCTIONS
        public byte[] getTiledImageData() {

            byte[] palettedImage = new byte[width * height];
            int tileCount = width * height / 64;
            int tileWidth = width / 8;

            for (int t = 0; t < tileCount; t++)
                for (int y = 0; y < 8; y++)
                    for (int x = 0; x < 8; x++) {
                        int tx = (t % tileWidth) * 8;
                        int ty = (int)(t / tileWidth) * 8;
                        palettedImage[t * 64 + y * 8 + x] =
                            imageData[tx + x, ty + y];
                    }
            return palettedImage;
        }
        public byte[] getTiledImageDataPart(int px, int py, int ptx, int pty) {

            byte[] palettedImage = new byte[ptx * pty];
            int tileCount = ptx * pty / 64;
            int tileWidth = ptx / 8;

            for (int t = 0; t < tileCount; t++)
                for (int y = 0; y < 8; y++)
                    for (int x = 0; x < 8; x++) {
                        int tx = (t % tileWidth) * 8;
                        int ty = (int)(t / tileWidth) * 8;
                        palettedImage[t * 64 + y * 8 + x] =
                            imageData[tx + x + px, ty + y + py];
                    }
            return palettedImage;
        }

        public Bitmap previewImage(int i) {
            Bitmap b = new Bitmap(width, height);
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++) {
                    b.SetPixel(x, y, palettes[i][imageData[x, y]]);
                }
            return b;
        }





        //ALGORITHM CORE
        private byte closestMultiColor(MultiColor mc) {
            if (mc.allTransparent()) return 0;
            else {
                int best = -1;
                float bestd = float.PositiveInfinity;
                for (int i = 0; i < multiPalette.Length; i++) {
                    if (multiPalette[i] is null) continue;
                    float d = mc.diff(multiPalette[i]);
                    if (d < bestd || best == -1) {
                        best = i;
                        bestd = d;
                    }
                }
                return (byte)best;
            }
        }

        private void split(Box b) {
            byte dim = b.dominantDimensionNum(freqTable); //0, 1, 2 = r, g, b
            List<byteint> values = new List<byteint>();
            int total = 0;
            foreach (MultiColor c in freqTable.Keys)
                if (b.inside(c)) {
                    if (!c.transp[dim]) {
                        values.Add(new byteint(c.data[dim], freqTable[c]));
                        total += freqTable[c];
                    }
                }
            values.Sort();

            if (values.Count == 0)
                throw new Exception("WTF?!");

            byte m = median(values, total);
            if (m == values[0].b)
                m++;

            Console.Out.Write("Split: " + b + " ");
            Box nb = new Box(b);
            nb.setDimMax(dim, (byte)(m - 1));
            b.setDimMin(dim, m);
            boxes.Add(shrinkBox(nb));
            boxes.Remove(b);
            boxes.Add(shrinkBox(b));
            //            Console.Out.WriteLine(b + " " + nb);
        }

        private byte median(List<byteint> values, int total) {
            //Naive median algorithm
            //Binary search would be better? 
            int acum = 0;
            foreach (byteint val in values) {
                acum += val.i;
                if (acum * 2 > total)
                    return val.b;
            }
            //median  is best, not mean!
            /*
            int totalval = 0;
            foreach (byteint val in values)
            {
                totalval += val.b;
            }
            return (byte)(totalval / total);*/

            throw new Exception("Bad, bad, bad!");
        }

        private Box getDominantBox() {
            Box best = null;
            int bestDim = 0;

            foreach (Box b in boxes) {
                int dim = b.dominantDimension(freqTable);
                if ((dim > bestDim || best is null) && b.canSplit(freqTable)) {
                    bestDim = dim;
                    best = b;
                }
            }
            return best;
        }

        private Box shrinkBox(Box b) {
            byte[] min = (byte[])b.min.Clone();
            byte[] max = (byte[])b.max.Clone();
            bool[] def = new bool[b.max.Length];

            foreach (MultiColor c in freqTable.Keys)
                if (b.inside(c)) {
                    for (int i = 0; i < c.data.Length; i++) {
                        if (!c.transp[i])
                            if (def[i]) {
                                if (min[i] > c.data[i]) min[i] = c.data[i];
                                if (max[i] < c.data[i]) max[i] = c.data[i];
                            } else {
                                min[i] = c.data[i];
                                max[i] = c.data[i];
                                def[i] = true;
                            }
                    }
                }

            return new Box(min, max);
        }

        public static byte reduce(int c) {
            return (byte)((c >> 3) << 3);
        }





        //HELPER CLASSES
        private class MultiColor {
            public byte[] data;
            public bool[] transp;
            public bool deleteFlag;
            public MultiColor(int count) {
                data = new byte[count];
                transp = new bool[count];
                deleteFlag = false;
            }

            public void setColor(int i, Color c) {
                transp[i * 3 + 0] = c.A < 128;
                transp[i * 3 + 1] = c.A < 128;
                transp[i * 3 + 2] = c.A < 128;
                if (c.A >= 128) {
                    data[i * 3 + 0] = reduce(c.R);
                    data[i * 3 + 1] = reduce(c.G);
                    data[i * 3 + 2] = reduce(c.B);
                } else {
                    data[i * 3 + 0] = 0;
                    data[i * 3 + 1] = 255;
                    data[i * 3 + 2] = 255;
                }
            }

            public void merge(MultiColor b) {
                for (int i = 0; i < data.Length; i++)
                    if (transp[i]) {
                        transp[i] = b.transp[i];
                        data[i] = b.data[i];
                    }
                calcHash();
            }

            public Color getColor(int i) {
                if (transp[i * 3]) return Color.Transparent;
                return Color.FromArgb(data[i * 3], data[i * 3 + 1], data[i * 3 + 2]);
            }

            public bool allTransparent() {
                for (int i = 0; i < transp.Length; i += 3)
                    if (!transp[i]) return false;
                return true;
            }
            public bool someTransparent() {
                for (int i = 0; i < transp.Length; i += 3)
                    if (transp[i]) return true;
                return false;
            }

            private int thehash;
            public void calcHash() {
                unchecked {
                    const int p = 16777619;
                    int hash = (int)2166136261;

                    for (int i = 0; i < data.Length; i++) {
                        hash = (hash ^ data[i]) * p;
                        if (transp[i]) hash++;
                    }

                    hash += hash << 13;
                    hash ^= hash >> 7;
                    hash += hash << 3;
                    hash ^= hash >> 17;
                    hash += hash << 5;

                    thehash = hash;
                }
            }

            public override int GetHashCode() {
                return thehash;
            }

            public override bool Equals(object obj) {
                if (obj is MultiColor) {
                    MultiColor c = obj as MultiColor;
                    if (data.Length != c.data.Length)
                        return false;

                    for (int i = 0; i < data.Length; i++) {
                        if (data[i] != c.data[i]) return false;
                        if (transp[i] != c.transp[i]) return false;
                    }
                    return true;
                } else return false;
            }

            public float diff(MultiColor b) {
                float res = 0;
                for (int i = 0; i < data.Length; i++)
                    if (!transp[i] && !b.transp[i]) {
                        float d = data[i] - b.data[i];
                        res += d * d;
                    }
                return res;
            }

            internal void removeAllTransparent() {
                for (int i = 0; i < data.Length; i++)
                    if (transp[i]) {
                        transp[i] = false;
                        data[i] = 0;
                    }
            }
        }

        private class Box {
            public byte[] min, max;
            private bool splittable = false;
            private bool splittablecached = false;
            public Box(byte[] min, byte[] max) {
                this.min = min;
                this.max = max;
            }

            public Box(int count) {
                min = new byte[count];
                max = new byte[count];
                for (int i = 0; i < count; i++) {
                    min[i] = 0;
                    max[i] = 255;
                }
            }

            public Box(Box b) {
                this.min = (byte[])b.min.Clone();
                this.max = (byte[])b.max.Clone();
            }

            public bool inside(MultiColor c) {
                for (int i = 0; i < min.Length; i++) {
                    if (c.transp[i]) continue;
                    if (c.data[i] < min[i]) return false;
                    if (c.data[i] > max[i]) return false;
                }
                return true;
            }

            public int dominantDimension(Dictionary<MultiColor, int> freqTable) {
                int d = dominantDimensionNum(freqTable);
                if (d == 255) return 0;
                return max[d] - min[d];
            }

            public byte dominantDimensionNum(Dictionary<MultiColor, int> freqTable) {
                int d = -1;
                int dl = -1;
                for (int i = 0; i < min.Length; i++) {
                    int il = max[i] - min[i];
                    if (il > dl && canSplitInDim(i, freqTable)) {
                        dl = il;
                        d = i;
                    }
                }
                return (byte)d;
            }

            public void setDimMin(byte d, byte a) {
                min[d] = a;
                splittablecached = false;
            }
            public void setDimMax(byte d, byte a) {
                max[d] = a;
                splittablecached = false;
            }

            public bool canSplitInDim(int i, Dictionary<MultiColor, int> freqTable) {
                byte data = 0;
                bool seen = false;

                foreach (MultiColor c in freqTable.Keys) {
                    if (inside(c)) {
                        if (!c.transp[i]) {
                            if (!seen) //First val we see
                            {
                                seen = true;
                                data = c.data[i];
                            } else {
                                if (data != c.data[i])
                                    return true;
                            }
                        }
                    }
                }
                return false;
            }

            public bool canSplit(Dictionary<MultiColor, int> freqTable) {
                if (splittablecached) return splittable;
                else {
                    splittablecached = true;
                    splittable = canSplit2(freqTable);
                    return splittable;
                }
            }
            public bool canSplit2(Dictionary<MultiColor, int> freqTable) {
                //Whoa... This gets complicated if I have to
                //take into acount the "don't care" of transparent colors...
                byte[] data = new byte[min.Length];
                bool[] seen = new bool[min.Length];

                foreach (MultiColor c in freqTable.Keys) {
                    if (inside(c)) {
                        for (int i = 0; i < min.Length; i++) {
                            if (!c.transp[i]) {
                                if (!seen[i]) //First val we see
                                {
                                    seen[i] = true;
                                    data[i] = c.data[i];
                                } else {
                                    if (data[i] != c.data[i])
                                        return true;
                                }
                            }
                        }
                    }
                }
                return false;
            }

            public MultiColor center() {
                MultiColor res = new MultiColor(min.Length);
                for (int i = 0; i < min.Length; i++)
                    res.data[i] = (byte)((min[i] + max[i]) / 2);
                return res;
            }

            public override string ToString() {
                return arr2str(min) + " - " + arr2str(max);
                //                return "("+r1+"-"+r2+","+g1+"-"+g2+","+b1+"-"+b2+")";
            }

            private string arr2str(byte[] a) {
                string s = "(" + a[0];
                for (int i = 1; i < a.Length; i++)
                    s += ", " + a[i];
                return s + ")";
            }
        }

        private class byteint : IComparable {
            public byte b;
            public int i;
            public byteint(byte b, int i) {
                this.b = b;
                this.i = i;
            }

            public int CompareTo(object obj) {
                byteint bi = obj as byteint;
                return b.CompareTo(bi.b);
            }

            public static bool operator <(byteint a, byteint b) {
                return a.b < b.b;
            }
            public static bool operator >(byteint a, byteint b) {
                return a.b > b.b;
            }

            public override string ToString() {
                return "(" + b + ", " + i + ")";
            }
        }





        //GENERAL PURPOSE FUNCTIONS
        public static Color[] createPaletteForImage(Bitmap b) {
            return createPaletteForImage(b, 256, false);
        }

        public static Color[] createPaletteForImage(Bitmap b, int palLen) {
            return createPaletteForImage(b, palLen, false);
        }

        public static Color[] createPaletteForImage(Bitmap b, int palLen, bool alpha) {
            List<Bitmap> bl = new List<Bitmap>();
            bl.Add(b);

            ImageIndexer i = new ImageIndexer(bl, palLen, alpha, 0);

            return i.palettes[0];
        }

        public static byte[] indexImageWithPalette(Bitmap b, Color[] palette) {
            //More efficient now.
            byte[] palettedImage = new byte[b.Width * b.Height];
            int tileCount = b.Width * b.Height / 64;
            int tileWidth = b.Width / 8;

            for (int t = 0; t < tileCount; t++)
                for (int y = 0; y < 8; y++)
                    for (int x = 0; x < 8; x++) {
                        int tx = (t % tileWidth) * 8;
                        int ty = (int)(t / tileWidth) * 8;
                        Color c = b.GetPixel(tx + x, ty + y);
                        if (c.A != 0) {
                            c = Color.FromArgb(c.R, c.G, c.B);

                            palettedImage[t * 64 + y * 8 + x] = closest(c, palette);
                        } else
                            palettedImage[t * 64 + y * 8 + x] = 0;
                    }

            return palettedImage;
        }

        public static byte[] indexImageWithPalette2(Bitmap b, Color[] palette) {
            //More efficient now.
            byte[] palettedImage = new byte[b.Width * b.Height];
            int tileCount = b.Width * b.Height / 64;
            int tileWidth = b.Width / 8;

            for (int t = 0; t < tileCount; t++)
                for (int y = 0; y < 8; y++)
                    for (int x = 0; x < 8; x++) {
                        int tx = (t % tileWidth) * 8;
                        int ty = (int)(t / tileWidth) * 8;
                        Color c = b.GetPixel(tx + x, ty + y);
                        if (c.A != 0) {
                            c = Color.FromArgb(c.R, c.G, c.B);

                            palettedImage[t * 64 + y * 8 + x] = closest(c, palette);
                        } else
                            palettedImage[t * 64 + y * 8 + x] = 0;
                    }

            return palettedImage;
        }

        public static float colorDifference(Color a, Color b) {
            if (a.A != b.A) return float.MaxValue;

            float res = 0;
            res += (a.R - b.R) * (a.R - b.R) / 40;
            res += (a.G - b.G) * (a.G - b.G) / 40;
            res += (a.B - b.B) * (a.B - b.B) / 40;

            if (res > float.MaxValue)
                return float.MaxValue;

            return res;
        }

        public static float colorDifferenceWithoutAlpha(Color a, Color b) {
            int res = 0;
            res += (a.R - b.R) * (a.R - b.R) / 40;
            res += (a.G - b.G) * (a.G - b.G) / 40;
            res += (a.B - b.B) * (a.B - b.B) / 40;

            if (res > float.MaxValue)
                return float.MaxValue;

            return (ushort)res;
        }


        public static byte closest(Color c, Color[] palette) {
            int best = 0;
            float bestDif = colorDifference(c, palette[0]);
            for (int i = 0; i < palette.Length; i++) {
                float dif = colorDifference(c, palette[i]);
                if (dif < bestDif) {
                    bestDif = dif;
                    best = i;
                }
            }
            if (best >= 256)
                Console.Out.WriteLine("GRAAH");
            return (byte)best;
        }
    }
}
