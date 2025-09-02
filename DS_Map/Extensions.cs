using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;
using Tao.Platform.Windows;

namespace DSPRE {
    public static class Extensions {
        public static void SetAllItemsChecked(this CheckedListBox clb, bool status) {
            for (int i = 0; i < clb.Items.Count; i++) {
                clb.SetItemChecked(i, status);
            }
        }
        public static int IndexOfFirstNumber(this string str) {
            return str.IndexOfAny("0123456789".ToCharArray());
        }
        public static bool ContainsNumber(this string str) {
            return str.IndexOfFirstNumber() > 0;
        }
        public static T[] SubArray<T>(this T[] array, int offset, int length) {
            T[] result = new T[length];
            Array.Copy(array, offset, result, 0, length);
            return result;
        }
        public static void Move<T>(this IList<T> l, int currentIndex, int newIndex) {
            T item = l[currentIndex];
            l.RemoveAt(currentIndex);
            l.Insert(newIndex, item);
        }
        public static Dictionary<string, ushort> Reverse (this Dictionary<ushort, string> source) {
            var dictionary = new Dictionary<string, ushort>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var entry in source) {
                string newKey = entry.Value;
                if (!dictionary.ContainsKey(newKey)) {
                    dictionary.Add(newKey, entry.Key);
                }
            }
            return dictionary;
        }
        public static void FadeIn(this Form o, int framelength = 16, int frames = 10) {
            //Object is not fully invisible. Fade it in
            while (o != null && !o.IsDisposed && o.Opacity < 1.0) {
                Thread.Sleep(framelength);
                o.Opacity += (1.0 / frames);
            }
            o.Opacity = 1; //make fully visible
        }

        public static void FadeOut(this Form o, int framelength = 16, int frames = 10) {
            //Object is fully visible. Fade it out
            while (o != null && o.Opacity > 0.0) {
                Thread.Sleep(framelength);
                o.Opacity -= (1.0 / frames);
            }
            o.Opacity = 0; //make fully invisible
            AppLogger.Debug("Fadeout done");
        }

        public static byte[] ToByteArrayChooseSize(this int num, byte size) {
            switch (size) {
                case 1:
                    return new byte[] { checked((byte)num) };
                case 2:
                    return BitConverter.GetBytes(checked((ushort)num));
                case 4:
                    return BitConverter.GetBytes(num);
                default:
                    MessageBox.Show("Invalid size for number conversion!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw new InvalidOperationException();
            }
        }
        public static string PurgeSpecial(this string str, char[] special) {
            foreach (char c in special) {
                int pos = str.IndexOf(c);
                if (pos >= 0) {
                    return str.Substring(pos + 1);
                }
            }
            return str;
        }
        public static NumberStyles GetNumberStyle(this string s) {
            int posOfPrefix = s.IndexOf("0x", StringComparison.InvariantCultureIgnoreCase);
            if (posOfPrefix >= 0) {
                foreach (char c in s.Substring(posOfPrefix + 2)) {
                    if (!char.IsDigit(c) && char.ToUpper(c) > 'F') {
                        return NumberStyles.None;
                    }
                }
                return NumberStyles.HexNumber;
            } else {
                foreach (char c in s) {
                    if (!char.IsDigit(c)) {
                        return NumberStyles.None;
                    }
                }
                return NumberStyles.Integer;
            }
        }
        public static bool IgnoreCaseEquals(this string str, string other) {
            return str.Equals(other, StringComparison.InvariantCultureIgnoreCase);
        }
        public static List<string> ToStringsList (this ScintillaNET.LineCollection lc, bool allowEmpty = true, bool trim = false) {
            IEnumerable<string> temp = lc.Select(x => x.Text);
            
            if (trim) {
                temp = temp.Select(x => x.Trim());
            }
            
            if (!allowEmpty) {
                temp = temp.Where(x => !string.IsNullOrEmpty(x));
            }
            
            return temp.ToList();
        }

        //public static Dictionary<TValue, TKey> Reverse<TKey, TValue>(this IDictionary<TKey, TValue> source) {
        //    var dictionary = new Dictionary<TValue, TKey>();
        //    foreach (var entry in source) {
        //        if (!dictionary.ContainsKey(entry.Value)) {
        //            dictionary.Add(entry.Value, entry.Key);
        //        }
        //    }
        //    return dictionary;
        //}

        public static Bitmap Resize(this Bitmap source, int width, int height) {
            if (source.Width == width && source.Height == height) {
                return source;
            }

            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result)) {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.Half;
                g.DrawImage(source, 0, 0, width, height);
            }
            return result;
        }
        public static Bitmap Resize(this Bitmap source, float factor) => source.Resize((int)(source.Width * factor), (int)(source.Height * factor));
    }

    public class ListBox2 : ListBox {
        public new void RefreshItem(int index) {
            base.RefreshItem(index);
        }
    }

    public class SimpleOpenGlControl2 : SimpleOpenGlControl {
        private bool designMode;

        public SimpleOpenGlControl2() : base() {
            designMode = LicenseManager.UsageMode == LicenseUsageMode.Designtime;
        }

        public new bool DesignMode { get { return designMode; } }

        protected override void OnPaint(PaintEventArgs e) {
            //if the control is allowed to paint in design mode, a message box prevents working with it
            //"No device or rendering context available!"
            if (DesignMode) {
                e.Graphics.Clear(this.BackColor);
                if (this.BackgroundImage != null)
                    e.Graphics.DrawImage(this.BackgroundImage, this.ClientRectangle, 0, 0, this.BackgroundImage.Width, this.BackgroundImage.Height, GraphicsUnit.Pixel);
                e.Graphics.Flush();
                return;
            };
            base.OnPaint(e);
        }
    }
}
