using SpriteMaster.Extensions;

namespace SpriteMaster.Types {
	public struct ArrayWrapper2D<T> {
		public readonly T[] Data;
		public readonly uint Width;
		public readonly uint Height;
		public readonly uint Stride;

		public ArrayWrapper2D (T[] data, int width, int height, int stride) {
			Contract.AssertNotNull(data);
			Contract.AssertNotNegative(width);
			Contract.AssertNotNegative(height);
			Contract.AssertNotNegative(stride);

			Data = data;
			Width = width.Unsigned();
			Height = height.Unsigned();
			Stride = stride.Unsigned();
		}

		public ArrayWrapper2D (T[] data, int width, int height) : this(data, width, height, width) { }

		private readonly uint GetIndex (int x, int y) {
			Contract.AssertNotNegative(x);
			Contract.AssertNotNegative(y);

			return GetIndex(x.Unsigned(), y.Unsigned());
		}

		private readonly uint GetIndex (uint x, uint y) {
			var offset = y * Stride + x;

			Contract.AssertLess(offset, Data.Length.Unsigned());

			return offset;
		}

		public T this[int x, int y] {
			readonly get {
				return Data[GetIndex(x, y)];
			}
			set {
				Data[GetIndex(x, y)] = value;
			}
		}

		public T this[uint x, uint y] {
			readonly get {
				return Data[GetIndex(x, y)];
			}
			set {
				Data[GetIndex(x, y)] = value;
			}
		}
	}
}
