/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using HarmonyLib;
using StardewValley.Objects;

namespace StopRugRemoval.HarmonyPatches.StorageFurnitureAdjustments;

/// <summary>
/// Patches against StorageFurniture (ie dressers).
/// </summary>
[HarmonyPatch(typeof(StorageFurniture))]
internal static class StorageFurniturePatches
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(StorageFurniture.ShowMenu))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony Convention")]
    private static void PrefixOpen(StorageFurniture __instance)
        => __instance.ClearNulls();

    [HarmonyPrefix]
    [HarmonyPatch(nameof(StorageFurniture.ShowChestMenu))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony Convention")]
    private static void PrefixChestOpen(StorageFurniture __instance)
        => __instance.ClearNulls();

#if DEBUG
    [HarmonyPrefix]
    [HarmonyPatch(nameof(StorageFurniture.checkForAction))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony Convention")]
    private static bool PrefixCheckedAction(StorageFurniture __instance)
    {
        if (ModEntry.Config.FurniturePlacementKey.IsDown() && Game1.player.ActiveObject is SObject obj && __instance.heldObject.Value is null)
        {
            Game1.player.ActiveObject = null;
            __instance.heldObject.Value = obj;
            return false;
        }
        return true;
    }
#endif
}