/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.Automate;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Stardew.Common.Harmony;
using HarmonyLib;
using JetBrains.Annotations;

using Stardew.Common.Extensions;

#endregion using directives

[UsedImplicitly]
internal class CrabPotMachineGetStatePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal CrabPotMachineGetStatePatch()
    {
        try
        {
            Original = "Pathoschild.Stardew.Automate.Framework.Machines.Objects.CrabPotMachine".ToType()
                .MethodNamed("GetState");
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Patch for conflicting Luremaster and Conservationist automation rules.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> CrabPotMachineGetStateTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Removed: || !this.PlayerNeedsBait()

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Brtrue_S)
                )
                .RemoveUntil(
                    new CodeInstruction(OpCodes.Call, "CrabPotMachine".ToType().MethodNamed("PlayerNeedsBait"))
                )
                .SetOpCode(OpCodes.Brfalse_S);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching bait conditions for automated Crab Pots.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}