using Omegasis.DailyQuestAnywhere.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Quests;
using System;

namespace Omegasis.DailyQuestAnywhere
{
    /*
     *TODO: Make quest core mod??? 
     */

    /// <summary>The mod entry point.</summary>
    public class DailyQuestAnywhere : Mod
    {
        /*********
        ** Properties
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

            ControlEvents.KeyPressed += this.ControlEvents_KeyPressed;
            StardewModdingAPI.Events.SaveEvents.AfterSave += SaveEvents_AfterSave;
        }




        /*********
        ** Private methods
        *********/
        /// <summary>The method invoked when the presses a keyboard button.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void ControlEvents_KeyPressed(object sender, EventArgsKeyPressed e)
        {
            if (Context.IsPlayerFree && e.KeyPressed.ToString() == this.Config.KeyBinding)
                if (Game1.player.hasDailyQuest() == false )
                {
                    if (this.dailyQuest == null)
                    {
                        this.dailyQuest = generateDailyQuest();
                    }
                    Game1.questOfTheDay = this.dailyQuest;
                    Game1.activeClickableMenu = new Billboard(true);
                }             
        }

        /// <summary>
        /// Makes my daily quest referene null so we can't just keep getting a new reference.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveEvents_AfterSave(object sender, System.EventArgs e)
        {
            this.dailyQuest = null; //Nullify my quest reference.
        }

        /// <summary>
        /// Generate a daily quest for sure.
        /// </summary>
        /// <returns></returns>
        public Quest generateDailyQuest()
        {

            Random chanceRandom = new Random((int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed);
            int chance = chanceRandom.Next(0, 101);
            float actualChance = chance / 100;

            //If we hit the chance for actually generating a daily quest do so, otherwise don't generate a daily quest.
            if (actualChance <= Config.chanceForDailyQuest)
            {
                Random r = new Random((int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed);
                int rand = r.Next(0, 7);

                if (rand == 0) return new ItemDeliveryQuest();
                if (rand == 1) return new FishingQuest();
                if (rand == 2) return new StardewValley.Quests.CraftingQuest();
                if (rand == 3) return new StardewValley.Quests.ItemDeliveryQuest();
                if (rand == 4) return new StardewValley.Quests.ItemHarvestQuest();
                if (rand == 5) return new StardewValley.Quests.ResourceCollectionQuest();
                if (rand == 6) return new StardewValley.Quests.SlayMonsterQuest();
            }
            return null; //This should never happen.
        }
    }
}
