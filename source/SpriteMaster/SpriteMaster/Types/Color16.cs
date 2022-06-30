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
using SpriteMaster.Hashing;
using SpriteMaster.Types.Fixed;
using SpriteMaster.Types.Spans;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SpriteMaster.Types;

[DebuggerDisplay("[{R.Value}, {G.Value}, {B.Value}, {A.Value}]")]
[StructLayout(LayoutKind.Sequential, Pack = sizeof(ulong), Size = sizeof(ulong))]
internal readonly struct Color16 : IEquatable<Color16>, IEquatable<ulong>, ILongHash {
	internal static readonly Color16 Zero = new(0UL);

	internal readonly ulong Packed = 0;

	internal readonly ulong AsPacked => Packed;

	internal readonly Fixed16 R {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new((ushort)(Packed >> 0));
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => Unsafe.AsRef(in Packed) = (Packed & 0xFFFF_FFFF_FFFF_0000UL) | (((ulong)value.Value) << 0);
	}

	internal readonly Fixed16 G {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new((ushort)(Packed >> 16));
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => Unsafe.AsRef(in Packed) = (Packed & 0xFFFF_FFFF_0000_FFFFUL) | (((ulong)value.Value) << 16);
	}

	internal readonly Fixed16 B {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new((ushort)(Packed >> 32));
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => Unsafe.AsRef(in Packed) = (Packed & 0xFFFF_0000_FFFF_FFFFUL) | (((ulong)value.Value) << 32);
	}

	internal readonly Fixed16 A {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new((ushort)(Packed >> 48));
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => Unsafe.AsRef(in Packed) = (Packed & 0x0000_FFFF_FFFF_FFFFUL) | (((ulong)value.Value) << 48);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal readonly void SetRgb(ushort r, ushort g, ushort b) {
		ulong tempPacked = Packed;
		ref ulong packed = ref Unsafe.AsRef(in Packed);
		packed = (tempPacked & 0xFFFF_0000_0000_0000UL) | (((ulong)r) | (((ulong)g) << 16) | (((ulong)b) << 32));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal readonly void SetRgb(Fixed16 r, Fixed16 g, Fixed16 b) =>
		SetRgb(r.Value, g.Value, b.Value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal readonly void SetRgbTest(ulong r, ulong g, ulong b) {
		ulong tempPacked = Packed;
		ref ulong packed = ref Unsafe.AsRef(in Packed);
		packed = (tempPacked & 0xFFFF_0000_0000_0000UL) | r | (g << 16) | (b << 32);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal readonly void SetRgbTest(Fixed16 r, Fixed16 g, Fixed16 b) =>
		SetRgb(r.Value, g.Value, b.Value);

	internal readonly Color16 NoAlpha => this with { A = 0 };

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ulong MakeMask(bool r, bool g, bool b, bool a) {
		// ToShort returns 0 or 1 for the mask. Negating it will turn that into 0 or -1, and -1 is 0xFF...
		var rr = (ulong)(ushort)(-r.ToUShort());
		var gg = ((ulong)(ushort)(-g.ToUShort())) << 16;
		var bb = ((ulong)(ushort)(-b.ToUShort())) << 32;
		var aa = ((ulong)(ushort)(-a.ToUShort())) << 48;
		return rr | gg | bb | aa;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal readonly Color16 Mask(bool r = true, bool g = true, bool b = true, bool a = true) => new(Packed & MakeMask(r, g, b, a));

	private static Color16 From(Color8 color) => new(
		(Fixed16)color.R,
		(Fixed16)color.G,
		(Fixed16)color.B,
		(Fixed16)color.A
	);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal Color16(ulong rgba) : this() {
		Packed = rgba;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal Color16((ushort R, ushort G, ushort B) color) : this(color.R, color.G, color.B) { }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal Color16(ushort r, ushort g, ushort b) : this() {
		R = r;
		G = g;
		B = b;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal Color16((Fixed16 R, Fixed16 G, Fixed16 B) color) : this(color.R, color.G, color.B) { }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal Color16(Fixed16 r, Fixed16 g, Fixed16 b) : this() {
		R = r;
		G = g;
		B = b;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal Color16((ushort R, ushort G, ushort B, ushort A) color) : this(color.R, color.G, color.B, color.A) { }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal Color16(ushort r, ushort g, ushort b, ushort a) : this() {
		R = r;
		G = g;
		B = b;
		A = a;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal Color16((Fixed16 R, Fixed16 G, Fixed16 B, Fixed16 A) color) : this(color.R, color.G, color.B, color.A) { }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal Color16(Fixed16 r, Fixed16 g, Fixed16 b, Fixed16 a) : this() {
		R = r;
		G = g;
		B = b;
		A = a;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator ulong(Color16 value) => value.Packed;
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator Color16(ulong value) => new(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly bool Equals(object? obj) {
		if (obj is Color16 color) {
			return this == color;
		}
		if (obj is ulong value) {
			return this == (Color16)value;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal readonly bool Equals(Color16 other) => this == other;
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal readonly bool Equals(ulong other) => this == (Color16)other;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly bool IEquatable<Color16>.Equals(Color16 other) => Equals(other);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly bool IEquatable<ulong>.Equals(ulong other) => Equals(other);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Color16 lhs, Color16 rhs) => lhs.Packed == rhs.Packed;
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
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

	internal static Span<Color16> Convert(ReadOnlySpan<Color8> source) {
		var destination = SpanExt.Make<Color16>(source.Length);
		for (int i = 0; i < source.Length; ++i) {
			destination[i] = From(source[i]);
		}
		return destination;
	}

	internal static PinnedSpan<Color16> ConvertPinned(ReadOnlySpan<Color8> source) {
		var destination = SpanExt.MakePinned<Color16>(source.Length);
		for (int i = 0; i < source.Length; ++i) {
			destination[i] = From(source[i]);
		}
		return destination;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly int GetHashCode() => Packed.GetHashCode();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly ulong GetLongHashCode() => HashUtility.Combine(Packed);

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
