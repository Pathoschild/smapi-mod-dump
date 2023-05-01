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

namespace Shockah.Kokoro
{
	public static class RandomExt
	{
		public static bool NextBool(this Random random)
			=> random.Next(2) == 0;

		public static T NextElement<T>(this Random random, IReadOnlyList<T> list)
			=> list[random.Next(list.Count)];

		public static T NextElement<T>(this Random random, IReadOnlyCollection<T> collection)
			=> collection.Skip(random.Next(collection.Count)).First();

		public static void Shuffle<T>(this Random random, IList<T> list)
		{
			if (list.Count < 2)
				return;

			for (int n = list.Count; n > 1; n--)
			{
				int k = random.Next(n);
				(list[k], list[n - 1]) = (list[n - 1], list[k]);
			}
		}

		public static void Shuffle<T>(this IList<T> list, Random random)
			=> random.Shuffle(list);

		public static List<T> Shuffled<T>(this Random random, IEnumerable<T> list)
		{
			var copy = list.ToList();
			random.Shuffle(copy);
			return copy;
		}

		public static List<T> Shuffled<T>(this IEnumerable<T> list, Random random)
			=> random.Shuffled(list);
	}
}