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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using System;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>A subclass of Stardew's Dust Sprite class, adjusted for use by this mod.</summary>
        public class DustSpiritFTM : DustSpirit
        {
            protected IReflectedField<bool> SeenFarmer;
            protected IReflectedField<bool> RunningAwayFromFarmer;
            protected IReflectedField<bool> ChargingFarmer;
            protected IReflectedField<Multiplayer> Multiplayer;

            /// <summary>Creates an instance of Stardew's Dust Sprite class, but with adjustments made for this mod.</summary>
            public DustSpiritFTM()
                : base()
            {
                InitializeReflection();
            }

            /// <summary>Creates an instance of Stardew's Dust Sprite class, but with adjustments made for this mod.</summary>
            /// <param name="position">The x,y coordinates of this monster's location.</param>
            public DustSpiritFTM(Vector2 position)
                : base(position)
            {
                InitializeReflection();
            }

            protected virtual void InitializeReflection()
            {
                SeenFarmer = Utility.Helper.Reflection.GetField<bool>(this, "seenFarmer");
                RunningAwayFromFarmer = Utility.Helper.Reflection.GetField<bool>(this, "runningAwayFromFarmer");
                ChargingFarmer = Utility.Helper.Reflection.GetField<bool>(this, "chargingFarmer");
                Multiplayer = Utility.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer");
            }

            /// <summary>
            /// Overrides the base method to fix the following issues:
            /// * GameLocation.destroyObject causes an error in some locations if the "who" argument is null.
            /// </summary>
            public override void behaviorAtGameTick(GameTime time)
            {
                Monster_behaviorAtGameTick(time); //call a copy of the base method
                if (yJumpOffset == 0)
                {
                    if (Game1.random.NextDouble() < 0.01)
                    {
                        Multiplayer.GetValue().broadcastSprites(base.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 128, 64, 64), 40f, 4, 0, getStandingPosition() + new Vector2(-21f, 0f), flicker: false, flipped: false)
                        {
                            layerDepth = (getStandingPosition().Y - 10f) / 10000f
                        });
                        foreach (Vector2 v2 in StardewValley.Utility.getAdjacentTileLocations(getTileLocation()))
                        {
                            if (base.currentLocation.objects.ContainsKey(v2) && (base.currentLocation.objects[v2].Name.Contains("Stone") || base.currentLocation.objects[v2].Name.Contains("Twig")))
                            {
                                //modify destruction to credit the monster's currently targeted player
                                Farmer who = Player;
                                if (who != null) //note: Player => findPlayer() currently never returns null, but may change or be Harmony patched
                                    base.currentLocation.destroyObject(v2, Player);
                            }
                        }
                        yJumpVelocity *= 2f;
                    }
                    if (!ChargingFarmer.GetValue())
                    {
                        xVelocity = (float)Game1.random.Next(-20, 21) / 5f;
                    }
                }
                if (ChargingFarmer.GetValue())
                {
                    base.Slipperiness = 10;
                    Vector2 v = StardewValley.Utility.getAwayFromPlayerTrajectory(GetBoundingBox(), base.Player);
                    xVelocity += (0f - v.X) / 150f + ((Game1.random.NextDouble() < 0.01) ? ((float)Game1.random.Next(-50, 50) / 10f) : 0f);
                    if (Math.Abs(xVelocity) > 5f)
                    {
                        xVelocity = Math.Sign(xVelocity) * 5;
                    }
                    yVelocity += (0f - v.Y) / 150f + ((Game1.random.NextDouble() < 0.01) ? ((float)Game1.random.Next(-50, 50) / 10f) : 0f);
                    if (Math.Abs(yVelocity) > 5f)
                    {
                        yVelocity = Math.Sign(yVelocity) * 5;
                    }
                    if (Game1.random.NextDouble() < 0.0001)
                    {
                        controller = new PathFindController(this, base.currentLocation, new Point((int)base.Player.getTileLocation().X, (int)base.Player.getTileLocation().Y), Game1.random.Next(4), null, 300);
                        ChargingFarmer.SetValue(false);
                    }
                    if (isHardModeMonster.Value && CaughtInWeb())
                    {
                        xVelocity = 0f;
                        yVelocity = 0f;
                        if (shakeTimer <= 0 && Game1.random.NextDouble() < 0.05)
                        {
                            shakeTimer = 200;
                        }
                    }
                }
                else if (!SeenFarmer.GetValue() && StardewValley.Utility.doesPointHaveLineOfSightInMine(base.currentLocation, getStandingPosition() / 64f, base.Player.getStandingPosition() / 64f, 8))
                {
                    SeenFarmer.SetValue(true);
                }
                else if (SeenFarmer.GetValue() && controller == null && !RunningAwayFromFarmer.GetValue())
                {
                    addedSpeed = 2;
                    controller = new PathFindController(this, base.currentLocation, StardewValley.Utility.isOffScreenEndFunction, -1, eraseOldPathController: false, offScreenBehavior, 350, Point.Zero);
                    RunningAwayFromFarmer.SetValue(true);
                }
                else if (controller == null && RunningAwayFromFarmer.GetValue())
                {
                    ChargingFarmer.SetValue(true);
                }
            }

            /// <summary>Except where commented, this is a copy of "Monster.behaviorAtGameTick", used to implement this monster's "base.behaviorAtGameTick" call.</summary>
            private void Monster_behaviorAtGameTick(GameTime time)
            {
                if (base.timeBeforeAIMovementAgain > 0f)
                {
                    base.timeBeforeAIMovementAgain -= time.ElapsedGameTime.Milliseconds;
                }
                if (this.Player?.isRafting != true || !this.withinPlayerThreshold(4)) //check for null on Player due to reported errors (not necessarily FTM-specific)
                {
                    return;
                }
                if (Math.Abs(this.Player.GetBoundingBox().Center.Y - this.GetBoundingBox().Center.Y) > 192)
                {
                    if (this.Player.GetBoundingBox().Center.X - this.GetBoundingBox().Center.X > 0)
                    {
                        this.SetMovingLeft(b: true);
                    }
                    else
                    {
                        this.SetMovingRight(b: true);
                    }
                }
                else if (this.Player.GetBoundingBox().Center.Y - this.GetBoundingBox().Center.Y > 0)
                {
                    this.SetMovingUp(b: true);
                }
                else
                {
                    this.SetMovingDown(b: true);
                }
                this.MovePosition(time, Game1.viewport, base.currentLocation);
            }
        }
    }
}
