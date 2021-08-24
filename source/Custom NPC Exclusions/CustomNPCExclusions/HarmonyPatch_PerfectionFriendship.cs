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
using StardewValley.Locations;
using StardewValley.Quests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace CustomNPCExclusions
{
    /// <summary>A Harmony patch that excludes designated NPCs from the perfection system's friendship percentage.</summary>
    public static class HarmonyPatch_PerfectionFriendship
    {
        public static void ApplyPatch(Harmony harmony)
        {
            ModEntry.Instance.Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_PerfectionFriendship)}\": transpiling SDV method \"Utility.getMaxedFriendshipPercent(Farmer)\".", LogLevel.Trace);
            harmony.Patch(
                original: AccessTools.Method(typeof(Utility), nameof(Utility.getMaxedFriendshipPercent), new[] { typeof(Farmer) }),
                transpiler: new HarmonyMethod(typeof(HarmonyPatch_PerfectionFriendship), nameof(Utility_getMaxedFriendshipPercent))
            );
        }

        /// <summary>Adds a method call to exclude NPCs after "NPCDispositions" is loaded.</summary>
        public static IEnumerable<CodeInstruction> Utility_getMaxedFriendshipPercent(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                MethodInfo getExclusionMethod = AccessTools.Method(typeof(HarmonyPatch_PerfectionFriendship), nameof(ExcludeFromNPCDispositions)); //get the exclusion method info

                List<CodeInstruction> patched = new List<CodeInstruction>(instructions); //make a copy of the instructions to modify

                for (int x = patched.Count - 1; x > 0; x--) //for each instruction (starting at index 1)
                {
                    if (patched[x].opcode == OpCodes.Callvirt //if this instruction is a virtual method call
                        && patched[x - 1].opcode == OpCodes.Ldstr //AND the previous instruction loaded a string
                        && (patched[x - 1].operand as string) == "Data\\NPCDispositions") //AND the previous instruction's string refers to the NPCDispositions asset
                    {
                        patched.Insert(x + 1, new CodeInstruction(OpCodes.Call, getExclusionMethod)); //add a call to the exclusion method after this instruction
                    }
                }

                return patched; //return the patched instructions
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_PerfectionFriendship)}\" has encountered an error. Transpiler \"{nameof(Utility_getMaxedFriendshipPercent)}\" will not be applied. Full error message:\n{ex.ToString()}", LogLevel.Error);
                return instructions; //return the original instructions
            }
        }

        /// <summary>Removes all entries from a dictionary where the key equals an excluded NPC's name.</summary>
        /// <param name="dispositions">A dictionary of data, generally loaded from Stardew's "Data/NPCDispositions" asset.</param>
        /// <returns>A copy of the dictionary with any excluded NPCs' entries removed.</returns>
        public static Dictionary<string, string> ExcludeFromNPCDispositions(Dictionary<string, string> dispositions)
        {
            if (dispositions == null) //if NPCDispositions is null for some reason
                return dispositions; //return the original asset

            try
            {
                Dictionary<string, string> dispositionsWithExclusions = new Dictionary<string, string>(dispositions); //copy the dispositions to avoid editing the game's cached version

                List<string> excluded = new List<string>(); //a list of NPC names to exclude from the dispositions

                foreach (KeyValuePair<string, List<string>> data in ModEntry.GetAllNPCExclusions()) //for each NPC's set of exclusion data
                {
                    if (data.Value.Exists(entry =>
                        entry.StartsWith("All", StringComparison.OrdinalIgnoreCase) //if this NPC is excluded from everything
                     || entry.StartsWith("OtherEvent", StringComparison.OrdinalIgnoreCase) //OR if this NPC is excluded from other events
                     || entry.StartsWith("PerfectFriend", StringComparison.OrdinalIgnoreCase) //OR if this NPC is excluded from the perfection system's friendship percentage
                    ))
                    {
                        excluded.Add(data.Key); //add this NPC's name to the excluded list
                    }
                }

                for (int x = excluded.Count - 1; x >= 0; x--) //for each excluded NPC (looping backward to allow removal)
                {
                    string matchingDispositionsKey = dispositionsWithExclusions.Keys.FirstOrDefault(key => key.Equals(excluded[x], StringComparison.OrdinalIgnoreCase)); //search for a dispositions key that matches the excluded NPC's name

                    if (matchingDispositionsKey != null) //if a key was found for this NPC
                    {
                        dispositionsWithExclusions.Remove(matchingDispositionsKey); //remove it from the dispositions
                    }
                    else //if a key was NOT found for this NPC
                    {
                        excluded.RemoveAt(x); //remove the excluded NPC from the list (for use in log messages)
                    }
                }

                if (excluded.Count > 0) //if any NPCs were excluded
                {
                    string logMessage = string.Join(", ", excluded);
                    ModEntry.Instance.Monitor.Log($"Excluded NPCs from perfect friendship tracking: {logMessage}", LogLevel.Trace);
                }

                return dispositionsWithExclusions; //return the filtered dispositions
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_PerfectionFriendship)}\" has encountered an error. Method \"{nameof(ExcludeFromNPCDispositions)}\" might not remove excluded NPCs. Full error message:\n{ex.ToString()}", LogLevel.Error);
                return dispositions; //return the original asset
            }
        }
    }
}