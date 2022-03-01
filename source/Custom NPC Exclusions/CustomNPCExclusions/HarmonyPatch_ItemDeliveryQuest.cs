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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Quests;
using System;
using System.Collections.Generic;

namespace CustomNPCExclusions
{
    /// <summary>A Harmony patch that excludes a list of NPCs from <see cref="ItemDeliveryQuest"/>.</summary>
    public static class HarmonyPatch_ItemDeliveryQuest
    {
        public static void ApplyPatch(Harmony harmony)
        {
            ModEntry.Instance.Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_ItemDeliveryQuest)}\": postfixing SDV method \"ItemDeliveryQuest.GetValidTargetList()\".", LogLevel.Trace);
            harmony.Patch(
                original: AccessTools.Method(typeof(ItemDeliveryQuest), nameof(ItemDeliveryQuest.GetValidTargetList)),
                postfix: new HarmonyMethod(typeof(HarmonyPatch_ItemDeliveryQuest), nameof(ItemDeliveryQuest_GetValidTargetList))
            );
        }

        /// <summary>A Harmony postfix patch that excludes a list of NPCs from <see cref="ItemDeliveryQuest"/>.</summary>
        /// <param name="__result">A list of NPCs this quest type could target.</param>
        public static void ItemDeliveryQuest_GetValidTargetList(ref List<NPC> __result)
        {
            try
            {
                List<string> excluded = new List<string>(); //a record of NPCs excluded during this process

                Dictionary<string, List<string>> exclusions = ModEntry.GetAllNPCExclusions(); //get all exclusion data

                for (int x = __result.Count - 1; x >= 0; x--) //for each valid NPC returned by the original method (looping backward to allow removal)
                {
                    if (exclusions.ContainsKey(__result[x].Name)) //if this NPC has exclusion data
                    {
                        if (exclusions[__result[x].Name].Exists(entry =>
                            entry.StartsWith("All", StringComparison.OrdinalIgnoreCase) //if this NPC is excluded from everything
                         || entry.StartsWith("TownQuest", StringComparison.OrdinalIgnoreCase) //OR if this NPC is excluded from town quests
                         || entry.StartsWith("ItemDelivery", StringComparison.OrdinalIgnoreCase) //OR if this NPC is excluded from item delivery quests
                        ))
                        {
                            excluded.Add(__result[x].Name); //add this NPC to the record
                            __result.RemoveAt(x); //remove this NPC from the original results
                        }
                    }
                }

                if (excluded.Count > 0 && ModEntry.Instance.Monitor.IsVerbose) //if any NPCs were excluded
                {
                    ModEntry.Instance.Monitor.Log($"Excluded NPCs from item delivery quest: {String.Join(", ", excluded)}", LogLevel.Trace);
                }
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.LogOnce($"Harmony patch \"{nameof(ItemDeliveryQuest_GetValidTargetList)}\" has encountered an error and may revert to default behavior. Full error message:\n{ex.ToString()}", LogLevel.Error);
            }
        }


    }
}