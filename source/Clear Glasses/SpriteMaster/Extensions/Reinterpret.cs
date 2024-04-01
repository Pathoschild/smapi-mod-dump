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
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SpriteMaster.Extensions;

internal static class Reinterpret {
	#region ReinterpretAs

	[MethodImpl(Runtime.MethodImpl.Inline)]
	[Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden]
	private static void AssertSize<T>() =>
		Marshal.SizeOf<T>().AssertEqual(Unsafe.SizeOf<T>());

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static TTo ReinterpretAs<TFrom, TTo>(this TFrom value) where TFrom : struct where TTo : struct {
		AssertSize<TFrom>();
		AssertSize<TTo>();
		Unsafe.SizeOf<TTo>().AssertLessEqual(Unsafe.SizeOf<TFrom>());
		return Unsafe.As<TFrom, TTo>(ref Unsafe.AsRef(in value));
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static unsafe TTo ReinterpretAsUnsafe<TFrom, TTo>(this TFrom value) where TFrom : unmanaged where TTo : unmanaged {
		sizeof(TTo).AssertLessEqual(sizeof(TFrom));
		return *(TTo*)&value;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static TTo As<TTo>(this bool value) where TTo : unmanaged {
		if (typeof(TTo) == typeof(bool)) {
			return (TTo)(object)value;
		}
		if (typeof(TTo) == typeof(byte)) {
			return (TTo)(object)value.ReinterpretAs<byte>();
		}
		if (typeof(TTo) == typeof(sbyte)) {
			return (TTo)(object)value.ReinterpretAs<sbyte>();
		}
		if (typeof(TTo) == typeof(ushort)) {
			return (TTo)(object)(ushort)value.ReinterpretAs<byte>();
		}
		if (typeof(TTo) == typeof(short)) {
			return (TTo)(object)(short)value.ReinterpretAs<sbyte>();
		}
		if (typeof(TTo) == typeof(uint)) {
			return (TTo)(object)(uint)value.ReinterpretAs<byte>();
		}
		if (typeof(TTo) == typeof(int)) {
			return (TTo)(object)(int)value.ReinterpretAs<sbyte>();
		}
		if (typeof(TTo) == typeof(ulong)) {
			return (TTo)(object)(ulong)value.ReinterpretAs<byte>();
		}
		if (typeof(TTo) == typeof(long)) {
			return (TTo)(object)(long)value.ReinterpretAs<sbyte>();
		}
		if (typeof(TTo) == typeof(float)) {
			return (TTo)(object)(float)value.ReinterpretAs<byte>();
		}
		if (typeof(TTo) == typeof(double)) {
			return (TTo)(object)(double)value.ReinterpretAs<sbyte>();
		}
		return ThrowHelper.ThrowInvalidOperationException<TTo>($"Cannot convert bool to {typeof(TTo)}");
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static TTo ReinterpretAs<TTo>(this bool value) where TTo : unmanaged =>
		ReinterpretAsUnsafe<bool, TTo>(value);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static TTo ReinterpretAs<TTo>(this byte value) where TTo : unmanaged =>
		ReinterpretAsUnsafe<byte, TTo>(value);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static TTo ReinterpretAs<TTo>(this sbyte value) where TTo : unmanaged =>
		ReinterpretAsUnsafe<sbyte, TTo>(value);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static TTo ReinterpretAs<TTo>(this ushort value) where TTo : unmanaged =>
		ReinterpretAsUnsafe<ushort, TTo>(value);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static TTo ReinterpretAs<TTo>(this short value) where TTo : unmanaged =>
		ReinterpretAsUnsafe<short, TTo>(value);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static TTo ReinterpretAs<TTo>(this uint value) where TTo : unmanaged =>
		ReinterpretAsUnsafe<uint, TTo>(value);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static TTo ReinterpretAs<TTo>(this int value) where TTo : unmanaged =>
		ReinterpretAsUnsafe<int, TTo>(value);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static TTo ReinterpretAs<TTo>(this ulong value) where TTo : unmanaged =>
		ReinterpretAsUnsafe<ulong, TTo>(value);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static TTo ReinterpretAs<TTo>(this long value) where TTo : unmanaged =>
		ReinterpretAsUnsafe<long, TTo>(value);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static TTo ReinterpretAs<TTo>(this half value) where TTo : unmanaged =>
		ReinterpretAsUnsafe<half, TTo>(value);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static TTo ReinterpretAs<TTo>(this float value) where TTo : unmanaged =>
		ReinterpretAsUnsafe<float, TTo>(value);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static TTo ReinterpretAs<TTo>(this double value) where TTo : unmanaged =>
		ReinterpretAsUnsafe<double, TTo>(value);

	#endregion

	#region ReinterpretAsRef

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ref TTo ReinterpretAsRef<TFrom, TTo>(in TFrom value) where TFrom : struct where TTo : struct {
		AssertSize<TFrom>();
		AssertSize<TTo>();
		Unsafe.SizeOf<TTo>().AssertLessEqual(Unsafe.SizeOf<TFrom>());
		return ref Unsafe.As<TFrom, TTo>(ref Unsafe.AsRef(in value));
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static unsafe ref TTo ReinterpretAsRefUnsafe<TFrom, TTo>(in TFrom value) where TFrom : unmanaged where TTo : unmanaged {
		sizeof(TTo).AssertLessEqual(sizeof(TFrom));
		return ref Unsafe.As<TFrom, TTo>(ref Unsafe.AsRef(in value));
	}

	#endregion

	#region ReinterpretAsRef

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static unsafe Span<TTo> ReinterpretAsSpan<TFrom, TTo>(in TFrom value) where TFrom : struct where TTo : struct {
		AssertSize<TFrom>();
		AssertSize<TTo>();
		Unsafe.SizeOf<TTo>().AssertLessEqual(Unsafe.SizeOf<TFrom>());
		return new(Unsafe.AsPointer(ref Unsafe.AsRef(in value)), Unsafe.SizeOf<TFrom>() / Unsafe.SizeOf<TTo>());
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static unsafe Span<TTo> ReinterpretAsSpanUnsafe<TFrom, TTo>(scoped in TFrom value) where TFrom : unmanaged where TTo : unmanaged {
		sizeof(TTo).AssertLessEqual(sizeof(TFrom));
		return new(Unsafe.AsPointer(ref Unsafe.AsRef(in value)), sizeof(TFrom) / sizeof(TTo));
	}

	#endregion
}
