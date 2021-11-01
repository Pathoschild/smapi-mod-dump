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

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public DataRef (T[] data, int offset = 0, int length = 0) {
			Contract.AssertPositiveOrZero(offset);

			Data = data;
			Offset = offset;
			Length = (length == 0 && Data != null) ? Data.Length : length;
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static implicit operator DataRef<T> (T[] data) {
			if (data == null)
				return Null;
			return new DataRef<T>(data);
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static bool operator == (DataRef<T> lhs, DataRef<T> rhs) => lhs.Data == rhs.Data && lhs.Offset == rhs.Offset;

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static bool operator == (DataRef<T> lhs, object rhs) => lhs.Data == rhs;

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static bool operator != (DataRef<T> lhs, DataRef<T> rhs) => lhs.Data == rhs.Data && lhs.Offset == rhs.Offset;

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static bool operator != (DataRef<T> lhs, object rhs) => lhs.Data != rhs;

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public readonly bool Equals (DataRef<T> other) => this == other;

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public readonly override bool Equals (object other) => this == other;

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public readonly override int GetHashCode () {
			// TODO : This isn't right. We need to hash Data _from_ the offset.
			return unchecked((int)Hash.Combine(Data.GetHashCode(), Offset.GetHashCode()));
		}
	}
}
