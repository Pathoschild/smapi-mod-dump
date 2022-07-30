/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

#if EXPERIMENTAL

using System;
using System.Runtime.InteropServices;

namespace SpriteMaster.Experimental;
internal static unsafe class ObjectExp {
	[Flags]
	internal enum SyncBlockFlags : uint {
		IsHashCode = 0x04000000U,
		IsHashOrSyncBlockIndex = 0x08000000U,
		SpinLock = 0x10000000U,
		GcReserve = 0x20000000U,
		FinalizerRun = 0x40000000U,
		Unused = 0x80000000U
	}

	[Flags]
	internal enum MethodTableFlags : int {
		Marked = 0x1,
	}

	[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 8)]
	internal readonly struct MethodTable {
		[FieldOffset(0)]
		internal readonly MethodTableFlags Flags;
		[FieldOffset(0)]
		private readonly nuint Value;

		internal readonly nuint Table => Value & ~(nuint)3;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
	internal readonly struct Header {
		internal readonly SyncBlockFlags SyncBlockFlags;
		internal readonly MethodTable MethodTable;
	}

	internal static Header GetHeader(this object obj) {
		GCHandle handle = GCHandle.Alloc(obj, GCHandleType.Pinned);
		IntPtr objAddress = handle.AddrOfPinnedObject();

		try {
			IntPtr headerAddress;
			if (obj is Array array) {
				headerAddress = objAddress - (8 * 3);
			}
			else {
				headerAddress = objAddress - (8 * 2);
			}

			var header = *(Header*)headerAddress;

			return header;
		}
		finally {
			handle.Free();
		}
	}
}

#endif
