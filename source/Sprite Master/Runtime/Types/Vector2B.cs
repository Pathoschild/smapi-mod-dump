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

[DebuggerDisplay("[{X}, {Y}]")]
[StructLayout(LayoutKind.Sequential, Pack = sizeof(byte), Size = sizeof(byte))]
struct Vector2B :
	ICloneable,
	IComparable,
	IComparable<Vector2B>,
	IComparable<bool>,
	IEquatable<Vector2B>,
	IEquatable<bool> {
	internal static readonly Vector2B True = new(Packed: All_Value);
	internal static readonly Vector2B False = new(Packed: ZeroByte);

	/*
	// TODO : would an int be faster? Since it would be a native type?
	// On x86, at least, populating a register with a byte should clear the upper bits anyways,
	// and our operations don't care about the upper bits.
	*/

	private const byte ZeroByte = 0;
	private const byte OneByte = 1;
	private const byte X_Bit = 0;
	private const byte Y_Bit = 1;
	private const byte X_Value = 1 << X_Bit;
	private const byte Y_Value = 1 << Y_Bit;
	private const byte All_Value = X_Value | Y_Value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static byte GetX(bool Value) => Value ? X_Value : ZeroByte;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static byte GetY(bool Value) => Value ? Y_Value : ZeroByte;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static byte Get(bool X, bool Y) => (byte)(GetX(X) | GetY(Y));

	internal byte Packed;

	internal bool X {
		[MethodImpl(Runtime.MethodImpl.Hot)]
		readonly get => (Packed & X_Value) != ZeroByte;
		[MethodImpl(Runtime.MethodImpl.Hot)]
		set => Packed.SetBit(X_Bit, value);
	}
	internal bool Y {
		[MethodImpl(Runtime.MethodImpl.Hot)]
		readonly get => (Packed & Y_Value) != ZeroByte;
		[MethodImpl(Runtime.MethodImpl.Hot)]
		set => Packed.SetBit(Y_Bit, value);
	}

	internal bool Width { [MethodImpl(Runtime.MethodImpl.Hot)] readonly get => X; [MethodImpl(Runtime.MethodImpl.Hot)] set => X = value; }
	internal bool Height { [MethodImpl(Runtime.MethodImpl.Hot)] readonly get => Y; [MethodImpl(Runtime.MethodImpl.Hot)] set => Y = value; }

	internal bool Negative { [MethodImpl(Runtime.MethodImpl.Hot)] readonly get => X; [MethodImpl(Runtime.MethodImpl.Hot)] set => X = value; }
	internal bool Positive { [MethodImpl(Runtime.MethodImpl.Hot)] readonly get => Y; [MethodImpl(Runtime.MethodImpl.Hot)] set => Y = value; }

	internal readonly bool None => Packed == ZeroByte;
	internal readonly bool Any => Packed != ZeroByte;
	internal readonly bool All => Packed == All_Value;

	internal bool this[int index] {
		[MethodImpl(Runtime.MethodImpl.Hot)]
		readonly get {
#if DEBUG
			if (index < 0 || index >= Y_Value) {
				throw new IndexOutOfRangeException(nameof(index));
			}
#endif
			return Packed.GetBit(index);
		}
		[MethodImpl(Runtime.MethodImpl.Hot)]
		set {
#if DEBUG
			if (index < 0 || index >= Y_Value) {
				throw new IndexOutOfRangeException(nameof(index));
			}
#endif
			Packed.SetBit(index, value);
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2B(byte Packed) => this.Packed = Packed;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static Vector2B From(byte Packed) => new(Packed: Packed);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2B(bool X, bool Y) : this(Packed: Get(X, Y)) { }

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static Vector2B From(bool X, bool Y) => new(X, Y);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2B(bool Value) : this(Value ? All_Value : ZeroByte) { }

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static Vector2B From(bool Value) => new(Value: Value);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2B(Vector2B Vector) : this(Vector.Packed) { }

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static Vector2B From(Vector2B Vector) => new(Vector: Vector);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2B Set(Vector2B Vector) {
		Packed = Vector.Packed;
		return this;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2B Set(bool X, bool Y) {
		Packed = Get(X, Y);
		return this;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2B Set(bool Value) {
		Packed = Value ? All_Value : ZeroByte;
		return this;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly Vector2B Clone() => this;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	readonly object ICloneable.Clone() => this;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2B operator &(Vector2B lhs, Vector2B rhs) => new((byte)(lhs.Packed & rhs.Packed));

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2B operator &(Vector2B lhs, bool rhs) => rhs ? lhs : False;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2B operator |(Vector2B lhs, Vector2B rhs) => new((byte)(lhs.Packed | rhs.Packed));

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2B operator |(Vector2B lhs, bool rhs) => rhs ? True : lhs;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2B operator ^(Vector2B lhs, Vector2B rhs) => new((byte)(lhs.Packed ^ rhs.Packed));

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static Vector2B operator ^(Vector2B lhs, bool rhs) => new((byte)(lhs.Packed ^ (rhs ? OneByte : ZeroByte)));

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public override readonly string ToString() => $"[{X}, {Y}]";

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly int CompareTo(object obj) => obj switch {
		Vector2B vector => CompareTo(vector),
		bool boolean => CompareTo(boolean),
		_ => throw new ArgumentException(obj.GetType().Name),
	};

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly int CompareTo(Vector2B other) => Packed.CompareTo(other.Packed);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly int CompareTo(bool other) => Packed.CompareTo(other ? OneByte : ZeroByte);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly bool Equals(Vector2B other) => Packed == other.Packed;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly bool Equals(bool other) => Packed == (other ? OneByte : ZeroByte);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly TypeCode GetTypeCode() => TypeCode.Object;
}
