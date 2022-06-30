/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using Microsoft.Toolkit.HighPerformance;
using SpriteMaster.Extensions;
using SpriteMaster.Hashing;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SpriteMaster.Types;

[StructLayout(LayoutKind.Auto)]
internal readonly ref struct DataRef<T> where T : unmanaged {
	internal static DataRef<T> Null => new(null);

	private readonly ReadOnlySpan<T> DataInternal = default;
	private readonly T[]? CopiedData = null;
	internal readonly int Length => DataInternal.Length;

	internal readonly ReadOnlySpan<T> Data => CopiedData is null ? DataInternal : CopiedData.AsReadOnlySpan();

	internal readonly T[] DataCopy {
		 get {
			if (DataInternal.IsEmpty) {
				return ThrowHelper.ThrowNullReferenceException<T[]>(nameof(DataCopy));
			}

			if (CopiedData is null) {
				Unsafe.AsRef(in CopiedData) = GC.AllocateUninitializedArray<T>(DataInternal.Length);
				DataInternal.CopyToUnsafe(CopiedData.AsSpan());
			}
			return CopiedData!;
		}
	}

	internal readonly bool IsEmpty => Length == 0;

	internal readonly bool IsNull => DataInternal.IsEmpty || Length == 0;

	internal readonly bool HasData => !DataInternal.IsEmpty;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal DataRef(ReadOnlySpan<T> data, T[]? referenceArray = null, bool copied = false) {
#if DEBUG
		if (referenceArray is null && copied) {
			throw new NullReferenceException(nameof(referenceArray));
		}
#endif
		DataInternal = data;
		CopiedData = copied ? referenceArray : null;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override readonly int GetHashCode() {
		return (int)DataInternal.AsBytes().Hash();
	}
}
