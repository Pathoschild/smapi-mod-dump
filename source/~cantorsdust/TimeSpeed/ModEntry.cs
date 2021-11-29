/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cantorsdust/StardewMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using TimeSpeed.Framework;

namespace TimeSpeed
{
    /// <summary>The entry class called by SMAPI.</summary>
    internal class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>Displays messages to the user.</summary>
        private readonly Notifier Notifier = new();

        /// <summary>Provides helper methods for tracking time flow.</summary>
        private readonly TimeHelper TimeHelper = new();

        /// <summary>The mod configuration.</summary>
        private ModConfig Config;

        /// <summary>Whether the player has manually frozen (<c>true</c>) or resumed (<c>false</c>) time.</summary>
        private bool? ManualFreeze;

        /// <summary>The reason time would be frozen automatically if applicable, regardless of <see cref="ManualFreeze"/>.</summary>
        private AutoFreezeReason AutoFreeze = AutoFreezeReason.None;

        /// <summary>Whether time should be frozen.</summary>
        private bool IsTimeFrozen =>
            this.ManualFreeze == true
            || (this.AutoFreeze != AutoFreezeReason.None && this.ManualFreeze != false);

        /// <summary>Whether the flow of time should be adjusted.</summary>
        private bool AdjustTime;

        /// <summary>Backing field for <see cref="TickInterval"/>.</summary>
        private int _tickInterval;

        /// <summary>The number of seconds per 10-game-minutes to apply.</summary>
        private int TickInterval
        {
            get => this._tickInterval;
            set => this._tickInterval = Math.Max(value, 0);
        }


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // read config
            this.Config = helper.ReadConfig<ModConfig>();

            // add time events
            this.TimeHelper.WhenTickProgressChanged(this.OnTickProgressed);
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.TimeChanged += this.OnTimeChanged;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.Input.ButtonsChanged += this.OnButtonsChanged;
            helper.Events.Player.Warped += this.OnWarped;

