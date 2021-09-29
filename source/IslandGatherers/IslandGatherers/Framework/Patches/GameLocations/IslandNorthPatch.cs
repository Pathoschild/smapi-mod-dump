/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/IslandGatherers
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Object = StardewValley.Object;

namespace IslandGatherers.Framework.Patches.GameLocations
{
    internal class IslandNorthPatch : PatchTemplate
    {
        private readonly Type _object = typeof(IslandNorth);

        internal IslandNorthPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(IslandNorth.getIslandMerchantTradeStock), new[] { typeof(Farmer) }), postfix: new HarmonyMethod(GetType(), nameof(GetIslandMerchantTradeStockPostfix)));
        }

        private static void GetIslandMerchantTradeStockPostfix(IslandNorth __instance, ref Dictionary<ISalable, int[]> __result, Farmer who)
        {
            if (Game1.MasterPlayer.mailReceived.Contains("Island_UpgradeHouse"))
            {
                var statueItem = ApiManager.GetDynamicGameAssetsInterface().SpawnDGAItem("PeacefulEnd.IslandGatherers.ParrotPot/ParrotPot") as Item;
                __result.Add(statueItem, new int[4] { 0, 2147483647, 848, 50 });
            }
        }
    }
}
