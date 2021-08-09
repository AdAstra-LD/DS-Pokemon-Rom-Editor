using System.Collections.Generic;

namespace DSPRE {
    public static class Extensions {
        public static int IndexOfNumber(this string str) {
            return str.IndexOfAny("0123456789".ToCharArray());
        }
        public static bool ContainsNumber(this string str) {
            return str.IndexOfNumber() > 0;
        }
        public static Dictionary<string, ushort> Reverse (this Dictionary<ushort, string> source) {
            var dictionary = new Dictionary<string, ushort>();
            foreach (var entry in source) {
                string newKey = entry.Value.ToLower();
                if (!dictionary.ContainsKey(newKey)) {
                    dictionary.Add(newKey, entry.Key);
                }
            }
            return dictionary;
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
