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

#if DEBUG
using System.Text;
#endif

namespace Shockah.Kokoro.Map
{
	public static class IMapExt
	{
		public static int Count<TTile>(this IMap<TTile>.WithKnownSize map, Func<IMap<TTile>.WithKnownSize, IntPoint, bool> predicate)
		{
			int count = 0;
			for (int y = map.Bounds.Min.Y; y <= map.Bounds.Max.Y; y++)
				for (int x = map.Bounds.Min.X; x <= map.Bounds.Max.X; x++)
					if (predicate(map, new(x, y)))
						count++;
			return count;
		}

		public static int Count<TTile>(this IMap<TTile>.WithKnownSize map, Func<TTile, bool> predicate)
			=> map.Count((map, point) => predicate(map[point]));

		public static IntRectangle? FindBounds<TTile>(this IMap<TTile>.WithKnownSize map, Func<IMap<TTile>.WithKnownSize, IntPoint, bool> predicate)
		{
			int? minX = null;
			int? minY = null;
			int? maxX = null;
			int? maxY = null;

			for (int y = map.Bounds.Min.Y; y <= map.Bounds.Max.Y; y++)
			{
				for (int x = map.Bounds.Min.X; x <= map.Bounds.Max.X; x++)
				{
					if (!predicate(map, new(x, y)))
						continue;
					if (minX is null || minX.Value > x)
						minX = x;
					if (minY is null || minY.Value > y)
						minY = y;
					if (maxX is null || maxX.Value < x)
						maxX = x;
					if (maxY is null || maxY.Value < y)
						maxY = y;
				}
			}

			if (minX is null || minY is null || maxX is null || maxY is null)
				return null;
			else
				return new(new(minX.Value, minY.Value), new(maxX.Value, maxY.Value));
		}

		public static IntRectangle? FindBounds<TTile>(this IMap<TTile>.WithKnownSize map, Func<TTile, bool> predicate)
			=> map.FindBounds((map, point) => predicate(map[point]));

#if DEBUG
		public static string ToString<TTile>(this IMap<TTile>.WithKnownSize map, Func<TTile, char> charMapper)
		{
			StringBuilder sb = new();
			for (int y = map.Bounds.Min.Y; y <= map.Bounds.Max.Y; y++)
			{
				if (y != map.Bounds.Min.Y)
					sb.AppendLine();
				for (int x = map.Bounds.Min.X; x <= map.Bounds.Max.X; x++)
					sb.Append(charMapper(map[new(x, y)]));
			}
			return $"{sb}";
		}
#endif
	}
}