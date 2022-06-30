/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace SpriteMaster.Hashing.Algorithms;

internal static unsafe partial class XxHash3 {
	[StructLayout(LayoutKind.Sequential, Pack = 0x10, Size = 0x40)]
	private struct CombinedVector128X512<T> where T : unmanaged {
		internal Vector128<T> Data0;
		internal Vector128<T> Data1;
		internal Vector128<T> Data2;
		internal Vector128<T> Data3;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 0x20, Size = 0x40)]
	private readonly struct CombinedVector256X512<T> where T : unmanaged {
		internal readonly ref Vector256<T> Data0 => ref GetPointer<Vector256<T>>()[0];
		internal readonly ref Vector256<T> Data1 => ref GetPointer<Vector256<T>>()[1];

		[MethodImpl(Inline)]
		private readonly TPtr* GetPointer<TPtr>() where TPtr : unmanaged => (TPtr*)Unsafe.AsPointer(ref Unsafe.AsRef(this));
	}

	[StructLayout(LayoutKind.Sequential, Pack = 0x40, Size = 0x40)]
	private readonly struct Accumulator {
		internal readonly ref CombinedVector128X512<ulong> Data128 => ref *GetPointer<CombinedVector128X512<ulong>>();

		internal readonly ref CombinedVector256X512<ulong> Data256 => ref *GetPointer<CombinedVector256X512<ulong>>();

		internal readonly ulong* Data => GetPointer<ulong>();

		[MethodImpl(Inline)]
		private readonly T* GetPointer<T>() where T : unmanaged => (T*)Unsafe.AsPointer(ref Unsafe.AsRef(this));

		[MethodImpl(Inline)]
		public Accumulator() {
			switch (VectorSize) {
				case 256: {
						Data256.Data0 = Vector256.Create(Prime32.Prime2, Prime64.Prime0, Prime64.Prime1, Prime64.Prime2);
						Data256.Data1 = Vector256.Create(Prime64.Prime3, Prime32.Prime1, Prime64.Prime4, Prime32.Prime0);
						break;
					}
				case 128: {
						Data128.Data0 = Vector128.Create(Prime32.Prime2, Prime64.Prime0);
						Data128.Data1 = Vector128.Create(Prime64.Prime1, Prime64.Prime2);
						Data128.Data2 = Vector128.Create(Prime64.Prime3, Prime32.Prime1);
						Data128.Data3 = Vector128.Create(Prime64.Prime4, Prime32.Prime0);
						break;
					}
				default: {
						Data[0] = Prime32.Prime2;
						Data[1] = Prime64.Prime0;
						Data[2] = Prime64.Prime1;
						Data[3] = Prime64.Prime2;
						Data[4] = Prime64.Prime3;
						Data[5] = Prime32.Prime1;
						Data[6] = Prime64.Prime4;
						Data[7] = Prime32.Prime0;
						break;
					}
			}
		}
	}
}
