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
internal class TerrainBush : BaseTerrain
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="TerrainBush" /> class.
    /// </summary>
    /// <param name="bush">The source bush.</param>
    public TerrainBush(Bush bush)
        : base(bush)
    {
        this.Bush = bush;
    }

    /// <inheritdoc />
    public override ModDataDictionary ModData
    {
        get => this.Bush.modData;
    }

    private Bush Bush { get; }

    /// <inheritdoc />
    public override bool CanHarvest()
    {
        return !this.Bush.townBush.Value && this.Bush.tileSheetOffset.Value == 1 && this.Bush.inBloom(Game1.GetSeasonForLocation(this.Bush.currentLocation), Game1.dayOfMonth);
    }

    /// <inheritdoc />
    public override bool TryHarvest()
    {
        return this.Bush.performUseAction(this.Bush.tilePosition.Value, this.Bush.currentLocation);
    }
}