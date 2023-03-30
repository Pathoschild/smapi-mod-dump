/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/inkle/ink
**
*************************************************/

using System.Collections.Generic;

namespace Ink
{
    public static class InkStringConversionExtensions
    {
        public static string[] ToStringsArray<T>(this List<T> list) {
            int count = list.Count;
            var strings = new string[count];

            for(int i = 0; i < count; i++) {
                strings[i] = list[i].ToString();
            }

            return strings;
        }
    }
}
