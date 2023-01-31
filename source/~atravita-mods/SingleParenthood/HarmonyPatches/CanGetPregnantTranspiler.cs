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
using StardewValley.Characters;

namespace SingleParenthood.HarmonyPatches;

/// <summary>
/// Transpiles CanGetPregnant to allow for more children.
/// </summary>
[HarmonyPatch(typeof(NPC))]
internal static class CanGetPregnantTranspiler
{
    private static int GetMaxChildCount() => ModEntry.Config.MaxKids;

    [HarmonyPatch(nameof(NPC.canGetPregnant))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            {
                new(SpecialCodeInstructionCases.LdLoc),
                new(OpCodes.Callvirt, typeof(List<Child>).GetCachedProperty("Count", ReflectionCache.FlagTypes.InstanceFlags).GetGetMethod()),
                new(OpCodes.Ldc_I4_2),
                new(OpCodes.Bge_S),
            })
            .Advance(2)
            .ReplaceInstruction(OpCodes.Call, typeof(CanGetPregnantTranspiler).GetCachedMethod(nameof(GetMaxChildCount), ReflectionCache.FlagTypes.StaticFlags), keepLabels: true)
            .FindNext(new CodeInstructionWrapper[]
            {
                new(SpecialCodeInstructionCases.LdLoc),
                new(OpCodes.Ldc_I4_0),
            })
            .Advance(1)
            .RemoveIncluding(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldc_I4_2),
                new(OpCodes.Cgt),
            })
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Call, typeof(SPUtils).GetCachedMethod(nameof(SPUtils.AllKidsOutOfCrib), ReflectionCache.FlagTypes.StaticFlags, new[] { typeof(List<Child>) } )),
            });

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Ran into errors transpiling {original.FullDescription()}.\n\n{ex}", LogLevel.Error);
            original?.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
}
