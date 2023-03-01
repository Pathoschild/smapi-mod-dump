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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpriteMaster.Harmonize.Patches.TMX;

/*
internal static class TMXFormat {

	private static readonly ConcurrentDictionary<object, object> MapCache = new();

	[Harmonize(
		"TMXTile.TMXFormat",
		"Load",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last
	)]
	public static bool LoadPre(object __instance, ref object __result, object tmxMap) {
		if (!MapCache.TryGetValue(tmxMap, out var cachedResult)) {
			return true;
		}

		__result = cachedResult;
		return false;

	}

	[Harmonize(
		"TMXTile.TMXFormat",
		"Load",
		Harmonize.Fixation.Postfix,
		Harmonize.PriorityLevel.Last
	)]
	public static void LoadPost(object __instance, object __result, object tmxMap) {
		_ = MapCache.TryAdd(tmxMap, __result);
	}

}
*/
