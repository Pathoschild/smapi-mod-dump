/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Caua-Oliveira/StardewValley-AutomateToolSwap
**
*************************************************/

using AutomateToolSwap;
using StardewValley;
using Microsoft.Xna.Framework;
using System;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Tools;
using xTile.Dimensions;
using xTile.Tiles;
using StardewValley.TerrainFeatures;
using StardewValley.Buildings;
using StardewModdingAPI;
using static StardewValley.Minigames.CraneGame;
using StardewValley.Monsters;
using StardewValley.Characters;
using Netcode;



public class Check
{
    private ModEntry ModEntry;

    public Check(ModEntry modEntry)
    {
        ModEntry = modEntry;
    }

    public bool Objects(GameLocation location, Vector2 tile, Farmer player)
    {

        var Config = ModEntry.Config;
        // Get the object at the specified tile
        StardewValley.Object obj = location.getObjectAtTile((int)tile.X, (int)tile.Y);

        // If obj is null, return false immediately
        if (obj == null)
        {
            return false;
        }

        // Check for objects and conditions
        if (obj.IsWeeds())
        {
            if (Config.AnyToolForWeeds && !(location is MineShaft))
            {
                ModEntry.SetTool(player, typeof(Pickaxe), anyTool: true);
                return true;
            }
            ModEntry.SetTool(player, typeof(MeleeWeapon));
            return true;
        }

        if (obj is CrabPot crabPot)
        {
            if (crabPot.bait.Value == null)
            {
                ModEntry.SetItem(player, "Bait", "Bait");

            }
            return true;
        }

        if (obj.IsBreakableStone())
        {
            if (player.CurrentItem == null || (!player.CurrentItem.Name.Contains("Bomb") && !player.CurrentItem.Name.Contains("Staircase")))
            {
                ModEntry.SetTool(player, typeof(Pickaxe));
                return true;
            }
        }

        if (obj.IsTwig())
        {
            ModEntry.SetTool(player, typeof(Axe));
            return true;
        }

        if (obj.Name == "Furnace")
        {
            ModEntry.SetItem(player, "Resource", "Ore");
            return true;
        }

        if (obj.Name == "Cheese Press")
        {
            ModEntry.SetItem(player, "Animal Product", "Milk");
            return true;
        }

        if (obj.Name == "Mayonnaise Machine")
        {
            ModEntry.SetItem(player, "Animal Product", "Egg");
            return true;
        }

        if (obj.Name == "Artifact Spot")
        {
            ModEntry.SetTool(player, typeof(Hoe));
            return true;
        }

        if (obj.Name == "Garden Pot")
        {
            ModEntry.SetTool(player, typeof(WateringCan));
            return true;
        }

        if (obj.Name == "Seed Spot")
        {
            ModEntry.SetTool(player, typeof(Hoe));
            return true;
        }

        if (obj.Name == "Barrel")
        {
            ModEntry.SetTool(player, typeof(MeleeWeapon), "Weapon");
            return true;
        }

        if (obj.Name == "Supply Crate")
        {
            ModEntry.SetTool(player, typeof(Hoe), anyTool: true);
            return true;
        }

        if (obj.Name == "Recycling Machine")
        {
            ModEntry.SetItem(player, "Trash", "Joja");
            return true;
        }

        if (obj.Name == "Bone Mill")
        {
            ModEntry.SetItem(player, "Resource", "Bone Fragment");
            return true;
        }

        if (obj.Name == "Loom")
        {
            ModEntry.SetItem(player, "Animal Product", "Wool");
            return true;
        }

        return false;
    }

