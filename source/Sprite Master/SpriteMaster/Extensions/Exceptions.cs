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

namespace SpriteMaster.Extensions {
	public static class Exceptions {
		[DebuggerStepThrough, DebuggerHidden()]
		public static void PrintTrace<T> (this T exception, [CallerMemberName] string caller = null) where T : Exception {
			Debug.Trace(exception: exception, caller: caller);
		}

		[DebuggerStepThrough, DebuggerHidden()]
		public static void PrintInfo<T> (this T exception, [CallerMemberName] string caller = null) where T : Exception {
			Debug.Info(exception: exception, caller: caller);
		}

		[DebuggerStepThrough, DebuggerHidden()]
		public static void PrintWarning<T> (this T exception, [CallerMemberName] string caller = null) where T : Exception {
			Debug.Warning(exception: exception, caller: caller);
		}

		[DebuggerStepThrough, DebuggerHidden()]
		public static void PrintError<T> (this T exception, [CallerMemberName] string caller = null) where T : Exception {
			Debug.Error(exception: exception, caller: caller);
		}

		[DebuggerStepThrough, DebuggerHidden()]
		public static void Print<T> (this T exception, [CallerMemberName] string caller = null) where T : Exception {
			exception.PrintWarning(caller);
		}
	}
}
