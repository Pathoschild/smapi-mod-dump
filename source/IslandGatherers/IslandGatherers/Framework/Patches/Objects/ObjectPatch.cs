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

namespace IslandGatherers.Framework.Patches.Objects
{
    internal class ObjectPatch : PatchTemplate
    {
        private readonly Type _object = typeof(Object);

        internal ObjectPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(Object.placementAction), new[] { typeof(GameLocation), typeof(int), typeof(int), typeof(Farmer) }), prefix: new HarmonyMethod(GetType(), nameof(PlacementActionPrefix)));
        }

        [HarmonyBefore(new string[] { "spacechase0.DynamicGameAssets", "furyx639.ExpandedStorage" })]
        internal static bool PlacementActionPrefix(Object __instance, GameLocation location, int x, int y, Farmer who = null)
        {
            if (__instance.name == "Parrot Pot")
            {
                if (!location.IsOutdoors || location.Name != "IslandWest")
                {
                    _monitor.Log("Attempted to place Parrot Pot indoors!", LogLevel.Trace);
                    Game1.showRedMessage("The Parrot Pot can only be placed on the farm at Ginger Island!");

                    return false;
                }

                if (location.objects.Values.Any(o => o.modData.ContainsKey(IslandGatherers.parrotPotFlag)))
                {
                    _monitor.Log("Attempted to place another Parrot Pot where there already is one!", LogLevel.Trace);
                    Game1.showRedMessage("You can only place one Parrot Pot!");

                    return false;
                }
            }

            return true;
        }
    }
}
