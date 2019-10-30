using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;
using Microsoft.Xna.Framework.Graphics;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>A subclass of Stardew's BigSlime class, adjusted for use by this mod.</summary>
        class BigSlimeFTM : BigSlime
        {
            public int MineLevelOfDeathSpawns { get; set; } = 0; //determines the subtype of any monsters spawned when this monster dies

            /// <summary>Creates an instance of Stardew's BigSlime class, but with adjustments made for this mod.</summary>
            public BigSlimeFTM()
                : base()
            {

            }

            /// <summary>Creates an instance of Stardew's BigSlime class, but with adjustments made for this mod.</summary>
            /// <param name="position">The x,y coordinates of this monster's location.</param>
            /// <param name="mineLevel">A number that affects the type and/or stats of this monster. This normally represents which floor of the mines the monster spawned on (121+ for skull cavern).</param>
            public BigSlimeFTM(Vector2 position, int mineLevel)
                : base(position, mineLevel)
            {
                MineLevelOfDeathSpawns = mineLevel;
            }

            //this override fixes the following BigSlime behavioral bugs:
            // * small slimes not spawning when the big slime dies
            public override int takeDamage(int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
            {
                int num1 = Math.Max(1, damage - resilience.Value);
                if (Game1.random.NextDouble() < missChance.Value - missChance.Value * addedPrecision)
                {
                    num1 = -1;
                }
                else
                {
                    this.Slipperiness = 3;
                    this.Health -= num1;
                    this.setTrajectory(xTrajectory, yTrajectory);
                    this.currentLocation.playSound("hitEnemy");
                    this.IsWalkingTowardPlayer = true;
                    if (this.Health <= 0)
                    {
                        this.deathAnimation();
                        ++Game1.stats.SlimesKilled;
                        if (Game1.gameMode == (byte)3 && Game1.random.NextDouble() < 0.75)
                        {
                            //find any existing farm data to save this slime in
                            //NOTE: this hacky workaround is used because SavedObjects are only processed in relation to Utility.FarmDataList, each of which generates a data file
                            if (Utility.FarmDataList.Count > 0) //if any farm data exists
                            {
                                int num2 = Game1.random.Next(2, 5);
                                for (int index = 0; index < num2; ++index)
                                {
                                    this.currentLocation.characters.Add((NPC)new GreenSlime(this.Position, MineLevelOfDeathSpawns)); //use MineLevelOfDeathSpawns instead of checking the game state
                                    this.currentLocation.characters[this.currentLocation.characters.Count - 1].setTrajectory(xTrajectory / 8 + Game1.random.Next(-2, 3), yTrajectory / 8 + Game1.random.Next(-2, 3));
                                    this.currentLocation.characters[this.currentLocation.characters.Count - 1].willDestroyObjectsUnderfoot = false;
                                    this.currentLocation.characters[this.currentLocation.characters.Count - 1].moveTowardPlayer(4);
                                    this.currentLocation.characters[this.currentLocation.characters.Count - 1].Scale = (float)(0.75 + (double)Game1.random.Next(-5, 10) / 100.0);
                                    this.currentLocation.characters[this.currentLocation.characters.Count - 1].currentLocation = this.currentLocation;

                                    int ID = Utility.RNG.Next(int.MinValue, -1); //generate a random ID for saving purposes (note: the ID is below -1 to avoid matching any known NPC values set by base game functions)
                                    this.currentLocation.characters[currentLocation.characters.Count - 1].id = ID; //assign the ID to this slime

                                    SavedObject save = new SavedObject(currentLocation.Name, Position, SavedObject.ObjectType.Monster, ID, null, 1); //create save data for this slime (set to expire overnight) 
                                    Utility.FarmDataList[0].Save.SavedObjects.Add(save); //store it in the first listed FarmData
                                }
                            }
                        }
                    }
                }
                return num1;
            }
        }
    }
}
