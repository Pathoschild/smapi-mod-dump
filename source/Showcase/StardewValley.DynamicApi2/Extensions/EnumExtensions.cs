using System;

namespace Igorious.StardewValley.DynamicApi2.Extensions
{
    public static class EnumExtensions
    {
        public static TEnum ToEnum<TEnum>(this string s) where TEnum : struct
        {
            return (TEnum)Enum.Parse(typeof(TEnum), s, true);
        }

        public static TEnum? TryToEnum<TEnum>(this string s) where TEnum : struct 
        {
            return Enum.TryParse(s, true, out TEnum value)? value : (TEnum?)null;
        }

        public static string ToLower(this Enum e)
        {
            return e.ToString().ToLower();
        }
    }
}