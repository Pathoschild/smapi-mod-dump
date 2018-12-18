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

        public int oldInterval;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<ModConfig>();

            GameEvents.UpdateTick += this.GameEvents_UpdateTick;
            //oldInterval = 7;
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
                if (Config.freezeIfEvenOnePlayerMeetsTimeFreezeConditions)
                {
                    bool isAnyFarmerSuitable = false;
                    foreach (Farmer farmer in Game1.getOnlineFarmers())
                    {
                        if (this.ShouldFreezeTime(farmer, farmer.currentLocation))
                        {
                            Game1.gameTimeInterval = 0;
                            isAnyFarmerSuitable = true;
                        }
                    }
                    if (isAnyFarmerSuitable == false)
                    {
                       // Game1.gameTimeInterval += Game1.currentGameTime.ElapsedGameTime.Milliseconds;
                    }
                }

                else if (Config.freezeIfMajorityPlayersMeetsTimeFreezeConditions)
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
                    {
                        Game1.gameTimeInterval = 0;

                    }
                    else
                    {
                       // Game1.gameTimeInterval += Game1.currentGameTime.ElapsedGameTime.Milliseconds;
                    }
                }

                else if (Config.freezeIfAllPlayersMeetTimeFreezeConditions)
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
                    {
                        Game1.gameTimeInterval = 0;

                    }
                    else
                    {
                       // Game1.gameTimeInterval = oldInterval;
                    }
                }


            }
            else
            {
                Farmer player = Game1.player;
                if (this.ShouldFreezeTime(player, player.currentLocation))
                {
                    Game1.gameTimeInterval = 0;
                }
                else
                {
                   // Game1.gameTimeInterval = oldInterval;
                }
            }
        }

        /// <summary>Get whether time should be frozen for the player at the given location.</summary>
        /// <param name="player">The player to check.</param>
        /// <param name="location">The location to check.</param>
        private bool ShouldFreezeTime(StardewValley.Farmer player, GameLocation location)
        {

            if (Config.PassTimeWhileInsideMine==true)
            {
                if(location.Name == "Mine" || location.Name.StartsWith("UndergroundMine"))
                {
                    return false;
                }
            }

            if (Config.PassTimeWhileInsideSkullCave==true)
            {
                if (location.Name == "SkullCave" || location.Name.StartsWith("SkullCave"))
                {
                    return false;
                }
            }

            if (location.IsOutdoors == true)
            {
                return false;
            }
                

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
