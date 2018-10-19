using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile.Dimensions;
using static DeepWoodsMod.DeepWoodsRandom;
using static DeepWoodsMod.DeepWoodsSettings;
using static DeepWoodsMod.DeepWoodsGlobals;
using System.Reflection;

namespace DeepWoodsMod
{
    class DeepWoodsStuffCreator
    {
        private DeepWoods deepWoods;
        private DeepWoodsRandom random;
        private DeepWoodsSpaceManager spaceManager;

        private DeepWoodsStuffCreator(DeepWoods deepWoods, DeepWoodsRandom random, DeepWoodsSpaceManager spaceManager)
        {
            this.deepWoods = deepWoods;
            this.random = random;
            this.spaceManager = spaceManager;
        }

        public static void AddStuff(DeepWoods deepWoods, DeepWoodsRandom random)
        {
            DeepWoodsSpaceManager spaceManager = new DeepWoodsSpaceManager(deepWoods.mapWidth.Value, deepWoods.mapHeight.Value);
            new DeepWoodsStuffCreator(deepWoods, random, spaceManager).ClearAndAddStuff();
        }

        private void ClearAndAddStuff()
        {
            if (!Game1.IsMasterGame)
                return;

            // this.random.EnterMasterMode();

            ClearStuff();
            AddStuff();

            // this.random.LeaveMasterMode();
        }

        private void ClearStuff()
        {
            deepWoods.resourceClumps.Clear();
            deepWoods.largeTerrainFeatures.Clear();
            deepWoods.terrainFeatures.Clear();
            deepWoods.objects.Clear();
            deepWoods.debris.Clear();
            deepWoods.overlayObjects.Clear();
        }

