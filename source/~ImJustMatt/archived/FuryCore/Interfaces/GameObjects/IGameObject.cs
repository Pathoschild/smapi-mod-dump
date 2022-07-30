/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

#nullable disable

namespace StardewMods.FuryCore.Interfaces.GameObjects;

using StardewValley;

/// <summary>
///     Represents any object in the game.
/// </summary>
public interface IGameObject
{
    /// <summary>
    ///     Gets the context object.
    /// </summary>
    object Context { get; }

    /// <summary>
    ///     Gets the ModData associated with the context object.
    /// </summary>
    ModDataDictionary ModData { get; }
}