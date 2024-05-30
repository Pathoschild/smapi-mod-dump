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
using System.Collections.Generic;

namespace Leclair.Stardew.Common.Types;

public class ValueEqualityList<TValue> : List<TValue> {

	private readonly IEqualityComparer<TValue>? equalityComparer;

	public IEqualityComparer<TValue> Comparer => equalityComparer ?? EqualityComparer<TValue>.Default;

	public ValueEqualityList() : base() { }

	public ValueEqualityList(IEqualityComparer<TValue> comparer) : base() {
		equalityComparer = comparer;
	}

	public ValueEqualityList(int capacity) : base(capacity) { }

	public ValueEqualityList(int capacity, IEqualityComparer<TValue> comparer) : base(capacity) {
		equalityComparer = comparer;
	}

	public ValueEqualityList(IEnumerable<TValue> values) : base(values) { }

	public ValueEqualityList(IEnumerable<TValue> values, IEqualityComparer<TValue> comparer) : base(values) {
		equalityComparer = comparer;
	}

	public override bool Equals(object? obj) {
		if (obj is not IList<TValue> olist || olist.Count != Count)
			return false;

		IEqualityComparer<TValue> comparer = Comparer;

		for (int i = 0; i < Count; i++) {
			if (!comparer.Equals(this[i], olist[i]))
				return false;
		}

		return true;
	}

	public override int GetHashCode() {
		var hash = new HashCode();
		foreach (var item in this)
			hash.Add(item, Comparer);
		return hash.ToHashCode();
	}

}
