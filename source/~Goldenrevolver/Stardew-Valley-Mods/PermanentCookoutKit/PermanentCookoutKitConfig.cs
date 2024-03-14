/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace PermanentCookoutKit
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
    public class PermanentCookoutKitConfig
    {
        public int WoodNeeded { get; set; } = 5;

        public int CoalNeeded { get; set; } = 1;

        public int FiberNeeded { get; set; } = 1;

        public float DriftwoodMultiplier { get; set; } = 5;

        public float HardwoodMultiplier { get; set; } = 5;

        public float NewspaperMultiplier { get; set; } = 2;

        public float WoolMultiplier { get; set; } = 0;

        public float ClothMultiplier { get; set; } = 0;

        public int CharcoalKilnWoodNeeded { get; set; } = 10;

        public int CharcoalKilnTimeNeeded { get; set; } = 30;

        public static void VerifyConfigValues(PermanentCookoutKitConfig config, PermanentCookoutKit mod)
        {
            bool invalidConfig = false;

            if (config.WoodNeeded < 0)
            {
                invalidConfig = true;
                config.WoodNeeded = 0;
            }

            if (config.FiberNeeded < 0)
            {
                invalidConfig = true;
                config.FiberNeeded = 0;
            }

            if (config.CoalNeeded < 0)
            {
                invalidConfig = true;
                config.CoalNeeded = 0;
            }

            if (config.DriftwoodMultiplier < 0)
            {
                invalidConfig = true;
                config.DriftwoodMultiplier = 0;
            }

            if (config.HardwoodMultiplier < 0)
            {
                invalidConfig = true;
                config.HardwoodMultiplier = 0;
            }

            if (config.NewspaperMultiplier < 0)
            {
                invalidConfig = true;
                config.NewspaperMultiplier = 0;
            }

            if (config.WoolMultiplier < 0)
            {
                invalidConfig = true;
                config.WoolMultiplier = 0;
            }

            if (config.ClothMultiplier < 0)
            {
                invalidConfig = true;
                config.ClothMultiplier = 0;
            }

            if (config.CharcoalKilnWoodNeeded < 1)
            {
                invalidConfig = true;
                config.CharcoalKilnWoodNeeded = 1;
            }

            if (config.CharcoalKilnTimeNeeded < 10)
            {
                invalidConfig = true;
                config.CharcoalKilnTimeNeeded = 10;
            }

            if (invalidConfig)
            {
                mod.DebugLog("At least one config value was out of range and was reset.");
                mod.Helper.WriteConfig(config);
            }
        }

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1107:CodeMustNotContainMultipleStatementsOnOneLine", Justification = "Reviewed.")]
        [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Necessary.")]
        public static void SetUpModConfigMenu(PermanentCookoutKitConfig config, PermanentCookoutKit mod)
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
                    config = new PermanentCookoutKitConfig();
                    mod.Helper.GameContent.InvalidateCacheAndLocalized("Data/Machines");
                },
                save: () =>
                {
                    mod.Helper.WriteConfig(config);
                    VerifyConfigValues(config, mod);
                    mod.Helper.GameContent.InvalidateCacheAndLocalized("Data/Machines");
                }
            );

            api.AddSectionTitle(manifest, () => "Cookout Kit Reignition Cost", null);

            api.AddNumberOption(manifest, () => config.WoodNeeded, (int val) => config.WoodNeeded = val, () => "Wood Needed", null, 0);
            api.AddNumberOption(manifest, () => config.CoalNeeded, (int val) => config.CoalNeeded = val, () => "Coal Needed", null, 0);
            api.AddNumberOption(manifest, () => config.FiberNeeded, (int val) => config.FiberNeeded = val, () => "Fiber/ Kindling Needed", null, 0);

            api.AddSectionTitle(manifest, () => "Charcoal Kiln", null);

            api.AddNumberOption(manifest, () => config.CharcoalKilnWoodNeeded, (int val) => config.CharcoalKilnWoodNeeded = val, () => "Wood Needed", () => "Also works with driftwood and hardwood", 1);
            api.AddNumberOption(manifest, () => config.CharcoalKilnTimeNeeded, (int val) => config.CharcoalKilnTimeNeeded = val, () => "Time Needed", () => "The game only checks every 10 minutes", 10);

            api.AddSectionTitle(manifest, () => "Wood Multipliers", null);

            api.AddNumberOption(manifest, () => config.DriftwoodMultiplier, (float val) => config.DriftwoodMultiplier = val, () => "Driftwood Multiplier¹", null, 0);
            api.AddNumberOption(manifest, () => config.HardwoodMultiplier, (float val) => config.HardwoodMultiplier = val, () => "Hardwood Multiplier¹", null, 0);

            api.AddSectionTitle(manifest, () => "Kindling Multipliers", null);

            api.AddNumberOption(manifest, () => config.NewspaperMultiplier, (float val) => config.NewspaperMultiplier = val, () => "Newspaper Multiplier¹", null, 0);
            api.AddNumberOption(manifest, () => config.WoolMultiplier, (float val) => config.WoolMultiplier = val, () => "Wool Multiplier¹", null, 0);
            api.AddNumberOption(manifest, () => config.ClothMultiplier, (float val) => config.ClothMultiplier = val, () => "Cloth Multiplier¹", null, 0);

            // this is a spacer
            api.AddSectionTitle(manifest, () => string.Empty, null);
            api.AddSectionTitle(manifest, () => "1: Set To 0 To Disallow Using It", null);
        }
    }
}