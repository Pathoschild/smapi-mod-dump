/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;

namespace BattleRoyale
{
    struct TileLocation
    {
        public readonly int tileX;
        public readonly int tileY;
        public readonly string locationName;

        public TileLocation(string locationName, int tileX, int tileY)
        {
            this.tileX = tileX;
            this.tileY = tileY;
            this.locationName = locationName;
        }

        public Warp CreateWarp() => new(0, 0, locationName, tileX, tileY, false);
        public Vector2 CreateVector2() => new(tileX, tileY);

        public GameLocation GetGameLocation()
        {
            return Game1.getLocationFromName(locationName);
        }
    }
}