        private void AddStuff()
        {
            int mapWidth = this.spaceManager.GetMapWidth();
            int mapHeight = this.spaceManager.GetMapHeight();

            if (deepWoods.level.Value > Settings.Level.MinLevelForThornyBushes)
            {
                // TODO: Add thorny bushes around entrance area.
                // deepWoods.terrainFeatures[new Vector2(10, 10), new ThornyBush(new Vector2(10, 10), deepWoods));
            }

            if (deepWoods.isLichtung)
            {
                // Add something awesome in the lichtung center
                AddSomethingAwesomeForLichtung(new Vector2(deepWoods.lichtungCenter.X, deepWoods.lichtungCenter.Y));
            }

            if (!deepWoods.isLichtung && deepWoods.level.Value >= Settings.Level.MinLevelForGingerbreadHouse && this.random.CheckChance(Settings.Luck.Terrain.ChanceForGingerbreadHouse))
            {
                // Add a gingerbread house
                deepWoods.resourceClumps.Add(new GingerBreadHouse(new Vector2(mapWidth / 2, mapHeight / 2)));
            }

            int numEasterEggs = 0;
            int maxEasterEggs = 0;
            if (IsEasterEggDay())
            {
                maxEasterEggs = 1 + this.random.GetRandomValue(Settings.Luck.Terrain.MaxEasterEggsPerLevel);
                if (this.random.CheckChance(Settings.Luck.Terrain.ChanceForExtraEasterEgg))
                {
                    maxEasterEggs++;
                }
                if (!this.random.CheckChance(Settings.Luck.Terrain.ChanceForEasterEggsDoubled))
                {
                    maxEasterEggs = Math.Max(1, maxEasterEggs / 2);
                }
            }

            List<int> allTilesInRandomOrder = Enumerable.Range(0, mapWidth * mapHeight).OrderBy(n => Game1.random.Next()).ToList();

            // Calculate maximum theoretical amount of terrain features for the current map.
            int maxTerrainFeatures = (mapWidth * mapHeight) / MINIMUM_TILES_FOR_TERRAIN_FEATURE;

            int i = 0;
            for (; i < maxTerrainFeatures; i++)
            {
                // int x = this.random.GetRandomValue(1, mapWidth - 1);
                // int y = this.random.GetRandomValue(1, mapHeight - 1);

                int tileIndex = allTilesInRandomOrder[i];
                int x = tileIndex % mapWidth;
                int y = tileIndex / mapWidth;

                Vector2 location = new Vector2(x, y);

                // Check if location is free
                if (/*deepWoods.terrainFeatures.ContainsKey(location) ||*/ !deepWoods.isTileLocationTotallyClearAndPlaceable(location))
                    continue;

                // Don't place anything on the bright grass in Lichtungen
                if (deepWoods.isLichtung && DeepWoodsBuilder.IsTileIndexBrightGrass(deepWoods.map.GetLayer("Back").Tiles[x, y]?.TileIndex ?? 0))
                    continue;

                // Don't place anything on water
                if (deepWoods.doesTileHaveProperty(x, y, "Water", "Back") != null)
                    continue;

                if (deepWoods.isLichtung)
                {
                    if (this.random.CheckChance(Settings.Luck.Terrain.ChanceForFlowerOnClearing))
                    {
                        deepWoods.terrainFeatures[location] = new Flower(GetRandomFlowerType(), location);
                    }
                    else if (IsEasterEggDay() && numEasterEggs < maxEasterEggs && this.random.CheckChance(Settings.Luck.Terrain.ChanceForEasterEgg))
                    {
                        deepWoods.terrainFeatures[location] = new EasterEgg();
                        numEasterEggs++;
                    }
                    else
                    {
                        deepWoods.terrainFeatures[location] = new LootFreeGrass(GetSeasonGrassType(), this.random.GetRandomValue(1, 3));
                    }
                }
                else
                {
                    if (deepWoods.level.Value >= Settings.Level.MinLevelForMeteorite && this.random.CheckChance(Settings.Luck.Terrain.ResourceClump.ChanceForMeteorite) && IsSpaceFree(location, new Size(2, 2)))
                    {
                        deepWoods.resourceClumps.Add(new ResourceClump(ResourceClump.meteoriteIndex, 2, 2, location));
                    }
                    else if (this.random.CheckChance(Settings.Luck.Terrain.ResourceClump.ChanceForBoulder) && IsSpaceFree(location, new Size(2, 2)))
                    {
                        deepWoods.resourceClumps.Add(new ResourceClump(this.random.CheckChance(Chance.FIFTY_FIFTY) ? ResourceClump.mineRock1Index : ResourceClump.mineRock2Index, 2, 2, location));
                    }
                    else if (this.random.CheckChance(Settings.Luck.Terrain.ResourceClump.ChanceForHollowLog) && IsSpaceFree(location, new Size(2, 2)))
                    {
                        deepWoods.resourceClumps.Add(new ResourceClump(ResourceClump.hollowLogIndex, 2, 2, location));
                    }
                    else if (this.random.CheckChance(Settings.Luck.Terrain.ResourceClump.ChanceForStump) && IsSpaceFree(location, new Size(2, 2)))
                    {
                        deepWoods.resourceClumps.Add(new ResourceClump(ResourceClump.stumpIndex, 2, 2, location));
                    }
                    else if (this.random.CheckChance(Settings.Luck.Terrain.ChanceForLargeBush) && IsSpaceFree(location, new Size(3, 1)))
                    {
                        deepWoods.largeTerrainFeatures.Add(new DestroyableBush(location, Bush.largeBush, deepWoods));
                    }
                    else if (this.random.CheckChance(Settings.Luck.Terrain.ChanceForMediumBush) && IsSpaceFree(location, new Size(2, 1)))
                    {
                        deepWoods.largeTerrainFeatures.Add(new DestroyableBush(location, Bush.mediumBush, deepWoods));
                    }
                    else if (this.random.CheckChance(Settings.Luck.Terrain.ChanceForSmallBush))
                    {
                        deepWoods.largeTerrainFeatures.Add(new DestroyableBush(location, Bush.smallBush, deepWoods));
                    }
                    else if (this.random.CheckChance(Settings.Luck.Terrain.ChanceForGrownTree) && IsRegionTreeFree(location, 1))
                    {
                        deepWoods.terrainFeatures[location] = new Tree(GetRandomTreeType(), Tree.treeStage);
                    }
                    else if (this.random.CheckChance(Settings.Luck.Terrain.ChanceForMediumTree))
                    {
                        deepWoods.terrainFeatures[location] = new Tree(GetRandomTreeType(), Tree.bushStage);
                    }
                    else if (this.random.CheckChance(Settings.Luck.Terrain.ChanceForSmallTree))
                    {
                        deepWoods.terrainFeatures[location] = new Tree(GetRandomTreeType(), this.random.GetRandomValue(Tree.sproutStage, Tree.saplingStage));
                    }
                    else if (this.random.CheckChance(Settings.Luck.Terrain.ChanceForGrownFruitTree) && IsRegionTreeFree(location, 2))
                    {
                        int numFruits = 0;
                        if (deepWoods.level.Value >= Settings.Level.MinLevelForFruits)
                        {
                            numFruits = this.random.GetRandomValue(Settings.Luck.Terrain.FruitCount);
                        }
                        AddFruitTree(location, FruitTree.treeStage, numFruits);
                    }
                    else if (this.random.CheckChance(Settings.Luck.Terrain.ChanceForSmallFruitTree))
                    {
                        AddFruitTree(location, FruitTree.bushStage);
                    }
                    else if (this.random.CheckChance(Settings.Luck.Terrain.ChanceForWeed))
                    {
                        deepWoods.objects[location] = new StardewValley.Object(location, GetRandomWeedType(), 1);
                    }
                    else if (this.random.CheckChance(Settings.Luck.Terrain.ChanceForTwig))
                    {
                        deepWoods.objects[location] = new StardewValley.Object(location, GetRandomTwigType(), 1);
                    }
                    else if (this.random.CheckChance(Settings.Luck.Terrain.ChanceForStone))
                    {
                        deepWoods.objects[location] = new StardewValley.Object(location, GetRandomStoneType(), 1);
                    }
                    else if (this.random.CheckChance(Settings.Luck.Terrain.ChanceForMushroom))
                    {
                        deepWoods.objects[location] = new StardewValley.Object(location, GetRandomMushroomType(), 1) { IsSpawnedObject = true };
                    }
                    else if (deepWoods.level.Value >= Settings.Level.MinLevelForFlowers && this.random.CheckChance(Game1.currentSeason == "winter" ? Settings.Luck.Terrain.ChanceForFlowerInWinter : Settings.Luck.Terrain.ChanceForFlower))
                    {
                        deepWoods.terrainFeatures[location] =new Flower(GetRandomFlowerType(), location);
                    }
                    else if (IsEasterEggDay() && numEasterEggs < maxEasterEggs && this.random.CheckChance(Settings.Luck.Terrain.ChanceForEasterEgg))
                    {
                        deepWoods.terrainFeatures[location] = new EasterEgg();
                        numEasterEggs++;
                    }
                    else
                    {
                        deepWoods.terrainFeatures[location] = new LootFreeGrass(GetSeasonGrassType(), this.random.GetRandomValue(1, 3));
                    }
                }
            }

            // Fill up with grass (if not a Lichtung)
            if (!deepWoods.isLichtung)
            {
                int maxGrass = (allTilesInRandomOrder.Count() - maxTerrainFeatures) / 3;
                if (Game1.currentSeason == "winter")
                {
                    // Leaveless trees and snow ground make winter forest look super empty and open,
                    // so we fill it with plenty of icy grass to give it a better atmosphere.
                    maxGrass *= 2;
                }
                for (int j = 0; j < maxGrass; j++, i++)
                {
                    int tileIndex = allTilesInRandomOrder[i];
                    int x = tileIndex % mapWidth;
                    int y = tileIndex / mapWidth;

                    Vector2 location = new Vector2(x, y);

                    if (/*deepWoods.terrainFeatures.ContainsKey(location) ||*/ !deepWoods.isTileLocationTotallyClearAndPlaceable(location))
                        continue;

                    deepWoods.terrainFeatures[location] = new LootFreeGrass(GetSeasonGrassType(), this.random.GetRandomValue(1, 3));
                }
            }
        }

