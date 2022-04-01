/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using Shockah.CommonModCode;
using System.Collections.Generic;

namespace Shockah.FlexibleSprinklers
{
	internal class CachingMap: IMap
	{
		private readonly IMap Wrapped;
		private SoilType?[,] Cache;
		private int MinX;
		private int MinY;
		private int Width;
		private int Height;

		internal CachingMap(IMap.WithKnownSize wrapped)
		{
			this.Wrapped = wrapped;

			MinX = 0;
			MinY = 0;
			Cache = new SoilType?[wrapped.Width, wrapped.Height];
			Width = wrapped.Width;
			Height = wrapped.Height;
		}

		internal CachingMap(IMap wrapped, IntPoint startingPoint, int initialSize = 8)
		{
			this.Wrapped = wrapped;

			MinX = startingPoint.X - initialSize / 2;
			MinY = startingPoint.Y - initialSize / 2;
			Cache = new SoilType?[initialSize, initialSize];
			Width = initialSize;
			Height = initialSize;
		}

		public override bool Equals(object? obj)
			=> obj is IMap other && Equals(other);

		public bool Equals(IMap? other)
			=> other is CachingMap map && Wrapped == map;

		public override int GetHashCode()
			=> Wrapped.GetHashCode();

		public SoilType this[IntPoint point]
		{
			get
			{
				PrepareForPoint(point);
				var cached = Cache[point.X - MinX, point.Y - MinY];
				if (cached is null)
				{
					cached = Wrapped[point];
					Cache[point.X - MinX, point.Y - MinY] = cached;
				}
				return cached.Value;
			}
		}

		private void PrepareForPoint(IntPoint point)
		{
			if (point.X < MinX || point.Y < MinY || point.X >= MinX + Width || point.Y >= MinY + Height)
			{
				int targetMinX = MinX;
				int targetMinY = MinY;
				int targetWidth = Width;
				int targetHeight = Height;

				do
				{
					targetMinX -= targetWidth / 2;
					targetWidth *= 2;
				}
				while (point.X < targetMinX || point.X >= targetWidth + targetMinX);

				do
				{
					targetMinY -= targetHeight / 2;
					targetHeight *= 2;
				}
				while (point.Y < targetMinY || point.Y >= targetHeight + targetMinY);

				var newCache = new SoilType?[targetWidth, targetHeight];
				for (int y = 0; y < Height; y++)
					for (int x = 0; x < Width; x++)
						newCache[x + (MinX - targetMinX), y + (MinY - targetMinY)] = Cache[x, y];

				Cache = newCache;
				MinX = targetMinX;
				MinY = targetMinY;
				Width = targetWidth;
				Height = targetHeight;
			}
		}

		public void WaterTile(IntPoint point)
			=> Wrapped.WaterTile(point);

		public IEnumerable<(IntPoint position, SprinklerInfo info)> GetAllSprinklers()
			=> Wrapped.GetAllSprinklers();
	}

	internal class KnownSizeCachingMap: IMap.WithKnownSize
	{
		private readonly IMap Wrapped;
		private SoilType?[,] Cache;
		public int Width { get; }
		public int Height { get; }

		internal KnownSizeCachingMap(IMap.WithKnownSize wrapped)
		{
			this.Wrapped = wrapped;

			Cache = new SoilType?[wrapped.Width, wrapped.Height];
			Width = wrapped.Width;
			Height = wrapped.Height;
		}

		public override bool Equals(object? obj)
			=> obj is IMap other && Equals(other);

		public bool Equals(IMap? other)
			=> other is CachingMap map && Wrapped == map;

		public override int GetHashCode()
			=> Wrapped.GetHashCode();

		public SoilType this[IntPoint point]
		{
			get
			{
				if (point.X < 0 || point.Y < 0 || point.X >= Width || point.Y >= Height)
					return SoilType.NonWaterable;
				var cached = Cache[point.X, point.Y];
				if (cached is null)
				{
					cached = Wrapped[point];
					Cache[point.X, point.Y] = cached;
				}
				return cached.Value;
			}
		}

		public void WaterTile(IntPoint point)
			=> Wrapped.WaterTile(point);

		public IEnumerable<(IntPoint position, SprinklerInfo info)> GetAllSprinklers()
			=> Wrapped.GetAllSprinklers();
	}
}
