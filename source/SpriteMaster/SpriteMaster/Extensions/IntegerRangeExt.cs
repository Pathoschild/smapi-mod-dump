/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using LinqFasterer;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Extensions;

static partial class Integer {
	private enum IntRangeDirection {
		Forward = -1,
		Reverse = 1,
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static unsafe IntRangeDirection GetDirection(bool getDirectionResult) {
		int v = *(byte*)&getDirectionResult;
		v = (v * 2) - 1;
		return (IntRangeDirection)v;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static IntRangeDirection GetDirection(int from, int to) => GetDirection(from < to);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static IntRangeDirection GetDirection(uint from, uint to) => GetDirection(from < to);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static IntRangeDirection GetDirection(long from, long to) => GetDirection(from < to);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static IntRangeDirection GetDirection(ulong from, ulong to) => GetDirection(from < to);

	private struct IntRangeEnumerator : IEnumerator<int>, IEnumerable<int> {
		public int Current { readonly get; private set; }
		private readonly int Start;
		private readonly int End;
		private readonly int Increment;

		object IEnumerator.Current => Current;

		[MethodImpl(Runtime.MethodImpl.Hot)]
		internal IntRangeEnumerator(int start, int end, IntRangeDirection direction) {
			start += (int)direction;
			Increment = -(int)direction;
			Start = start;
			End = end;
			Current = Start;
		}

		[MethodImpl(Runtime.MethodImpl.Hot)]
		internal IntRangeEnumerator(int start, int end) : this(start, end, GetDirection(start, end)) { }

		[MethodImpl(Runtime.MethodImpl.Hot)]
		public readonly void Dispose() { }

		[MethodImpl(Runtime.MethodImpl.Hot)]
		public bool MoveNext() {
			if (Current == End) {
				return false;
			}
			Current += Increment;
			return true;
		}

		[MethodImpl(Runtime.MethodImpl.Hot)]
		public void Reset() => Current = Start;

		[MethodImpl(Runtime.MethodImpl.Hot)]
		public readonly IEnumerator<int> GetEnumerator() => this;

		[MethodImpl(Runtime.MethodImpl.Hot)]
		readonly IEnumerator IEnumerable.GetEnumerator() => this;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static IEnumerable<int> RangeTo(this int from, int to) {
		if (from == to) {
			return EnumerableF.EmptyF<int>();
		}

		int greaterThan = (from > to).ToInt(); // 0 if not, 1 if is
		greaterThan = (greaterThan * 2) - 1; // -1 if it is less, 1 if it is greater
		var direction = (IntRangeDirection)greaterThan;
		to += greaterThan;

		return new IntRangeEnumerator(from, to, direction);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static IEnumerable<int> RangeToInclusive(this int from, int to) => new IntRangeEnumerator(from, to);

	private struct UIntRangeEnumerator : IEnumerator<uint>, IEnumerable<uint> {
		public uint Current { readonly get; private set; }
		private readonly uint Start;
		private readonly uint End;
		private readonly int Increment;

		object IEnumerator.Current => Current;

		[MethodImpl(Runtime.MethodImpl.Hot)]
		internal UIntRangeEnumerator(uint start, uint end, IntRangeDirection direction) {
			start = (uint)((int)start + (int)direction);
			Increment = -(int)direction;
			Start = start;
			End = end;
			Current = Start;
		}

		[MethodImpl(Runtime.MethodImpl.Hot)]
		internal UIntRangeEnumerator(uint start, uint end) : this(start, end, GetDirection(start, end)) { }

		[MethodImpl(Runtime.MethodImpl.Hot)]
		public readonly void Dispose() { }

		[MethodImpl(Runtime.MethodImpl.Hot)]
		public bool MoveNext() {
			if (Current == End) {
				return false;
			}
			Current = (uint)((int)Current + Increment);
			return true;
		}

		[MethodImpl(Runtime.MethodImpl.Hot)]
		public void Reset() => Current = Start;

		[MethodImpl(Runtime.MethodImpl.Hot)]
		public readonly IEnumerator<uint> GetEnumerator() => this;

		[MethodImpl(Runtime.MethodImpl.Hot)]
		readonly IEnumerator IEnumerable.GetEnumerator() => this;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static IEnumerable<uint> RangeTo(this uint from, uint to) {
		int greaterThan = (from > to).ToInt(); // 0 if not, 1 if is
		greaterThan = (greaterThan * 2) - 1; // -1 if it is less, 1 if it is greater
		var direction = (IntRangeDirection)greaterThan;
		to = (uint)((int)to + greaterThan);

		return new UIntRangeEnumerator(from, to, direction);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static IEnumerable<uint> RangeToInclusive(this uint from, uint to) => new UIntRangeEnumerator(from, to, GetDirection(from, to));

	/*
	internal static IEnumerator<int> GetEnumerator(this in Range range) {
		return new IntRangeEnumerator(range.Start.Value, range.End.Value);
	}
	*/
}
