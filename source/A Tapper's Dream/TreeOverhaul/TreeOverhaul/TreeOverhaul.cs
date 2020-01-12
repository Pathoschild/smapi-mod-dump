// tree types (default names)
//
// bushyTree = 1;
// leafyTree = 2;
// pineTree = 3;
// winterTree1 = 4;
// winterTree2 = 5;
// palmTree = 6;
// mushroomTree = 7;

namespace TreeOverhaul
{
    using Microsoft.Xna.Framework;
    using Netcode;
    using StardewModdingAPI;
    using StardewModdingAPI.Events;
    using StardewValley;
    using StardewValley.Network;
    using StardewValley.TerrainFeatures;
    using System.Collections.Generic;

    /// <summary>
    /// Tree overhaul mod class that changes the behavior of trees and fruit trees.
    /// </summary>
    public class TreeOverhaul : Mod
    {
        public TreeOverhaulConfig treeOverhaulConfig;

        /// <summary>
        /// The mod entry point, called after the mod is first loaded.
        /// Loads config file and adds method to the event of starting a new day.
        /// </summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            treeOverhaulConfig = helper.ReadConfig<TreeOverhaulConfig>();
        }

        /// <summary>
        /// Iterates through every tree and fruit tree and applies growth changes according to settings.
        /// Raised after the game begins a new day (including when the player loads a save).
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        public void OnDayStarted(object sender, DayStartedEventArgs e)
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
                            CalculateTreeGrowth(tree, location, terrainfeature.Key);
                            RevertSaplingGrowthInShade(tree, location, terrainfeature.Key);
                            break;

