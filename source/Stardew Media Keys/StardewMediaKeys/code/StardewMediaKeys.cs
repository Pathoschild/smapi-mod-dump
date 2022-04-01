/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AngeloC3/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace StardewMediaKeys
{
    /// <summary>The mod entry point.</summary>
    public class StardewMediaKeys : Mod
    {
        /// <summary>An instance of the mod's config.</summary>
        private ModConfig Config;
        /// <summary>Used to pass the helper into the menu</summary>
        private IModHelper helper;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.helper = helper;
            this.Config = this.Helper.ReadConfig<ModConfig>();
            if (!this.Config.OnlyUsableWithPhone)
                this.helper.Events.Input.ButtonsChanged += this.OnButtonsChanged;
            this.Helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
        {
            // ignored if a menu that isnt SMKMenu is opened, or the world has not initialized yet
            if ( (Game1.activeClickableMenu != null && !(Game1.activeClickableMenu is SMKMenu))
                 //(!Context.IsPlayerFree) || // Disabed so it can be opened during a cutscene to turn off music
                 ||  !Context.IsWorldReady) return;

           // opens menu if the configurable key is pressed
            if (this.Config.ToggleKey.JustPressed())
            {
                if (Game1.activeClickableMenu is SMKMenu)
                {
                    SMKMenu menu = (SMKMenu)Game1.activeClickableMenu;
                    menu.exitThisMenu();
                    return;
                }
                Game1.activeClickableMenu = new SMKMenu(this.helper, this.Config);
            }
        }

        /* code used from 
         *      https://github.com/spacechase0/StardewValleyMods/tree/develop/GenericModConfigMenu#for-c-mod-authors
         *      https://www.nexusmods.com/stardewvalley/articles/467
         */
        /// <summary>Sets up the mod config menu on launch</summary>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Generic Mod Config Menu stuff here
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.Config)
            );

            // add boolean option
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Blue title instead of default",
                tooltip: () => "The title in the pop up menu will be blue if true and default text color if false",
                getValue: () => this.Config.BlueNotDefaultTitle,
                setValue: value => this.Config.BlueNotDefaultTitle = value
            );

            // add KeyBindList option
            configMenu.AddKeybindList(
                mod: this.ModManifest,
                name: () => "Menu Keybind",
                tooltip: () => "The keybind to click to open up the menu",
                getValue: () => this.Config.ToggleKey,
                setValue: value => this.Config.ToggleKey = value
            );

            // add boolean option
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Only usable with Mobile Phone",
                tooltip: () => "If this is true, the keybind hotkey is disabled, making it only openable via the Mobile Phone mod. Essentially, this just disables the keybind activation. Restart your game for this to take effect.",
                getValue: () => this.Config.OnlyUsableWithPhone,
                setValue: value => this.Config.OnlyUsableWithPhone = value
            );

            // Mobile Phone app stuff here
            IMobilePhoneApi api = Helper.ModRegistry.GetApi<IMobilePhoneApi>("aedenthorn.MobilePhone");
            if (api != null)
            {
                void setSMKAsMenu()
                {
                    api.SetAppRunning(true);
                    Game1.activeClickableMenu = new SMKMenu(this.helper, this.Config, true);
                }

                Texture2D appIcon = Helper.Content.Load<Texture2D>(System.IO.Path.Combine("assets", "SMKApp.png"));
                bool success = api.AddApp(Helper.ModRegistry.ModID, "Stardew Media Keys", setSMKAsMenu, appIcon);
                Monitor.Log($"loaded phone app successfully: {success}", LogLevel.Debug);
            }

        }

    }
}
