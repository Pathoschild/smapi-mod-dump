/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/desto-git/smapi-RegularQuality
**
*************************************************/

namespace RegularQuality
{
	using System.Collections.Generic;
	using System.Linq;
	using Common;
	using StardewModdingAPI;
	using StardewModdingAPI.Events;
	using StardewValley;
	using StardewValley.GameData.Bundles;

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
				this.Helper.Events.Content.AssetRequested += this.OnAssetRequested;
			}

			++this.ticks;
		}

		private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
		{
			if (e.NameWithoutLocale.IsEquivalentTo("Data/Bundles"))
			{
				e.Edit(assetData =>
				{
					const int MaxItemStackSize = 999;
					const int BundleIngredientFields = 3;
					const char BundleFieldSeparator = '/';
					const char BundleIngredientSeparator = ' ';
					int configMultiplier = this.config.BundleIngredientQualityMultiplicator;

					// update community center bundles (mainly the quality crops bundle, but be compatible with other mods)
					IDictionary<string, string> data = assetData.AsDictionary<string, string>().Data;

					// https://stackoverflow.com/a/31767807
					// .ToList is part of System.Linq
					// Without it, the loop would error after an assignment to a dictionary element
					foreach (string key in data.Keys.ToList())
					{
						string[] fields = data[key].Split(BundleFieldSeparator);
						string[] bundleIngredients = fields[2].Split(BundleIngredientSeparator);
						for (int i = 0; i < bundleIngredients.Length / BundleIngredientFields; ++i)
						{
							int index = i * BundleIngredientFields;
							int indexItemId = index;
							int indexQuantity = index + 1;
							int indexQuality = index + 2;

							string itemId = bundleIngredients[indexItemId];
							string quantity = bundleIngredients[indexQuantity];
							string quality = bundleIngredients[indexQuality];

							// itemId -1 is a gold purchase, don't change anything here
							if (itemId == "-1") continue;

							// quality is already regular, no adjustment needed
							if (quality == "0") continue;

							// adjust amount -> multiply by rarity
							int intQuality = int.Parse(quality);
							int intQuantity = int.Parse(quantity);
							int newQuantity = intQuantity + (intQuantity * intQuality * configMultiplier);
							if (newQuantity > MaxItemStackSize)
							{
								Logger.Warn($"A bundle ingredient would have exceeded the maximum stack size of {MaxItemStackSize}. It has been limited to {MaxItemStackSize}.");
								Logger.Warn($"Bundle: {key} | itemId: {itemId} | adjusted quantity = {newQuantity} (= {quantity} + {quantity} * {quality} * {configMultiplier}");
								newQuantity = MaxItemStackSize;
							}

							bundleIngredients[indexQuantity] = newQuantity.ToString();
							bundleIngredients[indexQuality] = "0";
						}

						string newData = string.Join(BundleIngredientSeparator, bundleIngredients);

						// nothing changed, no need to touch the data dictionary
						if (newData == fields[2]) continue;

						fields[2] = newData;
						data[key] = string.Join(BundleFieldSeparator, fields);
					}
				});
			}
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
				// otherwise re-adding the item would "autosort" them to the first free slot when manually organizing the inventory
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