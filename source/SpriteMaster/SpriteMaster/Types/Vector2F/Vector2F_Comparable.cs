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
using System.Runtime.CompilerServices;

namespace SpriteMaster.Types;

internal partial struct Vector2F :
	IComparable,
	IComparable<Vector2F>,
	IComparable<(float, float)>,
	IComparable<XVector2> {
	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly int CompareTo(Vector2F other) {
		var result = X.CompareTo(other.X);
		if (result == 0) {
			return Y.CompareTo(other.Y);
		}
		return result;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly int CompareTo((float, float) other) => CompareTo((Vector2F)other);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly int CompareTo(Vector2I other) => CompareTo((Vector2F)other);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly int CompareTo(XVector2 other) => CompareTo((Vector2F)other);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	readonly int IComparable.CompareTo(object? other) => other switch {
		Vector2F vec => CompareTo(vec),
		Vector2I vec => CompareTo(vec),
		XVector2 vec => CompareTo(vec),
		Tuple<float, float> vector => CompareTo(new Vector2F(vector.Item1, vector.Item2)),
		ValueTuple<float, float> vector => CompareTo(vector),
		_ => Extensions.Exceptions.ThrowArgumentException<int>(nameof(other), other)
	};
}
