/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Types;

partial struct Bounds :
	IEquatable<Bounds>,
	IEquatable<Bounds?>,
	IEquatable<DrawingRectangle>,
	IEquatable<DrawingRectangle?>,
	IEquatable<XNA.Rectangle>,
	IEquatable<XNA.Rectangle?>,
	IEquatable<XTileRectangle>,
	IEquatable<XTileRectangle?> {

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly override bool Equals(object? other) => other switch {
		Bounds bounds => Equals(bounds),
		DrawingRectangle rect => Equals(rect),
		XNA.Rectangle rect => Equals(rect),
		XTileRectangle rect => Equals(rect),
		_ => false,
	};

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly bool Equals(Bounds other) => Offset == other.Offset && Extent == other.Extent;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly bool Equals(Bounds? other) => other.HasValue && Offset == other.Value.Offset && Extent == other.Value.Extent;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly bool Equals(in Bounds other) => Offset == other.Offset && Extent == other.Extent;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly bool Equals(in Bounds? other) => other.HasValue && Offset == other.Value.Offset && Extent == other.Value.Extent;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly bool Equals(DrawingRectangle other) => Equals((Bounds)other);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly bool Equals(DrawingRectangle? other) => other.HasValue && Equals((Bounds)other.Value);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly bool Equals(in DrawingRectangle other) => Equals((Bounds)other);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly bool Equals(in DrawingRectangle? other) => other.HasValue && Equals((Bounds)other.Value);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly bool Equals(XNA.Rectangle other) => Equals((Bounds)other);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly bool Equals(XNA.Rectangle? other) => other.HasValue && Equals((Bounds)other.Value);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly bool Equals(in XNA.Rectangle other) => Equals((Bounds)other);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly bool Equals(in XNA.Rectangle? other) => other.HasValue && Equals((Bounds)other.Value);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly bool Equals(XTileRectangle other) => Equals((Bounds)other);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly bool Equals(XTileRectangle? other) => other.HasValue && Equals((Bounds)other.Value);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly bool Equals(in XTileRectangle other) => Equals((Bounds)other);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly bool Equals(in XTileRectangle? other) => other.HasValue && Equals((Bounds)other.Value);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator ==(in Bounds lhs, in Bounds rhs) => lhs.Equals(in rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator !=(in Bounds lhs, in Bounds rhs) => !(lhs == rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator ==(in Bounds lhs, in DrawingRectangle rhs) => lhs.Equals(rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator !=(in Bounds lhs, in DrawingRectangle rhs) => !(lhs == rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator ==(in DrawingRectangle lhs, in Bounds rhs) => rhs.Equals(lhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator !=(in DrawingRectangle lhs, in Bounds rhs) => !(lhs == rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator ==(in Bounds lhs, in XNA.Rectangle rhs) => lhs.Equals(rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator !=(in Bounds lhs, in XNA.Rectangle rhs) => !(lhs == rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator ==(in XNA.Rectangle lhs, in Bounds rhs) => rhs.Equals(lhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator !=(in XNA.Rectangle lhs, in Bounds rhs) => !(lhs == rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator ==(in Bounds lhs, in XTileRectangle rhs) => lhs.Equals(rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator !=(in Bounds lhs, in XTileRectangle rhs) => !(lhs == rhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator ==(in XTileRectangle lhs, in Bounds rhs) => rhs.Equals(lhs);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator !=(in XTileRectangle lhs, in Bounds rhs) => !(lhs == rhs);
}
