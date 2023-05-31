/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shockah.FlexibleSprinklers
{
	internal interface ISprinklerProvider
	{
		IReadOnlySet<SprinklerInfo> GetSprinklers(GameLocation location);
	}

	internal sealed class ObjectSprinklerProvider : ISprinklerProvider
	{
		public IReadOnlySet<SprinklerInfo> GetSprinklers(GameLocation location)
		{
			HashSet<SprinklerInfo> results = new();
			results.UnionWith(
				location.Objects.Values
					.Where(o => o.IsSprinkler())
					.Select(s => FlexibleSprinklers.Instance.GetSprinklerInfo(s))
			);
			results.UnionWith(
				location.furniture
					.Where(o => o.IsSprinkler())
					.Select(s => FlexibleSprinklers.Instance.GetSprinklerInfo(s))
			);
			return results;
		}
	}

	internal sealed class InterceptingSprinklerProvider : ISprinklerProvider
	{
		private ISprinklerProvider Wrapped { get; init; }
		private IEnumerable<Action<GameLocation, ISet<SprinklerInfo>>> Interceptors { get; init; }

		public InterceptingSprinklerProvider(ISprinklerProvider wrapped, IEnumerable<Action<GameLocation, ISet<SprinklerInfo>>> interceptors)
		{
			this.Wrapped = wrapped;
			this.Interceptors = interceptors;
		}

		public IReadOnlySet<SprinklerInfo> GetSprinklers(GameLocation location)
		{
			var sprinklers = Wrapped.GetSprinklers(location).ToHashSet();
			foreach (var interceptor in Interceptors)
				interceptor(location, sprinklers);
			return sprinklers;
		}
	}
}