/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SpriteMaster.Types.Pooling;

internal interface IPooledObject<T> : IDisposable where T : class, new() {
	T Value { get; }
}

internal interface ISealedPooledObject<T, TPooledObject> : IPooledObject<T> where T : class, new() where TPooledObject : ISealedPooledObject<T, TPooledObject> {
	T IPooledObject<T>.Value => Value;
	protected Action<T> Clear { get; }
	protected bool HasValue { get; }

	[MethodImpl(Runtime.MethodImpl.Inline)]
	protected void OnDispose(T value);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	void IDisposable.Dispose() {
		DisposeImpl();
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private void DisposeImpl() {
		if (!HasValue) {
			return;
		}

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
	protected internal void DisposeInner() => DisposeImpl();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public string? ToString() => Value.ToString();
}

internal static class _PooledObjectClearMethodCache<T> where T : class, new() {
	internal static readonly Action<T> Method =
		typeof(T).GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public)?.CreateDelegate<Action<T>>() ?? NullClear;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static void NullClear(T _) {
	}
}

[StructLayout(LayoutKind.Auto)]
internal readonly struct PooledObject<T, TPool> : ISealedPooledObject<T, PooledObject<T, TPool>> where T : class, new() where TPool : IObjectPool<T> {
	public readonly T Value { get; }
	private readonly Action<T> Clear;
	Action<T> ISealedPooledObject<T, PooledObject<T, TPool>>.Clear => Clear;
	bool ISealedPooledObject<T, PooledObject<T, TPool>>.HasValue => true;

	private readonly IObjectPool<T> Pool;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal PooledObject(T value, IObjectPool<T> pool, Action<T>? clear) {
		Value = value;
		Pool = pool;

		clear ??= _PooledObjectClearMethodCache<T>.Method;

		clear(value);
		Clear = clear;
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

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly void Dispose() => ((ISealedPooledObject<T, PooledObject<T, TPool>>)this).DisposeInner();
}

[StructLayout(LayoutKind.Auto)]
internal readonly struct LazyPooledObject<T, TPool> : ISealedPooledObject<T, PooledObject<T, TPool>> where T : class, new() where TPool : IObjectPool<T> {
	public readonly T Value => InnerValue.Value;
	private readonly Lazy<T> InnerValue;
	private readonly Action<T> Clear;
	Action<T> ISealedPooledObject<T, PooledObject<T, TPool>>.Clear => Clear;
	bool ISealedPooledObject<T, PooledObject<T, TPool>>.HasValue { get; } = false;
	private readonly bool HasValue = false;
	private readonly IObjectPool<T> Pool;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private T GetLazyValue(Func<T> getter, Action<T> clear) {
		var result = getter();

		clear(result);

		Unsafe.AsRef(HasValue) = true;

		return result;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal LazyPooledObject(Func<T> getter, IObjectPool<T> pool, Action<T>? clear) {
		clear ??= _PooledObjectClearMethodCache<T>.Method;

		var @this = this;
		InnerValue = new(() => @this.GetLazyValue(getter, clear));

		Pool = pool;
		Clear = clear;
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

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly void Dispose() => ((ISealedPooledObject<T, PooledObject<T, TPool>>)this).DisposeInner();
}

[StructLayout(LayoutKind.Auto)]
internal readonly struct DefaultPooledObject<T> : ISealedPooledObject<T, DefaultPooledObject<T>> where T : class, new() {
	public readonly T Value { get; }
	private readonly Action<T> Clear;
	Action<T> ISealedPooledObject<T, DefaultPooledObject<T>>.Clear => Clear;
	bool ISealedPooledObject<T, DefaultPooledObject<T>>.HasValue => true;
	private static ObjectPool<T> Pool => ObjectPool<T>.Default;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal DefaultPooledObject(T value, Action<T>? clear) {
		Value = value;

		clear ??= _PooledObjectClearMethodCache<T>.Method;

		clear(value);
		Clear = clear;
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

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly void Dispose() => ((ISealedPooledObject<T, DefaultPooledObject<T>>)this).DisposeInner();
}

[StructLayout(LayoutKind.Auto)]
internal readonly struct LazyDefaultPooledObject<T> : ISealedPooledObject<T, DefaultPooledObject<T>> where T : class, new() {
	public readonly T Value => InnerValue.Value;
	private readonly Lazy<T> InnerValue;
	private readonly Action<T> Clear;
	Action<T> ISealedPooledObject<T, DefaultPooledObject<T>>.Clear => Clear;
	bool ISealedPooledObject<T, DefaultPooledObject<T>>.HasValue { get; } = false;
	private readonly bool HasValue = false;
	private static ObjectPool<T> Pool => ObjectPool<T>.Default;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private T GetLazyValue(Func<T> getter, Action<T> clear) {
		var result = getter();

		clear(result);

		Unsafe.AsRef(HasValue) = true;

		return result;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal LazyDefaultPooledObject(Func<T> getter, Action<T>? clear) {
		clear ??= _PooledObjectClearMethodCache<T>.Method;

		var @this = this;
		InnerValue = new(() => @this.GetLazyValue(getter, clear));

		Clear = clear;
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

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly void Dispose() => ((ISealedPooledObject<T, DefaultPooledObject<T>>)this).DisposeInner();
}
