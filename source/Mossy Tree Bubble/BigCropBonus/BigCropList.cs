/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tocseoj/StardewValleyMods
**
*************************************************/

using StardewValley;
using StardewValley.GameData.GiantCrops;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

namespace Tocseoj.Stardew.BigCropBonus;

public class BigCropList
{
	const string PUMPKIN = "(O)276";
	const string GOLDEN_PUMPKIN = "(O)373";
	private readonly Dictionary<string, int> cropCounts = [];

	public int Count => cropCounts.Count;

	public BigCropList()
	{
		cropCounts = [];

		Utility.ForEachLocation(location => {
			foreach (GiantCrop giantCrop in location.resourceClumps.OfType<GiantCrop>()) {
				GiantCropData? giantCropItem = giantCrop.GetData();
				if (giantCropItem == null) continue;

				// Note: the key is the source items id (i.e. for a Giant melon, it is '(O)254' which is for Melon)
				string cropId = giantCropItem.FromItemId;
				if (!cropCounts.ContainsKey(cropId)) {
					cropCounts[cropId] = 0;
				}
				cropCounts[cropId] += 1;
			}
			return true;
		});
	}

	public int GetCount(string itemId)
	{
		if (cropCounts.ContainsKey(itemId)) {
			return cropCounts[itemId];
		}
		return 0;
	}

	public int GetCount(SObject item) {
		if (HasBigCropOf(item, out string cropId)) {
			return GetCount(cropId);
		}
		return 0;
	}

	public bool HasBigCropOf(SObject item, out string cropId)
	{
		cropId = "";
		if (cropCounts.ContainsKey(item.QualifiedItemId)) {
			cropId = item.QualifiedItemId;
		}
		else if (cropCounts.ContainsKey($"(O){item.preservedParentSheetIndex}")) {
			cropId = $"(O){item.preservedParentSheetIndex}";
		}
		else if (item.Category == SObject.CookingCategory) {
			CraftingRecipe recipe = new(item.Name, true);
			foreach (string ingredient in recipe.recipeList.Keys) {
				string? qualifiedIngredientId = ItemRegistry.QualifyItemId(ingredient);
				if (qualifiedIngredientId != null && cropCounts.ContainsKey(qualifiedIngredientId)) {
					cropId = qualifiedIngredientId;
					break;
				}
			}
		}
		else if (item.QualifiedItemId == GOLDEN_PUMPKIN && cropCounts.ContainsKey(PUMPKIN)) {
			cropId = PUMPKIN;
		}
		return cropId != "";
	}
	public bool HasBigCropOf(SObject item)
	{
		return HasBigCropOf(item, out _);
	}
}