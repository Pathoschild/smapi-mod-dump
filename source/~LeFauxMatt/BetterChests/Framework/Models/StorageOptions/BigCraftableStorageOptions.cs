/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Framework.Models.StorageOptions;

using StardewMods.Common.Services.Integrations.BetterChests.Interfaces;
using StardewValley.GameData.BigCraftables;
using StardewValley.TokenizableStrings;

/// <inheritdoc />
internal sealed class BigCraftableStorageOptions : ChildStorageOptions
{
    /// <summary>Initializes a new instance of the <see cref="BigCraftableStorageOptions" /> class.</summary>
    /// <param name="default">The default storage options.</param>
    /// <param name="data">The big craftable data.</param>
    public BigCraftableStorageOptions(IStorageOptions @default, BigCraftableData data)
        : base(@default, new CustomFieldsStorageOptions(data.CustomFields)) =>
        this.Data = data;

    /// <summary>Gets the big craftable data.</summary>
    public BigCraftableData Data { get; }

    /// <inheritdoc />
    public override string GetDescription() => TokenParser.ParseText(this.Data.Description);

    /// <inheritdoc />
    public override string GetDisplayName() => TokenParser.ParseText(this.Data.DisplayName);
}