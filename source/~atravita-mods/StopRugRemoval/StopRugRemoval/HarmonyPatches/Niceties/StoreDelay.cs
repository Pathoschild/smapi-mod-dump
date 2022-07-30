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
using StardewModdingAPI.Utilities;
using StardewValley.Menus;

namespace StopRugRemoval.HarmonyPatches.Niceties;

/// <summary>
/// Holds patches that prevent the store from accepting input too quickly.
/// I kept on accidentally buying things.
/// </summary>
[HarmonyPatch(typeof(ShopMenu))]
internal static class StoreDelay
{
    private const int TICKS_TO_DELAY = 20;
    private static readonly PerScreen<int> OpenedTick = new(() => 0);

    [HarmonyPatch(nameof(ShopMenu.setUpStoreForContext))]
    private static void Postfix()
        => OpenedTick.Value = Game1.ticks;

    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    [HarmonyPatch(nameof(ShopMenu.receiveLeftClick))]
    private static bool PrefixLeftClick()
        => OpenedTick.Value + TICKS_TO_DELAY < Game1.ticks;

    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    [HarmonyPatch(nameof(ShopMenu.receiveRightClick))]
    private static bool PrefixRightClick()
        => OpenedTick.Value + TICKS_TO_DELAY < Game1.ticks;

    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    [HarmonyPatch(nameof(ShopMenu.receiveKeyPress))]
    private static bool PrefixKeyPress()
        => OpenedTick.Value + TICKS_TO_DELAY < Game1.ticks;
}
