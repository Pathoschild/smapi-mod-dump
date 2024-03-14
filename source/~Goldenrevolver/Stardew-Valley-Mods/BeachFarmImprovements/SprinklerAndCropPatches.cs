/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace BeachFarmImprovements
{
    using HarmonyLib;
    using Microsoft.Xna.Framework;
    using StardewValley;
    using StardewValley.Locations;
    using StardewValley.TerrainFeatures;
    using StardewObject = StardewValley.Object;

    internal class SprinklerAndCropPatches
    {
        private static BeachFarmImprovements mod;

        internal static void ApplyPatches(BeachFarmImprovements beachFarmImprovements, Harmony harmony)
        {
            mod = beachFarmImprovements;

            harmony.Patch(
               original: AccessTools.Method(typeof(StardewObject), nameof(StardewObject.ApplySprinkler)),
               prefix: new HarmonyMethod(typeof(SprinklerAndCropPatches), nameof(ApplySprinkler_Pre)));
            harmony.Patch(
               original: AccessTools.Method(typeof(StardewObject), nameof(StardewObject.ApplySprinkler)),
               postfix: new HarmonyMethod(typeof(SprinklerAndCropPatches), nameof(ApplySprinkler_Post)));
            harmony.Patch(
               original: AccessTools.Method(typeof(StardewObject), nameof(StardewObject.placementAction)),
               prefix: new HarmonyMethod(typeof(SprinklerAndCropPatches), nameof(PlacementAction_Pre)));
            harmony.Patch(
               original: AccessTools.Method(typeof(StardewObject), nameof(StardewObject.placementAction)),
               postfix: new HarmonyMethod(typeof(SprinklerAndCropPatches), nameof(PlacementAction_Post)));

            harmony.Patch(
               original: AccessTools.Method(typeof(Crop), nameof(Crop.newDay)),
               prefix: new HarmonyMethod(typeof(SprinklerAndCropPatches), nameof(DontGrowWithoutFertilizerInSand)));
            harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.CanRefillWateringCanOnTile)),
               postfix: new HarmonyMethod(typeof(SprinklerAndCropPatches), nameof(CantRefillWateringCanWithSaltWater)));
        }

        public static void ApplySprinkler_Pre(StardewObject __instance, Vector2 tile, ref bool __state)
        {
            __state = false;

            DisableSprinklerFlag_Pre(__instance, __instance.Location, (int)tile.X, (int)tile.Y, ref __state);
        }

        public static void ApplySprinkler_Post(StardewObject __instance, Vector2 tile, ref bool __state)
        {
            if (__state)
            {
                ReenableSprinklerFlag_Post(__instance.Location, (int)tile.X, (int)tile.Y);
            }
        }

        public static void PlacementAction_Pre(StardewObject __instance, GameLocation location, int x, int y, ref bool __state)
        {
            __state = false;
            var placementTile = new Vector2(x / 64, y / 64);

            DisableSprinklerFlag_Pre(__instance, location, (int)placementTile.X, (int)placementTile.Y, ref __state);
        }

        public static void PlacementAction_Post(GameLocation location, int x, int y, ref bool __state)
        {
            var placementTile = new Vector2(x / 64, y / 64);

            if (__state)
            {
                ReenableSprinklerFlag_Post(location, (int)placementTile.X, (int)placementTile.Y);
            }
        }

        private static void DisableSprinklerFlag_Pre(StardewObject __instance, GameLocation location, int x, int y, ref bool __state)
        {
            if (location is Farm && Game1.whichFarm == Farm.beach_layout && BeachFarmImprovements.HasUnlockedSprinklersInSand)
            {
                if (__instance.IsSprinkler() && location.doesTileHavePropertyNoNull(x, y, "NoSprinklers", "Back") == "T")
                {
                    location.map.GetLayer("Back").PickTile(new xTile.Dimensions.Location(x * 64, y * 64), Game1.viewport.Size).Properties["NoSprinklers"] = "F";
                    __state = true;
                }
            }
        }

        private static void ReenableSprinklerFlag_Post(GameLocation location, int x, int y)
        {
            if (location is Farm && Game1.whichFarm == Farm.beach_layout && BeachFarmImprovements.HasUnlockedSprinklersInSand)
            {
                location.map.GetLayer("Back").PickTile(new xTile.Dimensions.Location(x * 64, y * 64), Game1.viewport.Size).Properties["NoSprinklers"] = "T";
            }
        }

        public static void DontGrowWithoutFertilizerInSand(Crop __instance, ref int state)
        {
            if (!mod.Config.CantGrowInUnfertilizedSand)
            {
                return;
            }

            GameLocation environment = __instance.currentLocation;
            Vector2 tileVector = __instance.tilePosition;
            Point tile = Utility.Vector2ToPoint(tileVector);

            if (environment is not Farm || Game1.whichFarm != Farm.beach_layout)
            {
                return;
            }

            // only affect 'no sprinkler' tiles
            if (environment.doesTileHavePropertyNoNull(tile.X, tile.Y, "NoSprinklers", "Back") != "T")
            {
                return;
            }

            if (state != HoeDirt.watered)
            {
                return;
            }

            // no fertilizer -> make dry
            if (environment.terrainFeatures.TryGetValue(tileVector, out TerrainFeature terrainFeature)
                && terrainFeature is HoeDirt dirt && dirt.fertilizer.Value == null)
            {
                state = HoeDirt.dry;
            }
        }

        public static void CantRefillWateringCanWithSaltWater(GameLocation __instance, ref bool __result, int tileX, int tileY)
        {
            if (mod.Helper.ModRegistry.IsLoaded("Goldenrevolver.WateringGrantsXP"))
            {
                return;
            }

            if (__result && mod.Config.CantRefillCanWithSaltWater)
            {
                if ((__instance.doesTileHaveProperty(tileX, tileY, "Water", "Back") != null
                    || __instance.doesTileHaveProperty(tileX, tileY, "WaterSource", "Back") != null) && (__instance is Beach || __instance.catchOceanCrabPotFishFromThisSpot(tileX, tileY)))
                {
                    __result = false;
                }
            }
        }
    }
}