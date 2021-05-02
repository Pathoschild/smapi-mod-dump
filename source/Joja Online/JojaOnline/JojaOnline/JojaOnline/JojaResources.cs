/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/JojaOnline
**
*************************************************/

using JojaOnline.JojaOnline.Items;
using JojaOnline.JojaOnline.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Object = StardewValley.Object;

namespace JojaOnline
{
	public static class JojaResources
	{
		private static IMonitor modMonitor;

		private static Dictionary<ISalable, int[]> jojaOnlineStock;
		private static List<string> cachedItemNames;

		private static Texture2D jojaMailBackground;
		private static Texture2D jojaSiteBackground;
		private static Texture2D jojaCheckoutBackground;
		private static Texture2D jojaMobileVertBackground;
		private static Texture2D jojaMobileHorzBackground;

		private static Texture2D jojaSiteSpriteSheet;
		private static Texture2D jojaAdBanners;
		private static Texture2D jojaAppIcon;


		public static void LoadMonitor(IMonitor monitor)
		{
			modMonitor = monitor;
		}

		public static IMonitor GetMonitor()
		{
			return modMonitor;
		}

		public static void SetJojaOnlineStock(Dictionary<string, int> nameToPriceOverrides, bool doStockAllSeedsBeforeYearOne, bool doCopyPiereeSeedStock)
		{
			// Clone the current stock
			jojaOnlineStock = new Dictionary<ISalable, int[]>();
			cachedItemNames = new List<string>();

			// Add wood, stone and hardwood
			AddToJojaOnlineStock(new Object(Vector2.Zero, 388, int.MaxValue));
			AddToJojaOnlineStock(new Object(Vector2.Zero, 390, int.MaxValue));
			AddToJojaOnlineStock(new Object(Vector2.Zero, 709, int.MaxValue), 500);

			// Add coal
			AddToJojaOnlineStock(new Object(Vector2.Zero, 382, int.MaxValue));

			// Add battery pack
			AddToJojaOnlineStock(new Object(Vector2.Zero, 390, int.MaxValue), 2500);

			// Add cloth
			AddToJojaOnlineStock(new Object(Vector2.Zero, 428, int.MaxValue), 2000);

			// Add energy tonic
			AddToJojaOnlineStock(new Object(Vector2.Zero, 349, int.MaxValue));

			// Add some of Pierre's goods (if available)
			try
            {
				foreach (Item item in new SeedShop().shopStock().Keys)
				{
					if (item is null)
					{
						continue;
					}

					if (item.parentSheetIndex == 368)
					{
						AddToJojaOnlineStock(new Object(Vector2.Zero, item.parentSheetIndex, int.MaxValue), 100);
					}
					else if (item.parentSheetIndex == 369)
					{
						AddToJojaOnlineStock(new Object(Vector2.Zero, item.parentSheetIndex, int.MaxValue), 150);
					}
					else if (item.parentSheetIndex == 370)
					{
						AddToJojaOnlineStock(new Object(Vector2.Zero, item.parentSheetIndex, int.MaxValue), 100);
					}
					else if (item.parentSheetIndex == 371)
					{
						AddToJojaOnlineStock(new Object(Vector2.Zero, item.parentSheetIndex, int.MaxValue), 150);
					}
					else if (item.parentSheetIndex == 465)
					{
						AddToJojaOnlineStock(new Object(Vector2.Zero, item.parentSheetIndex, int.MaxValue), 100);
					}
					else if (item.parentSheetIndex == 466)
					{
						AddToJojaOnlineStock(new Object(Vector2.Zero, item.parentSheetIndex, int.MaxValue), 150);
					}
					else if (item.category == -74 && doCopyPiereeSeedStock) // Is a seed, add it to the stock
                    {
						modMonitor.Log($"Adding {item.Name}", LogLevel.Trace);
						AddToJojaOnlineStock(new Object(Vector2.Zero, item.parentSheetIndex, int.MaxValue));
					}
				}
			}
			catch (ArgumentNullException ex)
            {
				modMonitor.Log($"Unable to load some of Pierre's goods, SeedShop seems to be empty or null: {ex}", LogLevel.Warn);
            }

			// Add some of Marnie's goods (only hay for now)
			AddToJojaOnlineStock(new Object(Vector2.Zero, 178, int.MaxValue), 50);

			// Add the current JojaMart items
			Utility.getJojaStock().ToList().ForEach(x => AddToJojaOnlineStock(x.Key));

			// If past year one (or doStockAllSeedsBeforeYearOne), unlock all seeds (that aren't in the current season due to initial cloning)
			if (Game1.player.yearForSaveGame > 1 || doStockAllSeedsBeforeYearOne)
			{
				modMonitor.Log("Loading JojaMart's stock for all seasons for JojaOnline", LogLevel.Debug);
				if (!Game1.currentSeason.Equals("spring"))
				{
					AddToJojaOnlineStock(new Object(Vector2.Zero, 472, int.MaxValue));
					AddToJojaOnlineStock(new Object(Vector2.Zero, 473, int.MaxValue));
					AddToJojaOnlineStock(new Object(Vector2.Zero, 474, int.MaxValue));
					AddToJojaOnlineStock(new Object(Vector2.Zero, 475, int.MaxValue));
					AddToJojaOnlineStock(new Object(Vector2.Zero, 427, int.MaxValue));
					AddToJojaOnlineStock(new Object(Vector2.Zero, 429, int.MaxValue));
					AddToJojaOnlineStock(new Object(Vector2.Zero, 477, int.MaxValue));
				}
				if (!Game1.currentSeason.Equals("summer"))
				{
					AddToJojaOnlineStock(new Object(Vector2.Zero, 480, int.MaxValue));
					AddToJojaOnlineStock(new Object(Vector2.Zero, 482, int.MaxValue));
					AddToJojaOnlineStock(new Object(Vector2.Zero, 483, int.MaxValue));
					AddToJojaOnlineStock(new Object(Vector2.Zero, 484, int.MaxValue));
					AddToJojaOnlineStock(new Object(Vector2.Zero, 479, int.MaxValue));
					AddToJojaOnlineStock(new Object(Vector2.Zero, 302, int.MaxValue));
					AddToJojaOnlineStock(new Object(Vector2.Zero, 453, int.MaxValue));
					AddToJojaOnlineStock(new Object(Vector2.Zero, 455, int.MaxValue));
					AddToJojaOnlineStock(new Object(431, int.MaxValue, isRecipe: false, 100));
				}
				if (!Game1.currentSeason.Equals("fall"))
				{
					AddToJojaOnlineStock(new Object(Vector2.Zero, 487, int.MaxValue));
					AddToJojaOnlineStock(new Object(Vector2.Zero, 488, int.MaxValue));
					AddToJojaOnlineStock(new Object(Vector2.Zero, 483, int.MaxValue));
					AddToJojaOnlineStock(new Object(Vector2.Zero, 490, int.MaxValue));
					AddToJojaOnlineStock(new Object(Vector2.Zero, 299, int.MaxValue));
					AddToJojaOnlineStock(new Object(Vector2.Zero, 301, int.MaxValue));
					AddToJojaOnlineStock(new Object(Vector2.Zero, 492, int.MaxValue));
					AddToJojaOnlineStock(new Object(Vector2.Zero, 491, int.MaxValue));
					AddToJojaOnlineStock(new Object(Vector2.Zero, 493, int.MaxValue));
					AddToJojaOnlineStock(new Object(431, int.MaxValue, isRecipe: false, 100));
					AddToJojaOnlineStock(new Object(Vector2.Zero, 425, int.MaxValue));
				}
			}

			// Add Auto-Petter (normally only available from completing Joja route)
			AddToJojaOnlineStock(new Object(Vector2.Zero, 272), 50000);

			// Load in any Joja Prime Membership items from JojaItems
			int primeMembershipItemID = JojaItems.GetJojaPrimeMembershipID();
			if (primeMembershipItemID > 0 && !HasPrimeMembership())
            {
				AddToJojaOnlineStock(new Object(primeMembershipItemID, 1, false, -1, 0), 500000, 1);
			}

			// TODO: Load in any modded seeds
			// Need a way to get the store they are added to, as Json Asset adds the seeds OnMenuChanged...
			// Otherwise all the modded seeds will be added, regardless if they are available...
			/*
			if (JojaItems.IsJsonAssetApiConnected())
			{
				foreach (KeyValuePair<string, int> nameToID in JojaItems.GetJsonAssetApi().GetAllCropIds())
                {
					Object obj = new Object(Vector2.Zero, nameToID.Value, int.MaxValue);
					if (obj != null && obj.category == -74) // Is a seed
					{

					}
                }
			}
			*/

			// Override the prices if the user has given us any
			OverridePrices(nameToPriceOverrides);
		}

