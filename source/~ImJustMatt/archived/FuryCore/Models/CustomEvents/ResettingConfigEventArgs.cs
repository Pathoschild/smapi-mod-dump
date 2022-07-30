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

namespace StardewMods.FuryCore.Models.CustomEvents;

using StardewMods.FuryCore.Interfaces.CustomEvents;
using StardewMods.FuryCore.Interfaces.GameObjects;

/// <inheritdoc />
public class ResettingConfigEventArgs : IResettingConfigEventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ResettingConfigEventArgs" /> class.
    /// </summary>
    /// <param name="gameObject">The game object being reset.</param>
    public ResettingConfigEventArgs(IGameObject gameObject)
    {
        this.GameObject = gameObject;
    }

    /// <inheritdoc />
    public IGameObject GameObject { get; }
}