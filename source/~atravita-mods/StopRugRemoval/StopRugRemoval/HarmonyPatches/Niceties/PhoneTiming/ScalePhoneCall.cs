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
using AtraCore.Framework.ReflectionManager;
using AtraShared.Utils.Extensions;
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;

namespace StopRugRemoval.HarmonyPatches.Niceties.PhoneTiming;

/// <summary>
/// Scales the phone call so it doesn't take as long.
/// </summary>
[HarmonyPatch]
internal static class ScalePhoneCall
{
    /// <summary>
    /// Gets the methods to patch.
    /// </summary>
    /// <returns>An IEnumerable of methods to patch.</returns>
    internal static IEnumerable<MethodBase> TargetMethods()
    {
        yield return typeof(GameLocation).GetCachedMethod(nameof(GameLocation.answerDialogueAction), ReflectionCache.FlagTypes.InstanceFlags);

        Type? gima = AccessTools.TypeByName("GingerIslandMainlandAdjustments.Niceties.PhoneHandler");
        MethodInfo? gimaPhone = AccessTools.Method(gima, "PostfixAnswerDialogueAction");

        if (gimaPhone is not null)
        {
            ModEntry.ModMonitor.Log("Patching GIMA's phone");
            yield return gimaPhone;
        }

        Type? cart = AccessTools.TypeByName("PhoneTravelingCart.Framework.Patchers.GameLocationPatcher");
        MethodInfo? cartPhone = AccessTools.Method(cart, "GameLocation_answerDialogueAction_Prefix");
        if (cartPhone is not null)
        {
            ModEntry.ModMonitor.LogOnce("Found PhoneTravelingCart, patching for compat", LogLevel.Info);
            yield return cartPhone;
        }
        yield break;
    }

    private static int AdjustPhoneFreeze(int prevtime)
        => (int)(prevtime / ModEntry.Config.PhoneSpeedUpFactor);

    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.ForEachMatch(
                new CodeInstructionWrapper[]
                {
                    new(OpCodes.Ldc_I4, GameLocation.PHONE_RING_DURATION),
                    new(OpCodes.Stfld, typeof(Farmer).GetCachedField(nameof(Farmer.freezePause), ReflectionCache.FlagTypes.InstanceFlags)),
                },
                (helper) =>
                {
                    helper.Advance(1)
                    .Insert(new CodeInstruction[]
                    {
                        new(OpCodes.Call, typeof(ScalePhoneCall).GetCachedMethod(nameof(AdjustPhoneFreeze), ReflectionCache.FlagTypes.StaticFlags)),
                    });
                    return true;
                })
            .ForEachMatch(
                new CodeInstructionWrapper[]
                {
                    new(OpCodes.Ldc_I4, GameLocation.PHONE_RING_DURATION),
                    new(OpCodes.Call, typeof(DelayedAction).GetCachedMethod(nameof(DelayedAction.functionAfterDelay), ReflectionCache.FlagTypes.StaticFlags)),
                },
                (helper) =>
                {
                    helper.Advance(1)
                    .Insert(new CodeInstruction[]
                    {
                        new(OpCodes.Call, typeof(ScalePhoneCall).GetCachedMethod(nameof(AdjustPhoneFreeze), ReflectionCache.FlagTypes.StaticFlags)),
                    });
                    return true;
                });

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling {original.GetFullName()}\n\n{ex}", LogLevel.Error);
            original?.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
}
