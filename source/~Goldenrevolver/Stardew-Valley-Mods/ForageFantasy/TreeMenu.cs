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
    using StardewModdingAPI.Utilities;
    using StardewValley;
    using StardewValley.TerrainFeatures;
    using System;
    using System.Collections.Generic;
    using StardewObject = StardewValley.Object;

    public class TreeMenu : BaseMenu
    {
        private readonly TerrainFeature tree;

        private readonly ForageFantasy mod;

        public TreeMenu(ForageFantasy mod, TerrainFeature tree)
          : base(TreeTypeToName(tree))
        {
            this.mod = mod;
            this.tree = tree;
        }

        // TODO replace with I18n and make everything translatable

        public static string TreeTypeToName(TerrainFeature tree)
        {
            return tree is FruitTree ? "Fruit Tree" : TreeTypeToName((Tree)tree);
        }

        public static int GetFruitTreeCurrentQuality(FruitTree tree)
        {
            var val = Math.Max(0, Math.Min(3, -tree.daysUntilMature.Value / (28 * 4)));

            return val == 3 ? StardewObject.bestQuality : val;
        }

        public static string TreeTypeToName(Tree tree)
        {
            return tree?.treeType.Value switch
            {
                1 => "Oak Tree",
                2 => "Maple Tree",
                3 => "Pine Tree",
                9 or 6 => "Palm Tree",
                7 => "Mushroom Tree",
                8 => "Mahogany Tree",
                _ => "Tree",
            };
        }

        public static string QualityToName(int quality)
        {
            return quality switch
            {
                StardewObject.lowQuality => "Normal",
                StardewObject.medQuality => "Silver",
                StardewObject.highQuality => "Gold",
                StardewObject.bestQuality => "Iridium",
                _ => null,
            };
        }

        // similar to FruitTree.IsInSeason here, taken from LookUpAnything
        public static bool IsFruitTreeInSeason(FruitTree fruitTree, string season)
        {
            if (season == fruitTree.fruitSeason.Value || fruitTree.currentLocation.SeedsIgnoreSeasonsHere())
                return true;

            if (fruitTree.fruitSeason.Value == "island")
                return season == "summer" || fruitTree.currentLocation.GetLocationContext() == GameLocation.LocationContext.Island;

            return false;
        }

        public override string GetStatusMessage()
        {
            return tree is FruitTree ? GetFruitTreeStatusMessage() : GetTreeStatusMessage();
        }

        public string GetFruitTreeStatusMessage()
        {
            FruitTree fruitTree = tree as FruitTree;

            var daysAged = -fruitTree.daysUntilMature.Value;

            string outp = $"Age: {GetTreeAgeString(daysAged)}\n";

            string qualityString = QualityToName(GetFruitTreeCurrentQuality(fruitTree));

            if (qualityString != null)
            {
                outp += $"Age Quality: {qualityString}\n";
            }

            if (!fruitTree.stump.Value)
            {
                bool hasProduce = fruitTree.fruitsOnTree.Value > 0;

                if (hasProduce)
                {
                    outp += $"Produce: {new StardewObject(fruitTree.indexOfFruit.Value, 1).DisplayName}\n";
                }
                else
                {
                    var lightningDays = fruitTree.struckByLightningCountdown.Value;

                    // a fruit tree struck by lightning returns coal
                    var produce = lightningDays > 1 ? 382 : fruitTree.indexOfFruit.Value;

                    outp += $"Produce: {new StardewObject(produce, 1).DisplayName}\n";

                    var tomorrow = SDate.Now().AddDays(1);

                    if (IsFruitTreeInSeason(fruitTree, tomorrow.Season))
                    {
                        outp += "Producing In: 1 day\n";
                    }
                    else
                    {
                        var seasonToBear = fruitTree.fruitSeason.Value;

                        if (seasonToBear == SDate.Now().Season)
                        {
                            seasonToBear = $"Next {seasonToBear}";
                        }
                        else
                        {
                            seasonToBear = char.ToUpper(seasonToBear[0]) + seasonToBear[1..];
                        }

                        outp += $"Producing In: {seasonToBear}\n";
                    }
                }
            }

            // remove last newline character
            return outp[0..^1];
        }

        public string GetTreeStatusMessage()
        {
            Tree normalTree = tree as Tree;

            int daysAged = 0;
            tree.modData.TryGetValue($"{mod.ModManifest.UniqueID}/treeAge", out string moddata);

            if (!string.IsNullOrEmpty(moddata))
            {
                daysAged = int.Parse(moddata);
            }

            string outp = $"Age: {GetTreeAgeString(daysAged)}\n";

            if (mod.Config.TapperQualityOptions is 3 or 4)
            {
                int quality = TapperAndMushroomQualityLogic.DetermineTreeQuality(mod, (Tree)tree);
                string qualityString = QualityToName(quality);

                if (qualityString != null)
                {
                    outp += $"Age Quality: {qualityString}\n";
                }
            }

            if (!normalTree.stump.Value || normalTree.treeType.Value == Tree.mushroomTree)
            {
                StardewValley.Object tile_object = tree.currentLocation.getObjectAtTile((int)tree.currentTileLocation.X, (int)tree.currentTileLocation.Y);

                if (tile_object.IsTapper() && tile_object.heldObject.Value != null)
                {
                    if (tile_object.MinutesUntilReady > 0)
                    {
                        outp += $"Produce: {tile_object.heldObject.Value.DisplayName}\n";
                        outp += TapperProducingInString(tile_object.MinutesUntilReady);

                        if (mod.Config.TapperDaysNeededChangesEnabled)
                        {
                            outp += tile_object.heldObject.Value.ParentSheetIndex switch
                            {
                                724 => $"Base Value: {TapperAndMushroomQualityLogic.GetTapperProductValueForDaysNeeded(mod.Config.MapleDaysNeeded)}\n",
                                725 => $"Base Value: {TapperAndMushroomQualityLogic.GetTapperProductValueForDaysNeeded(mod.Config.OakDaysNeeded)}\n",
                                726 => $"Base Value: {TapperAndMushroomQualityLogic.GetTapperProductValueForDaysNeeded(mod.Config.PineDaysNeeded)}\n",
                                _ => "", // just so I can use a switch assignment
                            };
                        }
                    }
                }
            }

            // remove last newline character
            return outp[0..^1];
        }

        private static string TapperProducingInString(int minutes, bool simplified = true)
        {
            var span = TimeSpan.FromMinutes(minutes);

            if (simplified)
            {
                if (span.Days == 1)
                {
                    return $"Producing In: 1 day\n";
                }
                else
                {
                    return $"Producing In: { span.Days } days\n";
                }
            }
            else
            {
                var parts = new List<string>();

                if (span.Days > 0)
                {
                    if (span.Days == 1)
                    {
                        parts.Add($"1 day");
                    }
                    else
                    {
                        parts.Add($"{span.Days} days");
                    }
                }
                if (span.Hours > 0)
                {
                    if (span.Minutes == 1)
                    {
                        parts.Add($"1 hour");
                    }
                    else
                    {
                        parts.Add($"{span.Hours} hours");
                    }
                }
                if (span.Minutes > 0)
                {
                    if (span.Minutes == 1)
                    {
                        parts.Add($"1 minute");
                    }
                    else
                    {
                        parts.Add($"{span.Minutes} minutes");
                    }
                }

                return $"Producing In: { string.Join($",\n{new String(' ', "Producing In: ".Length + 3)}", parts) }\n";
            }
        }

        private static string GetTreeAgeString(int days)
        {
            // intentional int division
            int months = days / 28;
            int years = months / 4;

            if (years > 0)
            {
                if (years == 1)
                {
                    return "1 year";
                }
                else
                {
                    return $"{years} year";
                }
            }
            else if (months > 0)
            {
                if (months == 1)
                {
                    return "1 month";
                }
                else
                {
                    return $"{months} months";
                }
            }
            else
            {
                if (days == 1)
                {
                    return "1 day";
                }
                else
                {
                    return $"{days} days";
                }
            }
        }
    }
}