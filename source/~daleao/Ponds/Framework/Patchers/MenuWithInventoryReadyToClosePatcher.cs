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

using System.Linq;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class MenuWithInventoryReadyToClosePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MenuWithInventoryReadyToClosePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal MenuWithInventoryReadyToClosePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<MenuWithInventory>(nameof(MenuWithInventory.readyToClose));
    }

    #region harmony patches

    /// <summary>Update ItemsHeld data on grab menu close.</summary>
    [HarmonyPostfix]
    private static void MenuWithInventoryToClosePostfix(MenuWithInventory __instance, ref bool __result)
    {
        if (__instance is not ItemGrabMenu grabMenu || grabMenu.context is not FishPond pond)
        {
            return;
        }

        var inventory = grabMenu.ItemsToGrabMenu?.actualInventory.WhereNotNull().ToList();
        if (inventory?.Count is not > 0)
        {
            Data.Write(pond, DataKeys.ItemsHeld, null);
            pond.output.Value = null;
            return;
        }

        var output = inventory
            .OrderByDescending(i => i is ColoredObject
                ? ItemRegistry.Create<SObject>(i.QualifiedItemId).salePrice()
                : i.salePrice())
            .First() as SObject;
        inventory.Remove(output!);
        if (inventory.Count > 0)
        {
            var serialized = inventory
                .Select(i => $"{i.QualifiedItemId},{i.Stack},{((SObject)i).Quality}");
            Data.Write(pond, DataKeys.ItemsHeld, string.Join(';', serialized));
        }
        else
        {
            Data.Write(pond, DataKeys.ItemsHeld, null);
        }

        pond.output.Value = output;
        __result = true; // ready to close
    }

    #endregion harmony patches
}
