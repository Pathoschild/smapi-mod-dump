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
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Extensions;

static class Untraced {
	[DebuggerStepThrough, DebuggerHidden()]
	[MethodImpl(Runtime.MethodImpl.ErrorPath)]
	internal static bool IsUntraced(this MethodBase method) => method != null && (method.IsDefined(typeof(DebuggerStepThroughAttribute), true) || method.IsDefined(typeof(DebuggerHiddenAttribute), true));

	[DebuggerStepThrough, DebuggerHidden()]
	[MethodImpl(Runtime.MethodImpl.ErrorPath)]
	internal static string GetStackTrace(this Exception e) {
		var tracedStrings = new List<string>();
		foreach (var frame in new StackTrace(e, true).GetFrames()) {
			if (!frame.GetMethod().IsUntraced()) {
				tracedStrings.Add(new StackTrace(frame).ToString());
			}
		}

		return string.Concat(tracedStrings);
	}
}
