/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/razikh-git/BlueberryMushroomMachine
**
*************************************************/

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
			Log.T($"Editing {asset.AssetName}.",
				_isDebugging);

			// Inject crafting recipe data using custom appended index as the result.
			var name = ModValues.PropagatorInternalName;
			var data = asset.AsDictionary<string, string>().Data;
			if (!data.ContainsKey(name))
				data.Add(name, ModValues.CraftingRecipeData);

			Log.D($"Recipe injected: \"{name}\": \"{data[name]}\"",
				_isDebugging);
		}
	}
}
