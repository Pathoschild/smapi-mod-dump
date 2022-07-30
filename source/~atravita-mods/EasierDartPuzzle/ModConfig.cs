/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraShared.Integrations.GMCMAttributes;

namespace EasierDartPuzzle;

#pragma warning disable SA1201 // Elements should appear in the correct order. Backing fields kept near accessors.

/// <summary>
/// The config class for this mod.
/// </summary>
internal sealed class ModConfig
{
    private int mpPirateArrivalTime = 1600;

    /// <summary>
    /// Gets or sets when the pirates should show up in multiplayer.
    /// </summary>
    [GMCMInterval(10)]
    [GMCMRange(600, 2000)]
    public int MPPirateArrivalTime
    {
        get => this.mpPirateArrivalTime;
        set => this.mpPirateArrivalTime = (Math.Clamp(value, 600, 2000) / 10) * 10;
    }

    private int minDartCount = 10;

    /// <summary>
    /// Gets or sets a value indicating the minimum dart requirement.
    /// </summary>
    [GMCMRange(6, 30)]
    public int MinDartCount
    {
        get => this.minDartCount;
        set => this.minDartCount = Math.Clamp(value, 6, 30);
    }

    private int maxDartCount = 20;

    /// <summary>
    /// Gets or sets a value indicating the max dart requirement.
    /// </summary>
    [GMCMRange(6, 30)]
    public int MaxDartCount
    {
        get => this.maxDartCount;
        set => this.maxDartCount = Math.Clamp(value, 6, 30);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the dart location should just be drawn to the screen.
    /// </summary>
    public bool ShowDartMarker { get; set; } = false;

    private float jitterMultiplier = 1f;

    /// <summary>
    /// Gets or sets a value indicating how much the dart should jitter.
    /// </summary>
    [GMCMInterval(0.01)]
    [GMCMRange(0.05, 20)]
    public float JitterMultiplier
    {
        get => this.jitterMultiplier;
        set => this.jitterMultiplier = Math.Clamp(value, 0.05f, 20f);
    }

    private float dartPrecision = 1f;

    [GMCMRange(0.5, 5)]
    [GMCMInterval(0.01)]
    public float DartPrecision
    {
        get => this.dartPrecision;
        set => this.dartPrecision = Math.Clamp(value, 0.5f, 5f);
    }
}
#pragma warning restore SA1201 // Elements should appear in the correct order