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
using StardewDruid.Map;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#nullable disable
namespace StardewDruid.Monster
{
    public class Boss : StardewValley.Monsters.Monster
    {
        public bool loadedOut;
        public Texture2D characterTexture;
        public List<string> ouchList;
        public List<string> dialogueList;
        public bool defeated;
        public int combatModifier;
        public NetInt netDirection = new NetInt(0);
        public NetInt netWalkFrame = new NetInt(0);
        public NetInt netAlternative = new NetInt(0);
        public Dictionary<int, List<Rectangle>> walkFrames;
        public int moveDirection;
        public int altDirection;
        public bool haltActive;
        public NetBool netHaltActive = new NetBool(false);
        public int haltTimer;
        public double angleDirection;
        public Vector2 trackPosition;
        public bool cooldownActive;
        public int cooldownTimer;
        public int cooldownInterval;
        public Texture2D shadowTexture;
        public List<Rectangle> shadowFrames;
        public NetInt netFlightFrame = new NetInt(0);
        public NetInt netFlightHeight = new NetInt(0);
        public NetBool netDashActive = new NetBool(false);
        public Texture2D flightTexture;
        public Dictionary<int, List<Rectangle>> flightFrames;
        public bool flightActive;
        public int flightTimer;
        public Vector2 flightPosition;
        public Vector2 flightTo;
        public Vector2 flightInterval;
        public bool flightFlip;
        public int flightIncrement;
        public int flightExtend;
        public int flightHeight;
        public int flightCeiling;
        public int flightFloor;
        public int flightLast;
        public NetInt netFireFrame = new NetInt(0);
        public NetBool netFireActive = new NetBool(false);
        public Texture2D specialTexture;
        public bool specialActive;
        public int specialTimer;
        public Texture2D fireTexture;
        public Dictionary<int, List<Rectangle>> fireFrames;
        public List<Vector2> fireVectors;
        public int fireTimer;
        public int fireCeiling;
        public int fireFloor;
        public int fireInterval;
        public Vector2 followTarget;
        public bool followActive;
        public int followTimer;
        public Vector2 followInterval;
        public int specialThreshold;
        public int reachThreshold;

        public Boss()
        {
        }

        public Boss(Vector2 vector, int CombatModifier, string name = "PurpleDragon")
          : base("Pepper Rex", new(vector.X * 64, vector.Y * 64))
        {
            Name = name;
            combatModifier = CombatModifier;
            objectsToDrop.Clear();
            breather.Value = false;
            hideShadow.Value = true;
            Sprite = CharacterData.CharacterSprite(Name);
            BaseMode();
            LoadOut();
        }

        public virtual void BaseMode()
        {
            Health = combatModifier * 12;
            MaxHealth = Health;
            DamageToFarmer = (int)(combatModifier * 0.1);
            ouchList = new List<string>()
              {
                "Ah ha ha ha ha",
                "I'll Answer That... With FIRE!",
                "insolence!",
                "creep"
              };
            dialogueList = new List<string>()
              {
                "behold",
                "I am your new master",
                "Kneel Before Tyrannus!",
                "Why do you resist",
                "Where are my servants?"
              };
            flightIncrement = 12;
            fireInterval = 12;
            flightHeight = 8;
            specialThreshold = 480;
            reachThreshold = 64;
            haltActive = true;
            haltTimer = 20;
            cooldownInterval = 60;
            cooldownTimer = cooldownInterval;
        }

        public virtual void LoadOut()
        {
            walkFrames = new Dictionary<int, List<Rectangle>>();
            foreach (KeyValuePair<int, int> keyValuePair in new Dictionary<int, int>()
            {
                [0] = 2,
                [1] = 1,
                [2] = 0,
                [3] = 3
            })
            {
                walkFrames[keyValuePair.Key] = new List<Rectangle>();
                for (int index = 0; index < 4; ++index)
                {
                    Rectangle rectangle = new(0, 0, 64, 64);
                    rectangle.X += 64 * index;
                    rectangle.Y += 64 * keyValuePair.Value;
                    walkFrames[keyValuePair.Key].Add(rectangle);
                }
            }
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
            flightCeiling = 4;
            flightFloor = 1;
            flightLast = 5;
            specialTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", Name + "Breath.png"));
            fireTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "DragonFire.png"));
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
            fireCeiling = 3;
            fireFloor = 2;

            characterTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", Name + ".png"));

            loadedOut = true;

        }

        public virtual void HardMode()
        {
            Health *= 3;
            Health /= 2;
            MaxHealth = Health;
            DamageToFarmer *= 3;
            DamageToFarmer /= 2;
            fireInterval = 9;
            ouchList = new List<string>()
              {
                "Ah ha ha ha ha",
                "Such pitiful strikes",
                "insolence!",
                "The land has died... and so WILL YOU",
                "CREEP"
              };
                    dialogueList = new List<string>()
              {
                "Where are my servants",
                "The shamans have failed me",
                "The only recourse for humanity, is subjugation",
                "Beg for my mercy",
                "Kneel Before Tyrannus Steve!",
                "I WILL BURNINATE... EVERYTHING"
              };
            cooldownInterval = 48;
        }

        public override Rectangle GetBoundingBox()
        {
            Vector2 position = Position;
            return new Rectangle((int)position.X - 72, (int)position.Y - 32 - netFlightHeight.Value, 196, 128);
        }

        public override void draw(SpriteBatch b, float alpha = 1f)
        {
            if (IsInvisible || !Utility.isOnScreen(Position, 128))
            {
                return;
            }

            if(!loadedOut)
            {
                LoadOut();

            }

            Vector2 localPosition = getLocalPosition(Game1.viewport);

            float drawLayer = Game1.player.getDrawLayer();

            if (IsEmoting && !Game1.eventUp)
            {
                Vector2 emotePosition = localPosition;
                emotePosition.Y -= 32 + Sprite.SpriteHeight * 4;
                b.Draw(Game1.emoteSpriteSheet, localPosition, new Rectangle?(new Rectangle(CurrentEmoteIndex * 16 % Game1.emoteSpriteSheet.Width, CurrentEmoteIndex * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, 0, drawLayer);
            }
            if (netDashActive)
            {
                
                b.Draw(flightTexture, new Vector2(localPosition.X - 128f, localPosition.Y - 192f - netFlightHeight), new Rectangle?(flightFrames[netDirection][netFlightFrame]), Color.White * 0.65f, 0, new Vector2(0.0f, 0.0f), 4f, flip || netDirection == 3 ? (SpriteEffects)1 : 0, drawLayer);
                
                b.Draw(shadowTexture, new Vector2(localPosition.X - 80f, localPosition.Y - 48f), new Rectangle?(shadowFrames[netDirection + 4]), Color.White * 0.25f, 0.0f, new Vector2(0.0f, 0.0f), 5f, flip || netDirection == 3 ? (SpriteEffects)1 : 0, drawLayer - 1E-05f);
            
            }
            else
            {
                if (netFireActive)
                {
                    
                    b.Draw(specialTexture, new Vector2(localPosition.X - 96f, localPosition.Y - 192f), new Rectangle?(walkFrames[netDirection][netWalkFrame]), Color.White * 0.65f, 0.0f, new Vector2(0.0f, 0.0f), 4f, flip ? (SpriteEffects)1 : 0, drawLayer);
                    
                    DrawFire(b, localPosition, netDirection, drawLayer);
                
                }
                else
                {

                    //b.Draw(Sprite.Texture, new Vector2(localPosition.X - 96f, localPosition.Y - 192f), new Rectangle?(walkFrames[netDirection][netWalkFrame]), Color.White * 0.65f, 0.0f, new Vector2(0.0f, 0.0f), 4f, flip ? (SpriteEffects)1 : 0, drawLayer);

                    b.Draw(characterTexture, new Vector2(localPosition.X - 96f, localPosition.Y - 192f), new Rectangle?(walkFrames[netDirection][netWalkFrame]), Color.White * 0.65f, 0.0f, new Vector2(0.0f, 0.0f), 4f, flip ? (SpriteEffects)1 : 0, drawLayer);

                }

                b.Draw(shadowTexture, new Vector2(localPosition.X - 96f, localPosition.Y - 40f), new Rectangle?(shadowFrames[netDirection]), Color.White * 0.25f, 0.0f, new Vector2(0.0f, 0.0f), 4f, flip || netDirection == 3 ? (SpriteEffects)1 : 0, drawLayer - 1E-05f);

            }

        }

        public virtual void SpecialAttack()
        {
            Vector2 zero = Vector2.Zero;
            Rectangle zone;
            switch (moveDirection)
            {
                case 0:
   
                    zero = new((int)(Position.X / 64.0) + 2, (int)(Position.Y / 64.0) - 4);
                    if (altDirection == 3 || flip)
                    {
                        zero.X -= 4f;
                    }
                    zone = new((int)((zero.X - 3.0) * 64.0), (int)((zero.Y - 1.0) * 64.0), 448, 256);
                    break;
                case 1:

                    zero = new((int)(Position.X / 64.0) + 5, (int)(Position.Y / 64.0));

                    zone = new((int)((zero.X - 1.0) * 64.0), (int)((zero.Y - 3.0) * 64.0), 256, 448);
                    break;
                case 2:

                    zero = new((int)(Position.X / 64.0) + 3, (int)(Position.Y / 64.0) + 1);
                    if (altDirection == 3 || flip)
                    {
                        zero.X -= 4f;

                    }

                    zone = new((int)((zero.X - 3.0) * 64.0), (int)((zero.Y - 1.0) * 64.0), 448, 256);
                    break;
                default:

                    zero = new((int)(Position.X / 64.0) - 4, (int)(Position.Y / 64.0));

                    zone = new((int)((zero.X - 1.0) * 64.0), (int)((zero.Y - 3.0) * 64.0), 256, 448);
                    break;
            }
            currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(23, 200f, 6, 1, new Vector2(zero.X * 64f, zero.Y * 64f), false, Game1.random.NextDouble() < 0.5)
            {
                texture = Game1.mouseCursors,
                light = true,
                lightRadius = 3f,
                lightcolor = Color.Black,
                alphaFade = 0.03f,
                Parent = currentLocation
            });
            ModUtility.DamageFarmers(currentLocation, zone, (int)(DamageToFarmer * 0.4), this);
        }

        public virtual void DrawFire(
          SpriteBatch b,
          Vector2 position,
          int direction,
          float depth,
          int adjust = 0)
        {
            fireVectors = new List<Vector2>()
            {
                new Vector2(44f, -336f),
                new Vector2(-348f, -336f),
                new Vector2(148f, -96f),
                new Vector2(60f, -106f),
                new Vector2(-380f, -106f),
                new Vector2(-400f, -96f)
            };
            float num1 = 5f;
            depth += 1f / 1000f;
            int num2;
            switch (direction)
            {
                case 0:
                    num2 = 0;
                    if (netAlternative == 3)
                        num2 = 1;
                    num1 = 6f;
                    depth -= 1f / 500f;
                    break;
                case 1:
                    num2 = 2;
                    break;
                case 2:
                    num2 = 3;
                    if (netAlternative == 3)
                        num2 = 4;
                    num1 = 6f;
                    break;
                default:
                    num2 = 5;
                    break;
            }
            int index = num2 + adjust;
            b.Draw(fireTexture,
                new(position.X + fireVectors[index].X, position.Y + fireVectors[index].Y),
                fireFrames[direction][netFireFrame],
                Color.White * 0.65f,
                0,
                new Vector2(0.0f, 0.0f),
                num1,
                flip || direction == 3 ? (SpriteEffects)1 : 0,
                depth);
        }

        protected override void initNetFields()
        {
            base.initNetFields();
            NetFields.AddFields(new INetSerializable[8]
            {
                 netDirection,
                 netWalkFrame,
                 netAlternative,
                 netFlightFrame,
                 netFlightHeight,
                 netDashActive,
                 netFireFrame,
                 netFireActive
            });
        }

        public override void reloadSprite()
        {
            
            Sprite = MonsterData.MonsterSprite(Name);

            this.HideShadow = true;
        
        }

        public override void shedChunks(int number)
        {
        }

        protected override void sharedDeathAnimation()
        {
            currentLocation.playSound("skeletonDie", 0);
            currentLocation.playSound("grunt", 0);
        }

        protected override void localDeathAnimation()
        {
            Utility.makeTemporarySpriteJuicier(new TemporaryAnimatedSprite(44, Position, Color.HotPink, 10, false, 100f, 0, -1, -1f, -1, 0)
            {
                holdLastFrame = true,
                alphaFade = 0.01f,
                interval = 70f
            }, currentLocation, 8, 96, 64);
        }

        public override List<Item> getExtraDropItems() => new List<Item>();

        public override int takeDamage(
          int damage,
          int xTrajectory,
          int yTrajectory,
          bool isBomb,
          double addedPrecision,
          Farmer who)
        {
            int damage1 = Math.Max(1, damage - resilience.Value);
            Health -= damage1;
            if (Game1.random.Next(5) == 0)
                setTrajectory(xTrajectory, yTrajectory);
            if (Health <= 0)
            {
                deathAnimation();
                defeated = true;
            }
            int index = Game1.random.Next(15);
            if (index < ouchList.Count)
                this.showTextAboveHead(ouchList[index], -1, 2, 3000, 0);
            return damage1;
        }

        public override void behaviorAtGameTick(GameTime time)
        {
        }

        public override void updateMovement(GameLocation location, GameTime time)
        {
        }

        protected override void updateAnimation(GameTime time)
        {
        }

        protected override void updateMonsterSlaveAnimation(GameTime time)
        {
        }

        public override void Halt()
        {
            if (haltTimer <= 0)
            {
                haltActive = true;
                haltTimer = 60;
                if (Context.IsMainPlayer)
                {
                    netHaltActive.Set(true);
                }
                    
            }
            flightActive = false;
            specialActive = false;
            followActive = false;
            if (Context.IsMainPlayer)
            {
                netWalkFrame.Set(0);
                netDashActive.Set(false);
                netFireActive.Set(false);
            }

        }

        public override void update(GameTime time, GameLocation location)
        {

            if (!Context.IsMainPlayer)
            {

                if (Sprite.loadedTexture == null || Sprite.loadedTexture.Length == 0)
                {

                    Sprite.spriteTexture = MonsterData.MonsterTexture(Name);

                    Sprite.loadedTexture = Sprite.textureName.Value;

                }
            
            }

            if (Mod.instance.CasterBusy())
            {
                return;
            }

            if (!loadedOut)
            {
                LoadOut();

            }

            if (!Context.IsMainPlayer)
            {

                base.update(time, location);
                
                FixDirection();
            
            }
            else
            {
                
                base.update(time, location);

                ChooseBehaviour(time);

                FixDirection();

                if (haltActive) { UpdateHalt(time); }
                    
                if (flightActive) { UpdateFlight(time); }
                    
                if (followActive) { UpdateFollow(time); }
                    
                if (specialActive) { UpdateSpecial(); }
                    
                if (!cooldownActive) { return; }

                UpdateCooldown();

            }

        }

        public void ChooseBehaviour(GameTime time)
        {
            if (xVelocity != 0.0 || yVelocity != 0.0)
            {
                xVelocity = 0.0f;
                yVelocity = 0.0f;
                Halt();
            }
            if (haltActive || flightActive || specialActive || followActive)
                return;
            Random random = new Random();
            List<Farmer> source = TargetFarmers();
            if (source.Count > 0)
            {
                Farmer farmer = source.First<Farmer>();
                float num = Vector2.Distance(Position, farmer.Position);
                TargetPlayer(farmer);
                if (!cooldownActive)
                {
                    switch (random.Next(2))
                    {
                        case 0:
                            if ((double)num < specialThreshold)
                            {
                                Halt();
                                PerformSpecial();
                                return;
                            }
                            PerformSpecial();
                            break;
                        case 1:
                            PerformFlight();
                            if (flightActive)
                                return;
                            break;
                    }
                }
                PerformFollow(farmer.Position);
            }
            else
                PerformRandom();
        }

        public List<Farmer> TargetFarmers()
        {
            List<Farmer> farmerList = new List<Farmer>();
            float num1 = 1200f;
            foreach (Farmer allFarmer in Game1.getAllFarmers())
            {
                if (allFarmer.currentLocation.Name == currentLocation.Name)
                {
                    float num2 = Vector2.Distance(Position, allFarmer.Position);
                    if ((double)num2 <= (double)num1)
                    {
                        farmerList.Clear();
                        farmerList.Add(allFarmer);
                        num1 = num2;
                    }
                }
            }
            return farmerList;
        }

        public void TargetPlayer(Farmer farmer)
        {
            Vector2 vector2 = new(farmer.Position.X - Position.X, farmer.Position.Y - Position.Y);
            float num1 = Math.Abs(vector2.X);
            float num2 = Math.Abs(vector2.Y);
            int num3 = vector2.X < 1.0 / 1000.0 ? -1 : 1;
            int num4 = vector2.Y < 1.0 / 1000.0 ? -1 : 1;
            altDirection = 0;
            if ((double)num1 > (double)num2)
            {
                moveDirection = num3 < 0 ? 3 : 1;
            }
            else
            {
                moveDirection = num4 < 0 ? 0 : 2;
                altDirection = num3 < 0 ? 3 : 1;
            }
        }

        public void FixDirection()
        {
            flip = false;
            if (!Context.IsMainPlayer)
            {
                moveDirection = netDirection;
                altDirection = netAlternative;
                trackPosition = Position;
            }
            else
            {
                netDirection.Set(moveDirection);
                netAlternative.Set(altDirection);
            }
            FacingDirection = moveDirection;
            switch (moveDirection)
            {
                case 0:
                    if (altDirection != 3)
                        break;
                    flip = true;
                    break;
                case 2:
                    if (altDirection != 3)
                        break;
                    flip = true;
                    break;
            }
        }

        public void UpdateHalt(GameTime time)
        {
            if (haltTimer % 20 == 0)
            {
                List<Farmer> source = TargetFarmers();
                if (source.Count > 0)
                {
                    TargetPlayer(source.First<Farmer>());
                    FixDirection();
                }
            }
            if (haltTimer <= 0)
            {
                haltActive = false;
                netHaltActive.Set(false);
            }
            --haltTimer;
        }

        public void PerformFlight()
        {
            int num = FlightDestination();
            if (num == 0)
                return;
            flightActive = true;
            netFlightFrame.Set(0);
            flightTimer = flightIncrement * num;
            flightInterval = new((flightTo.X - Position.X) / flightTimer, (flightTo.Y - Position.Y) / flightTimer);//Vector2.op_Division(Vector2.op_Subtraction(flightTo, Position), (float)flightTimer);
            flightExtend = 0;
        }

        public void UpdateFlight(GameTime time)
        {
            netDashActive.Set(true);
            --flightTimer;
            if (flightTimer == 0)
            {
                flightActive = false;
                netDashActive.Set(false);
                cooldownActive = true;
                cooldownTimer = cooldownInterval * 2;
            }
            else
            {
                if (flightHeight != -1)
                {
                    if (netFlightHeight < 16 * flightHeight && flightTimer > 16)
                        netFlightHeight.Set(netFlightHeight.Value + flightHeight);
                    else if (netFlightHeight > 0 && flightTimer <= 16)
                        netFlightHeight.Set(netFlightHeight.Value - flightHeight);
                }
                Position += flightInterval;
                if (flightTimer % flightIncrement != 0)
                    return;
                if (flightTimer == flightIncrement)
                {
                    netFlightFrame.Set(flightLast);
                }
                else
                {
                    netFlightFrame.Set(netFlightFrame.Value + 1);
                    if (netFlightFrame.Value > flightCeiling)
                        netFlightFrame.Set(flightFloor);
                }
            }
        }

        public int FlightDestination()
        {
            Dictionary<int, Vector2> dictionary = new Dictionary<int, Vector2>()
            {
                [0] = new Vector2(1f, -2f),
                [1] = new Vector2(-1f, -2f),
                [2] = new Vector2(2f, 0.0f),
                [3] = new Vector2(1f, 2f),
                [4] = new Vector2(-1f, 2f),
                [5] = new Vector2(-2f, 0.0f)
            };
            int key = 0;
            switch (moveDirection)
            {
                case 0:
                    if (altDirection == 3)
                    {
                        key = 1;
                        break;
                    }
                    break;
                case 1:
                    key = 2;
                    break;
                case 2:
                    key = 3;
                    if (altDirection == 3)
                    {
                        key = 4;
                        break;
                    }
                    break;
                case 3:
                    key = 5;
                    break;
            }
            Vector2 vector2 = dictionary[key];
            int num1 = 16;
            for (int index = num1; index > 0; --index)
            {
                int num2 = index <= 12 ? 17 - index : index - 12;
                Vector2 vectorMultiple = new(vector2.X * num2, vector2.Y * num2);
                Vector2 tileLocation = getTileLocation();
                Vector2 neighbour = new(tileLocation.X + vectorMultiple.X, tileLocation.Y + vectorMultiple.Y);//Vector2.op_Addition(getTileLocation(), Vector2.op_Multiply(vector2, (float)num2));
                if (ModUtility.GroundCheck(currentLocation, neighbour) == "ground")
                {
                    Rectangle boundingBox = Game1.player.GetBoundingBox();
                    int num3 = (int)(boundingBox.X - (double)Game1.player.Position.X);
                    int num4 = (int)(boundingBox.Y - (double)Game1.player.Position.Y);
                    boundingBox.X = (int)(neighbour.X * 64.0) + num3;
                    boundingBox.Y = (int)(neighbour.Y * 64.0) + num4;
                    if (!currentLocation.isCollidingPosition(boundingBox, Game1.viewport, false, 0, false, Game1.player, false, false, false))
                    {
                        flightTo = new(neighbour.X * 64, neighbour.Y * 64);//Vector2.op_Multiply(neighbour, 64f);
                        return num2;
                    }
                }
            }
            return 0;
        }

        public void PerformSpecial()
        {
            specialActive = true;
            specialTimer = 72;
            netFireFrame.Set(-1);
            fireTimer = fireInterval;
        }

        public void UpdateSpecial()
        {
            --specialTimer;
            --fireTimer;
            if (specialTimer == 0 || Game1.player.IsBusyDoingSomething())
            {
                specialActive = false;
                cooldownActive = true;
                cooldownTimer = cooldownInterval;
                netFireActive.Set(false);
            }
            else
            {
                if (specialTimer % 12 == 0)
                {
                    netFireActive.Set(true);
                    netFireFrame.Set(netFireFrame.Value + 1);
                    if (netFireFrame > fireCeiling)
                        netFireFrame.Set(fireFloor);
                }
                if (fireTimer != 0)
                    return;
                SpecialAttack();
                fireTimer = fireInterval;
            }
        }

        public void PerformFollow(Vector2 target)
        {
            float num = Vector2.Distance(Position, target);
            if ((double)num > reachThreshold)
            {
                followTarget = target;
                followActive = true;
                followTimer = 40;
                followInterval = new((target.X - Position.X) / num * 2f, (target.Y - Position.Y) / num * 2f);//Vector2.op_Multiply(Vector2.op_Division(Vector2.op_Subtraction(target, Position), num), 2f);
            }
            else
                Halt();
        }

        public void UpdateFollow(GameTime time)
        {
            --followTimer;
            if (followTimer == 0)
                followActive = false;
            Position += followInterval;//Vector2.op_Addition(Position, followInterval);
            if (followTimer % 12 != 0)
                return;
            if (netWalkFrame.Value == 3)
                netWalkFrame.Set(0);
            else
                netWalkFrame.Set(netWalkFrame.Value + 1);
        }

        public void PerformRandom()
        {
            Dictionary<int, Vector2> dictionary = new Dictionary<int, Vector2>()
            {
                [0] = new Vector2(1f, -2f),
                [1] = new Vector2(-1f, -2f),
                [2] = new Vector2(2f, 0.0f),
                [3] = new Vector2(1f, 2f),
                [4] = new Vector2(-1f, 2f),
                [5] = new Vector2(-2f, 0.0f)
            };
            int key = new Random().Next(dictionary.Count);
            Vector2 target = new(dictionary[key].X * 128f, dictionary[key].Y * 128);//Vector2.op_Multiply(dictionary[key], 128f);
            switch (key)
            {
                case 0:
                    moveDirection = 0;
                    altDirection = 1;
                    break;
                case 1:
                    moveDirection = 0;
                    altDirection = 3;
                    break;
                case 2:
                    moveDirection = 1;
                    altDirection = 1;
                    break;
                case 3:
                    moveDirection = 2;
                    altDirection = 1;
                    break;
                case 4:
                    moveDirection = 2;
                    altDirection = 3;
                    break;
                case 5:
                    moveDirection = 3;
                    altDirection = 3;
                    break;
            }
            PerformFollow(target);
        }

        public void UpdateCooldown()
        {
            --cooldownTimer;
            if (cooldownTimer == cooldownInterval * 0.5 && new Random().Next(3) == 0)
                this.showTextAboveHead(dialogueList[Game1.random.Next(dialogueList.Count)], -1, 2, 3000, 0);
            if (cooldownTimer > 0)
                return;
            cooldownActive = false;
        }
    }
}