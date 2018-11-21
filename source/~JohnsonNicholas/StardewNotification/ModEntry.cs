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

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<SNConfiguration>();
			harvestableNotification = new HarvestNotification(Helper.Translation);
			generalNotification = new GeneralNotification();
			productionNotification = new ProductionNotification();

            SaveEvents.AfterLoad += ReceiveLoadedGame;
            TimeEvents.AfterDayStarted += DailyNotifications;
            TimeEvents.TimeOfDayChanged += ReceiveTimeOfDayChanged;
            PlayerEvents.Warped += ReceiveCurrentLocationChanged;
        }

        private void ReceiveLoadedGame(object sender, EventArgs e)
        {
            // Check for new save
            if (Game1.currentSeason.Equals("Spring") && Game1.dayOfMonth == 0 && Game1.year == 1)
                return;
			generalNotification.DoNewDayNotifications(Helper.Translation);
			harvestableNotification.CheckHarvestsAroundFarm();
        }

        private void DailyNotifications(object sender, EventArgs e)
        {
			generalNotification.DoNewDayNotifications(Helper.Translation);
			harvestableNotification.CheckHarvestsAroundFarm();
        }

        private void ReceiveTimeOfDayChanged(object sender, EventArgsIntChanged e)
        {
            if (Config.NotifyBirthdayReminder && e.NewInt == Config.BirthdayReminderTime)
				generalNotification.DoBirthdayReminder(Helper.Translation);
        }

        private void ReceiveCurrentLocationChanged(object sender, EventArgsPlayerWarped e)
        {
            if (e.NewLocation is Farm && Game1.timeOfDay < 2400 && Context.IsWorldReady)
            {
                harvestableNotification.CheckHarvestsOnFarm();
                productionNotification.CheckProductionAroundFarm(Helper.Translation);
            }
        }
    }
}
