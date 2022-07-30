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
using System.Runtime.CompilerServices;

namespace SpriteMaster.Types.Pooling;

// Based upon 'LeakTrackingObjectPool.cs'
// https://github.com/dotnet/aspnetcore/blob/4adb4258cb630114bc232af5339860dab9a5415e/src/ObjectPool/src/LeakTrackingObjectPool.cs
internal sealed class TrackingObjectPool<T> : ISealedObjectPool<T, TrackingObjectPool<T>> where T : class, new() {
	internal static readonly TrackingObjectPool<T> Default = new();

	private static readonly ObjectPool<Tracker> TrackerPool = new();

	private readonly ConditionalWeakTable<T, Tracker> Trackers = new();
	private readonly ObjectPool<T> Pool = new();

	public int Count => Pool.Count;

	public long Allocated => Pool.Allocated;

	internal TrackingObjectPool() {
	}

	internal TrackingObjectPool(int initialCapacity) {
		Pool = new(initialCapacity);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public T Get() {
		var result = Pool.Get();

		var tracker = TrackerPool.Get();
		tracker.Reinitialize();

		Trackers.Add(result, tracker);

		return result;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public void Return(T value) {
		if (Trackers.TryGetValue(value, out var tracker)) {
			Trackers.Remove(value);
			tracker.Dispose();
			TrackerPool.Return(tracker);
		}

		Pool.Return(value);
	}

	private sealed class Tracker : IDisposable {
		private string? StackTrace;

		[MethodImpl(Runtime.MethodImpl.Inline)]
		internal void Reinitialize() {
			GC.ReRegisterForFinalize(this);
			StackTrace = Environment.StackTrace;
		}

		[MethodImpl(Runtime.MethodImpl.Inline)]
		public void Dispose() {
			GC.SuppressFinalize(this);
			StackTrace = null;
		}

		~Tracker() {
			if (!Environment.HasShutdownStarted) {
				Debug.Error($"{typeof(T).GetTypeName()} has leaked from {nameof(TrackingObjectPool<T>)}. Created at:\n{StackTrace}");
			}
		}
	}
}