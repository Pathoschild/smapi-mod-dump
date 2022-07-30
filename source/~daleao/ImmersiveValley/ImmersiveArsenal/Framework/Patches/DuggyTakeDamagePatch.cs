/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

using HarmonyLib;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

namespace DaLion.Stardew.Arsenal.Framework.Patches;

internal sealed class DuggyTakeDamagePatch : HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal DuggyTakeDamagePatch()
    {
        Target = RequireMethod<ForgeMenu>("_ValidateCraft");
    }

    #region harmony patches

    /// <summary>Double damage to duggy from Club smash attack.</summary>
    [HarmonyPrefix]
    private static void DuggyTakeDamagePrefix(ref int damage, Farmer who)
    {
        if (who.CurrentTool is MeleeWeapon {type.Value: MeleeWeapon.club, isOnSpecial: true})
            damage *= 2;
    }

    #endregion harmony patches
}