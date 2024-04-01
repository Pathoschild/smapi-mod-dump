/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using SpriteMaster.Extensions.Reflection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Security;

namespace SpriteMaster.Types;

[SuppressMessage("Code Quality", "IDE0051:Remove unused private members")]
internal sealed class ComparableWeakReference<T> :
	ILongHash,
	IEquatable<WeakReference<T>>,
	IEquatable<ComparableWeakReference<T>>
where T : class? {
	internal const int NullHash = 0;

	private static class Reflect {
		internal static readonly Func<WeakReference<T>, bool> IsTrackResurrection = typeof(WeakReference<T>).
			GetInstanceMethod("IsTrackResurrection")?.CreateDelegate<Func<WeakReference<T>, bool>>() ??
			throw new NullReferenceException(GetName(nameof(IsTrackResurrection)));

		private static string GetName(string name) => $"{typeof(ComparableWeakReference<T>).Name}.{name}";
	}

	private readonly WeakReference<T> Reference = new(null!);

	internal T? Target {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get {
			_ = TryGetTarget(out T? target);
			return target;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => SetTarget(value);
	}

	internal bool IsAlive => Reference.TryGetTarget(out _);

	internal ComparableWeakReference(T target) : this(new WeakReference<T>(target)) { }

	private ComparableWeakReference(WeakReference<T> reference) {
		reference.TryGetTarget(out var target);
		Reference = new(target!);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal bool TryGetTarget([MaybeNullWhen(false)] out T target) => Reference.TryGetTarget(out target!);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void SetTarget(T? target) {
		_ = TryGetTarget(out T? currentTarget);
		if (target == currentTarget) {
			return;
		}
		Reference.SetTarget(target!);
	}

	[SecuritySafeCritical, MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool IsTrackResurrection() => Reflect.IsTrackResurrection(Reference);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() {
		return Reference.TryGetTarget(out var target) ? target.GetHashCode() : NullHash;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ulong GetLongHashCode() {
		return Reference.TryGetTarget(out var target) ? LongHash.From(target) : LongHash.Null;
	}

	public static implicit operator WeakReference<T>(ComparableWeakReference<T> reference) => reference.Reference;

	public static implicit operator ComparableWeakReference<T>(WeakReference<T> reference) => new(reference);

	public bool Equals(ComparableWeakReference<T>? obj) => obj is { } reference && this == reference;

	public bool Equals(WeakReference<T>? obj) => obj is { } reference && this == reference;

	public bool Equals(T? obj) => ReferenceEquals(Target, obj);

	public override bool Equals(object? obj) => obj switch {
		ComparableWeakReference<T> reference => this == reference,
		WeakReference<T> reference => this == reference,
		T typedObj => ReferenceEquals(Target, typedObj),
		_ => false,
	};

	public static bool operator ==(ComparableWeakReference<T> objA, ComparableWeakReference<T> objB) {
		if (objA.TryGetTarget(out T? l) && objB.TryGetTarget(out T? r)) {
			return l == r;
		}
		return false;
	}

	public static bool operator ==(ComparableWeakReference<T> objA, WeakReference<T> objB) {
		if (objA.TryGetTarget(out T? l) && objB.TryGetTarget(out T? r)) {
			return l == r;
		}
		return false;
	}

	public static bool operator ==(WeakReference<T> objA, ComparableWeakReference<T> objB) => objB == objA;

	public static bool operator ==(T? objA, ComparableWeakReference<T> objB) => objB.Equals(objA);

	public static bool operator ==(ComparableWeakReference<T> objA, T? objB) => objA.Equals(objB);

	public static bool operator !=(ComparableWeakReference<T> objA, ComparableWeakReference<T> objB) {
		if (objA.TryGetTarget(out T? l) && objB.TryGetTarget(out T? r)) {
			return l != r;
		}
		return true;
	}

	public static bool operator !=(ComparableWeakReference<T> objA, WeakReference<T> objB) {
		if (objA.TryGetTarget(out T? l) && objB.TryGetTarget(out T? r)) {
			return l != r;
		}
		return true;
	}

	public static bool operator !=(WeakReference<T> objA, ComparableWeakReference<T> objB) => objB != objA;

	public static bool operator !=(T? objA, ComparableWeakReference<T> objB) => !objB.Equals(objA);

	public static bool operator !=(ComparableWeakReference<T> objA, T? objB) => !objA.Equals(objB);
}
