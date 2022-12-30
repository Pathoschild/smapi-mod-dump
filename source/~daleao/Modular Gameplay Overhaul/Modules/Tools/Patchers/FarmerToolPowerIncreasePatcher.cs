/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Tools.Patchers;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerToolPowerIncreasePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerToolPowerIncreasePatcher"/> class.</summary>
    internal FarmerToolPowerIncreasePatcher()
    {
        this.Target = this.RequireMethod<Farmer>("toolPowerIncrease");
    }

    #region harmony patches

    /// <summary>Allow first two power levels on Pick.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> FarmerToolPowerIncreaseTranspiler(
        IEnumerable<CodeInstruction> instructions)
    {
        var l = instructions.ToList();
        for (var i = 0; i < l.Count; i++)
        {
            if (l[i].opcode != OpCodes.Isinst ||
                l[i].operand?.ToString() != "StardewValley.Tools.Pickaxe")
            {
                continue;
            }

            // inject branch over toolPower += 2
            l.Insert(i - 2, new CodeInstruction(OpCodes.Br_S, l[i + 1].operand));
            break;
        }

        return l.AsEnumerable();
    }

    #endregion harmony patches
}
