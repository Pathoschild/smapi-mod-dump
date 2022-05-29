/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/GiantCropFertilizer
**
*************************************************/

namespace GiantCropFertilizer;

/// <summary>
/// The config class for this mod.
/// </summary>
public class ModConfig
{
    private double giantCropChance = 1.1d;

    /// <summary>
    /// Gets or sets the probability of a fertilized square producing a giant crop.
    /// </summary>
    public double GiantCropChance
    {
        get => this.giantCropChance;
        set => this.giantCropChance = Math.Clamp(value, 0, 1.1d);
    }

    public bool AllowGiantCropsOffFarm { get; set; } = false;
}