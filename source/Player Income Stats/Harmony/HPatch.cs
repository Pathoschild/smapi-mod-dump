/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ha1fdaew/PlayerIncomeStats
**
*************************************************/

using System;
using System.Reflection;
using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;

namespace PlayerIncomeStats.Harmony
{
    public class HPatch
    {
        public static HarmonyInstance harmony;

        public static void Patch()
        {
            harmony = HarmonyInstance.Create(ModEntry.instance.Helper.ModRegistry.ModID);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(ShippingBin), "shipItem")]
    public static class ShippingBin_shipItem
    {
        public static void Postfix(Item i, Farmer who)
        {
            if (i != null)
                ModEntry.instance.OnItemShipped(i, who);
        }
    }

    [HarmonyPatch(typeof(ShippingBin), "leftClicked")]
    public static class ShippingBin_leftClicked
    {
        public static void Postfix(bool __result)
        {
            if (__result)
                ModEntry.instance.OnItemShipped(Game1.getFarm().lastItemShipped, Game1.player);
        }
    }
}