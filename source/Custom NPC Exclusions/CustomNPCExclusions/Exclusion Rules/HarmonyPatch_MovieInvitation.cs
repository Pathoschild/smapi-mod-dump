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
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace CustomNPCExclusions
{
    /// <summary>A Harmony patch that excludes designated NPCs from being invited to the movie theater.</summary>
    public static class HarmonyPatch_MovieInvitation
    {
        public static void ApplyPatch(Harmony harmony)
        {
            ModEntry.Instance.Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_MovieInvitation)}\": transpiling SDV method \"NPC.tryToReceiveActiveObject(Farmer, bool)\".", LogLevel.Trace);
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.tryToReceiveActiveObject), new[] { typeof(Farmer), typeof(bool) }),
                transpiler: new HarmonyMethod(typeof(HarmonyPatch_MovieInvitation), nameof(NPC_tryToReceiveActiveObject))
            );
        }

        /// <summary>Inserts an exclusion check and dialogue generation method at the beginning of the "receive Movie Ticket" code section.</summary>
        /// <remarks>
        /// Old C#:
        ///     case "(O)809":
        ///     {
        ///         if (!Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccMovieTheater"))
        ///
        /// New C#:
        ///     case "(O)809":
        ///     {
        ///	        if (ExcludeNPCFromTheaterInvitation(this, probe))
        ///		        return;
        ///	        if (!Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccMovieTheater"))
        ///		
        /// Old IL:
        ///     br IL_12b4
        ///     ldstr "ccMovieTheater" [OLD_LABEL]
        ///     call bool StardewValley.Utility::doesMasterPlayerHaveMailReceivedButNotMailForTomorrow(string)
        ///     brtrue.s IL_06f5
        /// 
        /// New IL:
        ///     br IL_12b4
        ///     ldarg.0 [OLD_LABEL]
        ///     ldarg.2
        ///     call bool CustomNPCExclusions.HarmonyPatch_MovieInvitation::ExcludeNPCFromTheaterInvitation(NPC, bool)
        ///     brfalse NEW_LABEL
        ///     ldc.i4.1
        ///     ret
        ///     ldstr "ccMovieTheater" [NEW_LABEL]
        ///     call bool StardewValley.Utility::doesMasterPlayerHaveMailReceivedButNotMailForTomorrow(string)
        ///     brtrue.s IL_06f5
        /// </remarks>
        public static IEnumerable<CodeInstruction> NPC_tryToReceiveActiveObject(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            try
            {
                MethodInfo getNearbyMethod = AccessTools.Method(typeof(Utility), nameof(Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow), new[] { typeof(string) }); //get the method used to position this transpiler's code edits
                MethodInfo getExclusionMethod = AccessTools.Method(typeof(HarmonyPatch_MovieInvitation), nameof(ExcludeNPCFromTheaterInvitation)); //get the exclusion method info

                List<CodeInstruction> patched = new List<CodeInstruction>(instructions); //make a copy of the instructions to modify

                for (int x = patched.Count - 3; x >= 0; x--) //for each instruction (looping backward, skipping the last 2 instructions)
                {
                    if (patched[x].opcode == OpCodes.Ldstr && (patched[x].operand as string) == "ccMovieTheater" //if this instruction loads the string "ccMovieTheater"
                        && patched[x + 1].opcode == OpCodes.Call && (patched[x + 1].operand as MethodInfo) == getNearbyMethod //and the next instruction checks the player's mail flags
                        && patched[x + 2].opcode == OpCodes.Brtrue_S) //and the next instruction is "break if true" (short form)
                    {
                        CodeInstruction firstNewInstruction = new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(patched[x]); //create the first new instruction (see below) and take the original instruction's labels (making it the new entry point)

                        Label goHereIfNotExcluded = generator.DefineLabel(); //create a new label
                        patched[x].labels.Add(goHereIfNotExcluded); //add it to the first original instruction (ldstr "ccMovieTheater")

                        patched.InsertRange(x, new[] //add these new instructions before the original instructions
                        {
                            firstNewInstruction, //load this NPC instance
                            new CodeInstruction(OpCodes.Ldarg_2), //load the "bool probe" argument
                            new CodeInstruction(OpCodes.Call, getExclusionMethod), //call the exclusion check, which will take "NPC, bool" and return a bool
                            new CodeInstruction(OpCodes.Brfalse, goHereIfNotExcluded), //if NOT excluded, break (go to the original instructions, skipping the lines below)
                            new CodeInstruction(OpCodes.Ldc_I4_1), //load 1 (i.e. true)
                            new CodeInstruction(OpCodes.Ret) //return (true)
                        });
                    }
                }

                return patched; //return the patched instructions
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_MovieInvitation)}\" has encountered an error. Transpiler \"{nameof(NPC_tryToReceiveActiveObject)}\" will not be applied. Full error message:\n{ex.ToString()}", LogLevel.Error);
                return instructions; //return the original instructions
            }
        }

        /// <summary>Prevents this NPC from being invited to the movie theater, depending on their exclusion data.</summary>
        /// <param name="npc">The NPC being invited.</param>
        /// <param name="probe">If true, this is just a hypothetical test that shouldn't modify the NPC. If false, the NPC is actually receiving a movie ticket.</param>
        /// <returns>True if this NPC was excluded.</returns>
        public static bool ExcludeNPCFromTheaterInvitation(NPC npc, bool probe)
        {
            try
            {
                if (DataHelper.GetNPCsWithExclusions("All", "TownEvent", "MovieInvite").Contains(npc.Name)) //if this NPC has the MovieInvite exclusion (or applicable categories)
                {
                    if (!probe) //if the NPC is really being offered a movie ticket
                    {
                        DrawMovieExclusionDialogue(npc); //generate exclusion dialogue for this NPC
                        if (ModEntry.Instance.Monitor.IsVerbose)
                            ModEntry.Instance.Monitor.Log($"Excluded NPC from being invited to a movie: {npc.Name})", LogLevel.Trace);
                    }

                    return true; //this NPC was excluded
                }
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_MovieInvitation)}\" has encountered an error. Method \"{nameof(ExcludeNPCFromTheaterInvitation)}\" might revert to default behavior. Full error message:\n{ex.ToString()}", LogLevel.Error);
            }

            return false; //this NPC was NOT excluded
        }

        /// <summary>Loads, parses, and draws the "can't be invited" dialogue for an NPC being excluded from a movie invitiation.</summary>
        /// <param name="npc">The NPC being excluded.</param>
        public static void DrawMovieExclusionDialogue(NPC npc)
        {
            string defaultKey = "Strings\\Characters:MovieInvite_CantInvite";
            string customKey = $"{defaultKey}_{npc.Name}";

            string dialogue = Game1.content.LoadString(customKey, npc.displayName); //try to load the custom key's dialogue (and replace "{0}" with the NPC's display name)
            if (dialogue.Equals(customKey)) //if custom dialogue was not found (i.e. the key itself was returned)
                dialogue = Game1.content.LoadString(defaultKey, npc.displayName); //load the default key's dialogue

            Game1.drawObjectDialogue(Game1.parseText(dialogue)); //parse and draw the NPC's dialogue (based on the original method's behavior)
        }
    }
}