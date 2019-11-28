using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using TwilightShards.Common;
using TwilightShards.Stardew.Common;

namespace TwilightShards.WeatherIllnesses
{
    public class WeatherIllnesses : Mod
    {
        private IllnessConfig IllnessConfig { get; set; }
        private MersenneTwister Dice { get; set; }
        private StaminaDrain StaminaMngr { get; set;}

        private int TicksOutside;
        private int TicksTotal;
        private int TicksInLocation;
        private int prevToEatStack = -1;
        private bool wasEating = false;
        private int TimeInBathHouse = 0;

        private bool UseClimates = false;
        private Integrations.IClimatesOfFerngillAPI climatesAPI;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            IllnessConfig = helper.ReadConfig<IllnessConfig>();
            Dice = new MersenneTwister();
            StaminaMngr = new StaminaDrain(IllnessConfig, Helper.Translation, Monitor);
            TicksOutside = TicksTotal = 0;

            helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.GameLoop.TimeChanged += OnTimeChanged;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            climatesAPI = SDVUtilities.GetModApi<Integrations.IClimatesOfFerngillAPI>(Monitor, Helper, "KoihimeNakamura.ClimatesOfFerngill", "1.5-beta.2");

            if (climatesAPI != null)
            {
                UseClimates = true;
            }

            var api = Helper.ModRegistry.GetApi<Integrations.GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
            if (api != null)
            {
                api.RegisterModConfig(ModManifest, () => IllnessConfig = new IllnessConfig(), () => Helper.WriteConfig(IllnessConfig));
                api.RegisterClampedOption(ModManifest, "Stamina Drain", "The drain per 10 minute tick. I wouldn't recommend a too high value.", () => IllnessConfig.StaminaDrain, (int val) => IllnessConfig.StaminaDrain = val, 0, 65);
                api.RegisterSimpleOption(ModManifest, "Sick More Than Once", "This option controls if you can get sick more than once a day.", () => IllnessConfig.SickMoreThanOnce, (bool val) => IllnessConfig.SickMoreThanOnce = val);
                api.RegisterSimpleOption(ModManifest, "Verbose", "This option controls verbosity in logs.", () => IllnessConfig.Verbose, (bool val) => IllnessConfig.Verbose = val);
                api.RegisterClampedOption(ModManifest, "Chance of Getting Sick", "The chance of getting sick every 10 minute tick.", () => (float)IllnessConfig.ChanceOfGettingSick, (float val) => IllnessConfig.ChanceOfGettingSick = val, 0f, 1f);
                api.RegisterClampedOption(ModManifest, "Percentage Outside", "How long you have to be outside every 10 minutes to be affected by the weather", () => (float)IllnessConfig.PercentageOutside, (float val) => IllnessConfig.PercentageOutside = val, 0f, 1f);
                api.RegisterClampedOption(ModManifest, "Too Cold Inside", "Under this temperature, you will get sick if you don't have the heat on in your house.", () => (float)IllnessConfig.TooColdInside, (float val) => IllnessConfig.TooColdInside= val, -15f, 10f);

            }
        }

        /// <summary>Raised after the in-game clock time changes.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTimeChanged(object sender, TimeChangedEventArgs e)
        {
            if (!Game1.hasLoadedGame)
                return;

            if (Game1.currentLocation is BathHousePool && Game1.player.swimming.Value)
            {
                TimeInBathHouse += 10;
                Monitor.Log($"In the BathHouse Pool for {TimeInBathHouse}");
            }

            if (TimeInBathHouse > 30)
            {
                StaminaMngr.ClearDrain(StaminaDrain.BathHouseClear);
                TimeInBathHouse = 0;
            }

            string weatherStatus;
            //get current weather string
            if (UseClimates)
                weatherStatus = climatesAPI.GetCurrentWeatherName();
            else
                weatherStatus = SDVUtilities.GetWeatherName();

            //handle being inside...
            double? temp = (UseClimates) ? climatesAPI.GetTodaysLow() : 100.0;
            if (temp is null)
            {
                temp = 100.0;
            }
            
            Game1.player.stamina += StaminaMngr.TenMinuteTick(Game1.player.hat.Value?.which.Value, temp, weatherStatus, TicksInLocation, TicksOutside, TicksTotal, Dice);

            if (Game1.player.stamina <= 0)
            {
                Game1.player.exhausted.Value = true;
                Game1.player.stamina = -20;
            }

            TicksTotal = 0;
            TicksOutside = 0;
            TicksInLocation = 0;
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Game1.hasLoadedGame)
                return;

            if (Game1.player.isEating != wasEating)
            {
                if (!Game1.player.isEating)
                {
                    // Apparently this happens when the ask to eat dialog opens, but they pressed no.
                    // So make sure something was actually consumed.
                    if (prevToEatStack != -1 && (prevToEatStack - 1 == Game1.player.itemToEat.Stack))
                    {
                        if (Game1.player.itemToEat.ParentSheetIndex == 351)
                            StaminaMngr.ClearDrain();
                    }
                }
                prevToEatStack = (Game1.player.itemToEat != null ? Game1.player.itemToEat.Stack : -1);
            }
            wasEating = Game1.player.isEating;

            if (Game1.currentLocation.IsOutdoors)
            {
                TicksOutside++;
            }

            if (Game1.currentLocation is FarmHouse)
            {
                TicksInLocation++;
            }

            TicksTotal++;
        }

        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            TicksOutside = TicksTotal = TicksInLocation = 0;
            StaminaMngr.OnNewDay();
            TimeInBathHouse = 0;
        }

        /// <summary>Raised after the game returns to the title screen.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            TicksTotal = TicksOutside = TicksInLocation = 0;
            StaminaMngr.Reset();
            TimeInBathHouse = 0;
        }
    }
}
