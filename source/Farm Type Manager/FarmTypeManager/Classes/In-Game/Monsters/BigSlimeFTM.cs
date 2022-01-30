/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Xml.Serialization;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>A subclass of Stardew's BigSlime class, adjusted for use by this mod.</summary>
        public class BigSlimeFTM : BigSlime
        {
            [XmlElement("FTM_mineLevelOfDeathSpawns")]
            public readonly NetInt mineLevelOfDeathSpawns = new NetInt(0);

            /// <summary>A number that affects the type and/or stats of any monsters spawned by this monster's death.</summary>
            [XmlIgnore]
            public int MineLevelOfDeathSpawns
            {
                get
                {
                    return mineLevelOfDeathSpawns.Value;
                }

                set
                {
                    mineLevelOfDeathSpawns.Value = value;
                }
            }

            /// <summary>Creates an instance of Stardew's BigSlime class, but with adjustments made for this mod.</summary>
            public BigSlimeFTM()
                : base()
            {

            }

            /// <summary>Creates an instance of Stardew's BigSlime class, but with adjustments made for this mod.</summary>
            /// <param name="position">The x,y coordinates of this monster's location.</param>
            public BigSlimeFTM(Vector2 position)
                : base(position, 0)
            {
                MineLevelOfDeathSpawns = 0;
            }

            /// <summary>Creates an instance of Stardew's BigSlime class, but with adjustments made for this mod.</summary>
            /// <param name="position">The x,y coordinates of this monster's location.</param>
            /// <param name="mineLevel">A number that affects the type and/or stats of this monster. This normally represents which floor of the mines the monster spawned on (121+ for skull cavern).</param>
            public BigSlimeFTM(Vector2 position, int mineLevel)
                : base(position, mineLevel)
            {
                MineLevelOfDeathSpawns = mineLevel;
            }

            /// <summary>This method adds the CustomDamage setting to to the monster's list of net fields for multiplayer functionality.</summary>
            protected override void initNetFields()
            {
                base.initNetFields();
                this.NetFields.AddField(mineLevelOfDeathSpawns);
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
                                SavedObject save = new SavedObject() //create save data for this slime (set to expire overnight) 
                                {
                                    MapName = currentLocation.Name,
                                    Tile = Position,
                                    Type = SavedObject.ObjectType.Monster,
                                    ID = ID,
                                    DaysUntilExpire = 1
                                };

                                if (Context.IsMainPlayer) //if this method was run by the host player
                                {
                                    Utility.FarmDataList[0].Save.SavedObjects.Add(save); //store the save data in the first listed FarmData
                                }
                                else //if this method was run by a client player (farmhand)
                                {
                                    //send a message to the host player to process this save data
                                    Utility.Helper.Multiplayer.SendMessage<SavedObject>(save, "SavedObject", new string[] { Utility.Helper.ModRegistry.ModID }, new long[] { Game1.MasterPlayer.UniqueMultiplayerID });
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
