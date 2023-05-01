/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace SpaceCore.Framework.Schedules
{
    public class ScheduleSpot
    {
        public string Location { get; set; } = null;
        public int TileX { get; set; }
        public int TileY { get; set; }
    }

    public class ScheduleDataPoint
    {
        public int Time { get; set; } = 600;

        public bool TimeIsEndingTime { get; set; } = false;

        public ScheduleSpot StandingPosition;
        public int FacingDirection = Game1.down;

        public List<ScheduleSpot> Waypoints = new();

        public string StartAnimationKey { get; set; }
        public string EndAnimationKey { get; set; }

        public string UntilNextSchedule_AnimationKey { get; set; }
        public string UntilNextSchedule_DialogueKey { get; set; }

        public int WalkSpeed { get; set; } = 2;
        // public string WalkAnimationOverride { get; set; } = null;

        public bool ShouldWarp { get; set; } = false;
    }
}
