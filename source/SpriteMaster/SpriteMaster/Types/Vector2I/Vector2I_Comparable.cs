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

partial struct Vector2I :
	IComparable,
	IComparable<Vector2I>,
	IComparable<Vector2I?>,
	IComparable<(int, int)>,
	IComparable<(int, int)?>,
	IComparable<DrawingPoint>,
	IComparable<DrawingPoint?>,
	IComparable<XNA.Point>,
	IComparable<XNA.Point?>,
	IComparable<XTilePoint>,
	IComparable<XTilePoint?>,
	IComparable<DrawingSize>,
	IComparable<DrawingSize?>,
	IComparable<XTileSize>,
	IComparable<XTileSize?> {
	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly int CompareTo(Vector2I other) => Packed.CompareTo(other.Packed);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly int CompareTo(Vector2I? other) => other.HasValue ? Packed.CompareTo(other.Value.Packed) : Packed.CompareTo((object?)null);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly int CompareTo((int, int) other) => CompareTo((Vector2I)other);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly int CompareTo((int, int)? other) => other.HasValue ? CompareTo((Vector2I)other.Value) : Packed.CompareTo((object?)null);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly int CompareTo(DrawingPoint other) => CompareTo((Vector2I)other);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly int CompareTo(DrawingPoint? other) => other.HasValue ? CompareTo((Vector2I)other.Value) : Packed.CompareTo((object?)null);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly int CompareTo(XNA.Point other) => CompareTo((Vector2I)other);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly int CompareTo(XNA.Point? other) => other.HasValue ? CompareTo((Vector2I)other.Value) : Packed.CompareTo((object?)null);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly int CompareTo(XTilePoint other) => CompareTo((Vector2I)other);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly int CompareTo(XTilePoint? other) => other.HasValue ? CompareTo((Vector2I)other.Value) : Packed.CompareTo((object?)null);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly int CompareTo(DrawingSize other) => CompareTo((Vector2I)other);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly int CompareTo(DrawingSize? other) => other.HasValue ? CompareTo((Vector2I)other.Value) : Packed.CompareTo((object?)null);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly int CompareTo(XTileSize other) => CompareTo((Vector2I)other);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly int CompareTo(XTileSize? other) => other.HasValue ? CompareTo((Vector2I)other.Value) : Packed.CompareTo((object?)null);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	readonly int IComparable.CompareTo(object? other) => other switch {
		Vector2I vec => CompareTo(vec),
		DrawingPoint vec => CompareTo((Vector2I)vec),
		XNA.Point vec => CompareTo((Vector2I)vec),
		XTilePoint vec => CompareTo((Vector2I)vec),
		DrawingSize vec => CompareTo((Vector2I)vec),
		XTileSize vec => CompareTo((Vector2I)vec),
		Tuple<int, int> vector => CompareTo(new Vector2I(vector.Item1, vector.Item2)),
		ValueTuple<int, int> vector => CompareTo(vector),
		_ => throw new ArgumentException(Exceptions.BuildArgumentException(nameof(other), other))
	};
}
