/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MusicMaster.Types;
internal class ConcurrentSet<T> :
	ICollection<T>,
	IEnumerable<T>,
	IEnumerable,
	ISet<T>,
	IReadOnlyCollection<T>,
	IReadOnlySet<T>
	where T : notnull {
	private static int DefaultConcurrencyLevel => Environment.ProcessorCount;
	private const int DefaultCapacity = 31;
	private const byte ValueValue = 0;

	private readonly ConcurrentDictionary<T, byte> Container;

	internal List<T> Copy => Container.Keys.ToList();

	internal void CopyTo(List<T> list) {
		foreach (var element in Container.Keys) {
			list.Add(element);
		}
	}

	#region Constructors

	internal ConcurrentSet() :
		this(
			concurrencyLevel: DefaultConcurrencyLevel,
			capacity: DefaultCapacity,
			comparer: null
		)
	{
	}

	internal ConcurrentSet(int concurrencyLevel, int capacity) :
		this(
			concurrencyLevel: concurrencyLevel,
			capacity: capacity,
			comparer: null
		)
	{
	}

	internal ConcurrentSet(IEqualityComparer<T>? comparer) :
		this(
			concurrencyLevel: DefaultConcurrencyLevel,
			capacity: DefaultCapacity,
			comparer: comparer
		) {
	}

	internal ConcurrentSet(int concurrencyLevel, int capacity, IEqualityComparer<T>? comparer) {
		Container = new(
			concurrencyLevel: concurrencyLevel,
			capacity: capacity,
			comparer: comparer
		);
	}

	internal ConcurrentSet(IEnumerable<T> collection) :
		this(
			concurrencyLevel: DefaultConcurrencyLevel,
			collection: collection,
			comparer: null
		)
	{
	}

	internal ConcurrentSet(IEnumerable<T> collection, IEqualityComparer<T>? comparer) :
		this(
			collection: collection,
			concurrencyLevel: DefaultConcurrencyLevel,
			comparer: comparer
		) {
	}

	internal ConcurrentSet(int concurrencyLevel, IEnumerable<T> collection, IEqualityComparer<T>? comparer) {
		Container = new(
			concurrencyLevel: concurrencyLevel,
			collection: collection.Select(MakeKv),
			comparer: comparer
		);
	}

	#endregion

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static KeyValuePair<T, byte> MakeKv(T key) => new(key, ValueValue);

	public IEnumerator<T> GetEnumerator() => Container.Keys.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => Container.Keys.GetEnumerator();

	void ICollection<T>.Add(T item) => _ = Container.TryAdd(item, ValueValue);

	public bool Add(T item) => Container.TryAdd(item, ValueValue);

	[Obsolete]
	public void ExceptWith(IEnumerable<T> other) => throw new NotImplementedException();

	[Obsolete]
	public void IntersectWith(IEnumerable<T> other) => throw new NotImplementedException();

	public bool Contains(T item) => Container.ContainsKey(item);

	[Obsolete]
	public bool IsProperSubsetOf(IEnumerable<T> other) => throw new NotImplementedException();

	[Obsolete]
	public bool IsProperSupersetOf(IEnumerable<T> other) => throw new NotImplementedException();

	[Obsolete]
	public bool IsSubsetOf(IEnumerable<T> other) => throw new NotImplementedException();

	[Obsolete]
	public bool IsSupersetOf(IEnumerable<T> other) => throw new NotImplementedException();

	[Obsolete]
	public bool Overlaps(IEnumerable<T> other) => throw new NotImplementedException();

	[Obsolete]
	public bool SetEquals(IEnumerable<T> other) => throw new NotImplementedException();

	[Obsolete]
	public void SymmetricExceptWith(IEnumerable<T> other) => throw new NotImplementedException();

	[Obsolete]
	public void UnionWith(IEnumerable<T> other) => throw new NotImplementedException();

	public void Clear() => Container.Clear();

	public void CopyTo(T[] array, int arrayIndex) => Container.Keys.CopyTo(array, arrayIndex);

	public T[] ToArray() => Container.Keys.ToArray();

	public bool Remove(T item) => Container.Remove(item, out _);

	public int Count => Container.Count;

	public bool IsReadOnly => false;

	internal bool TryTake([NotNullWhen(true)] out T? result) {
		while (!Container.IsEmpty) {
			var key = Container.Keys.FirstOrDefault();
			if (key is null) {
				break;
			}

			if (!Container.TryRemove(key, out _)) {
				continue;
			}

			result = key;
			return true;

			// Otherwise the collection was mutated, and we need to try again.
		}

		result = default;
		return false;
	}
}
