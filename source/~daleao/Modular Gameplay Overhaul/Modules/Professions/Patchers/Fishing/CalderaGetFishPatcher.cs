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
internal sealed class CalderaGetFishPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CalderaGetFishPatcher"/> class.</summary>
    internal CalderaGetFishPatcher()
    {
        this.Target = this.RequireMethod<Caldera>(nameof(Caldera.getFish));
    }

    #region harmony patches

    /// <summary>Patch for Fisher to reroll reeled fish if first roll resulted in trash.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? GameLocationGetFishTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: if (Game1.player.professions.Contains(<fisher_id>)) <baseChance> *= 2
        //	After: if (Game1.random.NextDouble() < 0.1 ...
        try
        {
            var isNotFisher = generator.DefineLabel();
            helper
                .Match(new[] { new CodeInstruction(OpCodes.Ldc_R8, 0.1) })
                .Move()
                .AddLabels(isNotFisher)
                .InsertProfessionCheck(Profession.Fisher.Value)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Brfalse_S, isNotFisher),
                        new CodeInstruction(OpCodes.Ldc_R8, 2.0),
                        new CodeInstruction(OpCodes.Mul),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding modded Fisher fish reroll.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
