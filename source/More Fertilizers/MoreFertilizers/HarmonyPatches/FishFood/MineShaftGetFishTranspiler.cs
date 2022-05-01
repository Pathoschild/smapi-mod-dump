/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/MoreFertilizers
**
*************************************************/

using System.Reflection;
using System.Reflection.Emit;
using AtraBase.Toolkit.Reflection;
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;
using StardewValley.Locations;

namespace MoreFertilizers.HarmonyPatches.FishFood;

[HarmonyPatch(typeof(MineShaft))]
internal static class MineShaftGetFishTranspiler
{
#pragma warning disable SA1116 // Split parameters should start on line after declaration
    [HarmonyPatch(nameof(MineShaft.getFish))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        // Being a little lazy here.
        // Adjusting for each (Game1.random.NextDouble() < double + double * chanceMultiplier)
        // Which is repeated three times.
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);

            // looking for three occurances of Game1.random.NextDouble() in that switch case.
            helper.FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldc_I4_M1),
                new(OpCodes.Call, typeof(MineShaft).InstanceMethodNamed(nameof(MineShaft.getMineArea))),
            });

            int startindex = helper.Pointer;

            helper.FindNext(new CodeInstructionWrapper[]
            {
                new(SpecialCodeInstructionCases.LdArg),
                new(OpCodes.Callvirt, typeof(Farmer).InstancePropertyNamed(nameof(Farmer.FishingLevel)).GetGetMethod()),
            });

            int endindex = helper.Pointer + 4; // since I'm adding instructions, the end point will move up.

            helper.ForEachMatch(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldsfld, typeof(Game1).StaticFieldNamed(nameof(Game1.random))),
                new(OpCodes.Callvirt, typeof(Random).InstanceMethodNamed(nameof(Random.NextDouble))),
            },
            transformer: (helper) =>
            {
                helper.FindNext(new CodeInstructionWrapper[]
                {
                    new(SpecialCodeInstructionCases.LdLoc),
                    new(OpCodes.Mul),
                    new(OpCodes.Add),
                    new(OpCodes.Bge_Un_S),
                })
                .Advance(3)
                .Insert(new CodeInstruction[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, typeof(GetFishTranspiler).StaticMethodNamed(nameof(GetFishTranspiler.AlterFishChance))),
                });
                return true;
            },
            startindex: startindex,
            intendedendindex: endindex,
            maxCount: 3);
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling MineShaft.GetFish:\n\n{ex}", LogLevel.Error);
        }
        return null;
    }
#pragma warning restore SA1116 // Split parameters should start on line after declaration
}
