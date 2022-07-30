/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

#if DEBUG

using System.Reflection;
using System.Reflection.Emit;
using AtraBase.Toolkit.Reflection;
using AtraShared.Utils.Extensions;
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Locations;

namespace PamTries.HarmonyPatches;

[HarmonyPatch(typeof(GameLocation))]
internal class BusDriverSchedulePatch
{

    /// <summary>
    /// Gets or sets the current bus driver.
    /// </summary>
    internal static string CurrentDriver { get; set; } = "Pam";

    internal static string GetCurrentDriver() => CurrentDriver;

    [HarmonyPatch(nameof(GameLocation.busLeave))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony Convention.")]
    private static bool Prefix(GameLocation __instance)
    {
        ModEntry.ModMonitor.Log("Reached BusLeave!", LogLevel.Alert);
        NPC? driver = __instance.getCharacterFromName(CurrentDriver);
        if (driver is null)
        {
            ModEntry.ModMonitor.Log($"Driver {CurrentDriver} is not found!", LogLevel.Error);
            if (__instance is BusStop || __instance.Name.Equals("BusStop", StringComparison.OrdinalIgnoreCase))
            {
                Game1.warpFarmer("Desert", 32, 27, flip: true);
            }
            else
            {
                Game1.warpFarmer("BusStop", 9, 9, flip: true);
            }
            return false;
        }
        return false;
    }
}

#endif