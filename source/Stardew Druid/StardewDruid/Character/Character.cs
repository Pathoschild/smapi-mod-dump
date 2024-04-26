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
using StardewDruid.Dialogue;
using StardewDruid.Event;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Timers;
using static StardewDruid.Event.SpellHandle;
using static StardewValley.Minigames.TargetGame;

namespace StardewDruid.Character
{
    public class Character : NPC
    {

        public Texture2D characterTexture;
        public CharacterData.characters characterType = CharacterData.characters.none;
        //public List<Vector2> targetVectors = new();

        public Vector2 occupied;
        public Vector2 destination;
        public Dictionary<Vector2,int> traversal = new();
        public Vector2 tether;

        public float gait;

        public NetInt netDirection = new NetInt(0);
        public NetInt netAlternative = new NetInt(0);
        public NetBool netFollowActive = new NetBool(false);
        public NetBool netWorkActive = new NetBool(false);

        public List<Vector2> roamVectors = new();
        public int roamIndex;
        public double roamLapse;

        public NetBool netSceneActive = new NetBool(false);
        public Dictionary<int,Vector2> eventVectors = new();
        public List<Vector2> closedVectors = new();
        public int eventIndex;
        public string eventName;
        public bool loadedOut;

        public enum mode
        {
            home,
            scene,
            track,
            roam,
            random,
        }

        public mode modeActive;

        public enum pathing
        {
            none,
            monster,
            player,
            roam,
            scene,
            random,

        }

        public pathing pathActive;

        public Dictionary<int, List<Rectangle>> walkFrames = new();
        public Dictionary<int, List<Rectangle>> dashFrames = new();
        public Dictionary<int, List<Rectangle>> idleFrames = new();
        public Dictionary<int, List<Rectangle>> haltFrames = new();
        public Dictionary<int, List<Rectangle>> specialFrames = new();
        public Dictionary<int, List<Rectangle>> workFrames = new();

        public NetBool netHaltActive = new NetBool(false);
        public NetBool netStandbyActive = new NetBool(false);
        public int idleTimer;
        public int stationaryTimer;

        public int collidePriority;
        public int collideTimer;
        public int moveTimer;
        public int moveInterval;
        public int moveFrame;
        public bool moveRetreat;
        public bool walkSide;
        public int walkLeft;
        public int walkRight;
        public int lookTimer;
        public int followTimer;
        public int attentionTimer;

        public NetBool netDashActive = new NetBool(false);
        public int dashFrame;
        public int dashFloor;
        public int dashCeiling;
        public bool dashSweep;
        public int dashHeight;

        public NetBool netSweepActive = new(false);
        public Dictionary<int, List<Rectangle>> sweepFrames = new();
        public int sweepTimer;
        public int sweepFrame;
        public int sweepInterval;

        public NetBool netSpecialActive = new NetBool(false);
        public int specialTimer;
        public int specialInterval;
        public int specialCeiling;
        public int specialFloor;
        public int specialFrame;
        public SpellHandle.schemes specialScheme;
        public Vector2 workVector;

        public int cooldownTimer;
        public int cooldownInterval;
        public int hitTimer;
        public int pushTimer;

        public int moveDirection;
        public int altDirection;
        public Vector2 setPosition = Vector2.Zero;

        public string previousLocation;
        public Vector2 previousPosition = Vector2.Zero;
        public mode previousMode;
        
        public Character()
        {
        }

        public Character(CharacterData.characters type)
          : base(
                new AnimatedSprite(Path.Combine("Characters","Abigail")), 
                CharacterData.CharacterStart(CharacterData.locations.home),
                CharacterData.CharacterLocation(CharacterData.locations.home),
                2, 
                CharacterData.CharacterNames()[type], 
                CharacterData.CharacterPortrait(type), 
                false
                )
        {

            characterType = type;

            willDestroyObjectsUnderfoot = false;

            HideShadow = true;

            SimpleNonVillagerNPC = true;

            SettleOccupied();

            LoadOut();
        
        }

        protected override void initNetFields()
        {
            base.initNetFields();
            NetFields.AddField(netDirection, "netDirection");
            NetFields.AddField(netAlternative, "netAlternative");
            NetFields.AddField(netSpecialActive, "netSpecialActive");
            NetFields.AddField(netDashActive, "netDashActive");
            NetFields.AddField(netHaltActive, "netHaltActive");
            NetFields.AddField(netFollowActive, "netFollowActive");
            NetFields.AddField(netStandbyActive, "netStandbyActive");
            NetFields.AddField(netSceneActive, "netSceneActive");
            NetFields.AddField(netSweepActive, "netSweepActive");
            NetFields.AddField(netWorkActive, "netWorkActive");
        }

        public virtual void LoadBase()
        {

            collidePriority = new Random().Next(20);

            gait = 1.6f;

            moveInterval = 12;

            modeActive = mode.random;

            walkLeft = 1;

            walkRight = 3;

            dashFloor = 1;

            dashCeiling = 4;

            sweepInterval = 7;

            specialInterval = 30;

            specialCeiling = 1;

            specialFloor = 1;

            cooldownInterval = 180;

            specialScheme = SpellHandle.schemes.fire;

        }

        public virtual void LoadOut()
        {

            if(characterType == CharacterData.characters.none)
            {
                characterType = CharacterData.CharacterTypes()[Name];

            }

            LoadBase();

            characterTexture = CharacterData.CharacterTexture(characterType);

            haltFrames = FrameSeries(16, 32, 0, 0, 1);

            walkFrames = FrameSeries(16, 32, 0, 0, 4);

            dashFrames = walkFrames;

            loadedOut = true;

        }

