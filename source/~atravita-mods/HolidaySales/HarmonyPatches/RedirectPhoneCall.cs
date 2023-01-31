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
using AtraBase.Toolkit.Extensions;
using AtraBase.Toolkit.Reflection;
using AtraCore.Framework.ReflectionManager;
using AtraShared.Utils.Extensions;
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;

namespace HolidaySales.HarmonyPatches;

/// <summary>
/// Patch to handle the phone.
/// </summary>
[HarmonyPatch(typeof(GameLocation))]
internal static class RedirectPhoneCall
{
    /// <summary>
    /// Gets the methods to patch.
    /// </summary>
    /// <returns>methods to patch.</returns>
    /// <exception cref="MethodNotFoundException">Method wasn't found.</exception>
    internal static IEnumerable<MethodBase> TargetMethods()
    {
        foreach (MethodInfo? method in typeof(GameLocation).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
        {
            if (method.Name.Contains("<answerDialogueAction>", StringComparison.Ordinal) && method.GetParameters().Length == 0
                && ShouldTranspileThisMethod(method))
            {
                yield return method;
            }
        }

        Type? inner = typeof(GameLocation).GetNestedType("<>c", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            ?? ReflectionThrowHelper.ThrowMethodNotFoundException<Type>("phone inner class");

        foreach (MethodInfo? method in inner.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
        {
            if (method.Name.Contains("<answerDialogueAction>", StringComparison.Ordinal) && method.GetParameters().Length == 0
                && ShouldTranspileThisMethod(method))
            {
                yield return method;
            }
        }

        yield break;
    }

    private static bool ShouldTranspileThisMethod(MethodInfo method)
        => PatchProcessor.GetOriginalInstructions(method)
        .Any((instr) => instr.Calls(typeof(GameLocation).GetCachedMethod(nameof(GameLocation.AreStoresClosedForFestival), ReflectionCache.FlagTypes.StaticFlags)));

    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);

            helper.ForEachMatch(
                new CodeInstructionWrapper[]
                {
                    (OpCodes.Call, typeof(GameLocation).GetCachedMethod(nameof(GameLocation.AreStoresClosedForFestival), ReflectionCache.FlagTypes.StaticFlags)),
                },
                (helper) =>
                {
                    helper.ReplaceOperand(typeof(HSUtils).GetCachedMethod(nameof(HSUtils.StoresClosedForFestival), ReflectionCache.FlagTypes.StaticFlags));
                    return true;
                });

            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling {original.GetFullName()}\n\n{ex}", LogLevel.Error);
            original.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
}