        private void AddSomethingAwesomeForLichtung(Vector2 location)
        {
            if (deepWoods.lichtungHasLake.Value)
                return;

            switch (this.random.GetRandomValue(Settings.Luck.Clearings.Perks))
            {
                /*case LichtungStuff.Lake:
                    this.deepWoodsBuilder.AddLakeToLichtung();
                    break;*/
                case LichtungStuff.MushroomTrees:
                    // TODO!
                    break;
                case LichtungStuff.Treasure:
                    if (this.random.CheckChance(Settings.Luck.Clearings.ChanceForTrashOrTreasure))
                    {
                        deepWoods.objects[location] = new TreasureChest(location, CreateRandomTreasureChestItems());
                        AddLichtungStuffPile(location, Settings.Luck.Clearings.Treasure.PileItems);
                    }
                    else
                    {
                        deepWoods.objects[location] = new TreasureChest(location, CreateRandomTrashCanItems(), true);
                        AddLichtungStuffPile(location, Settings.Luck.Clearings.Trash.PileItems);
                    }
                    break;
                case LichtungStuff.GingerbreadHouse:
                    deepWoods.resourceClumps.Add(new GingerBreadHouse(location - new Vector2(2, 4)));
                    AddGingerBreadHouseDeco(location - new Vector2(2, 4));
                    break;
                case LichtungStuff.HealingFountain:
                    deepWoods.largeTerrainFeatures.Add(new HealingFountain(location - new Vector2(2, 0)));
                    AddRipeFruitTreesAroundFountain(location - new Vector2(2, 2));
                    if (Game1.currentSeason == "winter")
                        AddWinterFruitsAroundFountain(location - new Vector2(2, 2));
                    break;
                case LichtungStuff.IridiumTree:
                    deepWoods.resourceClumps.Add(new IridiumTree(location));
                    AddIridiumNodesAroundTree(location);
                    Game1.currentLightSources.Add(new LightSource(LightSource.sconceLight, location, 6, new Color(1f, 0f, 0f)));
                    break;
                case LichtungStuff.Unicorn:
                    if (!Game1.isRaining)
                    {
                        deepWoods.characters.Add(new Unicorn(location));
                    }
                    break;
                case LichtungStuff.ExcaliburStone:
                    deepWoods.largeTerrainFeatures.Add(new ExcaliburStone(location));
                    break;
                case LichtungStuff.Nothing:
                default:
                    break;
            }
        }

