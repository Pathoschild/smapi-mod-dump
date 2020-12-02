/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/domsim1/stardew-valley-deluxe-hats-mod
**
*************************************************/

using StardewValley;
using StardewValley.Characters;
using System;

namespace DeluxeHats.Hats
{
    public static class PumpkinMask
    {
        public const string Name = "Pumpkin Mask";
        public const string Description = "Spawn a horse and mount it.\nThe horse will disappear when you unmount it.";
        private static Horse daredevil;
        public static void Activate()
        {
            if (!Game1.currentLocation.isOutdoors || Game1.eventUp)
            {
                return;
            }
            if (daredevil == null)
            {
                daredevil = new Horse(new Guid(), Game1.player.getTileX(), Game1.player.getTileY())
                {
                    currentLocation = Game1.currentLocation,
                };
                daredevil.faceDirection(Game1.player.getDirection());
                daredevil.Name = "Daredevil";
                daredevil.displayName = "Daredevil";
                Game1.getFarm().characters.Add((NPC)daredevil);
                daredevil.checkAction(Game1.player, Game1.currentLocation);
            }
        }

        public static void Disable()
        {
            if (daredevil != null) {
                daredevil.checkAction(Game1.player, Game1.currentLocation);
            }
            Game1.getFarm().characters.Remove(daredevil);
            daredevil = null;
        }
    }
}
