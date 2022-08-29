/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Buildings;
using StardewValley.Menus;
using System.Collections.Generic;

#endregion using directives

[UsedImplicitly]
internal sealed class ItemGrabMenuCtorPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal ItemGrabMenuCtorPatch()
    {
        Target = RequireConstructor<ItemGrabMenu>(typeof(List<Item>), typeof(object));
    }

    #region harmony patches

    /// <summary>Update ItemsHeld data on grab menu close.</summary>
    [HarmonyPostfix]
    private static void ItemGrabMenuCtorPostfix(ItemGrabMenu __instance)
    {
        if (__instance.context is FishPond) __instance.canExitOnKey = true;
    }

    #endregion harmony patches
}