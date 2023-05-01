/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Weapons.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Weapons.Extensions;
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

    /// <summary>Convert stabby swords + add intrinsic enchants.</summary>
    [HarmonyPostfix]
    private static void MeleeWeaponCtorPostfix(MeleeWeapon __instance)
    {
        if (__instance.ShouldBeStabbySword())
        {
            __instance.type.Value = MeleeWeapon.stabbingSword;
            Log.D($"The type of {__instance.Name} was converted to Stabbing sword.");
        }

        if (WeaponsModule.Config.EnableRebalance)
        {
            if (__instance.InitialParentTileIndex == ItemIDs.InsectHead)
            {
                __instance.type.Value = MeleeWeapon.dagger;
            }

            if (__instance.InitialParentTileIndex is ItemIDs.NeptuneGlaive)
            {
                __instance.specialItem = true;
            }

            __instance.AddIntrinsicEnchantments();
        }

        if (WeaponsModule.Config.InfinityPlusOne)
        {
            if (__instance.isGalaxyWeapon() || __instance.IsInfinityWeapon() || __instance.IsCursedOrBlessed())
            {
                __instance.specialItem = true;
            }
        }
    }

    #endregion harmony patches
}
