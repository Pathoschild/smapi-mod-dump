/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheMightyAmondee/Shoplifter
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewValley;
using System.Linq;
using StardewValley.GameData.Shops;
using StardewValley.Internal;

namespace Shoplifter
{	
	public class ShopStock
	{
		private readonly static List<Item> BasicStock = new List<Item>();
        private readonly static List<Item> RareStock = new List<Item>();

        /// <summary>
        /// Generates a random list of stock for the given shop
        /// </summary>
        /// <param name="maxstock">The maximum number of different stock items to generate</param>
        /// <param name="maxquantity">The maximum quantity of each stock</param>
        /// <param name="which">The shop to generate stock for</param>
        /// <returns>The generated stock list</returns>
        public static Dictionary<ISalable, ItemStockInformation> generateRandomStock(int maxstock, int maxquantity, string which, float rarestockchance)
		{
            //if (!System.Diagnostics.Debugger.IsAttached) { System.Diagnostics.Debugger.Launch(); }
            Dictionary<ISalable, ItemStockInformation> stock = new Dictionary<ISalable, ItemStockInformation>();
			Random random = new Random((int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.DaysPlayed + ModEntry.PerScreenShopliftCounter.Value);
			int stocklimit = random.Next(1, maxstock + 1);
            var shopstock = ShopBuilder.GetShopStock(which);
            var addrarestockchance = random.NextDouble();

            if (shopstock != null)
            {
                foreach (var stockinfo in shopstock)
                {
                    var stockobject = stockinfo.Key as StardewValley.Object;

                    if (stockobject == null || stockobject.QualifiedItemId.StartsWith("(O)") == false || stockobject.IsRecipe == true || stockobject.bigCraftable.Value == true)
                    {
                        var stockitem = stockinfo.Key as Item;

                        if (ModEntry.IDGAItem?.GetDGAItemId(stockitem) != null)
                        {
                            var id = ModEntry.IDGAItem?.SpawnDGAItem(ModEntry.IDGAItem.GetDGAItemId(stockitem)) as ISalable as Item ?? stockitem;

                            RareStock.Add(id);
                        }
                        else
                        {
                            RareStock.Add(stockitem);
                        }
                        
                        continue;
                    }

                    // Add object to array
                    if (stockobject != null)
                    {
                        if (stockobject.Category < -94)
                        {
                            RareStock.Add(stockobject);
                            continue;
                        }

                        if (ModEntry.IDGAItem?.GetDGAItemId(stockobject) != null)
                        {
                            var id = ModEntry.IDGAItem?.SpawnDGAItem(ModEntry.IDGAItem.GetDGAItemId(stockobject)) as ISalable as Item ?? stockobject;

                            BasicStock.Add(id);
                        }

                        else
                        {
                            BasicStock.Add(stockobject);
                        }
                    }
                }
            }           

            if (BasicStock.Count == 0)
            {
                switch (which)
                {
                    case "SeedShop":
                        // Grass starter if nothing available
                        BasicStock.Add(new StardewValley.Object("297", 1));
                        break;
                    case "FishShop":
                        // Trout soup if nothing available
                        BasicStock.Add(new StardewValley.Object("219", 1));
                        break;
                    case "Carpenter":
                        // Wood if nothing available
                        BasicStock.Add(new StardewValley.Object("388", 1));
                        break;
                    case "Hospital":
                        // Muscle Remedy if nothing available
                        BasicStock.Add(new StardewValley.Object("351", 1));
                        break;
                    case "AnimalShop":
                        // Hay if nothing available
                        BasicStock.Add(new StardewValley.Object("178", 1));
                        break;
                    case "Saloon":
                        // Beer if nothing available
                        BasicStock.Add(new StardewValley.Object("346", 1));
                        break;
                    case "Blacksmith":
                        // Coal if nothing available
                        BasicStock.Add(new StardewValley.Object("382", 1));
                        break;
                    case "IceCreamStand":
                        // Icecream if nothing available
                        BasicStock.Add(new StardewValley.Object("233", 1));
                        break;
                    case "Sandy":
                        // Cactus seeds if nothing available
                        BasicStock.Add(new StardewValley.Object("802", 1));                       
                        break;
                        // Should all else fail, add banana sapling as stock (nice)
                    default:
                        BasicStock.Add(new StardewValley.Object("69", 1));
                        break;
                }
                
            }

            // Add generated stock to store from array
            for (int i = 0; i < stocklimit; i++)
			{
                int quantity = random.Next(1, maxquantity + 1);
				var itemindex = random.Next(0, BasicStock.Count);

                if (BasicStock.Count == 0)
                {
                    break;
                }
                stock.Add(BasicStock[itemindex], new ItemStockInformation(0, quantity, null, null, LimitedStockMode.None));
                BasicStock.RemoveAt(itemindex);
            }

            if (RareStock.Count > 0 && addrarestockchance <= rarestockchance)
            {
                var itemindex = random.Next(0, RareStock.Count);               

                stock.Add(RareStock[itemindex], new ItemStockInformation(0, 1, null, null, LimitedStockMode.None));
            }

			// Clear stock list
			BasicStock.Clear();
            RareStock.Clear();

			return stock;
		}
	}
}