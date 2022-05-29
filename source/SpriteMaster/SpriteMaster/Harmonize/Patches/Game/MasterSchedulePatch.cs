/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Configuration;
using StardewValley;
using System.Collections.Generic;
using System.Threading;

namespace SpriteMaster.Harmonize.Patches.Game;

using MasterSchedule = Dictionary<int, SchedulePathDescription>;

internal class MasterSchedulePatch {
	private static readonly ThreadLocal<HashSet<string?>> MasterScheduleSet = new();
	private static readonly ThreadLocal<int> MasterScheduleDepth = new();

	[Harmonize(
		typeof(NPC),
		"parseMasterSchedule",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		critical: false
	)]
	public static bool ParseMasterSchedulePre(NPC __instance, ref MasterSchedule? __result, string? rawData, ref bool __state) {
		__state = false;

		if (!Config.IsUnconditionallyEnabled || !Config.Extras.FixMasterSchedule) {
			return true;
		}

		// Optimization for mods like Custom NPC Fixes
		if (__instance is StardewValley.Monsters.Monster) {
			__result = null;
			return false;
		}

		if (!MasterScheduleSet.IsValueCreated) {
			MasterScheduleSet.Value = new() { rawData };
		}
		else if (!MasterScheduleSet.Value!.Add(rawData)) {
			__result = null;
			return false;
		}

		if (!MasterScheduleDepth.IsValueCreated) {
			MasterScheduleDepth.Value = 1;
		}
		else {
			++MasterScheduleDepth.Value;
		}

		__state = true;
		return true;
	}

	[Harmonize(
		typeof(NPC),
		"parseMasterSchedule",
		Harmonize.Fixation.Finalizer,
		Harmonize.PriorityLevel.Last,
		critical: false
	)]
	public static void ParseMasterSchedulePost(NPC __instance, string rawData, bool __state) {
		if (!Config.IsUnconditionallyEnabled || !Config.Extras.FixMasterSchedule) {
			return;
		}

		if (!__state) {
			return;
		}

		if (!MasterScheduleDepth.IsValueCreated) {
			MasterScheduleDepth.Value = 0;
		}
		else {
			--MasterScheduleDepth.Value;
		}

		if (MasterScheduleDepth.Value == 0 && MasterScheduleSet.IsValueCreated) {
			MasterScheduleSet.Value!.Clear();
		}
	}

	[Harmonize(
		typeof(NPC),
		"getSchedule",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		critical: false
	)]
	public static bool GetSchedulePre(NPC __instance, ref MasterSchedule? __result, int dayOfMonth) {
		if (!Config.IsUnconditionallyEnabled || !Config.Extras.FixMasterSchedule) {
			return true;
		}

		// Optimization for mods such as Custom NPC Fixes
		if (__instance is StardewValley.Monsters.Monster) {
			__result = null;
			return false;
		}

		return true;
	}
}
