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
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewDruid.Cast;
using StardewDruid.Data;
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


namespace StardewDruid.Monster
{
    public class Boss : StardewValley.Monsters.Monster
    {

        public NetString realName = new NetString();

        public int moveDirection;
        public int altDirection;

        public bool localMonster;
        public bool loadedOut;
        public Texture2D characterTexture;
        public int combatModifier;
        public NetInt netDirection = new NetInt(0);
        public NetInt netAlternative = new NetInt(0);

        // ============================= Differentiation


        public int baseMode;
        public int baseJuice;
        public int basePulp;
        public int threat;
        public NetInt netMode = new NetInt(0);
        public NetBool netPosturing = new NetBool(false);

        public NetInt netScheme = new(0);
        public Dictionary<int, List<Rectangle>> schemeFrames = new();

        public enum difficulty
        {
            basic,
            medium,
            hard,
            miniboss,
            boss,
            thief,
        }

        public enum temperment
        {

            cautious,
            coward,
            aggressive,
            odd,
            random,
            ranged,
            group,

        }

        public temperment tempermentActive;

        // ============================= Behaviour

        public Dictionary<int, List<Rectangle>> idleFrames = new();
        public int idleTimer;
        public NetBool netHaltActive = new NetBool(false);
        public int idleFrame;
        public NetBool netAlert = new NetBool(false);

        public NetBool netWoundedActive = new NetBool(false);
        public Dictionary<int, List<Rectangle>> woundedFrames = new();
        public bool setWounded;

        // ============================= Follow behaviour

        public Vector2 pushVector;
        public int pushTimer;

        public Vector2 followIncrement;
        public int followTimer;
        public float gait;
        //public int reachThreshold;
        //public int safeThreshold;

        public Dictionary<int, List<Rectangle>> walkFrames = new();
        public int walkFrame;
        public int walkTimer;
        public bool walkSwitch;
        public bool walkSide;
        public int walkLeft;
        public int walkRight;
        public int walkInterval;
        public Vector2 setPosition;
        public int stationaryTimer;

        public bool cooldownActive;
        public int cooldownTimer;
        public int cooldownInterval;
        public int talkTimer;
        public Vector2 overHead;

        public int hoverHeight;
        public int hoverInterval;
        public int hoverIncrements;
        public float hoverElevate;
        public int hoverFrame;

        // ============================= Sweep

        public bool sweepSet;
        public int sweepFrame;
        public int sweepTimer;
        public NetBool netSweepActive = new NetBool(false);
        public Dictionary<int, List<Rectangle>> sweepFrames = new();
        public int sweepInterval;
        public Vector2 sweepIncrement;
        //public int sweepThreshold;

        // ============================= Flight

        public NetBool netFlightActive = new NetBool(false);
        public NetBool netSmashActive = new NetBool(false);
        public Dictionary<int, List<Rectangle>> flightFrames = new();
        public bool smashSet;
        public Dictionary<int, List<Rectangle>> smashFrames = new();

        public bool flightSet;
        public int flightFrame; // current animation frame of flight
        public int flightTimer; // current tick of flight
        public int flightInterval; // flight timer intervals to adjust frame speed
        public int flightTotal; // amount of ticks of full flight
        public int flightDefault; // determines whether to fly at, over or near target
        public Vector2 flightFrom; // origin of flight path
        public Vector2 flightTo; // destination of flight path
        public Vector2 flightIncrement; // how much to shift position toward destination
        public int flightSpeed; // how fast to travel during flight
        public bool flightFlip; // flips flight animation
        public int flightPeak; // peak height during flight
        public int flightHeight; // tracked height during flight
        public NetInt netFlightProgress = new NetInt(0);
        public int flightSegment;
        public int trackFlightProgress;

        // ============================= Special attack

        public bool specialSet;
        public int specialFrame;
        public int specialTimer;
        public NetBool netSpecialActive = new NetBool(false);
        public Dictionary<int, List<Rectangle>> specialFrames = new();
        public Dictionary<int, List<Rectangle>> channelFrames = new();
        //public int specialThreshold;
        public int specialCeiling;
        public int specialFloor;
        public int specialInterval;

        // ============================= Barrage attack

        public bool channelSet;
        public NetBool netChannelActive = new NetBool(false);
        public int channelCeiling;
        public int channelFloor;
        //public int barrageThreshold;


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
            DamageToFarmer = 0;

            //=================== reconfigurable fields

            tempermentActive = temperment.cautious;

            baseMode = 2;
            baseJuice = 4;
            basePulp = 25;
            cooldownInterval = 180;

            LoadOut();

            SetMode(baseMode);

            Halt();
            idleTimer = 30;

        }

