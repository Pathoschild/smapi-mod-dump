/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/MegaStorageMod
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewValley;
using StardewValley.Objects;
using SDVObject = StardewValley.Object;

namespace MegaStorage
{
    internal static class ItemExtensions
    {
        private static IDictionary<int, string> _storageTypes;
        internal static void Init(IDictionary<int, string> storageTypes)
        {
            _storageTypes = storageTypes;
        }
        internal static bool ShouldBeExpandedStorage(this Item item) =>
            item is SDVObject obj &&
            (bool)obj.bigCraftable &&
            _storageTypes.ContainsKey(item.ParentSheetIndex);
        internal static bool IsExpandedStorage(this Item item) =>
            item is Chest chest &&
            chest.modData.ContainsKey("ImJustMatt.ExpandedStorage/actual-capacity");
        internal static Chest ToExpandedStorage(this Item item)
        {
            if (!(item is SDVObject obj))
                throw new InvalidOperationException($"Cannot convert {item.Name} to Chest");
            if (!(obj is Chest chest))
            {
                chest = new Chest(true, obj.TileLocation, obj.ParentSheetIndex)
                {
                    name = obj.name
                };
            }
            
            // Copy modded capacity into Chest
            if (!chest.modData.ContainsKey("ImJustMatt.ExpandedStorage/actual-capacity"))
            {
                chest.modData["ImJustMatt.ExpandedStorage/actual-capacity"] =
                    obj.modData.TryGetValue("ImJustMatt.ExpandedStorage/actual-capacity", out var actualCapacity)
                        ? actualCapacity
                        : (_storageTypes[obj.ParentSheetIndex] == "Large Chest" ? 72 : int.MaxValue).ToString();
            }

            return chest;
        }
    }
}