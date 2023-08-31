/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/BuildOnAnyTile
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using System;
using xTile.Dimensions;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace BuildOnAnyTile
{
    /// <summary>A Harmony patch that makes <see cref="BuildableGameLocation"/> allow buildings to be placed on any tile (depending on <see cref="ModConfig"/> settings).</summary>
    public static class HarmonyPatch_BuildOnAnyTile
    {
        public static void ApplyPatch(Harmony harmony)
        {
            ModEntry.Instance.Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_BuildOnAnyTile)}\": postfixing SDV method \"BuildableGameLocation.isBuildable(Vector2)\".", LogLevel.Trace);
            harmony.Patch(
                original: AccessTools.Method(typeof(BuildableGameLocation), nameof(BuildableGameLocation.isBuildable), new[] { typeof(Vector2) }),
                postfix: new HarmonyMethod(typeof(HarmonyPatch_BuildOnAnyTile), nameof(IsBuildable))
            );
        }

        /// <summary>A Harmony postfix patch that causes <see cref="BuildableGameLocation.isBuildable(Vector2)"/> to return true under more conditions.</summary>
        /// <param name="__instance">The buildable location on which this method is called.</param>
        /// <param name="tileLocation">The tile being checked.</param>
        /// <param name="__result">True if this tile should allow <see cref="Building"/> placement.</param>
        [HarmonyPriority(Priority.Low)] //run after other patches on this method by default (avoids conflict with similar features in PlacementPlus, etc)
        public static void IsBuildable(BuildableGameLocation __instance, Vector2 tileLocation, ref bool __result)
        {
            try
            {
                if (__result == true || ModEntry.Config.EverythingEnabled()) //if the tile is already buildable, or all features are enabled
                {
                    __result = true; //this tile is buildable
                    return;
                }

                if (ModEntry.Config.EverythingDisabled()) //if every feature is disabled
                    return; //this tile is NOT buildable

                if (ModEntry.Config.BuildOnAllTerrainFeatures == false) //if most terrain features should prevent building (based on the original method's behavior)
                {
                    Rectangle tileLocationRect = new Rectangle((int)tileLocation.X * 64, (int)tileLocation.Y * 64, 64, 64); //get a rectangle representing this tile

                    if (__instance.terrainFeatures.TryGetValue(tileLocation, out TerrainFeature feature) //if this tile has a terrain feature
                        && tileLocationRect.Intersects(feature.getBoundingBox(tileLocation))) //AND the feature's box overlaps with the tile (note: copied from GameLocation.isOccupiedForPlacement)
                    {
                        if (!__instance.terrainFeatures[tileLocation].isPassable() //if the feature is impassable
                            || (feature is HoeDirt dirt && dirt.crop != null)) //OR the feature is a crop
                        {
                            return; //this tile is NOT buildable
                        }
                    }
                }

                if (ModEntry.Config.BuildOnOtherBuildings == false) //if collision with other buildings should prevent building
                {
                    foreach (Building building in __instance.buildings) //for each existing building
                    {
                        if (building.isTileOccupiedForPlacement(tileLocation, null)) //if this building occupies this tile
                            return; //this tile is NOT buildable
                    }
                }

                if (ModEntry.Config.BuildOnWater == false) //if water should prevent building
                {
                    if (__instance.doesTileHaveProperty((int)tileLocation.X, (int)tileLocation.Y, "Water", "Back") != null //if this tile is water
                        && __instance.doesTileHaveProperty((int)tileLocation.X, (int)tileLocation.Y, "Passable", "Buildings") == null) //AND this tile does NOT specifically allow buildings
                    {
                        return; //this tile is NOT buildable
                    }
                }

                if (ModEntry.Config.BuildOnImpassableTiles == false) //if impassable tiles should prevent building
                {
                    if (ModEntry.Config.BuildOnWater == false || __instance.isOpenWater((int)tileLocation.X, (int)tileLocation.Y) == false) //if this tile is NOT specifically allowed by the water setting
                    {
                        if (__instance.isTileOccupiedForPlacement(tileLocation) //if this tile is occupied
                            || __instance.isTilePassable(new Location((int)tileLocation.X, (int)tileLocation.Y), Game1.viewport) == false) //OR if this tile is NOT passable
                        {
                            return; //this tile is NOT buildable
                        }
                    }
                }

                if (ModEntry.Config.BuildOnNoFurnitureTiles == false) //if "no furniture" tiles should prevent building
                {
                    if (__instance.doesTileHaveProperty((int)tileLocation.X, (int)tileLocation.Y, "NoFurniture", "Back") != null) //if this tile has a "NoFurniture" property
                        return; //this tile is NOT buildable
                }

                if (ModEntry.Config.BuildOnCaveAndShippingZones == false) //if "no build" zones should prevent building
                {
                    //NOTE: as of SDV 1.5.5, the static preset rectangle "zones" no longer exist, and farm maps use the tile property "Buildable" "f" instead;
                    //that tile property may exist elsewhere too, but checking relevant properties here should be understandable enough

                    //try to get the cave and shipping no-build zones
                    Rectangle? caveNoBuildRect = ModEntry.Instance.Helper.Reflection.GetField<Rectangle>(__instance, "caveNoBuildRect", false)?.GetValue();
                    Rectangle? shippingAreaNoBuildRect = ModEntry.Instance.Helper.Reflection.GetField<Rectangle>(__instance, "shippingAreaNoBuildRect", false)?.GetValue();

                    if (caveNoBuildRect.HasValue && shippingAreaNoBuildRect.HasValue) //if these fields exist (e.g. SDV v1.5.4 or earlier is in use)
                    {
                        if (caveNoBuildRect.Value.Contains(Utility.Vector2ToPoint(tileLocation)) //if this tile is within the cave entrance zone
                        || shippingAreaNoBuildRect.Value.Contains(Utility.Vector2ToPoint(tileLocation))) //OR this tile is within the shipping bin zone
                        {
                            return; //this tile is NOT buildable
                        }
                    }

                    string buildableValue = __instance.doesTileHavePropertyNoNull((int)tileLocation.X, (int)tileLocation.Y, "Buildable", "Back"); //get the value of this tile's "Buildable" property ("" if null)
                    if (buildableValue.Equals("f", StringComparison.OrdinalIgnoreCase) || buildableValue.Equals("false", StringComparison.OrdinalIgnoreCase)) //if the value is false
                        return; //this tile is NOT buildable
                }

                //all features have been checked; building was not prevented by any disabled features

                __result = true; //this tile is buildable
                return;
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.LogOnce($"Encountered an error in Harmony patch \"{nameof(HarmonyPatch_BuildOnAnyTile)}\". The default building rules will be used instead. Full error message:\n-----\n{ex.ToString()}", LogLevel.Error);
                return; //do nothing
            }
        }
    }
}
