namespace RegularQuality
{
	using System.Collections.Generic;
	using System.Linq;
	using StardewModdingAPI;
	using StardewModdingAPI.Events;
	using StardewValley;

	/// <summary>Main method.</summary>
	public class ModEntry : Mod, IAssetEditor
	{
		private const int MaxItemStackSize = 999;
		private const int BundleIngredientFields = 3;
		private const char BundleFieldSeparator = '/';
		private const char BundleIngredientSeparator = ' ';
		private ModConfig config;

		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			this.config = this.Helper.ReadConfig<ModConfig>();
			if (this.config.BundleIngredientQualityMultiplicator < 0)
			{
					this.Monitor.Log("Error in config.json: \"RarityMultiplicator\" must be at least 0.", LogLevel.Error);
					this.Monitor.Log("Deactivating mod", LogLevel.Error);
					return;
			}

			helper.Events.Player.InventoryChanged += this.OnInventoryChanged;
		}

		/// <summary>Get whether this instance can edit the given asset.</summary>
		/// <param name="asset">Basic metadata about the asset being loaded.</param>
		/// <typeparam name="T">.</typeparam>
		/// <returns>Wether or not the asset may be edited by this mod.</returns>
		public bool CanEdit<T>(IAssetInfo asset)
		{
			return asset.AssetNameEquals("Data/Bundles");
		}

		/// <summary>Edit a matched asset.</summary>
		/// <typeparam name="T">.</typeparam>
		/// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it.</param>
		public void Edit<T>(IAssetData asset)
		{
			// update community center bundles (mainly the quality crops bundle, but be compatible with other mods)
			IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

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
					int indexItemId   = index;
					int indexQuantity = index + 1;
					int indexQuality  = index + 2;

					string itemId   = bundleIngredients[indexItemId];
					string quantity = bundleIngredients[indexQuantity];
					string quality  = bundleIngredients[indexQuality];

					// itemId -1 is a gold purchase, don't change anything here
					if (itemId == "-1") continue;

					// quality is already regular, no adjustment needed
					if (quality == "0") continue;

					// adjust amount -> multiply by rarity
					int intQuality = int.Parse(quality);
					int intQuantity = int.Parse(quantity);
					int newQuantity = intQuantity + (intQuantity * intQuality * this.config.BundleIngredientQualityMultiplicator);
					if (newQuantity > MaxItemStackSize)
					{
							this.Monitor.Log($"A bundle ingredient would have exceeded the maximum stack size of {MaxItemStackSize}. It has been limited to {MaxItemStackSize}.", LogLevel.Warn);
							this.Monitor.Log($"Bundle: {key} | itemId: {itemId} | adjusted quantity = {newQuantity} (= {quantity} + {quantity} * {quality} * {this.config.BundleIngredientQualityMultiplicator}", LogLevel.Warn);
							newQuantity = MaxItemStackSize;
					}

					bundleIngredients[indexQuantity] = newQuantity.ToString();
					bundleIngredients[indexQuality] = "0";
				}

				string newData = string.Join(BundleIngredientSeparator.ToString(), bundleIngredients);

				// nothing changed, no need to touch the data dictionary
				if (newData == fields[2]) continue;

				fields[2] = newData;
				data[key] = string.Join(BundleFieldSeparator.ToString(), fields);
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