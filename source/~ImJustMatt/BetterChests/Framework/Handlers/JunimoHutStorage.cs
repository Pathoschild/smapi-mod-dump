/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Framework.Handlers;

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.Network;
using StardewValley.Objects;

/// <inheritdoc />
internal sealed class JunimoHutStorage : BaseStorage
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="JunimoHutStorage" /> class.
    /// </summary>
    /// <param name="junimoHut">The junimo hut.</param>
    /// <param name="source">The context where the source object is contained.</param>
    /// <param name="position">The position of the source object.</param>
    public JunimoHutStorage(JunimoHut junimoHut, object? source, Vector2 position)
        : base(junimoHut, source, position)
    {
        this.JunimoHut = junimoHut;
    }

    /// <inheritdoc />
    public override IList<Item?> Items => this.Chest.GetItemsForPlayer(Game1.player.UniqueMultiplayerID);

    /// <summary>
    ///     Gets the Junimo Hut building.
    /// </summary>
    public JunimoHut JunimoHut { get; }

    /// <inheritdoc />
    public override ModDataDictionary ModData => this.JunimoHut.modData;

    /// <inheritdoc />
    public override NetMutex? Mutex => this.Chest.GetMutex();

    private Chest Chest => this.JunimoHut.output.Value;
}