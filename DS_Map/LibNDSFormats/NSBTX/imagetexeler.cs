using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace NSMBe4.NSBMD
{
    public class ImageTexeler
    {
        Bitmap img;

        List<Color[]> palettes;

        List<int> paletteCounts;
        int[,] paletteNumbers;
        float[,] paletteDiffs;

        public byte[] f5data, texdata;
        public Color[] finalPalette;

        /*public ImageTexeler(Bitmap img, int paletteMaxNum)
        {
            this.img = img;

            int tx = img.Width / 4;
            int ty = img.Height / 4;
            palettes = new Color[tx * ty][];
            paletteCounts = new int[tx * ty];
            paletteNumbers = new int[tx, ty];
            paletteDiffs = new float[tx * ty, tx * ty];

            int palNum = 0;
            for (int x = 0; x < tx; x++)
                for (int y = 0; y < ty; y++)
                {
                    ImageIndexerFast iif = new ImageIndexerFast(img, x * 4, y * 4);
                    palettes[palNum] = iif.palette;
                    paletteNumbers[x, y] = palNum;
                    paletteCounts[palNum] = 1;
                    int similar = calcPaletteDiffs(palNum);
                    /*                    if (similar != -1)
                                        {
                                            paletteCounts[palNum] = 0;
                                            paletteCounts[similar]++;
                                            paletteNumbers[x, y] = similar;
                                        }
                                        
                    palNum++;
                }

            while (countUsedPalettes() > paletteMaxNum)
            {
                Console.Out.WriteLine(countUsedPalettes());
                int besta = -1;
                int bestb = -1;
                float bestDif = float.MaxValue;


                //Find the two most similar palettes
                for (int i = 0; i < palettes.Length; i++)
                {
                    if (paletteCounts[i] == 0) continue;
                    for (int j = 0; j < palettes.Length; j++)
                    {
                        if (i == j) continue;
                        if (paletteCounts[j] == 0) continue;

                        if (paletteDiffs[i, j] < bestDif)
                        {
                            bestDif = paletteDiffs[i, j];
                            besta = j;
                            bestb = i;
                        }
                    }
                }

                //Merge the Palettes!!!
                palettes[besta] = palMerge(palettes[besta], palettes[bestb]);
                calcPaletteDiffs(besta);
                paletteCounts[besta] += paletteCounts[bestb];
                paletteCounts[bestb] = 0;

                for (int x = 0; x < tx; x++)
                    for (int y = 0; y < ty; y++)
                        if (paletteNumbers[x, y] == bestb)
                            paletteNumbers[x, y] = besta;
            }



            //CREATE THE FINAL PAL
            int currNum = 0;
            finalPalette = new Color[paletteMaxNum * 4];
            int[] newPalNums = new int[palettes.Length];
            for (int i = 0; i < palettes.Length; i++)
            {
                if (paletteCounts[i] != 0)
                {
                    transparentToTheEnd(palettes[i]);//
                    newPalNums[i] = currNum;
                    Array.Copy(palettes[i], 0, finalPalette, currNum * 4, 4);
                    currNum++;
                }
            }

            ByteArrayOutputStream texDat = new ByteArrayOutputStream();
            ByteArrayOutputStream f5Dat = new ByteArrayOutputStream();
            for (int y = 0; y < ty; y++)
                for (int x = 0; x < tx; x++)
                {
                    //Find out if texel has transparent.

                    bool hasTransparent = false;
                    for (int yy = 0; yy < 4; yy++)
                        for (int xx = 0; xx < 4; xx++)
                        {
                            Color coll = img.GetPixel(x * 4 + xx, y * 4 + yy);
                            if (coll.A < 128)
                                hasTransparent = true;
                        }

                    //WRITE THE IMAGE DATA
                    for (int yy = 0; yy < 4; yy++)
                    {
                        byte b = 0;
                        byte pow = 1;
                        for (int xx = 0; xx < 4; xx++)
                        {
                            Color coll = img.GetPixel(x * 4 + xx, y * 4 + yy);
                            byte col;
                            if (coll.A < 128)
                            {
                                col = 3;
                            }
                            else
                            {
                                col = (byte)ImageIndexer.closest(coll, palettes[paletteNumbers[x, y]]);
                                if (col == 3) { col = 2; }
                            }
                            b |= (byte)(pow * col);
                            pow *= 4;
                        }
                        texDat.writeByte(b);
                    }


                    //WRITE THE FORMAT-5 SPECIFIC DATA
                    ushort dat = (ushort)(newPalNums[paletteNumbers[x, y]] * 2);
                    if (!hasTransparent || !ContainsTransparent(img))
                    {
                        dat |= 2 << 14;
                    }
                    f5Dat.writeUShort(dat);
                }

            f5data = f5Dat.getArray();
            texdata = texDat.getArray();

        }*/
        public ImageTexeler(Bitmap img, int paletteMaxNum, ref System.ComponentModel.BackgroundWorker bw, bool color2 = false)
        {
            this.color2 = false;
            color2 = false;
            //this.color2 = color2;
            //bool trans = true;//ContainsTransparent(img);
            Bitmap im = new Bitmap(img.Width, img.Height, System.Drawing.Imaging.PixelFormat.Format64bppPArgb);
            using (Graphics gr = Graphics.FromImage(im))
            {
                gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                gr.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                gr.DrawImage(img, 0, 0);
            }
            this.img = im;
            LockBitmap iii = new LockBitmap(img);
            iii.LockBits();
            int tx = img.Width / 4;
            int ty = img.Height / 4;
            palettes = new List<Color[]>();//[tx * ty][];
            paletteCounts = new List<int>();//new int[tx * ty];
            paletteNumbers = new int[tx, ty];
            paletteDiffs = new float[tx * ty, tx * ty];
            double add = 18d / (double)(tx * ty);
            double Progress = 10;
            int palNum = 0;
            double percent = 0;
            double add2 = 100d / (double)(tx * ty);
            for (int x = 0; x < tx; x++)
                for (int y = 0; y < ty; y++)
                {

                    Bitmap ni = new Bitmap(4, 4/*, System.Drawing.Imaging.PixelFormat.Format16bppRgb555*/);
                    /*using (Graphics gr = Graphics.FromImage(ni))
                    {
                        //gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        //gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        //gr.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                        //gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;//.AntiAlias;
                        gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                        gr.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                        gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                        gr.DrawImage(this.img, new Rectangle(0, 0, 4, 4), new Rectangle(x * 4, y * 4, 4, 4), GraphicsUnit.Pixel);
                    }*/
                    LockBitmap nn = new LockBitmap(ni);
                    nn.LockBits();
                    bool haveTransparent = false;
                    for (int x1 = 0; x1 < 4; x1++)
                        for (int y1 = 0; y1 < 4; y1++)
                        {
                            Color c = iii.GetPixel(x * 4 + x1, y * 4 + y1);
                            nn.SetPixel(x1, y1, c);
                            if (c.A < 128)
                            {
                                haveTransparent = true;
                                //goto end;
                            }
                        }
                //end:
                    nn.UnlockBits();
                    List<Color> pal1 = new List<Color>();
                    pal1.AddRange(ImageIndexer.createPaletteForImage(ni, 4, haveTransparent));
                    //if(haveTransparent){pal1.Add(Color.Transparent);}
                    Color[] pal = pal1.ToArray(); //ImageIndexer.createPaletteForImage(ni, 4, false);
                    transparentToTheEnd(pal);
                    //if (haveTransparent)
                    //{
                    //    pal[0] = Color.Transparent;
                    //}
                    //ImageIndexerFast iif = new ImageIndexerFast(img, x * 4, y * 4);

                    int con = contains(palettes.ToArray(), pal);
                    if (con != -1)
                    {
                        paletteNumbers[x, y] = con;
                        //paletteCounts.Add(1);
                    }
                    else
                    {
                        palettes.Add(pal);//[palNum] = pal;//iif.palette;
                        paletteNumbers[x, y] = palettes.Count - 1;
                        paletteCounts.Add(1);//[palNum] = 1;
                        calcPaletteDiffs(palettes.Count - 1);
                    }
                    //int similar = calcPaletteDiffs(palNum);
                    //                    if (similar != -1)
                    //                    {
                    //                        paletteCounts[palNum] = 0;
                    //                        paletteCounts[similar]++;
                    //                        paletteNumbers[x, y] = similar;
                    //                    }

                    palNum++;
                    Progress += add;
                    bw.ReportProgress((int)Progress, "Generating Picture " + percent.ToString("000") + "%");
                    percent += add2;
                    if (bw.CancellationPending) { bw.ReportProgress(0, "Canceled"); return; }
                }
            percent = 0;
            add2 = 100d / (((double)countUsedPalettes() - (double)paletteMaxNum));
            add = 74d / (((double)countUsedPalettes() - (double)paletteMaxNum));
            //double iw = 0;
            while (countUsedPalettes() > paletteMaxNum)
            {
                //iw += 1;
                //Console.Out.WriteLine(countUsedPalettes());
                int besta = -1;
                int bestb = -1;
                float bestDif = float.MaxValue;

                //Find the two most similar palettes
                for (int i = 0; i < palettes.Count; i++)
                {
                    if (paletteCounts[i] == 0) continue;
                    for (int j = 0; j < palettes.Count; j++)
                    {
                        if (i == j) continue;
                        if (paletteCounts[j] == 0) continue;

                        if (paletteDiffs[i, j] < bestDif)
                        {
                            bestDif = paletteDiffs[i, j];
                            besta = j;
                            bestb = i;
                        }
                    }
                }

                //Merge the Palettes!!!
                palettes[besta] = palMerge(palettes[besta], palettes[bestb]);
                calcPaletteDiffs(besta);
                paletteCounts[besta] += paletteCounts[bestb];
                paletteCounts[bestb] = 0;

                for (int x = 0; x < tx; x++)
                    for (int y = 0; y < ty; y++)
                        if (paletteNumbers[x, y] == bestb)
                            paletteNumbers[x, y] = besta;
                Progress += add;
                percent += add2;
                bw.ReportProgress((int)Progress, "Generating Palette " + percent.ToString("000") + "%");

                if (bw.CancellationPending) { bw.ReportProgress(0, "Canceled"); return; }
            }



            //CREATE THE FINAL PAL
            int currNum = 0;
            finalPalette = new Color[countUsedPalettes() * 4];
            int[] newPalNums = new int[palettes.Count];
            for (int i = 0; i < palettes.Count; i++)
            {
                if (paletteCounts[i] != 0)
                {
                    transparentToTheEnd(palettes[i]);//
                    newPalNums[i] = currNum;
                    Array.Copy(palettes[i], 0, finalPalette, currNum * 4, 4);
                    currNum++;
                }
            }
           
            ByteArrayOutputStream texDat = new ByteArrayOutputStream();
            ByteArrayOutputStream f5Dat = new ByteArrayOutputStream();
            for (int y = 0; y < ty; y++)
                for (int x = 0; x < tx; x++)
                {
                    //Find out if texel has transparent.

                    bool hasTransparent = false;
                    for (int yy = 0; yy < 4; yy++)
                        for (int xx = 0; xx < 4; xx++)
                        {
                            Color coll = iii.GetPixel(x * 4 + xx, y * 4 + yy);
                            if (coll.A < 128)
                            {
                                hasTransparent = true;
                                goto End;
                            }
                        }
                End:

                    //WRITE THE IMAGE DATA
                    for (int yy = 0; yy < 4; yy++)
                    {
                        byte b = 0;
                        byte pow = 1;
                        for (int xx = 0; xx < 4; xx++)
                        {
                            Color coll = iii.GetPixel(x * 4 + xx, y * 4 + yy);
                            byte col;
                            if (coll.A < 128)
                            {
                                col = 3;
                            }
                            else
                            {
                                List<Color> colo = new List<Color>();
                                colo.AddRange(palettes[paletteNumbers[x, y]]);
                                if (hasTransparent)
                                {
                                    colo.RemoveAt(3);
                                }
                                col = (byte)ImageIndexer.closest(coll, colo.ToArray());
                                //if (col == 3) { col = 2; }
                            }
                            b |= (byte)(pow * col);
                            pow *= 4;
                        }
                        texDat.writeByte(b);
                    }
                    

                    //WRITE THE FORMAT-5 SPECIFIC DATA
                    ushort dat = (ushort)(newPalNums[paletteNumbers[x, y]] * 2);
                    if (!hasTransparent/* || !ContainsTransparent(img)*/)
                    {
                        dat |= 2 << 14;
                    }
                    f5Dat.writeUShort(dat);
                }
            iii.UnlockBits();

            f5data = f5Dat.getArray();
            texdata = texDat.getArray();
        }
        public class LockBitmap
        {
            Bitmap source = null;
            IntPtr Iptr = IntPtr.Zero;
            BitmapData bitmapData = null;

            public byte[] Pixels { get; set; }
            public int Depth { get; private set; }
            public int Width { get; private set; }
            public int Height { get; private set; }

            public LockBitmap(Bitmap source)
            {
                this.source = source;
            }

            /// <summary>
            /// Lock bitmap data
            /// </summary>
            public void LockBits()
            {
                try
                {
                    // Get width and height of bitmap
                    Width = source.Width;
                    Height = source.Height;

                    // get total locked pixels count
                    int PixelCount = Width * Height;

                    // Create rectangle to lock
                    Rectangle rect = new Rectangle(0, 0, Width, Height);

                    // get source bitmap pixel format size
                    Depth = System.Drawing.Bitmap.GetPixelFormatSize(source.PixelFormat);

                    // Check if bpp (Bits Per Pixel) is 8, 24, or 32
                    if (Depth != 8 && Depth != 24 && Depth != 32)
                    {
                        throw new ArgumentException("Only 8, 24 and 32 bpp images are supported.");
                    }

                    // Lock bitmap and return bitmap data
                    bitmapData = source.LockBits(rect, ImageLockMode.ReadWrite,
                                                 source.PixelFormat);

                    // create byte array to copy pixel values
                    int step = Depth / 8;
                    Pixels = new byte[PixelCount * step];
                    Iptr = bitmapData.Scan0;

                    // Copy data from pointer to array
                    Marshal.Copy(Iptr, Pixels, 0, Pixels.Length);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// Unlock bitmap data
            /// </summary>
            public void UnlockBits()
            {
                try
                {
                    // Copy data from byte array to pointer
                    Marshal.Copy(Pixels, 0, Iptr, Pixels.Length);

                    // Unlock bitmap data
                    source.UnlockBits(bitmapData);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// Get the color of the specified pixel
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public Color GetPixel(int x, int y)
            {
                Color clr = Color.Empty;

                // Get color components count
                int cCount = Depth / 8;

                // Get start index of the specified pixel
                int i = ((y * Width) + x) * cCount;

                if (i > Pixels.Length - cCount)
                    throw new IndexOutOfRangeException();

                if (Depth == 32) // For 32 bpp get Red, Green, Blue and Alpha
                {
                    byte b = Pixels[i];
                    byte g = Pixels[i + 1];
                    byte r = Pixels[i + 2];
                    byte a = Pixels[i + 3]; // a
                    clr = Color.FromArgb(a, r, g, b);
                }
                if (Depth == 24) // For 24 bpp get Red, Green and Blue
                {
                    byte b = Pixels[i];
                    byte g = Pixels[i + 1];
                    byte r = Pixels[i + 2];
                    clr = Color.FromArgb(r, g, b);
                }
                if (Depth == 8)
                // For 8 bpp get color value (Red, Green and Blue values are the same)
                {
                    byte c = Pixels[i];
                    clr = Color.FromArgb(c, c, c);
                }
                return clr;
            }

            /// <summary>
            /// Set the color of the specified pixel
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="color"></param>
            public void SetPixel(int x, int y, Color color)
            {
                // Get color components count
                int cCount = Depth / 8;

                // Get start index of the specified pixel
                int i = ((y * Width) + x) * cCount;

                if (Depth == 32) // For 32 bpp set Red, Green, Blue and Alpha
                {
                    Pixels[i] = color.B;
                    Pixels[i + 1] = color.G;
                    Pixels[i + 2] = color.R;
                    Pixels[i + 3] = color.A;
                }
                if (Depth == 24) // For 24 bpp set Red, Green and Blue
                {
                    Pixels[i] = color.B;
                    Pixels[i + 1] = color.G;
                    Pixels[i + 2] = color.R;
                }
                if (Depth == 8)
                // For 8 bpp set color value (Red, Green and Blue values are the same)
                {
                    Pixels[i] = color.B;
                }
            }
        }
        public int contains(Color[][] c, Color[] a)
        {
            for (int i = 0; i < c.Length; i++)
            {
                int equals = 0;
                for (int j = 0; j < a.Length; j++)
                {
                    if (c[i][j] == a[j])
                    {
                        equals += 1;
                    }
                }
                if (equals == c[i].Length)
                {
                    return i;
                }
            }
            return -1;
        }

        /*bool ContainsTransparent(Bitmap image)
        {
            for (int y = 0; y < image.Height; ++y)
            {
                for (int x = 0; x < image.Width; ++x)
                {
                    if (image.GetPixel(x, y).A < 128)
                    {
                        return true;
                    }
                }
            }
            return false;
        }*/
        private void transparentToTheEnd(Color[] pal)
        {
            bool transpFound = false;
            for (int i = 0; i < pal.Length; i++)
            {
                if (pal[i] == Color.Transparent)
                {
                    pal[i] = pal[pal.Length - 1];
                    transpFound = true;
                }
            }

            if (transpFound)
                pal[pal.Length - 1] = Color.Transparent;
        }


        public int calcPaletteDiffs(int pal)
        {
            int mostSimilar = -1;
            float bestDiff = int.MaxValue;
            for (int i = 0; i < palettes.Count; i++)
            {
                if (paletteCounts[i] != 0)
                    paletteDiffs[pal, i] = paletteDiffs[i, pal] =
                        palDif(palettes[pal], palettes[i]);
                if (paletteDiffs[pal, i] < bestDiff)
                {
                    bestDiff = paletteDiffs[pal, i];
                    mostSimilar = i;
                }
            }
            Console.Out.WriteLine(bestDiff);
            return -1;
        }

        public int countUsedPalettes()
        {
            int res = 0;
            for (int i = 0; i < paletteCounts.Count; i++)
                if (paletteCounts[i] != 0)
                    res++;

            return res;
        }

        public float palDif(Color[] a, Color[] b)
        {
            return palDifUni(a, b) + palDifUni(b, a);
        }

        public float palDifUni(Color[] a, Color[] b)
        {
            bool aTransp = a[3] == Color.Transparent;
            bool bTransp = b[3] == Color.Transparent;

            if (aTransp != bTransp) return float.PositiveInfinity;

            float dif = 0;
            int len = aTransp ? 3 : 4;

            bool[] sel = new bool[len];

            for (int i = 0; i < len; i++)
            {
                Color c = a[i];
                float diff = float.PositiveInfinity;
                int i2 = -1;
                for (int j = 0; j < len; j++)
                {
                    if (sel[j]) continue;
                    float diff2 = ImageIndexer.colorDifference(c, b[j]);
                    if (diff2 < diff || i2 == -1)
                    {
                        i2 = j;
                        diff = diff2;
                    }
                }
                sel[i2] = true;
                dif += diff;
            }

            return dif;
        }

        public Color[] palMerge(Color[] a, Color[] b)
        {
            //return a; //FIXME!!!!


            //Very ugly hack here. I put the 8 colors in a bitmap
            //and let ImageIndexer find me a good 4-color palette :P
            bool trans = false;
            Bitmap bi = new Bitmap(8, 1);
            LockBitmap iii = new LockBitmap(bi);
            iii.LockBits();
            for (int i = 0; i < 4; i++)
            {
                iii.SetPixel(i, 0, a[i]);
                iii.SetPixel(i + 4, 0, b[i]);
                if (b[i] == Color.Transparent || a[i] == Color.Transparent)
                {
                    trans = true;
                }
            }
            iii.UnlockBits();
            List<Color> pal1 = new List<Color>();

            //if (!color2)
            //{
                pal1.AddRange(ImageIndexer.createPaletteForImage(bi, 4, trans));//(trans ? 3 : 4), false));
            //}
            ///else
            //{
            //    pal1.AddRange(ImageIndexer.createPaletteForImage(bi, 2, trans));//(trans ? 3 : 4), false));
             //   pal1.AddRange(new Color[2]);
            //}
            //if (trans) { pal1.Add(Color.Transparent); }
            Color[] pal = pal1.ToArray();
            transparentToTheEnd(pal);
            return pal;

            //Haha, it was too slow :)

            /*Color[] pal = new Color[4];

            int one = 0;
            int two = 0;
            for (int i = 0; i < a.Length; i++)
            {

                int tdiff = (b[i].R - a[i].R) + (b[i].G - a[i].G) + (b[i].B - a[i].B);
                if (tdiff == 0)
                    one++;
                else if (tdiff < 0)
                    two++;
                else
                {
                    
                    two++;
                }
                    
            }
            return one >= two ? a : b;*/
        }
        bool color2 = false;
        public int getClosestColor(Color c, Color[] pal)
        {
            int bestInd = 0;
            float bestDif = ImageIndexer.colorDifferenceWithoutAlpha(pal[0], c);

            for (int i = 0; i < pal.Length; i++)
            {
                float d = ImageIndexer.colorDifferenceWithoutAlpha(pal[i], c);
                if (d < bestDif)
                {
                    bestDif = d;
                    bestInd = i;
                }
            }

            return bestInd;
        }
        public int getClosestColorWithAlpha(Color c, Color[] pal)
        {
            int bestInd = 0;
            float bestDif = ImageIndexer.colorDifference(pal[0], c);

            for (int i = 0; i < pal.Length; i++)
            {
                float d = ImageIndexer.colorDifference(pal[i], c);
                if (d < bestDif)
                {
                    bestDif = d;
                    bestInd = i;
                }
            }

            return bestInd;
        }
    }
}