        private Location GetTreasurePileOffset()
        {
            int y = this.random.GetRandomValue(0, 2) + this.random.GetRandomValue(0, 2);
            int x;
            if (y == 0)
                x = this.random.GetRandomValue(0, 2) == 1 ? -1 : 1;
            else if (y == 1)
                x = this.random.GetRandomValue(-2, 3);
            else
                x = this.random.GetRandomValue(-1, 2);
            return new Location(x, y);
        }

        private struct OffsetVariation
        {
            public readonly int minX;
            public readonly int maxX;
            public readonly int minY;
            public readonly int maxY;
            public OffsetVariation(int minX, int maxX, int minY, int maxY)
            {
                this.minX = minX;
                this.maxX = maxX;
                this.minY = minY;
                this.maxY = maxY;
            }
        }

        private OffsetVariation GetTreasurePileOffsetVariation(Location offset)
        {
            if (offset.X == -1 && offset.Y <= 1)
                return new OffsetVariation(-32, 0, -32, 32);

            if (offset.X == 1 && offset.Y <= 1)
                return new OffsetVariation(0, 32, -32, 32);

            if (offset.X == 0 && offset.Y == 1)
                return new OffsetVariation(-32, 32, 0, 32);

            return new OffsetVariation(-32, 32, -32, 32);
        }

