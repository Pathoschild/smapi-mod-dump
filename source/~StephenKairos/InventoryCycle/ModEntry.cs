using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using System.Linq;

namespace InventoryCycle
{
	/// <summary>The mod entry point.</summary>
	public class ModEntry : Mod
	{
		/*********
		** Fields
		*********/
		private SButton frontKey = SButton.E;
		private SButton backKey = SButton.Q;


		/*********
		** Public methods
		*********/
		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			helper.Events.Input.ButtonPressed += this.OnButtonPressed;

			ModConfig config = helper.ReadConfig<ModConfig>();

			this.frontKey = config.frontCycleKeyASCIINumber;
			this.backKey = config.backCycleKeyASCIINumber;

			this.Monitor.Log($"Loaded Cycle Keys as: \n For cycling forward: {frontKey.ToString()} \n For cycling backward {backKey.ToString()}");
		}


		/*********
		** Private methods
		*********/
		/// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
		{
			if (e.Button == frontKey)
			{
				Item[] oldInventory = Game1.player.Items.ToArray();
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
			else if (e.Button == backKey)
			{
				Item[] oldInventory = Game1.player.Items.ToArray();
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