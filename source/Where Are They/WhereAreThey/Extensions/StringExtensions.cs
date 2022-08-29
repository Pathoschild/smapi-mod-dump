/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/Capaldi12/wherearethey
**
*************************************************/

using System.Text.RegularExpressions;

namespace WhereAreThey.Extensions
{
    public static class StringExtensions
    {
        // To split on words borders (See https://stackoverflow.com/a/5796793)
        private static readonly Regex UUL = new(@"(\P{Ll})(\P{Ll}\p{Ll})", RegexOptions.Compiled);
        private static readonly Regex LU = new(@"(\p{Ll})(\P{Ll})", RegexOptions.Compiled);

        // To reduce multiple space characters to one (preserving exact character)
        private static readonly Regex S = new(@"(\s)\s+", RegexOptions.Compiled);

        // Converts ingame location name to proper format
        public static string HumanizeName(this string str)
        {
            return S.Replace(LU.Replace(UUL.Replace(str, "$1 $2"), "$1 $2"), "$1").Trim();
        }
    }
}
