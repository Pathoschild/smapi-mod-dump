/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/hootless/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusLocations.Framework;
using Microsoft.Xna.Framework;

namespace BusLocations
{
    public class BusStopEventsApi
    {
        Action jumpToLocation;
        bool hasJumpToOverride = false;
        BusLoc goingTo;

        internal void SetGoingTo(BusLoc goTo)
        {
            goingTo = goTo;
        }

        public string GetDestinationMap()
        {
            return goingTo.MapName;
        }

        public Point GetDestinationPoint()
        {
            return new Point(goingTo.DestinationX, goingTo.DestinationY);
        }

        public int GetDestinationFacing()
        {
            return goingTo.ArrivalFacing;
        }

        public void SetJumpToLocation(Action handler)
        {
            hasJumpToOverride = true;
            jumpToLocation = handler;
        }

        public void JumpToLocation()
        {
            jumpToLocation();
        }

        internal bool HasJumpToOverride()
        {
            return hasJumpToOverride;
        }

    }
}
