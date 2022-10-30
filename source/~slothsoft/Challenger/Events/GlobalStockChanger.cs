/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-challenger
**
*************************************************/

using System;
using System.Collections.Generic;
using HarmonyLib;
using StardewValley.Locations;

// ReSharper disable InconsistentNaming

namespace Slothsoft.Challenger.Events;

internal static class GlobalStockChanger {
    
    private static Harmony? _harmony;
    private static readonly IList<Action<IDictionary<ISalable, int[]>>> ChangerList =
        new List<Action<IDictionary<ISalable, int[]>>>();

    public static void AddChanger(Action<IDictionary<ISalable, int[]>> changer) {
        if (_harmony == null) {
            _harmony = new Harmony(ChallengerMod.Instance.ModManifest.UniqueID + ".GlobalStockChanger");
            _harmony.Patch(
                original: AccessTools.Method(
                    typeof(SeedShop),
                    nameof(SeedShop.shopStock)
                ),
                postfix: new HarmonyMethod(typeof(GlobalStockChanger), nameof(ChangeShopStock))
            );
            _harmony.Patch(
                original: AccessTools.Method(
                    typeof(Utility),
                    nameof(Utility.getJojaStock)
                ),
                postfix: new HarmonyMethod(typeof(GlobalStockChanger), nameof(ChangeShopStock))
            );
        }
        ChangerList.Add(changer);
    }

    public static void ChangeShopStock(ref Dictionary<ISalable, int[]> __result) {
        foreach (var changer in ChangerList) {
            changer(__result);
        }
    }

    public static void RemoveChanger(Action<IDictionary<ISalable, int[]>> changer) {
        ChangerList.Remove(changer);

        if (ChangerList.Count == 0) {
            _harmony?.UnpatchAll(ChallengerMod.Instance.ModManifest.UniqueID + ".GlobalStockChanger");
            _harmony = null;
        }
    }
}