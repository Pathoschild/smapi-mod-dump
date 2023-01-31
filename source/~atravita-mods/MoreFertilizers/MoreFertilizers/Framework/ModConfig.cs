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
using Microsoft.Xna.Framework;

namespace MoreFertilizers.Framework;

/// <summary>
/// The configuration class for this mod.
/// </summary>
internal sealed class ModConfig
{
    private bool banRaisedSeeds = false;

    /// <summary>
    /// Gets or sets a value indicating whether or not the mill should produce organic goods.
    /// </summary>
    public bool MillProducesOrganic { get; set; } = true;

    /// <summary>
    /// Gets or sets a value for what color to make the water overlay for fish food.
    /// </summary>
    [GMCMDefaultColor(147, 112, 219, 155)]
    public Color WaterOverlayColor { get; set; } = new(147, 112, 219, 155);

    /// <summary>
    /// Gets or sets a value indicating whether or not bushes should change color to indicate whether or not they've been fertilized.
    /// </summary>
    public bool RecolorBushes { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether fruit trees should be recolored when they're fertilized.
    /// </summary>
    public bool RecolorFruitTrees { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether trees should be recolored when they're fertilized.
    /// </summary>
    public bool RecolorTrees { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether or not raised seeds should be banned from the radioactive fertilizer.
    /// </summary>
    public bool BanRaisedSeeds
    {
        get => this.banRaisedSeeds;
        set
        {
            if (value != this.banRaisedSeeds)
            {
                RadioactiveFertilizerHandler.Reset();
            }
            this.banRaisedSeeds = value;
        }
    }
}