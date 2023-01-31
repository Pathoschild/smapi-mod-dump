/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cdaragorn/Ui-Info-Suite
**
*************************************************/

namespace UIInfoSuite.Extensions
{
    static class StringExtensions
    {


        public static int SafeParseInt32(this string s)
        {
            var result = 0;

            if (!string.IsNullOrWhiteSpace(s))
            {
                int.TryParse(s, out result);
            }

            return result;
        }

        public static long SafeParseInt64(this string s)
        {
            long result = 0;

            if (!string.IsNullOrWhiteSpace(s))
                long.TryParse(s, out result);

            return result;
        }

        public static bool SafeParseBool(this string s)
        {
            var result = false;

            if (!string.IsNullOrWhiteSpace(s))
            {
                bool.TryParse(s, out result);
            }

            return result;
        }
    }
}
