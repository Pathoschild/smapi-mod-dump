/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using AchtuurCore.Patches;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
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

        /// This sequence corresponds to `this.bobberBarSpeed += gravity` on line 133 of Menus/BobberBar.cs, in the update method
        List<CodeInstruction> bobberspeed_add_sequence = new List<CodeInstruction>()
        {
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(BobberBar, "bobberBarSpeed")),
            new CodeInstruction(OpCodes.Ldloc_3),
            new CodeInstruction(OpCodes.Add),
        };

        List<CodeInstruction> multiplyGravity = new List<CodeInstruction>()
        {
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ModEntry), nameof(ModEntry.GetGravityMultiplier))),
            new CodeInstruction(OpCodes.Mul),
        };


        /// `this.distanceFromCatching += 0.002f` on line 193 of Menus/BobberBar.cs, in update()
        /// Is increased when bobber is on fish
        List<CodeInstruction> distancefromcatching_add_sequence = new List<CodeInstruction>()
        {
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(BobberBar, "distanceFromCatching")),
            new CodeInstruction(OpCodes.Ldc_R4, 0.002f),
            new CodeInstruction(OpCodes.Add)
        };

        List<CodeInstruction> multiplyDistanceGain = new List<CodeInstruction>()
        {
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ModEntry), nameof(ModEntry.GetDistanceGainMultiplier))),
            new CodeInstruction(OpCodes.Mul),
        };


        /// part of `this.distanceFromCatching -= ((this.whichBobber == 694 || this.beginnersRod) ? 0.002f : 0.003f)`, line 228 of Menus/BobberBar.cs
        List<CodeInstruction> distancefromcatching_sub_sequence = new List<CodeInstruction>()
        {
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(BobberBar, "beginnersRod")),
            new CodeInstruction(OpCodes.Brtrue_S),
            new CodeInstruction(OpCodes.Ldc_R4, 0.003f),
            new CodeInstruction(OpCodes.Br_S),
            new CodeInstruction(OpCodes.Ldc_R4, 0.002f),
            new CodeInstruction(OpCodes.Sub)
        };

        List<CodeInstruction> multiplyDistanceLoss = new List<CodeInstruction>()
        {
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ModEntry), nameof(ModEntry.GetDistanceLossMultiplier))),
            new CodeInstruction(OpCodes.Mul),
        };

        // Match and insert codes
        MatchAndInsertILCode(instructions_list, bobberspeed_add_sequence, multiplyGravity, ILCodeInsertMethod.InsertOneBeforeEnd);
        MatchAndInsertILCode(instructions_list, distancefromcatching_add_sequence, multiplyDistanceGain, ILCodeInsertMethod.InsertOneBeforeEnd);
        MatchAndInsertILCode(instructions_list, distancefromcatching_sub_sequence, multiplyDistanceLoss, ILCodeInsertMethod.InsertOneBeforeEnd); //967

        return instructions_list.AsEnumerable();

    }
}
