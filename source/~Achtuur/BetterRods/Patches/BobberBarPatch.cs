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
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace BetterRods.Patches;

internal class BobberBarPatch : GenericPatcher
{
    public override void Patch(Harmony harmony)
    {
        harmony.Patch(
            original: this.GetOriginalMethod<StardewValley.Menus.BobberBar>(nameof(StardewValley.Menus.BobberBar.update)),
            transpiler: this.GetHarmonyMethod(nameof(TranspileBobberBar))
        );
    }


    private static IEnumerable<CodeInstruction> TranspileBobberBar(IEnumerable<CodeInstruction> instructions)
    {
        Type BobberBar = typeof(StardewValley.Menus.BobberBar);
        List<CodeInstruction> instructions_list = new List<CodeInstruction>(instructions);

        return new CodeMatcher(instructions)
            /// This sequence corresponds to `this.bobberBarSpeed += gravity` on line 133 of Menus/BobberBar.cs, in the update method
            .MatchStartForward(
                new CodeMatch(i => i.opcode == OpCodes.Ldarg_0),
                new CodeMatch(i => i.opcode == OpCodes.Ldarg_0),
                new CodeMatch(i => i.opcode == OpCodes.Ldfld && (FieldInfo)i.operand == AccessTools.Field(BobberBar, "bobberBarSpeed")),
                new CodeMatch(i => i.opcode == OpCodes.Ldloc_3),
                new CodeMatch(i => i.opcode == OpCodes.Add)
            )
            .Advance(4) // point to after LdLoc_3
                        // transform into this.BobberBarSpeed += this.gravity * GetGravityMultiplier()
            .Insert(
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ModEntry), nameof(ModEntry.GetGravityMultiplier))),
                new CodeInstruction(OpCodes.Mul)
            )
            /// `this.distanceFromCatching += 0.002f` on line 193 of Menus/BobberBar.cs, in update()
            /// Is increased when bobber is on fish
            .MatchStartForward(
            new CodeMatch(i => i.opcode == OpCodes.Ldarg_0),
                new CodeMatch(i => i.opcode == OpCodes.Ldarg_0),
                new CodeMatch(i => i.opcode == OpCodes.Ldfld && (FieldInfo)i.operand == AccessTools.Field(BobberBar, "distanceFromCatching")),
                new CodeMatch(i => i.opcode == OpCodes.Ldc_R4 && (float)i.operand == 0.002f),
                new CodeMatch(i => i.opcode == OpCodes.Add)
            )
            .Advance(4) // point to after Ldc_r4
                        // Multipliy 0.002f by GetDistanceGainMultiplier()
            .Insert(
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ModEntry), nameof(ModEntry.GetDistanceGainMultiplier))),
                new CodeInstruction(OpCodes.Mul)
            )
            /// this.distanceFromCatching -= (TrainingRod)? 0.002f : 0.003f
            .MatchStartForward(
                new CodeMatch(i => i.opcode == OpCodes.Ldarg_0),
                new CodeMatch(i => i.opcode == OpCodes.Ldarg_0),
                new CodeMatch(i => i.opcode == OpCodes.Ldfld && (FieldInfo)i.operand == AccessTools.Field(BobberBar, "beginnersRod")),
                new CodeMatch(i => i.opcode == OpCodes.Brtrue_S),
                new CodeMatch(i => i.opcode == OpCodes.Ldc_R4 && (float)i.operand == 0.003f),
                new CodeMatch(i => i.opcode == OpCodes.Br_S),
                new CodeMatch(i => i.opcode == OpCodes.Ldc_R4 && (float)i.operand == 0.002f),
                new CodeMatch(i => i.opcode == OpCodes.Sub)
            )
            .Advance(6) // Point to after Ldc_r4 0.002f
            .Insert(
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ModEntry), nameof(ModEntry.GetDistanceLossMultiplier))),
                new CodeInstruction(OpCodes.Mul)
            )
            .InstructionEnumeration();
    }
}
