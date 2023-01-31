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
using System.Runtime.Serialization;
using System.Text;

namespace Leclair.Stardew.Common.UI.Widgets;

public class KChildList : ICollection<KWidget>, IEnumerable<KWidget>, IReadOnlyCollection<KWidget>, IReadOnlySet<KWidget>, ISet<KWidget>, ICollection, IDeserializationCallback, ISerializable {

	private readonly KWidget Owner;

	public KChildList(KWidget owner) {
		Owner = owner;
	}

	public int Count => throw new NotImplementedException();

	public bool IsReadOnly => throw new NotImplementedException();

	#region Set Implementation

	public bool Add(KWidget child) {
		// Make sure we have a child list.
		if (Owner._Children is null)
			Owner._Children = new();

		// If the widget is already a child, just return now.
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
		child._Parent?._Children?.Remove(child);

		child._Parent = Owner;
		return Owner._Children.Add(child);
	}

	public void Clear() {
		if (Owner._Children is null)
			return;

		foreach (var child in Owner._Children)
			child._Parent = null;

		Owner._Children.Clear();
	}

	public bool Contains(KWidget child) {
		return Owner._Children is not null && Owner._Children.Contains(child);
	}

	public void CopyTo(KWidget[] array, int index) {
		Owner._Children?.CopyTo(array, index);
	}

	public void ExceptWith(IEnumerable<KWidget> other) {
		throw new NotImplementedException();
	}

	public IEnumerator<KWidget> GetEnumerator() {
		throw new NotImplementedException();
	}

	public void IntersectWith(IEnumerable<KWidget> other) {
		throw new NotImplementedException();
	}

	public bool IsProperSubsetOf(IEnumerable<KWidget> other) {
		throw new NotImplementedException();
	}

	public bool IsProperSupersetOf(IEnumerable<KWidget> other) {
		throw new NotImplementedException();
	}

	public bool IsSubsetOf(IEnumerable<KWidget> other) {
		throw new NotImplementedException();
	}

	public bool IsSupersetOf(IEnumerable<KWidget> other) {
		throw new NotImplementedException();
	}

	public bool Overlaps(IEnumerable<KWidget> other) {
		throw new NotImplementedException();
	}

	public bool Remove(KWidget item) {
		throw new NotImplementedException();
	}

	public bool SetEquals(IEnumerable<KWidget> other) {
		throw new NotImplementedException();
	}

	public void SymmetricExceptWith(IEnumerable<KWidget> other) {
		throw new NotImplementedException();
	}

	public void UnionWith(IEnumerable<KWidget> other) {
		throw new NotImplementedException();
	}

	void ICollection<KWidget>.Add(KWidget item) {
		throw new NotImplementedException();
	}

	IEnumerator IEnumerable.GetEnumerator() {
		throw new NotImplementedException();
	}




	#endregion

}
