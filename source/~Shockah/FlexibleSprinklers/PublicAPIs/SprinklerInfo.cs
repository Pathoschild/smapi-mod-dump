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
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shockah.FlexibleSprinklers
{
	public readonly struct SprinklerInfo : IEquatable<SprinklerInfo>
	{
		public static readonly Lazy<IReadOnlySet<IntPoint>> DefaultTier1Coverage = new(() => IntPoint.Zero.GetSpiralingTiles().Distinct().Take(4).ToHashSet());
		public static readonly Lazy<IReadOnlySet<IntPoint>> DefaultTier2Coverage = new(() => IntPoint.Zero.GetSpiralingTiles().Distinct().Take(3 * 3 - 1).ToHashSet());
		public static readonly Lazy<IReadOnlySet<IntPoint>> DefaultTier3Coverage = new(() => IntPoint.Zero.GetSpiralingTiles().Distinct().Take(5 * 5 - 1).ToHashSet());
		public static readonly Lazy<IReadOnlySet<IntPoint>> DefaultTier4Coverage = new(() => IntPoint.Zero.GetSpiralingTiles().Distinct().Take(7 * 7 - 1).ToHashSet());
		public static readonly Lazy<IReadOnlySet<IntPoint>> DefaultTier5Coverage = new(() => IntPoint.Zero.GetSpiralingTiles().Distinct().Take(9 * 9 - 1).ToHashSet());
		public static readonly Lazy<IReadOnlySet<IntPoint>> DefaultTier6Coverage = new(() => IntPoint.Zero.GetSpiralingTiles().Distinct().Take(11 * 11 - 1).ToHashSet());
		public static readonly Lazy<IReadOnlySet<IntPoint>> DefaultTier7Coverage = new(() => IntPoint.Zero.GetSpiralingTiles().Distinct().Take(13 * 13 - 1).ToHashSet());
		public static readonly Lazy<IReadOnlySet<IntPoint>> DefaultTier8Coverage = new(() => IntPoint.Zero.GetSpiralingTiles().Distinct().Take(15 * 15 - 1).ToHashSet());

		public readonly object Owner { get; init; }
		public readonly IntRectangle OccupiedSpace { get; init; }
		public readonly IReadOnlySet<IntPoint> Coverage { get; init; }

		public int Power
			=> Coverage.Count;

		public SprinklerInfo(object owner, IntRectangle occupiedSpace, IReadOnlySet<IntPoint> coverage)
		{
			this.Owner = owner;
			this.OccupiedSpace = occupiedSpace;
			this.Coverage = coverage.ToHashSet();
		}

		public void Deconstruct(out object owner, out IntRectangle occupiedSpace, out IReadOnlySet<IntPoint> coverage)
		{
			owner = Owner;
			occupiedSpace = OccupiedSpace;
			coverage = Coverage;
		}

		public bool Equals(SprinklerInfo other)
			=> Equals(Owner, other.Owner) && OccupiedSpace == other.OccupiedSpace && Coverage.SetEquals(other.Coverage);

		public override bool Equals(object? obj)
			=> obj is SprinklerInfo info && Equals(info);

		public override int GetHashCode()
			=> (Owner, OccupiedSpace, Coverage).GetHashCode();

		public static bool operator ==(SprinklerInfo left, SprinklerInfo right)
			=> left.Equals(right);

		public static bool operator !=(SprinklerInfo left, SprinklerInfo right)
			=> !(left == right);
	}
}