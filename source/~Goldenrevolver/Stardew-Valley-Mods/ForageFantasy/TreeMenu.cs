/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace ForageFantasy
{
    using HarmonyLib;
    using StardewValley;
    using StardewValley.GameData.FruitTrees;
    using StardewValley.TerrainFeatures;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using StardewObject = StardewValley.Object;

    public class TreeMenu : BaseMenu
    {
        private readonly TerrainFeature tree;

        private readonly ForageFantasy mod;

        public TreeMenu(ForageFantasy mod, TerrainFeature tree)
          : base(TreeTypeToName(mod, tree))
        {
            this.mod = mod;
            this.tree = tree;
        }

        public static string TreeTypeToName(ForageFantasy mod, TerrainFeature tree)
        {
            string key;

            if (tree is FruitTree fruitTree)
            {
                if (fruitTree.daysUntilMature.Value > 0 && ItemRegistry.Exists(fruitTree.treeId.Value))
                {
                    return ItemRegistry.Create(fruitTree.treeId.Value).DisplayName;
                }

                key = "FruitTree";
            }
            else
            {
                key = TreeTypeToNameKey((Tree)tree);
            }

            return mod.Helper.Translation.Get($"TreeMenu{key}");
        }

        public static int GetFruitTreeCurrentQuality(FruitTree tree)
        {
            var val = Math.Max(0, Math.Min(3, -tree.daysUntilMature.Value / (28 * 4)));

            return val == 3 ? StardewObject.bestQuality : val;
        }

        public static string TreeTypeToNameKey(Tree tree)
        {
            return tree?.treeType.Value switch
            {
                "1" or "10" => "OakTree",
                "2" or "11" => "MapleTree",
                "3" or "12" => "PineTree",
                "6" or "9" => "PalmTree",
                "7" => "MushroomTree",
                "8" => "MahoganyTree",
                "13" => "MysticTree",
                _ => "UnknownTree",
            };
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0066:Convert switch statement to expression", Justification = "Fallthrough is easier to read")]
        public string QualityToName(int quality)
        {
            switch (quality)
            {
                case StardewObject.lowQuality:
                case StardewObject.medQuality:
                case StardewObject.highQuality:
                case StardewObject.bestQuality:
                    return mod.Helper.Translation.Get($"TreeMenuQuality{(int)quality}");

                default:
                    return mod.Helper.Translation.Get($"TreeMenuQualityUnknown");
            }
        }

        public override string GetStatusMessage()
        {
            return tree is FruitTree ? GetFruitTreeStatusMessage() : GetTreeStatusMessage();
        }

        public string GetFruitTreeStatusMessage()
        {
            FruitTree fruitTree = tree as FruitTree;
            FruitTreeData data = fruitTree.GetData();

            if (fruitTree.daysUntilMature.Value > 0)
            {
                return mod.Helper.Translation.Get("TreeMenuMatureIn", new { info = GetTreeAgeString(fruitTree.daysUntilMature.Value) });
            }

            var daysAged = -fruitTree.daysUntilMature.Value;

            var output = new StringBuilder();

            output.Append(mod.Helper.Translation.Get("TreeMenuTreeAge", new { age = GetTreeAgeString(daysAged) }));
            output.Append('\n');

            string qualityString = QualityToName(GetFruitTreeCurrentQuality(fruitTree));

            if (qualityString != null)
            {
                output.Append(mod.Helper.Translation.Get("TreeMenuTreeAgeQuality", new { quality = qualityString }));
                output.Append('\n');
            }

            if (fruitTree.stump.Value)
            {
                // remove last newline character
                return output.ToString()[0..^1];
            }

            bool hasProduce = fruitTree.fruit.Count > 0;

            var defaultFruit = data?.Fruit.Where((t) => t.Id == "Default").FirstOrDefault();

            string produceName = null;

            if (fruitTree.struckByLightningCountdown.Value > 1)
            {
                // a fruit tree struck by lightning returns coal
                produceName = ItemRegistry.Create("(O)382").DisplayName;
            }
            else if (defaultFruit?.ItemId != null && ItemRegistry.Exists(defaultFruit.ItemId))
            {
                produceName = ItemRegistry.Create(defaultFruit.ItemId).DisplayName;
            }
            else
            {
                produceName = mod.Helper.Translation.Get("TreeMenuUnknownProduct");
            }

            output.Append(mod.Helper.Translation.Get("TreeMenuProduct", new { product = produceName }));
            output.Append('\n');

            if (!hasProduce)
            {
                if (fruitTree.IsInSeasonHere())
                {
                    string dayString = mod.Helper.Translation.Get("TreeMenu1Day");
                    output.Append(mod.Helper.Translation.Get("TreeMenuProductReadyIn", new { duration = dayString }));
                    output.Append('\n');
                }
                else
                {
                    int bestNextSeasonDiff = 10;

                    Season currentSeason = Game1.season;

                    List<Season> growSeasons = data?.Seasons;
                    if (growSeasons != null && growSeasons.Count > 0)
                    {
                        foreach (Season growSeason in growSeasons)
                        {
                            int seasonDiff = (-((int)currentSeason - (int)growSeason) + 4) % 4;

                            if (seasonDiff < bestNextSeasonDiff)
                            {
                                bestNextSeasonDiff = seasonDiff;
                            }
                        }
                    }

                    string seasonToBear = mod.Helper.Translation.Get("TreeMenuUnknownSeason");

                    if (bestNextSeasonDiff < 10)
                    {
                        Season bestSeaon = (Season)(((int)currentSeason + bestNextSeasonDiff + 4) % 4);
                        seasonToBear = Utility.getSeasonNameFromNumber((int)bestSeaon);
                    }

                    output.Append(mod.Helper.Translation.Get("TreeMenuProductReadyIn", new { duration = seasonToBear }));
                    output.Append('\n');
                }
            }

            // remove last newline character
            return output.ToString()[0..^1];
        }

        public string GetTreeStatusMessage()
        {
            Tree normalTree = tree as Tree;
            var data = normalTree.GetData();

            if (normalTree.growthStage.Value < 5)
            {
                int stagesUntilMature = 5 - normalTree.growthStage.Value;

                string matureText;
                if (GetTreeSeason(normalTree) == Season.Winter && data != null && !data.GrowsInWinter && !normalTree.fertilized.Value)
                {
                    matureText = mod.Helper.Translation.Get("TreeMenuNotInWinter");
                }
                else if (stagesUntilMature == 1)
                {
                    matureText = mod.Helper.Translation.Get("TreeMenu1GrowthStage");
                }
                else
                {
                    matureText = mod.Helper.Translation.Get("TreeMenuNGrowthStages", new { n = stagesUntilMature });
                }

                return mod.Helper.Translation.Get("TreeMenuMatureIn", new { info = matureText });
            }

            int daysAged = 0;
            tree.modData.TryGetValue($"{mod.ModManifest.UniqueID}/treeAge", out string moddata);

            if (!string.IsNullOrEmpty(moddata))
            {
                daysAged = int.Parse(moddata);
            }

            var output = new StringBuilder();

            output.Append(mod.Helper.Translation.Get("TreeMenuTreeAge", new { age = GetTreeAgeString(daysAged) }));
            output.Append('\n');

            if (mod.Config.TapperQualityOptions is 3 or 4)
            {
                int quality = TapperAndMushroomQualityLogic.DetermineTreeQuality(mod.Config, (Tree)tree);
                string qualityString = QualityToName(quality);

                if (qualityString != null)
                {
                    output.Append(mod.Helper.Translation.Get("TreeMenuTreeAgeQuality", new { quality = qualityString }));
                    output.Append('\n');
                }
            }

            if (!normalTree.stump.Value && data != null)
            {
                int stagesUntilMoss = Tree.stageForMossGrowth - normalTree.growthStage.Value;

                string infoText;
                if (!data.GrowsMoss)
                {
                    infoText = mod.Helper.Translation.Get("TreeMenuNo");
                }
                else if (normalTree.stopGrowingMoss.Value)
                {
                    infoText = mod.Helper.Translation.Get("TreeMenuNoDueToItem", new { itemName = ItemRegistry.Create("(O)419").DisplayName });
                }
                else if (normalTree.Location.IsGreenhouse)
                {
                    infoText = mod.Helper.Translation.Get("TreeMenuNotInGreenHouse");
                }
                else if (GetTreeSeason(normalTree) == Season.Winter)
                {
                    infoText = mod.Helper.Translation.Get("TreeMenuNotInWinter");
                }
                else if (stagesUntilMoss <= 0)
                {
                    infoText = mod.Helper.Translation.Get("TreeMenuYes");
                }
                else if (stagesUntilMoss == 1)
                {
                    infoText = mod.Helper.Translation.Get("TreeMenuIn1GrowthStage");
                }
                else
                {
                    infoText = mod.Helper.Translation.Get("TreeMenuInNGrowthStages", new { n = stagesUntilMoss });
                }

                output.Append(mod.Helper.Translation.Get("TreeMenuMossInfo", new { info = infoText }));
                output.Append('\n');
            }

            if (normalTree.tapped.Value)
            {
                StardewObject tile_object = tree.Location.getObjectAtTile((int)tree.Tile.X, (int)tree.Tile.Y);

                if (tile_object.IsTapper() && tile_object.heldObject.Value != null)
                {
                    output.Append(mod.Helper.Translation.Get("TreeMenuProduct", new { product = tile_object.heldObject.Value.DisplayName }));
                    output.Append('\n');

                    if (tile_object.MinutesUntilReady > 0)
                    {
                        output.Append(mod.Helper.Translation.Get("TreeMenuProductReadyIn", new { duration = FormatTapperMinutesUntil(tile_object.MinutesUntilReady) }));
                        output.Append('\n');
                    }

                    output.Append(mod.Helper.Translation.Get("TreeMenuProductValue", new { price = tile_object.heldObject.Value.Price }));
                    output.Append('\n');
                }
            }

            // remove last newline character
            return output.ToString()[0..^1];
        }

        private static Season? GetTreeSeason(Tree normalTree)
        {
            return (Season?)AccessTools.Field(typeof(Tree), "localSeason").GetValue(normalTree);
        }

        private string FormatTapperMinutesUntil(int minutes)
        {
            var span = TimeSpan.FromMinutes(minutes);

            if (span.Days == 1)
            {
                return mod.Helper.Translation.Get("TreeMenu1Day");
            }
            else
            {
                return mod.Helper.Translation.Get("TreeMenuNDays", new { n = span.Days });
            }
        }

        private string GetTreeAgeString(int days)
        {
            // intentional int division
            int months = days / 28;
            int years = months / 4;

            if (years > 0)
            {
                if (years == 1)
                {
                    return mod.Helper.Translation.Get("TreeMenu1Year");
                }
                else
                {
                    return mod.Helper.Translation.Get("TreeMenuNYears", new { n = years });
                }
            }
            else if (months > 0)
            {
                if (months == 1)
                {
                    return mod.Helper.Translation.Get("TreeMenu1Month");
                }
                else
                {
                    return mod.Helper.Translation.Get("TreeMenuNMonths", new { n = months });
                }
            }
            else
            {
                if (days == 1)
                {
                    return mod.Helper.Translation.Get("TreeMenu1Day");
                }
                else
                {
                    return mod.Helper.Translation.Get("TreeMenuNDays", new { n = days });
                }
            }
        }
    }
}