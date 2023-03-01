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

namespace Shockah.FlexibleSprinklers
{
	public readonly struct SprinklerInfo : IEquatable<SprinklerInfo>
	{
		public static SprinklerInfo Basic { get; private set; } = new SprinklerInfo(SprinklerLayouts.Basic);
		public static SprinklerInfo Quality { get; private set; } = new SprinklerInfo(SprinklerLayouts.Quality);
		public static SprinklerInfo Iridium { get; private set; } = new SprinklerInfo(SprinklerLayouts.Iridium);
		public static SprinklerInfo IridiumWithPressureNozzle { get; private set; } = new SprinklerInfo(SprinklerLayouts.IridiumWithPressureNozzle);

		public readonly IReadOnlySet<IntPoint> Layout { get; init; }

		public readonly int Power { get; init; }

		public SprinklerInfo(IReadOnlySet<IntPoint> layout) : this(layout, layout.Count) { }

		public SprinklerInfo(IReadOnlySet<IntPoint> layout, int power)
		{
			this.Layout = layout;
			this.Power = power;
		}

		public bool Equals(SprinklerInfo other)
			=> Power == other.Power && Layout.SetEquals(other.Layout);

		public override bool Equals(object? obj)
			=> obj is SprinklerInfo info && Equals(info);

		public override int GetHashCode()
			=> (Layout.Count, Power).GetHashCode();

		public static bool operator ==(SprinklerInfo left, SprinklerInfo right)
			=> left.Equals(right);

		public static bool operator !=(SprinklerInfo left, SprinklerInfo right)
			=> !(left == right);
	}
}