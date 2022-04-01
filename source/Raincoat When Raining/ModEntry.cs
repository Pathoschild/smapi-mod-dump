/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bikinavisho/RainCoatWhenRaining
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley.Objects;
using StardewValley;
using System.Collections.Generic;
using StardewValley.Menus;

namespace RainCoatWhenRaining
{
	/// <summary>The mod entry point.</summary>
	public class ModEntry : Mod
	{
		/*********
	    ** Properties
		*********/
		/// <summary>The mod configuration from the player.</summary>
		private ModConfig Config;
		private Boots previousBoots;
		private Clothing previousShirt;
		private Hat previousHat;
		private bool isWearingHat = false;
		private int rainBootsIndex;
		private int rainHatIndex;
		private int rainCoatIndex = 1260;   // part of vanilla
		private const string RAIN_HAT_DISPLAY_NAME = "Rain Hood";
		private const string RAIN_SHIRT_DISPLAY_NAME = "Rain Coat";
		private const string RAIN_BOOTIES_DISPLAY_NAME = "Gumboots";
		// used to store and fetch furniture and objects in the farmhouse more easily 
		private readonly List<StorageFurniture> dresserList = new List<StorageFurniture>();
		private readonly List<Chest> chestList = new List<Chest>();
		

		//note: Game1.clothingInformation holds all clothing information currently loaded into the game, key is ID value is the full item entry
		// value is / dilineated with the last field (9th) being what type of clothing it is (Shirt or Pants) and first field is display name
		// note: some have an extra last field (10th) of Prismatic 

		//note: Boots and Hats do not classify as Objects or Clothes, and are not stored in any programmatic object
		// therefore we must look at the constructed Data file at runtime 

		/*********
        ** Public methods
        *********/
		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			this.Config = this.Helper.ReadConfig<ModConfig>();

			helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
			helper.Events.GameLoop.DayStarted += this.OnDayStarted;
			helper.Events.GameLoop.DayEnding += this.OnDayEnding;
			helper.Events.Display.MenuChanged += this.OnMenuChanged;
		}

		/*********
        ** Private methods
        *********/
		// note: GameLaunched is too early, Json Assets haven't loaded in yet
		/// <summary>
		/// On load of save, determine the item ids of the items we need. 
		/// This needs to be done because Json Assets item IDs can change from save to save.
		/// </summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
		{
			// ignore if the rainy clothes aren't loaded
			if (!this.Helper.ModRegistry.IsLoaded("IMS.JA.RainyDayClothing"))
				return;

			// if they've already been defined via loading a save before now, don't redo the logic
			if (this.rainCoatIndex != 0 && this.rainHatIndex != 0 && this.rainBootsIndex != 0)
				return;

			//Check to see if Rain Coat (vanilla clothing item) exists where we expect it to 
			// and if not, then go find it in the clothing information dictionary 
			string[] potentialRainCoatData = Game1.clothingInformation[this.rainCoatIndex].Split('/');
			if (!potentialRainCoatData[0].Equals(RAIN_SHIRT_DISPLAY_NAME))
			{
				try {
					foreach (int key in Game1.clothingInformation.Keys)
					{
						string[] clothingData = Game1.clothingInformation[key].Split('/');
						string clothesItemName = clothingData[0];
						string clothesItemType = clothingData[8];
						if (clothesItemType.Equals("Shirt") && clothesItemName.Equals(RAIN_SHIRT_DISPLAY_NAME))
						{
							this.rainCoatIndex = key;
							break;
						}
					}
				} catch (Exception exception)
				{
					this.Monitor.Log("Exception occurred while trying to fetch modified ID of Rain Coat", LogLevel.Error);
					this.Monitor.Log(exception.Message, LogLevel.Error);
					this.Monitor.Log(exception.StackTrace, LogLevel.Error);
				}
				
			}

			//find the index of the rain boots 
			try {
				foreach (int id in Game1.content.Load<Dictionary<int, string>>("Data\\Boots").Keys) {
					Boots boots = new Boots(id);
					if (boots.DisplayName.Equals(RAIN_BOOTIES_DISPLAY_NAME) || boots.DisplayName.Equals(RAIN_BOOTIES_DISPLAY_NAME)) {
						this.rainBootsIndex = id;
						break;
					}
				}
				// if it hasn't been assigned above, it will still be 0 (default for ints without initialization)
				if (this.rainBootsIndex == 0)
					this.Monitor.Log("Unable to find index of Gumboots", LogLevel.Error);
			} catch (Exception exception)
			{
				this.Monitor.Log("Exception occurred while trying to fetch ID of Gumboots", LogLevel.Error);
				this.Monitor.Log(exception.Message, LogLevel.Error);
				this.Monitor.Log(exception.StackTrace, LogLevel.Error);
			}


			//find the index of the rain hood
			try
			{
				foreach (int id in Game1.content.Load<Dictionary<int, string>>("Data\\hats").Keys)
				{
					Hat hat = new Hat(id);
					if (hat.DisplayName.Equals(RAIN_HAT_DISPLAY_NAME))
					{
						this.rainHatIndex = hat.which;
						break;
					}
				}
				// if it hasn't been assigned above, it will still be 0 (default for ints without initialization)
				if (this.rainHatIndex == 0)
					this.Monitor.Log("Unable to find index of Rain Hood", LogLevel.Error);
			}
			catch (Exception exception)
			{
				this.Monitor.Log("Exception occurred while trying to fetch ID of Rain Hood", LogLevel.Error);
				this.Monitor.Log(exception.Message, LogLevel.Error);
				this.Monitor.Log(exception.StackTrace, LogLevel.Error);
			}

		}


