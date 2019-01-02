using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Events;
using StardewValley.Locations;
using System.Linq;
using StardewModdingAPI.Utilities;
using Microsoft.Xna.Framework;
using SObject = StardewValley.Object;
using StardewValley.Objects;
using TwilightShards.Common;

namespace CustomizableCartRedux
{
    public class CustomizableCartRedux : Mod
    {
        public static Mod instance;
        public CartConfig OurConfig;
        public MersenneTwister Dice;
        private Dictionary<Item, int[]> generatedStock;

        private ICustomizableCart API;

        public override object GetApi()
        {
            if (API == null)
                API = new CustomizableCartAPI(Helper.Reflection);

            return API;
        }

        public override void Entry(IModHelper helper)
        {
            instance = this;
            Dice = new MersenneTwister();
            OurConfig = helper.ReadConfig<CartConfig>();

            helper.Events.GameLoop.DayStarted += SetCartSpawn;
            helper.Events.Player.Warped += LocationMoved;
        }

        private void LocationMoved(object sender, WarpedEventArgs e)
        {
            if (!Context.IsMainPlayer)
                return;

            if (e.NewLocation is Forest)
            {
                Forest f = e.NewLocation as Forest;
                Helper.Reflection.GetField<Dictionary<Item, int[]>>(f, "travelerStock").SetValue(generatedStock);
            }
        }

        private void SetCartSpawn(object Sender, EventArgs e)
        {
            if (!Context.IsMainPlayer)
                return;

            Random r = new Random();
            double randChance = r.NextDouble(), dayChance = 0;

            if (!(Game1.getLocationFromName("Forest") is Forest f))
                throw new Exception("The Forest is not loaded. Please verify your game is properly installed.");
            
            //generate the stock
            generatedStock = GetTravelingMerchantStock(OurConfig.AmountOfItems);

            //get the day
            DayOfWeek day = GetDayOfWeek(SDate.Now());
            switch (day)
            {
                case DayOfWeek.Monday:
                    dayChance = OurConfig.MondayChance;
                    break;
                case DayOfWeek.Tuesday:
                    dayChance = OurConfig.TuesdayChance;
                    break;
                case DayOfWeek.Wednesday:
                    dayChance = OurConfig.WednesdayChance;
                    break;
                case DayOfWeek.Thursday:
                    dayChance = OurConfig.ThursdayChance;
                    break;
                case DayOfWeek.Friday:
                    dayChance = OurConfig.FridayChance;
                    break;
                case DayOfWeek.Saturday:
                    dayChance = OurConfig.SaturdayChance;
                    break;
                case DayOfWeek.Sunday:
                    dayChance = OurConfig.SundayChance;
                    break;
                default:
                    dayChance = 0;
                    break;
            }

            /* Start of the Season - Day 1. End of the Season - Day 28. Both is obviously day 1 and 28 
               Every other week is only on days 8-14 and 22-28) */

            bool setCartToOn = false;
            if (OurConfig.AppearOnlyAtEndOfSeason)
            {
                if (Game1.dayOfMonth == 28)
                    setCartToOn = true;
            }

            else if (OurConfig.AppearOnlyAtStartAndEndOfSeason)
            {
                if (Game1.dayOfMonth == 28 || Game1.dayOfMonth == 1)
                    setCartToOn = true;
            }

            else if (OurConfig.AppearOnlyAtStartAndEndOfSeason)
            {
                if (Game1.dayOfMonth == 28 || Game1.dayOfMonth == 1)
                    setCartToOn = true;
            }

            else if (OurConfig.AppearOnlyAtStartOfSeason)
            {
                if (Game1.dayOfMonth == 1)
                    setCartToOn = true;
            }

            else if (OurConfig.AppearOnlyEveryOtherWeek)
            {
                if ((Game1.dayOfMonth >= 8 && Game1.dayOfMonth <= 14) || (Game1.dayOfMonth >= 22 && Game1.dayOfMonth <= 28))
                {
                    if (dayChance > randChance)
                    {
                        setCartToOn = true;
                    }
                }
            }

            else
            {
                if (dayChance > randChance)
                {
                    setCartToOn = true;
                }
            }

            if (setCartToOn)
            {
                f.travelingMerchantDay = true;
                f.travelingMerchantBounds.Add(new Rectangle(1472, 640, 492, 112));
                f.travelingMerchantBounds.Add(new Rectangle(1652, 744, 76, 48));
                f.travelingMerchantBounds.Add(new Rectangle(1812, 744, 104, 48));            

                Helper.Reflection.GetField<Dictionary<Item, int[]>>(f, "travelerStock").SetValue(generatedStock);
                foreach (Rectangle travelingMerchantBound in f.travelingMerchantBounds)
                {
                    Utility.clearObjectsInArea(travelingMerchantBound, f);                 
                }

                ((CustomizableCartAPI)API).InvokeCartProcessingComplete();
            }
            else
            {
                //clear other values
                f.travelingMerchantBounds.Clear();
                f.travelingMerchantDay = false;
                Helper.Reflection.GetField<Dictionary<Item, int[]>>(f, "travelerStock").SetValue(null);                
            }
        }