    public bool TerrainFeatures(GameLocation location, Vector2 tile, Farmer player)
    {
        bool hasTerrainFeature = location.terrainFeatures.ContainsKey(tile);
        if (!hasTerrainFeature)
        {
            return false;
        }
        var feature = location.terrainFeatures[tile];

        //Check if need Axe
        if (feature is GiantCrop)
        {
            ModEntry.SetTool(player, typeof(Axe));

            return true;
        }
        if (feature is Tree tree)
        {
            if (player.CurrentItem != null && player.CurrentItem.Name == "Tapper") { return true; }

            if (tree.hasMoss && tree.growthStage >= Tree.stageForMossGrowth)
            {
                ModEntry.SetTool(player, typeof(MeleeWeapon));

                return true;
            }

            if (!(tree.growthStage < Tree.treeStage && ModEntry.Config.IgnoreGrowingTrees))
            {
                ModEntry.SetTool(player, typeof(Axe));

                return true;
            }
            return true;
        }

        //Check if need to harvest Grass
        if (feature is Grass && !(player.CurrentTool is MilkPail or Shears) && ModEntry.Config.ScytheForGrass)
        {
            ModEntry.SetTool(player, typeof(MeleeWeapon));
            return true;
        }

        //Check if is a harvestable bush
        if (feature is StardewValley.TerrainFeatures.Bush)
        {
            StardewValley.TerrainFeatures.Bush bush = (StardewValley.TerrainFeatures.Bush)feature;
            if (bush.inBloom())
            {
                ModEntry.SetTool(player, typeof(MeleeWeapon));
                return true;
            }
            return true;
        }

        //Check for tillable soil or crops
        if (feature is HoeDirt)
        {
            HoeDirt dirt = feature as HoeDirt;

            //Check if can harvest the crop
            if (dirt.crop != null && dirt.readyForHarvest())
            {
                ModEntry.SetTool(player, typeof(MeleeWeapon));

                return true;

            }
            if (dirt.crop != null && (bool)dirt.crop.dead)
            {
                ModEntry.SetTool(player, typeof(MeleeWeapon));

                return true;

            }

            //Check if need to water the crop
            if ((dirt.crop != null && !dirt.isWatered() && !dirt.readyForHarvest()) && !(player.isRidingHorse() && player.mount.Name.Contains("tractor") && player.CurrentTool is Hoe))
            {
                if (!(ModEntry.Config.PickaxeOverWateringCan && player.CurrentTool is Pickaxe))
                {
                    ModEntry.SetTool(player, typeof(WateringCan));

                    return true;

                }
            }
            return true;
        }
        return false;
    }

    public bool ResourceClumps(GameLocation location, Vector2 tile, Farmer player)
    {
        //Check if it is an boulder or logs and stumps
        for (int i = 0; i < location.resourceClumps.Count; i++)
        {
            if (location.resourceClumps[i].occupiesTile((int)tile.X, (int)tile.Y))
            {
                switch (location.resourceClumps[i].parentSheetIndex)
                {
                    //Id's for logs and stumps
                    case 602 or 600:
                        ModEntry.SetTool(player, typeof(Axe));

                        return true;

                    //Id's for boulders
                    case 758 or 756 or 754 or 752 or 672 or 622 or 148:
                        ModEntry.SetTool(player, typeof(Pickaxe));

                        return true;
                }
                return true;
            }
        }
        return false;
    }

    public bool Monsters(GameLocation location, Vector2 tile, Farmer player)
    {
        //Check for monsters 
        foreach (var monster in location.characters)
        {
            Vector2 monsterTile = monster.Tile;
            float distance = Vector2.Distance(tile, monsterTile);

            if (monster.IsMonster && distance < ModEntry.Config.MonsterRangeDetection)
            {
                //Pickaxe for non-moving cave crab
                if (monster is RockCrab)
                {
                    RockCrab crab = monster as RockCrab;
                    var isShellLess = ModEntry.Helper.Reflection.GetField<NetBool>(crab, "shellGone").GetValue();
                    if (!isShellLess && !monster.isMoving())
                    {
                        ModEntry.SetTool(player, typeof(Pickaxe));

                        return true;

                    }
                }

                if (player.CurrentItem == null || (!player.CurrentItem.Name.Contains("Bomb") && !player.CurrentItem.Name.Contains("Staircase")))
                {
                    ModEntry.SetTool(player, typeof(MeleeWeapon), "Weapon");

                    return true;

                }

                return true;
            }

        }
        return false;
    }

