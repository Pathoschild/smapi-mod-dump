/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/SwizzyStudios/SV-SwizzyMeads
**
*************************************************/

using System;
using Harmony;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Netcode;


namespace SwizzyMeads
{
    [HarmonyPatch(typeof(StardewValley.Object))]
    [HarmonyPatch("performObjectDropInAction")]
    [HarmonyPatch(new Type[] { typeof(StardewValley.Item), typeof(bool), typeof(StardewValley.Farmer) })]
    static class Object_PerformObjectDropInAction_Patcher
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = instructions.ToList();
            var indexForChanges = -1;
            var instructionsToInsert = new List<CodeInstruction>();

            //Looking for the first instance of "Mead"
            indexForChanges = SearchForInstances(instructionsList, 0);
            if (indexForChanges != -1)
            {
                //Adding a new line of code to adjust the price of the mead to match the base Honey used * 2
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldarg_0));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(StardewValley.Object), nameof(StardewValley.Object.heldObject))));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(NetRef<StardewValley.Object>), "get_Value")));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldloc_0));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(StardewValley.Object), "get_Price")));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldc_I4_2));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Mul));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(StardewValley.Object), "set_Price")));

                //Inject the new code
                instructionsList.InsertRange(indexForChanges, instructionsToInsert);
                instructionsToInsert.Clear();
            }
            //Looking for the second instance of "Mead"
            indexForChanges = SearchForInstances(instructionsList, 1);
            if (indexForChanges != - 1)
            {
                //Getting the Item's (Honey) name and replacing instances of "Honey" with "" and "Wild " with ""
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldloc_0));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(StardewValley.Item), "get_Name")));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldstr, "Wild "));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldstr, ""));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(String), "Replace", new Type[] { typeof(String), typeof(String) })));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldstr, "Honey"));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldstr, ""));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(String), "Replace", new Type[] { typeof(String), typeof(String) })));
                
                //Inject the new code
                instructionsList.InsertRange(indexForChanges, instructionsToInsert);
                instructionsToInsert.Clear();
                
                //Adding more code to complete the Concat between the above Honey + Mead
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(String), "Concat", new Type[] { typeof(String), typeof(String) })));
                
                //Inject the new code
                instructionsList.InsertRange(indexForChanges + 9, instructionsToInsert);
                instructionsToInsert.Clear();
            }
            return instructionsList;
        }

        private static int SearchForInstances(List<CodeInstruction> instructions, int stoppingPoint)
        {
            var index = -1;
            var skip = 0;
            for (int i = 0; i < instructions.Count; i++)
            {
                if ((instructions[i].opcode == OpCodes.Ldstr) && (instructions[i].operand as String == "Mead"))
                {
                    //When we want the first instance of "Mead" for adding a price
                    if (stoppingPoint == 0 && skip == 0)
                    {
                        //need to jump 7 lines further as no distinct indicator
                        index = i + 7;
                        break;
                    }
                    //When we want the second instance of "Mead" for rename
                    if (stoppingPoint == 1 && skip == 1)
                    {
                        index = i;
                        break;
                    }
                    skip++;
                }
            }
            return index;
        }
    }
}
