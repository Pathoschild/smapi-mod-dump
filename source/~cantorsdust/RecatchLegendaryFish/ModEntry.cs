/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cantorsdust/StardewMods
**
*************************************************/

using RecatchLegendaryFish.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Tools;

namespace RecatchLegendaryFish
{
    /// <summary>The entry class called by SMAPI.</summary>
    internal class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration.</summary>
        private ModConfig Config;

        /// <summary>Whether the mod is currently enabled.</summary>
        private bool IsEnabled = true;

        /// <summary>Temporarily hides caught legendary fish from the game.</summary>
        private readonly PerScreen<FishStash> Stash = new(() => new());


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.Saving += this.OnSaving;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.Input.ButtonsChanged += this.OnButtonsChanged;
        }


        /*********
        ** Private methods
        *********/
        /****
        ** Event handlers
        ****/
        /// <summary>Raised after the player loads a save slot and the world is initialised.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            this.Stash.Value.Clear();
        }

        /// <summary>Raised before the game begins writes data to the save file (except the initial save creation).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaving(object sender, SavingEventArgs e)
        {
            this.Stash.Value.Restore(); // just in case something weird happens
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            // stash legendaries while fishing
            var stash = this.Stash.Value;
            bool isFishing = Game1.player.UsingTool && Game1.player.CurrentTool is FishingRod;
            if (isFishing)
            {
                if (this.IsEnabled && !stash.IsStashed)
                    stash.Start();
            }
            else if (stash.IsStashed)
                stash.Restore();
        }

        /// <summary>Raised after the player presses or releases any buttons on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
        {
            if (Context.IsPlayerFree && this.Config.ToggleKey.JustPressed())
                this.OnToggle();
        }

        /// <summary>Handle the toggle key.</summary>
        private void OnToggle()
        {
            this.IsEnabled = !this.IsEnabled;

            string message = this.Helper.Translation.Get(
                this.IsEnabled ? "message.enabled" : "message.disabled",
                new
                {
                    key = this.Config.ToggleKey.GetKeybindCurrentlyDown().ToString()
                }
            );
            Game1.addHUDMessage(new HUDMessage(message, HUDMessage.newQuest_type) { timeLeft = 2500 });
        }
    }
}
