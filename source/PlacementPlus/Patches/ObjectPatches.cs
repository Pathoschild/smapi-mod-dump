/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/2Retr0/PlacementPlus
**
*************************************************/

#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using StardewModdingAPI;
using StardewValley;
using Object = StardewValley.Object;

#endregion

namespace PlacementPlus.Patches
{
    [HarmonyPatch(typeof(Object), nameof(Object.placementAction))]
    internal class ObjectPatches_PlacementAction
    {
        private static IMonitor Monitor                          => PlacementPlus.Instance.Monitor;
        private static readonly FieldInfo shakeTimerFI           = AccessTools.Field(typeof(Object), nameof(Object.shakeTimer));
        private static readonly FieldInfo contentFI              = AccessTools.Field(typeof(Game1),  nameof(Game1.content));
        private static readonly MethodInfo objectAtTileIsChestMI = AccessTools.Method(typeof(ObjectPatches_PlacementAction), nameof(ObjectAtTileIsChest));
        
        // We assume that x and y are raw coordinates and not tile coordinates...
        // We also assume that it has already been asserted that the Object at tile is not null...
        private static bool ObjectAtTileIsChest(GameLocation location, int x, int y)
        {
            var objectAtTile = location.getObjectAtTile(x / 64, y / 64);
            return PlacementPlus.CHEST_INFO_LIST.Contains(objectAtTile.ParentSheetIndex);
        }
        
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilGenerator)
        {
            var code           = new List<CodeInstruction>(instructions);
            var returnLabel    = ilGenerator.DefineLabel();
            var injectionIndex = -1;

            try
            {
                for (var i = 0; i < code.Count - 1; i++)
                {
                    // IL_1709: dup
                    // * >>> Iterator will be here! <<<
                    // IL_170a: ldc.i4.s     50 // 0x32
                    // IL_170c: stfld        int32 StardewValley.Object::shakeTimer
                    // IL_1711: ...etc.
                    if (!(code[i].opcode == OpCodes.Ldc_I4_S  && Convert.ToInt32(code[i].operand) == 50  &&
                          code[i + 1].opcode == OpCodes.Stfld && code[i + 1].operand.Equals(shakeTimerFI))
                    ) continue;

                    // IL_16df: ldc.i4.0
                    // * >>> Iterator will be here! <<<
                    // IL_16e0: ret
                    // IL_16e1: ...etc.
                    while (!(code[i].opcode == OpCodes.Ldc_I4_0 && code[i + 1].opcode == OpCodes.Ret)) i--;
                    code[i].labels.Add(returnLabel);
                    
                    // IL_16c9: brfalse.s    IL_16e1
                    // * >>> Iterator will be here! <<<
                    // IL_16cb: ldsfld       class StardewValley.LocalizedContentManager StardewValley.Game1::content
                    // IL_16d0: ldstr        "Strings\\StringsFromCSFiles:Object.cs.13053"
                    // IL_16d5: ...etc.
                    while (!(code[i].opcode == OpCodes.Ldsfld && code[i].operand.Equals(contentFI))) i--;
                    injectionIndex = i;
                    break;
                }

                if (injectionIndex == -1) throw new IndexOutOfRangeException();
                
                var instructionsToInject = new List<CodeInstruction>
                {
                    new CodeInstruction(OpCodes.Ldarg_1),                     // Push GameLocation location to evaluation stack.
                    new CodeInstruction(OpCodes.Ldarg_2),                     // Push int x to evaluation stack.
                    new CodeInstruction(OpCodes.Ldarg_3),                     // Push int y to evaluation stack.
                    new CodeInstruction(OpCodes.Call, objectAtTileIsChestMI), // Call objectAtTileIsChest.
                    new CodeInstruction(OpCodes.Brtrue_S, returnLabel)        // Skip showRedMessage invoke if method returns true via branching.
                };

                // Save the labels from the current opcode at injectionIndex since some branches jump there.
                var oldOpcodeLabels = code[injectionIndex].labels; code[injectionIndex].labels = null;
                code.InsertRange(injectionIndex, instructionsToInject); // Injecting code

                // Set the new opcode at injectionIndex to the previously saved labels.
                code[injectionIndex].labels = oldOpcodeLabels;

                return code.AsEnumerable();
            } catch (Exception e) {
                Monitor.Log($"Failed in {nameof(ObjectPatches_PlacementAction)}:\n{e}", LogLevel.Error);
                return code.AsEnumerable(); // Run original logic.
            }
        }
    }
}