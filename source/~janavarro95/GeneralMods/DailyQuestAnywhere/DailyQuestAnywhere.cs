using System;
using Omegasis.DailyQuestAnywhere.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Quests;

namespace Omegasis.DailyQuestAnywhere
{
    /*
     *TODO: Make quest core mod??? 
     */
    /// <summary>The mod entry point.</summary>
    public class DailyQuestAnywhere : Mod
    {
        /*********
        ** Fields
        *********/
        /// <summary>The mod configuration.</summary>
        private ModConfig Config;

        Quest dailyQuest;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<ModConfig>();

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (Context.IsPlayerFree && e.Button == this.Config.KeyBinding)
            {
                if (!Game1.player.hasDailyQuest())
                {
                    if (this.dailyQuest == null)
                        this.dailyQuest = this.generateDailyQuest();
                    Game1.questOfTheDay = this.dailyQuest;
                    Game1.activeClickableMenu = new Billboard(true);
                }
            }
        }

        /// <summary>Raised after the player loads a save slot and the world is initialised.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // makes daily quest null so we can't just keep getting a new reference
            this.dailyQuest = null;
        }

        /// <summary>Generate a daily quest for sure.</summary>
        public Quest generateDailyQuest()
        {
            Random chanceRandom = new Random((int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed);
            int chance = chanceRandom.Next(0, 101);
            float actualChance = chance / 100;

            //If we hit the chance for actually generating a daily quest do so, otherwise don't generate a daily quest.
            if (actualChance <= this.Config.chanceForDailyQuest)
            {
                Random r = new Random((int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed);
                int rand = r.Next(0, 7);
                switch (rand)
                {
                    case 0:
                        return new ItemDeliveryQuest();
                    case 1:
                        return new FishingQuest();
                    case 2:
                        return new CraftingQuest();
                    case 3:
                        return new ItemDeliveryQuest();
                    case 4:
                        return new ItemHarvestQuest();
                    case 5:
                        return new ResourceCollectionQuest();
                    case 6:
                        return new SlayMonsterQuest();
                }
            }
            return null; //This should never happen.
        }
    }
}
