/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sakorona/SDVMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using TwilightShards.Common;

namespace CustomizableTravelingCart.Patches
{
    class UtilityPatches
    {
        private static readonly int[] UnsellableFurniture = new int[] { 1952, 1953, 1954, 1955, 1956, 1957, 1958, 1959, 1960, 1961, 1971, 1902, 1907, 1909, 1914, 1915, 1916, 1917, 1918, 1796, 1798, 1800, 1802, 1838, 1840, 1842, 1844, 1846, 1848, 1850, 1852, 1854, 1900, 1554, 1669, 1671, 1733, 1760, 1761, 1762, 1763, 1764, 1471, 1541, 1545, 1371, 1373, 1375, 1226, 131, 989, 984, 985, 986, 1298, 1299, 1300, 1301, 1302, 1303, 1304, 1305, 1306, 1307, 1308, 1309,1402, 1466, 1468, 1680 };

        static bool getTravelingMerchantStockPrefix(int seed, ref Dictionary<ISalable, int[]> __result)
        {
            Dictionary<ISalable, int[]> travelingMerchantStock = generateLocalTravelingMerchantStock(seed);
            Game1.player.team.synchronizedShopStock.UpdateLocalStockWithSyncedQuanitities(SynchronizedShopStock.SynchedShop.TravelingMerchant, travelingMerchantStock, null);
            if (Game1.IsMultiplayer && !Game1.player.craftingRecipes.ContainsKey("Wedding Ring"))
            {
                StardewValley.Object @object = new StardewValley.Object(801, 1, true, -1, 0);
                travelingMerchantStock.Add(@object, new int[2]
                {
                    CustomizableCartRedux.OurConfig.UseCheaperPricing ? 450 : 500,
                    1
                });
            }
            __result = travelingMerchantStock;
            return false;
        }

        private static Dictionary<ISalable, int[]> generateLocalTravelingMerchantStock(int seed)
        {
            Random Dice = new Xoshiro.PRNG64.XoShiRo256starstar();
            Dictionary<ISalable, int[]> stock1 = new Dictionary<ISalable, int[]>();
            HashSet<int> stockIndices1 = new HashSet<int>();
            int numStock = (CustomizableCartRedux.OurConfig.AmountOfItems <= 4 ? 5 : CustomizableCartRedux.OurConfig.AmountOfItems);
            int maxItemID = CustomizableCartRedux.OurConfig.UseVanillaMax ? 790 :
                Game1.objectInformation.Keys.Max();
            var itemsToBeAdded = new List<int>();

            bool add_guaranteed_item = false;
            if (Game1.netWorldState.Value.VisitsUntilY1Guarantee == 0)
            {
                add_guaranteed_item = true;
            }

            for (int index1 = 0; index1 < (numStock - 3); ++index1)
            {
                Dictionary<ISalable, int[]> stock2;
                HashSet<int> stockIndices2;
                StardewValley.Object objectToAdd;
                int[] listing;
                do
                {
                    int index2 = GetItem(Dice, maxItemID);

                    while (!CanSellItem(index2))
                        index2 = GetItem(Dice, maxItemID);

                    while (itemsToBeAdded.Contains(index2) || !CanSellItem(index2))
                    {
                        index2 = GetItem(Dice, maxItemID);
                    }

                    if (index2 == 266 || index2 == 485)
                    {
                        add_guaranteed_item = false;
                    }

                    var strArray = Game1.objectInformation[index2].Split('/');

                    stock2 = stock1;
                    stockIndices2 = stockIndices1;
                    objectToAdd = new StardewValley.Object(index2, 1, false, -1, 0);
                    listing = new int[2]
                    {
                        (CustomizableCartRedux.OurConfig.UseCheaperPricing ? (int)Math.Max(Dice.Next(1,6) * 81, Math.Round(Dice.RollInRange(1.87,5.95) * Convert.ToInt32(strArray[1])))
                        : Math.Max(Dice.Next(1, 11) * 100, Convert.ToInt32(strArray[1]) * Dice.Next(3, 6))),
                    Dice.NextDouble() < 0.1 ? 5 : 1
                    };
                }
                while (!addToStock(stock2, stockIndices2, objectToAdd, listing));

                if (add_guaranteed_item)
                {
                    string[] split2 = Game1.objectInformation[485].Split('/');
                    addToStock(stock1, stockIndices1, new StardewValley.Object(485, 1), new int[2]
                    {
                    Math.Max(Dice.Next(1, 11) * 100, Convert.ToInt32(split2[1]) * Dice.Next(3, 6)),
                    (!(Dice.NextDouble() < 0.1)) ? 1 : 5
                    });
                }
            }
            addToStock(stock1, stockIndices1, getRandomFurniture(Dice, null, 0, 1613), new int[2]
            {
                CustomizableCartRedux.OurConfig.UseCheaperPricing ? (int)Math.Floor(Dice.RollInRange(.55,8.52) * 250.0) : Dice.Next(1, 11) * 250,
                1
            });
            if (Utility.getSeasonNumber(Game1.currentSeason) < 2)
                addToStock(stock1, stockIndices1, new StardewValley.Object(347, 1, false, -1, 0), new int[2]
                {
                    CustomizableCartRedux.OurConfig.UseCheaperPricing ? 800 : 1000,
                    Dice.NextDouble() < 0.1 ? 5 : 1
                });
            else if (Dice.NextDouble() < 0.4)
                addToStock(stock1, stockIndices1, new StardewValley.Object(Vector2.Zero, 136, false), new int[2]
                {
                    CustomizableCartRedux.OurConfig.UseCheaperPricing ? 3600 : 4000,
                    1
                });
            if (Dice.NextDouble() < 0.25)
                addToStock(stock1, stockIndices1, new StardewValley.Object(433, 1, false, -1, 0), new int[2]
                {
                    CustomizableCartRedux.OurConfig.UseCheaperPricing ? 2200 : 2500,
                    1
                });

            //add items from API call
            foreach (var v in CustomizableCartRedux.APIItemsToBeAdded)
            {
                addToStock(stock1, stockIndices1, v.Key, new int[2]{
                    v.Value[0],
                    v.Value[1]
                });
            }


            stockIndices1.Clear(); //clear this.
            return stock1;
        }

