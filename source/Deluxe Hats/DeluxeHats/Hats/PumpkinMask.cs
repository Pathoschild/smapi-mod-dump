using StardewValley;
using StardewValley.Characters;
using System;

namespace DeluxeHats.Hats
{
    public static class PumpkinMask
    {
        public const string Name = "Pumpkin Mask";
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
            daredevil.checkAction(Game1.player, Game1.currentLocation);
            Game1.getFarm().characters.Remove(daredevil);
            daredevil = null;
        }
    }
}
