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

using DaLion.Common;
using DaLion.Common.Harmony;
using Extensions;
using HarmonyLib;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly]
internal sealed class GreenSlimeOnDealContactDamagePatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal GreenSlimeOnDealContactDamagePatch()
    {
        Target = RequireMethod<GreenSlime>(nameof(GreenSlime.onDealContactDamage));
    }

    #region harmony patches

    /// <summary>Patch to make Piper immune to slimed debuff.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? GreenSlimeOnDealContactDamageTranspiler(
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
                .InsertProfessionCheck(Profession.Piper.Value, forLocalPlayer: false)
                .Insert(
                    new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                    new CodeInstruction(OpCodes.Ldarg_1) // arg 1 = Farmer who
                )
                .InsertProfessionCheck(Profession.Piper.Value + 100, forLocalPlayer: false)
                .Insert(
                    new CodeInstruction(OpCodes.Brfalse_S, returnLabel)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding Piper slime debuff immunity.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}