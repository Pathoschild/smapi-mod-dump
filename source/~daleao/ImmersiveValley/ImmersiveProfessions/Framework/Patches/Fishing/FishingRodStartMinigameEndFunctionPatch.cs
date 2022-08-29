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
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly]
internal sealed class FishingRodStartMinigameEndFunctionPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal FishingRodStartMinigameEndFunctionPatch()
    {
        Target = RequireMethod<FishingRod>(nameof(FishingRod.startMinigameEndFunction));
    }

    #region harmony patches

    /// <summary>Patch to remove Pirate bonus treasure chance.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? FishingRodStartMinigameEndFunctionTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Removed: lastUser.professions.Contains(<pirate_id>) ? baseChance ...

        try
        {
            helper // find index of pirate check
                .FindProfessionCheck(Farmer.pirate)
                .Retreat(2)
                .RemoveUntil(
                    new CodeInstruction(OpCodes.Add) // remove this check
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while removing vanilla Pirate bonus treasure chance.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}