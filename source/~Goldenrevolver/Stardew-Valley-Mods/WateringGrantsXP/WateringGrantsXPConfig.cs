/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace WateringGrantsXP
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using StardewModdingAPI;
    using System;
    using System.Diagnostics.CodeAnalysis;

    public interface IGenericModConfigMenuApi
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
    public class WateringGrantsXPConfig
    {
        public int WateringExperienceAmount { get; set; } = 1;

        public int WateringChanceToGetXP { get; set; } = 100;

        public bool ForageSeedWateringGrantsForagingXP { get; set; } = false;

        public bool CropsCanDieWithoutWater { get; set; } = false;

        public int DaysWithoutWaterForChanceToDie { get; set; } = 2;

        public int ChanceToDieWhenLeftForTooLong { get; set; } = 100;

        public static void VerifyConfigValues(WateringGrantsXPConfig config, WateringGrantsXP mod)
        {
            bool invalidConfig = false;

            if (config.WateringExperienceAmount < 0)
            {
                invalidConfig = true;
                config.WateringExperienceAmount = 0;
            }

            if (config.ChanceToDieWhenLeftForTooLong < 0)
            {
                invalidConfig = true;
                config.ChanceToDieWhenLeftForTooLong = 0;
            }

            if (config.WateringChanceToGetXP > 100)
            {
                invalidConfig = true;
                config.WateringChanceToGetXP = 100;
            }

            if (config.DaysWithoutWaterForChanceToDie < 1)
            {
                invalidConfig = true;
                config.DaysWithoutWaterForChanceToDie = 1;
            }

            if (config.ChanceToDieWhenLeftForTooLong < 0)
            {
                invalidConfig = true;
                config.ChanceToDieWhenLeftForTooLong = 0;
            }

            if (config.ChanceToDieWhenLeftForTooLong > 100)
            {
                invalidConfig = true;
                config.ChanceToDieWhenLeftForTooLong = 100;
            }

            if (invalidConfig)
            {
                mod.DebugLog("At least one config value was out of range and was reset.");
                mod.Helper.WriteConfig(config);
            }
        }

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1107:CodeMustNotContainMultipleStatementsOnOneLine", Justification = "Reviewed.")]
        [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Necessary.")]
        public static void SetUpModConfigMenu(WateringGrantsXPConfig config, WateringGrantsXP mod)
        {
            IGenericModConfigMenuApi api = mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (api == null)
            {
                return;
            }

            var manifest = mod.ModManifest;

            api.RegisterModConfig(manifest, () => config = new WateringGrantsXPConfig(), delegate { mod.Helper.WriteConfig(config); VerifyConfigValues(config, mod); });

            api.RegisterLabel(manifest, "Watering Grants XP", null);

            api.RegisterSimpleOption(manifest, "Amount Of Experience", null, () => config.WateringExperienceAmount, (int val) => config.WateringExperienceAmount = val);
            api.RegisterClampedOption(manifest, "Chance To Get XP", null, () => config.WateringChanceToGetXP, (int val) => config.WateringChanceToGetXP = val, 0, 100);
            api.RegisterSimpleOption(manifest, "Forage Seed Watering\nGrants Foraging XP", null, () => config.ForageSeedWateringGrantsForagingXP, (bool val) => config.ForageSeedWateringGrantsForagingXP = val);

            // this is a spacer due to the line break above
            api.RegisterLabel(manifest, string.Empty, null);
            api.RegisterLabel(manifest, "Crops Die Without Water", null);

            api.RegisterSimpleOption(manifest, "Withering Feature Enabled", null, () => config.CropsCanDieWithoutWater, (bool val) => config.CropsCanDieWithoutWater = val);
            api.RegisterSimpleOption(manifest, "Days For Chance Of Withering", null, () => config.DaysWithoutWaterForChanceToDie, (int val) => config.DaysWithoutWaterForChanceToDie = val);
            api.RegisterClampedOption(manifest, "Chance For Withering", null, () => config.ChanceToDieWhenLeftForTooLong, (int val) => config.ChanceToDieWhenLeftForTooLong = val, 0, 100);
        }
    }
}