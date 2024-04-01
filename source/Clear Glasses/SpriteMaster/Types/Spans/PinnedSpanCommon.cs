/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Types.Spans;

internal static class PinnedSpanCommon {
	[Conditional("DEBUG"), Conditional("DEVELOPMENT"), Conditional("RELEASE"), MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void CheckPinnedWeak(object obj) {
		//var header = obj.GetHeader();

		//GC.GetGeneration(obj).AssertEqual(2);
	}
}
