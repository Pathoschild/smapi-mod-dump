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

namespace Shockah.Kokoro.Map
{
	public sealed class ArrayMap<TTile> : IMap<TTile>.WithKnownSize, IMap<TTile>.Writable
	{
		public TTile this[IntPoint point]
		{
			get => Array[point.X + Bounds.Min.X, point.Y + Bounds.Min.Y];
			set => Array[point.X + Bounds.Min.X, point.Y + Bounds.Min.Y] = value;
		}

		public IntRectangle Bounds { get; init; }

		private readonly TTile[,] Array;

		public ArrayMap(IMap<TTile>.WithKnownSize map)
		{
			this.Bounds = map.Bounds;
			this.Array = new TTile[Bounds.Width, Bounds.Height];

			for (int y = Bounds.Min.Y; y <= Bounds.Max.Y; y++)
				for (int x = Bounds.Min.X; x <= Bounds.Max.X; x++)
					Array[x - Bounds.Min.X, y - Bounds.Min.Y] = map[new(x, y)];
		}

		public ArrayMap(TTile defaultTile, int width, int height, int minX = 0, int minY = 0) : this(_ => defaultTile, width, height, minX, minY) { }

		public ArrayMap(Func<IntPoint, TTile> defaultTile, int width, int height, int minX = 0, int minY = 0)
		{
			this.Bounds = new(new(minX, minY), width, height);
			this.Array = new TTile[width, height];

			for (int y = Bounds.Min.Y; y <= Bounds.Max.Y; y++)
				for (int x = Bounds.Min.X; x <= Bounds.Max.X; x++)
					Array[x - Bounds.Min.X, y - Bounds.Min.Y] = defaultTile(new(x, y));
		}

		public override bool Equals(object? obj)
		{
			if (obj is not ArrayMap<TTile> other)
				return false;
			if (other.Bounds != Bounds)
				return false;
			for (int y = Bounds.Min.Y; y <= Bounds.Max.Y; y++)
				for (int x = Bounds.Min.X; x <= Bounds.Max.X; x++)
					if (!Equals(other[new(x, y)], this[new(x, y)]))
						return false;
			return true;
		}

		public override int GetHashCode()
			=> base.GetHashCode();

		public ArrayMap<TTile> Clone()
			=> new(this);
	}
}