/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace Common.Extensions
{
    using System;
    using System.Linq;
    using StardewValley;
    using StardewValley.Objects;

    /// <summary>Extension methods for the <see cref="StardewValley.Item">StardewValley.Item</see> class.</summary>
    internal static class ItemExtensions
    {
        /// <summary>Recursively iterates chests held within chests.</summary>
        /// <param name="item">The originating item to search.</param>
        /// <param name="action">The action to perform on items within chests.</param>
        public static void RecursiveIterate(this Item item, Action<Item> action)
        {
            if (item is Chest {SpecialChestType: Chest.SpecialChestTypes.None} chest)
            {
                foreach (var chestItem in chest.items.Where(chestItem => chestItem is not null))
                {
                    chestItem.RecursiveIterate(action);
                }
            }

            action(item);
        }
    }
}