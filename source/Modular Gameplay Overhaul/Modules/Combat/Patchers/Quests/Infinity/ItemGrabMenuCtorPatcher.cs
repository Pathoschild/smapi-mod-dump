/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.Patchers.Quests.Infinity;

#region using directives

using DaLion.Shared.Constants;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ItemGrabMenuCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ItemGrabMenuCtorPatcher"/> class.</summary>
    internal ItemGrabMenuCtorPatcher()
    {
        this.Target = typeof(ItemGrabMenu).RequireConstructor(16);
    }

    #region harmony patches

    /// <summary>Replace highlighting method.</summary>
    [HarmonyPrefix]
    private static void ItemGrabMenuCtorPrefix(ItemGrabMenu __instance, ref InventoryMenu.highlightThisItem? highlightFunction)
    {
        if (!CombatModule.Config.Quests.CanStoreRuinBlade &&
            __instance.GetType().FullName?.Contains("CJBItemSpawner") == false &&
            highlightFunction?.Method.Name != "highlightShippableObjects" &&
            highlightFunction?.Method.DeclaringType?.Name.Contains("Shipping") == false)
        {
            highlightFunction = HighlightAllButDarkSword;
        }
    }

    #endregion harmony patches

    #region injected subroutines

    private static bool HighlightAllButDarkSword(Item i)
    {
        return i is not MeleeWeapon { InitialParentTileIndex: WeaponIds.DarkSword };
    }

    #endregion injected subroutines
}
