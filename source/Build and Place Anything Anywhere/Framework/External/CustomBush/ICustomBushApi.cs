/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Smoked-Fish/AnythingAnywhere
**
*************************************************/

using StardewValley;
using StardewValley.GameData;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;

namespace AnythingAnywhere.Framework.External.CustomBush;

/// <summary>Mod API for custom bushes.</summary>
public interface ICustomBushApi
{
    /// <summary>Retrieves the data model for all Custom Bush.</summary>
    /// <returns>An enumerable of objects implementing the ICustomBush interface. Each object represents a custom bush.</returns>
    public IEnumerable<(string Id, ICustomBush Data)> GetData();

    /// <summary>Determines if the given Bush instance is a custom bush.</summary>
    /// <param name="bush">The bush instance to check.</param>
    /// <returns>True if the bush is a custom bush, otherwise false.</returns>
    public bool IsCustomBush(Bush bush);

    /// <summary>Tries to get the custom bush model associated with the given bush.</summary>
    /// <param name="bush">The bush.</param>
    /// <param name="customBush">
    /// When this method returns, contains the custom bush associated with the given bush, if found;
    /// otherwise, it contains null.
    /// </param>
    /// <returns>true if the custom bush associated with the given bush is found; otherwise, false.</returns>
    public bool TryGetCustomBush(Bush bush, out ICustomBush? customBush);

    /// <summary>Tries to get the custom bush drop associated with the given bush id.</summary>
    /// <param name="id">The id of the bush.</param>
    /// <param name="drops">When this method returns, contains the items produced by the custom bush.</param>
    /// <returns>true if the drops associated with the given id is found; otherwise, false.</returns>
    public bool TryGetDrops(string id, out IList<ICustomBushDrop>? drops);
}
public interface ICustomBush
{
    /// <summary>Gets the age needed to produce.</summary>
    public int AgeToProduce { get; }

    /// <summary>Gets the day of month to begin producing.</summary>
    public int DayToBeginProducing { get; }

    /// <summary>Gets the description of the bush.</summary>
    public string Description { get; }

    /// <summary>Gets the display name of the bush.</summary>
    public string DisplayName { get; }

    /// <summary>Gets the default texture used when planted indoors.</summary>
    public string IndoorTexture { get; }

    /// <summary>Gets the season in which this bush will produce its drops.</summary>
    public List<Season> Seasons { get; }

    /// <summary>Gets the rules which override the locations that custom bushes can be planted in.</summary>
    public List<PlantableRule> PlantableLocationRules { get; }

    /// <summary>Gets the texture of the tea bush.</summary>
    public string Texture { get; }

    /// <summary>Gets the row index for the custom bush's sprites.</summary>
    public int TextureSpriteRow { get; }
}

public interface ICustomBushDrop : ISpawnItemData
{
    /// <summary>Gets the specific season when the item can be produced.</summary>
    public Season? Season { get; }

    /// <summary>Gets the probability that the item will be produced.</summary>
    public float Chance { get; }

    /// <summary>A game state query which indicates whether the item should be added. Defaults to always added.</summary>
    public string? Condition { get; }

    /// <summary>An ID for this entry within the current list (not the item itself, which is <see cref="GenericSpawnItemData.ItemId" />). This only needs to be unique within the current list. For a custom entry, you should use a globally unique ID which includes your mod ID like <c>ExampleMod.Id_ItemName</c>.</summary>
    public string? Id { get; }
}