    public bool Water(GameLocation location, Vector2 tile, Farmer player)
    {

        //Check for pet bowls
        bool isPetBowlOrStable = false;
        bool hasBuilding = location.getBuildingAt(tile) != null;
        bool hasWaterSource = location.doesTileHaveProperty((int)tile.X, (int)tile.Y, "WaterSource", "Back") != null;
        bool hasWater = location.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Water", "Back") != null;

        if (hasBuilding)
        {
            isPetBowlOrStable = location.getBuildingAt(tile).GetType() == typeof(PetBowl) || location.getBuildingAt(tile).GetType() == typeof(Stable);
        }

        if (isPetBowlOrStable)
        {
            ModEntry.SetTool(player, typeof(WateringCan));

            return true;

        }

        //Check for water for fishing
        if (ModEntry.Config.FishingRodOnWater && (!(location is Farm or VolcanoDungeon || location.InIslandContext() || location.isGreenhouse) && hasWater && !(player.CurrentTool is WateringCan or Pan)))
        {
            ModEntry.SetTool(player, typeof(FishingRod));

            return true;

        }

        //Check for water source to refil watering can
        if ((hasWaterSource || hasWater) && !(player.CurrentTool is FishingRod or Pan))
        {
            ModEntry.SetTool(player, typeof(WateringCan));

            return true;

        }
        return false;
    }

    public bool Animals(GameLocation location, Vector2 tile, Farmer player)
    {

        //Check for animals to milk or shear
        if (!(location is Farm or AnimalHouse))
        {
            return false;
        }

        foreach (FarmAnimal animal in location.getAllFarmAnimals())
        {
            string[] canMilk = { "Goat", "Cow" };
            string[] canShear = { "Rabbit", "Sheep" };
            float distance = Vector2.Distance(tile, animal.Tile);

            if (canMilk.Any(animal.displayType.Contains) && distance <= 1 && animal.currentLocation == player.currentLocation)
            {
                ModEntry.SetTool(player, typeof(MilkPail));

                return true;
            }

            if (canShear.Any(animal.displayType.Contains) && distance <= 1 && animal.currentLocation == player.currentLocation)
            {
                ModEntry.SetTool(player, typeof(Shears));

                return true;
            }
        }

        //Check for feeding bench
        bool hasFeedingBench = location.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Trough", "Back") != null;
        if (location is AnimalHouse && hasFeedingBench)
        {
            ModEntry.SetItem(player, "", "Hay");

            return true;
        }

        return false;

    }

    public bool ShouldSwapToHoe(GameLocation location, Vector2 tile, Farmer player)
    {
        if (!ModEntry.isTractorModInstalled || (player.isRidingHorse() && player.mount.Name.Contains("tractor")))
            return false;
        bool isNotScythe = true;
        bool isDiggable = location.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Diggable", "Back") != null;
        bool isFightingLocations = location is Mine or MineShaft or VolcanoDungeon;
        if (player.CurrentItem != null)
            isNotScythe = player.CurrentItem.getCategoryName().Contains("Level");


        //Check if it should swap to Hoe
        if (!isDiggable || isFightingLocations)
            return false;
        if (!ModEntry.Config.HoeForEmptySoil)
            return false;
        if (location.isPath(tile))
            return false;
        if (player.CurrentItem is MeleeWeapon && isNotScythe && Game1.spawnMonstersAtNight)
            return false;
        if (player.CurrentItem is FishingRod or GenericTool or Wand)
            return false;

        if (player.CurrentItem == null)
        {
            ModEntry.SetTool(player, typeof(Hoe));
            return true;
        }

        if (!player.CurrentItem.canBePlacedHere(location, tile, CollisionMask.All, true))
        {
            ModEntry.SetTool(player, typeof(Hoe));
            return true;
        }
        return false;
    }
}
