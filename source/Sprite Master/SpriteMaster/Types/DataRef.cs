using SpriteMaster.Extensions;

namespace SpriteMaster.Types {
	public ref struct DataRef<T> where T : struct {
		public readonly T[] Data;
		public readonly int Offset;
		public readonly int Length;

		public bool IsEmpty {
			get => Data == null || Length == 0;
		}

		public bool IsNull {
			get => Data == null;
		}

		public bool IsEntire {
			get => Data != null && Offset == 0 && Length == Data.Length;
		}

		public static DataRef<T> Null => new DataRef<T>(null);

		public DataRef (T[] data, int offset = 0, int length = 0) {
			Contract.AssertPositiveOrZero(offset);

			Data = data;
			Offset = offset;
			Length = (length == 0 && Data != null) ? Data.Length : length;
		}

		public static implicit operator DataRef<T> (T[] data) {
			if (data == null)
				return Null;
			return new DataRef<T>(data);
		}

		public static bool operator == (DataRef<T> lhs, DataRef<T> rhs) {
			return lhs.Data == rhs.Data && lhs.Offset == rhs.Offset;
		}

		public static bool operator == (DataRef<T> lhs, object rhs) {
			return lhs.Data == rhs;
		}

		public static bool operator != (DataRef<T> lhs, DataRef<T> rhs) {
			return lhs.Data == rhs.Data && lhs.Offset == rhs.Offset;
		}

		public static bool operator != (DataRef<T> lhs, object rhs) {
			return lhs.Data != rhs;
		}

		public readonly bool Equals (DataRef<T> other) {
			return this == other;
		}

		public readonly override bool Equals(object other) {
			return this == other;
		}

		public readonly override int GetHashCode () {
			return unchecked((int)Hashing.CombineHash(Data.GetHashCode(), Offset.GetHashCode()));
		}
	}
}
