/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/HappyHomeDesigner
**
*************************************************/

using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System.Collections.Generic;
using System.Linq;

namespace HappyHomeDesigner.Menus
{
	internal class FurniturePage : VariantPage<FurnitureEntry, Furniture>
	{
		private const int FURNITURE_MAX = 18;
		private const string KeyFavs = "tlitookilakin.HappyHomeDesigner/FurnitureFavorites";
		private const int DEFAULT_EXTENDED = 2;
		private const int DEFAULT_DEFAULT = 4;

		private static readonly int[] ExtendedTabMap = [0, 0, 1, 1, 2, 3, 4, 5, 6, 2, 2, 3, 7, 8, 2, 9, 5, 8];
		private static readonly int[] DefaultTabMap = [1, 1, 1, 1, 0, 0, 2, 4, 4, 4, 4, 0, 3, 2, 4, 5, 4, 4];
		private List<FurnitureEntry>[] Filters;
		private int[] Map;
		private int defaultSlot;

		public FurniturePage(IEnumerable<ISalable> items)
			: base(items, KeyFavs, "furniture") { }

		/// <inheritdoc/>
		public override void Init()
		{
			if (ModEntry.config.ExtendedCategories)
			{
				Map = ExtendedTabMap;
				defaultSlot = DEFAULT_EXTENDED;
				iconRow = 0;
			}
			else
			{
				Map = DefaultTabMap;
				defaultSlot = DEFAULT_DEFAULT;
				iconRow = 8;
			}

			filter_count = Map.Max() + 1;
			Filters = new List<FurnitureEntry>[filter_count];
			for (int i = 0; i < Filters.Length; i++)
				Filters[i] = [];
			filter_count += 2;
		}

		/// <inheritdoc/>
		public override IEnumerable<FurnitureEntry> GetItemsFrom(IEnumerable<ISalable> source, ICollection<string> favorites)
		{
			var season = Game1.currentLocation.GetSeason();
			var seasonName = season.ToString();

			foreach (var item in source)
			{
				if (item is Furniture furn)
				{
					var entry = new FurnitureEntry(furn, season, seasonName, favorites);
					var type = furn.furniture_type.Value;

					if (type is < FURNITURE_MAX and >= 0)
						Filters[Map[type]].Add(entry);
					else
						Filters[defaultSlot].Add(entry);

					if (entry.Favorited)
						Favorites.Add(entry);

					yield return entry;
				}
			}
		}

		/// <inheritdoc/>
		public override IReadOnlyList<IGridItem> ApplyFilter()
		{
			return	// all items
					(current_filter is 0) ? entries :
					// categories
					(current_filter <= Filters.Length) ? Filters[current_filter - 1] :
					// favorites
					Favorites;
		}

		/// <inheritdoc/>
		public override ClickableTextureComponent GetTab() 
			=> new(new(0, 0, 64, 64), Catalog.MenuTexture, new(64, 24, 16, 16), 4f);
	}
}
