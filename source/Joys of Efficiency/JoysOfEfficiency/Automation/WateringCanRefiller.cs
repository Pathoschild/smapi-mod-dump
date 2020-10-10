/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/pomepome/JoysOfEfficiency
**
*************************************************/

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
