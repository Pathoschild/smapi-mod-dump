/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Integrations.BetterChests;

/// <summary>
///     Represents <see cref="IStorageData" /> with parent-child relationship.
/// </summary>
public interface IStorageNode
{
    /// <summary>
    ///     Gets or sets the <see cref="IStorageData" />.
    /// </summary>
    public IStorageData Data { get; set; }

    /// <summary>
    ///     Gets or sets the parent <see cref="IStorageData" />.
    /// </summary>
    public IStorageData Parent { get; set; }
}