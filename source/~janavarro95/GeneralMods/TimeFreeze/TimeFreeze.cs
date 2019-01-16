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
        ** Fields
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

            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            /*
            if (Game1.gameTimeInterval != 0)
            {
                oldInterval = Game1.gameTimeInterval;
                if (oldInterval < 3)
                {
                    oldInterval = 7;
                }
            }
            */
            if (Game1.IsMultiplayer)
            {
                if (this.Config.freezeIfEvenOnePlayerMeetsTimeFreezeConditions)
                {
                    //bool isAnyFarmerSuitable = false;
                    foreach (Farmer farmer in Game1.getOnlineFarmers())
                    {
                        if (this.ShouldFreezeTime(farmer, farmer.currentLocation))
                        {
                            Game1.gameTimeInterval = 0;
                            //isAnyFarmerSuitable = true;
                        }
                    }
                    //if (!isAnyFarmerSuitable)
                    //{
                    //    Game1.gameTimeInterval += Game1.currentGameTime.ElapsedGameTime.Milliseconds;
                    //}
                }

                else if (this.Config.freezeIfMajorityPlayersMeetsTimeFreezeConditions)
                {
                    int freezeCount = 0;
                    int playerCount = 0;
                    foreach (Farmer farmer in Game1.getOnlineFarmers())
                    {
                        playerCount++;
                        if (this.ShouldFreezeTime(farmer, farmer.currentLocation))
                        {
                            //Game1.gameTimeInterval = 0;
                            freezeCount++;
                        }
                    }
                    if (freezeCount >= (playerCount / 2))
                        Game1.gameTimeInterval = 0;
                    //else
                    //    Game1.gameTimeInterval += Game1.currentGameTime.ElapsedGameTime.Milliseconds;
                }

                else if (this.Config.freezeIfAllPlayersMeetTimeFreezeConditions)
                {
                    int freezeCount = 0;
                    int playerCount = 0;
                    foreach (Farmer farmer in Game1.getOnlineFarmers())
                    {
                        playerCount++;
                        if (this.ShouldFreezeTime(farmer, farmer.currentLocation))
                        {
                            //Game1.gameTimeInterval = 0;
                            freezeCount++;
                        }
                    }
                    if (freezeCount >= playerCount)
                        Game1.gameTimeInterval = 0;
                    //else
                    //    Game1.gameTimeInterval = this.oldInterval;
                }
            }
            else
            {
                Farmer player = Game1.player;
                if (this.ShouldFreezeTime(player, player.currentLocation))
                    Game1.gameTimeInterval = 0;
                //else
                //    Game1.gameTimeInterval = this.oldInterval;
            }
        }

        /// <summary>Get whether time should be frozen for the player at the given location.</summary>
        /// <param name="player">The player to check.</param>
        /// <param name="location">The location to check.</param>
        private bool ShouldFreezeTime(Farmer player, GameLocation location)
        {
            if (this.Config.PassTimeWhileInsideMine)
            {
                if (location.Name == "Mine" || location.Name.StartsWith("UndergroundMine"))
                    return false;
            }

            if (this.Config.PassTimeWhileInsideSkullCave)
            {
                if (location.Name == "SkullCave" || location.Name.StartsWith("SkullCave"))
                    return false;
            }

            if (location.IsOutdoors)
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
