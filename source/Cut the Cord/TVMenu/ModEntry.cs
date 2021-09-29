/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/harshlele/CutTheCord
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Menus;

namespace CutTheCord
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {

        private enum MenuItem { WEATHER, FORTUNE, RECIPE, TIPS, FISHINGTIP};

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            //helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            // print button presses to the console window
            this.Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);
        }

        /// <summary>Raised after the day has started.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            List<MenuItem> todaysItems = this.GetTodaysItems();
            List<string> todaysReport = this.GenTodaysReport(todaysItems);
            string msg = string.Join("^", todaysReport.ToArray());

            Game1.activeClickableMenu = new LetterViewerMenu(msg);

        }

        /// <summary>Generate the report text based on today's items</summary>
        /// <param name="items">The items(weather forecast, fortune etc) for the day</param>
        private List<string> GenTodaysReport(List<MenuItem> items)
        {
            List<string> report = new List<string>();
            report.Add($"Good Morning, {Game1.player.Name}!");

            var date = SDate.Now();
            report.Add(date.ToLocaleString());

            if (Game1.player.mailbox.Count > 0)
            {
                report.Add($"You have {Game1.player.mailbox.Count} letters");
            }
            report.Add("");
            TV t = new TV();
            foreach (MenuItem i in items)
            {
                switch (i)
                {
                    case MenuItem.WEATHER:
                        report.Add("Weather Forecast:");
                        string weather = this.Helper.Reflection.GetMethod(t, "getWeatherForecast").Invoke<string>();
                        report.Add(weather);
                        report.Add("");
                        break;
                    case MenuItem.FORTUNE:
                        report.Add("Today's Luck: ");
                        string luck = this.Helper.Reflection.GetMethod(t, "getFortuneForecast").Invoke<string>(Game1.player);
                        report.Add(luck);
                        report.Add("");
                        break;
                    case MenuItem.TIPS:
                        report.Add("Livin' Off The Land: ");
                        string tips = this.Helper.Reflection.GetMethod(t, "getTodaysTip").Invoke<string>();
                        report.Add(tips);
                        report.Add("");
                        break;
                    case MenuItem.RECIPE:
                        report.Add("The Queen of Sauce: ");
                        string[] recipe = this.Helper.Reflection.GetMethod(t, "getWeeklyRecipe").Invoke<string[]>();
                        if(recipe[1].Contains("You already know how to cook"))
                        {
                            if(recipe[1].ToLower().Contains("carp surprise"))
                            {
                                report.Add("Re-run for Carp Surprise");
                            }
                            else if (recipe[1].ToLower().Contains("melon"))
                            {
                                report.Add("Re-run for Melon");
                            }
                            else
                            {
                                string[] n = recipe[0].Split('!');
                                if(n.Length > 0) report.Add($"Re-run for {n[0]}");
                            }
                           
                        }
                        else
                        {
                            report.Add(recipe[0]);
                        }
                        report.Add("");
                        break;
                }
            }

            return report;
        }

        /// <summary>Get the items to show for the daily briefing(uses same logic as game code)</summary>
        private List<MenuItem> GetTodaysItems()
        {
            List<MenuItem> todaysItems = new List<MenuItem>();
            todaysItems.Add(MenuItem.WEATHER);
            todaysItems.Add(MenuItem.FORTUNE);
            string day = Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth);
            if (day.Equals("Mon") || day.Equals("Thu"))
            {
                todaysItems.Add(MenuItem.TIPS);
            }
            if (day.Equals("Sun") || (day.Equals("Wed") && Game1.stats.DaysPlayed > 7))
            {
                todaysItems.Add(MenuItem.RECIPE);
            }
            if (Game1.player.mailReceived.Contains("pamNewChannel"))
            {
                todaysItems.Add(MenuItem.FISHINGTIP);
            }

            return todaysItems;
        }


        private void Log(string l)
        {
            this.Monitor.Log(l, LogLevel.Debug);
        }
    }
}