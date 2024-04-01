/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using xTile.Tiles;

namespace DynamicMapTiles
{
    public class PushedTile
    {
        public Tile tile;
        public Farmer farmer;
        public Point position;
        public int dir;
    }
}