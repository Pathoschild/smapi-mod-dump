/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Mining;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

using Stardew.Common.Harmony;

#endregion using directives

[UsedImplicitly]
internal class GameLocationBreakStonePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal GameLocationBreakStonePatch()
    {
        Original = RequireMethod<GameLocation>("breakStone");
    }

    #region harmony patches

    /// <summary>Patch to remove Geologist extra gem chance + remove Prospector double coal chance.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> GameLocationBreakStoneTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: if (who.professions.Contains(100 + <miner_id>) addedOres++;
        /// After: int addedOres = (who.professions.Contains(<miner_id>) ? 1 : 0);

        var notPrestigedMiner = generator.DefineLabel();
        try
        {
            helper
                .FindProfessionCheck((int) Profession.Miner)
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Stloc_1)
                )
                .AddLabels(notPrestigedMiner)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_S, (byte) 4) // arg 4 = Farmer who
                )
                .InsertProfessionCheckForPlayerOnStack((int) Profession.Miner + 100,
                    notPrestigedMiner)
                .Insert(
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Add)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding prestiged Miner extra ores.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        /// Skipped: if (who.professions.Contains(<geologist_id> && r.NextDouble() < 0.5) switch(indexOfStone) ...

        try
        {
            helper
                .FindProfessionCheck(Farmer.geologist) // find index of geologist check
                .Retreat()
                .StripLabels(out var labels) // backup and remove branch labels
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Brfalse) // the false case branch
                )
                .GetOperand(out var isNotGeologist) // copy destination
                .Return()
                .Insert( // insert uncoditional branch to skip this check
                    new CodeInstruction(OpCodes.Br, (Label) isNotGeologist)
                )
                .Retreat()
                .AddLabels(labels); // restore backed-up labels to inserted branch
        }
        catch (Exception ex)
        {
            Log.E($"Failed while removing vanilla Geologist paired gems.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        /// Skipped: if (who.professions.Contains(<prospector_id>)) ...

        try
        {
            helper
                .FindProfessionCheck(Farmer.burrower) // find index of prospector check
                .Retreat()
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Brfalse_S) // the false case branch
                )
                .GetOperand(out var isNotProspector) // copy destination
                .Return()
                .Insert( // insert uncoditional branch to skip this check
                    new CodeInstruction(OpCodes.Br_S, (Label) isNotProspector)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while removing vanilla Prospector double coal chance.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}