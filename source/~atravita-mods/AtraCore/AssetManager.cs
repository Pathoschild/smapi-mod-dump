/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraBase.Collections;
using AtraCore.Models;
using StardewModdingAPI.Events;

namespace AtraCore;

/// <summary>
/// Handles asset management for this mod.
/// </summary>
internal static class AssetManager
{
    private static IAssetName prismatic = null!;

    /// <summary>
    /// Initializes the asset manager.
    /// </summary>
    /// <param name="parser">GameContentHelper.</param>
    internal static void Initialize(IGameContentHelper parser)
        => prismatic = parser.ParseAssetName(AtraCoreConstants.PrismaticMaskData);

    /// <summary>
    /// Gets the prismatic models data asset.
    /// </summary>
    /// <returns>The prismatic models data asset.</returns>
    internal static Dictionary<string, DrawPrismaticModel>? GetPrismaticModels()
    {
        try
        {
            return Game1.content.Load<Dictionary<string, DrawPrismaticModel>>(AtraCoreConstants.PrismaticMaskData);
        }
        catch
        {
            ModEntry.ModMonitor.Log("Failed to load the prismatic mask data!", LogLevel.Error);
        }
        return null;
    }

    /// <inheritdoc cref="IContentEvents.AssetRequested"/>
    internal static void Apply(AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo(prismatic))
        {
            e.LoadFrom(EmptyContainers.GetEmptyDictionary<string, DrawPrismaticModel>, AssetLoadPriority.Low);
        }
        else if (e.NameWithoutLocale.IsEquivalentTo("Data/Events/AdventureGuild") || e.NameWithoutLocale.IsEquivalentTo("Data/Events/Blacksmith"))
        {
            e.LoadFrom(EmptyContainers.GetEmptyDictionary<string, string>, AssetLoadPriority.Low);
        }
    }
}
