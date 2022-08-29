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

using Events;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponAnimateSpecialMovePatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal MeleeWeaponAnimateSpecialMovePatch()
    {
        Target = RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.animateSpecialMove));
    }

    #region harmony patches

    /// <summary>Trigger stabby sword lunge.</summary>
    [HarmonyPrefix]
    private static bool MeleeWeaponAnimateSpecalMovePrefix(MeleeWeapon __instance, ref Farmer ___lastUser, Farmer who)
    {
        if (__instance.type.Value != MeleeWeapon.stabbingSword || MeleeWeapon.attackSwordCooldown > 0 ||
            !ModEntry.Config.BringBackStabbySwords) return true; // run original logic

        ___lastUser = who;
        ModEntry.Events.Enable<StabbySwordSpecialUpdateTickingEvent>();
        return false; // don't run original logic
    }

    #endregion harmony patches
}