#nullable enable

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

// Copied and improved upon from https://github.com/bgrainger/IndexRange/blob/master/src/IndexRange/Range.cs

#if NETSTANDARD2_1
[assembly: TypeForwardedTo(typeof(System.Range))]
#else
namespace System {
	/// <summary>Represent a range has start and end indexes.</summary>
	/// <remarks>
	/// Range is used by the C# compiler to support the range syntax.
	/// <code>
	/// int[] someArray = new int[5] { 1, 2, 3, 4, 5 };
	/// int[] subArray1 = someArray[0..2]; // { 1, 2 }
	/// int[] subArray2 = someArray[1..^0]; // { 2, 3, 4, 5 }
	/// </code>
	/// </remarks>
	public readonly struct Range :
		IEquatable<Range>,
		IEnumerable,
		IEnumerable<sbyte>,
		IEnumerable<short>,
		IEnumerable<int>,
		IEnumerable<long>,
		IEnumerable<byte>,
		IEnumerable<ushort>,
		IEnumerable<uint>,
		IEnumerable<ulong>
	{
		/// <summary>Represent the inclusive start index of the Range.</summary>
		public readonly Index Start;

		/// <summary>Represent the exclusive end index of the Range.</summary>
		public readonly Index End;

		/// <summary>Construct a Range object using the start and end indexes.</summary>
		/// <param name="start">Represent the inclusive start index of the range.</param>
		/// <param name="end">Represent the exclusive end index of the range.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Range (Index start, Index end) {
			Start = start;
			End = end;
		}

		/// <summary>Indicates whether the current Range object is equal to another object of the same type.</summary>
		/// <param name="value">An object to compare with this object</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Equals (object? value) =>
				value is Range r &&
				r.Start.Equals(Start) &&
				r.End.Equals(End);

		/// <summary>Indicates whether the current Range object is equal to another Range object.</summary>
		/// <param name="other">An object to compare with this object</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)] 
		public bool Equals (Range other) => other.Start.Equals(Start) && other.End.Equals(End);

		/// <summary>Returns the hash code for this instance.</summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode () {
			return Start.GetHashCode() * 31 + End.GetHashCode();
		}

		/// <summary>Converts the value of the current Range object to its equivalent string representation.</summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString () {
			return Start + ".." + End;
		}

		/// <summary>Create a Range object starting from start index to the end of the collection.</summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)] 
		public static Range StartAt (Index start) => new Range(start, Index.End);

		/// <summary>Create a Range object starting from first element in the collection to the end Index.</summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)] 
		public static Range EndAt (Index end) => new Range(Index.Start, end);

		/// <summary>Create a Range object starting from first element to the end.</summary>
		public static Range All => new Range(Index.Start, Index.End);

		public sealed class RangeEnumerator :
			IEnumerator,
			IEnumerator<sbyte>,
			IEnumerator<short>,
			IEnumerator<int>,
			IEnumerator<long>,
			IEnumerator<byte>,
			IEnumerator<ushort>,
			IEnumerator<uint>,
			IEnumerator<ulong>
		{
			readonly Type originalType;
			readonly long Start;
			readonly long End;
			private long _Current;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public RangeEnumerator (Index start, Index end) {
				originalType = start._originalType;
				Start = start.Value;
				End = end.Value;

				if (Start <= End) {
					_Current = Start - 1;
				}
				else {
					_Current = Start + 1;
				}
			}

			public object Current => Convert.ChangeType(_Current, originalType);
			sbyte IEnumerator<sbyte>.Current => (sbyte)_Current;
			short IEnumerator<short>.Current => (short)_Current;
			int IEnumerator<int>.Current => (int)_Current;
			long IEnumerator<long>.Current => _Current;
			byte IEnumerator<byte>.Current => (byte)_Current;
			ushort IEnumerator<ushort>.Current => (ushort)_Current;
			uint IEnumerator<uint>.Current => (uint)_Current;
			ulong IEnumerator<ulong>.Current => (ulong)_Current;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Dispose () {
				// I cannot fathom what we need to dispose of.
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext () {
				if (Start <= End) {
					++_Current;
				}
				else {
					--_Current;
				}
				return _Current != End;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Reset () {
				_Current = Start;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		IEnumerator IEnumerable.GetEnumerator () {
			return new RangeEnumerator(Start, End);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IEnumerator<long> GetEnumerator () {
			return new RangeEnumerator(Start, End);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		IEnumerator<sbyte> IEnumerable<sbyte>.GetEnumerator () {
			return new RangeEnumerator(Start, End);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		IEnumerator<short> IEnumerable<short>.GetEnumerator () {
			return new RangeEnumerator(Start, End);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		IEnumerator<int> IEnumerable<int>.GetEnumerator () {
			return new RangeEnumerator(Start, End);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		IEnumerator<byte> IEnumerable<byte>.GetEnumerator () {
			return new RangeEnumerator(Start, End);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		IEnumerator<ushort> IEnumerable<ushort>.GetEnumerator () {
			return new RangeEnumerator(Start, End);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		IEnumerator<uint> IEnumerable<uint>.GetEnumerator () {
			return new RangeEnumerator(Start, End);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		IEnumerator<ulong> IEnumerable<ulong>.GetEnumerator () {
			return new RangeEnumerator(Start, End);
		}
	}
}
#endif
