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
using StardewDruid.Data;
using StardewDruid.Event;
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
using System.Threading;
using static StardewDruid.Event.SpellHandle;
using static StardewValley.Menus.CharacterCustomization;
using static StardewValley.Minigames.TargetGame;

namespace StardewDruid.Monster.Boss
{
    public class Boss : StardewValley.Monsters.Monster
    {

        public NetString realName = new NetString();

        public int moveDirection;
        public int altDirection;

        public bool localMonster;
        public bool loadedOut;
        public Texture2D characterTexture;
        public List<string> ouchList;
        public List<string> dialogueList;
        public int combatModifier;
        public NetInt netDirection = new NetInt(0);
        public NetInt netAlternative = new NetInt(0);

        public NetInt netMode = new NetInt(0);

        public enum temperment
        {

            cautious,
            coward,
            aggressive,
            odd,

        }

        public temperment tempermentActive;

        // ============================= Behaviour

        public Dictionary<int, List<Rectangle>> idleFrames;
        public int idleTimer;
        public NetBool netHaltActive = new NetBool(false);
        public int idleFrame;

        // ============================= Follow behaviour

        public Vector2 followIncrement;
        public int followTimer;
        public int gait;
        public int reachThreshold;
        public int safeThreshold;

        public Dictionary<int, List<Rectangle>> walkFrames;
        public int walkFrame;
        public int walkTimer;
        public int walkFloor;
        public int walkCeiling;
        public int walkInterval;
        public Vector2 setPosition;
        public int stationaryTimer;

        public int abilities;
        public bool cooldownActive;
        public int cooldownTimer;
        public int cooldownInterval;
        public int talkTimer;
        public Vector2 overHead;

        // ============================= Sweep

        public int sweepFrame;
        public int sweepTimer;
        public bool sweepSet;
        public NetBool netSweepActive = new NetBool(false);
        public Texture2D sweepTexture;
        public Dictionary<int, List<Rectangle>> sweepFrames;
        public int sweepInterval;
        public Vector2 sweepIncrement;
        public int sweepThreshold;

        // ============================= Flight

        public int flightFrame;
        public int flightTimer;
        public NetBool netFlightActive = new NetBool(false);
        public Texture2D flightTexture;
        public Dictionary<int, List<Rectangle>> flightFrames;
        public Vector2 flightPosition;
        public Vector2 flightTo;
        public Vector2 flightIncrement;
        public int flightSpeed;
        public bool flightFlip; // flips flight animation
        public int flightInterval; // flight timer intervals to adjust speed
        public int flightHeight; // how high to raise the sprite on Y axis
        public int flightAscend; // how high to raise the sprite on Y axis
        public int flightCeiling; // lowest frame for flight cycle
        public int flightFloor; // highest frame for flight cycle
        public int flightLast; // last frame for flight cycle (strike, land)

        // ============================= Special attack

        public int specialFrame;
        public int specialTimer;
        public NetBool netSpecialActive = new NetBool(false);
        public Texture2D specialTexture;
        public Dictionary<int, List<Rectangle>> specialFrames;
        public int specialThreshold;
        public int specialCeiling;
        public int specialFloor;
        public int specialInterval;
        public SpellHandle.schemes specialScheme;

        // ============================= Barrage attack

        public NetBool netBarrageActive = new NetBool(false);
        public int barrageThreshold;


        public Boss()
        {
        }

        public Boss(Vector2 vector, int CombatModifier, string name = "Boss", string template = "Pepper Rex")
          : base(template, new(vector.X * 64, vector.Y * 64))
        {
            
            realName.Set(name);
            localMonster = true;
            combatModifier = CombatModifier;

            //=================== base fields

            focusedOnFarmers = true;
            objectsToDrop.Clear();
            breather.Value = false;
            hideShadow.Value = true;

            //=================== reconfigurable fields

            tempermentActive = temperment.cautious;

            SetMode(2);
            LoadOut();

            Halt();
            idleTimer = 30;

        }

        protected override void initNetFields()
        {
            base.initNetFields();
            NetFields.AddField(realName, "realName");
            NetFields.AddField(netMode, "netMode");
            NetFields.AddField(netDirection, "netDirection");
            NetFields.AddField(netAlternative, "netAlternative");
            NetFields.AddField(netFlightActive, "netFlightActive");
            NetFields.AddField(netSpecialActive, "netSpecialActive");
            NetFields.AddField(netSweepActive, "newSweepActive");
        }

