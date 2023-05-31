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

using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;

#endregion using directives

[UsedImplicitly]
[RequiresMod("PeacefulEnd.Archery", "Archery", "2.1.0")]
internal sealed class ChargeTimeRequiredMillisecondsGetterPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ChargeTimeRequiredMillisecondsGetterPatcher"/> class.</summary>
    internal ChargeTimeRequiredMillisecondsGetterPatcher()
    {
        this.Target = "Archery.Framework.Models.Weapons.WeaponModel"
            .ToType()
            .RequirePropertyGetter("ChargeTimeRequiredMilliseconds");
        this.Postfix!.after = new[] { OverhaulModule.Slingshots.Namespace };
    }

    #region harmony patches

    /// <summary>Patch to reduce Bow charge time for Desperado.</summary>
    [HarmonyPostfix]
    [HarmonyAfter("DaLion.Overhaul.Modules.Slingshots")]
    private static void ChargeTimeRequiredMillisecondsGetterPatcherPostfix(ref float __result)
    {
        var player = Game1.player;
        if (player.HasProfession(Profession.Desperado))
        {
            __result *= 1f - MathHelper.Lerp(0f, 0.5f, Math.Clamp(1f - ((float)player.health / player.maxHealth), 0f, 1f));
        }
    }

    #endregion harmony patches
}
