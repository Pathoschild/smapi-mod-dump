/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using Newtonsoft.Json.Linq;

using SpriteMaster.Extensions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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

		private Vector2I ExtentReal;

		public Vector2I Offset;
		public Vector2I Extent {
			readonly get => ExtentReal;
			set {
				if (value.X < 0) {
					Invert.X = true;
					ExtentReal.X = -value.X;
				}
				else {
					ExtentReal.X = value.X;
				}

				if (value.Y < 0) {
					Invert.Y = true;
					ExtentReal.Y = -value.Y;
				}
				else {
					ExtentReal.Y = value.Y;
				}
			}
		}
		public Vector2B Invert;

		public Vector2I Position {
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			readonly get { return Offset; }
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			set { Offset = value; }
		}

		public Vector2I Location {
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			readonly get { return Offset; }
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			set { Offset = value; }
		}

		[Browsable(false)]
		public Vector2I Size {
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			readonly get { return Extent; }
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			set {
				Extent = value;
			}
		}

		public int X {
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			readonly get { return Offset.X; }
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			set { Offset.X = value; }
		}

		public int Y {
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			readonly get { return Offset.Y; }
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			set { Offset.Y = value; }
		}

		public int Width {
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			readonly get { return Extent.X; }
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			set { Extent = Vector2I.From(value, Extent.Y); }
		}

		public readonly int InvertedWidth {
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			get { return Invert.X ? -Extent.X : Extent.X; }
		}

		public int Height {
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			readonly get { return Extent.Y; }
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			set { Extent = Vector2I.From(Extent.X, value); }
		}

		public readonly int InvertedHeight {
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			get { return Invert.Y ? -Extent.Y : Extent.Y; }
		}

		[Browsable(false)]
		public int Left {
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			readonly get { return Offset.X; }
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			set {
				Width += (Offset.X - value);
				Offset.X = value;
			}
		}

		[Browsable(false)]
		public int Top {
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			readonly get { return Offset.Y; }
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			set {
				Height += (Offset.Y - value);
				Offset.Y = value;
			}
		}

		[Browsable(false)]
		public int Right {
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			readonly get { return Offset.X + Extent.X; }
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			set { Width = value - Offset.X; }
		}

		[Browsable(false)]
		public int Bottom {
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			readonly get { return Offset.Y + Extent.Y; }
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			set { Height = value - Offset.Y; }
		}

		public readonly int Area => Extent.X * Extent.Y;

		public readonly bool Degenerate => Extent.X == 0 || Extent.Y == 0;

		public readonly bool IsEmpty => Area == 0;

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public Bounds (Vector2I offset, Vector2I extent) {
			//Contract.AssertNotZero(extent.Width, $"{nameof(extent.Width)} is zero");
			//Contract.AssertNotZero(extent.Height, $"{nameof(extent.Height)} is zero");
			Offset = offset;
			ExtentReal = extent;
			Invert = new();
			if (ExtentReal.X < 0) {
				ExtentReal.X = -ExtentReal.X;
				Invert.X = true;
			}
			if (ExtentReal.Y < 0) {
				ExtentReal.Y = -ExtentReal.Y;
				Invert.Y = true;
			}
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public Bounds (int x, int y, int width, int height) : this(new Vector2I(x, y), new Vector2I(width, height)) { }

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public Bounds (int width, int height) : this(0, 0, width, height) { }

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public Bounds (Vector2I extent) : this(Vector2I.Zero, extent) { }

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public Bounds (in Bounds bounds) {
			Offset = bounds.Offset;
			ExtentReal = bounds.Extent;
			Invert = bounds.Invert;
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public Bounds (in DrawingRectangle rect) : this(rect.X, rect.Y, rect.Width, rect.Height) { }

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public Bounds (in XNARectangle rect) : this(rect.X, rect.Y, rect.Width, rect.Height) { }

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public Bounds (in XTileRectangle rect) : this(rect.X, rect.Y, rect.Width, rect.Height) { }

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public Bounds (Microsoft.Xna.Framework.Graphics.Texture2D tex) : this(tex.Width, tex.Height) { }

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public Bounds (System.Drawing.Bitmap bmp) : this(bmp.Width, bmp.Height) { }

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public bool Overlaps (in Bounds other) =>
		!(
			other.Left > Right ||
			other.Right < Left ||
			other.Top > Bottom ||
			other.Bottom < Top
		);

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public readonly Bounds Clone () => this;

		readonly object ICloneable.Clone () => this;

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static implicit operator DrawingRectangle (in Bounds bounds) => new DrawingRectangle(bounds.X, bounds.Y, bounds.InvertedWidth, bounds.InvertedHeight);

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static implicit operator XNARectangle (in Bounds bounds) => new XNARectangle(bounds.X, bounds.Y, bounds.InvertedWidth, bounds.InvertedHeight);

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static implicit operator XTileRectangle (in Bounds bounds) => new XTileRectangle(bounds.X, bounds.Y, bounds.InvertedWidth, bounds.InvertedHeight);

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static implicit operator Bounds (in DrawingRectangle rect) => new Bounds(rect);

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static implicit operator Bounds (in XNARectangle rect) => new Bounds(rect);

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static implicit operator Bounds (in XTileRectangle rect) => new Bounds(rect);

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public override readonly string ToString () => $"[[{X}, {Y}] [{InvertedWidth}, {InvertedHeight}]]";

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public readonly int CompareTo (Bounds other) {
			var result = Offset.CompareTo(other.Offset);
			if (result != 0) {
				return result;
			}
			return Extent.CompareTo(other.Extent);
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public readonly int CompareTo (DrawingRectangle other) => CompareTo((Bounds)other);

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public readonly int CompareTo (XNARectangle other) => CompareTo((Bounds)other);

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public readonly int CompareTo (XTileRectangle other) => CompareTo((Bounds)other);

		readonly int IComparable.CompareTo (object other) => other switch {
			Bounds bounds => CompareTo(bounds),
			DrawingRectangle rect => CompareTo(rect),
			XNARectangle rect => CompareTo(rect),
			XTileRectangle rect => CompareTo(rect),
			_ => throw new ArgumentException(),
		};

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public readonly override int GetHashCode () => unchecked((int)Hash.Combine(Offset.GetHashCode(), Extent.GetHashCode()));

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public readonly override bool Equals (object other) => other switch {
			Bounds bounds => Equals(bounds),
			DrawingRectangle rect => Equals(rect),
			XNARectangle rect => Equals(rect),
			XTileRectangle rect => Equals(rect),
			_ => throw new ArgumentException(),
		};

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public readonly bool Equals (Bounds other) => Offset == other.Offset && Extent == other.Extent;

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public readonly bool Equals (DrawingRectangle other) => Equals((Bounds)other);

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public readonly bool Equals (XNARectangle other) => Equals((Bounds)other);

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public readonly bool Equals (XTileRectangle other) => Equals((Bounds)other);

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public readonly bool NotEquals (in Bounds other) => Offset != other.Offset || Extent != other.Extent;

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public readonly bool NotEquals (in DrawingRectangle other) => NotEquals((Bounds)other);

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public readonly bool NotEquals (in XNARectangle other) => NotEquals((Bounds)other);

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public readonly bool NotEquals (in XTileRectangle other) => NotEquals((Bounds)other);

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static bool operator == (in Bounds lhs, in Bounds rhs) => lhs.Equals(rhs);

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static bool operator != (in Bounds lhs, in Bounds rhs) => lhs.NotEquals(rhs);

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static bool operator == (in Bounds lhs, in DrawingRectangle rhs) => lhs.Equals(rhs);

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static bool operator != (in Bounds lhs, in DrawingRectangle rhs) => lhs.NotEquals(rhs);

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static bool operator == (in DrawingRectangle lhs, in Bounds rhs) => rhs.Equals(lhs);

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static bool operator != (in DrawingRectangle lhs, in Bounds rhs) => rhs.NotEquals(lhs);


		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static bool operator == (in Bounds lhs, in XNARectangle rhs) => lhs.Equals(rhs);

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static bool operator != (in Bounds lhs, in XNARectangle rhs) => lhs.NotEquals(rhs);

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static bool operator == (in XNARectangle lhs, in Bounds rhs) => rhs.Equals(lhs);

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static bool operator != (in XNARectangle lhs, in Bounds rhs) => rhs.NotEquals(lhs);

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static bool operator == (in Bounds lhs, in XTileRectangle rhs) => lhs.Equals(rhs);

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static bool operator != (in Bounds lhs, in XTileRectangle rhs) => lhs.NotEquals(rhs);

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static bool operator == (in XTileRectangle lhs, in Bounds rhs) => rhs.Equals(lhs);

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static bool operator != (in XTileRectangle lhs, in Bounds rhs) => rhs.NotEquals(lhs);
	}
}
