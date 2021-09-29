/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using StardewValley;

namespace SurfingFestival.Framework
{
    internal class RacerState
    {
        public int Speed { get; set; } = Mod.SurfSpeed;
        public int AddedSpeed { get; set; }
        public int Surfboard { get; set; }
        public int Facing { get; set; } = Game1.right;

        public int LapsDone { get; set; }
        public bool ReachedHalf { get; set; }

        public SurfItem? CurrentItem { get; set; }
        public int ItemObtainTimer { get; set; } = -1;
        public int ItemUsageTimer { get; set; } = -1;
        public int SlowdownTimer { get; set; } = -1;
        public int StunTimer { get; set; } = -1;

        public bool ShouldUseItem { get; set; }
    }
}
