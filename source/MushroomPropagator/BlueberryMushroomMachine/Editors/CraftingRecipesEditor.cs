/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/BlueberryMushroomMachine
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace BlueberryMushroomMachine.Editors
{
	internal static class CraftingRecipesEditor
	{
		public static bool ApplyEdit(AssetRequestedEventArgs e)
		{
            if (e.NameWithoutLocale.IsEquivalentTo(@"Data/CraftingRecipes"))
            {
                e.Edit(EditImpl);
                return true;
            }
            return false;
		}
		public static void EditImpl(IAssetData asset)
		{
			Log.T($"Editing {asset.Name}.",
                ModEntry.Instance.Config.DebugMode);

			// Inject crafting recipe data using custom appended index as the result.
			var name = ModValues.PropagatorInternalName;
			var data = asset.AsDictionary<string, string>().Data;
			if (!data.ContainsKey(name))
				data.Add(name, ModValues.CraftingRecipeData);

			Log.D($"Recipe injected: \"{name}\": \"{data[name]}\"",
                ModEntry.Instance.Config.DebugMode);
		}
	}
}
