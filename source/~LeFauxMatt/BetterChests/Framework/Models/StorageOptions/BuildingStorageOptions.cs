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
using StardewValley.GameData.Buildings;
using StardewValley.TokenizableStrings;

/// <inheritdoc />
internal sealed class BuildingStorageOptions : ChildStorageOptions
{
    /// <summary>Initializes a new instance of the <see cref="BuildingStorageOptions" /> class.</summary>
    /// <param name="default">The default storage options.</param>
    /// <param name="data">The building data.</param>
    public BuildingStorageOptions(IStorageOptions @default, BuildingData data)
        : base(@default, new CustomFieldsStorageOptions(data.CustomFields)) =>
        this.Data = data;

    /// <summary>Gets the building data.</summary>
    public BuildingData Data { get; }

    /// <inheritdoc />
    public override string GetDescription() => TokenParser.ParseText(this.Data.Description);

    /// <inheritdoc />
    public override string GetDisplayName() => TokenParser.ParseText(this.Data.Name);
}