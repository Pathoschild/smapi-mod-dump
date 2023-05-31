/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

namespace MappingExtensionsAndExtraProperties.Models.TileProperties;

public struct MapBackgroundTileSize : ITilePropertyData
{
    public static string PropertyKey => "MEEP_Background_Tile_Size";
    private int width;
    private int height;

    public int Width => this.width;
    public int Height => this.height;

    public MapBackgroundTileSize(int width, int height)
    {
        this.width = width;
        this.height = height;
    }
}
