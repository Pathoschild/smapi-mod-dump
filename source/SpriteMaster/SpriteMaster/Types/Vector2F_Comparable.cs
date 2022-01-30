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

partial struct Vector2F :
	IComparable,
	IComparable<Vector2F>,
	IComparable<(float, float)>,
	IComparable<XNA.Vector2>
{
	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly int CompareTo(Vector2F other) {
		var result = X.CompareTo(other.X);
		if (result == 0) {
			return Y.CompareTo(other.Y);
		}
		return result;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly int CompareTo((float, float) other) => CompareTo((Vector2F)other);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly int CompareTo(Vector2I other) => CompareTo((Vector2F)other);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly int CompareTo(XNA.Vector2 other) => CompareTo((Vector2F)other);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	readonly int IComparable.CompareTo(object? other) => other switch {
		Vector2F vec => CompareTo(vec),
		Vector2I vec => CompareTo(vec),
		XNA.Vector2 vec => CompareTo(vec),
		Tuple<float, float> vector => CompareTo(new Vector2F(vector.Item1, vector.Item2)),
		ValueTuple<float, float> vector => CompareTo(vector),
		_ => throw new ArgumentException(Exceptions.BuildArgumentException(nameof(other), other))
	};
}
