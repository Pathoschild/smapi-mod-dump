/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewDruid.Cast;
using StardewDruid.Map;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;


namespace StardewDruid.Character
{
    public class Dragon : StardewDruid.Character.Character
    {
        public NetInt netDirection = new NetInt(0);
        public NetInt netAlternative = new NetInt(0);
        public Texture2D shadowTexture;
        public bool endTransform;
        public List<Rectangle> shadowFrames;
        public SButton leftButton;
        public SButton rightButton;
        public NetInt netFlightFrame = new NetInt(0);
        public NetInt netFlightHeight = new NetInt(0);
        public NetBool netDashActive = new NetBool(false);
        public Texture2D flightTexture;
        public Dictionary<int, List<Rectangle>> flightFrames;
        public bool flightActive;
        public int flightDelay;
        public int flightTimer;
        public Vector2 flightPosition;
        public Vector2 flightTo;
        public Vector2 flightInterval;
        public bool flightFlip;
        public int flightIncrement;
        public int strikeTimer;
        public int flightExtend;
        public string flightTerrain;
        public NetInt netFireFrame = new NetInt(0);
        public NetBool netFireActive = new NetBool(false);
        public Texture2D breathTexture;
        public bool breathActive;
        public int breathDelay;
        public int breathTimer;
        public Texture2D fireTexture;
        public Dictionary<int, List<Rectangle>> fireFrames;
        public List<Vector2> fireVectors;
        public int fireTimer;
        public bool avatar;
        public Dictionary<Vector2, int> aoeZones;
        public Dictionary<Vector2,TemporaryAnimatedSprite> burnZones;
        public int aoeTimer;
        public int burnDamage;

        public Dragon()
        {
        }

        public Dragon(Vector2 position, string map, string Name)
          : base(position, map, Name)
        {
            moveDirection = Game1.player.FacingDirection;
            flightInterval = Vector2.Zero;
            breather.Value = false;
            hideShadow.Value = true;
            flightTo = Vector2.Zero;
            avatar = true;
            AnimateMovement(Game1.currentGameTime);
            LoadOut();
        }

