using System;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Quests;
using StardewValley.Locations;

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

        private ModConfig _config;
        private float _fishPercent;
        private float _resourcePercent;
        private float _itemPercent;
        private float _slayPercent;
        private float _totality;
        private float _noQuestPercent;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            _config = Helper.ReadConfig<ModConfig>();
            _noQuestPercent = (float)Math.Max(0,100 - _config.ChanceOfQuestPerDay)/100;

            _totality = _config.CategoryChancePercent.ItemDelivery + _config.CategoryChancePercent.Gathering + _config.CategoryChancePercent.Fishing + _config.CategoryChancePercent.SlayMonsters;
            _fishPercent = (1 - _noQuestPercent) * _config.CategoryChancePercent.Fishing / _totality;
            _resourcePercent = (1 - _noQuestPercent) * _config.CategoryChancePercent.Gathering / _totality;
            _itemPercent = (1 - _noQuestPercent) * _config.CategoryChancePercent.ItemDelivery / _totality;
            _slayPercent = (1 - _noQuestPercent) * _config.CategoryChancePercent.SlayMonsters / _totality;
            
            //Events
            helper.Events.GameLoop.DayStarted += Time_AfterDayStarted;
        }

        private void Time_AfterDayStarted(object sender, EventArgs e)
        {
            //Quest questy = null;
            Monitor.Log("Daily Help Wanted quest generated.");
            double num = new Random((int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed + Game1.timeOfDay).NextDouble();
            if (Game1.stats.DaysPlayed <= 1U)
            {
                Game1.questOfTheDay = null;
            }
            else if (num >= 1 - _itemPercent)
            {
                var itemy = new ItemDeliveryQuest();
                itemy.loadQuestInfo();
                Game1.questOfTheDay = itemy;
            }
            else if (num >= 1 - _itemPercent - _fishPercent)
            {
                FishingQuest fishy = new FishingQuest();
                fishy.loadQuestInfo();
                Game1.questOfTheDay = fishy;
            }
            else if (num >= 1 - _itemPercent - _fishPercent - _noQuestPercent || MineShaft.lowestLevelReached <= 0 || Game1.stats.DaysPlayed <= 5U)
            {
                Game1.questOfTheDay = null;
            }
            else if (num >= 1 - _itemPercent - _fishPercent - _noQuestPercent - _slayPercent)
            {
                SlayMonsterQuest slayey = new SlayMonsterQuest();
                slayey.loadQuestInfo();
                Game1.questOfTheDay = slayey;
            }
            else
            {
                ResourceCollectionQuest sourcy = new ResourceCollectionQuest();
                sourcy.loadQuestInfo();
                Game1.questOfTheDay = sourcy;
            }
        }


    }

    
}
