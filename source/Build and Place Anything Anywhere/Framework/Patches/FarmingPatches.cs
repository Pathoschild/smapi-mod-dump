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
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using xTile.ObjectModel;
using xTile.Tiles;

namespace AnythingAnywhere.Framework.Patches;
internal sealed class FarmingPatches : PatchHelper
{
    public void Apply()
    {
        // Planting Restrictions
        Patch<GameLocation>(PatchType.Prefix, nameof(GameLocation.CheckItemPlantRules), nameof(CheckPlantingRulesPrefix), [typeof(string), typeof(bool), typeof(bool), typeof(string).MakeByRefType()]);
        Patch<IslandWest>(PatchType.Prefix, nameof(IslandWest.CanPlantSeedsHere), nameof(CheckPlantingRulesPrefix), [typeof(string), typeof(int), typeof(int), typeof(bool), typeof(string).MakeByRefType()]);
        Patch<IslandWest>(PatchType.Prefix, nameof(IslandWest.CanPlantTreesHere), nameof(CheckPlantingRulesPrefix), [typeof(string), typeof(int), typeof(int), typeof(string).MakeByRefType()]);
        Patch<Town>(PatchType.Prefix, nameof(Town.CanPlantTreesHere), nameof(CheckPlantingRulesPrefix), [typeof(string), typeof(int), typeof(int), typeof(string).MakeByRefType()]);

        // Season Restrictions
        //Patch<GameLocation>(PatchType.Postfix, nameof(GameLocation.SeedsIgnoreSeasonsHere), nameof(SeedsIgnoreSeasonsHerePostfix));
        Patch<HoeDirt>(PatchType.Prefix, nameof(HoeDirt.canPlantThisSeedHere), nameof(CanPlantThisSeedHerePrefix), [typeof(string), typeof(bool)]);
        Patch<HoeDirt>(PatchType.Prefix, nameof(HoeDirt.plant), nameof(PlantPrefix), [typeof(string), typeof(Farmer), typeof(bool)]);
        Patch<Crop>(PatchType.Postfix, nameof(Crop.IsInSeason), nameof(CropIsInSeasonPostfix), [typeof(GameLocation)]);
        Patch<Bush>(PatchType.Postfix, nameof(Bush.IsSheltered), nameof(BushIsShelteredPostfix));
        Patch<FruitTree>(PatchType.Postfix, nameof(FruitTree.IgnoresSeasonsHere), nameof(FruitTreeIgnoresSeasonsHerePostfix));
        Patch<FruitTree>(PatchType.Postfix, nameof(FruitTree.IsInSeasonHere), nameof(FruitTreeIsInSeasonHerePostfix));

        // Tree Growth Restrictions
        Patch<FruitTree>(PatchType.Postfix, nameof(FruitTree.IsGrowthBlocked), nameof(IsGrowthBlockedPostfix), [typeof(Vector2), typeof(GameLocation)]);
        Patch<FruitTree>(PatchType.Postfix, nameof(FruitTree.IsTooCloseToAnotherTree), nameof(IsTooCloseToAnotherTreePostfix), [typeof(Vector2), typeof(GameLocation), typeof(bool)]);
        Patch<Tree>(PatchType.Postfix, nameof(Tree.IsGrowthBlockedByNearbyTree), nameof(IsGrowthBlockedByNearbyTreePostfix));

        // General Patches
        Patch<GameLocation>(PatchType.Postfix, nameof(GameLocation.doesTileHaveProperty), nameof(DoesTileHavePropertyPostfix), [typeof(int), typeof(int), typeof(string), typeof(string), typeof(bool)]);
        Patch<JunimoHut>(PatchType.Postfix, nameof(JunimoHut.dayUpdate), nameof(DayUpdatePostfix), [typeof(int)]);
    }

    #region Planting Restrictions

    // Enables tree and seed planting everywhere
    private static bool CheckPlantingRulesPrefix(ref string deniedMessage, ref bool __result)
    {
        if (!ModEntry.Config.EnableFarmingAnywhere)
            return true;

        __result = true;
        return false;
    }

    #endregion

    #region Season Restrictions