                        case FruitTree fruittree:
                            CalculateFruitTreeGrowth(fruittree, location, terrainfeature.Key);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Checks for saplings that are growth stage 1 despite being in the shade of another tree.
        /// Uses an unedited version of the location check StardewValley.TerrainFeatures.Tree.dayUpdate so saplings cant grow when tree stumps are in the way.
        /// </summary>
        /// <param name="tree">current sapling</param>
        /// <param name="location">ingame location (e.g. farm)</param>
        /// <param name="tileLocation">position in said location</param>
        public void RevertSaplingGrowthInShade(Tree tree, GameLocation location, Vector2 tileLocation)
        {
            if (tree.growthStage.Value != 1 || !treeOverhaulConfig.StopShadeSaplingGrowth)
            {
                return;
            }

            Rectangle growthRect = new Rectangle((int)((tileLocation.X - 1f) * 64f), (int)((tileLocation.Y - 1f) * 64f), 192, 192);

            using (NetDictionary<Vector2, TerrainFeature, NetRef<TerrainFeature>, SerializableDictionary<Vector2, TerrainFeature>, NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>>>.PairsCollection.Enumerator enumerator = location.terrainFeatures.Pairs.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    KeyValuePair<Vector2, TerrainFeature> t = enumerator.Current;
                    if (t.Value is Tree && !t.Value.Equals(this) && ((Tree)t.Value).growthStage >= 5 && t.Value.getBoundingBox(t.Key).Intersects(growthRect))
                    {
                        tree.growthStage.Set(0);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Adjusts tree and mushroom tree growth depending on settings like growing them in the winter.
        /// </summary>
        /// <param name="tree">current tree</param>
        /// <param name="location">ingame location (e.g. farm)</param>
        /// <param name="tileLocation">position in said location</param>
        public void CalculateTreeGrowth(Tree tree, GameLocation location, Vector2 tileLocation)
        {
            // Cancels out with "if (!Game1.IsWinter || this.treeType == 6 || environment.IsGreenhouse || this.fertilized.Value)" from StardewValley.TerrainFeatures.Tree.dayUpdate
            // so we get exactly one growth call if the config says to grow in winter
            if (Game1.IsWinter && tree.treeType.Value != 6 && !location.IsGreenhouse && !tree.fertilized.Value)
            {
                if (tree.treeType.Value != 7 && treeOverhaulConfig.NormalTreesGrowInWinter)
                {
                    GrowTree(tree, location, tileLocation);
                }

                if (tree.treeType.Value == 7 && treeOverhaulConfig.MushroomTreesGrowInWinter)
                {
                    FixMushroomStump(tree);
                    GrowTree(tree, location, tileLocation);
                }
            }

            if (!treeOverhaulConfig.FasterNormalTreeGrowth)
            {
                return;
            }

            if (Game1.IsWinter)
            {
                if (tree.treeType.Value == 6)
                {
                    return;
                }

                if (tree.treeType.Value != 7 && (treeOverhaulConfig.NormalTreesGrowInWinter || location.IsGreenhouse || tree.fertilized.Value))
                {
                    GrowTree(tree, location, tileLocation);
                }

                if (tree.treeType.Value == 7 && (treeOverhaulConfig.MushroomTreesGrowInWinter || location.IsGreenhouse || tree.fertilized.Value))
                {
                    FixMushroomStump(tree);
                    GrowTree(tree, location, tileLocation);
                }
            }
            else
            {
                if (tree.treeType.Value == 6)
                {
                    return;
                }

                if (tree.treeType.Value == 7)
                {
                    FixMushroomStump(tree);
                }

                GrowTree(tree, location, tileLocation);
            }
        }

        /// <summary>
        /// Adjusts tree and mushroom tree growth depending on settings like growing them in the winter.
        /// Uses a slightly edited version of the growth call of StardewValley.TerrainFeatures.Tree.dayUpdate.
        /// </summary>
        /// <param name="tree">current tree</param>
        /// <param name="location">ingame location (e.g. farm)</param>
        /// <param name="tileLocation">position in said location</param>
        public void GrowTree(Tree tree, GameLocation location, Vector2 tileLocation)
        {
            Rectangle growthRect = new Rectangle((int)((tileLocation.X - 1f) * 64f), (int)((tileLocation.Y - 1f) * 64f), 192, 192);

            string s = location.doesTileHaveProperty((int)tileLocation.X, (int)tileLocation.Y, "NoSpawn", "Back");
            if (s != null && (s.Equals("All") || s.Equals("Tree") || s.Equals("True")))
            {
                return;
            }

            if (tree.growthStage == 4 || treeOverhaulConfig.StopShadeSaplingGrowth)
            {
                using (NetDictionary<Vector2, TerrainFeature, NetRef<TerrainFeature>, SerializableDictionary<Vector2, TerrainFeature>, NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>>>.PairsCollection.Enumerator enumerator = location.terrainFeatures.Pairs.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        KeyValuePair<Vector2, TerrainFeature> t = enumerator.Current;
                        if (t.Value is Tree && !t.Value.Equals(this) && ((Tree)t.Value).growthStage >= 5 && t.Value.getBoundingBox(t.Key).Intersects(growthRect))
                        {
                            return;
                        }
                    }
                }
            }

            if (tree.growthStage == 0 && location.objects.ContainsKey(tileLocation))
            {
                return;
            }

            if (Game1.random.NextDouble() < 0.2 || tree.fertilized.Value)
            {
                tree.growthStage.Set(tree.growthStage.Value + 1);
            }
        }

        /// <summary>
        /// Reverts the mushroom stump back into a tree exactly like its done in StardewValley.TerrainFeatures.Tree.dayUpdate.
        /// That means chopping a mushroom tree and going to bed without removing the stump causes an exploit but I dont see a reason to fix that.
        /// </summary>
        /// <param name="tree">current mushroom tree</param>
        public void FixMushroomStump(Tree tree)
        {
            if (Game1.IsWinter)
            {
                if (tree.stump.Value)
                {
                    tree.stump.Set(false);
                    tree.health.Set(10f);
                }
            }
        }

        /// <summary>
        /// Calls growth methods depending on the current settings.
        /// </summary>
        /// <param name="fruittree">current fruit tree</param>
        /// <param name="location">ingame location (e.g. farm)</param>
        /// <param name="tileLocation">position in said location</param>
        public void CalculateFruitTreeGrowth(FruitTree fruittree, GameLocation location, Vector2 tileLocation)
        {
            if (treeOverhaulConfig.FruitTreesDontGrowInWinter)
            {
                if (Game1.IsWinter)
                {
                    if (location.IsGreenhouse)
                    {
                        CheckForExtraGrowth(fruittree, location, tileLocation);
                    }
                    else
                    {
                        // reverts the growth from the base code
                        GrowFruitTree(fruittree, location, tileLocation, "plus");
                    }
                }
                else
                {
                    CheckForExtraGrowth(fruittree, location, tileLocation);
                }
            }
            else
            {
                CheckForExtraGrowth(fruittree, location, tileLocation);
            }
        }

        /// <summary>
        /// Applies growth speedup (200%) or growth slowdown (50%) to the a fruit tree depending on the settings.
        /// </summary>
        /// <param name="fruittree">current fruit tree</param>
        /// <param name="location">ingame location (e.g. farm)</param>
        /// <param name="tileLocation">position in said location</param>
        public void CheckForExtraGrowth(FruitTree fruittree, GameLocation location, Vector2 tileLocation)
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

        /// <summary>
        /// Checks days until mature and updates growth stage accordingly.
        /// Taken from StardewValley.TerrainFeatures.FruitTree.dayUpdate.
        /// </summary>
        /// <param name="fruittree">current fruittree</param>
        public void UpdateGrowthStage(FruitTree fruittree)
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

        /// <summary>
        /// Grows the fruittree using the vanilla constraints taken from StardewValley.TerrainFeatures.FruitTree.dayUpdate.
        /// </summary>
        /// <param name="fruittree">current fruit tree</param>
        /// <param name="location">ingame location (e.g. farm)</param>
        /// <param name="tileLocation">position in said location</param>
        /// <param name="change">use the string minus to increase growth, plus to decrease growth, something else for nothing</param>
        public void GrowFruitTree(FruitTree fruittree, GameLocation location, Vector2 tileLocation, string change)
        {
            bool foundSomething = false;
            foreach (Vector2 v in Utility.getSurroundingTileLocationsArray(tileLocation))
            {
                bool isClearHoeDirt = location.terrainFeatures.ContainsKey(v) && location.terrainFeatures[v] is HoeDirt && (location.terrainFeatures[v] as HoeDirt).crop == null;
                if (location.isTileOccupied(v, "", true) && !isClearHoeDirt)
                {
                    Object o = location.getObjectAt((int)v.X, (int)v.Y);
                    if (o == null || o.isPassable())
                    {
                        foundSomething = true;
                        break;
                    }
                }
            }

            if (!foundSomething)
            {
                // grow tree (reduce days until mature)
                if (change == "minus")
                {
                    fruittree.daysUntilMature.Set(fruittree.daysUntilMature.Value - 1);
                }

                // revert tree growth (increase days until mature)
                if (change == "plus")
                {
                    fruittree.daysUntilMature.Set(fruittree.daysUntilMature.Value + 1);
                }

                UpdateGrowthStage(fruittree);
            }
        }
    }

    /// <summary>
    /// Config file for the mod
    /// </summary>
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