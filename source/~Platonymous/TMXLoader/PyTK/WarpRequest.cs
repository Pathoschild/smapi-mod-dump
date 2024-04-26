/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using StardewValley;

namespace TMXLoader
{
    public class WarpRequest
    {
        public long farmerId { get; set; }
        public string locationName { get; set; }
        public bool isStructure { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int facing { get; set; }

        public WarpRequest()
        {

        }

        public WarpRequest(Farmer farmer, string location, int x, int y, bool isStructure, int facingAfterWarp = -1)
        {
            farmerId = farmer.UniqueMultiplayerID;
            this.isStructure = isStructure;
            this.x = x;
            this.y = y;
            facing = facingAfterWarp;
            locationName = location;
        }
    }
}
