/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace CrabPotQuality
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
    public class CrabPotQualityConfig
    {
        public bool DisableAllQualityEffects { get; set; } = false;

        public bool DisableAllNonQualityEffects { get; set; } = false;

        public bool MarinerPerkForcesIridiumQuality { get; set; } = true;

        public bool LuremasterPerkForcesIridiumQuality { get; set; } = true;

        public bool EnableWildBaitEffect { get; set; } = true;

        public float WildBaitQualityChanceModifier { get; set; } = 1.5f;

        public int WildBaitDoubleItemAmountChance { get; set; } = 25;

        public int WildBaitSpecialItemChance { get; set; } = 0;

        public string WildBaitSpecialItem { get; set; } = string.Empty;

        public bool WildBaitSpecialItemHasNoQuality { get; set; } = false;

        public bool EnableDeluxeBaitEffect { get; set; } = true;

        public float DeluxeBaitQualityChanceModifier { get; set; } = 1.5f;

        public int DeluxeBaitDoubleItemAmountChance { get; set; } = 0;

        public int DeluxeBaitSpecialItemChance { get; set; } = 0;

        public string DeluxeBaitSpecialItem { get; set; } = string.Empty;

        public bool DeluxeBaitSpecialItemHasNoQuality { get; set; } = false;

        public bool EnableMagicBaitEffect { get; set; } = true;

        public float MagicBaitQualityChanceModifier { get; set; } = 1.5f;

        public int MagicBaitDoubleItemAmountChance { get; set; } = 0;

        public int MagicBaitSpecialItemChance { get; set; } = 50;

        public string MagicBaitSpecialItem { get; set; } = "(O)394";

        public bool MagicBaitSpecialItemHasNoQuality { get; set; } = false;

        public bool EnableChallengeBaitEffect { get; set; } = true;

        public float ChallengeBaitQualityChanceModifier { get; set; } = 1.5f;

        public int ChallengeBaitDoubleItemAmountChance { get; set; } = 30;

        public int ChallengeBaitSpecialItemChance { get; set; } = 0;

        public string ChallengeBaitSpecialItem { get; set; } = string.Empty;

        public bool ChallengeBaitSpecialItemHasNoQuality { get; set; } = false;

        public bool EnableMagnetBaitEffect { get; set; } = true;

        public float MagnetBaitQualityChanceModifier { get; set; } = 0.5f;

        public int MagnetBaitDoubleItemAmountChance { get; set; } = 0;

        public int MagnetBaitSpecialItemChance { get; set; } = 15;

        public string MagnetBaitSpecialItem { get; set; } = "(O)275";

        public bool MagnetBaitSpecialItemHasNoQuality { get; set; } = true;

        public bool EnableCustomModdedBaitEffect { get; set; } = false;

        public float CustomModdedBaitQualityChanceModifier { get; set; } = 1f;

        public int CustomModdedBaitDoubleItemAmountChance { get; set; } = 0;

        public int CustomModdedBaitSpecialItemChance { get; set; } = 0;

        public string CustomModdedBaitSpecialItem { get; set; } = string.Empty;

        public bool CustomModdedBaitSpecialItemHasNoQuality { get; set; } = false;

        public static void VerifyConfigValues(CrabPotQualityConfig config, CrabPotQuality mod)
        {
            bool invalidConfig = false;

            foreach (var prop in typeof(CrabPotQualityConfig).GetProperties())
            {
                if (prop.PropertyType == typeof(float))
                {
                    float qualityModifier = (float)prop.GetValue(config);

                    if (qualityModifier < 0f)
                    {
                        invalidConfig = true;
                        prop.SetValue(config, 1f); // assume that if they set it lower than 0 that they wanted to disable it (which is modifier of 1)
                    }
                }
                else if (prop.PropertyType == typeof(int))
                {
                    int specialItemChance = (int)prop.GetValue(config);

                    if (specialItemChance < 0)
                    {
                        invalidConfig = true;
                        prop.SetValue(config, 0);
                    }
                    else if (specialItemChance > 100)
                    {
                        invalidConfig = true;
                        prop.SetValue(config, 100);
                    }
                }
                else if (prop.PropertyType == typeof(string))
                {
                    string specialItem = (string)prop.GetValue(config);

                    if (specialItem == null)
                    {
                        prop.SetValue(config, string.Empty);
                    }
                    else
                    {
                        prop.SetValue(config, string.Empty.Trim());
                    }
                }
            }

            if (invalidConfig)
            {
                mod.DebugLog("At least one config value was out of range and was reset.");
                mod.Helper.WriteConfig(config);
            }
        }

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1107:CodeMustNotContainMultipleStatementsOnOneLine", Justification = "Reviewed.")]
        [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Necessary.")]
        public static void SetUpModConfigMenu(CrabPotQualityConfig config, CrabPotQuality mod)
        {
            IGenericModConfigMenuApi api = mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (api == null)
            {
                return;
            }

            var manifest = mod.ModManifest;

            api.Register(
                mod: manifest,
                reset: () => config = new CrabPotQualityConfig(),
                save: () => mod.Helper.WriteConfig(config)
            );

            api.AddSectionTitle(manifest, () => "Feature Toggles", null);

            api.AddBoolOption(manifest, () => config.DisableAllQualityEffects, (bool val) => config.DisableAllQualityEffects = val,
                () => "Disable All Quality Effects", null);
            api.AddBoolOption(manifest, () => config.DisableAllNonQualityEffects, (bool val) => config.DisableAllNonQualityEffects = val,
                () => "Disable All Non Quality Effects", null);

            api.AddSectionTitle(manifest, () => "Iridium Quality When", null);

            api.AddBoolOption(manifest, () => config.MarinerPerkForcesIridiumQuality, (bool val) => config.MarinerPerkForcesIridiumQuality = val,
                () => "Has Mariner Perk", null);
            api.AddBoolOption(manifest, () => config.LuremasterPerkForcesIridiumQuality, (bool val) => config.LuremasterPerkForcesIridiumQuality = val,
                () => "Has Luremaster Perk", null);

            api.AddSectionTitle(manifest, () => "Wild Bait", null);

            api.AddBoolOption(manifest, () => config.EnableWildBaitEffect, (bool val) => config.EnableWildBaitEffect = val,
                () => "Enable Wild Bait Effect", null);
            api.AddNumberOption(manifest, () => config.WildBaitQualityChanceModifier, (float val) => config.WildBaitQualityChanceModifier = val,
                () => "Quality Chance Modifier", null, 0f);
            api.AddNumberOption(manifest, () => config.WildBaitDoubleItemAmountChance, (int val) => config.WildBaitDoubleItemAmountChance = val,
                () => "Double Item Amount Chance", null, 0, 100);
            api.AddTextOption(manifest, () => config.WildBaitSpecialItem, (string val) => config.WildBaitSpecialItem = val,
                () => "Special Item", () => "By default: a rainbow shell");
            api.AddNumberOption(manifest, () => config.WildBaitSpecialItemChance, (int val) => config.WildBaitSpecialItemChance = val,
                () => "Special Item Chance", null, 0, 100);
            api.AddBoolOption(manifest, () => config.WildBaitSpecialItemHasNoQuality, (bool val) => config.WildBaitSpecialItemHasNoQuality = val,
                () => "Special Item Has No Quality", () => "Whether the special item is always default quality");

            api.AddSectionTitle(manifest, () => "Magic Bait", null);

            api.AddBoolOption(manifest, () => config.EnableMagicBaitEffect, (bool val) => config.EnableMagicBaitEffect = val,
                () => "Enable Magic Bait Effect", null);
            api.AddNumberOption(manifest, () => config.MagicBaitQualityChanceModifier, (float val) => config.MagicBaitQualityChanceModifier = val,
                () => "Quality Chance Modifier", null, 0f);
            api.AddNumberOption(manifest, () => config.MagicBaitDoubleItemAmountChance, (int val) => config.MagicBaitDoubleItemAmountChance = val,
                () => "Double Item Amount Chance", null, 0, 100);
            api.AddTextOption(manifest, () => config.MagicBaitSpecialItem, (string val) => config.MagicBaitSpecialItem = val,
                () => "Special Item", () => "By default: a rainbow shell");
            api.AddNumberOption(manifest, () => config.MagicBaitSpecialItemChance, (int val) => config.MagicBaitSpecialItemChance = val,
                () => "Special Item Chance", null, 0, 100);
            api.AddBoolOption(manifest, () => config.MagicBaitSpecialItemHasNoQuality, (bool val) => config.MagicBaitSpecialItemHasNoQuality = val,
                () => "Special Item Has No Quality", () => "Whether the special item is always default quality");

            api.AddSectionTitle(manifest, () => "Deluxe Bait", null);

            api.AddBoolOption(manifest, () => config.EnableDeluxeBaitEffect, (bool val) => config.EnableDeluxeBaitEffect = val,
                () => "Enable Deluxe Bait Effect", null);
            api.AddNumberOption(manifest, () => config.DeluxeBaitQualityChanceModifier, (float val) => config.DeluxeBaitQualityChanceModifier = val,
                () => "Quality Chance Modifier", null, 0f);
            api.AddNumberOption(manifest, () => config.DeluxeBaitDoubleItemAmountChance, (int val) => config.DeluxeBaitDoubleItemAmountChance = val,
                () => "Special Item Chance", null, 0, 100);
            api.AddTextOption(manifest, () => config.DeluxeBaitSpecialItem, (string val) => config.DeluxeBaitSpecialItem = val,
                () => "Special Item", () => "By default: a rainbow shell");
            api.AddNumberOption(manifest, () => config.DeluxeBaitSpecialItemChance, (int val) => config.DeluxeBaitSpecialItemChance = val,
                () => "Special Item Chance", null, 0, 100);
            api.AddBoolOption(manifest, () => config.DeluxeBaitSpecialItemHasNoQuality, (bool val) => config.DeluxeBaitSpecialItemHasNoQuality = val,
                () => "Special Item Has No Quality", () => "Whether the special item is always default quality");

            api.AddSectionTitle(manifest, () => "Challenge Bait", null);

            api.AddBoolOption(manifest, () => config.EnableChallengeBaitEffect, (bool val) => config.EnableChallengeBaitEffect = val,
                () => "Enable Challenge Bait Effect", null);
            api.AddNumberOption(manifest, () => config.ChallengeBaitQualityChanceModifier, (float val) => config.ChallengeBaitQualityChanceModifier = val,
                () => "Quality Chance Modifier", null, 0f);
            api.AddNumberOption(manifest, () => config.ChallengeBaitDoubleItemAmountChance, (int val) => config.ChallengeBaitDoubleItemAmountChance = val,
                () => "Double Item Amount Chance", null, 0, 100);
            api.AddTextOption(manifest, () => config.ChallengeBaitSpecialItem, (string val) => config.ChallengeBaitSpecialItem = val,
                () => "Special Item", () => "By default: a rainbow shell");
            api.AddNumberOption(manifest, () => config.ChallengeBaitSpecialItemChance, (int val) => config.ChallengeBaitSpecialItemChance = val,
                () => "Special Item Chance", null, 0, 100);
            api.AddBoolOption(manifest, () => config.ChallengeBaitSpecialItemHasNoQuality, (bool val) => config.ChallengeBaitSpecialItemHasNoQuality = val,
                () => "Special Item Has No Quality", () => "Whether the special item is always default quality");

            api.AddSectionTitle(manifest, () => "Magnet Bait", null);

            api.AddBoolOption(manifest, () => config.EnableMagnetBaitEffect, (bool val) => config.EnableMagnetBaitEffect = val,
                () => "Enable Magnet Bait Effect", null);
            api.AddNumberOption(manifest, () => config.MagnetBaitQualityChanceModifier, (float val) => config.MagnetBaitQualityChanceModifier = val,
                () => "Quality Chance Modifier", null, 0f);
            api.AddNumberOption(manifest, () => config.MagnetBaitDoubleItemAmountChance, (int val) => config.MagnetBaitDoubleItemAmountChance = val,
                () => "Double Item Amount Chance", null, 0, 100);
            api.AddTextOption(manifest, () => config.MagnetBaitSpecialItem, (string val) => config.MagnetBaitSpecialItem = val,
                () => "Special Item", () => "By default: a treasure chest");
            api.AddNumberOption(manifest, () => config.MagnetBaitSpecialItemChance, (int val) => config.MagnetBaitSpecialItemChance = val,
                () => "Special Item Chance", null, 0, 100);
            api.AddBoolOption(manifest, () => config.MagnetBaitSpecialItemHasNoQuality, (bool val) => config.MagnetBaitSpecialItemHasNoQuality = val,
                () => "Special Item Has No Quality", () => "Whether the special item is always default quality");

            api.AddSectionTitle(manifest, () => "Custom/Modded Bait", null);

            api.AddBoolOption(manifest, () => config.EnableCustomModdedBaitEffect, (bool val) => config.EnableCustomModdedBaitEffect = val,
                () => "Enable Custom Bait Effect", null);
            api.AddNumberOption(manifest, () => config.CustomModdedBaitQualityChanceModifier, (float val) => config.CustomModdedBaitQualityChanceModifier = val,
                () => "Quality Chance Modifier", null, 0f);
            api.AddNumberOption(manifest, () => config.CustomModdedBaitDoubleItemAmountChance, (int val) => config.CustomModdedBaitDoubleItemAmountChance = val,
                () => "Double Item Amount Chance", null, 0, 100);
            api.AddTextOption(manifest, () => config.CustomModdedBaitSpecialItem, (string val) => config.CustomModdedBaitSpecialItem = val,
                () => "Special Item", () => "By default: a rainbow shell");
            api.AddNumberOption(manifest, () => config.CustomModdedBaitSpecialItemChance, (int val) => config.CustomModdedBaitSpecialItemChance = val,
                () => "Special Item Chance", null, 0, 100);
            api.AddBoolOption(manifest, () => config.CustomModdedBaitSpecialItemHasNoQuality, (bool val) => config.CustomModdedBaitSpecialItemHasNoQuality = val,
                () => "Special Item Has No Quality", () => "Whether the special item is always default quality");
        }
    }
}