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

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = Helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.DayStarted += onDayStarted;
            helper.Events.Player.LevelChanged += onLevelChanged;
        }

        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void onDayStarted(object sender, DayStartedEventArgs e)
        {
            updateMaxUses();
        }

        /// <summary>Raised after a player skill level changes. This happens as soon as they level up, not when the game notifies the player after their character goes to bed.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void onLevelChanged(object sender, LevelChangedEventArgs e)
        {
            if (e.IsLocalPlayer)
                updateMaxUses();
        }

        private void updateMaxUses()
        {
            int maxUses = Math.Max(1, this.Config.DefaultMaxUses);
            maxUses += (this.Config.IncreaseMaxUsesWithFishingLevel * Game1.player.FishingLevel);
            if (maxUses != FishingRod.maxTackleUses)
            {
                this.Monitor.Log($"Set Max Lure Usage: {maxUses}");
                FishingRod.maxTackleUses = maxUses;
            }
        }
    }
}
