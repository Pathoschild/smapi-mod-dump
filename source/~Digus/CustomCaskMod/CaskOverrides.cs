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
using System.Threading.Tasks;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.GameData.BigCraftables;
using StardewValley.GameData.Machines;
using StardewValley.Objects;
using SObject = StardewValley.Object;

namespace CustomCaskMod
{
    internal class CaskOverrides
    {
        public static bool IsValidCaskLocation(ref Cask __instance, ref bool __result)
        {
            if ((IsVanillaCask(__instance) && DataLoader.ModConfig.EnableCasksAnywhere)
                || (AgerController.GetAger(__instance.QualifiedItemId) is CustomAger ager && ager.EnableAgingAnywhere))
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
                || (AgerController.GetAger(__instance.QualifiedItemId) is CustomAger ager && ager.EnableMoreThanOneQualityIncrementPerDay))
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

        public static bool placementAction(ref SObject __instance, GameLocation location, int x, int y, ref bool __result)
        {
            if (__instance.bigCraftable.Value && AgerController.HasAger(__instance.QualifiedItemId))
            {
                Vector2 placementTile = new(x / 64, y / 64);
                Cask cask = new(placementTile)
                {
                    ParentSheetIndex = __instance.ParentSheetIndex
                };
                Game1.bigCraftableData.TryGetValue(__instance.ItemId, out BigCraftableData objectInformation);
                if (objectInformation != null)
                {
                    cask.ItemId = __instance.ItemId;
                    cask.name = objectInformation.Name;
                    cask.Price = objectInformation.Price;
                    cask.setOutdoors.Value = objectInformation.CanBePlacedOutdoors;
                    cask.setIndoors.Value = objectInformation.CanBePlacedIndoors;
                    cask.Fragility = objectInformation.Fragility;
                    cask.isLamp.Value = objectInformation.IsLamp;
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
            LinkedList<CodeInstruction> newInstructions = new(instructions);
            CodeInstruction codeInstructionWoodWhack = newInstructions.FirstOrDefault(c => c.opcode == OpCodes.Ldstr && c.operand?.ToString() == "woodWhack");
            LinkedListNode<CodeInstruction> linkedListNode = newInstructions.Find(codeInstructionWoodWhack);
            if (linkedListNode != null && codeInstructionWoodWhack != null && linkedListNode.Next is { Value: { } codeInstructionAfterWoodWhack })
            {
                Label endLabelWoodWack = generator.DefineLabel();
                codeInstructionWoodWhack.labels.Add(endLabelWoodWack);
                Label endLabelBeforePlaySound = generator.DefineLabel();
                codeInstructionAfterWoodWhack.labels.Add(endLabelBeforePlaySound);

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

        internal static bool TryApplyFairyDust(SObject __instance, bool probe, ref bool __result)
        {
            if (__instance.GetMachineData() != null) return true;
            if (__instance.MinutesUntilReady <= 0) return true;
            if (!AgerController.HasAger(__instance.QualifiedItemId)) return true;
            if (!probe)
            {
                Utility.addSprinklesToLocation(__instance.Location, (int)__instance.TileLocation.X,
                    (int)__instance.TileLocation.Y, 1, 2, 400, 40, Color.White);
                Game1.playSound("yoba");
                __instance.MinutesUntilReady = 0;
                __instance.minutesElapsed(0);
            }
            __result = true;
            return false;
        }

        private static bool IsVanillaCask(SObject cask)
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
