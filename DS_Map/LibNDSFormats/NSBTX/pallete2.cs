using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace NSMBe4
{
    public abstract class Palette
    {
        public Color[] pal;

        public abstract void save();
        public abstract byte[] getRawData();
        public abstract void close();

        public int getClosestColor(Color c)
        {
            if (c.A == 0)
                return 0;

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

        public Color getColorSafe(int ind)
        {
            if (ind >= pal.Length)
                return Color.Fuchsia;

            return pal[ind];
        }
    }
}
