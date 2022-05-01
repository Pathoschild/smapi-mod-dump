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

namespace SpriteMaster.Types;

partial struct Vector2I {
	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2I operator -(Vector2I value) => new(
		-value.X,
		-value.Y
	);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2I operator +(Vector2I value) => value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2I operator >>(Vector2I value, int bits) => new(
		value.X >> bits,
		value.Y >> bits
	);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2I operator <<(Vector2I value, int bits) => new(
		value.X << bits,
		value.Y << bits
	);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2I operator +(Vector2I lhs, Vector2I rhs) => new(
		lhs.X + rhs.X,
		lhs.Y + rhs.Y
	);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2I operator -(Vector2I lhs, Vector2I rhs) => new(
		lhs.X - rhs.X,
		lhs.Y - rhs.Y
	);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2I operator *(Vector2I lhs, Vector2I rhs) => new(
		lhs.X * rhs.X,
		lhs.Y * rhs.Y
	);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2I operator /(Vector2I lhs, Vector2I rhs) => new(
		lhs.X / rhs.X,
		lhs.Y / rhs.Y
	);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2I operator %(Vector2I lhs, Vector2I rhs) => new(
		lhs.X % rhs.X,
		lhs.Y % rhs.Y
	);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2I operator &(Vector2I lhs, Vector2I rhs) => new(
		lhs.X & rhs.X,
		lhs.Y & rhs.Y
	);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2I operator |(Vector2I lhs, Vector2I rhs) => new(
		lhs.X | rhs.X,
		lhs.Y | rhs.Y
	);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2I operator ^(Vector2I lhs, Vector2I rhs) => new(
		lhs.X ^ rhs.X,
		lhs.Y ^ rhs.Y
	);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2I operator +(Vector2I lhs, int rhs) => new(
		lhs.X + rhs,
		lhs.Y + rhs
	);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2I operator -(Vector2I lhs, int rhs) => new(
		lhs.X - rhs,
		lhs.Y - rhs
	);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2I operator *(Vector2I lhs, int rhs) => new(
		lhs.X * rhs,
		lhs.Y * rhs
	);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2I operator /(Vector2I lhs, int rhs) => new(
		lhs.X / rhs,
		lhs.Y / rhs
	);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2I operator %(Vector2I lhs, int rhs) => new(
		lhs.X % rhs,
		lhs.Y % rhs
	);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2I operator &(Vector2I lhs, int rhs) => new(
		lhs.X & rhs,
		lhs.Y & rhs
	);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2I operator |(Vector2I lhs, int rhs) => new(
		lhs.X | rhs,
		lhs.Y | rhs
	);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2I operator ^(Vector2I lhs, int rhs) => new(
		lhs.X ^ rhs,
		lhs.Y ^ rhs
	);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2I operator +(Vector2I lhs, uint rhs) => new(
		lhs.X + (int)rhs,
		lhs.Y + (int)rhs
	);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2I operator -(Vector2I lhs, uint rhs) => new(
		lhs.X - (int)rhs,
		lhs.Y - (int)rhs
	);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2I operator *(Vector2I lhs, uint rhs) => new(
		lhs.X * (int)rhs,
		lhs.Y * (int)rhs
	);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2I operator /(Vector2I lhs, uint rhs) => new(
		lhs.X / (int)rhs,
		lhs.Y / (int)rhs
	);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2I operator %(Vector2I lhs, uint rhs) => new(
		lhs.X % (int)rhs,
		lhs.Y % (int)rhs
	);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2I operator &(Vector2I lhs, uint rhs) => new(
		lhs.X & (int)rhs,
		lhs.Y & (int)rhs
	);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2I operator |(Vector2I lhs, uint rhs) => new(
		lhs.X | (int)rhs,
		lhs.Y | (int)rhs
	);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2I operator ^(Vector2I lhs, uint rhs) => new(
		lhs.X ^ (int)rhs,
		lhs.Y ^ (int)rhs
	);
}
