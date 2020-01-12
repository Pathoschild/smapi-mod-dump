using SpriteMaster.Extensions;
using System;
using System.ComponentModel;

namespace SpriteMaster.Types {
	using DrawingRectangle = System.Drawing.Rectangle;
	using XNARectangle = Microsoft.Xna.Framework.Rectangle;
	using XTileRectangle = xTile.Dimensions.Rectangle;

	public struct Bounds :
		ICloneable,
		IComparable,
		IComparable<Bounds>,
		IComparable<DrawingRectangle>,
		IComparable<XNARectangle>,
		IComparable<XTileRectangle>,
		IEquatable<Bounds>,
		IEquatable<DrawingRectangle>,
		IEquatable<XNARectangle>,
		IEquatable<XTileRectangle> {
		public static readonly Bounds Empty = new Bounds(0, 0, 0, 0);

		public Vector2I Offset;
		private Vector2I _Extent;

		public Vector2I Position {
			readonly get { return Offset; }
			set { Offset = value; }
		}

		public Vector2I Location {
			readonly get { return Offset; }
			set { Offset = value; }
		}

		public Vector2I Extent {
			readonly get { return _Extent; }
			set {
				value.X.AssertGreaterEqual(0);
				value.Y.AssertGreaterEqual(0);
				_Extent = value;
			}
		}

		[Browsable(false)]
		public Vector2I Size {
			readonly get { return Extent; }
			set { Extent = value; }
		}

		public int X {
			readonly get { return Offset.X; }
			set { Offset.X = value; }
		}

		public int Y {
			readonly get { return Offset.Y; }
			set { Offset.Y = value; }
		}

		public int Width {
			readonly get { return Extent.X; }
			set { value.AssertGreaterEqual(0); _Extent.X = value; }
		}

		public int Height {
			readonly get { return Extent.Y; }
			set { value.AssertGreaterEqual(0); _Extent.Y = value; }
		}

		[Browsable(false)]
		public int Left {
			readonly get { return Offset.X; }
			set { Offset.X = value; }
		}

		[Browsable(false)]
		public int Top {
			readonly get { return Offset.Y; }
			set { Offset.Y = value; }
		}

		[Browsable(false)]
		public int Right {
			readonly get { return Offset.X + Extent.X; }
			set { value.AssertGreaterEqual(Offset.X); _Extent.X = value - Offset.X; }
		}

		[Browsable(false)]
		public int Bottom {
			readonly get { return Offset.Y + Extent.Y; }
			set { value.AssertGreaterEqual(Offset.Y); _Extent.Y = value - Offset.Y; }
		}

		public int Area {
			get { return Extent.X * Extent.Y; }
		}

		public bool Degenerate {
			get { return Extent.X <= 0 || Extent.Y <= 0; }
		}

		public bool IsEmpty {
			get { return (Extent.X * Extent.Y) == 0; }
		}

		public Bounds (Vector2I offset, Vector2I extent) {
			Contract.AssertPositiveOrZero(extent.Width, $"{nameof(extent.Width)} is not positive");
			Contract.AssertPositiveOrZero(extent.Height, $"{nameof(extent.Height)} is not positive");
			Offset = offset.Clone();
			_Extent = extent.Clone();
		}

		public Bounds (int x, int y, int width, int height) : this(new Vector2I(x, y), new Vector2I(width, height)) { }

		public Bounds (int width, int height) : this(0, 0, width, height) { }

		public Bounds (Vector2I extent) : this(Vector2I.Zero, extent) { }

		public Bounds (in Bounds bounds) {
			Offset = bounds.Offset.Clone();
			_Extent = bounds.Extent.Clone();
		}

		public Bounds (in DrawingRectangle rect) : this(rect.X, rect.Y, rect.Width, rect.Height) { }

		public Bounds (in XNARectangle rect) : this(rect.X, rect.Y, rect.Width, rect.Height) { }

		public Bounds (in XTileRectangle rect) : this(rect.X, rect.Y, rect.Width, rect.Height) { }

		public Bounds (Microsoft.Xna.Framework.Graphics.Texture2D tex) : this(tex.Width, tex.Height) { }

		public Bounds (System.Drawing.Bitmap bmp) : this(bmp.Width, bmp.Height) { }

		public readonly Bounds Clone () {
			return new Bounds(this);
		}

		readonly object ICloneable.Clone () {
			return Clone();
		}

		public static implicit operator DrawingRectangle (in Bounds bounds) {
			return new DrawingRectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height);
		}

