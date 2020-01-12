using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;

namespace SpriteMaster.Types {
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members")]
	public sealed class ComparableWeakReference<T> : ISerializable, ILongHash where T : class {
		public const int NullHash = 0;
		public const ulong NullLongHash = LongHash.Null;

		static private class Reflect {
			public static readonly PropertyInfo Target = typeof(WeakReference<T>).GetProperty("Target", BindingFlags.Instance | BindingFlags.NonPublic);
			public static readonly MethodInfo Create = typeof(WeakReference<T>).GetMethod("Create", BindingFlags.Instance | BindingFlags.NonPublic);
			public static readonly MethodInfo IsTrackResurrection = typeof(WeakReference<T>).GetMethod("IsTrackResurrection", BindingFlags.Instance | BindingFlags.NonPublic);

			private static string GetName (string name) {
				return $"{typeof(ComparableWeakReference<T>).Name}.{name}";
			}

			static Reflect () {
				_ = Target ?? throw new NullReferenceException(GetName(nameof(Target)));
				_ = Create ?? throw new NullReferenceException(GetName(nameof(Create)));
				_ = IsTrackResurrection ?? throw new NullReferenceException(GetName(nameof(IsTrackResurrection)));
			}
		}

		private readonly WeakReference<T> _Reference;

		private T Target {
			[SecuritySafeCritical]
			get { return (T)Reflect.Target.GetValue(_Reference); }
			[SecuritySafeCritical]
			set { _Reference.SetTarget(value); }
		}

		public bool IsAlive {
			get { return _Reference.TryGetTarget(out T _); }
		}

		public ComparableWeakReference (T target) : this(new WeakReference<T>(target)) { }

		public ComparableWeakReference (WeakReference<T> reference) {
			T target = null;
			reference.TryGetTarget(out target);
			_Reference = new WeakReference<T>(target);
		}

		public ComparableWeakReference (SerializationInfo info, StreamingContext context) {
			if (info == null) {
				throw new ArgumentNullException(nameof(info));
			}
			var target = (T)info.GetValue("TrackedObject", typeof(T));
			var boolean = info.GetBoolean("TrackResurrection");
			Create(target, boolean);
		}

		public bool TryGetTarget (out T target) {
			return _Reference.TryGetTarget(out target);
		}

		public void SetTarget (T target) {
			_Reference.SetTarget(target);
		}

		public void GetObjectData (SerializationInfo info, StreamingContext context) {
			if (info == null) {
				throw new ArgumentNullException(nameof(info));
			}
			info.AddValue("TrackedObject", Target, typeof(T));
			info.AddValue("TrackResurrection", IsTrackResurrection());
		}

		[SecuritySafeCritical]
		private void Create (T target, bool trackResurrection) {
			Reflect.Create.Invoke(_Reference, new object[] { target, trackResurrection });
		}

		[SecuritySafeCritical]
		private bool IsTrackResurrection () {
			return (bool)Reflect.IsTrackResurrection.Invoke(_Reference, null);
		}

		public override int GetHashCode () {
			if (_Reference.TryGetTarget(out T target)) {
				return target.GetHashCode();
			}
			return NullHash;
		}

		public ulong GetLongHashCode () {
			if (_Reference.TryGetTarget(out T target)) {
				return LongHash.From(target);
			}
			return LongHash.Null;
		}

		public static implicit operator WeakReference<T> (ComparableWeakReference<T> reference) {
			return reference._Reference;
		}

		public static implicit operator ComparableWeakReference<T> (WeakReference<T> reference) {
			return new ComparableWeakReference<T>(reference);
		}

		public bool Equals(ComparableWeakReference<T> obj) {
			return this == obj;
		}

		public bool Equals(WeakReference<T> obj) {
			return this == obj;
		}

		public static bool Equals(WeakReference<T> objA, ComparableWeakReference<T> objB) {
			return objA == objB;
		}

		public static bool Equals (object objA, ComparableWeakReference<T> objB) {
			return objB.Equals(objA);
		}

		public override bool Equals(object obj) {
			switch (obj) {
				case ComparableWeakReference<T> reference:
					return this == reference;
				case WeakReference<T> reference:
					return this == reference;
			}
			return false;
		}

		public static bool operator == (ComparableWeakReference<T> objA, ComparableWeakReference<T> objB) {
			if (objA.TryGetTarget(out T l) && objB.TryGetTarget(out T r)) {
				return l == r;
			}
			return false;
		}

		public static bool operator == (ComparableWeakReference<T> objA, WeakReference<T> objB) {
			if (objA.TryGetTarget(out T l) && objB.TryGetTarget(out T r)) {
				return l == r;
			}
			return false;
		}

		public static bool operator == (WeakReference<T> objA, ComparableWeakReference<T> objB) {
			return objB == objA;
		}

		public static bool operator != (ComparableWeakReference<T> objA, ComparableWeakReference<T> objB) {
			if (objA.TryGetTarget(out T l) && objB.TryGetTarget(out T r)) {
				return l != r;
			}
			return true;
		}

		public static bool operator != (ComparableWeakReference<T> objA, WeakReference<T> objB) {
			if (objA.TryGetTarget(out T l) && objB.TryGetTarget(out T r)) {
				return l != r;
			}
			return true;
		}

		public static bool operator != (WeakReference<T> objA, ComparableWeakReference<T> objB) {
			return objB != objA;
		}
	}
}
