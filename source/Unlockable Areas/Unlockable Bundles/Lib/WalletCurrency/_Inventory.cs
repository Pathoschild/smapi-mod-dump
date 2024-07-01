/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-areas
**
*************************************************/

using HarmonyLib;
using StardewValley;
using StardewValley.Inventories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unlockable_Bundles.NetLib;
using static Unlockable_Bundles.ModEntry;

namespace Unlockable_Bundles.Lib.WalletCurrency
{
    public class _Inventory
    {
        public static void Initialize()
        {
            var harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(Inventory), nameof(Inventory.ContainsId), new[] { typeof(string) }),
                prefix: new HarmonyMethod(typeof(_Inventory), nameof(_Inventory.ContainsId_Prefix1))
            );

            harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(Inventory), nameof(Inventory.ContainsId), new[] { typeof(string), typeof(int) }),
                prefix: new HarmonyMethod(typeof(_Inventory), nameof(_Inventory.ContainsId_Prefix2))
            );

            harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(Inventory), nameof(Inventory.CountId), new[] { typeof(string) }),
                prefix: new HarmonyMethod(typeof(_Inventory), nameof(_Inventory.CountId_Prefix))
            );

            harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(Inventory), nameof(Inventory.ReduceId), new[] { typeof(string), typeof(int) }),
                prefix: new HarmonyMethod(typeof(_Inventory), nameof(_Inventory.ReduceId_Prefix))
            );
        }

        public static bool ContainsId_Prefix1(string itemId, ref bool __result)
            => ContainsId_Prefix2(itemId, 1, ref __result);

        public static bool ContainsId_Prefix2(string itemId, int minimum, ref bool __result)
        {
            itemId = ItemRegistry.QualifyItemId(itemId);
            if (WalletCurrencyHandler.getCurrencyItemMatch(itemId, out var match, out var currency, out var relevantPlayer)) {
                var value = ModData.getWalletCurrency(currency.Id, relevantPlayer);
                if (__result = (value / match.Value) >= minimum)
                    return false;
            }

            return true;
        }

        public static bool CountId_Prefix(string itemId, ref int __result)
        {
            itemId = ItemRegistry.QualifyItemId(itemId);
            if (WalletCurrencyHandler.getCurrencyItemMatch(itemId, out var match, out var currency, out var relevantPlayer)) {
                var value = ModData.getWalletCurrency(currency.Id, relevantPlayer);
                __result = value / match.Value;
                return false;
            }

            return true;
        }

        public static bool ReduceId_Prefix(string itemId, int count, ref int __result)
        {
            itemId = ItemRegistry.QualifyItemId(itemId);
            if (WalletCurrencyHandler.getCurrencyItemMatch(itemId, out var match, out var currency, out var relevantPlayer)) {
                WalletCurrencyHandler.addWalletCurrency(currency, relevantPlayer, -(count * match.Value), true, true);
                __result = 0;
                return false;
            }

            return true;
        }
    }
}
