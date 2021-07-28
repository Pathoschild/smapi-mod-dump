/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
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

        public EventArgsBeforeWarp(LocationRequest req, int targetX, int targetY, int targetFacing)
        {
            this.WarpTargetLocation = req;
            this.WarpTargetX = targetX;
            this.WarpTargetY = targetY;
            this.WarpTargetFacing = targetFacing;
        }
    }
}
