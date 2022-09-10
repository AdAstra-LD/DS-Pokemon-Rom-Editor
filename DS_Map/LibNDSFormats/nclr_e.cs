using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace MKDS_Course_Editor {
    public partial class nclr_e : Form {
        int r = 255;
        int g = 255;
        int b = 255;
        List<Color> colors = new List<Color>();
        bool bpp8_ = false;
        uint unknown1;
        public nclr_e(Color[] c, bool bpp8, uint unknown) {
            colors.AddRange(c);
            bpp8_ = bpp8;
            unknown1 = unknown;
        }
        public nclr_e() {

        }
        public int decodeColor(int value, CColorFormat format) {
            int[] res = format.getResolution();
            int rgb = Convert.ToInt32(0xff000000);
            int shift = 0;
            int length = res.Length / 2;
            for (int i = 0; i < length; i++) {
                int mode = res[length - i - 1];
                int nshift = res[length * 2 - i - 1];
                int mult = shiftList[nshift][1];
                int and = shiftList[nshift][0];
                int n = (value >> shift & and) * mult;
                switch (mode) {
                    case 0: // '\0'
                        rgb |= n << 24;
                        break;

                    case 1: // '\001'
                        rgb |= n << 16;
                        break;

                    case 2: // '\002'
                        rgb |= n << 8;
                        break;

                    case 3: // '\003'
                        rgb |= n;
                        break;

                    case 4: // '\004'
                        rgb = n << 16 | n << 8 | n;
                        break;
                }
                shift += nshift;
            }

            return rgb;
        }
        public CColorFormat BGR555 = new CColorFormat("BGR555", 10, 16, new int[] {
        3, 2, 1, 5, 5, 5
    });

        public static int encodeColor(int value, CColorFormat format) {
            int[] res = format.getResolution();
            int rgb = 0;
            int shift = 0;
            int length = res.Length / 2;
            for (int i = 0; i < length; i++) {
                int mode = res[length - i - 1];
                int nshift = res[length * 2 - i - 1];
                int mult = shiftList[nshift][1];
                int and = shiftList[nshift][0];
                int n = 0;
                switch (mode) {
                    case 0: // '\0'
                        n = value >> 24 & 0xff;
                        break;

                    case 1: // '\001'
                        n = value >> 16 & 0xff;
                        break;

                    case 2: // '\002'
                        n = value >> 8 & 0xff;
                        break;

                    case 3: // '\003'
                        n = value & 0xff;
                        break;

                    case 4: // '\004'
                        n = value & 0xff;
                        break;
                }
                rgb |= (n / mult & and) << shift;
                shift += nshift;
            }

            return rgb;
        }

        public static int[][] shiftList = new int[][] {
            new int[] { 0, 0 },
            new int[] { 1, 255 },
            new int[] { 3, 85 },
            new int[] { 7, 36 },
            new int[] { 15, 17 },
            new int[] { 31, 8 },
            new int[] { 63, 4 },
            new int[] { 127, 2 },
            new int[] { 255, 1 }
        };

        public class CColorFormat {

            public CColorFormat(String name, int id, int depth, int[] res) {
                this.name = name;
                this.id = id;
                this.depth = depth;
                this.res = res;
            }

            public override string ToString() {
                return GetName();
            }

            public string GetName() {
                return name;
            }

            public int getId() {
                return id;
            }

            public int getDepth() {
                return depth;
            }

            public int[] getResolution() {
                return res;
            }

            public int getWidth(int field) {
                for (int i = 0; i < res.Length / 2; i++)
                    if (res[i] == field)
                        return res[i + res.Length / 2];

                return 0;
            }

            private String name;
            private int id;
            private int depth;
            private int[] res;
        }

    }
}
