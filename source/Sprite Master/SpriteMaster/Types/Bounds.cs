/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Extensions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Types;

struct Bounds :
	ICloneable,
	IComparable,
	IComparable<Bounds>,
	IComparable<DrawingRectangle>,
	IComparable<XNA.Rectangle>,
	IComparable<XTileRectangle>,
	IEquatable<Bounds>,
	IEquatable<DrawingRectangle>,
	IEquatable<XNA.Rectangle>,
	IEquatable<XTileRectangle> {
	internal static readonly Bounds Empty = new(0, 0, 0, 0);

	private Vector2I ExtentReal;

	internal Vector2I Offset;
	internal Vector2I Extent {
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
	internal Vector2B Invert;

	internal Vector2I Position {
		[MethodImpl(Runtime.MethodImpl.Hot)]
		readonly get => Offset;
		[MethodImpl(Runtime.MethodImpl.Hot)]
		set => Offset = value;
	}

	internal Vector2I Location {
		[MethodImpl(Runtime.MethodImpl.Hot)]
		readonly get => Offset;
		[MethodImpl(Runtime.MethodImpl.Hot)]
		set => Offset = value;
	}

	[Browsable(false)]
	internal Vector2I Size {
		[MethodImpl(Runtime.MethodImpl.Hot)]
		readonly get => Extent;
		[MethodImpl(Runtime.MethodImpl.Hot)]
		set => Extent = value;
	}

	internal int X {
		[MethodImpl(Runtime.MethodImpl.Hot)]
		readonly get => Offset.X;
		[MethodImpl(Runtime.MethodImpl.Hot)]
		set => Offset.X = value;
	}

	internal int Y {
		[MethodImpl(Runtime.MethodImpl.Hot)]
		readonly get => Offset.Y;
		[MethodImpl(Runtime.MethodImpl.Hot)]
		set => Offset.Y = value;
	}

	internal int Width {
		[MethodImpl(Runtime.MethodImpl.Hot)]
		readonly get => Extent.X;
		[MethodImpl(Runtime.MethodImpl.Hot)]
		set => Extent = Vector2I.From(value, Extent.Y);
	}

	internal readonly int InvertedWidth {
		[MethodImpl(Runtime.MethodImpl.Hot)]
		get => Invert.X ? -Extent.X : Extent.X;
	}

	internal int Height {
		[MethodImpl(Runtime.MethodImpl.Hot)]
		readonly get => Extent.Y;
		[MethodImpl(Runtime.MethodImpl.Hot)]
		set => Extent = Vector2I.From(Extent.X, value);
	}

	internal readonly int InvertedHeight {
		[MethodImpl(Runtime.MethodImpl.Hot)]
		get => Invert.Y ? -Extent.Y : Extent.Y;
	}

	[Browsable(false)]
	internal int Left {
		[MethodImpl(Runtime.MethodImpl.Hot)]
		readonly get => Offset.X;
		[MethodImpl(Runtime.MethodImpl.Hot)]
		set {
			Width += (Offset.X - value);
			Offset.X = value;
		}
	}

	[Browsable(false)]
	internal int Top {
		[MethodImpl(Runtime.MethodImpl.Hot)]
		readonly get => Offset.Y;
		[MethodImpl(Runtime.MethodImpl.Hot)]
		set {
			Height += (Offset.Y - value);
			Offset.Y = value;
		}
	}

	[Browsable(false)]
	internal int Right {
		[MethodImpl(Runtime.MethodImpl.Hot)]
		readonly get => Offset.X + Extent.X;
		[MethodImpl(Runtime.MethodImpl.Hot)]
		set => Width = value - Offset.X;
	}

	[Browsable(false)]
	internal int Bottom {
		[MethodImpl(Runtime.MethodImpl.Hot)]
		readonly get => Offset.Y + Extent.Y;
		[MethodImpl(Runtime.MethodImpl.Hot)]
		set => Height = value - Offset.Y;
	}

	internal readonly int Area => Extent.X * Extent.Y;

	internal readonly bool Degenerate => Extent.X == 0 || Extent.Y == 0;

	internal readonly bool IsEmpty => Area == 0;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Bounds(in Vector2I offset, in Vector2I extent) {
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

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Bounds(int x, int y, int width, int height) : this(new Vector2I(x, y), new Vector2I(width, height)) { }

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Bounds(int width, int height) : this(0, 0, width, height) { }

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Bounds(in Vector2I extent) : this(Vector2I.Zero, extent) { }

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Bounds(in Bounds bounds) {
		Offset = bounds.Offset;
		ExtentReal = bounds.Extent;
		Invert = bounds.Invert;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Bounds(in DrawingRectangle rect) : this(rect.X, rect.Y, rect.Width, rect.Height) { }

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Bounds(in XNA.Rectangle rect) : this(rect.X, rect.Y, rect.Width, rect.Height) { }

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Bounds(in XTileRectangle rect) : this(rect.X, rect.Y, rect.Width, rect.Height) { }

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Bounds(Microsoft.Xna.Framework.Graphics.Texture2D tex) : this(tex.Width, tex.Height) { }

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Bounds(System.Drawing.Bitmap bmp) : this(bmp.Width, bmp.Height) { }

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal bool Overlaps(in Bounds other) =>
	!(
		other.Left > Right ||
		other.Right < Left ||
		other.Top > Bottom ||
		other.Bottom < Top
	);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly Bounds Clone() => this;

	readonly object ICloneable.Clone() => this;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator DrawingRectangle(in Bounds bounds) => new(bounds.X, bounds.Y, bounds.InvertedWidth, bounds.InvertedHeight);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator XNA.Rectangle(in Bounds bounds) => new(bounds.X, bounds.Y, bounds.InvertedWidth, bounds.InvertedHeight);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator XTileRectangle(in Bounds bounds) => new(bounds.X, bounds.Y, bounds.InvertedWidth, bounds.InvertedHeight);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator Bounds(in DrawingRectangle rect) => new(rect);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator Bounds(in XNA.Rectangle rect) => new(rect);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator Bounds(in XTileRectangle rect) => new(rect);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public override readonly string ToString() => $"[[{X}, {Y}] [{InvertedWidth}, {InvertedHeight}]]";

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly int CompareTo(Bounds other) {
		var result = Offset.CompareTo(other.Offset);
		if (result != 0) {
			return result;
		}
		return Extent.CompareTo(other.Extent);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly int CompareTo(DrawingRectangle other) => CompareTo((Bounds)other);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly int CompareTo(XNA.Rectangle other) => CompareTo((Bounds)other);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly int CompareTo(XTileRectangle other) => CompareTo((Bounds)other);

	readonly int IComparable.CompareTo(object other) => other switch {
		Bounds bounds => CompareTo(bounds),
		DrawingRectangle rect => CompareTo(rect),
		XNA.Rectangle rect => CompareTo(rect),
		XTileRectangle rect => CompareTo(rect),
		_ => throw new ArgumentException(),
	};

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly override int GetHashCode() => (int)Hash.Combine(Offset.GetHashCode(), Extent.GetHashCode());

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly override bool Equals(object other) => other switch {
		Bounds bounds => Equals(bounds),
		DrawingRectangle rect => Equals(rect),
		XNA.Rectangle rect => Equals(rect),
		XTileRectangle rect => Equals(rect),
		_ => throw new ArgumentException(),
	};

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly bool Equals(Bounds other) => Offset == other.Offset && Extent == other.Extent;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly bool Equals(in Bounds other) => Offset == other.Offset && Extent == other.Extent;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly bool Equals(DrawingRectangle other) => Equals((Bounds)other);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly bool Equals(XNA.Rectangle other) => Equals((Bounds)other);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly bool Equals(XTileRectangle other) => Equals((Bounds)other);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly bool NotEquals(in Bounds other) => Offset != other.Offset || Extent != other.Extent;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly bool NotEquals(in DrawingRectangle other) => NotEquals((Bounds)other);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly bool NotEquals(in XNA.Rectangle other) => NotEquals((Bounds)other);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly bool NotEquals(in XTileRectangle other) => NotEquals((Bounds)other);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator ==(in Bounds lhs, in Bounds rhs) => lhs.Equals(in rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator !=(in Bounds lhs, in Bounds rhs) => lhs.NotEquals(rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator ==(in Bounds lhs, in DrawingRectangle rhs) => lhs.Equals(rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator !=(in Bounds lhs, in DrawingRectangle rhs) => lhs.NotEquals(rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator ==(in DrawingRectangle lhs, in Bounds rhs) => rhs.Equals(lhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator !=(in DrawingRectangle lhs, in Bounds rhs) => rhs.NotEquals(lhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator ==(in Bounds lhs, in XNA.Rectangle rhs) => lhs.Equals(rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator !=(in Bounds lhs, in XNA.Rectangle rhs) => lhs.NotEquals(rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator ==(in XNA.Rectangle lhs, in Bounds rhs) => rhs.Equals(lhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator !=(in XNA.Rectangle lhs, in Bounds rhs) => rhs.NotEquals(lhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator ==(in Bounds lhs, in XTileRectangle rhs) => lhs.Equals(rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator !=(in Bounds lhs, in XTileRectangle rhs) => lhs.NotEquals(rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator ==(in XTileRectangle lhs, in Bounds rhs) => rhs.Equals(lhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator !=(in XTileRectangle lhs, in Bounds rhs) => rhs.NotEquals(lhs);
}
