/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Framework.Models.Containers;

using Microsoft.Xna.Framework;
using StardewMods.Common.Services.Integrations.BetterChests.Interfaces;
using StardewValley.Mods;
using StardewValley.Objects;

/// <inheritdoc />
internal sealed class FridgeContainer : ChestContainer
{
    /// <summary>Initializes a new instance of the <see cref="FridgeContainer" /> class.</summary>
    /// <param name="baseOptions">The type of storage object.</param>
    /// <param name="location">The game location where the fridge storage is located.</param>
    /// <param name="chest">The chest storage of the container.</param>
    public FridgeContainer(IStorageOptions baseOptions, GameLocation location, Chest chest)
        : base(baseOptions, chest) =>
        this.Location = location;

    /// <inheritdoc />
    public override GameLocation Location { get; }

    /// <inheritdoc />
    public override Vector2 TileLocation => this.Location.GetFridgePosition()?.ToVector2() ?? Vector2.Zero;

    /// <inheritdoc />
    public override ModDataDictionary ModData => this.Location.modData;
}