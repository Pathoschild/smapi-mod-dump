/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/prismaticpride
**
*************************************************/

using StardewModdingAPI;
using StardewValley.GameData.Crafting;
using System.Collections.Generic;
using System.Linq;

namespace PrismaticPride
{
	internal class TailoringEditor : IAssetEditor
	{
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;

		public bool CanEdit<_T> (IAssetInfo asset)
		{
			return asset.AssetNameEquals ("Data\\TailoringRecipes");
		}

		public void Edit<T> (IAssetData asset)
		{
			var data = asset.GetData<List<TailorItemRecipe>> ();

			// Add the clothing items from this mod to the base game recipe
			// for prismatic clothing (Cloth + Prismatic Shard). Prismatic Skirts
			// and Dresses already adds its items to the same recipe.
			var recipe = data.Find ((recipe) =>
				recipe.FirstItemTags.Contains ("item_cloth") &&
				recipe.SecondItemTags.Contains ("item_prismatic_shard"));
			if (recipe != null)
			{
				recipe.CraftedItemIDs.AddRange (ClothingEditor.GetAllIDs ()
					.Select ((id) => id.ToString ()));
			}

			// Allow any boots to be made prismatic with a Prismatic Shard.
			// The TailoringMenu patch retains the stats from the source boots.
			if (ModEntry.Instance.bootsSheetIndex != -1)
			{
				data.Add (new TailorItemRecipe
				{
					FirstItemTags = new () { "category_boots" },
					SecondItemTags = new () { "item_prismatic_shard" },
					CraftedItemID = ModEntry.Instance.bootsSheetIndex,
				});
			}
		}
	}
}
