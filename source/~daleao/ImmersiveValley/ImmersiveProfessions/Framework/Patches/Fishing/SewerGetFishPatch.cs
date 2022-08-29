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

using DaLion.Common;
using DaLion.Common.Harmony;
using Extensions;
using HarmonyLib;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly]
internal sealed class SewerGetFishPatch : DaLion.Common.Harmony.HarmonyPatch
{
    private const int MUTANT_CARP_INDEX_I = 682;

    /// <summary>Construct an instance.</summary>
    internal SewerGetFishPatch()
    {
        Target = RequireMethod<Sewer>(nameof(Sewer.getFish));
    }

    #region harmony patches

    /// <summary>Patch for prestiged Angler to recatch Mutant Carp.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? SewerGetFishTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: if (!who.fishCaught.ContainsKey(<legendary_fish_id>)) ...
        /// To: if (!who.fishCaught.ContainsKey(<legendary_fish_id>) || !who.HasPrestigedProfession("Angler") ...

        var chooseLegendary = generator.DefineLabel();
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldc_I4, MUTANT_CARP_INDEX_I)
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Brtrue_S)
                )
                .GetOperand(out var skipLegendary)
                .ReplaceWith(
                    new(OpCodes.Brfalse_S, chooseLegendary))
                .Advance()
                .AddLabels(chooseLegendary)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)4) // arg 4 = Farmer who
                )
                .InsertProfessionCheck(Profession.Angler.Value + 100, forLocalPlayer: false)
                .Insert(
                    new CodeInstruction(OpCodes.Brfalse_S, skipLegendary)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding prestiged Angler legendary fish recatch.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}