/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using DecidedlyShared.APIs;
using DecidedlyShared.Logging;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace SmartBuilding.Utilities
{
    public class PlacementUtils
    {
        private readonly ModConfig config;
        private readonly ITapGiantCropsAPI? giantCropTapApi;
        private readonly IGrowableBushesAPI? growableBushesApi;
        private readonly IModHelper helper;
        private readonly IdentificationUtils identificationUtils;
        private readonly Logger logger;
        private readonly IMoreFertilizersAPI? moreFertilizersApi;

        public PlacementUtils(ModConfig config, IdentificationUtils identificationUtils,
            IMoreFertilizersAPI? moreFertilizersApi, ITapGiantCropsAPI? giantCropTapApi, IGrowableBushesAPI? growableBushesApi,
            Logger logger, IModHelper helper)
        {
            this.config = config;
            this.identificationUtils = identificationUtils;
            this.moreFertilizersApi = moreFertilizersApi;
            this.giantCropTapApi = giantCropTapApi;
            this.growableBushesApi = growableBushesApi;
            this.logger = logger;
            this.helper = helper;
        }

        public bool HasAdjacentNonWaterTile(Vector2 v)
        {
            // Right now, this is only applicable for crab pots.
            if (this.config.CrabPotsInAnyWaterTile)
                return true;
            this.logger.Log(I18n.SmartBuilding_Message_CheatyOptions_CrabPotsInAnyWaterTile_Disabled(), LogLevel.Trace,
                true);

            // We create our list of cardinal and ordinal directions.
            var directions = new List<Vector2>
            {
                v + new Vector2(-1, 0), // Left
                v + new Vector2(1, 0), // Right
                v + new Vector2(0, -1), // Up
                v + new Vector2(0, 1), // Down
                v + new Vector2(-1, -1), // Up left
                v + new Vector2(1, -1), // Up right
                v + new Vector2(-1, 1), // Down left
                v + new Vector2(1, 1) // Down right
            };

            // Then loop through in each of those directions relative to the passed in tile to determine if a water tile is adjacent.
            foreach (var vector in directions)
                if (!Game1.currentLocation.isWaterTile((int)vector.X, (int)vector.Y))
                    return true;

            return false;
        }

        /// <summary>
        ///     Will return whether or not a tile can be placed
        /// </summary>
        /// <param name="v">The world-space Tile in which the check is to be performed.</param>
        /// <param name="i">The placeable type.</param>
        /// <returns></returns>
        public bool CanBePlacedHere(Vector2 v, Item i)
        {
            // If the item is not an SObject, we return.
            if (i is not SObject)
                return false;

            var itemType = this.identificationUtils.IdentifyItemType((SObject)i);
            var itemInfo = this.identificationUtils.GetItemInfo((SObject)i);
            var here = Game1.currentLocation;

            switch (itemType)
            {
                case ItemType.NotPlaceable:
                    return false;
                case ItemType.Torch:
                    // We need to figure out whether there's a fence in the placement tile.
                    if (here.objects.ContainsKey(v))
                    {
                        // We know there's an object at these coordinates, so we grab a reference.
                        var o = here.objects[v];

                        if (this.identificationUtils.IsTypeOfObject(o, ItemType.Fence))
                        {
                            // It's a type of fence, but we also want to ensure that it isn't a gate.

                            if (o.Name.Equals("Gate"))
                                return false;

                            return true;
                        }
                    }
                    else // This is temporary until everything gets split out into separate methods eventually.
                        goto GenericPlaceable;

                    break;
                case ItemType.CrabPot
                    : // We need to determine if the crab pot is being placed in an appropriate water tile.
                    return CrabPot.IsValidCrabPotLocationTile(here, (int)v.X, (int)v.Y) &&
                           this.HasAdjacentNonWaterTile(v);
                case ItemType.GrassStarter:
                    // If there's a terrain feature here, we can't possibly place a grass starter.
                    return !here.terrainFeatures.ContainsKey(v);
                case ItemType.Floor:
                    // In this case, we need to know whether there's a TerrainFeature in the tile.
                    if (here.terrainFeatures.ContainsKey(v))
                    {
                        // At this point, we know there's a terrain feature here, so we grab a reference to it.
                        var tf = Game1.currentLocation.terrainFeatures[v];

                        // Then we check to see if it is, indeed, Flooring.
                        if (tf != null && tf is Flooring floor)
                        {
                            // We now know we're dealing with flooring, so if the floor replacement
                            // setting is enabled, we move on to our other checks.

                            if (this.config.EnableReplacingFloors)
                            {
                                // If the names aren't the same, we return true, because we want to replace. Otherwise, false.
                                if (!this.identificationUtils.GetFlooringNameFromId(floor.whichFloor).Equals(i.Name))
                                    return true;
                                return false;
                            }

                            this.logger.Log(I18n.SmartBuilding_Message_CheatyOptions_FloorReplacement_Disabled(),
                                LogLevel.Trace, true);
                        }

                        return false;
                    }

                    if (here.objects.ContainsKey(v))
                    {
                        // We know an object exists here now, so we grab it.
                        var o = here.objects[v];
                        ItemType type;
                        Item itemToDestroy;

                        itemToDestroy = Utility.fuzzyItemSearch(o.Name);
                        type = this.identificationUtils.IdentifyItemType((SObject)itemToDestroy);

                        if (type == ItemType.Fence)
                            // This is a fence, so we return true.
                            return true;
                    }

                    // Here, we want to display a message if the floor COULD be placed if the appropriate setting were enabled.
                    if (!here.isTileLocationOpen(v) && !this.config.LessRestrictiveFloorPlacement)
                        this.logger.Log(I18n.SmartBuilding_Message_CheatyOptions_MoreLaxFloorPlacement_Disabled(),
                            LogLevel.Trace, true);

                    // At this point, we return appropriately with vanilla logic, or true depending on the placement setting.
                    return this.config.LessRestrictiveFloorPlacement || here.isTileLocationOpen(v);
                case ItemType.Chest:
                    // We want to be extra safe here, so we confirm it is in fact of type Chest.
                    if (i is Chest)
                    {
                        // Then grab a reference to it...
                        var chest = (Chest)i;

                        // ...and return false if it contains any items.
                        if (chest.Items.Count > 0)
                            return false;
                    }

                    goto case ItemType.Generic;
                case ItemType.Fertilizer:
                    // If the setting to enable fertilizers is off, return false to ensure they can't be added to the queue.
                    if (!this.config.EnableFertilizers)
                    {
                        this.logger.Log(I18n.SmartBuilding_Message_CheatyOptions_EnableFertilisers_Disabled(),
                            LogLevel.Trace, true);
                        return false;
                    }

                    // If this is a More Fertilizers fertilizer, defer to More Fertilizer's placement logic.
                    if (i is SObject obj && this.moreFertilizersApi?.CanPlaceFertilizer(obj, here, v) == true)
                        return true;

                    // If there's an object present, we don't want to place any fertilizer.
                    // It is technically valid, but there's no reason someone would want to.
                    if (here.Objects.ContainsKey(v))
                        return false;

                    if (here.terrainFeatures.ContainsKey(v))
                        // We know there's a TerrainFeature here, so next we want to check if it's HoeDirt.
                        if (here.terrainFeatures[v] is HoeDirt)
                        {
                            // If it is, we want to grab the HoeDirt, and check for the possibility of planting.
                            var hd = (HoeDirt)here.terrainFeatures[v];

                            if (hd.crop != null)
                            {
                                // If the HoeDirt has a crop, we want to grab it and check for growth phase and fertilization status.
                                var cropToCheck = hd.crop;

                                if (cropToCheck.currentPhase.Value != 0)
                                    // If the crop's current phase is not zero, we return false.
                                    return false;
                            }

                            // At this point, we fall to vanilla logic to determine placement validity.
                            return hd.canPlantThisSeedHere(i.ItemId, true);
                        }

                    return false;
                case ItemType.TreeFertilizer:
                    // If the setting to enable tree fertilizers is off, return false to ensure they can't be added to the queue.
                    if (!this.config.EnableFertilizers)
                    {
                        this.logger.Log(I18n.SmartBuilding_Message_CheatyOptions_EnableFertilisers_Disabled(),
                            LogLevel.Trace, true);
                        return false;
                    }

                    // First, we determine if there's a TerrainFeature here.
                    if (here.terrainFeatures.ContainsKey(v))
                        // Then we check if it's a tree.
                        if (here.terrainFeatures[v] is Tree)
                        {
                            // It is a tree, so now we check to see if the tree is fertilised.
                            var tree = (Tree)here.terrainFeatures[v];

                            // If it's already fertilised, there's no need for us to want to place tree fertiliser on it, so we return false.
                            if (tree.fertilized.Value)
                                return false;
                            return true;
                        }

                    return false;
                case ItemType.Seed:
                    // If the setting to enable crops is off, return false to ensure they can't be added to the queue.
                    if (!this.config.EnablePlantingCrops)
                    {
                        this.logger.Log(I18n.SmartBuilding_Message_CheatyOptions_EnablePlantingCrops_Disabled(),
                            LogLevel.Trace, true);
                        return false;
                    }

                    // If there's an object present, we don't want to place a seed.
                    // It is technically valid, but there's no reason someone would want to.
                    if (here.Objects.ContainsKey(v))
                        return false;

                    // First, we check for a TerrainFeature.
                    if (here.terrainFeatures.ContainsKey(v))
                        // Then, we check to see if it's HoeDirt.
                        if (here.terrainFeatures[v] is HoeDirt)
                        {
                            // Next, we check to see if it's a DGA item.
                            if (itemInfo.IsDgaItem)
                            {
                                // It is, so we try to use DGA to determine plantability.
                                // First, we grab a reference to our HoeDirt.
                                var hd = (HoeDirt)here.terrainFeatures[v];

                                // Then reflect into DGA to get the CanPlantThisSeedHere method.
                                var canPlant = this.helper.Reflection.GetMethod(
                                    i,
                                    "CanPlantThisSeedHere"
                                );

                                if (canPlant != null)
                                    return canPlant.Invoke<bool>(hd, (int)v.X, (int)v.Y, false);
                                // And we return false here if the reflection failed, because we couldn't determine plantability.
                                this.logger.Log(
                                    "Reflecting into DGA to determine seed plantability failed. Please DO NOT report this to DGA's author.",
                                    LogLevel.Error);
                                return false;
                            }
                            else
                            {
                                // If it is, we grab a reference to the HoeDirt to use its canPlantThisSeedHere method.
                                var hd = (HoeDirt)here.terrainFeatures[v];

                                return hd.canPlantThisSeedHere(i.ItemId, false);
                            }
                        }

                    return false;
                case ItemType.Tapper:
                    // If the setting to enable tree tappers is off, we return false here to ensure nothing further happens.
                    if (!this.config.EnableTreeTappers)
                    {
                        this.logger.Log(I18n.SmartBuilding_Message_CheatyOptions_EnableTreeTappers_Disabled(),
                            LogLevel.Trace, true);
                        return false;
                    }

                    // First, we need to check if there's a TerrainFeature here.
                    if (here.terrainFeatures.ContainsKey(v))
                        // If there is, we check to see if it's a tree.
                        if (here.terrainFeatures[v] is Tree)
                        {
                            // If it is, we grab a reference to the tree to check its details.
                            var tree = (Tree)here.terrainFeatures[v];

                            // If the tree isn't tapped, we confirm that a tapper can be placed here.
                            if (!tree.tapped)
                                // If the tree is fully grown, we *can* place a tapper.
                                return tree.growthStage >= 5;
                        }

                    // If there isn't a tree here, we next check for a giant grop.
                    foreach (var clump in here.resourceClumps)
                        if (clump is GiantCrop && clump.occupiesTile((int)v.X, (int)v.Y))
                        {
                            // It's a giant crop, so we defer to Tap Giant Crop's API for placement validity.
                            if (this.giantCropTapApi != null)
                            {
                                bool canPlace = this.giantCropTapApi.CanPlaceTapper(here, v, (SObject)i);

                                // if (!canPlace)
                                //     Game1.showRedMessage("This is not a valid giant crop.");
                                // else
                                //     Game1.showRedMessage("This is a valid giant crop.");
                                return canPlace;
                            }

                            return false;
                        }

                    return false;
                case ItemType.atravitaBush:
                    if (i is not SObject bush)
                        return false;

                    if (this.growableBushesApi is null)
                        return false; // this should be impossible.

                    return this.growableBushesApi.CanPlaceBush(bush, here, v, this.config.LessRestrictiveObjectPlacement);

                case ItemType.Fence:
                    // We want to deal with fences specifically in order to handle fence replacements.
                    if (here.objects.ContainsKey(v))
                    {
                        // We know there's an object at these coordinates, so we grab a reference.
                        var o = here.objects[v];

                        // We first need to determine if it's a fence.
                        if (this.identificationUtils.IsTypeOfObject(o, ItemType.Fence))
                        {
                            // If it is, we only need to continue if the fence replacement setting is on.
                            if (!this.config.EnableReplacingFences)
                            {
                                // And create our notification.
                                this.logger.Log(I18n.SmartBuilding_Message_CheatyOptions_FenceReplacement_Disabled(),
                                    LogLevel.Trace, true);
                                return false;
                            }

                            // If they're the same fences, we return false.
                            if (o.Name.Equals(i.Name))
                                return false;
                            return true;
                        }
                    }
                    else if (here.terrainFeatures.ContainsKey(v))
                    {
                        // There's a terrain feature here, so we want to check if it's a HoeDirt with a crop.
                        var feature = here.terrainFeatures[v];

                        if (feature != null && feature is HoeDirt)
                        {
                            if ((feature as HoeDirt).crop != null)
                                // There's a crop here, so we return false.
                                return false;

                            // At this point, we know it's a HoeDirt, but has no crop, so we can return true.
                            return true;
                        }
                    }

                    goto case ItemType.Generic;
                case ItemType.FishTankFurniture:
                    // Fishtank furniture is locked out until I figure out how to transplant fish correctly.
                    return false;
                case ItemType.StorageFurniture:
                    // Since FishTankFurniture will sneak through here:
                    if (i is FishTankFurniture)
                        return false;

                    // If the setting for allowing storage furniture is off, we get the hell out.
                    if (!this.config.EnablePlacingStorageFurniture && !itemInfo.IsDgaItem)
                    {
                        this.logger.Log(I18n.SmartBuilding_Error_StorageFurniture_SettingIsOff(), LogLevel.Trace, true);

                        return false;
                    }

                    if (this.config.LessRestrictiveFurniturePlacement)
                        return true;
                    return (i as StorageFurniture).canBePlacedHere(here, v);

                    break;
                case ItemType.TvFurniture:
                    if (this.config.LessRestrictiveFurniturePlacement && !itemInfo.IsDgaItem)
                        return true;
                    if (!(i as TV).canBePlacedHere(here, v))
                    {
                        this.logger.Log(I18n.SmartBuilding_Message_CheatyOptions_MoreLaxFurniturePlacement_Disabled(),
                            LogLevel.Trace, true);
                        return false;
                    }

                    return true;
                case ItemType.BedFurniture:
                    if (this.config.LessRestrictiveBedPlacement && !itemInfo.IsDgaItem)
                        return true;
                    if (!(i as BedFurniture).canBePlacedHere(here, v))
                    {
                        this.logger.Log(I18n.SmartBuilding_Message_CheatyOptions_MoreLaxFurniturePlacement_Disabled(),
                            LogLevel.Trace, true);
                        return false;
                    }

                    return true;
                case ItemType.GenericFurniture:
                    // In this place, we play fast and loose, and return true.
                    if (this.config.LessRestrictiveFurniturePlacement && !itemInfo.IsDgaItem)
                        return true;
                    if (!(i as Furniture).canBePlacedHere(here, v))
                    {
                        this.logger.Log(I18n.SmartBuilding_Message_CheatyOptions_MoreLaxFurniturePlacement_Disabled(),
                            LogLevel.Trace, true);
                        return false;
                    }

                    return true;
                case ItemType.Generic:
                    GenericPlaceable
                        : // A goto, I know, gross, but... it works, and is fine for now, until I split out detection logic into methods.

                    if (this.config.LessRestrictiveObjectPlacement)
                    {
                        // If the less restrictive object placement setting is enabled, we first want to check if vanilla logic dictates the object be placeable.
                        if (Game1.currentLocation.isTileLocationOpen(v))
                            // It dictates that it is, so we can simply return true.
                            return true;
                        // Otherwise, we want to check for an object already present in this location.
                        if (!here.Objects.ContainsKey(v))
                            // There is no object here, so we return true, as we should be able to place the object here.
                            return true;
                    }

                    if (Game1.currentLocation.isTileLocationOpen(v))
                        // This is true, so we simply return true.
                        return true;
                    // It's false, so we want to warn that placement would be possible if the correct setting were enabled, and there's no object in the tile.

                    if (!here.objects.ContainsKey(v))
                        this.logger.Log(I18n.SmartBuilding_Message_CheatyOptions_MoreLaxObjectPlacement_Disabled(),
                            LogLevel.Trace, true);

                    return false;
            }

            // If the PlaceableType is somehow none of these, we want to be safe and return false.
            return false;
        }
    }
}
