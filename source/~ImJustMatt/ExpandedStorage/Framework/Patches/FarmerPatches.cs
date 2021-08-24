/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

using System.Collections.Generic;
using ExpandedStorage.Framework.Controllers;
using HarmonyLib;
using XSAutomate.Common.Patches;
using ExpandedStorage.Framework.Extensions;
using StardewModdingAPI;
using StardewValley;

// ReSharper disable InconsistentNaming

namespace ExpandedStorage.Framework.Patches
{
    internal class FarmerPatches : BasePatch<ExpandedStorage>
    {
        public FarmerPatches(IMod mod, Harmony harmony) : base(mod, harmony)
        {
            harmony.Patch(
                AccessTools.Method(typeof(Farmer), nameof(Farmer.addItemToInventory), new[] {typeof(Item), typeof(List<Item>)}),
                new HarmonyMethod(GetType(), nameof(AddItemToInventoryPrefix))
            );
        }

        /// <summary>Converted added items into Chests</summary>
        private static bool AddItemToInventoryPrefix(Farmer __instance, ref Item __result, Item item, List<Item> affected_items_list)
        {
            if (item.Stack > 1
                || !Mod.AssetController.TryGetStorage(item, out var storage)
                || storage.Config.Option("CanCarry", true) != StorageConfigController.Choice.Enable
                && storage.Config.Option("AccessCarried", true) != StorageConfigController.Choice.Enable) return true;

            // Find first stackable slot
            var chest = item.ToChest(storage);
            for (var j = 0; j < __instance.MaxItems; j++)
            {
                if (j >= __instance.Items.Count
                    || __instance.Items[j] == null
                    || !__instance.Items[j].Name.Equals(item.Name)
                    || __instance.Items[j].ParentSheetIndex != item.ParentSheetIndex
                    || !chest.canStackWith(__instance.Items[j])) continue;
                var stackLeft = __instance.Items[j].addToStack(chest);
                affected_items_list?.Add(__instance.Items[j]);
                if (stackLeft <= 0)
                {
                    __result = null;
                    return false;
                }

                chest.Stack = stackLeft;
            }

            // Find first empty slot
            for (var i = 0; i < __instance.MaxItems; i++)
            {
                if (i > __instance.Items.Count || __instance.Items[i] != null) continue;
                __instance.Items[i] = chest;
                affected_items_list?.Add(__instance.Items[i]);
                __result = null;
                return false;
            }

            __result = chest;
            return false;
        }
    }
}