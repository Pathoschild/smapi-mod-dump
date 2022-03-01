/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SpriteMaster.Harmonize.Patches.Mods.CustomNPCFixes;

static class PCustomNPCFixes {
	[MethodImpl(Runtime.MethodImpl.Hot)]
	[Harmonize("CustomNPCFixes", "CustomNPCFixes.Mod", "FixSchedules", fixation: Harmonize.Fixation.Prefix, priority: Harmonize.PriorityLevel.Last, critical: false)]
	internal static bool FixSchedules(StardewModdingAPI.Mod __instance) {
		if (!Config.IsEnabled || !Config.Extras.ModPatches.PatchCustomNPCFixes) {
			return true;
		}

		List<NPC> allCharacters = Utility.getAllCharacters(new())!;
		var processedSet = new ConcurrentDictionary<NPC, byte>();

		Parallel.ForEach(allCharacters, npc => {
			if (npc is null) {
				return;
			}
			if (npc.Schedule is not null) {
				return;
			}

			if (!processedSet.TryAdd(npc, 0)) {
				return;
			}

			try {
				var schedule = npc.getSchedule(Game1.dayOfMonth);
				npc.Schedule = schedule;
				npc.checkSchedule(Game1.timeOfDay);
			}
			catch (Exception ex) {
				Debug.Warning($"CustomNPCFixes Override: Exception processing schedule for NPC '{npc.Name}'", ex);
			}
		});

		return false;
	}
}