        public override Rectangle GetBoundingBox()
        {

            Vector2 position = Position;

            int netScale = netMode.Value > 5 ? netMode.Value - 4 : netMode.Value;

            return new Rectangle((int)position.X - 8 - (netScale * 4), (int)position.Y - 32 - flightHeight - (netScale * 8), 80 +(netScale * 4), 96 + (netScale * 8));

        }

        public virtual void SetMode(int mode)
        {

            netMode.Set(mode);

            switch (mode)
            {

                case 0: // small mode

                    MaxHealth = Math.Max(250, combatModifier * 50);

                    Health = MaxHealth;

                    DamageToFarmer = Math.Max(10, Math.Min(30, combatModifier * 1));

                    tempermentActive = temperment.aggressive;

                    abilities = 1;

                    break;

                case 1: // slightly bigger

                    MaxHealth = Math.Max(1000, combatModifier * 100);

                    Health = MaxHealth;

                    DamageToFarmer = Math.Max(20, Math.Min(40, combatModifier * 3));

                    tempermentActive = temperment.cautious;

                    abilities = 2;

                    break;

                default:
                case 2: // multiple bosses

                    MaxHealth = Math.Max(2000, combatModifier * 200);

                    Health = MaxHealth;

                    DamageToFarmer = Math.Max(20, Math.Min(40, combatModifier * 3));

                    tempermentActive = temperment.cautious;

                    abilities = 2;

                    break;

                case 3: // single boss

                    MaxHealth = Math.Max(4000, combatModifier * 400);

                    Health = MaxHealth;

                    DamageToFarmer = Math.Max(30, Math.Min(60, combatModifier * 5));

                    tempermentActive = temperment.aggressive;

                    abilities = 3;

                    break;

                case 4: // hard boss

                    MaxHealth = Math.Max(7500, combatModifier * 750);

                    Health = MaxHealth;

                    DamageToFarmer = Math.Max(40, Math.Min(80, combatModifier * 5));

                    tempermentActive = temperment.aggressive;

                    abilities = 3;

                    break;

                case 5: // chase mode

                    MaxHealth = Math.Max(750, combatModifier * 75);

                    Health = MaxHealth;

                    DamageToFarmer = Math.Max(20, Math.Min(50, combatModifier * 3));

                    cooldownTimer = 180;

                    cooldownActive = true;

                    tempermentActive = temperment.coward;

                    abilities = 2;

                    break;

            }

        }

        public virtual void RandomTemperment()
        {

            switch(new Random().Next(3))
            {

                case 1:

                    tempermentActive = temperment.cautious;

                    break;

                case 2:

                    tempermentActive = temperment.odd;

                    break;

                default:

                    tempermentActive = temperment.aggressive;

                    break;

            }

        }

        public virtual void LoadOut()
        {

            BaseWalk();

            BaseFlight();

            BaseSpecial();

            loadedOut = true;

        }

        public void BaseWalk()
        {
            
            characterTexture = MonsterHandle.MonsterTexture(realName.Value);

            walkCeiling = 3;

            walkFloor = 0;

            walkInterval = 9;

            gait = 2;

            idleFrames = FrameSeries(32, 32, 0, 0, 1);

            walkFrames = FrameSeries(32, 32);

            overHead = new(0, -128);

        }

        public void BaseFlight()
        {

            flightInterval = 9;

            flightSpeed = 12;

            flightAscend = 4;

            flightCeiling = 3;

            flightFloor = 0;

            flightLast = 2;

            flightTexture = characterTexture;

            flightFrames = walkFrames;

        }

        public virtual void BaseSpecial()
        {

            abilities = 2;

            cooldownInterval = 180;

            specialCeiling = 3;

            specialFloor = 0;

            reachThreshold = 64;

            safeThreshold = 544;

            sweepThreshold = 192;

            specialThreshold = 512;

            barrageThreshold = 640;

            specialInterval = 12;

            specialScheme = SpellHandle.schemes.fire;

            specialTexture = characterTexture;

            specialFrames = walkFrames;

            sweepSet = false;

            sweepInterval = 12;

            sweepTexture = characterTexture;

            sweepFrames = walkFrames;

        }

