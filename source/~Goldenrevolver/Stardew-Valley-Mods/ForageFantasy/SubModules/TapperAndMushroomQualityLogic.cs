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
    using StardewModdingAPI;
    using StardewValley;
    using StardewValley.TerrainFeatures;
    using System;
    using StardewObject = StardewValley.Object;

    public static class ExtensionMethods
    {
        public static bool IsMushroomBox(this StardewObject o)
        {
            return o != null && o.QualifiedItemId == "(BC)128";
        }
    }

    internal class TapperAndMushroomQualityLogic
    {
        internal const string mapleSyrupNonQID = "724";
        internal const string oakResinNonQID = "725";
        internal const string pineTarNonQID = "726";
        internal const string mysticSyrupNonQID = "MysticSyrup";
        internal const string sapQID = "(O)92";

        private static float GetTapperProductPricePerDay(string product, float? priceOverride = null)
        {
            float basePrice = product switch
            {
                mapleSyrupNonQID => 200f,
                oakResinNonQID => 150f,
                pineTarNonQID => 100f,
                mysticSyrupNonQID => 1000f,
                _ => 0,
            };

            if (priceOverride != null)
            {
                basePrice = priceOverride.Value;
            }

            return product switch
            {
                mapleSyrupNonQID => basePrice / 9f,
                oakResinNonQID => basePrice / 7f,
                pineTarNonQID => basePrice / 5f,
                mysticSyrupNonQID => basePrice / 7f,
                _ => 0,
            };
        }

        public static int GetTapperProductValueForDaysNeededWithEqualizedPriceCheck(ForageFantasyConfig config, int daysNeeded, string product, int? priceOverride, int? mapleSyrupPriceOverride, int? oakResinPriceOverride, int? pineTarPriceOverride)
        {
            return config.EqualizedPricePerDayForMapleOakPineTapperProduct switch
            {
                1 => GetTapperProductValueForDaysNeeded(daysNeeded, mapleSyrupNonQID, mapleSyrupPriceOverride),
                2 => GetTapperProductValueForDaysNeeded(daysNeeded, oakResinNonQID, oakResinPriceOverride),
                3 => GetTapperProductValueForDaysNeeded(daysNeeded, pineTarNonQID, pineTarPriceOverride),
                _ => GetTapperProductValueForDaysNeeded(daysNeeded, product, priceOverride),
            };
        }

        public static int GetTapperProductValueForDaysNeeded(int daysNeeded, string product, int? priceOverride)
        {
            float baseValuePerDay = GetTapperProductPricePerDay(product, priceOverride);

            int approxPrice = (int)Math.Round(daysNeeded * baseValuePerDay);

            int nextDivisibleByFive = (int)Math.Round(approxPrice / 5f) * 5;

            return nextDivisibleByFive;
        }

        public static void IncreaseTreeAges(ForageFantasy mod)
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
                        IncreaseTreeAge(mod, tree);
                    }
                }

                return true;
            });
        }

        public static void IncreaseTreeAge(ForageFantasy mod, Tree tree)
        {
            if (tree.growthStage.Value < 5)
            {
                return;
            }

            tree.modData.TryGetValue($"{mod.ModManifest.UniqueID}/treeAge", out string moddata);

            if (!string.IsNullOrEmpty(moddata))
            {
                int age = int.Parse(moddata);
                tree.modData[$"{mod.ModManifest.UniqueID}/treeAge"] = (age + 1).ToString();
            }
            else
            {
                tree.modData[$"{mod.ModManifest.UniqueID}/treeAge"] = 1.ToString();
            }
        }

        public static int DetermineTapperQuality(ForageFantasyConfig config, Farmer player, Tree tree)
        {
            switch (config.TapperQualityOptions)
            {
                case 1:
                case 2:
                    // has tapper profession or it's not required
                    if (!config.TapperQualityRequiresTapperPerk || player.professions.Contains(Farmer.tapper))
                    {
                        Random r = Utility.CreateDaySaveRandom(tree.Tile.X, tree.Tile.Y * 777f);
                        return ForageFantasy.DetermineForageQuality(player, r, config.TapperQualityOptions == 1);
                    }
                    break;

                case 3:
                case 4:
                    // quality increase once a year
                    return DetermineTreeQuality(config, tree);
            }

            // tapper perk required but doesn't have it or invalid option
            return 0;
        }

        public static int DetermineTreeQuality(ForageFantasyConfig config, Tree tree)
        {
            tree.modData.TryGetValue($"{ForageFantasy.Manifest.UniqueID}/treeAge", out string moddata);

            if (!string.IsNullOrEmpty(moddata))
            {
                int age = int.Parse(moddata);

                bool useMonths = config.TapperQualityOptions == 3;

                int timeForLevelUp = useMonths ? 28 : 28 * 4;

                if (age < timeForLevelUp)
                {
                    return 0;
                }
                else if (age < timeForLevelUp * 2)
                {
                    return 1;
                }
                else if (age < timeForLevelUp * 3)
                {
                    return 2;
                }
                else
                {
                    return 4;
                }
            }

            return 0;
        }
    }
}