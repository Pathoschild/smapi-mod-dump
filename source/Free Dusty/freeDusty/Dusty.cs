/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/skuldomg/freeDusty
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace freeDusty
{
    public class Dusty : NPC
    {
        [XmlIgnore]
        public int currentBehavior = 0;
        [XmlIgnore]
        public int startedBehavior = -1;
        [XmlIgnore]
        public bool collides = false;

        public Dusty(AnimatedSprite animatedSprite, Vector2 position, int facingDir, string name)
        {
            this.Sprite = animatedSprite;
            this.Position = position;
            this.FacingDirection = facingDir;
            this.Name = name;

            this.DefaultMap = "Town";
            this.speed = 2;
            this.willDestroyObjectsUnderfoot = false;            
        }

        // Just to be safe, a bug was reported that Dusty shows up in the relations tab
        public override bool CanSocialize => false;

        public override bool canTalk()
        {
            return false;
        }

        public bool moved = false;
        public int moveCount = 0;
        
        // TODO: Customize a bit more. Ideas: Walk towards player if he's near, try to follow Alex if he's in range, ...
        public override void update(GameTime time, GameLocation location, long id, bool move)
        {
            double next = Game1.random.NextDouble();
            location = this.currentLocation;

            // Behavior before 20:00
            //if (Game1.timeOfDay < 2000)
            //{
            // Walk around randomly
            if (next < 0.007)
            {
                this.Sprite.CurrentAnimation = null;

                int direction = Game1.random.Next(10);
                //if (direction != (this.FacingDirection + 2) % 4)
                //{
                    int facingDirection = this.FacingDirection;
                    this.faceDirection(direction);

                    if (direction < 4)
                    {
                        if (this.currentLocation.isCollidingPosition(this.nextPosition(direction), Game1.viewport, this))
                        {
                            this.faceDirection(facingDirection);
                            return;
                        }
                    }
                    switch (direction)
                    {
                        case 0:
                            this.SetMovingUp(true);
                            break;
                        case 1:
                            this.SetMovingRight(true);
                            break;
                        case 2:
                            this.SetMovingDown(true);
                            break;
                        case 3:
                            this.SetMovingLeft(true);
                            break;
                        default:
                            this.Halt();
                            this.Sprite.StopAnimation();
                            break;
                    }
                //}
            }
            // Animate ... if facing right, left or down
            if(next >= 0.007 && next < 0.014)
            {
                int right = Game1.random.Next(0, 1);

                if (this.FacingDirection == 1 && right == 0)
                    this.pant();
                else if (this.FacingDirection == 1 && right == 1)
                    this.wagTail();
                else if (this.FacingDirection == 2)
                    this.pantDown();
                else if (this.FacingDirection == 3 && right == 0)
                    this.pant(true);
                else if (this.FacingDirection == 3 && right == 1)
                    this.wagTail(true);

            }
            //}

            this.MovePosition(time, Game1.viewport, this.currentLocation);                       
        }
        // Default: right
        private void pant(bool flip = false)
        {
            /*
             * This looks so much like humping it's not even funny
             * 
                this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                        {
                            new FarmerSprite.AnimationFrame(17, 200),
                            new FarmerSprite.AnimationFrame(16, 200),
                            new FarmerSprite.AnimationFrame(0, 200)
                        });
            */
            this.Halt();
            this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
            {
                new FarmerSprite.AnimationFrame(23, 200, false, flip),                
                new FarmerSprite.AnimationFrame(24, 200, false, flip),
                new FarmerSprite.AnimationFrame(25, 200, false, flip),
                new FarmerSprite.AnimationFrame(26, 200, false, flip)
            });

            if (this.withinPlayerThreshold(5) && Game1.random.Next(0, 1) == 0)
                Game1.playSound("dog_pant");
        }

        private void pantDown()
        {
            this.Halt();
            this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
            {
                new FarmerSprite.AnimationFrame(17, 200),
                new FarmerSprite.AnimationFrame(18, 200),
                new FarmerSprite.AnimationFrame(19, 200)
            });

            if (this.withinPlayerThreshold(5) && Game1.random.Next(0, 1) == 0)
                Game1.playSound("dog_pant");
        }

        private void wagTail(bool flip = false)
        {
            this.Halt();
            this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
            {
                new FarmerSprite.AnimationFrame(23, 200, false, flip),
                new FarmerSprite.AnimationFrame(31, 200, false, flip)
            });

            if (this.withinPlayerThreshold(5) && Game1.random.Next(0, 1) == 0)
                Game1.playSound("dog_bark");

            /* Sleeping while floating :|
            this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
            {
                new FarmerSprite.AnimationFrame(28, 1000),
                new FarmerSprite.AnimationFrame(29, 850)                                    
            });
            */
        }

        /*{          

            if(Game1.timeOfDay > 2000 && this.Sprite.CurrentAnimation == null && ((double) this.xVelocity == 0.0 && (double)this.yVelocity == 0.0))            
                this.currentBehavior = 1;

            switch(this.currentBehavior)
            {
                case 0:
                    if (this.Sprite.CurrentAnimation == null && Game1.random.NextDouble() < 0.01)
                    {
                        switch (Game1.random.Next(7 + (this.currentLocation is Farm ? 1 : 0)))//StardewValley.Locations.Town ? 1 : 0)))
                        {
                            case 0:
                            case 1:
                                //this.randomSquareMovement(time);                                
                                this.setMovingInFacingDirection();
                                break;
                            case 2:
                            case 3:
                                this.currentBehavior = 0;
                                break;
                            case 4:
                            case 5:
                                switch (this.FacingDirection)
                                {
                                    case 0:
                                    case 1:
                                    case 3:
                                        this.Halt();
                                        if (this.FacingDirection == 0)
                                            this.FacingDirection = Game1.random.NextDouble() < 0.5 ? 3 : 1;
                                        this.faceDirection(this.FacingDirection);
                                        this.Sprite.loop = false;
                                        this.currentBehavior = 50;
                                        break;
                                    case 2:
                                        this.Halt();
                                        this.faceDirection(2);
                                        this.Sprite.loop = false;
                                        this.currentBehavior = 2;
                                        break;
                                }
                                break;
                            case 6:
                            case 7:
                                this.currentBehavior = 51;
                                break;
                        }
                    }
                    else
                        break;
                    break;
                case 1:
                    if(Game1.timeOfDay < 2000 && Game1.random.NextDouble() < 0.001)
                    {
                        this.currentBehavior = 0;
                        return;
                    }
                    if (Game1.random.NextDouble() >= 0.002)
                        return;
                    this.doEmote(24, true);
                    return;
                case 2:
                    if(this.Sprite.currentFrame != 18 && this.Sprite.CurrentAnimation == null)
                    {
                        //this.currentBehavior = 2;
                        this.currentBehavior = 0;
                        break;
                    }
                    if (this.Sprite.currentFrame == 18 && Game1.random.NextDouble() < 0.01)
                    {
                        switch (Game1.random.Next(4))
                        {
                            case 0:
                                this.currentBehavior = 0;
                                this.Halt();
                                this.faceDirection(2);
                                // TODO: Probably edit this
                                this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                                {
                                    new FarmerSprite.AnimationFrame(17, 200),
                                    new FarmerSprite.AnimationFrame(16, 200),
                                    new FarmerSprite.AnimationFrame(0, 200)
                                });
                                this.Sprite.loop = false;
                                break;
                            case 1:
                                List<FarmerSprite.AnimationFrame> animation1 = new List<FarmerSprite.AnimationFrame>()
                                {
                                    new FarmerSprite.AnimationFrame(18, 200, false, false),//, new AnimatedSprite.endOfAnimationBehavior(this.pantSound), false
                                    new FarmerSprite.AnimationFrame(19, 200)
                                };
                                int num1 = Game1.random.Next(7, 20);
                                for (int index = 0; index < num1; ++index)
                                {
                                    animation1.Add(new FarmerSprite.AnimationFrame(18, 200, false, false)); //new AnimatedSprite.endOfAnimationBehavior(this.pantSound), false
                                    animation1.Add(new FarmerSprite.AnimationFrame(19, 200));
                                }
                                this.Sprite.setCurrentAnimation(animation1);
                                break;
                            case 2:
                                //this.randomSquareMovement(time);
                                this.setMovingInFacingDirection();
                                break;
                            case 3:
                                this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                                {
                                    new FarmerSprite.AnimationFrame(27, Game1.random.NextDouble() < 0.3 ? 500 : Game1.random.Next(2000, 15000)),
                                    new FarmerSprite.AnimationFrame(18, 1, false, false) //new AnimatedSprite.endOfAnimationBehavior(((Pet) this).hold), false)
                                });
                                this.Sprite.loop = false;
                                break;
                        }
                    }
                    else
                        break;
                    break;
                case 50:
                    if(this.withinPlayerThreshold(2))
                    {
                        //if(!this.wagging)
                         // {
                         // this.wag(this.FacingDirection == 3);
                         // break;
                         // }
                        break;
                    }
                    if(this.Sprite.currentFrame != 23 && this.Sprite.CurrentAnimation == null)
                    {
                        this.Sprite.currentFrame = 23;
                        break;
                    }
                    if (this.Sprite.currentFrame == 23 && Game1.random.NextDouble() < 0.01)
                    {
                        bool flag = this.FacingDirection == 3;
                        switch (Game1.random.Next(7))
                        {
                            case 0:
                                this.currentBehavior = 0;
                                this.Halt();
                                this.faceDirection(flag ? 3 : 1);
                                this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                                {
                                    new FarmerSprite.AnimationFrame(23, 100, false, flag),//, (AnimatedSprite.endOfAnimationBehavior) null, false),
                                    new FarmerSprite.AnimationFrame(22, 100, false, flag),//, (AnimatedSprite.endOfAnimationBehavior) null, false),
                                    new FarmerSprite.AnimationFrame(21, 100, false, flag),//, (AnimatedSprite.endOfAnimationBehavior) null, false),
                                    new FarmerSprite.AnimationFrame(20, 100, false, flag)//, new AnimatedSprite.endOfAnimationBehavior(((Pet) this).hold), false)
                                });
                                this.Sprite.loop = false;
                                break;
                            case 1:
                                if (Utility.isOnScreen(this.getTileLocationPoint(), 640, this.currentLocation))
                                {
                                    Game1.playSound("dog_bark");
                                    this.shake(500);
                                }
                                this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                                {
                                    new FarmerSprite.AnimationFrame(26, 500, false, flag),//, (AnimatedSprite.endOfAnimationBehavior) null, false),
                                    new FarmerSprite.AnimationFrame(23, 1, false, flag)//, new AnimatedSprite.endOfAnimationBehavior(((Pet) this).hold), false)
                                });
                                break;
                            case 2:
                                //this.wag(flag);
                                break;
                            case 3:
                            case 4:
                                this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                                  {
                                    new FarmerSprite.AnimationFrame(23, Game1.random.Next(2000, 6000), false, flag),//, (AnimatedSprite.endOfAnimationBehavior) null, false),
                                    new FarmerSprite.AnimationFrame(23, 1, false, flag)//, new AnimatedSprite.endOfAnimationBehavior(((Pet) this).hold), false)
                                  });
                                break;
                            default:
                                this.Sprite.loop = false;
                                List<FarmerSprite.AnimationFrame> animation2 = new List<FarmerSprite.AnimationFrame>()
                                {
                                    new FarmerSprite.AnimationFrame(24, 200, false, flag),//, new AnimatedSprite.endOfAnimationBehavior(this.pantSound), false),
                                    new FarmerSprite.AnimationFrame(25, 200, false, flag)//, (AnimatedSprite.endOfAnimationBehavior) null, false)
                                };
                                int num2 = Game1.random.Next(5, 15);
                                for (int index = 0; index < num2; ++index)
                                {
                                    animation2.Add(new FarmerSprite.AnimationFrame(24, 200, false, flag),//, new AnimatedSprite.endOfAnimationBehavior(this.pantSound), false));
                                    animation2.Add(new FarmerSprite.AnimationFrame(25, 200, false, flag)//, (AnimatedSprite.endOfAnimationBehavior)null, false));
                                }
                                this.Sprite.setCurrentAnimation(animation2);
                                break;
                        }
                    }
                    else
                        break;
                    break;
            }
            if (this.Sprite.CurrentAnimation != null)
                this.Sprite.loop = false;
            //else
            // this.wagging = false;
            if (!Game1.IsMasterGame || this.Sprite.CurrentAnimation != null)
                return;
            //this.MovePosition(time, Game1.viewport, location);            
            //this.randomSquareMovement(time);
            this.setMovingInFacingDirection();

            
        }*/
    }
}
