/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using AnimalHusbandryMod.common;
using Harmony;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;

namespace AnimalHusbandryMod.animals
{
    public class FarmAnimalOverrides
    {
        public static void dayUpdate(FarmAnimal __instance)
        {
            if (__instance.harvestType.Value == FarmAnimal.layHarvestType
                && __instance.daysSinceLastLay.Value == 0
                && AnimalContestController.HasFertilityBonus(__instance)
                && !DataLoader.ModConfig.DisableContestBonus)
            {
                GameLocation homeIndoors = __instance.home.indoors.Value;
                if (homeIndoors.Objects.ContainsKey(__instance.getTileLocation()))
                {
                    StardewValley.Object originalLayedObject = homeIndoors.Objects[__instance.getTileLocation()];
                    if (originalLayedObject.Category == StardewValley.Object.EggCategory)
                    {
                        __instance.setRandomPosition(homeIndoors);
                        if (!homeIndoors.Objects.ContainsKey(__instance.getTileLocation()))
                        {
                            homeIndoors.Objects.Add(__instance.getTileLocation(), new StardewValley.Object(Vector2.Zero, originalLayedObject.ParentSheetIndex, (string)null, false, true, false, true)
                            {
                                Quality = originalLayedObject.Quality
                            });
                        }
                    }
                }
            }
        }

        public static IEnumerable<CodeInstruction> dayUpdate_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            LinkedList<CodeInstruction> newInstructions = new LinkedList<CodeInstruction>(instructions);
            CodeInstruction codeInstruction = newInstructions.FirstOrDefault(c => c.opcode == OpCodes.Ldfld && c.operand?.ToString() == "Netcode.NetByte harvestType");
            LinkedListNode<CodeInstruction> linkedListNode = newInstructions.Find(codeInstruction);
            if (linkedListNode != null)
            {
                var lastInstruction = new CodeInstruction(OpCodes.Ldarg_0, null);
                Label endLabel = generator.DefineLabel();
                lastInstruction.labels.Add(endLabel);

                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Pop, null));
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(DataLoader), "ModConfig")));
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ModConfig), "DisableContestBonus")));
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Brtrue, endLabel));
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Ldarg_0, null));
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(AnimalContestController), "HasProductionBonus", new Type[]{typeof(FarmAnimal)})));
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Brfalse, endLabel));
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Ldarg_0, null));
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(FarmAnimal), "produceQuality")));
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Ldc_I4_4, null));
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Callvirt, AccessTools.Property(typeof(NetInt), "Value").GetSetMethod()));
                newInstructions.AddBefore(linkedListNode, lastInstruction);
            }

            //This is really prone to future bugs since if the vanilla code add any addition to a Item list before this line, this will break.
            codeInstruction = newInstructions.FirstOrDefault(c => c.opcode == OpCodes.Callvirt && c.operand?.ToString() == "StardewValley.Item addItem(StardewValley.Item)");
            linkedListNode = newInstructions.Find(codeInstruction);
            if (linkedListNode != null && codeInstruction != null)
            {
                Label endLabel = generator.DefineLabel();
                codeInstruction.labels.Add(endLabel);

                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(DataLoader), "ModConfig")));
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ModConfig), "DisableContestBonus")));
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Brtrue, endLabel));
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Ldarg_0, null));
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(AnimalContestController), "HasFertilityBonus", new Type[] { typeof(FarmAnimal) })));
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Brfalse, endLabel));
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Dup, null));
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Ldc_I4_2, null));
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Callvirt, AccessTools.Property(typeof(StardewValley.Object), "Stack").GetSetMethod()));
            }

            return newInstructions;
        }
    }
}
