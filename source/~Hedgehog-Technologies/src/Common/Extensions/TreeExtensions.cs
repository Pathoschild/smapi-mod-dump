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

namespace HedgeTech.Common.Extensions
{
	public static class TreeExtensions
	{
		public static List<string> GetSeedAndSeedItemIds(this Tree tree)
		{
			var itemIds = new List<string>();
			var treeData = tree.GetData();

			if (treeData.SeedItemId is not null)
			{
				itemIds.Add(treeData.SeedItemId);
			}

			if (treeData.SeedDropItems is not null)
			{
				foreach (var item in treeData.SeedDropItems)
				{
					if (item?.ItemId is not null)
					{
						itemIds.Add(item.ItemId);
					}
				}
			}

			return itemIds;
		}
	}
}
