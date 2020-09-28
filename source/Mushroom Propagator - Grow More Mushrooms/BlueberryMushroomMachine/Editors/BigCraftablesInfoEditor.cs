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
			Log.T($"Editing {asset.AssetName}.",
				_isDebugging);

			var data = asset.AsDictionary<int, string>().Data;

			// Slide into a free tilesheet index.
			var index = data.Keys.Where(id => id < 300).Max()+1;	// Avoids JA incompatibilities.
			ModValues.PropagatorIndex = index;

			Log.D($"Object indexed:  {ModValues.PropagatorIndex}",
				_isDebugging);

			// Inject custom object data with appending index.
			ModValues.ObjectData = string.Format(ModValues.ObjectData,
				ModValues.PropagatorInternalName,
				ModEntry.Instance.i18n.Get("machine.desc"));

			if (!data.ContainsKey(ModValues.PropagatorIndex))
				data.Add(ModValues.PropagatorIndex, ModValues.ObjectData);

			// Update not-yet-injected crafting recipe data to match.
			ModValues.CraftingRecipeData = string.Format(ModValues.CraftingRecipeData,
				ModValues.PropagatorIndex);

			Log.D($"Object injected: \"{ModValues.PropagatorIndex}\": " +
			      $"\"{data[ModValues.PropagatorIndex]}\"",
				_isDebugging);

			// Invalidate cache of possibly-badly-indexed data.
			ModEntry.Instance.Helper.Content.InvalidateCache(@"Data/Events/Farm");
			ModEntry.Instance.Helper.Content.InvalidateCache(@"Data/CraftingRecipes");
			ModEntry.Instance.Helper.Content.InvalidateCache(@"Tilesheets/Craftables");
		}
	}
}
