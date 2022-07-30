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
/// scales down the dialing sounds.
/// </summary>
[HarmonyPatch(typeof(GameLocation))]
internal static class ScaleDialing
{
    /// <summary>
    /// Applies patches against PhoneTravelingCart.
    /// </summary>
    /// <param name="harmony">Harmony instance.</param>
    internal static void ApplyPatches(Harmony harmony)
    {
        Type cart = AccessTools.TypeByName("PhoneTravelingCart.Framework.Patchers.GameLocationPatcher");
        MethodInfo method = AccessTools.Method(cart, "playShopPhoneNumberSounds");

        if (method is not null)
        {
            harmony.Patch(method, transpiler: new HarmonyMethod(typeof(ScaleDialing), nameof(Transpiler)));
        }
        else
        {
            ModEntry.ModMonitor.Log("PhoneTravelingCart's playShopPhoneNumberSounds not found, integration may not work", LogLevel.Info);
        }
    }

    private static int AdjustPhoneFreeze(int prevtime)
        => (int)(prevtime / ModEntry.Config.PhoneSpeedUpFactor);

    [HarmonyPatch("playShopPhoneNumberSounds")]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.ForEachMatch(
                new CodeInstructionWrapper[]
                {
                    new(SpecialCodeInstructionCases.Wildcard, (instr) => instr.opcode == OpCodes.Ldstr && ((string)instr.operand).StartsWith("telephone", StringComparison.Ordinal)),
                    new(OpCodes.Ldc_I4),
                },
                (helper) =>
                {
                    helper.Advance(2)
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