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
using StardewValley.Objects;
using System.Collections;
using System.Collections.Generic;
using StardewValley.Locations;
using StardewValley;

namespace Shoplifter
{	
	public class ShopStock
	{
		public static ArrayList CurrentStock = new ArrayList();

		// Method from Utilities so stock can be added to a shop (why is it private?)
		private static bool addToStock(Dictionary<ISalable, int[]> stock, HashSet<int> stockIndices, StardewValley.Object objectToAdd, int[] listing)
		{
			int index = objectToAdd.ParentSheetIndex;

			if (!stockIndices.Contains(index))
			{
				stock.Add(objectToAdd, listing);
				stockIndices.Add(index);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Generates a random list of stock for the given shop
		/// </summary>
		/// <param name="maxstock">The maximum number of different stock items to generate</param>
		/// <param name="maxquantity">The maximum quantity of each stock</param>
		/// <param name="which">The shop to generate stock for</param>
		/// <returns>The generated stock list</returns>
		public static Dictionary<ISalable, int[]> generateRandomStock(int maxstock, int maxquantity, string which)
		{
			GameLocation location = Game1.currentLocation;
			Dictionary<ISalable, int[]> stock = new Dictionary<ISalable, int[]>();
			HashSet<int> stockIndices = new HashSet<int>();
			Random random = new Random((int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.DaysPlayed);
			int stocklimit = random.Next(1, maxstock + 1);
			int index;

			switch (which)
            {
				// Pierre's shop
				case "SeedShop":
					foreach (var shopstock in (location as SeedShop).shopStock())
					{
						// Stops illegal stock being added, will result in an error item
						if ((shopstock.Key as StardewValley.Object) == null || (shopstock.Key as Wallpaper) != null || (shopstock.Key as Furniture) != null || (shopstock.Key as StardewValley.Object).bigCraftable == true || (shopstock.Key as StardewValley.Object).IsRecipe == true || (shopstock.Key as Clothing) != null || (shopstock.Key as Ring) != null || (shopstock.Key as Boots) != null || (shopstock.Key as Hat) != null)
						{
							continue;
						}

						// Add object id to array
						if ((shopstock.Key as StardewValley.Object) != null && (shopstock.Key as StardewValley.Object).bigCraftable == false)
						{
							index = (shopstock.Key as StardewValley.Object).parentSheetIndex;

							CurrentStock.Add(index);
						}
					}
					break;

				// Willy's shop
				case "FishShop":
					foreach (var shopstock in Utility.getFishShopStock(Game1.player))
					{

						// Stops illegal stock being added, will result in an error item
						if ((shopstock.Key as StardewValley.Object) == null || (shopstock.Key as Wallpaper) != null || (shopstock.Key as Furniture) != null || (shopstock.Key as StardewValley.Object).bigCraftable == true || (shopstock.Key as StardewValley.Object).IsRecipe == true || (shopstock.Key as Clothing) != null || (shopstock.Key as Ring) != null || (shopstock.Key as Boots) != null || (shopstock.Key as Hat) != null)
						{
							continue;
						}

						// Add object id to array
						if ((shopstock.Key as StardewValley.Object) != null && (shopstock.Key as StardewValley.Object).bigCraftable == false)
						{
							index = (shopstock.Key as StardewValley.Object).parentSheetIndex;

							CurrentStock.Add(index);
						}
					}
					break;

				// Robin's shop
				case "Carpenters":
					foreach (var shopstock in Utility.getCarpenterStock())
					{
						// Stops illegal stock being added, will result in an error item
						if ((shopstock.Key as StardewValley.Object) == null || (shopstock.Key as Wallpaper) != null || (shopstock.Key as Furniture) != null || (shopstock.Key as StardewValley.Object).bigCraftable == true || (shopstock.Key as StardewValley.Object).IsRecipe == true || (shopstock.Key as Clothing) != null || (shopstock.Key as Ring) != null || (shopstock.Key as Boots) != null || (shopstock.Key as Hat) != null)
						{
							continue;
						}

						// Add object id to array
						if ((shopstock.Key as StardewValley.Object) != null && (shopstock.Key as StardewValley.Object).bigCraftable == false)
						{
							index = (shopstock.Key as StardewValley.Object).parentSheetIndex;

							CurrentStock.Add(index);
						}
					}
					break;

				// Marnie's shop
				case "AnimalShop":
					foreach (var shopstock in Utility.getAnimalShopStock())
					{
						// Stops illegal stock being added, will result in an error item
						if ((shopstock.Key as StardewValley.Object) == null || (shopstock.Key as Wallpaper) != null || (shopstock.Key as Furniture) != null || (shopstock.Key as StardewValley.Object).bigCraftable == true || (shopstock.Key as StardewValley.Object).IsRecipe == true || (shopstock.Key as Clothing) != null || (shopstock.Key as Ring) != null || (shopstock.Key as Boots) != null || (shopstock.Key as Hat) != null)
						{
							continue;
						}

						// Add object id to array
						if ((shopstock.Key as StardewValley.Object) != null && (shopstock.Key as StardewValley.Object).bigCraftable == false && CurrentStock.Contains((shopstock.Key as StardewValley.Object).parentSheetIndex) == false)
						{
							index = (shopstock.Key as StardewValley.Object).parentSheetIndex;

							CurrentStock.Add(index);
						}
					}
					break;

				// Clint's shop
				case "Blacksmith":
					foreach (var shopstock in Utility.getBlacksmithStock())
					{

						// Stops illegal stock being added, will result in an error item
						if ((shopstock.Key as StardewValley.Object) == null || (shopstock.Key as Wallpaper) != null || (shopstock.Key as Furniture) != null || (shopstock.Key as StardewValley.Object).bigCraftable == true || (shopstock.Key as StardewValley.Object).IsRecipe == true || (shopstock.Key as Clothing) != null || (shopstock.Key as Ring) != null || (shopstock.Key as Boots) != null || (shopstock.Key as Hat) != null)
						{
							continue;
						}

						// Add object id to array
						if ((shopstock.Key as StardewValley.Object) != null && (shopstock.Key as StardewValley.Object).bigCraftable == false)
						{
							index = (shopstock.Key as StardewValley.Object).parentSheetIndex;

							CurrentStock.Add(index);
						}
					}
					break;

				// Gus' shop
				case "Saloon":
					foreach (var shopstock in Utility.getSaloonStock())
					{

						// Stops illegal stock being added, will result in an error item
						if ((shopstock.Key as StardewValley.Object) == null || (shopstock.Key as Wallpaper) != null || (shopstock.Key as Furniture) != null || (shopstock.Key as StardewValley.Object).bigCraftable == true || (shopstock.Key as StardewValley.Object).IsRecipe == true || (shopstock.Key as Clothing) != null || (shopstock.Key as Ring) != null || (shopstock.Key as Boots) != null || (shopstock.Key as Hat) != null)
						{
							continue;
						}

						// Add object id to array
						if ((shopstock.Key as StardewValley.Object) != null && (shopstock.Key as StardewValley.Object).bigCraftable == false)
						{
							index = (shopstock.Key as StardewValley.Object).parentSheetIndex;

							CurrentStock.Add(index);
						}
					}
					break;

				// Harvey's shop
				case "HospitalShop":
					foreach (var shopstock in Utility.getHospitalStock())
					{

						// Stops illegal stock being added, will result in an error item
						if ((shopstock.Key as StardewValley.Object) == null || (shopstock.Key as Wallpaper) != null || (shopstock.Key as Furniture) != null || (shopstock.Key as StardewValley.Object).bigCraftable == true || (shopstock.Key as StardewValley.Object).IsRecipe == true || (shopstock.Key as Clothing) != null || (shopstock.Key as Ring) != null || (shopstock.Key as Boots) != null || (shopstock.Key as Hat) != null)
						{
							continue;
						}

						// Add object id to array
						if ((shopstock.Key as StardewValley.Object) != null && (shopstock.Key as StardewValley.Object).bigCraftable == false)
						{
							index = (shopstock.Key as StardewValley.Object).parentSheetIndex;

							CurrentStock.Add(index);
						}
					}
					break;

				// Sandy's shop
				case "SandyShop":
					// Add object id to array
					CurrentStock.Add(802);
					CurrentStock.Add(478);
					CurrentStock.Add(486);
					CurrentStock.Add(494);

					switch (Game1.dayOfMonth % 7)
					{
						case 0:
							CurrentStock.Add(233);
							break;
						case 1:
							CurrentStock.Add(88);
							break;
						case 2:
							CurrentStock.Add(90);
							break;
						case 3:
							CurrentStock.Add(749);
							break;
						case 4:
							CurrentStock.Add(466);
							break;
						case 5:
							CurrentStock.Add(340);
							break;
						case 6:
							CurrentStock.Add(371);
							break;
					}
					break;
			}

			// Add generated stock to store from array
			for (int i = 0; i < stocklimit; i++)
			{
				int quantity = random.Next(1, maxquantity + 1);
				int item = random.Next(0, CurrentStock.Count);

				ShopStock.addToStock(stock, stockIndices, new StardewValley.Object((int)CurrentStock[item], quantity), new int[2]
				{
					0,
					quantity
				});
			}

			// Clear stock array
			CurrentStock.Clear();

			return stock;
		}
	}
}