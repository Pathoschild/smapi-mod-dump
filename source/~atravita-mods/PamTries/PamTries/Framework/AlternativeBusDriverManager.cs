/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraBase.Toolkit.Extensions;
using AtraShared.Utils.Extensions;
using StardewModdingAPI.Events;

namespace PamTries.Framework;

[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "StyleCop doesn't understand records.")]
public sealed record ScheduleData(int TimesPamDrivenThisWeek);

/// <summary>
/// Manages alternative bus drivers.
/// </summary>
internal static class AlternativeBusDriverManager
{
    private static readonly HashSet<string> busdrivers = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets a random bus driver from the alternative bus drivers list.
    /// </summary>
    /// <returns>Name of a possible bus driver.</returns>
    internal static string GetRandomBusDriver()
        => busdrivers.Count == 0 ? "Pam" : busdrivers.ElementAt(Game1.random.Next(busdrivers.Count));

    /// <summary>
    /// Listens to AssetReady to find valid bus drivers.
    /// </summary>
    /// <param name="e">event args.</param>
    internal static void MonitorSchedule(AssetReadyEventArgs e)
    {
        if (e.Name.IsDirectlyUnderPath("Characters/schedules"))
        {
            ReadOnlySpan<char> name = e.Name.BaseName.GetNthChunk('/', 2);
            if ((Game1.year < 2 && name.Equals("Kent", StringComparison.OrdinalIgnoreCase))
                || name.Equals("Pam", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            if (Game1.content.Load<Dictionary<string, string>>(e.Name.ToString()).ContainsKey("bus"))
            {
                if (busdrivers.Add(name.ToString()))
                {
                    ModEntry.ModMonitor.DebugOnlyLog($"Adding {name.ToString()} to possible bus drivers.", LogLevel.Debug);
                }
            }
            else
            {
                if (busdrivers.Remove(name.ToString()))
                {
                    ModEntry.ModMonitor.DebugOnlyLog($"Removing {name.ToString()} from possible bus drivers.", LogLevel.Debug);
                }
            }
        }
    }
}
