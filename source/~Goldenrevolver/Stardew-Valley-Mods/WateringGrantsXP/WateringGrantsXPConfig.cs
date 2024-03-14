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
    public class WateringGrantsXPConfig
    {
        public int WateringExperienceAmount { get; set; } = 1;

        public int WateringChanceToGetXP { get; set; } = 100;

        public bool ForageSeedWateringGrantsForagingXP { get; set; } = false;

        public bool CropsCanDieWithoutWater { get; set; } = false;

        public int DaysWithoutWaterForChanceToDie { get; set; } = 3;

        public int ChanceToDieWhenLeftForTooLong { get; set; } = 100;

        public bool WitheringAlsoChecksGardenPots { get; set; } = false;

        public bool CantRefillCanWithSaltWater { get; set; } = false;

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

            api.Register(
                mod: manifest,
                reset: () => config = new WateringGrantsXPConfig(),
                save: () => { mod.Helper.WriteConfig(config); VerifyConfigValues(config, mod); }
            );

            api.AddSectionTitle(manifest, () => "Watering Grants XP", null);

            api.AddNumberOption(manifest, () => config.WateringExperienceAmount, (int val) => config.WateringExperienceAmount = val, () => "Amount Of Experience", null, 0);
            api.AddNumberOption(manifest, () => config.WateringChanceToGetXP, (int val) => config.WateringChanceToGetXP = val, () => "Chance To Get XP", null, 0, 100);
            api.AddBoolOption(manifest, () => config.ForageSeedWateringGrantsForagingXP, (bool val) => config.ForageSeedWateringGrantsForagingXP = val, () => "Forage Seed Watering\nGrants Foraging XP", null);

            api.AddSectionTitle(manifest, () => "Crops Die Without Water", null);

            api.AddBoolOption(manifest, () => config.CropsCanDieWithoutWater, (bool val) => config.CropsCanDieWithoutWater = val, () => "Withering Feature Enabled", null);
            api.AddNumberOption(manifest, () => config.DaysWithoutWaterForChanceToDie, (int val) => config.DaysWithoutWaterForChanceToDie = val, () => "Days For Chance Of Withering", null, 1);
            api.AddNumberOption(manifest, () => config.ChanceToDieWhenLeftForTooLong, (int val) => config.ChanceToDieWhenLeftForTooLong = val, () => "Chance For Withering", null, 0, 100);
            api.AddBoolOption(manifest, () => config.WitheringAlsoChecksGardenPots, (bool val) => config.WitheringAlsoChecksGardenPots = val, () => "Withering Also Checks Garden Pots", null);

            api.AddSectionTitle(manifest, () => "Other", null);

            api.AddBoolOption(manifest, () => config.CantRefillCanWithSaltWater, (bool val) => config.CantRefillCanWithSaltWater = val, () => "Can't Refill Watering Can\nWith Salt Water", null);
        }
    }
}