/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Reflection;
using System.Reflection.Emit;

using AtraCore.Framework.ReflectionManager;

using AtraShared.Utils.Extensions;
using AtraShared.Utils.HarmonyHelper;

using HarmonyLib;

using StardewValley.Monsters;

namespace CritterRings.HarmonyPatches.OwlRing;

/// <summary>
/// A patch so the farmer is seen less by lava lurks.
/// </summary>
[HarmonyPatch(typeof(LavaLurk))]
internal static class LavaLurkNerf
{
    private static float AdjustLavaLurkDistance(float original, Farmer farmer)
        => (ModEntry.OwlRing > 0 && farmer.isWearingRing(ModEntry.OwlRing)) ? original / 2 : original;

    [HarmonyPatch(nameof(LavaLurk.TargetInRange))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            {
                (OpCodes.Call, typeof(Math).GetCachedMethod<float>(nameof(Math.Abs), ReflectionCache.FlagTypes.StaticFlags)),
                (OpCodes.Ldc_R4, 640f),
            })
            .Advance(2)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, typeof(LavaLurk).GetCachedField(nameof(LavaLurk.targettedFarmer), ReflectionCache.FlagTypes.InstanceFlags)),
                new(OpCodes.Call, typeof(LavaLurkNerf).GetCachedMethod(nameof(AdjustLavaLurkDistance), ReflectionCache.FlagTypes.StaticFlags)),
            });

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Ran into error transpiling {original.Name}\n\n{ex}", LogLevel.Error);
            original.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
}
