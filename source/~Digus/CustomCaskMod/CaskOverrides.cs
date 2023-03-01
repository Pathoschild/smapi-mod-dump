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
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using SObject = StardewValley.Object;

namespace CustomCaskMod
{
    internal class CaskOverrides
    {
        public static bool GetAgingMultiplierForItemPrefix(ref Cask __instance, Item item, ref float __result)
        {
            __result = 0f;
            if (item != null && (Utility.IsNormalObjectAtParentSheetIndex(item, item.ParentSheetIndex) || IsColoredObjectAtParentSheetIndex(item, item.ParentSheetIndex)))
            {
                if (IsVanillaCask(__instance))
                {
                    if (DataLoader.CaskDataId.ContainsKey(item.ParentSheetIndex))
                    {
                        __result = DataLoader.CaskDataId[item.ParentSheetIndex];
                    }
                    else if (DataLoader.CaskDataId.ContainsKey(item.Category))
                    {
                        __result = DataLoader.CaskDataId[item.Category];
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    if (AgerController.GetAger(__instance.Name) is CustomAger ager)
                    {
                        var agingMultiplier = AgerController.GetAgingMultiplierForItem(ager, item);
                        if (agingMultiplier.HasValue)
                        {
                            __result = agingMultiplier.Value;
                        }
                    }
                }
            }
            return false;
        }

        public static void GetAgingMultiplierForItemPostfix(Cask __instance, Item item, ref float __result)
        {
            if (item != null && (Utility.IsNormalObjectAtParentSheetIndex(item, item.ParentSheetIndex) || IsColoredObjectAtParentSheetIndex(item, item.ParentSheetIndex)))
            {
                if (IsVanillaCask(__instance))
                {
                    if (Math.Abs(__result) <= 0f && DataLoader.ModConfig.EnableCaskAgeEveryObject)
                    {
                        __result = DataLoader.ModConfig.DefaultAgingRate;
                    }
                }
            }
        }

        public static bool IsValidCaskLocation(ref Cask __instance, ref bool __result)
        {
            if ((IsVanillaCask(__instance) && DataLoader.ModConfig.EnableCasksAnywhere)
                || (AgerController.GetAger(__instance.Name) is CustomAger ager && ager.EnableAgingAnywhere))
            {
                __result = true;
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool checkForMaturity(ref Cask __instance)
        {
            if ((IsVanillaCask(__instance) && DataLoader.ModConfig.EnableMoreThanOneQualityIncrementPerDay)
                || (AgerController.GetAger(__instance.Name) is CustomAger ager && ager.EnableMoreThanOneQualityIncrementPerDay))
            {
                if ((float)__instance.daysToMature.Value <= 0f)
                {
                    __instance.MinutesUntilReady = 1;
                    __instance.heldObject.Value.Quality = 4;
                }
                else if ((float)__instance.daysToMature.Value <= 28f)
                {
                    __instance.heldObject.Value.Quality = 2;
                }
                else if ((float)__instance.daysToMature.Value <= 42f)
                {
                    __instance.heldObject.Value.Quality = 1;
                }
                return false;
            }
            return true;
        }

        public static bool placementAction(ref SObject __instance, GameLocation location, int x, int y, Farmer who, ref bool __result)
        {
            if (__instance.bigCraftable.Value && AgerController.HasAger(__instance.Name))
            {
                Vector2 placementTile = new Vector2(x / 64, y / 64);
                Cask cask = new Cask(placementTile)
                {
                    ParentSheetIndex = __instance.ParentSheetIndex
                };
                Game1.bigCraftablesInformation.TryGetValue(cask.ParentSheetIndex, out string objectInformation);
                if (objectInformation != null)
                {
                    string[] objectInfoArray = objectInformation.Split('/');
                    cask.name = objectInfoArray[0];
                    cask.Price = Convert.ToInt32(objectInfoArray[1]);
                    cask.Edibility = Convert.ToInt32(objectInfoArray[2]);
                    string[] typeAndCategory = objectInfoArray[3].Split(' ');
                    cask.Type = typeAndCategory[0];
                    if (typeAndCategory.Length > 1)
                    {
                        cask.Category = Convert.ToInt32(typeAndCategory[1]);
                    }
                    cask.setOutdoors.Value = Convert.ToBoolean(objectInfoArray[5]);
                    cask.setIndoors.Value = Convert.ToBoolean(objectInfoArray[6]);
                    cask.Fragility = Convert.ToInt32(objectInfoArray[7]);
                    cask.isLamp.Value = (objectInfoArray.Length > 8 && objectInfoArray[8].Equals("true"));
                }
                cask.initializeLightSource(cask.TileLocation);
                cask.boundingBox.Value = new Microsoft.Xna.Framework.Rectangle((int)cask.TileLocation.X * 64, (int)cask.TileLocation.Y * 64, 64, 64);
                location.objects.Add(placementTile, cask);
                location.playSound("hammer");
                __result = true;
                return false;
            }
            return true;
        }

        public static IEnumerable<CodeInstruction> performToolAction_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            LinkedList<CodeInstruction> newInstructions = new LinkedList<CodeInstruction>(instructions);
            CodeInstruction codeInstructionWoodWhack = newInstructions.FirstOrDefault(c => c.opcode == OpCodes.Ldstr && c.operand?.ToString() == "woodWhack");
            CodeInstruction codeInstructionBeforePlaySound = newInstructions.FirstOrDefault(c => c.opcode == OpCodes.Ldc_I4_0);
            LinkedListNode<CodeInstruction> linkedListNode = newInstructions.Find(codeInstructionWoodWhack);
            if (linkedListNode != null && codeInstructionWoodWhack != null && codeInstructionBeforePlaySound != null)
            {
                Label endLabelWoodWack = generator.DefineLabel();
                codeInstructionWoodWhack.labels.Add(endLabelWoodWack);
                Label endLabelBeforePlaySound = generator.DefineLabel();
                codeInstructionBeforePlaySound.labels.Add(endLabelBeforePlaySound);

                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Ldarg_0, null));
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Callvirt, AccessTools.Property(typeof(Item), "Name").GetGetMethod()));
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Ldstr, "Cask"));
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(String), "op_Equality", new Type[] { typeof(string), typeof(string) })));
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Brtrue_S, endLabelWoodWack));
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Ldstr, "hammer"));
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Br_S, endLabelBeforePlaySound));
            }
            return newInstructions;
        }

        public static IEnumerable<CodeInstruction> performObjectDropInAction_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            LinkedList<CodeInstruction> newInstructions = new LinkedList<CodeInstruction>(instructions);
            CodeInstruction codeInstructionFirstPlaySound = newInstructions.FirstOrDefault(c => c.opcode == OpCodes.Callvirt && c.operand != null && c.operand.ToString().Contains("playSound"));
            CodeInstruction codeInstructionLastReturnTrue = newInstructions.LastOrDefault(c => c.opcode == OpCodes.Ldc_I4_1);
            LinkedListNode<CodeInstruction> linkedListNode = newInstructions.Find(codeInstructionFirstPlaySound)?.Next;
            if (linkedListNode?.Next != null && codeInstructionFirstPlaySound != null && codeInstructionLastReturnTrue != null)
            {
                Label endLabel = generator.DefineLabel();
                codeInstructionLastReturnTrue.labels.Add(endLabel);

                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Ldarg_0, null));
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Callvirt, AccessTools.Property(typeof(Item), "Name").GetGetMethod()));
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Ldstr, "Cask"));
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(String), "op_Equality", new Type[] { typeof(string), typeof(string) })));
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Brfalse_S, endLabel));
            }
            return newInstructions;
        }

        private static bool IsVanillaCask(Cask cask)
        {
            return cask.Name == "Cask";
        }

        private static bool IsColoredObjectAtParentSheetIndex(Item item, int index)
        {
            if (item == null)
            {
                return false;
            }
            if (item.GetType() != typeof(ColoredObject))
            {
                return false;
            }
            if ((item as ColoredObject)?.bigCraftable.Value == true)
            {
                return false;
            }
            return item.ParentSheetIndex == index;
        }
    }
}
