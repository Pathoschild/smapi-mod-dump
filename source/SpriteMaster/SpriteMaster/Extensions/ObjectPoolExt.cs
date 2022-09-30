/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Types.Pooling;
using System;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Extensions;

internal static class ObjectPoolExt {
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static DefaultPooledObject<T> Take<T>(Action<T>? clear = null) where T : class, new() =>
		ObjectPoolExt<T>.Take(clear);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static LazyDefaultPooledObject<T> TakeLazy<T>(Action<T>? clear = null) where T : class, new() =>
		ObjectPoolExt<T>.TakeLazy(clear);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static PooledObject<T, TrackingObjectPool<T>> TakeTracked<T>(Action<T>? clear = null) where T : class, new() =>
		ObjectPoolExt<T>.TakeTracked(clear);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static LazyPooledObject<T, TrackingObjectPool<T>> TakeLazyTracked<T>(Action<T>? clear = null) where T : class, new() =>
		ObjectPoolExt<T>.TakeLazyTracked(clear);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static T Get<T>() where T : class, new() =>
		ObjectPoolExt<T>.Get();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static T GetTracked<T>() where T : class, new() =>
		ObjectPoolExt<T>.GetTracked();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void Return<T>(T value) where T : class, new() =>
		ObjectPoolExt<T>.Return(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void ReturnTracked<T>(T value) where T : class, new() =>
		ObjectPoolExt<T>.ReturnTracked(value);
}

internal static class ObjectPoolExt<T> where T : class, new() {
	internal static ObjectPool<T> DefaultPool => ObjectPool<T>.Default;
	internal static TrackingObjectPool<T> DefaultTrackingPool => TrackingObjectPool<T>.Default;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static DefaultPooledObject<T> Take(Action<T>? clear = null) =>
		new(DefaultPool.Get(), clear);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static LazyDefaultPooledObject<T> TakeLazy(Action<T>? clear = null) =>
		new(DefaultPool.Get, clear);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static PooledObject<T, TrackingObjectPool<T>> TakeTracked(Action<T>? clear = null) =>
		new(DefaultTrackingPool.Get(), DefaultTrackingPool, clear);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static LazyPooledObject<T, TrackingObjectPool<T>> TakeLazyTracked(Action<T>? clear = null) =>
		new(DefaultTrackingPool.Get, DefaultTrackingPool, clear);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static T Get() =>
		DefaultPool.Get();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static T GetTracked() =>
		DefaultTrackingPool.Get();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void Return(T value) =>
		DefaultPool.Return(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void ReturnTracked(T value) =>
		DefaultTrackingPool.Return(value);
}
