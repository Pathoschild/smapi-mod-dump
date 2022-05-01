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
using Microsoft.Xna.Framework;
using StardewValley.Locations;

namespace MoreFertilizers.HarmonyPatches.FishFood;

[HarmonyPatch(typeof(Caldera))]
internal static class CalderaGetFishTranspiler
{
    [HarmonyPatch(nameof(Caldera.getFish))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            { // advance past the location where the painting is given.
                new(OpCodes.Call, typeof(Vector2).StaticPropertyNamed(nameof(Vector2.Zero)).GetGetMethod()),
                new(OpCodes.Newobj),
                new(OpCodes.Ret),
            })
            .FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldsfld, typeof(Game1).StaticFieldNamed(nameof(Game1.random))),
                new(OpCodes.Callvirt, typeof(Random).InstanceMethodNamed(nameof(Random.NextDouble))),
            })
            .FindNext(new CodeInstructionWrapper[]
            {
                new(SpecialCodeInstructionCases.LdArg),
                new(OpCodes.Conv_R8),
                new(OpCodes.Mul),
            })
            .FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Mul),
            })
            .Advance(1)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, typeof(GetFishTranspiler).StaticMethodNamed(nameof(GetFishTranspiler.AlterFishChance))),
            });
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling Caldera.GetFish:\n\n{ex}", LogLevel.Error);
        }
        return null;
    }
}