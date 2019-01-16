using System;
using System.Collections.Generic;
using Omegasis.MoreRain.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace Omegasis.MoreRain
{
    /// <summary>The mod entry point.</summary>
    public class MoreRain : Mod
    {
        /*********
        ** Fields
        *********/
        /// <summary>The weathers that can be safely overridden.</summary>
        private readonly HashSet<int> NormalWeathers = new HashSet<int> { Game1.weather_sunny, Game1.weather_rain, Game1.weather_lightning, Game1.weather_debris, Game1.weather_snow };

        /// <summary>The mod configuration.</summary>
        private ModConfig Config;


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
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player loads a save slot and the world is initialised.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            this.HandleNewDay();
        }

        /// <summary>Raised before the game begins writes data to the save file (except the initial save creation).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaving(object sender, SavingEventArgs e)
        {
            this.HandleNewDay();
        }

        /// <summary>Update all data for a new day.</summary>
        private void HandleNewDay()
        {
            // skip if special weather
            if (!this.NormalWeathers.Contains(Game1.weatherForTomorrow))
            {
                if (Game1.weatherForTomorrow == Game1.weather_festival)
                    this.VerboseLog("There is a festival tomorrow, therefore it will not rain.");
                else if (Game1.weatherForTomorrow == Game1.weather_wedding)
                    this.VerboseLog("There is a wedding tomorrow and rain on your wedding day will not happen.");
                else
                    this.VerboseLog("The weather tomorrow is unknown, so it will not rain.");
                return;
            }

            // set weather
            Random random = new Random();
            int chance = random.Next(0, 100);
            switch (Game1.currentSeason)
            {
                case "spring":
                    // set rain
                    if (chance <= this.Config.SpringRainChance)
                    {
                        Game1.weatherForTomorrow = Game1.weather_rain;
                        this.VerboseLog("It will rain tomorrow.");
                        return;
                    }

                    if (chance <= this.Config.SpringThunderChance)
                    {
                        Game1.weatherForTomorrow = Game1.weather_lightning;
                        this.VerboseLog("It will be stormy tomorrow.");
                        return;
                    }
                    break;

                case "summer":
                    // set rain
                    if (chance <= this.Config.SummerRainChance)
                    {
                        Game1.weatherForTomorrow = Game1.weather_rain;
                        this.VerboseLog("It will rain tomorrow.");
                        return;
                    }


                    if (chance <= this.Config.SummerThunderChance)
                    {
                        Game1.weatherForTomorrow = Game1.weather_lightning;
                        this.VerboseLog("It will be stormy tomorrow.");
                        return;
                    }
                    break;

                case "fall":
                case "autumn":
                    // set rain
                    if (chance <= this.Config.FallRainChance)
                    {
                        Game1.weatherForTomorrow = Game1.weather_rain;
                        this.VerboseLog("It will rain tomorrow.");
                        return;
                    }

                    if (chance <= this.Config.FallThunderChance)
                    {
                        Game1.weatherForTomorrow = Game1.weather_lightning;
                        this.VerboseLog("It will be stormy tomorrow.");
                        return;
                    }
                    break;

                case "winter":
                    // set snow
                    if (chance <= this.Config.WinterSnowChance)
                    {
                        Game1.weatherForTomorrow = Game1.weather_snow;
                        this.VerboseLog("It will snow tomorrow.");
                    }
                    else
                    {
                        //StardewValley.Game1.weatherForTomorrow = StardewValley.Game1.weather_sunny;
                        this.VerboseLog("It will not snow tomorrow.");
                    }
                    break;
            }
        }

        /// <summary>Log a message if <see cref="ModConfig.SuppressLog"/> is <c>false</c>.</summary>
        /// <param name="message">The message to log.</param>
        private void VerboseLog(string message)
        {
            if (!this.Config.SuppressLog)
                this.Monitor.Log(message);
        }
    }
}
