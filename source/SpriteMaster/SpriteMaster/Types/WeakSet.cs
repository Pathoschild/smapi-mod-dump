/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;

namespace SpriteMaster.Types;

sealed class WeakSet<T> : 
	IEnumerable<T>,
	IEnumerable,
	ICollection<T>,
	IReadOnlyCollection<T>,
	ISet<T>,
	IReadOnlySet<T>
	where T : class {
	private const object? Sentinel = null;

	private readonly ConditionalWeakTable<T, object> InternalTable = new();
	private readonly SharedLock Lock = new();

	private IEnumerable<T> Items => InternalTable.Select(kv => kv.Key);

	public int Count {
		[MethodImpl(Runtime.MethodImpl.Hot)]
		get {
			using (Lock.Read) {
				return InternalTable.Count();
			}
		}
	}

	public bool IsReadOnly => false;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	[SecuritySafeCritical]
	internal bool Contains(T item) {
		using (Lock.Read) {
			return InternalTable.TryGetValue(item, out var _);
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	bool ICollection<T>.Contains(T item) => Contains(item);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	bool IReadOnlySet<T>.Contains(T item) => Contains(item);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	[SecuritySafeCritical]
	internal bool Remove(T item) {
		using (Lock.Write) {
			return InternalTable.Remove(item);
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	bool ICollection<T>.Remove(T item) => Remove(item);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	[SecuritySafeCritical]
	internal bool Add(T item) {
		try {
			using (Lock.ReadWrite) {
				if (InternalTable.TryGetValue(item, out var _)) {
					return false;
				}

				AddOrIgnore(item);

				return true;
			}
		}
		catch (ArgumentException) {
			return false;
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	[SecuritySafeCritical]
	internal void AddOrIgnore(T item) {
		using (Lock.Write) {
			InternalTable.AddOrUpdate(item, Sentinel!);
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal void AddRange(IEnumerable<T> collection) {
		// TODO : This can be improved upon using reflection/delegates.

		using (Lock.Write) {
			foreach (var item in collection.Distinct()) {
				InternalTable.AddOrUpdate(item, Sentinel!);
			}
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal void RemoveRange(IEnumerable<T> collection) {
		// TODO : This can be improved upon using reflection/delegates.

		using (Lock.Write) {
			foreach (var item in collection.Distinct()) {
				InternalTable.Remove(item);
			}
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public void Clear() {
		using (Lock.Write) {
			InternalTable.Clear();
		}
	}

	public void CopyTo(T[] array, int arrayIndex) {
		// TODO : this can be implemented better
		using (Lock.Read) {
			InternalTable.ToArray().CopyTo(array, arrayIndex);
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	void ICollection<T>.Add(T item) => AddOrIgnore(item);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => this.GetEnumerator();

	bool ISet<T>.Add(T item) => Add(item);

	public bool IsProperSubsetOf(IEnumerable<T> other) => Items.ToHashSet().IsProperSubsetOf(other);

	public bool IsProperSupersetOf(IEnumerable<T> other) => Items.ToHashSet().IsProperSupersetOf(other);

	public bool IsSubsetOf(IEnumerable<T> other) => Items.ToHashSet().IsSubsetOf(other);

	public bool IsSupersetOf(IEnumerable<T> other) => Items.ToHashSet().IsSupersetOf(other);

	public bool Overlaps(IEnumerable<T> other) => Items.ToHashSet().Overlaps(other);

	public bool SetEquals(IEnumerable<T> other) => Items.ToHashSet().SetEquals(other);

	public void SymmetricExceptWith(IEnumerable<T> other) => Items.ToHashSet().SetEquals(other);

	public void UnionWith(IEnumerable<T> other) => AddRange(other);

	public void ExceptWith(IEnumerable<T> other) => RemoveRange(other);

	public void IntersectWith(IEnumerable<T> other) {
		var otherSet = other as ISet<T> ?? other.ToHashSet();
		AddRange(other);
		var current = Items;
		var currentCount = current.Count();

		if (currentCount > otherSet.Count) {
			var removeList = new List<T>(currentCount - otherSet.Count);
			foreach (var item in current) {
				if (!otherSet.Contains(item)) {
					removeList.Add(item);
				}
			}
			RemoveRange(removeList);
		}
	}
}
