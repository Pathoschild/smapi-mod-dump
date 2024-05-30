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

using Newtonsoft.Json.Linq;

namespace Leclair.Stardew.Common.Types;

/// <summary>
/// This is a <see cref="Dictionary{TKey, TValue}"/> subclass that overrides
/// <see cref="Equals(object?)"/> and <see cref="GetHashCode"/> to use
/// value-based equality checking. This allows you to use this dictionary
/// within record objects and still have robust equality checking behavior.
/// </summary>
public class ValueEqualityDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IEquatable<IDictionary<TKey, TValue>> where TKey : notnull {

	private readonly IEqualityComparer<TValue>? equalityComparer;

	public IEqualityComparer<TValue> ValueComparer => equalityComparer ?? EqualityComparer<TValue>.Default;

	public ValueEqualityDictionary() : base() { }
	public ValueEqualityDictionary(IEqualityComparer<TValue>? valueComparer) : base() {
		equalityComparer = valueComparer;
	}
	public ValueEqualityDictionary(IEqualityComparer<TKey>? keyComparer, IEqualityComparer<TValue>? valueComparer) : base(keyComparer) {
		equalityComparer = valueComparer;
	}
	public ValueEqualityDictionary(int capacity) : base(capacity) { }
	public ValueEqualityDictionary(int capacity, IEqualityComparer<TValue>? valueComparer) : base(capacity) {
		equalityComparer = valueComparer;
	}
	public ValueEqualityDictionary(int capacity, IEqualityComparer<TKey>? keyComparer, IEqualityComparer<TValue>? valueComparer) : base(capacity, keyComparer) {
		equalityComparer = valueComparer;
	}
	public ValueEqualityDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary) { }
	public ValueEqualityDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TValue>? valueComparer) : base(dictionary) {
		equalityComparer = valueComparer;
	}
	public ValueEqualityDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey>? keyComparer, IEqualityComparer<TValue>? valueComparer) : base(dictionary, keyComparer) {
		equalityComparer = valueComparer;
	}
	public ValueEqualityDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection) : base(collection) { }
	public ValueEqualityDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TValue>? valueComparer) : base(collection) {
		equalityComparer = valueComparer;
	}
	public ValueEqualityDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey>? keyComparer, IEqualityComparer<TValue>? valueComparer) : base(collection, keyComparer) {
		equalityComparer = valueComparer;
	}

	public override bool Equals(object? obj) {
		if (obj is not IDictionary<TKey, TValue> odict || odict.Count != Count)
			return false;

		IEqualityComparer<TValue> comparer = ValueComparer;

		foreach (var pair in this)
			if (!odict.TryGetValue(pair.Key, out TValue? other) || !comparer.Equals(pair.Value, other))
				return false;

		return true;
	}

	public override int GetHashCode() {
		var hash = new HashCode();
		foreach (var pair in this) {
			hash.Add(pair.Key, Comparer);
			hash.Add(pair.Value, ValueComparer);
		}
		return hash.ToHashCode();
	}

	public bool Equals(ValueEqualityDictionary<TKey, TValue>? other) {
		return Equals((object?) other);
	}

	public bool Equals(IDictionary<TKey, TValue>? other) {
		return Equals((object?) other);
	}

	public static bool operator ==(ValueEqualityDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second) {
		return first is null ? second is null : first.Equals(second);
	}

	public static bool operator !=(ValueEqualityDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second) {
		return first is null ? second is not null : !first.Equals(second);
	}

}


public class FieldsEqualityDictionary : ValueEqualityDictionary<string, JToken> {

	public readonly static JTokenEqualityComparer TokenComparer = new();

	public FieldsEqualityDictionary() : base(TokenComparer) { }

	public FieldsEqualityDictionary(int capacity) : base(capacity, TokenComparer) { }

	public FieldsEqualityDictionary(IDictionary<string, JToken> dictionary) : base(dictionary, TokenComparer) { }

	public FieldsEqualityDictionary(IEnumerable<KeyValuePair<string, JToken>> collection) : base(collection, TokenComparer) { }

}
