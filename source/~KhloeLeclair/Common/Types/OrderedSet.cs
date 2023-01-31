/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using StardewValley.Menus;

namespace Leclair.Stardew.Common.Types;

public class OrderedSet<T> : ICollection<T>, IEnumerable<T>, IReadOnlyCollection<T>, IReadOnlyList<T>, IList<T>, IReadOnlySet<T>, ISet<T>, ICollection where T : notnull {

	private readonly IEqualityComparer<T> _Comparer;
	private readonly IDictionary<T, LinkedListNode<T>> _Dictionary;
	private readonly LinkedList<T> _List;

	public OrderedSet() : this(EqualityComparer<T>.Default) { }

	public OrderedSet(IEqualityComparer<T> comparer) {
		_Comparer = comparer;
		_Dictionary = new Dictionary<T, LinkedListNode<T>>(comparer);
		_List = new LinkedList<T>();
	}

	public virtual bool IsReadOnly => _Dictionary.IsReadOnly;

	#region Basic Access

	public int Count => _Dictionary.Count;

	IEnumerator IEnumerable.GetEnumerator() {
		return _List.GetEnumerator();
	}

	public IEnumerator<T> GetEnumerator() {
		return _List.GetEnumerator();
	}

	#endregion

	#region Basic Manipulation

	void ICollection<T>.Add(T item) {
		Add(item);
	}

	public bool Add(T item) {
		if (_Dictionary.ContainsKey(item)) return false;
		LinkedListNode<T> node = _List.AddLast(item);
		_Dictionary.Add(item, node);
		return true;
	}

	public void Clear() {
		_Dictionary.Clear();
		_List.Clear();
	}

	public bool Remove(T item) {
		if (!_Dictionary.TryGetValue(item, out LinkedListNode<T>? node))
			return false;

		_Dictionary.Remove(item);
		_List.Remove(node);
		return true;
	}

	#endregion

	#region Order Manipulation

	/// <summary>
	/// Move the specified item to the start of the collection.
	///
	/// If the item is not in the collection, returns <c>false</c>.
	/// If the item is in the collection, but is already the first
	/// entry in the collection, returns false.
	///
	/// Returns true if the item is in the collection and was moved.
	/// </summary>
	/// <param name="item">The item to move.</param>
	public bool MoveToStart(T item) {
		if (!_Dictionary.TryGetValue(item, out LinkedListNode<T>? node))
			return false;

		if (_List.First == node)
			return false;

		_List.Remove(node);
		_List.AddFirst(node);
		return true;
	}

	public bool MoveBefore(T item, T sibling) {
		if (!_Dictionary.TryGetValue(item, out LinkedListNode<T>? node) || !_Dictionary.TryGetValue(sibling, out LinkedListNode<T>? siblingNode))
			return false;

		if (siblingNode.Previous == node)
			return false;

		_List.Remove(node);
		_List.AddBefore(siblingNode, node);
		return true;
	}

	public bool MoveAfter(T item, T sibling) {
		if (!_Dictionary.TryGetValue(item, out LinkedListNode<T>? node) || !_Dictionary.TryGetValue(sibling, out LinkedListNode<T>? siblingNode))
			return false;

		if (siblingNode.Next == node)
			return false;

		_List.Remove(node);
		_List.AddAfter(siblingNode, node);
		return true;
	}

	/// <summary>
	/// Move the specified item to the end of the collection.
	///
	/// If the item is not in the collection, returns <c>false</c>.
	/// If the item is in the collection, but is already the last
	/// entry in the collection, returns false.
	///
	/// Returns true if the item is in the collection and was moved.
	/// </summary>
	/// <param name="item">The item to move.</param>
	public bool MoveToEnd(T item) {
		if (!_Dictionary.TryGetValue(item, out LinkedListNode<T>? node))
			return false;

		if (_List.Last == node)
			return false;

		_List.Remove(node);
		_List.AddLast(node);
		return true;
	}

	#endregion

	#region ICollection Implementation

	public bool IsSynchronized => ((ICollection) _Dictionary).IsSynchronized;

	public object SyncRoot => ((ICollection) _Dictionary).SyncRoot;

	public void CopyTo(Array array, int index) {
		if (array != null && array.Rank != 1)
			throw new ArgumentException("Multidimensional arrays are not supported", nameof(array));

		if (array is not T[] tarray)
			throw new ArgumentException("Invalid array type", nameof(array));

		_List.CopyTo(tarray, index);
	}

