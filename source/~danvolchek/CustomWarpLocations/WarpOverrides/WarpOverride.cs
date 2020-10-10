/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using StardewValley;

namespace CustomWarpLocations.WarpOverrides
{
    internal abstract class WarpOverride
    {
        internal static NewWarpLocations WarpLocations;

        internal abstract WarpLocation GetWarpLocation();

        internal void DoWarp()
        {
            WarpLocation location = this.GetWarpLocation();
            Game1.warpFarmer(location.locationName, location.xCoord, location.yCoord, false);
            this.AfterWarp();
        }

        private void AfterWarp()
        {
            Game1.fadeToBlackAlpha = 0.99f;
            Game1.screenGlow = false;
            Game1.player.temporarilyInvincible = false;
            Game1.player.temporaryInvincibilityTimer = 0;
            Game1.displayFarmer = true;
        }

        internal enum WarpLocationCategory
        {
            Farm,
            Mountains,
            Beach,
            Desert
        }
    }
}
