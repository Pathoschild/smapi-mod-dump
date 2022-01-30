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
using System.Runtime.CompilerServices;

using static SpriteMaster.Runtime;

namespace SpriteMaster.Extensions;

static class Exceptions {
	[DebuggerStepThrough, DebuggerHidden()]
	[MethodImpl(MethodImpl.ErrorPath)]
	internal static void PrintTrace<T>(this T exception, [CallerMemberName] string caller = null!) where T : Exception => Debug.Trace(exception: exception, caller: caller);

	[DebuggerStepThrough, DebuggerHidden()]
	[MethodImpl(MethodImpl.ErrorPath)]
	internal static void PrintInfo<T>(this T exception, [CallerMemberName] string caller = null!) where T : Exception => Debug.Info(exception: exception, caller: caller);

	[DebuggerStepThrough, DebuggerHidden()]
	[MethodImpl(MethodImpl.ErrorPath)]
	internal static void PrintWarning<T>(this T exception, [CallerMemberName] string caller = null!) where T : Exception => Debug.Warning(exception: exception, caller: caller);

	[DebuggerStepThrough, DebuggerHidden()]
	[MethodImpl(MethodImpl.ErrorPath)]
	internal static void PrintError<T>(this T exception, [CallerMemberName] string caller = null!) where T : Exception => Debug.Error(exception: exception, caller: caller);

	[MethodImpl(MethodImpl.ErrorPath)]
	internal static string BuildArgumentException(string name, in object? value) => $"'{name}' = '{((value is null) ? "null" : value.GetType().FullName)}'";
}
