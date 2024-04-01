/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hedgehog-Technologies/StardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using AutoForager.Classes;

namespace AutoForager.Helpers
{
    public static class ListExtensions
    {
        public static void AddDistinct(this List<ForageableItem> items, ForageableItem newItem)
        {
            if (items.Any(i => i.QualifiedItemId.Equals(newItem.QualifiedItemId))) return;

            items.Add(newItem);
        }

        public static void SortByDisplayName(this List<ForageableItem> items)
        {
            items.Sort((x, y) => string.CompareOrdinal(x.DisplayName, y.DisplayName));
        }

        public static bool TryGetItem(this List<ForageableItem> items, string qualifiedItemId, out ForageableItem? item)
        {
            item = null;

            if (items is null || qualifiedItemId is null) return false;

            foreach (var fItem in items)
            {
                if (fItem.QualifiedItemId.IEquals(qualifiedItemId))
                {
                    item = fItem;
                    return true;
                }
            }

            return false;
        }

        public static bool IsNullOrEmpty<T>(this List<T>? list)
        {
            if (list is null) return true;

            return !list.Any();
        }
    }
}
