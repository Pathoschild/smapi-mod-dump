/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/silentoak/StardewMods
**
*************************************************/

using System;
using System.Text.RegularExpressions;

namespace SilentOak.QualityProducts.Extensions
{
    public static class StringExtensions
    {
        public static string SplitCamelCase(this string str, string join = " ")
        {
            /// From https://stackoverflow.com/questions/4488969/split-a-string-by-capital-letters
            string[] words = Regex.Split(str, @"(?<=[A-Z])(?=[A-Z][a-z])|(?<=[^A-Z])(?=[A-Z])|(?<=[A-Za-z])(?=[^A-Za-z])");
            return string.Join(join, words);
        }
    }
}
