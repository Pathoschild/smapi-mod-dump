/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Exblosis/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

namespace LetsMoveIt.TargetData
{
    internal partial class Target
    {
        /// <summary>Remove the current target.</summary>
        public void Remove()
        {
            if (!Config.ModEnabled)
            {
                TargetObject = null;
                return;
            }
            if (TargetObject is null)
                return;

            if (TargetObject is Farmer farmer)
            {
                //farmer.health = 0;
                farmer.faceDirection(2);
                farmer.FarmerSprite.animateOnce(234, 500f, 1);
                farmer.performPlayerEmote("angry");
                Game1.playSound("batFlap");
                TargetObject = null;
                return;
            }
            else if (TargetObject is NPC character)
            {
                TargetLocation.characters.Remove(character);
                TargetObject = null;
            }
            else if (TargetObject is FarmAnimal farmAnimal)
            {
                if (MarniesLivestock && TargetLocation is Forest forest)
                {
                    forest.marniesLivestock.Remove(farmAnimal);
                }
                if (farmAnimal.home?.GetIndoors() is AnimalHouse animalHouse)
                {
                    animalHouse.animalsThatLiveHere.Remove(farmAnimal.myID.Value);
                }
                TargetLocation.animals.Remove(farmAnimal.myID.Value);
                TargetObject = null;
            }
            else if (TargetObject is SObject)
            {
                TargetLocation.objects.Remove(TilePosition);
                TargetObject = null;
            }
            else if (TargetObject is ResourceClump resourceClump)
            {
                TargetLocation.resourceClumps.Remove(resourceClump);
                TargetObject = null;
            }
            else if (TargetObject is TerrainFeature)
            {
                if (TargetObject is Bush bush && bush.size.Value == 3)
                {
                    if (TargetLocation.objects.TryGetValue(TilePosition, out var obj1) && obj1 is IndoorPot pot1 && pot1.bush.Value is not null)
                    {
                        pot1.bush.Value = null;
                        TargetObject = null;
                    }
                    else if (TargetLocation.terrainFeatures.ContainsKey(TilePosition))
                    {
                        TargetLocation.terrainFeatures.Remove(TilePosition);
                        TargetObject = null;
                    }
                }
                else if (TargetObject is LargeTerrainFeature largeTerrainFeature)
                {
                    TargetLocation.largeTerrainFeatures.Remove(largeTerrainFeature);
                    TargetObject = null;
                }
                else
                {
                    TargetLocation.terrainFeatures.Remove(TilePosition);
                    TargetObject = null;
                }
            }
            else if (TargetObject is Crop)
            {
                if (TargetLocation.objects.TryGetValue(TilePosition, out var oldPot))
                {
                    if (oldPot is IndoorPot pot && pot.hoeDirt.Value.crop is not null)
                    {
                        pot.hoeDirt.Value.crop = null;
                        TargetObject = null;
                    }
                }
                else if (TargetLocation.terrainFeatures.TryGetValue(TilePosition, out var oldHoeDirt))
                {
                    if (oldHoeDirt is HoeDirt hoeDirt && hoeDirt.crop is not null)
                    {
                        hoeDirt.crop = null;
                        TargetObject = null;
                    }
                }
            }
            else if (TargetObject is Building building)
            {
                if (!Game1.IsMasterGame)
                {
                    Game1.addHUDMessage(new(I18n.Message("OnlyHost"), 3));
                    Game1.playSound("cancel");
                    TargetObject = null;
                    return;
                }
                if (!TargetLocation.HasMinBuildings(Name, 2))
                {
                    if (Name == "Farmhouse" || Name == "Greenhouse" || Name == "Pet Bowl" || Name == "Shipping Bin")
                    {
                        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_LockFailed"), 3));
                        Game1.playSound("cancel");
                        TargetObject = null;
                        return;
                    }
                }
                // From CarpenterMenu Copy
                GameLocation interior = building.GetIndoors();
                Cabin? cabin = interior as Cabin;
                if (building.daysOfConstructionLeft.Value > 0 || building.daysUntilUpgrade.Value > 0)
                {
                    Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_DuringConstruction"), 3));
                    Game1.playSound("cancel");
                    TargetObject = null;
                    return;
                }
                else if (interior is AnimalHouse animalHouse && animalHouse.animalsThatLiveHere.Count > 0)
                {
                    Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_AnimalsHere"), 3));
                    Game1.playSound("cancel");
                    TargetObject = null;
                    return;
                }
                else if (interior != null && interior.farmers.Any())
                {
                    Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_PlayerHere"), 3));
                    Game1.playSound("cancel");
                    TargetObject = null;
                    return;
                }
                else
                {
                    if (cabin != null)
                    {
                        foreach (Farmer allFarmer in Game1.getAllFarmers())
                        {
                            if (allFarmer.currentLocation != null && allFarmer.currentLocation.Name == cabin.GetCellarName())
                            {
                                Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_PlayerHere"), 3));
                                Game1.playSound("cancel");
                                TargetObject = null;
                                return;
                            }
                        }
                        if (cabin.IsOwnerActivated)
                        {
                            Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_FarmhandOnline"), 3));
                            Game1.playSound("cancel");
                            TargetObject = null;
                            return;
                        }
                    }
                    building.BeforeDemolish();
                    Chest chest = null!;
                    if (cabin != null)
                    {
                        List<Item> list = cabin.demolish();
                        if (list.Count > 0)
                        {
                            chest = new Chest(playerChest: true);
                            chest.fixLidFrame();
                            chest.Items.OverwriteWith(list);
                        }
                    }
                    if (TargetLocation.destroyStructure(building))
                    {
                        Game1.flashAlpha = 1f;
                        building.showDestroyedAnimation(TargetLocation);
                        Game1.playSound("explosion");
                        Utility.spreadAnimalsAround(building, TargetLocation);
                        if (chest != null)
                        {
                            TargetLocation.objects[new Vector2(building.tileX.Value + building.tilesWide.Value / 2, building.tileY.Value + building.tilesHigh.Value / 2)] = chest;
                        }
                        TargetObject = null;
                        return;
                    }
                }
            }
        }
    }
}