	#endregion

	#region List Implementation

	public int IndexOf(T item) {
		// O(1) check to see if we even have it.
		if (!_Dictionary.ContainsKey(item))
			return -1;

		// Loop through the list.
		int i = 0;
		var node = _List.First;
		while (node is not null) {
			if (_Comparer.Equals(node.Value, item))
				return i;
			i++;
			node = node.Next;
		}

		return -1;
	}

	void IList<T>.Insert(int index, T item) {
		Insert(index, item);
	}

	public bool Insert(int index, T item) {
		// Make sure the index is within range.
		if (index < 0 || index > _Dictionary.Count)
			throw new ArgumentOutOfRangeException(nameof(index));

		// If the index is equal to our count, we're just adding to the
		// end so use the standard Add logic.
		if (index == _Dictionary.Count)
			return Add(item);

		// We do this after potentially calling Add only to avoid
		// this check running twice in that case.
		if (_Dictionary.ContainsKey(item)) return false;

		// Find the node we're inserting after.
		LinkedListNode<T>? node = _List.First;
		while (index > 0)
			node = node?.Next;

		// If we somehow ran out of nodes, throw an exception.
		if (node is null)
			throw new ArgumentOutOfRangeException(nameof(index));

		LinkedListNode<T> newNode = _List.AddAfter(node, item);
		_Dictionary.Add(item, newNode);
		return true;
	}

	void IList<T>.RemoveAt(int index) {
		RemoveAt(index);
	}

	public void RemoveAt(int index) {
		// Make sure the index is within range.
		if (index < 0 || index >= _Dictionary.Count)
			throw new ArgumentOutOfRangeException(nameof(index));

		// If we only have one item, clear.
		if (_Dictionary.Count == 1) {
			Clear();
			return;
		}

		// Find the node we're removing.
		LinkedListNode<T>? node = _List.First;
		while (index > 0)
			node = node?.Next;

		// If we somehow ran out of nodes, throw an exception.
		if (node is null)
			throw new ArgumentOutOfRangeException(nameof(index));

		// Remove the entry.
		_Dictionary.Remove(node.Value);
		_List.Remove(node);
	}

	public T this[int index] {
		get {
			// Make sure the index is within range.
			if (index < 0 || index >= _Dictionary.Count)
				throw new ArgumentOutOfRangeException(nameof(index));

			// Find the node we're returning.
			LinkedListNode<T>? node = _List.First;
			while (index > 0)
				node = node?.Next;

			// If we somehow ran out of nodes, throw an exception.
			if (node is null)
				throw new ArgumentOutOfRangeException(nameof(index));

			return node.Value;
		}
		set {
			// Make sure the index is within range.
			if (index < 0 || index >= _Dictionary.Count)
				throw new ArgumentOutOfRangeException(nameof(index));

			// Find the node we're returning.
			LinkedListNode<T>? node = _List.First;
			while (index > 0)
				node = node?.Next;

			// If we somehow ran out of nodes, throw an exception.
			if (node is null)
				throw new ArgumentOutOfRangeException(nameof(index));

			// If this node already contains this value, that's okay.
			if (_Comparer.Equals(node.Value, value))
				return;

			// If the value exists anywhere else though, that is not okay.
			if (_Dictionary.ContainsKey(value))
				throw new ArgumentException("Set already contains value");

			// Update the dictionary and change our node's value.
			_Dictionary.Remove(node.Value);
			node.Value = value;
			_Dictionary.Add(value, node);
		}
	}

	#endregion

	#region Set Implementation

	public bool Contains(T item) {
		return _Dictionary.ContainsKey(item);
	}

	public void CopyTo(T[] array, int index) {
		_List.CopyTo(array, index);
	}

	public void ExceptWith(IEnumerable<T> other) {
		foreach (T item in other)
			Remove(item);
	}

	public void IntersectWith(IEnumerable<T> other) {
		if (other is null)
			throw new ArgumentNullException(nameof(other));

		if (_Dictionary.Count == 0 || other == this)
			return;

		if (other is ICollection<T> collection && collection.Count == 0) {
			Clear();
			return;
		}

		IntersectWithEnumerable(other);
	}

