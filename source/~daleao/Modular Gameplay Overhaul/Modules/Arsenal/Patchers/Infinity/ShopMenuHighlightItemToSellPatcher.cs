/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Infinity;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ShopMenuHighlightItemToSellPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ShopMenuHighlightItemToSellPatcher"/> class.</summary>
    internal ShopMenuHighlightItemToSellPatcher()
    {
        this.Target = this.RequireMethod<ShopMenu>(nameof(ShopMenu.highlightItemToSell));
    }

    #region harmony patches

    /// <summary>Replace highlighting method.</summary>
    [HarmonyPostfix]
    private static void ShopMenuHighlightItemToSellPostfix(ref bool __result, Item i)
    {
        if (__result && i is MeleeWeapon weapon)
        {
            __result = WeaponTier.GetFor(weapon) < WeaponTier.Legendary;
        }
    }

    #endregion harmony patches
}
