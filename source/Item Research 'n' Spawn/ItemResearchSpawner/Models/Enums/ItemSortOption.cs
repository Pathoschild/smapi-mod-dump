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

namespace ItemResearchSpawner.Models
{
    public enum ItemSortOption
    {
        Name,
        Category,
        ID
    }

    internal static class ItemSortOptionExtensions
    {
        public static ItemSortOption GetNext(this ItemSortOption current)
        {
            return current switch
            {
                ItemSortOption.Name => ItemSortOption.Category,
                ItemSortOption.Category => ItemSortOption.ID,
                ItemSortOption.ID => ItemSortOption.Name,
                _ => throw new NotSupportedException($"Unknown sort '{current}'.")
            };
        }
        
        public static ItemSortOption GetPrevious(this ItemSortOption current)
        {
            return current switch
            {
                ItemSortOption.Name => ItemSortOption.ID,
                ItemSortOption.Category => ItemSortOption.Name,
                ItemSortOption.ID => ItemSortOption.Category,
                _ => throw new NotSupportedException($"Unknown sort '{current}'.")
            };
        }
    }
}