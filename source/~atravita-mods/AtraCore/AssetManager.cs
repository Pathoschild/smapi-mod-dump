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
using AtraBase.Toolkit.Extensions;

using AtraCore.Models;

using AtraShared.Utils.Extensions;

using Microsoft.Xna.Framework.Content;

using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

namespace AtraCore;

/// <summary>
/// Handles asset management for this mod.
/// </summary>
internal static class AssetManager
{
    private static readonly HashSet<string> eventLocations = new(StringComparer.OrdinalIgnoreCase);
    private static readonly string dataEvents = PathUtilities.NormalizeAssetName("Data/Events") + "/";

    private static IAssetName prismatic = null!;

    /// <summary>
    /// Initializes the asset manager.
    /// </summary>
    /// <param name="parser">GameContentHelper.</param>
    internal static void Initialize(IGameContentHelper parser)
    {
        prismatic = parser.ParseAssetName(AtraCoreConstants.PrismaticMaskData);

        // check and populate the event locations.
        foreach (string? location in new[] { "AdventureGuild", "Blacksmith", "WitchHut", "WitchSwamp", "Summit" })
        {
            try
            {
                _ = parser.Load<Dictionary<string, string>>(dataEvents + location);
            }
            catch (ContentLoadException)
            {
                ModEntry.ModMonitor.DebugOnlyLog($"Adding location {location} to event file loaders.");
                eventLocations.Add(location);
            }
            catch (Exception ex)
            {
                ModEntry.ModMonitor.Log($"Unexpected error checking {location}'s event file!", LogLevel.Error);
                ModEntry.ModMonitor.Log(ex.ToString());
            }
        }

        ModEntry.ModMonitor.Log($"Checked event data, adding {eventLocations.Count}");
    }

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
        else if (e.NameWithoutLocale.StartsWith(dataEvents, false, false))
        {
            string loc = e.NameWithoutLocale.BaseName.GetNthChunk('/', 2).ToString();
            if (eventLocations.Contains(loc))
            {
                e.LoadFrom(EmptyContainers.GetEmptyDictionary<string, string>, AssetLoadPriority.Low - 1000);
            }
        }
    }
}
