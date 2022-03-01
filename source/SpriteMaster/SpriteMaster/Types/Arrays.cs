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
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace SpriteMaster.Types;

static class Arrays {
	private static class EmptyArrayStatic<T> {
		[ImmutableObject(true)]
		internal static readonly T[] Value = new T[0];
	}

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	[ImmutableObject(true)]
	internal static T[] Empty<T>() => EmptyArrayStatic<T>.Value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static T[] Singleton<T>(T value) => new []{ value };

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static T[] Of<T>(params T[] values) => values;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static MemoryStream Stream(this byte[] data) => new MemoryStream(data, 0, data.Length, true, true);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static MemoryStream Stream(this byte[] data, int offset = 0, int length = -1, FileAccess access = FileAccess.ReadWrite) {
		if (length == -1) {
			length = data.Length - offset;
		}
		return new MemoryStream(data, offset, length, (access != FileAccess.Read), true);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static T[] Reverse<T>(this T[] array) {
		//Contract.AssertNotNull(array);
		Array.Reverse(array);
		return array;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static T[] Reversed<T>(this T[] array) {
		//Contract.AssertNotNull(array);
		var result = (T[])array.Clone();
		Array.Reverse(result);
		return result;
	}
}

internal static class Arrays<T> {
	[ImmutableObject(true)]
	internal static readonly T[] Empty = Arrays.Empty<T>();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static T[] Singleton(T value) => Arrays.Singleton<T>(value);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static T[] Of(params T[] values) => Arrays.Of<T>(values);
}
