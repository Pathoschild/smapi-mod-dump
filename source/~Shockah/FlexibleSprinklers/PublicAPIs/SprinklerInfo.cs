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