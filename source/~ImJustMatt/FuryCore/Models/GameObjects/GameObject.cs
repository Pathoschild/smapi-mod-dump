/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.FuryCore.Models.GameObjects;

using StardewMods.FuryCore.Interfaces.GameObjects;
using StardewValley;

/// <inheritdoc />
public abstract class GameObject : IGameObject
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="GameObject" /> class.
    /// </summary>
    /// <param name="context">The source object.</param>
    protected GameObject(object context)
    {
        this.Context = context;
    }

    /// <inheritdoc />
    public object Context { get; }

    /// <inheritdoc />
    public abstract ModDataDictionary ModData { get; }
}