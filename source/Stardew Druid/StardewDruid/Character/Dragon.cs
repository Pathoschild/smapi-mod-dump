/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewDruid.Cast;
using StardewDruid.Event;
using StardewDruid.Event.World;
using StardewDruid.Map;
using StardewDruid.Monster;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using static StardewValley.Minigames.TargetGame;

namespace StardewDruid.Character
{
    public class Dragon : NPC
    {

        public NetLong netAnchor = new NetLong(0);
        public Farmer anchor;
        public bool avatar;

        public int moveDirection;
        public int altDirection;
        public List<Event.BarrageHandle> barrages;
        public bool loadedOut;

        public Texture2D characterTexture;
        public Texture2D characterSpecial;
        public float walkTimer;
        public int walkFrame;
        public int stationaryTimer;
        public Dictionary<int, List<Rectangle>> walkFrames;
        public NetInt netWalkFrame = new NetInt(0);
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
        public Texture2D flightSpecial;
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

        public List<Rectangle> sweepFrames;
        public bool sweepActive;
        public NetBool netSweepActive = new NetBool(false);
        public NetInt netSweepFrame = new NetInt(0);
        public int sweepDelay;
        public int sweepTimer;
        public bool cooldownActive;
        public int cooldownTimer;

        public NetBool netSpecialActive = new NetBool(false);
        public bool specialActive;
        public int specialDelay;
        public int specialTimer;
        public int fireTimer;
        public int roarTimer;
        public bool roarActive;
        public int burnDamage;
        public string fireColor;

        public NetBool netDigActive = new NetBool(false);
        public NetInt netDigMoment = new NetInt(0);
        public bool digActive;
        public int digTimer;
        public Vector2 digPosition;
        public int digMoment;
        public List<Rectangle> digFrames;
        public Texture2D digTexture;
        public Texture2D dirtTexture;

        public NetBool netDiveActive = new NetBool(false);
        public NetInt netDiveMoment = new NetInt(0);
        public NetBool netSwimActive = new NetBool(false);
        public List<float> diveRotates;
        public bool terrainActive;
        public bool swimActive;
        public int swimCheckTimer;
        public List<Rectangle> floatFrames;
        public Texture2D floatTexture;
        public Texture2D swimTexture;
        public List<Rectangle> swimFrames;
        public bool diveActive;
        public int diveTimer;
        public Vector2 divePosition;
        public int diveMoment;
        public List<Rectangle> diveFrames;
        public List<Rectangle> bobberFrames;
        public Texture2D splashTexture;

        public Dragon()
        {
        }

        public Dragon(Farmer Farmer, Vector2 position, string map, string Name)
          : base(CharacterData.CharacterSprite(Name), position, map, 2, Name, new Dictionary<int, int[]>(), CharacterData.CharacterPortrait(Name), false, null)
        {
            
            willDestroyObjectsUnderfoot = false;
            
            DefaultMap = map;
            
            DefaultPosition = position;
            
            HideShadow = true;
            
            breather.Value = false;

            avatar = true;
            
            anchor = Farmer;
           
            netAnchor.Set(Farmer.UniqueMultiplayerID);
            
            moveDirection = Game1.player.FacingDirection;
           
            AnimateMovement(false);
            
            LoadOut();
        
        }

        public void LoadOut()
        {

            anchor = Game1.getFarmer(netAnchor.Value);

            barrages = new();

            loadedOut = true;

            characterTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", Name + ".png"));

            characterSpecial = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", Name + "Special.png"));

            walkFrames = new Dictionary<int, List<Rectangle>>()
            {
                [0] = new List<Rectangle>()
                {
                new Rectangle(0, 128, 64, 64),
                new Rectangle(64, 128, 64, 64),
                new Rectangle(128, 128, 64, 64),
                new Rectangle(192, 128, 64, 64),
                new Rectangle(256, 128, 64, 64),
                new Rectangle(320, 128, 64, 64),
                new Rectangle(384, 128, 64, 64),
                },
                [1] = new List<Rectangle>()
                {
                new Rectangle(0, 64, 64, 64),
                new Rectangle(64, 64, 64, 64),
                new Rectangle(128, 64, 64, 64),
                new Rectangle(192, 64, 64, 64),
                new Rectangle(256, 64, 64, 64),
                new Rectangle(320, 64, 64, 64),
                new Rectangle(384, 64, 64, 64),
                },
                [2] = new List<Rectangle>()
                {
                new Rectangle(0, 0, 64, 64),
                new Rectangle(64, 0, 64, 64),
                new Rectangle(128, 0, 64, 64),
                new Rectangle(192, 0, 64, 64),
                new Rectangle(256, 0, 64, 64),
                new Rectangle(320, 0, 64, 64),
                new Rectangle(384, 0, 64, 64),
                },
                [3] = new List<Rectangle>()
                {
                new Rectangle(0, 64, 64, 64),
                new Rectangle(64, 64, 64, 64),
                new Rectangle(128, 64, 64, 64),
                new Rectangle(192, 64, 64, 64),
                new Rectangle(256, 64, 64, 64),
                new Rectangle(320, 64, 64, 64),
                new Rectangle(384, 64, 64, 64),
                }
            };
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
            
            flightTo = Vector2.Zero;

            flightInterval = Vector2.Zero;

            flightTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", Name + "Flight.png"));

            flightSpecial = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", Name + "FlightSpecial.png"));

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

            fireColor = Name.Replace("Dragon", "");

            barrages = new();

            burnDamage = Mod.instance.DamageLevel() / 5;

            digFrames = new List<Rectangle>()
            {
                new Rectangle(0, 0, 128, 64),
                new Rectangle(0, 64, 128, 64),
            };

            digTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", Name+"Dig.png"));

            swimTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "DragonSwim.png"));

            floatTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", Name+"Float.png"));

            floatFrames = new()
            {

                new Rectangle(0, 128, 64, 64),

                new Rectangle(0, 64, 64, 64),

                new Rectangle(0, 0, 64, 64),

                new Rectangle(0, 64, 64, 64),

            };

            sweepFrames = new()
            {
                new Rectangle(0, 64, 128, 64),
                new Rectangle(0, 0, 128, 64),
                new Rectangle(0, 128, 128, 64),
                new Rectangle(0, 128, 128, 64),
                new Rectangle(0, 0, 128, 64),
                new Rectangle(0, 64, 128, 64),

            };

            swimFrames = new()
            {
                new Rectangle(0, 128, 64, 64),
                new Rectangle(64, 128, 64, 64),
                new Rectangle(0, 64, 64, 64),
                new Rectangle(64, 64, 64, 64),
                new Rectangle(0, 0, 64, 64),
                new Rectangle(64, 0, 64, 64),
                new Rectangle(0, 64, 64, 64),
                new Rectangle(64, 64, 64, 64),
            };

            diveFrames = new List<Rectangle>()
            {
                new Rectangle(0, 192, 64, 64),
                new Rectangle(0, 192, 64, 64),
                new Rectangle(0, 192, 64, 64),
                new Rectangle(0, 256, 64, 64),
                new Rectangle(0, 256, 64, 64),
                new Rectangle(0, 256, 64, 64),
            };

            bobberFrames = new List<Rectangle>()
            {
                new Rectangle(0, 192, 64, 64),
                new Rectangle(64, 192, 64, 64),
                new Rectangle(0, 192, 64, 64),
                new Rectangle(0, 256, 64, 64),
                new Rectangle(64, 256, 64, 64),
                new Rectangle(0, 256, 64, 64),
            };

            splashTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Splash.png"));

            if (Mod.instance.CurrentProgress() >= 31)
            {

                terrainActive = true;

            }

