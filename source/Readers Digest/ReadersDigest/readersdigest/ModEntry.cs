/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/remybach/stardew-valley-readersdigest
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Objects;
using StardewValley;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YourProjectName
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Fields
        *********/
        private ModConfig Config;
        private string LastLearned = "";


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper) {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.Display.MenuChanged += OnMenuChanged;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e) {
            if (!this.Config.Clairvoyance && e.OldMenu is StardewValley.Menus.LetterViewerMenu && LastLearned != "") {
               Game1.addHUDMessage(new HUDMessage(LastLearned, 2));

                LastLearned = "";
            }
            
        }

        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e) {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady) return;

            TV tv = new TV();
            string day = Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth);

            // Logic for which days each these are displayed taken from here: https://github.com/sndcode/stardewvalleycode/blob/master/Stardew%20Valley/Objects/TV.cs#L31-L43
            if (this.Config.EnableTips && (day.Equals("Mon") || day.Equals("Thu"))) {
                string tip = this.Helper.Reflection
                               .GetMethod(tv, "getTodaysTip")
                               .Invoke<string>();

                if (!tip.Contains("Just kidding")) {
                    // Calculation taken from here: https://github.com/sndcode/stardewvalleycode/blob/master/Stardew%20Valley/Objects/TV.cs#L272
                    uint tipNumber = Game1.stats.DaysPlayed % 224u;

                    Game1.mailbox.Add($"tip_{tipNumber}");
                }
            }

            if ((this.Config.EnableCooking || this.Config.Clairvoyance) && day.Equals("Sun")) {
                // Calculation taken from here: https://github.com/sndcode/stardewvalleycode/blob/master/Stardew%20Valley/Objects/TV.cs#L287
                int whichWeek = (int)(Game1.stats.DaysPlayed % 224u / 7u);
                if (day.Equals("Wed")) {
                    Random r = new Random((int)(Game1.stats.DaysPlayed + (uint)((int)Game1.uniqueIDForThisGame / 2)));
                    whichWeek = Math.Max(1, 1 + r.Next((int)(Game1.stats.DaysPlayed % 224u)) / 7);
                }

                string recipeName = this.getRecipeName(whichWeek);

                if (recipeName != "" && !Game1.player.cookingRecipes.ContainsKey(recipeName)) {
                    string[] cooking = this.Helper.Reflection
                               .GetMethod(tv, "getWeeklyRecipe") // This is the part that actually adds the recipe as a learned recipe
                               .Invoke<string[]>();

                    if (cooking.Length > -1) {
                        LastLearned = cooking[1];

                        // Insert this at the top of the queue so the next close event shows the correct recipe learned message.
                        if (this.Config.EnableCooking) {
                            Game1.mailbox.Insert(0, $"cooking_{whichWeek}");
                        }

                        // If the farmer is clairvoyant, show the 'learned' message as a tooltip upon waking up.
                        if (this.Config.Clairvoyance) {
                            Game1.delayedActions.Add(new DelayedAction(1000, () => Game1.addHUDMessage(new HUDMessage(LastLearned, 2))));
                        }
                    }
                }
            }
        }

        // Taken and adapted from https://github.com/sndcode/stardewvalleycode/blob/master/Stardew%20Valley/Objects/TV.cs#L280-L333
        private string getRecipeName(int whichWeek) {
            string recipeName;

            Dictionary<string, string> cookingRecipeChannel = Game1.temporaryContent.Load<Dictionary<string, string>>("Data\\TV\\CookingChannel");
            
            try {
                recipeName = cookingRecipeChannel[whichWeek.ToString()].Split('/')[0];
            } catch (Exception) {
                recipeName = cookingRecipeChannel["1"].Split('/')[0];
            }

            return recipeName;
        }
    }
}