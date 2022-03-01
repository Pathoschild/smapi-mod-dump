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

using StardewMods.FuryCore.Interfaces.GameObjects;
using StardewValley;

/// <inheritdoc cref="StardewMods.FuryCore.Interfaces.GameObjects.ITerrainFeature" />
public abstract class BaseTerrain : GameObject, ITerrainFeature
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseTerrain" /> class.
    /// </summary>
    /// <param name="context">The source object.</param>
    protected BaseTerrain(object context)
        : base(context)
    {
    }

    /// <inheritdoc />
    public abstract override ModDataDictionary ModData { get; }

    /// <inheritdoc />
    public abstract bool CanHarvest();

    /// <inheritdoc />
    public abstract bool TryHarvest();
}