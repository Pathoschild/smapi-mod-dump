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

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ForgeMenuIsValidUnforgePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ForgeMenuIsValidUnforgePatcher"/> class.</summary>
    internal ForgeMenuIsValidUnforgePatcher()
    {
        this.Target = this.RequireMethod<ForgeMenu>(nameof(ForgeMenu.IsValidUnforge));
    }

    #region harmony patches

    /// <summary>Allow unforge Holy Blade.</summary>
    [HarmonyPostfix]
    private static void ForgeMenuIsValidUnforgePostfix(ForgeMenu __instance, ref bool __result)
    {
        if (__result)
        {
            return;
        }

        __result = __instance.leftIngredientSpot.item is MeleeWeapon
        {
            InitialParentTileIndex: ItemIDs.HolyBlade
        } weapon && weapon.GetTotalForgeLevels() <= 0;
    }

    #endregion harmony patches
}
