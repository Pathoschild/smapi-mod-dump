/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using Microsoft.Xna.Framework;
using SpriteMaster.Extensions;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SpriteMaster.Types;

[DebuggerDisplay("[{X}, {Y}}")]
[StructLayout(LayoutKind.Explicit, Pack = sizeof(ulong), Size = sizeof(ulong))]
unsafe struct Vector2I :
	ICloneable,
	IComparable,
	IComparable<Vector2I>,
	IComparable<(int, int)>,
	IComparable<DrawingPoint>,
	IComparable<XNA.Point>,
	IComparable<XTilePoint>,
	IComparable<DrawingSize>,
	IComparable<XTileSize>,
	IEquatable<Vector2I>,
	IEquatable<(int, int)>,
	IEquatable<DrawingPoint>,
	IEquatable<XNA.Point>,
	IEquatable<XTilePoint>,
	IEquatable<DrawingSize>,
	IEquatable<XTileSize> {

	internal static readonly Vector2I Zero = (0, 0);
	internal static readonly Vector2I One = (1, 1);
	internal static readonly Vector2I MinusOne = (-1, -1);
	internal static readonly Vector2I Empty = Zero;

	[FieldOffset(0)]
	private fixed int Value[2];

	[FieldOffset(0)]
	internal int X;
	[FieldOffset(sizeof(int))]
	internal int Y;

	[FieldOffset(0)]
	internal ulong Packed;


	internal int Width { [MethodImpl(Runtime.MethodImpl.Hot)] readonly get => X; [MethodImpl(Runtime.MethodImpl.Hot)] set { X = value; } }
	internal int Height { [MethodImpl(Runtime.MethodImpl.Hot)] readonly get => Y; [MethodImpl(Runtime.MethodImpl.Hot)] set { Y = value; } }

	internal int this[int index] {
		[MethodImpl(Runtime.MethodImpl.Hot)]
		readonly get {
#if DEBUG
			if (index < 0 || index >= 2) {
				throw new IndexOutOfRangeException(nameof(index));
			}
#endif
			return Value[index];
		}
		[MethodImpl(Runtime.MethodImpl.Hot)]
		set {
#if DEBUG
			if (index < 0 || index >= 2) {
				throw new IndexOutOfRangeException(nameof(index));
			}
#endif
			Value[index] = value;
		}
	}

	internal readonly int Area => X * Y;

	internal readonly bool IsEmpty => Packed == 0UL;
	internal readonly bool IsZero => Packed == 0UL;
	internal readonly int MinOf => Math.Min(X, Y);
	internal readonly int MaxOf => Math.Max(X, Y);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2I(ulong Packed) : this() => this.Packed = Packed;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static Vector2I From(ulong Packed) => new(Packed: Packed);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2I(int X, int Y) : this() {
		this.X = X;
		this.Y = Y;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static Vector2I From(int X, int Y) => new(X, Y);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2I(in (int X, int Y) vec) : this(vec.X, vec.Y) {}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static Vector2I From(in (int X, int Y) vec) => new(vec.X, vec.Y);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2I(int Value) : this(Value, Value) { }

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static Vector2I From(int Value) => new(Value);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2I(in Vector2 Vector, bool Round = true) : this(Round ? Vector.NearestInt() : Vector.TruncateInt()) { }

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2I(Vector2I vec) : this(vec.Packed) { }

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2I(DrawingPoint v) : this(v.X, v.Y) { }

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2I(XNA.Point v) : this(v.X, v.Y) { }

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2I(XTilePoint v) : this(v.X, v.Y) { }

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2I(DrawingSize v) : this(v.Width, v.Height) { }

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2I(XTileSize v) : this(v.Width, v.Height) { }

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2I(Microsoft.Xna.Framework.Graphics.Texture2D tex) : this(tex.Width, tex.Height) { }

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2I(System.Drawing.Bitmap bmp) : this(bmp.Width, bmp.Height) { }

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2I(TeximpNet.Surface surface) : this(surface.Width, surface.Height) { }

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2I Set(int x, int y) {
		X = x;
		Y = y;
		return this;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2I Set(in (int X, int Y) vec) {
		X = vec.X;
		Y = vec.Y;
		return this;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2I Set(int v) => Set(v, v);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2I Set(Vector2I vec) {
		Packed = vec.Packed;
		return this;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2I Set(DrawingPoint vec) => Set(vec.X, vec.Y);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2I Set(XNA.Point vec) => Set(vec.X, vec.Y);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2I Set(XTilePoint vec) => Set(vec.X, vec.Y);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2I Set(DrawingSize vec) => Set(vec.Width, vec.Height);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2I Set(XTileSize vec) => Set(vec.Width, vec.Height);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator Vector2I(in (int X, int Y) vec) => new (vec.X, vec.Y);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator (int X, int Y)(Vector2I vec) => (vec.X, vec.Y);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator DrawingPoint(Vector2I vec) => new(vec.X, vec.Y);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator XNA.Point(Vector2I vec) => new(vec.X, vec.Y);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator XTilePoint(Vector2I vec) => new(vec.X, vec.Y);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator DrawingSize(Vector2I vec) => new(vec.X, vec.Y);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator XTileSize(Vector2I vec) => new(vec.X, vec.Y);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator Vector2(Vector2I vec) => new(vec.X, vec.Y);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator Vector2I(DrawingPoint vec) => new(vec);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator Vector2I(XNA.Point vec) => new(vec);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator Vector2I(XTilePoint vec) => new(vec);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator Vector2I(DrawingSize vec) => new(vec);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator Vector2I(XTileSize vec) => new(vec);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator Bounds(Vector2I vec) => new(vec);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly Vector2I Min() => new(Math.Min(X, Y));

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly Vector2I Max() => new(Math.Max(X, Y));

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly Vector2I Min(Vector2I v) => new(
		Math.Min(X, v.X),
		Math.Min(Y, v.Y)
	);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly Vector2I Max(Vector2I v) => new(
		Math.Max(X, v.X),
		Math.Max(Y, v.Y)
	);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly Vector2I Clamp(Vector2I min, Vector2I max) => new(
		X.Clamp(min.X, max.X),
		Y.Clamp(min.Y, max.Y)
	);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly Vector2I Min(int v) => new(
		Math.Min(X, v),
		Math.Min(Y, v)
	);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly Vector2I Max(int v) => new(
		Math.Max(X, v),
		Math.Max(Y, v)
	);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly Vector2I Clamp(int min, int max) => new(
		X.Clamp(min, max),
		Y.Clamp(min, max)
	);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly Vector2I Clone() => this;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	readonly object ICloneable.Clone() => this;

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

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public override readonly string ToString() => $"{{{X}, {Y}}}";

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly int CompareTo(Vector2I other) => Packed.CompareTo(other.Packed);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly int CompareTo((int, int) other) => CompareTo((Vector2I)other);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly int CompareTo(DrawingPoint other) => CompareTo((Vector2I)other);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly int CompareTo(XNA.Point other) => CompareTo((Vector2I)other);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly int CompareTo(XTilePoint other) => CompareTo((Vector2I)other);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly int CompareTo(DrawingSize other) => CompareTo((Vector2I)other);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly int CompareTo(XTileSize other) => CompareTo((Vector2I)other);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	readonly int IComparable.CompareTo(object other) => other switch {
		Vector2I vec => CompareTo(vec),
		DrawingPoint vec => CompareTo(vec),
		XNA.Point vec => CompareTo(vec),
		XTilePoint vec => CompareTo(vec),
		DrawingSize vec => CompareTo(vec),
		XTileSize vec => CompareTo(vec),
		_ => throw new ArgumentException(),
	};

	// C# GetHashCode on all integer primitives, even longs, just returns it truncated to an int.
	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly override int GetHashCode() => (int)Hash.Combine(X.GetHashCode(), Y.GetHashCode());

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly override bool Equals(object other) => other switch {
		Vector2I vec => Equals(vec),
		DrawingPoint vec => Equals(vec),
		XNA.Point vec => Equals(vec),
		XTilePoint vec => Equals(vec),
		DrawingSize vec => Equals(vec),
		XTileSize vec => Equals(vec),
		_ => throw new ArgumentException(),
	};

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly bool Equals(Vector2I other) => Packed == other.Packed;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly bool Equals((int, int) other) => this == (Vector2I)other;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly bool Equals(in (int X, int Y) other) => this == (Vector2I)other;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly bool Equals(DrawingPoint other) => this == (Vector2I)other;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly bool Equals(XNA.Point other) => this == (Vector2I)other;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly bool Equals(XTilePoint other) => this == (Vector2I)other;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly bool Equals(DrawingSize other) => this == (Vector2I)other;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly bool Equals(XTileSize other) => this == (Vector2I)other;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly bool NotEquals(Vector2I other) => Packed != other.Packed;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly bool NotEquals(in (int X, int Y) other) => this != (Vector2I)other;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly bool NotEquals(DrawingPoint other) => this != (Vector2I)other;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly bool NotEquals(XNA.Point other) => this != (Vector2I)other;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly bool NotEquals(XTilePoint other) => this != (Vector2I)other;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly bool NotEquals(DrawingSize other) => this != (Vector2I)other;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly bool NotEquals(XTileSize other) => this != (Vector2I)other;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator ==(Vector2I lhs, Vector2I rhs) => lhs.Packed == rhs.Packed;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator !=(Vector2I lhs, Vector2I rhs) => lhs.Packed != rhs.Packed;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator ==(Vector2I lhs, in (int X, int Y) rhs) => lhs.Equals(rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator !=(Vector2I lhs, in (int X, int Y) rhs) => lhs.NotEquals(rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator ==(in (int X, int Y) lhs, Vector2I rhs) => rhs.Equals(lhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator !=(in (int X, int Y) lhs, Vector2I rhs) => rhs.NotEquals(lhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator ==(Vector2I lhs, DrawingPoint rhs) => lhs.Equals(rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator !=(Vector2I lhs, DrawingPoint rhs) => lhs.NotEquals(rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator ==(DrawingPoint lhs, Vector2I rhs) => rhs.Equals(lhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator !=(DrawingPoint lhs, Vector2I rhs) => rhs.NotEquals(lhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator ==(Vector2I lhs, XNA.Point rhs) => lhs.Equals(rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator !=(Vector2I lhs, XNA.Point rhs) => lhs.NotEquals(rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator ==(XNA.Point lhs, Vector2I rhs) => rhs.Equals(lhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator !=(XNA.Point lhs, Vector2I rhs) => rhs.NotEquals(lhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator ==(Vector2I lhs, XTilePoint rhs) => lhs.Equals(rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator !=(Vector2I lhs, XTilePoint rhs) => lhs.NotEquals(rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator ==(XTilePoint lhs, Vector2I rhs) => rhs.Equals(lhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator !=(XTilePoint lhs, Vector2I rhs) => rhs.NotEquals(lhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator ==(Vector2I lhs, DrawingSize rhs) => lhs.Equals(rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator !=(Vector2I lhs, DrawingSize rhs) => lhs.NotEquals(rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator ==(DrawingSize lhs, Vector2I rhs) => rhs.Equals(lhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator !=(DrawingSize lhs, Vector2I rhs) => rhs.NotEquals(lhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator ==(Vector2I lhs, XTileSize rhs) => lhs.Equals(rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator !=(Vector2I lhs, XTileSize rhs) => lhs.NotEquals(rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator ==(XTileSize lhs, Vector2I rhs) => rhs.Equals(lhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator !=(XTileSize lhs, Vector2I rhs) => rhs.NotEquals(lhs);
}
