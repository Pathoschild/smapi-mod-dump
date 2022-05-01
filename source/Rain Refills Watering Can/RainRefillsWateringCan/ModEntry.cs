/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JudeRV/Stardew-RainRefillsWateringCan
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;

namespace MaxCastStamina
{
    public class ModEntry : Mod
    {
        ModConfig Config;

        int waterIncreaseAmount;
        bool requireEquipped;
        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();
            waterIncreaseAmount = Config.WaterIncreaseAmount;
            requireEquipped = Config.RequireWateringCanToBeEquipped;

            helper.Events.GameLoop.TimeChanged += OnTimeChanged;
        }

        private void OnTimeChanged(object sender, TimeChangedEventArgs e)
        {
            if (Game1.isRaining)
            {
                if (requireEquipped)
                {
                    if (Game1.player.CurrentTool is WateringCan can)
                    {
                        if (Game1.player.currentLocation.IsOutdoors)
                        {
                            if (can.WaterLeft + waterIncreaseAmount > can.waterCanMax)
                            {
                                can.WaterLeft += (can.waterCanMax - can.WaterLeft);
                            }
                            else
                            {
                                can.WaterLeft += waterIncreaseAmount;
                            }
                        }
                    }
                }
                else
                {
                    WateringCan can = null;
                    foreach (Item item in Game1.player.Items)
                    {
                        if (item is WateringCan can1)
                        {
                            can = can1;
                            break;
                        }
                    }
                    if (can != null)
                    {
                        if (Game1.player.currentLocation.IsOutdoors)
                        {
                            if (can.WaterLeft + waterIncreaseAmount > can.waterCanMax)
                            {
                                can.WaterLeft += (can.waterCanMax - can.WaterLeft);
                            }
                            else
                            {
                                can.WaterLeft += waterIncreaseAmount;
                            }
                        }
                    }
                }
            }
        }
    }

    class ModConfig
    {
        public int WaterIncreaseAmount { get; set; } = 1;
        public bool RequireWateringCanToBeEquipped { get; set; } = true;
    }
}