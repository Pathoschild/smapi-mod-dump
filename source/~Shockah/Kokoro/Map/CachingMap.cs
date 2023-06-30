/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using System.Collections.Generic;

namespace Shockah.Kokoro.Map;

public sealed class CachingMap<TTile> : IMap<TTile>
{
	private IMap<TTile> Wrapped { get; init; }
	private readonly Dictionary<IntPoint, TTile> Cache = new();

	public TTile this[IntPoint point]
	{
		get
		{
			if (!Cache.TryGetValue(point, out var value))
			{
				value = Wrapped[point];
				Cache[point] = value;
			}
			return value;
		}
	}

	public CachingMap(IMap<TTile> wrapped)
	{
		this.Wrapped = wrapped;
	}

	public sealed class WithKnownSize : IMap<TTile>.WithKnownSize
	{
		private IMap<TTile>.WithKnownSize Wrapped { get; init; }

		public IntRectangle Bounds
			=> Wrapped.Bounds;

		private readonly bool[,] IsCached;
		private readonly TTile[,] Cache;

		public TTile this[IntPoint point]
		{
			get
			{
				if (!IsCached[point.X - Bounds.Min.X, point.Y - Bounds.Min.Y])
					Cache[point.X - Bounds.Min.X, point.Y - Bounds.Min.Y] = Wrapped[point];
				return Cache[point.X - Bounds.Min.X, point.Y - Bounds.Min.Y];
			}
		}

		public WithKnownSize(IMap<TTile>.WithKnownSize wrapped)
		{
			this.Wrapped = wrapped;
			this.IsCached = new bool[wrapped.Bounds.Width, wrapped.Bounds.Height];
			this.Cache = new TTile[wrapped.Bounds.Width, wrapped.Bounds.Height];
		}
	}
}