using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace NSMBe4
{
    public abstract class PixelPalettedImage : PalettedImage
    {
        public override void replaceImgAndPal(Bitmap b, Palette p)
        {
            p.pal = ImageIndexer.createPaletteForImage(b, p.pal.Length);
            replaceWithPal(b, p);
        }

        public override void replaceWithPal(Bitmap b, Palette p)
        {
            for (int x = 0; x < getWidth(); x++)
                for (int y = 0; y < getHeight(); y++)
                {
                    Color c = b.GetPixel(x, y);
                    int i = p.getClosestColor(c);
                    setPixel(x, y, i);
                }

            save();
        }

        public override Bitmap render(Palette p)
        {
            if (color0 == true)
            {
                p.pal[0] = Color.Transparent;
            }
            int w = getWidth();
            int h = getHeight();

            Bitmap b = new Bitmap(w, h);
            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                    b.SetPixel(x, y, p.getColorSafe(getPixel(x, y)));

            return b;
        }

        public void setPixelData(byte[,] a, int xx, int yy)
        {
            int tx = getWidth();
            int ty = getHeight();

            for (int x = 0; x < tx; x++)
                for (int y = 0; y < ty; y++)
                {
                    setPixel(x, y, a[x + xx, y + yy]);
                }
        }

        public void setPixelData(int[,] a, int xx, int yy)
        {
            int tx = getWidth();
            int ty = getHeight();

            for (int x = 0; x < tx; x++)
                for (int y = 0; y < ty; y++)
                {
                    setPixel(x, y, a[x + xx, y + yy]);
                }
        }

        public abstract void setPixel(int x, int y, int c);
        public abstract int getPixel(int x, int y);
    }
}