        public override void draw(SpriteBatch b, float alpha = 1f)
        {
            if (IsInvisible || !Utility.isOnScreen(Position, 128))
            {
                return;
            }

            Vector2 localPosition = getLocalPosition(Game1.viewport);

            float drawLayer = (float)StandingPixel.Y / 10000f;

            int adjustDirection = netDirection.Value == 3 ? 1 : netDirection.Value;

            DrawEmote(b, localPosition, drawLayer);

            b.Draw(characterTexture, new Vector2(localPosition.X - 96f, localPosition.Y - 192f), new Rectangle?(walkFrames[adjustDirection][walkFrame]), Color.White * 0.65f, 0.0f, new Vector2(0.0f, 0.0f), 4f, (netDirection.Value % 2 == 0 && netAlternative.Value == 3) || netDirection.Value == 3 ? (SpriteEffects)1 : 0, drawLayer);

        }

        public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
        {

            if (textAboveHeadTimer <= 0 || textAboveHead == null)
            {
                return;
            }
            Vector2 localPosition = getLocalPosition(Game1.viewport);

            SpriteText.drawStringWithScrollCenteredAt(b, textAboveHead, (int)localPosition.X + (int)overHead.X, (int)localPosition.Y + (int)overHead.Y, "", textAboveHeadAlpha, textAboveHeadColor, 1, (float)(Tile.Y * 64 / 10000.0 + 1.0 / 1000.0 + Tile.X / 10000.0), false);

        }

