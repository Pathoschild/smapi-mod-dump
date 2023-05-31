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

namespace Shockah.FlexibleSprinklers
{
	internal readonly struct WateringStep
	{
		public readonly IReadOnlySet<IntPoint> Tiles { get; init; }
		public readonly float Time { get; init; }

		public WateringStep(IReadOnlySet<IntPoint> tiles, float time)
		{
			this.Tiles = tiles;
			this.Time = time;
		}
	}
}