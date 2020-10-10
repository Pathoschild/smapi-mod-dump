/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/Stardew_Valley_Showcase_Mod
**
*************************************************/

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