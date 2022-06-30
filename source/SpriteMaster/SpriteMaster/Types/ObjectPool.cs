/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SpriteMaster.Types;
internal sealed class ObjectPool<T> where T : class, new() {
#if DEBUG || DEVELOPMENT
	private readonly ConcurrentSet<T> Pool = new();
#else
	private readonly ConcurrentBag<T> Pool = new();
#endif

	internal int Count => Pool.Count;

	internal ObjectPool() {
	}

	internal ObjectPool(int initialCapacity) {
		for (int i = 0; i < initialCapacity; ++i) {
			Pool.Add(new());
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal T Get() {
		if (!Pool.TryTake(out var result)) {
			result = new();
		}

		return result;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal PooledObject GetSafe() {
		if (!Pool.TryTake(out var result)) {
			result = new();
		}

		return new(result, this);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal void Return(T value) {
#if DEBUG || DEVELOPMENT
		if (value is null) {
			throw new NullReferenceException($"{nameof(Return)} value {nameof(value)} is null");
		}

		if (!Pool.Add(value)) {
			throw new InvalidOperationException($"Object {value} already exists in {nameof(ObjectPool<T>)}");
		}
#else
		Pool.Add(value);
#endif
	}

	[StructLayout(LayoutKind.Auto)]
	internal readonly ref struct PooledObject {
		internal readonly T Value;
		private readonly ObjectPool<T> Pool;

		[MethodImpl(Runtime.MethodImpl.Inline)]
		internal PooledObject(T value, ObjectPool<T> pool) {
			Value = value;
			Pool = pool;
		}

		[MethodImpl(Runtime.MethodImpl.Inline)]
		public void Dispose() {
#if DEBUG || DEVELOPMENT
			if (Value is null) {
				throw new InvalidOperationException($"{nameof(PooledObject)}.{nameof(Value)} was already disposed!");
			}
#endif

			Pool.Return(Value);

#if DEBUG || DEVELOPMENT
			Unsafe.AsRef(in Value) = null!;
#endif
		}
	}
}
