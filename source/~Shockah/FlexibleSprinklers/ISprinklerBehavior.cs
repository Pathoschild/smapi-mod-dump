/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;

namespace Shockah.FlexibleSprinklers
{
	internal interface ISprinklerBehavior
	{
		void ClearCache()
		{
		}

		void ClearCacheForMap(IMap map)
		{
		}

		IList<(ISet<IntPoint>, float)> GetSprinklerTilesWithSteps(IMap map);

		ISet<IntPoint> GetSprinklerTiles(IMap map)
			=> GetSprinklerTilesWithSteps(map).SelectMany(step => step.Item1).ToHashSet();

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Nested in another interface")]
		public interface Independent: ISprinklerBehavior
		{
			IList<(ISet<IntPoint>, float)> GetSprinklerTilesWithSteps(IMap map, IntPoint sprinklerPosition, SprinklerInfo info);

			IList<(ISet<IntPoint>, float)> GetSprinklerTilesWithSteps(IMap map, IEnumerable<(IntPoint position, SprinklerInfo info)> sprinklers)
			{
				var results = new List<(ISet<IntPoint>, float)>();
				foreach (var (sprinklerPosition, info) in sprinklers)
					foreach (var step in GetSprinklerTilesWithSteps(map, sprinklerPosition, info))
						results.Add(step);
				return results.OrderBy(step => step.Item2).ToList();
			}

			IList<(ISet<IntPoint>, float)> ISprinklerBehavior.GetSprinklerTilesWithSteps(IMap map)
				=> GetSprinklerTilesWithSteps(map, map.GetAllSprinklers());

			ISet<IntPoint> GetSprinklerTiles(IMap map, IntPoint sprinklerPosition, SprinklerInfo info)
				=> GetSprinklerTilesWithSteps(map, sprinklerPosition, info).SelectMany(step => step.Item1).ToHashSet();

			ISet<IntPoint> GetSprinklerTiles(IMap map, IEnumerable<(IntPoint position, SprinklerInfo info)> sprinklers)
				=> GetSprinklerTilesWithSteps(map, sprinklers).SelectMany(step => step.Item1).ToHashSet();
		}
	}
}