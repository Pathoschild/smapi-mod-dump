using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Locations;
using StardewModdingAPI;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WhatAreYouMissing
{
    public struct ConfigOptions
    {
        public SButton Button { get; }
        public bool ShowItemsFromLockedPlaces { get; }
        public bool ShowAllFishFromCurrentSeason { get; }
        public bool ShowAllRecipes { get; }
        public bool AlwasyShowAllRecipes { get; }
        public int CommonQualityAmount { get; }
        public int HighestQualityAmount { get; }

        public ConfigOptions(SButton button, bool showLockedItems, bool showAllFishFromCurrentSeason, 
                                bool showAllRecipes, bool alwaysShowAllRecipes, int commonAmount, int highestQualityAmount)
        {
            Button = button;
            ShowItemsFromLockedPlaces = showLockedItems;
            ShowAllFishFromCurrentSeason = showAllFishFromCurrentSeason;
            ShowAllRecipes = showAllRecipes;
            AlwasyShowAllRecipes = alwaysShowAllRecipes;
            CommonQualityAmount = commonAmount;
            HighestQualityAmount = highestQualityAmount;
        }
    };

    public class Utilities
    {
        public static bool IsDesertUnlocked()
        {
            return Game1.player.mailReceived.Contains("ccVault");
        }

        public static bool IsGreenHouseUnlocked()
        {
            return Game1.player.mailReceived.Contains("ccPantry");
        }

        public static bool IsSecretWoodsUnlocked()
        {
            //Not 100% confident in this check
            return Game1.player.mailReceived.Contains("beenToWoods");
        }

        public static bool IsSecondBeachUnlocked()
        {
            return new Beach().bridgeFixed.Value;
        }

        public static bool IsMushroomCaveUnlocked()
        {
            return Game1.player.caveChoice.Value == 2;
        }

        public static bool IsBatCaveUnlocked()
        {
            return Game1.player.caveChoice.Value == 1;
        }

        public static bool CheckMerchantForItemAndSeed(int item)
        {
            CropConversion cropConverter = new CropConversion();
            int seedIndex = cropConverter.CropToSeedIndex(item);
            if (IsMerchantAvailiableAndHasItem(item) || (IsMerchantAvailiableAndHasItem(seedIndex) && IsThereEnoughTimeToGrowSeeds(seedIndex)))
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// Checks if there is enough time left in the month to grow the seeds
        /// or if the greenhouse is unlocked (then they can grow it for sure)
        /// </summary>
        /// <param name="seedIndex"></param>
        /// <returns></returns>
        public static bool IsThereEnoughTimeToGrowSeeds(int seedIndex)
        {
            Dictionary<int, string> data = Game1.content.Load<Dictionary<int, string>>("Data\\Crops");
            if (data.ContainsKey(seedIndex))
            {
                int totalDaysNeeded = GetTotalDaysToGrowNewSeed(seedIndex);
                string[] growthSeasons = new string[data[seedIndex].Split('/')[1].Split(' ').Length];

                if (IsGreenHouseUnlocked())
                {
                    return true;
                }
                else if(Game1.Date.Season == growthSeasons[growthSeasons.Length - 1] && Game1.Date.DayOfMonth + totalDaysNeeded < 29)
                {
                    //Its last growth season, make sure there is enough time
                    return true;
                }
                else if (growthSeasons.Contains(Game1.Date.Season))
                {
                    return true;
                }
            }
            return false;
        }

        private static int GetTotalDaysToGrowNewSeed(int seedIndex)
        {
            int[] daysInEachStage = GetDaysInEachGrowthStage(seedIndex);
            int totalDays = 0;
            foreach (int numDays in daysInEachStage)
            {
                totalDays += numDays;
            }

            return totalDays;
        }

        private static int[] GetDaysInEachGrowthStage(int seedIndex)
        {
            Dictionary<int, string> data = Game1.content.Load<Dictionary<int, string>>("Data\\Crops");
            int[] daysInEachStage = new int[data[seedIndex].Split('/')[0].Split(' ').Length];

            for (int i = 0; i < daysInEachStage.Length; ++i)
            {
                daysInEachStage[i] = Convert.ToInt32(data[seedIndex].Split('/')[0].Split(' ')[i]);
            }

            return daysInEachStage;
        }

        public static bool IsMerchantAvailiableAndHasItem(int parentSheetIndex)
        {
            if (IsMerchantAvailiable())
            {
                return IsInMerchantInventory(parentSheetIndex);
            }
            return false;
        }

        private static bool IsInMerchantInventory(int parentSheetIndex)
        {
            //this is how the source code generates the seed
            int seed = (int)((long)Game1.uniqueIDForThisGame + (long)Game1.stats.DaysPlayed);

            Dictionary<Item, int[]> stock = Utility.getTravelingMerchantStock(seed);
            foreach (Item item in stock.Keys)
            {
                if (item.ParentSheetIndex == parentSheetIndex)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsMerchantAvailiable()
        {
            return Game1.Date.DayOfWeek == DayOfWeek.Friday || Game1.Date.DayOfWeek == DayOfWeek.Sunday;
        }

        public static void DrawSilverStar(SpriteBatch b, Vector2 coords, float scale)
        {
            Rectangle? spriteInMouseCursors = new Rectangle?(new Rectangle(338, 400, 8, 8));
            DrawStar(b, coords, scale, spriteInMouseCursors);
        }

        public static void DrawGoldStar(SpriteBatch b, Vector2 coords, float scale)
        {
            Rectangle? spriteInMouseCursors = new Rectangle?(new Rectangle(346, 400, 8, 8));
            DrawStar(b, coords, scale, spriteInMouseCursors);
        }

        public static void DrawIridiumStar(SpriteBatch b, Vector2 coords, float scale)
        {
            Rectangle? spriteInMouseCursors = new Rectangle?(new Rectangle(346, 392, 8, 8));
            DrawStar(b, coords, scale, spriteInMouseCursors);
        }

        private static void DrawStar(SpriteBatch b, Vector2 coords, float scale, Rectangle? starSprite)
        {
            float rotation = 0f;
            Vector2 originToDrawRelativeTo = new Vector2(4f, 4f);
            float layerDepth = 1f;

            b.Draw(Game1.mouseCursors, coords, starSprite, Color.White, rotation,
                    originToDrawRelativeTo, scale, SpriteEffects.None, layerDepth);
        }
    }
}
