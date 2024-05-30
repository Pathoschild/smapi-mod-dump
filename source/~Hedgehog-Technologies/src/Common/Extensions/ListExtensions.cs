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

namespace HedgeTech.Common.Extensions
{
	public static class ListExtensions
	{
		public static void AddDistinct<T>(this List<T> items, T newItem)
			where T : notnull
		{
			if (items.Any(i => i.Equals(newItem))) return;

			items.Add(newItem);
		}

		public static bool IsNullOrEmpty<T>(this List<T>? list)
		{
			if (list is null) return true;

			return !list.Any();
		}
	}
}