		private static void OverridePrices(Dictionary<string, int> nameToPriceOverrides)
        {
			foreach (string itemName in nameToPriceOverrides.Keys)
            {
				ISalable item = jojaOnlineStock.Keys.FirstOrDefault(i => i.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase) || i.DisplayName.Equals(itemName, StringComparison.OrdinalIgnoreCase));

				if (item != null)
                {
					jojaOnlineStock[item][0] = nameToPriceOverrides[itemName];
					modMonitor.Log($"{itemName} was overridden by the user with the price of {nameToPriceOverrides[itemName]}.", LogLevel.Trace);
				}
				else
                {
					modMonitor.Log($"{itemName} was requested for override with the price of {nameToPriceOverrides[itemName]}, but item was missing from the stock.", LogLevel.Trace);
                }
            }
        }

		public static void RemoveFromJojaOnlineStock(ISalable item)
		{
			// Remove the item
			if (jojaOnlineStock.ContainsKey(item))
            {
				jojaOnlineStock.Remove(item);
			}
		}

		public static void AddToJojaOnlineStock(ISalable item, int salePrice = -1, int stock = -1)
        {
			if (cachedItemNames.Contains(item.DisplayName) && item.DisplayName != "Wallpaper" && item.DisplayName != "Flooring")
            {
				return;
            }
			cachedItemNames.Add(item.DisplayName);

			// Add the unique item
			jojaOnlineStock.Add(item, new int[] { salePrice == -1 ? item.salePrice() : salePrice, stock == -1 ? int.MaxValue : stock });
		}

