/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.CustomBush.Framework.Models;

using StardewMods.Common.Services.Integrations.CustomBush;
using StardewValley.GameData;

/// <inheritdoc />
internal sealed class CustomBush : ICustomBush
{
    /// <inheritdoc />
    public int AgeToProduce { get; set; } = 20;

    /// <inheritdoc />
    public int DayToBeginProducing { get; set; } = 22;

    /// <inheritdoc />
    public string Description { get; set; } = string.Empty;

    /// <inheritdoc />
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>Gets or sets the id of the custom bush.</summary>
    public string Id { get; set; } = string.Empty;

    /// <inheritdoc />
    public string IndoorTexture { get; set; } = string.Empty;

    /// <summary>Gets or sets the items produced by this custom bush.</summary>
    public List<CustomBushDrop> ItemsProduced { get; set; } = [];

    /// <inheritdoc />
    public List<PlantableRule> PlantableLocationRules { get; set; } = [];

    /// <inheritdoc />
    public List<Season> Seasons { get; set; } = [];

    /// <inheritdoc />
    public string Texture { get; set; } = string.Empty;

    /// <inheritdoc />
    public int TextureSpriteRow { get; set; }
}