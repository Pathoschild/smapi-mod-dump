/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Crafting;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerCouldInventoryAcceptThisItemPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerCouldInventoryAcceptThisItemPatcher"/> class.</summary>
    internal FarmerCouldInventoryAcceptThisItemPatcher()
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.couldInventoryAcceptThisItem));
    }

    #region harmony patches

    /// <summary>Allow picking up blueprints.</summary>
    [HarmonyPrefix]
    private static bool FarmerCouldInventoryAcceptThisItemPrefix(ref bool __result, Item item)
    {
        if (!Globals.DwarvishBlueprintIndex.HasValue || Globals.DwarvishBlueprintIndex.Value != item.ParentSheetIndex)
        {
            return true; // run original logic
        }

        __result = true;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
