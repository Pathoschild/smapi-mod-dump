/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.AnimalHusbandryMod;

#region using directives

using DaLion.Common;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly]
internal sealed class FeedingBasketOverridesDoFunctionPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal FeedingBasketOverridesDoFunctionPatch()
    {
        try
        {
            Target = "AnimalHusbandryMod.tools.FeedingBasketOverrides".ToType().RequireMethod("DoFunction");
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Patch for Rancher to combine Shepherd and Coopmaster friendship bonus.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? InseminationSyringeOverridesDoFunctionTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: if ((!animal.isCoopDweller() && who.professions.Contains(3)) || (animal.isCoopDweller() && who.professions.Contains(2)))
        /// To: if (who.professions.Contains(<rancher_id>)
        /// -- and also
        /// Injected: if (who.professions.Contains(<rancher_id> + 100)) repeat professionAdjust ...

        var isNotPrestiged = generator.DefineLabel();
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(FarmAnimal).RequireMethod(nameof(FarmAnimal.isCoopDweller)))
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[7]),
                    new CodeInstruction(OpCodes.Ldsfld),
                    new CodeInstruction(OpCodes.Ldfld)
                )
                .RetreatUntil(
                    new CodeInstruction(OpCodes.Brfalse_S)
                )
                .GetOperand(out var isNotRancher)
                .Return(2)
                .RemoveUntil(
                    new CodeInstruction(OpCodes.Nop)
                )
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)5) // arg 5 = Farmer who
                )
                .InsertProfessionCheck(Profession.Rancher.Value, forLocalPlayer: false)
                .Insert(
                    new CodeInstruction(OpCodes.Brfalse_S, isNotRancher)
                )
                .GetInstructionsUntil(out var got,
                    pattern: new CodeInstruction(OpCodes.Stloc_S, $"{typeof(double)} (7)")
                )
                .Insert(got)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)5)
                )
                .InsertProfessionCheck(Profession.Rancher.Value + 100, forLocalPlayer: false)
                .Insert(
                    new CodeInstruction(OpCodes.Brfalse_S, isNotPrestiged)
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Nop)
                )
                .Remove()
                .AddLabels(isNotPrestiged);

        }
        catch (Exception ex)
        {
            Log.E(
                $"Failed while moving combined feeding basket Coopmaster + Shepherd friendship bonuses to Rancher.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}