/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Ponds.Framework.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class InventoryMenuHoverPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="InventoryMenuHoverPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal InventoryMenuHoverPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<InventoryMenu>(nameof(InventoryMenu.hover));
    }

    #region harmony patches

    [HarmonyPostfix]
    private static void ItemGrabMenuCtorPostfix(InventoryMenu __instance, Item __result, Item heldItem)
    {
        if (__result is not null)
        {
            return;
        }

        return;
    }

    #endregion harmony patches
}