            // add time freeze/unfreeze notification
            {
                bool wasPaused = false;
                helper.Events.Display.RenderingHud += (_, _) =>
                {
                    wasPaused = Game1.paused;
                    if (this.IsTimeFrozen)
                        Game1.paused = true;
                };

                helper.Events.Display.RenderedHud += (_, _) =>
                {
                    Game1.paused = wasPaused;
                };
            }
        }


        /*********
        ** Private methods
        *********/
        /****
        ** Event handlers
        ****/
        /// <summary>Raised after the player loads a save slot and the world is initialized.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (!Context.IsMainPlayer)
                this.Monitor.Log("Disabled mod; only works for the main player in multiplayer.", LogLevel.Warn);
        }

        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            if (!this.ShouldEnable())
                return;

            this.UpdateScaleForDay(Game1.currentSeason, Game1.dayOfMonth);
            this.UpdateTimeFreeze(clearPreviousOverrides: true);
            this.UpdateSettingsForLocation(Game1.currentLocation);
        }

        /// <summary>Raised after the player presses or releases any buttons on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
        {
            if (!this.ShouldEnable(forInput: true))
                return;

            if (this.Config.Keys.FreezeTime.JustPressed())
                this.ToggleFreeze();
            else if (this.Config.Keys.IncreaseTickInterval.JustPressed())
                this.ChangeTickInterval(increase: true);
            else if (this.Config.Keys.DecreaseTickInterval.JustPressed())
                this.ChangeTickInterval(increase: false);
            else if (this.Config.Keys.ReloadConfig.JustPressed())
                this.ReloadConfig();
        }

        /// <summary>Raised after a player warps to a new location.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnWarped(object sender, WarpedEventArgs e)
        {
            if (!this.ShouldEnable() || !e.IsLocalPlayer)
                return;

            this.UpdateSettingsForLocation(e.NewLocation);
        }

        /// <summary>Raised after the in-game clock time changes.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTimeChanged(object sender, TimeChangedEventArgs e)
        {
            if (!this.ShouldEnable())
                return;

            this.UpdateFreezeForTime();
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!this.ShouldEnable())
                return;

            this.TimeHelper.Update();
        }

        /// <summary>Raised after the <see cref="Framework.TimeHelper.TickProgress"/> value changes.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTickProgressed(object sender, TickProgressChangedEventArgs e)
        {
            if (!this.ShouldEnable())
                return;

            if (this.IsTimeFrozen)
                this.TimeHelper.TickProgress = e.TimeChanged ? 0 : e.PreviousProgress;
            else
            {
                if (!this.AdjustTime)
                    return;
                if (this.TickInterval == 0)
                    this.TickInterval = 1000;

                if (e.TimeChanged)
                    this.TimeHelper.TickProgress = this.ScaleTickProgress(this.TimeHelper.TickProgress, this.TickInterval);
                else
                    this.TimeHelper.TickProgress = e.PreviousProgress + this.ScaleTickProgress(e.NewProgress - e.PreviousProgress, this.TickInterval);
            }
        }

        /****
        ** Methods
        ****/
        /// <summary>Get whether time features should be enabled.</summary>
        /// <param name="forInput">Whether to check for input handling.</param>
        private bool ShouldEnable(bool forInput = false)
        {
            // is loaded and host player (farmhands can't change time)
            if (!Context.IsWorldReady || !Context.IsMainPlayer)
                return false;

            // only handle keys if the player is free, or currently watching an event
            if (forInput && !Context.IsPlayerFree && !Game1.eventUp)
                return false;

            return true;
        }

        /// <summary>Reload <see cref="Config"/> from the config file.</summary>
        private void ReloadConfig()
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            this.UpdateScaleForDay(Game1.currentSeason, Game1.dayOfMonth);
            this.UpdateSettingsForLocation(Game1.currentLocation);
            this.Notifier.ShortNotify(this.Helper.Translation.Get("message.config-reloaded"));
        }

        /// <summary>Increment or decrement the tick interval, taking into account the held modifier key if applicable.</summary>
        /// <param name="increase">Whether to increment the tick interval; else decrement.</param>
        private void ChangeTickInterval(bool increase)
        {
            // get offset to apply
            int change = 1000;
            {
                KeyboardState state = Keyboard.GetState();
                if (state.IsKeyDown(Keys.LeftControl))
                    change *= 100;
                else if (state.IsKeyDown(Keys.LeftShift))
                    change *= 10;
                else if (state.IsKeyDown(Keys.LeftAlt))
                    change /= 10;
            }

            // update tick interval
            if (!increase)
            {
                int minAllowed = Math.Min(this.TickInterval, change);
                this.TickInterval = Math.Max(minAllowed, this.TickInterval - change);
            }
            else
                this.TickInterval = this.TickInterval + change;

            // log change
            this.Notifier.QuickNotify(
                this.Helper.Translation.Get("message.speed-changed", new { seconds = this.TickInterval / 1000 })
            );
            this.Monitor.Log($"Tick length set to {this.TickInterval / 1000d: 0.##} seconds.", LogLevel.Info);
        }

        /// <summary>Toggle whether time is frozen.</summary>
        private void ToggleFreeze()
        {
            if (!this.IsTimeFrozen)
            {
                this.UpdateTimeFreeze(manualOverride: true);
                this.Notifier.QuickNotify(this.Helper.Translation.Get("message.time-stopped"));
                this.Monitor.Log("Time is frozen globally.", LogLevel.Info);
            }
            else
            {
                this.UpdateTimeFreeze(manualOverride: false);
                this.Notifier.QuickNotify(this.Helper.Translation.Get("message.time-resumed"));
                this.Monitor.Log($"Time is resumed at \"{Game1.currentLocation.Name}\".", LogLevel.Info);
            }
        }

        /// <summary>Update the time freeze settings for the given time of day.</summary>
        private void UpdateFreezeForTime()
        {
            bool wasFrozen = this.IsTimeFrozen;
            this.UpdateTimeFreeze();

            if (!wasFrozen && this.IsTimeFrozen)
            {
                this.Notifier.ShortNotify(this.Helper.Translation.Get("message.on-time-change.time-stopped"));
                this.Monitor.Log($"Time automatically set to frozen at {Game1.timeOfDay}.", LogLevel.Info);
            }
        }

        /// <summary>Update the time settings for the given location.</summary>
        /// <param name="location">The game location.</param>
        private void UpdateSettingsForLocation(GameLocation location)
        {
            if (location == null)
                return;

            // update time settings
            this.UpdateTimeFreeze();
            if (this.Config.GetTickInterval(location) != null)
                this.TickInterval = this.Config.GetTickInterval(location) ?? this.TickInterval;

            // notify player
            if (this.Config.LocationNotify)
            {
                switch (this.AutoFreeze)
                {
                    case AutoFreezeReason.FrozenAtTime when this.IsTimeFrozen:
                        this.Notifier.ShortNotify(this.Helper.Translation.Get("message.on-location-change.time-stopped-globally"));
                        break;

                    case AutoFreezeReason.FrozenForLocation when this.IsTimeFrozen:
                        this.Notifier.ShortNotify(this.Helper.Translation.Get("message.on-location-change.time-stopped-here"));
                        break;

                    default:
                        this.Notifier.ShortNotify(this.Helper.Translation.Get("message.on-location-change.time-speed-here", new { seconds = this.TickInterval / 1000 }));
                        break;
                }
            }
        }

        /// <summary>Update the <see cref="AutoFreeze"/> and <see cref="ManualFreeze"/> flags based on the current context.</summary>
        /// <param name="manualOverride">An explicit freeze (<c>true</c>) or unfreeze (<c>false</c>) requested by the player, if applicable.</param>
        /// <param name="clearPreviousOverrides">Whether to clear any previous explicit overrides.</param>
        private void UpdateTimeFreeze(bool? manualOverride = null, bool clearPreviousOverrides = false)
        {
            bool? wasManualFreeze = this.ManualFreeze;
            AutoFreezeReason wasAutoFreeze = this.AutoFreeze;

            // update auto freeze
            this.AutoFreeze = this.GetAutoFreezeType();

            // update manual freeze
            if (manualOverride.HasValue)
                this.ManualFreeze = manualOverride.Value;
            else if (clearPreviousOverrides)
                this.ManualFreeze = null;

            // clear manual unfreeze if it's no longer needed
            if (this.ManualFreeze == false && this.AutoFreeze == AutoFreezeReason.None)
                this.ManualFreeze = null;

            // log change
            if (wasAutoFreeze != this.AutoFreeze)
                this.Monitor.Log($"Auto freeze changed from {wasAutoFreeze} to {this.AutoFreeze}.");
            if (wasManualFreeze != this.ManualFreeze)
                this.Monitor.Log($"Manual freeze changed from {wasManualFreeze?.ToString() ?? "null"} to {this.ManualFreeze?.ToString() ?? "null"}.");
        }

        /// <summary>Update the time settings for the given date.</summary>
        /// <param name="season">The current season.</param>
        /// <param name="dayOfMonth">The current day of month.</param>
        private void UpdateScaleForDay(string season, int dayOfMonth)
        {
            this.AdjustTime = this.Config.ShouldScale(season, dayOfMonth);
        }

        /// <summary>Get the adjusted progress towards the next 10-game-minute tick.</summary>
        /// <param name="progress">The current progress.</param>
        /// <param name="newTickInterval">The new tick interval.</param>
        private double ScaleTickProgress(double progress, int newTickInterval)
        {
            return progress * this.TimeHelper.CurrentDefaultTickInterval / newTickInterval;
        }

        /// <summary>Get the freeze type which applies for the current context, ignoring overrides by the player.</summary>
        private AutoFreezeReason GetAutoFreezeType()
        {
            if (this.Config.ShouldFreeze(Game1.currentLocation))
                return AutoFreezeReason.FrozenForLocation;

            if (this.Config.ShouldFreeze(Game1.timeOfDay))
                return AutoFreezeReason.FrozenAtTime;

            return AutoFreezeReason.None;
        }
    }
}
