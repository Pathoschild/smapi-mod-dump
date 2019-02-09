using System;

namespace TehPers.Core.Helpers.Static {
    public static class MathHelpers {
        public static sbyte Clamp(this sbyte value, sbyte lower, sbyte upper) => Math.Max(Math.Min(value, upper), lower);
        public static byte Clamp(this byte value, byte lower, byte upper) => Math.Max(Math.Min(value, upper), lower);
        public static ushort Clamp(this ushort value, ushort lower, ushort upper) => Math.Max(Math.Min(value, upper), lower);
        public static short Clamp(this short value, short lower, short upper) => Math.Max(Math.Min(value, upper), lower);
        public static uint Clamp(this uint value, uint lower, uint upper) => Math.Max(Math.Min(value, upper), lower);
        public static int Clamp(this int value, int lower, int upper) => Math.Max(Math.Min(value, upper), lower);
        public static ulong Clamp(this ulong value, ulong lower, ulong upper) => Math.Max(Math.Min(value, upper), lower);
        public static long Clamp(this long value, long lower, long upper) => Math.Max(Math.Min(value, upper), lower);
        public static float Clamp(this float value, float lower, float upper) => Math.Max(Math.Min(value, upper), lower);
        public static double Clamp(this double value, double lower, double upper) => Math.Max(Math.Min(value, upper), lower);
        public static decimal Clamp(this decimal value, decimal lower, decimal upper) => Math.Max(Math.Min(value, upper), lower);
    }
}
