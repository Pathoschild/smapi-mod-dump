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
using StardewValley.Menus;

namespace BattleRoyale.Patches
{
    class DesertBusListener : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(Desert), "playerReachedBusDoor");

        public static bool Prefix()
        {
            Game1.activeClickableMenu = new DialogueBox("Head to the right of the road");
            return false;
        }
    }
}
