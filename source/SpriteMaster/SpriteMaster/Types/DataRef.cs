/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Types;
readonly ref struct DataRef<T> where T : struct {
	private readonly T[]? Data_;
	internal readonly int Offset;
	internal readonly int Length;

	internal readonly T[] Data {
		get {
			if (Data_ is null) {
				throw new NullReferenceException(nameof(Data));
			}
			return Data_;
		}
	}

	internal readonly bool IsEmpty => Data_ is null || Length == 0;

	internal readonly bool IsNull => Data_ is null;

	internal readonly bool IsEntire => Data_ is not null && Offset == 0 && Length == Data_.Length;

	internal static DataRef<T> Null => new(null);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal DataRef(T[]? data, int offset = 0, int length = 0) {
		Contracts.AssertPositiveOrZero(offset);

		Data_ = data;
		Offset = offset;
		Length = (length == 0 && Data_ is not null) ? Data_.Length : length;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator DataRef<T>(T[]? data) {
		if (data is null)
			return Null;
		return new DataRef<T>(data);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator ==(in DataRef<T> lhs, in DataRef<T> rhs) => lhs.Data_ == rhs.Data_ && lhs.Offset == rhs.Offset;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator ==(in DataRef<T> lhs, object rhs) => lhs.Data_ == rhs;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator !=(in DataRef<T> lhs, in DataRef<T> rhs) => lhs.Data_ == rhs.Data_ && lhs.Offset == rhs.Offset;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool operator !=(in DataRef<T> lhs, object rhs) => lhs.Data_ != rhs;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly bool Equals(in DataRef<T> other) => this == other;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly override bool Equals(object? other) => other is not null && this == other;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly override int GetHashCode() {
		// TODO : This isn't right. We need to hash Data _from_ the offset.
		return Hashing.Combine32(Data_, Offset);
	}
}