        private Dictionary<Item, int[]> GetTravelingMerchantStock(int numStock)
        {
            Dictionary<Item, int[]> dictionary = new Dictionary<Item, int[]>();
            int maxItemID = 0;

            maxItemID = OurConfig.UseVanillaMax ? 803 : Game1.objectInformation.Keys.Max();

            numStock = (numStock <= 3 ? 4 : numStock); //Ensure the stock isn't too low.
            var itemsToBeAdded = new List<int>();

            //get items
            for (int i = 0; i < (numStock - 3); i++)
            {
                int index2 = GetItem(maxItemID);

                while (!CanSellItem(index2))
                    index2 = GetItem(maxItemID);

                if (OurConfig.DisableDuplicates)
                {
                    while (itemsToBeAdded.Contains(index2) || !CanSellItem(index2))
                    {
                        index2 = GetItem(maxItemID);
                    }
                }

                itemsToBeAdded.Add(index2);
            }

            //assign price
            foreach(int i in itemsToBeAdded)
            {
                string[] strArray = Game1.objectInformation[i].Split('/');
                dictionary.Add(new SObject(i, 1), new int[2]
                {
                    (OurConfig.UseCheaperPricing ? (int)Math.Max(Dice.Next(1,6) * 81, Math.Round(Dice.RollInRange(1.87,5.95) * Convert.ToInt32(strArray[1])))
                        : Math.Max(Dice.Next(1, 11) * 100, Convert.ToInt32(strArray[1]) * Dice.Next(3, 6))),
                    Dice.NextDouble() < 0.1 ? 5 : 1
                });
            }

            //hardcoded item add.
            dictionary.Add(GetRandomFurniture(null, 0, 1613), new int[2]
            {
                Dice.Next(1, 11) * 250,
                1
            });

            // if it's less than fall, add a rare seed
            if (Utility.getSeasonNumber(Game1.currentSeason) < 2)
            {
                dictionary.Add(new SObject(347, 1), new int[2]
                {
                    1000, Dice.NextDouble() < 0.1 ? 5 : 1
                });

            }
            else if (Dice.NextDouble() < 0.4)
            {
                dictionary.Add(new SObject(Vector2.Zero, 136), new int[2]
                {
                    4000, 1
                });
            }

            dictionary.Add(key: Dice.NextDouble() < 0.25 ? new SObject(433, 1) : new SObject(578, 1), value: new int[2]
            {
                1000, 1
            });

            if (Context.IsMultiplayer && !Game1.player.craftingRecipes.ContainsKey("Wedding Ring"))
            {
                if (!dictionary.ContainsKey(new SObject(801, 1, true)))
                    dictionary.Add(key: new SObject(801, 1, true), value: new[] 
                    {
                        500,
                        1
                    });
            }

            return dictionary;
        }

        private int GetItem(int maxItemID)
        {
            string[] strArray;
            int index2 = Dice.Next(2, maxItemID);
            do
            {
                do //find the nearest one if it doesn't exist
                {
                    index2 = (index2 + 1) % maxItemID;
                }
                while (!Game1.objectInformation.ContainsKey(index2) || Utility.isObjectOffLimitsForSale(index2));

                strArray = Game1.objectInformation[index2].Split('/');
            }
            while (BannedItemsByCondition(index2, strArray));

            return index2;
        }

        private bool CanSellItem(int item)
        {
            bool Allowed = true;

            List<int> RestrictedItems = new List<int>() { 680, 681, 682, 688, 689, 690, 774, 775, 454, 460, 645, 413, 437, 439, 158, 159, 160, 161, 162, 163, 326, 341, 795, 796 };

            if (RestrictedItems.Contains(item))
                Allowed = false;

            if (OurConfig.AllowedItems.Contains(item))
                Allowed = true;

            if (OurConfig.BlacklistedItems.Contains(item))
                Allowed = false;

            return Allowed;
        }

        private bool BannedItemsByCondition(int item, string[] strArray)
        {
            bool categoryBanned =
                (!strArray[3].Contains('-') || 
                 Convert.ToInt32(strArray[1]) <= 0 || 
                 (strArray[3].Contains("-13") || strArray[3].Equals("Quest")) || 
                 (strArray[0].Equals("Weeds") || strArray[3].Contains("Minerals") || strArray[3].Contains("Arch")));

            if (OurConfig.AllowedItems.Contains(item))
                categoryBanned = false;

            return categoryBanned;
        }

        private Furniture GetRandomFurniture(List<Item> stock, int lowerIndexBound = 0, int upperIndexBound = 1462)
        {
            Dictionary<int, string> dictionary = Game1.content.Load<Dictionary<int, string>>("Data\\Furniture");
            int num;
            do
            {
                num = Dice.Next(lowerIndexBound, upperIndexBound);
                if (stock != null)
                {
                    foreach (Item obj in stock)
                    {
                        if (obj is Furniture && obj.ParentSheetIndex == num)
                            num = -1;
                    }
                }
            }
            while (IsFurnitureOffLimitsForSale(num) || !dictionary.ContainsKey(num));
            Furniture furniture = new Furniture(num, Vector2.Zero);
            int maxValue = int.MaxValue;
            furniture.Stack = maxValue;
            return furniture;
        }

        private static bool IsFurnitureOffLimitsForSale(int index)
        {
            switch (index)
            {
                case 1680:
                case 1733:
                case 1669:
                case 1671:
                case 1541:
                case 1545:
                case 1554:
                case 1402:
                case 1466:
                case 1468:
                case 131:
                case 1226:
                case 1298:
                case 1299:
                case 1300:
                case 1301:
                case 1302:
                case 1303:
                case 1304:
                case 1305:
                case 1306:
                case 1307:
                case 1308:
                    return true;
                default:
                    return false;
            }
        }

        private DayOfWeek GetDayOfWeek(SDate Target)
        {
            switch (Target.Day % 7)
            {
                case 0:
                    return DayOfWeek.Sunday;
                case 1:
                    return DayOfWeek.Monday;
                case 2:
                    return DayOfWeek.Tuesday;
                case 3:
                    return DayOfWeek.Wednesday;
                case 4:
                    return DayOfWeek.Thursday;
                case 5:
                    return DayOfWeek.Friday;
                case 6:
                    return DayOfWeek.Saturday;
                default:
                    return 0;
            }
        }
    }
}   
