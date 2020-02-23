using System.Linq;
using StardewModdingAPI;

namespace BlueberryMushroomMachine.Editors
{
	internal class BigCraftablesInfoEditor : IAssetEditor
	{
		private readonly bool _isDebugging;

		public BigCraftablesInfoEditor()
		{
			_isDebugging = ModEntry.Instance.Config.DebugMode;
		}

		public bool CanEdit<T>(IAssetInfo asset)
		{
			return asset.AssetNameEquals(@"Data/BigCraftablesInformation");
		}

		public void Edit<T>(IAssetData asset)
		{
			Log.D($"Editing {asset.AssetName}.",
				_isDebugging);

			var data = asset.AsDictionary<int, string>().Data;

			// Slide into a free tilesheet index.
			var index = data.Keys.Where(id => id < 300).Max()+1;	// Avoids JA/CFR incompatibilities.
			Data.PropagatorIndex = index;

			Log.D($"Object indexed:  {Data.PropagatorIndex}",
				_isDebugging);

			// Inject custom object data with appending index.
			Data.ObjectData = string.Format(Data.ObjectData,
				ModEntry.Instance.i18n.Get("machine.name"),
				ModEntry.Instance.i18n.Get("machine.desc"));

			if (!data.ContainsKey(Data.PropagatorIndex))
				data.Add(Data.PropagatorIndex, Data.ObjectData);

			// Update not-yet-injected crafting recipe data to match.
			Data.CraftingRecipeData = string.Format(Data.CraftingRecipeData,
				Data.PropagatorIndex);

			Log.D($"Object injected: \"{Data.PropagatorIndex}\": \"{data[Data.PropagatorIndex]}\"",
				_isDebugging);

			// Invalidate cache of possibly-badly-indexed data.
			ModEntry.Instance.Helper.Content.InvalidateCache(@"Data/Events/Farm");
			ModEntry.Instance.Helper.Content.InvalidateCache(@"Data/CraftingRecipes");
			ModEntry.Instance.Helper.Content.InvalidateCache(@"Tilesheets/Craftables");
		}
	}
}