    // For hoed dirt, check plant method in HoeDirt class for more info
    private static void SeedsIgnoreSeasonsHerePostfix(GameLocation __instance, ref bool __result)
    {
        if (ModEntry.Config.EnableCropsAnytime)
            __result = true;
    }

    private static bool CanPlantThisSeedHerePrefix(HoeDirt __instance, string itemId, ref bool __result, bool isFertilizer = false)
    {
        if (isFertilizer || __instance.crop != null|| !ModEntry.Config.EnableCropsAnytime)
            return true;

        __result = true;
        return false;
    }

    private static bool PlantPrefix(HoeDirt __instance, string itemId, Farmer who, bool isFertilizer, ref bool __result)
    {
        if (isFertilizer || !ModEntry.Config.EnableCropsAnytime)
            return true;

        Point tilePos = Utility.Vector2ToPoint(__instance.Tile);
        __instance.crop = new Crop(itemId, tilePos.X, tilePos.Y, __instance.Location);
        if (__instance.crop.raisedSeeds.Value)
        {
            __instance.Location.playSound("stoneStep");
        }
        __instance.Location.playSound("dirtyHit");
        Game1.stats.SeedsSown++;
        __instance.applySpeedIncreases(who);
        __instance.nearWaterForPaddy.Value = -1;
        if (__instance.hasPaddyCrop() && __instance.paddyWaterCheck())
        {
            __instance.state.Value = 1;
            __instance.updateNeighbors();
        }
        __result = true;
        return false;
    }

    // Enable growing crops in all seasons
    private static void CropIsInSeasonPostfix(Crop __instance, GameLocation location, ref bool __result)
    {
        if (ModEntry.Config.EnableCropsAnytime)
            __result = true;
    }

    // Make bushes produce in all seasons
    private static void BushIsShelteredPostfix(Bush __instance, ref bool __result)
    {
        if (ModEntry.Config.EnableBushesAnytime)
            __result = true;
    }

    // Make fruit trees produce in all seasons
    private static void FruitTreeIsInSeasonHerePostfix(FruitTree __instance, ref bool __result)
    {
        if (ModEntry.Config.EnableTreesAnytime)
            __result = true;
    }

    // Use Seasonal sprite while enabling fruit tree to continue fruit production
    private static void FruitTreeIgnoresSeasonsHerePostfix(FruitTree __instance, ref bool __result)
    {
        if (ModEntry.Config.ForceGreenhouseTreeSprite && ModEntry.Config.EnableTreesAnytime)
            __result = true;
    }

    #endregion

    #region Tree Growth Restrictions

    // Allows fruit trees to be planted right next to each other
    private static void IsTooCloseToAnotherTreePostfix(FruitTree __instance, Vector2 tileLocation, GameLocation environment, ref bool __result, bool fruitTreesOnly = false)
    {
        if (ModEntry.Config.EnableFruitTreeTweaks)
            __result = false;
    }

    // Allows fruit trees to grow even if there is an obstruction
    public static void IsGrowthBlockedPostfix(FruitTree __instance, Vector2 tileLocation, GameLocation environment, ref bool __result)
    {
        if (ModEntry.Config.EnableFruitTreeTweaks)
            __result = false;
    }

    // Allows wild trees to grow even if there is an obstruction
    private static void IsGrowthBlockedByNearbyTreePostfix(Tree __instance, ref bool __result)
    {
        if (ModEntry.Config.EnableWildTreeTweaks)
            __result = false;
    }

    #endregion

    #region General Patches

    // Set all tiles as diggable
    private static void DoesTileHavePropertyPostfix(GameLocation __instance, int xTile, int yTile, string propertyName, string layerName, ref string __result)
    {
        if (!Context.IsWorldReady || !__instance.farmers.Any() || propertyName != "Diggable" || layerName != "Back" || !ModEntry.Config.EnableFarmingAnywhere)
            return;

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

    // Send juminos out even if the farmer isn't there.
    private static void DayUpdatePostfix(JunimoHut __instance)
    {
        __instance.shouldSendOutJunimos.Value = true;
    }

    #endregion
}