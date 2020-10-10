/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/nman130/Stardew-Mods
**
*************************************************/

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Monsters;

namespace PeacefulMining
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary> The mod configuration from the player. </summary>
        private ModConfig Config;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            if (this.Config.IsModActive)
            {
                helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            }
        }


        /*********
        ** Private methods
        *********/
        
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if(Game1.inMine)
            {
                var characters = Game1.currentLocation.getCharacters();
                for (int i = characters.Count; i > -1; i--)
                {
                    if(i < characters.Count && characters[i].IsMonster)
                    {
                        /***
                        Game1.player.addBuffAttributes
                        if (Game1.currentLocation is MineShaft)
                        {
                            MineShaft ms = (MineShaft)Game1.currentLocation;
                            ms.fog
                        }

                        if (Game1.currentLocation.isCollidingPosition(characters[i].nextPosition(characters[i].getDirection()), Game1.viewport, false, 1, false, null, false, false, true))
                        {
                            characters.RemoveAt(i);
                        }
                        else
                        {

                            Monster m = (Monster)characters[i];

                            m.currentLocation = new GameLocation();
                            m.Halt();
                            m.focusedOnFarmers = false;
                            m.hasMoved = true;
                            m.movementPause = 1000000;
                            m.stopWithoutChangingFrame();
                            m.timeBeforeAIMovementAgain = 1000000;
                            m.timerSinceLastMovement = -1000000;
                            m.xVelocity = 0;
                            m.yVelocity = 0;
                            m.yJumpVelocity = 0;
                            m.speed = 0;
                            m.Speed = 0;
                            m.addedSpeed = 0;
                            m.moveTowardPlayer(0);
                            m.faceDirection(-1);

                            m.up

                            if (m is GreenSlime)
                            {
                                GreenSlime gs = (GreenSlime)m;
                                gs.readyToMate = 1000000;
                            }

                        }
                        ***/
                        characters.RemoveAt(i);   
                    }
                }
            }
        }

    }
}