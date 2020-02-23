using SpriteMaster.Extensions;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Types {
	public ref struct DataRef<T> where T : struct {
		public readonly T[] Data;
		public readonly int Offset;
		public readonly int Length;

		public readonly bool IsEmpty => Data == null || Length == 0;

		public readonly bool IsNull => Data == null; 

		public readonly bool IsEntire => !IsNull && Offset == 0 && Length == Data.Length;

		public static DataRef<T> Null => new DataRef<T>(null);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DataRef (T[] data, int offset = 0, int length = 0) {
			Contract.AssertPositiveOrZero(offset);

			Data = data;
			Offset = offset;
			Length = (length == 0 && Data != null) ? Data.Length : length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator DataRef<T> (T[] data) {
			if (data == null)
				return Null;
			return new DataRef<T>(data);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator == (DataRef<T> lhs, DataRef<T> rhs) {
			return lhs.Data == rhs.Data && lhs.Offset == rhs.Offset;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator == (DataRef<T> lhs, object rhs) {
			return lhs.Data == rhs;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator != (DataRef<T> lhs, DataRef<T> rhs) {
			return lhs.Data == rhs.Data && lhs.Offset == rhs.Offset;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator != (DataRef<T> lhs, object rhs) {
			return lhs.Data != rhs;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals (DataRef<T> other) {
			return this == other;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly override bool Equals(object other) {
			return this == other;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly override int GetHashCode () {
			// TODO : This isn't right. We need to hash Data _from_ the offset.
			return unchecked((int)Hash.Combine(Data.GetHashCode(), Offset.GetHashCode()));
		}
	}
}
