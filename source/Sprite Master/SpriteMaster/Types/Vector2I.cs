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

namespace SpriteMaster.Types {
	using DrawingPoint = System.Drawing.Point;
	using XNAPoint = Point;
	using XTilePoint = xTile.Dimensions.Location;

	using DrawingSize = System.Drawing.Size;
	using XTileSize = xTile.Dimensions.Size;

	[DebuggerDisplay("[{X}, {Y}}")]
	[StructLayout(LayoutKind.Explicit, Pack = sizeof(ulong), Size = sizeof(ulong))]
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
		public int X;
		[FieldOffset(sizeof(int))]
		public int Y;

		[FieldOffset(0)]
		public ulong Packed;


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

		public readonly bool IsEmpty => Packed == 0UL;
		public readonly bool IsZero => Packed == 0UL;
		public readonly int MinOf => Math.Min(X, Y);
		public readonly int MaxOf => Math.Max(X, Y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2I (ulong Packed) : this() => this.Packed = Packed;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I From (ulong Packed) => new Vector2I(Packed: Packed);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2I (int X, int Y) : this() {
			this.X = X;
			this.Y = Y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I From (int X, int Y) => new Vector2I(X, Y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2I (int Value) : this(Value, Value) { }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I From (int Value) => new Vector2I(Value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2I (in Vector2 Vector, bool Round = true) : this(Round ? Vector.NearestInt() : Vector.TruncateInt()) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2I (Vector2I vec) : this(vec.Packed) { }

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
		public Vector2I Set (int v) => Set(v, v);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2I Set (Vector2I vec) {
			Packed = vec.Packed;
			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2I Set (DrawingPoint vec) => Set(vec.X, vec.Y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2I Set (XNAPoint vec) => Set(vec.X, vec.Y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2I Set (XTilePoint vec) => Set(vec.X, vec.Y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2I Set (DrawingSize vec) => Set(vec.Width, vec.Height);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2I Set (XTileSize vec) => Set(vec.Width, vec.Height);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator DrawingPoint (Vector2I vec) => new DrawingPoint(vec.X, vec.Y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator XNAPoint (Vector2I vec) => new XNAPoint(vec.X, vec.Y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator XTilePoint (Vector2I vec) => new XTilePoint(vec.X, vec.Y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator DrawingSize (Vector2I vec) => new DrawingSize(vec.X, vec.Y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator XTileSize (Vector2I vec) => new XTileSize(vec.X, vec.Y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vector2 (Vector2I vec) => new Vector2(vec.X, vec.Y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vector2I (DrawingPoint vec) => new Vector2I(vec);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vector2I (XNAPoint vec) => new Vector2I(vec);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vector2I (XTilePoint vec) => new Vector2I(vec);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vector2I (DrawingSize vec) => new Vector2I(vec);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vector2I (XTileSize vec) => new Vector2I(vec);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Bounds (Vector2I vec) => new Bounds(vec);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Vector2I Min () => new Vector2I(Math.Min(X, Y));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Vector2I Max () => new Vector2I(Math.Max(X, Y));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Vector2I Min (Vector2I v) => new Vector2I(
			Math.Min(X, v.X),
			Math.Min(Y, v.Y)
		);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Vector2I Max (Vector2I v) => new Vector2I(
			Math.Max(X, v.X),
			Math.Max(Y, v.Y)
		);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Vector2I Clamp (Vector2I min, Vector2I max) => new Vector2I(
			X.Clamp(min.X, max.X),
			Y.Clamp(min.Y, max.Y)
		);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Vector2I Min (int v) => new Vector2I(
			Math.Min(X, v),
			Math.Min(Y, v)
		);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Vector2I Max (int v) => new Vector2I(
			Math.Max(X, v),
			Math.Max(Y, v)
		);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Vector2I Clamp (int min, int max) => new Vector2I(
			X.Clamp(min, max),
			Y.Clamp(min, max)
		);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Vector2I Clone () => this;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly object ICloneable.Clone () => this;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator + (Vector2I lhs, Vector2I rhs) => new Vector2I(
			lhs.X + rhs.X,
			lhs.Y + rhs.Y
		);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator - (Vector2I lhs, Vector2I rhs) => new Vector2I(
			lhs.X - rhs.X,
			lhs.Y - rhs.Y
		);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator * (Vector2I lhs, Vector2I rhs) => new Vector2I(
			lhs.X * rhs.X,
			lhs.Y * rhs.Y
		);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator / (Vector2I lhs, Vector2I rhs) => new Vector2I(
			lhs.X / rhs.X,
			lhs.Y / rhs.Y
		);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator % (Vector2I lhs, Vector2I rhs) => new Vector2I(
			lhs.X % rhs.X,
			lhs.Y % rhs.Y
		);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator & (Vector2I lhs, Vector2I rhs) => new Vector2I(
			lhs.X & rhs.X,
			lhs.Y & rhs.Y
		);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator | (Vector2I lhs, Vector2I rhs) => new Vector2I(
			lhs.X | rhs.X,
			lhs.Y | rhs.Y
		);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator ^ (Vector2I lhs, Vector2I rhs) => new Vector2I(
			lhs.X ^ rhs.X,
			lhs.Y ^ rhs.Y
		);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator + (Vector2I lhs, int rhs) => new Vector2I(
			lhs.X + rhs,
			lhs.Y + rhs
		);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator - (Vector2I lhs, int rhs) => new Vector2I(
			lhs.X - rhs,
			lhs.Y - rhs
		);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator * (Vector2I lhs, int rhs) => new Vector2I(
			lhs.X * rhs,
			lhs.Y * rhs
		);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator / (Vector2I lhs, int rhs) => new Vector2I(
			lhs.X / rhs,
			lhs.Y / rhs
		);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator % (Vector2I lhs, int rhs) => new Vector2I(
			lhs.X % rhs,
			lhs.Y % rhs
		);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator & (Vector2I lhs, int rhs) => new Vector2I(
			lhs.X & rhs,
			lhs.Y & rhs
		);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator | (Vector2I lhs, int rhs) => new Vector2I(
			lhs.X | rhs,
			lhs.Y | rhs
		);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator ^ (Vector2I lhs, int rhs) => new Vector2I(
			lhs.X ^ rhs,
			lhs.Y ^ rhs
		);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator + (Vector2I lhs, uint rhs) => new Vector2I(
			lhs.X + (int)rhs,
			lhs.Y + (int)rhs
		);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator - (Vector2I lhs, uint rhs) => new Vector2I(
			lhs.X - (int)rhs,
			lhs.Y - (int)rhs
		);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator * (Vector2I lhs, uint rhs) => new Vector2I(
			lhs.X * (int)rhs,
			lhs.Y * (int)rhs
		);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator / (Vector2I lhs, uint rhs) => new Vector2I(
			lhs.X / (int)rhs,
			lhs.Y / (int)rhs
		);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator % (Vector2I lhs, uint rhs) => new Vector2I(
			lhs.X % (int)rhs,
			lhs.Y % (int)rhs
		);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator & (Vector2I lhs, uint rhs) => new Vector2I(
			lhs.X & (int)rhs,
			lhs.Y & (int)rhs
		);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator | (Vector2I lhs, uint rhs) => new Vector2I(
			lhs.X | (int)rhs,
			lhs.Y | (int)rhs
		);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I operator ^ (Vector2I lhs, uint rhs) => new Vector2I(
			lhs.X ^ (int)rhs,
			lhs.Y ^ (int)rhs
		);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly string ToString () => $"{{{X}, {Y}}}";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly int CompareTo (Vector2I other) => Packed.CompareTo(other.Packed);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly int CompareTo (DrawingPoint other) => CompareTo((Vector2I)other);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly int CompareTo (XNAPoint other) => CompareTo((Vector2I)other);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly int CompareTo (XTilePoint other) => CompareTo((Vector2I)other);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly int CompareTo (DrawingSize other) => CompareTo((Vector2I)other);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly int CompareTo (XTileSize other) => CompareTo((Vector2I)other);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly int IComparable.CompareTo (object other) => other switch {
			Vector2I vec => CompareTo(vec),
			DrawingPoint vec => CompareTo(vec),
			XNAPoint vec => CompareTo(vec),
			XTilePoint vec => CompareTo(vec),
			DrawingSize vec => CompareTo(vec),
			XTileSize vec => CompareTo(vec),
			_ => throw new ArgumentException(),
		};

		// C# GetHashCode on all integer primitives, even longs, just returns it truncated to an int.
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly override int GetHashCode () => unchecked((int)Hash.Combine(X.GetHashCode(), Y.GetHashCode()));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly override bool Equals (object other) => other switch {
			Vector2I vec => Equals(vec),
			DrawingPoint vec => Equals(vec),
			XNAPoint vec => Equals(vec),
			XTilePoint vec => Equals(vec),
			DrawingSize vec => Equals(vec),
			XTileSize vec => Equals(vec),
			_ => throw new ArgumentException(),
		};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals (Vector2I other) => Packed == other.Packed;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals (DrawingPoint other) => this == (Vector2I)other;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals (XNAPoint other) => this == (Vector2I)other;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals (XTilePoint other) => this == (Vector2I)other;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals (DrawingSize other) => this == (Vector2I)other;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals (XTileSize other) => this == (Vector2I)other;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool NotEquals (Vector2I other) => Packed != other.Packed;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool NotEquals (DrawingPoint other) => this != (Vector2I)other;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool NotEquals (XNAPoint other) => this != (Vector2I)other;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool NotEquals (XTilePoint other) => this != (Vector2I)other;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool NotEquals (DrawingSize other) => this != (Vector2I)other;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool NotEquals (XTileSize other) => this != (Vector2I)other;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator == (Vector2I lhs, Vector2I rhs) => lhs.Packed == rhs.Packed;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator != (Vector2I lhs, Vector2I rhs) => lhs.Packed != rhs.Packed;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator == (Vector2I lhs, DrawingPoint rhs) => lhs.Equals(rhs);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator != (Vector2I lhs, DrawingPoint rhs) => lhs.NotEquals(rhs);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator == (DrawingPoint lhs, Vector2I rhs) => rhs.Equals(lhs);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator != (DrawingPoint lhs, Vector2I rhs) => rhs.NotEquals(lhs);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator == (Vector2I lhs, XNAPoint rhs) => lhs.Equals(rhs);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator != (Vector2I lhs, XNAPoint rhs) => lhs.NotEquals(rhs);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator == (XNAPoint lhs, Vector2I rhs) => rhs.Equals(lhs);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator != (XNAPoint lhs, Vector2I rhs) => rhs.NotEquals(lhs);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator == (Vector2I lhs, XTilePoint rhs) => lhs.Equals(rhs);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator != (Vector2I lhs, XTilePoint rhs) => lhs.NotEquals(rhs);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator == (XTilePoint lhs, Vector2I rhs) => rhs.Equals(lhs);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator != (XTilePoint lhs, Vector2I rhs) => rhs.NotEquals(lhs);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator == (Vector2I lhs, DrawingSize rhs) => lhs.Equals(rhs);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator != (Vector2I lhs, DrawingSize rhs) => lhs.NotEquals(rhs);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator == (DrawingSize lhs, Vector2I rhs) => rhs.Equals(lhs);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator != (DrawingSize lhs, Vector2I rhs) => rhs.NotEquals(lhs);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator == (Vector2I lhs, XTileSize rhs) => lhs.Equals(rhs);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator != (in Vector2I lhs, XTileSize rhs) => lhs.NotEquals(rhs);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator == (XTileSize lhs, Vector2I rhs) => rhs.Equals(lhs);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator != (XTileSize lhs, Vector2I rhs) => rhs.NotEquals(lhs);
	}
}
