using Microsoft.Xna.Framework;
using SpriteMaster.Extensions;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SpriteMaster.Types {
	using DrawingPoint = System.Drawing.Point;
	using XNAPoint = Point;
	using XTilePoint = xTile.Dimensions.Location;

	using DrawingSize = System.Drawing.Size;
	using XTileSize = xTile.Dimensions.Size;

	[DebuggerDisplay("[{X}, {Y}}")]
	[StructLayout(LayoutKind.Explicit, Pack = 4)]
	public unsafe struct Vector2I :
		ICloneable,
		IComparable,
		IComparable<Vector2I>,
		IComparable<DrawingPoint>,
		IComparable<XNAPoint>,
		IComparable<XTilePoint>,
		IComparable<DrawingSize>,
		IComparable<XTileSize>,
		IEquatable<Vector2I>,
		IEquatable<DrawingPoint>,
		IEquatable<XNAPoint>,
		IEquatable<XTilePoint>,
		IEquatable<DrawingSize>,
		IEquatable<XTileSize> {

		public static readonly Vector2I Zero = new Vector2I(0, 0);
		public static readonly Vector2I One = new Vector2I(1, 1);
		public static readonly Vector2I MinusOne = new Vector2I(-1, -1);
		public static readonly Vector2I Empty = Zero;

		[FieldOffset(0)]
		private fixed int Value[2];
		[FieldOffset(0)]
		private long LongValue;

		public long Packed { readonly get => LongValue; set => LongValue = value; }

		public int X { [MethodImpl(MethodImplOptions.AggressiveInlining)] readonly get => Value[0]; [MethodImpl(MethodImplOptions.AggressiveInlining)] set { Value[0] = value; } }
		public int Y { [MethodImpl(MethodImplOptions.AggressiveInlining)] readonly get => Value[1]; [MethodImpl(MethodImplOptions.AggressiveInlining)] set { Value[1] = value; } }

		public int Width { [MethodImpl(MethodImplOptions.AggressiveInlining)] readonly get => X; [MethodImpl(MethodImplOptions.AggressiveInlining)] set { X = value; } }
		public int Height { [MethodImpl(MethodImplOptions.AggressiveInlining)] readonly get => Y; [MethodImpl(MethodImplOptions.AggressiveInlining)] set { Y = value; } }

		public int this[int index] {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get {
#if DEBUG
				if (index < 0 || index >= 2) {
					throw new IndexOutOfRangeException(nameof(index));
				}
#endif
				return Value[index];
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set {
#if DEBUG
				if (index < 0 || index >= 2) {
					throw new IndexOutOfRangeException(nameof(index));
				}
#endif
				Value[index] = value;
			}
		}

		public readonly int Area => X * Y;

		public readonly bool IsEmpty => LongValue == 0L; // (X == 0 && Y == 0);
		public readonly bool IsZero => IsEmpty;
		public readonly int MinOf => Math.Min(X, Y);
		public readonly int MaxOf => Math.Max(X, Y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2I (long longValue) : this() {
			LongValue = longValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2I (int x, int y) : this() {
			Value[0] = x;
			Value[1] = y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2I (int v) : this(v, v) { }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2I (Vector2 vec, bool round = true) : this(
			round ? vec.X.NearestInt() : vec.X.TruncateInt(),
			round ? vec.Y.NearestInt() : vec.Y.TruncateInt()
		) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2I (Vector2I vec) : this(vec.LongValue) { }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2I (DrawingPoint v) : this(v.X, v.Y) { }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2I (XNAPoint v) : this(v.X, v.Y) { }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2I (XTilePoint v) : this(v.X, v.Y) { }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2I (DrawingSize v) : this(v.Width, v.Height) { }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2I (XTileSize v) : this(v.Width, v.Height) { }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2I (Microsoft.Xna.Framework.Graphics.Texture2D tex) : this(tex.Width, tex.Height) { }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2I (System.Drawing.Bitmap bmp) : this(bmp.Width, bmp.Height) { }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2I Set (int x, int y) {
			X = x;
			Y = y;
			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2I Set (int v) {
			return Set(v, v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2I Set (Vector2I vec) {
			LongValue = vec.LongValue;
			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2I Set (DrawingPoint vec) {
			return Set(vec.X, vec.Y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2I Set (XNAPoint vec) {
			return Set(vec.X, vec.Y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2I Set (XTilePoint vec) {
			return Set(vec.X, vec.Y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2I Set (DrawingSize vec) {
			return Set(vec.Width, vec.Height);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2I Set (XTileSize vec) {
			return Set(vec.Width, vec.Height);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator DrawingPoint (Vector2I vec) {
			return new DrawingPoint(vec.X, vec.Y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator XNAPoint (Vector2I vec) {
			return new XNAPoint(vec.X, vec.Y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator XTilePoint (Vector2I vec) {
			return new XTilePoint(vec.X, vec.Y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator DrawingSize (Vector2I vec) {
			return new DrawingSize(vec.X, vec.Y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator XTileSize (Vector2I vec) {
			return new XTileSize(vec.X, vec.Y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vector2 (Vector2I vec) {
			return new Vector2(vec.X, vec.Y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vector2I (DrawingPoint vec) {
			return new Vector2I(vec);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vector2I (XNAPoint vec) {
			return new Vector2I(vec);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vector2I (XTilePoint vec) {
			return new Vector2I(vec);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vector2I (DrawingSize vec) {
			return new Vector2I(vec);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vector2I (XTileSize vec) {
			return new Vector2I(vec);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Bounds (Vector2I vec) {
			return new Bounds(vec);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Vector2I Min () {
			return new Vector2I(Math.Min(X, Y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Vector2I Max () {
			return new Vector2I(Math.Max(X, Y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Vector2I Min (Vector2I v) {
			return new Vector2I(
				Math.Min(X, v.X),
				Math.Min(Y, v.Y)
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Vector2I Max (Vector2I v) {
			return new Vector2I(
				Math.Max(X, v.X),
				Math.Max(Y, v.Y)
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Vector2I Clamp (Vector2I min, Vector2I max) {
			return new Vector2I(
				X.Clamp(min.X, max.X),
				Y.Clamp(min.Y, max.Y)
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Vector2I Min (int v) {
			return new Vector2I(
				Math.Min(X, v),
				Math.Min(Y, v)
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Vector2I Max (int v) {
			return new Vector2I(
				Math.Max(X, v),
				Math.Max(Y, v)
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Vector2I Clamp (int min, int max) {
			return new Vector2I(
				X.Clamp(min, max),
				Y.Clamp(min, max)
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Vector2I Clone () {
			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly object ICloneable.Clone () {
			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator + (Vector2I lhs, Vector2I rhs) {
			return new Vector2I(
				lhs.X + rhs.X,
				lhs.Y + rhs.Y
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator - (Vector2I lhs, Vector2I rhs) {
			return new Vector2I(
				lhs.X - rhs.X,
				lhs.Y - rhs.Y
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator * (Vector2I lhs, Vector2I rhs) {
			return new Vector2I(
				lhs.X * rhs.X,
				lhs.Y * rhs.Y
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator / (Vector2I lhs, Vector2I rhs) {
			return new Vector2I(
				lhs.X / rhs.X,
				lhs.Y / rhs.Y
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator % (Vector2I lhs, Vector2I rhs) {
			return new Vector2I(
				lhs.X % rhs.X,
				lhs.Y % rhs.Y
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator & (Vector2I lhs, Vector2I rhs) {
			return new Vector2I(
				lhs.X & rhs.X,
				lhs.Y & rhs.Y
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator | (Vector2I lhs, Vector2I rhs) {
			return new Vector2I(
				lhs.X | rhs.X,
				lhs.Y | rhs.Y
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator + (Vector2I lhs, int rhs) {
			return new Vector2I(
				lhs.X + rhs,
				lhs.Y + rhs
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator - (Vector2I lhs, int rhs) {
			return new Vector2I(
				lhs.X - rhs,
				lhs.Y - rhs
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator * (Vector2I lhs, int rhs) {
			return new Vector2I(
				lhs.X * rhs,
				lhs.Y * rhs
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator / (Vector2I lhs, int rhs) {
			return new Vector2I(
				lhs.X / rhs,
				lhs.Y / rhs
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator % (Vector2I lhs, int rhs) {
			return new Vector2I(
				lhs.X % rhs,
				lhs.Y % rhs
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator & (Vector2I lhs, int rhs) {
			return new Vector2I(
				lhs.X & rhs,
				lhs.Y & rhs
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator | (Vector2I lhs, int rhs) {
			return new Vector2I(
				lhs.X | rhs,
				lhs.Y | rhs
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator + (Vector2I lhs, uint rhs) {
			return new Vector2I(
				lhs.X + (int)rhs,
				lhs.Y + (int)rhs
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator - (Vector2I lhs, uint rhs) {
			return new Vector2I(
				lhs.X - (int)rhs,
				lhs.Y - (int)rhs
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator * (Vector2I lhs, uint rhs) {
			return new Vector2I(
				lhs.X * (int)rhs,
				lhs.Y * (int)rhs
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator / (Vector2I lhs, uint rhs) {
			return new Vector2I(
				lhs.X / (int)rhs,
				lhs.Y / (int)rhs
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator % (Vector2I lhs, uint rhs) {
			return new Vector2I(
				lhs.X % (int)rhs,
				lhs.Y % (int)rhs
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator & (Vector2I lhs, uint rhs) {
			return new Vector2I(
				lhs.X & (int)rhs,
				lhs.Y & (int)rhs
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator | (Vector2I lhs, uint rhs) {
			return new Vector2I(
				lhs.X | (int)rhs,
				lhs.Y | (int)rhs
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly string ToString () {
			return $"{{{X}, {Y}}}";
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly int CompareTo (Vector2I other) {
			return LongValue.CompareTo(other.LongValue);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly int CompareTo (DrawingPoint other) {
			return CompareTo((Vector2I)other);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly int CompareTo (XNAPoint other) {
			return CompareTo((Vector2I)other);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly int CompareTo (XTilePoint other) {
			return CompareTo((Vector2I)other);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly int CompareTo (DrawingSize other) {
			return CompareTo((Vector2I)other);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly int CompareTo (XTileSize other) {
			return CompareTo((Vector2I)other);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly int IComparable.CompareTo (object other) {
			return other switch
			{
				Vector2I vec => CompareTo(vec),
				DrawingPoint vec => CompareTo(vec),
				XNAPoint vec => CompareTo(vec),
				XTilePoint vec => CompareTo(vec),
				DrawingSize vec => CompareTo(vec),
				XTileSize vec => CompareTo(vec),
				_ => throw new ArgumentException(),
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly override int GetHashCode () {
			// C# GetHashCode on all integer primitives, even longs, just returns it truncated to an int.
			return unchecked((int)Hash.Combine(X.GetHashCode(), Y.GetHashCode()));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly override bool Equals (object other) {
			return other switch
			{
				Vector2I vec => Equals(vec),
				DrawingPoint vec => Equals(vec),
				XNAPoint vec => Equals(vec),
				XTilePoint vec => Equals(vec),
				DrawingSize vec => Equals(vec),
				XTileSize vec => Equals(vec),
				_ => throw new ArgumentException(),
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals (Vector2I other) {
			return LongValue == other.LongValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals (DrawingPoint other) {
			return this == (Vector2I)other;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals (XNAPoint other) {
			return this == (Vector2I)other;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals (XTilePoint other) {
			return this == (Vector2I)other;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals (DrawingSize other) {
			return this == (Vector2I)other;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals (XTileSize other) {
			return this == (Vector2I)other;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool NotEquals (Vector2I other) {
			return LongValue != other.LongValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool NotEquals (DrawingPoint other) {
			return this != (Vector2I)other;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool NotEquals (XNAPoint other) {
			return this != (Vector2I)other;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool NotEquals (XTilePoint other) {
			return this != (Vector2I)other;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool NotEquals (DrawingSize other) {
			return this != (Vector2I)other;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool NotEquals (XTileSize other) {
			return this != (Vector2I)other;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator == (Vector2I lhs, Vector2I rhs) {
			return lhs.LongValue == rhs.LongValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator != (Vector2I lhs, Vector2I rhs) {
			return lhs.LongValue != rhs.LongValue;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator == (Vector2I lhs, DrawingPoint rhs) {
			return lhs.Equals(rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator != (Vector2I lhs, DrawingPoint rhs) {
			return lhs.NotEquals(rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator == (DrawingPoint lhs, Vector2I rhs) {
			return rhs.Equals(lhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator != (DrawingPoint lhs, Vector2I rhs) {
			return rhs.NotEquals(lhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator == (Vector2I lhs, XNAPoint rhs) {
			return lhs.Equals(rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator != (Vector2I lhs, XNAPoint rhs) {
			return lhs.NotEquals(rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator == (XNAPoint lhs, Vector2I rhs) {
			return rhs.Equals(lhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator != (XNAPoint lhs, Vector2I rhs) {
			return rhs.NotEquals(lhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator == (Vector2I lhs, XTilePoint rhs) {
			return lhs.Equals(rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator != (Vector2I lhs, XTilePoint rhs) {
			return lhs.NotEquals(rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator == (XTilePoint lhs, Vector2I rhs) {
			return rhs.Equals(lhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator != (XTilePoint lhs, Vector2I rhs) {
			return rhs.NotEquals(lhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator == (Vector2I lhs, DrawingSize rhs) {
			return lhs.Equals(rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator != (Vector2I lhs, DrawingSize rhs) {
			return lhs.NotEquals(rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator == (DrawingSize lhs, Vector2I rhs) {
			return rhs.Equals(lhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator != (DrawingSize lhs, Vector2I rhs) {
			return rhs.NotEquals(lhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator == (Vector2I lhs, XTileSize rhs) {
			return lhs.Equals(rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator != (in Vector2I lhs, XTileSize rhs) {
			return lhs.NotEquals(rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator == (XTileSize lhs, Vector2I rhs) {
			return rhs.Equals(lhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator != (XTileSize lhs, Vector2I rhs) {
			return rhs.NotEquals(lhs);
		}
	}
}
