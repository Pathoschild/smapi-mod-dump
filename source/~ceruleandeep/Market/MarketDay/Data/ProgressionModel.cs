/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using MarketDay.Utility;
using StardewModdingAPI;
using StardewValley;

namespace MarketDay.Data
{
    public class PrizeLevel
    {
        public string Name { get; set; }
        public int Gold { get; set; }
        public int GoldForDifficulty => (int)(Gold * Game1.MasterPlayer.difficultyModifier);
        public int Score { get; set; }
        public string Object { get; set; }
        public string Flavor { get; set; }
        public int Quality { get; set; } = 0;
        public int Stack { get; set; } = 3;
    }
    
    public class ProgressionLevel
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public int NumberOfShops { get; set; }
        public int NumberOfTownieVisitors { get; set; }
        public int NumberOfRandomVisitors { get; set; }
        public int UnlockAtEarnings { get; set; }
        public int UnlockAtEarningsForDifficulty => (int)(UnlockAtEarnings * Game1.MasterPlayer.difficultyModifier);
        public int AutoRestock { get; set; } = 4;

        public int ShopSize { get; set; } = 9;

        public double SellPriceMultiplierLimit { get; set; } = 1;
        
        public List<PrizeLevel> Prizes { get; set; }

        public PrizeLevel PrizeForEarnings(int gold)
        {
            var eligiblePrizes = Prizes.Where(p => p.Score == 0 && p.GoldForDifficulty <= gold).OrderBy(p => p.GoldForDifficulty);
            return eligiblePrizes.Any() ? eligiblePrizes.Last() : null;
        }
        
    }
    
    public class ProgressionModel
    {
        public List<ProgressionLevel> Levels { get; set; }

        internal ProgressionLevel CurrentLevel
        {
            get
            {
                ProgressionLevel highestUnlocked = null;
                var gold = MarketDay.GetSharedValue(MarketDay.TotalGoldKey);
                foreach (var level in Levels.Where(level => level.UnlockAtEarningsForDifficulty <= gold)) highestUnlocked = level;
                return highestUnlocked;
            }
        }
        
        internal ProgressionLevel NextLevel
        {
            get
            {
                var gold = MarketDay.GetSharedValue(MarketDay.TotalGoldKey);
                return Levels.OrderBy(l => l.UnlockAtEarnings).FirstOrDefault(level => level.UnlockAtEarningsForDifficulty > gold);
            }
        }

        internal int AutoRestock =>
            Math.Max(0, 
                MarketDay.Config.Progression 
                ? CurrentLevel.AutoRestock
                : MarketDay.Config.RestockItemsPerHour
                );

        internal int ShopSize =>
            Math.Max(1, Math.Min(9, 
                MarketDay.Config.Progression 
                ? CurrentLevel.ShopSize
                : 9
                ));
        
        /// <summary>
        /// Number of shops to open, accounting for challenge mode level
        /// and free play mode configuration
        /// </summary>
        internal int NumberOfShops
        {
            get
            {
                var farmhands = Game1.getAllFarmers().Count(f => f.isActive()) - 1; 
                return Math.Max(1, Math.Min(15,
                    MarketDay.Config.Progression
                        ? CurrentLevel.NumberOfShops + farmhands
                        : MarketDay.Config.NumberOfShops
                ));
            }
        }

        internal int NumberOfTownieVisitors =>
            Math.Max(1, Math.Min(100, 
                MarketDay.Config.Progression 
                    ? CurrentLevel.NumberOfTownieVisitors
                    : MarketDay.Config.NumberOfTownieVisitors
            ));

        internal int NumberOfRandomVisitors =>
            Math.Max(1, Math.Min(24, 
                MarketDay.Config.Progression 
                    ? CurrentLevel.NumberOfRandomVisitors
                    : MarketDay.Config.NumberOfRandomVisitors
            ));
        
        internal double SellPriceMultiplierLimit =>
            Math.Max(1, Math.Min(4, 
                MarketDay.Config.Progression 
                ? CurrentLevel.SellPriceMultiplierLimit
                : 10
            ));

        internal int WeeklyGoldTarget => CurrentLevel?.Prizes.Where(p => p.Score==0).OrderBy(p => p.GoldForDifficulty).First().GoldForDifficulty ?? 0;

        internal void CheckItems()
        {
            MarketDay.Log("Checking progression data", LogLevel.Debug);
            if (Levels.Count == 0) MarketDay.Log($"    No levels loaded", LogLevel.Error);
            foreach (var level in Levels)
            {
                foreach (var prizeLevel in level.Prizes)
                {
                    var name = prizeLevel.Object;

                    var item = ItemsUtil.GetIndexByName(name);
                    if (item == -1) MarketDay.Log($"    Could not get index for object: {name}", LogLevel.Warn);
                    
                    if (name is "Wine" or "Jelly" or "Juice" or "Pickle" or "Roe" or "Aged Roe")
                    {
                        var preservedGoods = prizeLevel.Flavor;
                        var item1 = ItemsUtil.GetIndexByName(preservedGoods);
                        if (item1 == -1) MarketDay.Log($"    Could not get index for flavor: {preservedGoods}", LogLevel.Warn);
                    }
                }
            }
        }
    }
}