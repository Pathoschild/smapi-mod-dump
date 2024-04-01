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
using StardewValley.GameData.Locations;

/// <inheritdoc />
internal sealed class LocationStorageOptions : ChildStorageOptions
{
    /// <summary>Initializes a new instance of the <see cref="LocationStorageOptions" /> class.</summary>
    /// <param name="default">The default storage options.</param>
    /// <param name="data">The location data.</param>
    public LocationStorageOptions(IStorageOptions @default, LocationData data)
        : base(@default, new CustomFieldsStorageOptions(data.CustomFields)) =>
        this.Data = data;

    /// <summary>Gets the location data.</summary>
    public LocationData Data { get; }

    /// <inheritdoc />
    public override string GetDescription() => I18n.Storage_Fridge_Tooltip();

    /// <inheritdoc />
    public override string GetDisplayName() => I18n.Storage_Fridge_Name();
}