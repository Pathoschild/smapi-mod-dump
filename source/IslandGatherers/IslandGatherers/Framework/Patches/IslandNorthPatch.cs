/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/IslandGatherers
**
*************************************************/

using Harmony;
using IslandGatherers.Framework.Objects;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IslandGatherers.Framework.Patches
{
    internal class IslandNorthPatch
    {
        private static IMonitor monitor;
        private readonly System.Type _islandNorth = typeof(IslandNorth);

        internal IslandNorthPatch(IMonitor modMonitor)
        {
            monitor = modMonitor;
        }

        internal void Apply(HarmonyInstance harmony)
        {
            harmony.Patch(AccessTools.Method(_islandNorth, nameof(IslandNorth.getIslandMerchantTradeStock), new[] { typeof(Farmer) }), postfix: new HarmonyMethod(GetType(), nameof(GetIslandMerchantTradeStockPostfix)));
        }

        private static void GetIslandMerchantTradeStockPostfix(IslandNorth __instance, ref Dictionary<ISalable, int[]> __result, Farmer who)
        {
            if (Game1.MasterPlayer.mailReceived.Contains("Island_UpgradeHouse"))
            {
                __result.Add(new Object(Vector2.Zero, IslandGatherers.parrotStorageID), new int[4] { 0, 2147483647, 848, 50 });
            }
        }
    }
}
