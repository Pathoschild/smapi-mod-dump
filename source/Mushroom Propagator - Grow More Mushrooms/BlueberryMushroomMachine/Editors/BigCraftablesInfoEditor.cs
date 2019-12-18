using System.Linq;
using StardewValley;
using StardewModdingAPI;

namespace BlueberryMushroomMachine.Editors
{
	class BigCraftablesInfoEditor : IAssetEditor
	{
		public bool CanEdit<T>(IAssetInfo asset)
		{
			return asset.AssetNameEquals(@"Data/BigCraftablesInformation");
		}

		public void Edit<T>(IAssetData asset)
		{
			var data = asset.AsDictionary<int, string>().Data;

			// Slide into a free tilesheet index.
			var indicesPerRow = Game1.bigCraftableSpriteSheet.Width / 16;
			var index = data.Keys.Max();						// Seek to the end of the spritesheet.
			index += indicesPerRow - (index % indicesPerRow);   // Add to the start of the next row.
			Data.PropagatorIndex = index;                   // It works this time I promise

			Log.T("Propagator Index:" + Data.PropagatorIndex);

			Data.ObjectData = string.Format(Data.ObjectData,
				ModEntry.Instance.i18n.Get("machine.name"),
				ModEntry.Instance.i18n.Get("machine.desc"));

			// Inject custom object data with appending index.
			data.Add(Data.PropagatorIndex, Data.ObjectData);

			// Update not-yet-injected crafting recipe data to match.
			Data.CraftingRecipeData = string.Format(
				Data.CraftingRecipeData, Data.PropagatorIndex);
		}
	}
}
