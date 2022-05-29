/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Fishing;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Locations;

using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal class MineShaftGetFishPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal MineShaftGetFishPatch()
    {
        Original = RequireMethod<MineShaft>(nameof(MineShaft.getFish));
    }

    #region harmony patches

    /// <summary>Patch for Fisher to reroll reeled fish if first roll resulted in trash.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> GameLocationGetFishTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: if (Game1.player.professions.Contains(<fisher_id>)) <baseChance> *= 2 
        ///	Before each of the three fish rolls

        var i = 0;
        repeat:
        try
        {
            var isNotFisher = generator.DefineLabel();
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Callvirt, typeof(Random).RequireMethod(nameof(Random.NextDouble)))
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_R8, 0.02 - i * 0.005)
                )
                .Advance()
                .AddLabels(isNotFisher)
                .InsertProfessionCheck((int) Profession.Fisher)
                .Insert(
                    new CodeInstruction(OpCodes.Brfalse_S, isNotFisher),
                    new CodeInstruction(OpCodes.Ldc_R8, 2.0),
                    new CodeInstruction(OpCodes.Mul)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding modded Fisher fish reroll.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        if (++i < 3) goto repeat;

        return helper.Flush();
    }

    #endregion harmony patches
}