            if (ModUtility.GroundCheck(currentLocation, new Vector2((int)(Position.X / 64), (int)(Position.Y / 64))) == "water")
            {

                swimActive = true;

                netSwimActive.Set(true);

            }

        }

        protected override void initNetFields()
        {
            base.initNetFields();
            NetFields.AddFields(new INetSerializable[15]
            {
                 netAnchor,
                 netDirection,
                 netAlternative,
                 netWalkFrame,
                 netFlightFrame,
                 netFlightHeight,
                 netSweepActive,
                 netSweepFrame,
                 netDashActive,
                 netSpecialActive,
                 netDigActive,
                 netDigMoment,
                 netDiveActive,
                 netDiveMoment,
                 netSwimActive,
            });
        }

        public override void draw(SpriteBatch b, float alpha = 1f)
        {
            if (IsInvisible || !Utility.isOnScreen(Position, 128) || (avatar && Game1.displayFarmer) || anchor == null)
            {
                return;
            }

            if (netDashActive.Value)
            {

                return;

            }

            Vector2 localPosition = getLocalPosition(Game1.viewport);

            float drawLayer = anchor.getDrawLayer();

            if (IsEmoting && !Game1.eventUp)
            {
                localPosition.Y -= 32 + Sprite.SpriteHeight * 4;
                b.Draw(Game1.emoteSpriteSheet, localPosition, new Rectangle?(new Rectangle(CurrentEmoteIndex * 16 % Game1.emoteSpriteSheet.Width, CurrentEmoteIndex * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, 0, drawLayer);
            }

            Rectangle spriteRect = Sprite.SourceRect;

            if(Sprite.currentFrame >= 18)
            {

                spriteRect.Y -= 128;

            }

            if (netDiveActive.Value)
            {

                b.Draw(floatTexture, new Vector2(localPosition.X - 64f, localPosition.Y - 160f - (2f * (netDiveMoment.Value % 3))), diveFrames[netDiveMoment.Value], Color.White, 0f, new Vector2(0.0f, 0.0f), 3f, flip || netDirection.Value == 3 ? (SpriteEffects)1 : 0, drawLayer);

                b.Draw(swimTexture, new Vector2(localPosition.X - 64f, localPosition.Y - 160f - (2f * (netDiveMoment.Value % 3))), bobberFrames[netDiveMoment.Value], Color.White * 0.35f, 0.0f, new Vector2(0.0f, 0.0f), 3f, flip || netDirection.Value == 3 ? (SpriteEffects)1 : 0, drawLayer);

                return;
            }

            if(netSwimActive.Value)
            {
                
                int swimFrame = (netDirection.Value * 2) + (netWalkFrame.Value % 2);

                b.Draw(floatTexture, new Vector2(localPosition.X - 64f, localPosition.Y - 160f), floatFrames[netDirection.Value], Color.White, 0.0f, new Vector2(0.0f, 0.0f), 3f, flip || netDirection.Value == 3 ? (SpriteEffects)1 : 0, drawLayer);

                b.Draw(swimTexture, new Vector2(localPosition.X - 64f, localPosition.Y - 160f), swimFrames[swimFrame], Color.White * 0.35f, 0.0f, new Vector2(0.0f, 0.0f), 3f, flip || netDirection.Value == 3 ? (SpriteEffects)1 : 0, drawLayer);

                return;

            }

            if (netDigActive.Value)
            {

                int digFrame = netDigMoment.Value % 2;

                b.Draw(digTexture, new Vector2(localPosition.X - 224f, localPosition.Y - 160f), digFrames[digFrame], Color.White, 0f, new Vector2(0.0f, 0.0f), 3f, SpriteEffects.None, drawLayer);

                b.Draw(shadowTexture, new Vector2(localPosition.X - 192f, localPosition.Y - 40f), shadowFrames[1], Color.White * 0.25f, 0.0f, new Vector2(0.0f, 0.0f), 3f, SpriteEffects.None, drawLayer - 1E-05f);

                return;

            }

            if (netSweepActive.Value)
            {

                int sweepAdjust = 0;
                int shadowAdjust = 1;

                switch (netDirection.Value)
                {
                    case 0:
                        if(netAlternative.Value == 3)
                        {
                            sweepAdjust = 3;
                        }
                        shadowAdjust = 2;

                        break;
                    case 2:
                        if (netAlternative.Value == 3)
                        {
                            sweepAdjust = 1;
                        }
                        shadowAdjust = 0;
                        break;
                    case 3:
                        sweepAdjust = 2;
                        break;
                }

                int sweepFrame = (netDirection.Value + netSweepFrame.Value + sweepAdjust + 1) % 6;

                switch (sweepFrame)
                {
                    case 0:
                    case 5:
                        shadowAdjust = 2;
                        break;
                    case 2:
                    case 3:
                        shadowAdjust = 0;
                        break;

                }

                bool sweepFlip = sweepFrame > 2;

                if (netSpecialActive.Value)
                {

                    b.Draw(flightSpecial, new Vector2(localPosition.X - 96f, localPosition.Y - 160f - netFlightHeight.Value), sweepFrames[sweepFrame], Color.White, 0f, new Vector2(0.0f, 0.0f), 3f, sweepFlip ? (SpriteEffects)1 : 0, drawLayer);

                }
                else
                {
                    b.Draw(flightTexture, new Vector2(localPosition.X - 96f, localPosition.Y - 160f - netFlightHeight.Value), sweepFrames[sweepFrame], Color.White, 0f, new Vector2(0.0f, 0.0f), 3f, sweepFlip ? (SpriteEffects)1 : 0, drawLayer);

                }

                b.Draw(shadowTexture, new Vector2(localPosition.X - 64f, localPosition.Y - 40f), shadowFrames[shadowAdjust], Color.White * 0.25f, 0.0f, new Vector2(0.0f, 0.0f), 3f, sweepFlip ? (SpriteEffects)1 : 0, drawLayer - 1E-05f);

                return;

            }

            if (netSpecialActive.Value)
            {

                b.Draw(characterSpecial, new Vector2(localPosition.X - 64f, localPosition.Y - 160f), walkFrames[netDirection.Value][netWalkFrame.Value], Color.White, 0.0f, new Vector2(0.0f, 0.0f), 3f, flip || netDirection.Value == 3 ? (SpriteEffects)1 : 0, drawLayer);

            }
            else
            {

                b.Draw(characterTexture, new Vector2(localPosition.X - 64f, localPosition.Y - 160f), walkFrames[netDirection.Value][netWalkFrame.Value], Color.White, 0.0f, new Vector2(0.0f, 0.0f), 3f, flip || netDirection.Value == 3 ? (SpriteEffects)1 : 0, drawLayer);

            }

            b.Draw(shadowTexture, new Vector2(localPosition.X - 64f, localPosition.Y - 40f), shadowFrames[netDirection.Value], Color.White * 0.25f, 0.0f, new Vector2(0.0f, 0.0f), 3f, flip || netDirection.Value == 3 ? (SpriteEffects)1 : 0, drawLayer - 1E-05f);

        }

        public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
        {

            if (netDashActive.Value)
            {
                Vector2 localPosition = getLocalPosition(Game1.viewport);

                float drawLayer = anchor.getDrawLayer();

                if (netSpecialActive.Value)
                {

                    b.Draw(flightSpecial, new Vector2(localPosition.X - 96f, localPosition.Y - 160f - netFlightHeight.Value), flightFrames[netDirection.Value][netFlightFrame.Value], Color.White, 0f, new Vector2(0.0f, 0.0f), 3f, flip || netDirection.Value == 3 ? (SpriteEffects)1 : 0, drawLayer);

                }
                else
                {

                    b.Draw(flightTexture, new Vector2(localPosition.X - 96f, localPosition.Y - 160f - netFlightHeight.Value), flightFrames[netDirection.Value][netFlightFrame.Value], Color.White, 0f, new Vector2(0.0f, 0.0f), 3f, flip || netDirection.Value == 3 ? (SpriteEffects)1 : 0, drawLayer);

                }

                b.Draw(shadowTexture, new Vector2(localPosition.X - 48f, localPosition.Y - 56f), shadowFrames[netDirection.Value + 4], Color.White * 0.25f, 0.0f, new Vector2(0.0f, 0.0f), 4f, flip || netDirection.Value == 3 ? (SpriteEffects)1 : 0, drawLayer - 1E-05f);

            }

            if (textAboveHeadTimer <= 0 || textAboveHead == null)
            {
                return;
            }

            Vector2 local = Game1.GlobalToLocal(new Vector2(Position.X, Position.Y - 256f));

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

            if (currentLocation != Game1.player.currentLocation)
            {

                return;

            }

            if (!avatar)
            {

                if (Sprite.loadedTexture == null || Sprite.loadedTexture.Length == 0)
                {
                    
                    Sprite.spriteTexture = CharacterData.CharacterTexture(Name);
                    
                    Sprite.loadedTexture = Sprite.textureName.Value;

                }

                return;

            }

            if (!loadedOut)
            {
                LoadOut();
            }

            if(anchor == null)
            {

                ShutDown();

                return;

            }

            if (Mod.instance.CasterBusy())
            {
                return;
            }

            if (shakeTimer > 0)
            {
                shakeTimer = 0;
            }

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
                    {
                        
                        textAboveHeadAlpha = Math.Min(1f, textAboveHeadAlpha + 0.1f);
                    
                    }
                    else
                    {
                        
                        textAboveHeadAlpha = Math.Max(0.0f, textAboveHeadAlpha - 0.04f);
                    
                    }

                }
            }

            int stepAnimation = Game1.player.FarmerSprite.currentAnimationIndex % 4;

            if (stepAnimation == 0)
            {

                Game1.player.FarmerSprite.currentAnimationIndex++;

            }
            else if (stepAnimation == 3)
            {

                Game1.player.FarmerSprite.currentAnimationIndex -= 2;

            }

            updateEmote(time);

            DefenseBuff();

            moveDirection = Game1.player.FacingDirection;

            if (flightActive)
            {
                UpdateFlight();
            }

            if (sweepActive)
            {
                UpdateSweep();
            }

            if (swimActive)
            {
                UpdateSwim();

            }

            if (diveActive)
            {
                UpdateDive();
            }

            if (digActive)
            {

                UpdateDig();

            }

            if(!flightActive)
            {
                UpdateFollow();
            }

            if (specialActive)
            {
                UpdateSpecial();
            }

            if (barrages.Count > 0)
            {
                UpdateBarrages();
            }

            if (cooldownActive)
            {
                UpdateCooldown();
            }

        }

        public void UpdateFollow()
        {
            if (Position != Game1.player.Position)
            {
                if (moveDirection % 2 == 1)
                {
                    netAlternative.Set(Game1.player.Position.X > (double)Position.X ? 1 : 3);
                }
                    
                Position = Game1.player.Position;

                AnimateMovement();

                return;

            }
            
            if(netDirection.Value != moveDirection)
            {
                
                netAlternative.Set(netDirection.Value);

                AnimateMovement();

                return;

            }

            if(stationaryTimer > 0)
            {

                stationaryTimer--;

                if(stationaryTimer == 0)
                {

                    AnimateMovement(false);

                }

            }

        }

        public void AnimateMovement(bool movement = true)
        {

            if (!avatar)
            {
                moveDirection = netDirection.Value;
            }
            else
            {
                netDirection.Set(moveDirection);
            }

            FacingDirection = moveDirection;

            flip = false;

            if (moveDirection % 2 == 0 && netAlternative.Value == 3)
            {
                flip = true;

            }

            if (movement)
            {

                walkTimer--;

                if (walkTimer <= 0)
                {

                    walkFrame++;

                    if (walkFrame > 6)
                    {
                        walkFrame = 1;
                    }

                    netWalkFrame.Set(walkFrame);

                    walkTimer = 9;

                    stationaryTimer = 5;

                }

                return;

            }

            walkFrame = 0;

            walkTimer = 0;

            stationaryTimer = 0;

            netWalkFrame.Set(0);

        }

        public void PlayerBusy()
        {
            
            if (specialActive && specialDelay > 0)
            {
                specialActive = false;
                netSpecialActive.Set(false);
            }

            if (flightActive && flightDelay > 0)
            {
                flightActive = false;
                netDashActive.Set(false);
            }
            
        }

        public void LeftClickAction(SButton Button)
        {

            leftButton = Button;

            if (diveActive || sweepActive || flightActive || digActive)
            {

                return;

            }

            PerformFlight();

        }

        public void RightClickAction(SButton Button)
        {

            rightButton = Button;

            if (swimActive && !netDashActive.Value)
            {

                if (!diveActive && !Mod.instance.eventRegister.ContainsKey("active"))
                {

                    PerformDive();

                }

                return;

            }

            if (!specialActive)
            {

                PerformSpecial();

            }

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

            if (SweepVictims().Count > 0)
            {

                if (AccountStamina(20))
                {
                    PerformSweep();

                    return;

                }

            }

            int flightRange = FlightDestination();

            if (flightRange == 0)
            {
                return;
            }

            flightActive = true;

            flightDelay = 3;

            netFlightFrame.Set(0);

            flightTimer = flightIncrement * flightRange;

            flightInterval = new((flightTo.X - Position.X) / flightTimer, (flightTo.Y - Position.Y) / flightTimer); //Vector2.op_Division(Vector2.op_Subtraction(this.flightTo, ((StardewValley.Character) Game1.player).Position), (float) this.flightTimer);

            flightPosition = Game1.player.Position;

            strikeTimer = 56;

            flightExtend = flightIncrement * 6;

            Game1.player.temporarilyInvincible = true;

            Game1.player.temporaryInvincibilityTimer = 0;

            Game1.player.currentTemporaryInvincibilityDuration = 100 * flightIncrement;

        }

        public void PerformSweep()
        {

            sweepActive = true;

            sweepDelay = 3;

            netSweepFrame.Set(0);

            sweepTimer = 25;

            Game1.player.temporarilyInvincible = true;

            Game1.player.temporaryInvincibilityTimer = 0;

            Game1.player.currentTemporaryInvincibilityDuration = 500;

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

            if (strikeTimer > 0)
            {

                if (strikeTimer % (flightIncrement * 3) == 0)
                {

                    FlightStrike();

                }

                strikeTimer--;

            }

            if (flightTimer == 0)
            {

                flightActive = false;

                netDashActive.Set(false);

                if (flightTerrain == "water")
                {

                    swimActive = true;

                    netSwimActive.Set(true);

                    swimCheckTimer = 60;

                }

                if (strikeTimer == 0)
                {

                    FlightStrike();

                }

                return false;

            }

            if (netFlightHeight.Value < 128 && flightTimer > 16)
            {

                netFlightHeight.Set(netFlightHeight.Value + 8);

            }
            else if (netFlightHeight.Value > 0 && flightTimer <= 16)
            {

                netFlightHeight.Set(netFlightHeight.Value - 8);

            }

            flightPosition += flightInterval;

            Game1.player.Position = flightPosition;

            Position = flightPosition;

            if (flightTimer % flightIncrement == 0)
            {

                if (netFlightFrame.Value == 4)
                {

                    netFlightFrame.Set(1);
                
                }
                else
                {
                    
                    netFlightFrame.Set(netFlightFrame.Value + 1);
                
                }
                    
                if (Mod.instance.Helper.Input.IsDown(leftButton))
                {
                    
                    int num = FlightDestination();
                    
                    if (num != 0)
                    {
                        
                        flightTimer = flightIncrement * num;
                        
                        flightInterval = new((flightTo.X - Position.X) / flightTimer, (flightTo.Y - Position.Y) / flightTimer);
                        
                        ++flightExtend;

                        if (flightExtend > 16 && !Mod.instance.TaskList().ContainsKey("masterFlight"))
                        {
                            
                            Mod.instance.UpdateTask("lessonFlight", 1);

                        }
                            
                    }
                
                }
                
                if (flightTimer == flightIncrement)
                {
                    
                    netFlightFrame.Set(0);
                    
                    Game1.player.temporarilyInvincible = true;

                    Game1.player.temporaryInvincibilityTimer = 0;

                    Game1.player.currentTemporaryInvincibilityDuration = 500;
                
                }
            
            }

            return true;

        }

        public void FlightStrike()
        {

            if (Mod.instance.TaskList().ContainsKey("masterFlight"))
            {

                Rectangle hitBox = new((int)Position.X - 96, (int)Position.Y - 64, 256, 128);

                ModUtility.DamageMonsters(currentLocation, ModUtility.MonsterIntersect(currentLocation, hitBox, true), Game1.player, Mod.instance.DamageLevel() * 3 / 2, true);

            }

        }

        public bool UpdateSweep()
        {

            if (sweepDelay > 0)
            {
                sweepDelay--;

                return true;

            }

            if(sweepTimer == 25)
            {

                if (Game1.player.CurrentTool is MeleeWeapon)
                {
                    (Game1.player.CurrentTool as MeleeWeapon).isOnSpecial = true;
                }

                netSweepActive.Set(true);

                netFlightHeight.Set(0);

            }

            sweepTimer--;

            if (sweepTimer == 15)
            {

                SweepStrike();

                if (netSpecialActive)
                {

                    PerformFireballs();

                }

            }

            if (sweepTimer > 12)
            {

                netFlightHeight.Set(netFlightHeight.Value + 2);

            }
            else
            {

                netFlightHeight.Set(netFlightHeight.Value - 2);

            }

            if (sweepTimer <= 0)
            {

                if (Game1.player.CurrentTool is MeleeWeapon)
                {
                    (Game1.player.CurrentTool as MeleeWeapon).isOnSpecial = false;
                }

                sweepActive = false;

                //cooldownActive = true;

                //cooldownTimer = 120;

                netSweepActive.Set(false);

                netFlightHeight.Set(0);

                return false;

            }

            if(sweepTimer % 5 == 0)
            {

                netSweepFrame.Set(netSweepFrame.Value + 1);

            }

            return true;

        }

        public void PerformFireballs()
        {

            Vector2 centralVector = new((int)(Position.X / 64), (int)(Position.Y / 64));

            List<Vector2> castSelection = ModUtility.GetTilesWithinRadius(currentLocation, centralVector, 7); // 2,3,4,5,6,7

            Random randomIndex = new();

            if (randomIndex.Next(2) == 0) { castSelection.Reverse(); }

            int castSelect = castSelection.Count; // 12, 16, 24, 28, 32, 28

            if (castSelect == 0)
            {

                return;

            }

            int castIndex;

            Vector2 newVector;

            int fireDamage = Mod.instance.DamageLevel() / 2;

            for (int k = 0; k < 7; k++)
            {

                int castLower = 4 * k;

                if (castLower + 2 >= castSelect)
                {

                    continue;

                }

                int castHigher = Math.Min(castLower + 4, castSelection.Count);

                castIndex = randomIndex.Next(castLower, castHigher);

                newVector = castSelection[castIndex];

                BarrageHandle fireball = new(currentLocation, newVector, centralVector, 2, 1, fireColor, -1, fireDamage);

                fireball.type = BarrageHandle.barrageType.fireball;

                barrages.Add(fireball);

            }

            fireTimer = 72;

        }

        public void UpdateCooldown()
        {

            cooldownTimer--;

            if (cooldownTimer <= 0)
            {
                cooldownActive = false;

            }

        }

        public List<StardewValley.Monsters.Monster> SweepVictims()
        {
            
            Rectangle hitBox = new((int)Position.X - 96, (int)Position.Y - 64, 256, 192);

            return ModUtility.MonsterIntersect(currentLocation, hitBox, true);
        
        }

        public void SweepStrike()
        {

            ModUtility.DamageMonsters(currentLocation, SweepVictims(), Game1.player, Mod.instance.DamageLevel() * 3 / 2, true);
        
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

            int alternate = netAlternative.Value;

            if (netDirection.Value != moveDirection)
            {
                alternate = netDirection.Value;

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

            Vector2 flightOffset = flightVectors[key];

            int flightRange = 16;

            Vector2 tile = new Vector2((int)(Position.X / 64), (int)(Position.Y / 64));

            for (int index = flightRange; index > 0; index--)
            {

                int checkRange = 17 - index;

                if (index > 12)
                {

                    checkRange = index - 12;

                }

                Vector2 neighbour = tile + (flightOffset * checkRange);

                List<string> safeTerrain = new() { "ground", };

                if (terrainActive)
                {
                    safeTerrain.Add("water");
                }

                string groundCheck = ModUtility.GroundCheck(currentLocation, neighbour);

                if (!safeTerrain.Contains(groundCheck))
                {

                    continue;

                }

                if (groundCheck == "water")
                {

                    if (!ModUtility.WaterCheck(currentLocation, neighbour, 2))
                    {

                        continue;

                    }

                }

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

                    AnimateMovement(false);

                    return checkRange;

                }


            }

            return 0;

        }

        public bool TreasureZone(bool activate = false)
        {

            if (!swimActive && currentLocation.objects.Count() > 0)
            {

                Vector2 tile = new Vector2((int)(Position.X / 64), (int)(Position.Y / 64));

                List<Vector2> tileVectors = ModUtility.GetTilesWithinRadius(currentLocation, tile, 1);

                foreach (Vector2 tileVector in tileVectors)
                {

                    if (currentLocation.objects.ContainsKey(tileVector))
                    {

                        StardewValley.Object targetObject = currentLocation.objects[tileVector];

                        if (targetObject.Name.Contains("Artifact Spot"))
                        {

                            if (activate)
                            {

                                currentLocation.digUpArtifactSpot((int)tileVector.X, (int)tileVector.Y, anchor);
                                
                                currentLocation.objects.Remove(tileVector);
                            
                            }

                            return true;

                        }

                    }

                    continue;

                }

            }

            if (Mod.instance.eventRegister.ContainsKey("crate"))
            {

                Crate treasureEvent = Mod.instance.eventRegister["crate"] as Crate;

                if (treasureEvent.chaseEvent)
                {

                    return false;

                }

                if (Vector2.Distance(Position, treasureEvent.treasurePosition) <= 128f)
                {

                    if (!treasureEvent.treasureClaim && activate)
                    {

                        treasureEvent.treasureClaim = true;

                    }

                    return true;

                }

            }

            return false;

        }

        public void PerformSpecial()
        {

            if (!flightActive)
            {
                
                if (TreasureZone())
                {

                    digActive = true;

                    specialDelay = 6;

                    digTimer = -1;

                    return;

                }

                FaceCursorTarget();

            }

            specialActive = true;

            specialDelay = 6;

            specialTimer = -1;

            netSpecialActive.Set(true);

        }
        
        public void UpdateDig()
        {
            if (specialDelay > 0)
            {

                specialDelay--;

                return;

            }

            if(digTimer == -1)
            {

                netDigActive.Set(true);

                digTimer = 108;

                digPosition = (Mod.instance.eventRegister["crate"] as Crate).treasurePosition;

                digMoment = 0;

                netDigMoment.Set(0);

            }

            digTimer--;

            if (digTimer == 0)
            {

                digActive = false;
                
                netDigActive.Set(false);
                
                return;

            }

            Game1.player.Position = digPosition;
            
            Position = digPosition;

            if (digTimer % 12 == 0)
            {

                digMoment++;

                netDigMoment.Set(digMoment);

            }

            if (digTimer == 24)
            {

                TreasureZone(true);

            }

        }

        public void FaceCursorTarget()
        {
            Point mousePoint = Game1.getMousePosition();

            if (mousePoint.Equals(new(0)))
            {
                return;

            }

            Vector2 viewPortPosition = Game1.viewportPositionLerp;

            Vector2 mousePosition = new(mousePoint.X + viewPortPosition.X, mousePoint.Y + viewPortPosition.Y);

            Vector2 diffPosition = mousePosition - Position;

            float rotate = (float)Math.Atan2(diffPosition.Y, diffPosition.X);

            if(rotate < 0.0001f)
            {

                rotate = (float)(Math.PI * 2) + rotate;

            }

            if(rotate < 0.525 || rotate > 5.75)
            {

                moveDirection = 1;
                altDirection = 0;

            }
            else if(rotate < 1.575)
            {

                moveDirection = 2;
                altDirection = 1;

            }
            else if (rotate < 2.625)
            {

                moveDirection = 2;
                altDirection = 3;

            }
            else if (rotate < 3.675)
            {

                moveDirection = 3;
                altDirection = 0;

            }
            else if (rotate < 4.725)
            {

                moveDirection = 0;
                altDirection = 3;

            }
            else if(rotate < 5.775)
            {

                moveDirection = 0;
                altDirection = 1;

            }

            /*float absX = Math.Abs(diffPosition.X); // x position

            float absY = Math.Abs(diffPosition.Y); // y position

            if (absX > absY)
            {

                Game1.player.FacingDirection = diffPosition.X < 0.001f ? 3 : 1;

            }
            else
            {

                Game1.player.FacingDirection = diffPosition.Y < 0.001f ? 0 : 2;

                altDirection = mousePosition.X > (double)Position.X ? 1 : 3;

                netAlternative.Set(altDirection);

            }

            moveDirection = Game1.player.FacingDirection;*/

            Game1.player.FacingDirection = moveDirection;

            netDirection.Set(moveDirection);

            netAlternative.Set(altDirection);

            AnimateMovement(false);

        }

        public bool RoarCheck(Vector2 zero)
        {

            Vector2 target = zero * 64;

            roarActive = false;

            if (currentLocation.characters.Count > 0)
            {

                foreach (NPC witness in currentLocation.characters)
                {

                    if (witness is StardewValley.Monsters.Monster)
                    {
                        continue;
                    }

                    if (witness is StardewDruid.Character.Character)
                    {
                        continue;
                    }

                    if (witness is StardewDruid.Character.Dragon)
                    {
                        continue;
                    }

                    if (witness.IsInvisible)
                    {

                        continue;
                    
                    }

                    if (Vector2.Distance(witness.Position, target) < 320)
                    {

                        if (roarTimer <= 0)
                        {

                            showTextAboveHead("RWWWRRR", duration: 2000);

                            currentLocation.playSoundPitched("DragonRoar", 600);

                        }

                        roarTimer = 120;

                        roarActive = true;

                        return true;

                    }

                }

            }

            return false;

        }

        public bool UpdateSpecial()
        {
            if (specialDelay > 0)
            {

                specialDelay--;

                return true;

            }

            if (specialTimer == -1)
            {

                List<Vector2> zeroes = BlastTarget();

                RoarCheck(zeroes[0]);

                if(!roarActive)
                {
                    currentLocation.playSoundPitched("furnace",600);
                }

                specialTimer = 48;

                if (!Mod.instance.TaskList().ContainsKey("masterBlast"))
                {

                    Mod.instance.UpdateTask("lessonBlast", 1);

                }

            }

            specialTimer--;

            fireTimer--;

            roarTimer--;

            if (specialTimer == 0 || swimActive || Game1.player.IsBusyDoingSomething())
            {

                specialActive = false;

                netSpecialActive.Set(false);

                return false;

            }

            if (Mod.instance.Helper.Input.IsDown(rightButton) && specialTimer == 12)
            {

                specialTimer = 18;

            }

            if (fireTimer <= 0)
            {

                List<Vector2> zeroes = BlastTarget();

                if (RoarCheck(zeroes[0]) || !AccountStamina(5))
                {

                    fireTimer = 24;

                    return true;

                };

                Vector2 minus = new((int)(Position.X / 64), (int)(Position.Y / 64));

                BarrageHandle fireball = new(currentLocation, zeroes[0], zeroes[1] - new Vector2(0,1), 3, 1, fireColor, -1, Mod.instance.DamageLevel() / 2, 3, 2);

                fireball.type = BarrageHandle.barrageType.fireball;

                fireball.counter = 30;

                fireball.LaunchFireball(2);

                barrages.Add(fireball);

                BarrageHandle burn = new(currentLocation, zeroes[0], minus, 2, 0, fireColor, -1, burnDamage);

                burn.type = BarrageHandle.barrageType.burn;

                burn.counter = -60;

                barrages.Add(burn);

                fireTimer = 48;

            }

            return true;

        }

        public List<Vector2> BlastTarget()
        {

            Vector2 tile = new Vector2((int)(Position.X / 64), (int)(Position.Y / 64));

            Vector2 zero = tile;

            Vector2 start = tile;

            List<Vector2> zeroes = new();

            switch (netDirection.Value)
            {
                case 0:

                    zero.X += 3;

                    zero.Y -= 5;

                    if (netAlternative.Value == 3 || flip)
                    {
                        zero.X -= 6;

                        break;

                    }

                    start.Y -= 1;

                    break;

                case 1:

                    zero.X += 6;

                    start.X += 1;

                    break;

                case 2:

                    zero.X += 3;

                    zero.Y += 4;

                    if (netAlternative.Value == 3 || flip)
                    {
                        zero.X -= 6;
                        break;

                    }
                    //start.Y += 1;

                    break;

                default:

                    zero.X -= 6;

                    start.X -= 1;

                    break;

            }

            zeroes.Add(zero);

            zeroes.Add(start);

            return zeroes;

        }

        public void UpdateSwim()
        {

            if (netDashActive.Value)
            {

                swimActive = false;

                netSwimActive.Set(false);

                return;

            }

            if (ModUtility.GroundCheck(currentLocation, new Vector2((int)(Position.X / 64), (int)(Position.Y / 64))) != "water")
            {

                swimActive = false;

                netSwimActive.Set(false);

                return;

            }

        }

        public void PerformDive()
        {

            diveActive = true;

            netDiveActive.Set(true);

            diveTimer = 240;

            divePosition = Game1.player.Position;

            diveMoment = 0;

            netDiveMoment.Set(0);

            currentLocation.playSound("pullItemFromWater");

            if (!Mod.instance.TaskList().ContainsKey("masterDive"))
            {

                Mod.instance.UpdateTask("lessonDive", 1);

            }

            currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(0, 125f, 8, 1, Position - new Vector2(64f, 160f), false, false)
            {
                sourceRect = new Rectangle(0, 0, 64, 64),
                sourceRectStartingPos = new Vector2(0.0f, 0.0f),
                texture = splashTexture,
                scale = 3f,
                layerDepth = 999f,
                alpha = 0.5f,
            });

        }

        public bool UpdateDive()
        {
            diveTimer--;

            if (diveTimer <= 0)
            {

                diveActive = false;
                netDiveActive.Set(false);
                return false;

            }

            Game1.player.Position = divePosition;
            Position = divePosition;

            if (diveTimer % 40 == 0)
            {

                diveMoment++;

                netDiveMoment.Set(diveMoment);

            }

            if (diveTimer == 120)
            {

                currentLocation.playSound("quickSlosh");

                currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(0, 125f, 8, 1, Position - new Vector2(96f, 180f), false, false)
                {
                    sourceRect = new Rectangle(0, 0, 64, 64),
                    sourceRectStartingPos = new Vector2(0.0f, 0.0f),
                    texture = splashTexture,
                    scale = 4f,
                    layerDepth = 999f,
                    alpha = 0.5f,
                });

                if (TreasureZone(true))
                {

                    return true;

                }

                Vector2 treasurePosition = Position + new Vector2(64, 0);

                int treasureIndex = SpawnData.RandomTreasure(currentLocation, Mod.instance.TaskList().ContainsKey("masterDive"));

                Throw treasure = new(Game1.player, treasurePosition, treasureIndex);

                if (treasure.objectInstance.Category == -4)
                {

                    treasure.UpdateQuality(2);

                    Game1.player.checkForQuestComplete(null, treasureIndex, 1, treasure.objectInstance, "fish", 7);

                    Game1.player.gainExperience(1, 8); // gain fishing experience


                }

                treasure.throwFade = 0.0005f;

                treasure.throwHeight = 3;

                treasure.ThrowObject();

                Mod.instance.CastMessage("Dove for a " + treasure.objectInstance.Name);

            }

            return true;

        }

        public void ShutDown()
        {

            if (flightActive && currentLocation.Name == Game1.player.currentLocation.Name)
            {

                Game1.player.Position = flightTo;

            }

            DelayedAction.functionAfterDelay(RemoveInstance,1);

        }

        public void RemoveInstance()
        {


            currentLocation.characters.Remove(this);


        }

        public bool SafeExit()
        {

            if (swimActive || flightTerrain == "water")
            {

                return false;

            }

            return true;

        }

        public bool AccountStamina(int cost)
        {

            if (Game1.player.Stamina <= 32 || Game1.player.health <= 25)
            {

                Mod.instance.AutoConsume();

            }

            if (Game1.player.Stamina <= cost)
            {

                Mod.instance.CastMessage("Not enough energy to perform skill", 3);

                return false;

            }

            float oldStamina = Game1.player.Stamina;

            float staminaCost = Math.Min(cost, oldStamina - 1);

            Game1.player.Stamina -= staminaCost;

            Game1.player.checkForExhaustion(oldStamina);

            return true;

        }

        public virtual void UpdateBarrages()
        {

            for (int i = barrages.Count - 1; i >= 0; i--)
            {

                BarrageHandle barrage = barrages[i];

                if (!barrage.Update())
                {

                    barrages.Remove(barrage);

                }

            }

        }

    }

}
