/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/CustomNPCExclusions
**
*************************************************/

using Harmony;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Quests;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomNPCExclusions
{
    /// <summary>A Harmony patch that excludes designated NPCs from visiting <see cref="IslandSouth"/>.</summary>
    public static class HarmonyPatch_IslandVisit
    {
        public static void ApplyPatch(HarmonyInstance harmony)
        {
            ModEntry.Instance.Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_IslandVisit)}\": prefixing SDV method \"IslandSouth.CanVisitIslandToday(NPC)\".", LogLevel.Trace);
            harmony.Patch(
                original: AccessTools.Method(typeof(IslandSouth), nameof(IslandSouth.CanVisitIslandToday), new[] { typeof(NPC) }),
                prefix: new HarmonyMethod(typeof(HarmonyPatch_IslandVisit), nameof(IslandSouth_CanVisitIslandToday))
            );
        }

        /// <summary>A Harmony prefix patch that excludes designated NPCs from visiting <see cref="IslandSouth"/>.</summary>
        /// <param name="npc">The NPC being checked.</param>
        /// <param name="__result">The original method's return value. True if the NPC can visit the island today; false if they cannot.</param>
        public static bool IslandSouth_CanVisitIslandToday(NPC npc, ref bool __result)
        {
            try
            {
                List<string> exclusions = ModEntry.GetNPCExclusions(npc.Name); //get exclusion data for the provided NPC

                if (exclusions.Exists(entry =>
                    entry.StartsWith("All", StringComparison.OrdinalIgnoreCase) //if this NPC is excluded from everything
                    || entry.StartsWith("IslandEvent", StringComparison.OrdinalIgnoreCase) //if this NPC is excluded from island events
                    || entry.StartsWith("IslandVisit", StringComparison.OrdinalIgnoreCase) //OR if this NPC is excluded from visting the island resort
                ))
                {
                    ModEntry.Instance.Monitor.Log($"Excluded NPC from possible island visit: {npc.Name}", LogLevel.Trace);
                    __result = false; //return false (prevent this NPC visiting the island today)
                    return false; //skip the original method
                }

                return true; //run the original method
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.LogOnce($"Harmony patch \"{nameof(IslandSouth_CanVisitIslandToday)}\" has encountered an error and may revert to default behavior. Full error message:\n{ex.ToString()}", LogLevel.Error);
                return true; //run the original method
            }
        }


    }
}