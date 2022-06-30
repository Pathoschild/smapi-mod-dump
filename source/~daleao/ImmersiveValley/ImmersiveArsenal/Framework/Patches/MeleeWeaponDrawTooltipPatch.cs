/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using Common;
using Common.Harmony;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponDrawTooltipPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal MeleeWeaponDrawTooltipPatch()
    {
        Target = RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.drawTooltip));
    }

    #region harmony patches

    /// <summary>Make weapon stats human-readable..</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? MeleeWeaponDrawTooltipTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            throw new NotImplementedException("Please implement.");
        }
        catch (Exception ex)
        {
            Log.E($"Failed xxx.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}