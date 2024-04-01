/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using SpriteMaster.Extensions;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SpriteMaster.Types;

[DebuggerDisplay("[{X}, {Y}]")]
[StructLayout(LayoutKind.Sequential, Pack = sizeof(byte), Size = sizeof(byte))]
internal struct Vector2B :
	IComparable,
	IComparable<Vector2B>,
	IComparable<bool>,
	IComparable<(bool, bool)>,
	IEquatable<Vector2B>,
	IEquatable<bool>,
	IEquatable<(bool, bool)> {
	internal static readonly Vector2B True = new(packed: AllValue);
	internal static readonly Vector2B False = new(packed: ZeroByte);

	/*
	// TODO : would an int be faster? Since it would be a native type?
	// On x86, at least, populating a register with a byte should clear the upper bits anyways,
	// and our operations don't care about the upper bits.
	*/

	private const byte ZeroByte = 0;
	private const byte OneByte = 1;
	private const byte XBit = 0;
	private const byte YBit = 1;
	private const byte XValue = 1 << XBit;
	private const byte YValue = 1 << YBit;
	private const byte AllValue = XValue | YValue;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static byte GetX(bool value) => (byte)(value.ToByte() << XBit);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static byte GetY(bool value) => (byte)(value.ToByte() << YBit);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static byte Get(bool x, bool y) => (byte)(GetX(x) | GetY(y));

	private byte Packed = 0;

	internal bool X {
		[MethodImpl(Runtime.MethodImpl.Inline)]
		readonly get => (Packed & XValue) != ZeroByte;
		[MethodImpl(Runtime.MethodImpl.Inline)]
		set => Packed.SetBit(XBit, value);
	}
	internal bool Y {
		[MethodImpl(Runtime.MethodImpl.Inline)]
		readonly get => (Packed & YValue) != ZeroByte;
		[MethodImpl(Runtime.MethodImpl.Inline)]
		set => Packed.SetBit(YBit, value);
	}

	internal bool Width {
		[MethodImpl(Runtime.MethodImpl.Inline)]
		readonly get => X;
		[MethodImpl(Runtime.MethodImpl.Inline)]
		set => X = value;
	}
	internal bool Height {
		[MethodImpl(Runtime.MethodImpl.Inline)]
		readonly get => Y;
		[MethodImpl(Runtime.MethodImpl.Inline)]
		set => Y = value;
	}

	internal readonly bool None => Packed == ZeroByte;
	internal readonly bool Any => Packed != ZeroByte;
	internal readonly bool All => Packed == AllValue;

	internal readonly Vector2B Invert => new((byte)(~Packed & AllValue));

	[MethodImpl(Runtime.MethodImpl.Inline), DebuggerStepThrough, DebuggerHidden]
	private static int CheckIndex(int index) {
#if DEBUG
		if (index < 0 || index >= 2) {
			throw new IndexOutOfRangeException(nameof(index));
		}
#endif
		return index;
	}

	internal bool this[int index] {
		[MethodImpl(Runtime.MethodImpl.Inline)]
		readonly get => Packed.GetBit(CheckIndex(index));
		[MethodImpl(Runtime.MethodImpl.Inline)]
		set => Packed.SetBit(CheckIndex(index), value);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal Vector2B(byte packed) => Packed = packed;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal Vector2B(bool x, bool y) : this(packed: Get(x, y)) { }

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal Vector2B((bool X, bool Y) value) : this(value.X, value.Y) { }

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal Vector2B(bool value) : this(value ? AllValue : ZeroByte) { }

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal Vector2B(Vector2B vector) : this(vector.Packed) { }

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static implicit operator Vector2B((bool X, bool Y) vec) => new(vec.X, vec.Y);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static implicit operator (bool X, bool Y)(Vector2B vec) => (vec.X, vec.Y);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static Vector2B operator &(Vector2B lhs, Vector2B rhs) => new((byte)(lhs.Packed & rhs.Packed));

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static Vector2B operator &(Vector2B lhs, bool rhs) => rhs ? lhs : False;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static Vector2B operator |(Vector2B lhs, Vector2B rhs) => new((byte)(lhs.Packed | rhs.Packed));

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static Vector2B operator |(Vector2B lhs, bool rhs) => rhs ? True : lhs;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static Vector2B operator ^(Vector2B lhs, Vector2B rhs) => new((byte)(lhs.Packed ^ rhs.Packed));

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static Vector2B operator ^(Vector2B lhs, bool rhs) => new((byte)(lhs.Packed ^ (rhs ? OneByte : ZeroByte)));

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override readonly string ToString() => $"[{X}, {Y}]";

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator ==(Vector2B lhs, Vector2B rhs) => lhs.Packed == rhs.Packed;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator !=(Vector2B lhs, Vector2B rhs) => lhs.Packed != rhs.Packed;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly int CompareTo(object? obj) => obj switch {
		Vector2B vector => CompareTo(vector),
		Tuple<bool, bool> vector => CompareTo(new Vector2B(vector.Item1, vector.Item2)),
		ValueTuple<bool, bool> vector => CompareTo(vector),
		bool boolean => CompareTo(boolean),
		_ => Extensions.Exceptions.ThrowArgumentException<int>(nameof(obj), obj)
	};

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly int CompareTo(Vector2B other) => Packed.CompareTo(other.Packed);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly int CompareTo((bool, bool) other) => CompareTo((Vector2B)other);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly int CompareTo(bool other) => Packed.CompareTo(other ? OneByte : ZeroByte);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override readonly bool Equals(object? obj) => obj switch {
		Vector2B vector => Equals(vector),
		Tuple<bool, bool> vector => Equals(new Vector2B(vector.Item1, vector.Item2)),
		ValueTuple<bool, bool> vector => Equals(vector),
		bool boolean => Equals(boolean),
		_ => false
	};

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly bool Equals(Vector2B other) => Packed == other.Packed;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly bool Equals((bool, bool) other) => Equals((Vector2B)other);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly bool Equals(bool other) => Packed == (other ? OneByte : ZeroByte);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override readonly int GetHashCode() => (X, Y).GetHashCode();
}
