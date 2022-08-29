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
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SpriteMaster.Types;

[DebuggerDisplay("[{Horizontal}, {Vertical}]")]
[StructLayout(LayoutKind.Sequential, Pack = sizeof(byte), Size = sizeof(byte))]
internal struct QuadB :
	IComparable,
	IComparable<QuadB>,
	IComparable<bool>,
	IComparable<(bool, bool, bool, bool)>,
	IComparable<(Vector2B, Vector2B)>,
	IEquatable<QuadB>,
	IEquatable<bool>,
	IEquatable<(bool, bool, bool, bool)>,
	IEquatable<(Vector2B, Vector2B)> {
	internal static readonly QuadB True = new(packed: AllValue);
	internal static readonly QuadB False = new(packed: ZeroByte);

	/*
// TODO : would an int be faster? Since it would be a native type?
// On x86, at least, populating a register with a byte should clear the upper bits anyways,
// and our operations don't care about the upper bits.
*/

	private const byte ZeroByte = 0;
	private const byte OneByte = 1;
	private const byte LeftBit = 0;
	private const byte RightBit = 1;
	private const byte TopBit = 2;
	private const byte BottomBit = 3;
	private const byte LeftValue = 1 << LeftBit;
	private const byte RightValue = 1 << RightBit;
	private const byte TopValue = 1 << TopBit;
	private const byte BottomValue = 1 << BottomBit;
	private const byte AllValue = LeftValue | RightValue | TopValue | BottomValue;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static byte GetLeft(bool value) => (byte)(value.ToByte() << LeftBit);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static byte GetRight(bool value) => (byte)(value.ToByte() << RightBit);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static byte GetTop(bool value) => (byte)(value.ToByte() << TopBit);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static byte GetBottom(bool value) => (byte)(value.ToByte() << BottomBit);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static byte Get(bool left, bool right, bool top, bool bottom) => (byte)(GetLeft(left) | GetRight(right) | GetTop(top) | GetBottom(bottom));

	private byte Packed = 0;

	internal bool Left {
		[MethodImpl(Runtime.MethodImpl.Inline)]
		readonly get => (Packed & LeftValue) != ZeroByte;
		[MethodImpl(Runtime.MethodImpl.Inline)]
		set => Packed.SetBit(LeftBit, value);
	}
	internal bool Right {
		[MethodImpl(Runtime.MethodImpl.Inline)]
		readonly get => (Packed & RightValue) != ZeroByte;
		[MethodImpl(Runtime.MethodImpl.Inline)]
		set => Packed.SetBit(RightBit, value);
	}
	internal bool Top {
		[MethodImpl(Runtime.MethodImpl.Inline)]
		readonly get => (Packed & TopValue) != ZeroByte;
		[MethodImpl(Runtime.MethodImpl.Inline)]
		set => Packed.SetBit(TopBit, value);
	}
	internal bool Bottom {
		[MethodImpl(Runtime.MethodImpl.Inline)]
		readonly get => (Packed & BottomValue) != ZeroByte;
		[MethodImpl(Runtime.MethodImpl.Inline)]
		set => Packed.SetBit(BottomBit, value);
	}

	internal Vector2B Horizontal {
		[MethodImpl(Runtime.MethodImpl.Inline)]
		readonly get => (Left, Right);
		[MethodImpl(Runtime.MethodImpl.Inline)]
		set {
			Left = value.X;
			Right = value.Y;
		}
	}

	internal Vector2B Vertical {
		[MethodImpl(Runtime.MethodImpl.Inline)]
		readonly get => (Top, Bottom);
		[MethodImpl(Runtime.MethodImpl.Inline)]
		set {
			Top = value.X;
			Bottom = value.Y;
		}
	}

	internal readonly bool None => Packed == ZeroByte;
	internal readonly bool Any => Packed != ZeroByte;
	internal readonly bool All => Packed == AllValue;

	internal readonly QuadB Invert => new((byte)(~Packed & AllValue));

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal QuadB(byte packed) => Packed = packed;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal QuadB(bool left, bool right, bool top, bool bottom) : this(packed: Get(left, right, top, bottom)) { }

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal QuadB((bool Left, bool Right, bool Top, bool Bottom) value) : this(value.Left, value.Right, value.Top, value.Bottom) { }

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal QuadB(bool value) : this(value ? AllValue : ZeroByte) { }

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal QuadB(Vector2B horizontal, Vector2B vertical) : this(horizontal.X, horizontal.Y, vertical.X, vertical.Y) { }

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal QuadB((Vector2B Horizontal, Vector2B Vertical) value) : this(value.Horizontal.X, value.Horizontal.Y, value.Vertical.X, value.Vertical.Y) { }

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static implicit operator QuadB((bool Left, bool Right, bool Top, bool Bottom) vec) => new(vec);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static implicit operator (bool Left, bool Right, bool Top, bool Bottom)(QuadB vec) => (vec.Left, vec.Right, vec.Top, vec.Bottom);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static implicit operator QuadB((Vector2B Horizontal, Vector2B Vertical) vec) => new(vec);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static implicit operator (Vector2B Horizontal, Vector2B Vertical)(QuadB vec) => (vec.Horizontal, vec.Vertical);

	public static QuadB operator &(QuadB lhs, QuadB rhs) => new((byte)(lhs.Packed & rhs.Packed));

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static QuadB operator &(QuadB lhs, bool rhs) => rhs ? lhs : False;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static QuadB operator |(QuadB lhs, QuadB rhs) => new((byte)(lhs.Packed | rhs.Packed));

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static QuadB operator |(QuadB lhs, bool rhs) => rhs ? True : lhs;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static QuadB operator ^(QuadB lhs, QuadB rhs) => new((byte)(lhs.Packed ^ rhs.Packed));

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static QuadB operator ^(QuadB lhs, bool rhs) => new((byte)(lhs.Packed ^ (rhs ? OneByte : ZeroByte)));

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override readonly string ToString() => $"[{Horizontal}, {Vertical}]";

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator ==(QuadB lhs, QuadB rhs) => lhs.Packed == rhs.Packed;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator !=(QuadB lhs, QuadB rhs) => lhs.Packed != rhs.Packed;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly int CompareTo(object? obj) => obj switch {
		QuadB quad => CompareTo(quad),
		Tuple<Vector2B, Vector2B> quad => CompareTo(new QuadB(quad.Item1, quad.Item2)),
		ValueTuple<Vector2B, Vector2B> quad => CompareTo(quad),
		Tuple<bool, bool, bool, bool> quad => CompareTo(new QuadB(quad.Item1, quad.Item2, quad.Item3, quad.Item4)),
		ValueTuple<bool, bool, bool, bool> quad => CompareTo(quad),
		bool boolean => CompareTo(boolean),
		_ => Extensions.Exceptions.ThrowArgumentException<int>(nameof(obj), obj)
	};

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly int CompareTo(QuadB other) => Packed.CompareTo(other.Packed);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly int CompareTo((bool, bool, bool, bool) other) => CompareTo((QuadB)other);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly int CompareTo((Vector2B, Vector2B) other) => CompareTo((QuadB)other);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly int CompareTo(bool other) => Packed.CompareTo(other ? OneByte : ZeroByte);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override readonly bool Equals(object? obj) => obj switch {
		QuadB quad => Equals(quad),
		Tuple<Vector2B, Vector2B> quad => Equals(new QuadB(quad.Item1, quad.Item2)),
		ValueTuple<Vector2B, Vector2B> quad => Equals(quad),
		Tuple<bool, bool, bool, bool> quad => Equals(new QuadB(quad.Item1, quad.Item2, quad.Item3, quad.Item4)),
		ValueTuple<bool, bool, bool, bool> quad => Equals(quad),
		bool boolean => Equals(boolean),
		_ => false
	};

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly bool Equals(QuadB other) => Packed == other.Packed;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly bool Equals((bool, bool, bool, bool) other) => Equals((QuadB)other);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly bool Equals((Vector2B, Vector2B) other) => Equals((QuadB)other);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly bool Equals(bool other) => Packed == (other ? OneByte : ZeroByte);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override readonly int GetHashCode() => (Left, Right, Top, Bottom).GetHashCode();
}
