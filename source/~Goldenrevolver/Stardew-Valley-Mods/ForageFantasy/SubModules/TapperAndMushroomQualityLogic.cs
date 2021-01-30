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
    using StardewObject = StardewValley.Object;

    internal class TapperAndMushroomQualityLogic
    {
        public static void IncreaseTreeAges(ForageFantasy mod)
        {
            if (!Context.IsMainPlayer)
            {
                return;
            }

            foreach (var location in Game1.locations)
            {
                foreach (var terrainfeature in location.terrainFeatures.Pairs)
                {
                    if (terrainfeature.Value is Tree tree)
                    {
                        IncreaseTreeAge(mod, tree);
                    }
                }
            }
        }

        public static void IncreaseTreeAge(ForageFantasy mod, Tree tree)
        {
            string moddata;
            tree.modData.TryGetValue($"{mod.ModManifest.UniqueID}/treeAge", out moddata);

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

        public static bool IsMushroomBox(StardewObject o)
        {
            return o != null && o.bigCraftable && o.ParentSheetIndex == 128;
        }

        public static bool IsTapper(StardewObject o)
        {
            return o != null && o.bigCraftable && (o.ParentSheetIndex == 105 || o.parentSheetIndex == 264);
        }

        public static void RewardMushroomBoxExp(ForageFantasy mod)
        {
            if (mod.Config.MushroomXPAmount > 0)
            {
                Game1.player.gainExperience(2, mod.Config.MushroomXPAmount);
            }
        }

        public static void RewardTapperExp(ForageFantasy mod)
        {
            if (mod.Config.TapperXPAmount > 0)
            {
                Game1.player.gainExperience(2, mod.Config.TapperXPAmount);
            }
        }

        public static int DetermineTapperQuality(ForageFantasy mod, Farmer player, StardewObject o, Tree tree)
        {
            int option = mod.Config.TapperQualityOptions;

            if (option == 1 || option == 2)
            {
                // has tapper profession or it's not required
                if (!mod.Config.TapperQualityRequiresTapperPerk || player.professions.Contains(Farmer.tapper))
                {
                    return ForageFantasy.DetermineForageQuality(player, mod.Config.TapperQualityOptions == 1);
                }
            }
            else if (option == 3 || option == 4)
            {
                // quality increase once a year
                return DetermineTreeQuality(mod, tree);
            }

            // tapper perk required but doesn't have it or invalid option
            return 0;
        }

        private static int DetermineTreeQuality(ForageFantasy mod, Tree tree)
        {
            string moddata;
            tree.modData.TryGetValue($"{mod.ModManifest.UniqueID}/treeAge", out moddata);

            if (!string.IsNullOrEmpty(moddata))
            {
                int age = int.Parse(moddata);

                bool useMonths = mod.Config.TapperQualityOptions == 3;

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