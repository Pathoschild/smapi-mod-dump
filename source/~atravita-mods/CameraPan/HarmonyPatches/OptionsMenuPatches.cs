/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Reflection;

using AtraCore.Framework.ReflectionManager;

using HarmonyLib;

using StardewValley.Menus;

namespace CameraPan.HarmonyPatches;

/// <summary>
/// Patches on the options menu.
/// </summary>
[HarmonyPatch(typeof(DayTimeMoneyBox))]
internal static class OptionsMenuPatches
{
    /// <summary>
    /// Updates the position of our button in relation to the day time money box if needed.
    /// </summary>
    [HarmonyPatch("updatePosition")]
    private static void Postfix() => ModEntry.CreateOrModifyButton();
}
