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
    internal class WandWarpOverride : WarpOverride
    {
        internal override WarpLocation GetWarpLocation()
        {
            if (!Game1.isStartingToGetDarkOut())
                Game1.playMorningSong();
            else
                Game1.changeMusicTrack("none");

            return WarpLocations.FarmWarpLocation_Scepter;
        }
    }
}
