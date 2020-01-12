using Microsoft.Xna.Framework;
using SpriteMaster.Extensions;
using System;

namespace SpriteMaster.Types {
	using DrawingPoint = System.Drawing.Point;
	using XNAPoint = Microsoft.Xna.Framework.Point;
	using XTilePoint = xTile.Dimensions.Location;

	using DrawingSize = System.Drawing.Size;
	using XTileSize = xTile.Dimensions.Size;

	public struct Vector2I :
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

		public int X;
		public int Y;

		public int Width {
			readonly get { return X; }
			set { X = value; }
		}

		public int Height {
			readonly get { return Y; }
			set { Y = value; }
		}

		public int this[int index] {
			readonly get {
				return index switch
				{
					0 => X,
					1 => Y,
					_ => throw new IndexOutOfRangeException(nameof(index))
				};
			}
			set {
				switch (index) {
					case 0:
						X = value;
						return;
					case 1:
						Y = value;
						return;
					default:
						throw new IndexOutOfRangeException(nameof(index));
				}
			}
		}

		public int Area {
			get { return X * Y; }
		}

		public bool IsEmpty {
			get { return this == Zero; }
		}

		public bool IsZero {
			get { return this == Zero; }
		}

		public int MinOf {
			get { return Math.Min(X, Y); }
		}

		public int MaxOf {
			get { return Math.Max(X, Y); }
		}

		public Vector2I (int x, int y) {
			X = x;
			Y = y;
		}

		public Vector2I (int v) : this(v, v) { }

		public Vector2I (Vector2 vec, bool round = true) {
			if (round) {
				X = vec.X.NearestInt();
				Y = vec.Y.NearestInt();
			}
			else {
				X = vec.X.TruncateInt();
				Y = vec.Y.TruncateInt();
			}
		}

		public Vector2I (Vector2I vec) : this(vec.X, vec.Y) { }

		public Vector2I (DrawingPoint v) : this(v.X, v.Y) { }

		public Vector2I (XNAPoint v) : this(v.X, v.Y) { }

		public Vector2I (XTilePoint v) : this(v.X, v.Y) { }

		public Vector2I (DrawingSize v) : this(v.Width, v.Height) { }

		public Vector2I (XTileSize v) : this(v.Width, v.Height) { }

		public Vector2I (Microsoft.Xna.Framework.Graphics.Texture2D tex) : this(tex.Width, tex.Height) { }

		public Vector2I (System.Drawing.Bitmap bmp) : this(bmp.Width, bmp.Height) { }

		public Vector2I Set (int x, int y) {
			X = x;
			Y = y;
			return this;
		}

		public Vector2I Set (int v) {
			return Set(v, v);
		}

		public Vector2I Set (Vector2I vec) {
			return Set(vec.X, vec.Y);
		}

		public Vector2I Set (DrawingPoint vec) {
			return Set(vec.X, vec.Y);
		}

		public Vector2I Set (XNAPoint vec) {
			return Set(vec.X, vec.Y);
		}

		public Vector2I Set (XTilePoint vec) {
			return Set(vec.X, vec.Y);
		}

		public Vector2I Set (DrawingSize vec) {
			return Set(vec.Width, vec.Height);
		}

		public Vector2I Set (XTileSize vec) {
			return Set(vec.Width, vec.Height);
		}

		public static implicit operator DrawingPoint (Vector2I vec) {
			return new DrawingPoint(vec.X, vec.Y);
		}

		public static implicit operator XNAPoint (Vector2I vec) {
			return new XNAPoint(vec.X, vec.Y);
		}

		public static implicit operator XTilePoint (Vector2I vec) {
			return new XTilePoint(vec.X, vec.Y);
		}

		public static implicit operator DrawingSize (Vector2I vec) {
			return new DrawingSize(vec.X, vec.Y);
		}

		public static implicit operator XTileSize (Vector2I vec) {
			return new XTileSize(vec.X, vec.Y);
		}

		public static implicit operator Vector2 (Vector2I vec) {
			return new Vector2(vec.X, vec.Y);
		}

		public static implicit operator Vector2I (DrawingPoint vec) {
			return new Vector2I(vec);
		}

		public static implicit operator Vector2I (XNAPoint vec) {
			return new Vector2I(vec);
		}

		public static implicit operator Vector2I (XTilePoint vec) {
			return new Vector2I(vec);
		}

		public static implicit operator Vector2I (DrawingSize vec) {
			return new Vector2I(vec);
		}

		public static implicit operator Vector2I (XTileSize vec) {
			return new Vector2I(vec);
		}

		public static implicit operator Bounds (Vector2I vec) {
			return new Bounds(vec);
		}

		public readonly Vector2I Min () {
			return new Vector2I(Math.Min(X, Y));
		}

		public readonly Vector2I Max () {
			return new Vector2I(Math.Max(X, Y));
		}

