/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Patchers.Mining;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Locations;

#endregion using directives

[UsedImplicitly]
internal sealed class MineShaftCheckActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MineShaftCheckActionPatcher"/> class.</summary>
    internal MineShaftCheckActionPatcher()
    {
        this.Target = this.RequireMethod<MineShaft>(nameof(MineShaft.checkAction));
    }

    #region harmony patches

    /// <summary>Increment Spelunker buff on ladder interaction.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? MineShaftCheckActionTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            var resumeExecution = generator.DefineLabel();
            helper
                .Match(
                    new[] { new CodeInstruction(OpCodes.Ldstr, "stairsdown"), })
                .Match(
                    new[] { new CodeInstruction(OpCodes.Ldc_I4_1), })
                .AddLabels(resumeExecution)
                .Insert(
                    new[] { new CodeInstruction(OpCodes.Ldarg_3), })
                .InsertProfessionCheck(Profession.Spelunker.Value, forLocalPlayer: false)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                        new CodeInstruction(OpCodes.Ldarg_3),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(FarmerExtensions).RequireMethod(nameof(FarmerExtensions.AddSpelunkerMomentum))),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding Spelunker effects on ladder down.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
