using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewModdingAPI.Framework;
using StardewValley;
using StardewValley.Quests;
using StardewValley.Locations;
using System.Reflection;

namespace HelpWantedQuestFixes
{
    class ModConfig
    {
        public int ChanceOfQuestPerDay { get; set; } = 65;

        public QuestCategoryChance CategoryChancePercent { get; set; } = new QuestCategoryChance();

        internal class QuestCategoryChance
        {
            public int ItemDelivery { get; set; } = 62;
            public int Gathering { get; set; } = 11;
            public int Fishing { get; set; } = 15;
            public int SlayMonsters { get; set; } = 12;
        }

    }

    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {

        private ModConfig Config;
        private float fishPercent;
        private float resourcePercent;
        private float itemPercent;
        private float slayPercent;
        private float totality;
        private float noQuestPercent;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            noQuestPercent = Math.Max(0,(100 - this.Config.ChanceOfQuestPerDay))/100;

            totality = this.Config.CategoryChancePercent.ItemDelivery + this.Config.CategoryChancePercent.Gathering + this.Config.CategoryChancePercent.Fishing + this.Config.CategoryChancePercent.SlayMonsters;
            fishPercent = (1 - noQuestPercent) * this.Config.CategoryChancePercent.Fishing / totality;
            resourcePercent = (1 - noQuestPercent) * this.Config.CategoryChancePercent.Gathering / totality;
            itemPercent = (1 - noQuestPercent) * this.Config.CategoryChancePercent.ItemDelivery / totality;
            slayPercent = (1 - noQuestPercent) * this.Config.CategoryChancePercent.SlayMonsters / totality;
           
            TimeEvents.AfterDayStarted += Time_AfterDayStarted;
        }

        private void Time_AfterDayStarted(object sender, EventArgs e)
        {
            Quest questy = (Quest)null;
            ItemDeliveryQuest itemy = (ItemDeliveryQuest)null;
            FishingQuest fishy = (FishingQuest)null;
            SlayMonsterQuest slayey = (SlayMonsterQuest)null;
            ResourceCollectionQuest sourcy = (ResourceCollectionQuest)null;


            this.Monitor.Log($"Daily Help Wanted quest generated.");
            double num = new Random((int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed + (int)Game1.timeOfDay).NextDouble();
            if (Game1.stats.DaysPlayed <= 1U)
            {
                Game1.questOfTheDay = questy;
            }
            else if (num >= 1 - this.itemPercent)
            {
                itemy = new ItemDeliveryQuest();
                itemy.loadQuestInfo();
                Game1.questOfTheDay = itemy;
            }
            else if (num >= 1 - this.itemPercent - this.fishPercent)
            {
                fishy = new FishingQuest();
                fishy.loadQuestInfo();
                Game1.questOfTheDay = fishy;
            }
            else if (num >= 1 - this.itemPercent - this.fishPercent - this.noQuestPercent || MineShaft.lowestLevelReached <= 0 || Game1.stats.DaysPlayed <= 5U)
            {
                Game1.questOfTheDay = (Quest)null;
            }
            else if (num >= 1 - this.itemPercent - this.fishPercent - this.noQuestPercent - this.slayPercent)
            {
                slayey = new SlayMonsterQuest();
                slayey.loadQuestInfo();
                Game1.questOfTheDay = slayey;
            }
            else
            {
                sourcy = new ResourceCollectionQuest();
                sourcy.loadQuestInfo();
                Game1.questOfTheDay = sourcy;
            }
        }


    }

    
}
