/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/AlternativeTextures
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Shops;
using StardewValley.Internal;
using System;
using System.Collections.Generic;

namespace AlternativeTextures.Framework.Patches.ShopLocations
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

        private static void GetShopStockPostfix(string shopId, ShopData shop, ref Dictionary<ISalable, ItemStockInformation> __result)
        {
            if (shopId == "Carpenter")
            {
                __result.Add(GetPaintBucketTool(), new ItemStockInformation(500, 1));
                __result.Add(GetScissorsTool(), new ItemStockInformation(500, 1));
                __result.Add(GetPaintBrushTool(), new ItemStockInformation(500, 1));
                __result.Add(GetSprayCanTool(), new ItemStockInformation(500, 1));
                __result.Add(GetCatalogueTool(), new ItemStockInformation(500, 1));
            }
        }
    }
}
