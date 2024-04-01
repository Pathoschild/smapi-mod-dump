/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System.Text;
using DecidedlyShared.Logging;
using StardewValley;
using xTile;
using xTile.Dimensions;
using xTile.Layers;
using xTile.ObjectModel;
using xTile.Tiles;

namespace DecidedlyShared.Utilities;

public class TilePropertyHandler
{
    private Logger logger;

    public TilePropertyHandler(Logger logger)
    {
        this.logger = logger;
    }

    public bool TryGetBackProperty(int x, int y, GameLocation location, string key,
        out PropertyValue tileProperty)
    {
        return this.TryGetTileProperty(x, y, location, "Back", key, out tileProperty);
    }

    public bool TryGetBuildingProperty(int x, int y, GameLocation location, string key,
        out PropertyValue tileProperty)
    {
        return this.TryGetTileProperty(x, y, location, "Buildings", key, out tileProperty);
    }

    public bool TryGetPropertyFromString(string keyToCheck, string property, out PropertyValue tileProperty)
    {
        tileProperty = "";

        string[] splitProperty = property.Split(" ");

        if (splitProperty.Length < 1)
            return false;

        if (!splitProperty[0].Equals(keyToCheck))
            return false;

        StringBuilder args = new StringBuilder();

        for (int i = 1; i < splitProperty.Length; i++)
        {
            args.Append($"{splitProperty[i]} ");
        }

        tileProperty = args.ToString();

        return true;
    }

    public bool TryGetTileProperty(int x, int y, GameLocation location, string layer, string key,
        out PropertyValue tileProperty)
    {
        // We need a default assignment.
        tileProperty = "";

        if (location == null)
            return false;

        // Grab our map reference to check for null.
        Map map = location.Map;
        if (map == null)
        {
            this.logger.Error($"{location.Name}'s map was somehow null. This is spooky.");

            return false;
        }

        // Then our layer.
        Layer desiredLayer = map.GetLayer(layer);
        if (desiredLayer == null)
        {
            this.logger.Error($"${location.Name}'s {layer} layer didn't exist. This is spooky.");

            return false;
        }

        // And, finally, our tile.
        Tile desiredTile = desiredLayer.PickTile(new Location(x * 64, y * 64), Game1.viewport.Size);
        if (desiredTile == null)
        {
            // this.logger.Error($"Tile {x}, {y} in {location.Name} on layer {layer} ");

            return false;
        }

        // Check if the tile has any properties, or if its Properties property is null.
        if (desiredTile?.Properties == null || desiredTile.Properties.Count == 0)
            return false;

        // It's not, so we can safely move forward.
        return desiredTile.Properties.TryGetValue(key, out tileProperty);

        // return location.Map?.GetLayer(layer)?.PickTile(new Location(x * 64, y * 64), Game1.viewport.Size)?.Properties
        //     ?.TryGetValue(key, out tileProperty) != false;

        // return location.Map.GetLayer(layer).PickTile(new Location(x * 64, y * 64), Game1.viewport.Size).Properties
        //     .TryGetValue(key, out tileProperty);
    }
}
