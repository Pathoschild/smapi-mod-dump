/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
// ReSharper disable SuspiciousTypeConversion.Global

namespace MusicMaster.Extensions;

/// <summary>
/// Enumerator Extensions
/// </summary>
internal static class EnumExt {
	/// <summary>
	/// Return an array name and value pairs representing the enum
	/// </summary>
	/// <typeparam name="TEnum">Enumerator Type</typeparam>
	/// <returns>Array of name-value pairs</returns>
	[Pure, MustUseReturnValue]
	internal static KeyValuePair<string, TEnum>[] Get<TEnum>() where TEnum : struct, Enum {
		var names = Enum.GetNames(typeof(TEnum));
		var result = new KeyValuePair<string, TEnum>[names.Length];
		for (int i = 0; i < names.Length; ++i) {
			result[i] = new(names[i], Enum.Parse<TEnum>(names[i]));
		}

		return result;
	}

	/// <summary>
	/// Return an array name and value pairs representing the enum
	/// </summary>
	/// <param name="type">Enumerator Type</param>
	/// <returns>Array of name-value pairs</returns>
	[Pure, MustUseReturnValue]
	internal static KeyValuePair<string, int>[] Get(Type type) {
		var names = Enum.GetNames(type);
		var result = new KeyValuePair<string, int>[names.Length];
		for (int i = 0; i < names.Length; ++i) {
			result[i] = new(names[i], (int)Enum.Parse(type, names[i]));
		}

		return result;
	}

