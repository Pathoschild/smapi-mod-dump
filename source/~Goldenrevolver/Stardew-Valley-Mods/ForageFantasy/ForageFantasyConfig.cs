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
    using StardewModdingAPI.Utilities;
    using StardewValley;
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

        void AddKeybindList(IManifest mod, Func<KeybindList> getValue, Action<KeybindList> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);

        void SetTitleScreenOnlyForNextOptions(IManifest mod, bool titleScreenOnly);
    }

    /// <summary>
    /// Config file for the mod
    /// </summary>
    public class ForageFantasyConfig
    {
        private static readonly KeybindList TreeMenuKeyDefault = KeybindList.Parse("None");

        public bool BerryBushQuality { get; set; } = true;

        public bool MushroomBoxQuality { get; set; } = true;

        public int TapperQualityOptions { get; set; } = 1;

        public bool TapperQualityRequiresTapperPerk { get; set; } = false;

        public int BerryBushChanceToGetXP { get; set; } = 100;

        public int BerryBushXPAmount { get; set; } = 1;

        public int MushroomBoxXPAmount { get; set; } = 1;

        public int TapperXPAmount { get; set; } = 3;

        public int SmallStumpBonusXPAmount { get; set; } = 1;

        public int TwigDebrisXPAmount { get; set; } = 1;

        public bool AutomationHarvestsGrantXP { get; set; } = false;

        public bool TapperDaysNeededChangesEnabled { get; set; } = true;

        public int MapleTapperDaysNeeded { get; set; } = 9;

        public int OakTapperDaysNeeded { get; set; } = 7;

        public int PineTapperDaysNeeded { get; set; } = 5;

        public bool MushroomTreeTappersConsistentDaysNeeded { get; set; } = true;

        public bool CommonFiddleheadFern { get; set; } = true;

        public bool WildAndFineGrapes { get; set; } = true;

        public bool ForageSurvivalBurger { get; set; } = true;

        public KeybindList TreeMenuKey { get; set; } = TreeMenuKeyDefault;

        public bool MushroomTapperCalendar { get; set; } = false;

        private static string[] TQChoices { get; set; } = new string[] { "Disabled", "ForageLevelBased", "ForageLevelBasedNoBotanist", "TreeAgeBasedMonths", "TreeAgeBasedYears" };

        public static void VerifyConfigValues(ForageFantasyConfig config, ForageFantasy mod)
        {
            bool invalidConfig = false;

            if (config.MapleTapperDaysNeeded <= 0)
            {
                invalidConfig = true;
                config.MapleTapperDaysNeeded = 1;
            }

            if (config.PineTapperDaysNeeded <= 0)
            {
                invalidConfig = true;
                config.PineTapperDaysNeeded = 1;
            }

            if (config.OakTapperDaysNeeded <= 0)
            {
                invalidConfig = true;
                config.OakTapperDaysNeeded = 1;
            }

            if (config.TapperQualityOptions < 0 || config.TapperQualityOptions > 4)
            {
                invalidConfig = true;
                config.TapperQualityOptions = 0;
            }

            if (config.BerryBushChanceToGetXP < 0)
            {
                invalidConfig = true;
                config.BerryBushChanceToGetXP = 0;
            }

            if (config.BerryBushChanceToGetXP > 100)
            {
                invalidConfig = true;
                config.BerryBushChanceToGetXP = 100;
            }

            if (config.BerryBushXPAmount < 0)
            {
                invalidConfig = true;
                config.BerryBushXPAmount = 0;
            }

            if (config.TapperXPAmount < 0)
            {
                invalidConfig = true;
                config.TapperXPAmount = 0;
            }

            if (config.MushroomBoxXPAmount < 0)
            {
                invalidConfig = true;
                config.MushroomBoxXPAmount = 0;
            }

            if (config.SmallStumpBonusXPAmount < 0)
            {
                invalidConfig = true;
                config.SmallStumpBonusXPAmount = 0;
            }

            if (config.TwigDebrisXPAmount < 0)
            {
                invalidConfig = true;
                config.TwigDebrisXPAmount = 0;
            }

            if (invalidConfig)
            {
                mod.DebugLog("At least one config value was out of range and was reset.");
                mod.Helper.WriteConfig(config);
            }
        }

        public static void InvalidateCache(ForageFantasy mod)
        {
            try
            {
                // CommonFiddleheadFern, ForageSurvivalBurger
                mod.Helper.GameContent.InvalidateCacheAndLocalized("Data/CraftingRecipes");

                // CommonFiddleheadFern
                mod.Helper.GameContent.InvalidateCacheAndLocalized("Data/Locations");

                // ForageSurvivalBurger
                mod.Helper.GameContent.InvalidateCacheAndLocalized("Data/CookingRecipes");

                // Tapper days needed changes, Fine Grapes
                mod.Helper.GameContent.InvalidateCacheAndLocalized("Data/Objects");

                // Tapper days needed changes, Tree Menu
                mod.Helper.GameContent.InvalidateCacheAndLocalized("Data/WildTrees");

                // Fine Grapes
                mod.Helper.GameContent.InvalidateCacheAndLocalized("Data/NPCGiftTastes");
            }
            catch (Exception e)
            {
                mod.DebugLog($"Exception when trying to invalidate cache on config change {e}");
            }
        }

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1107:CodeMustNotContainMultipleStatementsOnOneLine", Justification = "Reviewed.")]
        [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Necessary.")]
        public static void SetUpModConfigMenu(ForageFantasyConfig config, ForageFantasy mod)
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
                    // if the world is ready, then we are not in the main menu, so reset should only reset the keybindings and calendar
                    if (Context.IsWorldReady)
                    {
                        config.TreeMenuKey = TreeMenuKeyDefault;
                        config.MushroomTapperCalendar = false;
                    }
                    else
                    {
                        config = new ForageFantasyConfig();
                    }
                    InvalidateCache(mod);
                },
                save: delegate
                {
                    mod.Helper.WriteConfig(config);
                    VerifyConfigValues(config, mod);
                    InvalidateCache(mod);
                }
            );

            api.SetTitleScreenOnlyForNextOptions(manifest, true);

            api.AddSectionTitle(manifest, GetConfigName(mod, "SectionQualityTweaks"));

            api.AddBoolOption(manifest, () => config.BerryBushQuality, (bool val) => config.BerryBushQuality = val,
                GetConfigName(mod, "BerryBushQuality"), GetConfigDescription(mod, "BerryBushQuality"));
            api.AddBoolOption(manifest, () => config.MushroomBoxQuality, (bool val) => config.MushroomBoxQuality = val,
                GetConfigName(mod, "MushroomBoxQuality"), GetConfigDescription(mod, "MushroomBoxQuality"));
            api.AddTextOption(manifest, () => GetElementFromConfig(TQChoices, config.TapperQualityOptions), (string val) => config.TapperQualityOptions = GetIndexFromArrayElement(TQChoices, val),
                GetConfigName(mod, "TapperQualityOptions"), GetConfigDescription(mod, "TapperQualityOptions"), TQChoices, GetConfigDropdownChoice(mod, "TapperQualityOptions"));
            api.AddBoolOption(manifest, () => config.TapperQualityRequiresTapperPerk, (bool val) => config.TapperQualityRequiresTapperPerk = val,
                GetConfigName(mod, "TapperQualityRequiresTapperPerk"), GetConfigDescription(mod, "TapperQualityRequiresTapperPerk"));

            api.AddSectionTitle(manifest, GetConfigName(mod, "SectionXPRewards"));

            api.AddNumberOption(manifest, () => config.BerryBushChanceToGetXP, (int val) => config.BerryBushChanceToGetXP = val,
                GetConfigName(mod, "BerryBushChanceToGetXP"), GetConfigDescription(mod, "BerryBushChanceToGetXP"), 0, 100);
            api.AddNumberOption(manifest, () => config.BerryBushXPAmount, (int val) => config.BerryBushXPAmount = val,
                GetConfigName(mod, "BerryBushXPAmount"), GetConfigDescription(mod, "BerryBushXPAmount"), 0);
            api.AddNumberOption(manifest, () => config.MushroomBoxXPAmount, (int val) => config.MushroomBoxXPAmount = val,
                GetConfigName(mod, "MushroomBoxXPAmount"), GetConfigDescription(mod, "MushroomBoxXPAmount"), 0);
            api.AddNumberOption(manifest, () => config.TapperXPAmount, (int val) => config.TapperXPAmount = val,
                GetConfigName(mod, "TapperXPAmount"), GetConfigDescription(mod, "TapperXPAmount"), 0);
            api.AddNumberOption(manifest, () => config.SmallStumpBonusXPAmount, (int val) => config.SmallStumpBonusXPAmount = val,
                GetConfigName(mod, "SmallStumpBonusXPAmount"), GetConfigDescription(mod, "SmallStumpBonusXPAmount"), 0);
            api.AddNumberOption(manifest, () => config.TwigDebrisXPAmount, (int val) => config.TwigDebrisXPAmount = val,
                GetConfigName(mod, "TwigDebrisXPAmount"), GetConfigDescription(mod, "TwigDebrisXPAmount"), 0);
            api.AddBoolOption(manifest, () => config.AutomationHarvestsGrantXP, (bool val) => config.AutomationHarvestsGrantXP = val,
                GetConfigName(mod, "AutomationHarvestsGrantXP"), GetConfigDescription(mod, "AutomationHarvestsGrantXP"));

            api.AddSectionTitle(manifest, GetConfigName(mod, "SectionTapperDaysNeededChanges"));

            api.AddBoolOption(manifest, () => config.TapperDaysNeededChangesEnabled, (bool val) => config.TapperDaysNeededChangesEnabled = val,
                GetConfigName(mod, "TapperDaysNeededChangesEnabled"), GetConfigDescription(mod, "TapperDaysNeededChangesEnabled"));
            api.AddNumberOption(manifest, () => config.MapleTapperDaysNeeded, (int val) => config.MapleTapperDaysNeeded = val,
                GetConfigName(mod, "MapleDaysNeeded"), GetConfigDescription(mod, "MapleDaysNeeded"), 1);
            api.AddNumberOption(manifest, () => config.OakTapperDaysNeeded, (int val) => config.OakTapperDaysNeeded = val,
                GetConfigName(mod, "OakDaysNeeded"), GetConfigDescription(mod, "OakDaysNeeded"), 1);
            api.AddNumberOption(manifest, () => config.PineTapperDaysNeeded, (int val) => config.PineTapperDaysNeeded = val,
                GetConfigName(mod, "PineDaysNeeded"), GetConfigDescription(mod, "PineDaysNeeded"), 1);
            api.AddBoolOption(manifest, () => config.MushroomTreeTappersConsistentDaysNeeded, (bool val) => config.MushroomTreeTappersConsistentDaysNeeded = val,
                GetConfigName(mod, "MushroomTreeTappersConsistencyChange"), GetConfigDescription(mod, "MushroomTreeTappersConsistencyChange"));

            api.AddSectionTitle(manifest, GetConfigName(mod, "SectionOtherFeatures"));

            api.AddBoolOption(manifest, () => config.CommonFiddleheadFern, (bool val) => config.CommonFiddleheadFern = val,
                GetConfigName(mod, "CommonFiddleheadFern"), GetConfigDescription(mod, "CommonFiddleheadFern"));
            api.AddBoolOption(manifest, () => config.ForageSurvivalBurger, (bool val) => config.ForageSurvivalBurger = val,
                GetConfigName(mod, "ForageSurvivalBurger"), GetConfigDescription(mod, "ForageSurvivalBurger"));
            api.AddBoolOption(manifest, () => config.WildAndFineGrapes, (bool val) => config.WildAndFineGrapes = val,
                GetConfigName(mod, "WildAndFineGrapes"), GetConfigDescription(mod, "WildAndFineGrapes"));

            api.SetTitleScreenOnlyForNextOptions(manifest, false);

            api.AddBoolOption(manifest, () => config.MushroomTapperCalendar, (bool val) => config.MushroomTapperCalendar = val,
                GetConfigName(mod, "MushroomTapperCalendar"), GetConfigDescription(mod, "MushroomTapperCalendar"));

            api.AddKeybindList(manifest, () => config.TreeMenuKey, (KeybindList keybindList) => config.TreeMenuKey = keybindList,
                GetConfigName(mod, "TreeMenuKey"), GetConfigDescription(mod, "TreeMenuKey"));

            api.SetTitleScreenOnlyForNextOptions(manifest, true);
        }

        private static Func<string, string> GetConfigDropdownChoice(ForageFantasy mod, string key)
        {
            return (s) => mod.Helper.Translation.Get($"Config{key}{s}");
        }

        private static Func<string> GetConfigName(ForageFantasy mod, string key)
        {
            return () => mod.Helper.Translation.Get($"Config{key}");
        }

        private static Func<string> GetConfigDescription(ForageFantasy mod, string key)
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

    /// <summary>
    /// Extension methods for IGameContentHelper.
    /// </summary>
    public static class GameContentHelperExtensions
    {
        /// <summary>
        /// Invalidates both an asset and the locale-specific version of an asset.
        /// </summary>
        /// <param name="helper">The game content helper.</param>
        /// <param name="assetName">The (string) asset to invalidate.</param>
        /// <returns>if something was invalidated.</returns>
        public static bool InvalidateCacheAndLocalized(this IGameContentHelper helper, string assetName)
            => helper.InvalidateCache(assetName)
                | (helper.CurrentLocaleConstant != LocalizedContentManager.LanguageCode.en && helper.InvalidateCache(assetName + "." + helper.CurrentLocale));
    }
}