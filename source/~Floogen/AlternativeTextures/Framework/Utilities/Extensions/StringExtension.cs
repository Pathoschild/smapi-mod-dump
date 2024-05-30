/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/AlternativeTextures
**
*************************************************/

using System;

namespace AlternativeTextures.Framework.Utilities.Extensions
{
    public static class StringExtension
    {
        public static string ReplaceLastInstance(this string source, string target, string replacement)
        {
            int index = source.LastIndexOf(target, StringComparison.OrdinalIgnoreCase);

            if (index == -1)
            {
                return source;
            }

            return source.Remove(index, target.Length).Insert(index, replacement);
        }
    }
}