		public readonly Vector2I Min (Vector2I v) {
			return new Vector2I(
				Math.Min(X, v.X),
				Math.Min(Y, v.Y)
			);
		}

		public readonly Vector2I Max (Vector2I v) {
			return new Vector2I(
				Math.Max(X, v.X),
				Math.Max(Y, v.Y)
			);
		}

		public readonly Vector2I Clamp (Vector2I min, Vector2I max) {
			return new Vector2I(
				X.Clamp(min.X, max.X),
				Y.Clamp(min.Y, max.Y)
			);
		}

		public readonly Vector2I Min (int v) {
			return new Vector2I(
				Math.Min(X, v),
				Math.Min(Y, v)
			);
		}

		public readonly Vector2I Max (int v) {
			return new Vector2I(
				Math.Max(X, v),
				Math.Max(Y, v)
			);
		}

		public readonly Vector2I Clamp (int min, int max) {
			return new Vector2I(
				X.Clamp(min, max),
				Y.Clamp(min, max)
			);
		}

		public readonly Vector2I Clone () {
			return new Vector2I(this);
		}

		readonly object ICloneable.Clone () {
			return Clone();
		}

		public static Vector2I operator + (Vector2I lhs, Vector2I rhs) {
			return new Vector2I(
				lhs.X + rhs.X,
				lhs.Y + rhs.Y
			);
		}

		public static Vector2I operator - (Vector2I lhs, Vector2I rhs) {
			return new Vector2I(
				lhs.X - rhs.X,
				lhs.Y - rhs.Y
			);
		}

		public static Vector2I operator * (Vector2I lhs, Vector2I rhs) {
			return new Vector2I(
				lhs.X * rhs.X,
				lhs.Y * rhs.Y
			);
		}

		public static Vector2I operator / (Vector2I lhs, Vector2I rhs) {
			return new Vector2I(
				lhs.X / rhs.X,
				lhs.Y / rhs.Y
			);
		}

		public static Vector2I operator % (Vector2I lhs, Vector2I rhs) {
			return new Vector2I(
				lhs.X % rhs.X,
				lhs.Y % rhs.Y
			);
		}

		public static Vector2I operator & (Vector2I lhs, Vector2I rhs) {
			return new Vector2I(
				lhs.X & rhs.X,
				lhs.Y & rhs.Y
			);
		}

		public static Vector2I operator | (Vector2I lhs, Vector2I rhs) {
			return new Vector2I(
				lhs.X | rhs.X,
				lhs.Y | rhs.Y
			);
		}

		public static Vector2I operator + (Vector2I lhs, int rhs) {
			return new Vector2I(
				lhs.X + rhs,
				lhs.Y + rhs
			);
		}

		public static Vector2I operator - (Vector2I lhs, int rhs) {
			return new Vector2I(
				lhs.X - rhs,
				lhs.Y - rhs
			);
		}

		public static Vector2I operator * (Vector2I lhs, int rhs) {
			return new Vector2I(
				lhs.X * rhs,
				lhs.Y * rhs
			);
		}

		public static Vector2I operator / (Vector2I lhs, int rhs) {
			return new Vector2I(
				lhs.X / rhs,
				lhs.Y / rhs
			);
		}

		public static Vector2I operator % (Vector2I lhs, int rhs) {
			return new Vector2I(
				lhs.X % rhs,
				lhs.Y % rhs
			);
		}

		public static Vector2I operator & (Vector2I lhs, int rhs) {
			return new Vector2I(
				lhs.X & rhs,
				lhs.Y & rhs
			);
		}

		public static Vector2I operator | (Vector2I lhs, int rhs) {
			return new Vector2I(
				lhs.X | rhs,
				lhs.Y | rhs
			);
		}

		public override readonly string ToString () {
			return $"{{{X}, {Y}}}";
		}

		public readonly int CompareTo (Vector2I other) {
			var results = new [] {
				X.CompareTo(other.X),
				Y.CompareTo(other.Y)
			};
			foreach (var result in results) {
				if (result != 0) {
					return result;
				}
			}
			return 0;
		}

		public readonly int CompareTo (DrawingPoint other) {
			var results = new [] {
				X.CompareTo(other.X),
				Y.CompareTo(other.Y)
			};
			foreach (var result in results) {
				if (result != 0) {
					return result;
				}
			}
			return 0;
		}

		public readonly int CompareTo (XNAPoint other) {
			var results = new [] {
				X.CompareTo(other.X),
				Y.CompareTo(other.Y)
			};
			foreach (var result in results) {
				if (result != 0) {
					return result;
				}
			}
			return 0;
		}

		public readonly int CompareTo (XTilePoint other) {
			var results = new [] {
				X.CompareTo(other.X),
				Y.CompareTo(other.Y)
			};
			foreach (var result in results) {
				if (result != 0) {
					return result;
				}
			}
			return 0;
		}

