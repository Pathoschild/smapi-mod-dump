using System;
using Omegasis.TimeFreeze.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;

namespace Omegasis.TimeFreeze
{
    /// <summary>The mod entry point.</summary>
    public class TimeFreeze : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration.</summary>
        private ModConfig Config;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<ModConfig>();

            GameEvents.UpdateTick += this.GameEvents_UpdateTick;
        }

        /*********
        ** Private methods
        *********/
        /// <summary>The method invoked when the game updates (roughly 60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void GameEvents_UpdateTick(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (this.ShouldFreezeTime(Game1.player, Game1.player.currentLocation))
                Game1.gameTimeInterval = 0;
        }

        /// <summary>Get whether time should be frozen for the player at the given location.</summary>
        /// <param name="player">The player to check.</param>
        /// <param name="location">The location to check.</param>
        private bool ShouldFreezeTime(StardewValley.Farmer player, GameLocation location)
        {
            if (location.Name == "Mine" || location.Name == "SkullCave" || location.Name == "UndergroundMine" || location.IsOutdoors)
                return false;
            if (player.swimming.Value)
            {
                if (this.Config.PassTimeWhileSwimmingInBathhouse && location is BathHousePool)
                    return false;
                if (this.Config.PassTimeWhileSwimming)
                    return false;
            }
            return true;
        }
    }
}
