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
using StardewValley;

namespace TestMod.Patches;

internal class GameLocationPatcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            RequireMethod<GameLocation>(nameof(GameLocation.performTenMinuteUpdate)),
            transpiler: GetHarmonyMethod(nameof(PerformTenMinuteUpdateTranspiler))
        );
    }

    public static IEnumerable<CodeInstruction> PerformTenMinuteUpdateTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = instructions.ToList();
        var index = codes.FindIndex(code => code.opcode == OpCodes.Ldc_R8 && Math.Abs((double)code.operand - 0.01) < 0.0001);
        codes[index].operand = 1d;
        index = codes.FindIndex(code => code.opcode == OpCodes.Ldc_R8 && Math.Abs((double)code.operand - 0.008) < 0.0001);
        codes[index].operand = 1d;
        index = codes.FindIndex(code => code.opcode == OpCodes.Ldc_I4_2);
        index = codes.FindIndex(index, code => code.opcode == OpCodes.Ldc_I4_2);
        codes[index].operand = 999;
        return codes.AsEnumerable();
    }
}