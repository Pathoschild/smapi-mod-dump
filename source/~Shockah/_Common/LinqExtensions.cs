/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

namespace Shockah.CommonModCode
{
	public static class LinqExtensions
	{
		public static T? FirstOrNull<T>(this IEnumerable<T> self) where T: struct
			=> self.Select(e => new T?(e)).FirstOrDefault();

		public static T? FirstOrNull<T>(this IEnumerable<T> self, Func<T, bool> predicate) where T: struct
			=> self.Where(predicate).Select(e => new T?(e)).FirstOrDefault();

		public static T? LastOrNull<T>(this IEnumerable<T> self) where T : struct
			=> self.Select(e => new T?(e)).LastOrDefault();

		public static T? LastOrNull<T>(this IEnumerable<T> self, Func<T, bool> predicate) where T: struct
			=> self.Where(predicate).Select(e => new T?(e)).LastOrDefault();

		public static int? FirstIndex<T>(this IList<T> self, Func<T, bool> predicate)
		{
			int index = 0;
			foreach (var item in self)
			{
				if (predicate(item))
					return index;
				index++;
			}
			return null;
		}
	}
}