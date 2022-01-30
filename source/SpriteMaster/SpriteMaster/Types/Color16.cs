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
using SpriteMaster.Types.Fixed;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SpriteMaster.Types;

[DebuggerDisplay("[{R.Value}, {G.Value}, {B.Value}, {A.Value}}")]
[StructLayout(LayoutKind.Explicit, Pack = sizeof(ulong), Size = sizeof(ulong))]
struct Color16 : IEquatable<Color16>, IEquatable<ulong>, ILongHash {
	internal static readonly Color16 Zero = new(0UL);

	[FieldOffset(0)]
	internal ulong Packed = 0;

	internal readonly ulong AsPacked => Packed;

	[FieldOffset(0)]
	internal Fixed16 R = 0;
	[FieldOffset(2)]
	internal Fixed16 G = 0;
	[FieldOffset(4)]
	internal Fixed16 B = 0;
	[FieldOffset(6)]
	internal Fixed16 A = 0;

	internal readonly Color16 NoAlpha => this with { A = 0 };

	private static ulong MakeMask(bool r, bool g, bool b, bool a) {
		// ToShort returns 0 or 1 for the mask. Negating it will turn that into 0 or -1, and -1 is 0xFF...
		var rr = (ulong)(ushort)(-r.ToShort());
		var gg = ((ulong)(ushort)(-g.ToShort())) << 16;
		var bb = ((ulong)(ushort)(-b.ToShort())) << 32;
		var aa = ((ulong)(ushort)(-a.ToShort())) << 48;
		return rr | gg | bb | aa;
	}
	internal readonly Color16 Mask(bool r = true, bool g = true, bool b = true, bool a = true) => new(Packed & MakeMask(r, g, b, a));

	private static Color16 From(Color8 color) => new(
		(Fixed16)color.R,
		(Fixed16)color.G,
		(Fixed16)color.B,
		(Fixed16)color.A
	);

	internal Color16(ulong rgba) : this() {
		Packed = rgba;
	}

	internal Color16(in (ushort R, ushort G, ushort B) color) : this(color.R, color.G, color.B) { }

	internal Color16(ushort r, ushort g, ushort b) : this() {
		R = r;
		G = g;
		B = b;
	}

	internal Color16(in (Fixed16 R, Fixed16 G, Fixed16 B) color) : this(color.R, color.G, color.B) { }

	internal Color16(Fixed16 r, Fixed16 g, Fixed16 b) : this() {
		R = r;
		G = g;
		B = b;
	}

	internal Color16(in (ushort R, ushort G, ushort B, ushort A) color) : this(color.R, color.G, color.B, color.A) { }

	internal Color16(ushort r, ushort g, ushort b, ushort a) : this() {
		R = r;
		G = g;
		B = b;
		A = a;
	}

	internal Color16(in (Fixed16 R, Fixed16 G, Fixed16 B, Fixed16 A) color) : this(color.R, color.G, color.B, color.A) { }

	internal Color16(Fixed16 r, Fixed16 g, Fixed16 b, Fixed16 a) : this() {
		R = r;
		G = g;
		B = b;
		A = a;
	}

	public static explicit operator ulong(Color16 value) => value.Packed;
	public static explicit operator Color16(ulong value) => new(value);

	public override readonly bool Equals(object? obj) {
		if (obj is Color16 color) {
			return this == color;
		}
		if (obj is ulong value) {
			return this == (Color16)value;
		}
		return false;
	}

	internal readonly bool Equals(Color16 other) => this == other;
	internal readonly bool Equals(ulong other) => this == (Color16)other;

	readonly bool IEquatable<Color16>.Equals(Color16 other) => this.Equals(other);

	readonly bool IEquatable<ulong>.Equals(ulong other) => this.Equals(other);

	public static bool operator ==(Color16 lhs, Color16 rhs) => lhs.Packed == rhs.Packed;
	public static bool operator !=(Color16 lhs, Color16 rhs) => lhs.Packed != rhs.Packed;

	internal static unsafe void Convert(Color8* source, Color16* destination, int count) {
		for (int i = 0; i < count; ++i) {
			destination[i] = From(source[i]);
		}
	}

	internal static void Convert(ReadOnlySpan<Color8> source, Span<Color16> destination, int count) {
		for (int i = 0; i < count; ++i) {
			destination[i] = From(source[i]);
		}
	}

	internal static Span<Color16> Convert(ReadOnlySpan<Color8> source, bool pinned = true) {
		var destination = SpanExt.MakeUninitialized<Color16>(source.Length, pinned: pinned);
		for (int i = 0; i < source.Length; ++i) {
			destination[i] = From(source[i]);
		}
		return destination;
	}

	public override readonly int GetHashCode() => Packed.GetHashCode();

	readonly ulong ILongHash.GetLongHashCode() => Hashing.Combine(Packed);

	static Color16() {
#if SM_INTERNAL_TESTING
		// Testing
		var rMask = MakeMask(true, false, false, false);
		Contracts.AssertEqual(rMask, 0x0000_0000_0000_FFFFUL);
		var gMask = MakeMask(false, true, false, false);
		Contracts.AssertEqual(gMask, 0x0000_0000_FFFF_0000UL);
		var bMask = MakeMask(false, false, true, false);
		Contracts.AssertEqual(bMask, 0x0000_FFFF_0000_0000UL);
		var aMask = MakeMask(false, false, false, true);
		Contracts.AssertEqual(aMask, 0xFFFF_0000_0000_0000UL);
		var allMask = MakeMask(true, true, true, true);
		Contracts.AssertEqual(allMask, 0xFFFF_FFFF_FFFF_FFFFUL);
		var noneMask = MakeMask(false, false, false, false);
		Contracts.AssertEqual(noneMask, 0x0000_0000_0000_0000UL);
#endif //SM_INTERNAL_TESTING
	}
}
