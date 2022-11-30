/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System.IO;

namespace AllChestsMenu
{
    /// <summary>The mod entry point.</summary>
    public partial class ModEntry : Mod
    {

        public static IMonitor SMonitor;
        public static IModHelper SHelper;
        public static ModConfig Config;

        public static ModEntry context;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();

            if (!Config.ModEnabled)
                return;

            context = this;

            SMonitor = Monitor;
            SHelper = helper;

            Helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
            Helper.Events.Input.ButtonPressed += Input_ButtonPressed;

            var harmony = new Harmony(ModManifest.UniqueID);
            harmony.PatchAll();
        }

        public void Input_ButtonPressed(object sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            if (!Config.ModEnabled)
                return;
            if (Game1.activeClickableMenu is StorageMenu)
            {
                if (Game1.options.snappyMenus && Game1.options.gamepadControls && e.Button == Config.SwitchButton)
                {
                    Game1.playSound("shwip");
                    if (!(Game1.activeClickableMenu as StorageMenu).focusBottom)
                        (Game1.activeClickableMenu as StorageMenu).lastTopSnappedCC = Game1.activeClickableMenu.currentlySnappedComponent;
                    (Game1.activeClickableMenu as StorageMenu).focusBottom = !(Game1.activeClickableMenu as StorageMenu).focusBottom;
                    Game1.activeClickableMenu.currentlySnappedComponent = null;
                    Game1.activeClickableMenu.snapToDefaultClickableComponent();
                }
                if (((Game1.activeClickableMenu as StorageMenu).locationText.Selected || (Game1.activeClickableMenu as StorageMenu).renameBox.Selected) && e.Button.ToString().Length == 1)
                {
                    SHelper.Input.Suppress(e.Button);
                }
            }
            if (e.Button == Config.MenuKey && (Config.ModKey == SButton.None || !Config.ModToOpen || Helper.Input.IsDown(Config.ModKey)))
            {
                OpenMenu();
            }
        }

        public void GameLoop_GameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            var phoneAPI = Helper.ModRegistry.GetApi<IMobilePhoneApi>("aedenthorn.MobilePhone");
            if (phoneAPI != null)
            {
                phoneAPI.AddApp("aedenthorn.AllChestsMenu", "Mailbox", OpenMenu, Helper.ModContent.Load<Texture2D>(Path.Combine("assets", "icon.png")));
            }
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Mod Enabled",
                getValue: () => Config.ModEnabled,
                setValue: value => Config.ModEnabled = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Current Loc Only",
                getValue: () => Config.LimitToCurrentLocation,
                setValue: value => Config.LimitToCurrentLocation = value
            );
            configMenu.AddKeybind(
                mod: ModManifest,
                name: () => "Menu Key",
                getValue: () => Config.MenuKey,
                setValue: value => Config.MenuKey = value
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Req. Mod Key To Open",
                getValue: () => Config.ModToOpen,
                setValue: value => Config.ModToOpen = value
            );
            configMenu.AddKeybind(
                mod: ModManifest,
                name: () => "Mod Key",
                tooltip: () => "Hold down to open menu and to transfer contents instead of swapping menus",
                getValue: () => Config.ModKey,
                setValue: value => Config.ModKey = value
            );
            configMenu.AddKeybind(
                mod: ModManifest,
                name: () => "Mod Key 2",
                tooltip: () => "Hold down to transfer only same items when transfering all",
                getValue: () => Config.SwitchButton,
                setValue: value => Config.SwitchButton = value
            );
            configMenu.AddKeybind(
                mod: ModManifest,
                name: () => "Switch Button",
                tooltip: () => "For controllers to switch between upper and lower interfaces",
                getValue: () => Config.SwitchButton,
                setValue: value => Config.ModKey2 = value
            );
        }

    }
}