		/// <summary>Raised after the day starts. Checks to see if it is raining. If yes, equips rain equipment.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnDayStarted(object sender, DayStartedEventArgs e)
		{
			this.Monitor.Log("Rain Coat ID: " + this.rainCoatIndex, LogLevel.Debug);
			this.Monitor.Log("Gumboots ID: " + this.rainBootsIndex, LogLevel.Debug);
			this.Monitor.Log("Rain Hood ID: " + this.rainHatIndex, LogLevel.Debug);
			this.Monitor.Log($"Rain Coat ID transformed into Clothes: {new Clothing(this.rainCoatIndex).displayName}", LogLevel.Debug);
			this.Monitor.Log($"Rain Boots ID transformed into Boots: {new Boots(this.rainBootsIndex).DisplayName}", LogLevel.Debug);
			this.Monitor.Log($"Rain Hat ID transformed into Hat: {new Hat(this.rainHatIndex).DisplayName}", LogLevel.Debug);

			// ignore if player hasn't loaded a save yet
			if (!Context.IsWorldReady)
				return;

			// ignore if the rainy clothes aren't loaded
			if (!this.Helper.ModRegistry.IsLoaded("IMS.JA.RainyDayClothing"))
				return;

			try {
				foreach (Furniture furniture in Game1.player.currentLocation.furniture)
					if (furniture is StorageFurniture)
						this.dresserList.Add(furniture as StorageFurniture);
				foreach (KeyValuePair<Vector2, StardewValley.Object> pair in Game1.player.currentLocation.Objects.Pairs)
					if (pair.Value is Chest)
						this.chestList.Add(pair.Value as Chest);
			} catch (Exception exception)
			{
				this.Monitor.Log("Exception ocurred while attempting to store Game1.player.currentLocation furniture and Objects as Lists", LogLevel.Error);
				this.Monitor.Log(exception.Message);
				this.Monitor.Log(exception.StackTrace);
			}
			

			/*
			 * 
					public readonly NetRef<Clothing> shirtItem;
					public readonly NetRef<Boots> boots;
					public readonly NetRef<Hat> hat;

					//note: changeShirt appears to use female/male tilesheet index, whereas the initialization uses something else
					public void changeShirt(int whichShirt, bool is_customization_screen = false);
					public void ConvertClothingOverrideToClothesItems();

					public Color GetShirtColor();
					public List<string> GetShirtExtraData();
					public int GetShirtIndex();

					public bool hasItemInInventory(int itemIndex, int quantity, int minPrice = 0);
					public bool hasItemInInventoryNamed(string name);
					public Item hasItemWithNameThatContains(string name);

					public void removeFirstOfThisItemFromInventory(int parentSheetIndexOfItem);
					public void removeItemFromInventory(Item which);
					public Item removeItemFromInventory(int whichItemIndex);
					public bool removeItemsFromInventory(int index, int stack);
			 */

			//When it is raining, equip Rain Hood, Coat, and Boots at start of day
			if (this.WeatherJustifiesRainCoat())
			{
				// Rain Coat!
				if (this.Config.RainCoatEnabled)
				{
					// Look to see if the player owns a rain coat 
					bool ownsRainCoat = Game1.player.hasItemInInventoryNamed(RAIN_SHIRT_DISPLAY_NAME);
					if (!ownsRainCoat) {
						// check to see if there is a rain coat in any nearby containers
						ownsRainCoat = IsAnyRainItem(RAIN_SHIRT_DISPLAY_NAME);
					}

					// only change player into raincoat if one is available
					if (ownsRainCoat)
					{
						// save current shirt for later retrieval (reequip at end of rainy day)
						this.previousShirt = Game1.player.shirtItem.Value;
						this.PutItemAway(this.previousShirt);
						
						// Change current shirt to be rain coat!
						Clothing raincoat = new Clothing(this.rainCoatIndex);
						Game1.player.changeShirt(ConvertToMaleOrFemaleIndex(raincoat));
						Game1.player.ConvertClothingOverrideToClothesItems();

						// remove rain coat from inventory 
						this.RemoveItemFromInventoryOrContainer(raincoat as Item);
					}
				}
				

				// Rain Hood!
				if (this.Config.RainHoodEnabled)
				{
					// Look to see if the player owns a rain hood 
					bool ownsRainHood = Game1.player.hasItemInInventoryNamed(RAIN_HAT_DISPLAY_NAME);
					if (!ownsRainHood)
						ownsRainHood = IsAnyRainItem(RAIN_HAT_DISPLAY_NAME);

					// only change player into rain hood if one is available
					if (ownsRainHood)
					{
						// Ensure hat is not null (no need to save hat if there is none!)
						if (Game1.player.hat.Value != null)
						{
							// save current hat for later retrieval (reequip at end of rainy day)
							this.isWearingHat = true;
							this.previousHat = Game1.player.hat.Value;
							// put previous hat into player's inventory or nearby dresser
							this.PutItemAway(this.previousHat);
						}
						// Replace hat with rain hood
						Game1.player.changeHat(this.rainHatIndex);

						// Remove rain hood from inventory 
						this.RemoveItemFromInventoryOrContainer(new Hat(this.rainHatIndex) as Item);

						//this.Monitor.Log($"CURRENT HAIR??: {Game1.player.hair}", LogLevel.Debug);
						//TODO: add config option to force change hair on rainy days - hair 8 (index 7)
						// Game1.player.changeHairStyle();
					}

				}

				// Rain Boots!
				if (this.Config.RainBootsEnabled)
				{
					// Step 0 - Look to see if the player owns rain booties (named Gumshoes)
					bool ownsRainBooties = Game1.player.hasItemInInventoryNamed(RAIN_BOOTIES_DISPLAY_NAME);
					if (!ownsRainBooties)
						ownsRainBooties = IsAnyRainItem(RAIN_BOOTIES_DISPLAY_NAME);

					// only change into rain booties if they are available 
					if (ownsRainBooties)
					{
						// Step 1 - take off old boots, put in nearby dresser or player's inventory
						if (Game1.player.boots.Value != null && Game1.player.boots.Value.DisplayName != new Boots(this.rainBootsIndex).DisplayName)
						{
							this.previousBoots = Game1.player.boots.Value;
							Game1.player.boots.Value.onUnequip();
							// put boots into nearby dresser or player's inventory
							this.PutItemAway(this.previousBoots);
						}
						// Step 2 - equip new boots (rain boots!)
						Game1.player.boots.Value = new Boots(this.rainBootsIndex);
						Game1.player.boots.Value.onEquip();

						// Step 3 - remove rain boots from inventory 
						this.RemoveItemFromInventoryOrContainer(new Boots(this.rainBootsIndex) as Item);
					}

				}

				if (this.Config.RainBootsEnabled || this.Config.RainCoatEnabled || this.Config.RainHoodEnabled)
					Game1.player.UpdateClothing();
			}
			// clear these lists to ensure they stay up to date during each function exection
			this.dresserList.Clear();
			this.chestList.Clear();
		}
		/// <summary>Raised as the day ends. Checks to see if it is raining. If it is, unequip rain equipment.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnDayEnding(object sender, DayEndingEventArgs e)
		{
			// ignore if player hasn't loaded a save yet
			if (!Context.IsWorldReady)
				return;

			// ignore if the rainy clothes aren't loaded
			if (!this.Helper.ModRegistry.IsLoaded("IMS.JA.RainyDayClothing"))
				return;


			// When it is raining, unequip Rain Hood, Coat, and Boots at end of day
			if (this.WeatherJustifiesRainCoat())
			{
				this.Monitor.Log($"Previous Shirt: {this.previousShirt?.displayName}", LogLevel.Debug);
				this.Monitor.Log($"Previous Hat: {this.previousHat?.DisplayName}", LogLevel.Debug);
				this.Monitor.Log($"Previous Boots: {this.previousBoots?.DisplayName}", LogLevel.Debug);

				foreach (Furniture furniture in Game1.player.currentLocation.furniture)
					if (furniture is StorageFurniture)
						this.dresserList.Add(furniture as StorageFurniture);
				foreach (KeyValuePair<Vector2, StardewValley.Object> pair in Game1.player.currentLocation.Objects.Pairs)
					if (pair.Value is Chest)
						this.chestList.Add(pair.Value as Chest);

				// Read what was previously equipped and equip it
				if (this.Config.RainCoatEnabled)
				{
					// only change back if player is still wearing the rain coat 
					if (Game1.player.shirtItem.Value.displayName.Equals(RAIN_SHIRT_DISPLAY_NAME))
					{
						// put previous shirt back on 
						Game1.player.changeShirt(ConvertToMaleOrFemaleIndex(this.previousShirt));
						Game1.player.ConvertClothingOverrideToClothesItems();
						// remove previous shirt from inventory 
						this.RemoveItemFromInventoryOrContainer(this.previousShirt as Item);

						// put rain coat back in dresser or player's inventory
						Clothing rainCoat = new Clothing(this.rainCoatIndex);
						this.PutItemAway(rainCoat);
					}
				}

				if (this.Config.RainHoodEnabled)
				{
					// if is currently wearing the rain hat, remove it 
					if (Game1.player.hat.Value.which.Value == this.rainHatIndex)
					{
						// if the player was previously wearing a hat, put it back on, else just take the rain hood off
						if (this.isWearingHat)
						{
							Game1.player.changeHat(this.previousHat.which);
							// find and remove player's previous hat from their inventory or a nearby dresser or chest
							this.RemoveItemFromInventoryOrContainer(this.previousHat as Item);
						}
						else
							Game1.player.hat.Value = null;

						// put rain hood back in nearby dresser or player's inventory
						Hat rainHood = new Hat(this.rainHatIndex);
						this.PutItemAway(rainHood);
					}
				}

				if (this.Config.RainBootsEnabled)
				{
					Boots rainBoots = new Boots(this.rainBootsIndex);
					// if player is currently wearing the rain boots, EXECUTE THE LOGIC!
					if (Game1.player.boots.Value.DisplayName == rainBoots.DisplayName)
					{
						// Step 1 - take off rain boots
						Game1.player.boots.Value.onUnequip();

						// Step 2 - put the boots in a nearby dresser  or player inventory
						Boots rainBooties = new Boots(this.rainBootsIndex);
						this.PutItemAway(rainBooties);

						// Step 3 - equip previous boots
						if (this.previousBoots != null)
						{
							// find and remove player's previous boots from their inventory or a nearby dresser or chest
							this.RemoveItemFromInventoryOrContainer(this.previousBoots as Item);

							// put the player's previous boots back on
							Game1.player.boots.Value = this.previousBoots;
							Game1.player.boots.Value.onEquip();
						}
						else
						{
							// if player didn't have previousBoots, unequip the rain boots
							Game1.player.boots.Value.onUnequip();
							Game1.player.boots.Value = null;
						}
					}
				}

				this.Monitor.Log($"Chances of it raining tomorrow: {Game1.chanceToRainTomorrow}", LogLevel.Debug);
				this.Monitor.Log($"Weather Int tomorrow: {Game1.weatherForTomorrow}", LogLevel.Debug);
				//this.Monitor.Log($"Weather Int Tomorrow (save): {SaveGame.weatherForTomorrow}", LogLevel.Debug);

				if (this.Config.RainBootsEnabled || this.Config.RainCoatEnabled || this.Config.RainHoodEnabled)
					Game1.player.UpdateClothing();

				this.ResetPreviousData();
				
			}
			// clear these lists to ensure they stay up to date during each function exection
			this.dresserList.Clear();
			this.chestList.Clear();
		}
	
