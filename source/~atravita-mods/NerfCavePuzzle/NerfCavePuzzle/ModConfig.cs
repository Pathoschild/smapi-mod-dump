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

namespace NerfCavePuzzle;

/// <summary>
/// The config class for this mod.
/// </summary>
internal sealed class ModConfig
{
    private float speedModifier = 1f;
    private float flashScale = 1f;
    private int maxNotes = 7;

    /// <summary>
    /// Gets or sets the speed modifer.
    /// </summary>
    [GMCMRange(0.1, 10)]
    public float SpeedModifer
    {
        get => this.speedModifier;
        set => this.speedModifier = Math.Clamp(value, 0.1f, 10f);
    }

    /// <summary>
    /// Gets or sets the scaling factor for the flash speed.
    /// </summary>
    [GMCMRange(0.1, 10)]
    public float FlashScale
    {
        get => this.flashScale;
        set => this.flashScale = Math.Clamp(value, 0.1f, 10f);
    }

    /// <summary>
    /// Gets or sets a value that caps the maximum amount of notes.
    /// </summary>
    [GMCMRange(4, 7)]
    public int MaxNotes
    {
        get => this.maxNotes;
        set => this.maxNotes = Math.Clamp(value, 4, 7);
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not to pause between rounds.
    /// </summary>
    public bool PauseBetweenRounds { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether or not re-asking should be enabled.
    /// </summary>
    public bool AllowReAsks { get; set; } = true;
}