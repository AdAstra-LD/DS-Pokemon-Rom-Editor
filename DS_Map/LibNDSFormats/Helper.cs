using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using OpenTK;

namespace SM64DSe {
    static class Helper {
        public static ushort ColorToBGR15(Color color) {
            uint r = (uint)((color.R & 0xF8) >> 3);
            uint g = (uint)((color.G & 0xF8) << 2);
            uint b = (uint)((color.B & 0xF8) << 7);
            return (ushort)(r | g | b);
        }

        public static Color BGR15ToColor(ushort bgr15) {
            byte red = (byte)((bgr15 << 3) & 0xF8);
            byte green = (byte)((bgr15 >> 2) & 0xF8);
            byte blue = (byte)((bgr15 >> 7) & 0xF8);
            return Color.FromArgb(red, green, blue);
        }

        public static ushort BlendColorsBGR15(ushort c1, int w1, ushort c2, int w2) {
            int r1 = c1 & 0x1F;
            int g1 = (c1 >> 5) & 0x1F;
            int b1 = (c1 >> 10) & 0x1F;
            int r2 = c2 & 0x1F;
            int g2 = (c2 >> 5) & 0x1F;
            int b2 = (c2 >> 10) & 0x1F;

            int rf = ((r1 * w1) + (r2 * w2)) / (w1 + w2);
            int gf = ((g1 * w1) + (g2 * w2)) / (w1 + w2);
            int bf = ((b1 * w1) + (b2 * w2)) / (w1 + w2);
            return (ushort)(rf | (gf << 5) | (bf << 10));
        }

        public static bool VectorsEqual(Vector3 a, Vector3 b) {
            float epsilon = 0.00001f;
            if (Math.Abs(a.X - b.X) > epsilon) return false;
            if (Math.Abs(a.Y - b.Y) > epsilon) return false;
            if (Math.Abs(a.Z - b.Z) > epsilon) return false;
            return true;
        }
    }
}
