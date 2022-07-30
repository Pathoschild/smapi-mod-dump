/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using Common;
using Common.Extensions.Reflection;
using Common.Harmony;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerTakeDamagePatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal FarmerTakeDamagePatch()
    {
        Target = RequireMethod<Farmer>(nameof(Farmer.takeDamage));
    }

    #region harmony patches

    /// <summary>Removes damage mitigation soft cap.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? FarmerTakeDamageTranspiler(IEnumerable<CodeInstruction> instructions,
        ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: if (ModEntry.Config.RemoveDefenseSoftCap)
        ///     skip
        ///     {
        ///         effectiveResilience >= damage * 0.5f)
        ///         effectiveResilience -= (int) (effectiveResilience * Game1.random.Next(3) / 10f);
        ///     }

        var skipSoftCap = generator.DefineLabel();
        try
        {
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Ldloc_3),
                    new CodeInstruction(OpCodes.Conv_R4),
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Conv_R4),
                    new CodeInstruction(OpCodes.Ldc_R4, 0.5f)
                )
                .StripLabels(out var labels)
                .InsertWithLabels(
                    labels,
                    new CodeInstruction(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.RemoveDefenseSoftCap))),
                    new CodeInstruction(OpCodes.Brtrue_S, skipSoftCap)
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Stloc_3)
                )
                .Advance()
                .AddLabels(skipSoftCap);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while removing vanilla defense cap.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}