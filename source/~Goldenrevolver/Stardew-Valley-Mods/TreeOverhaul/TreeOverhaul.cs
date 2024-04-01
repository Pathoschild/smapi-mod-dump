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
    using StardewModdingAPI;
    using StardewModdingAPI.Events;
    using StardewValley;
    using StardewValley.GameData.WildTrees;
    using StardewValley.TerrainFeatures;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Tree overhaul mod class that changes the behavior of wild trees.
    /// </summary>
    public class TreeOverhaul : Mod
    {
        internal TreeOverhaulConfig Config { get; set; }

        /// <summary>
        /// The mod entry point, called after the mod is first loaded.
        /// Loads config file and subscribes methods to some of the events
        /// </summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<TreeOverhaulConfig>();

            TreeOverhaulConfig.VerifyConfigValues(Config, this);

            Helper.Events.GameLoop.DayStarted += delegate { OnDayStarted(); };

            Helper.Events.GameLoop.GameLaunched += delegate { TreeOverhaulConfig.SetUpModConfigMenu(Config, this); };

            Helper.Events.Content.AssetRequested += OnAssetRequested;

            Patcher.PatchAll(this);
        }

        /// <summary>
        /// Small helper method to log to the console because I keep forgetting the signature
        /// </summary>
        /// <param name="o">the object I want to log as a string</param>
        public void DebugLog(object o)
        {
            this.Monitor.Log(o == null ? "null" : o.ToString(), LogLevel.Debug);
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/WildTrees"))
            {
                e.Edit((asset) =>
                {
                    IDictionary<string, WildTreeData> data = asset.AsDictionary<string, WildTreeData>().Data;

                    var normalTrees = new string[] { Tree.bushyTree, Tree.leafyTree, Tree.pineTree, Tree.mahoganyTree, Tree.greenRainTreeBushy, Tree.greenRainTreeLeafy, Tree.greenRainTreeFern, Tree.mysticTree };

                    foreach (var entry in data)
                    {
                        if (entry.Key == Tree.mushroomTree)
                        {
                            var mushroomTreeData = entry.Value;

                            if (Config.MushroomTreesGrowInWinter)
                            {
                                mushroomTreeData.GrowsInWinter = true;
                                mushroomTreeData.IsStumpDuringWinter = false;
                                FixWinterTapper(mushroomTreeData);
                            }

                            if (Config.CustomMushroomTreeSeedOnShakeChance >= 0)
                            {
                                mushroomTreeData.SeedOnShakeChance = Math.Clamp(Config.CustomMushroomTreeSeedOnShakeChance / 100f, 0f, 1f);
                            }

                            if (Config.CustomMushroomTreeSeedOnChopChance >= 0)
                            {
                                mushroomTreeData.SeedOnChopChance = Math.Clamp(Config.CustomMushroomTreeSeedOnChopChance / 100f, 0f, 1f);
                            }

                            if (Config.CustomMushroomTreeSpawnSeedNearbyChance >= 0)
                            {
                                mushroomTreeData.SeedSpreadChance = Math.Clamp(Config.CustomMushroomTreeSpawnSeedNearbyChance / 100f, 0f, 1f);
                            }

                            if (Config.CustomMushroomTreeGrowthChance >= 0)
                            {
                                mushroomTreeData.GrowthChance = Math.Clamp(Config.CustomMushroomTreeGrowthChance / 100f, 0f, 1f);
                            }
                        }
                        else if (entry.Key is Tree.palmTree or Tree.palmTree2)
                        {
                            var palmTreeData = entry.Value;

                            if (Config.NormalTreesGrowInWinter)
                            {
                                palmTreeData.GrowsInWinter = true;

                                if (palmTreeData.IsStumpDuringWinter)
                                {
                                    palmTreeData.IsStumpDuringWinter = false;
                                    FixWinterTapper(palmTreeData);
                                }
                            }

                            if (Config.CustomPalmTreeSeedOnShakeChance >= 0)
                            {
                                palmTreeData.SeedOnShakeChance = Math.Clamp(Config.CustomPalmTreeSeedOnShakeChance / 100f, 0f, 1f);
                            }

                            if (Config.CustomPalmTreeSeedOnChopChance >= 0)
                            {
                                palmTreeData.SeedOnChopChance = Math.Clamp(Config.CustomPalmTreeSeedOnChopChance / 100f, 0f, 1f);
                            }

                            if (Config.CustomPalmTreeSpawnSeedNearbyChance >= 0)
                            {
                                palmTreeData.SeedSpreadChance = Math.Clamp(Config.CustomPalmTreeSpawnSeedNearbyChance / 100f, 0f, 1f);
                            }

                            if (Config.CustomPalmTreeGrowthChance >= 0)
                            {
                                palmTreeData.GrowthChance = Math.Clamp(Config.CustomPalmTreeGrowthChance / 100f, 0f, 1f);
                            }
                        }
                        else
                        {
                            bool isNormalTree = normalTrees.Contains(entry.Key);

                            var treeData = entry.Value;

                            if ((isNormalTree && Config.NormalTreesGrowInWinter)
                            || (!isNormalTree && Config.CustomTreesGrowInWinter))
                            {
                                treeData.GrowsInWinter = true;

                                if (treeData.IsStumpDuringWinter)
                                {
                                    treeData.IsStumpDuringWinter = false;
                                    FixWinterTapper(treeData);
                                }
                            }

                            if (isNormalTree || Config.CustomChancesAlsoAffectCustomTrees)
                            {
                                if (Config.CustomTreeGrowthChance >= 0)
                                {
                                    treeData.GrowthChance = Math.Clamp(Config.CustomTreeGrowthChance / 100f, 0f, 1f);
                                }

                                if (entry.Key == Tree.mysticTree)
                                {
                                    continue;
                                }

                                if (Config.CustomSeedOnShakeChance >= 0)
                                {
                                    treeData.SeedOnShakeChance = Math.Clamp(Config.CustomSeedOnShakeChance / 100f, 0f, 1f);
                                }

                                if (Config.CustomSeedOnChopChance >= 0)
                                {
                                    treeData.SeedOnChopChance = Math.Clamp(Config.CustomSeedOnChopChance / 100f, 0f, 1f);
                                }

                                if (Config.CustomSpawnSeedNearbyChance >= 0)
                                {
                                    treeData.SeedSpreadChance = Math.Clamp(Config.CustomSpawnSeedNearbyChance / 100f, 0f, 1f);
                                }
                            }
                        }
                    }

                    if (data.TryGetValue(Tree.bushyTree, out var oakTreeData))
                    {
                        if (Config.BuffMahoganyTreeGrowthChance && data.TryGetValue(Tree.mahoganyTree, out var mahoganyTreeData))
                        {
                            mahoganyTreeData.GrowthChance = oakTreeData.GrowthChance;
                            mahoganyTreeData.FertilizedGrowthChance = oakTreeData.FertilizedGrowthChance;
                        }
                        if (Config.BuffMysticTreeGrowthChance && data.TryGetValue(Tree.mysticTree, out var mythicTreeData))
                        {
                            mythicTreeData.GrowthChance = oakTreeData.GrowthChance;
                            mythicTreeData.FertilizedGrowthChance = oakTreeData.FertilizedGrowthChance;
                        }
                    }
                }, AssetEditPriority.Late);
            }
        }

        private static void FixWinterTapper(WildTreeData treeData)
        {
            foreach (var tapItem in treeData.TapItems)
            {
                var queries = GameStateQuery.SplitRaw(tapItem.Condition).Where(
                    (q) => q.ToLower() != "!LOCATION_SEASON Target Winter".ToLower());

                tapItem.Condition = string.Join(',', queries);
            }
        }

        private void OnDayStarted()
        {
            if (!Context.IsMainPlayer)
            {
                return;
            }

            Utility.ForEachLocation(delegate (GameLocation location)
            {
                foreach (var terrainfeature in location.terrainFeatures.Pairs)
                {
                    if (terrainfeature.Value is Tree tree)
                    {
                        if (tree.treeType.Value == Tree.mushroomTree && Config.MushroomTreesGrowInWinter && tree.stump.Value && Helper.Reflection.GetField<Season?>(tree, "localSeason").GetValue() == Season.Winter)
                        {
                            FixMushroomStump(tree, location);
                        }

                        if (tree.treeType.Value == Tree.mahoganyTree)
                        {
                            tree.modData.Remove($"{this.ModManifest.UniqueID}/growthStage");
                        }
                    }
                }

                return true;
            });
        }

        /// <summary>
        /// Reverts the mushroom stump back into a tree exactly like its done in StardewValley.TerrainFeatures.Tree.dayUpdate, but only if it's not a chopped down tree
        /// Then updates the tapper, so it works again
        /// </summary>
        /// <param name="tree">current mushroom tree</param>
        private void FixMushroomStump(Tree tree, GameLocation location)
        {
            var shakeRotation = Helper.Reflection.GetField<float>(tree, "shakeRotation");

            // if the value is higher than this, the game considers the tree as falling or having fallen
            if (Math.Abs(shakeRotation.GetValue()) <= Math.PI / 2.0)
            {
                tree.stump.Value = false;
                tree.health.Value = 10f;
                shakeRotation.SetValue(0f);
            }

            if (tree.stump.Value)
            {
                return;
            }

            if (tree.tapped.Value)
            {
                StardewValley.Object tile_object = location.getObjectAtTile((int)tree.Tile.X, (int)tree.Tile.Y, false);

                if (tile_object != null && tile_object.IsTapper() && tile_object.heldObject.Value == null)
                {
                    tree.UpdateTapperProduct(tile_object);
                }
            }
        }
    }

    /// <summary>
    /// Extension methods for IGameContentHelper.
    /// </summary>
    public static class GameContentHelperExtensions
    {
        /// <summary>
        /// Invalidates both an asset and the locale-specific version of an asset.
        /// </summary>
        /// <param name="helper">The game content helper.</param>
        /// <param name="assetName">The (string) asset to invalidate.</param>
        /// <returns>if something was invalidated.</returns>
        public static bool InvalidateCacheAndLocalized(this IGameContentHelper helper, string assetName)
            => helper.InvalidateCache(assetName)
                | (helper.CurrentLocaleConstant != LocalizedContentManager.LanguageCode.en && helper.InvalidateCache(assetName + "." + helper.CurrentLocale));
    }
}