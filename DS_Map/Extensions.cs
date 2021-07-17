namespace DSPRE {
    public static class Extensions {
        public static int IndexOfNumber(this string str) {
            return str.IndexOfAny("0123456789".ToCharArray());
        }
        public static bool ContainsNumber(this string str) {
            return str.IndexOfNumber() > 0;
        }
    }
}
