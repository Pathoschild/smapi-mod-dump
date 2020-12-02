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
    /// <summary>A Harmony patch that excludes a list of NPCs from random town-related shop dialog.</summary>
    public static class HarmonyPatch_ShopDialog
    {
        public static void ApplyPatch(HarmonyInstance harmony)
        {
            ModEntry.Instance.Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_ShopDialog)}\": transpiling SDV method \"Game1.UpdateShopPlayerItemInventory(string, HashSet<NPC>)\".", LogLevel.Trace);
            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.UpdateShopPlayerItemInventory), new[] { typeof(string), typeof(HashSet<NPC>) }),
                transpiler: new HarmonyMethod(typeof(HarmonyPatch_ShopDialog), nameof(Game1_UpdateShopPlayerItemInventory))
            );
        }

        /// <summary>Replaces calls to <see cref="Utility.getRandomTownNPC()"/> with <see cref="GetRandomTownNPC_WinterStarExclusions()"/>.</summary>
        public static IEnumerable<CodeInstruction> Game1_UpdateShopPlayerItemInventory(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                MethodInfo getOriginal = AccessTools.Method(typeof(Utility), nameof(Utility.getRandomTownNPC)); //get the original method's info
                MethodInfo getWithExclusions = AccessTools.Method(typeof(HarmonyPatch_ShopDialog), nameof(GetRandomTownNPC_ShopDialogExclusions)); //get the new method's info

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
                ModEntry.Instance.Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_WinterStarGifts)}\" has encountered an error. Transpiler \"{nameof(Game1_UpdateShopPlayerItemInventory)}\" will not be applied. Full error message:\n{ex.ToString()}", LogLevel.Error);
                return instructions; //return the original instructions
            }
        }

        /// <summary>Gets a random NPC from <see cref="Utility.getRandomTownNPC(Random)"/> while removing any NPCs who are excluded from random town-related shop dialog.</summary>
        /// <returns>An random NPC from the Town who is NOT excluded from random town-related shop dialog.</returns>
        public static NPC GetRandomTownNPC_ShopDialogExclusions()
        {
            List<string> excluded = new List<string>(); //a list of NPC names to exclude from giving or receiving gifts

            foreach (KeyValuePair<string, List<string>> data in ModEntry.GetAllNPCExclusions()) //for each NPC's set of exclusion data
            {
                if (data.Value.Exists(entry =>
                    entry.StartsWith("All", StringComparison.OrdinalIgnoreCase) //if this NPC is excluded from everything
                    || entry.StartsWith("Event", StringComparison.OrdinalIgnoreCase) //if this NPC is excluded from events
                    || entry.StartsWith("TownEvent", StringComparison.OrdinalIgnoreCase) //OR if this NPC is excluded from town events
                    || entry.StartsWith("ShopDialog", StringComparison.OrdinalIgnoreCase) //OR this NPC is excluded from the Winter Star event
                ))
                {
                    excluded.Add(data.Key); //add this NPC's name to the excluded list
                }
            }

            HashSet<string> rerolledNames = new HashSet<string>(); //a record of NPCs excluded below

            NPC npc = Utility.getRandomTownNPC(); //get a random NPC
            while (excluded.Contains(npc?.Name, StringComparer.OrdinalIgnoreCase)) //while the selected NPC is NOT in the excluded list
            {
                rerolledNames.Add(npc?.Name); //add NPC name to record
                npc = Utility.getRandomTownNPC(); //get another random NPC
            }

            if (rerolledNames.Count > 0) //if any NPCs were excluded
            {
                string logMessage = String.Join(", ", rerolledNames);
                ModEntry.Instance.Monitor.Log($"Excluded NPCs from random shop dialog: {logMessage}", LogLevel.Trace);
            }

            return npc;
        }
    }
}