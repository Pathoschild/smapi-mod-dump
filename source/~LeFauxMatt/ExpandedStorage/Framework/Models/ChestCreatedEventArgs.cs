/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.ExpandedStorage.Framework.Models;

using Microsoft.Xna.Framework;
using StardewMods.Common.Services.Integrations.ExpandedStorage;
using StardewValley.Objects;

/// <inheritdoc cref="IChestCreated" />
internal sealed class ChestCreatedEventArgs(
    Chest chest,
    GameLocation location,
    Vector2 tileLocation,
    IStorageData storageData) : EventArgs, IChestCreated
{
    /// <inheritdoc />
    public Chest Chest { get; } = chest;

    /// <inheritdoc />
    public GameLocation Location { get; } = location;

    /// <inheritdoc />
    public IStorageData StorageData { get; } = storageData;

    /// <inheritdoc />
    public Vector2 TileLocation { get; } = tileLocation;
}