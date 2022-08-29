/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Foraging;

#region using directives

using DaLion.Common;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using DaLion.Common.ModData;
using Extensions;
using HarmonyLib;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly]
internal sealed class BushShakePatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal BushShakePatch()
    {
        Target = RequireMethod<Bush>("shake");
    }

    #region harmony patches

    /// <summary>Patch to nerf Ecologist berry quality and increment forage counter for wild berries.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? BushShakeTranspiler(IEnumerable<CodeInstruction> instructions,
        ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: Game1.player.professions.Contains(16) ? 4 : 0
        /// To: Game1.player.professions.Contains(16) ? GetEcologistForageQuality() : 0

        try
        {
            helper
                .FindProfessionCheck(Farmer.botanist) // find index of botanist check
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_I4_4)
                )
                .GetLabels(out var labels) // backup branch labels
                .ReplaceWith( // replace with custom quality
                    new(OpCodes.Call,
                        typeof(FarmerExtensions).RequireMethod(nameof(FarmerExtensions.GetEcologistForageQuality)))
                )
                .InsertWithLabels(
                    labels: labels, // restore backed-up labels
                    new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching modded Ecologist wild berry quality.\nHelper returned {ex}");
            return null;
        }

        /// Injected: if (Game1.player.professions.Contains(<ecologist_id>))
        ///		Data.IncrementField<uint>("EcologistItemsForaged")

        var dontIncreaseEcologistCounter = generator.DefineLabel();
        try
        {
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Ldarg_0)
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldarg_0)
                )
                .InsertProfessionCheck(Profession.Ecologist.Value)
                .Insert(
                    new CodeInstruction(OpCodes.Brfalse_S, dontIncreaseEcologistCounter),
                    new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                    new CodeInstruction(OpCodes.Ldstr, "EcologistItemsForaged"),
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModDataIO)
                            .RequireMethod(nameof(ModDataIO.Increment), new[] { typeof(Farmer), typeof(string) })
                            .MakeGenericMethod(typeof(uint)))
                )
                .AddLabels(dontIncreaseEcologistCounter);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding Ecologist counter increment.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}