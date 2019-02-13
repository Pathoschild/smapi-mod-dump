using System.Collections;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace System {
	public readonly partial struct ValueTuple {

		public static ValueTuple<T1> Create<T1>(T1 item1) {
			return new ValueTuple<T1>(item1);
		}

		public static ValueTuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2) {
			return new ValueTuple<T1, T2>(item1, item2);
		}

		public static ValueTuple<T1, T2, T3> Create<T1, T2, T3>(T1 item1, T2 item2, T3 item3) {
			return new ValueTuple<T1, T2, T3>(item1, item2, item3);
		}

		public static ValueTuple<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 item1, T2 item2, T3 item3, T4 item4) {
			return new ValueTuple<T1, T2, T3, T4>(item1, item2, item3, item4);
		}

		public static ValueTuple<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5) {
			return new ValueTuple<T1, T2, T3, T4, T5>(item1, item2, item3, item4, item5);
		}

		public static ValueTuple<T1, T2, T3, T4, T5, T6> Create<T1, T2, T3, T4, T5, T6>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6) {
			return new ValueTuple<T1, T2, T3, T4, T5, T6>(item1, item2, item3, item4, item5, item6);
		}

		public static ValueTuple<T1, T2, T3, T4, T5, T6, T7> Create<T1, T2, T3, T4, T5, T6, T7>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7) {
			return new ValueTuple<T1, T2, T3, T4, T5, T6, T7>(item1, item2, item3, item4, item5, item6, item7);
		}

		public static ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8>> Create<T1, T2, T3, T4, T5, T6, T7, T8>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8) {
			return new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8>>(item1, item2, item3, item4, item5, item6, item7, new ValueTuple<T8>(item8));
		}
	}

	[Serializable]
	public readonly struct ValueTuple<T1> : IEquatable<ValueTuple<T1>>, IStructuralEquatable, IStructuralComparable, IComparable, IComparable<ValueTuple<T1>>
	{
		public readonly T1 Item1;

		public ValueTuple(T1 item1) {
			this.Item1 = item1;
		}

        public override bool Equals(object obj) {
            return obj is ValueTuple<T1> other && this.Equals(other);
        }

		public bool Equals(ValueTuple<T1> other) {
            return EqualityComparer<T1>.Default.Equals(this.Item1, other.Item1);
        }

		public bool Equals(object other, IEqualityComparer comparer) {
            if (!(other is ValueTuple<T1> otherTuple)) {
				return false;
			}

			return comparer.Equals(this.Item1, otherTuple.Item1);
        }

        public int CompareTo(object obj) {
			return obj is ValueTuple<T1> other ? this.CompareTo(other) : throw new ArgumentException();
		}

        public int CompareTo(ValueTuple<T1> other) {
			return Comparer<T1>.Default.Compare(this.Item1, other.Item1);		
        }

		public int CompareTo(object obj, IComparer comparer) {
			if (!(obj is ValueTuple<T1> other)) {
				throw new ArgumentException();
			}

			return comparer.Compare(this.Item1, other.Item1);		
        }

		public override int GetHashCode() {
			return ValueTuple.CombineHashes(this.Item1?.GetHashCode() ?? 0);
		}

        public int GetHashCode(IEqualityComparer comparer) {
            return ValueTuple.CombineHashes(comparer.GetHashCode(this.Item1));
        }
	}

	[Serializable]
	public readonly struct ValueTuple<T1, T2> : IEquatable<ValueTuple<T1, T2>>, IStructuralEquatable, IStructuralComparable, IComparable, IComparable<ValueTuple<T1, T2>>
	{
		public readonly T1 Item1;
		public readonly T2 Item2;

		public ValueTuple(T1 item1, T2 item2) {
			this.Item1 = item1;
			this.Item2 = item2;
		}

        public override bool Equals(object obj) {
            return obj is ValueTuple<T1, T2> other && this.Equals(other);
        }

		public bool Equals(ValueTuple<T1, T2> other) {
            return EqualityComparer<T1>.Default.Equals(this.Item1, other.Item1)
                && EqualityComparer<T2>.Default.Equals(this.Item2, other.Item2);
        }

		public bool Equals(object other, IEqualityComparer comparer) {
            if (!(other is ValueTuple<T1, T2> otherTuple)) {
				return false;
			}

			return comparer.Equals(this.Item1, otherTuple.Item1)
                && comparer.Equals(this.Item2, otherTuple.Item2);
        }

        public int CompareTo(object obj) {
			return obj is ValueTuple<T1, T2> other ? this.CompareTo(other) : throw new ArgumentException();
		}

        public int CompareTo(ValueTuple<T1, T2> other) {
			int result = Comparer<T1>.Default.Compare(this.Item1, other.Item1);
			if (result != 0) {
				return result;
			}

			return Comparer<T2>.Default.Compare(this.Item2, other.Item2);		
        }

		public int CompareTo(object obj, IComparer comparer) {
			if (!(obj is ValueTuple<T1, T2> other)) {
				throw new ArgumentException();
			}

			int result = comparer.Compare(this.Item1, other.Item1);
			if (result != 0) {
				return result;
			}

			return comparer.Compare(this.Item2, other.Item2);		
        }

		public override int GetHashCode() {
			return ValueTuple.CombineHashes(this.Item1?.GetHashCode() ?? 0, this.Item2?.GetHashCode() ?? 0);
		}

        public int GetHashCode(IEqualityComparer comparer) {
            return ValueTuple.CombineHashes(comparer.GetHashCode(this.Item1), comparer.GetHashCode(this.Item2));
        }
	}

	[Serializable]
	public readonly struct ValueTuple<T1, T2, T3> : IEquatable<ValueTuple<T1, T2, T3>>, IStructuralEquatable, IStructuralComparable, IComparable, IComparable<ValueTuple<T1, T2, T3>>
	{
		public readonly T1 Item1;
		public readonly T2 Item2;
		public readonly T3 Item3;

		public ValueTuple(T1 item1, T2 item2, T3 item3) {
			this.Item1 = item1;
			this.Item2 = item2;
			this.Item3 = item3;
		}

        public override bool Equals(object obj) {
            return obj is ValueTuple<T1, T2, T3> other && this.Equals(other);
        }

		public bool Equals(ValueTuple<T1, T2, T3> other) {
            return EqualityComparer<T1>.Default.Equals(this.Item1, other.Item1)
                && EqualityComparer<T2>.Default.Equals(this.Item2, other.Item2)
                && EqualityComparer<T3>.Default.Equals(this.Item3, other.Item3);
        }

		public bool Equals(object other, IEqualityComparer comparer) {
            if (!(other is ValueTuple<T1, T2, T3> otherTuple)) {
				return false;
			}

			return comparer.Equals(this.Item1, otherTuple.Item1)
                && comparer.Equals(this.Item2, otherTuple.Item2)
                && comparer.Equals(this.Item3, otherTuple.Item3);
        }

        public int CompareTo(object obj) {
			return obj is ValueTuple<T1, T2, T3> other ? this.CompareTo(other) : throw new ArgumentException();
		}

        public int CompareTo(ValueTuple<T1, T2, T3> other) {
			int result = Comparer<T1>.Default.Compare(this.Item1, other.Item1);
			if (result != 0) {
				return result;
			}

			result = Comparer<T2>.Default.Compare(this.Item2, other.Item2);
			if (result != 0) {
				return result;
			}

			return Comparer<T3>.Default.Compare(this.Item3, other.Item3);		
        }

		public int CompareTo(object obj, IComparer comparer) {
			if (!(obj is ValueTuple<T1, T2, T3> other)) {
				throw new ArgumentException();
			}

			int result = comparer.Compare(this.Item1, other.Item1);
			if (result != 0) {
				return result;
			}

			result = comparer.Compare(this.Item2, other.Item2);
			if (result != 0) {
				return result;
			}

			return comparer.Compare(this.Item3, other.Item3);		
        }

		public override int GetHashCode() {
			return ValueTuple.CombineHashes(this.Item1?.GetHashCode() ?? 0, this.Item2?.GetHashCode() ?? 0, this.Item3?.GetHashCode() ?? 0);
		}

        public int GetHashCode(IEqualityComparer comparer) {
            return ValueTuple.CombineHashes(comparer.GetHashCode(this.Item1), comparer.GetHashCode(this.Item2), comparer.GetHashCode(this.Item3));
        }
	}

	[Serializable]
	public readonly struct ValueTuple<T1, T2, T3, T4> : IEquatable<ValueTuple<T1, T2, T3, T4>>, IStructuralEquatable, IStructuralComparable, IComparable, IComparable<ValueTuple<T1, T2, T3, T4>>
	{
		public readonly T1 Item1;
		public readonly T2 Item2;
		public readonly T3 Item3;
		public readonly T4 Item4;

		public ValueTuple(T1 item1, T2 item2, T3 item3, T4 item4) {
			this.Item1 = item1;
			this.Item2 = item2;
			this.Item3 = item3;
			this.Item4 = item4;
		}

        public override bool Equals(object obj) {
            return obj is ValueTuple<T1, T2, T3, T4> other && this.Equals(other);
        }

		public bool Equals(ValueTuple<T1, T2, T3, T4> other) {
            return EqualityComparer<T1>.Default.Equals(this.Item1, other.Item1)
                && EqualityComparer<T2>.Default.Equals(this.Item2, other.Item2)
                && EqualityComparer<T3>.Default.Equals(this.Item3, other.Item3)
                && EqualityComparer<T4>.Default.Equals(this.Item4, other.Item4);
        }

		public bool Equals(object other, IEqualityComparer comparer) {
            if (!(other is ValueTuple<T1, T2, T3, T4> otherTuple)) {
				return false;
			}

			return comparer.Equals(this.Item1, otherTuple.Item1)
                && comparer.Equals(this.Item2, otherTuple.Item2)
                && comparer.Equals(this.Item3, otherTuple.Item3)
                && comparer.Equals(this.Item4, otherTuple.Item4);
        }

        public int CompareTo(object obj) {
			return obj is ValueTuple<T1, T2, T3, T4> other ? this.CompareTo(other) : throw new ArgumentException();
		}

        public int CompareTo(ValueTuple<T1, T2, T3, T4> other) {
			int result = Comparer<T1>.Default.Compare(this.Item1, other.Item1);
			if (result != 0) {
				return result;
			}

			result = Comparer<T2>.Default.Compare(this.Item2, other.Item2);
			if (result != 0) {
				return result;
			}

			result = Comparer<T3>.Default.Compare(this.Item3, other.Item3);
			if (result != 0) {
				return result;
			}

			return Comparer<T4>.Default.Compare(this.Item4, other.Item4);		
        }

		public int CompareTo(object obj, IComparer comparer) {
			if (!(obj is ValueTuple<T1, T2, T3, T4> other)) {
				throw new ArgumentException();
			}

			int result = comparer.Compare(this.Item1, other.Item1);
			if (result != 0) {
				return result;
			}

			result = comparer.Compare(this.Item2, other.Item2);
			if (result != 0) {
				return result;
			}

			result = comparer.Compare(this.Item3, other.Item3);
			if (result != 0) {
				return result;
			}

			return comparer.Compare(this.Item4, other.Item4);		
        }

		public override int GetHashCode() {
			return ValueTuple.CombineHashes(this.Item1?.GetHashCode() ?? 0, this.Item2?.GetHashCode() ?? 0, this.Item3?.GetHashCode() ?? 0, this.Item4?.GetHashCode() ?? 0);
		}

        public int GetHashCode(IEqualityComparer comparer) {
            return ValueTuple.CombineHashes(comparer.GetHashCode(this.Item1), comparer.GetHashCode(this.Item2), comparer.GetHashCode(this.Item3), comparer.GetHashCode(this.Item4));
        }
	}

	[Serializable]
	public readonly struct ValueTuple<T1, T2, T3, T4, T5> : IEquatable<ValueTuple<T1, T2, T3, T4, T5>>, IStructuralEquatable, IStructuralComparable, IComparable, IComparable<ValueTuple<T1, T2, T3, T4, T5>>
	{
		public readonly T1 Item1;
		public readonly T2 Item2;
		public readonly T3 Item3;
		public readonly T4 Item4;
		public readonly T5 Item5;

		public ValueTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5) {
			this.Item1 = item1;
			this.Item2 = item2;
			this.Item3 = item3;
			this.Item4 = item4;
			this.Item5 = item5;
		}

        public override bool Equals(object obj) {
            return obj is ValueTuple<T1, T2, T3, T4, T5> other && this.Equals(other);
        }

		public bool Equals(ValueTuple<T1, T2, T3, T4, T5> other) {
            return EqualityComparer<T1>.Default.Equals(this.Item1, other.Item1)
                && EqualityComparer<T2>.Default.Equals(this.Item2, other.Item2)
                && EqualityComparer<T3>.Default.Equals(this.Item3, other.Item3)
                && EqualityComparer<T4>.Default.Equals(this.Item4, other.Item4)
                && EqualityComparer<T5>.Default.Equals(this.Item5, other.Item5);
        }

		public bool Equals(object other, IEqualityComparer comparer) {
            if (!(other is ValueTuple<T1, T2, T3, T4, T5> otherTuple)) {
				return false;
			}

			return comparer.Equals(this.Item1, otherTuple.Item1)
                && comparer.Equals(this.Item2, otherTuple.Item2)
                && comparer.Equals(this.Item3, otherTuple.Item3)
                && comparer.Equals(this.Item4, otherTuple.Item4)
                && comparer.Equals(this.Item5, otherTuple.Item5);
        }

        public int CompareTo(object obj) {
			return obj is ValueTuple<T1, T2, T3, T4, T5> other ? this.CompareTo(other) : throw new ArgumentException();
		}

        public int CompareTo(ValueTuple<T1, T2, T3, T4, T5> other) {
			int result = Comparer<T1>.Default.Compare(this.Item1, other.Item1);
			if (result != 0) {
				return result;
			}

			result = Comparer<T2>.Default.Compare(this.Item2, other.Item2);
			if (result != 0) {
				return result;
			}

			result = Comparer<T3>.Default.Compare(this.Item3, other.Item3);
			if (result != 0) {
				return result;
			}

			result = Comparer<T4>.Default.Compare(this.Item4, other.Item4);
			if (result != 0) {
				return result;
			}

			return Comparer<T5>.Default.Compare(this.Item5, other.Item5);		
        }

		public int CompareTo(object obj, IComparer comparer) {
			if (!(obj is ValueTuple<T1, T2, T3, T4, T5> other)) {
				throw new ArgumentException();
			}

			int result = comparer.Compare(this.Item1, other.Item1);
			if (result != 0) {
				return result;
			}

			result = comparer.Compare(this.Item2, other.Item2);
			if (result != 0) {
				return result;
			}

			result = comparer.Compare(this.Item3, other.Item3);
			if (result != 0) {
				return result;
			}

			result = comparer.Compare(this.Item4, other.Item4);
			if (result != 0) {
				return result;
			}

			return comparer.Compare(this.Item5, other.Item5);		
        }

		public override int GetHashCode() {
			return ValueTuple.CombineHashes(this.Item1?.GetHashCode() ?? 0, this.Item2?.GetHashCode() ?? 0, this.Item3?.GetHashCode() ?? 0, this.Item4?.GetHashCode() ?? 0, this.Item5?.GetHashCode() ?? 0);
		}

        public int GetHashCode(IEqualityComparer comparer) {
            return ValueTuple.CombineHashes(comparer.GetHashCode(this.Item1), comparer.GetHashCode(this.Item2), comparer.GetHashCode(this.Item3), comparer.GetHashCode(this.Item4), comparer.GetHashCode(this.Item5));
        }
	}

	[Serializable]
	public readonly struct ValueTuple<T1, T2, T3, T4, T5, T6> : IEquatable<ValueTuple<T1, T2, T3, T4, T5, T6>>, IStructuralEquatable, IStructuralComparable, IComparable, IComparable<ValueTuple<T1, T2, T3, T4, T5, T6>>
	{
		public readonly T1 Item1;
		public readonly T2 Item2;
		public readonly T3 Item3;
		public readonly T4 Item4;
		public readonly T5 Item5;
		public readonly T6 Item6;

		public ValueTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6) {
			this.Item1 = item1;
			this.Item2 = item2;
			this.Item3 = item3;
			this.Item4 = item4;
			this.Item5 = item5;
			this.Item6 = item6;
		}

        public override bool Equals(object obj) {
            return obj is ValueTuple<T1, T2, T3, T4, T5, T6> other && this.Equals(other);
        }

		public bool Equals(ValueTuple<T1, T2, T3, T4, T5, T6> other) {
            return EqualityComparer<T1>.Default.Equals(this.Item1, other.Item1)
                && EqualityComparer<T2>.Default.Equals(this.Item2, other.Item2)
                && EqualityComparer<T3>.Default.Equals(this.Item3, other.Item3)
                && EqualityComparer<T4>.Default.Equals(this.Item4, other.Item4)
                && EqualityComparer<T5>.Default.Equals(this.Item5, other.Item5)
                && EqualityComparer<T6>.Default.Equals(this.Item6, other.Item6);
        }

		public bool Equals(object other, IEqualityComparer comparer) {
            if (!(other is ValueTuple<T1, T2, T3, T4, T5, T6> otherTuple)) {
				return false;
			}

			return comparer.Equals(this.Item1, otherTuple.Item1)
                && comparer.Equals(this.Item2, otherTuple.Item2)
                && comparer.Equals(this.Item3, otherTuple.Item3)
                && comparer.Equals(this.Item4, otherTuple.Item4)
                && comparer.Equals(this.Item5, otherTuple.Item5)
                && comparer.Equals(this.Item6, otherTuple.Item6);
        }

        public int CompareTo(object obj) {
			return obj is ValueTuple<T1, T2, T3, T4, T5, T6> other ? this.CompareTo(other) : throw new ArgumentException();
		}

        public int CompareTo(ValueTuple<T1, T2, T3, T4, T5, T6> other) {
			int result = Comparer<T1>.Default.Compare(this.Item1, other.Item1);
			if (result != 0) {
				return result;
			}

			result = Comparer<T2>.Default.Compare(this.Item2, other.Item2);
			if (result != 0) {
				return result;
			}

			result = Comparer<T3>.Default.Compare(this.Item3, other.Item3);
			if (result != 0) {
				return result;
			}

			result = Comparer<T4>.Default.Compare(this.Item4, other.Item4);
			if (result != 0) {
				return result;
			}

			result = Comparer<T5>.Default.Compare(this.Item5, other.Item5);
			if (result != 0) {
				return result;
			}

			return Comparer<T6>.Default.Compare(this.Item6, other.Item6);		
        }

		public int CompareTo(object obj, IComparer comparer) {
			if (!(obj is ValueTuple<T1, T2, T3, T4, T5, T6> other)) {
				throw new ArgumentException();
			}

			int result = comparer.Compare(this.Item1, other.Item1);
			if (result != 0) {
				return result;
			}

			result = comparer.Compare(this.Item2, other.Item2);
			if (result != 0) {
				return result;
			}

			result = comparer.Compare(this.Item3, other.Item3);
			if (result != 0) {
				return result;
			}

			result = comparer.Compare(this.Item4, other.Item4);
			if (result != 0) {
				return result;
			}

			result = comparer.Compare(this.Item5, other.Item5);
			if (result != 0) {
				return result;
			}

			return comparer.Compare(this.Item6, other.Item6);		
        }

		public override int GetHashCode() {
			return ValueTuple.CombineHashes(this.Item1?.GetHashCode() ?? 0, this.Item2?.GetHashCode() ?? 0, this.Item3?.GetHashCode() ?? 0, this.Item4?.GetHashCode() ?? 0, this.Item5?.GetHashCode() ?? 0, this.Item6?.GetHashCode() ?? 0);
		}

        public int GetHashCode(IEqualityComparer comparer) {
            return ValueTuple.CombineHashes(comparer.GetHashCode(this.Item1), comparer.GetHashCode(this.Item2), comparer.GetHashCode(this.Item3), comparer.GetHashCode(this.Item4), comparer.GetHashCode(this.Item5), comparer.GetHashCode(this.Item6));
        }
	}

	[Serializable]
	public readonly struct ValueTuple<T1, T2, T3, T4, T5, T6, T7> : IEquatable<ValueTuple<T1, T2, T3, T4, T5, T6, T7>>, IStructuralEquatable, IStructuralComparable, IComparable, IComparable<ValueTuple<T1, T2, T3, T4, T5, T6, T7>>
	{
		public readonly T1 Item1;
		public readonly T2 Item2;
		public readonly T3 Item3;
		public readonly T4 Item4;
		public readonly T5 Item5;
		public readonly T6 Item6;
		public readonly T7 Item7;

		public ValueTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7) {
			this.Item1 = item1;
			this.Item2 = item2;
			this.Item3 = item3;
			this.Item4 = item4;
			this.Item5 = item5;
			this.Item6 = item6;
			this.Item7 = item7;
		}

        public override bool Equals(object obj) {
            return obj is ValueTuple<T1, T2, T3, T4, T5, T6, T7> other && this.Equals(other);
        }

		public bool Equals(ValueTuple<T1, T2, T3, T4, T5, T6, T7> other) {
            return EqualityComparer<T1>.Default.Equals(this.Item1, other.Item1)
                && EqualityComparer<T2>.Default.Equals(this.Item2, other.Item2)
                && EqualityComparer<T3>.Default.Equals(this.Item3, other.Item3)
                && EqualityComparer<T4>.Default.Equals(this.Item4, other.Item4)
                && EqualityComparer<T5>.Default.Equals(this.Item5, other.Item5)
                && EqualityComparer<T6>.Default.Equals(this.Item6, other.Item6)
                && EqualityComparer<T7>.Default.Equals(this.Item7, other.Item7);
        }

		public bool Equals(object other, IEqualityComparer comparer) {
            if (!(other is ValueTuple<T1, T2, T3, T4, T5, T6, T7> otherTuple)) {
				return false;
			}

			return comparer.Equals(this.Item1, otherTuple.Item1)
                && comparer.Equals(this.Item2, otherTuple.Item2)
                && comparer.Equals(this.Item3, otherTuple.Item3)
                && comparer.Equals(this.Item4, otherTuple.Item4)
                && comparer.Equals(this.Item5, otherTuple.Item5)
                && comparer.Equals(this.Item6, otherTuple.Item6)
                && comparer.Equals(this.Item7, otherTuple.Item7);
        }

        public int CompareTo(object obj) {
			return obj is ValueTuple<T1, T2, T3, T4, T5, T6, T7> other ? this.CompareTo(other) : throw new ArgumentException();
		}

        public int CompareTo(ValueTuple<T1, T2, T3, T4, T5, T6, T7> other) {
			int result = Comparer<T1>.Default.Compare(this.Item1, other.Item1);
			if (result != 0) {
				return result;
			}

			result = Comparer<T2>.Default.Compare(this.Item2, other.Item2);
			if (result != 0) {
				return result;
			}

			result = Comparer<T3>.Default.Compare(this.Item3, other.Item3);
			if (result != 0) {
				return result;
			}

			result = Comparer<T4>.Default.Compare(this.Item4, other.Item4);
			if (result != 0) {
				return result;
			}

			result = Comparer<T5>.Default.Compare(this.Item5, other.Item5);
			if (result != 0) {
				return result;
			}

			result = Comparer<T6>.Default.Compare(this.Item6, other.Item6);
			if (result != 0) {
				return result;
			}

			return Comparer<T7>.Default.Compare(this.Item7, other.Item7);		
        }

		public int CompareTo(object obj, IComparer comparer) {
			if (!(obj is ValueTuple<T1, T2, T3, T4, T5, T6, T7> other)) {
				throw new ArgumentException();
			}

			int result = comparer.Compare(this.Item1, other.Item1);
			if (result != 0) {
				return result;
			}

			result = comparer.Compare(this.Item2, other.Item2);
			if (result != 0) {
				return result;
			}

			result = comparer.Compare(this.Item3, other.Item3);
			if (result != 0) {
				return result;
			}

			result = comparer.Compare(this.Item4, other.Item4);
			if (result != 0) {
				return result;
			}

			result = comparer.Compare(this.Item5, other.Item5);
			if (result != 0) {
				return result;
			}

			result = comparer.Compare(this.Item6, other.Item6);
			if (result != 0) {
				return result;
			}

			return comparer.Compare(this.Item7, other.Item7);		
        }

		public override int GetHashCode() {
			return ValueTuple.CombineHashes(this.Item1?.GetHashCode() ?? 0, this.Item2?.GetHashCode() ?? 0, this.Item3?.GetHashCode() ?? 0, this.Item4?.GetHashCode() ?? 0, this.Item5?.GetHashCode() ?? 0, this.Item6?.GetHashCode() ?? 0, this.Item7?.GetHashCode() ?? 0);
		}

        public int GetHashCode(IEqualityComparer comparer) {
            return ValueTuple.CombineHashes(comparer.GetHashCode(this.Item1), comparer.GetHashCode(this.Item2), comparer.GetHashCode(this.Item3), comparer.GetHashCode(this.Item4), comparer.GetHashCode(this.Item5), comparer.GetHashCode(this.Item6), comparer.GetHashCode(this.Item7));
        }
	}

	[Serializable]
	public readonly struct ValueTuple<T1, T2, T3, T4, T5, T6, T7, T8> : IEquatable<ValueTuple<T1, T2, T3, T4, T5, T6, T7, T8>>, IStructuralEquatable, IStructuralComparable, IComparable, IComparable<ValueTuple<T1, T2, T3, T4, T5, T6, T7, T8>>
	{
		public readonly T1 Item1;
		public readonly T2 Item2;
		public readonly T3 Item3;
		public readonly T4 Item4;
		public readonly T5 Item5;
		public readonly T6 Item6;
		public readonly T7 Item7;
		public readonly T8 Item8;

		public ValueTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8) {
			this.Item1 = item1;
			this.Item2 = item2;
			this.Item3 = item3;
			this.Item4 = item4;
			this.Item5 = item5;
			this.Item6 = item6;
			this.Item7 = item7;
			this.Item8 = item8;
		}

        public override bool Equals(object obj) {
            return obj is ValueTuple<T1, T2, T3, T4, T5, T6, T7, T8> other && this.Equals(other);
        }

		public bool Equals(ValueTuple<T1, T2, T3, T4, T5, T6, T7, T8> other) {
            return EqualityComparer<T1>.Default.Equals(this.Item1, other.Item1)
                && EqualityComparer<T2>.Default.Equals(this.Item2, other.Item2)
                && EqualityComparer<T3>.Default.Equals(this.Item3, other.Item3)
                && EqualityComparer<T4>.Default.Equals(this.Item4, other.Item4)
                && EqualityComparer<T5>.Default.Equals(this.Item5, other.Item5)
                && EqualityComparer<T6>.Default.Equals(this.Item6, other.Item6)
                && EqualityComparer<T7>.Default.Equals(this.Item7, other.Item7)
                && EqualityComparer<T8>.Default.Equals(this.Item8, other.Item8);
        }

		public bool Equals(object other, IEqualityComparer comparer) {
            if (!(other is ValueTuple<T1, T2, T3, T4, T5, T6, T7, T8> otherTuple)) {
				return false;
			}

			return comparer.Equals(this.Item1, otherTuple.Item1)
                && comparer.Equals(this.Item2, otherTuple.Item2)
                && comparer.Equals(this.Item3, otherTuple.Item3)
                && comparer.Equals(this.Item4, otherTuple.Item4)
                && comparer.Equals(this.Item5, otherTuple.Item5)
                && comparer.Equals(this.Item6, otherTuple.Item6)
                && comparer.Equals(this.Item7, otherTuple.Item7)
                && comparer.Equals(this.Item8, otherTuple.Item8);
        }

        public int CompareTo(object obj) {
			return obj is ValueTuple<T1, T2, T3, T4, T5, T6, T7, T8> other ? this.CompareTo(other) : throw new ArgumentException();
		}

        public int CompareTo(ValueTuple<T1, T2, T3, T4, T5, T6, T7, T8> other) {
			int result = Comparer<T1>.Default.Compare(this.Item1, other.Item1);
			if (result != 0) {
				return result;
			}

			result = Comparer<T2>.Default.Compare(this.Item2, other.Item2);
			if (result != 0) {
				return result;
			}

			result = Comparer<T3>.Default.Compare(this.Item3, other.Item3);
			if (result != 0) {
				return result;
			}

			result = Comparer<T4>.Default.Compare(this.Item4, other.Item4);
			if (result != 0) {
				return result;
			}

			result = Comparer<T5>.Default.Compare(this.Item5, other.Item5);
			if (result != 0) {
				return result;
			}

			result = Comparer<T6>.Default.Compare(this.Item6, other.Item6);
			if (result != 0) {
				return result;
			}

			result = Comparer<T7>.Default.Compare(this.Item7, other.Item7);
			if (result != 0) {
				return result;
			}

			return Comparer<T8>.Default.Compare(this.Item8, other.Item8);		
        }

		public int CompareTo(object obj, IComparer comparer) {
			if (!(obj is ValueTuple<T1, T2, T3, T4, T5, T6, T7, T8> other)) {
				throw new ArgumentException();
			}

			int result = comparer.Compare(this.Item1, other.Item1);
			if (result != 0) {
				return result;
			}

			result = comparer.Compare(this.Item2, other.Item2);
			if (result != 0) {
				return result;
			}

			result = comparer.Compare(this.Item3, other.Item3);
			if (result != 0) {
				return result;
			}

			result = comparer.Compare(this.Item4, other.Item4);
			if (result != 0) {
				return result;
			}

			result = comparer.Compare(this.Item5, other.Item5);
			if (result != 0) {
				return result;
			}

			result = comparer.Compare(this.Item6, other.Item6);
			if (result != 0) {
				return result;
			}

			result = comparer.Compare(this.Item7, other.Item7);
			if (result != 0) {
				return result;
			}

			return comparer.Compare(this.Item8, other.Item8);		
        }

		public override int GetHashCode() {
			return ValueTuple.CombineHashes(this.Item1?.GetHashCode() ?? 0, this.Item2?.GetHashCode() ?? 0, this.Item3?.GetHashCode() ?? 0, this.Item4?.GetHashCode() ?? 0, this.Item5?.GetHashCode() ?? 0, this.Item6?.GetHashCode() ?? 0, this.Item7?.GetHashCode() ?? 0, this.Item8?.GetHashCode() ?? 0);
		}

        public int GetHashCode(IEqualityComparer comparer) {
            return ValueTuple.CombineHashes(comparer.GetHashCode(this.Item1), comparer.GetHashCode(this.Item2), comparer.GetHashCode(this.Item3), comparer.GetHashCode(this.Item4), comparer.GetHashCode(this.Item5), comparer.GetHashCode(this.Item6), comparer.GetHashCode(this.Item7), comparer.GetHashCode(this.Item8));
        }
	}
}
