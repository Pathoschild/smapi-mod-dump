/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FashionSense
**
*************************************************/

using FashionSense.Framework.Utilities;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Shops;
using StardewValley.Internal;
using StardewValley.Tools;
using System;
using System.Collections.Generic;

namespace FashionSense.Framework.Patches.ShopLocations
{
    internal class ShopBuilderPatch : PatchTemplate
    {
        private readonly Type _object = typeof(ShopBuilder);

        internal ShopBuilderPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(ShopBuilder.GetShopStock), new[] { typeof(string), typeof(ShopData) }), postfix: new HarmonyMethod(GetType(), nameof(GetShopStockPostfix)));
        }

        internal static GenericTool GetHandMirrorTool()
        {
            var handMirror = new GenericTool();
            handMirror.modData[ModDataKeys.HAND_MIRROR_FLAG] = true.ToString();

            return handMirror;
        }

        private static void GetShopStockPostfix(string shopId, ShopData shop, ref Dictionary<ISalable, ItemStockInformation> __result)
        {
            if (shopId == "SeedShop")
            {
                __result.Add(GetHandMirrorTool(), new ItemStockInformation(1500, 1));
            }
        }
    }
}
