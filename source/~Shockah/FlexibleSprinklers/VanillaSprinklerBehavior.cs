/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using Shockah.Kokoro.Map;
using System.Collections.Generic;

namespace Shockah.FlexibleSprinklers
{
	internal class VanillaSprinklerBehavior : ISprinklerBehavior.Independent
	{
		public IReadOnlyList<WateringStep> GetSprinklerTilesWithSteps(IMap<SoilType>.WithKnownSize map, SprinklerInfo sprinkler)
			=> new List<WateringStep> { new(sprinkler.Coverage, 0f) };
	}
}