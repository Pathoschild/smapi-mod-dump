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
using System.Collections.Generic;
using System.Linq;

namespace BlueberryMushroomMachine.Editors
{
	internal static class BigCraftablesInfoEditor
	{
		private static int LastIndexedValue = -1;

		public static bool ApplyEdit(AssetRequestedEventArgs e)
		{
			if (e.NameWithoutLocale.IsEquivalentTo(@"Data/BigCraftablesInformation"))
			{
				e.Edit(apply: BigCraftablesInfoEditor.EditImpl);
				return true;
			}
			return false;
		}

		public static void EditImpl(IAssetData asset)
		{
			Log.T($"Editing {asset.Name}.",
				ModEntry.Config.DebugMode);

			IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;

			// Slide into a free tilesheet index
			int index = data.Keys.Where((int id) => id < 300).Max() + 1;    // Avoids JA incompatibilities

			if (index == 1)
			{
				Log.W($"Could not find free index for mushroom propagator. This may cause issues");
			}

			ModValues.PropagatorIndex = index;

			Log.D($"Object indexed:  {ModValues.PropagatorIndex}",
				ModEntry.Config.DebugMode);

			// Inject custom object data with appending index
			ModValues.ObjectData = string.Format(ModValues.ObjectDataFormat,
				ModValues.PropagatorInternalName,
				ModEntry.I18n.Get("machine.desc"));

			if (!data.ContainsKey(ModValues.PropagatorIndex))
			{
				data.Add(ModValues.PropagatorIndex, ModValues.ObjectData);
			}
			else
			{
				Log.W($"Chosen ID {index} seems previously occupied somehow.");
			}

			// Update not-yet-injected crafting recipe data to match
			ModValues.RecipeData = string.Format(ModValues.RecipeDataFormat,
				ModValues.PropagatorIndex);

			Log.D($"Object injected: \"{ModValues.PropagatorIndex}\": " +
				  $"\"{data[ModValues.PropagatorIndex]}\"",
				ModEntry.Config.DebugMode);

			if (BigCraftablesInfoEditor.LastIndexedValue != index)
			{
				Log.D("Invalidating cache for ID change", ModEntry.Config.DebugMode);
				// Invalidate cache of possibly-badly-indexed data
				ModEntry.Instance.Helper.GameContent.InvalidateCache(@"Data/Events/Farm");
				ModEntry.Instance.Helper.GameContent.InvalidateCache(@"Data/CraftingRecipes");
				ModEntry.Instance.Helper.GameContent.InvalidateCache(@"Tilesheets/Craftables");
			}
			BigCraftablesInfoEditor.LastIndexedValue = index;
		}
	}
}
