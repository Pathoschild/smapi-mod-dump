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
		private int previousShirt;
		private int previousHatIndex;
		private bool isWearingHat = false;
		private readonly int rainBootsIndex = 804;
		private readonly int rainHatIndex = 40;

		/*********
        ** Public methods
        *********/
		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			this.Config = this.Helper.ReadConfig<ModConfig>();

			helper.Events.GameLoop.DayStarted += this.OnDayStarted;
			helper.Events.GameLoop.DayEnding += this.OnDayEnding;
			helper.Events.Display.MenuChanged += this.OnMenuChanged;
		}


		/*********
        ** Private methods
        *********/
		/// <summary>Raised after the day starts. Checks to see if it is raining. If yes, equips rain equipment.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnDayStarted(object sender, DayStartedEventArgs e)
		{
			// ignore if player hasn't loaded a save yet
			if (!Context.IsWorldReady)
				return;

			// ignore if the rainy clothes aren't loaded
			if (!this.Helper.ModRegistry.IsLoaded("IMS.Rainy.Day.Clothes"))
				return;

			//When it is raining, equip Rain Hood, Coat, and Boots at start of day
			if (this.WeatherJustifiesRainCoat())
			{
				// Rain Coat!
				if (this.Config.RainCoatEnabled)
				{
					// save current shirt for later retrieval (reequip at end of rainy day)
					this.previousShirt = Game1.player.shirt.Value;
					// Change current shirt to be rain coat!
					Game1.player.changeShirt(this.Config.RaincoatClothingIndex - 1);
				}
				

				// Rain Hood!
				if (this.Config.RainHoodEnabled)
				{
					// Make sure the hood is UP! 
					this.Helper.Content.InvalidateCache("Data/hats");
					IDictionary<int, string> data = this.Helper.Content.Load<Dictionary<int, string>>("Data/hats", ContentSource.GameContent);
					data[this.rainHatIndex] = "Rain Hood/Optional extra for your raincoat./false/false";
					this.Helper.Content.InvalidateCache("Data/hats");

					// Ensure hat is not null (no need to save hat if there is none!)
					if (Game1.player.hat.Value != null)
					{
						// save current hat for later retrieval (reequip at end of rainy day)
						this.isWearingHat = true;
						this.previousHatIndex = Game1.player.hat.Value.which.Value;
						// put previous hat into player's inventory
						Hat previousHat = new Hat(this.previousHatIndex);
						if (Game1.player.isInventoryFull())
							Game1.player.dropItem(previousHat);
						else
							Game1.player.addItemToInventory(new Hat(this.previousHatIndex));
					}
					// Replace hat with rain hood
					Game1.player.changeHat(this.rainHatIndex);
				}

				// Rain Boots!
				if (this.Config.RainBootsEnabled)
				{
					// Step 1 - take off old boots, put in player's inventory
					if (Game1.player.boots.Value != null && Game1.player.boots.Value.displayName != new Boots(this.rainBootsIndex).displayName)
					{
						this.previousBoots = Game1.player.boots.Value;
						Game1.player.boots.Value.onUnequip();
						if (Game1.player.isInventoryFull())
							Game1.player.dropItem(previousBoots);
						else
							Game1.player.addItemToInventory(previousBoots);
						

					}
					// Step 2 - equip new boots (rain boots!)
					Game1.player.boots.Value = new Boots(this.rainBootsIndex);
					Game1.player.boots.Value.onEquip();
				}
			}
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
			if (!this.Helper.ModRegistry.IsLoaded("IMS.Rainy.Day.Clothes"))
				return;

			// When it is raining, unequip Rain Hood, Coat, and Boots at end of day
			if (this.WeatherJustifiesRainCoat())
			{
				// Read what was previously equipped and equip it
				if (this.Config.RainCoatEnabled)
					Game1.player.changeShirt(this.previousShirt);
				// if is currently wearing the rain hat, remove it 
				if (this.Config.RainHoodEnabled)
				{
					// Hood back DOWN
					this.Helper.Content.InvalidateCache("Data/hats");
					IDictionary<int, string> data = this.Helper.Content.Load<Dictionary<int, string>>("Data/hats", ContentSource.GameContent);
					data[this.rainHatIndex] = "Rain Hood/Optional extra for your raincoat./true/false";
					this.Helper.Content.InvalidateCache("Data/hats");

					if (Game1.player.hat.Value.which.Value == this.rainHatIndex)
					{
						// if the player was previously wearing a hat, put it back , else take it off
						if (this.isWearingHat)
						{
							Game1.player.changeHat(this.previousHatIndex);
							// if the player has their previous hat in their inventory, remove it 
							Hat previousHat = new Hat(this.previousHatIndex);
							if (Game1.player.hasItemInInventoryNamed(previousHat.Name))
								Game1.player.removeItemFromInventory(Game1.player.hasItemWithNameThatContains(previousHat.Name));
						}
						else
							Game1.player.hat.Value = null;

					}
					// If the player still has the rain hat in their inventory, remove it
					Hat rainHat = new Hat(this.rainHatIndex);
					if (Game1.player.hasItemInInventoryNamed(rainHat.Name))
						Game1.player.removeItemFromInventory(Game1.player.hasItemWithNameThatContains(rainHat.Name));
				}

				if (this.Config.RainBootsEnabled)
				{
					Boots rainBoots = new Boots(this.rainBootsIndex);
					// Step 1 - take off rain boots
					if (Game1.player.boots.Value.displayName == rainBoots.displayName)
						Game1.player.boots.Value.onUnequip();
					// Step 2 - equip previous boots
					if (Game1.player.boots.Value.displayName == rainBoots.displayName)
					{
						if (this.previousBoots != null)
						{
							// if player has the previousBoots in their inventory, remove it 
							if (Game1.player.hasItemInInventoryNamed(this.previousBoots.Name))
								Game1.player.removeItemFromInventory(Game1.player.hasItemWithNameThatContains(this.previousBoots.Name));
							Game1.player.boots.Value = this.previousBoots;
							Game1.player.boots.Value.onEquip();
						}
						else
						{
							// if player didn't have previousBoots, unequip the boots
							Game1.player.boots.Value.onUnequip();
							Game1.player.boots.Value = null;
						}
					}
					// If the player still has the rain boots in their inventory, remove it 
					if (Game1.player.hasItemInInventoryNamed(rainBoots.Name))
						Game1.player.removeItemFromInventory(Game1.player.hasItemWithNameThatContains(rainBoots.Name));
				}

				this.ResetPreviousData();
			}
		}
	
		private void ResetPreviousData()
		{
			this.previousHatIndex = 0;
			this.previousBoots = null;
			this.isWearingHat = false;
		}

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

		/// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		private void OnMenuChanged(object sender, MenuChangedEventArgs e)
		{
			// ignore if the rainy clothes aren't loaded
			if (!this.Helper.ModRegistry.IsLoaded("IMS.Rainy.Day.Clothes"))
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
	}
}