/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TSlex/StardewValley
**
*************************************************/

using System;
using StardewValley;

namespace ItemResearchSpawner.Utils
{
    public static class Helpers
    {
        public static bool EqualsCaseInsensitive(string a, string b)
        {
            return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }

        public static string GetItemUniqueKey(Item item)
        {
            return $"{item.Name}:" + $"{item.ParentSheetIndex}";
        }
    }
}