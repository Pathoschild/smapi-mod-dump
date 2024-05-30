/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Smoked-Fish/AnythingAnywhere
**
*************************************************/

using Common.Helpers;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System.Linq;
using xTile.ObjectModel;
using xTile.Tiles;

namespace AnythingAnywhere.Framework.Patches.GameLocations
{
    internal sealed class GameLocationPatch : PatchHelper
    {
        internal GameLocationPatch() : base(typeof(GameLocation)) { }
        internal void Apply()
        {
            Patch(PatchType.Postfix, nameof(GameLocation.CanPlaceThisFurnitureHere), nameof(CanPlaceThisFurnitureHerePostfix), [typeof(Furniture)]);
            Patch(PatchType.Postfix, nameof(GameLocation.isBuildable), nameof(IsBuildablePostfix), [typeof(Vector2), typeof(bool)]);
            Patch(PatchType.Postfix, nameof(GameLocation.doesTileHaveProperty), nameof(DoesTileHavePropertyPostfix), [typeof(int), typeof(int), typeof(string), typeof(string), typeof(bool)]);
            Patch(PatchType.Postfix, nameof(GameLocation.CanFreePlaceFurniture), nameof(CanFreePlaceFurniturePostfix));
            Patch(PatchType.Prefix, nameof(GameLocation.spawnWeedsAndStones), nameof(SpawnWeedsAndStonesPrefix), [typeof(int), typeof(bool), typeof(bool)]);
            Patch(PatchType.Prefix, nameof(GameLocation.loadWeeds), nameof(LoadWeedsPrefix));
        }

        // Sets all furniture types as placeable in all locations.
        private static void CanPlaceThisFurnitureHerePostfix(GameLocation __instance, Furniture furniture, ref bool __result)
        {
            if (ModEntry.Config.EnablePlacing)
                    __result = true;
        }

        // Sets tiles buildable for construction (just visual)
        private static void IsBuildablePostfix(GameLocation __instance, Vector2 tileLocation, ref bool __result, bool onlyNeedsToBePassable = false)
        {
            if (!ModEntry.Config.EnableBuilding) return;
            if (ModEntry.Config.EnableBuildAnywhere)
            {
                __result = true;
            }
            else if (!__instance.IsOutdoors && !ModEntry.Config.EnableBuildingIndoors)
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

        // Set all tiles as diggable
        private static void DoesTileHavePropertyPostfix(GameLocation __instance, int xTile, int yTile, string propertyName, string layerName, ref string __result)
        {
            if (!Context.IsWorldReady || !__instance.farmers.Any() || propertyName != "Diggable" || layerName != "Back" || !ModEntry.Config.EnablePlanting)
            {
                return;
            }

            Tile? tile = __instance.Map.GetLayer("Back")?.Tiles[xTile, yTile];
            if (tile?.TileSheet == null)
            {
                return;
            }
            string? text = null;
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
            if (ModEntry.Config.EnableDiggingAll)
            {
                __result = "T";
            }
            if (text is "Dirt" or "Grass")
            {
                __result = "T";
            }
        }

        // Allows longer reach when placing furniture
        private static void CanFreePlaceFurniturePostfix(GameLocation __instance, ref bool __result)
        {
            if (ModEntry.Config.EnablePlacing)
                __result = true;
        }

        private static bool SpawnWeedsAndStonesPrefix(GameLocation __instance, int numDebris = -1, bool weedsOnly = false, bool spawnFromOldWeeds = true)
        {
            if (!ModEntry.Config.EnableGoldClockAnywhere)
                return true;

            bool hasGoldClock = __instance.buildings.Any(building => building.buildingType.Value == "Gold Clock");
            return !hasGoldClock || Game1.netWorldState.Value.goldenClocksTurnedOff.Value;
        }

        private static bool LoadWeedsPrefix(GameLocation __instance)
        {
            if (!ModEntry.Config.EnableGoldClockAnywhere)
                return true;

            bool hasGoldClock = __instance.buildings.Any(building => building.buildingType.Value == "Gold Clock");
            return !hasGoldClock || Game1.netWorldState.Value.goldenClocksTurnedOff.Value;
        }
    }
}