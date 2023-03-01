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
	internal class SelfWaterSprinklerBehavior : ISprinklerBehavior
	{
		private readonly ISprinklerBehavior Wrapped;

		public SelfWaterSprinklerBehavior(ISprinklerBehavior wrapped)
		{
			this.Wrapped = wrapped;
		}

		public IReadOnlyList<(IReadOnlySet<IntPoint>, float)> GetSprinklerTilesWithSteps(IMap map)
		{
			var results = new List<(IReadOnlySet<IntPoint>, float)>
			{
				(map.GetAllSprinklers().Select(s => s.position).ToHashSet(), 0f)
			};
			foreach (var wrappedResult in Wrapped.GetSprinklerTilesWithSteps(map))
				results.Add((wrappedResult.Item1, 0.2f + wrappedResult.Item2 * 0.8f));
			return results;
		}

		internal class Independent : ISprinklerBehavior.Independent
		{
			private readonly ISprinklerBehavior.Independent Wrapped;

			public Independent(ISprinklerBehavior.Independent wrapped)
			{
				this.Wrapped = wrapped;
			}

			public IReadOnlyList<(IReadOnlySet<IntPoint>, float)> GetSprinklerTilesWithSteps(IMap map, IntPoint sprinklerPosition, SprinklerInfo info)
			{
				var results = new List<(IReadOnlySet<IntPoint>, float)>
				{
					(new HashSet<IntPoint>() { sprinklerPosition }, 0f)
				};
				foreach (var wrappedResult in Wrapped.GetSprinklerTilesWithSteps(map, sprinklerPosition, info))
					results.Add((wrappedResult.Item1, 0.2f + wrappedResult.Item2 * 0.8f));
				return results;
			}
		}
	}
}