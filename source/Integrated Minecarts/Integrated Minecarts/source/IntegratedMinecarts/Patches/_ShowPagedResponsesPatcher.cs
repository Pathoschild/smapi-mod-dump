/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Jibblestein/StardewMods
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
//Currently Unused
namespace IntegratedMinecarts.Patches
{
    internal class _ShowPagedResponsesPatcher : GameLocation
    {
        public static IMonitor? Monitor;
        // call this method from your Entry class
        public static void Patch(ModEntry mod)
        {
            Monitor = mod.Monitor;

            try
            {
                mod.Harmony!.Patch(
                   original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.ShowPagedResponses)),
                   transpiler: new HarmonyMethod(typeof(_ShowPagedResponsesPatcher), nameof(ShowPagedResponses_Transpiler))
                    );
                mod.Harmony!.Patch(
                   original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.answerDialogueAction)),
                   transpiler: new HarmonyMethod(typeof(_ShowPagedResponsesPatcher), nameof(answerDialogueAction_Transpiler))
                    );
            }
            catch (Exception ex)
            {
                Monitor.Log($"An error occurred while registering a harmony patch for ShowPagedResponses.\n{ex}", LogLevel.Warn);
            }
        }
        private static IEnumerable<CodeInstruction> ShowPagedResponses_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var patchedmethod = AccessTools.Method(typeof(_ShowPagedResponsesPatcher), nameof(Patched_ShowPagedResponses));
            var foundCall_ShowPagedResponses = false;
            var codes = new List<CodeInstruction>(instructions);
            var startIndex = -1;
            MethodInfo calledoperand;
            //find _ShowPagedResponses
            for (int i = 0; i < codes.Count; i++)
            {
                if (!foundCall_ShowPagedResponses && codes[i].opcode == OpCodes.Callvirt)
                {
                    calledoperand = codes[i].operand as MethodInfo;
                    if (calledoperand.Name == "_ShowPagedResponses")
                    {
                        foundCall_ShowPagedResponses = true;
                        startIndex = i;
//                        Monitor.Log($"Found _ShowPagedResponses at Code {i} calling {calledoperand.Name}", LogLevel.Warn);
                        break;
                    }
                }
            }
            //If we have found the code range
            if (startIndex > -1)
            {
                //Set our method
                codes[startIndex].opcode = OpCodes.Call;
                codes[startIndex].operand = patchedmethod;
                Monitor.Log($"Successfully changed _ShowPagedResponses method for {patchedmethod} at Code {startIndex}", LogLevel.Trace);
            }
            return codes.AsEnumerable();

        }
        private static IEnumerable<CodeInstruction> answerDialogueAction_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var patchedmethod = AccessTools.Method(typeof(_ShowPagedResponsesPatcher), nameof(Patched_ShowPagedResponses));
            var foundCall_ShowPagedResponses1 = false;
            var foundCall_ShowPagedResponses2 = false;
            var codes = new List<CodeInstruction>(instructions);
            var startIndex1 = -1;
            var startIndex2 = -1;
            MethodInfo calledoperand;
            //find _ShowPagedResponses1
            for (int i = 0; i < codes.Count; i++)
            {
                if (!foundCall_ShowPagedResponses1 && codes[i].opcode == OpCodes.Callvirt)
                {
                    calledoperand = codes[i].operand as MethodInfo;
                    if (calledoperand.Name == "_ShowPagedResponses")
                    {
                        foundCall_ShowPagedResponses1 = true;
                        startIndex1 = i;
                        startIndex2 = i + 1;
//                        Monitor.Log($"Found _ShowPagedResponses1 at Code {i} calling {calledoperand.Name}", LogLevel.Warn);
                        break;
                    }
                }
            }
            calledoperand = null;
            //find _ShowPagedResponses2
            for (int i = startIndex2; i < codes.Count; i++)
            {
                if (!foundCall_ShowPagedResponses2 && codes[i].opcode == OpCodes.Callvirt)
                {
                    calledoperand = codes[i].operand as MethodInfo;
                    if (calledoperand.Name == "_ShowPagedResponses")
                    {
                        foundCall_ShowPagedResponses2 = true;
                        startIndex2 = i;
//                        Monitor.Log($"Found _ShowPagedResponses2 at Code {i} calling {calledoperand.Name}", LogLevel.Warn);
                        break;
                    }
                }
            }
            //If we have found the code range
            if (startIndex1 > -1 && startIndex2 > -1)
            {
                //Set our method
                codes[startIndex1].opcode = OpCodes.Call;
                codes[startIndex1].operand = patchedmethod;
                codes[startIndex2].opcode = OpCodes.Call;
                codes[startIndex2].operand = patchedmethod;
                Monitor.Log($"Successfully changed _ShowPagedResponses method for {patchedmethod} at Code {startIndex1} and {startIndex2}", LogLevel.Trace);
            }
            return codes.AsEnumerable();

        }
        public void Patched_ShowPagedResponses(int page=-1)
        {
//            Monitor.Log($"Executing CreateResponses", LogLevel.Warn);
            GameLocation._PagedResponsePage = page;
            int itemsPerPage = GameLocation._PagedResponseItemsPerPage;
            int pages = (GameLocation._PagedResponses.Count - 1) / itemsPerPage;
            int itemsOnCurPage = itemsPerPage;
            if (GameLocation._PagedResponsePage == pages - 1 && GameLocation._PagedResponses.Count % itemsPerPage == 1)
            {
                itemsOnCurPage++;
                pages--;
            }
            List<Response> locationResponses = new List<Response>();
            for (int i = 0; i < itemsOnCurPage; i++)
            {
                int index = i + GameLocation._PagedResponsePage * itemsPerPage;
                if (index < GameLocation._PagedResponses.Count)
                {
                    KeyValuePair<string, string> response = GameLocation._PagedResponses[index];
                    locationResponses.Add(new Response(response.Key, response.Value));
                }
            }
            if (GameLocation._PagedResponsePage > 0)
            {
                locationResponses.Add(new Response("previousPage", Game1.content.LoadString("Strings\\UI:PreviousPage")));
            }
            if (GameLocation._PagedResponsePage < pages)
            {
                locationResponses.Add(new Response("nextPage", Game1.content.LoadString("Strings\\UI:NextPage")));
            }

            if (GameLocation._PagedResponseAddCancel)
            {
                locationResponses.Add(new Response("cancel", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Cancel")));
            }
            this.createQuestionDialogue(GameLocation._PagedResponsePrompt, locationResponses.ToArray(), "pagedResponse");
        }
    }
}
