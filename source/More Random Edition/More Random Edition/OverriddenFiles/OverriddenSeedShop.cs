using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Randomizer
{
	/// <summary>
	/// Represents an overridden seed shop - it's used to override the normal
	/// things the shop can sell. In this case, it will make the fruit tree
	/// prices NOT hard-coded
	/// </summary>
	public class OverriddenSeedShop
	{
		/// <summary>
		/// A copy of the original "shopStock" function in SeedShop.cs, with the fruit tree
		/// prices not hard-coded. There was a code snippet that was likely intended to be used
		/// as a buyback feature, but it didn't seem to work, so it's been taken out for now.
		/// </summary>
		/// <returns></returns>
		public static Dictionary<ISalable, int[]> NewShopStock()
		{
			Dictionary<ISalable, int[]> stock = new Dictionary<ISalable, int[]>();
			if (Game1.currentSeason.Equals("spring"))
			{
				addStock(stock, 472, -1);
				addStock(stock, 473, -1);
				addStock(stock, 474, -1);
				addStock(stock, 475, -1);
				addStock(stock, 427, -1);
				addStock(stock, 477, -1);
				addStock(stock, 429, -1);
				if (Game1.year > 1)
				{
					addStock(stock, 476, -1);
					addStock(stock, 273, -1);
				}
			}
			if (Game1.currentSeason.Equals("summer"))
			{
				addStock(stock, 479, -1);
				addStock(stock, 480, -1);
				addStock(stock, 481, -1);
				addStock(stock, 482, -1);
				addStock(stock, 483, -1);
				addStock(stock, 484, -1);
				addStock(stock, 453, -1);
				addStock(stock, 455, -1);
				addStock(stock, 302, -1);
				addStock(stock, 487, -1);
				addStock(stock, 431, 100);
				if (Game1.year > 1)
					addStock(stock, 485, -1);
			}
			if (Game1.currentSeason.Equals("fall"))
			{
				addStock(stock, 490, -1);
				addStock(stock, 487, -1);
				addStock(stock, 488, -1);
				addStock(stock, 491, -1);
				addStock(stock, 492, -1);
				addStock(stock, 493, -1);
				addStock(stock, 483, -1);
				addStock(stock, 431, 100);
				addStock(stock, 425, -1);
				addStock(stock, 299, -1);
				addStock(stock, 301, -1);
				if (Game1.year > 1)
					addStock(stock, 489, -1);
			}
			addStock(stock, 297, -1);
			if (!Game1.player.craftingRecipes.ContainsKey("Grass Starter"))
				stock.Add((ISalable)new StardewValley.Object(297, 1, true, -1, 0), new int[2]
				{
					1000,
					1
				});
			addStock(stock, 245, -1);
			addStock(stock, 246, -1);
			addStock(stock, 423, -1);
			addStock(stock, 247, -1);
			addStock(stock, 419, -1);
			if ((int)Game1.stats.DaysPlayed >= 15)
			{
				addStock(stock, 368, 50);
				addStock(stock, 370, 50);
				addStock(stock, 465, 50);
			}
			if (Game1.year > 1)
			{
				addStock(stock, 369, 75);
				addStock(stock, 371, 75);
				addStock(stock, 466, 75);
			}
			Random random = new Random((int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame / 2);
			int which = random.Next(112);
			if (which == 21)
				which = 36;
			Wallpaper wallpaper1 = new Wallpaper(which, false);
			stock.Add((ISalable)wallpaper1, new int[2]
			{
		wallpaper1.salePrice(),
		int.MaxValue
			});
			Wallpaper wallpaper2 = new Wallpaper(random.Next(56), true);
			stock.Add((ISalable)wallpaper2, new int[2]
			{
				wallpaper2.salePrice(),
				int.MaxValue
			});
			Furniture furniture = new Furniture(1308, Vector2.Zero);
			stock.Add((ISalable)furniture, new int[2]
			{
				furniture.salePrice(),
				int.MaxValue
			});

			// -- Replaced code
			addStock(stock, (int)ObjectIndexes.CherrySapling, -1);
			addStock(stock, (int)ObjectIndexes.ApricotSapling, -1);
			addStock(stock, (int)ObjectIndexes.OrangeSapling, -1);
			addStock(stock, (int)ObjectIndexes.PeachSapling, -1);
			addStock(stock, (int)ObjectIndexes.PomegranateSapling, -1);
			addStock(stock, (int)ObjectIndexes.AppleSapling, -1);
			// -- End replaced code

			if (Game1.player.hasAFriendWithHeartLevel(8, true))
				addStock(stock, 458, -1);

			return stock;
		}


		/// <summary>
		/// Part of the replaced code - taken from the original code called addStock in SeedShop.cs
		/// </summary>
		/// <param name="stock"></param>
		/// <param name="parentSheetIndex"></param>
		/// <param name="buyPrice"></param>
		private static void addStock(Dictionary<ISalable, int[]> stock, int parentSheetIndex, int buyPrice = -1)
		{
			int num = buyPrice * 2;
			StardewValley.Object @object = new StardewValley.Object(Vector2.Zero, parentSheetIndex, 1);
			if (buyPrice == -1)
				num = @object.salePrice();
			stock.Add((ISalable)@object, new int[2]
			{
				num,
				int.MaxValue
			});
		}

		/// <summary>
		/// Replaces the shopStock method in SeedShop.cs with this file's NewShopStock method
		/// NOTE: THIS IS UNSAFE CODE, CHANGE WITH EXTREME CAUTION
		/// </summary>
		public static void ReplaceShopStockMethod()
		{
			if (Globals.Config.RandomizeFruitTrees)
			{
				MethodInfo methodToReplace = typeof(SeedShop).GetMethod("shopStock");
				MethodInfo methodToInject = typeof(OverriddenSeedShop).GetMethod("NewShopStock");
				Globals.RepointMethod(methodToReplace, methodToInject);
			}
		}
	}
}
