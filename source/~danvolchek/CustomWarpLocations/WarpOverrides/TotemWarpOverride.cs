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
    internal class TotemWarpOverride : WarpOverride
    {
        private readonly int parentSheetIndex;

        internal TotemWarpOverride(object target)
        {
            this.parentSheetIndex = ((Object)target).ParentSheetIndex;
        }

        internal override WarpLocation GetWarpLocation()
        {
            WarpLocation newLocation = null;
            switch (this.parentSheetIndex)
            {
                case 688:
                    newLocation = WarpLocations.FarmWarpLocation_Totem;
                    break;

                case 689:
                    newLocation = WarpLocations.MountainWarpLocation_Totem;
                    break;

                case 690:
                    newLocation = WarpLocations.BeachWarpLocation_Totem;
                    break;

                case 261:
                    newLocation = WarpLocations.DesertWarpLocation_Totem;
                    break;
            }

            return newLocation;
        }
    }
}
