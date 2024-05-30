/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FlyingTNT/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Netcode;
using StardewModdingAPI.Utilities;
using StardewValley;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace LongerSeasons
{
    /// <summary>The mod entry point.</summary>
    public partial class ModEntry
    {
        private static bool WorldDate_TotalDays_Getter_Prefix(WorldDate __instance, ref int __result)
        {
            __result = ((__instance.Year - 1) * 4 * Config.MonthsPerSeason + __instance.SeasonIndex) * Config.DaysPerMonth + (__instance.DayOfMonth - 1);
            return false;
        }
        private static bool WorldDate_TotalDays_Setter_Prefix(WorldDate __instance, ref int value)
        {
            int totalMonths = value / Config.DaysPerMonth;
            __instance.DayOfMonth = value % Config.DaysPerMonth + 1;
            ((NetInt)AccessTools.Field(typeof(WorldDate), "seasonIndex").GetValue(__instance)).Value = totalMonths % (4 * Config.MonthsPerSeason);
            __instance.Year = totalMonths / (4 * Config.MonthsPerSeason) + 1;
            return false;
        }
    }
}