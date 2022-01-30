/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace TreeOverhaul
{
    using Microsoft.Xna.Framework;
    using Netcode;
    using StardewModdingAPI;
    using StardewModdingAPI.Events;
    using StardewValley;
    using StardewValley.Network;
    using StardewValley.TerrainFeatures;
    using StardewValley.Tools;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Tree overhaul mod class that changes the behavior of trees and fruit trees.
    /// </summary>
    public class TreeOverhaul : Mod
    {
        /// <summary>
        /// The terrain features (should be only trees and fruit trees) that are currently tracked by the 'Save Sprouts' feature
        /// </summary>
        private readonly List<TerrainFeature> terrainFeatures = new();

        /// <summary>
        /// Whether the 'Save Sprouts' feature is currently waiting for the tool animation to finish
        /// </summary>
        private bool waitingForAnimationToFinish = false;

        /// <summary>
        /// The current config file
        /// </summary>
        private TreeOverhaulConfig config;

        /// <summary>
        /// The mod entry point, called after the mod is first loaded.
        /// Loads config file and subscribes methods to some of the events
        /// </summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            config = Helper.ReadConfig<TreeOverhaulConfig>();

            TreeOverhaulConfig.VerifyConfigValues(config, this);

            Helper.Events.GameLoop.DayStarted += delegate { OnDayStarted(); };

            // when the day ends reset all relevant data for 'SafeSprouts' feature just to be sure. This could happen if the day ends in the middle of an animation.
            Helper.Events.GameLoop.DayEnding += delegate
            {
                ResetSprouts();
                SaveMahoganyTreeGrowth();
            };

            // when we return to title reset all relevant data for 'SafeSprouts' feature just to be sure.
            Helper.Events.GameLoop.ReturnedToTitle += delegate { ResetSprouts(); };

            Helper.Events.GameLoop.UpdateTicked += delegate { CheckForToolUseToSaveSprouts(); };

            Helper.Events.Input.ButtonPressed += CheckForWeaponUseToSaveSprouts;

            Helper.Events.GameLoop.GameLaunched += delegate { TreeOverhaulConfig.SetUpModConfigMenu(config, this); };
        }

        /// <summary>
        /// Small helper method to log to the console because I keep forgetting the signature
        /// </summary>
        /// <param name="o">the object I want to log as a string</param>
        public void DebugLog(object o)
        {
            this.Monitor.Log(o == null ? "null" : o.ToString(), LogLevel.Debug);
        }

        /// <summary>
        /// Should be called when day ends to save the growth stages for every mahogany tree in the mod data attribute
        /// </summary>
        private void SaveMahoganyTreeGrowth()
        {
            if (!Context.IsMainPlayer || !config.BuffMahoganyTrees)
            {
                return;
            }

            foreach (var location in Game1.locations)
            {
                foreach (var terrainfeature in location.terrainFeatures.Pairs)
                {
                    if (terrainfeature.Value is Tree tree)
                    {
                        if (tree.treeType.Value == Tree.mahoganyTree)
                        {
                            tree.modData[$"{this.ModManifest.UniqueID}/growthStage"] = tree.growthStage.Value.ToString();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks for button input for scythe (or optionally melee weapon) use and makes all sprouts invincible without knowing if the button click actually triggered the animation
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="args">the button pressed event args, useful to know which button was pressed</param>
        private void CheckForWeaponUseToSaveSprouts(object sender, ButtonPressedEventArgs args)
        {
            if (!Context.IsWorldReady || waitingForAnimationToFinish || config.SaveSprouts <= 1)
            {
                return;
            }

            if (args.Button.IsUseToolButton() || (config.SaveSprouts > 2 && args.Button.IsActionButton()))
            {
                var tool = Game1.player.CurrentTool;
                if (tool is MeleeWeapon weapon && (config.SaveSprouts > 2 || weapon.isScythe(-1)))
                {
                    foreach (var terrainfeature in Game1.currentLocation.terrainFeatures.Pairs)
                    {
                        if (terrainfeature.Value is Tree)
                        {
                            waitingForAnimationToFinish = true;

                            SaveSprout(terrainfeature.Value);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Every frame check if the player (host or farm hand) is using a tool and make the sapling invulnerable during the animation. Make it vulnerable afterwards.
        /// </summary>
        private void CheckForToolUseToSaveSprouts()
        {
            if (!Context.IsWorldReady || config.SaveSprouts <= 0)
            {
                return;
            }

            if (!waitingForAnimationToFinish && Game1.player.UsingTool)
            {
                Tool t = Game1.player.CurrentTool;

                if (t is Pickaxe || t is Hoe)
                {
                    foreach (var terrainfeature in Game1.currentLocation.terrainFeatures.Pairs)
                    {
                        if (terrainfeature.Value is Tree || terrainfeature.Value is FruitTree)
                        {
                            waitingForAnimationToFinish = true;

                            SaveSprout(terrainfeature.Value);
                        }
                    }
                }
            }
            else if (waitingForAnimationToFinish && !Game1.player.UsingTool)
            {
                ResetSprouts();
            }
        }

        /// <summary>
        /// Save a potential sprout from getting destroyed.
        /// </summary>
        /// <param name="feature">the potential sprout to save</param>
        private void SaveSprout(TerrainFeature feature)
        {
            if (feature != null)
            {
                if (feature is Tree tree)
                {
                    if (tree.growthStage.Value == 1 || tree.growthStage.Value == 2)
                    {
                        terrainFeatures.Add(tree);

                        // tapped trees can't be destroyed
                        tree.tapped.Value = true;
                    }
                }
                else if (feature is FruitTree fruitTree)
                {
                    if (fruitTree.growthStage.Value >= 0 && fruitTree.growthStage.Value <= 2)
                    {
                        terrainFeatures.Add(fruitTree);

                        // dead fruit trees can't be destroyed
                        fruitTree.health.Value = -99f;
                    }
                }
            }
        }

        /// <summary>
        /// For each sprout we made invulnerable set them to vulnerable again and reset the local variables
        /// </summary>
        private void ResetSprouts()
        {
            foreach (var feature in terrainFeatures)
            {
                if (feature != null)
                {
                    if (feature is Tree tree)
                    {
                        if (tree.growthStage.Value == 1 || tree.growthStage.Value == 2)
                        {
                            tree.tapped.Value = false;
                        }
                    }
                    else if (feature is FruitTree fruitTree)
                    {
                        if (fruitTree.growthStage.Value >= 0 && fruitTree.growthStage.Value <= 2)
                        {
                            fruitTree.health.Value = 10f;
                        }
                    }
                }
            }

            terrainFeatures.Clear();
            waitingForAnimationToFinish = false;
        }

        /// <summary>
        /// Iterates through every tree and fruit tree and applies growth changes according to settings.
        /// Raised after the game begins a new day (including when the player loads a save).
        /// </summary>
        private void OnDayStarted()
        {
            if (!Context.IsMainPlayer)
            {
                return;
            }

            foreach (var location in Game1.locations)
            {
                if (IsWinter(location) && config.MushroomTreesGrowInWinter)
                {
                    foreach (var terrainfeature in location.terrainFeatures.Pairs)
                    {
                        if (terrainfeature.Value is Tree tree && tree.treeType.Value == Tree.mushroomTree)
                        {
                            FixMushroomStump(tree, location);
                        }
                    }
                }

                foreach (var terrainfeature in location.terrainFeatures.Pairs)
                {
                    switch (terrainfeature.Value)
                    {
                        case Tree tree:
                            CalculateTreeGrowth(tree, location, terrainfeature.Key);
                            RevertSaplingGrowthInShade(tree, location, terrainfeature.Key);
                            RecalculateSeedChance(tree);
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
        /// </summary>
        /// <param name="tree">current sapling</param>
        /// <param name="location">ingame location (e.g. farm)</param>
        /// <param name="tileLocation">position in said location</param>
        private void RevertSaplingGrowthInShade(Tree tree, GameLocation location, Vector2 tileLocation)
        {
            // stop if it's a palm tree
            if (tree.treeType.Value == Tree.palmTree || tree.treeType.Value == Tree.palmTree2)
            {
                return;
            }

            if (tree.growthStage.Value != 1 || !config.StopShadeSaplingGrowth)
            {
                return;
            }

            if (IsGrowthBlocked(tree, location, tileLocation))
            {
                tree.growthStage.Value = 0;
            }
        }

        /// <summary>
        /// Helper method for the new way to check if the location is considered to be in the season winter
        /// </summary>
        /// <param name="location">the location to check if it is winter in it</param>
        /// <returns>whether it's winter in that location</returns>
        private static bool IsWinter(GameLocation location)
        {
            return Game1.GetSeasonForLocation(location).Equals("winter");
        }

        /// <summary>
        /// Calls tree and mushroom tree growth methods depending on settings like growing them in the winter.
        /// </summary>
        /// <param name="tree">current tree</param>
        /// <param name="location">ingame location (e.g. farm)</param>
        /// <param name="tileLocation">position in said location</param>
        private void CalculateTreeGrowth(Tree tree, GameLocation location, Vector2 tileLocation)
        {
            // stop if it's a palm tree
            if (tree.treeType.Value == Tree.palmTree || tree.treeType.Value == Tree.palmTree2)
            {
                return;
            }

            // Cancels out with "if (!Game1.GetSeasonForLocation(this.currentLocation).Equals("winter") || this.treeType == 6 || this.treeType == 9 || environment.CanPlantTreesHere(-1, (int)tileLocation.X, (int)tileLocation.Y) || this.fertilized.Value)" from StardewValley.TerrainFeatures.Tree.dayUpdate
            // so we get exactly one growth call if the config says to grow in winter
            if (IsWinter(location) && !location.CanPlantTreesHere(-1, (int)tileLocation.X, (int)tileLocation.Y) && !tree.fertilized.Value)
            {
                if (tree.treeType.Value != Tree.mushroomTree && config.NormalTreesGrowInWinter)
                {
                    GrowTree(tree, location, tileLocation);
                }

                if (tree.treeType.Value == Tree.mushroomTree && config.MushroomTreesGrowInWinter)
                {
                    GrowTree(tree, location, tileLocation);
                }
            }
            else
            {
                // fixes stage 4 trees that should have grown and buffs mahogany trees if config says so
                FixAlreadyGrownTree(tree, location, tileLocation);
            }

            if (config.FasterNormalTreeGrowth)
            {
                if (IsWinter(location))
                {
                    if (tree.treeType.Value != Tree.mushroomTree && (config.NormalTreesGrowInWinter || location.CanPlantTreesHere(-1, (int)tileLocation.X, (int)tileLocation.Y) || tree.fertilized.Value))
                    {
                        GrowTree(tree, location, tileLocation);
                    }

                    if (tree.treeType.Value == Tree.mushroomTree && (config.MushroomTreesGrowInWinter || location.CanPlantTreesHere(-1, (int)tileLocation.X, (int)tileLocation.Y) || tree.fertilized.Value))
                    {
                        GrowTree(tree, location, tileLocation);
                    }
                }
                else
                {
                    GrowTree(tree, location, tileLocation);
                }
            }
        }

        /// <summary>
        /// Tries to grow state 4 trees that are only blocked by stumps and recalculates mahogany tree growth
        /// </summary>
        /// <param name="tree">current tree</param>
        /// <param name="location">ingame location (e.g. farm)</param>
        /// <param name="tileLocation">position in said location</param>
        private void FixAlreadyGrownTree(Tree tree, GameLocation location, Vector2 tileLocation)
        {
            string s = location.doesTileHaveProperty((int)tileLocation.X, (int)tileLocation.Y, "NoSpawn", "Back");
            if (s != null && (s.Equals("All") || s.Equals("Tree") || s.Equals("True")))
            {
                return;
            }

            if (config.GrowthIgnoresStumps && tree.growthStage.Value == 4 && (!config.BuffMahoganyTrees || tree.treeType.Value != Tree.mahoganyTree) && IsGrowthOnlyBlockedByStump(tree, location, tileLocation))
            {
                // mahogany trees have a lower chance to grow for some reason
                if (tree.treeType.Value == Tree.mahoganyTree)
                {
                    if (Game1.random.NextDouble() < 0.15 || (tree.fertilized.Value && Game1.random.NextDouble() < 0.6))
                    {
                        tree.growthStage.Value++;
                    }
                }
                else
                {
                    if (Game1.random.NextDouble() < 0.2 || tree.fertilized.Value)
                    {
                        tree.growthStage.Value++;
                    }
                }

                return;
            }

            if (tree.growthStage.Value == 0 && location.objects.ContainsKey(tileLocation))
            {
                return;
            }

            if (config.BuffMahoganyTrees && tree.treeType.Value == Tree.mahoganyTree)
            {
                tree.modData.TryGetValue($"{this.ModManifest.UniqueID}/growthStage", out string moddata);

                if (!string.IsNullOrEmpty(moddata))
                {
                    int yesterdaysGrowthStage = int.Parse(moddata);

                    if (!(Game1.random.NextDouble() < 0.2 || tree.fertilized.Value) || ((yesterdaysGrowthStage == 4 || (yesterdaysGrowthStage < 4 && config.StopShadeSaplingGrowth)) && IsGrowthBlocked(tree, location, tileLocation)))
                    {
                        tree.growthStage.Value = yesterdaysGrowthStage;
                    }
                    else
                    {
                        tree.growthStage.Value = yesterdaysGrowthStage + 1;
                    }
                }
            }
        }

        /// <summary>
        /// Uses a slightly edited version of the seed chance calculation call in StardewValley.TerrainFeatures.Tree.dayUpdate.
        /// </summary>
        /// <param name="tree">the tree to recalculate the chance of</param>
        private void RecalculateSeedChance(Tree tree)
        {
            // stop if it's a palm tree
            if (tree.treeType.Value == Tree.palmTree || tree.treeType.Value == Tree.palmTree2)
            {
                return;
            }

            tree.hasSeed.Value = false;

            if (tree.growthStage.Value < 5)
            {
                return;
            }

            float defaultSeedChance = 0.05f;
            float seedChance = config.ShakingSeedChance;

            if (seedChance < 0f)
            {
                seedChance = defaultSeedChance;
            }
            else
            {
                seedChance /= 100f;
            }

            if (Game1.random.NextDouble() < seedChance)
            {
                tree.hasSeed.Value = true;
            }
        }

        /// <summary>
        /// Uses a slightly edited version of the growth call of StardewValley.TerrainFeatures.Tree.dayUpdate.
        /// </summary>
        /// <param name="tree">current tree</param>
        /// <param name="location">ingame location (e.g. farm)</param>
        /// <param name="tileLocation">position in said location</param>
        /// <returns>whether the tree's growth is only blocked by a stump and not a still fully grown stage</returns>
        private static bool IsGrowthOnlyBlockedByStump(Tree tree, GameLocation location, Vector2 tileLocation)
        {
            var growthRect = new Rectangle((int)((tileLocation.X - 1f) * 64f), (int)((tileLocation.Y - 1f) * 64f), 192, 192);

            using NetDictionary<Vector2, TerrainFeature, NetRef<TerrainFeature>, SerializableDictionary<Vector2, TerrainFeature>, NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>>>.PairsCollection.Enumerator enumerator = location.terrainFeatures.Pairs.GetEnumerator();
            bool foundStump = false;

            while (enumerator.MoveNext())
            {
                KeyValuePair<Vector2, TerrainFeature> t = enumerator.Current;
                if (t.Value is Tree treeValue && !t.Value.Equals(tree) && treeValue.growthStage.Value >= 5 && t.Value.getBoundingBox(t.Key).Intersects(growthRect))
                {
                    if (treeValue.stump.Value)
                    {
                        foundStump = true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return foundStump;
        }

        /// <summary>
        /// Uses a slightly edited version of the growth call of StardewValley.TerrainFeatures.Tree.dayUpdate.
        /// </summary>
        /// <param name="tree">current tree</param>
        /// <param name="location">ingame location (e.g. farm)</param>
        /// <param name="tileLocation">position in said location</param>
        /// <returns>whether the tree's growth is blocked by a stage 5 tree, depending on config even a stump is enough</returns>
        private bool IsGrowthBlocked(Tree tree, GameLocation location, Vector2 tileLocation)
        {
            Rectangle growthRect = new((int)((tileLocation.X - 1f) * 64f), (int)((tileLocation.Y - 1f) * 64f), 192, 192);

            using NetDictionary<Vector2, TerrainFeature, NetRef<TerrainFeature>, SerializableDictionary<Vector2, TerrainFeature>, NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>>>.PairsCollection.Enumerator enumerator = location.terrainFeatures.Pairs.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<Vector2, TerrainFeature> t = enumerator.Current;
                if (t.Value is Tree treeValue && !t.Value.Equals(tree) && treeValue.growthStage.Value >= 5 && (!treeValue.stump.Value || !config.GrowthIgnoresStumps) && t.Value.getBoundingBox(t.Key).Intersects(growthRect))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Adjusts tree and mushroom tree growth depending on settings like growing them in the winter.
        /// </summary>
        /// <param name="tree">current tree</param>
        /// <param name="location">ingame location (e.g. farm)</param>
        /// <param name="tileLocation">position in said location</param>
        private void GrowTree(Tree tree, GameLocation location, Vector2 tileLocation)
        {
            string s = location.doesTileHaveProperty((int)tileLocation.X, (int)tileLocation.Y, "NoSpawn", "Back");
            if (s != null && (s.Equals("All") || s.Equals("Tree") || s.Equals("True")))
            {
                return;
            }

            if ((tree.growthStage.Value == 4 || (tree.growthStage.Value < 4 && config.StopShadeSaplingGrowth)) && IsGrowthBlocked(tree, location, tileLocation))
            {
                return;
            }

            if (tree.growthStage.Value == 0 && location.objects.ContainsKey(tileLocation))
            {
                return;
            }

            // mahogany trees have a lower chance to grow for some reason
            if (tree.treeType.Value == Tree.mahoganyTree)
            {
                if (config.BuffMahoganyTrees)
                {
                    if (Game1.random.NextDouble() < 0.2 || tree.fertilized.Value)
                    {
                        tree.growthStage.Value++;
                    }
                }
                else
                {
                    if (Game1.random.NextDouble() < 0.15 || (tree.fertilized.Value && Game1.random.NextDouble() < 0.6))
                    {
                        tree.growthStage.Value++;
                    }
                }
            }
            else if (Game1.random.NextDouble() < 0.2 || tree.fertilized.Value)
            {
                tree.growthStage.Value++;
            }
        }

        /// <summary>
        /// Reverts the mushroom stump back into a tree exactly like its done in StardewValley.TerrainFeatures.Tree.dayUpdate, but only if it's not a chopped down tree
        /// </summary>
        /// <param name="tree">current mushroom tree</param>
        private void FixMushroomStump(Tree tree, GameLocation location)
        {
            var shakeRotation = Helper.Reflection.GetField<float>(tree, "shakeRotation");

            // if the value is higher than this the game considers the tree as falling or having fallen
            if (Math.Abs(shakeRotation.GetValue()) < 1.5707963267948966)
            {
                tree.stump.Value = false;
                tree.health.Value = 10f;
                shakeRotation.SetValue(0f);
            }

            if (tree.stump.Value)
            {
                return;
            }

            StardewValley.Object tapper_instance = location.getObjectAtTile((int)tree.currentTileLocation.X, (int)tree.currentTileLocation.Y);

            if (tapper_instance?.bigCraftable.Value == true && (tapper_instance.ParentSheetIndex is 105 or 264))
            {
                int daysUntilSpring = new WorldDate(Game1.year + 1, "spring", 1).TotalDays - Game1.Date.TotalDays;
                int minutesUntilSpring = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, daysUntilSpring);
                int minutesUntilTomorrow = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay);

                if (tapper_instance.MinutesUntilReady == minutesUntilSpring)
                {
                    // red mushroom
                    if (tapper_instance.heldObject.Value.ParentSheetIndex is 420)
                    {
                        tapper_instance.MinutesUntilReady = 0;
                    }
                    else if (minutesUntilSpring > minutesUntilTomorrow)
                    {
                        tapper_instance.MinutesUntilReady = minutesUntilTomorrow;
                    }
                }
            }
        }

        /// <summary>
        /// Calls growth methods depending on the current settings.
        /// </summary>
        /// <param name="fruittree">current fruit tree</param>
        /// <param name="location">ingame location (e.g. farm)</param>
        /// <param name="tileLocation">position in said location</param>
        private void CalculateFruitTreeGrowth(FruitTree fruittree, GameLocation location, Vector2 tileLocation)
        {
            if (config.FruitTreesDontGrowInWinter)
            {
                if (IsWinter(location))
                {
                    if (location.IsGreenhouse)
                    {
                        CheckForExtraGrowth(fruittree, location, tileLocation);
                    }
                    else
                    {
                        // reverts the growth from the base code
                        GrowFruitTree(fruittree, location, tileLocation, false);
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
        /// Applies growth speedup (200%) or growth slowdown (50%) to a fruit tree depending on the settings.
        /// </summary>
        /// <param name="fruittree">current fruit tree</param>
        /// <param name="location">ingame location (e.g. farm)</param>
        /// <param name="tileLocation">position in said location</param>
        private void CheckForExtraGrowth(FruitTree fruittree, GameLocation location, Vector2 tileLocation)
        {
            if (config.FruitTreeGrowth == 1)
            {
                GrowFruitTree(fruittree, location, tileLocation, true);
            }

            if (config.FruitTreeGrowth == 2)
            {
                // odd day
                if (Game1.dayOfMonth % 2 == 1)
                {
                    GrowFruitTree(fruittree, location, tileLocation, false);
                }
            }
        }

        /// <summary>
        /// Checks days until mature and updates growth stage accordingly.
        /// Taken from StardewValley.TerrainFeatures.FruitTree.dayUpdate.
        /// </summary>
        /// <param name="fruittree">current fruit tree</param>
        private static void UpdateGrowthStage(FruitTree fruittree)
        {
            if (fruittree.daysUntilMature.Value > 28)
            {
                fruittree.daysUntilMature.Value = 28;
            }

            if (fruittree.daysUntilMature.Value <= 0)
            {
                fruittree.growthStage.Value = 4;
            }
            else if (fruittree.daysUntilMature.Value <= 7)
            {
                fruittree.growthStage.Value = 3;
            }
            else if (fruittree.daysUntilMature.Value <= 14)
            {
                fruittree.growthStage.Value = 2;
            }
            else if (fruittree.daysUntilMature.Value <= 21)
            {
                fruittree.growthStage.Value = 1;
            }
            else
            {
                fruittree.growthStage.Value = 0;
            }
        }

        /// <summary>
        /// Grows the fruit tree using the vanilla constraints taken from StardewValley.TerrainFeatures.FruitTree.dayUpdate.
        /// </summary>
        /// <param name="fruittree">current fruit tree</param>
        /// <param name="location">ingame location (e.g. farm)</param>
        /// <param name="tileLocation">position in said location</param>
        /// <param name="increaseGrowthElseRevertGrowth">if true reduce days until mature (grow), otherwise increase days (revert growth)</param>
        private static void GrowFruitTree(FruitTree fruittree, GameLocation location, Vector2 tileLocation, bool increaseGrowthElseRevertGrowth)
        {
            bool foundSomething = FruitTree.IsGrowthBlocked(tileLocation, location);

            // check if the tree can grow and if it's not fully matured. the base game reduces daysUntilMatured for fully grown trees to determine the tree age.
            if (!foundSomething && fruittree.growthStage.Value != 4)
            {
                if (increaseGrowthElseRevertGrowth)
                {
                    // grow tree (reduce days until mature)
                    fruittree.daysUntilMature.Value--;
                }
                else
                {
                    // revert tree growth (increase days until mature)
                    fruittree.daysUntilMature.Value++;
                }

                UpdateGrowthStage(fruittree);
            }
        }
    }
}