	[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	internal static Type GetUnderlyingType<T>() where T : Enum => Enum.GetUnderlyingType(typeof(T));

	[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	internal static T? Parse<T>(Type? enumType, string value) where T : unmanaged {
		if (enumType is null) {
			return null;
		}

		if (Enum.TryParse(enumType, value, out var result)) {
			return (T)result!;
		}

		return null;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static TTo CastTo<TFrom, TTo>(this TFrom value) where TFrom : unmanaged where TTo : unmanaged =>
		(TTo)(object)value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static TEnum GetOr<TEnum>(this TEnum e, TEnum value) where TEnum : unmanaged, Enum =>
		e switch {
			byte ev => ((byte)(e.CastTo<TEnum, byte>() | value.CastTo<TEnum, byte>())).CastTo<byte, TEnum>(),
			ushort ev => ((ushort)(e.CastTo<TEnum, ushort>() | value.CastTo<TEnum, ushort>())).CastTo<ushort, TEnum>(),
			uint ev => ((uint)(e.CastTo<TEnum, uint>() | value.CastTo<TEnum, uint>())).CastTo<uint, TEnum>(),
			ulong ev => ((ulong)(e.CastTo<TEnum, ulong>() | value.CastTo<TEnum, ulong>())).CastTo<ulong, TEnum>(),
			sbyte ev => ((sbyte)(e.CastTo<TEnum, sbyte>() | value.CastTo<TEnum, sbyte>())).CastTo<sbyte, TEnum>(),
			short ev => ((short)(e.CastTo<TEnum, short>() | value.CastTo<TEnum, short>())).CastTo<short, TEnum>(),
			int ev => ((int)(e.CastTo<TEnum, int>() | value.CastTo<TEnum, int>())).CastTo<int, TEnum>(),
			long ev => ((long)(e.CastTo<TEnum, long>() | value.CastTo<TEnum, long>())).CastTo<long, TEnum>(),
			_ => ThrowHelper.ThrowInvalidOperationException<TEnum>($"Unknown Conversion Type for enum: {typeof(TEnum)}")
		};

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void ConditionalOr<T>(this ref T e, T value, bool condition) where T : unmanaged, Enum {
		if (condition) {
			e = e.GetOr(value);
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void ConditionalOr<T>(this ref T e, T? value) where T : unmanaged, Enum {
		if (value.HasValue) {
			e = e.GetOr(value.Value);
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static TTo CastToVia<TFrom, TVia, TTo>(this TFrom value) where TFrom : unmanaged where TVia : unmanaged where TTo : unmanaged =>
		(TTo)(object)(TVia)(object)value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static uint GetOrBranchless(this uint e, uint value, bool condition) {
		int conditionValue = condition.ReinterpretAs<int>();
		return (e & ~value) | ((uint)-conditionValue & value);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static ulong GetOrBranchless(this ulong e, ulong value, bool condition) {
		long conditionValue = condition.ReinterpretAs<long>();
		return (e & ~value) | ((ulong)-conditionValue & value);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static int GetOrBranchless(this int e, int value, bool condition) {
		int conditionValue = condition.ReinterpretAs<int>();
		return (e & ~value) | (-conditionValue & value);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static long GetOrBranchless(this long e, long value, bool condition) {
		long conditionValue = condition.ReinterpretAs<long>();
		return (e & ~value) | (-conditionValue & value);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static TEnum GetOrBranchless<TEnum>(this TEnum e, TEnum value, bool condition) where TEnum : unmanaged, Enum =>
		e switch {
			byte eCast => ((uint)eCast).GetOrBranchless(value.CastToVia<TEnum, byte, uint>(), condition).CastToVia<uint, byte, TEnum>(),
			ushort eCast => ((uint)eCast).GetOrBranchless(value.CastToVia<TEnum, ushort, uint>(), condition).CastToVia<uint, ushort, TEnum>(),
			uint eCast => eCast.GetOrBranchless(value.CastTo<TEnum, uint>(), condition).CastTo<uint, TEnum>(),
			ulong eCast => eCast.GetOrBranchless(value.CastTo<TEnum, ulong>(), condition).CastTo<ulong, TEnum>(),
			sbyte eCast => ((int)eCast).GetOrBranchless(value.CastToVia<TEnum, sbyte, int>(), condition).CastToVia<int, sbyte, TEnum>(),
			short eCast => ((int)eCast).GetOrBranchless(value.CastToVia<TEnum, short, int>(), condition).CastToVia<int, short, TEnum>(),
			int eCast => eCast.GetOrBranchless(value.CastTo<TEnum, int>(), condition).CastTo<int, TEnum>(),
			long eCast => eCast.GetOrBranchless(value.CastTo<TEnum, long>(), condition).CastTo<long, TEnum>(),
			_ => ThrowHelper.ThrowInvalidOperationException<TEnum>($"Unknown Conversion Type for enum: {typeof(TEnum)}")
		};

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void ConditionalOrBranchless<T>(this ref T e, T value, bool condition) where T : unmanaged, Enum {
		e = e.GetOrBranchless(value, condition);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void ConditionalOrBranchless<T>(this ref T e, T? value) where T : unmanaged, Enum {
		e = e.GetOrBranchless(value ?? default, value.HasValue);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static unsafe TEnum ConditionalFlag<TEnum>(this TEnum e, bool condition) where TEnum : unmanaged, Enum {
		if (sizeof(TEnum) == 1) {
			byte conditionValue = (byte)-condition.ReinterpretAs<sbyte>(); // 0 || 1 -> 0 || -1
			return ((byte)(e.ReinterpretAsUnsafe<TEnum, byte>() & conditionValue)).ReinterpretAsUnsafe<byte, TEnum>();
		}
		else if (sizeof(TEnum) == 2) {
			ushort conditionValue = (ushort)(short)-condition.ReinterpretAs<sbyte>(); // 0 || 1 -> 0 || -1
			return ((ushort)(e.ReinterpretAsUnsafe<TEnum, ushort>() & conditionValue)).ReinterpretAsUnsafe<ushort, TEnum>();
		}
		else if (sizeof(TEnum) == 4) {
			uint conditionValue = (uint)(int)-condition.ReinterpretAs<sbyte>(); // 0 || 1 -> 0 || -1
			return ((uint)(e.ReinterpretAsUnsafe<TEnum, uint>() & conditionValue)).ReinterpretAsUnsafe<uint, TEnum>();
		}
		else {
			ulong conditionValue = (ulong)(long)-condition.ReinterpretAs<sbyte>(); // 0 || 1 -> 0 || -1
			return ((ulong)(e.ReinterpretAsUnsafe<TEnum, ulong>() & conditionValue)).ReinterpretAsUnsafe<ulong, TEnum>();
		}
	}
}
