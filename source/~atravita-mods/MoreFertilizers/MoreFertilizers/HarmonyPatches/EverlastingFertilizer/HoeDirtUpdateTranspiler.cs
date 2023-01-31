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

using StardewValley.TerrainFeatures;

namespace MoreFertilizers.HarmonyPatches.EverlastingFertilizer;

// casey nop's this for JA but we don't want to depend on JA forever.
// so I'm gonna insert a check to skip destroyCrop if we're in a Everlasting Fertilizer.
[HarmonyPatch(typeof(HoeDirt))]
internal static class HoeDirtUpdateTranspiler
{
    [HarmonyPatch(nameof(HoeDirt.dayUpdate))]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:Split parameters should start on line after declaration", Justification = "Reviewed.")]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            {
                OpCodes.Ldarg_1,
                (OpCodes.Callvirt, typeof(GameLocation).GetCachedProperty(nameof(GameLocation.IsGreenhouse), ReflectionCache.FlagTypes.InstanceFlags).GetGetMethod()),
                OpCodes.Brtrue_S,
            })
            .Push()
            .Advance(2)
            .StoreBranchDest()
            .AdvanceToStoredLabel()
            .DefineAndAttachLabel(out Label jumppoint)
            .Pop()
            .GetLabels(out IList<Label>? labels)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, typeof(RemoveSeasonCheck).GetCachedMethod(nameof(RemoveSeasonCheck.IsInEverlasting), ReflectionCache.FlagTypes.StaticFlags)),
                new(OpCodes.Brtrue, jumppoint),
            }, withLabels: labels);

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling {original.FullDescription()}:\n\n{ex}", LogLevel.Error);
            original.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
}
