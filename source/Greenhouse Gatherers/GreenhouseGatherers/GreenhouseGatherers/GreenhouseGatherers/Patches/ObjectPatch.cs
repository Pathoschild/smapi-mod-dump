/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/GreenhouseGatherers
**
*************************************************/

using Harmony;
using StardewValley;
using System.Reflection;
using StardewModdingAPI;
using GreenhouseGatherers.GreenhouseGatherers.Objects;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using System.Linq;

namespace GreenhouseGatherers.GreenhouseGatherers.Patches
{
    [HarmonyPatch]
    public class ObjectPatch
    {
        private static IMonitor monitor = ModResources.GetMonitor();

        internal static MethodInfo TargetMethod()
        {
            return AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.placementAction));
        }

        internal static bool Prefix(Object __instance, GameLocation location, int x, int y, Farmer who = null)
        {
            if (__instance.name == "Harvest Statue")
            {
                if (location.IsOutdoors)
                {
                    monitor.Log("Attempted to place Harvest Statue outdoors!", LogLevel.Trace);
                    Game1.showRedMessage("Harvest Statues can only be placed indoors!");

                    return false;
                }

                if (location.numberOfObjectsWithName("Harvest Statue") > 0)
                {
                    monitor.Log("Attempted to place another Harvest Statue where there already is one!", LogLevel.Trace);
                    Game1.showRedMessage("You can only place one Harvest Statue per building!");

                    return false;
                }

                monitor.Log("Attempting to place Harvest Statue.", LogLevel.Debug);
                Vector2 placementTile = new Vector2(x / 64, y / 64);

                HarvestStatue harvestStatue = __instance as HarvestStatue;
                if (harvestStatue is null)
                {
                    harvestStatue = new HarvestStatue(placementTile, __instance.ParentSheetIndex);
                }

                harvestStatue.placementAction(location, x, y);
                location.objects.Add(placementTile, harvestStatue);
                location.playSound("hammer");

                // Remove the placed item from the player's inventory
                Game1.player.reduceActiveItemByOne();

                return false;
            }

            return true;
        }
    }
}
