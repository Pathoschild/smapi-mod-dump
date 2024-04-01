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
using Microsoft.Toolkit.HighPerformance;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SpriteMaster.Extensions;

internal static class ArrayExt {
	// https://stackoverflow.com/a/3577227/5055153
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	private struct ArrayHeader {
		public nuint Type;
		public nuint Length;
	}

	[Pure, MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static unsafe ref ArrayHeader GetHeaderRef<T>(T[] array) where T : unmanaged {
		ref ArrayHeader element = ref Unsafe.As<T, ArrayHeader>(ref MemoryMarshal.GetArrayDataReference(array));
		ref ArrayHeader header = ref Unsafe.Subtract(ref element, 1);
		return ref header;
	}

	[Pure, MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static unsafe ArrayHeader* GetHeaderPtr<T>(T* arrayPtr) where T : unmanaged {
		return (ArrayHeader*)arrayPtr - 1;
	}

	private static class ArrayTypeCode<T> where T : unmanaged {
		// ReSharper disable once StaticMemberInGenericType
		private static readonly nuint TypeCode = GetArrayType();

		[Pure, MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static nuint GetArrayType() {
			var refArray = Array.Empty<T>();
			return GetHeaderRef(refArray).Type;
		}

		[MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static unsafe T[] Apply<TFrom>(TFrom[] data) where TFrom : unmanaged {
			fixed (TFrom* dataPtr = data) {
				if (dataPtr is null) {
					ref var header = ref GetHeaderRef(data);
					header.Type = TypeCode;
				}
				else {
					var header = GetHeaderPtr(dataPtr);
					header->Type = TypeCode;
				}
			}

			return Unsafe.As<TFrom[], T[]>(ref data);
		}

		[MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static unsafe T[] Apply<TFrom>(TFrom[] data, nuint length) where TFrom : unmanaged {
			fixed (TFrom* dataPtr = data) {
				if (dataPtr is null) {
					ref var header = ref GetHeaderRef(data);
					header.Type = TypeCode;
					header.Length = length;
				}
				else {
					var header = GetHeaderPtr(dataPtr);
					header->Type = TypeCode;
					header->Length = length;
				}
			}

			return Unsafe.As<TFrom[], T[]>(ref data);
		}
	}

	/// <summary>
	/// Converts the input array (<paramref name="source"/>)'s type and length to the requested destination type (<typeparamref name="TTo"/>).
	/// <para>
	/// Highly destructive and unsafe. <paramref name="source"/> must not be used after calling this.
	/// </para>
	/// </summary>
	/// <typeparam name="TFrom">Type to convert from.</typeparam>
	/// <typeparam name="TTo">Type to convert to.</typeparam>
	/// <param name="source"><seealso cref="Array"/> to convert.</param>
	/// <returns>The same array, but converted.</returns>
	[MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static unsafe TTo[] Convert<TFrom, TTo>(this TFrom[] source) where TFrom : unmanaged where TTo : unmanaged {
		if (typeof(TTo) == typeof(TFrom)) {
			return (TTo[])(object)source;
		}

		if (source.LongLength == 0L) {
			return Array.Empty<TTo>();
		}

		nuint fromSize = (nuint)sizeof(TFrom);
		nuint toSize = (nuint)sizeof(TTo);

		nuint sourceLength = (nuint)source.LongLength;

		if (fromSize == toSize) {
			return ArrayTypeCode<TTo>.Apply(source);
		}
		else {
			nuint byteLength = sourceLength * fromSize;

			(sourceLength * fromSize).AssertAligned(
				toSize,
				$"Invalid Array Conversion (misaligned sizes): {typeof(TFrom)} ({fromSize}/{byteLength}) -> {typeof(TTo)} ({toSize})"
			);

			return ArrayTypeCode<TTo>.Apply(source, byteLength / toSize);
		}
	}

	/// <summary>
	/// Converts the input array (<paramref name="source"/>)'s type and length to the requested destination type (<typeparamref name="TTo"/>).
	/// <para>
	/// Highly destructive and unsafe unless <paramref name="copy"/> is <see langword="true"/>. <paramref name="source"/> must not be used after calling this.
	/// </para>
	/// </summary>
	/// <typeparam name="TFrom">Type to convert from.</typeparam>
	/// <typeparam name="TTo">Type to convert to.</typeparam>
	/// <param name="source"><seealso cref="Array"/> to convert.</param>
	/// <param name="copy">Should the array be copied instead of converted?</param>
	/// <returns>The same array, but converted, if <paramref name="copy"/> is <see langword="true"/>, otherwise a new array representing the converted data.</returns>
	[MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static TTo[] Convert<TFrom, TTo>(this TFrom[] source, bool copy) where TFrom : unmanaged where TTo : unmanaged {
		if (!copy) {
			return Convert<TFrom, TTo>(source);
		}

		if (source.LongLength == 0L) {
			return Array.Empty<TTo>();
		}

		return source.AsReadOnlySpan().Cast<TFrom, TTo>().ToArray();
	}

	[MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static unsafe T* GetPointerFromPinned<T>(this T[] array) where T : unmanaged =>
		(T*)Unsafe.AsPointer(ref MemoryMarshal.GetArrayDataReference(array));

	[MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static T[] Clone<T>(this T[] array) where T : unmanaged {
		var temp = GC.AllocateUninitializedArray<T>(array.Length);
		Unsafe.CopyBlock(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetArrayDataReference(temp)), ref Unsafe.As<T, byte>(ref MemoryMarshal.GetArrayDataReference(array)), (uint)(array.LongLength * Unsafe.SizeOf<T>()));
		return temp;
	}

	[MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static T[] CloneFast<T>(this T[] array) where T : unmanaged {
		var temp = GC.AllocateUninitializedArray<T>(array.Length);
		Unsafe.CopyBlock(
			ref Unsafe.As<T, byte>(ref MemoryMarshal.GetArrayDataReference(temp)), 
			ref Unsafe.As<T, byte>(ref MemoryMarshal.GetArrayDataReference(array)),
			(uint)(array.LongLength * Unsafe.SizeOf<T>())
		);
		return temp;
	}

	[MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static List<T> CloneFast<T>(this List<T> list) where T : unmanaged {
		var temp = GC.AllocateUninitializedArray<T>(list.Count);
		Unsafe.CopyBlock(
			ref Unsafe.As<T, byte>(ref MemoryMarshal.GetArrayDataReference(temp)),
			ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(list.AsSpan())),
			(uint)(list.Count * Unsafe.SizeOf<T>())
		);
		return temp.BeList();
	}

	[Pure, MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static int[] Range(int start, int count, int change = 1) {
		var result = GC.AllocateUninitializedArray<int>(count);
		for (int i = 0; count > 0; --count) {
			result[i++] = start;
			start += change;
		}
		return result;
	}

	[Pure, MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static long[] Range(long start, long count, long change = 1L) {
		count.AssertLessEqual((long)int.MaxValue);
		count.AssertGreater((long)int.MinValue);

		var result = GC.AllocateUninitializedArray<long>((int)count);
		for (int i = 0; count > 0; --count) {
			result[i++] = start;
			start += change;
		}
		return result;
	}

	[Pure, MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveOptimization)]
	internal static bool ContainsFast<T>(this T[] array, T element) {
		foreach (var item in array) {
			if (EqualityComparer<T>.Default.Equals(item, element)) {
				return true;
			}
		}

		return false;
	}

	[Pure, MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveOptimization)]
	internal static bool ContainsFast<T, TComparer>(this T[] array, T element, TComparer comparer) where TComparer : IEqualityComparer<T> {
		foreach (var item in array) {
			if (comparer.Equals(item, element)) {
				return true;
			}
		}

		return false;
	}
}