		/// <summary>
		/// Resets the previous hat/shirt/boots after the rainy weather is over and the player has been changed back into their original attire.
		/// </summary>
		private void ResetPreviousData()
		{
			this.previousHat = null;
			this.previousShirt = null;
			this.previousBoots = null;
			this.isWearingHat = false;
		}

		/// <summary>Checks to see if the current weather justifies rain attire.</summary>
		/// <returns>True if weather DOES justify rain attire, False if weather DOES NOT justify rain attire.</returns>
		private bool WeatherJustifiesRainCoat()
		{
			bool equipRainGear = Game1.isRaining || Game1.isLightning;
			// Allow for configuration of when raincoat is triggered
			if (this.Config.EnableDuringSnow)
			{
				equipRainGear = equipRainGear || Game1.isSnowing;
			}
			return equipRainGear;
		}

		private void EnsureRainHoodDataIsCorrect()
		{
			if (this.WeatherJustifiesRainCoat())
			{
				// Make sure the hood is UP! 
				IDictionary<int, string> data = this.Helper.Content.Load<Dictionary<int, string>>("Data/hats", ContentSource.GameContent);
				data[this.rainHatIndex] = "Rain Hood/Optional extra for your raincoat./false/false";
				this.Helper.Content.InvalidateCache("Data/hats");
			}
			else
			{
				// Hood back DOWN
				IDictionary<int, string> data = this.Helper.Content.Load<Dictionary<int, string>>("Data/hats", ContentSource.GameContent);
				data[this.rainHatIndex] = "Rain Hood/Optional extra for your raincoat./true/false";
				this.Helper.Content.InvalidateCache("Data/hats");
				//this.Helper.Content.InvalidateCache("Characters/Farmer/hats");
			}
		}

