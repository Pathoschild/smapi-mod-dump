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
internal class TerrainFruitTree : BaseTerrain
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="TerrainFruitTree" /> class.
    /// </summary>
    /// <param name="fruitTree">The source fruit tree.</param>
    public TerrainFruitTree(FruitTree fruitTree)
        : base(fruitTree)
    {
        this.FruitTree = fruitTree;
    }

    /// <inheritdoc />
    public override ModDataDictionary ModData
    {
        get => this.FruitTree.modData;
    }

    private FruitTree FruitTree { get; }

    /// <inheritdoc />
    public override bool CanHarvest()
    {
        return this.FruitTree.growthStage.Value >= 4 && !this.FruitTree.stump.Value;
    }

    /// <inheritdoc />
    public override bool TryHarvest()
    {
        return this.FruitTree.performUseAction(this.FruitTree.currentTileLocation, this.FruitTree.currentLocation);
    }
}