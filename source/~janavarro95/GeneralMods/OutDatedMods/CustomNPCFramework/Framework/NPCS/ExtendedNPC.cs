/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using CustomNPCFramework.Framework.Enums;
using CustomNPCFramework.Framework.ModularNpcs;
using CustomNPCFramework.Framework.ModularNpcs.ModularRenderers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace CustomNPCFramework.Framework.NPCS
{
    // TODO:
    // - Add in an resource loader for all of the character graphics and use it to populate some character graphics.
    // - Make .json way to load in assets to the mod.
    // - Organize it all.
    // - Profit???

    public class ExtendedNpc : NPC
    {
        /// <summary>The custom character renderer for this npc.</summary>
        public BasicRenderer characterRenderer;
        public bool hasBeenKissedToday;
        public Point previousEndPoint;
        public bool returningToEndPoint;
        public bool hasSaidAfternoonDialogue;
        public int timeAfterSquare;

        /// <summary>Used to hold sprite information to be used in the case the npc renderer is null.</summary>
        public Sprite spriteInformation;

        /// <summary>Used to hold the portrait information for the npc and display it.</summary>
        public Portrait portraitInformation;

        /// <summary>The default location for this npc to reside in.</summary>
        public GameLocation defaultLocation;

        /// <summary>Construct an instance.</summary>
        public ExtendedNpc() { }

        /// <summary>Construct an instance.</summary>
        /// <param name="sprite">The sprite for the character.</param>
        /// <param name="position">The position of the npc on the map.</param>
        /// <param name="facingDirection">The direction of the npc</param>
        /// <param name="name">The name of the npc.</param>
        public ExtendedNpc(Sprite sprite, Vector2 position, int facingDirection, string name)
            : base(sprite.sprite, position, facingDirection, name)
        {
            this.characterRenderer = null;
            this.Portrait = null;
            this.portraitInformation = null;
            this.spriteInformation = sprite;
            this.spriteInformation?.setCharacterSpriteFromThis(this);
            this.swimming.Value = false;
        }

        /// <summary>Construct an instance.</summary>
        /// <param name="sprite">The sprite for the character.</param>
        /// <param name="portrait">The portrait texture for this npc.</param>
        /// <param name="position">The position of the npc on the map.</param>
        /// <param name="facingDirection">The direction of the npc</param>
        /// <param name="name">The name of the npc.</param>
        public ExtendedNpc(Sprite sprite, Portrait portrait, Vector2 position, int facingDirection, string name)
            : base(sprite.sprite, position, facingDirection, name)
        {
            this.characterRenderer = null;
            this.portraitInformation = portrait;
            this.portraitInformation?.setCharacterPortraitFromThis(this);
            this.spriteInformation = sprite;
            this.spriteInformation.setCharacterSpriteFromThis(this);
            this.swimming.Value = false;
        }

        /// <summary>Construct an instance.</summary>
        /// <param name="sprite">The sprite for the character to use incase the renderer is null.</param>
        /// <param name="renderer">The custom npc render. Used to draw the npcfrom a collection of assets.</param>
        /// <param name="position">The position of the npc on the map.</param>
        /// <param name="facingDirection">The direction of the npc</param>
        /// <param name="name">The name of the npc.</param>
        public ExtendedNpc(Sprite sprite, BasicRenderer renderer, Vector2 position, int facingDirection, string name)
            : base(sprite.sprite, position, facingDirection, name)
        {
            this.characterRenderer = renderer;
            this.Portrait = null;
            this.portraitInformation = null;
            this.spriteInformation = sprite;
            this.spriteInformation?.setCharacterSpriteFromThis(this);
            this.swimming.Value = false;
        }

        /// <summary>Construct an instance.</summary>
        /// <param name="sprite">The sprite for the npc to use incase the renderer is null.</param>
        /// <param name="renderer">The custom npc renderer used to draw the npc from the collection of textures.</param>
        /// <param name="portrait">The portrait texture for the npc.</param>
        /// <param name="position">The positon for the npc to be.</param>
        /// <param name="facingDirection">The direction for the npc to face.</param>
        /// <param name="name">The name for the npc.</param>
        public ExtendedNpc(Sprite sprite, BasicRenderer renderer, Portrait portrait, Vector2 position, int facingDirection, string name)
            : base(sprite.sprite, position, facingDirection, name)
        {
            this.characterRenderer = renderer;
            this.portraitInformation = portrait;
            this.portraitInformation?.setCharacterPortraitFromThis(this);
            this.spriteInformation = sprite;
            this.spriteInformation?.setCharacterSpriteFromThis(this);
            this.swimming.Value = false;
        }

        /// <summary>Used to reload the sprite for the npc.</summary>
        public void reloadSprite()
        {
            if (this.characterRenderer == null)
            {
                this.spriteInformation.reload();
                try
                {
                    this.portraitInformation.reload();
                }
                catch
                {
                    this.Portrait = null;
                }
            }
            else
            {
                this.characterRenderer.reloadSprites();
                try
                {
                    this.portraitInformation.reload();
                }
                catch
                {
                    this.Portrait = null;
                }
            }
            if (!Game1.newDay && Game1.gameMode != 6)
                return;
            this.faceDirection(this.DefaultFacingDirection);
            this.scheduleTimeToTry = 9999999;
            this.previousEndPoint = new Point((int)this.DefaultPosition.X / Game1.tileSize, (int)this.DefaultPosition.Y / Game1.tileSize);
            this.Schedule = this.getSchedule(Game1.dayOfMonth);
            this.faceDirection(this.defaultFacingDirection);
            this.Sprite.standAndFaceDirection(this.defaultFacingDirection);

            if (this.isMarried())
                this.marriageDuties();
            try
            {
                this.displayName = this.Name;
            }
            catch { }
        }

        /// <summary>Functionality used when interacting with the npc.</summary>
        public override bool checkAction(Farmer who, GameLocation l)
        {
            base.checkAction(who, l);
            return false;
        }

        /// <summary>Used to move the npc. Different code is used depending if the character renderer is null or not.</summary>
        public override void MovePosition(GameTime time, xTile.Dimensions.Rectangle viewport, GameLocation currentLocation)
        {
            if (this.characterRenderer != null)
                this.ModularMovement(time, viewport, currentLocation);
            else
                this.NonModularMovement(time, viewport, currentLocation);
        }

        /// <summary>Set's the npc to move a certain direction and then executes the movement.</summary>
        /// <param name="moveDirection">The direction to move the npc.</param>
        /// <param name="Move">Set's the npc's sprite to halt if Move=false. Else set it to true.</param>
        public virtual void SetMovingAndMove(GameTime time, xTile.Dimensions.Rectangle viewport, GameLocation currentLocation, Direction moveDirection, bool Move = true)
        {
            switch (moveDirection)
            {
                case Direction.down:
                    this.SetMovingDown(Move);
                    break;

                case Direction.left:
                    this.SetMovingLeft(Move);
                    break;

                case Direction.up:
                    this.SetMovingUp(Move);
                    break;

                case Direction.right:
                    this.SetMovingRight(Move);
                    break;
            }

            this.MovePosition(time, viewport, currentLocation);
        }

        /// <summary>USed to move the npc if the character renderer is null.</summary>
        public virtual void NonModularMovement(GameTime time, xTile.Dimensions.Rectangle viewport, GameLocation location)
        {
            base.MovePosition(time, viewport, this.currentLocation);
        }

        /// <summary>Used to determine if the npc can move past the next location.</summary>
        public virtual bool canMovePastNextLocation(xTile.Dimensions.Rectangle viewport)
        {
            //Up
            if (!this.currentLocation.isTilePassable(this.nextPosition(0), viewport) || !this.willDestroyObjectsUnderfoot)
                return false;

            //Right
            if (!this.currentLocation.isTilePassable(this.nextPosition(1), viewport) || !this.willDestroyObjectsUnderfoot)
                return false;

            //Down
            if (!this.currentLocation.isTilePassable(this.nextPosition(2), viewport) || !this.willDestroyObjectsUnderfoot)
                return false;

            //Left
            if (!this.currentLocation.isTilePassable(this.nextPosition(3), viewport) || !this.willDestroyObjectsUnderfoot)
                return false;

            return true;
        }

        /// <summary>Used to move the npc if the character renderer is valid. Handles animating all of the sprites associated with the renderer.</summary>
        public virtual void ModularMovement(GameTime time, xTile.Dimensions.Rectangle viewport, GameLocation location, float interval = 1000f)
        {
            this.characterRenderer.setAnimation(AnimationKeys.walkingKey);
            if (!this.canMovePastNextLocation(viewport))
            {
                this.Halt();
                return;
            }
            if (this.GetType() == typeof(FarmAnimal))
                this.willDestroyObjectsUnderfoot = false;
            if (this.xVelocity != 0.0 || this.yVelocity != 0.0)
            {
                var boundingBox = this.GetBoundingBox();
                boundingBox.X += (int)this.xVelocity;
                boundingBox.Y -= (int)this.yVelocity;
                if (this.currentLocation == null || !this.currentLocation.isCollidingPosition(boundingBox, viewport, false, 0, false, this))
                {
                    this.position.X += this.xVelocity;
                    this.position.Y -= this.yVelocity;
                }
                this.xVelocity = (int)(this.xVelocity - this.xVelocity / 2.0);
                this.yVelocity = (int)(this.yVelocity - this.yVelocity / 2.0);
            }
            else if (this.moveUp)
            {
                if (this.currentLocation == null || !this.currentLocation.isCollidingPosition(this.nextPosition(0), viewport, false, 0, false, this) || this.isCharging)
                {
                    this.position.Y -= this.speed + this.addedSpeed;
                    if (!this.ignoreMovementAnimation)
                    {
                        this.spriteInformation.setUp(this);
                        this.characterRenderer.Animate(interval);
                        //this.sprite.AnimateUp(time, (this.speed - 2 + this.addedSpeed) * -25, Utility.isOnScreen(this.getTileLocationPoint(), 1, currentLocation) ? "Cowboy_Footstep" : "");
                        this.faceDirection(0);
                    }
                }
                else if (!this.currentLocation.isTilePassable(this.nextPosition(0), viewport) || !this.willDestroyObjectsUnderfoot)
                    this.Halt();
                else if (this.willDestroyObjectsUnderfoot)
                {
                    Vector2 vector2 = new Vector2(this.getStandingX() / Game1.tileSize, this.getStandingY() / Game1.tileSize - 1);
                    if (this.currentLocation.characterDestroyObjectWithinRectangle(this.nextPosition(0), true))
                    {
                        this.doEmote(12);
                        this.position.Y -= this.speed + this.addedSpeed;
                    }
                    else
                        this.blockedInterval = this.blockedInterval + time.ElapsedGameTime.Milliseconds;
                }
            }
            else if (this.moveRight)
            {
                if (this.currentLocation == null || !this.currentLocation.isCollidingPosition(this.nextPosition(1), viewport, false, 0, false, this) || this.isCharging)
                {
                    this.position.X += this.speed + this.addedSpeed;
                    if (!this.ignoreMovementAnimation)
                    {
                        this.spriteInformation.setRight(this);
                        this.characterRenderer.Animate(interval);
                        //this.spriteInformation.sprite.Animate(time, 0, 3, 1f);
                        //this.sprite.AnimateRight(time, (this.speed - 2 + this.addedSpeed) * -25, Utility.isOnScreen(this.getTileLocationPoint(), 1, currentLocation) ? "Cowboy_Footstep" : "");
                        this.faceDirection(1);
                    }
                }
                else if (!this.currentLocation.isTilePassable(this.nextPosition(1), viewport) || !this.willDestroyObjectsUnderfoot)
                    this.Halt();
                else if (this.willDestroyObjectsUnderfoot)
                {
                    Vector2 vector2 = new Vector2(this.getStandingX() / Game1.tileSize + 1, this.getStandingY() / Game1.tileSize);
                    if (this.currentLocation.characterDestroyObjectWithinRectangle(this.nextPosition(1), true))
                    {
                        this.doEmote(12);
                        this.position.X += this.speed + this.addedSpeed;
                    }
                    else
                        this.blockedInterval = this.blockedInterval + time.ElapsedGameTime.Milliseconds;
                }
            }
            else if (this.moveDown)
            {
                if (this.currentLocation == null || !this.currentLocation.isCollidingPosition(this.nextPosition(2), viewport, false, 0, false, this) || this.isCharging)
                {
                    this.position.Y += this.speed + this.addedSpeed;
                    if (!this.ignoreMovementAnimation)
                    {
                        this.spriteInformation.setDown(this);
                        this.characterRenderer.Animate(interval);
                        //this.spriteInformation.sprite.Animate(time, 0, 3, 1f);
                        //this.sprite.AnimateDown(time, (this.speed - 2 + this.addedSpeed) * -25, Utility.isOnScreen(this.getTileLocationPoint(), 1, currentLocation) ? "Cowboy_Footstep" : "");
                        this.faceDirection(2);
                    }
                }
                else if (!this.currentLocation.isTilePassable(this.nextPosition(2), viewport) || !this.willDestroyObjectsUnderfoot)
                    this.Halt();
                else if (this.willDestroyObjectsUnderfoot)
                {
                    Vector2 vector2 = new Vector2(this.getStandingX() / Game1.tileSize, this.getStandingY() / Game1.tileSize + 1);
                    if (this.currentLocation.characterDestroyObjectWithinRectangle(this.nextPosition(2), true))
                    {
                        this.doEmote(12);
                        this.position.Y += this.speed + this.addedSpeed;
                    }
                    else
                        this.blockedInterval = this.blockedInterval + time.ElapsedGameTime.Milliseconds;
                }
            }
            else if (this.moveLeft)
            {
                if (this.currentLocation == null || !this.currentLocation.isCollidingPosition(this.nextPosition(3), viewport, false, 0, false, this) || this.isCharging)
                {
                    this.position.X -= this.speed + this.addedSpeed;
                    if (!this.ignoreMovementAnimation)
                    {
                        this.spriteInformation.setLeft(this);
                        this.characterRenderer.Animate(interval);
                        //this.spriteInformation.sprite.Animate(time, 0, 3, 1f);
                        //this.sprite.AnimateLeft(time, (this.speed - 2 + this.addedSpeed) * -25, Utility.isOnScreen(this.getTileLocationPoint(), 1, currentLocation) ? "Cowboy_Footstep" : "");
                        this.faceDirection(3);
                    }
                }
                else if (!this.currentLocation.isTilePassable(this.nextPosition(3), viewport) || !this.willDestroyObjectsUnderfoot)
                    this.Halt();
                else if (this.willDestroyObjectsUnderfoot)
                {
                    Vector2 vector2 = new Vector2(this.getStandingX() / Game1.tileSize - 1, this.getStandingY() / Game1.tileSize);
                    if (this.currentLocation.characterDestroyObjectWithinRectangle(this.nextPosition(3), true))
                    {
                        this.doEmote(12);
                        this.position.X -= this.speed + this.addedSpeed;
                    }
                    else
                        this.blockedInterval = this.blockedInterval + time.ElapsedGameTime.Milliseconds;
                }
            }
            if (this.blockedInterval >= 3000 && this.blockedInterval <= 3750.0 && !Game1.eventUp)
            {
                this.doEmote(Game1.random.NextDouble() < 0.5 ? 8 : 40);
                this.blockedInterval = 3750;
            }
            else
            {
                if (this.blockedInterval < 5000)
                    return;
                this.speed = 4;
                this.isCharging = true;
                this.blockedInterval = 0;
            }
        }

        /// <summary>Used to halt the npc sprite. Sets the npc's animation to the standing animation if the character renderer is not null.</summary>
        public override void Halt()
        {
            this.characterRenderer?.setAnimation(AnimationKeys.standingKey);
            base.Halt();
        }

        /// <summary>Pathfinding code.</summary>
        public virtual void routeEndAnimationFinished(Farmer who)
        {
            this.doingEndOfRouteAnimation.Value = false;
            this.freezeMotion = false;
            this.Sprite.SpriteHeight = 32;
            this.Sprite.StopAnimation();
            this.endOfRouteMessage.Value = (string)null;
            this.isCharging = false;
            this.speed = 2;
            this.addedSpeed = 0;
            this.goingToDoEndOfRouteAnimation.Value = false;
            if (!this.IsWalkingInSquare)
                return;
            this.returningToEndPoint = true;
            this.timeAfterSquare = Game1.timeOfDay;
        }

        /// <summary>Pathfinding code.</summary>
        public virtual void doAnimationAtEndOfScheduleRoute(Character c, GameLocation l) { }


        /// <summary>Pathfinding code.</summary>
        public virtual void startRouteBehavior(string behaviorName)
        {
            if (behaviorName.Length > 0 && (int)behaviorName[0] == 34)
                this.endOfRouteMessage.Value = behaviorName.Replace("\"", "");
            else
            {
                if (behaviorName.Contains("square_"))
                {
                    this.lastCrossroad = new Microsoft.Xna.Framework.Rectangle(this.getTileX() * Game1.tileSize, this.getTileY() * Game1.tileSize, Game1.tileSize, Game1.tileSize);
                    string[] strArray = behaviorName.Split('_');
                    this.walkInSquare(Convert.ToInt32(strArray[1]), Convert.ToInt32(strArray[2]), 6000);
                    this.squareMovementFacingPreference = strArray.Length <= 3 ? -1 : Convert.ToInt32(strArray[3]);
                }
                else
                {
                    Utility.getGameLocationOfCharacter(this).temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(167, 1714, 19, 14), 100f, 3, 999999, new Vector2(2f, 3f) * (float)Game1.tileSize + new Vector2(7f, 12f) * (float)Game1.pixelZoom, false, false, 0.0002f, 0.0f, Color.White, (float)Game1.pixelZoom, 0.0f, 0.0f, 0.0f)
                    {
                        id = 688f
                    });
                    this.doEmote(52);
                }
            }
        }


        /// <summary>Occurs when the npc gets hit by the player.</summary>
        public new void getHitByPlayer(StardewValley.Farmer who, GameLocation location)
        {
            this.doEmote(12);
            if (who == null)
            {
                if (Game1.IsMultiplayer)
                    return;
                who = Game1.player;
            }
            if (who.friendshipData.ContainsKey(this.Name))
            {
                who.friendshipData.TryGetValue(this.Name, out Friendship f);
                f.Points -= 30;
                if (who.IsMainPlayer)
                {
                    this.CurrentDialogue.Clear();
                    //this.CurrentDialogue.Push(new StardewValley.Dialogue(Game1.random.NextDouble() < 0.5 ? Game1.LoadStringByGender(this.gender, "Strings\\StringsFromCSFiles:NPC.cs.4293") : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4294"), this));
                }
                //location.debris.Add(new Debris(this.sprite.Texture, Game1.random.Next(3, 8), new Vector2((float)this.GetBoundingBox().Center.X, (float)this.GetBoundingBox().Center.Y)));
            }
            if (this.Name.Equals("Bouncer"))
                Game1.playSound("crafting");
            else
                Game1.playSound("hitEnemy");
        }

        //ERROR NEED FIXING
        public override void dayUpdate(int dayOfMonth)
        {
            if (this.currentLocation != null)
                Game1.warpCharacter(this, this.DefaultMap, this.DefaultPosition / Game1.tileSize);
            Game1.player.mailReceived.Remove(this.Name);
            Game1.player.mailReceived.Remove(this.Name + "Cooking");
            this.doingEndOfRouteAnimation.Value = false;
            this.Halt();
            this.hasBeenKissedToday = false;
            this.faceTowardFarmer = false;
            this.faceTowardFarmerTimer = 0;
            this.drawOffset.Value = Vector2.Zero;
            this.hasSaidAfternoonDialogue = false;
            this.ignoreScheduleToday = false;
            this.Halt();
            this.controller = null;
            this.temporaryController = null;
            this.DirectionsToNewLocation = null;
            this.faceDirection(this.DefaultFacingDirection);
            this.scheduleTimeToTry = 9999999;
            this.previousEndPoint = new Point((int)this.DefaultPosition.X / Game1.tileSize, (int)this.DefaultPosition.Y / Game1.tileSize);
            this.IsWalkingInSquare = false;
            this.returningToEndPoint = false;
            this.lastCrossroad = Rectangle.Empty;
            if (this.isVillager())
                this.Schedule = this.getSchedule(dayOfMonth);
            this.endOfRouteMessage.Value = null;
            bool flag = Utility.isFestivalDay(dayOfMonth, Game1.currentSeason);
            if (!this.isMarried())
                return;
            this.marriageDuties();
            //Friendship f=Game1.player.GetSpouseFriendship();
            //this.daysMarried = this.daysMarried + 1;
        }

        /// <summary>Does effectively nothing.</summary>
        public new void setUpForOutdoorPatioActivity() { }

        /// <summary>Used to draw the npc with the custom renderer.</summary>
        public virtual void drawModular(SpriteBatch b, float alpha = 1f)
        {
            if (this.characterRenderer == null || this.IsInvisible || !Utility.isOnScreen(this.position, 2 * Game1.tileSize))
                return;
            //Checks if the npc is swimming. If not draw it's default graphic. Do characters aside from Farmer and Penny Swim???
            if (this.swimming.Value)
            {
                this.characterRenderer.setAnimation(AnimationKeys.swimmingKey);
                this.characterRenderer.setDirection(this.facingDirection);
                this.characterRenderer.draw(b, this, this.getLocalPosition(Game1.viewport) + new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize + Game1.tileSize / 4 + this.yJumpOffset * 2)) + (this.shakeTimer > 0 ? new Vector2((float)Game1.random.Next(-1, 2), (float)Game1.random.Next(-1, 2)) : Vector2.Zero) - new Vector2(0.0f, this.yOffset), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(this.Sprite.SourceRect.X, this.Sprite.SourceRect.Y, this.Sprite.SourceRect.Width, this.Sprite.SourceRect.Height / 2 - (int)((double)this.yOffset / (double)Game1.pixelZoom))), Color.White, this.rotation, new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize * 3 / 2)) / 4f, Math.Max(0.2f, this.Scale) * (float)Game1.pixelZoom, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, (float)this.getStandingY() / 10000f));
                //Vector2 localPosition = this.getLocalPosition(Game1.viewport);
                //b.Draw(Game1.staminaRect, new Microsoft.Xna.Framework.Rectangle((int)localPosition.X + (int)this.yOffset + Game1.pixelZoom * 2, (int)localPosition.Y - 32 * Game1.pixelZoom + this.sprite.SourceRect.Height * Game1.pixelZoom + Game1.tileSize * 3 / 4 + this.yJumpOffset * 2 - (int)this.yOffset, this.sprite.SourceRect.Width * Game1.pixelZoom - (int)this.yOffset * 2 - Game1.pixelZoom * 4, Game1.pixelZoom), new Microsoft.Xna.Framework.Rectangle?(Game1.staminaRect.Bounds), Color.White * 0.75f, 0.0f, Vector2.Zero, SpriteEffects.None, (float)((double)this.getStandingY() / 10000.0 + 1.0 / 1000.0));
            }
            else
            {
                //FIX THIS LINE WITH LAYER DEPTH!!!
                //Shadow???
                //this.characterRenderer.draw(b,this, this.getLocalPosition(Game1.viewport) + new Vector2((float)(this.sprite.spriteWidth * Game1.pixelZoom / 2), (float)(this.GetBoundingBox().Height / 2)) + (this.shakeTimer > 0 ? new Vector2((float)Game1.random.Next(-1, 2), (float)Game1.random.Next(-1, 2)) : Vector2.Zero), new Microsoft.Xna.Framework.Rectangle?(this.Sprite.SourceRect), Color.White * alpha, this.rotation, new Vector2((float)(this.sprite.spriteWidth / 2), (float)((double)this.sprite.spriteHeight * 3.0 / 4.0)), Math.Max(0.2f, this.scale) * (float)Game1.pixelZoom, this.flip || this.sprite.currentAnimation != null && this.sprite.currentAnimation[this.sprite.currentAnimationIndex].flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float)this.getStandingY() / 10000f));
            }
            //If the npc breathes then this code is ran.
            if (this.Breather && this.shakeTimer <= 0 && (!this.swimming.Value && this.Sprite.CurrentFrame < 16) && !this.farmerPassesThrough)
            {
                Microsoft.Xna.Framework.Rectangle sourceRect = this.Sprite.SourceRect;
                sourceRect.Y += this.Sprite.SpriteHeight / 2 + this.Sprite.SpriteHeight / 32;
                sourceRect.Height = this.Sprite.SpriteHeight / 4;
                sourceRect.X += this.Sprite.SpriteWidth / 4;
                sourceRect.Width = this.Sprite.SpriteWidth / 2;
                Vector2 vector2 = new Vector2((float)(this.Sprite.SpriteWidth * Game1.pixelZoom / 2), (float)(Game1.tileSize / 8));
                if (this.Age == 2)
                {
                    sourceRect.Y += this.Sprite.SpriteHeight / 6 + 1;
                    sourceRect.Height /= 2;
                    vector2.Y += (float)(this.Sprite.SpriteHeight / 8 * Game1.pixelZoom);
                }
                else if (this.Gender == 1)
                {
                    ++sourceRect.Y;
                    vector2.Y -= (float)Game1.pixelZoom;
                    sourceRect.Height /= 2;
                }
                //The actual character drawing to the screen?
                this.characterRenderer.draw(b, this, this.getLocalPosition(Game1.viewport) + (this.shakeTimer > 0 ? new Vector2((float)Game1.random.Next(-1, 2), (float)Game1.random.Next(-1, 2)) : Vector2.Zero), new Microsoft.Xna.Framework.Rectangle?(sourceRect), Color.White * alpha, this.rotation, Vector2.Zero, Math.Max(0.2f, this.Scale), this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.992f : (float)((double)this.getStandingY() / 10000.0 + 1.0 / 1000.0)));
                //this.characterRenderer.draw(b,this, this.getLocalPosition(Game1.viewport) + vector2 + (this.shakeTimer > 0 ? new Vector2((float)Game1.random.Next(-1, 2), (float)Game1.random.Next(-1, 2)) : Vector2.Zero), new Microsoft.Xna.Framework.Rectangle?(sourceRect), Color.White * alpha, this.rotation, new Vector2((float)(sourceRect.Width / 2), (float)(sourceRect.Height / 2 + 1)), Math.Max(0.2f, this.scale) * (float)Game1.pixelZoom + num, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.992f : (float)((double)this.getStandingY() / 10000.0 + 1.0 / 1000.0)));
                //this.characterRenderer.draw(b, this, this.getLocalPosition(Game1.viewport) + vector2 + (this.shakeTimer > 0 ? new Vector2((float)Game1.random.Next(-1, 2), (float)Game1.random.Next(-1, 2)) : Vector2.Zero), new Microsoft.Xna.Framework.Rectangle?(sourceRect), Color.White * alpha, this.rotation, new Vector2((float)(sourceRect.Width / 2), (float)(sourceRect.Height / 2 + 1)), Math.Max(0.2f, this.scale) * (float)Game1.pixelZoom + num, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, .99f);
            }
            else
            {
                //float num = Math.Max(0.0f, (float)(Math.Ceiling(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 600.0 + (double)this.DefaultPosition.X * 20.0)) / 4.0));
                this.characterRenderer.draw(b, this, this.getLocalPosition(Game1.viewport) + (this.shakeTimer > 0 ? new Vector2((float)Game1.random.Next(-1, 2), (float)Game1.random.Next(-1, 2)) : Vector2.Zero), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle()), Color.White * alpha, this.rotation, Vector2.Zero, Math.Max(0.2f, this.Scale), this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.992f : (float)((double)this.getStandingY() / 10000.0 + 1.0 / 1000.0)));
            }

            //Checks if the npc is glowing.
            if (this.isGlowing)
                this.characterRenderer.draw(b, this, this.getLocalPosition(Game1.viewport) + new Vector2((float)(this.Sprite.SpriteWidth * Game1.pixelZoom / 2), (float)(this.GetBoundingBox().Height / 2)) + (this.shakeTimer > 0 ? new Vector2((float)Game1.random.Next(-1, 2), (float)Game1.random.Next(-1, 2)) : Vector2.Zero), new Microsoft.Xna.Framework.Rectangle?(this.Sprite.SourceRect), this.glowingColor * this.glowingTransparency, this.rotation, new Vector2((float)(this.Sprite.SpriteWidth / 2), (float)((double)this.Sprite.SpriteHeight * 3.0 / 4.0)), Math.Max(0.2f, this.Scale) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.99f : (float)((double)this.getStandingY() / 10000.0 + 1.0 / 1000.0)));

            //This code runs if the npc is emoting.
            if (!this.IsEmoting || Game1.eventUp)
                return;
            Vector2 localPosition1 = this.getLocalPosition(Game1.viewport);
            localPosition1.Y -= (float)(Game1.tileSize / 2 + this.Sprite.SpriteHeight * Game1.pixelZoom);
            b.Draw(Game1.emoteSpriteSheet, localPosition1, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(this.CurrentEmoteIndex * 16 % Game1.emoteSpriteSheet.Width, this.CurrentEmoteIndex * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16)), Color.White, 0.0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, (float)this.getStandingY() / 10000f);
        }

        /// <summary>Used to draw the sprite without the modular npc renderer</summary>
        /// <param name="b"></param>
        /// <param name="alpha"></param>
        public virtual void drawNonModularSprite(SpriteBatch b, float alpha = 1f)
        {
            if (this.Sprite == null || this.IsInvisible || !Utility.isOnScreen(this.position, 2 * Game1.tileSize))
                return;

            //Swimming just has a height difference on the sprite.
            if (this.swimming.Value)
            {
                b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize + Game1.tileSize / 4 + this.yJumpOffset * 2)) + (this.shakeTimer > 0 ? new Vector2((float)Game1.random.Next(-1, 2), (float)Game1.random.Next(-1, 2)) : Vector2.Zero) - new Vector2(0.0f, this.yOffset), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(this.Sprite.SourceRect.X, this.Sprite.SourceRect.Y, this.Sprite.SourceRect.Width, this.Sprite.SourceRect.Height / 2 - (int)((double)this.yOffset / (double)Game1.pixelZoom))), Color.White, this.rotation, new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize * 3 / 2)) / 4f, Math.Max(0.2f, this.Scale) * (float)Game1.pixelZoom, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float)this.getStandingY() / 10000f));
                Vector2 localPosition = this.getLocalPosition(Game1.viewport);
                //b.Draw(Game1.staminaRect, new Microsoft.Xna.Framework.Rectangle((int)localPosition.X + (int)this.yOffset + Game1.pixelZoom * 2, (int)localPosition.Y - 32 * Game1.pixelZoom + this.sprite.SourceRect.Height * Game1.pixelZoom + Game1.tileSize * 3 / 4 + this.yJumpOffset * 2 - (int)this.yOffset, this.sprite.SourceRect.Width * Game1.pixelZoom - (int)this.yOffset * 2 - Game1.pixelZoom * 4, Game1.pixelZoom), new Microsoft.Xna.Framework.Rectangle?(Game1.staminaRect.Bounds), Color.White * 0.75f, 0.0f, Vector2.Zero, SpriteEffects.None, (float)((double)this.getStandingY() / 10000.0 + 1.0 / 1000.0));
            }
            else
                b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2((float)(this.Sprite.SpriteWidth * Game1.pixelZoom / 2), (float)(this.GetBoundingBox().Height / 2)) + (this.shakeTimer > 0 ? new Vector2((float)Game1.random.Next(-1, 2), (float)Game1.random.Next(-1, 2)) : Vector2.Zero), new Microsoft.Xna.Framework.Rectangle?(this.Sprite.SourceRect), Color.White * alpha, this.rotation, new Vector2((float)(this.Sprite.SpriteWidth / 2), (float)((double)this.Sprite.SpriteHeight * 3.0 / 4.0)), Math.Max(0.2f, this.Scale) * (float)Game1.pixelZoom, this.flip || this.Sprite.currentAnimation != null && this.Sprite.currentAnimation[this.Sprite.currentAnimationIndex].flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float)this.getStandingY() / 10000f));
            if (this.Breather && this.shakeTimer <= 0 && (!this.swimming.Value && this.Sprite.CurrentFrame < 16) && !this.farmerPassesThrough)
            {
                Microsoft.Xna.Framework.Rectangle sourceRect = this.Sprite.SourceRect;
                sourceRect.Y += this.Sprite.SpriteHeight / 2 + this.Sprite.SpriteHeight / 32;
                sourceRect.Height = this.Sprite.SpriteHeight / 4;
                sourceRect.X += this.Sprite.SpriteWidth / 4;
                sourceRect.Width = this.Sprite.SpriteWidth / 2;
                Vector2 vector2 = new Vector2((float)(this.Sprite.SpriteWidth * Game1.pixelZoom / 2), (float)(Game1.tileSize / 8));
                if (this.Age == 2)
                {
                    sourceRect.Y += this.Sprite.SpriteHeight / 6 + 1;
                    sourceRect.Height /= 2;
                    vector2.Y += (float)(this.Sprite.SpriteHeight / 8 * Game1.pixelZoom);
                }
                else if (this.Gender == 1)
                {
                    ++sourceRect.Y;
                    vector2.Y -= (float)Game1.pixelZoom;
                    sourceRect.Height /= 2;
                }
                float num = Math.Max(0.0f, (float)(Math.Ceiling(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 600.0 + (double)this.DefaultPosition.X * 20.0)) / 4.0));

                b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + vector2 + (this.shakeTimer > 0 ? new Vector2((float)Game1.random.Next(-1, 2), (float)Game1.random.Next(-1, 2)) : Vector2.Zero), new Microsoft.Xna.Framework.Rectangle?(sourceRect), Color.White * alpha, this.rotation, new Vector2((float)(sourceRect.Width / 2), (float)(sourceRect.Height / 2 + 1)), Math.Max(0.2f, this.Scale) * (float)Game1.pixelZoom + num, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.992f : (float)((double)this.getStandingY() / 10000.0 + 1.0 / 1000.0)));
            }
            if (this.isGlowing)
                b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2((float)(this.Sprite.SpriteWidth * Game1.pixelZoom / 2), (float)(this.GetBoundingBox().Height / 2)) + (this.shakeTimer > 0 ? new Vector2((float)Game1.random.Next(-1, 2), (float)Game1.random.Next(-1, 2)) : Vector2.Zero), new Microsoft.Xna.Framework.Rectangle?(this.Sprite.SourceRect), this.glowingColor * this.glowingTransparency, this.rotation, new Vector2((float)(this.Sprite.SpriteWidth / 2), (float)((double)this.Sprite.SpriteHeight * 3.0 / 4.0)), Math.Max(0.2f, this.Scale) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.99f : (float)((double)this.getStandingY() / 10000.0 + 1.0 / 1000.0)));
            Vector2 localPosition1 = this.getLocalPosition(Game1.viewport);
            localPosition1.Y -= (float)(Game1.tileSize / 2 + this.Sprite.SpriteHeight * Game1.pixelZoom);
            //b.Draw(Game1.emoteSpriteSheet, localPosition1, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(this.CurrentEmoteIndex * 16 % Game1.emoteSpriteSheet.Width, this.CurrentEmoteIndex * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16)), Color.White, 0.0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, (float)this.getStandingY() / 10000f);
        }


        /// <summary>Basic draw functionality to checkn whether or not to draw the npc using it's default sprite or using a custom character renderer.</summary>
        /// <param name="b"></param>
        /// <param name="alpha"></param>
        public override void draw(SpriteBatch b, float alpha = 1f)
        {
            if (this.characterRenderer == null)
            {
                this.drawNonModularSprite(b, alpha);
            }
            else
            {
                this.drawModular(b, alpha);
            }
        }
    }
}