		/// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		private void OnMenuChanged(object sender, MenuChangedEventArgs e)
		{
			// ignore if the rainy clothes aren't loaded
			if (!this.Helper.ModRegistry.IsLoaded("IMS.JA.RainyDayClothing"))
				return;
			// ignore if STF mod is loaded, since it already adds the rainy day clothing to the shops 
			if (this.Helper.ModRegistry.IsLoaded("IMS.STF.RainyDayClothing"))
				return;
			if (e.NewMenu == null)
				return;
			if (e.NewMenu is TitleMenu)
				return;
			var menu = e.NewMenu as ShopMenu;
			bool hatMouse = menu != null && menu.potraitPersonDialogue == Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11494"), Game1.dialogueFont, Game1.tileSize * 5 - Game1.pixelZoom * 4);
			if (menu == null || (menu?.portraitPerson?.Name == null || menu?.portraitPerson?.Name == "" ) && !hatMouse)
                return;
			
			var forSale = this.Helper.Reflection.GetField<List<Item>>(menu, "forSale").GetValue();
			var itemPriceAndStock = this.Helper.Reflection.GetField<Dictionary<Item, int[]>>(menu, "itemPriceAndStock").GetValue();

			if (hatMouse)
			{
				Item item = new Hat(this.rainHatIndex);
				forSale.Add(item);
				itemPriceAndStock.Add(item, new int[] { 500, int.MaxValue });
			}
		}

