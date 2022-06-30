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
[StructLayout(LayoutKind.Sequential, Pack = sizeof(uint), Size = sizeof(uint))]
internal readonly partial struct Color8 : IEquatable<Color8>, IEquatable<uint>, ILongHash {
	internal static readonly Color8 Zero = new(0U);

	internal readonly uint Packed = 0;

	internal readonly uint AsPacked => Packed;

	internal readonly Fixed8 R {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new((byte)(Packed >> 0));
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => Unsafe.AsRef(in Packed) = (Packed & 0xFFFFFF00U) | (((uint)value.Value) << 0);
	}

	internal readonly Fixed8 G {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new((byte)(Packed >> 8));
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => Unsafe.AsRef(in Packed) = (Packed & 0xFFFF00FFU) | (((uint)value.Value) << 8);
	}

	internal readonly Fixed8 B {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new((byte)(Packed >> 16));
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => Unsafe.AsRef(in Packed) = (Packed & 0xFF00FFFFU) | (((uint)value.Value) << 16);
	}

	internal readonly Fixed8 A {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new((byte)(Packed >> 24));
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => Unsafe.AsRef(in Packed) = (Packed & 0x00FFFFFFU) | (((uint)value.Value) << 24);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal readonly void SetRgb(byte r, byte g, byte b) {
		ref uint packed = ref Unsafe.AsRef(in Packed);
		packed = (packed & 0xFF_00_00_00U) | (((uint)r) | (((uint)g) << 8) | (((uint)b) << 16));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal readonly void SetRgb(Fixed8 r, Fixed8 g, Fixed8 b) =>
		SetRgb(r.Value, g.Value, b.Value);

	internal uint ARGB => new PackedUInt(
		A.Value, R.Value, G.Value, B.Value
	);

	internal readonly Color8 NoAlpha => new(R, G, B, 0);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint MakeMask(bool r, bool g, bool b, bool a) {
		// ToSByte returns 0 or 1 for the mask. Negating it will turn that into 0 or -1, and -1 is 0xFF...
		var rr = (uint)(byte)(-r.ToByte());
		var gg = ((uint)(byte)(-g.ToByte())) << 8;
		var bb = ((uint)(byte)(-b.ToByte())) << 16;
		var aa = ((uint)(byte)(-a.ToByte())) << 24;
		return rr | gg | bb | aa;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal readonly Color8 Mask(bool r = true, bool g = true, bool b = true, bool a = true) => new(Packed & MakeMask(r, g, b, a));

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Color8 From(Color16 color) => new(
		(Fixed8)color.R,
		(Fixed8)color.G,
		(Fixed8)color.B,
		(Fixed8)color.A
	);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal Color8(uint rgba) : this() {
		Packed = rgba;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal Color8((byte R, byte G, byte B) color) : this(color.R, color.G, color.B) { }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal Color8(byte r, byte g, byte b) : this() {
		R = r;
		G = g;
		B = b;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal Color8((Fixed8 R, Fixed8 G, Fixed8 B) color) : this(color.R, color.G, color.B) { }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal Color8(Fixed8 r, Fixed8 g, Fixed8 b) : this() {
		R = r;
		G = g;
		B = b;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal Color8((byte R, byte G, byte B, byte A) color) : this(color.R, color.G, color.B, color.A) { }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal Color8(byte r, byte g, byte b, byte a) : this() {
		R = r;
		G = g;
		B = b;
		A = a;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal Color8((Fixed8 R, Fixed8 G, Fixed8 B, Fixed8 A) color) : this(color.R, color.G, color.B, color.A) { }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal Color8(Fixed8 r, Fixed8 g, Fixed8 b, Fixed8 a) : this() {
		R = r;
		G = g;
		B = b;
		A = a;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator uint(Color8 value) => value.Packed;
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator Color8(uint value) => new(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator XColor(Color8 value) => new(value.Packed);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Color8(XColor value) => new(value.PackedValue);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly bool Equals(object? obj) {
		if (obj is Color8 color) {
			return this == color;
		}
		if (obj is uint value) {
			return this == (Color8)value;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal readonly bool Equals(Color8 other) => this == other;
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal readonly bool Equals(uint other) => this == (Color8)other;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal readonly bool Equals(Color8 other, int threshold) {
		var diffR = Math.Abs(R.Value - other.R.Value);
		var diffG = Math.Abs(G.Value - other.G.Value);
		var diffB = Math.Abs(B.Value - other.B.Value);
		var diffA = Math.Abs(A.Value - other.A.Value);
		return diffR <= threshold && diffG <= threshold && diffB <= threshold && diffA <= threshold;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly bool IEquatable<Color8>.Equals(Color8 other) => Equals(other);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly bool IEquatable<uint>.Equals(uint other) => Equals(other);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Color8 lhs, Color8 rhs) => lhs.Packed == rhs.Packed;
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
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

	internal static Span<Color8> Convert(ReadOnlySpan<Color16> source) {
		var destination = SpanExt.Make<Color8>(source.Length);
		for (int i = 0; i < source.Length; ++i) {
			destination[i] = From(source[i]);
		}
		return destination;
	}

	internal static PinnedSpan<Color8> ConvertPinned(ReadOnlySpan<Color16> source) {
		var destination = SpanExt.MakePinned<Color8>(source.Length);
		for (int i = 0; i < source.Length; ++i) {
			destination[i] = From(source[i]);
		}
		return destination;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly int GetHashCode() => Packed.GetHashCode();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly ulong GetLongHashCode() => HashUtility.Combine(Packed);

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
