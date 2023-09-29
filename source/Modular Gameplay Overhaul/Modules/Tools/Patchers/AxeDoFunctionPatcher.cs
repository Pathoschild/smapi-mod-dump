/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Tools.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Tools.Configs;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class AxeDoFunctionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="AxeDoFunctionPatcher"/> class.</summary>
    internal AxeDoFunctionPatcher()
    {
        this.Target = this.RequireMethod<Axe>(nameof(Axe.DoFunction));
    }

    #region harmony patches

    /// <summary>Charge shockwave stamina cost.</summary>
    [HarmonyPostfix]
    private static void AxeDoFunctionPostfix(Farmer who)
    {
        var power = who.toolPower;
        if (power <= 0)
        {
            return;
        }

        who.Stamina -=
            (int)Math.Round(Math.Sqrt(Math.Max((2 * (power + 1)) - (who.ForagingLevel * 0.1f), 0.1f) *
                                      (int)Math.Pow(2d * (power + 1), 2d))) *
            (float)Math.Pow(ToolsModule.Config.Axe.ChargedStaminaCostMultiplier, power);
    }

    /// <summary>Apply base stamina multiplier + stamina cost cap.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? AxeDoFunctionTranspiler(
        IEnumerable<CodeInstruction> instructions,
        MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        //// From: who.Stamina -= (float)(2 * power) - (float)who.<SkillLevel> * 0.1f;
        //// To: who.Stamina -= Math.Max(((float)(2 * power) - (float)who.<SkillLevel> * 0.1f) * AxeConfig.BaseStaminaCostMultiplier, 0.1f);
        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Farmer).RequirePropertySetter(nameof(Farmer.Stamina))),
                    })
                .Move(-1) // OpCodes.Sub
                .Insert(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Tools))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Config).RequirePropertyGetter(nameof(Config.Axe))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(AxeConfig).RequirePropertyGetter(nameof(AxeConfig.BaseStaminaCostMultiplier))),
                        new CodeInstruction(OpCodes.Mul),
                        new CodeInstruction(OpCodes.Ldc_R4, 0.1f),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Math).RequireMethod(nameof(Math.Max), new[] { typeof(float), typeof(float) })),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding stamina cost multiplier and lower bound for the Axe.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
