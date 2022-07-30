/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Common;

#region using directives

using DaLion.Common;
using DaLion.Common.Harmony;
using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly]
internal sealed class Game1DrawHUDPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal Game1DrawHUDPatch()
    {
        Target = RequireMethod<Game1>("drawHUD");
    }

    #region harmony patches

    /// <summary>Patch for Scavenger and Prospector to track different stuff.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? Game1DrawHUDTranspiler(IEnumerable<CodeInstruction> instructions,
        MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Removed:
        ///     From: if (!player.professions.Contains(<scavenger_id>)
        ///     Until: end ...

        try
        {
            helper
               .FindProfessionCheck(Farmer.tracker) // find index of tracker check
               .Retreat()
               .GetLabels(out var leave) // the exception block leave opcode destination
               .GoTo(helper.LastIndex)
               .GetLabels(out var labels) // get the labels of the final return instruction
               .Return()
               .RemoveUntil( // remove everything after the profession check
                   new CodeInstruction(OpCodes.Ret)
               )
               .AddWithLabels( // add back a new return statement
                   labels.Take(2).Concat(leave).ToArray(), // exclude the labels defined after the profession check
                   new CodeInstruction(OpCodes.Ret)
               );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while removing vanilla Tracker behavior.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}