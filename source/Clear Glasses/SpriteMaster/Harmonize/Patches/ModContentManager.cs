/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System.IO;
using static SpriteMaster.Harmonize.Harmonize;

namespace SpriteMaster.Harmonize.Patches;

internal static class ModContentManager {
	[HarmonizeSmapiVersionConditional(Comparator.GreaterThanOrEqual, "3.15.0")]
	[Harmonize(
		"StardewModdingAPI.Framework.ContentManagers.ModContentManager",
		"LoadRawImageData",
		Fixation.Prefix,
		PriorityLevel.Last,
		instance: true
	)]
	public static bool OnLoadRawImageData(
		LocalizedContentManager __instance, ref IRawTextureData __result, FileInfo file, bool forRawData
	) {
		return Caching.TextureFileCache.OnLoadRawImageData(__instance, ref __result, file, forRawData);
	}
}