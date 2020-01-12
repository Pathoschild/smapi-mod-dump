using System;

namespace SpriteMaster.Types {
	public struct Vector2B :
		ICloneable,
		IComparable,
		IComparable<Vector2B>,
		IEquatable<Vector2B> {
		public static readonly Vector2B True = new Vector2B(true);
		public static readonly Vector2B False = new Vector2B(false);

		public bool X;
		public bool Y;

		public bool Width {
			readonly get { return X; }
			set { X = value; }
		}

		public bool Height {
			readonly get { return Y; }
			set { Y = value; }
		}

		public bool Negative {
			readonly get { return X; }
			set { X = value; }
		}

		public bool Positive {
			readonly get { return Y; }
			set { Y = value; }
		}

		public bool Any {
			get { return X || Y; }
		}

		public bool All {
			get { return X && Y; }
		}

		public bool this[int index] {
			readonly get {
				return index switch
				{
					0 => X,
					1 => Y,
					_ => throw new IndexOutOfRangeException(nameof(index))
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

		public Vector2B (bool x, bool y) {
			X = x;
			Y = y;
		}

		public Vector2B (bool v) : this(v, v) { }

		public Vector2B (Vector2B vec) : this(vec.X, vec.Y) { }

		public Vector2B Set (Vector2B vec) {
			X = vec.X;
			Y = vec.Y;
			return this;
		}

		public Vector2B Set (bool x, bool y) {
			X = x;
			Y = y;
			return this;
		}

		public Vector2B Set (bool v) {
			Y = X = v;
			return this;
		}

		public readonly Vector2B Clone () {
			return new Vector2B(X, Y);
		}

		readonly object ICloneable.Clone () {
			return Clone();
		}

		public static Vector2B operator & (Vector2B lhs, Vector2B rhs) {
			return new Vector2B(
				lhs.X && rhs.X,
				lhs.Y && rhs.Y
			);
		}

		public static Vector2B operator & (Vector2B lhs, bool rhs) {
			return new Vector2B(
				lhs.X && rhs,
				lhs.Y && rhs
			);
		}

		public static Vector2B operator | (Vector2B lhs, Vector2B rhs) {
			return new Vector2B(
				lhs.X || rhs.X,
				lhs.Y || rhs.Y
			);
		}

		public static Vector2B operator | (Vector2B lhs, bool rhs) {
			return new Vector2B(
				lhs.X || rhs,
				lhs.Y || rhs
			);
		}

		public override readonly string ToString () {
			return $"{{{X}, {Y}}}";
		}

		public readonly int CompareTo (object obj) {
			if (obj is Vector2B other) {
				return CompareTo(other);
			}
			else {
				throw new ArgumentException();
			}
		}

		public readonly int CompareTo (Vector2B other) {
			var xComp = X.CompareTo(other);
			var YComp = Y.CompareTo(other);
			if (xComp != 0)
				return xComp;
			if (YComp != 0)
				return YComp;
			return 0;
		}

		public readonly bool Equals (Vector2B other) {
			return X == other.X && Y == other.Y;
		}
	}
}