        public virtual void DrawEmote(SpriteBatch b, Vector2 localPosition, float drawLayer)
        {

            if (IsEmoting && !Game1.eventUp)
            {

                localPosition.Y -= 32 + Sprite.SpriteHeight * 4;

                b.Draw(Game1.emoteSpriteSheet, localPosition, new Rectangle?(new Rectangle(CurrentEmoteIndex * 16 % Game1.emoteSpriteSheet.Width, CurrentEmoteIndex * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, 0, drawLayer);

            }

        }

        public virtual Dictionary<int, List<Rectangle>> FrameSeries(int width, int height, int startX = 0, int startY = 0, int length = -1)
        {

            Dictionary<int, List<Rectangle>> walkFrames = new();

            if(length == -1)
            {

                length = walkCeiling + 1;

            }

            foreach (KeyValuePair<int, int> keyValuePair in new Dictionary<int, int>()
            {
                [0] = 2,
                [1] = 1,
                [2] = 0,
                [3] = 3
            })
            {
                walkFrames[keyValuePair.Key] = new List<Rectangle>();

                for (int index = 0; index < length; index++)
                {
                    
                    Rectangle rectangle = new(0 + startX, 0 + startY, width, height);
                    
                    rectangle.X += width * index;
                    
                    rectangle.Y += height * keyValuePair.Value;
                    
                    walkFrames[keyValuePair.Key].Add(rectangle);
                
                }

            }

            return walkFrames;

        }

        //=================== overriden base fields

        public override List<Item> getExtraDropItems()
        {
            return new List<Item>();
        }

        public override void onDealContactDamage(Farmer who)
        {

            if ((who.health + who.buffs.Defense) - DamageToFarmer < 10)
            {

                who.health = (DamageToFarmer - who.buffs.Defense) + 10;

                Mod.instance.CriticalCondition();

            }

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
                ModUtility.AnimateDeathSpray(currentLocation,Position,Color.Gray);

            }

            if (talkTimer < (int)Game1.currentGameTime.TotalGameTime.TotalSeconds)
            {

                DialogueData.DisplayText(this, 3, 0, realName.Value);

                talkTimer = (int)Game1.currentGameTime.TotalGameTime.TotalSeconds + 6;

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

            ResetActives();

            netHaltActive.Set(true);

            idleTimer = 60;

        }

        public virtual void ResetActives()
        {

            moveDirection = 2;

            netDirection.Set(2);

            altDirection = 1;

            netAlternative.Set(1);

            ClearIdle();

            ClearMove();

            ClearSpecial();

        }

        public virtual void ResetFrames()
        {

            idleFrame = 0;

            walkFrame = 0;

            flightFrame = 0;

            sweepFrame = 0;

            specialFrame = 0;

        }

        public virtual void ClearIdle()
        {

            if (netHaltActive.Value)
            {

                netHaltActive.Set(false);

            }

            idleFrame = 0;

            idleTimer = 540;

        }

        public virtual void ClearMove()
        {

            if (netFlightActive.Value)
            {

                netFlightActive.Set(false);

            }

            flightTimer = 0;

            flightFrame = 0;

            if (netSweepActive.Value)
            {

                netSweepActive.Set(false);

            }

            sweepTimer = 0;

            sweepFrame = 0;

            walkFrame = 0;

            walkTimer = 0;

            followIncrement = Vector2.Zero;

        }
        
        public virtual void ClearSpecial()
        {
            if (netSpecialActive.Value)
            {

                netSpecialActive.Set(false);

            }

            specialTimer = 0;

            specialFrame = 0;

            //cooldownTimer = 0;

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

        public virtual void LookAtFarmer()
        {

            List<Farmer> targets = ModUtility.FarmerProximity(currentLocation, new() { Position, }, 20*64);

            if (targets.Count > 0)
            {

                SetDirection(targets.First().Position);

            }

        }

        public virtual void TalkSmack()
        {

            if (talkTimer > (int)Game1.currentGameTime.TotalGameTime.TotalSeconds)
            {
                return;
            }

            int talk = 1;

            if (tempermentActive == temperment.coward)
            {
                talk = 2;
            }

            if (tempermentActive == temperment.aggressive)
            {
                talk = 3;
            }

            DialogueData.DisplayText(this, 1, talk, realName.Value);

            talkTimer = (int)Game1.currentGameTime.TotalGameTime.TotalSeconds + 6;

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

                if (stunTime.Value > 0)
                {

                    stunTime.Set(stunTime.Value - (int)time.ElapsedGameTime.TotalMilliseconds);

                    if (!netHaltActive.Value)
                    {
                        Halt();
                        idleTimer = (int)stunTime.Value;
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

                UpdateMultiplayer();

                return;

            }

            ChooseBehaviour();

            if (netSweepActive.Value)
            {

                UpdateSweep();

            }

            if (netFlightActive.Value)
            {

                UpdateFlight();

            }

            if (netSpecialActive.Value)
            {

                UpdateSpecial();
            
            }

            if (cooldownActive)
            {

                UpdateCooldown();

            }

            if (netHaltActive.Value)
            {

                UpdateHalt();

            }

            UpdateWalk();

        }

        public void UpdateMultiplayer()
        {

            if (netSweepActive.Value)
            {

                sweepTimer--;

                if (sweepTimer <= 0)
                {

                    sweepFrame++;

                    sweepTimer = sweepInterval;

                }

                return;

            }
            else
            {
                sweepFrame = 0;

                sweepTimer = flightInterval;

            }

            if (netFlightActive.Value)
            {

                flightTimer--;

                if (flightTimer <= 0)
                {

                    flightFrame++;

                    if (flightFrame > flightCeiling)
                    {
                        flightFrame = flightFloor;

                    }

                    flightTimer = flightInterval;

                }

                flightHeight++;

                return;

            }
            else
            {

                if(flightHeight > 0)
                {

                    flightHeight--;

                }

                flightFrame = 0;

                flightTimer = sweepInterval;

            }

            if (netSpecialActive.Value)
            {

                specialTimer--;

                if (specialTimer <= 0)
                {

                    specialFrame++;

                    if (specialFrame > specialCeiling)
                    {
                        specialFrame = specialFloor;
                    }

                    specialTimer = specialInterval;

                }

                return;

            }
            else
            {
                specialFrame = 0;

                specialTimer = specialInterval;

            }

            if (setPosition != Position || netDirection.Value != moveDirection || netAlternative.Value != altDirection)
            {

                setPosition = Position;

                moveDirection = netDirection.Value;

                altDirection = netAlternative.Value;

                walkTimer--;

                if (walkTimer <= 0)
                {

                    walkFrame++;

                    if (walkFrame > walkCeiling)
                    {
                        walkFrame = walkFloor;
                    }

                    walkTimer = walkInterval;

                    stationaryTimer = walkInterval * 3;

                }

                return;

            }

            if (stationaryTimer > 0)
            {

                stationaryTimer--;

                if (stationaryTimer == 0)
                {

                    walkFrame = 0;

                    walkTimer = 0;

                }

            }

        }

        public void UpdateHalt()
        {

            idleTimer--;

            if (idleTimer % 20 == 0)
            {

                LookAtFarmer();

            }

            if (idleTimer <= 0)
            {

                ClearIdle();

            }

        }

        public int WalkSpeed()
        {
            return gait;

        }

        public void UpdateWalk()
        {

            if (flightHeight > 0)
            {

                flightHeight--;

            }

            followTimer--;

            if (followIncrement == Vector2.Zero)
            {

                return;

            }

            Position += (followIncrement * WalkSpeed());

            walkTimer++;

            if(walkTimer != walkInterval)
            {

                return;

            }

            walkFrame++;

            if (walkFrame > walkCeiling)
            {

                walkFrame = walkFloor;

            }

            walkTimer = 0;

        }

        public void UpdateSweep()
        {

            sweepTimer--;

            if (sweepTimer <= 0)
            {

                ClearMove();

                SetCooldown(2);

            }
            else
            {

                Position += sweepIncrement;

                if (sweepTimer == sweepInterval)
                {

                    List<Farmer> targets = ModUtility.FarmerProximity(currentLocation, new() { Position, }, 128f);

                    ModUtility.DamageFarmers(targets, (int)(damageToFarmer.Value * 1.5), this, true);

                }
                
                if (sweepTimer % sweepInterval == 0)
                {

                    sweepFrame++;

                    if (sweepFrame == sweepFrames[0].Count)
                    {

                        sweepFrame = 0;

                    }

                }

            }

        }

        public void UpdateFlight()
        {

            flightTimer--;

            if (flightTimer <= 0)
            {

                ClearMove();

            }
            else
            {

                if (flightAscend != -1)
                {

                    if (flightHeight < (16 * flightAscend) && flightTimer > 16)
                    {

                        flightHeight += flightAscend;

                    }
                    else if (flightHeight > 0 && flightTimer <= 16)
                    {

                        flightHeight -= flightAscend;

                    }

                }

                Position += flightIncrement;

                if (flightTimer % flightInterval != 0)
                {

                    return;

                }

                if (flightTimer == flightInterval)
                {

                    PerformSweep();

                    flightFrame = flightLast;

                }
                else
                {

                    int next = flightFrame + 1;

                    if (next > flightCeiling)
                    {

                        next = flightFloor;

                    }

                    if (flightFrames[netDirection.Value].Count == next)
                    {

                        next = 0;

                    }

                    flightFrame=next;

                }

            }

        }

        public virtual void UpdateSpecial()
        {

            specialTimer--;

            if (specialTimer <= 0 || Game1.player.IsBusyDoingSomething())
            {

                ClearSpecial();

            }
            else
            {

                if (specialTimer % specialInterval == 0)
                {

                    specialFrame++;

                    if (specialFrame > specialCeiling)
                    {

                        specialFrame = specialFloor;

                    }

                }

            }

        }

        public void SetCooldown(int factor)
        {

            cooldownActive = true;

            int netScale = netMode.Value > 5 ? netMode.Value - 4 : netMode.Value;

            cooldownTimer = (int)((cooldownInterval + new Random().NextDouble() * cooldownInterval) * (1 - (0.1 * netScale)) * factor);

        }

        public void UpdateCooldown()
        {

            cooldownTimer--;

            if (cooldownTimer == cooldownInterval * 0.5 && new Random().Next(3) == 0)
            {

                TalkSmack();

            }

            if (cooldownTimer <= 0)
            {

                cooldownActive = false;

            }

        }

        public virtual bool ChangeBehaviour()
        {

            if (netHaltActive.Value)
            {

                return false;

            }

            if (netSpecialActive.Value)
            {

                return false;

            }

            if (netSweepActive.Value)
            {

                return false;

            }

            if (netFlightActive.Value)
            {

                return false;

            }

            if (followTimer > 0)
            {

                return false;

            }

            return true;

        }

        public virtual void ChooseBehaviour()
        {

            if (!ChangeBehaviour())
            {

                return;

            }

            Random random = new Random();

            List<Farmer> targets = ModUtility.FarmerProximity(currentLocation, new() { Position, }, 20*64);

            if (targets.Count == 0)
            {

                PerformRandom();

                return;

            }

            Farmer farmer = targets.First();

            float threshold = Vector2.Distance(Position, farmer.Position);

            ResetActives();

            SetDirection(farmer.Position);

            if (!cooldownActive)
            {

                switch (random.Next(abilities))
                {

                    case 2:

                        if(threshold > barrageThreshold)
                        {
       
                            PerformFlight();

                        }
                        else if (threshold <= barrageThreshold)
                        {

                            PerformBarrage();

                        }


                        return;

                    case 1:
                        
                        if (threshold > specialThreshold)
                        {

                            PerformFlight();

                        }
                        else if (threshold <= specialThreshold)
                        {

                            PerformSpecial(farmer.Position);

                        }


                        return;

                    default:

                        if(threshold <= sweepThreshold)
                        {
                            
                            if (PerformSweep())
                            {

                                break;

                            }

                        }

                        PerformFlight();

                        if (netFlightActive.Value)
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
                
                if (new Random().Next(3) == 0)
                {
                    netHaltActive.Value = true;

                    idleTimer = 120;

                    LookAtFarmer();

                    TalkSmack();

                    return;

                }

                PerformFollow(position);


            }
            else if(tempermentActive == temperment.odd)
            {

                switch(new Random().Next(4))
                {

                    case 0: PerformRandom(); break;

                    case 1: PerformFollow(position); break;

                    case 2: PerformRetreat(position); break;

                    case 3: PerformCircle(position); break;

                }

            }
            else
            {

                if (new Random().Next(4) == 0)
                {

                    netHaltActive.Value = true;

                    LookAtFarmer();

                    idleTimer = 90;

                    TalkSmack();

                    return;

                }

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

        public virtual bool PerformSweep()
        {

            if (!sweepSet) {  return false; }

            List<Farmer> targets = ModUtility.FarmerProximity(currentLocation, new() { Position, }, 192);

            if (targets.Count > 0)
            {

                Vector2 targetFarmer = targets.First().Position;

                SetDirection(targetFarmer);

                sweepTimer = sweepInterval * sweepFrames[0].Count;

                sweepIncrement = new((targetFarmer.X - Position.X) / sweepTimer, (targetFarmer.Y - Position.Y) / sweepTimer);

                netSweepActive.Set(true);

                return true;

            }

            return false;

        }
        public override void shedChunks(int number, float scale)
        {
            int size = 0;
            if (walkFrames[0][0].Width > 32) { size++; }
            ModUtility.AnimateImpact(currentLocation, Position, size, 2, "FlashBang", 50);
        }

        public virtual void PerformFlight(int adjust = 0)
        {

            int destination = FlightDestination(adjust);
            
            if (destination == 0)
            {
                return;
            }

            SetCooldown(1);

            netFlightActive.Set(true);

            flightTimer = flightSpeed * destination;

            flightIncrement = new((flightTo.X - Position.X) / flightTimer, (flightTo.Y - Position.Y) / flightTimer);

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
                
                Vector2 tileLocation = Tile;
                
                Vector2 neighbour = new(tileLocation.X + vectorMultiple.X, tileLocation.Y + vectorMultiple.Y);//Vector2.op_Addition(Tile, Vector2.op_Multiply(vector2, (float)num2));
                
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

        public virtual List<Vector2> BlastZero(int randomisation = -1)
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

        public virtual void PerformSpecial(Vector2 target)
        {

            specialTimer = (specialCeiling + 1) * specialInterval;

            netSpecialActive.Set(true);

            SetCooldown(1);

            SpellHandle fireball = new(currentLocation, target, GetBoundingBox().Center.ToVector2(), 128, DamageToFarmer);

            fireball.type = SpellHandle.spells.missile;

            fireball.scheme = specialScheme;

            fireball.display = displays.Impact;

            fireball.threshold = specialThreshold;

            fireball.boss = this;

            Mod.instance.spellRegister.Add(fireball);

        }

        public void PerformFollow(Vector2 target)
        {

            SetDirection(target);

            float distance = Vector2.Distance(Position, target);

            if (distance > reachThreshold)
            {

                followIncrement = (target - Position) / distance;

            }

            followTimer = 60;
 
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

                netDirection.Set(moveDirection);

                netAlternative.Set(altDirection);

                netFlightActive.Set(true);

                flightTimer = flightInterval * destination;

                flightIncrement = (flightTo - Position) / flightTimer;

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

            specialTimer = (specialCeiling + 1) * specialInterval / 2;

            netSpecialActive.Set(true);

            SetCooldown(2);

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

                Vector2 impact = newVector * 64;

                SpellHandle missile = new(currentLocation, impact, impact, 256, DamageToFarmer);

                missile.type = SpellHandle.spells.ballistic;

                missile.display = SpellHandle.displays.Impact;

                missile.scheme = specialScheme;

                missile.boss = this;

                Mod.instance.spellRegister.Add(missile);

            }

        }

    }

}