        public virtual Dictionary<int, List<Rectangle>> FrameSeries( int width, int height, int startX = 0, int startY = 0, int length = 6, Dictionary<int, List<Rectangle>> frames = null)
        {

            if(frames == null)
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

            foreach (KeyValuePair<int, int> keyValuePair in normalSequence )
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

        public override void draw(SpriteBatch b, float alpha = 1f)
        {

            if (IsInvisible || !Utility.isOnScreen(Position, 128))
            {
                
                return;

            }

            DrawEmote(b);

        }

        public override void DrawEmote(SpriteBatch b)
        {

            if (IsEmoting && !Game1.eventUp)
            {
                Vector2 localPosition = getLocalPosition(Game1.viewport);

                float drawLayer = (float)StandingPixel.Y / 10000f;

                b.Draw(Game1.emoteSpriteSheet, localPosition - new Vector2(0, 160), new Microsoft.Xna.Framework.Rectangle(base.CurrentEmoteIndex * 16 % Game1.emoteSpriteSheet.Width, base.CurrentEmoteIndex * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, drawLayer);
            }

        }

        public virtual void DrawShadow(SpriteBatch b, Vector2 localPosition, float drawLayer, float offset = 0)
        {

            b.Draw(
                Game1.shadowTexture,
                localPosition + new Vector2(6 + offset, 44f),
                Game1.shadowTexture.Bounds,
                Color.White * 0.65f,
                0f,
                Vector2.Zero,
                4f,
                SpriteEffects.None,
                drawLayer - 0.0001f
                );

        }

        public int IdleFrame()
        {

            int interval = 12000 / idleFrames[0].Count();

            int timeLapse = (int)(Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 12000);

            if (timeLapse == 0) { return 0; }

            int frame = (int)timeLapse / interval;

            return frame;

        }

        public override Rectangle GetBoundingBox()
        {
            
            return new Rectangle((int)Position.X + 8, (int)Position.Y+ 8, 48, 48);
        
        }

        public virtual Rectangle GetHitBox()
        {
            
            return GetBoundingBox();
        
        }

        public override void reloadSprite(bool onlyAppearance = false)
        {
            base.reloadSprite(onlyAppearance);
            Portrait = CharacterData.CharacterPortrait(characterType);

        }

        public override void reloadData()
        {
            CharacterDisposition characterDisposition = CharacterData.CharacterDisposition(characterType);
            Age = characterDisposition.Age;
            Manners = characterDisposition.Manners;
            SocialAnxiety = characterDisposition.SocialAnxiety;
            Optimism = characterDisposition.Optimism;
            Gender = characterDisposition.Gender;
            datable.Value = characterDisposition.datable;
            Birthday_Season = characterDisposition.Birthday_Season;
            Birthday_Day = characterDisposition.Birthday_Day;
            id = characterDisposition.id;
        }

        public override void reloadDefaultLocation()
        {
            DefaultMap = "DruidGrove";
            DefaultPosition = WarpData.WarpStart(DefaultMap);
        }

        public override void receiveGift(StardewValley.Object o, Farmer giver, bool updateGiftLimitInfo = true, float friendshipChangeMultiplier = 1, bool showResponse = true)
        {

        }

        public override bool checkAction(Farmer who, GameLocation l)
        {
            
            if (Mod.instance.eventRegister.ContainsKey("transform"))
            {

                Mod.instance.CastMessage("Unable to converse while transformed");

                return false;

            }

            foreach (NPC character in currentLocation.characters)
            {

                if (character is StardewValley.Monsters.Monster monster && (double)Vector2.Distance(Position, monster.Position) <= 1280.0)
                {

                    return false;

                }

            }

            if (netDashActive.Value || netSpecialActive.Value)
            {

                return false;

            }

            if (!EngageDialogue())
            {
                
                return false;
            
            }

            if (!netSceneActive.Value)
            {

                Halt();

            }

            LookAtTarget(who.Position, true);

            return true;

        }

        public virtual bool EngageDialogue()
        {

            if (!Mod.instance.dialogue.ContainsKey(characterType))
            {

                Mod.instance.dialogue[characterType] = new(this);

                return true;

            }

            if (netSceneActive.Value)
            {

                if(eventName == null)
                {

                    return false;

                }

                if (Mod.instance.eventRegister.ContainsKey(eventName))
                {

                    if (Mod.instance.eventRegister[eventName].DialogueNext(this))
                    {

                        return true;

                    };

                }

                return false;

            }

            Mod.instance.dialogue[characterType].DialogueApproach();

            return true;

        }

        public override void Halt()
        {

            netHaltActive.Set(true);
            //ModUtility.LogStrings(new() { Name, "idle", "halt"});
            TargetIdle();

        }

        public virtual void ResetActives()
        {

            ClearIdle();

            ClearMove();

            ClearSweep();

            ClearSpecial();

            ResetTimers();

            SettleOccupied();

            //targetVectors.Clear();

        }

        public virtual void ResetTimers()
        {

            idleTimer = 0;

            moveTimer = 0;

            sweepTimer = 0;

            specialTimer = 0;

            cooldownTimer = 0;

            hitTimer = 0;

            lookTimer = 0;

            collideTimer = 0;

            pushTimer = 0;

            dashHeight = 0;

            followTimer = 0;

            attentionTimer = 0;

        }

        public virtual void ClearIdle()
        {

            if (netHaltActive.Value)
            {

                netHaltActive.Set(false);

            }

            if (netStandbyActive.Value)
            {

                netStandbyActive.Set(false);

            }

            idleTimer = 0;

        }

        public virtual void ClearMove()
        {

            pathActive = pathing.none;

            destination = Vector2.Zero;

            traversal.Clear();

            if (netDashActive.Value)
            {

                netDashActive.Set(false);

            }

            dashFrame = 0;

            moveTimer = 0;

            moveFrame = 0;

            dashSweep = false;

        }

        public virtual void ClearSweep(bool apply = false)
        {

            if (apply)
            {

                List<StardewValley.Monsters.Monster> monsters = ModUtility.MonsterProximity(currentLocation, new() { Position, }, 128);

                foreach (StardewValley.Monsters.Monster monster in monsters)
                {

                    DealDamageToMonster(monster, Mod.instance.CombatDamage() / 2);

                }

            }

            if (netSweepActive.Value)
            {

                netSweepActive.Set(false);

            }

            sweepFrame = 0;

            sweepTimer = 0;

        }

        public virtual void ClearSpecial()
        {
            
            if (netSpecialActive.Value)
            {

                netSpecialActive.Set(false);

            }

            specialTimer = 0;

            specialFrame = 0;

            if (netWorkActive.Value)
            {

                netWorkActive.Set(false);

            }

        }

        public void LookAtTarget(Vector2 target, bool force = false)
        {
            
            if (lookTimer > 0 && !force) { return; }

            List<int> directions = ModUtility.DirectionToTarget(Position, target);

            moveDirection = directions[0];

            altDirection = directions[1];

            netDirection.Set(moveDirection);

            netAlternative.Set(altDirection);

            lookTimer = (int)(20f * MoveSpeed(Vector2.Distance(Position,target)));

        }

        public override void performTenMinuteUpdate(int timeOfDay, GameLocation l)
        {

        }

        public override void behaviorOnFarmerPushing()
        {
            
            if (netDashActive.Value || netSpecialActive.Value || netSceneActive.Value || netHaltActive.Value)
            {

                return;

            }

            if (Context.IsMainPlayer)
            {

                pushTimer += 2;

                if(pushTimer > 3)
                {

                    TargetRandom(4);

                    pushTimer = 0;

                }

            }

        }

        public virtual void normalUpdate(GameTime time, GameLocation location)
        {

            if (!loadedOut)
            {
                LoadOut();
            }

            if (Context.IsMainPlayer)
            {

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

                updateEmote(time);

            }

        }

        public override void update(GameTime time, GameLocation location)
        {

            normalUpdate(time, location);

            if (!Context.IsMainPlayer)
            {
                
                UpdateMultiplayer();

                return;

            }

            /*Vector2 thisTile = new((int)(Position.X / 64), (int)(Position.Y / 64));

            if (ModUtility.GroundCheck(currentLocation, thisTile, false) != "ground")
            {

                WarpToEntrance(thisTile);

                return;

            }*/

            if (modeActive == mode.scene)
            {

                ProgressScene();

                return;

            }

            UpdateBehaviour();

            ChooseBehaviour();

            //MoveTowardsTarget();

            Traverse();

            //ModUtility.LogStrings(new() { modeActive.ToString(),pathActive.ToString(),destination.ToString(),traversal.Count.ToString()});

        }

        // ========================================
        // SET BEHAVIOUR
        // ========================================

        public virtual void ProgressScene()
        {

            if (netHaltActive.Value)
            {

                netHaltActive.Set(false);

            }

            UpdateSweep();

            UpdateSpecial();

            if (eventVectors.Count > 0)
            {

                KeyValuePair<int, Vector2> eventVector = eventVectors.First();

                float distance = Vector2.Distance(Position, eventVector.Value);

                //LookAtTarget(eventVector.Value);

                Position = ModUtility.PathMovement(Position, eventVector.Value, MoveSpeed(distance));

                UpdateMove();

                if (Vector2.Distance(Position, eventVector.Value) <= 4f)
                {

                    Position = eventVector.Value;

                    eventVectors.Remove(eventVector.Key);

                    if (Mod.instance.eventRegister.ContainsKey(eventName))
                    {

                        Mod.instance.eventRegister[eventName].EventScene(eventVector.Key);

                    }

                    if(eventVectors.Count > 0)
                    {

                        LookAtTarget(eventVectors.First().Value, true);

                    }
                    else
                    {

                        ClearMove();

                    }

                }

            }

        }

        public virtual void TargetEvent(int key, Vector2 target, bool clear = true)
        {

            pathActive = pathing.scene;

            if (clear)
            {

                eventVectors.Clear();

            }

            eventVectors.Add(key, target);

            destination = target / 64; //ModUtility.PositionToTile(target);

            if (eventVectors.Count == 1)
            {

                LookAtTarget(target, true);

            }

        }

        public virtual bool ChangeBehaviour()
        {

            if (netHaltActive.Value)
            {
                /*if (collideTimer <= 0)
                {

                    if (CollideCharacters())
                    {
 
                        return true;

                    }

                }*/

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

            if (netDashActive.Value)
            {

                return false;

            }

            if (destination != Vector2.Zero)
            {

                if (ArrivedDestination())
                {

                    return true;

                }

                if(pathActive == pathing.player)
                {

                    if (TrackToClose())
                    {

                        ClearMove();

                        return true;

                    }

                    if (modeActive == mode.track)
                    {
                        
                        if (TrackToFar())
                        {
                            followTimer = 0;
                            ClearMove();

                            return true;

                        }

                    }

                }

                return false;

            }

            if (idleTimer > 0)
            {

                if (modeActive == mode.track)
                {

                    // need to stay where the action is

                    if (TrackToFar())
                    {
                        followTimer = 0;
                        ClearIdle();

                        return true;

                    }

                }

                if (collideTimer<= 0)
                {
                    
                    if (CollideCharacters(occupied))
                    {

                        ClearIdle();

                        TargetRandom(4);

                    }

                }

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

            switch (modeActive)
            {

                case mode.track:

                    if (TargetMonster())
                    {

                        return;

                    }

                    if (TargetWork())
                    { 
                        
                        return; 
                    
                    }

                    if (TargetTrack())
                    {

                        return;

                    };

                    TargetRandom(12);

                    return;

                case mode.roam:

                    if (TargetWork())
                    {

                        return;

                    }

                    if (TargetRoam())
                    {

                        return;

                    };

                    TargetRandom(12);

                    return;

            }

            TargetRandom();

        }

        public virtual bool TargetIdle(int timer = -1)
        {

            if (!netSceneActive.Value)
            {

                if (ModUtility.TileAccessibility(currentLocation, occupied) != 0)
                {

                    WarpToEntrance();

                }

            }

            if (timer == -1)
            {

                switch (modeActive)
                {

                    case mode.track:

                        timer = 180; 

                        break;

                    case mode.roam:

                        timer = 360;

                        break;

                    default: //mode.scene: mode.standby:

                        timer = 720;

                        break;

                }

            }

            Random random = new();

            if(random.Next(2) == 0)
            {

                moveDirection = 2;

                netDirection.Set(2);

            }

            idleTimer = timer;

            return true;

        }

        public virtual bool TargetMonster()
        {
            
            if(cooldownTimer > 0)
            {

                return false;

            }

            List<StardewValley.Monsters.Monster> monsters = ModUtility.MonsterProximity(currentLocation, new() { Position, }, 640f);

            if (monsters.Count > 0)
            {

                foreach(StardewValley.Monsters.Monster monster in monsters)
                {

                    if (MonsterAttack(monster))
                    {

                        return true;

                    }

                }

            }

            return false;

        }

        public virtual bool MonsterAttack(StardewValley.Monsters.Monster monster)
        {

            float distance = Vector2.Distance(Position, monster.Position);

            string terrain = ModUtility.GroundCheck(currentLocation, new Vector2((int)(monster.Position.X/64),(int)(monster.Position.Y/64)));

            if (new Random().Next(3) == 0 || terrain != "ground")
            {

                return SpecialAttack(monster);

            }

            if (distance >= 192f)
            {

                return CloseDistance(monster);

            }

            return SweepAttack(monster);

        }

        public virtual bool CloseDistance(StardewValley.Monsters.Monster monster)
        {

            ResetActives();

            if (PathTarget(monster.Position, 2, 1))
            {
                pathActive = pathing.monster;

                netDashActive.Set(true);

                return true;

            }

            return false;

        }

        public virtual bool SweepAttack(StardewValley.Monsters.Monster monster)
        {

            ResetActives();

            if (PathTarget(monster.Position, 2, 0))
            {
                pathActive = pathing.monster;

                netSweepActive.Set(true);

                sweepFrame = 0;

                sweepTimer = sweepFrames[0].Count() * sweepInterval;

                int stun = Math.Max(monster.stunTime.Value, 500);

                monster.stunTime.Set(stun);

                return true;

            }

            return false;

        }

        public virtual bool SpecialAttack(StardewValley.Monsters.Monster monster)
        {

            ResetActives();

            netSpecialActive.Set(true);

            specialTimer = 90;

            LookAtTarget(monster.Position, true);

            SpellHandle fireball = new(Game1.player, new() { monster, }, Mod.instance.CombatDamage() / 2);

            fireball.origin = GetBoundingBox().Center.ToVector2();

            fireball.type = SpellHandle.spells.missile;

            fireball.scheme = specialScheme;
            
            fireball.display = displays.Impact;

            fireball.added = new() { SpellHandle.effects.aiming, };

            fireball.power = 3;

            Mod.instance.spellRegister.Add(fireball);

            return true;

        }

        public virtual bool TargetTrack()
        {
            
            if(followTimer > 0)
            {

                return false;

            }

            if (!Mod.instance.trackers.ContainsKey(characterType))
            {
                
                return false;
            
            }

            if (TrackToClose())
            {
                
                if(attentionTimer == 0)
                {
                    
                    // idle or random direction

                    followTimer = 120;
                    
                    return false;

                }

                // keep attention on farmer

                idleTimer = 10;

                LookAtTarget(Mod.instance.trackers[characterType].followPlayer.Position);

                return true;

            }

            if (TrackToFar())
            {

                Vector2 lastPosition = Position;

                if (Mod.instance.trackers[characterType].WarpToPlayer())
                {

                    attentionTimer = 360;

                    LookAtTarget(Mod.instance.trackers[characterType].followPlayer.Position);

                    ModUtility.AnimateQuickWarp(currentLocation, Position);

                    ModUtility.AnimateQuickWarp(currentLocation, lastPosition, true);

                    return true;

                }

            }

            if (PathTrack())
            {

                pathActive = pathing.player;

                return true;

            }

            return false;

        }

        public virtual bool TrackToClose(int close = 128)
        {

            if (Mod.instance.trackers.ContainsKey(characterType))
            {
                
                if (Vector2.Distance(Position, Mod.instance.trackers[characterType].followPlayer.Position) <= close)
                {

                    return true;

                }

            }
            else
            {

                if (Vector2.Distance(Position, Game1.player.Position) <= close)
                {

                    return true;

                }

            }

            return false;

        }

        public virtual bool TrackToFar(int limit = 960)
        {

            if (netStandbyActive.Value)
            {

                return false;

            }

            if(Vector2.Distance(Position, Mod.instance.trackers[characterType].followPlayer.Position) >= limit || !Utility.isOnScreen(Position, 128))
            {

                return true;

            }

            if (cooldownTimer <= 0)
            {
                
                if (ModUtility.MonsterProximity(currentLocation, new() { Mod.instance.trackers[characterType].followPlayer.Position }, 384f).Count > 0)
                {

                    return true;

                }

            }

            return false;

        }

        public virtual bool TargetRoam()
        {

            if (roamVectors.Count == 0)
            {

                return false;

            }

            Vector2 roamVector = roamVectors[roamIndex];

            if (roamVector.X < 0)
            {

                UpdateRoam();

                TargetIdle(720);

                netStandbyActive.Set(true);

                return true;

            }

            if (Vector2.Distance(roamVectors[roamIndex], Position) <= 192f || roamLapse < Game1.currentGameTime.TotalGameTime.TotalMinutes)
            {

                UpdateRoam();

                return true;

            }

            if (PathTarget(roamVectors[roamIndex], 2, 2))
            {
                
                pathActive = pathing.roam;

                return true;

            }

            return false;

        }

        public virtual void TargetRandom(int level = 8)
        {

            Random random = new Random();

            int decision = random.Next(level);

            switch (decision)
            {
                case 0:
                case 1:
                case 2:
                case 3:

                    int newDirection = random.Next(10);

                    if (newDirection >= 8)
                    {

                        newDirection = ModUtility.DirectionToTarget(Position, tether)[2];

                    }

                    List<int> directions = new()
                    {

                        (newDirection + 4) % 8,
                        (newDirection + 6) % 8,
                        (newDirection + 2) % 8,

                    };

                    foreach (int direction in directions)
                    {

                        if (PathTarget(occupied*64, 0, 1, direction))
                        {
                            
                            pathActive = pathing.random;

                            return;

                        }

                    }

                    break;

            }

            TargetIdle();

        }

        public virtual bool TargetWork()
        {

            return false;

        }

        public virtual void PerformWork()
        {


        }


        // ========================================
        // UPDATE
        // ========================================

        public virtual void UpdateBehaviour()
        {

            UpdateIdle();

            UpdateMove();

            UpdateSweep();

            UpdateSpecial();

            if (cooldownTimer > 0)
            {

                cooldownTimer--;

            }

            if (hitTimer > 0)
            {

                hitTimer--;


            }

            if (lookTimer > 0)
            {

                lookTimer--;

            }

            if (collideTimer > 0)
            {

                collideTimer--;

            }

            if (pushTimer > 0)
            {

                pushTimer--;


            }

            if (dashHeight > 0)
            {

                dashHeight--;

            }

            if(followTimer > 0)
            {

                followTimer--;

            }

            if (attentionTimer > 0)
            {

                attentionTimer--;

            }

        }

        public virtual void UpdateMultiplayer()
        {
            
            if (netHaltActive.Value)
            {

                return;

            }

            if (netSweepActive.Value)
            {

                sweepTimer++;

                if (sweepTimer == sweepInterval)
                {

                    sweepFrame++;

                    sweepTimer = 0;

                }

            }
            else
            {
                sweepFrame = 0;

                sweepTimer = 0;

            }

            if (netSpecialActive.Value)
            {

                specialTimer++;

                if (specialTimer == specialInterval)
                {

                    specialFrame++;

                    if (specialFrame > specialCeiling)
                    {

                        specialFrame = specialFloor;

                    }

                    specialTimer = 0;

                }

                return;

            }
            else
            {
                specialFrame = 0;

                specialTimer = 0;

            }

            if (setPosition != Position || netDirection.Value != moveDirection || netAlternative.Value != altDirection)
            {

                setPosition = Position;

                moveDirection = netDirection.Value;

                altDirection = netAlternative.Value;

                moveTimer--;

                if (netDashActive.Value)
                {
                    if (dashFrame < (dashCeiling / 2) && dashHeight < 128)
                    {

                        dashHeight += 2;

                    }
                    else if (dashHeight > 1)
                    {

                        dashHeight -= 2;

                    }
                }

                if (moveTimer <= 0)
                {

                    moveFrame++;

                    if (moveFrame >= walkFrames[0].Count)
                    {

                        moveFrame = 1;
                    
                    }

                    moveTimer = moveInterval;

                    moveTimer -= 3;

                    dashFrame++;

                    if (dashFrame > dashCeiling)
                    {

                        dashFrame = dashFloor;

                    }

                    stationaryTimer = 30;

                    idleTimer = 0;

                }

                return;

            }

            if (stationaryTimer > 0)
            {

                stationaryTimer--;

                if (stationaryTimer == 0)
                {

                    moveFrame = 0;

                    moveTimer = 0;

                }

            }
            else
            {

                idleTimer++;
            
            }

        }

        public virtual void UpdateEvent(int index)
        {
            
            if (eventName == null)
            {

                return;

            }

            if (!Mod.instance.eventRegister.ContainsKey(eventName))
            {

                eventName = null;

                return;

            }

            Mod.instance.eventRegister[eventName].EventScene(index);
        
        }

        public virtual void UpdateIdle()
        {

            if (idleTimer > 0)
            {

                idleTimer--;

            }

            if (netHaltActive.Value)
            {

                if (idleTimer <= 0)
                {

                    ClearIdle();

                    ClearMove();

                    return;

                }

            }

        }

        public virtual void UpdateSweep()
        {

            if (sweepTimer > 0)
            {

                sweepTimer--;

            }

            if (netSweepActive.Value)
            {

                if (sweepTimer <= 0)
                {

                    ClearSweep(true);

                    cooldownTimer = cooldownInterval;

                }
                else
                {

                    if (sweepTimer % sweepInterval == 0)
                    {

                        sweepFrame++;

                        if (sweepFrame == sweepFrames[0].Count)
                        {

                            ClearSweep(true);

                        }

                    }

                }

            }

        }

        public virtual void UpdateMove()
        {

            if (moveTimer > 0)
            {

                moveTimer--;

            }

            if (destination == Vector2.Zero)
            {

                ClearMove();

                return;

            }

            float distance = Vector2.Distance(Position, destination*64);

            if (moveTimer <= 0)
            {

                moveTimer = (int)MoveSpeed(distance, true);

                moveFrame++;

                if(moveFrame == walkLeft)
                {

                    if (walkSide)
                    {

                        moveFrame = walkRight;

                    }

                }

                if (moveFrame == walkRight)
                {

                    walkSide = false;

                }

                if (moveFrame >= walkFrames[0].Count)
                {

                    moveFrame = walkLeft;

                    walkSide = true;

                }

                dashFrame++;

                if (dashFrame > dashCeiling)
                {

                    dashFrame = dashFloor;

                }

                if (dashSweep)
                {

                    float moveSpeed = (int)MoveSpeed(distance);

                    float sweepFactor = moveTimer * (sweepFrames[0].Count-1) * moveSpeed;

                    if (distance <= sweepFactor)
                    {

                        netSweepActive.Set(true);

                        sweepTimer = moveTimer * sweepFrames[0].Count;

                        dashSweep = false;

                    }

                }
            }

            if (netDashActive.Value)
            {
                if (dashFrame < (dashCeiling / 2) && dashHeight < 128)
                {

                    dashHeight += 2;

                }
                else if (dashHeight > 1)
                {

                    dashHeight -= 2;

                }

            }

        }

        public virtual void UpdateSpecial()
        {

            if (specialTimer > 0)
            {

                specialTimer--;

            }

            if (netSpecialActive.Value)
            {

                if (specialTimer <= 0)
                {

                    ClearSpecial();

                    cooldownTimer = cooldownInterval;

                }
                else if (specialTimer % specialInterval == 0)
                {

                    specialFrame++;

                    if(specialFrame > specialCeiling)
                    {

                        specialFrame = specialFloor;

                    }

                }

            }

            if (netWorkActive.Value)
            {
                
                if (specialTimer <= 0)
                {

                    ClearSpecial();

                }

                PerformWork();

            }

        }

        public void UpdateRoam()
        {

            roamLapse = Game1.currentGameTime.TotalGameTime.TotalMinutes + 1.0;

            roamIndex++;

            if (roamIndex == roamVectors.Count)
            {

                SwitchToMode(mode.roam, Game1.player);

                return;

            }

            tether = roamVectors[roamIndex];

        }

        public virtual float MoveSpeed(float distance = 0, bool moveFrames = false)
        {

            float moveSpeed = gait;

            float moveFrame = moveInterval;

            switch (pathActive)
            {

                case pathing.monster:

                    moveSpeed = gait * 2f;

                    moveFrame -= 2;

                    break;

                case pathing.scene:


                    if (distance > 640)
                    {

                        moveFrame -= 3;

                        moveSpeed = gait * 3f;

                    }
                    else if (distance > 360)
                    {

                        moveFrame -= 2;

                        moveSpeed = gait * 2.25f;

                    }
                    else
                    {
                        moveFrame -= 1;

                        moveSpeed = gait * 1.5f;

                    }

                    break;


                case pathing.player:

                    if (distance > 640)
                    {

                        moveFrame -= 3;

                        moveSpeed = gait * 3f;
                    
                    }
                    else if (distance > 360)
                    {

                        moveFrame -= 2;

                        moveSpeed = gait * 2.25f;

                    }
                    else
                    {
                        moveFrame -= 1;

                        moveSpeed = gait * 1.5f;

                    }

                    break;

                case pathing.random:

                    break;

                case pathing.roam:

                    if (distance > 360)
                    {

                        moveFrame -= 2;

                        moveSpeed *= 1.5f;

                    }

                    break;

                case pathing.none:

                    break;


            }

            if (netDashActive.Value)
            {

                moveSpeed *= 2.5f;

            }

            return moveFrames ? moveFrame : moveSpeed;

        }


        // ========================================
        // MOVEMENT
        // ========================================

        public bool PathTarget(Vector2 target, int ability, int proximity, int direction = -1)
        {

            Vector2 center = ModUtility.PositionToTile(target);

            if(center == occupied && proximity == 0)
            {

                return true;

            }

            if (direction == -1)
            {

                // direction from target (center) to origin (occupied), will search for tiles in between

                direction = ModUtility.DirectionToTarget(target, Position)[2]; // uses 64

            }

            Dictionary<Vector2, int> paths = ModUtility.TraversalToTarget(currentLocation, occupied, center, ability, proximity, direction); // uses tiles

            if (paths.Count > 0)
            {

                destination = paths.Keys.Last();

                traversal = paths;

                LookAtTarget(destination * 64, false); // uses 64

                return true;

            }

            return false;

        }

        public bool PathTrack()
        {

            // check if can walk / jump to player

            if (PathTarget(Mod.instance.trackers[characterType].followPlayer.Position, 1, 2))
            {

                // dont need the tracked path now

                Mod.instance.trackers[characterType].nodes.Clear();

                return true;

            }

            Dictionary<Vector2, int> paths = Mod.instance.trackers[characterType].NodesToTraversal();

            if(paths.Count > 0)
            {

                // walk / jump to the start of the path

                if (PathTarget(paths.Keys.First()*64, 1, 0))
                {

                    // add remaining path segments to traversal

                    if(paths.Count > 1)
                    {
                        
                        for (int p = paths.Count - 2; p >= 0; p--)
                        {

                            traversal.Append(paths.ElementAt(p));

                        }

                    }

                }
                {
                    
                    // might have to warp to the start of the path

                    paths[paths.ElementAt(0).Key] = 2;

                    traversal = paths;

                }

                destination = traversal.Keys.Last();

                return true;

            }

            return false;

        }

        public virtual void SettleOccupied()
        {

            occupied = new Vector2((int)(Position.X / 64), (int)(Position.Y / 64));

        }

        public virtual void Traverse()
        {

            if (destination == Vector2.Zero || netHaltActive.Value || netSceneActive.Value)
            {

                SettlePosition();

                return;

            }

            if (ArrivedDestination())
            {

                return;

            }

            KeyValuePair<Vector2,int> target = traversal.First();

            LookAtTarget(target.Key * 64, false);

            if (target.Value == 2)
            {

                ModUtility.AnimateQuickWarp(currentLocation, Position, true);

                Position = target.Key * 64;

                ModUtility.AnimateQuickWarp(currentLocation, Position);

                occupied = target.Key;

                traversal.Remove(target.Key);

            }
            else
            {

                if(target.Value == 1 && !netDashActive.Value)
                {

                    netDashActive.Set(true);

                }

                float speed = MoveSpeed(Vector2.Distance(Position, target.Key * 64));

                Position = ModUtility.PathMovement(Position, target.Key*64, speed);

                float remain = Vector2.Distance(Position, target.Key * 64);

                if (remain <= 4f)
                {

                    occupied = target.Key;

                    traversal.Remove(target.Key);

                }

            }

            ArrivedDestination();

        }

        public virtual bool ArrivedDestination()
        {

            if (occupied == destination || traversal.Count == 0)
            {

                ClearMove();

                return true;

            }

            return false;

        }

        public virtual void SettlePosition()
        {

            // Settle position slowly shifts the character towards the set occupied tile
            // This is because Position can be offset by the floating coordinates obtained from traversal
            // and the occupied tile position might not match up

            Vector2 occupation = occupied * 64;

            if (Position != occupation)
            {

                if(Vector2.Distance(Position,occupation) >= 32f)
                {

                    LookAtTarget(occupation, false);

                }

                Position = ModUtility.PathMovement(Position, occupation, 2);

            }

        }

        public virtual bool TightPosition()
        {

            if (destination != Vector2.Zero)
            {

                return false;

            }

            if (Position / 64 == new Vector2((int)(Position.X/64), (int)(Position.Y / 64)))
            {

                return true;

            }

            return false;

        }

        public virtual bool CollideCharacters(Vector2 tile)
        {
            //------------- Collision check

            collideTimer = 180;

            foreach(Farmer farmer in currentLocation.farmers)
            {

                if(farmer.Tile == tile)
                {

                    return false;

                }

            }

            foreach (NPC NPChar in currentLocation.characters)
            {

                if (NPChar is StardewDruid.Character.Actor || /*NPChar is StardewDruid.Character.Dragon ||*/ NPChar == this || NPChar is StardewValley.Monsters.Monster)
                {

                    continue;

                }

                if (NPChar is StardewDruid.Character.Character Buddy)
                {

                    Vector2 check = Buddy.destination != Vector2.Zero ? Buddy.destination : Buddy.occupied;

                    if(tile == check)
                    {

                        if (Buddy.collidePriority > collidePriority)
                        {

                            return true;

                        }

                    }

                }
                else if(!NPChar.isMoving() && NPChar.Tile == tile)
                {

                    return true;

                }

            }

            return false;

        }

        // ========================================
        // ADJUST MODE
        // ========================================

        public virtual void WarpToEntrance()
        {

            ResetActives();

            ModUtility.AnimateQuickWarp(currentLocation, Position, true);

            Vector2 warppoint = new Vector2(-1);

            if(modeActive == mode.track)
            {

                if (Mod.instance.trackers[characterType].WarpToPlayer())
                {

                    attentionTimer = 360;

                    LookAtTarget(Mod.instance.trackers[characterType].followPlayer.Position);

                    ModUtility.AnimateQuickWarp(currentLocation, Position);

                    return;
                
                }

            }

            if (currentLocation is MineShaft)
            {

                warppoint = WarpData.WarpXZone(currentLocation);

                if (warppoint != Vector2.Zero)
                {

                    for(int i = 0; i < 5; i++)
                    {

                        if(i == 4)
                        {

                            WarpToDefault(false);

                        }

                        if(ModUtility.GroundCheck(currentLocation, new Vector2((int)(warppoint.X / 64), (int)(warppoint.Y / 64)),true) == "ground")
                        {

                            break;

                        }

                        warppoint += new Vector2(0, 64);

                    }

                    Position = warppoint;

                    SettleOccupied();

                    Mod.instance.Monitor.Log(Name + " warped to the entrance of " + currentLocation.DisplayName + " because they got stuck", LogLevel.Debug);

                    ModUtility.AnimateQuickWarp(currentLocation, Position);

                    return;

                }

            }
            else
            {

                warppoint = WarpData.WarpStart(currentLocation.Name);

                if (warppoint == Vector2.Zero)
                {

                    warppoint = WarpData.WarpEntrance(currentLocation, Position);

                }

                if (warppoint != Vector2.Zero)
                {

                    int centerDirection = ModUtility.DirectionToCenter(currentLocation, Position)[2];

                    Vector2 centerMovement = ModUtility.DirectionAsVector(centerDirection) * 64;

                    for (int i = 0; i < 5; i++)
                    {

                        if (i == 4)
                        {
                            
                            Mod.instance.Monitor.Log(Name + " warped home because they got stuck and couldnt find a warp point", LogLevel.Debug);

                            WarpToDefault(false);

                        }

                        Vector2 warppointTile = new Vector2((int)(warppoint.X / 64), (int)(warppoint.Y / 64));

                        string groundCheck = ModUtility.GroundCheck(currentLocation, warppointTile, true);

                        if (groundCheck == "ground")
                        {
                            
                            break;

                        }

                        warppoint += centerMovement;

                    }

                    Position = warppoint;

                    SettleOccupied();

                    if (currentLocation is not FarmCave)
                    {

                        Mod.instance.Monitor.Log(Name + " warped to the entrance of " + currentLocation.DisplayName + " because they got stuck", LogLevel.Debug);

                    }

                    ModUtility.AnimateQuickWarp(currentLocation, Position);

                    return;

                }

            }

            Mod.instance.Monitor.Log(Name + " warped home because they got stuck and couldnt find a warp point", LogLevel.Debug);

            WarpToDefault(false);

        }

        public virtual void WarpToDefault(bool updateAfter = true)
        {

            CharacterData.CharacterWarp(this, CharacterData.locations.home);

            SettleOccupied();

            if (updateAfter)
            {
                
                //ModUtility.LogStrings(new() { Name, "idle", "warp default" });
                
                TargetIdle(120);

                update(Game1.currentGameTime, currentLocation);

                return;

            }

            ResetActives();

        }

        public virtual void SwitchToMode(mode modechoice, Farmer player)
        {

            ResetActives();

            ResetTimers();

            netSceneActive.Set(false);

            netStandbyActive.Set(false);

            netFollowActive.Set(false);

            Mod.instance.trackers.Remove(characterType);

            switch (modechoice)
            {

                case mode.home:

                    modeActive = mode.random;

                    CharacterData.CharacterWarp(this, CharacterData.locations.home);

                    break;

                case mode.random:

                    modeActive = mode.random;

                    break;

                case mode.track:

                    Mod.instance.trackers.Add(characterType, new TrackHandle(characterType, player));

                    modeActive = mode.track;

                    netFollowActive.Set(true);

                    break;

                case mode.scene:

                    modeActive = mode.scene;

                    netSceneActive.Set(true);

                    netStandbyActive.Set(true);

                    break;

                case mode.roam:

                    CharacterData.CharacterWarp(this, CharacterData.locations.farm);

                    roamVectors.Clear();

                    roamIndex = 0;

                    roamLapse = Game1.currentGameTime.TotalGameTime.TotalMinutes + 1.0;

                    roamVectors = RoamAnalysis();

                    modeActive = mode.roam;

                    TetherMiddle();

                    break;

            }

        }

        public virtual List<Vector2> RoamAnalysis()
        {
            
            int layerWidth = currentLocation.map.Layers[0].LayerWidth;
            
            int layerHeight = currentLocation.map.Layers[0].LayerHeight;
            
            int num = layerWidth / 8;

            int midWidth = (layerWidth / 2) * 64;
            
            int fifthWidth = (layerWidth / 5) * 64;
            
            int nextWidth = (layerWidth * 64) - fifthWidth;
            
            int midHeight = (layerHeight / 2) * 64;
            
            int fifthHeight = (layerHeight / 5) * 64;
            
            int nextHeight = (layerHeight * 64) - fifthHeight;

            List<Vector2> roamList = new()
            {
                new(midWidth,midHeight),
                new(midWidth,midHeight),
                new(midWidth,midHeight),
                new(midWidth,midHeight),
                new(nextWidth,nextHeight),
                new(nextWidth,fifthHeight),
                new(fifthWidth,nextHeight),
                new(fifthWidth,fifthHeight),

            };

            if(currentLocation.IsOutdoors)
            {
                roamList = new()
                {
                    new(midWidth, midHeight),
                    new(midWidth, midHeight),
                    new(midWidth, midHeight),
                    new(midWidth, midHeight),
                    new(nextWidth, nextHeight),
                    new(nextWidth, fifthHeight),
                    new(fifthWidth, nextHeight),
                    new(fifthWidth, fifthHeight),
                    new(-1f),
                    new(-1f),
                    new(-1f),
                    new(-1f),
                };

            }

            List<Vector2> randomList = new();

            Random random = new Random();

            for(int i = 0; i < 12; i++)
            {

                if(roamList.Count == 0)
                {
                    
                    break;

                }

                int j = random.Next(roamList.Count);

                Vector2 randomVector = roamList[j];

                randomList.Add(randomVector);

                roamList.Remove(randomVector);

            }

            return randomList;

        }

        public virtual void DealDamageToMonster(StardewValley.Monsters.Monster monsterCharacter,int damage = -1,bool push = true)
        {

            if (!ModUtility.MonsterVitals(monsterCharacter, currentLocation))
            {

                return;

            }

            if (damage == -1)
            {

                damage = Mod.instance.CombatDamage() / 2;

            }
                
            List<int> pushList = new() { 0, 0 };

            if (push)
            {

                pushList = ModUtility.CalculatePush(monsterCharacter, Position);

            }

            ModUtility.HitMonster(currentLocation, Game1.player, monsterCharacter, damage, false, diffX: pushList[0], diffY: pushList[1]);

        }

        public virtual void SummonToPlayer(Vector2 position)
        {

            if (modeActive == mode.roam && currentLocation is Farm)
            {

                if (PathTarget(position, 2, 2))
                {

                    pathActive = pathing.player;

                    roamLapse = Math.Max(roamLapse,Game1.currentGameTime.TotalGameTime.TotalMinutes + 0.5);

                }

            }

        }

        public virtual void TetherMiddle()
        {
            
            tether = new((int)(currentLocation.map.Layers[0].LayerWidth / 2), (int)(currentLocation.map.Layers[0].LayerHeight / 2));


        }

        public virtual List<Chest> CaveChests()
        {

            List<Chest> chests = new();

            GameLocation farmcave = Game1.getLocationFromName("FarmCave");

            int chestCount = 0;

            foreach (Dictionary<Vector2, StardewValley.Object> dictionary in farmcave.Objects)
            {

                foreach (KeyValuePair<Vector2, StardewValley.Object> keyValuePair in dictionary)
                {

                    if (keyValuePair.Value is Chest foundChest)
                    {

                        chests.Add(foundChest);

                        if (chestCount == 2)
                        {

                            break;

                        }

                        chestCount++;

                    }

                }

            }

            return chests;

        }


    }

}
