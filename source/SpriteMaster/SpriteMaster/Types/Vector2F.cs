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
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using SystemVector2 = System.Numerics.Vector2;

namespace SpriteMaster.Types;

[DebuggerDisplay("[{X}, {Y}}")]
[StructLayout(LayoutKind.Explicit, Pack = sizeof(float) * 2, Size = sizeof(float) * 2)]
unsafe partial struct Vector2F : ILongHash, ICloneable {

	internal static readonly Vector2F Zero = (0.0f, 0.0f);
	internal static readonly Vector2F One = (1.0f, 1.0f);
	internal static readonly Vector2F MinusOne = (-1.0f, -1.0f);
	internal static readonly Vector2F Empty = Zero;

	[FieldOffset(0)]
	private SystemVector2 NumericVector;

	[FieldOffset(0)]
	private fixed float Value[2];

	internal float X {
		[MethodImpl(Runtime.MethodImpl.Hot)]
		readonly get => NumericVector.X;
		[MethodImpl(Runtime.MethodImpl.Hot)]
		set => NumericVector.X = value;
	}
	internal float Y {
		[MethodImpl(Runtime.MethodImpl.Hot)]
		readonly get => NumericVector.Y;
		[MethodImpl(Runtime.MethodImpl.Hot)]
		set => NumericVector.Y = value;
	}

	internal float Width {
		[MethodImpl(Runtime.MethodImpl.Hot)]
		readonly get => X;
		[MethodImpl(Runtime.MethodImpl.Hot)]
		set => X = value;
	}
	internal float Height {
		[MethodImpl(Runtime.MethodImpl.Hot)]
		readonly get => Y;
		[MethodImpl(Runtime.MethodImpl.Hot)]
		set => Y = value;
	}

	[MethodImpl(Runtime.MethodImpl.Hot), DebuggerStepThrough, DebuggerHidden()]
	private static int CheckIndex(int index) {
#if DEBUG
		if (index < 0 || index >= 2) {
			throw new IndexOutOfRangeException(nameof(index));
		}
#endif
		return index;
	}

	internal float this[int index] {
		[MethodImpl(Runtime.MethodImpl.Hot)]
		readonly get => Value[CheckIndex(index)];
		[MethodImpl(Runtime.MethodImpl.Hot)]
		set => Value[CheckIndex(index)] = value;
	}

	internal readonly float Area => X * Y;

	internal readonly bool IsEmpty => NumericVector.Equals(SystemVector2.Zero);
	internal readonly bool IsZero => IsEmpty;
	internal readonly float MinOf => Math.Min(X, Y);
	internal readonly float MaxOf => Math.Max(X, Y);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2F(float X, float Y) => NumericVector = new(X, Y);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static Vector2F From(float X, float Y) => new(X, Y);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2F(in (float X, float Y) vec) : this(vec.X, vec.Y) { }

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static Vector2F From(in (float X, float Y) vec) => new(vec.X, vec.Y);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2F(float value) => NumericVector = new(value);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static Vector2F From(float value) => new(value);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2F(in XNA.Vector2 Vector) : this(Vector.X, Vector.Y) { }

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2F(Vector2F vec) : this(vec.NumericVector) { }

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2F(Vector2I v) : this(v.X, v.Y) { }

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2F(SystemVector2 v) => NumericVector = v;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal void Set(float x, float y) => NumericVector = new(x, y);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal void Set(in (float X, float Y) vec) => NumericVector = new(vec.X, vec.Y);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal void Set(float v) => Set(v, v);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal void Set(Vector2F vec) => NumericVector = vec.NumericVector;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal void Set(Vector2I vec) => Set(vec.X, vec.Y);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal void Set(XNA.Vector2 vec) => Set(vec.X, vec.Y);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator Vector2F(in (float X, float Y) vec) => new(vec.X, vec.Y);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator (float X, float Y)(Vector2F vec) => (vec.X, vec.Y);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator Vector2I(Vector2F vec) => new((int)vec.X, (int)vec.Y);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator XNA.Vector2(Vector2F vec) => new(vec.X, vec.Y);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator Vector2F(Vector2I vec) => new(vec);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator Vector2F(XNA.Vector2 vec) => new(vec);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator Vector2F(SystemVector2 vec) => new(vec);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator SystemVector2(Vector2F vec) => vec.NumericVector;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly Vector2F Min() => new(Math.Min(X, Y));

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly Vector2F Max() => new(Math.Max(X, Y));

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly Vector2F Min(Vector2F v) => SystemVector2.Min(this, v);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly Vector2F Max(Vector2F v) => SystemVector2.Max(this, v);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly Vector2F Clamp(Vector2F min, Vector2F max) => SystemVector2.Clamp(this, min, max);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly Vector2F Min(float v) => SystemVector2.Min(this, new SystemVector2(v));

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly Vector2F Max(float v) => SystemVector2.Max(this, new SystemVector2(v));

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly Vector2F Clamp(float min, float max) => SystemVector2.Clamp(this, new(min), new(max));

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly Vector2F Clone() => this;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	readonly object ICloneable.Clone() => this;

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
	public override readonly string ToString() => $"{{{X}, {Y}}}";

	// C# GetHashCode on all integer primitives, even longs, just returns it truncated to an int.
	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly override int GetHashCode() => NumericVector.GetHashCode();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	readonly ulong ILongHash.GetLongHashCode() => (ulong)NumericVector.GetHashCode();
}
