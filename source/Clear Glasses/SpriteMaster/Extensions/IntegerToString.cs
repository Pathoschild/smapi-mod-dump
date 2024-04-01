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
using System.Runtime.CompilerServices;

namespace SpriteMaster.Extensions;

internal static partial class Integer {
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static string ToString16(this int value) => Convert.ToString(value, 16);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static string ToString16(this uint value) => Convert.ToString(value, 16);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static string ToString16(this long value) => Convert.ToString(value, 16);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static string ToString16(this ulong value) => Convert.ToString((long)value, 16);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static unsafe string ToString64(this int value) => Convert.ToBase64String(new ReadOnlySpan<byte>(&value, sizeof(int)));

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static unsafe string ToString64(this uint value) => Convert.ToBase64String(new ReadOnlySpan<byte>(&value, sizeof(uint)));

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static unsafe string ToString64(this long value) => Convert.ToBase64String(new ReadOnlySpan<byte>(&value, sizeof(long)));

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static unsafe string ToString64(this ulong value) => Convert.ToBase64String(new ReadOnlySpan<byte>(&value, sizeof(ulong)));
}
