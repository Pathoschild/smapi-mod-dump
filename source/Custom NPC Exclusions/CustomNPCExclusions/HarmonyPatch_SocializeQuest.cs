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
using StardewValley.Quests;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomNPCExclusions
{
    /// <summary>A Harmony patch that excludes a list of NPCs from <see cref="SocializeQuest"/>.</summary>
    public static class HarmonyPatch_SocializeQuest
    {
        public static void ApplyPatch(HarmonyInstance harmony)
        {
            ModEntry.Instance.Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_SocializeQuest)}\": postfixing SDV method \"SocializeQuest.loadQuestInfo()\".", LogLevel.Trace);
            harmony.Patch(
                original: AccessTools.Method(typeof(SocializeQuest), nameof(SocializeQuest.loadQuestInfo)),
                postfix: new HarmonyMethod(typeof(HarmonyPatch_SocializeQuest), nameof(SocializeQuest_loadQuestInfo))
            );
        }

        /// <summary>A Harmony postfix patch that excludes a list of NPCs from <see cref="SocializeQuest"/>.</summary>
        /// <param name="__instance">This instance of <see cref="SocializeQuest"/>.</param>
        public static void SocializeQuest_loadQuestInfo(SocializeQuest __instance)
        {
            try
            {
                List<string> excluded = new List<string>(); //a record of NPCs excluded during this process

                Dictionary<string, List<string>> exclusions = ModEntry.GetAllNPCExclusions(); //get all exclusion data

                for (int x = __instance.whoToGreet.Count - 1; x >= 0; x--) //for each NPC name selected by the original method (looping backward to allow removal)
                {
                    if (exclusions.ContainsKey(__instance.whoToGreet[x])) //if this NPC has exclusion data
                    {
                        if (exclusions[__instance.whoToGreet[x]].Exists(entry =>
                            entry.StartsWith("All", StringComparison.OrdinalIgnoreCase) //if this NPC is excluded from everything
                         || entry.StartsWith("TownQuest", StringComparison.OrdinalIgnoreCase) //OR if this NPC is excluded from town quests
                         || entry.StartsWith("Socialize", StringComparison.OrdinalIgnoreCase) //OR if this NPC is excluded from socialize quests
                        ))
                        {
                            excluded.Add(__instance.whoToGreet[x]); //add this NPC to the record
                            __instance.whoToGreet.RemoveAt(x); //remove this NPC from the greeting list
                        }
                    }
                }

                if (excluded.Count > 0) //if any NPCs were excluded
                {
                    __instance.total.Value -= excluded.Count; //subtract the removed NPCs from the quest's total
                    __instance.objective.Value.param[1] = __instance.total.Value; //update the displayed total

                    ModEntry.Instance.Monitor.Log($"Excluded NPCs from socialize quest: {String.Join(", ", excluded)}", LogLevel.Trace);
                }
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.LogOnce($"Harmony patch \"{nameof(SocializeQuest_loadQuestInfo)}\" has encountered an error and may revert to default behavior. Full error message:\n{ex.ToString()}", LogLevel.Error);
            }
        }


    }
}