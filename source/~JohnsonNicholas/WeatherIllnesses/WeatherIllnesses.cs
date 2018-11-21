using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Objects;
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

        /// <summary> Main mod function. </summary>
        /// <param name="helper">The helper. </param>
        public override void Entry(IModHelper helper)
        {
            IllnessConfig = helper.ReadConfig<IllnessConfig>();
            Dice = new MersenneTwister();
            StaminaMngr = new StaminaDrain(IllnessConfig, Helper.Translation, Monitor);
            TicksOutside = TicksTotal = 0;

            SaveEvents.AfterReturnToTitle += HandleResetToMenu;
            TimeEvents.AfterDayStarted += HandleNewDay;
            GameEvents.UpdateTick += HandleChangesPerTick;
            TimeEvents.TimeOfDayChanged += TenMinuteUpdate;
            GameEvents.FirstUpdateTick += HandleIntegrations;
        }

        private void HandleIntegrations(object sender, EventArgs e)
        {
            climatesAPI = SDVUtilities.GetModApi<Integrations.IClimatesOfFerngillAPI>(Monitor, Helper, "KoihimeNakamura.ClimatesOfFerngill", "1.4-beta.12");

            if (climatesAPI != null)
            {
                UseClimates = true;
            }
        }

        private void TenMinuteUpdate(object sender, EventArgsIntChanged e)
        {
            if (!Game1.hasLoadedGame)
                return;

            if (Game1.currentLocation is BathHousePool bh && Game1.player.swimming.Value)
            {
                TimeInBathHouse += 10;
                Monitor.Log($"In the BathHouse Pool for {TimeInBathHouse}");
            }

            if (TimeInBathHouse > 30)
            {
                StaminaMngr.ClearDrain(StaminaDrain.BathHouseClear);
                TimeInBathHouse = 0;
            }

            string weatherStatus = "";
            //get current weather string
            if (UseClimates)
                weatherStatus = climatesAPI.GetCurrentWeatherName();
            else
                weatherStatus = SDVUtilities.GetWeatherName();

            //handle being inside...
            double temp = (UseClimates) ? climatesAPI.GetTodaysLow() : 100.0;
            
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

        private void HandleChangesPerTick(object sender, EventArgs e)
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

        private void HandleNewDay(object sender, EventArgs e)
        {
            TicksOutside = TicksTotal = TicksInLocation = 0;
            StaminaMngr.OnNewDay();
            TimeInBathHouse = 0;
        }

        private void HandleResetToMenu(object sender, EventArgs e)
        {
            TicksTotal = TicksOutside = TicksInLocation = 0;
            StaminaMngr.Reset();
            TimeInBathHouse = 0;
        }
    }
}
