/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Monsters;

using DaLion.Common.Harmony;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal class GreenSlimeOnDealContactDamagePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal GreenSlimeOnDealContactDamagePatch()
    {
        Original = RequireMethod<GreenSlime>(nameof(GreenSlime.onDealContactDamage));
    }

    #region harmony patches

    /// <summary>Patch to make Piper immune to slimed debuff.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> GreenSlimeOnDealContactDamageTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: if (who.professions.Contains(<piper_id>) && !who.professions.Contains(100 + <piper_id>)) return;

        var resumeExecution = generator.DefineLabel();
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Bge_Un_S) // find index of first branch instruction
                )
                .GetOperand(out var returnLabel) // get return label
                .Return()
                .AddLabels(resumeExecution)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_1) // arg 1 = Farmer who
                )
                .InsertProfessionCheck((int) Profession.Piper, forLocalPlayer: false)
                .Insert(
                    new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                    new CodeInstruction(OpCodes.Ldarg_1) // arg 1 = Farmer who
                )
                .InsertProfessionCheck((int) Profession.Piper + 100, forLocalPlayer: false)
                .Insert(
                    new CodeInstruction(OpCodes.Brfalse_S, returnLabel)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding Piper slime debuff immunity.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}