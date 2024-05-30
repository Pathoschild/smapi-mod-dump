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

        void AddParagraph(IManifest mod, Func<string> text);

        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);

        void AddNumberOption(IManifest mod, Func<int> getValue, Action<int> setValue, Func<string> name, Func<string> tooltip = null, int? min = null, int? max = null, int? interval = null, Func<int, string> formatValue = null, string fieldId = null);

        void AddNumberOption(IManifest mod, Func<float> getValue, Action<float> setValue, Func<string> name, Func<string> tooltip = null, float? min = null, float? max = null, float? interval = null, Func<float, string> formatValue = null, string fieldId = null);

        void AddTextOption(IManifest mod, Func<string> getValue, Action<string> setValue, Func<string> name, Func<string> tooltip = null, string[] allowedValues = null, Func<string, string> formatAllowedValue = null, string fieldId = null);

        void AddKeybindList(IManifest mod, Func<KeybindList> getValue, Action<KeybindList> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);
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

        public bool TapperSapHasQuality { get; set; } = false;

        public int TapperXPAmount { get; set; } = 3;

        public int SmallStumpBonusXPAmount { get; set; } = 1;

        public int TwigDebrisXPAmount { get; set; } = 1;

        public bool TapperDaysNeededChangesEnabled { get; set; } = true;

        public int MapleTapperDaysNeeded { get; set; } = 7;

        public int OakTapperDaysNeeded { get; set; } = 7;

        public int PineTapperDaysNeeded { get; set; } = 7;

        public int EqualizedPricePerDayForMapleOakPineTapperProduct { get; set; } = 0;

        public int MysticTapperDaysNeeded { get; set; } = 7;

        public int MahoganyTapperDaysNeeded { get; set; } = 7;

        public bool MushroomTreeTappersConsistentDaysNeeded { get; set; } = true;

        public bool CommonFiddleheadFern { get; set; } = true;

        public bool WildAndFineGrapes { get; set; } = true;

        public bool ForageSurvivalBurger { get; set; } = true;

        public KeybindList TreeMenuKey { get; set; } = TreeMenuKeyDefault;

        public bool MushroomTapperCalendar { get; set; } = false;

        public bool HazelnutSeasonCalendarReminder { get; set; } = false;

        private static string[] TapperQualityOptionsChoices { get; set; } = new string[] { "Disabled", "ForageLevelBased", "ForageLevelBasedNoBotanist", "TreeAgeBasedMonths", "TreeAgeBasedYears" };

        private static string[] EqualizedPricePerDayChoices { get; set; } = new string[] { "Disabled", "BasedOnMaple", "BasedOnOak", "BasedOnPine" };

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

            // this can be disabled by turning it to 0, for the 'oak, maple, pine' that logic would be too complex for something noone uses
            if (config.MysticTapperDaysNeeded < 0)
            {
                invalidConfig = true;
                config.MysticTapperDaysNeeded = 0;
            }

            // this can be disabled by turning it to 0, for 'oak, maple and pine' that logic would be too complex for something noone uses
            if (config.MahoganyTapperDaysNeeded < 0)
            {
                invalidConfig = true;
                config.MahoganyTapperDaysNeeded = 0;
            }

            if (config.TapperQualityOptions < 0 || config.TapperQualityOptions > 4)
            {
                invalidConfig = true;
                config.TapperQualityOptions = 0;
            }

            if (config.EqualizedPricePerDayForMapleOakPineTapperProduct < 0 || config.EqualizedPricePerDayForMapleOakPineTapperProduct > 3)
            {
                invalidConfig = true;
                config.EqualizedPricePerDayForMapleOakPineTapperProduct = 0;
            }

            if (config.TapperXPAmount < 0)
            {
                invalidConfig = true;
                config.TapperXPAmount = 0;
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
                // Common Fiddlehead Fern, Forage Survival Burger
                mod.Helper.GameContent.InvalidateCacheAndLocalized("Data/CraftingRecipes");

                // Common Fiddlehead Fern
                mod.Helper.GameContent.InvalidateCacheAndLocalized("Data/Locations");
                mod.Helper.GameContent.InvalidateCacheAndLocalized("Data/Bundles");

                // Forage Survival Burger
                mod.Helper.GameContent.InvalidateCacheAndLocalized("Data/CookingRecipes");

                // Tapper XP, Fine Grapes
                mod.Helper.GameContent.InvalidateCacheAndLocalized("Data/Machines");

                // needs to be done before 'Data/Objects' is invalidated, so the tapper prices are correct
                // Tapper days needed changes, Tree Menu
                mod.Helper.GameContent.InvalidateCacheAndLocalized("Data/WildTrees");

                // Tapper days needed changes, Fine Grapes, Common Fiddlehead Fern, Forage Survival Burger
                mod.Helper.GameContent.InvalidateCacheAndLocalized("Data/Objects");

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
                    config = new ForageFantasyConfig();
                    InvalidateCache(mod);
                },
                save: delegate
                {
                    mod.Helper.WriteConfig(config);
                    VerifyConfigValues(config, mod);
                    InvalidateCache(mod);
                }
            );

            api.AddSectionTitle(manifest, GetConfigName(mod, "SectionQualityTweaks"));

            api.AddBoolOption(manifest, () => config.BerryBushQuality, (bool val) => config.BerryBushQuality = val,
                GetConfigName(mod, "BerryBushQuality"), GetConfigDescription(mod, "BerryBushQuality"));
            api.AddBoolOption(manifest, () => config.MushroomBoxQuality, (bool val) => config.MushroomBoxQuality = val,
                GetConfigName(mod, "MushroomBoxQuality"), GetConfigDescription(mod, "MushroomBoxQuality"));
            api.AddTextOption(manifest, () => GetElementFromConfig(TapperQualityOptionsChoices, config.TapperQualityOptions), (string val) => config.TapperQualityOptions = GetIndexFromArrayElement(TapperQualityOptionsChoices, val),
                GetConfigName(mod, "TapperQualityOptions"), GetConfigDescription(mod, "TapperQualityOptions"), TapperQualityOptionsChoices, GetConfigDropdownChoice(mod, "TapperQualityOptions"));
            api.AddBoolOption(manifest, () => config.TapperQualityRequiresTapperPerk, (bool val) => config.TapperQualityRequiresTapperPerk = val,
                GetConfigName(mod, "TapperQualityRequiresTapperPerk"), GetConfigDescription(mod, "TapperQualityRequiresTapperPerk"));
            api.AddBoolOption(manifest, () => config.TapperSapHasQuality, (bool val) => config.TapperSapHasQuality = val,
                GetConfigName(mod, "TapperSapHasQuality"), GetConfigDescription(mod, "TapperSapHasQuality"));

            api.AddSectionTitle(manifest, GetConfigName(mod, "SectionXPRewards"));

            api.AddParagraph(manifest, GetConfigName(mod, "MushroomBoxBerryBushXPUpdate"));

            api.AddNumberOption(manifest, () => config.TapperXPAmount, (int val) => config.TapperXPAmount = val,
                GetConfigName(mod, "TapperXPAmount"), GetConfigDescription(mod, "TapperXPAmount"), 0);
            api.AddNumberOption(manifest, () => config.SmallStumpBonusXPAmount, (int val) => config.SmallStumpBonusXPAmount = val,
                GetConfigName(mod, "SmallStumpBonusXPAmount"), GetConfigDescription(mod, "SmallStumpBonusXPAmount"), 0);
            api.AddNumberOption(manifest, () => config.TwigDebrisXPAmount, (int val) => config.TwigDebrisXPAmount = val,
                GetConfigName(mod, "TwigDebrisXPAmount"), GetConfigDescription(mod, "TwigDebrisXPAmount"), 0);

            api.AddSectionTitle(manifest, GetConfigName(mod, "SectionTapperDaysNeededChanges"));

            api.AddBoolOption(manifest, () => config.TapperDaysNeededChangesEnabled, (bool val) => config.TapperDaysNeededChangesEnabled = val,
                GetConfigName(mod, "TapperDaysNeededChangesEnabled"), GetConfigDescription(mod, "TapperDaysNeededChangesEnabled"));
            api.AddNumberOption(manifest, () => config.MapleTapperDaysNeeded, (int val) => config.MapleTapperDaysNeeded = val,
                GetConfigName(mod, "MapleDaysNeeded"), GetConfigDescription(mod, "MapleDaysNeeded"), 1);
            api.AddNumberOption(manifest, () => config.OakTapperDaysNeeded, (int val) => config.OakTapperDaysNeeded = val,
                GetConfigName(mod, "OakDaysNeeded"), GetConfigDescription(mod, "OakDaysNeeded"), 1);
            api.AddNumberOption(manifest, () => config.PineTapperDaysNeeded, (int val) => config.PineTapperDaysNeeded = val,
                GetConfigName(mod, "PineDaysNeeded"), GetConfigDescription(mod, "PineDaysNeeded"), 1);

            api.AddTextOption(manifest, () => GetElementFromConfig(EqualizedPricePerDayChoices, config.EqualizedPricePerDayForMapleOakPineTapperProduct), (string val) => config.EqualizedPricePerDayForMapleOakPineTapperProduct = GetIndexFromArrayElement(EqualizedPricePerDayChoices, val),
                GetConfigName(mod, "EqualizedPricePerDayForMapleOakPineTapperProduct"), GetConfigDescription(mod, "EqualizedPricePerDayForMapleOakPineTapperProduct"), EqualizedPricePerDayChoices, GetConfigDropdownChoice(mod, "EqualizedPricePerDayOptions"));

            api.AddNumberOption(manifest, () => config.MysticTapperDaysNeeded, (int val) => config.MysticTapperDaysNeeded = val,
                GetConfigName(mod, "MysticDaysNeeded"), GetConfigDescription(mod, "MysticDaysNeeded"), 0);
            api.AddNumberOption(manifest, () => config.MahoganyTapperDaysNeeded, (int val) => config.MahoganyTapperDaysNeeded = val,
                GetConfigName(mod, "MahoganyDaysNeeded"), GetConfigDescription(mod, "MahoganyDaysNeeded"), 0);
            api.AddBoolOption(manifest, () => config.MushroomTreeTappersConsistentDaysNeeded, (bool val) => config.MushroomTreeTappersConsistentDaysNeeded = val,
                GetConfigName(mod, "MushroomTreeTappersConsistentDaysNeeded"), GetConfigDescription(mod, "MushroomTreeTappersConsistentDaysNeeded"));

            api.AddSectionTitle(manifest, GetConfigName(mod, "SectionOtherFeatures"));

            api.AddBoolOption(manifest, () => config.CommonFiddleheadFern, (bool val) => config.CommonFiddleheadFern = val,
                GetConfigName(mod, "CommonFiddleheadFern"), GetConfigDescription(mod, "CommonFiddleheadFern"));
            api.AddBoolOption(manifest, () => config.ForageSurvivalBurger, (bool val) => config.ForageSurvivalBurger = val,
                GetConfigName(mod, "ForageSurvivalBurger"), GetConfigDescription(mod, "ForageSurvivalBurger"));
            api.AddBoolOption(manifest, () => config.WildAndFineGrapes, (bool val) => config.WildAndFineGrapes = val,
                GetConfigName(mod, "WildAndFineGrapes"), GetConfigDescription(mod, "WildAndFineGrapes"));

            api.AddBoolOption(manifest, () => config.HazelnutSeasonCalendarReminder, (bool val) => config.HazelnutSeasonCalendarReminder = val,
                GetConfigName(mod, "HazelnutSeasonCalendarReminder"), GetConfigDescription(mod, "HazelnutSeasonCalendarReminder"));
            api.AddBoolOption(manifest, () => config.MushroomTapperCalendar, (bool val) => config.MushroomTapperCalendar = val,
                GetConfigName(mod, "MushroomTapperCalendar"), GetConfigDescription(mod, "MushroomTapperCalendar"));

            api.AddKeybindList(manifest, () => config.TreeMenuKey, (KeybindList keybindList) => config.TreeMenuKey = keybindList,
                GetConfigName(mod, "TreeMenuKey"), GetConfigDescription(mod, "TreeMenuKey"));
        }

        private static Func<string, string> GetConfigDropdownChoice(ForageFantasy mod, string key)
        {
            return (choice) => mod.Helper.Translation.Get($"Config{key}{choice}");
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