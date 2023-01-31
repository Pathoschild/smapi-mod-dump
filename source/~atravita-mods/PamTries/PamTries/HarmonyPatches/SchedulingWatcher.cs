/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraShared.Utils.Extensions;

using HarmonyLib;

using StardewModdingAPI.Utilities;

namespace PamTries.HarmonyPatches;

/// <summary>
/// Watches the scheduler to check if Pam's driving the bus today.
/// </summary>
[HarmonyPatch(typeof(NPC))]
internal static class SchedulingWatcher
{
    /// <summary>
    /// Gets a value indicating whether or not Pam drove the bus today.
    /// </summary>
    internal static bool DidPamDriveBusToday { get; private set; } = false;

    [HarmonyPatch(nameof(NPC.parseMasterSchedule))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention")]
    private static void Postfix(NPC __instance)
    {
        try
        {
            if (Context.IsMainPlayer && __instance?.Name.Equals("Pam", StringComparison.OrdinalIgnoreCase) == true
                && __instance.TryGetScheduleEntry(__instance.dayScheduleName.Value, out string? rawData))
            {
                DidPamDriveBusToday = ModEntry.ScheduleUtilityFunctions.TryFindGOTOschedule(__instance, SDate.Now(), rawData, out var redirected)
                    && redirected.Contains("BusStop 11 10");
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Error in postfixing get master schedule to get Pam's schedule.\n\n{ex}", LogLevel.Error);
        }
    }
}
