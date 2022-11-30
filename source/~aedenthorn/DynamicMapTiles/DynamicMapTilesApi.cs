/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using xTile.Tiles;

namespace DynamicMapTiles
{
    public interface IDynamicMapTilesApi
    {
        public void PushTile(GameLocation location, Tile tile, int dir, Point start, Farmer farmer);
    }
    public class DynamicMapTilesApi : IDynamicMapTilesApi
    {
        public void PushTile(GameLocation location, Tile tile, int dir, Point start, Farmer farmer)
        {
            ModEntry.PushTile(location, tile, dir, start, farmer);
        }
    }
}