		public static Dictionary<ISalable, int[]> GetJojaOnlineStock()
        {
			return jojaOnlineStock;
        }

		public static bool HasPrimeMembership()
        {
			if (Game1.MasterPlayer.mailReceived.Contains("JojaPrimeShipping") || Game1.MasterPlayer.mailForTomorrow.Contains("JojaPrimeShipping"))
			{
				return true;
            }

			return false;
		}

		public static void LoadTextures(IModHelper helper)
		{
			// Load the MFM related background(s)
			jojaMailBackground = helper.Content.Load<Texture2D>(Path.Combine("assets", "jojaLetterBG.png"));

			// Load in the JojaSite background
			jojaSiteBackground = helper.Content.Load<Texture2D>(Path.Combine("assets", "jojaStoreBG.png"));

			// Load in the JojaSite spritesheet
			jojaSiteSpriteSheet = helper.Content.Load<Texture2D>(Path.Combine("assets", "jojaSiteSprites.png"));

			// Load JojaSite checkout background
			jojaCheckoutBackground = helper.Content.Load<Texture2D>(Path.Combine("assets", "jojaCheckoutBG.png"));

			// Load JojaSite ad banners
			jojaAdBanners = helper.Content.Load<Texture2D>(Path.Combine("assets", "jojaBanners.png"));

			// Load JojaSite's app icon for Mobile Phone mod
			jojaAppIcon = helper.Content.Load<Texture2D>(Path.Combine(@"assets\mobile", "jojaIcon.png"));

			// Load JojaSite's app vertical background for Mobile Phone mod
			jojaMobileVertBackground = helper.Content.Load<Texture2D>(Path.Combine(@"assets\mobile", "jojaMobileVerticalBG.png"));

			// Load JojaSite's app horizontal background for Mobile Phone mod
			jojaMobileHorzBackground = helper.Content.Load<Texture2D>(Path.Combine(@"assets\mobile", "jojaMobileHorizontalBG.png"));
		}

		public static JojaSite GetScaledJojaSite()
        {
			// Check if we need to scale back the UI
			int width = 750;
			int height = 1000;
			float scale = 1f;

			if (height > Game1.uiViewport.Height)
			{
				scale = 750 / 1000f;
				width = 525;
				height = 700;
			}

			return new JojaSite(width, height, scale);
		}

		public static Texture2D GetJojaMailBackground()
		{
			return jojaMailBackground;
		}

		public static Texture2D GetJojaSiteBackground()
		{
			return jojaSiteBackground;
		}

		public static Texture2D GetJojaCheckoutBackground()
		{
			return jojaCheckoutBackground;
		}

		public static Texture2D GetJojaMobileVertBackground()
		{
			return jojaMobileVertBackground;
		}

		public static Texture2D GetJojaMobileHorzBackground()
		{
			return jojaMobileHorzBackground;
		}

		public static Texture2D GetJojaSiteSpriteSheet()
		{
			return jojaSiteSpriteSheet;
		}

		public static Texture2D GetJojaAdBanners()
		{
			return jojaAdBanners;
		}

		public static Texture2D GetJojaAppIcon()
		{
			return jojaAppIcon;
		}
	}
}
