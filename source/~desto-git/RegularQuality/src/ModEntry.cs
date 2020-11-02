/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/desto-git/sdv-mods
**
*************************************************/

namespace RegularQuality
{
	using System.Collections.Generic;
	using Common;
	using StardewModdingAPI;
	using StardewModdingAPI.Events;
	using StardewValley;

	/// <summary>Main class.</summary>
	internal class ModEntry : Mod
	{
		private ModConfig config;
		private int ticks = 0;

		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			Logger.Init(this.Monitor);

			this.config = this.Helper.ReadConfig<ModConfig>();
			if (this.config.BundleIngredientQualityMultiplicator < 0)
			{
				Logger.Error("Error in config.json: \"BundleIngredientQualityMultiplicator\" must be at least 0.");
				Logger.Error("Deactivating mod");
				return;
			}

			helper.Events.Player.InventoryChanged += this.OnInventoryChanged;
			helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
		}

		/// <summary>Raised after each tick.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			if (this.ticks == 2)
			{
				this.Helper.Events.GameLoop.UpdateTicked -= this.OnUpdateTicked;
				this.Helper.Content.AssetEditors.Add(new BundleAssetEditor(this.config.BundleIngredientQualityMultiplicator));
			}

			++this.ticks;
		}

		/// <summary>Raised AFTER the player receives an item.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
		{
			// only update the inventory of the target player
			if (!e.IsLocalPlayer) return;

			IEnumerator<Item> enumerator = e.Added.GetEnumerator();
			while (enumerator.MoveNext())
			{
				// not an item with a quality property, skip
				if (!(enumerator.Current is StardewValley.Object item)) return;

				// quality is already regular, nothing to do
				// otherwise the below code would "autosort" items to the first free slot when manually organizing the inventory
				if (item.Quality == 0) return;

				// remove quality
				// because this happens only AFTER the item was added to the inventory,
				// make a best effort to stack the item with an already existing stack
				Game1.player.removeItemFromInventory(item);
				item.Quality = 0;
				Game1.player.addItemToInventory(item);
			}
		}
	}
}