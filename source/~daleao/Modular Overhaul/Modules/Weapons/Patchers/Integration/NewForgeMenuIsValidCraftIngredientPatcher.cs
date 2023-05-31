/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Weapons.Patchers.Integration;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Harmony;
using HarmonyLib;
using SpaceCore.Interface;

#endregion using directives

[UsedImplicitly]
[RequiresMod("spacechase0.SpaceCore")]
internal sealed class NewForgeMenuIsValidCraftIngredientPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="NewForgeMenuIsValidCraftIngredientPatcher"/> class.</summary>
    internal NewForgeMenuIsValidCraftIngredientPatcher()
    {
        this.Target = this.RequireMethod<NewForgeMenu>(nameof(NewForgeMenu.IsValidCraftIngredient));
    }

    #region harmony patches

    /// <summary>Allow forging with Hero Soul.</summary>
    [HarmonyPostfix]
    private static void NewForgeMenuIsValidCraftIngredientPostfix(ref bool __result, Item item)
    {
        if (Globals.HeroSoulIndex.HasValue && item.ParentSheetIndex == Globals.HeroSoulIndex.Value)
        {
            __result = true;
        }
    }

    #endregion harmony patches
}
