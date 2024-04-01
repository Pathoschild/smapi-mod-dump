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
using System.Runtime.CompilerServices;

namespace SpriteMaster.Types;

internal partial struct Vector2F :
	IEquatable<Vector2F>,
	IEquatable<(float, float)>,
	IEquatable<XVector2> {
	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override readonly bool Equals(object? other) => other switch {
		Vector2F vec => Equals(vec),
		Vector2I vec => Equals(vec),
		XVector2 vec => Equals(vec),
		Tuple<float, float> vector => Equals(new Vector2F(vector.Item1, vector.Item2)),
		ValueTuple<float, float> vector => Equals(vector),
		_ => false,
	};

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly bool Equals(Vector2F other) => NumericVector == other.NumericVector;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly bool Equals((float, float) other) => this == (Vector2F)other;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly bool Equals(Vector2I other) => this == (Vector2F)other;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly bool Equals(XVector2 other) => this == (Vector2F)other;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator ==(Vector2F lhs, Vector2F rhs) => lhs.NumericVector == rhs.NumericVector;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator !=(Vector2F lhs, Vector2F rhs) => lhs.NumericVector != rhs.NumericVector;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator ==(Vector2F lhs, (float X, float Y) rhs) => lhs.Equals(rhs);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator !=(Vector2F lhs, (float X, float Y) rhs) => !(lhs == rhs);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator ==((float X, float Y) lhs, Vector2F rhs) => rhs.Equals(lhs);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator !=((float X, float Y) lhs, Vector2F rhs) => !(rhs == lhs);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator ==(Vector2F lhs, Vector2I rhs) => lhs.Equals(rhs);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator !=(Vector2F lhs, Vector2I rhs) => !(lhs == rhs);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator ==(Vector2I lhs, Vector2F rhs) => rhs.Equals(lhs);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator !=(Vector2I lhs, Vector2F rhs) => !(rhs == lhs);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator ==(Vector2F lhs, XVector2 rhs) => lhs.Equals(rhs);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator !=(Vector2F lhs, XVector2 rhs) => !(lhs == rhs);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator ==(XVector2 lhs, Vector2F rhs) => rhs.Equals(lhs);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator !=(XVector2 lhs, Vector2F rhs) => !(rhs == lhs);
}
