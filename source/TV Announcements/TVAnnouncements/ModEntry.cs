/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Zyin055/TVAnnouncements
**
*************************************************/

using System;
using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;

namespace TVAnnouncements
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /// <summary>The mod configuration from the player.</summary>
        private ModConfig Config;
        
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.DayStarted += DayStarted;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.Config)
            );

            // add some config options
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Show Daily Weather Forcast",
                //tooltip: () => "An optional description shown as a tooltip to the player.",
                getValue: () => this.Config.ShowWeatherForcast,
                setValue: value => this.Config.ShowWeatherForcast = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Show Daily Queen Of Sauce",
                //tooltip: () => "An optional description shown as a tooltip to the player.",
                getValue: () => this.Config.ShowQueenOfSauce,
                setValue: value => this.Config.ShowQueenOfSauce = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Show Daily Luck",
                //tooltip: () => "An optional description shown as a tooltip to the player.",
                getValue: () => this.Config.ShowDailyLuck,
                setValue: value => this.Config.ShowDailyLuck = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "...and show Luck number",
                tooltip: () => "A positive or negative luck value which is added to the end of the luck message.",
                getValue: () => this.Config.ShowDailyLuckNumber,
                setValue: value => this.Config.ShowDailyLuckNumber = value
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Notification Duration (s)",
                tooltip: () => "How long the message in the bottom left is shown, in seconds.",
                getValue: () => this.Config.NotificationDuration / 1000,
                setValue: value => this.Config.NotificationDuration = value * 1000,//we store the value in milliseconds
                min: 1,
                max: 20,
                interval: 1
            );
        }

        private void DayStarted(object sender, EventArgs e)
        {
            //Game1.addHUDMessage(new HUDMessage(text, 0));
            //HUDMessage icon types:
            //1 = yellow star
            //2 = yellow "!"
            //3 = red "X"
            //4 = energy symbol
            //5 = health symbol

            int maxTextLength = 100;    //long weather reports can run off the screen, so truncate them

            if(Config.ShowDailyLuck)
            {
                //Farmer farmer = Game1.getPlayerOrEventFarmer();
                string fortune = this.Helper.Reflection.GetMethod(new TV(), "getFortuneForecast").Invoke<string>(Game1.player);
                fortune = fortune.Substring(0, Math.Min(maxTextLength, fortune.Length));
                if (Config.ShowDailyLuckNumber)
                {
                   if (Game1.player.DailyLuck > 0)
                        fortune += " (+" + Game1.player.DailyLuck + ")";
                    else
                        fortune += " (" + Game1.player.DailyLuck + ")";
                }

                HUDMessage fortuneMessage = new HUDMessage(fortune, new Color(), Config.NotificationDuration, true);    //the Color variable does nothing
                fortuneMessage.noIcon = true;
                Game1.addHUDMessage(fortuneMessage);
            }


            if (Config.ShowQueenOfSauce)
            {
                string recipe = getTheQueenOfSauceRecipeToday();
                if (recipe != null)
                {
                    HUDMessage recipeMessage = new HUDMessage(recipe + " is on today!", new Color(), Config.NotificationDuration, true);    //the Color variable does nothing
                    recipeMessage.noIcon = true;
                    Game1.addHUDMessage(recipeMessage);
                }
            }

            
            if (Config.ShowWeatherForcast)
            {
                string forcast = this.Helper.Reflection.GetMethod(new TV(), "getWeatherForecast").Invoke<string>();
                forcast = forcast.Substring(0, Math.Min(maxTextLength, forcast.Length));
                HUDMessage forcastMessage = new HUDMessage(forcast, new Color(), Config.NotificationDuration, true);    //the Color variable does nothing
                forcastMessage.noIcon = true;
                Game1.addHUDMessage(forcastMessage);
            }
        }


        private string getTheQueenOfSauceRecipeToday()
        {
            //this logic was copied from TV.checkForAction()
            string str = Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth);
            if (str.Equals("Sun"))
                return Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13114");
            if (str.Equals("Wed") && Game1.stats.DaysPlayed > 7U)
                return Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13117");

            return null;
        }
    }
}