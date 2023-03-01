/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;

using xTile.Dimensions;
using xTile.ObjectModel;

using XTile = xTile.Tiles.Tile;

namespace AtraShared.Utils.Extensions;

/// <summary>
/// Extensions for IAssetDataForMaps.
/// </summary>
public static class IAssetDataForMapsExtensions
{
    /// <summary>
    /// Adds a tile property on a specific tile.
    /// </summary>
    /// <param name="map">map to add to.</param>
    /// <param name="monitor">logger instance.</param>
    /// <param name="layer">layer to grab from.</param>
    /// <param name="key">key.</param>
    /// <param name="property">value.</param>
    /// <param name="placementTile">tile to edit.</param>
    public static void AddTileProperty(this IAssetDataForMap map, IMonitor monitor, string layer, string key, string property, Vector2 placementTile)
    {
        XTile? tile = map.Data.GetLayer(layer).PickTile(
            new Location((int)placementTile.X * Game1.tileSize, (int)placementTile.Y * Game1.tileSize),
            Game1.viewport.Size);
        if (tile is null)
        {
            monitor.Log($"Tile could not be edited for shop, please let atra know!", LogLevel.Warn);
            return;
        }
        tile.Properties[key] = new PropertyValue(property);
    }
}