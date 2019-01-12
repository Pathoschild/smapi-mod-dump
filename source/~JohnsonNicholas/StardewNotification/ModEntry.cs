using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace StardewNotification
{
    /// <summary>
    /// The mod entry point
    /// </summary>
    class StardewNotification : Mod
    {
        private HarvestNotification harvestableNotification;
        private GeneralNotification generalNotification;
        private ProductionNotification productionNotification;
        public static SNConfiguration Config { get; set; }

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<SNConfiguration>();
            harvestableNotification = new HarvestNotification(Helper.Translation);
            generalNotification = new GeneralNotification();
            productionNotification = new ProductionNotification();

            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.Player.Warped += OnWarped;
            helper.Events.GameLoop.TimeChanged += OnTimeChanged;
        }

        /// <summary>Raised after the in-game clock time changes.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTimeChanged(object sender, TimeChangedEventArgs e)
        {
            if (Config.NotifyBirthdayReminder && e.NewTime == Config.BirthdayReminderTime)
                generalNotification.DoBirthdayReminder(Helper.Translation);
        }

        /// <summary>Raised after a player warps to a new location.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnWarped(object sender, WarpedEventArgs e)
        {
            if (e.IsLocalPlayer && e.NewLocation is Farm && Game1.timeOfDay < 2400 && Context.IsWorldReady)
            {
                harvestableNotification.CheckHarvestsOnFarm();
                productionNotification.CheckProductionAroundFarm(Helper.Translation);
            }
        }

        /// <summary>Raised after the player loads a save slot and the world is initialised.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // Check for new save
            if (Game1.currentSeason.Equals("Spring") && Game1.dayOfMonth == 0 && Game1.year == 1)
                return;
            generalNotification.DoNewDayNotifications(Helper.Translation);
            harvestableNotification.CheckHarvestsAroundFarm();
        }

        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            // send daily notifications
            generalNotification.DoNewDayNotifications(Helper.Translation);
            harvestableNotification.CheckHarvestsAroundFarm();
        }
    }
}
