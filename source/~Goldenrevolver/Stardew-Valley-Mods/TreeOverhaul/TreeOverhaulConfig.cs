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
    using System;
    using System.Diagnostics.CodeAnalysis;

    public interface IGenericModConfigMenuApi
    {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);

        void AddSectionTitle(IManifest mod, Func<string> text, Func<string> tooltip = null);

        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);

        void AddNumberOption(IManifest mod, Func<int> getValue, Action<int> setValue, Func<string> name, Func<string> tooltip = null, int? min = null, int? max = null, int? interval = null, Func<int, string> formatValue = null, string fieldId = null);

        void AddNumberOption(IManifest mod, Func<float> getValue, Action<float> setValue, Func<string> name, Func<string> tooltip = null, float? min = null, float? max = null, float? interval = null, Func<float, string> formatValue = null, string fieldId = null);

        void AddTextOption(IManifest mod, Func<string> getValue, Action<string> setValue, Func<string> name, Func<string> tooltip = null, string[] allowedValues = null, Func<string, string> formatAllowedValue = null, string fieldId = null);
    }

    /// <summary>
    /// Config file for the mod
    /// </summary>
    public class TreeOverhaulConfig
    {
        public bool StopShadeSaplingGrowth { get; set; } = true;

        public bool GrowthIgnoresStumps { get; set; } = true;

        public bool GrowthRespectsFruitTrees { get; set; } = true;

        public bool NormalTreesGrowInWinter { get; set; } = true;

        public bool MushroomTreesGrowInWinter { get; set; } = false;

        public bool CustomTreesGrowInWinter { get; set; } = false;

        public bool BuffMahoganyTreeGrowthChance { get; set; } = false;

        public bool CustomChancesAlsoAffectCustomTrees { get; set; } = false;

        public int CustomSeedOnShakeChance { get; set; } = -1;
        public int CustomSeedOnChopChance { get; set; } = -1;
        public int CustomSpawnSeedNearbyChance { get; set; } = -1;
        public int CustomTreeGrowthChance { get; set; } = -1;

        public int CustomMushroomTreeSeedOnShakeChance { get; set; } = -1;
        public int CustomMushroomTreeSeedOnChopChance { get; set; } = -1;
        public int CustomMushroomTreeSpawnSeedNearbyChance { get; set; } = -1;
        public int CustomMushroomTreeGrowthChance { get; set; } = -1;

        public int SaveSprouts { get; set; } = 0;

        private static string[] SSChoices { get; set; } = new string[] { "Disabled", "Hoe And Pickaxe", "Hoe, Pickaxe And Scythe", "Hoe, Pickaxe And All Melee Weapons" };

        public static void VerifyConfigValues(TreeOverhaulConfig config, TreeOverhaul mod)
        {
            bool invalidConfig = false;

            if (config.SaveSprouts < 0 || config.SaveSprouts > 3)
            {
                invalidConfig = true;
                config.SaveSprouts = 0;
            }

            int updatedValue = 0;

            if (VerifyPercentageRange(config.CustomSeedOnShakeChance, ref updatedValue))
            {
                invalidConfig = true;
                config.CustomSeedOnShakeChance = updatedValue;
            }

            if (VerifyPercentageRange(config.CustomSeedOnChopChance, ref updatedValue))
            {
                invalidConfig = true;
                config.CustomSeedOnChopChance = updatedValue;
            }

            if (VerifyPercentageRange(config.CustomSpawnSeedNearbyChance, ref updatedValue))
            {
                invalidConfig = true;
                config.CustomSpawnSeedNearbyChance = updatedValue;
            }

            if (VerifyPercentageRange(config.CustomTreeGrowthChance, ref updatedValue))
            {
                invalidConfig = true;
                config.CustomTreeGrowthChance = updatedValue;
            }

            if (VerifyPercentageRange(config.CustomMushroomTreeSeedOnShakeChance, ref updatedValue))
            {
                invalidConfig = true;
                config.CustomMushroomTreeSeedOnShakeChance = updatedValue;
            }

            if (VerifyPercentageRange(config.CustomMushroomTreeSeedOnChopChance, ref updatedValue))
            {
                invalidConfig = true;
                config.CustomMushroomTreeSeedOnChopChance = updatedValue;
            }

            if (VerifyPercentageRange(config.CustomMushroomTreeSpawnSeedNearbyChance, ref updatedValue))
            {
                invalidConfig = true;
                config.CustomMushroomTreeSpawnSeedNearbyChance = updatedValue;
            }

            if (VerifyPercentageRange(config.CustomMushroomTreeGrowthChance, ref updatedValue))
            {
                invalidConfig = true;
                config.CustomMushroomTreeGrowthChance = updatedValue;
            }

            if (invalidConfig)
            {
                mod.DebugLog("At least one config value was out of range and was reset.");
                mod.Helper.WriteConfig(config);
            }
        }

        private static bool VerifyPercentageRange(int configValue, ref int updatedValue)
        {
            if (configValue < -1 || configValue > 100)
            {
                updatedValue = Math.Clamp(configValue, -1, 100);
                return true;
            }

            return false;
        }

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1107:CodeMustNotContainMultipleStatementsOnOneLine", Justification = "Reviewed.")]
        [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Necessary.")]
        public static void SetUpModConfigMenu(TreeOverhaulConfig config, TreeOverhaul mod)
        {
            IGenericModConfigMenuApi api = mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (api == null)
            {
                return;
            }

            var manifest = mod.ModManifest;

            api.Register(
                mod: manifest,
                reset: () =>
                {
                    config = new TreeOverhaulConfig();
                    mod.Helper.GameContent.InvalidateCacheAndLocalized("Data/WildTrees");
                },
                save: () =>
                {
                    mod.Helper.WriteConfig(config);
                    VerifyConfigValues(config, mod);
                    mod.Helper.GameContent.InvalidateCacheAndLocalized("Data/WildTrees");
                }
            );

            api.AddSectionTitle(manifest, () => "General Tweaks", null);

            api.AddBoolOption(manifest, () => config.StopShadeSaplingGrowth, (bool val) => config.StopShadeSaplingGrowth = val,
                () => "Stop Seed Growth In Shade", () => "Seeds don't sprout in the 8 surrounding tiles of a tree (but they can still spawn unless you use 'CustomSpawnSeedNearbyChance' to disable it).");
            api.AddBoolOption(manifest, () => config.GrowthIgnoresStumps, (bool val) => config.GrowthIgnoresStumps = val,
                () => "Growth Ignores Stumps", () => "Trees can fully grow even if a small stump is in the 8 surrounding tiles.");
            api.AddBoolOption(manifest, () => config.GrowthRespectsFruitTrees, (bool val) => config.GrowthRespectsFruitTrees = val,
                () => "Growth Respects Fruit Trees", () => "Trees can't fully grow if a mature fruit tree is in the 8 surrounding tiles.");

            api.AddBoolOption(manifest, () => config.BuffMahoganyTreeGrowthChance, (bool val) => config.BuffMahoganyTreeGrowthChance = val,
                () => "Buff Mahogany Tree Growth", () => "Changes the growth chance of mahogany trees to be the same as the other base game tree, 20% unfertilized and 100% fertilized (from 15% and 60%). If 'CustomTreeGrowthChance' is used, it properly takes priority for the unfertilized growth chance.");

            api.AddTextOption(manifest, () => GetElementFromConfig(SSChoices, config.SaveSprouts), (string val) => config.SaveSprouts = GetIndexFromArrayElement(SSChoices, val), () => "Save Sprouts From Tools", () => "Normal and fruit trees can't be killed by the selected tools", SSChoices);

            api.AddSectionTitle(manifest, () => "Winter Tweaks", null);

            api.AddBoolOption(manifest, () => config.NormalTreesGrowInWinter, (bool val) => config.NormalTreesGrowInWinter = val,
                () => "Normal Trees Grow In Winter", null);
            api.AddBoolOption(manifest, () => config.MushroomTreesGrowInWinter, (bool val) => config.MushroomTreesGrowInWinter = val,
                () => "Mushroom Trees Grow In Winter", () => "Also allows tappers to produce in winter");
            api.AddBoolOption(manifest, () => config.CustomTreesGrowInWinter, (bool val) => config.CustomTreesGrowInWinter = val,
                () => "Custom Trees Grow In Winter", () => "Also allows tappers to produce in winter");

            api.AddSectionTitle(manifest, () => "Custom Chances", null);

            api.AddBoolOption(manifest, () => config.CustomChancesAlsoAffectCustomTrees, (bool val) => config.CustomChancesAlsoAffectCustomTrees = val,
                () => "Also Affect Custom Trees", () => "Whether, in addition to base game trees, mod added trees should also be affected");

            api.AddNumberOption(manifest, () => config.CustomSeedOnShakeChance, (int val) => config.CustomSeedOnShakeChance = val,
                () => "'Seed On Shake' Chance", () => "Chance that a seed drops when shaking a tree (base game: 5-15%, -1 to use base game value)", -1, 100);
            api.AddNumberOption(manifest, () => config.CustomSeedOnChopChance, (int val) => config.CustomSeedOnChopChance = val,
                () => "'Seed On Chop' Chance", () => "Chance that a seed drops when chopping down a tree (base game: 56-75%, -1 to use base game value)", -1, 100);
            api.AddNumberOption(manifest, () => config.CustomSpawnSeedNearbyChance, (int val) => config.CustomSpawnSeedNearbyChance = val,
                () => "'Spawn Seed Nearby' Chance", () => "Chance to attempt to spawn a seed near a tree overnight (base game: 15%, -1 to use base game value)", -1, 100);
            api.AddNumberOption(manifest, () => config.CustomTreeGrowthChance, (int val) => config.CustomTreeGrowthChance = val,
                () => "Tree Growth Chance", () => "Chance for a tree to grow overnight (including mahogany trees) (base game: 20%, -1 to use base game value)", -1, 100);

            api.AddSectionTitle(manifest, () => "Custom Chances (Mushroom Trees)", null);

            api.AddNumberOption(manifest, () => config.CustomMushroomTreeSeedOnShakeChance, (int val) => config.CustomMushroomTreeSeedOnShakeChance = val,
                () => "'Seed On Shake' Chance", () => "Chance that a seed drops when shaking a mushroom tree (base game: 0%, -1 to use base game value)", -1, 100);
            api.AddNumberOption(manifest, () => config.CustomSeedOnChopChance, (int val) => config.CustomSeedOnChopChance = val,
                () => "'Seed On Chop' Chance", () => "Chance that a seed drops when chopping down a mushroom tree (base game: 0%, -1 to use base game value)", -1, 100);
            api.AddNumberOption(manifest, () => config.CustomMushroomTreeSpawnSeedNearbyChance, (int val) => config.CustomMushroomTreeSpawnSeedNearbyChance = val,
                () => "'Spawn Seed Nearby' Chance", () => "Chance to attempt to spawn a seed near a mushroom tree overnight (base game: 15%, -1 to use base game value)", -1, 100);
            api.AddNumberOption(manifest, () => config.CustomMushroomTreeGrowthChance, (int val) => config.CustomMushroomTreeGrowthChance = val,
                () => "Tree Growth Chance", () => "Chance for a mushroom tree to grow overnight (base game: 20%, -1 to use base game value)", -1, 100);
        }

        private static string GetElementFromConfig(string[] options, int config)
        {
            if (config >= 0 && config < options.Length)
            {
                return options[config];
            }
            else
            {
                return options[0];
            }
        }

        private static int GetIndexFromArrayElement(string[] options, string element)
        {
            var index = Array.IndexOf(options, element);

            return index == -1 ? 0 : index;
        }
    }
}