/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Gaphodil/GlobalConfigSettings
**
*************************************************/

using System.Collections.Generic;
using System.Reflection;
using GenericModConfigMenu;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace GlobalConfigSettings
{
    public class ModEntry : Mod
    {
        private ModConfig config;

        private bool afterCreate = false;

        /*********
        ** Public methods
        *********/

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.SaveCreated += OnSaveCreated;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        /*********
        ** Private methods
        *********/

        /// <summary>
        /// Raised after the game creates the save file.
        /// </summary>
        private void OnSaveCreated(object sender, SaveCreatedEventArgs e)
        {
            afterCreate = true;
        }

        /// <summary>
        /// Raised after loading a save (including after creating a save).
        /// </summary>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (afterCreate)
            {
                afterCreate = false;
                Monitor.Log("Loading global settings after created farm!");
                WriteConfigToOptions();
            }
            else
            {
                if (config.ChangeOnEveryLoad)
                {
                    Monitor.Log("Loading global settings after loaded save!");
                    WriteConfigToOptions();
                }
            }
        }

        // from https://github.com/spacechase0/StardewValleyMods/tree/develop/GenericModConfigMenu#readme
        private void OnGameLaunched(object Sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu API (if it's installed)
            var api = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (api is null)
                return;

            // register mod configuration
            api.RegisterModConfig(
                mod: ModManifest,
                revertToDefault: () => config = new ModConfig(),
                saveToFile: () => Helper.WriteConfig(config)
            );

            // do not let players configure your mod in-game (instead of just from the title screen)
            api.SetDefaultIngameOptinValue(ModManifest, false);

            // add some config options
            api.RegisterSimpleOption(
                mod: ModManifest,
                optionName: Helper.Translation.Get("GlobalConfigSettings:ChangeOnEveryLoad"),
                optionDesc: Helper.Translation.Get("GlobalConfigSettings:ChangeOnEveryLoadDescription"),
                optionGet: () => config.ChangeOnEveryLoad,
                optionSet: value => config.ChangeOnEveryLoad = value
            );

            string[] choices;
            // General
            {
                api.RegisterLabel(
                    mod: ModManifest,
                    labelName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11233"),
                    labelDesc: ""
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11234"),
                    optionDesc: "",
                    optionGet: () => config.AutoRun,
                    optionSet: value => config.AutoRun = value
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11235"),
                    optionDesc: "",
                    optionGet: () => config.ShowPortraits,
                    optionSet: value => config.ShowPortraits = value
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11236"),
                    optionDesc: "",
                    optionGet: () => config.ShowMerchantPortraits,
                    optionSet: value => config.ShowMerchantPortraits = value
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11237"),
                    optionDesc: "",
                    optionGet: () => config.AlwaysShowToolLocation,
                    optionSet: value => config.AlwaysShowToolLocation = value
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11238"),
                    optionDesc: "",
                    optionGet: () => config.HideToolHitLocationWhenMoving,
                    optionSet: value => config.HideToolHitLocationWhenMoving = value
                );

                choices = new string[3] { "auto", "force_on", "force_off" };
                api.RegisterChoiceOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\UI:Options_GamepadMode"),
                    optionDesc: "",
                    optionGet: () => config.GamepadMode,
                    optionSet: value => config.GamepadMode = value,
                    choices: choices
                );
                choices = new string[3] { "off", "gamepad", "both" };
                api.RegisterChoiceOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\UI:Options_StowingMode"),
                    optionDesc: "",
                    optionGet: () => config.ItemStowing,
                    optionSet: value => config.ItemStowing = value,
                    choices: choices
                );
                choices = new string[2] { "hold", "legacy" };
                api.RegisterChoiceOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\UI:Options_SlingshotMode"),
                    optionDesc: "",
                    optionGet: () => config.SlingshotFireMode,
                    optionSet: value => config.SlingshotFireMode = value,
                    choices: choices
                );

                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11239"),
                    optionDesc: "",
                    optionGet: () => config.ControllerPlacementTileIndicator,
                    optionSet: value => config.ControllerPlacementTileIndicator = value
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11240"),
                    optionDesc: "",
                    optionGet: () => config.PauseWhenGameWindowIsInactive,
                    optionSet: value => config.PauseWhenGameWindowIsInactive = value
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\UI:Options_GamepadStyleMenus"),
                    optionDesc: "",
                    optionGet: () => config.UseControllerStyleMenus,
                    optionSet: value => config.UseControllerStyleMenus = value
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\UI:Options_ShowAdvancedCraftingInformation"),
                    optionDesc: "",
                    optionGet: () => config.ShowAdvancedCraftingInformation,
                    optionSet: value => config.ShowAdvancedCraftingInformation = value
                );
            }

            // Sound
            {
                api.RegisterLabel(
                    mod: ModManifest,
                    labelName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11241"),
                    labelDesc: ""
                );
                api.RegisterClampedOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11242"),
                    optionDesc: "",
                    optionGet: () => config.MusicVolume,
                    optionSet: value => config.MusicVolume = value,
                    min: 0,
                    max: 100
                );
                api.RegisterClampedOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11243"),
                    optionDesc: "",
                    optionGet: () => config.SoundVolume,
                    optionSet: value => config.SoundVolume = value,
                    min: 0,
                    max: 100
                );
                api.RegisterClampedOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11244"),
                    optionDesc: "",
                    optionGet: () => config.AmbientVolume,
                    optionSet: value => config.AmbientVolume = value,
                    min: 0,
                    max: 100
                );
                api.RegisterClampedOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11245"),
                    optionDesc: "",
                    optionGet: () => config.FootstepVolume,
                    optionSet: value => config.FootstepVolume = value,
                    min: 0,
                    max: 100
                );

                choices = new string[5] { "-1", "0", "1", "2", "3" };
                api.RegisterChoiceOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:BiteChime"),
                    optionDesc: "",
                    optionGet: () => config.FishingBiteSound,
                    optionSet: value => config.FishingBiteSound = value,
                    choices: choices
                );

                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11246"),
                    optionDesc: "",
                    optionGet: () => config.DialogueTypingSound,
                    optionSet: value => config.DialogueTypingSound = value
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:Options_ToggleAnimalSounds"),
                    optionDesc: "",
                    optionGet: () => config.MuteAnimalSounds,
                    optionSet: value => config.MuteAnimalSounds = value
                );
            }

            // Graphics
            {
                api.RegisterLabel(
                    mod: ModManifest,
                    labelName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11247"),
                    labelDesc: ""
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\UI:Options_Vsync"),
                    optionDesc: "",
                    optionGet: () => config.VSync,
                    optionSet: value => config.VSync = value
                );

                // like most of this: from OptionsPage.cs
                List<string> zoom_options = new List<string>();
                for (int zoom2 = 75; zoom2 <= 150; zoom2 += 5)
                {
                    zoom_options.Add(zoom2 + "%");
                }
                api.RegisterChoiceOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage_UIScale"),
                    optionDesc: "",
                    optionGet: () => config.UiScale,
                    optionSet: value => config.UiScale = value,
                    choices: zoom_options.ToArray()
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11252"),
                    optionDesc: "",
                    optionGet: () => config.MenuBackgrounds,
                    optionSet: value => config.MenuBackgrounds = value
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11253"),
                    optionDesc: "",
                    optionGet: () => config.LockToolbar,
                    optionSet: value => config.LockToolbar = value
                );

                zoom_options = new List<string>();
                for (int zoom = 75; zoom <= 200; zoom += 5)
                {
                    zoom_options.Add(zoom + "%");
                }
                api.RegisterChoiceOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11254"),
                    optionDesc: "",
                    optionGet: () => config.ZoomLevel,
                    optionSet: value => config.ZoomLevel = value,
                    choices: zoom_options.ToArray()
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11266"),
                    optionDesc: "",
                    optionGet: () => config.ZoomButtons,
                    optionSet: value => config.ZoomButtons = value
                );

                choices = new string[3] { "Low", "Med.", "High" };
                api.RegisterChoiceOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11267"),
                    optionDesc: "",
                    optionGet: () => config.LightingQuality,
                    optionSet: value => config.LightingQuality = value,
                    choices: choices
                );
                api.RegisterClampedOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11271"),
                    optionDesc: "",
                    optionGet: () => config.SnowTransparency,
                    optionSet: value => config.SnowTransparency = value,
                    min: 0,
                    max: 100
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11272"),
                    optionDesc: "",
                    optionGet: () => config.ShowFlashEffects,
                    optionSet: value => config.ShowFlashEffects = value
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11273"),
                    optionDesc: "",
                    optionGet: () => config.UseHardwareCursor,
                    optionSet: value => config.UseHardwareCursor = value
                );
            }

            // Controls
            {
                api.RegisterLabel(
                    mod: ModManifest,
                    labelName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11274"),
                    labelDesc: ""
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11275"),
                    optionDesc: "",
                    optionGet: () => config.ControllerRumble,
                    optionSet: value => config.ControllerRumble = value
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11276"),
                    optionDesc: "",
                    optionGet: () => config.InvertToolbarScrollDirection,
                    optionSet: value => config.InvertToolbarScrollDirection = value
                );

                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11278"),
                    optionDesc: "",
                    optionGet: () => config.CheckDoAction,
                    optionSet: value => config.CheckDoAction = value
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11279"),
                    optionDesc: "",
                    optionGet: () => config.UseTool,
                    optionSet: value => config.UseTool = value
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11280"),
                    optionDesc: "",
                    optionGet: () => config.AccessMenu,
                    optionSet: value => config.AccessMenu = value
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11281"),
                    optionDesc: "",
                    optionGet: () => config.AccessJournal,
                    optionSet: value => config.AccessJournal = value
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11282"),
                    optionDesc: "",
                    optionGet: () => config.AccessMap,
                    optionSet: value => config.AccessMap = value
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11283"),
                    optionDesc: "",
                    optionGet: () => config.MoveUp,
                    optionSet: value => config.MoveUp = value
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11284"),
                    optionDesc: "",
                    optionGet: () => config.MoveLeft,
                    optionSet: value => config.MoveLeft = value
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11285"),
                    optionDesc: "",
                    optionGet: () => config.MoveDown,
                    optionSet: value => config.MoveDown = value
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11286"),
                    optionDesc: "",
                    optionGet: () => config.MoveRight,
                    optionSet: value => config.MoveRight = value
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11287"),
                    optionDesc: "",
                    optionGet: () => config.ChatBox,
                    optionSet: value => config.ChatBox = value
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\UI:Input_EmoteButton"),
                    optionDesc: "",
                    optionGet: () => config.EmoteMenu,
                    optionSet: value => config.EmoteMenu = value
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11288"),
                    optionDesc: "",
                    optionGet: () => config.Run,
                    optionSet: value => config.Run = value
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.toolbarSwap"),
                    optionDesc: "",
                    optionGet: () => config.ShiftToolbar,
                    optionSet: value => config.ShiftToolbar = value
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11289"),
                    optionDesc: "",
                    optionGet: () => config.InventorySlot1,
                    optionSet: value => config.InventorySlot1 = value
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11290"),
                    optionDesc: "",
                    optionGet: () => config.InventorySlot2,
                    optionSet: value => config.InventorySlot2 = value
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11291"),
                    optionDesc: "",
                    optionGet: () => config.InventorySlot3,
                    optionSet: value => config.InventorySlot3 = value
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11292"),
                    optionDesc: "",
                    optionGet: () => config.InventorySlot4,
                    optionSet: value => config.InventorySlot4 = value
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11293"),
                    optionDesc: "",
                    optionGet: () => config.InventorySlot5,
                    optionSet: value => config.InventorySlot5 = value
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11294"),
                    optionDesc: "",
                    optionGet: () => config.InventorySlot6,
                    optionSet: value => config.InventorySlot6 = value
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11295"),
                    optionDesc: "",
                    optionGet: () => config.InventorySlot7,
                    optionSet: value => config.InventorySlot7 = value
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11296"),
                    optionDesc: "",
                    optionGet: () => config.InventorySlot8,
                    optionSet: value => config.InventorySlot8 = value
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11297"),
                    optionDesc: "",
                    optionGet: () => config.InventorySlot9,
                    optionSet: value => config.InventorySlot9 = value
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11298"),
                    optionDesc: "",
                    optionGet: () => config.InventorySlot10,
                    optionSet: value => config.InventorySlot10 = value
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11299"),
                    optionDesc: "",
                    optionGet: () => config.InventorySlot11,
                    optionSet: value => config.InventorySlot11 = value
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11300"),
                    optionDesc: "",
                    optionGet: () => config.InventorySlot12,
                    optionSet: value => config.InventorySlot12 = value
                );
            }
        }

        // i learned reflection to do this and i think hardcoding each setting might've been faster
        private void WriteConfigToOptions()
        {
            GlobalConfigSettingsHelper gcsHelper = new GlobalConfigSettingsHelper();

            PropertyInfo[] properties = config.GetType().GetProperties();
            foreach (var prop in properties)
            {
                if (prop.Name.Equals("ChangeOnEveryLoad"))
                    continue;

                //Monitor.Log(prop.PropertyType.ToString() + " " + prop.Name + " = " + prop.GetValue(config).ToString());

                GlobalConfigSettingsHelper.OptionChangeTypes type;
                try
                {
                    type = gcsHelper.GetChangeType(prop.PropertyType.ToString(), prop.Name);
                }
                catch (System.Exception e)
                {
                    Monitor.Log(e.Message + " for " + prop.Name, LogLevel.Error);
                    continue;
                }

                switch (type)
                {
                    case GlobalConfigSettingsHelper.OptionChangeTypes.CheckBox:
                        Game1.options.changeCheckBoxOption(
                            gcsHelper.OptionToId[prop.Name],
                            (bool)prop.GetValue(config)
                        );
                        break;

                    case GlobalConfigSettingsHelper.OptionChangeTypes.DropDown:
                        Game1.options.changeDropDownOption(
                            gcsHelper.OptionToId[prop.Name],
                            (string)prop.GetValue(config)
                        );
                        break;

                    case GlobalConfigSettingsHelper.OptionChangeTypes.InputListener:
                        SButton button = (SButton)prop.GetValue(config);
                        button.TryGetKeyboard(out Keys key);
                        Game1.options.changeInputListenerValue(
                            gcsHelper.KeyToId[prop.Name],
                            key
                        );
                        break;

                    case GlobalConfigSettingsHelper.OptionChangeTypes.Slider:
                        Game1.options.changeSliderOption(
                            gcsHelper.OptionToId[prop.Name],
                            (int)prop.GetValue(config)
                        );
                        break;

                    default:
                        Monitor.Log("invalid type for " + prop.Name, LogLevel.Error);
                        break;
                }
            }
        }
    }
}