/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using System.Reflection.Emit;
using Common.Patch;
using HarmonyLib;
using StardewValley.Tools;

namespace TestMod.Patches;

public class FishingRodPatcher : BasePatcher
{
    public FishingRodPatcher()
    {
        FishingRod.baseChanceForTreasure = 1;
    }
    
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            RequireMethod<FishingRod>(nameof(FishingRod.startMinigameEndFunction)),
            transpiler: GetHarmonyMethod(nameof(StartMinigameEndFunctionTranspiler))
        );
    }

    private static IEnumerable<CodeInstruction> StartMinigameEndFunctionTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = instructions.ToList();
        var index = codes.FindIndex(code => code.opcode == OpCodes.Ldc_R8 && code.operand.Equals(0.25));
        codes[index].operand = 1.0;
        return codes.AsEnumerable();
    }
}