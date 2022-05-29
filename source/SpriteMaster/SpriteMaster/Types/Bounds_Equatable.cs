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

internal partial struct Bounds :
	IEquatable<Bounds>,
	IEquatable<Bounds?>,
	IEquatable<DrawingRectangle>,
	IEquatable<DrawingRectangle?>,
	IEquatable<XRectangle>,
	IEquatable<XRectangle?>,
	IEquatable<XTileRectangle>,
	IEquatable<XTileRectangle?> {

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override readonly bool Equals(object? other) => other switch {
		Bounds bounds => Equals(bounds),
		DrawingRectangle rect => Equals(rect),
		XRectangle rect => Equals(rect),
		XTileRectangle rect => Equals(rect),
		_ => false,
	};

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly bool Equals(Bounds other) => (Offset == other.Offset) & (Extent == other.Extent);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly bool Equals(Bounds? other) => other.HasValue && ((Offset == other.Value.Offset) & (Extent == other.Value.Extent));

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly bool Equals(DrawingRectangle other) => Equals((Bounds)other);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly bool Equals(DrawingRectangle? other) => other.HasValue && Equals((Bounds)other.Value);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly bool Equals(XRectangle other) => Equals((Bounds)other);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly bool Equals(XRectangle? other) => other.HasValue && Equals((Bounds)other.Value);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly bool Equals(XTileRectangle other) => Equals((Bounds)other);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly bool Equals(XTileRectangle? other) => other.HasValue && Equals((Bounds)other.Value);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator ==(Bounds lhs, Bounds rhs) => lhs.Equals(rhs);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator !=(Bounds lhs, Bounds rhs) => !(lhs == rhs);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator ==(Bounds lhs, DrawingRectangle rhs) => lhs.Equals(rhs);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator !=(Bounds lhs, DrawingRectangle rhs) => !(lhs == rhs);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator ==(DrawingRectangle lhs, Bounds rhs) => rhs.Equals(lhs);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator !=(DrawingRectangle lhs, Bounds rhs) => !(lhs == rhs);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator ==(Bounds lhs, XRectangle rhs) => lhs.Equals(rhs);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator !=(Bounds lhs, XRectangle rhs) => !(lhs == rhs);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator ==(XRectangle lhs, Bounds rhs) => rhs.Equals(lhs);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator !=(XRectangle lhs, Bounds rhs) => !(lhs == rhs);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator ==(Bounds lhs, XTileRectangle rhs) => lhs.Equals(rhs);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator !=(Bounds lhs, XTileRectangle rhs) => !(lhs == rhs);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator ==(XTileRectangle lhs, Bounds rhs) => rhs.Equals(lhs);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator !=(XTileRectangle lhs, Bounds rhs) => !(lhs == rhs);
}