		public static implicit operator XNARectangle (in Bounds bounds) {
			return new XNARectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height);
		}

		public static implicit operator XTileRectangle (in Bounds bounds) {
			return new XTileRectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height);
		}

		public static implicit operator Bounds (in DrawingRectangle rect) {
			return new Bounds(rect);
		}

		public static implicit operator Bounds (in XNARectangle rect) {
			return new Bounds(rect);
		}

		public static implicit operator Bounds (in XTileRectangle rect) {
			return new Bounds(rect);
		}

		public override readonly string ToString () {
			return $"{{{X}, {Y}, {Width}, {Height}}}";
		}

		public readonly int CompareTo (Bounds other) {
			var results = new [] {
				X.CompareTo(other.X),
				Y.CompareTo(other.Y),
				Width.CompareTo(other.Width),
				Height.CompareTo(other.Height)
			};
			foreach (var result in results) {
				if (result != 0) {
					return result;
				}
			}
			return 0;
		}

		public readonly int CompareTo (DrawingRectangle other) {
			var results = new [] {
				X.CompareTo(other.X),
				Y.CompareTo(other.Y),
				Width.CompareTo(other.Width),
				Height.CompareTo(other.Height)
			};
			foreach (var result in results) {
				if (result != 0) {
					return result;
				}
			}
			return 0;
		}

		public readonly int CompareTo (XNARectangle other) {
			var results = new [] {
				X.CompareTo(other.X),
				Y.CompareTo(other.Y),
				Width.CompareTo(other.Width),
				Height.CompareTo(other.Height)
			};
			foreach (var result in results) {
				if (result != 0) {
					return result;
				}
			}
			return 0;
		}

		public readonly int CompareTo (XTileRectangle other) {
			var results = new [] {
				X.CompareTo(other.X),
				Y.CompareTo(other.Y),
				Width.CompareTo(other.Width),
				Height.CompareTo(other.Height)
			};
			foreach (var result in results) {
				if (result != 0) {
					return result;
				}
			}
			return 0;
		}

		readonly int IComparable.CompareTo (object other) {
			switch (other) {
				case Bounds bounds:
					return CompareTo(bounds);
				case DrawingRectangle rect:
					return CompareTo(rect);
				case XNARectangle rect:
					return CompareTo(rect);
				case XTileRectangle rect:
					return CompareTo(rect);
				default:
					throw new ArgumentException();
			}
		}

		public readonly override int GetHashCode () {
			return unchecked((int)Hashing.CombineHash(X.GetHashCode(), Y.GetHashCode(), Width.GetHashCode(), Height.GetHashCode()));
		}

		public readonly override bool Equals (object other) {
			switch (other) {
				case Bounds bounds:
					return Equals(bounds);
				case DrawingRectangle rect:
					return Equals(rect);
				case XNARectangle rect:
					return Equals(rect);
				case XTileRectangle rect:
					return Equals(rect);
				default:
					throw new ArgumentException();
			}
		}

		public readonly bool Equals (Bounds other) {
			return
				X == other.X &&
				Y == other.Y &&
				Width == other.Width &&
				Height == other.Height;
		}

		public readonly bool Equals (DrawingRectangle other) {
			return
				X == other.X &&
				Y == other.Y &&
				Width == other.Width &&
				Height == other.Height;
		}

		public readonly bool Equals (XNARectangle other) {
			return
				X == other.X &&
				Y == other.Y &&
				Width == other.Width &&
				Height == other.Height;
		}

		public readonly bool Equals (XTileRectangle other) {
			return
				X == other.X &&
				Y == other.Y &&
				Width == other.Width &&
				Height == other.Height;
		}

		public readonly bool NotEquals (in Bounds other) {
			return
				X != other.X ||
				Y != other.Y ||
				Width != other.Width ||
				Height != other.Height;
		}

		public readonly bool NotEquals (in DrawingRectangle other) {
			return
				X != other.X ||
				Y != other.Y ||
				Width != other.Width ||
				Height != other.Height;
		}

		public readonly bool NotEquals (in XNARectangle other) {
			return
				X != other.X ||
				Y != other.Y ||
				Width != other.Width ||
				Height != other.Height;
		}

		public readonly bool NotEquals (in XTileRectangle other) {
			return
				X != other.X ||
				Y != other.Y ||
				Width != other.Width ||
				Height != other.Height;
		}

		public static bool operator == (in Bounds lhs, in Bounds rhs) {
			return lhs.Equals(rhs);
		}

		public static bool operator != (in Bounds lhs, in Bounds rhs) {
			return lhs.NotEquals(rhs);
		}

		public static bool operator == (in Bounds lhs, in DrawingRectangle rhs) {
			return lhs.Equals(rhs);
		}

		public static bool operator != (in Bounds lhs, in DrawingRectangle rhs) {
			return lhs.NotEquals(rhs);
		}

		public static bool operator == (in DrawingRectangle lhs, in Bounds rhs) {
			return rhs.Equals(lhs);
		}

		public static bool operator != (in DrawingRectangle lhs, in Bounds rhs) {
			return rhs.NotEquals(lhs);
		}


		public static bool operator == (in Bounds lhs, in XNARectangle rhs) {
			return lhs.Equals(rhs);
		}

		public static bool operator != (in Bounds lhs, in XNARectangle rhs) {
			return lhs.NotEquals(rhs);
		}

		public static bool operator == (in XNARectangle lhs, in Bounds rhs) {
			return rhs.Equals(lhs);
		}

		public static bool operator != (in XNARectangle lhs, in Bounds rhs) {
			return rhs.NotEquals(lhs);
		}

		public static bool operator == (in Bounds lhs, in XTileRectangle rhs) {
			return lhs.Equals(rhs);
		}

		public static bool operator != (in Bounds lhs, in XTileRectangle rhs) {
			return lhs.NotEquals(rhs);
		}

		public static bool operator == (in XTileRectangle lhs, in Bounds rhs) {
			return rhs.Equals(lhs);
		}

		public static bool operator != (in XTileRectangle lhs, in Bounds rhs) {
			return rhs.NotEquals(lhs);
		}
	}
}
