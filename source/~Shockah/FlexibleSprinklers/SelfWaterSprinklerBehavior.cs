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
using System.Linq;

namespace Shockah.FlexibleSprinklers
{
	internal class SelfWaterSprinklerBehavior : ISprinklerBehavior
	{
		private readonly ISprinklerBehavior Wrapped;

		public SelfWaterSprinklerBehavior(ISprinklerBehavior wrapped)
		{
			this.Wrapped = wrapped;
		}

		public IReadOnlyList<WateringStep> GetSprinklerTilesWithSteps(IMap<SoilType>.WithKnownSize map, IReadOnlySet<SprinklerInfo> sprinklers)
		{
			var results = new List<WateringStep>
			{
				new(sprinklers.SelectMany(s => s.OccupiedSpace.AllPointEnumerator()).ToHashSet(), 0f)
			};
			foreach (var wrappedResult in Wrapped.GetSprinklerTilesWithSteps(map, sprinklers))
				results.Add(new(wrappedResult.Tiles, 0.2f + wrappedResult.Time * 0.8f));
			return results;
		}

		internal class Independent : ISprinklerBehavior.Independent
		{
			private readonly ISprinklerBehavior.Independent Wrapped;

			public Independent(ISprinklerBehavior.Independent wrapped)
			{
				this.Wrapped = wrapped;
			}

			public IReadOnlyList<WateringStep> GetSprinklerTilesWithSteps(IMap<SoilType>.WithKnownSize map, SprinklerInfo sprinkler)
			{
				var results = new List<WateringStep>
				{
					new(sprinkler.OccupiedSpace.AllPointEnumerator().ToHashSet(), 0f)
				};
				foreach (var wrappedResult in Wrapped.GetSprinklerTilesWithSteps(map, sprinkler))
					results.Add(new(wrappedResult.Tiles, 0.2f + wrappedResult.Time * 0.8f));
				return results;
			}
		}
	}
}