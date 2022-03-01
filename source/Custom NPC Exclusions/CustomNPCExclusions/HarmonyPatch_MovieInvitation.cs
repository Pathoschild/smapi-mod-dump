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
            ModEntry.Instance.Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_MovieInvitation)}\": transpiling SDV method \"NPC.tryToReceiveActiveObject(Farmer)\".", LogLevel.Trace);
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.tryToReceiveActiveObject), new[] { typeof(Farmer) }),
                transpiler: new HarmonyMethod(typeof(HarmonyPatch_MovieInvitation), nameof(NPC_tryToReceiveActiveObject))
            );
        }

        /// <summary>Inserts an exclusion check and dialogue generation method at the beginning of the "receive Movie Ticket" code section.</summary>
        /// <remarks>
        /// Old C#:
        ///     if ((int)who.ActiveObject.parentSheetIndex == 809 && !who.ActiveObject.bigCraftable)
        ///     {
        ///         if (!Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccMovieTheater"))
        ///
        /// New C#:
        ///     if ((int)who.ActiveObject.parentSheetIndex == 809 && !who.ActiveObject.bigCraftable)
        ///     {
        ///	        if (ExcludeNPCFromTheaterInvitation(who))
        ///		        return;
        ///	        if (!Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccMovieTheater"))
        ///		
        /// Old IL:
        ///     brtrue IL_0d18
        ///     ldstr "ccMovieTheater"
        ///     call bool StardewValley.Utility::doesMasterPlayerHaveMailReceivedButNotMailForTomorrow(string)
        ///     brtrue.s IL_0796
        /// 
        /// New IL:
        ///     brtrue IL_0d18
        ///     ldarg.0
        ///     call bool CustomNPCExclusions.HarmonyPatch_MovieInvitation::ExcludeNPCFromTheaterInvitation(NPC)
        ///     brfalse.s NEW_LABEL
        ///     ret
        ///     ldstr "ccMovieTheater" [NEW_LABEL]
        ///     call bool StardewValley.Utility::doesMasterPlayerHaveMailReceivedButNotMailForTomorrow(string)
        ///     brtrue.s IL_0796
        /// </remarks>
        public static IEnumerable<CodeInstruction> NPC_tryToReceiveActiveObject(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            try
            {
                MethodInfo getNearbyMethod = AccessTools.Method(typeof(Utility), nameof(Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow), new[] { typeof(string) }); //get the method used to position this transpiler's code edits
                MethodInfo getExclusionMethod = AccessTools.Method(typeof(HarmonyPatch_MovieInvitation), nameof(ExcludeNPCFromTheaterInvitation)); //get the exclusion method info

                List<CodeInstruction> patched = new List<CodeInstruction>(instructions); //make a copy of the instructions to modify

                for (int x = patched.Count - 1; x >= 2; x--) //for each instruction (looping backward, skipping the first 2)
                {
                    if (patched[x].opcode == OpCodes.Call && (patched[x].operand as MethodInfo) == getNearbyMethod //if this instruction checks the player's mail flags
                        && patched[x - 1].opcode == OpCodes.Ldstr && (patched[x - 1].operand as string) == "ccMovieTheater" //and the previous instruction loads the string "ccMovieTheater"
                        && patched[x - 2].opcode == OpCodes.Brtrue) //and the previous instruction is "break if true"
                    {
                        Label goHereIfNotExcluded = generator.DefineLabel();
                        patched[x - 1].labels.Add(goHereIfNotExcluded); //add the label to the first original instruction following these patched instructions 

                        patched.InsertRange(x - 1, new[] //add these instructions at the beginning of the movie ticket logic:
                        {
                            new CodeInstruction(OpCodes.Ldarg_0), //load the NPC
                            new CodeInstruction(OpCodes.Call, getExclusionMethod), //call the exclusion check, which will use the NPC and return a bool
                            new CodeInstruction(OpCodes.Brfalse, goHereIfNotExcluded), //if NOT excluded, break (i.e. go to the original logic)
                            new CodeInstruction(OpCodes.Ret) //if excluded, return
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
        /// <returns>True if this NPC was excluded.</returns>
        public static bool ExcludeNPCFromTheaterInvitation(NPC npc)
        {
            try
            {
                List<string> exclusions = ModEntry.GetNPCExclusions(npc.Name); //get this NPC's exclusion data

                foreach (string exclusion in exclusions) //for each of this NPC's exclusion settings
                {
                    if (exclusion.StartsWith("All", StringComparison.OrdinalIgnoreCase) //if this NPC is excluded from everything
                     || exclusion.StartsWith("TownEvent", StringComparison.OrdinalIgnoreCase) //OR if this NPC is excluded from town events
                     || exclusion.StartsWith("MovieInvite", StringComparison.OrdinalIgnoreCase)) //OR if this NPC is excluded from movie invitations
                    {
                        DrawMovieExclusionDialogue(npc); //generate exclusion dialogue for this NPC
                        if (ModEntry.Instance.Monitor.IsVerbose)
                            ModEntry.Instance.Monitor.Log($"Excluded NPC from being invited to a movie: {npc.Name}", LogLevel.Trace);
                        return true; //this NPC was excluded
                    }
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