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
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace CustomNPCExclusions
{
    /// <summary>A Harmony patch that excludes a list of NPCs from giving or receiving gifts during the Winter Star festival.</summary>
    public static class HarmonyPatch_WinterStarGifts
    {
        public static void ApplyPatch(HarmonyInstance harmony)
        {
            ModEntry.Instance.Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_WinterStarGifts)}\": transpiling SDV method \"Event.setUpPlayerControlSequence(string)\".", LogLevel.Trace);
            harmony.Patch(
                original: AccessTools.Method(typeof(Event), nameof(Event.setUpPlayerControlSequence), new[] { typeof(string) }),
                transpiler: new HarmonyMethod(typeof(HarmonyPatch_WinterStarGifts), nameof(Event_setUpPlayerControlSequence))
            );

            ModEntry.Instance.Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_WinterStarGifts)}\": transpiling SDV method \"LetterViewerMenu(string, string, bool)\".", LogLevel.Trace);
            harmony.Patch(
                original: AccessTools.Constructor(typeof(LetterViewerMenu), new[] { typeof(string), typeof(string), typeof(bool) }),
                transpiler: new HarmonyMethod(typeof(HarmonyPatch_WinterStarGifts), nameof(LetterViewerMenu_Constructor))
            );
        }

        /// <summary>Replaces calls to <see cref="Utility.getRandomTownNPC(Random)"/> with <see cref="GetRandomTownNPC_WinterStarExclusions(Random)"/>.</summary>
        public static IEnumerable<CodeInstruction> Event_setUpPlayerControlSequence(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                MethodInfo getOriginal = AccessTools.Method(typeof(Utility), nameof(Utility.getRandomTownNPC), new[] { typeof(Random) }); //get the original method's info
                MethodInfo getWithExclusions = AccessTools.Method(typeof(HarmonyPatch_WinterStarGifts), nameof(GetRandomTownNPC_WinterStarExclusions)); //get the new method's info

                List<CodeInstruction> patched = new List<CodeInstruction>(instructions); //make a copy of the instructions to modify

                for (int x = 0; x < patched.Count; x++) //for each instruction
                {
                    if (patched[x].opcode == OpCodes.Call //if this instruction is a method call
                        && (patched[x].operand as MethodInfo) == getOriginal) //AND this instruction is calling Utility.getRandomTownNPC
                    {
                        patched[x] = new CodeInstruction(OpCodes.Call, getWithExclusions); //replace it with a call to the exclusions method
                    }
                }

                return patched;
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_WinterStarGifts)}\" has encountered an error. Transpiler \"{nameof(Event_setUpPlayerControlSequence)}\" will not be applied. Full error message:\n{ex.ToString()}", LogLevel.Error);
                return instructions; //return the original instructions
            }
        }

        /// <summary>Replaces calls to <see cref="Utility.getRandomTownNPC(Random)"/> with <see cref="GetRandomTownNPC_WinterStarExclusions(Random)"/>.</summary>
        public static IEnumerable<CodeInstruction> LetterViewerMenu_Constructor(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                MethodInfo getOriginal = AccessTools.Method(typeof(Utility), nameof(Utility.getRandomTownNPC), new[] { typeof(Random) }); //get the original method's info
                MethodInfo getWithExclusions = AccessTools.Method(typeof(HarmonyPatch_WinterStarGifts), nameof(GetRandomTownNPC_WinterStarExclusions)); //get the new method's info

                List<CodeInstruction> patched = new List<CodeInstruction>(instructions); //make a copy of the instructions to modify

                for (int x = 0; x < patched.Count; x++) //for each instruction
                {
                    if (patched[x].opcode == OpCodes.Call //if this instruction is a method call
                        && (patched[x].operand as MethodInfo) == getOriginal) //AND this instruction is calling Utility.getRandomTownNPC
                    {
                        patched[x] = new CodeInstruction(OpCodes.Call, getWithExclusions); //replace it with a call to the exclusions method
                    }
                }

                return patched;
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_WinterStarGifts)}\" has encountered an error. Transpiler \"{nameof(LetterViewerMenu_Constructor)}\" will not be applied. Full error message:\n{ex.ToString()}", LogLevel.Error);
                return instructions; //return the original instructions
            }
        }

        /// <summary>Gets a random NPC from <see cref="Utility.getRandomTownNPC(Random)"/> while removing any NPCs who are excluded from Winter Star gifts.</summary>
        /// <param name="r">The random number generator to use.</param>
        /// <returns>An random NPC from the Town who is NOT excluded from Winter Star gifts.</returns>
        public static NPC GetRandomTownNPC_WinterStarExclusions(Random r)
        {
            Dictionary<string, List<string>> exclusions = ModEntry.GetAllNPCExclusions(); //get all exclusion data

            List<string> excluded = new List<string>(); //a list of NPC names to exclude from giving or receiving gifts

            foreach (KeyValuePair<string, List<string>> data in exclusions) //for each NPC's set of exclusion data
            {
                if (data.Value.Exists(entry =>
                    entry.StartsWith("All", StringComparison.OrdinalIgnoreCase) //if this NPC is excluded from everything
                 || entry.StartsWith("TownEvent", StringComparison.OrdinalIgnoreCase) //OR if this NPC is excluded from town events
                 || entry.StartsWith("WinterStar", StringComparison.OrdinalIgnoreCase) //OR this NPC is excluded from the Winter Star event
                ))
                {
                    excluded.Add(data.Key); //add this NPC's name to the excluded list
                }
            }

            HashSet<string> rerolledNames = new HashSet<string>(); //a record of NPCs excluded below

            NPC npc = Utility.getRandomTownNPC(r); //get a random NPC
            while (excluded.Contains(npc?.Name, StringComparer.OrdinalIgnoreCase)) //while the selected NPC is NOT in the excluded list
            {
                rerolledNames.Add(npc?.Name); //add NPC name to record
                npc = Utility.getRandomTownNPC(r); //get another random NPC
            }

            if (rerolledNames.Count > 0) //if any NPCs were excluded
            {
                string logMessage = String.Join(", ", rerolledNames);
                ModEntry.Instance.Monitor.Log($"Excluded NPCs from Winter Star gifts: {logMessage}", LogLevel.Trace);
            }

            return npc;
        }
    }
}