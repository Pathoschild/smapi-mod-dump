/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Smoked-Fish/AnythingAnywhere
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData;
using StardewValley.Objects;
using StardewValley.TokenizableStrings;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using xTile;
using Object = StardewValley.Object;

namespace AnythingAnywhere.Framework.Patches.GameLocations
{
    internal class GameLocationPatch : PatchTemplate
    {
        private readonly Type _object = typeof(GameLocation);

        internal GameLocationPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(GameLocation.CanPlaceThisFurnitureHere), new[] { typeof(Furniture)}), postfix: new HarmonyMethod(GetType(), nameof(CanPlaceThisFurnitureHerePostfix)));
            harmony.Patch(AccessTools.Method(_object, nameof(GameLocation.isBuildable), new[] { typeof(Vector2), typeof(bool) }), postfix: new HarmonyMethod(GetType(), nameof(IsBuildablePostfix)));
            harmony.Patch(AccessTools.Method(_object, nameof(GameLocation.IsBuildableLocation)), postfix: new HarmonyMethod(GetType(), nameof(IsBuildableLocationPostfix)));
        }

        // Sets all furniture types as placeable in all locations. This lets you place beds outside.
        private static void CanPlaceThisFurnitureHerePostfix(GameLocation __instance, Furniture furniture, ref bool __result)
        {
            if (ModEntry.modConfig.EnablePlacing)
                    __result = true;
        }

        // Sets tiles buildable for construction
        private static void IsBuildablePostfix(GameLocation __instance, Vector2 tileLocation, ref bool __result, bool onlyNeedsToBePassable = false)
        {
            if (ModEntry.modConfig.EnableBuilding)
            {
                if (__instance.isTilePassable(tileLocation) && !__instance.isWaterTile((int)tileLocation.X, (int)tileLocation.Y))
                {
                    __result = !__instance.IsTileOccupiedBy(tileLocation, CollisionMask.All, CollisionMask.All);
                }
                else if (ModEntry.modConfig.EnableFreeBuild)
                {
                    __result = true;
                }
                else
                {
                    __result = false; // Set to false if the tile is not passable
                }
            }
        }

        // Set all locations buildable.
        private static void IsBuildableLocationPostfix(GameLocation __instance, ref bool __result)
        {
            if (ModEntry.modConfig.EnableBuilding)
                __result = true;
        }
    }
}