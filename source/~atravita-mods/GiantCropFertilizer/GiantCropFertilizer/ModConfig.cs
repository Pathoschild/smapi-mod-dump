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

namespace GiantCropFertilizer;

/// <summary>
/// The config class for this mod.
/// </summary>
internal sealed class ModConfig
{
    private double giantCropChance = 1.1d;

    /// <summary>
    /// Gets or sets the probability of a fertilized square producing a giant crop.
    /// </summary>
    [GMCMRange(0, 1.1)]
    [GMCMInterval(0.01)]
    public double GiantCropChance
    {
        get => this.giantCropChance;
        set => this.giantCropChance = Math.Clamp(value, 0, 1.1d);
    }

    /// <summary>
    /// Gets or sets a value indicating whether giant crops should be allowed off-farm.
    /// </summary>
    public bool AllowGiantCropsOffFarm { get; set; } = false;
}