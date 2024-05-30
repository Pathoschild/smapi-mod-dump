/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Exblosis/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Extensions;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

namespace LetsMoveIt.TargetData
{
    internal partial class Target
    {
        /// <summary>Move the current target.</summary>
        /// <param name="location">The current location.</param>
        /// <param name="tile">The current tile position.</param>
        /// <param name="overwriteTile">To Overwrite existing Object.</param>
        public static void CopyTo(GameLocation location, Vector2 tile, bool overwriteTile)
        {
            if (!Config.ModEnabled)
            {
                TargetObject = null;
                return;
            }
            if (TargetObject is null)
                return;

            if (TargetObject is Farmer)
            {
                Game1.playSound("Duck");
                return;
            }
            else if (TargetObject is NPC)
            {
                Game1.addHUDMessage(new(I18n.Message("NotImplemented"), 2));
                Game1.playSound("cancel");
                return;
            }
            else if (TargetObject is FarmAnimal farmAnimal)
            {
                if (farmAnimal.home is null)
                {
                    Game1.playSound("cancel");
                    return;
                }
                if (farmAnimal.home.GetIndoors() is AnimalHouse animalHouse)
                {
                    if (animalHouse.isFull())
                    {
                        Game1.addHUDMessage(new(I18n.Message("BuildingIsFull"), 3));
                        Game1.playSound("cancel");
                        return;
                    }
                    else
                    {
                        FarmAnimal farmAnimalCopy = new(farmAnimal.type.Value, Game1.Multiplayer.getNewID(), Game1.player.UniqueMultiplayerID);
                        if (farmAnimal.isAdult())
                            farmAnimalCopy.growFully();
                        farmAnimalCopy.Name = Dialogue.randomName();
                        farmAnimalCopy.displayName = farmAnimalCopy.Name;
                        animalHouse.adoptAnimal(farmAnimalCopy);
                        location.animals.TryAdd(farmAnimalCopy.myID.Value, farmAnimalCopy);
                        farmAnimalCopy.Position = (Game1.getMousePosition() + new Point(Game1.viewport.Location.X - 32, Game1.viewport.Location.Y - 32)).ToVector2();
                        farmAnimalCopy.makeSound();
                    }
                }
            }
            else if (TargetObject is SObject sObject)
            {
                if (location.objects.ContainsKey(tile))
                {
                    location.objects.Remove(tile);
                }
                if (TargetObject is BreakableContainer)
                {
                    BreakableContainer breakableContainerCopy;
                    if (sObject.ItemId == "174")
                    {
                        breakableContainerCopy = BreakableContainer.GetBarrelForVolcanoDungeon(tile);
                    }
                    else
                    {
                        breakableContainerCopy = new(tile, sObject.ItemId);
                        breakableContainerCopy.showNextIndex.Value = Game1.random.NextBool();
                    }
                    location.objects.Add(tile, breakableContainerCopy);
                    location.playSound("axe", tile);
                }
                else
                {
                    SObject sObjectCopy;
                    if (sObject.bigCraftable.Value)
                    {
                        sObjectCopy = new(tile, sObject.ItemId, sObject.IsRecipe);
                    }
                    else
                    {
                        sObjectCopy = new(sObject.ItemId, sObject.Stack, sObject.IsRecipe, sObject.Price, sObject.Quality);
                    }
                    if (sObject.isPlaceable())
                    {
                        sObjectCopy.placementAction(location, (int)tile.X * 64, (int)tile.Y * 64, Game1.player);
                    }
                    else
                    {
                        sObjectCopy.MinutesUntilReady = sObject.MinutesUntilReady;
                        sObjectCopy.IsSpawnedObject = sObject.IsSpawnedObject;
                        location.objects.Add(tile, sObjectCopy);
                        location.playSound("woodyStep", tile);
                    }
                }
            }
            else if (TargetObject is ResourceClump resourceClump)
            {
                if (TargetObject is GiantCrop giantCrop)
                {
                    GiantCrop giantCropCopy = new(giantCrop.Id, tile);
                    location.resourceClumps.Add(giantCropCopy);
                    location.playSound("axe", tile);
                }
                else
                {
                    ResourceClump resourceClumpCopy = new(resourceClump.parentSheetIndex.Value, resourceClump.width.Value, resourceClump.height.Value, tile, null, resourceClump.textureName.Value);
                    location.resourceClumps.Add(resourceClumpCopy);
                    location.playSound("axe", tile);
                }
            }
            else if (TargetObject is TerrainFeature)
            {
                if (TargetObject is Bush bush && bush.size.Value == 3)
                {
                    if (location.objects.TryGetValue(tile, out var obj))
                    {
                        if (obj is IndoorPot pot)
                        {
                            if (pot.bush.Value is not null || pot.hoeDirt.Value.crop is not null)
                            {
                                Game1.playSound("cancel");
                                return;
                            }
                        }
                    }
                    if (location.objects.TryGetValue(tile, out var obj2) && obj2 is IndoorPot pot2)
                    {
                        Bush bushCopy = new(tile, bush.size.Value, location, bush.datePlanted.Value);
                        bushCopy.inPot.Value = true;
                        pot2.bush.Value = bushCopy;
                        location.playSound("leafrustle", tile);
                    }
                    else
                    {
                        if (location.terrainFeatures.ContainsKey(tile))
                        {
                            location.terrainFeatures.Remove(tile);
                        }
                        Bush bushCopy = new(tile, bush.size.Value, location, bush.datePlanted.Value);
                        location.terrainFeatures.Add(tile, bushCopy);
                        location.playSound("leafrustle", tile);
                    }
                }
                else if (TargetObject is LargeTerrainFeature)
                {
                    if (TargetObject is Bush bush1)
                    {
                        Bush bushCopy = new(tile, bush1.size.Value, location, bush1.datePlanted.Value);
                        bushCopy.townBush.Value = bush1.townBush.Value;
                        bushCopy.tileSheetOffset.Value = bush1.tileSheetOffset.Value;
                        bushCopy.setUpSourceRect();
                        location.largeTerrainFeatures.Add(bushCopy);
                        location.playSound("leafrustle", tile);
                    }
                }
                else
                {
                    if (location.terrainFeatures.ContainsKey(tile))
                    {
                        location.terrainFeatures.Remove(tile);
                    }
                    if (TargetObject is Flooring flooring)
                    {
                        Flooring flooringCopy = new(flooring.whichFloor.Value);
                        location.terrainFeatures.Add(tile, flooringCopy);
                        location.playSound(flooring.GetData().PlacementSound, tile);
                    }
                    else if (TargetObject is HoeDirt hoeDirt)
                    {
                        HoeDirt hoeDirtCopy = new(hoeDirt.state.Value);
                        location.terrainFeatures.Add(tile, hoeDirtCopy);
                        location.playSound("hoeHit", tile);
                    }
                    else if (TargetObject is Grass grass)
                    {
                        Grass grassCopy = new(grass.grassType.Value, grass.numberOfWeeds.Value);
                        location.terrainFeatures.Add(tile, grassCopy);
                        location.playSound("grassyStep", tile);
                    }
                    else if (TargetObject is FruitTree fruitTree)
                    {
                        FruitTree fruitTreeCopy = new(fruitTree.treeId.Value, fruitTree.growthStage.Value);
                        location.terrainFeatures.Add(tile, fruitTreeCopy);
                        location.playSound("leafrustle", tile);
                    }
                    else if (TargetObject is Tree tree)
                    {
                        Tree treeCopy = new(tree.treeType.Value, tree.growthStage.Value, tree.isTemporaryGreenRainTree.Value);
                        location.terrainFeatures.Add(tile, treeCopy);
                        location.playSound("leafrustle", tile);
                    }
                }
            }
            else if (TargetObject is Crop crop)
            {
                if (location.isCropAtTile((int)tile.X, (int)tile.Y) || !location.isTileHoeDirt(tile))
                {
                    Game1.playSound("cancel");
                    return;
                }
                if (location.objects.TryGetValue(tile, out var isPot))
                {
                    if (isPot is IndoorPot pot && pot.hoeDirt.Value.crop is not null)
                    {
                        Game1.playSound("cancel");
                        return;
                    }
                }
                Crop cropCopy;
                if (crop.forageCrop.Value)
                {
                    cropCopy = new(crop.forageCrop.Value, crop.whichForageCrop.Value, (int)tile.X, (int)tile.Y, location);
                }
                else
                {
                    cropCopy = new(crop.netSeedIndex.Value, (int)tile.X, (int)tile.Y, location);
                }
                cropCopy.currentPhase.Value = crop.currentPhase.Value;
                cropCopy.dayOfCurrentPhase.Value = crop.dayOfCurrentPhase.Value;
                cropCopy.phaseToShow.Value = crop.phaseToShow.Value;
                cropCopy.fullyGrown.Value = crop.fullyGrown.Value;
                cropCopy.phaseDays.Set(crop.phaseDays);
                if (location.objects.TryGetValue(tile, out var newPot))
                {
                    if (newPot is IndoorPot pot)
                    {
                        pot.hoeDirt.Value.crop = cropCopy;
                        pot.hoeDirt.Value.crop.updateDrawMath(tile);
                    }
                }
                else if (location.terrainFeatures.TryGetValue(tile, out var newHoeDirt))
                {
                    if (newHoeDirt is HoeDirt hoeDirt)
                    {
                        hoeDirt.crop = cropCopy;
                        hoeDirt.crop.updateDrawMath(tile);
                    }
                }
                location.playSound("dirtyHit", tile);
            }
            else if (TargetObject is Building)
            {
                Game1.addHUDMessage(new(I18n.Message("NotImplemented"), 2));
                Game1.playSound("cancel");
                return;
            }
        }
    }
}
