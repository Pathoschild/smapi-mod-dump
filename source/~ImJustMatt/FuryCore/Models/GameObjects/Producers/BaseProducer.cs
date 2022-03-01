/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.FuryCore.Models.GameObjects.Producers;

using StardewMods.FuryCore.Interfaces.GameObjects;
using StardewValley;

/// <inheritdoc cref="StardewMods.FuryCore.Interfaces.GameObjects.IProducer" />
internal abstract class BaseProducer : GameObject, IProducer
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseProducer" /> class.
    /// </summary>
    /// <param name="context">The source object.</param>
    protected BaseProducer(object context)
        : base(context)
    {
    }

    /// <inheritdoc />
    public abstract override ModDataDictionary ModData { get; }

    /// <inheritdoc />
    public abstract Item OutputItem { get; protected set; }

    /// <inheritdoc />
    public abstract bool TryGetOutput(out Item item);

    /// <inheritdoc />
    public abstract bool TrySetInput(Item item);
}