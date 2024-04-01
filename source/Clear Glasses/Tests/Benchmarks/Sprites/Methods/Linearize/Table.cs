/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using SpriteMaster.Colors;
using SpriteMaster.Types.Fixed;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Benchmarks.Sprites.Methods.Linearize;
internal static unsafe class Table {
	private static T* GetArrayPointer<T>(this T[] array) where T : unmanaged =>
		(T*)Unsafe.AsPointer(ref MemoryMarshal.GetArrayDataReference(array));

	[StructLayout(LayoutKind.Auto)]
	private readonly unsafe struct PinnedTable<T> where T : unmanaged {
		internal readonly T[] Array;
		internal readonly T* Pointer;

		internal readonly ref T this[int index] => ref Pointer[index];

		internal PinnedTable(int length) {
			Array = GC.AllocateUninitializedArray<T>(length, pinned: true);
			Pointer = Array.GetArrayPointer();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator T[](PinnedTable<T> table) => table.Array;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator T*(PinnedTable<T> table) => table.Pointer;
	}

	[StructLayout(LayoutKind.Auto, Pack = 1, Size = 3)]
	private readonly struct UInt24 {
		internal readonly byte Byte0;
		internal readonly byte Byte1;
		internal readonly byte Byte2;

		internal UInt24(byte byte0, byte byte1, byte byte2) {
			Byte0 = byte0;
			Byte1 = byte1;
			Byte2 = byte2;
		}

		internal unsafe readonly ref byte this[int index] => ref Unsafe.Add(
			ref Unsafe.As<UInt24, byte>(ref Unsafe.AsRef(*(UInt24*)Unsafe.AsPointer(ref Unsafe.AsRef(Byte0)))), index
		);
	}

	// 16-bit x 16-bit
	private static readonly PinnedTable<ushort> Table16x16 = new(ushort.MaxValue + 1);
	// Interpolated
	private static readonly PinnedTable<ushort> Table8x16 = new(byte.MaxValue + 1);
	// 24-bit x 24-bit
	private static readonly PinnedTable<UInt24> Table24x24 = new(0x1000000);
	// 24-bit x 32-bit
	private static readonly PinnedTable<uint> Table24x32 = new(0x1000000);

	static Table() {
		ConverterRef converter = ColorSpace.sRGB_Precise.GetConverterRef();

		for (int i = 0; i <= ushort.MaxValue; ++i) {
			Table16x16[i] = converter.Linearize(new Fixed16((ushort)i)).Value;
		}

		for (int i = 0; i <= byte.MaxValue; ++i) {
			ushort value = (ushort)(((uint)i << 8) + (uint)i);
			Table8x16[i] = converter.Linearize(new Fixed16(value)).Value;
		}

		for (uint i = 0; i < 0x1000000U; ++i) {
		}
	}
}
