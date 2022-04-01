/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/DrawFishPondsOverGrass
**
*************************************************/

using System.Reflection;
using System.Reflection.Emit;
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;
using StardewValley.Buildings;

namespace DrawFishPondsOverGrass.HarmonyPatches;

/// <summary>
/// Class that holds patches against the fish pond's draw function.
/// </summary>
[HarmonyPatch(typeof(FishPond))]
internal static class FishPondLayerDepthFix
{
    private const float Offset = 280f;

    [HarmonyPatch(nameof(FishPond.draw))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);

            helper.ForEachMatch( // Adjust all calls that add a fudge factor first.
                    new CodeInstructionWrapper[]
                    {
                        new(OpCodes.Ldc_R4, 64),
                        new(OpCodes.Mul),
                        new(OpCodes.Ldc_R4),
                        new(OpCodes.Add),
                    },
                    transformer: (helper) =>
                    {
                        helper.Advance(2);
                        float operand = (float)helper.CurrentInstruction.operand;
                        helper.ReplaceOperand(Offset + operand);
                        return true;
                    })
                .ForEachMatch( // adjust calls that didn't add a fudge factor.
                    new CodeInstructionWrapper[]
                    {
                        new(OpCodes.Ldc_R4, 64),
                        new(OpCodes.Mul),
                        new(OpCodes.Ldc_R4, 10000),
                        new(OpCodes.Div),
                    },
                    transformer: (helper) =>
                    {
                        helper.Advance(2)
                        .Insert(new CodeInstruction[]
                        {
                            new(OpCodes.Ldc_R4, Offset),
                            new(OpCodes.Add),
                        });
                        return true;
                    })
                .ForEachMatch( // adjust calls with a negative fudge factor.
                    new CodeInstructionWrapper[]
                    {
                        new(OpCodes.Ldc_R4, 64),
                        new(OpCodes.Mul),
                        new(OpCodes.Ldc_R4),
                        new(OpCodes.Sub),
                    },
                    transformer: (helper) =>
                    {
                        helper.Advance(2);
                        float operand = (float)helper.CurrentInstruction.operand;
                        helper.ReplaceOperand(Offset - operand)
                            .Advance(1)
                            .ReplaceInstruction(new(OpCodes.Add));
                        return true;
                    });
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed while trying to transpile fish pond layer depth\n{ex}", LogLevel.Error);
        }
        return null;
    }
}