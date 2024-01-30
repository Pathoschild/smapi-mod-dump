/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hikari-BS/StardewMods
**
*************************************************/

using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace EmptyYourHands
{
    /// <summary>The mod entry point.</summary>
    internal class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration from the player.</summary>
        ModConfig Config;

        private int oldToolIndex;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += GameLaunched;
            helper.Events.Input.ButtonsChanged += ButtonsChanged;
            helper.Events.Input.ButtonPressed += ButtonPressed;
            helper.Events.Input.MouseWheelScrolled += MouseWheelScrolled;
        }

        private void MouseWheelScrolled(object sender, MouseWheelScrolledEventArgs e)
        {
            if (Context.IsWorldReady && Config.EnableReselectOldToolPosition)
                ResetToolIndex((e.Delta > 0) ? (-1) : 1);
        }

        private void ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (Context.IsWorldReady && Config.EnableReselectOldToolPosition)
            {
                if (e.Button != SButton.LeftTrigger && e.Button != SButton.RightTrigger) return;
                
                ResetToolIndex(e.IsDown(SButton.LeftTrigger) ? (-1) : 1);
            }
        }

        private void ButtonsChanged(object sender, ButtonsChangedEventArgs e)
        {
            if (!Context.IsWorldReady) return;

            if (Config.Keyboard.JustPressed() || Config.Controller.JustPressed())
            {
                if (Game1.player.CurrentToolIndex > 11) return;
                if (!Game1.player.IsBusyDoingSomething()) // prevent game softlock
                {
                    oldToolIndex = Game1.player.CurrentToolIndex;
                    Game1.player.CurrentToolIndex = int.MaxValue;
                }
            }
        }

        private void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null) return;

            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config));

            configMenu.AddKeybindList(
                mod: ModManifest,
                name: () => "Keyboard hotkey(s)",
                tooltip: () => "Key(s) to use when playing using keyboard",
                getValue: () => Config.Keyboard,
                setValue: value => Config.Keyboard = value);

            configMenu.AddKeybindList(
                mod: ModManifest,
                name: () => "Controller hotkey(s)",
                tooltip: () => "Button(s) to use when playing using controller",
                getValue: () => Config.Controller,
                setValue: value => Config.Controller = value);

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Enable reselect old tool position",
                tooltip: () => "Reselect toolbar position to where it was prior emptying hands\n" +
                "when you switch items using mouse scroll wheel or controller triggers.\n\n" +
                "Turn this off and report to me if you experienced weird behaviour.",
                getValue: () => Config.EnableReselectOldToolPosition,
                setValue: value => Config.EnableReselectOldToolPosition = value);
        }

        // This is the part where my brain gets overloaded x.x
        // It works but please kindly tell me if I do this wrong
        private void ResetToolIndex(int whichWay)
        {
            if (Game1.player.CurrentToolIndex < 12) return;
            if (Game1.options.invertScrollDirection) whichWay *= -1;

            switch (whichWay)
            {
                case 1:
                    if (oldToolIndex == 0) // so that it won't assign negative number to the index
                    {
                        Game1.player.CurrentToolIndex = 11;
                        return;
                    }
                    Game1.player.CurrentToolIndex = oldToolIndex - 1;
                    return;
                case -1:
                    Game1.player.CurrentToolIndex = oldToolIndex + 1;
                    return;
                default:
                    return;
            }
        }
    }

    class ModConfig
    {
        public KeybindList Keyboard { get; set; } = KeybindList.Parse("X");
        public KeybindList Controller { get; set; } = KeybindList.Parse("LeftStick");
        public bool EnableReselectOldToolPosition { get; set; } = true;
    }
}
