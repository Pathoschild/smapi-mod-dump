/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul;

#region using directives

using StardewValley.Monsters;

#endregion using directives

/// <summary>Holds global variables that may be used by different modules.</summary>
internal sealed class Globals
{
    /// <summary>Gets or sets <see cref="Item"/> index of the Garnet gemstone (provided by Json Assets).</summary>
    internal static int? GarnetIndex { get; set; }

    /// <summary>Gets or sets <see cref="Item"/> index of the Garnet Ring (provided by Json Assets).</summary>
    internal static int? GarnetRingIndex { get; set; }

    /// <summary>Gets or sets <see cref="Item"/> index of the Infinity Band (provided by Json Assets).</summary>
    internal static int? InfinityBandIndex { get; set; }

    /// <summary>Gets or sets <see cref="Item"/> index of the Hero Soul (provided by Dynamic Game Assets).</summary>
    internal static int? HeroSoulIndex { get; set; }

    /// <summary>Gets or sets <see cref="Item"/> index of Dwarven Scrap (provided by Dynamic Game Assets).</summary>
    internal static int? DwarvenScrapIndex { get; set; }

    /// <summary>Gets or sets <see cref="Item"/> index of Elderwood (provided by Dynamic Game Assets).</summary>
    internal static int? ElderwoodIndex { get; set; }

    /// <summary>Gets or sets <see cref="Item"/> index of Dwarvish weapon blueprints (provided by Dynamic Game Assets).</summary>
    internal static int? DwarvishBlueprintIndex { get; set; }

    /// <summary>Gets or sets a value indicating whether the current location has any characters of type <see cref="Monster"/>.</summary>
    internal static bool AreEnemiesAround { get; set; }

    /// <summary>Gets or sets the number of elapsed seconds since the last combat-related action.</summary>
    internal static int SecondsOutOfCombat { get; set; }

    /// <summary>Gets or sets the <see cref="FrameRateCounter"/>.</summary>
    internal static FrameRateCounter? FpsCounter { get; set; }

    /// <summary>Gets or sets the latest position of the cursor.</summary>
    internal static ICursorPosition? DebugCursorPosition { get; set; }
}
