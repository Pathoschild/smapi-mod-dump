/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Patchers.Farming;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class HoeDirtApplySpeedIncreasesPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="HoeDirtApplySpeedIncreasesPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal HoeDirtApplySpeedIncreasesPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<HoeDirt>("applySpeedIncreases");
    }

    #region harmony patches

    /// <summary>Patch to increase prestiged Agriculturist crop growth speed.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? HoeDirtApplySpeedIncreasesTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: if (who.professions.Contains(<agriculturist_id>)) speedIncrease += 0.1f;
        // To: if (who.professions.Contains(<agriculturist_id>)) speedIncrease += who.professions.Contains(100 + <agriculturist_id>)) ? 0.2f : 0.1f;
        try
        {
            var isNotPrestiged = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .MatchProfessionCheck(Profession.Agriculturist.Value)
                .Move()
                .MatchProfessionCheck(Profession.Agriculturist.Value)
                .PatternMatch([new CodeInstruction(OpCodes.Ldc_R4, 0.1f)])
                .AddLabels(isNotPrestiged)
                .Insert([new CodeInstruction(OpCodes.Ldarg_1)])
                .InsertProfessionCheck(Farmer.agriculturist + 100, forLocalPlayer: false)
                .Insert([
                    new CodeInstruction(OpCodes.Brfalse_S, isNotPrestiged),
                    new CodeInstruction(OpCodes.Ldc_R4, 0.2f),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution),
                ])
                .Move()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed patching prestiged Agriculturist bonus.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
