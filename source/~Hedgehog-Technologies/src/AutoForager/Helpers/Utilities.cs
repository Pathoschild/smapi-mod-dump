/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hedgehog-Technologies/StardewMods
**
*************************************************/

using System.Linq;
using AutoForager.Extensions;
using StardewValley;

namespace AutoForager.Helpers
{
	public static class Utilities
	{
		public static string? GetItemIdFromName(string name)
		{
			string? itemId = null;

			if (!name.IsNullOrEmpty())
			{
				name = name.Trim();

				if (int.TryParse(name, out var i))
				{
					name = $"(O){i}";
				}

				var item = ItemRegistry.GetMetadata(name);

				if (!item.Exists())
				{
					item = ItemRegistry.GetMetadata($"(O){name}");
				}

				if (item.Exists())
				{
					itemId = item.QualifiedItemId.Substring(3);
				}
				else
				{
					itemId = Game1.objectData
						.Where(d => d.Value.Name.IEquals(name))
						.Select(d => d.Key)
						.FirstOrDefault();
				}
			}

			return itemId;
		}
	}
}
