/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sakorona/SDVMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace StardewNotification
{
    /// <summary>
    /// The mod entry point
    /// </summary>
    public class StardewNotification : Mod
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

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.Player.Warped += OnWarped;
            helper.Events.GameLoop.TimeChanged += OnTimeChanged;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var api = Helper.ModRegistry.GetApi<Integrations.IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (api != null)
            {
                Monitor.Log("Accessed mod-provided API for Generic Mod Config Menu.", LogLevel.Trace);
                api.Register(ModManifest, () => Config = new SNConfiguration(), () => Helper.WriteConfig(Config));
               
                api.AddNumberOption(ModManifest, () => (float) Config.NotificationDuration, (float val) => Config.NotificationDuration = val,()=> Helper.Translation.Get("gmcmNotDurTitle"),() => Helper.Translation.Get("gmcmNotDurDesc"),0f, 14000f);
                api.AddBoolOption(ModManifest, () => Config.NotifyBirthdays, (bool val) => Config.NotifyBirthdays = val, () => Helper.Translation.Get("gmcmNotifOnBirthTitle"), () => Helper.Translation.Get("gmcmNotifOnBirthDesc"));
                api.AddBoolOption(ModManifest, () => Config.NotifyBirthdayReminder, (bool val) => Config.NotifyBirthdayReminder = val, () => Helper.Translation.Get("gmcmNotifOnBirthRemindTitle"), () => Helper.Translation.Get("gmcmNotifOnBirthRemindDesc"));
                api.AddNumberOption(ModManifest, () => Config.BirthdayReminderTime, (int val) => Config.BirthdayReminderTime = val, () => Helper.Translation.Get("gmcmNotifOnBirthRemindTimeTitle"), () => Helper.Translation.Get("gmcmNotifOnBirthRemindTimeDesc"), 900, 1900,10);                
                api.AddBoolOption(ModManifest, () => Config.NotifyFestivals, (bool val) => Config.NotifyFestivals = val, () => Helper.Translation.Get("gmcmNotifOnFestivalTitle"), () => Helper.Translation.Get("gmcmNotifOnFestivalDesc"));

                api.AddBoolOption(ModManifest, () => Config.NotifyTravelingMerchant, (bool val) => Config.NotifyTravelingMerchant = val, () => Helper.Translation.Get("gmcmNotifOnMerchantTitle"), () => Helper.Translation.Get("gmcmNotifOnMerchantDesc"));
                api.AddBoolOption(ModManifest, () => Config.NotifyToolUpgrade, (bool val) => Config.NotifyToolUpgrade = val, () => Helper.Translation.Get("gmcmNotifOnToolTitle"), () => Helper.Translation.Get("gmcmNotifOnToolDesc"));
                api.AddBoolOption(ModManifest, () => Config.NotifyMaxLuck, (bool val) => Config.NotifyMaxLuck = val, () => Helper.Translation.Get("gmcmNotifOnGLuckTitle"), () => Helper.Translation.Get("gmcmNotifOnGLuckDesc"));
                api.AddBoolOption(ModManifest, () => Config.NotifyMinLuck, (bool val) => Config.NotifyMinLuck = val, () => Helper.Translation.Get("gmcmNotifOnBLuckTitle"), () => Helper.Translation.Get("gmcmNotifOnBLuckDesc"));

                api.AddBoolOption(ModManifest, () => Config.NotifyHay, (bool val) => Config.NotifyHay = val, () => Helper.Translation.Get("gmcmNotifShowHayCountTitle"), () => Helper.Translation.Get("gmcmNotifShowHayCountDesc"));
                api.AddBoolOption(ModManifest, () => Config.ShowEmptyhay, (bool val) => Config.ShowEmptyhay = val, () => Helper.Translation.Get("gmcmNotifEmptyHayTitle"), () => Helper.Translation.Get("gmcmNotifEmptyHayDesc"));

                api.AddBoolOption(ModManifest, () => Config.ShowWeatherNextDay, (bool val) => Config.ShowWeatherNextDay = val, () => Helper.Translation.Get("gmcmNotifWeatNDTitle"), () => Helper.Translation.Get("gmcmNotifWeatNDDesc"));
                api.AddNumberOption(ModManifest, () => Config.WeatherNextDayTime, (int val) => Config.WeatherNextDayTime = val, () => Helper.Translation.Get("gmcmRemindTimeForNWDTitle"), () => Helper.Translation.Get("gmcmRemindTimeForNWDDesc"), 900, 2600, 10);

                api.AddBoolOption(ModManifest, () => Config.NotifyTVChannels, (bool val) => Config.NotifyTVChannels = val, () => Helper.Translation.Get("gmcmNotifTVChanTitle"), () => Helper.Translation.Get("gmcmNotifTVChanDesc"));
                api.AddBoolOption(ModManifest, () => Config.ShowSpringOnionCount, (bool val) => Config.ShowSpringOnionCount = val, () => Helper.Translation.Get("gmcmNotifSpringOnionTitle"), () => Helper.Translation.Get("gmcmNotifSpringOnionDesc"));

                api.AddBoolOption(ModManifest, () => Config.NotifyFarmCave, (bool val) => Config.NotifyFarmCave = val, () => Helper.Translation.Get("gmcmNotifFarmCaveTitle"), () => Helper.Translation.Get("gmcmNotifFarmCaveDesc"));
                api.AddBoolOption(ModManifest, () => Config.NotifyGreenhouseCrops, (bool val) => Config.NotifyGreenhouseCrops = val, () => Helper.Translation.Get("gmcmNotifGreenCropTitle"), () => Helper.Translation.Get("gmcmNotifGreenCropDesc"));

                api.AddBoolOption(ModManifest, () => Config.NotifyShed, (bool val) => Config.NotifyShed = val, () => Helper.Translation.Get("gmcmNotifShedProdTitle"), () => Helper.Translation.Get("gmcmNotifShedProdDesc"));
                api.AddBoolOption(ModManifest, () => Config.NotifyGreenhouse, (bool val) => Config.NotifyGreenhouse = val, () => Helper.Translation.Get("gmcmNotifGreenProdTitle"), () => Helper.Translation.Get("gmcmNotifGreenProdDesc"));
                api.AddBoolOption(ModManifest, () => Config.NotifyCellar, (bool val) => Config.NotifyCellar = val, () => Helper.Translation.Get("gmcmNotifCellarProdTitle"), () => Helper.Translation.Get("gmcmNotifCellarProdDesc"));
                api.AddBoolOption(ModManifest, () => Config.NotifyBarn, (bool val) => Config.NotifyBarn = val, () => Helper.Translation.Get("gmcmNotifBarnProdTitle"), () => Helper.Translation.Get("gmcmNotifBarnProdDesc"));
            }
        }

        /// <summary>Raised after the in-game clock time changes.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTimeChanged(object sender, TimeChangedEventArgs e)
        {
            // send daily notifications
            if (Config.RunNotificationsTime != 600 && Config.RunNotificationsTime == e.NewTime)
            {
                generalNotification.DoNewDayNotifications(Helper.Translation);
                harvestableNotification.CheckHarvestsAroundFarm();
            }

            if (Config.NotifyBirthdayReminder && e.NewTime == Config.BirthdayReminderTime)
                GeneralNotification.DoBirthdayReminder(Helper.Translation);

            if (Config.ShowWeatherNextDay && e.NewTime == Config.WeatherNextDayTime)
                GeneralNotification.DoWeatherReminder(Helper.Translation);
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

        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            if (Game1.currentSeason.Equals("Spring") && Game1.dayOfMonth == 0 && Game1.year == 1)
                return;

            // send daily notifications
            if (Config.RunNotificationsTime == 600) { 
                generalNotification.DoNewDayNotifications(Helper.Translation);
                harvestableNotification.CheckHarvestsAroundFarm();
            }
        }
    }
}