        private void AddLichtungStuffPile(Vector2 location, WeightedInt[] itemIds)
        {
            int x = (int)location.X;
            int y = (int)location.Y;
            int numStuff = this.random.GetRandomValue(32, 128);
            for (int i = 0; i < numStuff; i++)
            {
                Location offset = GetTreasurePileOffset();
                OffsetVariation offsetVariation = GetTreasurePileOffsetVariation(offset);
                int debrisX = (x + offset.X) * 64 + this.random.GetRandomValue(offsetVariation.minX, offsetVariation.maxX);
                int debrisY = (x + offset.Y) * 64 + this.random.GetRandomValue(offsetVariation.minY, offsetVariation.maxY);

                Vector2 debrisLocation = new Vector2(debrisX, debrisY);

                Debris debris = new Debris(this.random.GetRandomValue(itemIds), debrisLocation, debrisLocation) {
                    chunkFinalYLevel = debrisY,
                    chunkFinalYTarget = debrisY,
                    movingFinalYLevel = false
                };

                for (int j = 0; j < 100; j++)
                {
                    debris.timeSinceDoneBouncing += 100;
                    debris.updateChunks(Game1.currentGameTime, deepWoods);
                }

                foreach (Chunk chunk in debris.Chunks)
                {
                    chunk.xVelocity.Value = 0;
                    chunk.yVelocity.Value = 0;
                    chunk.rotationVelocity = 0;
                    chunk.position.X = debrisX;
                    chunk.position.Y = debrisY;
                    chunk.hasPassedRestingLineOnce.Value = true;
                    chunk.bounces = 3;
                }

                deepWoods.debris.Add(debris);
            }
        }

        private void AddGingerBreadHouseDeco(Vector2 location)
        {
            for (int y = 1; y <= 5; y++)
            {
                Vector2 leftPos = new Vector2(location.X - 2, location.Y + y);
                Vector2 rightPos = new Vector2(location.X + 6, location.Y + y);
                if (y == 1 || y == 5)
                {
                    int id = this.random.GetRandomValue(new int[] { 40, 44 });  // Big Green Cane or Big Red Cane
                    deepWoods.objects[leftPos] = new StardewValley.Object(leftPos, id) { Flipped = true };
                    deepWoods.objects[rightPos] = new StardewValley.Object(rightPos, id) { Flipped = false };
                }
                else
                {
                    deepWoods.objects[leftPos] = new StardewValley.Object(leftPos, GetRandomSmallCaneType()) { Flipped = this.random.CheckChance(Chance.FIFTY_FIFTY) };
                    deepWoods.objects[rightPos] = new StardewValley.Object(rightPos, GetRandomSmallCaneType()) { Flipped = this.random.CheckChance(Chance.FIFTY_FIFTY) };
                }
                if (y >= 3)
                {
                    Vector2 centerPos = new Vector2(location.X + 2, location.Y + y);
                    deepWoods.objects[centerPos] = new StardewValley.Object(centerPos, 409, 1) { Flipped = this.random.CheckChance(Chance.FIFTY_FIFTY) }; // Crystal Floor
                }
            }

            for (int x = -1; x <= 5; x++)
            {
                if (x != 2)
                {
                    Vector2 pos = new Vector2(location.X + x, location.Y + 5);
                    deepWoods.terrainFeatures[pos] = new Flower(431, pos);   // Sunflower
                }
                if (x == -1 || x == 5)
                {
                    Vector2 pos = new Vector2(location.X + x, location.Y + 1);
                    deepWoods.terrainFeatures[pos] = new Flower(431, pos);   // Sunflower
                }
            }
        }

        private int GetRandomSmallCaneType()
        {
            // Green Canes, Mixed Canes or Red Canes
            return this.random.GetRandomValue(41, 43+1);
        }

        private void AddIridiumNodesAroundTree(Vector2 location)
        {
            int numIridiumNodes = 5;
            for (int dist = 1; numIridiumNodes > 0; dist++, numIridiumNodes /= 2)
            {
                for (int i = 0; i < numIridiumNodes; i++)
                {
                    int nodeX, nodeY;
                    do
                    {
                        nodeX = this.random.GetRandomValue(-dist, dist + 2);
                        nodeY = this.random.GetRandomValue(-dist, dist + 1);
                    }
                    while ((nodeX == 0 || nodeX == 1) && nodeY == 0);

                    Vector2 nodeLocation = new Vector2(location.X + nodeX, location.Y + nodeY);

                    if (deepWoods.objects.ContainsKey(nodeLocation))
                        continue;

                    deepWoods.objects[nodeLocation] = new StardewValley.Object(nodeLocation, 765, "Stone", true, false, false, false) { MinutesUntilReady = 16 };
                }
            }
        }

