/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/ExpandedStorage
**
*************************************************/

using System.Collections.Generic;
using Harmony;
using ImJustMatt.Common.PatternPatches;
using ImJustMatt.ExpandedStorage.Framework.Extensions;
using StardewModdingAPI;
using StardewValley;

// ReSharper disable InconsistentNaming

namespace ImJustMatt.ExpandedStorage.Framework.Patches
{
    internal class FarmerPatch : Patch<ModConfig>
    {
        internal FarmerPatch(IMonitor monitor, ModConfig config) : base(monitor, config)
        {
        }

        protected internal override void Apply(HarmonyInstance harmony)
        {
            harmony.Patch(
                AccessTools.Method(typeof(Farmer), nameof(Farmer.addItemToInventory), new[] {typeof(Item), typeof(List<Item>)}),
                new HarmonyMethod(GetType(), nameof(AddItemToInventoryPrefix))
            );
        }

        public static bool AddItemToInventoryPrefix(Farmer __instance, ref Item __result, Item item, List<Item> affected_items_list)
        {
            var storage = ExpandedStorage.GetStorage(item);
            if (storage == null || item.Stack > 1)
                return true;

            var chest = item.ToChest(storage);

            // Find first stackable slot
            for (var j = 0; j < __instance.MaxItems; j++)
            {
                if (j >= __instance.Items.Count
                    || __instance.Items[j] == null
                    || !__instance.Items[j].Name.Equals(item.Name)
                    || __instance.Items[j].ParentSheetIndex != item.ParentSheetIndex
                    || !chest.canStackWith(__instance.Items[j]))
                    continue;

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
                if (i > __instance.Items.Count || __instance.Items[i] != null)
                    continue;

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