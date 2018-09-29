using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using System;

namespace LongerLastingLures
{
    class ModEntry : Mod
    {

        private ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            this.Config = Helper.ReadConfig<ModConfig>();
            TimeEvents.AfterDayStarted += updateMaxUses;
            GameEvents.FirstUpdateTick += updateMaxUses;
            PlayerEvents.LeveledUp += updateMaxUses;
            SaveEvents.AfterLoad += updateMaxUses;
            
        }

        private void updateMaxUses(object sender, EventArgs e)
        {
            int maxUses = Math.Max(1, this.Config.DefaultMaxUses);
            maxUses += (this.Config.IncreaseMaxUsesWithFishingLevel * Game1.player.FishingLevel);
            if (maxUses != FishingRod.maxTackleUses)
            {
                this.Monitor.Log("Set Max Lure Usage: " + maxUses.ToString());
                FishingRod.maxTackleUses = maxUses;
            }
            
        }
    }
}
