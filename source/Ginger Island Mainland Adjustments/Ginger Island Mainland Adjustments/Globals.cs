/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/Ginger-Island-Mainland-Adjustments
**
*************************************************/

using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace GingerIslandMainlandAdjustments;

/// <summary>
/// Class to handle global variables.
/// </summary>
internal static class Globals
{
    // Values are set in the Mod.Entry method, so should never be null.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    /// <summary>
    /// Gets SMAPI's logging service.
    /// </summary>
    internal static IMonitor ModMonitor { get; private set; }

    /// <summary>
    /// Gets or sets mod configuration class.
    /// </summary>
    internal static ModConfig Config { get; set; }

    /// <summary>
    /// Gets SMAPI's reflection helper.
    /// </summary>
    internal static IReflectionHelper ReflectionHelper { get; private set; }

    /// <summary>
    /// Gets SMAPI's Content helper.
    /// </summary>
    internal static IContentHelper ContentHelper { get; private set; }

    /// <summary>
    /// Gets SMAPI's mod registry helper.
    /// </summary>
    internal static IModRegistry ModRegistry { get; private set; }

    /// <summary>
    /// Gets SMAPI's helper class.
    /// </summary>
    internal static IModHelper Helper { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    /// <summary>
    /// Regex for a schedulepoint format.
    /// </summary>
    [RegexPattern]
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "Reviewed")]
    internal static readonly Regex ScheduleRegex = new(
        // <time> [location] <tileX> <tileY> [facingDirection] [animation] \"[dialogue]\"
        pattern: @"(?<arrival>a)?(?<time>\d{1,4})(?<location> \S+)*?(?<x> \d{1,4})(?<y> \d{1,4})(?<direction> \d)?(?<animation> [^\s\""]+)?(?<dialogue> \"".*\"")?",
        options: RegexOptions.CultureInvariant | RegexOptions.Compiled,
        new TimeSpan(1000000));

    /// <summary>
    /// Initialize globals, including reading config file.
    /// </summary>
    /// <param name="helper">SMAPI's IModHelper.</param>
    /// <param name="monitor">SMAPI's logging service.</param>
    internal static void Initialize(IModHelper helper, IMonitor monitor)
    {
        Globals.ModMonitor = monitor;
        Globals.ReflectionHelper = helper.Reflection;
        Globals.ContentHelper = helper.Content;
        Globals.ModRegistry = helper.ModRegistry;
        Globals.Helper = helper;

        try
        {
            Globals.Config = helper.ReadConfig<ModConfig>();
        }
        catch
        {
            Globals.ModMonitor.Log(I18n.IllFormatedConfig(), LogLevel.Warn);
            Globals.Config = new();
        }
    }
}
