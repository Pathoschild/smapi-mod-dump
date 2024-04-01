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

namespace Swim
{
    public class SwimmerData
    {
        public int oxygen;
        public bool isJumping;
        public bool isUnderwater;
        public Vector2 startJumpLoc;
        public Vector2 endJumpLoc;
        public ulong lastJump = 0;
        public int ticksUnderwater = 0;
        public int ticksWearingScubaGear = 0;
        public int bubbleOffset = 0;
        public int lastBreatheSound;
        public bool surfacing;
        public bool swimSuitAlways;
        public bool readyToSwim = true;
    }
}