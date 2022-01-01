/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using Netcode;
using StardewModdingAPI;
using StardewValley.TerrainFeatures;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches;

[UsedImplicitly]
internal class TreeTickUpdatePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal TreeTickUpdatePatch()
    {
        Original = RequireMethod<Tree>(nameof(Tree.tickUpdate));
    }

    #region harmony patches

    /// <summary>Patch to add bonus wood for prestiged Lumberjack.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> TreeTickUpdateTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: Game1.getFarmer(lastPlayerToHit).professions.Contains(<lumberjack_id>) ? 1.25 : 1.0
        /// To: Game1.getFarmer(lastPlayerToHit).professions.Contains(100 + <lumberjack_id>) ? 1.4 : Game1.getFarmer(lastPlayerToHit).professions.Contains(12) ? 1.25 : 1.0

        var i = 0;
        repeat1:
        try
        {
            var isPrestiged = iLGenerator.DefineLabel();
            var resumeExecution = iLGenerator.DefineLabel();
            helper
                .FindProfessionCheck(Utility.Professions.IndexOf("Lumberjack"), true)
                .Advance()
                .Insert(
                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(OpCodes.Ldc_I4_S, 100 + Utility.Professions.IndexOf("Lumberjack")),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(NetList<int, NetInt>).MethodNamed(nameof(NetList<int, NetInt>.Contains))),
                    new CodeInstruction(OpCodes.Brtrue_S, isPrestiged)
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_R8, 1.25)
                )
                .Advance()
                .AddLabels(resumeExecution)
                .Insert(
                    new CodeInstruction(OpCodes.Br_S, resumeExecution)
                )
                .Insert(
                    new[] {isPrestiged},
                    new CodeInstruction(OpCodes.Pop),
                    new CodeInstruction(OpCodes.Ldc_R8, 1.4)
                );
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed while adding prestiged Lumberjack bonus wood.\nHelper returned {ex}",
                LogLevel.Error);
            return null;
        }

        // repeat injection
        if (++i < 2) goto repeat1;

        // find the Arborist profession check
        helper
            .FindProfessionCheck(Utility.Professions.IndexOf("Arborist"), true)
            .RetreatUntil(
                new CodeInstruction(OpCodes.Ldarg_0)
            )
            .ToBufferUntil(
                true,
                false,
                new CodeInstruction(OpCodes.Callvirt,
                    typeof(NetList<int, NetInt>).MethodNamed(nameof(NetList<int, NetInt>.Contains)))
            );

        // copy these instructions and replace Arborist check for prestiged Arborist check
        var checkForArboristInstructions = helper.Buffer;
        checkForArboristInstructions[5] = new(OpCodes.Ldc_I4_S, 100 + Utility.Professions.IndexOf("Arborist"));

        /// From: numHardwood++;
        /// To: numHardwood += Game1.getFarmer(lastPlayerToHit).professions.Contains(100 + <arborist_id>) ? 2 : 1;
        /// -- and also
        /// From: numHardwood += (int) (numHardwood * 0.25f + 0.9f);
        /// To: numHardwood += (int) (numHardwood * (Game1.getFarmer(lastPlayerToHit).professions.Contains(100 + <arborist_id>) ? 0.5f : 0.25f) + 0.9f);

        helper.ReturnToFirst();
        i = 0;
        repeat2:
        try
        {
            var notPrestigedArborist1 = iLGenerator.DefineLabel();
            var notPrestigedArborist2 = iLGenerator.DefineLabel();
            var resumeExecution1 = iLGenerator.DefineLabel();
            var resumeExecution2 = iLGenerator.DefineLabel();
            helper
                .FindProfessionCheck(Utility.Professions.IndexOf("Arborist"), true)
                .RetreatUntil(
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Add)
                )
                .AddLabels(notPrestigedArborist1)
                .Insert(checkForArboristInstructions)
                .Insert(
                    new CodeInstruction(OpCodes.Brfalse_S, notPrestigedArborist1),
                    new CodeInstruction(OpCodes.Ldc_I4_2),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution1)
                )
                .Advance()
                .AddLabels(resumeExecution1)
                .FindProfessionCheck(Utility.Professions.IndexOf("Arborist"), true)
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_R4, 0.25f),
                    new CodeInstruction(OpCodes.Mul)
                )
                .AddLabels(notPrestigedArborist2)
                .Insert(checkForArboristInstructions)
                .Insert(
                    new CodeInstruction(OpCodes.Brfalse_S, notPrestigedArborist2),
                    new CodeInstruction(OpCodes.Ldc_R4, 0.5f),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution2)
                )
                .Advance()
                .AddLabels(resumeExecution2);
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed while adding prestiged Arborist bonus hardwood.\nHelper returned {ex}",
                LogLevel.Error);
            return null;
        }

        // repeat injection
        if (++i < 2) goto repeat2;

        return helper.Flush();
    }

    #endregion harmony patches
}