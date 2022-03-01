/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Farming;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.TerrainFeatures;

using Stardew.Common.Harmony;

#endregion using directives

[UsedImplicitly]
internal class HoeDirtApplySpeedIncreases : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal HoeDirtApplySpeedIncreases()
    {
        Original = RequireMethod<HoeDirt>("applySpeedIncreases");
    }

    #region harmony patches

    /// <summary>Patch to increase prestiged Agriculturist crop growth speed.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> HoeDirtApplySpeedIncreasesTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: if (who.professions.Contains(<agriculturist_id>)) speedIncrease += 0.1f;
        /// To: if (who.professions.Contains(<agriculturist_id>)) speedIncrease += who.professions.Contains(100 + <agriculturist_id>)) ? 0.2f : 0.1f;

        var notPrestigedAgriculturist = generator.DefineLabel();
        var resumeExecution = generator.DefineLabel();
        try
        {
            helper
                .FindProfessionCheck((int) Profession.Agriculturist)
                .Advance()
                .FindProfessionCheck((int) Profession.Agriculturist, true)
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_R4, 0.1f)
                )
                .AddLabels(notPrestigedAgriculturist)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_1)
                )
                .InsertProfessionCheckForPlayerOnStack((int) Profession.Agriculturist + 100,
                    notPrestigedAgriculturist)
                .Insert(
                    new CodeInstruction(OpCodes.Ldc_R4, 0.2f),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution)
                )
                .Advance()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching prestiged Agriculturist bonus.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}