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
using System.Linq;
using HedgeTech.Common.Extensions;
using StardewModdingAPI;
using AutoForager.Classes;

using Constants = AutoForager.Helpers.Constants;

namespace AutoForager.Extensions
{
	public static class ListExtensions
	{
		public static void AddDistinct(this List<ForageableItem> items, ForageableItem newItem)
		{
			if (items.Any(i => i.QualifiedItemId.Equals(newItem.QualifiedItemId))) return;

			items.Add(newItem);
		}

		public static void AddOrMergeCustomFieldsRange(this List<ForageableItem> items, IEnumerable<ForageableItem> newItems)
		{
			foreach (var newItem in newItems)
			{
				if (items.TryGetItem(newItem.QualifiedItemId, out var oldItem))
				{
					if (newItem.CustomFields.Count > 0)
					{
						foreach (var kvp in newItem.CustomFields)
						{
							oldItem?.CustomFields.TryAdd(kvp.Key, kvp.Value);
						}
					}
				}
				else
				{
					items.AddDistinct(newItem);
				}
			}
		}

		public static IOrderedEnumerable<IGrouping<string, ForageableItem>> GroupByCategory(this List<ForageableItem> list, IModHelper helper, string? categoryKey = null, IComparer<string>? comparer = null)
		{
			categoryKey ??= Constants.CustomFieldCategoryKey;

			return list.GroupBy(f =>
			{
				var category = I18n.Category_Other();

				if (f.CustomFields?.TryGetValue(categoryKey, out var customCategory) ?? false)
				{
					category = helper.Translation.Get(customCategory);

					if (category.StartsWith("(no translation:"))
					{
						category = customCategory;
					}
				}

				return category;
			})
			.OrderBy(g => g.Key, comparer ?? new CategoryComparer());
		}

		public static void SortByDisplayName(this List<ForageableItem> items)
		{
			items.Sort((x, y) => string.CompareOrdinal(x.DisplayName, y.DisplayName));
		}

		public static bool TryGetItem(this List<ForageableItem> items, string qualifiedItemId, out ForageableItem? item)
		{
			item = null;

			if (items is null || qualifiedItemId is null) return false;

			foreach (var fItem in items)
			{
				if (fItem.QualifiedItemId.IEquals(qualifiedItemId))
				{
					item = fItem;
					return true;
				}
			}

			return false;
		}
	}
}