		/// <summary>
		/// Intakes a piece of clothing and translates its item index (as shown in game) into the proper index in either the male
		/// or female tilesheet, depending on the gender of the farmer and the availability of index for that particular piece of clothing.
		/// This is only used for Game1.player.changeShirt
		/// </summary>
		/// <param name="clothes">The Clothing item to convert.</param>
		/// <returns>The index number of the clothing, in either Male or Female Index. </returns>
		private int ConvertToMaleOrFemaleIndex(Clothing clothes)
		{
			if (Game1.player.isMale)
			{
				if (clothes.indexInTileSheetMale == -1 && clothes.indexInTileSheetFemale != -1)
				{
					return clothes.indexInTileSheetFemale;
				} else
				{
					return clothes.indexInTileSheetMale;
				}
			} else
			{
				if (clothes.indexInTileSheetFemale == -1 && clothes.indexInTileSheetMale != -1)
				{
					return clothes.indexInTileSheetMale;
				}
				else
				{
					return clothes.indexInTileSheetFemale;
				}
			}
		}

		/// <summary>
		/// Intakes an item (in our use cases, Clothing, Boots, or Hat, which all extend Item) and puts it away either 
		/// (1) in a nearby dresser or (2) in the player's inventory.
		/// </summary>
		/// <param name="item">The item to put away.</param>
		private void PutItemAway(Item item)
		{
			bool itemHasBeenPutAway = false;
			try
			{
				// first try to put away in dresser 
				if (this.dresserList[0] != null)
				{
					(this.dresserList[0] as StorageFurniture).AddItem(item);
					itemHasBeenPutAway = true;
					this.Monitor.Log($"{item.DisplayName} has been put away in {this.dresserList[0].DisplayName}.", LogLevel.Info);
				}
			}
			catch (Exception e)
			{
				this.Monitor.Log($"Exception thrown while trying to put \"{item.DisplayName}\" into dresser", LogLevel.Error);
				this.Monitor.Log(e.Message, LogLevel.Error);
				this.Monitor.Log(e.StackTrace, LogLevel.Error);
			}

			// if unable to put away in dresser, put into player's inventory
			if (!itemHasBeenPutAway)
				if (Game1.player.isInventoryFull())
					Game1.player.dropItem(item);
				else
					Game1.player.addItemToInventory(item);

		}

