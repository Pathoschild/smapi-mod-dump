/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;

namespace DecidedlyShared.Models;

public class World
{
    private GameLocation location;
    private Dictionary<Vector2, WorldTile> tiles;

    public void AddWorldTile(Vector2 tile)
    {
        if (!this.tiles.ContainsKey(tile))
            this.tiles.Add(tile, new WorldTile(tile, this.location));
        else
            this.tiles[tile].UpdateTile();
    }

    public bool TryGetWorldTile(Vector2 tile, out WorldTile? worldTile)
    {
        if (this.tiles.ContainsKey(tile))
        {
            worldTile = this.tiles[tile];
            return true;
        }
        else
        {
            worldTile = null;
            return false;
        }
    }

    public WorldTile GetWorldTile(Vector2 tile)
    {
        return this.tiles[tile];
    }
}
