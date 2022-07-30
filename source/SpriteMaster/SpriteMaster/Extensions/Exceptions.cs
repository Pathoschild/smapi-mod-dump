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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using static SpriteMaster.Runtime;

namespace SpriteMaster.Extensions;

internal static class Exceptions {
	[Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden]
	[MethodImpl(MethodImpl.ErrorPath)]
	internal static void PrintTrace<T>(this T exception, [CallerMemberName] string caller = "") where T : Exception => Debug.Trace(exception: exception, caller: caller);

	[DebuggerStepThrough, DebuggerHidden]
	[MethodImpl(MethodImpl.ErrorPath)]
	internal static void PrintInfo<T>(this T exception, [CallerMemberName] string caller = "") where T : Exception => Debug.Info(exception: exception, caller: caller);

	[DebuggerStepThrough, DebuggerHidden]
	[MethodImpl(MethodImpl.ErrorPath)]
	internal static void PrintWarning<T>(this T exception, [CallerMemberName] string caller = "") where T : Exception => Debug.Warning(exception: exception, caller: caller);

	[DebuggerStepThrough, DebuggerHidden]
	[MethodImpl(MethodImpl.ErrorPath)]
	internal static void PrintError<T>(this T exception, [CallerMemberName] string caller = "") where T : Exception => Debug.Error(exception: exception, caller: caller);

	[DoesNotReturn]
	[MethodImpl(MethodImpl.ErrorPath)]
	internal static void ThrowArgumentException(string name, object? value) =>
		throw new ArgumentException($"'{name}' = '{((value is null) ? "null" : value.GetType().FullName)}'", name);

	[DoesNotReturn]
	[MethodImpl(MethodImpl.ErrorPath)]
	internal static TReturn ThrowArgumentException<TReturn>(string name, object? value) =>
		throw new ArgumentException($"'{name}' = '{((value is null) ? "null" : value.GetType().FullName)}'", name);
}
