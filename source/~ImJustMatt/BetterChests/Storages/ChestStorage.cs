/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Storages;

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewMods.Common.Integrations.BetterChests;
using StardewValley;
using StardewValley.Network;
using StardewValley.Objects;

/// <inheritdoc />
internal class ChestStorage : BaseStorage
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ChestStorage" /> class.
    /// </summary>
    /// <param name="chest">The source chest.</param>
    /// <param name="parent">The context where the source object is contained.</param>
    /// <param name="defaultChest">Config options for <see cref="ModConfig.DefaultChest" />.</param>
    /// <param name="position">The position of the source object.</param>
    public ChestStorage(Chest chest, object? parent, IStorageData defaultChest, Vector2 position)
        : base(chest is { SpecialChestType: Chest.SpecialChestTypes.JunimoChest } ? Game1.player.team : chest, parent, defaultChest, position)
    {
        this.Chest = chest;
    }

    /// <summary>
    ///     Gets the source chest object.
    /// </summary>
    public Chest Chest { get; }

    /// <inheritdoc />
    public override IList<Item?> Items
    {
        get => this.Chest.GetItemsForPlayer(Game1.player.UniqueMultiplayerID);
    }

    /// <inheritdoc />
    public override ModDataDictionary ModData
    {
        get => this.Chest.modData;
    }

    /// <inheritdoc />
    public override NetMutex? Mutex
    {
        get => this.Chest.GetMutex();
    }

    /// <inheritdoc />
    public override void ShowMenu()
    {
        this.Chest.ShowMenu();
    }
}