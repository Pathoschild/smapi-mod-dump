/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace MushroomRancher
{
    using StardewModdingAPI;
    using System;
    using System.Diagnostics.CodeAnalysis;

    public interface IGenericModConfigMenuApi
    {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);

        void AddSectionTitle(IManifest mod, Func<string> text, Func<string> tooltip = null);

        void AddNumberOption(IManifest mod, Func<int> getValue, Action<int> setValue, Func<string> name, Func<string> tooltip = null, int? min = null, int? max = null, int? interval = null, Func<int, string> formatValue = null, string fieldId = null);

        void AddNumberOption(IManifest mod, Func<float> getValue, Action<float> setValue, Func<string> name, Func<string> tooltip = null, float? min = null, float? max = null, float? interval = null, Func<float, string> formatValue = null, string fieldId = null);

        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);

        void AddTextOption(IManifest mod, Func<string> getValue, Action<string> setValue, Func<string> name, Func<string> tooltip = null, string[] allowedValues = null, Func<string, string> formatAllowedValue = null, string fieldId = null);
    }

    /// <summary>
    /// Config file for the mod
    /// </summary>
    public class MushroomRancherConfig
    {
        public int HutchInterior { get; set; } = 1;

        public bool RandomizeMonsterPositionInHutch { get; set; } = true;

        public bool RandomizeMonsterPositionOnlyAffectsLivingMushrooms { get; set; } = true;

        public bool RemovableSlimeHutchIncubator { get; set; } = true;

        public int IncubatorDuration { get; set; } = 2;

        public bool IncubatorDurationIsInDaysInsteadOfMinutes { get; set; } = true;

        public bool IncubatorIsAffectedByCoopmaster { get; set; } = true;

        public bool IncubatorWobblesWhileIncubating { get; set; } = true;

        internal const string DefaultIncubatorAdditionalRequiredItemID = "(O)773";
        public string IncubatorAdditionalRequiredItemID { get; set; } = DefaultIncubatorAdditionalRequiredItemID;

        public int IncubatorAdditionalRequiredItemCount { get; set; } = 5;

        internal const string DefaultIncubatorRecipe = "709 1 388 20";

        public string IncubatorRecipe { get; set; } = DefaultIncubatorRecipe;

        internal const string DefaultIncubatorRecipeUnlock = "Foraging 6";

        public string IncubatorRecipeUnlock { get; set; } = DefaultIncubatorRecipeUnlock;

        internal static string[] InteriorChoices { get; set; } = new string[] { "Disabled", "Dynamic", "Slime", "Cave", "Volcano" };

        internal static string[] IncubatorDurationIsInChoices { get; set; } = new string[] { "Minutes", "Days" };

        public static void VerifyConfigValues(MushroomRancherConfig config, MushroomRancher mod)
        {
            bool invalidConfig = false;
            bool minorInvalidConfig = false;

            if (config.HutchInterior < 0 || config.HutchInterior >= InteriorChoices.Length)
            {
                config.HutchInterior = 1;
                invalidConfig = true;
            }

            if (config.IncubatorDuration < 1)
            {
                config.IncubatorDuration = 1;
                invalidConfig = true;
            }

            if (config.IncubatorAdditionalRequiredItemCount < 0)
            {
                config.IncubatorAdditionalRequiredItemCount = 0;
                minorInvalidConfig = true;
            }

            if (minorInvalidConfig || invalidConfig)
            {
                if (invalidConfig)
                {
                    mod.DebugLog("At least one config value was out of range and was reset.");
                }

                mod.Helper.WriteConfig(config);
            }
        }

        public static void InvalidateCache(MushroomRancher mod)
        {
            try
            {
                mod.Helper.GameContent.InvalidateCacheAndLocalized("Data/CraftingRecipes");
                mod.Helper.GameContent.InvalidateCacheAndLocalized("Data/BigCraftables");
                mod.Helper.GameContent.InvalidateCacheAndLocalized("Data/Machines");
            }
            catch (Exception e)
            {
                mod.DebugLog($"Exception when trying to invalidate cache after config change: {e}");
            }
        }

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1107:CodeMustNotContainMultipleStatementsOnOneLine", Justification = "Reviewed.")]
        [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Necessary.")]
        public static void SetUpModConfigMenu(MushroomRancherConfig config, MushroomRancher mod)
        {
            IGenericModConfigMenuApi api = mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (api == null)
            {
                return;
            }

            var manifest = mod.ModManifest;

            api.Register(
                mod: manifest,
                reset: () => { config = new MushroomRancherConfig(); InvalidateCache(mod); },
                save: () => { mod.Helper.WriteConfig(config); VerifyConfigValues(config, mod); InvalidateCache(mod); }
            );

            api.AddSectionTitle(manifest, GetConfigName(mod, "HutchSection"));

            api.AddTextOption(manifest, () => GetElementFromConfig(InteriorChoices, config.HutchInterior), (string val) => config.HutchInterior = GetIndexFromArrayElement(InteriorChoices, val),
                GetConfigName(mod, "HutchInterior"), null, InteriorChoices, GetConfigDropdownChoice(mod, "HutchInterior"));
            api.AddBoolOption(manifest, () => config.RemovableSlimeHutchIncubator, (bool val) => config.RemovableSlimeHutchIncubator = val,
                GetConfigName(mod, "RemovableSlimeHutchIncubator"), GetConfigDescription(mod, "RemovableSlimeHutchIncubator"));
            api.AddBoolOption(manifest, () => config.RandomizeMonsterPositionInHutch, (bool val) => config.RandomizeMonsterPositionInHutch = val,
                GetConfigName(mod, "RandomizeMonsterPositionInHutch"), GetConfigDescription(mod, "RandomizeMonsterPositionInHutch"));
            api.AddBoolOption(manifest, () => config.RandomizeMonsterPositionOnlyAffectsLivingMushrooms, (bool val) => config.RandomizeMonsterPositionOnlyAffectsLivingMushrooms = val,
                GetConfigName(mod, "RandomizeMonsterPositionOnlyAffectsLivingMushrooms"), null);

            api.AddSectionTitle(manifest, GetConfigName(mod, "IncubatorSection"));

            api.AddNumberOption(manifest, () => config.IncubatorDuration, (int val) => config.IncubatorDuration = val,
                GetConfigName(mod, "IncubatorDuration"), GetConfigDescription(mod, "IncubatorDuration"), 1);
            api.AddTextOption(manifest, () => GetElementFromConfig(IncubatorDurationIsInChoices, config.IncubatorDurationIsInDaysInsteadOfMinutes ? 1 : 0), (string val) => config.IncubatorDurationIsInDaysInsteadOfMinutes = GetIndexFromArrayElement(IncubatorDurationIsInChoices, val) == 1,
                GetConfigName(mod, "IncubatorDurationIsIn"), null, IncubatorDurationIsInChoices, GetConfigDropdownChoice(mod, "IncubatorDurationIsIn"));

            api.AddBoolOption(manifest, () => config.IncubatorIsAffectedByCoopmaster, (bool val) => config.IncubatorIsAffectedByCoopmaster = val,
                GetConfigName(mod, "IncubatorIsAffectedByCoopmaster"), GetConfigDescription(mod, "IncubatorIsAffectedByCoopmaster"));

            api.AddBoolOption(manifest, () => config.IncubatorWobblesWhileIncubating, (bool val) => config.IncubatorWobblesWhileIncubating = val,
                GetConfigName(mod, "IncubatorWobblesWhileIncubating"));

            api.AddTextOption(manifest, () => config.IncubatorAdditionalRequiredItemID, (string val) => config.IncubatorAdditionalRequiredItemID = val,
                GetConfigName(mod, "IncubatorAdditionalRequiredItemID"), GetConfigDescription(mod, "IncubatorAdditionalRequiredItemID"));
            api.AddNumberOption(manifest, () => config.IncubatorAdditionalRequiredItemCount, (int val) => config.IncubatorAdditionalRequiredItemCount = val,
                GetConfigName(mod, "IncubatorAdditionalRequiredItemCount"), GetConfigDescription(mod, "IncubatorAdditionalRequiredItemCount"), 0);

            api.AddTextOption(manifest, () => config.IncubatorRecipe, (string val) => config.IncubatorRecipe = val,
                GetConfigName(mod, "IncubatorRecipe"), GetConfigDescription(mod, "IncubatorRecipe"));
            api.AddTextOption(manifest, () => config.IncubatorRecipeUnlock, (string val) => config.IncubatorRecipeUnlock = val,
                GetConfigName(mod, "IncubatorRecipeUnlock"), GetConfigDescription(mod, "IncubatorRecipeUnlock"));
        }

        private static Func<string, string> GetConfigDropdownChoice(MushroomRancher mod, string key)
        {
            return (s) => mod.Helper.Translation.Get($"Config{key}{s}");
        }

        private static Func<string> GetConfigName(MushroomRancher mod, string key)
        {
            return () => mod.Helper.Translation.Get($"Config{key}");
        }

        private static Func<string> GetConfigDescription(MushroomRancher mod, string key)
        {
            return () => mod.Helper.Translation.Get($"Config{key}Description");
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