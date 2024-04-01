/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hedgehog-Technologies/StardewMods
**
*************************************************/

using System.Collections.Generic;
using xTile.Tiles;

namespace AllowBeachSprinklers.Helpers
{
    internal class TileEqualityComparer : IEqualityComparer<Tile>
    {
        public bool Equals(Tile? x, Tile? y)
        {
            if (x is null) return false;
            if (y is null) return false;
            if (ReferenceEquals(x, y)) return true;
            if (x.GetType() != y.GetType()) return false;

            return Equals(x.TileSheet, y.TileSheet) && x.TileIndex == y.TileIndex;
        }

        public int GetHashCode(Tile obj)
        {
            unchecked
            {
                var hashCode = obj.Layer != null ? obj.Layer.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (int)obj.BlendMode;
                hashCode = (hashCode * 397) ^ (obj.TileSheet != null ? obj.TileSheet.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ obj.TileIndex;
                hashCode = (hashCode * 397) ^ (obj.TileIndexProperties != null ? obj.TileIndexProperties.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
