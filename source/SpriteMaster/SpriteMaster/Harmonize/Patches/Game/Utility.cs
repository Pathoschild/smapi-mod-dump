/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using Microsoft.Toolkit.HighPerformance;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SpriteMaster.Harmonize.Patches.Game;

internal static class Utility {

	[Harmonize(
		typeof(StardewValley.Utility),
		"removeDuplicates",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		instance: false,
		critical: false
	)]
	public static bool RemoveDuplicates(ref List<XVector2> __result, List<XVector2> list) {
		var span = list.AsSpan();
		ref var listRef = ref MemoryMarshal.GetReference(span);

		for (int i = 0; i < list.Count; i++) {
			var current = Unsafe.Add(ref listRef, i);
			for (int j = list.Count - 1; j > i; j--) {
				if (current.Equals(Unsafe.Add(ref listRef, j))) {
					list.RemoveAt(j);
				}
			}
		}

		__result = list;

		return false;
	}
}
