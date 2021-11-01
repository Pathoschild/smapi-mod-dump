/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/BattleRoyalley-Year2
**
*************************************************/

using StardewValley;
using StardewValley.Locations;

namespace BattleRoyale.Patches
{
    class DisableIslandBoat : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(IslandSouth), "performTouchAction");

        public static bool Prefix(string fullActionString)
        {
            if (Game1.eventUp)
                return true;

            string action = fullActionString.Split(' ')[0];

            if (action == "LeaveIsland")
                return false;

            return true;
        }
    }
}
