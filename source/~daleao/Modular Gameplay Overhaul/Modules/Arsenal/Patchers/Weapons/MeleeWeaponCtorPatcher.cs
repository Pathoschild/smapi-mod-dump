/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Weapons;

#region using directives

using DaLion.Overhaul.Modules.Arsenal.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponCtorPatcher"/> class.</summary>
    internal MeleeWeaponCtorPatcher()
    {
        this.Target = this.RequireConstructor<MeleeWeapon>(typeof(int));
    }

    #region harmony patches

    /// <summary>Add intrinsic enchants.</summary>
    [HarmonyPostfix]
    private static void MeleeWeaponCtorPostfix(MeleeWeapon __instance)
    {
        if (ArsenalModule.Config.Weapons.EnableRebalance &&
            __instance.InitialParentTileIndex == Constants.InsectHeadIndex)
        {
            __instance.type.Value = MeleeWeapon.dagger;
            __instance.specialItem = true;
            return;
        }

        if (ArsenalModule.Config.Weapons.EnableStabbySwords &&
            Collections.StabbingSwords.Contains(__instance.InitialParentTileIndex))
        {
            __instance.type.Value = MeleeWeapon.stabbingSword;
        }

        __instance.AddIntrinsicEnchantments();
    }

    #endregion harmony patches
}
