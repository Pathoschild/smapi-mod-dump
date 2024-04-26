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
using StardewValley;
using StardewModdingAPI;
using StardewValley.Objects;
using Microsoft.Xna.Framework;
using xTile.ObjectModel;
using xTile.Tiles;
using System;

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
            harmony.Patch(AccessTools.Method(_object, nameof(GameLocation.doesTileHaveProperty), new [] { typeof(int), typeof(int), typeof(string), typeof(string), typeof(bool) }), postfix: new HarmonyMethod(GetType(), nameof(DoesTileHavePropertyPostfix)));
            harmony.Patch(AccessTools.Method(_object, nameof(GameLocation.CanFreePlaceFurniture)), postfix: new HarmonyMethod(GetType(), nameof(CanFreePlaceFurniturePostfix)));

        }

        // Sets all furniture types as placeable in all locations.
        private static void CanPlaceThisFurnitureHerePostfix(GameLocation __instance, Furniture furniture, ref bool __result)
        {
            if (ModEntry.modConfig.EnablePlacing)
                    __result = true;
        }

        // Sets tiles buildable for construction (just visual)
        private static void IsBuildablePostfix(GameLocation __instance, Vector2 tileLocation, ref bool __result, bool onlyNeedsToBePassable = false)
        {
            if (ModEntry.modConfig.EnableBuilding)
            {
                if (ModEntry.modConfig.EnableBuildAnywhere)
                {
                    __result = true;
                }
                else if (!__instance.IsOutdoors && !ModEntry.modConfig.EnableBuildingIndoors)
                {
                    __result = false;
                }
                else if (__instance.isTilePassable(tileLocation) && !__instance.isWaterTile((int)tileLocation.X, (int)tileLocation.Y))
                {
                    __result = !__instance.IsTileOccupiedBy(tileLocation, CollisionMask.All, CollisionMask.All);
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
            {
                if (ModEntry.modConfig.EnableBuildingIndoors)
                    __result = true;

                if (__instance.IsOutdoors)
                    __result = true;
            }

            if (ModEntry.modConfig.EnableBuildAnywhere)
                __result = true;
        }

        // Set all tiles as diggable
        private static void DoesTileHavePropertyPostfix(GameLocation __instance, int xTile, int yTile, string propertyName, string layerName, ref string __result)
        {
            if (!Context.IsWorldReady || !__instance.farmers.Any() || !(propertyName == "Diggable") || !(layerName == "Back") || !ModEntry.modConfig.EnablePlanting)
            {
                return;
            }

            Tile tile = __instance.Map.GetLayer("Back")?.Tiles[xTile, yTile];
            if (tile?.TileSheet == null)
            {
                return;
            }
            string text = null;
            IPropertyCollection tileIndexProperties = tile.TileIndexProperties;
            if (tileIndexProperties != null && tileIndexProperties.TryGetValue("Type", out var value))
            {
                text = value?.ToString();
            }
            else
            {
                IPropertyCollection properties = tile.Properties;
                if (properties != null && properties.TryGetValue("Type", out value))
                {
                    text = value?.ToString();
                }
            }
            if (ModEntry.modConfig.EnableDiggingAll)
            {
                __result = "T";
            }
            if (text == "Dirt" || text == "Grass")
            {
                __result = "T";
            }
        }

        private static void CanFreePlaceFurniturePostfix(GameLocation __instance, ref bool __result)
        {
            if (ModEntry.modConfig.EnablePlacing)
                __result = true;
        }
    }
}