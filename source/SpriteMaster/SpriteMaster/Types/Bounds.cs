/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Hashing;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SpriteMaster.Types;

[StructLayout(LayoutKind.Sequential, Pack = Vector2I.Alignment)]
internal partial struct Bounds :
	ILongHash {
	internal static readonly Bounds Empty = new(0, 0, 0, 0);

	private Vector2I ExtentReal;

	internal Vector2I Offset;
	internal readonly Vector2F OffsetF => new(Offset);
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
	internal readonly Vector2F ExtentF => new(Extent);
	internal Vector2B Invert;

	internal Vector2I Position {
		[MethodImpl(Runtime.MethodImpl.Inline)]
		readonly get => Offset;
		[MethodImpl(Runtime.MethodImpl.Inline)]
		set => Offset = value;
	}

	internal Vector2I Location {
		[MethodImpl(Runtime.MethodImpl.Inline)]
		readonly get => Offset;
		[MethodImpl(Runtime.MethodImpl.Inline)]
		set => Offset = value;
	}

	[Browsable(false)]
	internal Vector2I Size {
		[MethodImpl(Runtime.MethodImpl.Inline)]
		readonly get => Extent;
		[MethodImpl(Runtime.MethodImpl.Inline)]
		set => Extent = value;
	}

	internal int X {
		[MethodImpl(Runtime.MethodImpl.Inline)]
		readonly get => Offset.X;
		[MethodImpl(Runtime.MethodImpl.Inline)]
		set => Offset.X = value;
	}

	internal int Y {
		[MethodImpl(Runtime.MethodImpl.Inline)]
		readonly get => Offset.Y;
		[MethodImpl(Runtime.MethodImpl.Inline)]
		set => Offset.Y = value;
	}

	internal int Width {
		[MethodImpl(Runtime.MethodImpl.Inline)]
		readonly get => Extent.X;
		[MethodImpl(Runtime.MethodImpl.Inline)]
		set => Extent = Vector2I.From(value, Extent.Y);
	}

	internal readonly int InvertedWidth {
		[MethodImpl(Runtime.MethodImpl.Inline)]
		get => Invert.X ? -Extent.X : Extent.X;
	}

	internal int Height {
		[MethodImpl(Runtime.MethodImpl.Inline)]
		readonly get => Extent.Y;
		[MethodImpl(Runtime.MethodImpl.Inline)]
		set => Extent = Vector2I.From(Extent.X, value);
	}

	internal readonly int InvertedHeight {
		[MethodImpl(Runtime.MethodImpl.Inline)]
		get => Invert.Y ? -Extent.Y : Extent.Y;
	}

	[Browsable(false)]
	internal int Left {
		[MethodImpl(Runtime.MethodImpl.Inline)]
		readonly get => Offset.X;
		[MethodImpl(Runtime.MethodImpl.Inline)]
		set {
			Width += (Offset.X - value);
			Offset.X = value;
		}
	}

	[Browsable(false)]
	internal int Top {
		[MethodImpl(Runtime.MethodImpl.Inline)]
		readonly get => Offset.Y;
		[MethodImpl(Runtime.MethodImpl.Inline)]
		set {
			Height += (Offset.Y - value);
			Offset.Y = value;
		}
	}

	[Browsable(false)]
	internal int Right {
		[MethodImpl(Runtime.MethodImpl.Inline)]
		readonly get => Offset.X + Extent.X;
		[MethodImpl(Runtime.MethodImpl.Inline)]
		set => Width = value - Offset.X;
	}

	[Browsable(false)]
	internal int Bottom {
		[MethodImpl(Runtime.MethodImpl.Inline)]
		readonly get => Offset.Y + Extent.Y;
		[MethodImpl(Runtime.MethodImpl.Inline)]
		set => Height = value - Offset.Y;
	}

	[Browsable(false)] internal readonly Vector2I End => Offset + Extent;

	[Browsable(false)]
	internal readonly Vector2I Center => Offset + (Extent / 2);

	internal readonly ExtentI XAxis => new(Left, Right);
	internal readonly ExtentI YAxis => new(Top, Bottom);

	internal readonly int Area => Extent.X * Extent.Y;

	internal readonly bool Degenerate => Extent.X == 0 || Extent.Y == 0;

	internal readonly bool IsEmpty => Area == 0;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal Bounds(Vector2I offset, Vector2I extent) {
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

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal Bounds(int x, int y, int width, int height) : this(new Vector2I(x, y), new Vector2I(width, height)) { }

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal Bounds(int width, int height) : this(0, 0, width, height) { }

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal Bounds(Vector2I extent) : this(Vector2I.Zero, extent) { }

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal Bounds(Bounds bounds) {
		Offset = bounds.Offset;
		ExtentReal = bounds.Extent;
		Invert = bounds.Invert;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal Bounds(DrawingRectangle rect) : this(rect.X, rect.Y, rect.Width, rect.Height) { }

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal Bounds(XRectangle rect) : this(rect.X, rect.Y, rect.Width, rect.Height) { }

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal Bounds(XTileRectangle rect) : this(rect.X, rect.Y, rect.Width, rect.Height) { }

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal Bounds(XTexture2D tex) : this(tex.Width, tex.Height) { }

	//[MethodImpl(Runtime.MethodImpl.Inline)]
	//internal Bounds(System.Drawing.Bitmap bmp) : this(bmp.Width, bmp.Height) { }

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal readonly bool Overlaps(Bounds other) =>
	!(
		other.Left > Right ||
		other.Right < Left ||
		other.Top > Bottom ||
		other.Bottom < Top
	);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal readonly bool Contains(Bounds other) =>
	(
		Left <= other.Left &&
		Right >= other.Right &&
		Top <= other.Top &&
		Bottom >= other.Bottom
	);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal readonly bool Contains(Vector2I vector) =>
	(
		Left <= vector.X &&
		Right >= vector.X &&
		Top <= vector.Y &&
		Bottom >= vector.Y
	);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static implicit operator DrawingRectangle(Bounds bounds) => new(bounds.X, bounds.Y, bounds.InvertedWidth, bounds.InvertedHeight);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static implicit operator XRectangle(Bounds bounds) => new(bounds.X, bounds.Y, bounds.InvertedWidth, bounds.InvertedHeight);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static implicit operator XTileRectangle(Bounds bounds) => new(bounds.X, bounds.Y, bounds.InvertedWidth, bounds.InvertedHeight);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static implicit operator Bounds(DrawingRectangle rect) => new(rect);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static implicit operator Bounds(XRectangle rect) => new(rect);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static implicit operator Bounds(XTileRectangle rect) => new(rect);

	public override readonly string ToString() => $"[[{X}, {Y}] [{InvertedWidth}, {InvertedHeight}]]";

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override readonly int GetHashCode() => HashUtility.Combine(Offset.GetHashCode(), Extent.GetHashCode());

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public ulong GetLongHashCode() => HashUtility.Combine(Offset.GetLongHashCode(), Extent.GetLongHashCode());

	internal readonly Bounds ClampTo(Bounds clamp) {
		Bounds source = this;

		// Validate that the bounds even overlap at all
		if (source.Left >= clamp.Right || source.Right <= clamp.Left || source.Top >= clamp.Bottom || source.Bottom <= clamp.Top) {
			return Empty;
		}

		int leftDiff = clamp.Left - source.Left;
		if (leftDiff > 0) {
			source.X += leftDiff;
			source.Width -= leftDiff;
		}

		int topDiff = clamp.Top - source.Top;
		if (topDiff > 0) {
			source.Y += topDiff;
			source.Height -= topDiff;
		}

		int rightDiff = source.Right - clamp.Right;
		if (rightDiff > 0) {
			source.Width -= rightDiff;
		}

		int bottomDiff = source.Bottom - clamp.Bottom;
		if (bottomDiff > 0) {
			source.Height -= bottomDiff;
		}

		return source;
	}

	internal readonly bool ClampToChecked(Bounds clamp, out Bounds clamped) {
		Bounds result = ClampTo(clamp);
		if (result != this) {
			clamped = result;
			return false;
		}
		else {
			clamped = result;
			return true;
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal readonly Bounds OffsetBy(Vector2I offset) => new(
		Offset + offset,
		Extent
	);


	internal readonly Bounds Expand(Vector2I size) {
		int left = size.X / 2;
		int right = size.X - left;

		int top = size.Y / 2;
		int bottom = size.Y - top;

		return new(
			Offset - (left, top),
			Extent + (left + right, top + bottom)
		);
	}
}
