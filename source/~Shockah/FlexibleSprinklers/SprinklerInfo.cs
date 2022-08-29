/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Shockah.FlexibleSprinklers
{
	public readonly struct SprinklerInfo : IEquatable<SprinklerInfo>
	{
		public readonly IReadOnlySet<Vector2> Layout { get; init; }

		public readonly int Power { get; init; }

		public SprinklerInfo(IReadOnlySet<Vector2> layout) : this(layout, layout.Count) { }

		public SprinklerInfo(IReadOnlySet<Vector2> layout, int power)
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