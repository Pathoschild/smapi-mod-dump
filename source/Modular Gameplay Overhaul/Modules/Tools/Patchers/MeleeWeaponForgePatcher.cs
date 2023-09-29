/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Tools.Patchers;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponForgePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponForgePatcher"/> class.</summary>
    internal MeleeWeaponForgePatcher()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.Forge));
    }

    #region harmony patches

    /// <summary>Allow enchanting Scythe.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? ToolForgeTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        // From: if (isScythe()) return false;
        // To: if (isScythe && !ModEntry.Config.Tools.Scythe.AllowHaymakerEnchantment) return false;
        return instructions.SkipWhile(instruction => instruction.opcode != OpCodes.Ldarg_1);
    }

    #endregion harmony patches
}
