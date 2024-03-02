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
using StardewDruid.Event;
using StardewDruid.Map;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Events;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace StardewDruid.Monster.Boss
{
    public class Dragon : StardewValley.Monsters.Monster
    {

        public NetString realName = new NetString();

        public bool localMonster;
        public bool loadedOut;
        public Texture2D characterTexture;
        public List<string> ouchList;
        public List<string> dialogueList;
        public int combatModifier;
        public NetInt netDirection = new NetInt(0);
        public NetInt netWalkFrame = new NetInt(0);
        public NetInt netAlternative = new NetInt(0);
        public Dictionary<int, List<Rectangle>> walkFrames;
        public int walkFloor;
        public int walkCeiling;

        public enum behaviour
        {
            idle,
            halt,
            flight,
            special,
            follow,
            barrage,
        }

        public behaviour behaviourActive;

        public int behaviourTimer;

        public enum temperment
        {

            cautious,
            coward,
            aggressive,

        }

        public temperment tempermentActive;

        public int abilities;
        public bool cooldownActive;
        public int cooldownTimer;
        public int cooldownInterval;
        public int ouchTimer;
        public Vector2 overHead;

        // ============================= Flight

        public NetInt netFlightFrame = new NetInt(0);
        public NetInt netFlightHeight = new NetInt(0);
        public NetBool netFlightActive = new NetBool(false);
        public Texture2D flightTexture;
        public Dictionary<int, List<Rectangle>> flightFrames;
        public Vector2 flightPosition;
        public Vector2 flightTo;
        public Vector2 flightInterval;
        public int flightSpeed;
        public bool flightFlip; // flips flight animation
        public int flightIncrement; // flight timer intervals to adjust speed
        public int flightHeight; // how high to raise the sprite on Y axis
        public int flightCeiling; // lowest frame for flight cycle
        public int flightFloor; // highest frame for flight cycle
        public int flightLast; // last frame for flight cycle (strike, land)

        // ============================= Special attack

        public NetInt netSpecialFrame = new NetInt(0);
        public NetBool netSpecialActive = new NetBool(false);
        public Texture2D specialTexture;
        public Dictionary<int, List<Rectangle>> specialFrames;
        public int specialThreshold;
        public int specialCeiling;
        public int specialFloor;

        // ============================= Barrage attack

        public NetInt netBarrageFrame = new NetInt(0);
        public NetBool netBarrageActive = new NetBool(false);
        public int barrageThreshold;
        public List<BarrageHandle> barrages;
        public string barrageColor;

        // ============================= Follow behaviour

        public Vector2 followInterval;
        public int followIncrement;
        public int reachThreshold;
        public int safeThreshold;

        // ============================= Dragon Specific

        public Texture2D shadowTexture;
        public List<Rectangle> shadowFrames;

        public Dragon()
        {
        }

        public Dragon(Vector2 vector, int CombatModifier, string name = "PurpleDragon", string template = "Pepper Rex")
          : base(template, new(vector.X * 64, vector.Y * 64))
        {
            realName.Set(name);
            localMonster = true;
            combatModifier = CombatModifier;

            //=================== base fields

            Health = Math.Min(4000,combatModifier * 400);
            MaxHealth = Health;
            DamageToFarmer = Math.Max(10, Math.Min(50, combatModifier * 3));
            focusedOnFarmers = true;
            objectsToDrop.Clear();
            breather.Value = false;
            hideShadow.Value = true;

            //=================== reconfigurable fields

            behaviourActive = behaviour.halt;
            behaviourTimer = 20;
            tempermentActive = temperment.cautious;

            LoadOut();

        }

        public virtual void LoadOut()
        {

            DragonWalk();

            DragonFlight();

            DragonSpecial();

            loadedOut = true;

        }

        public void BaseWalk()
        {
            
            characterTexture = MonsterData.MonsterTexture(realName.Value);

            walkCeiling = 3;

            walkFloor = 0;

            followIncrement = 2;

            walkFrames = WalkFrames(32, 32);

            overHead = new(0, -128);

        }

        public virtual void DragonWalk()
        {

            characterTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", realName.Value + ".png"));

            walkCeiling = 6;

            walkFloor = 1;

            followIncrement = 2;

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

            walkFrames = WalkFrames(64, 64);

            overHead = new(0, -224);

        }

        public void BaseFlight()
        {

            flightIncrement = 9;

            flightSpeed = 12;

            flightHeight = 4;

            flightCeiling = 3;

            flightFloor = 0;

            flightLast = 2;

            flightTexture = characterTexture;

            flightFrames = walkFrames;

        }

        public virtual void DragonFlight()
        {

            flightIncrement = 9;

            flightSpeed = 12;

            flightHeight = 8;

            flightCeiling = 4;

            flightFloor = 1;

            flightLast = 5;

            flightTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", realName.Value + "Flight.png"));

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

        }

        public virtual void BaseSpecial()
        {

            abilities = 2;

            cooldownInterval = 60;

            specialCeiling = 3;

            specialFloor = 0;

            reachThreshold = 64;

            safeThreshold = 544;

            specialThreshold = 320;

            barrageThreshold = 544;

            barrageColor = "Blue";

            barrages = new();

            specialTexture = characterTexture;

            specialFrames = walkFrames;

        }

        public virtual void DragonSpecial()
        {

            abilities = 3;

            cooldownInterval = 60;

            specialCeiling = 1;

            specialFloor = 0;

            reachThreshold = 64;

            safeThreshold = 544;

            specialThreshold = 320;

            barrageThreshold = 544;

            barrageColor = realName.Value.Replace("Dragon", "");

            barrages = new();

            specialFrames = walkFrames;

            specialTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", realName.Value + "Special.png"));

        }

        public virtual void HardMode()
        {

            Health *= 3 / 2;
            MaxHealth = Health;

            cooldownInterval = 48;

            tempermentActive = temperment.aggressive;

        }

        public virtual void ChaseMode()
        {

            followIncrement = 2;

            cooldownInterval = 120;

            cooldownTimer = 120;

            cooldownActive = true;

            tempermentActive = temperment.coward;

        }

        public override void draw(SpriteBatch b, float alpha = 1f)
        {
            if (IsInvisible || !Utility.isOnScreen(Position, 128))
            {
                return;
            }

            Vector2 localPosition = getLocalPosition(Game1.viewport);

            float drawLayer = Game1.player.getDrawLayer();

            if (IsEmoting && !Game1.eventUp)
            {
                Vector2 emotePosition = localPosition;
                emotePosition.Y -= 32 + Sprite.SpriteHeight * 4;
                b.Draw(Game1.emoteSpriteSheet, localPosition, new Rectangle?(new Rectangle(CurrentEmoteIndex * 16 % Game1.emoteSpriteSheet.Width, CurrentEmoteIndex * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, 0, drawLayer);
            }

            int adjustDirection = netDirection.Value == 3 ? 1 : netDirection.Value;

            if (netFlightActive.Value)
            {

                b.Draw(flightTexture, new Vector2(localPosition.X - 128f, localPosition.Y - 192f - netFlightHeight.Value), new Rectangle?(flightFrames[adjustDirection][netFlightFrame.Value]), Color.White * 0.65f, 0, new Vector2(0.0f, 0.0f), 4f, (netDirection.Value % 2 == 0 && netAlternative.Value == 3) || netDirection.Value == 3 ? (SpriteEffects)1 : 0, drawLayer);

                b.Draw(shadowTexture, new Vector2(localPosition.X - 80f, localPosition.Y - 48f), new Rectangle?(shadowFrames[netDirection.Value + 4]), Color.White * 0.25f, 0.0f, new Vector2(0.0f, 0.0f), 5f, (netDirection.Value % 2 == 0 && netAlternative.Value == 3) || netDirection.Value == 3 ? (SpriteEffects)1 : 0, drawLayer - 1E-05f);

                return;

            }

            if (netSpecialActive.Value)
            {

                b.Draw(specialTexture, new Vector2(localPosition.X - 96f, localPosition.Y - 192f), new Rectangle?(walkFrames[adjustDirection][netWalkFrame.Value]), Color.White * 0.65f, 0.0f, new Vector2(0.0f, 0.0f), 4f, (netDirection.Value % 2 == 0 && netAlternative.Value == 3) || netDirection.Value == 3 ? (SpriteEffects)1 : 0, drawLayer);

            }
            else
            {

                b.Draw(characterTexture, new Vector2(localPosition.X - 96f, localPosition.Y - 192f), new Rectangle?(walkFrames[adjustDirection][netWalkFrame.Value]), Color.White * 0.65f, 0.0f, new Vector2(0.0f, 0.0f), 4f, (netDirection.Value % 2 == 0 && netAlternative.Value == 3) || netDirection.Value == 3 ? (SpriteEffects)1 : 0, drawLayer);

            }

            b.Draw(shadowTexture, new Vector2(localPosition.X - 96f, localPosition.Y - 40f), new Rectangle?(shadowFrames[adjustDirection]), Color.White * 0.25f, 0.0f, new Vector2(0.0f, 0.0f), 4f, (netDirection.Value % 2 == 0 && netAlternative.Value == 3) || netDirection.Value == 3 ? (SpriteEffects)1 : 0, drawLayer - 1E-05f);

        }

        public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
        {

            if (textAboveHeadTimer <= 0 || textAboveHead == null)
            {
                return;
            }
            Vector2 localPosition = getLocalPosition(Game1.viewport);

            SpriteText.drawStringWithScrollCenteredAt(b, textAboveHead, (int)localPosition.X + (int)overHead.X, (int)localPosition.Y + (int)overHead.Y, "", textAboveHeadAlpha, textAboveHeadColor, 1, (float)(getTileY() * 64 / 10000.0 + 1.0 / 1000.0 + getTileX() / 10000.0), false);

        }

        public virtual Dictionary<int, List<Rectangle>> WalkFrames(int height, int width)
        {

            Dictionary<int, List<Rectangle>> walkFrames = new();

            foreach (KeyValuePair<int, int> keyValuePair in new Dictionary<int, int>()
            {
                [0] = 2,
                [1] = 1,
                [2] = 0,
                [3] = 3
            })
            {
                walkFrames[keyValuePair.Key] = new List<Rectangle>();
                for (int index = 0; index < (walkCeiling + 1); index++)
                {
                    Rectangle rectangle = new(0, 0, width, height);
                    rectangle.X += width * index;
                    rectangle.Y += height * keyValuePair.Value;
                    walkFrames[keyValuePair.Key].Add(rectangle);
                }

            }

            return walkFrames;

        }

        public override Rectangle GetBoundingBox()
        {
            
            Vector2 position = Position;
            
            return new Rectangle((int)position.X - 72, (int)position.Y - 64 - netFlightHeight.Value, 196, 160);

        }

        protected override void initNetFields()
        {
            base.initNetFields();
            NetFields.AddFields(new INetSerializable[9]
            {
                 realName,
                 netDirection,
                 netWalkFrame,
                 netAlternative,
                 netFlightFrame,
                 netFlightHeight,
                 netFlightActive,
                 netSpecialFrame,
                 netSpecialActive
            });
        }

        //=================== overriden base fields

        public override List<Item> getExtraDropItems()
        {
            return new List<Item>();
        }

        public override int takeDamage(int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
        {

            if (!localMonster)
            {

                damage = Math.Min(damage, Health - 2);

                if(damage <= 0)
                {

                    return -1;

                }

                Health -= damage;

                return damage;

            }

            damage = Math.Max(1, damage);

            Health -= damage;

            if (Health <= 0)
            {

                if (barrages.Count > 0)
                {

                    foreach (BarrageHandle barrage in barrages)
                    {

                        barrage.Shutdown();

                    }

                }

                ModUtility.AnimateDeathSpray(currentLocation,Position,Color.Gray);

            }

            if (ouchTimer < (int)Game1.currentGameTime.TotalGameTime.TotalSeconds)
            {

                DialogueData.DisplayText(this, 3, 0, realName);

                ouchTimer = (int)Game1.currentGameTime.TotalGameTime.TotalSeconds + 6;

            }

            return damage;

        }

        //=================== behaviour methods

        public override void Halt()
        {

            if (!localMonster)
            {
                return;
            }

            if (behaviourTimer <= 0)
            {
                
                behaviourActive = behaviour.halt;
                
                behaviourTimer = 60;
            
            }

            netWalkFrame.Set(0);

            netFlightActive.Set(false);

            netFlightFrame.Set(0);

            netFlightHeight.Set(0);

            netSpecialActive.Set(false);

            netSpecialFrame.Set(0);

        }

        public void SetDirection(Vector2 target)
        {

            int moveDirection;

            int altDirection;

            Vector2 difference = new(target.X - Position.X, target.Y - Position.Y);

            float absoluteX = Math.Abs(difference.X);

            float absoluteY = Math.Abs(difference.Y);

            int signX = difference.X < 0.001f ? -1 : 1;

            int signY = difference.Y < 0.001f ? -1 : 1;

            if (absoluteX > absoluteY)
            {

                moveDirection = 2 - signX;

                altDirection = 1 + signY;

            }
            else
            {
                moveDirection = 1 + signY;

                altDirection = 2 - signX;

            }

            netDirection.Set(moveDirection);

            netAlternative.Set(altDirection);

            FacingDirection = netDirection.Value;

        }

        public virtual bool baseupdate(GameTime time, GameLocation location)
        {
        
            if (!location.farmers.Any())
            {
                
                return false;

            }

            if (invincibleCountdown > 0)
            {

                invincibleCountdown -= time.ElapsedGameTime.Milliseconds;

                if (invincibleCountdown <= 0)
                {

                    stopGlowing();

                }

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

                        textAboveHeadAlpha = Math.Min(1f, textAboveHeadAlpha + 0.1f);

                    else
                        textAboveHeadAlpha = Math.Max(0.0f, textAboveHeadAlpha - 0.04f);

                }

            }

            updateGlow();

            updateEmote(time);

            updateFaceTowardsFarmer(time, location);

            if (localMonster)
            {

                currentLocation = location;

                if (stunTime > 0)
                {

                    stunTime -= (int)time.ElapsedGameTime.TotalMilliseconds;

                    if (behaviourActive != behaviour.halt)
                    {
                        Halt();
                    }

                    return false;

                }

            }

            return true;

        }

        public override void update(GameTime time, GameLocation location)
        {

            if (!loadedOut)
            {

                LoadOut();

            }

            if (Mod.instance.CasterBusy())
            {

                return;
            }

            if (!baseupdate(time, location))
            {

                return;

            };

            if(!localMonster)
            {

                return;

            }

            ChooseBehaviour();

            behaviourTimer--;

            switch (behaviourActive)
            {

                case behaviour.halt:

                    UpdateHalt();

                    break;

                case behaviour.flight:

                    UpdateFlight();

                    break;

                case behaviour.follow:

                    UpdateFollow();

                    break;

                case behaviour.special:
                case behaviour.barrage:

                    UpdateSpecial();

                    break;
            
            }

            if (cooldownActive)
            {

                UpdateCooldown();

            }

            if (barrages.Count > 0)
            {

                UpdateBarrages();

            }

        }

        public void ResetAll()
        {

            netWalkFrame.Set(0);

            netFlightFrame.Set(0);

            netSpecialFrame.Set(0);

        }

        public void UpdateFollow()
        {

            if (behaviourTimer <= 0)
            {

                behaviourActive = behaviour.idle;

            }

            Position += followInterval;

            if (behaviourTimer % 12 == 0)
            {

                UpdateWalk();

            }

        }

        public void UpdateWalk()
        {

            int nextFrame = netWalkFrame.Value + 1;

            if (nextFrame == walkCeiling)
            {

                nextFrame = walkFloor;

            }

            if (walkFrames[netDirection.Value].Count == nextFrame)
            {

                nextFrame = 0;

            }

            netWalkFrame.Set(nextFrame);

        }

        public void UpdateFlight()
        {

            if (behaviourTimer <= 0)
            {

                behaviourActive = behaviour.idle;

                netFlightActive.Set(false);

                netFlightFrame.Set(0);

                netFlightHeight.Set(0);

                cooldownActive = true;

                cooldownTimer = cooldownInterval * 2;

            }
            else
            {

                if (flightHeight != -1)
                {

                    if (netFlightHeight.Value < (16 * flightHeight) && behaviourTimer > 16)
                    {

                        netFlightHeight.Set(netFlightHeight.Value + flightHeight);

                    }
                    else if (netFlightHeight.Value > 0 && behaviourTimer <= 16)
                    {

                        netFlightHeight.Set(netFlightHeight.Value - flightHeight);

                    }

                }

                Position += flightInterval;

                if (behaviourTimer % flightIncrement != 0)
                {

                    return;

                }

                if (behaviourTimer == flightIncrement)
                {

                    netFlightFrame.Set(flightLast);

                }
                else
                {

                    int nextFlight = netFlightFrame.Value + 1;

                    if (nextFlight > flightCeiling)
                    {

                        nextFlight = flightFloor;

                    }

                    if (flightFrames[netDirection.Value].Count == nextFlight)
                    {

                        nextFlight = 0;

                    }

                    netFlightFrame.Set(nextFlight);

                }

            }

        }

        public void UpdateSpecial()
        {

            if (behaviourTimer <= 0 || Game1.player.IsBusyDoingSomething())
            {

                behaviourActive = behaviour.idle;

                cooldownActive = true;

                cooldownTimer = cooldownInterval;

                netSpecialActive.Set(false);

                netSpecialFrame.Set(0);

            }
            else
            {

                if (behaviourTimer % 12 == 0)
                {

                    int nextFrame = netSpecialFrame.Value + 1;

                    if (nextFrame > specialCeiling)
                    {

                        nextFrame = specialFloor;

                    }

                    if (specialFrames[netDirection.Value].Count == nextFrame)
                    {

                        nextFrame = 0;

                    }

                    netSpecialFrame.Set(nextFrame);

                }

            }

        }


        public void UpdateCooldown()
        {

            cooldownTimer--;

            if (cooldownTimer == cooldownInterval * 0.5)
            {

                DialogueData.DisplayText(this, 1, (int)tempermentActive, realName);

            }

            if (cooldownTimer > 0)
            {

                return;

            }

            cooldownActive = false;

        }

        public virtual void ChooseBehaviour()
        {

            if (behaviourActive != behaviour.idle)
            {

                return;

            }

            Random random = new Random();

            List<Farmer> source = TargetFarmers();

            if (source.Count == 0)
            {

                PerformRandom();

                return;
            }

            Farmer farmer = source.First();

            float threshold = Vector2.Distance(Position, farmer.Position);

            SetDirection(farmer.Position);

            if (!cooldownActive)
            {

                switch (random.Next(abilities))
                {

                    case 2:

                        Halt();

                        if (threshold > specialThreshold && threshold < barrageThreshold)
                        {

                            PerformBarrage();

                        }
                        else if (threshold < specialThreshold)
                        {

                            PerformSpecial();

                        }
                        else
                        {

                            ChooseMovement(threshold, farmer.Position);

                        }

                        return;

                    case 1:

                        Halt();

                        if (threshold < specialThreshold)
                        {

                            PerformSpecial();

                        }
                        else
                        {

                            ChooseMovement(threshold, farmer.Position);

                        }

                        return;

                    default:
                        
                        PerformFlight();

                        if (behaviourActive == behaviour.flight)
                        {
                            return;
                        }

                        break;

                }

            }

            ChooseMovement(threshold, farmer.Position);

        }

        public void ChooseMovement(float threshold, Vector2 position)
        {

            if (tempermentActive == temperment.coward)
            {

                PerformRetreat(position);

            }
            else if (tempermentActive == temperment.aggressive)
            {

                PerformFollow(position);

            }
            else
            {

                if (threshold > specialThreshold)
                {

                    PerformFollow(position);

                }
                else if (threshold <= reachThreshold)
                {

                    PerformRetreat(position);

                }
                else
                {

                    PerformCircle(position);

                }

            }

        }

        public List<Farmer> TargetFarmers()
        {

            List<Farmer> farmerList = new List<Farmer>();

            float threshold = 1200f;

            FarmerCollection farmers = currentLocation.farmers;

            if (!farmers.Any())
            {

                return farmerList;
           
            }

            using (FarmerCollection.Enumerator enumerator = farmers.GetEnumerator())
            {
                
                while (enumerator.MoveNext())
                {

                    float distance = Vector2.Distance(Position, enumerator.Current.Position);

                    if (distance <= threshold)
                    {

                        farmerList.Clear();

                        farmerList.Add(enumerator.Current);

                        threshold = distance;

                    }

                }

            }

            return farmerList;

        }

        public void UpdateHalt()
        {

            if (behaviourTimer % 20 == 0)
            {

                List<Farmer> source = TargetFarmers();

                if (source.Count > 0)
                {
                    
                    SetDirection(source.First().Position);

                }

            }

            if (behaviourTimer <= 0)
            {

                behaviourActive = behaviour.idle;

            }

        }

        public void PerformFlight(int adjust = 0)
        {

            int destination = FlightDestination(adjust);
            
            if (destination == 0)
            {
                return;
            }

            ResetAll();

            behaviourActive = behaviour.flight;

            netFlightActive.Set(true);

            behaviourTimer = flightSpeed * destination;

            flightInterval = new((flightTo.X - Position.X) / behaviourTimer, (flightTo.Y - Position.Y) / behaviourTimer);

        }

        public int FlightDestination(int adjust = 0, int newDirection = -1, int newAlternative = -1)
        {

            int moveDirection = newDirection == -1 ? netDirection.Value : newDirection;

            int altDirection = newAlternative == -1 ? netAlternative.Value : newAlternative;

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

            for (int index = 16; index > adjust; index--)
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

        public virtual List<Vector2> BlastTarget(int randomisation = -1)
        {

            Vector2 centerPosition = GetBoundingBox().Center.ToVector2();

            Vector2 tile = new Vector2((int)(centerPosition.X / 64), (int)(centerPosition.Y / 64));

            Vector2 zero = tile;

            Vector2 start = tile;

            List<Vector2> zeroes = new();

            switch (netDirection.Value)
            {
                case 0:

                    zero.X += 3;

                    zero.Y -= 4;

                    if (netAlternative.Value == 3)
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

                    if (netAlternative.Value == 3)
                    {
                        zero.X -= 6;
                        break;

                    }

                    start.X += 1;

                    break;

                default:

                    zero.X -= 6;

                    start.X -= 1;
                    break;

            }

            if (randomisation != -1)
            {
                Random random = new();

                zero.X -= randomisation;
                zero.X += random.Next(randomisation * 2);
                zero.Y -= randomisation;
                zero.Y += random.Next(randomisation * 2);

            }
            zeroes.Add(zero);

            zeroes.Add(start);

            return zeroes;

        }

        public virtual void PerformSpecial()
        {

            ResetAll();

            behaviourActive = behaviour.special;

            behaviourTimer = 72;

            netSpecialActive.Set(true);

            currentLocation.playSound("furnace");

            List<Vector2> zero = BlastTarget();

            BarrageHandle fireball = new(currentLocation, zero[0], zero[1] - new Vector2(0, 2), 4, 1, barrageColor, DamageToFarmer * 0.5f);

            fireball.type = BarrageHandle.barrageType.fireball;

            fireball.counter = 30;

            fireball.LaunchFireball(2);

            barrages.Add(fireball);

            BarrageHandle burn = new(currentLocation, zero[0], zero[1], 2, 0, barrageColor, DamageToFarmer * 0.2f);

            burn.type = BarrageHandle.barrageType.burn;

            burn.monster = this;

            burn.counter = -60;

            barrages.Add(burn);

        }

        public void PerformFollow(Vector2 target)
        {

            SetDirection(target);

            float distance = Vector2.Distance(Position, target);

            if (distance > reachThreshold)
            {

                behaviourActive = behaviour.follow;

                behaviourTimer = 36;

                followInterval = new((target.X - Position.X) / distance * followIncrement, (target.Y - Position.Y) / distance * followIncrement);//Vector2.op_Multiply(Vector2.op_Division(Vector2.op_Subtraction(target, Position), num), 2f);

            }
            else
            {

                Halt();

            }

        }

        public void PerformRetreat(Vector2 target)
        {

            float distance = Vector2.Distance(Position, target);

            int moveDirection = (netDirection.Value + 2) % 4;

            int altDirection = (netAlternative.Value + 2) % 4;

            int destination = FlightDestination(8,moveDirection,altDirection);

            if (destination != 0)
            {

                if (distance > safeThreshold || new Random().Next(2) == 0)
                {

                    PerformFollow(flightTo);

                    return;

                }

                ResetAll();

                netDirection.Set(moveDirection);

                netAlternative.Set(altDirection);

                netFlightActive.Set(true);

                behaviourActive = behaviour.flight;

                behaviourTimer = flightIncrement * destination;

                flightInterval = new((flightTo.X - Position.X) / behaviourTimer, (flightTo.Y - Position.Y) / behaviourTimer);

                return;

            }

            PerformCircle(target);

        }

        public void PerformCircle(Vector2 target, bool reverse = false)
        {

            int moveDirection = netDirection.Value;

            int altDirection = netAlternative.Value;

            if (reverse)
            {

                moveDirection = (moveDirection + 2) % 4;

                altDirection = (altDirection + 2) % 4;

            }

            switch (moveDirection)
            {

                case 0:

                    if (altDirection == 3)
                    {

                        altDirection = 1;

                        break;

                    }

                    moveDirection = 1;

                    break;

                case 1:

                    moveDirection = 2;

                    altDirection = 1;

                    break;

                case 2:

                    if (altDirection == 1)
                    {

                        altDirection = 3;

                        break;

                    }

                    moveDirection = 3;

                    break;

                case 3:

                    moveDirection = 0;

                    altDirection = 3;

                    break;

            }

            float distance = Vector2.Distance(Position, target);

            int destination = FlightDestination(12,moveDirection,altDirection);

            if (destination != 0)
            {

                PerformFollow(flightTo);

                return;

            }

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

            PerformFollow(target);

        }

        public void PerformBarrage()
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

            ResetAll();

            behaviourActive = behaviour.barrage;

            behaviourTimer = 60;

            netSpecialActive.Set(true);

            cooldownActive = true;

            cooldownTimer = 300;

            int castIndex;

            Vector2 newVector;

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

                BarrageHandle missile = new(currentLocation, newVector, centralVector, 3, 1, barrageColor, DamageToFarmer);

                missile.monster = this;

                barrages.Add(missile);

            }

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