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
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using StardewModdingAPI;
    using StardewModdingAPI.Utilities;
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

        public int MushroomXPAmount { get; set; } = 1;

        public int TapperXPAmount { get; set; } = 3;

        public bool AutomationHarvestsGrantXP { get; set; } = false;

        public bool TapperDaysNeededChangesEnabled { get; set; } = true;

        public int MapleDaysNeeded { get; set; } = 9;

        public int OakDaysNeeded { get; set; } = 7;

        public int PineDaysNeeded { get; set; } = 5;

        public bool MushroomTreeHeavyTappersFix { get; set; } = true;

        public bool MushroomTreeTappersConsistencyChange { get; set; } = true;

        public bool MushroomTreeSeedsDrop { get; set; } = false;

        public bool CommonFiddleheadFern { get; set; } = true;

        public bool ForageSurvivalBurger { get; set; } = true;

        public KeybindList TreeMenuKey { get; set; } = TreeMenuKeyDefault;

        public bool MushroomTapperCalendar { get; set; } = false;

        private static string[] TQChoices { get; set; } = new string[] { "Disabled", "Forage Level Based", "Forage Level Based (No Botanist)", "Tree Age Based (Months)", "Tree Age Based (Years)" };

        public static void VerifyConfigValues(ForageFantasyConfig config, ForageFantasy mod)
        {
            bool invalidConfig = false;

            if (config.MapleDaysNeeded <= 0)
            {
                invalidConfig = true;
                config.MapleDaysNeeded = 1;
            }

            if (config.PineDaysNeeded <= 0)
            {
                invalidConfig = true;
                config.PineDaysNeeded = 1;
            }

            if (config.OakDaysNeeded <= 0)
            {
                invalidConfig = true;
                config.OakDaysNeeded = 1;
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

            if (config.MushroomXPAmount < 0)
            {
                invalidConfig = true;
                config.MushroomXPAmount = 0;
            }

            if (mod.Helper.ModRegistry.IsLoaded("thelion.AwesomeProfessions"))
            {
                if (config.MushroomBoxQuality || config.BerryBushQuality)
                {
                    invalidConfig = true;

                    config.MushroomBoxQuality = false;
                    config.BerryBushQuality = false;

                    mod.DebugLog("Enabled Walk of Life compatibility.");
                }
            }

            try
            {
                // CommonFiddleheadFern and ForageSurvivalBurger
                mod.Helper.Content.InvalidateCache("Data/CraftingRecipes");

                // CommonFiddleheadFern
                mod.Helper.Content.InvalidateCache("Data/Locations");

                // ForageSurvivalBurger
                mod.Helper.Content.InvalidateCache("Data/CookingRecipes");

                // Tapper days needed changes
                mod.Helper.Content.InvalidateCache("Data/ObjectInformation");
            }
            catch (Exception e)
            {
                mod.DebugLog($"Exception when trying to invalidate cache on config change {e}");
            }

            if (invalidConfig)
            {
                mod.DebugLog("At least one config value was out of range and was reset.");
                mod.Helper.WriteConfig(config);
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

            api.RegisterModConfig(
                manifest,
                delegate
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
                },
                delegate
                {
                    mod.Helper.WriteConfig(config);
                    VerifyConfigValues(config, mod);
                }
            );

            api.SetTitleScreenOnlyForNextOptions(manifest, true);

            api.RegisterLabel(manifest, "Quality Tweaks", null);

            if (mod.Helper.ModRegistry.IsLoaded("thelion.AwesomeProfessions"))
            {
                api.RegisterLabel(manifest, "Berry Bush Quality Disabled (Walk Of Life)", null);
                api.RegisterLabel(manifest, "Mushroom Box Quality Disabled (Walk Of Life)", null);
            }
            else
            {
                api.RegisterSimpleOption(manifest, "Berry Bush Quality", "Salmonberries and blackberries have quality based\non forage level even without botanist perk.", () => config.BerryBushQuality, (bool val) => config.BerryBushQuality = val);
                api.RegisterSimpleOption(manifest, "Mushroom Box Quality", "Mushrooms have quality based on forage level and botanist perk.", () => config.MushroomBoxQuality, (bool val) => config.MushroomBoxQuality = val);
            }

            api.RegisterChoiceOption(manifest, "Tapper Quality Options", null, () => GetElementFromConfig(TQChoices, config.TapperQualityOptions), (string val) => config.TapperQualityOptions = GetIndexFromArrayElement(TQChoices, val), TQChoices);
            api.RegisterSimpleOption(manifest, "Tapper Perk Is Required", null, () => config.TapperQualityRequiresTapperPerk, (bool val) => config.TapperQualityRequiresTapperPerk = val);

            api.RegisterLabel(manifest, "XP Rewards", null);

            api.RegisterClampedOption(manifest, "Berry Bush Chance To Get XP", "Chance to get foraging experience when harvesting bushes.\nSet to 0 to disable feature.", () => config.BerryBushChanceToGetXP, (int val) => config.BerryBushChanceToGetXP = val, 0, 100);
            api.RegisterSimpleOption(manifest, "Berry Bush XP Amount", "Amount of XP gained per bush. For reference:\nChopping down a tree is 12XP, a foraging good is 7XP\nNegative values will be reset to 0.", () => config.BerryBushXPAmount, (int val) => config.BerryBushXPAmount = val);
            api.RegisterSimpleOption(manifest, "Mushroom Box XP Amount", "For reference:\nChopping down a tree is 12XP, a foraging good is 7XP\nNegative values will be reset to 0.", () => config.MushroomXPAmount, (int val) => config.MushroomXPAmount = val);
            api.RegisterSimpleOption(manifest, "Tapper XP Amount", "For reference:\nChopping down a tree is 12XP, a foraging good is 7XP\nNegative values will be reset to 0.", () => config.TapperXPAmount, (int val) => config.TapperXPAmount = val);
            api.RegisterSimpleOption(manifest, "Automation Harvests Grant XP", "Whether automatic harvests with the Automate, Deluxe\nGrabber Redux or One Click Shed Reloader should grant XP.\nKeep in mind that some of those only affect the host.", () => config.AutomationHarvestsGrantXP, (bool val) => config.AutomationHarvestsGrantXP = val);

            api.RegisterLabel(manifest, "Tapper Days Needed Changes", null);

            api.RegisterSimpleOption(manifest, "Days Needed Changes Enabled", "If this is disabled, then all features\nin this category don't do anything", () => config.TapperDaysNeededChangesEnabled, (bool val) => config.TapperDaysNeededChangesEnabled = val);
            api.RegisterSimpleOption(manifest, "Maple Tree Days Needed", "default: 9 days, recommended: 7 days", () => config.MapleDaysNeeded, (int val) => config.MapleDaysNeeded = val);
            api.RegisterSimpleOption(manifest, "Oak Tree Days Needed", "default: 7 days, recommended: 7 days", () => config.OakDaysNeeded, (int val) => config.OakDaysNeeded = val);
            api.RegisterSimpleOption(manifest, "Pine Tree Days Needed", "default: 5 days, recommended: 7 days", () => config.PineDaysNeeded, (int val) => config.PineDaysNeeded = val);
            api.RegisterSimpleOption(manifest, "Mushroom Tree Heavy Tapper Fix", null, () => config.MushroomTreeHeavyTappersFix, (bool val) => config.MushroomTreeHeavyTappersFix = val);
            api.RegisterSimpleOption(manifest, "Mushroom Tree Tapper\nConsistency Change", null, () => config.MushroomTreeTappersConsistencyChange, (bool val) => config.MushroomTreeTappersConsistencyChange = val);

            api.RegisterLabel(manifest, "Other Features", null);

            api.RegisterSimpleOption(manifest, "Mushroom Tree Seeds Drop", null, () => config.MushroomTreeSeedsDrop, (bool val) => config.MushroomTreeSeedsDrop = val);
            api.RegisterSimpleOption(manifest, "Common Fiddlehead Fern", "Fiddlehead fern is available outside of the secret forest\nand added to the wild seeds pack and summer foraging bundle.", () => config.CommonFiddleheadFern, (bool val) => config.CommonFiddleheadFern = val);
            api.RegisterSimpleOption(manifest, "Forage Survival Burger", "Forage based early game crafting recipes\nand even more efficient cooking recipes.", () => config.ForageSurvivalBurger, (bool val) => config.ForageSurvivalBurger = val);

            api.SetTitleScreenOnlyForNextOptions(manifest, false);

            api.RegisterSimpleOption(manifest, "Mushroom Tapper Calendar", null, () => config.MushroomTapperCalendar, (bool val) => config.MushroomTapperCalendar = val);

            api.AddKeybindList(manifest, () => config.TreeMenuKey, (KeybindList keybindList) => config.TreeMenuKey = keybindList, () => "Tree Menu Key");

            api.SetTitleScreenOnlyForNextOptions(manifest, true);

            if (GrapeLogic.AreGrapeJsonModsInstalled(mod))
            {
                api.RegisterLabel(manifest, "Fine Grapes Feature Installed And Enabled", "Remove the Json Assets mod pack to disable this option");
            }
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