        private void AddWinterFruitsAroundFountain(Vector2 location)
        {
            int x = (int)location.X;
            int y = (int)location.Y;
            xTile.Dimensions.Rectangle fountainRectangle = new xTile.Dimensions.Rectangle(x, y, 6, 5);
            int minX = x - 6;
            int maxX = x + 9;
            int minY = y - 6;
            int maxY = y + 9;
            int numWinterFruits = this.random.GetRandomValue(9, 16);
            for (int i = 0; i < numWinterFruits; i++)
            {
                int fruitX = this.random.GetRandomValue(minX, maxX);
                int fruitY = this.random.GetRandomValue(minX, maxX);
                if (fountainRectangle.Contains(new Location(fruitX, fruitY)))
                    continue;

                Vector2 fruitLocation = new Vector2(fruitX, fruitY);

                if (deepWoods.objects.ContainsKey(fruitLocation))
                    continue;

                deepWoods.objects[fruitLocation] = new StardewValley.Object(fruitLocation, 414, 1);
            }
        }

        private void AddRipeFruitTreesAroundFountain(Vector2 location)
        {
            AddRipeFruitTree(location + new Vector2(-3, -1));
            AddRipeFruitTree(location + new Vector2(-3, 6));
            AddRipeFruitTree(location + new Vector2(8, -1));
            AddRipeFruitTree(location + new Vector2(8, 6));
        }

        private void AddRipeFruitTree(Vector2 location)
        {
            AddFruitTree(location, FruitTree.treeStage, 3);
        }

        private void AddFruitTree(Vector2 location, int growthStage, int fruitsOnTree = 0)
        {
            FruitTree fruitTree = new FruitTree(GetRandomFruitTreeType(), FruitTree.treeStage);
            fruitTree.fruitsOnTree.Value = Game1.currentSeason == "winter" ? 0 : fruitsOnTree;
            fruitTree.daysUntilMature.Value = 28 - (growthStage * 7);
            deepWoods.terrainFeatures[location] = fruitTree;
        }

        private void AddItem(List<Item> items, int id, int stackSize = 1)
        {
            items.Add(new StardewValley.Object(id, stackSize));
        }

        private List<Item> CreateRandomTreasureChestItems()
        {
            List<Item> items = new List<Item>();

            if (this.random.CheckChance(Settings.Luck.Clearings.Treasure.ChanceForMetalBarsInChest))
            {
                AddItem(items, this.random.GetRandomValue(334, 338 + 1), this.random.GetRandomValue(Settings.Luck.Clearings.Treasure.MetalBarStackSize));
            }

            if (this.random.CheckChance(Settings.Luck.Clearings.Treasure.ChanceForElixirsInChest))
            {
                AddItem(items, this.random.GetRandomValue(new int[] { 772, 773 }), this.random.GetRandomValue(Settings.Luck.Clearings.Treasure.ElixirStackSize));
            }

            if (this.random.CheckChance(Settings.Luck.Clearings.Treasure.ChanceForArtefactInChest))
            {
                AddItem(items, 124);
            }

            if (this.random.CheckChance(Settings.Luck.Clearings.Treasure.ChanceForDwarfScrollInChest))
            {
                AddItem(items, this.random.GetRandomValue(96, 99 + 1));
            }

            if (this.random.CheckChance(Settings.Luck.Clearings.Treasure.ChanceForRingInChest))
            {
                AddItem(items, this.random.GetRandomValue(516, 534 + 1));
            }

            if (this.random.CheckChance(Settings.Luck.Clearings.Treasure.ChanceForRandomPileItemInChest))
            {
                AddItem(items, this.random.GetRandomValue(Settings.Luck.Clearings.Treasure.PileItems), this.random.GetRandomValue(Settings.Luck.Clearings.Treasure.PileItemStackSize));
            }

            // Shuffle items around
            return new List<Item>(items.OrderBy<Item, int>(a => Game1.random.Next()));
        }

