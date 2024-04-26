/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hedgehog-Technologies/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;

namespace AutoForager.Classes
{
	internal class CategoryComparer : IComparer<string>
	{
		private readonly List<string> _packCategories = new();

		int IComparer<string>.Compare(string? x, string? y)
		{
			if (x is null || y is null)
			{
				throw new NullReferenceException();
			}

			if (x.Equals(y)) return 0;

			// Push to bottom of grouping
			if (x.Equals(I18n.Category_Other(), StringComparison.InvariantCultureIgnoreCase)) return 1;
			if (y.Equals(I18n.Category_Other(), StringComparison.InvariantCultureIgnoreCase)) return -1;

			var xIsPack = _packCategories.Contains(x);
			var yIsPack = _packCategories.Contains(y);
			if (xIsPack && yIsPack)
			{
				return string.Compare(x, y);
			}
			else if (xIsPack)
			{
				return 1;
			}
			else if (yIsPack)
			{
				return -1;
			}

			if (x.Equals(I18n.Category_Special(), StringComparison.InvariantCultureIgnoreCase)) return 1;
			if (y.Equals(I18n.Category_Special(), StringComparison.InvariantCultureIgnoreCase)) return -1;

			// Push to top of grouping
			if (x.Equals(I18n.Category_Vanilla(), StringComparison.InvariantCultureIgnoreCase)) return -1;
			if (y.Equals(I18n.Category_Vanilla(), StringComparison.InvariantCultureIgnoreCase)) return 1;

			return string.Compare(x, y);
		}

		public CategoryComparer(IEnumerable<IContentPack> packs)
		{
			_packCategories = packs.Select(p => p?.ReadJsonFile<ContentEntry>("content.json"))
				.Select(e => e?.Category ?? I18n.Category_Unknown())
				.ToList();

			_packCategories.Sort();
		}

		public CategoryComparer() { }
	}
}
