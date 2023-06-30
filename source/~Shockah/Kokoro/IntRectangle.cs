/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using System;
using System.Collections.Generic;

namespace Shockah.Kokoro;

public readonly struct IntRectangle : IEquatable<IntRectangle>
{
	public readonly IntPoint Min { get; init; }
	public readonly IntPoint Max { get; init; }

	public int Width
		=> Max.X - Min.X + 1;

	public int Height
		=> Max.Y - Min.Y + 1;

	public IntRectangle(IntPoint point) : this(point, point) { }

	public IntRectangle(IntPoint a, IntPoint b) : this()
	{
		this.Min = new(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
		this.Max = new(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
	}

	public IntRectangle(IntPoint point, int width, int height) : this()
	{
		if (width < 1)
			throw new ArgumentException($"{nameof(width)} must be at least 1.");
		if (height < 1)
			throw new ArgumentException($"{nameof(height)} must be at least 1.");

		IntPoint a = point;
		IntPoint b = new(point.X + width - 1, point.Y + height - 1);
		this.Min = new(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
		this.Max = new(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
	}

	public override string ToString()
		=> $"IntRectangle(Min = {Min}, Max = {Max}, Width = {Width}, Height = {Height})";

	public override bool Equals(object? obj)
		=> obj is IntRectangle rectangle && Equals(rectangle);

	public bool Equals(IntRectangle other)
		=> Min == other.Min && Max == other.Max;

	public override int GetHashCode()
		=> (Min, Max).GetHashCode();

	public void Deconstruct(out IntPoint min, out IntPoint max)
	{
		min = Min;
		max = Max;
	}

	public static bool operator ==(IntRectangle lhs, IntRectangle rhs)
		=> lhs.Equals(rhs);

	public static bool operator !=(IntRectangle lhs, IntRectangle rhs)
		=> !lhs.Equals(rhs);

	public bool Contains(IntPoint point)
		=> point.X >= Min.X && point.Y >= Min.Y && point.X <= Max.X && point.Y <= Max.Y;

	public IEnumerable<IntPoint> AllPointEnumerator()
	{
		var min = Min;
		var max = Max;
		for (int y = min.Y; y <= max.Y; y++)
			for (int x = min.X; x <= max.X; x++)
				yield return new(x, y);
	}
}