        private List<Item> CreateRandomTrashCanItems()
        {
            List<Item> items = new List<Item>();

            if (this.random.CheckChance(Settings.Luck.Clearings.Trash.ChanceForLewisShortsInGarbagebin))
            {
                AddItem(items, 789);
            }

            if (this.random.CheckChance(Settings.Luck.Clearings.Trash.ChanceForBoneInGarbagebin))
            {
                AddItem(items, this.random.GetRandomValue(579, 585 + 1));
            }

            if (this.random.CheckChance(Settings.Luck.Clearings.Trash.ChanceForArtefactInGarbagebin))
            {
                AddItem(items, this.random.GetRandomValue(new int[] { 111, 112, 113, 115 }));
            }

            if (this.random.CheckChance(Settings.Luck.Clearings.Trash.ChanceForPuppetInGarbagebin))
            {
                AddItem(items, this.random.GetRandomValue(new int[] { 103, 126, 127 }));
            }

            if (this.random.CheckChance(Settings.Luck.Clearings.Trash.ChanceForRandomPileItemInGarbagebin))
            {
                AddItem(items, this.random.GetRandomValue(Settings.Luck.Clearings.Trash.PileItems), this.random.GetRandomValue(Settings.Luck.Clearings.Trash.PileItemStackSize));
            }

            // Shuffle items around
            return new List<Item>(items.OrderBy<Item, int>(a => Game1.random.Next()));
        }

        private bool IsEasterEggDay()
        {
            return Game1.currentSeason == "spring" && (Game1.dayOfMonth == 13 || Game1.dayOfMonth == 14);
        }

        private int GetRandomFlowerType()
        {
            if (Game1.currentSeason == "winter")
            {
                return this.random.GetRandomValue(Settings.Luck.Terrain.WinterFlowers);
            }
            else
            {
                return this.random.GetRandomValue(Settings.Luck.Terrain.Flowers);
            }
        }

        private bool IsSpaceFree(Vector2 location, Size resourceClumpSize)
        {
            for (int x = 0; x < resourceClumpSize.Width; x++)
            {
                for (int y = 0; y < resourceClumpSize.Height; y++)
                {
                    Vector2 check = new Vector2(location.X + x, location.Y + y);
                    if (deepWoods.terrainFeatures.ContainsKey(check)
                        || !deepWoods.isTileLocationTotallyClearAndPlaceable(check))
                        return false;
                }
            }
            return true;
        }

        private bool TileHasTree(Vector2 location)
        {
            return deepWoods.terrainFeatures.ContainsKey(location)
                && (deepWoods.terrainFeatures[location] is FruitTree
                    || ((deepWoods.terrainFeatures[location] as Tree)?.growthStage ?? 0) >= Tree.treeStage
                );
        }

        private bool IsRegionTreeFree(Vector2 location, int radius)
        {
            for (int x = -radius; x < radius*2; x++)
            {
                for (int y = -radius; y < radius * 2; y++)
                {
                    if (TileHasTree(new Vector2(location.X + x, location.Y + y)))
                        return false;
                }
            }
            return true;
        }

        private int GetSeasonGrassType()
        {
            return Game1.currentSeason == "winter" ? Grass.frostGrass : Grass.springGrass;
        }

        private int GetRandomWeedType()
        {
            return GameLocation.getWeedForSeason(new Random(this.random.GetRandomValue()), Game1.currentSeason);
        }

        private int GetRandomStoneType()
        {
            return this.random.GetRandomValue(new int[] { 343, 668, 670 });
        }

        private int GetRandomTwigType()
        {
            return this.random.GetRandomValue(new int[] { 294, 295 });
        }

        private int GetRandomTreeType()
        {
            return this.random.GetRandomValue(new int[] { Tree.bushyTree, Tree.leafyTree, Tree.pineTree });
        }

        private int GetRandomMushroomType()
        {
            return this.random.GetRandomValue(new WeightedInt[] {
                new WeightedInt(422, 1),  // Purple one
                new WeightedInt(420, 5),  // Red one
                new WeightedInt(257, 10), // Morel
                new WeightedInt(281, 20), // Big brown one
                new WeightedInt(404, 50), // Normal one
            });
        }

        private int GetRandomFruitTreeType()
        {
            Dictionary<int, string> fruitTrees = Game1.content.Load<Dictionary<int, string>>("Data\\fruitTrees");
            int[] fruitTreeTypes = fruitTrees.Keys.ToArray();
            Array.Sort(fruitTreeTypes);
            return this.random.GetRandomValue(fruitTreeTypes);
        }
    }
}
