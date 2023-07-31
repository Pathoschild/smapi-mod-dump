/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using AchtuurCore.Patches;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace JunimoBeacon.Patches;
internal class PathFindToNewCropPatcher : GenericPatcher
{
    public override void Patch(Harmony harmony)
    {
        harmony.Patch(
            original: GetOriginalMethod<JunimoHarvester>(nameof(JunimoHarvester.pathFindToNewCrop_doWork)),
            transpiler: GetHarmonyMethod(nameof(transpiler))
        );
    }



    /// <summary>
    /// Transpiles <see cref="JunimoHarvester.pathFindToNewCrop_doWork"/>
    /// </summary>
    private static IEnumerable<CodeInstruction> transpiler(IEnumerable<CodeInstruction> instructions)
    {
        /// Line 119 in pathFindToNewCrop_doWork() contains if statement
        /// do first part of if statement, until 'brfalse.s IL_00F3' and get that label
        /// detect last part of if statement that jumps to true part 'ble IL_0181', get that label
        /// right after first part of if statement, call custom range check function
        /// then branch to IL_00F3 if out of range, or IL_0181 if in range


        // TODO: rewrite using codematcher

        List<CodeInstruction> instructions_list = new List<CodeInstruction>(instructions);

        List<CodeInstruction> controllerNullCheck = new List<CodeInstruction>()
        {
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldfld),
            new CodeInstruction(OpCodes.Ldfld),
            new CodeInstruction(OpCodes.Brfalse_S)
        };

        List<CodeInstruction> ifTrueBranch = new List<CodeInstruction>()
        {
            new CodeInstruction(OpCodes.Call),
            new CodeInstruction(OpCodes.Ldc_I4_8),
            new CodeInstruction(OpCodes.Ble),
        };

        Range? controllerNullCheckRange = GenericPatcher.FindILSequence(instructions_list, controllerNullCheck);
        Range? ifTrueBranchRange = GenericPatcher.FindILSequence(instructions_list, ifTrueBranch);

        if (controllerNullCheckRange is null || ifTrueBranchRange is null)
            return instructions_list.AsEnumerable();

        // label to jump to when outside of range
        Label FalseLabel = (Label)instructions_list[controllerNullCheckRange.Value.End].operand;
        // label to jump to when inside range
        Label TrueLabel = (Label)instructions_list[ifTrueBranchRange.Value.End].operand;

        List<CodeInstruction> insertSequence = new List<CodeInstruction>()
        {
            // Load 'this' JunimoHarvester
            new CodeInstruction(OpCodes.Ldarg_0),
            //new CodeInstruction(OpCodes.p)
            new CodeInstruction(OpCodes.Call, AccessTools.Method(
                typeof(PathFindToNewCropPatcher),
                nameof(JunimoEndpointInRangeOfHut),
                new Type[] {typeof(JunimoHarvester)}
                )
            ),
            // Branch if out of range
            new CodeInstruction(OpCodes.Brfalse, FalseLabel),
            // Always branch to in range (because it wasnt false)
            new CodeInstruction(OpCodes.Br, TrueLabel)
        };


        int i = GetInsertMethodIndex(controllerNullCheckRange.Value, ILCodeInsertMethod.InsertAtEnd);
        GenericPatcher.InsertILCode(instructions_list, insertSequence, i);
        return instructions_list.AsEnumerable();
    }

    /// <summary>
    /// returns true when junimo.controller.endpoint is in range of junimo hut
    /// </summary>
    /// <returns></returns>
    public static bool JunimoEndpointInRangeOfHut(JunimoHarvester junimo)
    {
        if (junimo.controller is null)
            return false;

        Vector2 endpoint = junimo.controller.pathToEndPoint.Last<Point>().ToVector2();
        return ModEntry.Instance.IsInRangeOfAnyGroup(endpoint);
    }
}
