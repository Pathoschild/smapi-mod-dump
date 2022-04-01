/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

#if !SHIPPING

using StardewModdingAPI;

namespace SpriteMaster.Harmonize.Patches.SMAPI;

static class ShutUp {
	[Harmonize(
		typeof(StardewModdingAPI.Framework.ModLoading.RewriteFacades.AccessToolsFacade),
		"StardewModdingAPI.Framework.Monitor",
		"LogImpl",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		critical: false
	)]
	public static bool LogImplPre(IMonitor __instance, string? source, string? message, object level) {
		if ((int)level != (int)LogLevel.Trace) {
			return true;
		}

		switch (source) {
			case "Farm Type Manager":
			case "Quest Framework":
			case "AntiSocial NPCs":
			case "SMAPI":
			case "Json Assets":
			case "Content Patcher":
				return false;
			default:
				return true;
		}
	}
}

#endif
