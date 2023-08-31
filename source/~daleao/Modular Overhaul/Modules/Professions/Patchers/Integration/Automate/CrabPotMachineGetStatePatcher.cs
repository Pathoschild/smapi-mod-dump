/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Patchers.Integration;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
[ModRequirement("Pathoschild.Automate")]
internal sealed class CrabPotMachineGetStatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CrabPotMachineGetStatePatcher"/> class.</summary>
    internal CrabPotMachineGetStatePatcher()
    {
        this.Target = "Pathoschild.Stardew.Automate.Framework.Machines.Objects.CrabPotMachine"
            .ToType()
            .RequireMethod("GetState");
    }

    #region harmony patches

    /// <summary>Patch for conflicting Luremaster and Conservationist automation rules.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? CrabPotMachineGetStateTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Removed: || !this.PlayerNeedsBait()
        try
        {
            helper
                .Match(new[] { new CodeInstruction(OpCodes.Brtrue_S) })
                .CountUntil(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Call, "CrabPotMachine"
                            .ToType()
                            .RequireMethod("PlayerNeedsBait")),
                    },
                    out var count)
                .Remove(count)
                .SetOpCode(OpCodes.Brfalse_S);
        }
        catch (Exception ex)
        {
            Log.E("Professions module failed patching bait conditions for automated Crab Pots." +
                  $"\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
