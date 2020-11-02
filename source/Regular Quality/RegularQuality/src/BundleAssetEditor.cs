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

	/// <summary>Main class.</summary>
	internal class BundleAssetEditor : IAssetEditor
	{
		private const int MaxItemStackSize = 999;
		private const int BundleIngredientFields = 3;
		private const char BundleFieldSeparator = '/';
		private const char BundleIngredientSeparator = ' ';
		private readonly int configMultiplier;

		/// <summary>Initializes a new instance of the <see cref="BundleAssetEditor"/> class.</summary>
		/// <param name="configMultiplier">Bundle ingredient multiplier setting.</param>
		public BundleAssetEditor(int configMultiplier)
		{
			this.configMultiplier = configMultiplier;
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
					int newQuantity = intQuantity + (intQuantity * intQuality * this.configMultiplier);
					if (newQuantity > MaxItemStackSize)
					{
						Logger.Warn($"A bundle ingredient would have exceeded the maximum stack size of {MaxItemStackSize}. It has been limited to {MaxItemStackSize}.");
						Logger.Warn($"Bundle: {key} | itemId: {itemId} | adjusted quantity = {newQuantity} (= {quantity} + {quantity} * {quality} * {this.configMultiplier}");
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
	}
}