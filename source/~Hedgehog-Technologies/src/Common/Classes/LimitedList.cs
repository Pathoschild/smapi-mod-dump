/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hedgehog-Technologies/StardewMods
**
*************************************************/

using System.Collections.Generic;

namespace HedgeTech.Common.Classes
{
	public class LimitedList<T>
	{
		private readonly List<T> _items;
		private int _maxSize;

		public int Count => _items.Count;

		public LimitedList(int maxSize)
		{
			_maxSize = maxSize;
			_items = new();
		}

		public void UpdateMaxSize(int newMaxSize)
		{
			if (newMaxSize > 0)
			{
				_maxSize = newMaxSize;
			}
		}

		public void Add(T item)
		{
			_items.Add(item);

			if (_items.Count >= _maxSize)
			{
				_items.RemoveAt(0);
			}
		}

		public void RemoveAt(int index)
		{
			if (index < 0 || index >= _items.Count) return;

			_items.RemoveAt(index);
		}

		public void Remove(T item)
		{
			if (_items.Contains(item))
			{
				_items.Remove(item);
			}
		}

		public IEnumerable<T> GetEnumerator()
		{
			for (int i = _maxSize - 1; i >= 0; i--)
			{
				if (i >= _items.Count) continue;

				yield return _items[i];
			}

			yield break;
		}
	}
}
