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

namespace SpriteMaster.Types;

readonly struct ArrayWrapper2D<T> {
	internal readonly T[] Data;
	internal readonly uint Width;
	internal readonly uint Height;
	internal readonly uint Stride;

	internal ArrayWrapper2D(T[] data, int width, int height, int stride) {
		Contract.AssertNotNull(data);
		Contract.AssertNotNegative(width);
		Contract.AssertNotNegative(height);
		Contract.AssertNotNegative(stride);

		Data = data;
		Width = width.Unsigned();
		Height = height.Unsigned();
		Stride = stride.Unsigned();
	}

	internal ArrayWrapper2D(T[] data, int width, int height) : this(data, width, height, width) { }

	private readonly uint GetIndex(int x, int y) {
		Contract.AssertNotNegative(x);
		Contract.AssertNotNegative(y);

		return GetIndex(x.Unsigned(), y.Unsigned());
	}

	private readonly uint GetIndex(uint x, uint y) {
		var offset = y * Stride + x;

		Contract.AssertLess(offset, Data.Length.Unsigned());

		return offset;
	}

	internal T this[int x, int y] {
		readonly get => Data[GetIndex(x, y)];
		set => Data[GetIndex(x, y)] = value;
	}

	internal T this[uint x, uint y] {
		readonly get => Data[GetIndex(x, y)];
		set => Data[GetIndex(x, y)] = value;
	}
}
