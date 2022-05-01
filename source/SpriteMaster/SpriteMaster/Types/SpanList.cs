/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

#if false

using SpriteMaster.Types.DebugViews;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpriteMaster.Types;

[DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
[DebuggerDisplay("Count = {Count}")]
class SpanList<T> : IList<T>, IList, IReadOnlyList<T> {
	private static readonly T[] EmptyArray = new T[0];

	private T[] Items = EmptyArray;
	private int Size = 0;
#if !SHIPPING
	private int Version = 0;
#endif

	internal int Capacity {
		get => Items.Length;
		set {
			if (value < Size) {
				throw new ArgumentOutOfRangeException(nameof(value), "capacity cannot be smaller than size");
			}

			if (value == Size) {
				return;
			}

			if (value == 0) {
				Items = EmptyArray;
			}

			var newItems = new T[value];
			if (Size > 0) {
				Array.Copy(Items, newItems, Size);
			}
			Items = newItems;
		}
	}

	public int Count => Size;

	bool IList.IsFixedSize => false;

	bool ICollection<T>.IsReadOnly => false;

	bool IList.IsReadOnly => false;

	bool ICollection.IsSynchronized => false;

	object ICollection.SyncRoot => this;

	internal ref T this[int index] {
		get {
			if ((uint)index >= (uint)Size) {
				throw new ArgumentOutOfRangeException(nameof(index), $"{index} >= {Size}");
			}

			return ref Items[index];
		}
	}

	T IList<T>.this[int index] {
		get {
			if ((uint)index >= (uint)Size) {
				throw new ArgumentOutOfRangeException(nameof(index), $"{index} >= {Size}");
			}

			return Items[index];
		}

		set {
			if ((uint)index >= (uint)Size) {
				throw new ArgumentOutOfRangeException(nameof(index), $"{index} >= {Size}");
			}

			Items[index] = value;
		}
	}

	T IReadOnlyList<T>.this[int index] {
		get {
			if ((uint)index >= (uint)Size) {
				throw new ArgumentOutOfRangeException(nameof(index), $"{index} >= {Size}");
			}

			return Items[index];
		}
	}

	internal SpanList() {}

	internal SpanList(int capacity) {
		if (capacity < 0) {
			throw new ArgumentOutOfRangeException(nameof(capacity), "capacity must not be negative");
		}

		Items = (capacity == 0) ? EmptyArray : new T[capacity];
	}

	internal SpanList(IEnumerable<T> enumerable!!) {
		switch (enumerable) {
			case SpanList<T> list: {
					int count = list.Size;
					if (count == 0) {
						Items = EmptyArray;
					}
					else {
						Items = new T[count];
						Array.Copy(list.Items, Items, count);
						Size = count;
					}
				} break;
			case ICollection<T> collection: {
					int count = collection.Count;
					if (count == 0) {
						Items = EmptyArray;
					}
					else {
						Items = new T[count];
						collection.CopyTo(Items, 0);
						Size = count;
					}
				} break;
			case IReadOnlyCollection<T> collection: {
					int count = collection.Count;
					if (count == 0) {
						Items = EmptyArray;
					}
					else {
						Items = new T[count];
						int i = 0;
						foreach (T item in collection) {
							Items[i++] = item;
						}
						Size = count;
					}
				}
				break;
			default: {
					Items = EmptyArray;
					foreach (var item in enumerable) {
						Add(item);
					}
				} break;
		}
	}
}

#endif
