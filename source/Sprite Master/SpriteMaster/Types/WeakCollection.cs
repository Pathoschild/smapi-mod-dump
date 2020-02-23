using SpriteMaster.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Types {
	class WeakCollection<T> : ICollection<T>, IEnumerable<T>, IEnumerable, /*ICollection, */IReadOnlyList<T>, IReadOnlyCollection<T> where T : class {
		static private class Reflect {
			public static readonly PropertyInfo IsReadOnly = typeof(List<ComparableWeakReference<T>>).GetProperty("IsReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private static string GetName (string name) {
				return $"{typeof(WeakCollection<T>).Name}.{name}";
			}

			static Reflect () {
				_ = IsReadOnly ?? throw new NullReferenceException(GetName(nameof(IsReadOnly)));
			}
		}

		private readonly List<ComparableWeakReference<T>> _List;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ComparableWeakReference<T> Weak (T obj) {
			return new ComparableWeakReference<T>(obj);
		}

		public int Count => _List.Count;

		public int Capacity {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get { return _List.Capacity; }
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set { _List.Capacity = value; }
		}

		public bool IsReadOnly => (bool)Reflect.IsReadOnly.GetValue(_List);

		public T this[int index] {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get {
				return _List[index].TryGetTarget(out T target) ? target : null;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set {
				_List[index].SetTarget(value);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WeakCollection () {
			_List = new List<ComparableWeakReference<T>>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WeakCollection(IEnumerable<T> collection) : this() {
			foreach (var obj in collection) {
				_List.Add(obj.MakeWeak());
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WeakCollection (IEnumerable<WeakReference<T>> collection) : this() {
			foreach (var obj in collection) {
				// TODO : Should we check if the object is still alive or not bother?
				_List.Add(obj);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WeakCollection (IEnumerable<ComparableWeakReference<T>> collection) : this() {
			foreach (var obj in collection) {
				// TODO : Should we check if the object is still alive or not bother?
				_List.Add(obj);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WeakCollection (WeakCollection<T> collection) : this(collection.Count) {
			foreach (var obj in collection._List) {
				_List.Add(obj);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WeakCollection (int capacity) {
			_List = new List<ComparableWeakReference<T>>(capacity);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add (T item) {
			_List.Add(item?.MakeWeak());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add (WeakReference<T> item) {
			_List.Add(item);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add (ComparableWeakReference<T> item) {
			_List.Add(item);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AddRange (IEnumerable<T> collection) {
			foreach (T item in collection) {
				_List.Add(item?.MakeWeak());
			}
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AddRange (IEnumerable<WeakReference<T>> collection) {
			foreach (var item in collection) {
				_List.Add(item);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AddRange (IEnumerable<ComparableWeakReference<T>> collection) {
			foreach (var item in collection) {
				_List.Add(item);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public System.Collections.ObjectModel.ReadOnlyCollection<ComparableWeakReference<T>> AsReadOnly () {
			return _List.AsReadOnly();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int BinarySearch (T item) {
			return _List.BinarySearch(Weak(item));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int BinarySearch (WeakReference<T> item) {
			return _List.BinarySearch(item);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int BinarySearch (ComparableWeakReference<T> item) {
			return _List.BinarySearch(item);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int Comparison (ComparableWeakReference<T> x, ComparableWeakReference<T> y, Comparison<T> comparer) {
			bool hasX = x.TryGetTarget(out T xTarget);
			bool hasY = y.TryGetTarget(out T yTarget);

			if (hasX != hasY) {
				return hasX ? int.MaxValue : int.MinValue;
			}
			else if (!hasX) {
				return 0;
			}

			return comparer.Invoke(xTarget, yTarget);
		}

		private sealed class ReferenceComparer : IComparer<ComparableWeakReference<T>> {
			private readonly IComparer<T> Comparer;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public ReferenceComparer(IComparer<T> comparer) {
				Comparer = comparer;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			int IComparer<ComparableWeakReference<T>>.Compare (ComparableWeakReference<T> x, ComparableWeakReference<T> y) {
				return Comparison(x, y, Comparer.Compare);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int BinarySearch (T item, IComparer<T> comparer) {
			return _List.BinarySearch(Weak(item), new ReferenceComparer(comparer));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int BinarySearch (WeakReference<T> item, IComparer<T> comparer) {
			return _List.BinarySearch(item, new ReferenceComparer(comparer));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int BinarySearch (ComparableWeakReference<T> item, IComparer<T> comparer) {
			return _List.BinarySearch(item, new ReferenceComparer(comparer));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear() {
			_List.Clear();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Contains (T item) {
			return _List.Contains(Weak(item));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Contains (WeakReference<T> item) {
			return _List.Contains(item);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Contains (ComparableWeakReference<T> item) {
			return _List.Contains(item);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public List<TOutput> ConvertAll<TOutput> (Converter<T, TOutput> converter) {
			var result = new List<TOutput>(_List.Count);
			foreach (var item in _List) {
				if (item.TryGetTarget(out T target)) {
					result.Add(converter.Invoke(target));
				}
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo (T[] array) {
			CopyTo(array: array, arrayIndex: 0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo (WeakReference<T>[] array) {
			CopyTo(array: array, arrayIndex: 0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo (ComparableWeakReference<T>[] array) {
			CopyTo(array: array, arrayIndex: 0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo(T[] array, int arrayIndex) {
			CopyTo(array: array, arrayIndex: arrayIndex, array.Length - arrayIndex);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo (WeakReference<T>[] array, int arrayIndex) {
			CopyTo(array: array, arrayIndex: arrayIndex, array.Length - arrayIndex);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo (ComparableWeakReference<T>[] array, int arrayIndex) {
			CopyTo(array: array, arrayIndex: arrayIndex, array.Length - arrayIndex);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Check_CopyTo<U>(U[] array, int arrayIndex, int count) where U : class {
			_ = array ?? throw new ArgumentNullException(nameof(array));
			if (arrayIndex < 0)
				throw new ArgumentOutOfRangeException(nameof(arrayIndex));
			if (count < 0)
				throw new ArgumentOutOfRangeException(nameof(count));
			if (_List.Count > count)
				throw new ArgumentException($"The number of elements in the source WeakList<T> ({_List.Count}) is greater than the available space from {nameof(arrayIndex)} ({arrayIndex}) to the end of the destination {nameof(array)} ({array.Length})");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo(T[] array, int arrayIndex, int count) {
			Check_CopyTo(array, arrayIndex, count);
			Purge();

			int dstOffset = arrayIndex;
			int dstLastOffset = dstOffset + count;
			foreach (int i in 0..count) {
				if (_List[i].TryGetTarget(out T target)) {
					array[dstOffset++] = target;
				}
			}
			foreach (int i in dstOffset..dstLastOffset) {
				array[i] = null;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo (WeakReference<T>[] array, int arrayIndex, int count) {
			Check_CopyTo(array, arrayIndex, count);
			Purge();

			int dstOffset = arrayIndex;
			foreach (int i in 0..count) {
				array[dstOffset + i] = _List[i];
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo (ComparableWeakReference<T>[] array, int arrayIndex, int count) {
			Check_CopyTo(array, arrayIndex, count);
			Purge();

			int dstOffset = arrayIndex;
			foreach (int i in 0..count) {
				array[dstOffset + i] = _List[i];
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool Predicate(ComparableWeakReference<T> reference, Predicate<T> predicate) {
			if (reference.TryGetTarget(out T target)) {
				return predicate.Invoke(target);
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Exists (Predicate<T> match) {
			foreach (var item in _List) {
				if (Predicate(item, match)) {
					return true;
				}
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Exists (Predicate<WeakReference<T>> match) {
			foreach (var item in _List) {
				if (match.Invoke(item)) {
					return true;
				}
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Exists (Predicate<ComparableWeakReference<T>> match) {
			foreach (var item in _List) {
				if (match.Invoke(item)) {
					return true;
				}
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T Find (Predicate<T> match) {
			foreach (var item in _List) {
				if (item.TryGetTarget(out T target) && match.Invoke(target)) {
					return target;
				}
			}
			return default;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WeakReference<T> Find (Predicate<WeakReference<T>> match) {
			foreach (var item in _List) {
				if (match.Invoke(item)) {
					return item;
				}
			}
			return default;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ComparableWeakReference<T> Find (Predicate<ComparableWeakReference<T>> match) {
			foreach (var item in _List) {
				if (match.Invoke(item)) {
					return item;
				}
			}
			return default;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public List<T> FindAll (Predicate<T> match) {
			var result = new List<T>();

			foreach (var item in _List) {
				if (item.TryGetTarget(out T target)) {
					if (match.Invoke(target)) {
						result.Add(target);
					}
				}
			}

			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public List<WeakReference<T>> FindAll (Predicate<WeakReference<T>> match) {
			var result = new List<WeakReference<T>>();

			foreach (var item in _List) {
				if (match.Invoke(item)) {
					result.Add(item);
				}
			}

			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public List<ComparableWeakReference<T>> FindAll (Predicate<ComparableWeakReference<T>> match) {
			var result = new List<ComparableWeakReference<T>>();

			foreach (var item in _List) {
				if (match.Invoke(item)) {
					result.Add(item);
				}
			}

			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach (Action<T> action) {
			Purge();

			foreach (var item in _List) {
				if (item.TryGetTarget(out T target)) {
					action.Invoke(target);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach (Action<WeakReference<T>> action) {
			Purge();

			foreach (var item in _List) {
				action.Invoke(item);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach (Action<ComparableWeakReference<T>> action) {
			Purge();

			foreach (var item in _List) {
				action.Invoke(item);
			}
		}

		// TODO : must implement GetEnumerator
		// class? Enumerator
		// public Enumerator GetEnumerator ()

		// GetRange
		// IndexOf
		// Insert
		// InsertRange
		// LastIndexof
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Remove (T item) {
			return _List.Remove(Weak(item));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Remove (WeakReference<T> item) {
			return _List.Remove(item);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Remove (ComparableWeakReference<T> item) {
			return _List.Remove(item);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int RemoveAll (Predicate<T> match) {
			return _List.RemoveAll((ComparableWeakReference<T> reference) => {
				if (reference.TryGetTarget(out T target)) {
					return match.Invoke(target);
				}
				return false;
			});
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int RemoveAll (Predicate<WeakReference<T>> match) {
			return _List.RemoveAll((ComparableWeakReference<T> reference) => {
				return match.Invoke(reference);
			});
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int RemoveAll (Predicate<ComparableWeakReference<T>> match) {
			return _List.RemoveAll(match);
		}

		// RemoveAt
		// RemoveRange

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int Purge() {
			return _List.RemoveAll(
				(ComparableWeakReference<T> reference) => !reference.IsAlive
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Reverse() {
			Purge();

			foreach (int i in 0.._List.Count) {
				var swapIndex = _List.Count - (i + 1);
				var temp = _List[i];
				_List[i] = _List[swapIndex];
				_List[swapIndex] = temp;
			}
		}

		// Reverse(int, int)

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Sort(Comparison<T> comparison) {
			Purge();

			_List.Sort((ComparableWeakReference<T> referenceA, ComparableWeakReference<T> referenceB) => Comparison(referenceA, referenceB, comparison));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Sort (Comparer<T> comparer) {
			Purge();

			_List.Sort(new ReferenceComparer(comparer));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Sort() {
			Purge();

			_List.Sort(
				(ComparableWeakReference<T> referenceA, ComparableWeakReference<T> referenceB) => Comparison(referenceA, referenceB, (T a, T b) => Comparer<T>.Default.Compare(a, b))
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T[] ToArray() {
			Purge();

			var result = new T[_List.Count];
			int listSize = 0;
			
			foreach (var item in _List) {
				if (item.TryGetTarget(out T target)) {
					result[listSize++] = target;
				}
			}

			Array.Resize(ref result, listSize);

			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void TrimExcess () {
			Purge();
			_List.TrimExcess();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TrueForAll (Predicate<T> match) {
			foreach (var item in _List) {
				if (!Predicate(item, match)) {
					return false;
				}
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TrueForAll (Predicate<WeakReference<T>> match) {
			foreach (var item in _List) {
				if (!match.Invoke(item)) {
					return false;
				}
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TrueForAll (Predicate<ComparableWeakReference<T>> match) {
			foreach (var item in _List) {
				if (!match.Invoke(item)) {
					return false;
				}
			}
			return true;
		}
		public sealed class Enumerator : IEnumerator<T> {
			private readonly IEnumerator<ComparableWeakReference<T>> _Enumerator;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Enumerator (IEnumerator<ComparableWeakReference<T>> enumerator) {
				_Enumerator = enumerator;
			}

			public T Current => GetCurrent();

			object IEnumerator.Current => GetCurrent();

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private T GetCurrent () {
				T target = null;
				while (!_Enumerator.Current.TryGetTarget(out target)) {
					if (!_Enumerator.MoveNext()) {
						return null; // TODO
					}
				}
				return target;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Dispose () {
				_Enumerator.Dispose();
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext () {
				do {
					if (!_Enumerator.MoveNext()) {
						return false;
					}
				} while (!_Enumerator.Current.IsAlive);
				return true;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Reset () {
				_Enumerator.Reset();
				while (!_Enumerator.Current.IsAlive && _Enumerator.MoveNext()) {}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IEnumerator<T> GetEnumerator () {
			return new Enumerator(_List.GetEnumerator());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		IEnumerator IEnumerable.GetEnumerator () {
			return new Enumerator(_List.GetEnumerator());
		}
	}
}
