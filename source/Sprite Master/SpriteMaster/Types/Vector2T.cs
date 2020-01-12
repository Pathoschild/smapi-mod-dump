using System;

namespace SpriteMaster.Types {
	[Obsolete("Not presently being maintained")]
	public struct Vector2T<T> : ICloneable where T : struct {
		public T X;
		public T Y;

		public T this[int index] {
			readonly get {
				return index switch
				{
					0 => X,
					1 => Y,
					_ => throw new IndexOutOfRangeException(nameof(index)),
				};
			}
			set {
				switch (index) {
					case 0:
						X = value;
						return;
					case 1:
						Y = value;
						return;
					default:
						throw new IndexOutOfRangeException(nameof(index));
				}
			}
		}

		public Vector2T (T x, T y) {
			X = x;
			Y = y;
		}

		public Vector2T (in T v) : this(v, v) { }

		public Vector2T (in Vector2T<T> vec) : this(vec.X, vec.Y) { }

		public Vector2T<T> Set (Vector2T<T> vec) {
			X = vec.X;
			Y = vec.Y;
			return this;
		}

		public Vector2T<T> Set (T x, T y) {
			X = x;
			Y = y;
			return this;
		}

		public Vector2T<T> Set (T v) {
			Y = X = v;
			return this;
		}

		public readonly Vector2T<T> Clone () {
			return new Vector2T<T>(X, Y);
		}

		readonly object ICloneable.Clone () {
			return Clone();
		}
	}
}
