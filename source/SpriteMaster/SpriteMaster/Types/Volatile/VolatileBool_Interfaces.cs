/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Extensions;
using System;

namespace SpriteMaster.Types.Volatile;

partial struct VolatileBool :
	IComparable,
	IComparable<VolatileBool?>,
	IComparable<VolatileBool>,
	IComparable<bool?>,
	IComparable<bool>,
	IEquatable<VolatileBool?>,
	IEquatable<VolatileBool>,
	IEquatable<bool?>,
	IEquatable<bool> {
	#region Comparable

	readonly int IComparable.CompareTo(object? obj) => obj switch {
		VolatileBool other => Value.CompareTo(other.Value),
		bool other => Value.CompareTo(other),
		_ => throw new ArgumentException(Exceptions.BuildArgumentException(nameof(obj), obj))
	};

	readonly int IComparable<VolatileBool?>.CompareTo(VolatileBool? other) => other is VolatileBool otherBool ? Value.CompareTo(otherBool.Value) : throw new ArgumentException(nameof(other));
	readonly int IComparable<VolatileBool>.CompareTo(VolatileBool other) => Value.CompareTo(other.Value);

	readonly int IComparable<bool?>.CompareTo(bool? other) => other is bool otherBool ? Value.CompareTo(otherBool) : throw new ArgumentException(nameof(other));
	readonly int IComparable<bool>.CompareTo(bool other) => Value.CompareTo(other);

	#endregion

	#region Equatable

	readonly bool IEquatable<VolatileBool?>.Equals(VolatileBool? other) => other is VolatileBool otherBool && _Value == otherBool._Value;
	readonly bool IEquatable<VolatileBool>.Equals(VolatileBool other) => _Value == other._Value;
	internal readonly bool Equals(in VolatileBool? other) => other is VolatileBool otherBool && _Value == otherBool._Value;

	readonly bool IEquatable<bool?>.Equals(bool? other) => other is bool otherBool && _Value == otherBool;
	readonly bool IEquatable<bool>.Equals(bool other) => _Value == other;
	internal readonly bool Equals(in bool? other) => other is bool otherBool && _Value == otherBool;

	#endregion
}
