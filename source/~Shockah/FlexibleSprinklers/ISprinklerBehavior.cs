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
using Shockah.Kokoro.Map;
using System.Collections.Generic;
using System.Linq;

namespace Shockah.FlexibleSprinklers
{
	internal interface ISprinklerBehavior
	{
		void ClearCache()
		{
		}

		void ClearCacheForMap(IMap<SoilType>.WithKnownSize map)
		{
		}

		IReadOnlyList<WateringStep> GetSprinklerTilesWithSteps(IMap<SoilType>.WithKnownSize map, IReadOnlySet<SprinklerInfo> sprinklers);

		IReadOnlySet<IntPoint> GetSprinklerTiles(IMap<SoilType>.WithKnownSize map, IReadOnlySet<SprinklerInfo> sprinklers)
			=> GetSprinklerTilesWithSteps(map, sprinklers).SelectMany(step => step.Tiles).ToHashSet();

		public interface Independent : ISprinklerBehavior
		{
			IReadOnlyList<WateringStep> GetSprinklerTilesWithSteps(IMap<SoilType>.WithKnownSize map, SprinklerInfo sprinkler);

			IReadOnlyList<WateringStep> ISprinklerBehavior.GetSprinklerTilesWithSteps(IMap<SoilType>.WithKnownSize map, IReadOnlySet<SprinklerInfo> sprinklers)
			{
				List<WateringStep> results = new();
				foreach (var sprinkler in sprinklers)
					foreach (var step in GetSprinklerTilesWithSteps(map, sprinkler))
						results.Add(step);
				return results.OrderBy(step => step.Time).ToList();
			}

			IReadOnlySet<IntPoint> GetSprinklerTiles(IMap<SoilType>.WithKnownSize map, SprinklerInfo sprinkler)
				=> GetSprinklerTilesWithSteps(map, sprinkler).SelectMany(step => step.Tiles).ToHashSet();
		}
	}
}