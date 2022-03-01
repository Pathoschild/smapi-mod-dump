/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/CustomNPCExclusions
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework.Content;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;

namespace CustomNPCExclusions
{
    /// <summary>A Harmony patch that excludes designated NPCs from visiting <see cref="IslandSouth"/>.</summary>
    public static class HarmonyPatch_IslandVisit
    {
        public static void ApplyPatch(Harmony harmony, IModHelper helper)
        {
            //add SMAPI events
            helper.Events.GameLoop.DayStarted += DayStarted_SetupIslandSchedules;

            //apply Harmony patches
            ModEntry.Instance.Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_IslandVisit)}\": prefixing SDV method \"IslandSouth.SetupIslandSchedules()\".", LogLevel.Trace);
            harmony.Patch(
                original: AccessTools.Method(typeof(IslandSouth), nameof(IslandSouth.SetupIslandSchedules)),
                prefix: new HarmonyMethod(typeof(HarmonyPatch_IslandVisit), nameof(IslandSouth_SetupIslandSchedules))
            );

            ModEntry.Instance.Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_IslandVisit)}\": prefixing SDV method \"IslandSouth.CanVisitIslandToday(NPC)\".", LogLevel.Trace);
            harmony.Patch(
                original: AccessTools.Method(typeof(IslandSouth), nameof(IslandSouth.CanVisitIslandToday), new[] { typeof(NPC) }),
                prefix: new HarmonyMethod(typeof(HarmonyPatch_IslandVisit), nameof(IslandSouth_CanVisitIslandToday))
            );
        }

        /// <summary>A case-insensitive set of NPC names to exclude from island visit scheduling. If null, scheduling should be disabled.</summary>
        private static HashSet<string> ExcludedVisitors { get; set; } = null;

        /// <summary>Loads exclusion data and updates visitor schedules at the beginning of each day.</summary>
        /// <remarks>
        /// This method is meant to replace SDV's original calls to <see cref="IslandSouth.SetupIslandSchedules"/>, avoiding inconsistencies in exclusion data changes (e.g. token values in Content Patcher when sleeping vs loading).
        /// This is intended to run after Content Patcher updates its context for the day, which currently happens in a DayStarted event with normal priority.
        /// It may be necessary to modify this in the future; consider running one tick after DayStarted instead. Remember to test NPC behavior when modifying schedule timing.
        /// </remarks>
        [EventPriority(EventPriority.Low)] //use low priority to run after most asset updates
        private static void DayStarted_SetupIslandSchedules(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            if (!Context.IsMainPlayer) //if this is NOT the main player
                return; //do nothing

            ExcludedVisitors = new HashSet<string>(StringComparer.OrdinalIgnoreCase); //create a new case-insensitive set and enable scheduling

            foreach (KeyValuePair<string, List<string>> data in ModEntry.GetAllNPCExclusions(forceCacheUpdate: true)) //for each NPC's set of exclusion data
            {
                if (data.Value.Exists(entry =>
                    entry.StartsWith("All", StringComparison.OrdinalIgnoreCase) //if this NPC is excluded from everything
                 || entry.StartsWith("IslandEvent", StringComparison.OrdinalIgnoreCase) //OR if this NPC is excluded from island events
                 || entry.StartsWith("IslandVisit", StringComparison.OrdinalIgnoreCase) //OR if this NPC is excluded from visting the island resort
                ))
                {
                    ExcludedVisitors.Add(data.Key); //add this NPC's name to the excluded set
                }
            }

            if (ExcludedVisitors.Count > 0 && ModEntry.Instance.Monitor.IsVerbose) //if any NPCs were excluded
            {
                string logMessage = string.Join(", ", ExcludedVisitors);
                ModEntry.Instance.Monitor.Log($"Excluded NPCs from possible island visit: {logMessage}", LogLevel.Trace);
            }

            IslandSouth.SetupIslandSchedules(); //set up visitors' schedules

            ExcludedVisitors = null; //clear the set and disable scheduling
        }

        /// <summary>Skips <see cref="IslandSouth.SetupIslandSchedules"/> whenever this patch has NOT prepared NPC exclusion data.</summary>
        /// <remarks>
        /// This patch negates SDV's calls to the method while allowing it to be "manually" called by <see cref="DayStarted_SetupIslandSchedules"/>.
        /// See that method's remarks for more information.
        /// </remarks>
        [HarmonyPriority(Priority.High)] //use high priority to avoid duplicate calls to other prefixes on this method, if any
        private static bool IslandSouth_SetupIslandSchedules()
        {
            try
            {
                if (ExcludedVisitors == null) //if exclusion data is NOT currently prepared
                {
                    Game1.netWorldState.Value.IslandVisitors.Clear(); //clear island visitors as the method normally would (should minimize possible side effects)
                    ModEntry.Instance.Monitor.VerboseLog("Skipping IslandSouth.SetupIslandSchedules() call.");
                    return false; //skip the original method
                }
                else
                {
                    ModEntry.Instance.Monitor.VerboseLog("Allowing IslandSouth.SetupIslandSchedules() call.");
                    return true; //run the original method
                }
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.LogOnce($"Harmony patch \"{nameof(IslandSouth_SetupIslandSchedules)}\" has encountered an error and may revert to default behavior. Full error message:\n{ex.ToString()}", LogLevel.Error);
                return true; //run the original method
            }
        }

        /// <summary>A Harmony prefix patch that excludes designated NPCs from visiting <see cref="IslandSouth"/>.</summary>
        /// <param name="npc">The NPC being checked.</param>
        /// <param name="__result">The original method's return value. True if the NPC can visit the island today; false if they cannot.</param>
        private static bool IslandSouth_CanVisitIslandToday(NPC npc, ref bool __result)
        {
            try
            {
                if (ExcludedVisitors?.Contains(npc.Name) == true) //if this NPC is excluded from visiting the island today
                {
                    __result = false; //return false (prevent this NPC visiting the island)
                    return false; //skip the original method
                }
                else if (npc.isVillager()) //if this NPC is non-excluded villager
                {
                    try
                    {
                        _ = Game1.content.Load<Dictionary<string, string>>($"Characters\\Dialogue\\{npc.Name}"); //try to load this NPC's dialogue
                    }
                    catch (ContentLoadException) //if the dialogue asset does not exist
                    {
                        if (ModEntry.Instance.Monitor.IsVerbose)
                            ModEntry.Instance.Monitor.Log($"Excluding NPC due to missing dialogue data: {npc.Name}", LogLevel.Trace);

                        __result = false; //return false (prevent this NPC visiting the island)
                        return false; //skip the original method
                    }
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