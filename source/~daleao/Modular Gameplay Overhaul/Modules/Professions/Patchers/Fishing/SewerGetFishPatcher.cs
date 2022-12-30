/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Patchers.Fishing;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Locations;

#endregion using directives

[UsedImplicitly]
internal sealed class SewerGetFishPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SewerGetFishPatcher"/> class.</summary>
    internal SewerGetFishPatcher()
    {
        this.Target = this.RequireMethod<Sewer>(nameof(Sewer.getFish));
    }

    #region harmony patches

    /// <summary>Patch for prestiged Angler to recatch Mutant Carp.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? SewerGetFishTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: if (!who.fishCaught.ContainsKey(<legendary_fish_id>)) ...
        // To: if (!who.fishCaught.ContainsKey(<legendary_fish_id>) || !who.HasPrestigedProfession("Angler") ...
        try
        {
            var chooseLegendary = generator.DefineLabel();
            helper
                .Match(new[] { new CodeInstruction(OpCodes.Ldc_I4, Constants.MutantCarpIndex) })
                .Match(new[] { new CodeInstruction(OpCodes.Brtrue_S) })
                .GetOperand(out var skipLegendary)
                .ReplaceWith(new CodeInstruction(OpCodes.Brfalse_S, chooseLegendary))
                .Move()
                .AddLabels(chooseLegendary)
                .Insert(new[] { new CodeInstruction(OpCodes.Ldarg_S, (byte)4) }) // arg 4 = Farmer who
                .InsertProfessionCheck(Profession.Angler.Value + 100, forLocalPlayer: false)
                .Insert(new[] { new CodeInstruction(OpCodes.Brfalse_S, skipLegendary) });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding prestiged Mutant Carp legendary fish recatch.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
