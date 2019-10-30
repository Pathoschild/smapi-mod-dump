using JoysOfEfficiency.Core;
using JoysOfEfficiency.Utils;
using StardewValley;
using StardewValley.Tools;

namespace JoysOfEfficiency.Automation
{
    internal class WateringCanRefiller
    {
        private static Config Config => InstanceHolder.Config;
        public static void RefillWateringCan()
        {
            WateringCan can = Util.FindToolFromInventory<WateringCan>(Config.FindCanFromInventory);
            if (can == null || can.WaterLeft >= Util.GetMaxCan(can) ||
                !Util.IsThereAnyWaterNear(Game1.player.currentLocation, Game1.player.getTileLocation()))
            {
                return;
            }
            can.WaterLeft = can.waterCanMax;
            Game1.playSound("slosh");
            DelayedAction.playSoundAfterDelay("glug", 250);
        }
    }
}