        protected override void initNetFields()
        {
            base.initNetFields();
            NetFields.AddField(realName, "realName");
            NetFields.AddField(netMode, "netMode");
            NetFields.AddField(netPosturing, "netPosturing");
            NetFields.AddField(netHaltActive, "netHaltActive");
            NetFields.AddField(netDirection, "netDirection");
            NetFields.AddField(netAlternative, "netAlternative");
            NetFields.AddField(netFlightActive, "netFlightActive");
            NetFields.AddField(netFlightProgress, "netFlightProgress");
            NetFields.AddField(netSpecialActive, "netSpecialActive");
            NetFields.AddField(netChannelActive, "netChannelActive");
            NetFields.AddField(netSweepActive, "netSweepActive");
            NetFields.AddField(netWoundedActive, "netWoundedActive");
            NetFields.AddField(netScheme, "netScheme");

        }

        public override Rectangle GetBoundingBox()
        {

            Vector2 position = Position;

            int netScale = (int)GetScale();

            Rectangle box =  new((int)position.X - 8 - netScale * 4, (int)position.Y - 32 - netScale * 8, 80 + netScale * 4, 96 + netScale * 8);

            if(netFlightActive.Value || netSmashActive.Value)
            {

                box.Y -= flightHeight;

            }

            return box;

        }

        public virtual void SetMode(int mode)
        {

            netMode.Set(mode);

            DamageToFarmer = 0;

            switch (mode)
            {

                case 0: // small mode

                    MaxHealth = combatModifier * basePulp;

                    Health = MaxHealth;

                    tempermentActive = temperment.aggressive;

                    experienceGained.Set(10);

                    break;

                case 1: // slightly bigger

                    MaxHealth = combatModifier * basePulp * 3;

                    Health = MaxHealth;

                    tempermentActive = temperment.cautious;

                    experienceGained.Set(20);

                    break;

                default:
                case 2: // multiple bosses

                    MaxHealth = combatModifier * basePulp * 8;

                    Health = MaxHealth;

                    tempermentActive = temperment.cautious;

                    experienceGained.Set(50);

                    break;

                case 3: // single boss

                    MaxHealth = combatModifier * basePulp * 16;

                    Health = MaxHealth;

                    tempermentActive = temperment.aggressive;

                    experienceGained.Set(100);

                    break;

                case 4: // hard boss

                    MaxHealth = combatModifier * basePulp * 30;

                    Health = MaxHealth;

                    tempermentActive = temperment.aggressive;

                    experienceGained.Set(200);

                    break;

            }

            GetThreat();

        }

        public virtual int GetThreat()
        {

            switch (netMode.Value)
            {

                case 0: // small mode

                    threat = Math.Max(baseJuice * 2, (int)((float)combatModifier * 0.6f));

                    break;

                case 1: // slightly bigger

                    threat = Math.Max(baseJuice * 3, (int)((float)combatModifier * 0.8f));

                    break;

                default:
                case 2: // multiple bosses

                    threat = Math.Max(baseJuice * 4, combatModifier);

                    break;

                case 3: // single boss

                    threat = Math.Max(baseJuice * 6, (int)(combatModifier * 1.2f));

                    break;

                case 4: // hard boss

                    threat = Math.Max(baseJuice * 8, (int)(combatModifier * 1.6f));

                    break;

                case 5: // mega boss

                    threat = Math.Max(baseJuice * 10, (int)(combatModifier * 2f));

                    break;

            }

            threat = Math.Min(threat, (Game1.player.maxHealth / 2));

            return threat;

        }

        public virtual void RandomTemperment()
        {

            switch (Mod.instance.randomIndex.Next(3))
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

            walkInterval = 9;

            gait = 2f;

            idleFrames = FrameSeries(32, 32,0,0,1);

            walkFrames = FrameSeries(32, 32,0,0,7);

            overHead = new(0, -128);

        }

        public void BaseFlight()
        {

            flightInterval = 9;

            flightSpeed = 8;

            flightPeak = 192;

            flightFrames = walkFrames;

            flightDefault = 2;

            smashFrames = flightFrames;

        }

        public virtual void BaseSpecial()
        {

            specialCeiling = 6;

            specialFloor = 1;

            channelCeiling = 6;

            channelFloor = 1;

            specialInterval = 12;

            specialFrames = walkFrames;

            channelFrames = walkFrames;

            sweepInterval = 12;

            sweepFrames = walkFrames;

        }

        public virtual float GetScale()
        {

            return 1 + netMode.Value;

        }

