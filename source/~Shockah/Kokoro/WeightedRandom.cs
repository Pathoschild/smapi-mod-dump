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

namespace Shockah.Kokoro
{
	public record struct WeightedItem<T>(
		double Weight,
		T Item
	);

	public sealed class WeightedRandom<T>
	{
		public IReadOnlyList<WeightedItem<T>> Items
			=> ItemStorage;

		public double WeightSum { get; private set; } = 0;

		private readonly List<WeightedItem<T>> ItemStorage = new();

		public WeightedRandom()
		{
		}

		public WeightedRandom(IEnumerable<WeightedItem<T>> items)
		{
			this.ItemStorage = items.ToList();
			this.WeightSum = items.Sum(item => item.Weight);
		}

		public void Add(WeightedItem<T> item)
		{
			if (item.Weight <= 0)
				return;
			ItemStorage.Add(item);
			WeightSum += item.Weight;
		}

		public T Next(Random random, bool consume = false)
		{
			if (ItemStorage.Count == 0)
				throw new IndexOutOfRangeException("Cannot choose a random element, as the list is empty.");
			if (ItemStorage.Count == 1)
			{
				T result = ItemStorage[0].Item;
				if (consume)
				{
					WeightSum = 0;
					ItemStorage.RemoveAt(0);
				}
				return result;
			}

			double weightedRandom = random.NextDouble() * WeightSum;
			for (int i = 0; i < ItemStorage.Count; i++)
			{
				var item = ItemStorage[i];
				weightedRandom -= item.Weight;

				if (weightedRandom <= 0)
				{
					if (consume)
					{
						WeightSum -= ItemStorage[i].Weight;
						ItemStorage.RemoveAt(i);
					}
					return item.Item;
				}
			}
			throw new InvalidOperationException("Invalid state.");
		}

		public IEnumerable<T> GetConsumingEnumerable(Random random)
		{
			int count;
			while ((count = ItemStorage.Count) != 0)
			{
				if (count == 1)
				{
					var item = ItemStorage[0];
					WeightSum = 0;
					ItemStorage.RemoveAt(0);
					yield return item.Item;
					yield break;
				}
				else
				{
					double weightedRandom = random.NextDouble() * WeightSum;
					for (int i = 0; i < ItemStorage.Count; i++)
					{
						var item = ItemStorage[i];
						weightedRandom -= item.Weight;

						if (weightedRandom <= 0)
						{
							WeightSum -= ItemStorage[i].Weight;
							ItemStorage.RemoveAt(i);
							yield return item.Item;
							break;
						}
					}
				}
			}
		}
	}

	public static class WeightedRandomClassExt
	{
		public static T? NextOrNull<T>(this WeightedRandom<T> weightedRandom, Random random, bool consume = false)
			where T : class
		{
			if (weightedRandom.Items.Count == 0)
				return null;
			else
				return weightedRandom.Next(random, consume);
		}
	}

	public static class WeightedRandomStructExt
	{
		public static T? NextOrNull<T>(this WeightedRandom<T> weightedRandom, Random random, bool consume = false)
			where T : struct
		{
			if (weightedRandom.Items.Count == 0)
				return null;
			else
				return weightedRandom.Next(random, consume);
		}
	}
}