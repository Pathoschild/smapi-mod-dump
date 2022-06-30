/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace SpriteMaster.Types;

internal interface ILazy<out T> {
	bool IsInitialized { get; }
	T Value { get; }
}

[DebuggerDisplay("Value={Value}, IsInitialized={IsInitialized}")]
[StructLayout(LayoutKind.Auto)]
internal readonly struct ValueLazy<T> : ILazy<T> {
	internal delegate T FactoryDelegate();

	private readonly FactoryDelegate? _factory = null;
	private readonly T? _value = default;

	public readonly bool IsInitialized => _factory is null;

	public readonly T Value {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get {
			if (_factory is not null) {
				InitializeValue();
			}

			return _value!;
		}
	}

	internal ValueLazy(T value) {
		_value = value;
	}

	internal ValueLazy(FactoryDelegate factory) {
		_factory = factory;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal readonly T GetValue(out bool wasInitialized) {
		bool initialized = _factory is null;
		wasInitialized = initialized;
		if (!initialized) {
			InitializeValue();
		}
		return _value!;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private readonly void InitializeValue() {
		_value.AssertDefault();
		_factory.AssertNotNull();
		Unsafe.AsRef(_value) = _factory!();
		Unsafe.AsRef(_factory) = null!;
	}
}

[StructLayout(LayoutKind.Auto)]
internal readonly struct ConcurrentValueLazy<T> : ILazy<T> {
	internal delegate T FactoryDelegate();

	private readonly object? _state = null;
	private readonly FactoryDelegate? _factory = null;
	private readonly T? _value = default;

	public readonly bool IsInitialized => (Volatile.Read(ref Unsafe.AsRef(_factory)) is null && Volatile.Read(ref Unsafe.AsRef(_state)) is null);

	public readonly T Value {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get {
			if (_state is null) {
				return _value!;
			}

			InitializeValue();

			return _value!;
		}
	}

	internal ConcurrentValueLazy(T value) {
		_value = value;
	}

	internal ConcurrentValueLazy(FactoryDelegate factory) {
		_factory = factory;
		_state = new();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal readonly T GetValue(out bool wasInitialized) {
		if (_state is null) {
			wasInitialized = true;
			return _value!;
		}

		wasInitialized = InitializeValue();
		return _value!;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private readonly bool InitializeValue() {
		if (Interlocked.Exchange(ref Unsafe.AsRef(_factory), null) is not {} factory) {
			// If the factory is null, the object may still be being constructed.
			if (Volatile.Read(ref Unsafe.AsRef(_state)) is {} state) {
				// Do nothing with the lock, we are using it as a sequence point.
				lock (state) { }
			}
			return true;
		}

		lock (_state!) {
			_value.AssertDefault();
			factory.AssertNotNull();
			var value = factory();
			
			if (typeof(T).IsValueType) {
				Unsafe.AsRef(_value) = value;
			}
			else {
				Volatile.Write(ref Unsafe.As<T?, object>(ref Unsafe.AsRef(_value))!, value!);
			}

			Volatile.Write(ref Unsafe.AsRef(_factory), null!);
			Volatile.Write(ref Unsafe.AsRef(_state), null!);
		}

		return false;
	}
}
