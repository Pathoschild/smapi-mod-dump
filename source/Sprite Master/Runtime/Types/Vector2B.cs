using SpriteMaster.Extensions;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Types {
	[DebuggerDisplay("[{X}, {Y}}")]
	public struct Vector2B :
		ICloneable,
		IComparable,
		IComparable<Vector2B>,
		IComparable<bool>,
		IEquatable<Vector2B>,
		IEquatable<bool> {
		public static readonly Vector2B True = new Vector2B(true);
		public static readonly Vector2B False = new Vector2B(false);

		/*
		// TODO : would an int be faster? Since it would be a native type?
		// On x86, at least, populating a register with a byte should clear the upper bits anyways,
		// and our operations don't care about the upper bits.
		*/

		private const byte ZeroByte = 0;
		private const byte X_Bit = 0;
		private const byte Y_Bit = 1;
		private const byte X_Value = 1 << X_Bit;
		private const byte Y_Value = 1 << Y_Bit;
		private const byte All_Value = X_Value | Y_Value;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static byte GetX(bool value) {
			return value ? X_Value : ZeroByte;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static byte GetY (bool value) {
			return value ? Y_Value : ZeroByte;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static byte Get(bool x, bool y) {
			return (byte)(GetX(x) | GetY(y));
		}

		public byte Packed;

		public bool X {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get {
				return (byte)(Packed & X_Value) != ZeroByte;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set {
				Packed.SetBit(X_Bit, value);
			}
		}
		public bool Y {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get {
				return (byte)(Packed & Y_Value) != ZeroByte;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set {
				Packed.SetBit(Y_Bit, value);
			}
		}

		public bool Width { [MethodImpl(MethodImplOptions.AggressiveInlining)] readonly get => X; [MethodImpl(MethodImplOptions.AggressiveInlining)] set { X = value; } }
		public bool Height { [MethodImpl(MethodImplOptions.AggressiveInlining)] readonly get => Y; [MethodImpl(MethodImplOptions.AggressiveInlining)] set { Y = value; } }

		public bool Negative { [MethodImpl(MethodImplOptions.AggressiveInlining)] readonly get => X; [MethodImpl(MethodImplOptions.AggressiveInlining)] set { X = value; } }
		public bool Positive { [MethodImpl(MethodImplOptions.AggressiveInlining)] readonly get => Y; [MethodImpl(MethodImplOptions.AggressiveInlining)] set { Y = value; } }

		public readonly bool None => Packed == ZeroByte;
		public readonly bool Any => Packed != ZeroByte;
		public readonly bool All => Packed == All_Value;

		public bool this[int index] {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get {
#if DEBUG
				if (index < 0 || index >= Y_Value) {
					throw new IndexOutOfRangeException(nameof(index));
				}
#endif
				return Packed.GetBit(index);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set {
#if DEBUG
				if (index < 0 || index >= Y_Value) {
					throw new IndexOutOfRangeException(nameof(index));
				}
#endif
				// https://graphics.stanford.edu/~seander/bithacks.html#ConditionalSetOrClearBitsWithoutBranching
				Packed.SetBit(index, value);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2B (byte value) {
			Packed = value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2B (bool x, bool y) : this(Get(x, y)) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2B (bool value) : this(value ? All_Value : ZeroByte) { }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2B (Vector2B vec) : this(vec.Packed) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2B Set (Vector2B vec) {
			Packed = vec.Packed;
			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2B Set (bool x, bool y) {
			Packed = Get(x, y);
			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2B Set (bool v) {
			Packed = v ? All_Value : ZeroByte;
			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Vector2B Clone () {
			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly object ICloneable.Clone () {
			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2B operator & (Vector2B lhs, Vector2B rhs) {
			return new Vector2B((byte)(lhs.Packed & rhs.Packed));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2B operator & (Vector2B lhs, bool rhs) {
			return rhs ? lhs : False;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2B operator | (Vector2B lhs, Vector2B rhs) {
			return new Vector2B((byte)(lhs.Packed | rhs.Packed));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2B operator | (Vector2B lhs, bool rhs) {
			return rhs ? True : lhs;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly string ToString () {
			return $"[{X}, {Y}]";
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly int CompareTo (object obj) {
			if (obj is Vector2B other) {
				return CompareTo(other);
			}
			throw new ArgumentException();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly int CompareTo (Vector2B other) {
			return Packed.CompareTo(other.Packed);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals (Vector2B other) {
			return Packed == other.Packed;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int CompareTo (bool other) {
			return Packed.CompareTo(other ? All_Value : ZeroByte);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals (bool other) {
			return Packed == (other ? All_Value : ZeroByte);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TypeCode GetTypeCode () {
			return TypeCode.Object;
		}
	}
}
