/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hedgehog-Technologies/StardewMods
**
*************************************************/

using System.Collections.Generic;
using StardewValley.TerrainFeatures;

namespace AutoForager.Extensions
{
	public static class FruitTreeExtensions
	{
		public static List<string> GetFruitItemIds(this FruitTree fruitTree)
		{
			var itemIds = new List<string>();

			foreach (var fruit in fruitTree.fruit)
			{
				itemIds.Add(fruit.QualifiedItemId);
			}

			return itemIds;
		}
	}
}
