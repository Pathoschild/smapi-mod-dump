/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#if COMMON_WIDGETS

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Leclair.Stardew.Common.UI.Widgets;

public class KChildSet : ICollection<KObject>, IEnumerable<KObject>, IReadOnlyCollection<KObject>, IReadOnlySet<KObject>, ISet<KObject>, ICollection {

	private readonly KObject Owner;

	public KChildSet(KObject owner) {
		Owner = owner;
	}

	public bool MoveBefore(KObject child, KObject sibling) {
		return Owner._Children?.MoveBefore(child, sibling) ?? false;
	}

	public bool MoveAfter(KObject child, KObject sibling) {
		return Owner._Children?.MoveAfter(child, sibling) ?? false;
	}

	public bool MoveToStart(KObject child) {
		return Owner._Children?.MoveToStart(child) ?? false;
	}

	public bool MoveToEnd(KObject child) {
		return Owner._Children?.MoveToEnd(child) ?? false;
	}

	#region Set Implementation

	public bool Add(KObject child) {
		// Make sure we have a child list.
		if (Owner._Children is null)
			Owner._Children = new();

		// If the object is already a child, just return now.
		else if (Owner._Children.Contains(child))
			return false;

		// Sanity check that we're not creating a loop.
		var node = Owner;
		while (node is not null) {
			if (node == child)
				throw new Exception($"Adding '{child}' as a child of '{Owner}' would create an infinite loop.");

			node = node._Parent;
		}

		// Remove the child from its old parent.
		KObject? oldParent = child._Parent;
		if (oldParent is not null && !oldParent.Children.Remove(child))
			throw new Exception($"Unable to remove '{child}' from existing parent '{oldParent}'.");

		// Add it to the new parent.
		bool added = Owner._Children.Add(child);
		if (added)
			child._Parent = Owner;

		// Dispatch a parent changed event to the child.
		if (child._Parent != oldParent)
			child.OnParentChanged(oldParent, child._Parent);

		return added;
	}

	public void Clear() {
		if (Owner._Children is null)
			return;

		foreach (var child in Owner._Children)
			child._Parent = null;

		Owner._Children.Clear();
	}

	public bool Contains(KObject child) {
		return Owner._Children is not null && Owner._Children.Contains(child);
	}

	public void CopyTo(KObject[] array, int index) {
		Owner._Children?.CopyTo(array, index);
	}

	public void ExceptWith(IEnumerable<KObject> other) {
		if (Owner._Children is not null)
			foreach (KObject child in other)
				Remove(child);
	}

	public IEnumerator<KObject> GetEnumerator() {
		if (Owner._Children is null)
			return Empty();

		return Owner._Children.GetEnumerator();
	}

	public void IntersectWith(IEnumerable<KObject> other) {
		throw new NotImplementedException();
	}

	public bool IsProperSubsetOf(IEnumerable<KObject> other) {
		if (other is null)
			throw new ArgumentNullException(nameof(other));

		// If we have no children, then we just need to know that other has at
		// least a single entry.
		if (Owner._Children is null)
			return other.Any();

		return Owner._Children.IsProperSubsetOf(other);
	}

	public bool IsProperSupersetOf(IEnumerable<KObject> other) {
		if (other is null)
			throw new ArgumentNullException(nameof(other));

		// If we have no children, we can't be a proper superset of anything.
		if (Owner._Children is null)
			return false;

		return Owner._Children.IsProperSupersetOf(other);
	}

	public bool IsSubsetOf(IEnumerable<KObject> other) {
		if (other is null)
			throw new ArgumentNullException(nameof(other));

		// If we have no children, then we're a subset of every other set.
		// Even empty sets.
		if (Owner._Children is null)
			return true;

		return Owner._Children.IsSubsetOf(other);
	}

	public bool IsSupersetOf(IEnumerable<KObject> other) {
		if (other is null)
			throw new ArgumentNullException(nameof(other));

		// If we have no children, then we're a superset of empty collections
		// but not of collections with any items.
		if (Owner._Children is null)
			return !other.Any();

		return Owner._Children.IsSupersetOf(other);
	}

	public bool Overlaps(IEnumerable<KObject> other) {
		if (other is null)
			throw new ArgumentNullException(nameof(other));

		// We cannot overlap if we have no children.
		if (Owner._Children is null)
			return false;

		return Owner._Children.Overlaps(other);
	}

	public bool Remove(KObject child) {
		// If we do remove the child, make sure to update its parent.
		if (Owner._Children is not null && Owner._Children.Remove(child)) {
			child._Parent = null;
			return true;
		}

		return false;
	}

	public bool SetEquals(IEnumerable<KObject> other) {
		if (other is null)
			throw new ArgumentNullException(nameof(other));

		if (Owner._Children is null)
			return !other.Any();

		return Owner._Children.SetEquals(other);
	}

	public void SymmetricExceptWith(IEnumerable<KObject> other) {
		throw new NotImplementedException();
	}

	public void UnionWith(IEnumerable<KObject> other) {
		foreach (KObject child in other)
			Add(child);
	}

	void ICollection<KObject>.Add(KObject child) {
		Add(child);
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return GetEnumerator();
	}

	private static IEnumerator<KObject> Empty() {
		yield break;
	}

	public void CopyTo(Array array, int index) {
		throw new NotImplementedException();
	}

	#endregion

	#region ICollection Stuff

	public int Count => Owner._Children?.Count ?? 0;

	public bool IsReadOnly => false;

	public bool IsSynchronized => false;

	public object SyncRoot {
		get {
			Owner._Children ??= new();
			return (Owner._Children as ICollection).SyncRoot;
		}
	}

	#endregion

}

#endif
