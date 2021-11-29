/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/GiveMeMyCursorBack
**
*************************************************/

using System;
using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Minigames;

namespace GiveMeMyCursorBack
{
    public class GiveMyMeCursorBackEntry : Mod
    {
        private byte _tickCounter;
        private Config _config;
        
        public override void Entry(IModHelper helper)
        {
            this._config = this.Helper.ReadConfig<Config>();
            
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            
            Monitor.Log("Your cursor is now yours again. Rejoice!", LogLevel.Info);
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            try
            {
                var gmcm = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

                if (gmcm == null)
                    return;

                gmcm.Register(
                    mod: this.ModManifest,
                    reset: () => this._config = new Config(),
                    save: () => Helper.WriteConfig(_config)
                );

                gmcm.AddSectionTitle(
                    mod: ModManifest,
                    text: () => "Get your cursor back!");

                gmcm.AddParagraph(
                    mod: ModManifest,
                    text: () =>
                        "This setting controls how many ticks the game will wait before checking if the game window has focus."
                );

                gmcm.AddNumberOption(
                    mod: this.ModManifest,
                    name: () => "Tick Threshold",
                    tooltip: () => "How many frames the mod should wait before checking if the window is in focus.",
                    getValue: () => _config.TickThreshold,
                    setValue: value => _config.TickThreshold = value,
                    max: 60,
                    min: 0
                );
            }
            catch (Exception ex)
            {
                Monitor.Log($"Caught exception trying to set up GMCM: {ex.Message}");
            }
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            _tickCounter++;

            if (_tickCounter < _config.TickThreshold)
                return;

            _tickCounter = 0;
            Game1.options.hardwareCursor = (!Game1.game1.IsActive || !Game1.displayHUD);
        }
    }
}