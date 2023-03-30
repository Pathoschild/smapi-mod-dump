/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraShared.Integrations;
using AtraShared.Integrations.Interfaces;
using AtraShared.Niceties;
using AtraShared.Utils.Extensions;
using Microsoft.Xna.Framework;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace AtraCore.Framework.Niceties;

/// <summary>
/// Helper to check whether or not a tile will be handled by a sprinkler.
/// Modded or otherwise.
///
/// Optimized for checking over an entire map's worth of tiles.
/// Will need to drop the cache if the player has a chance to place a sprinkler.
/// </summary>
public sealed class IsWithinSprinklerRadiusHelper
{
    // APIs - check in order.
    private static IFlexibleSprinklersApi? flexibleSprinklersApi;
    private static ILineSprinklersApi? lineSprinklersApi;
    private static IBetterSprinklersApi? betterSprinklersApi;

    private readonly HashSet<Tile> wateredTiles = new();
    private readonly HashSet<string> processedMaps = new();

    // helpers
    private IMonitor monitor;

    /// <summary>
    /// Initializes a new instance of the <see cref="IsWithinSprinklerRadiusHelper"/> class.
    /// </summary>
    /// <param name="monitor">SMAPI monitor.</param>
    /// <param name="translation">Translation helper.</param>
    /// <param name="registry">The mod registry, used to grab integrations.</param>
    public IsWithinSprinklerRadiusHelper(IMonitor monitor, ITranslationHelper translation, IModRegistry registry)
    {
        this.monitor = monitor;

        // grab APIs.
        if (flexibleSprinklersApi is null && lineSprinklersApi is null && betterSprinklersApi is null)
        {
            IntegrationHelper helper = new(monitor, translation, registry, LogLevel.Trace);
            _ = helper.TryGetAPI("Shockah.FlexibleSprinklers", "1.2.5", out flexibleSprinklersApi)
                || helper.TryGetAPI("hootless.LineSprinklers", "1.1.1", out lineSprinklersApi)
                || helper.TryGetAPI("Speeder.BetterSprinklers", "2.5.0", out betterSprinklersApi);
        }
    }

    /// <summary>
    /// Clears all the data saved.
    /// </summary>
    public void Reset()
    {
        this.wateredTiles.Clear();
        this.processedMaps.Clear();
        this.monitor.DebugOnlyLog("Clearing sprinkler-handled tiles", LogLevel.Info);
    }

    /// <summary>
    /// Whether a specific tile on a <see cref="GameLocation"/> is currently covered by a sprinkler.
    /// </summary>
    /// <param name="location">Location to check.</param>
    /// <param name="pos">Vector2 tile.</param>
    /// <returns>True if the tile will be covered, false otherwise.</returns>
    public bool IsTileInWateringRange(GameLocation? location, Vector2 pos)
    {
        if (location is null)
        {
            return false;
        }
        this.CalculateWateredTiles(location);
        return this.wateredTiles.Contains(new Tile(location.NameOrUniqueName, pos));
    }

    /// <summary>
    /// Whether or not a specific tile on a location by the given name is currently covered by sprinklers.
    /// </summary>
    /// <param name="locname">Location name.</param>
    /// <param name="pos">Vector2 tile.</param>
    /// <returns>True if the tile will be covered by a sprinkler, false otherwise.</returns>
    public bool IsTileInWateringRange(string locname, Vector2 pos)
        => this.IsTileInWateringRange(new Tile(locname, pos));

    /// <summary>
    /// Whether or not a specific <see cref="Tile" /> on a location by the given name is currently covered by sprinklers.
    /// </summary>
    /// <param name="tile">The <see cref="Tile"/>.</param>
    /// <returns>True if the tile will be covered by a sprinkler, false otherwise.</returns>
    public bool IsTileInWateringRange(Tile tile)
    {
        if (!this.processedMaps.Contains(tile.Map))
        {
            this.CalculateWateredTiles(Game1.getLocationFromName(tile.Map));
        }
        return this.wateredTiles.Contains(tile);
    }

    /// <summary>
    /// Calculates the sprinkler-covered tiles for a location.
    /// </summary>
    /// <param name="location">The GameLocation.</param>
    /// <returns>True if locations were calculated, false otherwise.</returns>
    private bool CalculateWateredTiles(GameLocation? location)
    {
        if (location is null || this.processedMaps.Contains(location.NameOrUniqueName))
        {
            return false;
        }

        this.processedMaps.Add(location.NameOrUniqueName);

        this.monitor?.DebugOnlyLog($"Calculating sprinkler-handled tiles for {location.NameOrUniqueName}", LogLevel.Info);
        if (flexibleSprinklersApi is not null)
        {
            foreach (Vector2 vec in flexibleSprinklersApi.GetAllTilesInRangeOfSprinklers(location))
            {
                if ((location.terrainFeatures.TryGetValue(vec, out TerrainFeature? terrain) && terrain is HoeDirt)
                    || (location.objects.TryGetValue(vec, out SObject? obj) && obj is IndoorPot pot && pot.hoeDirt?.Value is not null))
                {
                    this.wateredTiles.Add(new Tile(location.NameOrUniqueName, vec));
                }
            }
        }
        else
        {
            // If either better sprinklers or line sprinklers are installed
            // ask them for the relative sprinkler watered area.
            IDictionary<int, Vector2[]>? tilemap = lineSprinklersApi?.GetSprinklerCoverage()
                ?? betterSprinklersApi?.GetSprinklerCoverage();

            foreach (SObject obj in location.objects.Values)
            {
                IEnumerable<Vector2> tiles;
                if (tilemap?.TryGetValue(obj.ParentSheetIndex, out Vector2[]? vector2s) == true)
                { // got tile map from api, adjust from relative to absolute location.
                    tiles = vector2s.Select((v) => v + obj.TileLocation);
                }
                else
                { // default to vanilla logic.
                    tiles = obj.GetSprinklerTiles();
                }

                foreach (Vector2 vec in tiles)
                {
                    if (location.terrainFeatures.TryGetValue(vec, out TerrainFeature? terrain)
                        && terrain is HoeDirt)
                    {
                        this.wateredTiles.Add(new Tile(location.NameOrUniqueName, vec));
                    }
                }
            }
        }
        return true;
    }
}
