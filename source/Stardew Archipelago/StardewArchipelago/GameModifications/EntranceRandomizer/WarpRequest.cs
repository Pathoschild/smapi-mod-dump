/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using StardewValley;

namespace StardewArchipelago.GameModifications.EntranceRandomizer
{
    public class WarpRequest
    {
        public LocationRequest LocationRequest { get; set; }
        public int TileX { get; set; }
        public int TileY { get; set; }
        public FacingDirection FacingDirectionAfterWarp { get; set; }

        public WarpRequest(LocationRequest locationRequest, int tileX, int tileY, FacingDirection facingDirectionAfterWarp)
        {
            LocationRequest = locationRequest;
            TileX = tileX;
            TileY = tileY;
            FacingDirectionAfterWarp = facingDirectionAfterWarp;
        }
    }
}
