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
using StardewValley.Menus;

namespace WhatAreYouMissing
{
    public struct ConfigOptions
    {
        public SButton Button { get; }
        public bool ShowItemsFromLockedPlaces { get; }
        public bool ShowAllFishFromCurrentSeason { get; }
        public bool ShowAllRecipes { get; }
        public bool AlwaysShowAllRecipes { get; }
        public int CommonQualityAmount { get; }
        public int HighestQualityAmount { get; }

        public ConfigOptions(SButton button, bool showLockedItems, bool showAllFishFromCurrentSeason, 
                                bool showAllRecipes, bool alwaysShowAllRecipes, int commonAmount, int highestQualityAmount)
        {
            Button = button;
            ShowItemsFromLockedPlaces = showLockedItems;
            ShowAllFishFromCurrentSeason = showAllFishFromCurrentSeason;
            ShowAllRecipes = showAllRecipes;
            AlwaysShowAllRecipes = alwaysShowAllRecipes;
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

        public static void DrawHoverTextBox(SpriteBatch b, string text, int spaceBetweenLines)
        {
            string[] lines = text.Split('\n');
            int lineHeight = (int)Game1.smallFont.MeasureString("ABC").Y;
            Vector2 position = new Vector2(Game1.getOldMouseX() + 32, Game1.getOldMouseY() + 32);
            Vector2 boxDimensions = new Vector2(Game1.smallFont.MeasureString(text).X, lines.Length * lineHeight);

            boxDimensions.Y += 32 + (lines.Length - 1) * spaceBetweenLines + 4;
            boxDimensions.X += 32;

            if(IsGoingOutOfXView((int)position.X, (int)boxDimensions.X))
            {
                position.X = Game1.viewport.Width - boxDimensions.X;
            }
            if(IsGoingOutOfYView((int)position.Y, (int)boxDimensions.Y))
            {
                position.Y = Game1.getOldMouseY() - boxDimensions.Y;
            }

            IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), (int)position.X, (int)position.Y, (int)boxDimensions.X, (int)boxDimensions.Y, Color.White);

            position.X += 16;
            position.Y += 16 + 4;
            for (int i = 0; i < lines.Length; ++i)
            {
                b.DrawString(Game1.smallFont, lines[i], position, Game1.textColor);
                position.Y += Game1.smallFont.MeasureString(lines[i]).Y + spaceBetweenLines;
            }
        }

        private static bool IsGoingOutOfXView(int x, int width)
        {
            return x + width > Game1.viewport.Width;
        }

        private static bool IsGoingOutOfYView(int y, int height)
        {
            return y + height > Game1.viewport.Height;
        }

        public static string GetTranslation(string key)
        {
            return ModEntry.Translator.Get(key);
        }
    }
}