        private static bool CanSellItem(int item)
        {
            bool Allowed = true;

            List<int> RestrictedItems = new List<int>() { 79, 261, 277, 279, 305, 308, 680, 681, 682, 688, 689, 690, 774, 775, 454, 460, 645, 413, 437, 439, 158, 159, 160, 161, 162, 163, 326, 341, 447, 797, 798, 799, 800,801,802,803,807,812,808,809,810,811 };


            if (RestrictedItems.Contains(item))
                Allowed = false;

            if (CustomizableCartRedux.OurConfig.AllowedItems.Contains(item) && (item != 447 || item != 812))
                Allowed = true;

            if (CustomizableCartRedux.OurConfig.BlacklistedItems.Contains(item))
                Allowed = false;

            return Allowed;
        }

        private static bool BannedItemsByCondition(int item, string[] strArray)
        {
            bool categoryBanned =
                (!strArray[3].Contains('-') ||
                 Convert.ToInt32(strArray[1]) <= 0 ||
                 (strArray[3].Contains("-13") || strArray[3].Equals("Quest")) ||
                 (strArray[0].Equals("Weeds") || strArray[3].Contains("Minerals") || strArray[3].Contains("Arch")));

            if (CustomizableCartRedux.OurConfig.AllowedItems.Contains(item))
                categoryBanned = false;

            return categoryBanned;
        }

        private static int GetItem(Random r, int maxItemID)
        {
            string[] strArray;
            int index2 = r.Next(2, maxItemID);
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

        private static bool addToStock(Dictionary<ISalable, int[]> stock, HashSet<int> stockIndices, StardewValley.Object objectToAdd, int[] listing)
        {
            int parentSheetIndex = objectToAdd.ParentSheetIndex;
            if (stockIndices.Contains(parentSheetIndex))
                return false;
            stock.Add(objectToAdd, listing);
            stockIndices.Add(parentSheetIndex);
            return true;
        }

        private static Furniture getRandomFurniture(Random r,  Dictionary<ISalable, int[]> stock, int lowerIndexBound = 0, int upperIndexBound = 1462)
        {
            Dictionary<int, string> dictionary = Game1.content.Load<Dictionary<int, string>>("Data\\Furniture");
            int num;
            do
            {
                num = r.Next(lowerIndexBound, upperIndexBound);
                if (stock != null)
                {
                    foreach (Item key in stock.Keys)
                    {
                        if (key is Furniture && key.ParentSheetIndex == num)
                            num = -1;
                    }
                }
            }
            while (isFurnitureOffLimitsForSale(num) || !dictionary.ContainsKey(num));
            Furniture furniture = new Furniture(num, Vector2.Zero)
            {
                Stack = int.MaxValue
            };
            return furniture;
        }

        private static bool isFurnitureOffLimitsForSale(int index)
        {
            if (UnsellableFurniture.Contains(index))
                return true;

            return false;
        }
    }
}
