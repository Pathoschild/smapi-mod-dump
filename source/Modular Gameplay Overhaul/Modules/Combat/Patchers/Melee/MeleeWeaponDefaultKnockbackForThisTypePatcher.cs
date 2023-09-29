/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.Patchers.Melee;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponDefaultKnockbackForThisTypePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponDefaultKnockbackForThisTypePatcher"/> class.</summary>
    internal MeleeWeaponDefaultKnockbackForThisTypePatcher()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.defaultKnockBackForThisType));
    }

    #region harmony patches

    /// <summary>Rebalance knockback for all weapons.</summary>
    [HarmonyPrefix]
    private static bool MeleeWeaponDefaultKnockbackForThisTypePrefix(MeleeWeapon __instance, ref float __result, int type)
    {
        if (!CombatModule.Config.EnableWeaponOverhaul)
        {
            return true; // run original logic
        }

        if (__instance.Name == "Diamond Wand")
        {
            __result = 31f;
            return false; // don't run original logic
        }

        __result = type switch
        {
            MeleeWeapon.dagger => 0.25f,
            MeleeWeapon.stabbingSword or MeleeWeapon.defenseSword=> 0.5f,
            MeleeWeapon.club => 1f,
            _ => -1f,
        };

        return false; // don't run original logic
    }

    #endregion harmony patches
}
