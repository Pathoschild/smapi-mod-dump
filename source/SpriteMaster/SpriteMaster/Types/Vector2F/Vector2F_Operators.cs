/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System.Runtime.CompilerServices;

using SystemVector2 = System.Numerics.Vector2;

namespace SpriteMaster.Types;

partial struct Vector2F {
	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2F operator +(Vector2F vector) => vector;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2F operator -(Vector2F vector) => SystemVector2.Negate(vector);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2F operator +(Vector2F lhs, Vector2F rhs) => SystemVector2.Add(lhs, rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2F operator -(Vector2F lhs, Vector2F rhs) => SystemVector2.Subtract(lhs, rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2F operator *(Vector2F lhs, Vector2F rhs) => SystemVector2.Multiply(lhs, rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2F operator /(Vector2F lhs, Vector2F rhs) => SystemVector2.Divide(lhs, rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2F operator %(Vector2F lhs, Vector2F rhs) => new(
		lhs.X % rhs.X,
		lhs.Y % rhs.Y
	);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2F operator +(Vector2F lhs, float rhs) => SystemVector2.Add(lhs, new(rhs));

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2F operator -(Vector2F lhs, float rhs) => SystemVector2.Subtract(lhs, new(rhs));

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2F operator *(Vector2F lhs, float rhs) => SystemVector2.Multiply(lhs, rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2F operator /(Vector2F lhs, float rhs) => SystemVector2.Divide(lhs, rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2F operator %(Vector2F lhs, float rhs) => new(
		lhs.X % rhs,
		lhs.Y % rhs
	);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2F operator +(Vector2I lhs, Vector2F rhs) => SystemVector2.Add((Vector2F)lhs, rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2F operator -(Vector2I lhs, Vector2F rhs) => SystemVector2.Subtract((Vector2F)lhs, rhs);
}
