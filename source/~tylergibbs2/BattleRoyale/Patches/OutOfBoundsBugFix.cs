/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using StardewValley;

namespace BattleRoyale.Patches
{
    class OutOfBoundsBugFix : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(Farmer), "MovePosition");

        public static void Prefix(Farmer __instance)
        {
            if (Game1.activeClickableMenu != null)
            {
                if (Game1.CurrentEvent == null)
                {
                    return;
                }
                if (Game1.CurrentEvent.playerControlSequence)
                {
                    return;
                }
            }
            if (__instance.isRafting)
            {
                return;
            }

            if (__instance.CanMove || Game1.eventUp || __instance.controller != null)
            {
                if (__instance.movementDirections.Count == 0)
                {
                    Warp warp = Game1.currentLocation.isCollidingWithWarp(__instance.nextPosition(-1), Game1.player);
                    if (warp != null && __instance.IsLocalPlayer)
                    {
                        __instance.warpFarmer(warp);
                        return;
                    }
                }
            }
        }
    }
}
