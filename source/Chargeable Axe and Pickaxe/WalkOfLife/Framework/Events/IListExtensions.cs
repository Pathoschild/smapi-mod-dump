/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

namespace TheLion.AwesomeProfessions
{
	public static class IListExtensions
	{
		/// <summary>Determine if the calling list contains any instance of a given type.</summary>
		/// <param name="type">The type to search for.</param>
		public static bool ContainsType<T>(this IList<T> list, Type type)
		{
			return list.Any(item => item != null && item.GetType() == type);
		}

		/// <summary>Remove the first instance of a given type from the calling list.</summary>
		/// <param name="type">The type to search for.</param>
		/// <param name="removed">The removed instance.</param>
		/// <returns>Returns true if an instance was successfully removed, else returns false.</returns>
		public static bool RemoveType<T>(this IList<T> list, Type type, out T removed)
		{
			var toRemove = list.SingleOrDefault(item => item != null && item.GetType() == type);
			if (toRemove != null)
			{
				removed = toRemove;
				return list.Remove(toRemove);
			}
			else
			{
				removed = default;
				return false;
			}
		}
	}
}