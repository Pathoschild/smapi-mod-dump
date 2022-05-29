/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StopRugRemoval
**
*************************************************/

using AtraBase.Toolkit.StringHandler;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;

namespace StopRugRemoval.HarmonyPatches.Niceties;

/// <summary>
/// A patch to try to unfuck schedules.
/// I think this may be antisocial causing issues.
/// </summary>
[HarmonyPatch(typeof(NPC))]
internal static class ScheduleErrorFixer
{
    [HarmonyPriority(Priority.First)]
    [HarmonyPatch(nameof(NPC.parseMasterSchedule))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony Convention")]
    private static void Prefix(string rawData, NPC __instance)
    {
        if (__instance.currentLocation is not null || !__instance.isVillager())
        {
            return;
        }
        ModEntry.ModMonitor.Log($"{__instance.Name} seems to have a null current location, attempting to fix. Please inform their author! The current day is {SDate.Now()}, their attempted schedule string was {rawData}", LogLevel.Info);
        if (__instance.DefaultMap is not null && Game1.getLocationFromName(__instance.DefaultMap) is GameLocation location)
        { // Attempt to first just assign their position from their default map.
            __instance.currentLocation = location;
        }
        else if (Game1.content.Load<Dictionary<string, string>>(@"Data\NPCDispositions").TryGetValue(__instance.Name, out string? dispo)
            && dispo.SpanSplit('/').TryGetAtIndex(10, out SpanSplitEntry pos))
        { // Okay, if that didn't work, try getting from NPCDispositions.
            SpanSplit locParts = pos.SpanSplit();
            string defaultMap = locParts[0].ToString();
            if (Game1.getLocationFromName(defaultMap) is GameLocation loc)
            {
                __instance.DefaultMap = defaultMap;
                __instance.currentLocation = loc;
            }
            if (locParts.TryGetAtIndex(1, out SpanSplitEntry strX) && int.TryParse(strX, out int x)
                && locParts.TryGetAtIndex(2, out SpanSplitEntry strY) && int.TryParse(strY, out int y))
            {
                __instance.DefaultPosition = new Vector2(x * 64, y * 64);
                return;
            }
        }

        // Still no go, let's try parsing from the first schedule entry.
        if (__instance.currentLocation is null)
        {
            SpanSplit splits = rawData.SpanSplit();
            if (splits.TryGetAtIndex(1, out SpanSplitEntry locName) && Game1.getLocationFromName(locName.ToString()) is GameLocation loc)
            {
                __instance.currentLocation = loc;
                if (splits.TryGetAtIndex(2, out SpanSplitEntry strX) && int.TryParse(strX, out int x)
                    && splits.TryGetAtIndex(3, out SpanSplitEntry strY) && int.TryParse(strY, out int y))
                {
                    __instance.DefaultPosition = new Vector2(x * 64, y * 64);
                }
                return;
            }
        }
    }
}