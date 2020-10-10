/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/SpaceCore_SDV
**
*************************************************/

using SpaceShared;
using StardewValley;

namespace SpaceCore.Events
{
    public class EventArgsBeforeWarp : CancelableEventArgs
    {
        public LocationRequest WarpTargetLocation;
        public int WarpTargetX;
        public int WarpTargetY;
        public int WarpTargetFacing;

        public EventArgsBeforeWarp( LocationRequest req, int targetX, int targetY, int targetFacing )
        {
            WarpTargetLocation = req;
            WarpTargetX = targetX;
            WarpTargetY = targetY;
            WarpTargetFacing = targetFacing;
        }
    }
}
