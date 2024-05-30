/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#if COMMON_WIDGETS

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

using Microsoft.Xna.Framework;

namespace Leclair.Stardew.Common.UI;

[DataContract]
[DebuggerDisplay("{DebugDisplayString,nq}")]
public struct KSize : IEquatable<KSize> {

	private static readonly KSize zeroSize = new(0, 0);
	private static readonly KSize invalidSize = new(-1, -1);

	public static KSize Zero => zeroSize;
	public static KSize Invalid => invalidSize;

	[DataMember]
	public int Width;

	[DataMember]
	public int Height;

	public bool IsEmpty => Width <= 0 || Height <= 0;

	public bool IsZero => Width == 0 && Height == 0;

	public bool IsValid => Width >= 0 && Height >= 0;

	internal string DebugDisplayString => Width + "x" + Height;

	public KSize() {
		Width = -1;
		Height = -1;
	}

	public KSize(int width, int height) {
		Width = width;
		Height = height;
	}

	public KSize(int value) {
		Width = value;
		Height = value;
	}

	public KSize(Point value) {
		Width = value.X;
		Height = value.Y;
	}

	public KSize(Rectangle value) {
		Width = value.Width;
		Height = value.Height;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static KSize NewSafe(int width, int height) {
		if (width < 0)
			width = 0;
		if (height < 0)
			height = 0;
		return new KSize(width, height);
	}

	public static bool operator ==(KSize left, KSize right) {
		return left.Width == right.Width && left.Height == right.Height;
	}

	public static bool operator !=(KSize left, KSize right) {
		return left.Width != right.Width || left.Height != right.Height;
	}

	public static KSize operator +(KSize a, int amount) => new(a.Width + amount, a.Height + amount);
	public static KSize operator +(KSize a, KSize b) => new(a.Width + b.Width, a.Height + b.Height);
	public static KSize operator +(KSize a, Point b) => new(a.Width + b.X, a.Height + b.Y);
	public static KSize operator +(KSize a, Rectangle b) => new(a.Width + b.Width, a.Height + b.Height);
	public static KSize operator +(KSize a, KMargins b) => new(a.Width + b.Left + b.Right, a.Height + b.Top + b.Bottom);
	public static KSize operator +(KSize a, Vector2 b) => new(a.Width + (int) MathF.Round(b.X), a.Height + (int) MathF.Round(b.Y));

	public static KSize operator -(KSize a, int amount) => new(a.Width - amount, a.Height - amount);
	public static KSize operator -(KSize a, KSize b) => new(a.Width - b.Width, a.Height - b.Height);
	public static KSize operator -(KSize a, Point b) => new(a.Width - b.X, a.Height - b.Y);
	public static KSize operator -(KSize a, Rectangle b) => new(a.Width - b.Width, a.Height - b.Height);
	public static KSize operator -(KSize a, KMargins b) => new(a.Width - b.Left - b.Right, a.Height - b.Top - b.Bottom);
	public static KSize operator -(KSize a, Vector2 b) => new(a.Width - (int) MathF.Round(b.X), a.Height - (int) MathF.Round(b.Y));

	public static KSize operator *(KSize a, int factor) => new(a.Width * factor, a.Height * factor);
	public static KSize operator *(KSize a, float factor) => new((int) MathF.Round(a.Width * factor), (int) MathF.Round(a.Height * factor));
	public static KSize operator *(KSize a, KSize b) => new(a.Width * b.Width, a.Height * b.Height);
	public static KSize operator *(KSize a, Point b) => new(a.Width * b.X, a.Height * b.Y);
	public static KSize operator *(KSize a, Rectangle b) => new(a.Width * b.Width, a.Height * b.Height);
	public static KSize operator *(KSize a, Vector2 b) => new((int) MathF.Round(a.Width * b.X), (int) MathF.Round(a.Height * b.Y));

	public static KSize operator /(KSize a, int factor) => new((int) MathF.Round(a.Width / (float) factor), (int) MathF.Round(a.Height / (float) factor));
	public static KSize operator /(KSize a, float factor) => new((int) MathF.Round(a.Width / factor), (int) MathF.Round(a.Height / factor));
	public static KSize operator /(KSize a, KSize b) => new((int)MathF.Round(a.Width / (float) b.Width), (int)MathF.Round(a.Height / (float) b.Height));
	public static KSize operator /(KSize a, Point b) => new((int)MathF.Round(a.Width / (float) b.X), (int)MathF.Round(a.Height / (float) b.Y));
	public static KSize operator /(KSize a, Rectangle b) => new((int)MathF.Round(a.Width / (float) b.Width), (int)MathF.Round(a.Height / (float) b.Height));
	public static KSize operator /(KSize a, Vector2 b) => new((int)MathF.Round(a.Width / b.X), (int)MathF.Round(a.Height / b.Y));

	public void Transpose() {
		(Width, Height) = (Height, Width);
	}

	public KSize Transposed() {
		return new(Height, Width);
	}

	public override bool Equals(object? obj) {
		if (obj is KSize size)
			return this == size;
		return false;
	}

	public bool Equals(KSize other) {
		return this == other;
	}

	public override int GetHashCode() {
		return HashCode.Combine(Width, Height);
	}

	public override string ToString() {
		return $"{{Width:{Width} Height:{Height}}}";
	}

	public void Deconstruct(out int width, out int height) {
		width = Width;
		height = Height;
	}
}

#endif
