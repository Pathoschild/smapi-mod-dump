using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;

//tree types (default names)
//
//bushyTree = 1;
//leafyTree = 2;
//pineTree = 3;
//winterTree1 = 4;
//winterTree2 = 5;
//palmTree = 6;
//mushroomTree = 7;

namespace TreeOverhaul
{
    public class TreeOverhaul : Mod
    {
        public TreeOverhaulConfig treeOverhaulConfig;

        public override void Entry(IModHelper helper)
        {
            TimeEvents.AfterDayStarted += Events_NewDay;
            treeOverhaulConfig = helper.ReadConfig<TreeOverhaulConfig>();
        }

        public void Events_NewDay(object sender, EventArgs e)
        {
            if (!Context.IsMainPlayer)
            {
                return;
            }

            foreach (var location in Game1.locations)
            {
                foreach (var terrainfeature in location.terrainFeatures.Pairs)
                {
                    switch (terrainfeature.Value)
                    {
                        case Tree tree:
                            CheckTree(tree, location, terrainfeature.Key);
                            StopSapling(tree, location, terrainfeature.Key);
                            break;
                        case FruitTree fruittree:
                            CheckFruitTree(fruittree, location, terrainfeature.Key);
                            break;
                    }
                }
            }
        }

        public void StopSapling(Tree tree, GameLocation location, Vector2 tileLocation)
        {
            if (!treeOverhaulConfig.StopShadeSaplingGrowth || tree?.growthStage.Value != 1) return;
            Rectangle rectangle = new Rectangle((int)(((double)tileLocation.X - 1.0) * (double)Game1.tileSize), (int)(((double)tileLocation.Y - 1.0) * (double)Game1.tileSize), Game1.tileSize * 3, Game1.tileSize * 3);

            var idk = from keyValuePair in location.terrainFeatures.Pairs
                let t = keyValuePair.Value as Tree
                where t != null && !t.Equals(tree) && t.growthStage.Value >= 5 && t.getBoundingBox(keyValuePair.Key).Intersects(rectangle)
                select t;

            if (idk.Any())
            {
                tree.growthStage.Set(0);
            }
        }

        public void CheckTree(Tree tree, GameLocation location, Vector2 tileLocation)
        {
            if (Game1.currentSeason.Equals("winter"))
            {
                if (tree.treeType.Value != 6 || location.Name.ToLower().Contains("greenhouse"))
                {
                    if (tree.treeType.Value != 7 && treeOverhaulConfig.NormalTreesGrowInWinter)
                    {
                        GrowTree(tree, location, tileLocation);
                    }
                    if (tree.treeType.Value == 7 && treeOverhaulConfig.MushroomTreesGrowInWinter)
                    {
                        FixMushroomStump(tree, location, tileLocation);
                        GrowTree(tree, location, tileLocation);
                    }
                }
            }

            if (!treeOverhaulConfig.FasterNormalTreeGrowth) return;

            if (Game1.currentSeason.Equals("winter"))
            {
                if (tree.treeType.Value == 6 && !location.Name.ToLower().Contains("greenhouse")) return;
                if (tree.treeType.Value != 7 && treeOverhaulConfig.NormalTreesGrowInWinter)
                {
                    GrowTree(tree, location, tileLocation);
                }
                if (tree.treeType.Value == 7 && treeOverhaulConfig.MushroomTreesGrowInWinter)
                {
                    FixMushroomStump(tree, location, tileLocation);
                    GrowTree(tree, location, tileLocation);
                }
            }
            else
            {
                if (tree.treeType.Value == 6 && !location.Name.ToLower().Contains("greenhouse")) return;
                if (tree.treeType.Value != 7)
                {
                    GrowTree(tree, location, tileLocation);
                }
                if (tree.treeType.Value == 7)
                {
                    FixMushroomStump(tree, location, tileLocation);
                    GrowTree(tree, location, tileLocation);
                }
            }
        }

        public void GrowTree(Tree tree, GameLocation location, Vector2 tileLocation)
        {
            Rectangle value = new Rectangle((int)((tileLocation.X - 1f) * (float)Game1.tileSize), (int)((tileLocation.Y - 1f) * (float)Game1.tileSize), Game1.tileSize * 3, Game1.tileSize * 3);
            string text = location.doesTileHaveProperty((int)tileLocation.X, (int)tileLocation.Y, "NoSpawn", "Back");
            if (text != null && (text.Equals("All") || text.Equals("Tree")))
            {
                return;
            }

            if (tree.growthStage.Value == 4)
            {
                foreach (var pair in location.terrainFeatures.Pairs)
                {
                    if (pair.Value is Tree tree1 && !tree1.Equals(tree) && tree1.growthStage.Value >= 5 && tree1.getBoundingBox(pair.Key).Intersects(value)) return;
                }
                if (Game1.random.NextDouble() < 0.2)
                {
                    tree.growthStage.Set(tree.growthStage.Value + 1);
                }
            }

            if (tree.growthStage.Value != 0 || !location.objects.ContainsKey(tileLocation))
            {
                if (Game1.random.NextDouble() < 0.2)
                {
                    tree.growthStage.Set(tree.growthStage.Value + 1);
                }
            }
        }