	private void IntersectWithEnumerable(IEnumerable<T> other) {
		// This is not at all optimized, but we never use this ourselves.
		List<T> existing = _Dictionary.Keys.ToList();

		foreach(T item in other)
			existing.Remove(item);

		foreach (T item in existing)
			Remove(item);
	}

	public bool IsProperSubsetOf(IEnumerable<T> other) {
		if (other is null)
			throw new ArgumentNullException(nameof(other));

		if (other == this)
			return false;

		if (other is ICollection<T> collection) {
			if (collection.Count == 0)
				return false;
			if (_Dictionary.Count == 0)
				return collection.Count > 0;
		}

		var (unique, unfound) = CheckUniqueAndUnfoundElements(other, returnIfUnfound: false);
		return unique == _Dictionary.Count && unfound > 0;
	}

	public bool IsSubsetOf(IEnumerable<T> other) {
		if (other is null)
			throw new ArgumentNullException(nameof(other));

		if (_Dictionary.Count == 0 || other == this)
			return true;

		if (other is ICollection<T> collection && _Dictionary.Count > collection.Count)
			return false;

		var (unique, _) = CheckUniqueAndUnfoundElements(other, returnIfUnfound: false);
		return unique == _Dictionary.Count;
	}

	public bool IsProperSupersetOf(IEnumerable<T> other) {
		if (other is null)
			throw new ArgumentNullException(nameof(other));

		if (_Dictionary.Count == 0 || other == this)
			return false;

		if (other is ICollection<T> collection && collection.Count == 0)
			return true;

		var (unique, unfound) = CheckUniqueAndUnfoundElements(other, returnIfUnfound: true);
		return unique < _Dictionary.Count && unfound == 0;
	}

	public bool IsSupersetOf(IEnumerable<T> other) {
		if (other is null)
			throw new ArgumentNullException(nameof(other));

		if (other == this)
			return true;

		if (other is ICollection<T> collection ) {
			if (collection.Count == 0)
				return true;
			if (collection.Count > _Dictionary.Count)
				return false;
		}

		foreach (T item in other)
			if (!_Dictionary.ContainsKey(item))
				return false;

		return true;
	}

	public bool Overlaps(IEnumerable<T> other) {
		if (other is null)
			throw new ArgumentNullException(nameof(other));

		if (_Dictionary.Count == 0)
			return false;

		if (other == this)
			return true;

		foreach (T item in other)
			if (_Dictionary.ContainsKey(item))
				return true;

		return false;
	}

	public bool SetEquals(IEnumerable<T> other) {
		if (other is null)
			throw new ArgumentNullException(nameof(other));

		if (other == this)
			return true;

		if (other is ICollection<T> collection && collection.Count != _Dictionary.Count)
			return false;

		var (unique, unfound) = CheckUniqueAndUnfoundElements(other, returnIfUnfound: true);
		return unique == _Dictionary.Count && unfound == 0;
	}

	public void SymmetricExceptWith(IEnumerable<T> other) {
		if (other is null)
			throw new ArgumentNullException(nameof(other));

		if (_Dictionary.Count == 0)
			UnionWith(other);

		else if (other == this)
			Clear();

		else {
			HashSet<T> removed = new HashSet<T>(_Comparer);

			foreach (T item in other) {
				if (Remove(item))
					removed.Add(item);
				else if (! removed.Contains(item))
					Add(item);
			}
		}
	}

	public void UnionWith(IEnumerable<T> other) {
		if (other is null)
			throw new ArgumentNullException(nameof(other));

		foreach (T item in other)
			Add(item);
	}

	private (int UniqueCount, int UnfoundCount) CheckUniqueAndUnfoundElements(IEnumerable<T> other, bool returnIfUnfound = false) {
		if (_Dictionary.Count == 0)
			return (0, other.Count());

		int unique = 0;
		int unfound = 0;

		int count = _Dictionary.Count;
		int length = BitHelper.ToIntArrayLength(count);
		Span<int> data = length <= 100 ? stackalloc int[length] : GC.AllocateUninitializedArray<int>(length);
		BitHelper helper = new BitHelper(data, clear: true);

		foreach(T item in other) {
			int idx = IndexOf(item);
			if (idx != -1) {
				if (!helper.IsMarked(idx)) {
					helper.Mark(idx);
					unique++;
				}
			} else {
				unfound++;
				if (returnIfUnfound)
					break;
			}
		}

		return (unique, unfound);
	}

	#endregion

}
