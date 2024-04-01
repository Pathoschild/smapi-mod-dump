/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using Microsoft.Toolkit.HighPerformance;
using SpriteMaster.Extensions;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SpriteMaster.Hashing.Algorithms;

internal static partial class XxHash3 {
	[StructLayout(LayoutKind.Auto)]
	private readonly ref struct SegmentedSpan {
		private readonly ReadOnlySpan2D<byte> Source;

		internal readonly uint Length => (uint)Source.Length;
		internal readonly uint Width => (uint)Source.Width;

		[MethodImpl(Inline)]
		internal SegmentedSpan(ReadOnlySpan2D<byte> source) {
			Source = source;
		}

		[MethodImpl(Inline)]
		internal readonly void CopyTo(Span<byte> span) =>
			Source.CopyTo(span);

		[MethodImpl(Inline)]
		private readonly ReadOnlySpan<byte> Slice(Span<byte> destination, uint offset, uint length) {
			((uint)destination.Length).AssertGreaterEqual(length);

			uint end = offset + length;

			uint offsetRow = offset / Width;
			uint endRow = (end - 1U) / Width;

			var result = 
				offsetRow == endRow ?
					SliceFast(offset, length, offsetRow) :
					SliceSlow(destination, offset, length, offsetRow, endRow);

#if DEBUG
			var copySlice = Source.ToArray().AsSpan().Slice((int)offset, (int)length);
			copySlice.SequenceEqual(result).AssertTrue();
#endif

			return result;
		}

		[MethodImpl(Inline)]
		internal readonly unsafe byte* SlicePointer(Span<byte> destination, uint offset, uint length) =>
			Slice(destination, offset, length).AsPointerUnsafe();

		[MethodImpl(Inline)]
		internal readonly ReadOnlySpan<byte> Slice(Span<byte> destination, uint offset) =>
			Slice(destination, offset, (uint)destination.Length);

		[MethodImpl(Inline)]
		internal readonly unsafe byte* SlicePointer(Span<byte> destination, uint offset) =>
			Slice(destination, offset).AsPointerUnsafe();

		[Pure]
		[MethodImpl(Inline)]
		internal readonly ReadOnlySpan<byte> GetAtOffset(uint row, uint offset) {
#if DEBUG
			return Source.GetRowSpan((int)row).Slice((int)offset);
#else
			ref byte value = ref Source.DangerousGetReferenceAt((int)row, (int)offset);
			return MemoryMarshal.CreateReadOnlySpan(ref value, (int)(Width - offset));
#endif
		}

		[Pure]
		[MethodImpl(Inline)]
		internal readonly ReadOnlySpan<byte> GetAtOffset(uint row, uint offset, uint length) {
#if DEBUG
			return Source.GetRowSpan((int)row).Slice((int)offset, (int)length);
#else
			ref byte value = ref Source.DangerousGetReferenceAt((int)row, (int)offset);
			return MemoryMarshal.CreateReadOnlySpan(ref value, (int)length);
#endif
		}

		[MethodImpl(Hot)]
		// The slice crosses multiple rows
		private readonly ReadOnlySpan<byte> SliceSlow(Span<byte> destination, uint offset, uint length, uint startRow, uint endRow) {
			// TODO : find a faster way to do this
			uint rowOffset = offset % Width;
			uint currentWriteOffset = 0U;

			if (rowOffset != 0U) {
				uint copyLength = Math.Min(length, Width - rowOffset);

				var rowSpan = GetAtOffset(startRow, rowOffset, copyLength);
				rowSpan.CopyTo(destination.Slice(0, (int)copyLength));

				currentWriteOffset += copyLength;
				++startRow;
			}

			for (uint row = startRow; row <= endRow; ++row) {
				uint copyLength = Math.Min(length - currentWriteOffset, Width);

				var rowSpan = GetAtOffset(row, 0, copyLength);
				rowSpan.CopyTo(destination.Slice((int)currentWriteOffset, (int)copyLength));

				currentWriteOffset += copyLength;
			}

			return destination;
		}


		[Pure]
		[MethodImpl(Inline)]
		// The slice is entirely within a row
		private readonly ReadOnlySpan<byte> SliceFast(uint offset, uint length, uint row) {
			uint rowOffset = offset % Width;
			return GetAtOffset(row, rowOffset, length);
		}
	}
}
