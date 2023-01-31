/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Integrations;

#region using directives

using System.Reflection;
using DaLion.Shared.Attributes;
using DaLion.Shared.Harmony;
using HarmonyLib;
using SpaceCore.Interface;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
[RequiresMod("spacechase0.SpaceCore")]
internal sealed class NewForgeMenuSpendRightItemPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="NewForgeMenuSpendRightItemPatcher"/> class.</summary>
    internal NewForgeMenuSpendRightItemPatcher()
    {
        this.Target = this.RequireMethod<NewForgeMenu>(nameof(NewForgeMenu.SpendRightItem));
    }

    #region harmony patches

    /// <summary>Prevent spending Hero Soul.</summary>
    [HarmonyPrefix]
    private static bool NewForgeMenuSpendRightItemPrefix(NewForgeMenu __instance)
    {
        try
        {
            return !Globals.HeroSoulIndex.HasValue || __instance.rightIngredientSpot.item?.ParentSheetIndex != Globals.HeroSoulIndex.Value;
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