		/// <summary>Checks to see if there is any of the given rain item in any of the dressers or chests nearby.</summary>
		/// <param name="rainItemDisplayName">The name of the rain item we are searching for.</param>
		/// <returns>True if rain item was found, False if rain item not found.</returns>
		private bool IsAnyRainItem(string rainItemDisplayName)
		{
			bool isAnyRainItem = false;

			// check in any dressers in the player's current location
			if (this.dresserList.Count > 0)
			{
				foreach (StorageFurniture dresser in this.dresserList)
				{
					foreach (Item item in dresser.heldItems)
						if (item.DisplayName.Equals(rainItemDisplayName))
						{
							isAnyRainItem = true;
							break;
						}
					if (isAnyRainItem)
						break;
				}
			}

			// check in any chests in the player's current location 
			if (this.chestList.Count > 0)
			{
				foreach (Chest chest in this.chestList)
				{
					foreach (Item item in chest.items) 
						if (item.DisplayName.Equals(rainItemDisplayName))
						{
							isAnyRainItem = true;
							break;
						}
					if (isAnyRainItem)
						break;
				}
			}

			return isAnyRainItem;
		}

		/// <summary>
		/// Searches in the player's inventory for item. If it's found in inventory, removes it from inventory. 
		/// If it isn't found in the player's inventory, checks nearby containers (1) dressers (2) chests to find and remove it 
		/// </summary>
		/// <param name="itemToRemove">The item we are trying to remove.</param>
		private void RemoveItemFromInventoryOrContainer(Item itemToRemove)
		{
			if (Game1.player.hasItemInInventoryNamed(itemToRemove.DisplayName))
				Game1.player.removeItemFromInventory(Game1.player.hasItemWithNameThatContains(itemToRemove.DisplayName));
			else
				this.RemoveItemFromContainer(itemToRemove);
		}

		/// <summary>
		/// Searches through (1) dressers nearby (2) chests nearby to find the given rain attire and then removes it from the container it is found in. 
		/// </summary>
		/// <param name="itemToRemove">The item we are trying to remove.</param>
		private void RemoveItemFromContainer(Item itemToRemove)
		{
			try {
				bool itemHasBeenRemoved = false;

				// check in any dressers in the player's current location
				if (this.dresserList.Count > 0)
				{
					foreach (StorageFurniture dresser in this.dresserList)
					{
						foreach(Item item in dresser.heldItems)
							if (item.DisplayName.Equals(itemToRemove.DisplayName))
							{
								// remove from dresser (this copies what's in the base game code)
								dresser.heldItems.Remove(item);
								dresser.ClearNulls();

								// set itemHasBeenRemoved to true so it isn't removed multiple times 
								itemHasBeenRemoved = true;

								break;
							}
						if (itemHasBeenRemoved)
							break;
					}
				}

				// if item hasn't already been removed, keep searching 
				if (!itemHasBeenRemoved && this.chestList.Count > 0)
				{
					// check in any chests in the player's current location 
					foreach (Chest chest in this.chestList)
					{
						foreach (Item item in chest.items) 
							if (item.DisplayName.Equals(itemToRemove.DisplayName))
							{
								// remove from chest (this copies what's in the base game code)
								chest.GetItemsForPlayer(Game1.player.UniqueMultiplayerID).Remove(item);
								chest.clearNulls();

								// set itemHasBeenRemoved to true so it isn't removed multiple times 
								itemHasBeenRemoved = true;
								break;
							}
						if (itemHasBeenRemoved)
							break;
					}
				}
				// if item still was not removed, log a warning message
				if (!itemHasBeenRemoved)
					this.Monitor.Log($"Was unable to find {itemToRemove.DisplayName} to be removed.", LogLevel.Warn);
			} catch (Exception e)
			{
				this.Monitor.Log($"Exception thrown while trying to remove \"{itemToRemove.DisplayName}\" from dresser/chest", LogLevel.Error);
				this.Monitor.Log(e.Message, LogLevel.Error);
				this.Monitor.Log(e.StackTrace, LogLevel.Error);
			}
			
		}
	}
}