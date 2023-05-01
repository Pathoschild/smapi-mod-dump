/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

namespace Shockah.Kokoro.Map
{
	public sealed class DictionaryMap<TTile> : IMap<TTile>.Writable
	{
		public TTile this[IntPoint point]
		{
			get
			{
				if (Dictionary.TryGetValue(point, out var value))
					return value;
				return DefaultTile(point);
			}
			set
			{
				Dictionary[point] = value;
			}
		}

		private readonly Dictionary<IntPoint, TTile> Dictionary = new();
		private readonly Func<IntPoint, TTile> DefaultTile;

		public DictionaryMap(TTile defaultTile) : this(_ => defaultTile) { }

		public DictionaryMap(Func<IntPoint, TTile> defaultTile)
		{
			this.DefaultTile = defaultTile;
		}

		public override bool Equals(object? obj)
		{
			if (obj is not DictionaryMap<TTile> other)
				return false;
			if (!other.Dictionary.ToHashSet().SequenceEqual(Dictionary.ToHashSet()))
				return false;
			return true;
		}

		public override int GetHashCode()
			=> base.GetHashCode();

		public DictionaryMap<TTile> Clone()
		{
			DictionaryMap<TTile> clone = new(DefaultTile);
			foreach (var (point, tile) in Dictionary)
				clone.Dictionary[point] = tile;
			return clone;
		}
	}
}