        public void LoadOut()
        {
            shadowTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "DragonShadow.png"));
            shadowFrames = new List<Rectangle>()
            {
            new Rectangle(0, 0, 64, 32),
            new Rectangle(0, 32, 64, 32),
            new Rectangle(0, 64, 64, 32),
            new Rectangle(0, 32, 64, 32),
            new Rectangle(64, 0, 64, 32),
            new Rectangle(64, 32, 64, 32),
            new Rectangle(64, 64, 64, 32),
            new Rectangle(64, 32, 64, 32)
            };
            flightTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", Name + "Flight.png"));
            flightFrames = new Dictionary<int, List<Rectangle>>()
            {
                [0] = new List<Rectangle>()
                {
                new Rectangle(0, 64, 128, 64),
                new Rectangle(128, 64, 128, 64),
                new Rectangle(256, 64, 128, 64),
                new Rectangle(384, 64, 128, 64),
                new Rectangle(256, 64, 128, 64),
                new Rectangle(0, 64, 128, 64)
                },
                [1] = new List<Rectangle>()
                {
                new Rectangle(0, 0, 128, 64),
                new Rectangle(128, 0, 128, 64),
                new Rectangle(256, 0, 128, 64),
                new Rectangle(384, 0, 128, 64),
                new Rectangle(256, 0, 128, 64),
                new Rectangle(0, 0, 128, 64)
                },
                [2] = new List<Rectangle>()
                {
                new Rectangle(0, 128, 128, 64),
                new Rectangle(128, 128, 128, 64),
                new Rectangle(256, 128, 128, 64),
                new Rectangle(384, 128, 128, 64),
                new Rectangle(256, 128, 128, 64),
                new Rectangle(0, 128, 128, 64)
                },
                [3] = new List<Rectangle>()
                {
                new Rectangle(0, 0, 128, 64),
                new Rectangle(128, 0, 128, 64),
                new Rectangle(256, 0, 128, 64),
                new Rectangle(384, 0, 128, 64),
                new Rectangle(256, 0, 128, 64),
                new Rectangle(0, 0, 128, 64)
                }
            };
            breathTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", Name + "Breath.png"));
            fireTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "DragonFire.png"));
            //pointTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "FirePoint.png"));
            fireFrames = new Dictionary<int, List<Rectangle>>()
            {
                [0] = new List<Rectangle>()
                {
                new Rectangle(0, 0, 64, 32),
                new Rectangle(64, 0, 64, 32),
                new Rectangle(128, 0, 64, 32),
                new Rectangle(192, 0, 64, 32)
                },
                [1] = new List<Rectangle>()
                {
                new Rectangle(0, 32, 64, 32),
                new Rectangle(64, 32, 64, 32),
                new Rectangle(128, 32, 64, 32),
                new Rectangle(192, 32, 64, 32)
                },
                [2] = new List<Rectangle>()
                {
                new Rectangle(0, 64, 64, 32),
                new Rectangle(64, 64, 64, 32),
                new Rectangle(128, 64, 64, 32),
                new Rectangle(192, 64, 64, 32)
                },
                [3] = new List<Rectangle>()
                {
                new Rectangle(0, 32, 64, 32),
                new Rectangle(64, 32, 64, 32),
                new Rectangle(128, 32, 64, 32),
                new Rectangle(192, 32, 64, 32)
                }
            };
            fireVectors = new List<Vector2>()
            {
                new Vector2(44f, -300f),
                new Vector2(-300f, -300f),
                new Vector2(128f, -96f),
                new Vector2(60f, -96f),
                //new Vector2(-384f, -96f),
                new Vector2(-320f, -96f),
                new Vector2(-320f, -96f),

                new Vector2(136f, -280f),
                new Vector2(-260f, -280f),
                new Vector2(264f, -72f),
                new Vector2(180f, -24f),
                //new Vector2(-364f, -24f),
                new Vector2(-300f, -24f),
                new Vector2(-328f, -72f)
            };

            //firePoints = new();

            //aoeTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "AoeFire.png"));

            aoeZones = new();

            burnZones = new();

            burnDamage = Mod.instance.DamageLevel() / 5;

        }

        protected override void initNetFields()
        {
            base.initNetFields();
            NetFields.AddFields(new INetSerializable[7]
            {
                 netDirection,
                 netAlternative,
                 netFlightFrame,
                 netFlightHeight,
                 netDashActive,
                 netFireFrame,
                 netFireActive
            });
        }

        public override void draw(SpriteBatch b, float alpha = 1f)
        {
            if (IsInvisible || !Utility.isOnScreen(Position, 128) || avatar && Game1.displayFarmer)
            {
                return;
            }

            if (netDashActive)
            {

                return;

            }

            Vector2 localPosition = getLocalPosition(Game1.viewport);

            float drawLayer = Game1.player.getDrawLayer();

            if (IsEmoting && !Game1.eventUp)
            {
                localPosition.Y -= 32 + Sprite.SpriteHeight * 4;
                b.Draw(Game1.emoteSpriteSheet, localPosition, new Rectangle?(new Rectangle(CurrentEmoteIndex * 16 % Game1.emoteSpriteSheet.Width, CurrentEmoteIndex * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, 0, drawLayer);
            }

            //DrawZones(b, drawLayer);

            /*if (netDashActive)
            {
                
                b.Draw(flightTexture, new Vector2(localPosition.X - 96f, localPosition.Y - 160f - netFlightHeight), flightFrames[netDirection][netFlightFrame], Color.White, rotation, new Vector2(0.0f, 0.0f), 3f, flip || netDirection == 3 ? (SpriteEffects)1 : 0, drawLayer);
                
                b.Draw(shadowTexture, new Vector2(localPosition.X - 48f, localPosition.Y - 56f), shadowFrames[netDirection + 4], Color.White*0.35f, 0.0f, new Vector2(0.0f, 0.0f), 4f, flip || netDirection == 3 ? (SpriteEffects)1 : 0, drawLayer - 1E-05f);
                
                if (netFireActive) { DrawFire(b, new(localPosition.X, localPosition.Y - netFlightHeight), netDirection, drawLayer, 6); }

                return;
                
            }*/

            if (netFireActive)
            {
                
                b.Draw(breathTexture, new Vector2(localPosition.X - 64f, localPosition.Y - 160f), Sprite.SourceRect, Color.White, 0.0f, new Vector2(0.0f, 0.0f), 3f, flip ? (SpriteEffects)1 : 0, drawLayer);
                
                DrawFire(b, localPosition,netDirection, drawLayer);
            
            }
            else
            {
                b.Draw(Sprite.Texture, new Vector2(localPosition.X - 64f, localPosition.Y - 160f),Sprite.SourceRect, Color.White, 0.0f, new Vector2(0.0f, 0.0f), 3f, flip ? (SpriteEffects)1 : 0, drawLayer);

            }

            b.Draw(shadowTexture, new Vector2(localPosition.X - 64f, localPosition.Y - 40f), shadowFrames[netDirection], Color.White * 0.35f, 0.0f, new Vector2(0.0f, 0.0f), 3f, flip || netDirection == 3 ? (SpriteEffects)1 : 0, drawLayer - 1E-05f);

        }

        public void DrawFire(SpriteBatch b, Vector2 position, int direction, float depth, int adjust = 0)
        {
            float num1 = 4f;
            depth += 1f / 1000f;
            int num2;
            switch (direction)
            {
                case 0:
                    num2 = 0;
                    if (netAlternative == 3)
                        num2 = 1;
                    num1 = 5f;
                    depth -= 1f / 500f;
                    break;
                case 1:
                    num2 = 2;
                    break;
                case 2:
                    num2 = 3;
                    if (netAlternative == 3)
                        num2 = 4;
                    num1 = 5f;
                    break;
                default:
                    num2 = 5;
                    break;
            }
            int index = num2 + adjust;

            b.Draw(fireTexture, new(position.X+fireVectors[index].X, position.Y + fireVectors[index].Y),fireFrames[direction][netFireFrame], Color.White, rotation, new Vector2(0.0f, 0.0f), num1, flip || direction == 3 ? (SpriteEffects)1 : 0, depth);

        }

        /*public void DrawZones(SpriteBatch b, float depth)
        {

            foreach (KeyValuePair<Vector2, Event.World.AoeZone> aoeZone in aoeZones)
            {

                float token = aoeZone.Key.X + aoeZone.Key.Y;

                //float height = aoeZone.Key.X % 3;

                float frame = (token + aoeZone.Value.aoeFrame) % 4;

                float circle = (token + aoeZone.Value.aoeFrame) % 16;

                b.Draw(aoeTexture, new Vector2((aoeZone.Key.X * 64) - Game1.viewport.X, (aoeZone.Key.Y * 64) - Game1.viewport.Y), new Rectangle(32*(int)frame, 0, 64, 32), Color.White, rotation, new Vector2(0.0f, 0.0f), 2f, SpriteEffects.None, depth - 0.001f);

            }

        }*/

        public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
        {
            if (!Context.IsMainPlayer)
            {
                
                base.drawAboveAlwaysFrontLayer(b);

                return;
           
            }

            if (netDashActive)
            {
                Vector2 localPosition = getLocalPosition(Game1.viewport);

                float drawLayer = Game1.player.getDrawLayer();

                b.Draw(flightTexture, new Vector2(localPosition.X - 96f, localPosition.Y - 160f - netFlightHeight), flightFrames[netDirection][netFlightFrame], Color.White, rotation, new Vector2(0.0f, 0.0f), 3f, flip || netDirection == 3 ? (SpriteEffects)1 : 0, drawLayer);

                b.Draw(shadowTexture, new Vector2(localPosition.X - 48f, localPosition.Y - 56f), shadowFrames[netDirection + 4], Color.White * 0.35f, 0.0f, new Vector2(0.0f, 0.0f), 4f, flip || netDirection == 3 ? (SpriteEffects)1 : 0, drawLayer - 1E-05f);

                if (netFireActive) { DrawFire(b, new(localPosition.X, localPosition.Y - netFlightHeight), netDirection, drawLayer, 6); }

                //return;

            }

            if (textAboveHeadTimer <= 0 || textAboveHead == null)
            {
                return;
            }
                    
            Vector2 local = Game1.GlobalToLocal(new Vector2(getStandingX(), getStandingY() - Sprite.SpriteHeight * 4 - 64 + yJumpOffset));
                
            SpriteText.drawStringWithScrollCenteredAt(b, textAboveHead, (int)local.X, (int)local.Y, "", textAboveHeadAlpha, textAboveHeadColor, 1, (float)(getTileY() * 64 / 10000.0 + 1.0 / 1000.0 + getTileX() / 10000.0), false);

        }

        public override bool checkAction(Farmer who, GameLocation l) => false;

        public override void collisionWithFarmerBehavior()
        {
        }

        public override void behaviorOnFarmerPushing()
        {
        }

        public override Rectangle GetBoundingBox() => new Rectangle(-1, -1, 0, 0);

        public override void update(GameTime time, GameLocation location)
        {
            if (!Context.IsMainPlayer)
            {
                
                if (Sprite.loadedTexture == null || Sprite.loadedTexture.Length == 0)
                {
                    Sprite.spriteTexture = CharacterData.CharacterTexture(Name);
                    Sprite.loadedTexture = Sprite.textureName.Value;
                    LoadOut();
                }

                if (netDirection.Value != moveDirection)
                {
                    AnimateMovement(time);

                }

                Sprite.animateOnce(time);

            }
            else
            {
                if (Mod.instance.CasterBusy())
                    return;
                if (shakeTimer > 0)
                    shakeTimer = 0;
                if (textAboveHeadTimer > 0)
                {
                    if (textAboveHeadPreTimer > 0)
                    {
                        textAboveHeadPreTimer -= time.ElapsedGameTime.Milliseconds;
                    }
                    else
                    {
                        textAboveHeadTimer -= time.ElapsedGameTime.Milliseconds;
                        if (textAboveHeadTimer > 500)
                            textAboveHeadAlpha = Math.Min(1f, textAboveHeadAlpha + 0.1f);
                        else
                            textAboveHeadAlpha = Math.Max(0.0f, textAboveHeadAlpha - 0.04f);
                    }
                }
                updateEmote(time);
                if (currentLocation != Game1.player.currentLocation)
                {
                    currentLocation.characters.Remove(this);
                    Game1.player.currentLocation.characters.Add(this);
                    currentLocation = Game1.player.currentLocation;
                }
                DefenseBuff();
                moveDirection = Game1.player.FacingDirection;
                if (flightActive)
                    UpdateFlight();
                if (!flightActive)
                    UpdateFollow(time);
                if (breathActive)
                    UpdateBreath();
                if(aoeZones.Count > 0)
                {
                    UpdateZones();
                }

                Sprite.animateOnce(time);
            }
        }

        public void UpdateFollow(GameTime time)
        {
            if (Position != Game1.player.Position)
            {
                if (moveDirection % 2 == 1)
                    netAlternative.Set(Game1.player.Position.X > (double)Position.X ? 1 : 3);
                Position = Game1.player.Position;
                AnimateMovement(time);
            }
            else
            {
                if (netDirection.Value == moveDirection)
                    return;
                netAlternative.Set(netDirection.Value);
                AnimateMovement(time);
            }
        }

        public override void AnimateMovement(GameTime time)
        {
            flip = false;
            if (!Context.IsMainPlayer)
                moveDirection = netDirection;
            else
                netDirection.Set(moveDirection);
            FacingDirection = moveDirection;
            switch (moveDirection)
            {
                case 0:
                    Sprite.AnimateUp(time, 0, "");
                    if (netAlternative != 3)
                        break;
                    flip = true;
                    break;
                case 1:
                    Sprite.AnimateRight(time, 0, "");
                    break;
                case 2:
                    Sprite.AnimateDown(time, 0, "");
                    if (netAlternative != 3)
                        break;
                    flip = true;
                    break;
                case 3:
                    Sprite.AnimateLeft(time, 0, "");
                    break;
            }
        }

        public override void PlayerBusy()
        {
            if (breathActive && breathDelay > 0)
                breathActive = false;
            if (!flightActive || flightDelay <= 0)
                return;
            flightActive = false;
        }

        public override void LeftClickAction(SButton Button)
        {
            leftButton = Button;
            if (flightActive)
                return;
            PerformFlight();
        }

        public override void RightClickAction(SButton Button)
        {
            rightButton = Button;
            if (breathActive)
                return;
            PerformBreath();
        }

        public void DefenseBuff()
        {
            if (Game1.buffsDisplay.hasBuff(184655))
                return;
            Buff buff = new Buff("Dragon Scales", 3000, "Rite of the Ether", 10);
            buff.buffAttributes[10] = 5;
            buff.which = 184655;
            Game1.buffsDisplay.addOtherBuff(buff);
        }

        public void PerformFlight()
        {
            int num = FlightDestination();
            if (num == 0)
            {
                return;
            }
            flightActive = true;
            flightDelay = 3;
            netFlightFrame.Set(0);
            flightTimer = flightIncrement * num;
            flightInterval = new((flightTo.X - Position.X) / flightTimer, (flightTo.Y - Position.Y) / flightTimer); //Vector2.op_Division(Vector2.op_Subtraction(this.flightTo, ((StardewValley.Character) Game1.player).Position), (float) this.flightTimer);
            flightPosition = Game1.player.Position;
            strikeTimer = 12;
            flightExtend = 0;
        }

        public bool UpdateFlight()
        {
            if (flightDelay > 0)
            {
                flightDelay--;

                return true;

            }

            netDashActive.Set(true);

            flightTimer--;

            strikeTimer--;

            if (flightTimer == 0)
            {
                flightActive = false;

                netDashActive.Set(false);

                if (Mod.instance.TaskList().ContainsKey("masterFlight"))
                {
                    Rectangle hitBox = new((int)Position.X - 120, (int)Position.Y - 64, 240, 160);
                    PerformStrike(hitBox, Mod.instance.DamageLevel() * 2);
                }
                return false;
            }

            Game1.player.temporarilyInvincible = true;

            Game1.player.temporaryInvincibilityTimer = 0;

            Game1.player.currentTemporaryInvincibilityDuration = 200;

            if (netFlightHeight < 128 && flightTimer > 16)
            {
                
                netFlightHeight.Set(netFlightHeight.Value + 8);

            }
            else if (netFlightHeight > 0 && flightTimer <= 16)
            {

                netFlightHeight.Set(netFlightHeight.Value - 8);

            }

            flightPosition += flightInterval;

            Game1.player.Position = flightPosition;

            Position = flightPosition;

            if (flightTimer % flightIncrement == 0)
            {
                if (flightTimer == 8)
                    netFlightFrame.Set(0);
                else if (netFlightFrame.Value == 4)
                    netFlightFrame.Set(1);
                else
                    netFlightFrame.Set(netFlightFrame.Value + 1);
                if (Mod.instance.Helper.Input.IsDown(leftButton))
                {
                    int num = FlightDestination();
                    if (num != 0)
                    {
                        flightTimer = flightIncrement * num;
                        flightInterval = new((flightTo.X - Position.X) / flightTimer, (flightTo.Y - Position.Y) / flightTimer);
                        ++flightExtend;
                        if (flightExtend > 16 && !Mod.instance.TaskList().ContainsKey("masterFlight"))
                            Mod.instance.UpdateTask("lessonFlight", 1);
                    }
                }
            }
            if (strikeTimer == 0)
            {
                if (Mod.instance.TaskList().ContainsKey("masterFlight"))
                {
                    Rectangle hitBox= new((int)Position.X - 120, (int)Position.Y - 64, 240, 160);
                    PerformStrike(hitBox, Mod.instance.DamageLevel() * 2);
                }
                strikeTimer = 36;
            }
            return true;
        }

        public int FlightDestination()
        {
            //Dictionary<int, Vector2> dictionary = new Dictionary<int, Vector2>()
            Dictionary<int, Vector2> flightVectors = new Dictionary<int, Vector2>()
            {
                [0] = new Vector2(1f, -2f),
                [1] = new Vector2(-1f, -2f),
                [2] = new Vector2(2f, 0.0f),
                [3] = new Vector2(1f, 2f),
                [4] = new Vector2(-1f, 2f),
                [5] = new Vector2(-2f, 0.0f)
            };

            int key = 0;

            int increment = 8;

            int alternate = netAlternative;

            if (netDirection != moveDirection)
            {
                alternate = netDirection;

            }

            switch (moveDirection)
            {
                case 0:
                    if (alternate == 3)
                    {
                        key = 1;
                    } 
                    increment = 12;
                    break;
                case 1:
                    key = 2;
                    break;
                case 2:
                    key = 3;
                    if (alternate == 3)
                    {
                        key = 4;
                    }
                    increment = 12;
                    break;
                case 3:
                    key = 5;
                    break;
            }

            //Vector2 vector2 = dictionary[key];

            Vector2 flightOffset = flightVectors[key];

            int flightRange = 16;

            for (int index = flightRange; index > 0; index--)
            {

                //int num3 = index <= 12 ? 17 - index : index - 12;

                int checkRange = 17 - index;

                if (index > 12)
                {

                    checkRange = index - 12;

                }

                //Vector2 tileLocation = getTileLocation();

                //Vector2 multiple = new(vector2.X * num3, vector2.Y * num3);

                //Vector2 neighbour = new(multiple.X + tileLocation.X, multiple.Y + tileLocation.Y);//Vector2.op_Addition(Game1.player.getTileLocation(), Vector2.op_Multiply(vector2, (float)num3));

                Vector2 neighbour = Game1.player.getTileLocation() + (flightOffset * checkRange);

                string groundCheck = ModUtility.GroundCheck(currentLocation, neighbour);

                if (groundCheck == "ground" || groundCheck == "water")
                {

                    Rectangle boundingBox = Game1.player.GetBoundingBox();

                    int xOffset = boundingBox.X - (int)Game1.player.Position.X;

                    int yoffset = boundingBox.Y - (int)Game1.player.Position.Y;

                    boundingBox.X = (int)(neighbour.X * 64.0) + xOffset;

                    boundingBox.Y = (int)(neighbour.Y * 64.0) + yoffset;

                    if (!currentLocation.isCollidingPosition(boundingBox, Game1.viewport, false, 0, false, Game1.player, false, false, false))
                    {

                        flightTo = new(neighbour.X * 64, neighbour.Y * 64);//Vector2.op_Multiply(neighbour, 64f);

                        flightIncrement = increment;

                        flightTerrain = groundCheck;

                        netAlternative.Set(alternate);

                        AnimateMovement(Game1.currentGameTime);

                        return checkRange;

                    }

                }

            }

            return 0;

        }

        public void PerformBreath()
        {
            breathActive = true;

            breathDelay = 3;

            breathTimer = 48;

            netFireFrame.Set(-1);

            fireTimer = 12;

            if (!Mod.instance.TaskList().ContainsKey("masterBlast"))
            {

                Mod.instance.UpdateTask("lessonBlast", 1);

            }

        }

        public bool UpdateBreath()
        {
            if (breathDelay > 0)
            {
                
                --breathDelay;
                
                return true;
            
            }

            if(breathTimer == 48)
            {

                currentLocation.playSound("furnace");

            }

            --breathTimer;
            
            --fireTimer;

            if (breathTimer == 0 || Game1.player.IsBusyDoingSomething())
            {
               
                breathActive = false;
                
                netFireActive.Set(false);
                
                return false;
            
            }

            if (breathTimer % 12 == 0)
            {
                
                netFireActive.Set(true);

                netFireFrame.Set(netFireFrame.Value + 1);

                if (netFireFrame== 4)
                {

                    netFireFrame.Set(2);

                }

                if (Mod.instance.Helper.Input.IsDown(rightButton) && breathTimer == 12)
                {

                    breathTimer = 18;

                }

            }

            if (fireTimer == 0)
            {
                Vector2 zero = Vector2.Zero;

                switch (netDirection)
                {
                    case 0:

                        zero = new((float)((int)(Position.X / 64.0) + 2), (float)((int)(Position.Y / 64.0) - 4));

                        if (netAlternative == 3 || flip)
                        {
                            zero.X -= 4f;

                            break;

                        }

                        break;

                    case 1:

                        zero = new((float)((int)(Position.X / 64.0) + 5), (float)(int)(Position.Y / 64.0));

                        break;

                    case 2:

                        zero = new((float)((int)(Position.X / 64.0) + 3), (float)((int)(Position.Y / 64.0) + 1));

                        if (netAlternative == 3 || flip)
                        {
                            zero.X -= 4f;
                            break;

                        }

                        break;

                    default:

                        zero = new((float)((int)(Position.X / 64.0) - 4), (float)(int)(Position.Y / 64.0));

                        break;

                }

                ModUtility.Explode(currentLocation, zero, Game1.player, 2, 0, powerLevel:3,dirt:2);

                aoeZones[zero*64] = 10;

                List<Vector2> impactVectors;

                for (int i = 0; i < 3; i++)
                {

                    impactVectors = ModUtility.GetTilesWithinRadius(currentLocation, zero, i);

                    float scale = 2 - (i*0.5f);

                    foreach (Vector2 vector in impactVectors)
                    {

                        Vector2 position = new((vector.X * 64) + (i * 8), (vector.Y * 64) + (i * 8));

                        if (burnZones.ContainsKey(vector))
                        {

                            if (currentLocation.temporarySprites.Contains(burnZones[vector]))
                            {
                                
                                burnZones[vector].scale = scale;

                                burnZones[vector].Position = position;

                                burnZones[vector].reset();

                                continue;

                            }

                        }

                        burnZones[vector] = new(0, 100, 5, 6, position, false, false)
                        {

                            sourceRect = new(0, 0, 32, 32),

                            sourceRectStartingPos = new Vector2(0, 0),

                            texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images","Fire.png")),

                            scale = scale,

                            layerDepth = vector.Y / 10000,

                            alphaFade = 0.001f,

                        };

                        currentLocation.temporarySprites.Add(burnZones[vector]);

                    }

                }

                fireTimer = 12;
            
            }
            
            return true;

        }

        public void UpdateZones()
        {

            aoeTimer--;

            if(aoeTimer > 0)
            {

                return;
                
            }

            aoeTimer = 18;

            List<StardewValley.Monsters.Monster> monsters = new();

            for (int i = aoeZones.Count - 1; i >= 0; i--)
            {

                KeyValuePair<Vector2, int> aoeZone = aoeZones.ElementAt(i);

                aoeZones[aoeZone.Key] -= 1;

                if (aoeZones[aoeZone.Key] <= 0)
                {

                    aoeZones.Remove(aoeZone.Key);

                }

            }

            foreach (NPC nonPlayableCharacter in currentLocation.characters)
            {

                if (nonPlayableCharacter is StardewValley.Monsters.Monster monsterCharacter)
                {

                    if (monsterCharacter.IsInvisible || monsterCharacter.isInvincible() || monsterCharacter.Health <= 0)
                    {

                        continue;

                    }

                    float threshold = 160f;

                    if (monsterCharacter.Sprite.SpriteWidth > 16)
                    {
                        threshold += 64f;
                    }

                    if (monsterCharacter.Sprite.SpriteWidth > 32)
                    {
                        threshold += 64f;
                    }

                    for (int i = aoeZones.Count - 1; i >= 0; i--)
                    {

                        float monsterDifference = Vector2.Distance(monsterCharacter.Position, aoeZones.ElementAt(i).Key);

                        if (monsterDifference < threshold)
                        {

                            monsters.Add(monsterCharacter);

                            break;

                        }

                    }

                }

            }

            foreach(StardewValley.Monsters.Monster monster in monsters)
            {

                ModUtility.HitMonster(currentLocation, Game1.player, monster, burnDamage, false);

            }

        }

        public void PerformStrike(Rectangle hitBox, int damage)
        {
            
            for (int index = currentLocation.characters.Count - 1; index >= 0; --index)
            {
                
                if (currentLocation.characters.ElementAtOrDefault<NPC>(index) != null)
                {
                    
                    NPC character = currentLocation.characters[index];
                    
                    Rectangle boundingBox = character.GetBoundingBox();
                    
                    if (character is StardewValley.Monsters.Monster monster)
                    {

                        if(monster.Health > 0 && !monster.IsInvisible && boundingBox.Intersects(hitBox))
                        {

                            List<int> diff = ModUtility.CalculatePush(currentLocation, monster, Position, 128);

                            ModUtility.HitMonster(currentLocation, Game1.player, monster, damage, false, diffX: diff[0], diffY: diff[1]);

                        }

                    }
                        
                }
            
            }
        
        }

        public override void ShutDown()
        {

            if (flightActive && currentLocation.Name == Game1.player.currentLocation.Name)
            {
                
                Game1.player.Position = flightTo;

            }
            
        }
    }
}
