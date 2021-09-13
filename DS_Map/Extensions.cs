using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSPRE {
    public static class Extensions {
        public static int IndexOfNumber(this string str) {
            return str.IndexOfAny("0123456789".ToCharArray());
        }
        public static bool ContainsNumber(this string str) {
            return str.IndexOfNumber() > 0;
        }
        public static T[] SubArray<T>(this T[] array, int offset, int length) {
            T[] result = new T[length];
            Array.Copy(array, offset, result, 0, length);
            return result;
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
            Console.WriteLine("Fadeout done");
        }

        public static byte[] ToByteArrayChooseSize(this int num, byte size) {
            switch (size) {
                case 1:
                    return new byte[] { checked((byte)num) };
                case 2:
                    return BitConverter.GetBytes(checked((ushort)num));
                case 4:
                    return BitConverter.GetBytes(num);
            }
            throw new InvalidOperationException();
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
    }
}