        public override void draw(SpriteBatch b, float alpha = 1f)
        {
            if (IsInvisible || !Utility.isOnScreen(Position, 128))
            {
                return;
            }

            Vector2 localPosition = getLocalPosition(Game1.viewport);

            float drawLayer = StandingPixel.Y / 10000f;

            int adjustDirection = netDirection.Value == 3 ? 1 : netDirection.Value;

            DrawEmote(b, localPosition, drawLayer);

            b.Draw(characterTexture, new Vector2(localPosition.X - 96f, localPosition.Y - 192f), new Rectangle?(walkFrames[adjustDirection][walkFrame]), Color.White * 0.65f, 0.0f, new Vector2(0.0f, 0.0f), 4f, netDirection.Value % 2 == 0 && netAlternative.Value == 3 || netDirection.Value == 3 ? (SpriteEffects)1 : 0, drawLayer);

        }

        public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
        {
            Vector2 localPosition = getLocalPosition(Game1.viewport);

            DrawTextAboveHead(b,localPosition);

        }

        public virtual void DrawEmote(SpriteBatch b, Vector2 localPosition, float drawLayer)
        {

            if (IsEmoting && !Game1.eventUp)
            {

                localPosition.Y -= 32 + Sprite.SpriteHeight * 4;

                b.Draw(Game1.emoteSpriteSheet, localPosition, new Rectangle?(new Rectangle(CurrentEmoteIndex * 16 % Game1.emoteSpriteSheet.Width, CurrentEmoteIndex * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, 0, drawLayer);

            }

        }

        public virtual void DrawTextAboveHead(SpriteBatch b, Vector2 localPosition)
        {

            if (textAboveHeadTimer <= 0 || textAboveHead == null)
            {
                return;
            }

            SpriteText.drawStringWithScrollCenteredAt(b, textAboveHead, (int)localPosition.X + (int)overHead.X, (int)localPosition.Y + (int)overHead.Y, "", textAboveHeadAlpha, textAboveHeadColor, 1, (float)(Tile.Y * 64 / 10000.0 + 1.0 / 1000.0 + Tile.X / 10000.0), false);

        }

        public virtual Dictionary<int, List<Rectangle>> FrameSeries(int width, int height, int startX = 0, int startY = 0, int length = 6, Dictionary<int, List<Rectangle>> frames = null)
        {

            if (frames == null)
            {
                frames = new()
                {
                    [0] = new(),
                    [1] = new(),
                    [2] = new(),
                    [3] = new(),
                };
            }

            Dictionary<int, int> normalSequence = new()
            {
                [0] = 2,
                [1] = 1,
                [2] = 0,
                [3] = 3
            };

            foreach (KeyValuePair<int, int> keyValuePair in normalSequence)
            {

                for (int index = 0; index < length; index++)
                {

                    Rectangle rectangle = new(startX, startY, width, height);

                    rectangle.X += width * index;

                    rectangle.Y += height * keyValuePair.Value;

                    frames[keyValuePair.Key].Add(rectangle);

                }

            }

            return frames;

        }

        public virtual SpellHandle.effects IsCursable(SpellHandle.effects effect = SpellHandle.effects.knock)
        {

            if(effect == SpellHandle.effects.morph)
            {

                return SpellHandle.effects.blind;

            }

            if (effect == SpellHandle.effects.daze)
            {

                return SpellHandle.effects.blind;

            }

            if (effect == SpellHandle.effects.knock)
            {

                return SpellHandle.effects.blind;

            }

            if (netMode.Value >= 3)
            {

                return SpellHandle.effects.blind;

            }

            return effect;

        }
        
        //=================== overriden base fields

        public override List<Item> getExtraDropItems()
        {
            
            return new List<Item>();
        
        }

        public override int takeDamage(int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
        {

            if (netPosturing.Value || netWoundedActive.Value)
            {
                
                if (netChannelActive.Value)
                {

                    ClearSpecial();

                }

                return 0;

            }

            if (!Context.IsMainPlayer)
            {

                damage = Math.Min(damage, Health - 2);

                if (damage <= 0)
                {

                    return -1;

                }

                Health -= damage;

                return damage;

            }

            damage = damage < 1 ? 1 : damage;

            Health -= damage;

            if (ValidPush())
            {

                pushVector = new(xTrajectory, yTrajectory);

                pushTimer = 4;

            }

            if (Health <= 0)
            {

                if (setWounded)
                {

                    ResetActives();

                    netWoundedActive.Set(true);

                    return damage;

                }

                deathIsNoEscape();

            }

            if (talkTimer < (int)Game1.currentGameTime.TotalGameTime.TotalSeconds && textAboveHeadTimer <= 0)
            {

                DialogueData.DisplayText(this, 3, 0);

                talkTimer = (int)Game1.currentGameTime.TotalGameTime.TotalSeconds + 6;

            }

            return damage;

        }

        public virtual void deathIsNoEscape()
        {

            Microsoft.Xna.Framework.Rectangle box = GetBoundingBox();

            SpellHandle death = new(new(box.Center.X, box.Top), 64 + (64* (int)GetScale()), IconData.impacts.death, new());

            Mod.instance.spellRegister.Add(death);

        }

        public override void shedChunks(int number, float scale)
        {

            float size = 2f;

            if(walkFrames.Count > 0)
            {

                if (walkFrames[0][0].Width > 32) { size += 1f; }

            }

            Mod.instance.iconData.ImpactIndicator(currentLocation, Position, IconData.impacts.flashbang, size, new() { frame = 2, interval = 50, });

        }

        public override bool isInvincible()
        {

            if (netChannelActive.Value)
            {

                return false;

            }

            if (netPosturing.Value)
            {

                return true;

            }

            if(invincibleCountdown > 0)
            {

                return true;

            }

            return false;

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

                netAlert.Set(false);

            }

            idleFrame = 0;

            idleTimer = 540;

        }

        public virtual void ClearMove()
        {

            if (netFlightActive.Value)
            {

                netFlightActive.Set(false);
                
                netFlightProgress.Set(0);
            
            }

            if (netSmashActive.Value)
            {

                netSmashActive.Set(false);
                
                netFlightProgress.Set(0);
            
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

            if (netChannelActive.Value)
            {

                netChannelActive.Set(false);

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

            if(currentLocation == null)
            {

                return;

            }

            List<Farmer> targets = ModUtility.FarmerProximity(currentLocation, new() { Position, }, 15 * 64);

            if (targets.Count > 0)
            {

                if (!netAlert.Value)
                {
                    
                    netAlert.Set(true);

                }

                SetDirection(targets.First().Position);

            }
            else
            {
                if (netAlert.Value)
                {

                    netAlert.Set(false);

                }

            }

        }

        public virtual void TalkSmack()
        {

            if (talkTimer > (int)Game1.currentGameTime.TotalGameTime.TotalSeconds || textAboveHeadTimer > 0)
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

            DialogueData.DisplayText(this, 1, talk);

            talkTimer = (int)Game1.currentGameTime.TotalGameTime.TotalSeconds + 6;

        }

        public virtual bool baseupdate(GameTime time, GameLocation location)
        {
            
            if (localMonster)
            {

                currentLocation = location;

            }

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
                    {

                        textAboveHeadAlpha = Math.Min(1f, textAboveHeadAlpha + 0.1f);

                    }
                    else
                    {

                        float newAlpha = textAboveHeadAlpha - 0.04f;

                        textAboveHeadAlpha = newAlpha < 0f ? 0f : newAlpha;

                    }

                }

            }

            updateGlow();

            updateEmote(time);

            updateFaceTowardsFarmer(time, location);

            if (localMonster)
            {

                if (stunTime.Value > 0)
                {

                    stunTime.Set(stunTime.Value - (int)time.ElapsedGameTime.TotalMilliseconds);

                    if (!netHaltActive.Value)
                    {
                        
                        Halt();
                        
                        idleTimer = stunTime.Value;
                    
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

            if (netWoundedActive.Value)
            {

                return;

            }

            if (!localMonster)
            {

                UpdateMultiplayer();

                return;

            }

            ChooseBehaviour();

            bool notBusy = true;

            if (netSweepActive.Value)
            {

                UpdateSweep();

                notBusy = false;

            }

            if (netFlightActive.Value || netSmashActive.Value)
            {

                UpdateFlight();

                notBusy = false;

            }

            if (netSpecialActive.Value || netChannelActive.Value)
            {

                UpdateSpecial();

                notBusy = false;

            }

            if (cooldownActive && notBusy)
            {

                UpdateCooldown();

            }

            if (netHaltActive.Value)
            {

                UpdateHalt();

            }

            UpdatePush();

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

            if (netFlightActive.Value || netSmashActive.Value)
            {

                if (netFlightProgress.Value != trackFlightProgress)
                {

                    flightFrame = 0;

                    trackFlightProgress = netFlightProgress.Value;

                    flightTimer = flightInterval;

                }

                flightTimer--;

                if (flightTimer <= 0)
                {

                    flightFrame++;

                    flightTimer = flightInterval;

                }

                if (netFlightProgress.Value == 0 && flightHeight <= flightPeak)
                {

                    flightHeight += 2;

                }
                else if (netFlightProgress.Value == 2 && flightHeight > 0)
                {

                    flightHeight -= Math.Min(flightHeight, 2);

                }

                return;

            }
            else
            {

                if (flightHeight > 0)
                {

                    flightHeight -= Math.Min(flightHeight, 2);

                }

                flightFrame = 0;

                flightTimer = sweepInterval;

            }

            if (netSpecialActive.Value || netChannelActive.Value)
            {

                specialTimer--;

                if (specialTimer <= 0)
                {

                    specialFrame++;

                    if (netSpecialActive.Value)
                    {

                        if (specialFrame > specialCeiling)
                        {
                            specialFrame = specialFloor;
                        }

                    }

                    if (netChannelActive.Value)
                    {

                        if (specialFrame > channelCeiling)
                        {
                            specialFrame = channelFloor;
                        }

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


            if (hoverInterval > 0)
            {

                hoverHeight++;

                int heightLimit = (hoverIncrements * hoverInterval);

                if (hoverHeight > heightLimit)
                {
                    hoverHeight -= (heightLimit * 2);
                }

                if (Math.Abs(hoverHeight) % hoverInterval == 0)
                {

                    hoverFrame++;

                    if (hoverFrame >= IdleCount())
                    {

                        hoverFrame = 0;

                    }

                }

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

                    if (walkFrame >= WalkCount())
                    {

                        walkFrame = 1;

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

        public virtual void UpdateHalt()
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

        public float WalkSpeed()
        {

            return gait + (0.25f * (GetScale() - 1));

        }

        public virtual bool ValidPush()
        {

            if(netMode.Value <= 2)
            {

                return true;

            }

            return false;

        }

        public void UpdatePush()
        {

            if(pushVector != Vector2.Zero)
            {

                if(pushTimer <= 0)
                {

                    pushVector = Vector2.Zero;

                    return;

                }

                Vector2 increment = pushVector * (0.1f * pushTimer);

                Position += increment;

                pushTimer--;

            }

        }

        public void UpdateWalk()
        {

            if(hoverInterval > 0)
            {

                hoverHeight++;

                int heightLimit = (hoverIncrements * hoverInterval);

                if (hoverHeight > heightLimit)
                {
                    hoverHeight -= (heightLimit * 2);
                }

                if(Math.Abs(hoverHeight) % hoverInterval == 0)
                {

                    hoverFrame++;

                    if (hoverFrame >= IdleCount())
                    {

                        hoverFrame = 0;

                    }

                }

            }

            if (!netFlightActive.Value)
            {

                if (flightHeight > 0)
                {

                    flightHeight--;

                }

            }

            followTimer--;

            if (followIncrement == Vector2.Zero)
            {

                return;

            }

            Position += followIncrement * WalkSpeed();

            walkTimer--;

            if (walkTimer > 0)
            {

                return;

            }

            walkFrame++;

            int right = 1 + ((WalkCount() - 1) / 2);

            if (walkSwitch)
            {

                if (walkFrame == 1)
                {

                    if (walkSide)
                    {

                        walkFrame = right;

                    }

                }

                if (walkFrame == right)
                {

                    walkSide = false;

                }

            }

            if (walkFrame >= WalkCount())
            {

                walkFrame = 1;

                walkSide = true;

            }

            walkTimer = walkInterval;

        }

        public void UpdateSweep()
        {

            sweepTimer--;

            if (sweepTimer <= 0)
            {

                ClearMove();

            }
            else
            {

                Position += sweepIncrement;

                if (sweepTimer == sweepInterval * 2)
                {

                    ConnectSweep();

                }

                if (sweepTimer % sweepInterval == 0)
                {

                    sweepFrame++;

                    if (sweepFrame == SweepCount())
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

                FlightAscension();

                Position += flightIncrement;

                if (flightTimer % flightSegment != 0)
                {

                    return;

                }

                flightFrame++;

                if (netFlightActive.Value)
                {

                    if (flightTimer + (flightSegment * FlightCount()) <= flightTotal)
                    {

                        if (netFlightProgress.Value != 1)
                        {

                            netFlightProgress.Set(1);

                            flightFrame = 0;

                        }

                    }

                    if (flightTimer <= (flightSegment * FlightCount(2)))
                    {

                        if (netFlightProgress.Value != 2)
                        {

                            netFlightProgress.Set(2);

                            flightFrame = 0;

                        }

                    }

                }

                if (netSmashActive.Value)
                {

                    if (flightTimer + (flightSegment * SmashCount()) <= flightTotal)
                    {

                        if (netFlightProgress.Value != 1)
                        {

                            netFlightProgress.Set(1);

                            flightFrame = 0;

                        }

                    }

                    if (flightTimer <= (flightSegment * SmashCount(2)))
                    {

                        if (netFlightProgress.Value != 2)
                        {

                            netFlightProgress.Set(2);

                            flightFrame = 0;

                        }

                    }

                    if (flightTimer == flightSegment)
                    {

                        ConnectSweep();

                    }

                }


            }

        }

        public virtual void FlightAscension()
        {

            if(flightPeak == 0)
            {

                return;

            }

            float distance = Vector2.Distance(flightFrom, flightTo);

            float length = distance / 2;

            float lengthSq = (length * length);

            float heightFr = 4 * flightPeak;

            float coefficient = lengthSq / heightFr;

            int midpoint = (flightTotal / 2);

            float newHeight = 0;

            if (flightTimer != midpoint)
            {
                float newLength;

                if (flightTimer < midpoint)
                {

                    newLength = length * (midpoint - flightTimer) / midpoint;

                }
                else
                {

                    newLength = (length * (flightTimer - midpoint) / midpoint);

                }

                float newLengthSq = newLength * newLength;

                float coefficientFr = (4 * coefficient);

                newHeight = newLengthSq / coefficientFr;

            }

            flightHeight = flightPeak - (int)newHeight;

        }

        public virtual void UpdateSpecial()
        {

            specialTimer--;

            if (specialTimer <= 0) //|| Game1.player.IsBusyDoingSomething())
            {

                ClearSpecial();

            }
            else
            {

                if (specialTimer % specialInterval == 0)
                {

                    specialFrame++;

                    if (netSpecialActive.Value)
                    {

                        if (specialFrame > specialCeiling)
                        {
                            specialFrame = specialFloor;
                        }

                    }

                    if (netChannelActive.Value)
                    {

                        if (specialFrame > channelCeiling)
                        {
                            specialFrame = channelFloor;
                        }

                    }


                }

            }

        }

        public void SetCooldown(float factor = 1f)
        {

            cooldownActive = true;

            float netScale = GetScale();

            cooldownTimer = (int)(((float)cooldownInterval + (float)(new Random().NextDouble() * cooldownInterval)) * (1f - 0.1f * netScale) * factor);

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

        //=================== behaviour control

        public virtual bool ChangeBehaviour()
        {

            if (netHaltActive.Value)
            {

                return false;

            }

            if (netSpecialActive.Value || netChannelActive.Value)
            {

                return false;

            }

            if (netSweepActive.Value)
            {

                return false;

            }

            if (netFlightActive.Value || netSmashActive.Value)
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

            if (netPosturing.Value)
            {

                netHaltActive.Value = true;

                idleTimer = 180;

                return;

                //LookAtFarmer();

            }

            Random random = new Random();

            List<Vector2> targets = new();

            List<Farmer> farmers = ModUtility.FarmerProximity(currentLocation, new() { Position, }, 20 * 64);

            if (farmers.Count > 0)
            {
                
                targets.Add(farmers.First().Position);

            }

            if(tempermentActive == temperment.group)
            {

                List<StardewDruid.Character.Character> characters = ModUtility.CompanionProximity(currentLocation, new() { Position, }, 20 * 64);

                if (characters.Count > 0)
                {

                    targets.Add(characters.First().Position);

                }

            }


            if (targets.Count == 0)
            {

                PerformRandom();

                return;

            }

            Vector2 target = targets[Mod.instance.randomIndex.Next(targets.Count)];

            float threshold = Vector2.Distance(Position, target);

            ResetActives();

            SetDirection(target);

            if (!cooldownActive)
            {

                if (threshold > 256)
                {

                    if (channelSet)
                    {
                        
                        if(threshold <= 576 + (96 * GetScale()) && Mod.instance.randomIndex.Next(2) == 0)
                        {
                            
                            if (PerformChannel(target))
                            {

                                return;

                            };

                        }

                    }

                    if(specialSet)
                    {
                        
                        if (threshold <= 384 + (64 * GetScale()) && Mod.instance.randomIndex.Next(2) == 0)
                        {

                            if (PerformSpecial(target))
                            {

                                return;

                            }

                        }

                    }

                    if (Mod.instance.randomIndex.Next(2) == 0 && flightSet)
                    {

                        if(PerformFlight(target))
                        {

                            return;

                        }

                    }

                }

                if (threshold <= 128 + (32 * GetScale()) && sweepSet)
                {

                    if (PerformSweep())
                    {

                        return;

                    }

                }

            }

            ChooseMovement(threshold, target);

        }

        public virtual void ChooseMovement(float threshold, Vector2 position)
        {

            switch (tempermentActive)
            {

                case temperment.coward:

                    PerformRetreat(position);

                    break;

                case temperment.ranged:

                    float distance = Vector2.Distance(position, Position);

                    if(distance >= 192)
                    {

                        switch (new Random().Next(2))
                        {

                            case 0:

                                PerformCircle(position);

                                return;

                            case 1:

                                netHaltActive.Value = true;

                                idleTimer = 60;

                                LookAtFarmer();

                                TalkSmack();

                                return;

                        }

                    }
                    else
                    {
                        
                        PerformRetreat(position);

                    }

                    break;

                case temperment.aggressive:

                    switch (Mod.instance.randomIndex.Next(4))
                    {
                        case 1:

                            netHaltActive.Value = true;

                            idleTimer = 60;

                            LookAtFarmer();

                            TalkSmack();

                            return;

                        case 2:

                            PerformCircle(position);

                            return;

                    }

                    PerformFollow(position);
      
                    break;

                case temperment.odd:

                    switch (new Random().Next(4))
                    {

                        case 0: PerformRandom(); break;

                        case 1: PerformFollow(position); break;

                        case 2: PerformRetreat(position); break;

                        case 3: PerformCircle(position); break;

                    }

                    break;

                default:

                    switch (Mod.instance.randomIndex.Next(4))
                    {
                        case 1:

                            netHaltActive.Value = true;

                            idleTimer = 30;

                            LookAtFarmer();

                            TalkSmack();

                            return;

                        case 2:

                            if (threshold <= 320)
                            {
                                
                                PerformCircle(position);

                                return;

                            }

                            return;

                        case 3:
                            
                            if(threshold <= 192)
                            {
                                PerformRetreat(position);

                                return;

                            }

                            break;

                    }

                    PerformFollow(position);

                    break;


            }

        }

        public virtual bool PerformSweep()
        {

            if (!sweepSet) { return false; }

            List<Farmer> targets = ModUtility.FarmerProximity(currentLocation, new() { Position, }, 192);

            if (targets.Count > 0)
            {

                Vector2 targetFarmer = targets.First().Position;

                SetDirection(targetFarmer);

                sweepTimer = sweepInterval * SweepCount();

                SetCooldown(0.5f);

                sweepIncrement = new((targetFarmer.X - Position.X) / sweepTimer, (targetFarmer.Y - Position.Y) / sweepTimer);

                netSweepActive.Set(true);

                return true;

            }

            return false;

        }

        public virtual void ConnectSweep()
        {

            List<Farmer> targets = ModUtility.FarmerProximity(currentLocation, new() { Position, }, 96 + (32 * GetScale()));

            if(targets.Count > 0)
            {

                ModUtility.DamageFarmers(targets, (int)((float)GetThreat()*0.65f), this, true);

            }

        }

        public virtual bool PerformFlight(Vector2 target, int flightType = -1)
        {

            bool smash = false;

            if (flightType == -1)
            {

                switch (tempermentActive)
                {

                    case temperment.coward:


                        flightType = 3;

                        break;

                    case temperment.ranged:

                        flightType = 4;

                        break;

                    default:

                        flightType = flightDefault;

                        break;

                }
            }

            switch (flightType)
            {
                case 1: // dash close to target

                    List<Vector2> closeTargets = ModUtility.GetOccupiableTilesNearby(currentLocation, ModUtility.PositionToTile(target), (ModUtility.DirectionToTarget(Position, target)[2] + 4) % 8, 1, 2);

                    if(closeTargets.Count > 0)
                    {

                        flightTo = closeTargets.First() * 64;

                    }

                    if (smashSet)
                    {

                        smash = true;

                    }

                    break;

                case 2: // dash past target

                    List<Vector2> farTargets = ModUtility.GetOccupiableTilesNearby(currentLocation, ModUtility.PositionToTile(target), ModUtility.DirectionToTarget(Position, target)[2], 2, 3);

                    if (farTargets.Count > 0)
                    {

                        flightTo = farTargets.First() * 64;

                    }

                    if (smashSet)
                    {

                        smash = true;

                    }

                    break;

                case 3: // get away from target

                    List<Vector2> retreatTargets = ModUtility.GetOccupiableTilesNearby(currentLocation, ModUtility.PositionToTile(Position), (ModUtility.DirectionToTarget(Position, target)[2] + 4) % 8, 5, 3);

                    if (retreatTargets.Count > 0)
                    {

                        flightTo = retreatTargets.Last() * 64;

                    }
                    else
                    {

                        return false;

                    }

                    break;

                case 4: // circle target

                    int direction = ModUtility.DirectionToTarget(Position, target)[2];

                    int offset = Mod.instance.randomIndex.Next(2) == 0 ? 2 : -2;

                    int tangent = (direction + 8 + offset) % 8;

                    int distance = Math.Min(6, (int)Vector2.Distance(ModUtility.PositionToTile(Position), ModUtility.PositionToTile(target)));

                    List<Vector2> circleTargets = ModUtility.GetOccupiableTilesNearby(currentLocation, ModUtility.PositionToTile(target), tangent, distance, 2);

                    if(circleTargets.Count > 0)
                    {

                        flightTo = circleTargets.First() * 64;

                    }
                    else
                    {

                        return false;

                    }

                    break;

                default:

                    flightTo = target;

                    if (smashSet)
                    {

                        smash = true;

                    }

                    break;

            }

            SetCooldown(1);

            if (smash)
            {

                netSmashActive.Set(true);

            }
            else
            {

                netFlightActive.Set(true);

            }

            flightFrom = new(Position.X, Position.Y);

            flightIncrement = ModUtility.PathFactor(Position, flightTo) * flightSpeed;

            flightTimer = (int)(Vector2.Distance(Position, flightTo) / Vector2.Distance(new(0, 0), flightIncrement));

            flightTotal = flightTimer;

            flightSegment = flightInterval;

            int pathRequirement;

            if (!smash)
            {

                pathRequirement = FlightCount(3); 
            }
            else
            {
                pathRequirement = SmashCount(3);

            }

            int pathSqueeze = (int)(flightTimer / pathRequirement);

            if (pathSqueeze < flightInterval)
            {

                flightSegment = Math.Max(6,pathSqueeze);

            }

            return true;

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

        public virtual bool PerformSpecial(Vector2 target)
        {

            specialTimer = (specialCeiling + 1) * specialInterval;

            netSpecialActive.Set(true);

            SetCooldown(1f);

            TalkSmack();

            return true;

        }

        public void PerformFollow(Vector2 target)
        {

            SetDirection(target);

            followIncrement = ModUtility.PathFactor(Position, target);

            followTimer = (WalkCount() -1) * walkInterval;

            if (Vector2.Distance(Position,target) <= 32)
            {

                followTimer *= 2;

            }

        }

        public void PerformRetreat(Vector2 target)
        {

            int tangent = (ModUtility.DirectionToTarget(Position, target)[2] + 4) % 8;

            List<Vector2> retreatTargets = ModUtility.GetOccupiableTilesNearby(currentLocation, ModUtility.PositionToTile(Position), tangent, 2, 3);

            if (retreatTargets.Count > 0)
            {

                PerformFollow(retreatTargets.Last() * 64);

            }

        }

        public void PerformCircle(Vector2 target)
        {

            int direction = ModUtility.DirectionToTarget(Position, target)[2];

            int offset = Mod.instance.randomIndex.Next(2) == 0 ? 2 : -2;

            int tangent = (direction + 8 + offset) % 8;

            List<Vector2> circleTargets = ModUtility.GetOccupiableTilesNearby(currentLocation, ModUtility.PositionToTile(target), tangent, 2, 3);

            if(circleTargets.Count > 0)
            {

                PerformFollow(circleTargets.First() * 64);

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

            Vector2 target = Position + new Vector2(dictionary[key].X * 128f, dictionary[key].Y * 128);//Vector2.op_Multiply(dictionary[key], 128f);

            PerformFollow(target);

        }

        public virtual bool PerformChannel(Vector2 target)
        {

            specialTimer = (specialCeiling + 1) * specialInterval * 2;

            netChannelActive.Set(true);

            SetCooldown(2f);

            return true;

        }

        //=================== frame offsets

        public virtual int WalkCount()
        {

            return walkFrames[0].Count;

        }        
        
        public virtual int IdleCount()
        {

            return idleFrames[0].Count;

        }        
        
        public virtual int SweepCount()
        {

            return sweepFrames[0].Count;

        }        
        
        public virtual int FlightCount(int segment = 0)
        {

            switch (segment)
            {
                default:
                case 0: return flightFrames[0].Count;

                case 1: return flightFrames[4].Count;
                
                case 2: return flightFrames[8].Count;

                case 3: return flightFrames[0].Count + flightFrames[4].Count + flightFrames[8].Count;
            
            }

        }

        public virtual int SmashCount(int segment = 0)
        {

            switch (segment)
            {
                default:
                case 0: return smashFrames[0].Count;

                case 1: return smashFrames[4].Count;

                case 2: return smashFrames[8].Count;

                case 3: return smashFrames[0].Count + smashFrames[4].Count + smashFrames[8].Count;

            }

        }

        public virtual Vector2 CastPosition()
        {

            Vector2 castAt = Position + new Vector2(32, 32);

            switch (netDirection.Value)
            {

                case 0:

                    castAt.Y -= 96;

                    if (netAlternative.Value == 1)
                    {

                        castAt.X += 96;

                        break;
                    }

                    castAt.X -= 96;

                    break;

                case 1:

                    castAt.Y -= 32;

                    castAt.X += 96;

                    break;

                case 2:

                    if(netAlternative.Value == 1)
                    {

                        castAt.X += 96;

                        break;
                    }

                    castAt.X -= 96;

                    break;

                case 3:

                    castAt.Y -= 32;

                    castAt.X -= 96;

                    break;

            }

            return castAt;

        }

    }

}