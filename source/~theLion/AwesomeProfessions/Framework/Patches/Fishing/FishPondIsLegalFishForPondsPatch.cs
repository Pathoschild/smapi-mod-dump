/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
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
using StardewValley;
using StardewValley.Buildings;

using Stardew.Common.Harmony;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal class FishPondIsLegalFishForPondsPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal FishPondIsLegalFishForPondsPatch()
    {
        Original = RequireMethod<FishPond>("isLegalFishForPonds");
    }

    #region harmony patches

    /// <summary>Patch for prestiged Aquarist to raise legendary fish.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> FishPondIsLegalFishForPondsTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator ilGenerator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: if (fish_item.HasContextTag("fish_legendary")) ...
        /// To: if (fish_item.HasContextTag("fish_legendary") && !who.HasPrestigedProfession("Aquarist"))

        var checkProfession = ilGenerator.DefineLabel();
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldstr, "fish_legendary")
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Brfalse_S)
                )
                .GetOperand(out var resumeExecution)
                .Advance()
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call,
                        typeof(FishPondIsLegalFishForPondsPatch).MethodNamed(nameof(IsLegalFishForPondsSubroutine))),
                    new CodeInstruction(OpCodes.Brtrue_S, resumeExecution)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding prestiged Aquarist permission to raise legendary fish.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static bool IsLegalFishForPondsSubroutine(FishPond pond)
    {
        var who = Game1.getFarmerMaybeOffline(pond.owner.Value) ?? Game1.MasterPlayer;
        return who.HasProfession(Profession.Aquarist, true);
    }

    #endregion injected subroutines
}