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
	public sealed class OutOfBoundsValuesMap<TTile> : IMap<TTile>.WithKnownSize
	{
		public TTile this[IntPoint point]
		{
			get
			{
				if (KnownSizeMap.Bounds.Contains(point))
					return KnownSizeMap[point];
				else
					return OutOfBoundsProvider(point);
			}
		}

		public IntRectangle Bounds
			=> KnownSizeMap.Bounds;

		private readonly IMap<TTile>.WithKnownSize KnownSizeMap;
		private readonly Func<IntPoint, TTile> OutOfBoundsProvider;

		public OutOfBoundsValuesMap(IMap<TTile>.WithKnownSize knownSizeMap, TTile outOfBoundsDefaultTile) : this(knownSizeMap, _ => outOfBoundsDefaultTile) { }

		public OutOfBoundsValuesMap(IMap<TTile>.WithKnownSize knownSizeMap, Func<IntPoint, TTile> outOfBoundsProvider)
		{
			this.KnownSizeMap = knownSizeMap;
			this.OutOfBoundsProvider = outOfBoundsProvider;
		}
	}
}