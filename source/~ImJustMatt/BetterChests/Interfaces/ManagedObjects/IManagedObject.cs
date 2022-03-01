/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Interfaces.ManagedObjects;

using StardewMods.BetterChests.Interfaces.Config;
using StardewMods.FuryCore.Helpers;
using StardewMods.FuryCore.Interfaces.GameObjects;

/// <inheritdoc cref="StardewMods.FuryCore.Interfaces.GameObjects.IGameObject" />
internal interface IManagedObject : IGameObject, IStorageData
{
    /// <summary>
    ///     Gets an <see cref="FuryCore.Helpers.ItemMatcher" /> that is assigned to each storage type.
    /// </summary>
    public ItemMatcher ItemMatcher { get; }

    /// <summary>
    ///     Gets the Qualified Item Id of the storage object.
    /// </summary>
    public string QualifiedItemId { get; }
}