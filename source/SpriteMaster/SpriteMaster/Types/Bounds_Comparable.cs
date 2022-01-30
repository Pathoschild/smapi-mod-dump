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
using System.Runtime.CompilerServices;

namespace SpriteMaster.Types;

partial struct Bounds :
	IComparable,
	IComparable<Bounds>,
	IComparable<Bounds?>,
	IComparable<DrawingRectangle>,
	IComparable<DrawingRectangle?>,
	IComparable<XNA.Rectangle>,
	IComparable<XNA.Rectangle?>,
	IComparable<XTileRectangle>,
	IComparable<XTileRectangle?> {

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly int CompareTo(object? other) => other switch {
		Bounds bounds => CompareTo(bounds),
		DrawingRectangle rect => CompareTo((Bounds)rect),
		XNA.Rectangle rect => CompareTo((Bounds)rect),
		XTileRectangle rect => CompareTo((Bounds)rect),
		_ => throw new ArgumentException(Exceptions.BuildArgumentException(nameof(other), other))
	};

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly int CompareTo(Bounds other) => Offset.CompareTo(other.Offset) << 16 | (Extent.CompareTo(other.Extent) & 0xFFFF);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly int CompareTo(Bounds? other) => other.HasValue ? CompareTo(other.Value) : CompareTo((object?)null);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly int CompareTo(in Bounds other) => Offset.CompareTo(other.Offset) << 16 | (Extent.CompareTo(other.Extent) & 0xFFFF);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly int CompareTo(in Bounds? other) => other.HasValue ? CompareTo(other.Value) : CompareTo((object?)null);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly int CompareTo(DrawingRectangle other) => CompareTo((Bounds)other);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly int CompareTo(DrawingRectangle? other) => other.HasValue ? CompareTo((Bounds)other.Value) : CompareTo((object?)null);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly int CompareTo(in DrawingRectangle other) => CompareTo((Bounds)other);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly int CompareTo(in DrawingRectangle? other) => other.HasValue ? CompareTo((Bounds)other.Value) : CompareTo((object?)null);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly int CompareTo(XNA.Rectangle other) => CompareTo((Bounds)other);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly int CompareTo(XNA.Rectangle? other) => other.HasValue ? CompareTo((Bounds)other.Value) : CompareTo((object?)null);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly int CompareTo(in XNA.Rectangle other) => CompareTo((Bounds)other);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly int CompareTo(in XNA.Rectangle? other) => other.HasValue ? CompareTo((Bounds)other.Value) : CompareTo((object?)null);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly int CompareTo(XTileRectangle other) => CompareTo((Bounds)other);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly int CompareTo(XTileRectangle? other) => other.HasValue ? CompareTo((Bounds)other.Value) : CompareTo((object?)null);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly int CompareTo(in XTileRectangle other) => CompareTo((Bounds)other);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly int CompareTo(in XTileRectangle? other) => other.HasValue ? CompareTo((Bounds)other.Value) : CompareTo((object?)null);
}
