/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

namespace BetterReturnScepter
{
    public class RodCooldown
    {
        public byte Countdown { get; private set; }

        public bool CanWarp { get; private set; }

        public void IncrementTimer()
        {
            // Increment our timer.
            this.Countdown++;

            // First, if the timer is above our threshold... 
            if (this.Countdown > 140)
            {
                // We mark that the player can return to the previous sceptre point, and reset the timer.
                this.CanWarp = true;
                this.Countdown = 0;
            }
        }

        public void ResetCountdown()
        {
            this.CanWarp = false;
            this.Countdown = 0;
        }
    }
}
