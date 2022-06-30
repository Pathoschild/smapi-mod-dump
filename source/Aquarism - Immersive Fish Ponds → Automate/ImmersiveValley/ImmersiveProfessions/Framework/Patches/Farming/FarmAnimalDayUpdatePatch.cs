/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Farming;

#region using directives

using DaLion.Common;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using HarmonyLib;
using JetBrains.Annotations;
using Netcode;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmAnimalDayUpdatePatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal FarmAnimalDayUpdatePatch()
    {
        Target = RequireMethod<FarmAnimal>(nameof(FarmAnimal.dayUpdate));
    }

    #region harmony patches

    /// <summary>
    ///     Patch for Producer to double produce frequency at max animal happiness + remove Shepherd and Coopmaster hidden
    ///     produce quality boosts.
    /// </summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? FarmAnimalDayUpdateTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: FarmeAnimal.daysToLay -= (FarmAnimal.type.Value.Equals("Sheep") && Game1.getFarmer(FarmAnimal.ownerID).professions.Contains(Farmer.shepherd)) ? 1 : 0
        /// To: FarmAnimal.daysToLay /= (FarmAnimal.happiness.Value >= 200 && Game1.getFarmer(FarmAnimal.ownerID).professions.Contains(<producer_id>))
        ///		? Game1.getFarmer(FarmAnimal.ownerID).professions.Contains(100 + <producer_id>)) ? 3 : 2
        ///		: 1

        var notPrestigedProducer = generator.DefineLabel();
        var resumeExecution1 = generator.DefineLabel();
        try
        {
            helper
                .FindFirst( // find index of FarmAnimal.type.Value.Equals("Sheep")
                    new CodeInstruction(OpCodes.Ldstr, "Sheep"),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(string).RequireMethod(nameof(string.Equals), new[] { typeof(string) }))
                )
                .RetreatUntil(
                    new CodeInstruction(OpCodes.Ldarg_0)
                )
                .Insert(
                    new CodeInstruction(OpCodes.Conv_R8)
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldfld, typeof(FarmAnimal).RequireField(nameof(FarmAnimal.type)))
                )
                .SetOperand(typeof(FarmAnimal).RequireField(nameof(FarmAnimal.happiness))) // was FarmAnimal.type
                .Advance()
                .SetOperand(typeof(NetFieldBase<byte, NetByte>)
                    .RequirePropertyGetter(nameof(NetFieldBase<byte, NetByte>.Value))) // was <string, NetString>
                .Advance()
                .ReplaceWith(
                    new(OpCodes.Ldc_I4_S, 200) // was Ldstr "Sheep"
                )
                .Advance()
                .Remove()
                .SetOpCode(OpCodes.Blt_S) // was Brfalse_S
                .Advance()
                .GetInstructionsUntil(out var got, false, true,
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(NetList<int, NetInt>).RequireMethod(nameof(NetList<int, NetInt>.Contains)))
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_I4_0)
                )
                .ReplaceWith(
                    new(OpCodes.Ldc_R8, 1.0),
                    true
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_I4_1)
                )
                .ReplaceWith(
                    new(OpCodes.Ldc_R8, 1.75),
                    true
                )
                .AddLabels(notPrestigedProducer)
                .Insert(got)
                .RetreatUntil(
                    new CodeInstruction(OpCodes.Ldc_I4_3)
                )
                .ReplaceWith(
                    new(OpCodes.Ldc_I4_S, Profession.Producer.Value + 100)
                )
                .Return()
                .Insert(
                    new CodeInstruction(OpCodes.Brfalse_S, notPrestigedProducer),
                    new CodeInstruction(OpCodes.Ldc_R8, 3.0),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution1)
                )
                .Advance()
                .SetOpCode(OpCodes.Div) // was Sub
                .AddLabels(resumeExecution1)
                .Advance()
                .Insert(
                    new CodeInstruction(OpCodes.Call,
                        typeof(Math).RequireMethod(nameof(Math.Round), new[] { typeof(double) })),
                    new CodeInstruction(OpCodes.Conv_U1)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching modded Producer produce frequency.\nHelper returned {ex}");
            return null;
        }

        /// Skipped: if ((!isCoopDweller() && Game1.getFarmer(FarmAnimal.ownerID).professions.Contains(<shepherd_id>)) || (isCoopDweller() && Game1.getFarmer(FarmAnimal.ownerID).professions.Contains(<coopmaster_id>))) chanceForQuality += 0.33

        try
        {
            helper
                .FindNext( // find index of first FarmAnimal.isCoopDweller check
                    new CodeInstruction(OpCodes.Call,
                        typeof(FarmAnimal).RequireMethod(nameof(FarmAnimal.isCoopDweller)))
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Brfalse_S) // the all cases false branch
                )
                .GetOperand(out var resumeExecution2) // copy destination
                .Return()
                .Retreat()
                .Insert( // insert unconditional branch to skip this whole section
                    new CodeInstruction(OpCodes.Br_S, (Label)resumeExecution2)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while removing vanilla Coopmaster + Shepherd produce quality bonuses.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}