/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/HDPortraits
**
*************************************************/

using AeroCore.Utils;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace HDPortraits.Patches
{
    internal class STFPatch
    {
        private static Type STFShop;
        private static MethodInfo ShopName;
        internal static void Init()
        {
            if (!ModEntry.helper.ModRegistry.IsLoaded("Cherry.ShopTileFramework"))
                return;

            ModEntry.monitor.Log("Patching STF...", LogLevel.Debug);
            STFShop = AccessTools.TypeByName("ShopTileFramework.Shop.ItemShop");
            ShopName = AccessTools.PropertyGetter(STFShop, "ShopName");

            ModEntry.harmony.Patch(STFShop.MethodNamed("DisplayShop"), postfix: new(typeof(STFPatch), nameof(AfterShopOpened)));
        }
        private static void AfterShopOpened(object __instance)
        {
            string name = (string)ShopName.Invoke(__instance, Array.Empty<object>());
            if (name is null || Game1.activeClickableMenu is not ShopMenu shop || shop.portraitPerson is null)
                return;

            name = "STF." + name;

            if (ModEntry.TryGetMetadata(name, PortraitDrawPatch.GetSuffix(shop.portraitPerson), out var meta))
            {
                PortraitDrawPatch.lastLoaded.Value.Add(meta);
                PortraitDrawPatch.currentMeta.Value = meta;
                meta.Animation?.Reset();
            }
        }
    }
}
