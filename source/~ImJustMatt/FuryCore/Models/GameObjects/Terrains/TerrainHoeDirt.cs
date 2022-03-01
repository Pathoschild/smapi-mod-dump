/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.FuryCore.Models.GameObjects.Terrains;

using StardewValley;
using StardewValley.TerrainFeatures;

/// <inheritdoc />
internal class TerrainHoeDirt : BaseTerrain
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="TerrainHoeDirt" /> class.
    /// </summary>
    /// <param name="hoeDirt">The source hoe dirt.</param>
    public TerrainHoeDirt(HoeDirt hoeDirt)
        : base(hoeDirt)
    {
        this.HoeDirt = hoeDirt;
    }

    /// <inheritdoc />
    public override ModDataDictionary ModData
    {
        get => this.HoeDirt.modData;
    }

    private HoeDirt HoeDirt { get; }

    /// <inheritdoc />
    public override bool CanHarvest()
    {
        return this.HoeDirt.crop.currentPhase.Value >= this.HoeDirt.crop.phaseDays.Count - 1 && (!this.HoeDirt.crop.fullyGrown.Value || this.HoeDirt.crop.dayOfCurrentPhase.Value <= 0);
    }

    /// <inheritdoc />
    public override bool TryHarvest()
    {
        return this.HoeDirt.performUseAction(this.HoeDirt.currentTileLocation, this.HoeDirt.currentLocation);
    }
}