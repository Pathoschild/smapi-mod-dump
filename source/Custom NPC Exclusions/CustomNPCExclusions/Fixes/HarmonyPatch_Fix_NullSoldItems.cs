/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/CustomNPCExclusions
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;

namespace CustomNPCExclusions
{
    /// <summary>A Harmony patch that attempts to fix possible null errors caused by certain bugs and/or mods.</summary>
    /// <remarks>
    /// Fix designed by atravita for SDV v1.5.6.
    /// The patched method will become null-safe in SDV v1.6, but this fix may still solve similar problems in other code that checks the items.
    /// </remarks>
    public static class HarmonyPatch_Fix_NullSoldItems
    {
        public static void ApplyPatch(Harmony harmony)
        {
            ModEntry.Instance.Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_Fix_NullSoldItems)}\": prefixing SDV method \"Game1.UpdateShopPlayerItemInventory(string, HashSet<NPC>)\".", LogLevel.Trace);
            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.UpdateShopPlayerItemInventory), new[] { typeof(string), typeof(HashSet<NPC>) }),
                prefix: new HarmonyMethod(typeof(HarmonyPatch_Fix_NullSoldItems), nameof(Game1_UpdateShopPlayerItemInventory_Prefix))
            );
        }

        /// <summary>Attempts to remove null and "empty" items from the shop's "player items being resold" list, in order to prevent null reference errors.</summary>
        /// <param name="location_name">The name of the shop location being checked.</param>
        private static void Game1_UpdateShopPlayerItemInventory_Prefix(string location_name)
        {
            try
            {
                if (Game1.getLocationFromName(location_name) is not ShopLocation shop || shop.itemsFromPlayerToSell.Count == 0) //if the named shop doesn't exist OR the shop isn't selling any of the players' items
                    return; //do nothing

                int badItems = 0;
                for (int x = shop.itemsFromPlayerToSell.Count - 1; x >= 0; x--)
                {
                    Item item = shop.itemsFromPlayerToSell[x];

                    if (item is null || item.Stack <= 0) //if the item is null or would otherwise call null reference errors
                    {
                        shop.itemsFromPlayerToSell.RemoveAt(x); //remove it from the list
                        badItems++;
                    }
                }

                if (badItems > 0)
                    ModEntry.Instance.Monitor.Log($"Removed {badItems} null/hazardous items from {location_name}.itemsFromPlayerToSell", LogLevel.Trace);
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_Fix_NullSoldItems)}\" has encountered an error. Full error message: \n{ex.ToString()}", LogLevel.Error);
                return; //run the original method
            }
        }
    }
}