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
    using Microsoft.Xna.Framework.Graphics;
    using StardewModdingAPI;
    using System;

    public interface GenericModConfigMenuAPI
    {
        void RegisterModConfig(IManifest mod, Action revertToDefault, Action saveToFile);

        void RegisterLabel(IManifest mod, string labelName, string labelDesc);

        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<bool> optionGet, Action<bool> optionSet);

        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<int> optionGet, Action<int> optionSet);

        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<float> optionGet, Action<float> optionSet);

        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<string> optionGet, Action<string> optionSet);

        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<SButton> optionGet, Action<SButton> optionSet);

        void RegisterClampedOption(IManifest mod, string optionName, string optionDesc, Func<int> optionGet, Action<int> optionSet, int min, int max);

        void RegisterClampedOption(IManifest mod, string optionName, string optionDesc, Func<float> optionGet, Action<float> optionSet, float min, float max);

        void RegisterChoiceOption(IManifest mod, string optionName, string optionDesc, Func<string> optionGet, Action<string> optionSet, string[] choices);

        void RegisterComplexOption(IManifest mod, string optionName, string optionDesc, Func<Vector2, object, object> widgetUpdate, Func<SpriteBatch, Vector2, object, object> widgetDraw, Action<object> onSave);
    }

    /// <summary>
    /// Config file for the mod
    /// </summary>
    public class TreeOverhaulConfig
    {
        public bool StopShadeSaplingGrowth { get; set; } = true;

        public bool GrowthIgnoresStumps { get; set; } = false;

        public int SaveSprouts { get; set; } = 0;

        public bool NormalTreesGrowInWinter { get; set; } = true;

        public bool MushroomTreesGrowInWinter { get; set; } = false;

        public bool FruitTreesDontGrowInWinter { get; set; } = false;

        public bool BuffMahoganyTrees { get; set; } = false;

        public int ShakingSeedChance { get; set; } = 5;

        public bool FasterNormalTreeGrowth { get; set; } = false;

        public int FruitTreeGrowth { get; set; } = 0;

        private static string[] SSChoices { get; set; } = new string[] { "Disabled", "Hoe And Pickaxe", "Hoe, Pickaxe And Scythe", "Hoe, Pickaxe And All Melee Weapons" };

        private static string[] FTChoices { get; set; } = new string[] { "Default", "Twice As Fast", "Half As Fast" };

        public static void VerifyConfigValues(TreeOverhaulConfig config, TreeOverhaul mod)
        {
            bool invalidConfig = false;

            if (config.SaveSprouts < 0 || config.SaveSprouts > 3)
            {
                invalidConfig = true;
                config.SaveSprouts = 0;
            }

            if (config.FruitTreeGrowth < 0 || config.FruitTreeGrowth > 2)
            {
                invalidConfig = true;
                config.FruitTreeGrowth = 0;
            }

            if (config.ShakingSeedChance < 0)
            {
                invalidConfig = true;
                config.ShakingSeedChance = 0;
            }

            if (config.ShakingSeedChance > 100)
            {
                invalidConfig = true;
                config.ShakingSeedChance = 100;
            }

            if (invalidConfig)
            {
                mod.DebugLog("At least one config value was out of range and was reset.");
                mod.Helper.WriteConfig(config);
            }
        }

        public static void SetUpModConfigMenu(TreeOverhaulConfig config, TreeOverhaul mod)
        {
            GenericModConfigMenuAPI api = mod.Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");

            if (api == null)
            {
                return;
            }

            var manifest = mod.ModManifest;

            api.RegisterModConfig(manifest, () => config = new TreeOverhaulConfig(), delegate { mod.Helper.WriteConfig(config); VerifyConfigValues(config, mod); });

            api.RegisterLabel(manifest, "General Tweaks", null);

            api.RegisterSimpleOption(manifest, "Stop Seed Growth In Shade", "Seeds don't sprout in the 8 surrounding tiles of a tree", () => config.StopShadeSaplingGrowth, (bool val) => config.StopShadeSaplingGrowth = val);
            api.RegisterSimpleOption(manifest, "Growth Ignores Stumps", "Trees can grow even if a small stump is next to them", () => config.GrowthIgnoresStumps, (bool val) => config.GrowthIgnoresStumps = val);
            api.RegisterChoiceOption(manifest, "Save Sprouts From Tools", "Normal and fruit trees can't be killed by the selected tools", () => GetElementFromConfig(SSChoices, config.SaveSprouts), (string val) => config.SaveSprouts = GetIndexFromArrayElement(SSChoices, val), SSChoices);

            api.RegisterLabel(manifest, "Winter Tweaks", null);

            api.RegisterSimpleOption(manifest, "Normal Trees Grow In Winter", null, () => config.NormalTreesGrowInWinter, (bool val) => config.NormalTreesGrowInWinter = val);
            api.RegisterSimpleOption(manifest, "Mushroom Trees Grow In Winter", null, () => config.MushroomTreesGrowInWinter, (bool val) => config.MushroomTreesGrowInWinter = val);
            api.RegisterSimpleOption(manifest, "Fruit Trees Don't Grow In Winter", null, () => config.FruitTreesDontGrowInWinter, (bool val) => config.FruitTreesDontGrowInWinter = val);

            api.RegisterLabel(manifest, "Buffs And Nerfs", null);

            api.RegisterSimpleOption(manifest, "Buff Mahogany Tree Growth", "20% unfertilized and 100% fertilized (from 15% and 60%)", () => config.BuffMahoganyTrees, (bool val) => config.BuffMahoganyTrees = val);
            api.RegisterClampedOption(manifest, "Seed Chance From Shaking", "Chance that a seed drops from shaking a tree (default: 5%, chance depends on host)", () => config.ShakingSeedChance, (int val) => config.ShakingSeedChance = val, 0, 100);
            api.RegisterSimpleOption(manifest, "Faster Normal Tree Growth", "Normal trees try to grow twice every day, still random whether they succeed", () => config.FasterNormalTreeGrowth, (bool val) => config.FasterNormalTreeGrowth = val);
            api.RegisterChoiceOption(manifest, "Fruit Tree Growth Options", null, () => GetElementFromConfig(FTChoices, config.FruitTreeGrowth), (string val) => config.FruitTreeGrowth = GetIndexFromArrayElement(FTChoices, val), FTChoices);
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
            for (int i = 0; i < options.Length; i++)
            {
                if (options[i] == element)
                {
                    return i;
                }
            }

            return 0;
        }
    }
}