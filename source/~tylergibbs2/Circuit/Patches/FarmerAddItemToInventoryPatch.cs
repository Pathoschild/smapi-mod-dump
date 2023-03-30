/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using HarmonyLib;
using StardewValley;

namespace Circuit.Patches
{
    [HarmonyPatch(typeof(Farmer), nameof(Farmer.addItemToInventory), new Type[] { typeof(Item), typeof(List<Item>) })]
    internal class FarmerAddItemToInventoryPatch
    {
        public static bool Prefix(Item item)
        {
            if (!ModEntry.ShouldPatch() || item is null)
                return true;

            ModEntry.Instance.TaskManager?.OnItemObtained(item);
            ModEntry.Instance.EventManager?.OnItemObtained(item);

            return true;
        }
    }
}
