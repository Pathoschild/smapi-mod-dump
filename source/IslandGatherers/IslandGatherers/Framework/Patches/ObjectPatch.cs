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
    internal class ObjectPatch
    {
        private static IMonitor monitor;
        private readonly System.Type _object = typeof(Object);

        internal ObjectPatch(IMonitor modMonitor)
        {
            monitor = modMonitor;
        }

        internal void Apply(HarmonyInstance harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(Object.placementAction), new[] { typeof(GameLocation), typeof(int), typeof(int), typeof(Farmer) }), prefix: new HarmonyMethod(GetType(), nameof(PlacementActionPrefix)));
        }

        [HarmonyPriority(Priority.VeryHigh)]
        private static bool PlacementActionPrefix(Object __instance, GameLocation location, int x, int y, Farmer who = null)
        {
            if (__instance.name == "Parrot Pot")
            {
                if (!location.IsOutdoors || location.Name != "IslandWest")
                {
                    monitor.Log("Attempted to place Parrot Pot indoors!", LogLevel.Trace);
                    Game1.showRedMessage("The Parrot Pot can only be placed on the farm at Ginger Island!");

                    return false;
                }

                if (location.numberOfObjectsWithName("Parrot Pot") > 0)
                {
                    monitor.Log("Attempted to place another Parrot Pot where there already is one!", LogLevel.Trace);
                    Game1.showRedMessage("You can only place one Parrot Pot!");

                    return false;
                }

                monitor.Log("Attempting to place Parrot Pot.", LogLevel.Debug);
                Vector2 placementTile = new Vector2(x / 64, y / 64);

                ParrotPot parrotPot = __instance as ParrotPot;
                if (parrotPot is null)
                {
                    parrotPot = new ParrotPot(placementTile, __instance.ParentSheetIndex);
                }

                parrotPot.placementAction(location, x, y);
                location.objects.Add(placementTile, parrotPot);
                location.playSound("hammer");

                // Remove the placed item from the player's inventory
                Game1.player.reduceActiveItemByOne();

                return false;
            }

            return true;
        }
    }
}
