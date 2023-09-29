/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;

#endregion using directives

[UsedImplicitly]
internal sealed class BuffCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BuffCtorPatcher"/> class.</summary>
    internal BuffCtorPatcher()
    {
        this.Target = this.RequireConstructor<Buff>(typeof(int));
    }

    #region harmony patches

    /// <summary>Adjust Jinxed debuff for defense overhaul.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? BuffCtorPostfix(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: if (ModEntry.Config.Combat.OverhauledDefense) value += 3; <-- from -8 to -5
        try
        {
            var resumeExecution = generator.DefineLabel();
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Color).RequirePropertyGetter(nameof(Color.HotPink))),
                    })
                .Match(new[] { new CodeInstruction(OpCodes.Stelem_I4) }, ILHelper.SearchOption.Previous)
                .AddLabels(resumeExecution)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Combat))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Config).RequirePropertyGetter(nameof(Config.NewResistanceFormula))),
                        new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                        new CodeInstruction(OpCodes.Ldc_I4_S, 3),
                        new CodeInstruction(OpCodes.Add),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed to rebalance Jinxed debuff for overhauled defense.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
