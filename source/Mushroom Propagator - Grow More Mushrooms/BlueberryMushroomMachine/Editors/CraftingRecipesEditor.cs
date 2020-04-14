using StardewModdingAPI;

namespace BlueberryMushroomMachine.Editors
{
	internal class CraftingRecipesEditor : IAssetEditor
	{
		private readonly bool _isDebugging;

		public CraftingRecipesEditor()
		{
			_isDebugging = ModEntry.Instance.Config.DebugMode;
		}

		public bool CanEdit<T>(IAssetInfo asset)
		{
			return asset.AssetNameEquals(@"Data/CraftingRecipes");
		}
		public void Edit<T>(IAssetData asset)
		{
			Log.D($"Editing {asset.AssetName}.",
				_isDebugging);

			// Inject crafting recipe data using custom appended index as the result.
			//var name = ModEntry.Instance.i18n.Get("machine.name");
			var name = Const.PropagatorInternalName;
			var data = asset.AsDictionary<string, string>().Data;
			if (!data.ContainsKey(name))
				data.Add(name, ModValues.CraftingRecipeData);

			Log.D($"Recipe injected: \"{name}\": \"{data[name]}\"",
				_isDebugging);
		}
	}
}
