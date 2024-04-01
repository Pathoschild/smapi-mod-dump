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
using HarmonyLib;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.GameData.FarmAnimals;
using DataLoader = AnimalHusbandryMod.common.DataLoader;
using SObject = StardewValley.Object;

namespace AnimalHusbandryMod.animals
{
    public class FarmAnimalOverrides
    {
        public static IEnumerable<CodeInstruction> dayUpdate_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            LinkedList<CodeInstruction> newInstructions = new LinkedList<CodeInstruction>(instructions);
            CodeInstruction codeInstruction = newInstructions.FirstOrDefault(c => c.opcode == OpCodes.Ldfld && c.operand?.ToString() == "StardewValley.GameData.FarmAnimals.FarmAnimalHarvestType HarvestType");
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

            codeInstruction = newInstructions.FirstOrDefault(c => c.opcode == OpCodes.Call && c.operand?.ToString() == "Boolean spawnObjectAround(Microsoft.Xna.Framework.Vector2, StardewValley.Object, StardewValley.GameLocation, Boolean, System.Action`1[StardewValley.Object])");
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
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Ldarg_0, null));
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Call, AccessTools.Property(typeof(Character), "Tile").GetGetMethod()));
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Ldloc_S, 17));
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(StardewValley.Object), "getOne")));
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Castclass, typeof(StardewValley.Object)));
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Ldarg_1,null));
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Ldc_I4_1,null));
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Ldnull,null));
                newInstructions.AddBefore(linkedListNode, codeInstruction.Clone());
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Pop, null));
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
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Dup, null));
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Callvirt, AccessTools.Property(typeof(StardewValley.Object), "Stack").GetGetMethod()));
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Ldc_I4_1, null));
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Add, null));
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Callvirt, AccessTools.Property(typeof(StardewValley.Object), "Stack").GetSetMethod()));
            }

            return newInstructions;
        }
    }
}
