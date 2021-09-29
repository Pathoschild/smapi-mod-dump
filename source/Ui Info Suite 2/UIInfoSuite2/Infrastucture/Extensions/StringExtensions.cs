/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Annosz/UiInfoSuite2
**
*************************************************/

namespace UIInfoSuite.Infrastructure.Extensions
{
    static class StringExtensions
    {
        public static int SafeParseInt32(this string s)
        {
            int result = 0;

            if (!string.IsNullOrWhiteSpace(s))
            {
                int.TryParse(s, out result);
            }

            return result;
        }

        public static int SafeParseInt64(this string s)
        {
            int result = 0;

            if (!string.IsNullOrWhiteSpace(s))
                int.TryParse(s, out result);

            return result;
        }

        public static bool SafeParseBool(this string s)
        {
            bool result = false;

            if (!string.IsNullOrWhiteSpace(s))
            {
                bool.TryParse(s, out result);
            }

            return result;
        }
    }
}
