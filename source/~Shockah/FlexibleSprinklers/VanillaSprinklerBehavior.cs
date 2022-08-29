/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using Shockah.CommonModCode;
using System.Collections.Generic;
using System.Linq;

namespace Shockah.FlexibleSprinklers
{
	internal class VanillaSprinklerBehavior : ISprinklerBehavior.Independent
	{
		public IReadOnlyList<(IReadOnlySet<IntPoint>, float)> GetSprinklerTilesWithSteps(IMap map, IntPoint sprinklerPosition, SprinklerInfo info)
			=> new List<(IReadOnlySet<IntPoint>, float)> { (info.Layout.Select(t => new IntPoint((int)t.X + sprinklerPosition.X, (int)t.Y + sprinklerPosition.Y)).ToHashSet(), 0f) };
	}
}