        public void FixMushroomStump(Tree tree, GameLocation location, Vector2 tileLocation)
        {
            if (Game1.currentSeason.Equals("winter"))
            {
                if (tree.stump.Value)
                {
                    tree.stump.Set(false);
                    tree.health.Set(10f);
                }
            }
        }

        public void CheckFruitTree(FruitTree fruittree, GameLocation location, Vector2 tileLocation)
        {
            if (treeOverhaulConfig.FruitTreesDontGrowInWinter)
            {
                if (Game1.currentSeason.Equals("winter"))
                {
                    if (location.Name.ToLower().Contains("greenhouse"))
                    {
                        CheckGrowthType(fruittree, location, tileLocation);
                    }
                    else
                    {
                        GrowFruitTree(fruittree, location, tileLocation, "plus");
                    }
                }
                else
                {
                    CheckGrowthType(fruittree, location, tileLocation);
                }

            }
            else
            {
                CheckGrowthType(fruittree, location, tileLocation);
            }
        }

        public void CheckGrowthType(FruitTree fruittree, GameLocation location, Vector2 tileLocation)
        {
            if (treeOverhaulConfig.FruitTreeGrowth == 1)
            {
                GrowFruitTree(fruittree, location, tileLocation, "minus");
            }

            if (treeOverhaulConfig.FruitTreeGrowth == 2)
            {
                if (Game1.dayOfMonth % 2 == 1) //odd day
                {
                    GrowFruitTree(fruittree, location, tileLocation, "plus");
                }
            }
        }

        public void CheckGrowthStage(FruitTree fruittree, GameLocation location, Vector2 tileLocation)
        {
            if (fruittree.daysUntilMature.Value > 28)
            {
                fruittree.daysUntilMature.Set(28);
            }
            if (fruittree.daysUntilMature.Value <= 0)
            {
                fruittree.growthStage.Set(4);
            }
            else if (fruittree.daysUntilMature.Value <= 7)
            {
                fruittree.growthStage.Set(3);
            }
            else if (fruittree.daysUntilMature.Value <= 14)
            {
                fruittree.growthStage.Set(2);
            }
            else if (fruittree.daysUntilMature.Value <= 21)
            {
                fruittree.growthStage.Set(1);
            }
            else
            {
                fruittree.growthStage.Set(0);
            }
        }

        public void GrowFruitTree(FruitTree fruittree, GameLocation location, Vector2 tileLocation, string change)
        {
            bool flag = false;
            Vector2[] surroundingTileLocationsArray = Utility.getSurroundingTileLocationsArray(tileLocation);
            foreach (var vector in surroundingTileLocationsArray)
            {
                bool flag2 = location.terrainFeatures.ContainsKey(vector) && location.terrainFeatures[vector] is HoeDirt && ((HoeDirt) location.terrainFeatures[vector]).crop == null;
                if (location.isTileOccupied(vector, "") && !flag2)
                {
                    flag = true;
                    break;
                }
            }

            if (flag) return;
            if (fruittree.daysUntilMature.Value > 28)
            {
                fruittree.daysUntilMature.Set(28);
            }
            if (fruittree.daysUntilMature.Value > 0)
            {
                if (change == "minus")
                {
                    fruittree.daysUntilMature.Set(fruittree.daysUntilMature.Value--);
                }
                if (change == "plus")
                {
                    fruittree.daysUntilMature.Set(fruittree.daysUntilMature.Value++);
                }
                CheckGrowthStage(fruittree, location, tileLocation);
            }
        }                
    }

    public class TreeOverhaulConfig
    {
        public bool StopShadeSaplingGrowth { get; set; } = true;
        public bool NormalTreesGrowInWinter { get; set; } = true;
        public bool MushroomTreesGrowInWinter { get; set; } = false;
        public bool FruitTreesDontGrowInWinter { get; set; } = false;
        public bool FasterNormalTreeGrowth { get; set; } = false;
        public int FruitTreeGrowth { get; set; } = 0;
    }
}