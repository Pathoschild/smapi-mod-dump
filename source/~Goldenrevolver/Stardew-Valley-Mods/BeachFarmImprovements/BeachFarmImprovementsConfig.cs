/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System;

namespace BeachFarmImprovements
{
    public interface IGenericModConfigMenuApi
    {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);

        void AddSectionTitle(IManifest mod, Func<string> text, Func<string> tooltip = null);

        void AddParagraph(IManifest mod, Func<string> text);

        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);

        void AddNumberOption(IManifest mod, Func<int> getValue, Action<int> setValue, Func<string> name, Func<string> tooltip = null, int? min = null, int? max = null, int? interval = null, Func<int, string> formatValue = null, string fieldId = null);

        void AddNumberOption(IManifest mod, Func<float> getValue, Action<float> setValue, Func<string> name, Func<string> tooltip = null, float? min = null, float? max = null, float? interval = null, Func<float, string> formatValue = null, string fieldId = null);

        void AddTextOption(IManifest mod, Func<string> getValue, Action<string> setValue, Func<string> name, Func<string> tooltip = null, string[] allowedValues = null, Func<string, string> formatAllowedValue = null, string fieldId = null);

        void AddKeybindList(IManifest mod, Func<KeybindList> getValue, Action<KeybindList> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);

        void SetTitleScreenOnlyForNextOptions(IManifest mod, bool titleScreenOnly);
    }

    public class BeachFarmImprovementsConfig
    {
        public int SprinklersInSandPrice { get; set; } = 10000;

        public int BeachFarmExpansionPrice { get; set; } = 50000;

        public bool CantRefillCanWithSaltWater { get; set; } = true;

        public bool CantGrowInUnfertilizedSand { get; set; } = true;

        public static void VerifyConfigValues(BeachFarmImprovementsConfig config, BeachFarmImprovements mod)
        {
            bool invalidConfig = false;

            if (config.SprinklersInSandPrice < 0)
            {
                invalidConfig = true;
                config.SprinklersInSandPrice = 0;
            }

            if (config.BeachFarmExpansionPrice < 0)
            {
                invalidConfig = true;
                config.BeachFarmExpansionPrice = 0;
            }

            if (invalidConfig)
            {
                mod.DebugLog("At least one config value was out of range and was reset.");
                mod.Helper.WriteConfig(config);
            }
        }

        public static void SetUpModConfigMenu(BeachFarmImprovementsConfig config, BeachFarmImprovements mod)
        {
            IGenericModConfigMenuApi api = mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (api == null)
            {
                return;
            }

            var manifest = mod.ModManifest;

            api.Register(
                mod: manifest,
                reset: delegate
                {
                    config = new BeachFarmImprovementsConfig();
                },
                save: delegate
                {
                    mod.Helper.WriteConfig(config);
                    VerifyConfigValues(config, mod);
                }
            );

            api.AddSectionTitle(manifest, () => "Improvement Prices", null);

            api.AddNumberOption(manifest, () => config.SprinklersInSandPrice, (int val) => config.SprinklersInSandPrice = val, () => "Sprinklers In Sand Price", null, 0);
            api.AddNumberOption(manifest, () => config.BeachFarmExpansionPrice, (int val) => config.BeachFarmExpansionPrice = val, () => "Beach Farm Expansion Price", null, 0);

            api.AddSectionTitle(manifest, () => "Additional Difficulty", null);

            api.AddBoolOption(manifest, () => config.CantGrowInUnfertilizedSand, (bool val) => config.CantGrowInUnfertilizedSand = val, () => "Can't Grow Crops\nIn Unfertilized Sand", null);

            if (mod.Helper.ModRegistry.IsLoaded("Goldenrevolver.WateringGrantsXP"))
            {
                string setting = "Can't Refill Watering Can With Salt Water";
                string waterMod = "'Watering Grants XP' detected!";
                string waterMod2 = "Please change this setting in the config file of that mod instead.";

                api.AddParagraph(manifest, () => $"{setting}: {waterMod} {waterMod2}");
            }
            else
            {
                api.AddBoolOption(manifest, () => config.CantRefillCanWithSaltWater, (bool val) => config.CantRefillCanWithSaltWater = val, () => "Can't Refill Watering Can\nWith Salt Water", null);
            }
        }
    }
}