/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Weapons;

#region using directives

using DaLion.Overhaul.Modules.Arsenal.Events;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponAnimateSpecialMovePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponAnimateSpecialMovePatcher"/> class.</summary>
    internal MeleeWeaponAnimateSpecialMovePatcher()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.animateSpecialMove));
    }

    #region harmony patches

    /// <summary>Trigger Stabbing Sword lunge.</summary>
    [HarmonyPrefix]
    private static bool MeleeWeaponAnimateSpecalMovePrefix(MeleeWeapon __instance, ref Farmer ___lastUser, Farmer who)
    {
        if (__instance.isScythe() || __instance.type.Value != MeleeWeapon.stabbingSword || MeleeWeapon.attackSwordCooldown > 0)
        {
            return true; // run original logic
        }

        ___lastUser = who;
        EventManager.Enable<StabbingSwordSpecialUpdateTickingEvent>();
        return false; // don't run original logic
    }

    #endregion harmony patches
}
