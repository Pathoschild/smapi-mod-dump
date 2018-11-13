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

        private ModConfig Config;
        private string lastLearned = "";

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper) {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            TimeEvents.AfterDayStarted += this.TimeEvents_AfterDayStarted;
            MenuEvents.MenuClosed += this.MenuEvents_MenuClosed;
        }


        /*********
        ** Private methods
        *********/
        private void MenuEvents_MenuClosed(object sender, EventArgsClickableMenuClosed e) {
            if (!this.Config.clairvoyance && e.PriorMenu is StardewValley.Menus.LetterViewerMenu && lastLearned != "") {
               Game1.addHUDMessage(new HUDMessage(lastLearned, 2));

                lastLearned = "";
            }
            
        }

        /// <summary>The method invoked when the player presses a controller, keyboard, or mouse button.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        async private void TimeEvents_AfterDayStarted(object sender, EventArgs e) {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady) return;

            TV tv = new TV();
            string day = Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth);

            // Logic for which days each these are displayed taken from here: https://github.com/sndcode/stardewvalleycode/blob/master/Stardew%20Valley/Objects/TV.cs#L31-L43
            if (this.Config.enableTips && (day.Equals("Mon") || day.Equals("Thu"))) {
                string tip = this.Helper.Reflection
                               .GetMethod(tv, "getTodaysTip")
                               .Invoke<string>();

                if (!tip.Contains("Just kidding")) {
                    // Calculation taken from here: https://github.com/sndcode/stardewvalleycode/blob/master/Stardew%20Valley/Objects/TV.cs#L272
                    uint tipNumber = Game1.stats.DaysPlayed % 224u;

                    Game1.mailbox.Add($"tip_{tipNumber}");
                }
            }

            if ((this.Config.enableCooking || this.Config.clairvoyance) && day.Equals("Sun")) {
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
                        lastLearned = cooking[1];

                        // Insert this at the top of the queue so the next close event shows the correct recipe learned message.
                        if (this.Config.enableCooking) {
                            Game1.mailbox.Insert(0, $"cooking_{whichWeek}");
                        }

                        // If the farmer is clairvoyant, show the 'learned' message as a tooltip upon waking up.
                        if (this.Config.clairvoyance) {
                            await Task.Delay(1000);
                            Game1.addHUDMessage(new HUDMessage(lastLearned, 2));
                        }
                    }
                }
            }
        }

        // Taken and adapted from https://github.com/sndcode/stardewvalleycode/blob/master/Stardew%20Valley/Objects/TV.cs#L280-L333
        private string getRecipeName(int whichWeek) {
            string recipeName = "";

            Dictionary<string, string> cookingRecipeChannel = Game1.temporaryContent.Load<Dictionary<string, string>>("Data\\TV\\CookingChannel");
            
            try {
                recipeName = cookingRecipeChannel[string.Concat(whichWeek)].Split(new char[]
                {
                    '/'
                })[0];
            } catch (Exception) {
                recipeName = cookingRecipeChannel["1"].Split(new char[]
                {
                    '/'
                })[0];
            }

            return recipeName;
        }
    }
}