		public readonly int CompareTo (DrawingSize other) {
			var results = new [] {
				X.CompareTo(other.Width),
				Y.CompareTo(other.Height)
			};
			foreach (var result in results) {
				if (result != 0) {
					return result;
				}
			}
			return 0;
		}

		public readonly int CompareTo (XTileSize other) {
			var results = new [] {
				X.CompareTo(other.Width),
				Y.CompareTo(other.Height)
			};
			foreach (var result in results) {
				if (result != 0) {
					return result;
				}
			}
			return 0;
		}

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

		public readonly override int GetHashCode () {
			return unchecked((int)Hashing.CombineHash(X.GetHashCode(), Y.GetHashCode()));
		}

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

		public readonly bool Equals (Vector2I other) {
			return
				X == other.X &&
				Y == other.Y;
		}

		public readonly bool Equals (DrawingPoint other) {
			return
				X == other.X &&
				Y == other.Y;
		}

		public readonly bool Equals (XNAPoint other) {
			return
				X == other.X &&
				Y == other.Y;
		}

		public readonly bool Equals (XTilePoint other) {
			return
				X == other.X &&
				Y == other.Y;
		}

		public readonly bool Equals (DrawingSize other) {
			return
				Width == other.Width &&
				Height == other.Height;
		}

		public readonly bool Equals (XTileSize other) {
			return
				Width == other.Width &&
				Height == other.Height;
		}

		public readonly bool NotEquals (Vector2I other) {
			return
				X != other.X ||
				Y != other.Y;
		}

		public readonly bool NotEquals (DrawingPoint other) {
			return
				X != other.X ||
				Y != other.Y;
		}

		public readonly bool NotEquals (XNAPoint other) {
			return
				X != other.X ||
				Y != other.Y;
		}

		public readonly bool NotEquals (XTilePoint other) {
			return
				X != other.X ||
				Y != other.Y;
		}

		public readonly bool NotEquals (DrawingSize other) {
			return
				Width != other.Width ||
				Height != other.Height;
		}

		public readonly bool NotEquals (XTileSize other) {
			return
				Width != other.Width ||
				Height != other.Height;
		}

		public static bool operator == (Vector2I lhs, Vector2I rhs) {
			return lhs.Equals(rhs);
		}

		public static bool operator != (Vector2I lhs, Vector2I rhs) {
			return lhs.NotEquals(rhs);
		}
		public static bool operator == (Vector2I lhs, DrawingPoint rhs) {
			return lhs.Equals(rhs);
		}

		public static bool operator != (Vector2I lhs, DrawingPoint rhs) {
			return lhs.NotEquals(rhs);
		}

		public static bool operator == (DrawingPoint lhs, Vector2I rhs) {
			return rhs.Equals(lhs);
		}

		public static bool operator != (DrawingPoint lhs, Vector2I rhs) {
			return rhs.NotEquals(lhs);
		}

		public static bool operator == (Vector2I lhs, XNAPoint rhs) {
			return lhs.Equals(rhs);
		}

		public static bool operator != (Vector2I lhs, XNAPoint rhs) {
			return lhs.NotEquals(rhs);
		}

		public static bool operator == (XNAPoint lhs, Vector2I rhs) {
			return rhs.Equals(lhs);
		}

		public static bool operator != (XNAPoint lhs, Vector2I rhs) {
			return rhs.NotEquals(lhs);
		}

		public static bool operator == (Vector2I lhs, XTilePoint rhs) {
			return lhs.Equals(rhs);
		}

		public static bool operator != (Vector2I lhs, XTilePoint rhs) {
			return lhs.NotEquals(rhs);
		}

		public static bool operator == (XTilePoint lhs, Vector2I rhs) {
			return rhs.Equals(lhs);
		}

		public static bool operator != (XTilePoint lhs, Vector2I rhs) {
			return rhs.NotEquals(lhs);
		}

		public static bool operator == (Vector2I lhs, DrawingSize rhs) {
			return lhs.Equals(rhs);
		}

		public static bool operator != (Vector2I lhs, DrawingSize rhs) {
			return lhs.NotEquals(rhs);
		}

		public static bool operator == (DrawingSize lhs, Vector2I rhs) {
			return rhs.Equals(lhs);
		}

		public static bool operator != (DrawingSize lhs, Vector2I rhs) {
			return rhs.NotEquals(lhs);
		}

		public static bool operator == (Vector2I lhs, XTileSize rhs) {
			return lhs.Equals(rhs);
		}

		public static bool operator != (in Vector2I lhs, XTileSize rhs) {
			return lhs.NotEquals(rhs);
		}

		public static bool operator == (XTileSize lhs, Vector2I rhs) {
			return rhs.Equals(lhs);
		}

		public static bool operator != (XTileSize lhs, Vector2I rhs) {
			return rhs.NotEquals(lhs);
		}
	}
}
