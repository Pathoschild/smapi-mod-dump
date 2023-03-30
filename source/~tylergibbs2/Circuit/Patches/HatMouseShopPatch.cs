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
using StardewValley.Menus;

namespace Circuit.Patches
{
    [HarmonyPatch(typeof(ShopMenu), MethodType.Constructor, new Type[] { typeof(Dictionary<ISalable, int[]>), typeof(int), typeof(string), typeof(Func<ISalable, Farmer, int, bool>), typeof(Func<ISalable, bool>), typeof(string) })]
    internal class HatMouseShopPatch
    {
        public static bool Prefix(string? who, ref Func<ISalable, Farmer, int, bool> on_purchase)
        {
            if (!ModEntry.ShouldPatch() || who != "HatMouse" || ModEntry.Instance.TaskManager is null)
                return true;

            on_purchase = ModEntry.Instance.TaskManager.OnShopMenuPurchase;
            return true;
        }
    }
}
