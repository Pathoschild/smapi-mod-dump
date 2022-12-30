/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Patchers.Foraging;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Netcode;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class FruitTreePerformToolActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FruitTreePerformToolActionPatcher"/> class.</summary>
    internal FruitTreePerformToolActionPatcher()
    {
        this.Target = this.RequireMethod<FruitTree>(nameof(FruitTree.performToolAction));
    }

    #region harmony patches

    /// <summary>Patch to add bonus wood for prestiged Lumberjack.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? FruitTreePerformToolActionTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: Game1.getFarmer(lastPlayerToHit).professions.Contains(<lumberjack_id>) ? 1.25 : 1.0
        // To: Game1.getFarmer(lastPlayerToHit).professions.Contains(100 + <lumberjack_id>) ? 1.4 : Game1.getFarmer(lastPlayerToHit).professions.Contains(12) ? 1.25 : 1.0
        try
        {
            helper
                .Repeat(
                    2,
                    i =>
                    {
                        var isPrestiged = generator.DefineLabel();
                        var resumeExecution = generator.DefineLabel();
                        helper
                            .FindProfessionCheck(Profession.Lumberjack.Value)
                            .Move()
                            .Insert(
                                new[]
                                {
                                    new CodeInstruction(OpCodes.Dup),
                                    new CodeInstruction(OpCodes.Ldc_I4_S, Profession.Lumberjack.Value + 100),
                                    new CodeInstruction(
                                        OpCodes.Callvirt,
                                        typeof(NetList<int, NetInt>).RequireMethod(
                                            nameof(NetList<int, NetInt>.Contains))),
                                    new CodeInstruction(OpCodes.Brtrue_S, isPrestiged),
                                })
                            .Match(
                                i > 0
                                    ? new[] { new CodeInstruction(OpCodes.Ldc_R8, 1.25) }
                                    : new[] { new CodeInstruction(OpCodes.Ldc_I4_5) })
                            .Move()
                            .AddLabels(resumeExecution)
                            .Insert(new[] { new CodeInstruction(OpCodes.Br_S, resumeExecution) })
                            .Insert(
                                new[]
                                {
                                    new CodeInstruction(OpCodes.Pop), i > 0
                                        ? new CodeInstruction(OpCodes.Ldc_R8, 1.4)
                                        : new CodeInstruction(OpCodes.Ldc_I4_6),
                                },
                                new[] { isPrestiged });
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding prestiged Lumberjack bonus wood.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
