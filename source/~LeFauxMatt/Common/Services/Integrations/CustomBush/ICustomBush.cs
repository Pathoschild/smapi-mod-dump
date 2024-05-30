/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

#if IS_FAUXCORE
namespace StardewMods.FauxCore.Common.Services.Integrations.CustomBush;
#else
namespace StardewMods.Common.Services.Integrations.CustomBush;
#endif

using StardewValley.GameData;

/// <summary>Model used for custom bushes.</summary>
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