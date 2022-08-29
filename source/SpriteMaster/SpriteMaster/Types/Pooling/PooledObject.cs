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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SpriteMaster.Types.Pooling;

internal interface IPooledObject<T> : IDisposable where T : class, new() {
	T Value { get; }
}

internal interface ISealedPooledObject<T, TPooledObject> : IPooledObject<T> where T : class, new() where TPooledObject : ISealedPooledObject<T, TPooledObject> {
	T IPooledObject<T>.Value => Value;
	protected Action<T> Clear { get; }

	[MethodImpl(Runtime.MethodImpl.Inline)]
	protected void OnDispose(T value);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	void IDisposable.Dispose() {
#if DEBUG || DEVELOPMENT
			if (Value is not {} value) {
				ThrowHelper.ThrowInvalidOperationException($"{nameof(ISealedPooledObject<T, TPooledObject>)}.{nameof(Value)} was already disposed!");
				return;
			}
#else
		var value = Value;
#endif

		Clear(value);

		OnDispose(value);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public string? ToString() => Value.ToString();
}

[StructLayout(LayoutKind.Auto)]
internal readonly struct PooledObject<T, TPool> : ISealedPooledObject<T, PooledObject<T, TPool>> where T : class, new() where TPool : IObjectPool<T> {
	public readonly T Value { get; }
	private readonly Action<T> Clear;
	Action<T> ISealedPooledObject<T, PooledObject<T, TPool>>.Clear => Clear;
	private readonly IObjectPool<T> Pool;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal PooledObject(T value, IObjectPool<T> pool, Action<T>? clear) {
		Value = value;
		Pool = pool;

		if (clear is not null) {
			clear(value);
			Clear = clear;
		}
		else {
			Clear = _ => { };
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	readonly void ISealedPooledObject<T, PooledObject<T, TPool>>.OnDispose(T value) {
		Pool.Return(value);

#if DEBUG || DEVELOPMENT
			Unsafe.AsRef(Value) = null!;
#endif
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override readonly string? ToString() => Value.ToString();
}

[StructLayout(LayoutKind.Auto)]
internal readonly struct DefaultPooledObject<T> : ISealedPooledObject<T, DefaultPooledObject<T>> where T : class, new() {
	public readonly T Value { get; }
	private readonly Action<T> Clear;
	Action<T> ISealedPooledObject<T, DefaultPooledObject<T>>.Clear => Clear;
	private static ObjectPool<T> Pool => ObjectPool<T>.Default;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal DefaultPooledObject(T value, Action<T>? clear) {
		Value = value;

		if (clear is not null) {
			clear(value);
			Clear = clear;
		}
		else {
			Clear = _ => { };
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	readonly void ISealedPooledObject<T, DefaultPooledObject<T>>.OnDispose(T value) {
		Pool.Return(value);

#if DEBUG || DEVELOPMENT
			Unsafe.AsRef(Value) = null!;
#endif
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override readonly string? ToString() => Value.ToString();
}

