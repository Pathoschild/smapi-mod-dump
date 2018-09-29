using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace InventoryCycle
{
	/// <summary>The mod entry point.</summary>
	public class ModEntry : Mod
	{

		/*********
		** Public Variables
		*********/
		Keys frontKey = Keys.E;
		Keys backKey = Keys.Q;

		/*********
		** Public methods
		*********/
		/// <summary>Initialise the mod.</summary>
		/// <param name="helper">Provides methods for interacting with the mod directory, such as read/writing a config file or custom JSON files.</param>
		public override void Entry(IModHelper helper)
		{
			ControlEvents.KeyPressed += this.ReceiveKeyPress;


			ModConfig config = helper.ReadConfig<ModConfig>();

			this.frontKey = config.frontCycleKeyASCIINumber;
			this.backKey = config.backCycleKeyASCIINumber;

			this.Monitor.Log($"Loaded Cycle Keys as: \n For cycling forward: {frontKey.ToString()} \n For cycling backward {backKey.ToString()}");
		}


		/*********
		** Private methods
		*********/
		/// <summary>The method invoked when the player presses a keyboard button.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void ReceiveKeyPress(object sender, EventArgsKeyPressed e)
		{
			if (e.KeyPressed.Equals(frontKey))
			{
				Item[] oldInventory = Game1.player.items.ToArray();
				List<Item> newInventory = new List<Item>();
				for (int i = 12; i < oldInventory.Length; i++)
				{
					newInventory.Add(oldInventory[i]);
				}
				for (int i = 0; i < 12; i++)
				{
					newInventory.Add(oldInventory[i]);
				}

				Game1.player.setInventory(newInventory);
				if (Game1.activeClickableMenu is GameMenu)
				{
					Game1.activeClickableMenu = new GameMenu();
				}
			}
			else if (e.KeyPressed.Equals(backKey))
			{
				Item[] oldInventory = Game1.player.items.ToArray();
				List<Item> newInventory = new List<Item>();
				for (int i = 24; i < oldInventory.Length; i++)
				{
					newInventory.Add(oldInventory[i]);
				}
				for (int i = 0; i < 24; i++)
				{
					newInventory.Add(oldInventory[i]);
				}

				Game1.player.setInventory(newInventory);
				if (Game1.activeClickableMenu is GameMenu)
				{
					Game1.activeClickableMenu = new GameMenu();
				}
			}
		}
	}
}