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
using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;

//

namespace SpriteMaster.Types;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members")]
internal sealed class ComparableWeakReference<T> :
	ISerializable,
	ILongHash,
	IEquatable<WeakReference<T>>,
	IEquatable<ComparableWeakReference<T>>
where T : class? {
	private static readonly Type WeakReferenceType = typeof(WeakReference<T>);

	internal const int NullHash = 0;
	internal const ulong NullLongHash = LongHash.Null;

	private static class Reflect {
		private const BindingFlags InstanceFlags = BindingFlags.Instance | BindingFlags.NonPublic;

		internal static readonly Action<WeakReference<T>, T> SetTarget =
			WeakReferenceType.GetPropertySetter<WeakReference<T>, T>(WeakReferenceType.GetProperty("Target", InstanceFlags)) ??
			throw new NullReferenceException(GetName(nameof(SetTarget)));
		internal static readonly Func<WeakReference<T>, T> GetTarget =
			WeakReferenceType.GetPropertyGetter<WeakReference<T>, T>(WeakReferenceType.GetProperty("Target", InstanceFlags)) ??
			throw new NullReferenceException(GetName(nameof(GetTarget)));

		internal static readonly Action<WeakReference<T>, T, bool> Create = typeof(WeakReference<T>).
			GetMethod("Create", InstanceFlags)?.CreateDelegate<Action<WeakReference<T>, T, bool>>() ??
			throw new NullReferenceException(GetName(nameof(Create)));

		internal static readonly Func<WeakReference<T>, bool> IsTrackResurrection = typeof(WeakReference<T>).
			GetMethod("IsTrackResurrection", InstanceFlags)?.CreateDelegate<Func<WeakReference<T>, bool>>() ??
			throw new NullReferenceException(GetName(nameof(IsTrackResurrection)));

		private static string GetName(string name) => $"{typeof(ComparableWeakReference<T>).Name}.{name}";
	}

	private readonly WeakReference<T> Reference = new(null!);

	private T Target {
		[SecuritySafeCritical]
		get => Reflect.GetTarget(Reference);
		[SecuritySafeCritical]
		set => Reference.SetTarget(value);
	}

	internal bool IsAlive => Reference.TryGetTarget(out _);

	internal ComparableWeakReference(T target) : this(new WeakReference<T>(target)) { }

	internal ComparableWeakReference(WeakReference<T> reference) {
		reference.TryGetTarget(out var target);
		Reference = new WeakReference<T>(target!);
	}

	internal ComparableWeakReference(SerializationInfo info, StreamingContext context) {
		if (info is null) {
			ThrowHelper.ThrowArgumentNullException(nameof(info));
			return;
		}
		var target = (T?)info.GetValue("TrackedObject", typeof(T));
		var trackResurrection = info.GetBoolean("TrackResurrection");
		Reflect.Create(Reference, target!, trackResurrection);
	}

	internal bool TryGetTarget(out T target) => Reference.TryGetTarget(out target!);

	internal void SetTarget(T? target) => Reference.SetTarget(target!);

	public void GetObjectData(SerializationInfo info, StreamingContext context) {
		if (info is null) {
			ThrowHelper.ThrowArgumentNullException(nameof(info));
		}
		info.AddValue("TrackedObject", Target, typeof(T));
		info.AddValue("TrackResurrection", IsTrackResurrection());
	}

	[SecuritySafeCritical]
	private bool IsTrackResurrection() => Reflect.IsTrackResurrection(Reference);

	public override int GetHashCode() {
		return Reference.TryGetTarget(out var target) ? target.GetHashCode() : NullHash;
	}

	public ulong GetLongHashCode() {
		return Reference.TryGetTarget(out var target) ? LongHash.From(target) : LongHash.Null;
	}

	public static implicit operator WeakReference<T>(ComparableWeakReference<T> reference) => reference.Reference;

	public static implicit operator ComparableWeakReference<T>(WeakReference<T> reference) => new(reference);

	public bool Equals(ComparableWeakReference<T>? obj) => obj is { } reference && this == reference;

	public bool Equals(WeakReference<T>? obj) => obj is { } reference && this == reference;

	public static bool Equals(WeakReference<T> objA, ComparableWeakReference<T> objB) => objA == objB;

	internal static bool Equals(object objA, ComparableWeakReference<T> objB) => objB.Equals(objA);

	public override bool Equals(object? obj) => obj switch {
		ComparableWeakReference<T> reference => this == reference,
		WeakReference<T> reference => this == reference,
		_ => false,
	};

	public static bool operator ==(ComparableWeakReference<T> objA, ComparableWeakReference<T> objB) {
		if (objA.TryGetTarget(out T l) && objB.TryGetTarget(out T r)) {
			return l == r;
		}
		return false;
	}

	public static bool operator ==(ComparableWeakReference<T> objA, WeakReference<T> objB) {
		if (objA.TryGetTarget(out T l) && objB.TryGetTarget(out T? r)) {
			return l == r;
		}
		return false;
	}

	public static bool operator ==(WeakReference<T> objA, ComparableWeakReference<T> objB) => objB == objA;

	public static bool operator !=(ComparableWeakReference<T> objA, ComparableWeakReference<T> objB) {
		if (objA.TryGetTarget(out T l) && objB.TryGetTarget(out T r)) {
			return l != r;
		}
		return true;
	}

	public static bool operator !=(ComparableWeakReference<T> objA, WeakReference<T> objB) {
		if (objA.TryGetTarget(out T l) && objB.TryGetTarget(out T? r)) {
			return l != r;
		}
		return true;
	}

	public static bool operator !=(WeakReference<T> objA, ComparableWeakReference<T> objB) => objB != objA;
}
