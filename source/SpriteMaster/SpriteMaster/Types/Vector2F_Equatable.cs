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
using System.Runtime.CompilerServices;

namespace SpriteMaster.Types;

partial struct Vector2F :
	IEquatable<Vector2F>,
	IEquatable<(float, float)>,
	IEquatable<XNA.Vector2>
{
	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly override bool Equals(object? other) => other switch {
		Vector2F vec => Equals(vec),
		Vector2I vec => Equals(vec),
		XNA.Vector2 vec => Equals(vec),
		Tuple<float, float> vector => Equals(new Vector2F(vector.Item1, vector.Item2)),
		ValueTuple<float, float> vector => Equals(vector),
		_ => false,
	};

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly bool Equals(Vector2F other) => NumericVector == other.NumericVector;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly bool Equals((float, float) other) => this == (Vector2F)other;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly bool Equals(in (float X, float Y) other) => this == (Vector2F)other;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly bool Equals(Vector2I other) => this == (Vector2F)other;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly bool Equals(XNA.Vector2 other) => this == (Vector2F)other;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator ==(Vector2F lhs, Vector2F rhs) => lhs.NumericVector == rhs.NumericVector;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator !=(Vector2F lhs, Vector2F rhs) => lhs.NumericVector != rhs.NumericVector;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator ==(Vector2F lhs, in (float X, float Y) rhs) => lhs.Equals(rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator !=(Vector2F lhs, in (float X, float Y) rhs) => !(lhs == rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator ==(in (float X, float Y) lhs, Vector2F rhs) => rhs.Equals(lhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator !=(in (float X, float Y) lhs, Vector2F rhs) => !(rhs == lhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator ==(Vector2F lhs, Vector2I rhs) => lhs.Equals(rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator !=(Vector2F lhs, Vector2I rhs) => !(lhs == rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator ==(Vector2I lhs, Vector2F rhs) => rhs.Equals(lhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator !=(Vector2I lhs, Vector2F rhs) => !(rhs == lhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator ==(Vector2F lhs, XNA.Vector2 rhs) => lhs.Equals(rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator !=(Vector2F lhs, XNA.Vector2 rhs) => !(lhs == rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator ==(XNA.Vector2 lhs, Vector2F rhs) => rhs.Equals(lhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator !=(XNA.Vector2 lhs, Vector2F rhs) => !(rhs == lhs);
}
