/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-challenger
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using StardewValley.Menus;

// ReSharper disable InconsistentNaming

namespace Slothsoft.Challenger.Events;

internal static class GlobalSellChanger {
    
    private static Harmony? _harmony;
    private static readonly IList<int> AllowedCategories = new List<int>();

    public static void AddAllowedCategories(IEnumerable<int> allowedCategories) {
        if (_harmony == null) {
            _harmony = new Harmony(ChallengerMod.Instance.ModManifest.UniqueID + ".GlobalStockChanger");
            _harmony.Patch(
                original: AccessTools.Method(
                    typeof(ShopMenu),
                    nameof(ShopMenu.setUpStoreForContext)
                ),
                postfix: new HarmonyMethod(typeof(GlobalSellChanger), nameof(ChangeShopContext))
            );
            _harmony.Patch(
                original: AccessTools.Method(
                    typeof(SObject),
                    nameof(SObject.canBeShipped)
                ),
                postfix: new HarmonyMethod(typeof(GlobalSellChanger), nameof(ChangeCanBeShipped))
            );
        }
        foreach (var allowedCategory in allowedCategories) {
            AllowedCategories.Add(allowedCategory);
        }
    }

    public static void ChangeShopContext(ShopMenu __instance) {
        // use __instance.storeContext to see which shop we are
        if (AllowedCategories.Count > 0) {
            __instance.categoriesToSellHere = __instance.categoriesToSellHere.Intersect(AllowedCategories).ToList();
        }
    }

    public static void ChangeCanBeShipped(SObject __instance, ref bool __result) {
        if (AllowedCategories.Count > 0) {
            __result &= AllowedCategories.Contains(__instance.Category);
        }
    }

    public static void RemoveAllowedCategories(IEnumerable<int> allowedCategories) {
        foreach (var allowedCategory in allowedCategories) {
            AllowedCategories.Remove(allowedCategory);
        }

        if (AllowedCategories.Count == 0) {
            _harmony?.UnpatchAll(ChallengerMod.Instance.ModManifest.UniqueID + ".GlobalStockChanger");
            _harmony = null;
        }
    }
}