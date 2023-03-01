/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using Shockah.Kokoro;
using System.Collections.Generic;
using System.Linq;

namespace Shockah.FlexibleSprinklers
{
	internal static class SprinklerLayouts
	{
		public static readonly IReadOnlySet<IntPoint> Basic = IntPoint.NeighborOffsets.ToHashSet();
		public static IReadOnlySet<IntPoint> Quality => Box(1).ToHashSet();
		public static IReadOnlySet<IntPoint> Iridium => Box(2).ToHashSet();
		public static IReadOnlySet<IntPoint> IridiumWithPressureNozzle => Box(3).ToHashSet();

		public static IReadOnlySet<IntPoint> Vanilla(int tier)
		{
			if (tier <= 1)
				return Basic;
			else
				return Box(tier - 1).ToHashSet();
		}

		private static IEnumerable<IntPoint> Box(int radius)
		{
			for (var y = -radius; y <= radius; y++)
			{
				for (var x = -radius; x <= radius; x++)
				{
					if (x != 0 || y != 0)
						yield return new IntPoint(x, y);
				}
			}
		}
	}
}