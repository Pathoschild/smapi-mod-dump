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
using System.Runtime.InteropServices;

namespace SpriteMaster.Types;

[StructLayout(LayoutKind.Explicit, Pack = sizeof(uint), Size = sizeof(uint))]
struct Color8 : IEquatable<Color8>, IEquatable<uint>, ILongHash {
	internal static readonly Color8 Zero = new(0U);

	[FieldOffset(0)]
	internal uint Packed = 0;

	internal readonly uint AsPacked => Packed;

	[FieldOffset(0)]
	internal Fixed8 R = 0;
	[FieldOffset(1)]
	internal Fixed8 G = 0;
	[FieldOffset(2)]
	internal Fixed8 B = 0;
	[FieldOffset(3)]
	internal Fixed8 A = 0;

	internal readonly Color8 NoAlpha => new(R, G, B, 0);

	private static uint MakeMask(bool r, bool g, bool b, bool a) {
		// ToSByte returns 0 or 1 for the mask. Negating it will turn that into 0 or -1, and -1 is 0xFF...
		var rr = (uint)(byte)(-r.ToSByte());
		var gg = ((uint)(byte)(-g.ToSByte())) << 8;
		var bb = ((uint)(byte)(-b.ToSByte())) << 16;
		var aa = ((uint)(byte)(-a.ToSByte())) << 24;
		return rr | gg | bb | aa;
	}
	internal readonly Color8 Mask(bool r = true, bool g = true, bool b = true, bool a = true) => new(Packed & MakeMask(r, g, b, a));

	private static Color8 From(Color16 color) => new(
		(Fixed8)color.R,
		(Fixed8)color.G,
		(Fixed8)color.B,
		(Fixed8)color.A
	);

	internal Color8(uint rgba) : this() {
		Packed = rgba;
	}

	internal Color8(in (byte R, byte G, byte B) color) : this(color.R, color.G, color.B) {}

	internal Color8(byte r, byte g, byte b) : this() {
		R = r;
		G = g;
		B = b;
	}

	internal Color8(in (Fixed8 R, Fixed8 G, Fixed8 B) color) : this(color.R, color.G, color.B) { }

	internal Color8(Fixed8 r, Fixed8 g, Fixed8 b) : this() {
		R = r;
		G = g;
		B = b;
	}

	internal Color8(in (byte R, byte G, byte B, byte A) color) : this(color.R, color.G, color.B, color.A) { }

	internal Color8(byte r, byte g, byte b, byte a) : this() {
		R = r;
		G = g;
		B = b;
		A = a;
	}

	internal Color8(in (Fixed8 R, Fixed8 G, Fixed8 B, Fixed8 A) color) : this(color.R, color.G, color.B, color.A) { }

	internal Color8(Fixed8 r, Fixed8 g, Fixed8 b, Fixed8 a) : this() {
		R = r;
		G = g;
		B = b;
		A = a;
	}

	public static explicit operator uint(Color8 value) => value.Packed;
	public static explicit operator Color8(uint value) => new(value);

	public override readonly bool Equals(object? obj) {
		if (obj is Color8 color) {
			return this == color;
		}
		if (obj is uint value) {
			return this == (Color8)value;
		}
		return false;
	}

	internal readonly bool Equals(Color8 other) => this == other;
	internal readonly bool Equals(uint other) => this == (Color8)other;

	readonly bool IEquatable<Color8>.Equals(Color8 other) => this.Equals(other);

	readonly bool IEquatable<uint>.Equals(uint other) => this.Equals(other);

	public static bool operator ==(Color8 lhs, Color8 rhs) => lhs.Packed == rhs.Packed;
	public static bool operator !=(Color8 lhs, Color8 rhs) => lhs.Packed != rhs.Packed;

	internal static unsafe void Convert(Color16* source, Color8* destination, int count) {
		for (int i = 0; i < count; ++i) {
			destination[i] = From(source[i]);
		}
	}

	internal static void Convert(ReadOnlySpan<Color16> source, Span<Color8> destination, int count) {
		for (int i = 0; i < count; ++i) {
			destination[i] = From(source[i]);
		}
	}

	internal static Span<Color8> Convert(ReadOnlySpan<Color16> source, bool pinned = true) {
		var destination = SpanExt.MakeUninitialized<Color8>(source.Length, pinned: pinned);
		for (int i = 0; i < source.Length; ++i) {
			destination[i] = From(source[i]);
		}
		return destination;
	}

	public override readonly int GetHashCode() => Packed.GetHashCode();

	readonly ulong ILongHash.GetLongHashCode() => Hashing.Combine(Packed);

	static Color8() {
#if SM_INTERNAL_TESTING
		// Testing
		var rMask = MakeMask(true, false, false, false);
		Contracts.AssertEqual(rMask, 0x00_00_00_FFU);
		var gMask = MakeMask(false, true, false, false);
		Contracts.AssertEqual(gMask, 0x00_00_FF_00U);
		var bMask = MakeMask(false, false, true, false);
		Contracts.AssertEqual(bMask, 0x00_FF_00_00U);
		var aMask = MakeMask(false, false, false, true);
		Contracts.AssertEqual(aMask, 0xFF_00_00_00U);
		var allMask = MakeMask(true, true, true, true);
		Contracts.AssertEqual(allMask, 0xFF_FF_FF_FFU);
		var noneMask = MakeMask(false, false, false, false);
		Contracts.AssertEqual(noneMask, 0x00_00_00_00U);
#endif // SM_INTERNAL_TESTING
	}
}
