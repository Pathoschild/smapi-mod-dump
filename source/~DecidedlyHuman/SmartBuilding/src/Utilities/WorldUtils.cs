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
using SmartBuilding.APIs;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

namespace SmartBuilding.Utilities
{
    public class WorldUtils
    {
        private readonly ModConfig config;
        private readonly ITapGiantCropsAPI? giantCropTapApi;
        private readonly IdentificationUtils identificationUtils;
        private readonly Logger logger;
        private readonly IMoreFertilizersAPI? moreFertilizersApi;
        private readonly PlacementUtils placementUtils;
        private readonly PlayerUtils playerUtils;

        public WorldUtils(IdentificationUtils identificationUtils, PlacementUtils placementutils,
                          PlayerUtils playerUtils, ITapGiantCropsAPI giantCropTapApi, ModConfig config, Logger logger,
                          IMoreFertilizersAPI moreFertilizersApi)
        {
            this.identificationUtils = identificationUtils;
            this.placementUtils = placementutils;
            this.playerUtils = playerUtils;
            this.config = config;
            this.logger = logger;
            this.moreFertilizersApi = moreFertilizersApi;
            this.giantCropTapApi = giantCropTapApi;
        }

        /// <summary>
        ///     Determine how to correctly place an item in the world, and place it.
        /// </summary>
        /// <param name="item">
        ///     The <see cref="KeyValuePair" /> containing the <see cref="Vector2" /> tile, and
        ///     <see cref="ItemInfo" /> information about the item to be placed.
        /// </param>
        public void PlaceObject(KeyValuePair<Vector2, ItemInfo> item)
        {
            var itemToPlace = (SObject)item.Value.Item;
            var targetTile = item.Key;
            var itemInfo = item.Value;
            var here = Game1.currentLocation;

            if (itemToPlace != null && this.placementUtils.CanBePlacedHere(targetTile, itemInfo.Item))
            {
                // The item can be placed here.
                if (itemInfo.ItemType == ItemType.Floor)
                {
                    // We're specifically dealing with a floor/path.

                    int? floorType = this.identificationUtils.GetFlooringIdFromName(itemToPlace.Name);
                    Flooring floor;

                    if (floorType.HasValue)
                        floor = new Flooring(floorType.Value);
                    else
                    {
                        // At this point, something is very wrong, so we want to refund the item to the player's inventory, and print an error.
                        this.playerUtils.RefundItem(itemToPlace,
                            I18n.SmartBuilding_Error_TerrainFeature_Flooring_CouldNotIdentifyFloorType(),
                            LogLevel.Error);

                        return;
                    }

                    // At this point, we *need* there to be no TerrainFeature present.
                    if (!here.terrainFeatures.ContainsKey(targetTile))
                        here.terrainFeatures.Add(targetTile, floor);
                    else
                    {
                        // At this point, we know there's a terrain feature here.
                        if (this.config.EnableReplacingFloors)
                        {
                            var tf = here.terrainFeatures[targetTile];

                            if (tf != null && tf is Flooring)
                            {
                                // At this point, we know it's Flooring, so we remove the existing terrain feature, and add our new one.
                                this.DemolishOnTile(targetTile, TileFeature.TerrainFeature);
                                here.terrainFeatures.Add(targetTile, floor);
                            }
                            else
                            {
                                // At this point, there IS a terrain feature here, but it isn't flooring, so we want to refund the item, and return.
                                this.playerUtils.RefundItem(item.Value.Item,
                                    I18n.SmartBuilding_Error_TerrainFeature_Generic_AlreadyPresent(), LogLevel.Error);

                                // We now want to jump straight out of this method, because this will flow through to the below if, and bad things will happen.
                                return;
                            }
                        }
                    }

                    // By this point, we'll have returned false if this could be anything but our freshly placed floor.
                    if (!(here.terrainFeatures.ContainsKey(item.Key) && here.terrainFeatures[item.Key] is Flooring))
                        this.playerUtils.RefundItem(item.Value.Item,
                            I18n.SmartBuilding_Error_TerrainFeature_Generic_UnknownError(), LogLevel.Error);
                }
                else if (itemInfo.ItemType == ItemType.Chest)
                {
                    // We're dealing with a chest.
                    int? chestType = this.identificationUtils.GetChestType(itemToPlace.Name);
                    Chest chest;

                    if (chestType.HasValue)
                        chest = new Chest(true, chestType.Value);
                    else
                    {
                        // At this point, something is very wrong, so we want to refund the item to the player's inventory, and print an error.
                        this.playerUtils.RefundItem(itemToPlace, I18n.SmartBuilding_Error_Chest_CouldNotIdentifyChest(),
                            LogLevel.Error);

                        return;
                    }

                    // We do our second placement possibility check, just in case something was placed in the meantime.
                    if (this.placementUtils.CanBePlacedHere(targetTile, itemToPlace))
                    {
                        bool placed = chest.placementAction(here, (int)targetTile.X * 64, (int)targetTile.Y * 64,
                            Game1.player);

                        // Apparently, chests placed in the world are hardcoded with the name "Chest".
                        if (!here.objects.ContainsKey(targetTile) || !here.objects[targetTile].Name.Equals("Chest"))
                            this.playerUtils.RefundItem(itemToPlace, I18n.SmartBuilding_Error_Object_PlacementFailed(),
                                LogLevel.Error);
                    }
                }
                else if (itemInfo.ItemType == ItemType.Fence)
                {
                    // We want to check to see if the target tile contains an object.
                    if (here.objects.ContainsKey(targetTile))
                    {
                        var o = here.objects[targetTile];

                        if (o != null)
                        {
                            // We try to identify what kind of object is placed here.
                            if (this.identificationUtils.IsTypeOfObject(o, ItemType.Fence))
                            {
                                if (this.config.EnableReplacingFences)
                                    // We have a fence, so we want to remove it before placing our new one.
                                    this.DemolishOnTile(targetTile, TileFeature.Object);
                            }
                            else
                            {
                                // If it isn't a fence, we want to refund the item, and return to avoid placing the fence.
                                this.playerUtils.RefundItem(item.Value.Item,
                                    I18n.SmartBuilding_Error_Object_PlacementFailed(), LogLevel.Error);
                                return;
                            }
                        }
                    }

                    if (!itemToPlace.placementAction(Game1.currentLocation, (int)item.Key.X * 64, (int)item.Key.Y * 64,
                            Game1.player))
                        this.playerUtils.RefundItem(item.Value.Item, I18n.SmartBuilding_Error_Object_PlacementFailed(),
                            LogLevel.Error);
                }
                else if (itemInfo.ItemType == ItemType.GrassStarter)
                {
                    var grassStarter = new Grass(1, 4);

                    // At this point, we *need* there to be no TerrainFeature present.
                    if (!here.terrainFeatures.ContainsKey(targetTile))
                        here.terrainFeatures.Add(targetTile, grassStarter);
                    else
                    {
                        this.playerUtils.RefundItem(item.Value.Item,
                            I18n.SmartBuilding_Error_TerrainFeature_Generic_AlreadyPresent(), LogLevel.Error);

                        // We now want to jump straight out of this method, because this will flow through to the below if, and bad things may happen.
                        return;
                    }

                    if (!(here.terrainFeatures.ContainsKey(item.Key) && here.terrainFeatures[targetTile] is Grass))
                        this.playerUtils.RefundItem(item.Value.Item,
                            I18n.SmartBuilding_Error_TerrainFeature_Generic_AlreadyPresent(), LogLevel.Error);
                }
                else if (itemInfo.ItemType == ItemType.CrabPot)
                {
                    var pot = new CrabPot(targetTile);

                    if (this.placementUtils.CanBePlacedHere(targetTile, itemToPlace))
                        itemToPlace.placementAction(Game1.currentLocation, (int)targetTile.X * 64,
                            (int)targetTile.Y * 64, Game1.player);
                }
                else if (itemInfo.ItemType == ItemType.Seed)
                {
                    // Here, we're dealing with a seed, so we need very special logic for this.
                    // Item.placementAction for seeds is semi-broken, unless the player is currently
                    // holding the specific seed being planted.

                    bool successfullyPlaced = false;

                    // First, we check for a TerrainFeature.
                    if (Game1.currentLocation.terrainFeatures.ContainsKey(targetTile))
                        // Then, we check to see if it's a HoeDirt.
                        if (Game1.currentLocation.terrainFeatures[targetTile] is HoeDirt)
                        {
                            // If it is, we grab a reference to it.
                            var hd = (HoeDirt)Game1.currentLocation.terrainFeatures[targetTile];

                            // We check to see if it can be planted, and act appropriately.
                            if (this.placementUtils.CanBePlacedHere(targetTile, itemToPlace))
                            {
                                if (itemInfo.IsDgaItem)
                                    successfullyPlaced = itemToPlace.placementAction(here, (int)targetTile.X * 64,
                                        (int)targetTile.Y * 64, Game1.player);
                                else
                                    successfullyPlaced = hd.plant(itemToPlace.ParentSheetIndex, (int)targetTile.X,
                                        (int)targetTile.Y, Game1.player, false, Game1.currentLocation);
                            }
                        }

                    // If the planting failed, we refund the seed.
                    if (!successfullyPlaced)
                        this.playerUtils.RefundItem(item.Value.Item, I18n.SmartBuilding_Error_Seeds_PlacementFailed(),
                            LogLevel.Error);
                }
                else if (itemInfo.ItemType == ItemType.Fertilizer)
                {
                    // First, we get whether or not More Fertilizers can place this fertiliser.
                    if (this.moreFertilizersApi?.CanPlaceFertilizer(itemToPlace, here, targetTile) == true)
                    {
                        // If it can, we try to place it.
                        if (this.moreFertilizersApi.TryPlaceFertilizer(itemToPlace, here, targetTile))
                            // If the placement is successful, we do the fancy animation thing.
                            this.moreFertilizersApi.AnimateFertilizer(itemToPlace, here, targetTile);
                        else
                            // Otherwise, the fertiliser gets refunded.
                            this.playerUtils.RefundItem(itemToPlace,
                                $"{I18n.SmartBuilding_Integrations_MoreFertilizers_InvalidFertiliserPosition()}: {itemToPlace.Name} @ {targetTile}",
                                LogLevel.Debug, true);
                    }

                    if (here.terrainFeatures.ContainsKey(targetTile))
                        // We know there's a TerrainFeature here, so next we want to check if it's HoeDirt.
                        if (here.terrainFeatures[targetTile] is HoeDirt)
                        {
                            // If it is, we want to grab the HoeDirt, check if it's already got a fertiliser, and fertilise if not.
                            var hd = (HoeDirt)here.terrainFeatures[targetTile];

                            // 0 here means no fertilizer. This is a known change in 1.6.
                            if (hd.fertilizer.Value == 0)
                            {
                                // Next, we want to check if there's already a crop here.
                                if (hd.crop != null)
                                {
                                    var cropToCheck = hd.crop;

                                    if (cropToCheck.currentPhase.Value == 0)
                                        // If the current crop phase is zero, we can plant the fertilizer here.
                                        hd.plant(itemToPlace.ParentSheetIndex, (int)targetTile.X, (int)targetTile.Y,
                                            Game1.player, true, Game1.currentLocation);
                                }
                                else
                                    // If there is no crop here, we can plant the fertilizer with reckless abandon.
                                    hd.plant(itemToPlace.ParentSheetIndex, (int)targetTile.X, (int)targetTile.Y,
                                        Game1.player, true, Game1.currentLocation);
                            }
                            else
                                // If there is already a fertilizer here, we want to refund the item.
                                this.playerUtils.RefundItem(itemToPlace,
                                    I18n.SmartBuilding_Error_Fertiliser_AlreadyFertilised(), LogLevel.Warn);

                            // Now, we want to run the final check to see if the fertilization was successful.
                            if (hd.fertilizer.Value == 0)
                                // If there's still no fertilizer here, we need to refund the item.
                                this.playerUtils.RefundItem(itemToPlace,
                                    I18n.SmartBuilding_Error_Fertiliser_IneligibleForFertilisation(), LogLevel.Warn);
                        }
                }
                else if (itemInfo.ItemType == ItemType.TreeFertilizer)
                {
                    if (here.terrainFeatures.ContainsKey(targetTile))
                        // If there's a TerrainFeature here, we check if it's a tree.
                        if (here.terrainFeatures[targetTile] is Tree)
                        {
                            // It is a tree, so now we check to see if the tree is fertilised.
                            var tree = (Tree)here.terrainFeatures[targetTile];

                            // If it's already fertilised, there's no need for us to want to place tree fertiliser on it.
                            if (!tree.fertilized.Value)
                                tree.fertilize(here);
                        }
                }
                else if (itemInfo.ItemType == ItemType.Tapper)
                {
                    if (this.placementUtils.CanBePlacedHere(targetTile, itemToPlace))
                    {
                        // If there's a TerrainFeature here, we need to know if it's a tree.
                        if (here.terrainFeatures.ContainsKey(targetTile) && here.terrainFeatures[targetTile] is Tree)
                        {
                            // If it is, we grab a reference, and check for a tapper on it already.
                            var tree = (Tree)here.terrainFeatures[targetTile];

                            if (!tree.tapped.Value)
                                if (!itemToPlace.placementAction(here, (int)targetTile.X * 64, (int)targetTile.Y * 64,
                                        Game1.player))
                                    // If the placement action didn't succeed, we refund the item.
                                    this.playerUtils.RefundItem(itemToPlace,
                                        I18n.SmartBuilding_Error_TreeTapper_PlacementFailed(), LogLevel.Error);

                            // And return to avoid triggering the giant crop check.
                            return;
                        }

                        // Now we check for a giant crop.
                        foreach (var clump in here.resourceClumps)
                        {
                            if (clump is GiantCrop && clump.occupiesTile((int)targetTile.X, (int)targetTile.Y))
                            {
                                // It's a giant crop, so we defer to Tap Giant Crops's placement.
                                if (!this.giantCropTapApi.TryPlaceTapper(here, targetTile, itemToPlace))
                                {
                                    // If the placement action didn't succeed, we refund the item.
                                    this.playerUtils.RefundItem(itemToPlace,
                                        I18n.SmartBuilding_Error_TreeTapper_PlacementFailed(), LogLevel.Error);
                                }
                            }
                        }
                    }
                }
                else if (itemInfo.ItemType == ItemType.FishTankFurniture)
                {
                    // This cannot be reached, because placement of fish tanks is blocked for now.

                    // // We're dealing with a fish tank. This has dangerous consequences.
                    // if (config.LessRestrictiveFurniturePlacement)
                    // {
                    //     FishTankFurniture tank = new FishTankFurniture(itemToPlace.ParentSheetIndex, targetTile);
                    //
                    //     foreach (var fish in (itemToPlace as FishTankFurniture).tankFish)
                    //     {
                    //         tank.tankFish.Add(fish);
                    //     }
                    //
                    //     foreach (var fish in tank.tankFish)
                    //     {
                    //         fish.ConstrainToTank();
                    //     }
                    //
                    //     here.furniture.Add(tank);
                    // }
                    // else
                    // {
                    //     (itemToPlace as FishTankFurniture).placementAction(here, (int)targetTile.X, (int)targetTile.Y, Game1.player);
                    // }
                }
                else if (itemInfo.ItemType == ItemType.StorageFurniture)
                {
                    if (this.config.EnablePlacingStorageFurniture && !itemInfo.IsDgaItem)
                    {
                        bool placedSuccessfully = false;

                        // We need to create a new instance of StorageFurniture.
                        var storage = new StorageFurniture(itemToPlace.ParentSheetIndex, targetTile);

                        // A quick bool to avoid an unnecessary log to console later.
                        bool anyItemsAdded = false;

                        // Then, we iterate through all of the items in the existing StorageFurniture, and add them to the new one.
                        foreach (var itemInStorage in (itemToPlace as StorageFurniture).heldItems)
                        {
                            this.logger.Log(
                                $"{I18n.SmartBuilding_Message_StorageFurniture_AddingItem()} {itemInStorage.Name} ({itemInStorage.ParentSheetIndex}).");
                            storage.AddItem(itemInStorage);

                            anyItemsAdded = true;
                        }

                        // If any items were added, inform the user of the purpose of logging them.
                        if (anyItemsAdded)
                            this.logger.Log(I18n.SmartBuilding_Message_StorageFurniture_RetrievalTip());

                        // If we have less restrictive furniture placement enabled, we simply try to place it. Otherwise, we use the vanilla placementAction.
                        if (this.config.LessRestrictiveFurniturePlacement)
                            here.furniture.Add(storage);
                        else
                            placedSuccessfully = storage.placementAction(here, (int)targetTile.X * 64,
                                (int)targetTile.Y * 64, Game1.player);

                        // Here, we check to see if the placement was successful. If not, we refund the item.
                        if (!here.furniture.Contains(storage) && !placedSuccessfully)
                            this.playerUtils.RefundItem(storage,
                                I18n.SmartBuilding_Error_StorageFurniture_PlacementFailed(), LogLevel.Error);
                    }
                    else
                        this.playerUtils.RefundItem(itemToPlace,
                            I18n.SmartBuilding_Error_StorageFurniture_SettingIsOff(), LogLevel.Info, true);
                }
                else if (itemInfo.ItemType == ItemType.TvFurniture)
                {
                    bool placedSuccessfully = false;
                    TV tv = null;

                    // We need to determine which we we're placing this TV based upon the furniture placement restriction option.
                    if (this.config.LessRestrictiveFurniturePlacement && !itemInfo.IsDgaItem)
                    {
                        tv = new TV(itemToPlace.ParentSheetIndex, targetTile);
                        here.furniture.Add(tv);
                    }
                    else
                        placedSuccessfully = (itemToPlace as TV).placementAction(here, (int)targetTile.X * 64,
                            (int)targetTile.Y * 64, Game1.player);

                    // If both of these are false, the furniture was not successfully placed, so we need to refund the item.
                    if (tv != null && !here.furniture.Contains(tv) && !placedSuccessfully)
                        this.playerUtils.RefundItem(itemToPlace,
                            I18n.SmartBuilding_Error_TvFurniture_PlacementFailed(), LogLevel.Error);
                }
                else if (itemInfo.ItemType == ItemType.BedFurniture)
                {
                    bool placedSuccessfully = false;
                    BedFurniture bed = null;

                    // We decide exactly how we're placing the furniture based upon the less restrictive setting.
                    if (this.config.LessRestrictiveBedPlacement && !itemInfo.IsDgaItem)
                    {
                        bed = new BedFurniture(itemToPlace.ParentSheetIndex, targetTile);
                        here.furniture.Add(bed);
                    }
                    else
                        placedSuccessfully = (itemToPlace as BedFurniture).placementAction(here,
                            (int)targetTile.X * 64, (int)targetTile.Y * 64, Game1.player);

                    // If both of these are false, the furniture was not successfully placed, so we need to refund the item.
                    if (bed != null && !here.furniture.Contains(bed) && !placedSuccessfully)
                        this.playerUtils.RefundItem(itemToPlace,
                            I18n.SmartBuilding_Error_BedFurniture_PlacementFailed(), LogLevel.Error);
                }
                else if (itemInfo.ItemType == ItemType.GenericFurniture)
                {
                    bool placedSuccessfully = false;
                    Furniture furniture = null;

                    // Determine exactly how we're placing this furniture.
                    if (this.config.LessRestrictiveFurniturePlacement && !itemInfo.IsDgaItem)
                    {
                        furniture = new Furniture(itemToPlace.ParentSheetIndex, targetTile);
                        here.furniture.Add(furniture);
                    }
                    else
                        placedSuccessfully = (itemToPlace as Furniture).placementAction(here,
                            (int)targetTile.X * 64, (int)targetTile.Y * 64, Game1.player);

                    // If both of these are false, the furniture was not successfully placed, so we need to refund the item.
                    if (furniture != null && !here.furniture.Contains(furniture) && !placedSuccessfully)
                        this.playerUtils.RefundItem(itemToPlace,
                            I18n.SmartBuilding_Error_Furniture_PlacementFailed(), LogLevel.Error);
                }
                else if (itemInfo.ItemType == ItemType.Torch)
                {
                    // We need to figure out whether there's a fence in the placement tile.
                    if (here.objects.ContainsKey(targetTile))
                    {
                        // We know there's an object at these coordinates, so we grab a reference.
                        var o = here.objects[targetTile];

                        if (this.identificationUtils.IsTypeOfObject(o, ItemType.Fence))
                        {
                            // If the object in this tile is a fence, we add the torch to it.
                            //itemToPlace.placementAction(Game1.currentLocation, (int)item.Key.X * 64, (int)item.Key.Y * 64, Game1.player);

                            // We know it's a fence by type, but we need to make sure it isn't a gate, and to ensure it isn't already "holding" anything.
                            if (o.Name.Equals("Gate") && o.heldObject != null)
                                // There's something in there, so we need to refund the torch.
                                this.playerUtils.RefundItem(item.Value.Item,
                                    I18n.SmartBuilding_Error_Torch_PlacementInFenceFailed(), LogLevel.Error);

                            o.performObjectDropInAction(itemToPlace, false, Game1.player);

                            if (this.identificationUtils.IdentifyItemType(o.heldObject) != ItemType.Torch)
                                // If the fence isn't "holding" a torch, there was a problem, so we should refund.
                                this.playerUtils.RefundItem(item.Value.Item,
                                    I18n.SmartBuilding_Error_Torch_PlacementInFenceFailed(), LogLevel.Error);

                            return;
                        }

                        // If it's not a fence, we want to refund the item.
                        this.playerUtils.RefundItem(item.Value.Item,
                            I18n.SmartBuilding_Error_Object_PlacementFailed(), LogLevel.Error);

                        return;
                    }

                    // There is no object here, so we treat it like a generic placeable.
                    if (!itemToPlace.placementAction(Game1.currentLocation, (int)item.Key.X * 64,
                            (int)item.Key.Y * 64, Game1.player))
                        this.playerUtils.RefundItem(item.Value.Item,
                            I18n.SmartBuilding_Error_Object_PlacementFailed(), LogLevel.Error);
                }
                else
                {
                    // We're dealing with a generic placeable.
                    bool successfullyPlaced = itemToPlace.placementAction(Game1.currentLocation,
                        (int)item.Key.X * 64, (int)item.Key.Y * 64, Game1.player);

                    // if (Game1.currentLocation.objects.ContainsKey(item.Key) && Game1.currentLocation.objects[item.Key].Name.Equals(itemToPlace.Name))
                    if (!successfullyPlaced)
                        this.playerUtils.RefundItem(item.Value.Item,
                            I18n.SmartBuilding_Error_Object_PlacementFailed(), LogLevel.Error);
                }

            }
            else
            {
                this.playerUtils.RefundItem(item.Value.Item, I18n.SmartBuilding_Error_Object_PlacementFailed(), LogLevel.Error);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="tile">
        ///     The tile we want to demolish the <see cref="StardewValley.Object" /> or
        ///     <see cref="StardewValley.TerrainFeature" /> on.
        /// </param>
        /// <param name="feature">Which type of <see cref="TileFeature" /> we're dealing with.</param>
        public void DemolishOnTile(Vector2 tile, TileFeature feature)
        {
            var here = Game1.currentLocation;
            var playerTile = Game1.player.getTileLocation();
            Item itemToDestroy;
            ItemType type;

            // We're working with an SObject in this specific instance.
            if (feature == TileFeature.Object)
                if (here.objects.ContainsKey(tile))
                {
                    var o = here.objects[tile];

                    // Firstly, we need to determine if the object has a category of zero, and a name of Chest.
                    if (o.Category == 0 && o.Name.Equals("Chest"))
                        return; // It does, so we return immediately.

                    // Then we want to determine if the object contains modData from Market Day, because we don't want to affect their chests.
                    if (this.identificationUtils.DoesObjectContainModData(o, "ceruleandeep.MarketDay"))
                        return;

                    // We have an object in this tile, so we want to try to figure out what it is.
                    itemToDestroy = Utility.fuzzyItemSearch(o.Name);

                    type = this.identificationUtils.IdentifyItemType((SObject)itemToDestroy);

                    if (type == ItemType.NotPlaceable)
                    {
                        // If we're here, this means this is a specifically blacklisted item, and so we simply do nothing.
                    }
                    else if (type == ItemType.Chest) // Chests need special handling because they can store items.
                    {
                        // We're double checking at this point for safety. I want to be extra careful with chests.
                        if (here.objects.ContainsKey(tile))
                        {
                            // If the setting to disable chest pickup is enabled, we pick up the chest. If not, we do nothing.
                            if (this.config.CanDestroyChests)
                            {
                                // This is fairly fragile, but it's fine with vanilla chests, at least.
                                var chest = new Chest(o.ParentSheetIndex, tile, 0, 1);

                                (o as Chest).destroyAndDropContents(tile * 64, here);
                                Game1.player.addItemByMenuIfNecessary(chest.getOne());
                                here.objects.Remove(tile);
                            }
                            else
                                this.logger.Log(I18n.SmartBuilding_Message_CheatyOptions_CanPickUpChests_Disabled(),
                                    LogLevel.Trace, true);
                        }
                    }
                    else if (o is Chest)
                    {
                        // We're double checking at this point for safety. I want to be extra careful with chests.
                        if (here.objects.ContainsKey(tile))
                        {
                            // If the setting to disable chest pickup is enabled, we pick up the chest. If not, we do nothing.
                            if (this.config.CanDestroyChests)
                            {
                                // This is fairly fragile, but it's fine with vanilla chests, at least.
                                var chest = new Chest(o.ParentSheetIndex, tile, 0, 1);

                                (o as Chest).destroyAndDropContents(tile * 64, here);
                                Game1.player.addItemByMenuIfNecessary(chest.getOne());
                                here.objects.Remove(tile);
                            }
                            else
                                this.logger.Log(I18n.SmartBuilding_Message_CheatyOptions_CanPickUpChests_Disabled(),
                                    LogLevel.Trace, true);
                        }
                    }
                    else if (type == ItemType.Fence)
                    {
                        // We need special handling for fences, since we don't want to pick them up if their health has deteriorated too much.
                        var fenceToRemove = (Fence)o;

                        // We also need to check to see if the fence has a torch on it so we can remove the light source.
                        if (o.heldObject.Value != null)
                        {
                            // There's an item there, so we can relatively safely assume it's a torch.
                            // We remove its light source from the location, and refund the torch.
                            here.removeLightSource(o.heldObject.Value.lightSource.identifier);

                            this.playerUtils.RefundItem(o.heldObject, "No error. Do not log.");
                        }

                        fenceToRemove.performRemoveAction(tile * 64, here);
                        here.objects.Remove(tile);

                        // And, if the fence had enough health remaining, we refund it.
                        if (fenceToRemove.maxHealth.Value - fenceToRemove.health.Value < 0.5f)
                            Game1.player.addItemByMenuIfNecessary(fenceToRemove.getOne());
                    }
                    else if (type == ItemType.Tapper)
                    {
                        // Tappers need special handling to mark the tree as untapped, otherwise they can't be chopped down with an axe.
                        if (here.terrainFeatures.ContainsKey(tile))
                        {
                            // We've confirmed there's a TerrainFeature here, so next we grab a reference to it if it is a tree.
                            if (here.terrainFeatures[tile] is Tree treeToUntap)
                                // After double checking there's a tree here, we grab a reference to it.
                                treeToUntap.tapped.Value = false;

                            o.performRemoveAction(tile * 64, here);
                            Game1.player.addItemByMenuIfNecessary(o.getOne());

                            here.objects.Remove(tile);
                        }
                    }
                    else
                    {
                        if (o.Fragility == 2)
                            // A fragility of 2 means the item should not be broken, or able to be picked up, like incubators in coops, so we return.
                            return;

                        // Now we need to figure out whether the object has a heldItem within it.
                        if (o.heldObject != null)
                            // There's an item inside here, so we need to determine whether to refund the item, or discard it if it's a chest.
                            if (o.heldObject.Value is Chest)
                                // It's a chest, so we want to force it to drop all of its items.
                                if ((o.heldObject.Value as Chest).items.Count > 0)
                                    (o.heldObject.Value as Chest).destroyAndDropContents(tile * 64, here);

                        o.performRemoveAction(tile * 64, here);
                        Game1.player.addItemByMenuIfNecessary(o.getOne());

                        here.objects.Remove(tile);
                    }

                    return;
                }

            // We're working with a TerrainFeature.
            if (feature == TileFeature.TerrainFeature)
                if (here.terrainFeatures.ContainsKey(tile))
                {
                    var tf = here.terrainFeatures[tile];

                    // We want to determine if the terrain feature contains modData from Market Day, because we don't want to affect their items.
                    if (this.identificationUtils.DoesTerrainFeatureContainModData(tf, "ceruleandeep.MarketDay"))
                        return;

                    // We only really want to be handling flooring when removing TerrainFeatures.
                    if (tf is Flooring)
                    {
                        var floor = (Flooring)tf;

                        int? floorType = floor.whichFloor.Value;
                        string? floorName = this.identificationUtils.GetFlooringNameFromId(floorType.Value);
                        SObject finalFloor;

                        if (floorType.HasValue)
                        {
                            floorName = this.identificationUtils.GetFlooringNameFromId(floorType.Value);
                            finalFloor = (SObject)Utility.fuzzyItemSearch(floorName);
                        }
                        else
                            finalFloor = null;

                        if (finalFloor != null)
                            Game1.player.addItemByMenuIfNecessary(finalFloor);

                        // Game1.createItemDebris(finalFloor, playerTile * 64, 1, here);
                        here.terrainFeatures.Remove(tile);
                    }
                }

            if (feature == TileFeature.Furniture)
            {
                Furniture furnitureToGrab = null;

                foreach (var f in here.furniture)
                    if (f.boundingBox.Value.Intersects(new Rectangle((int)tile.X * 64, (int)tile.Y * 64, 1, 1)))
                        furnitureToGrab = f;

                if (furnitureToGrab != null)
                {
                    // If it's a StorageFurniture, and the setting to allow working with it is false, do nothing.
                    if (furnitureToGrab is StorageFurniture && !this.config.EnablePlacingStorageFurniture)
                        return;

                    // Otherwise, we can continue.
                    this.logger.Log($"{I18n.SmartBuilding_Message_TryingToGrab()} {furnitureToGrab.Name}");
                    Game1.player.addItemToInventory(furnitureToGrab);
                    here.furniture.Remove(furnitureToGrab);
                }
            }
        }

        public void InsertItem(Item item, SObject o, bool shouldManuallyDeduct)
        {
            // For some reason, apparently, we always need to deduct the held item by one, even if we're working with a producer which does it by itself.

            // First, we perform the drop in action.
            bool successfullyInserted = o.performObjectDropInAction(item, false, Game1.player);

            // Then, perplexingly, we still need to manually deduct the item by one, or we can end up with an item that has a stack size of zero.
            if (successfullyInserted) Game1.player.reduceActiveItemByOne();
        }

        public void ShoveGemIntoTorch(Item item, SObject o, Vector2 targetTile)
        {
            // If the gem isn't in our list, we nope out of it.
            if (!this.identificationUtils.IsValidPrismaticFireGem(item))
                return;

            // First, we want to check to see if we're dealing with a fence.
            var type = this.identificationUtils.IdentifyItemType(o);

            if (type == ItemType.Fence)
            {
                // It's a fence, so we want to grab a reference to the torch inside it, if any.
                if (o.heldObject.Value != null && o.heldObject.Value is Torch)
                {
                    // It's a torch, so we grab a reference to it.
                    var torch = (Torch)o.heldObject;

                    // Now we check to see if the torch is already coloured.
                    if (torch.modData.ContainsKey("aedenthorn.PrismaticFire") &&
                        torch.modData["aedenthorn.PrismaticFire"].Equals(Game1.player.ActiveObject.Name))
                        // This colour gem is already inserted, so we do nothing.
                        return;
                    // Otherwise, we insert our gem.
                    torch.modData["aedenthorn.PrismaticFire"] = Game1.player.ActiveObject.Name;
                    Game1.player.reduceActiveItemByOne();
                }
            }
            else
            {
                // It isn't a fence, so we simply try to grab a reference to the torch.
                if (o is Torch torch)
                {
                    // We've got the torch, so we check its modData.
                    // Now we check to see if the torch is already coloured.
                    if (torch.modData.ContainsKey("aedenthorn.PrismaticFire") &&
                        torch.modData["aedenthorn.PrismaticFire"].Equals(Game1.player.ActiveObject.Name))
                        // This colour gem is already inserted, so we do nothing.
                        return;
                    // Otherwise, we insert our gem.
                    torch.modData["aedenthorn.PrismaticFire"] = Game1.player.ActiveObject.Name;
                    Game1.player.reduceActiveItemByOne();
                }
            }
        }
    }
}
