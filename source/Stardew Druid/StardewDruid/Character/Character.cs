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
using StardewValley.Menus;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewDruid.Character
{
    public class Character : NPC
    {

        //public List<Vector2> moveVectors;
        //public Dictionary<string, int> timers;
        //public List<string> priorities;

        //public List<StardewValley.Monsters.Monster> targetOpponents;

        public List<Vector2> targetVectors;
        public float gait;
        public int opponentThreshold;
        public List<Vector2> roamVectors;
        public int roamIndex;
        public double roamLapse;
        public List<Vector2> eventVectors;
        public List<Event.BarrageHandle> barrages;
        public bool loadedOut;

        public enum mode
        {
            scene,
            track,
            standby,
            roam,
            random,
        }

        public mode modeActive;

        public enum behaviour
        {
            idle,
            follow,
            hurry,
            dash,
            special,
            barrage,
        }

        public behaviour behaviourActive;

        public Dictionary<int, List<Rectangle>> walkFrames;
        public Dictionary<int, List<Rectangle>> dashFrames;
        public Dictionary<int,Rectangle> haltFrames;
        public Dictionary<int,Rectangle> specialFrames;

        public int idleTimer;
        public int idleInterval;
        public NetInt idleFrame = new NetInt(0);

        public int moveTimer;
        public int moveLength;
        public int moveInterval;
        public NetInt moveFrame = new NetInt(0);

        public int specialTimer;
        public int specialInterval;
        public NetInt specialFrame = new NetInt(0);

        public int cooldownTimer;
        public int hitTimer;

        public NetInt netDirection = new NetInt(0);
        
        public NetInt netAlternative = new NetInt(0);

        public NetBool netSpecialActive = new NetBool(false);

        public NetBool netDashActive = new NetBool(false);

        public NetBool netHaltActive = new NetBool(false);

        public NetBool netFollowActive = new NetBool(false);

        public NetBool netStandbyActive = new NetBool(false);

        public Character()
        {
        }

        public Character(Vector2 position, string map, string Name)
          : base(CharacterData.CharacterSprite(Name), position, map, 2, Name, new Dictionary<int, int[]>(), CharacterData.CharacterPortrait(Name), false, null)
        {
            
            willDestroyObjectsUnderfoot = false;
            
            DefaultMap = map;
            
            DefaultPosition = position;
            
            HideShadow = true;
            
            LoadOut();
        
        }


        protected override void initNetFields()
        {
            base.initNetFields();
            NetFields.AddFields(new INetSerializable[10]
            {
                 netDirection,
                 netAlternative,
                 idleFrame,
                 moveFrame,
                 specialFrame,
                 netSpecialActive,
                 netDashActive,
                 netHaltActive,
                 netFollowActive,
                 netStandbyActive,

            });
        }

        public virtual void LoadOut()
        {
            
            barrages = new();

            roamVectors = new List<Vector2>();
            
            eventVectors = new List<Vector2>();

            targetVectors = new();
            
            opponentThreshold = 640;
            
            gait = 1.2f;

            modeActive = mode.random;

            behaviourActive = behaviour.idle;

            idleInterval = 90;

            moveLength = 4;

            moveInterval = 12;

            specialInterval = 30;

            walkFrames = WalkFrames(32, 16);

            dashFrames = walkFrames;

            haltFrames = new();

            specialFrames = new();

            loadedOut = true;

        }

        public virtual Dictionary<int, List<Rectangle>> WalkFrames(int height, int width, int startX = 0, int startY = 0)
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
                
                for (int index = 0; index < moveLength; index++)
                {
                    
                    Rectangle rectangle = new(startX, startY, width, height);
                    
                    rectangle.X += width * index;
                    
                    rectangle.Y += height * keyValuePair.Value;
                    
                    walkFrames[keyValuePair.Key].Add(rectangle);
                
                }

            }

            return walkFrames;

        }


        public override void draw(SpriteBatch b, float alpha = 1f)
        {

            if (IsInvisible || !Utility.isOnScreen(Position, 128))
            {
                return;
            }

            if (base.IsEmoting && !Game1.eventUp)
            {
                Vector2 localPosition2 = getLocalPosition(Game1.viewport);
                localPosition2.Y -= 32 + Sprite.SpriteHeight * 4;
                b.Draw(Game1.emoteSpriteSheet, localPosition2, new Microsoft.Xna.Framework.Rectangle(base.CurrentEmoteIndex * 16 % Game1.emoteSpriteSheet.Width, base.CurrentEmoteIndex * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, getStandingY() / 10000f);
            }

            Vector2 localPosition = getLocalPosition(Game1.viewport);


            b.Draw(
                Sprite.Texture,
                localPosition + new Vector2(32f, 16f),
                walkFrames[netDirection.Value][moveFrame.Value],
                Color.White,
                0f,
                new Vector2(Sprite.SpriteWidth / 2, Sprite.SpriteHeight * 3f / 4f),
                Math.Max(0.2f, scale) * 4f,
                flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                drawOnTop ? 0.991f : ((float)getStandingY() / 10000f)
                );

            b.Draw(
                Game1.shadowTexture,
                localPosition + new Vector2(32f, 40f),
                Game1.shadowTexture.Bounds,
                Color.White * 0.65f,
                0f,
                new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y),
                4f,
                SpriteEffects.None,
                Math.Max(0.0f, (getStandingY() / 10000f) - 0.0001f)
                );


        }

        public override Rectangle GetBoundingBox()
        {
            return new Rectangle((int)Position.X + 8, (int)Position.Y, 48, 64);
        }

        public virtual Rectangle GetHitBox()
        {
            return GetBoundingBox();
        }
        
        public override void reloadSprite()
        {
            Sprite = CharacterData.CharacterSprite(Name);
            Portrait = CharacterData.CharacterPortrait(Name);
        }

        public override void reloadData()
        {
            CharacterDisposition characterDisposition = CharacterData.CharacterDisposition(Name);
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
            DefaultMap = Mod.instance.CharacterMap(Name);
            if (DefaultMap == null)
            {
                DefaultMap = "FarmCave";
            }
            DefaultPosition = CharacterData.CharacterPosition(DefaultMap);
        }

        protected override string translateName(string name) => name;

        public override void tryToReceiveActiveObject(Farmer who)
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

            if(netDashActive.Value || netSpecialActive.Value)
            {

                return false;

            }

            Halt();

            NextTarget(who.Position);

            ResetAll();

            return true;

        }

        public override void Halt()
        {

            netHaltActive.Set(true);

        }

        public virtual void ResetActives()
        {
            
            behaviourActive = behaviour.idle;

            netDirection.Set(0);

            netAlternative.Set(0);

            netHaltActive.Set(false);

            idleTimer = 0;

            idleFrame.Set(0);

            netDashActive.Set(false);

            moveTimer = 0;

            moveFrame.Set(0);

            netSpecialActive.Set(false);

            specialTimer = 0;

            specialFrame.Set(0);

            cooldownTimer = 0;

            hitTimer = 0;

            targetVectors.Clear();

        }

        public virtual void ResetAll()
        {

            idleFrame.Set(0);

            moveFrame.Set(0);

            specialFrame.Set(0);

            targetVectors.Clear();

        }

        public void NextTarget(Vector2 target, float span = 1.5f)
        {

            int moveDirection;

            int altDirection;

            Vector2 moveTarget = target;

            float distance = Vector2.Distance(Position, target);

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

            if (distance >= 128.0 & span > 0)
            {

                float checkAhead = Math.Min(64f * span, distance);

                Vector2 checkTarget;

                if (absoluteX > absoluteY)
                {

                    float ratio = absoluteY / absoluteX;

                    float checkX = signX * checkAhead;

                    float checkY = ratio * signY * checkAhead;

                    checkTarget = new(checkX, checkY);

                }
                else
                {

                    float ratio = absoluteX / absoluteY;

                    float checkX = ratio * signX * checkAhead;

                    float checkY = signY * checkAhead;

                    checkTarget = new(checkX, checkY);

                }

                moveTarget = new Vector2(Position.X + checkTarget.X, Position.Y + checkTarget.Y);

            }

            netDirection.Set(moveDirection);

            netAlternative.Set(altDirection);

            targetVectors.Clear();

            targetVectors.Add(moveTarget);

        }

        public virtual void DirectTarget(int direction, int distance)
        {

            int num = 64 * distance;

            int alt = 1;

            Vector2 vector2;

            if (new Random().Next(2) == 0)
            {

                alt = -1;

            }

            switch (direction)
            {
                case 0:
                    vector2 = new(Position.X + alt, Position.Y - num);
                    break;
                case 1:
                    vector2 = new(Position.X + num, Position.Y + alt);
                    break;
                case 2:
                    vector2 = new(Position.X + alt, Position.Y + num);
                    break;
                default:
                    vector2 = new(Position.X - num, Position.Y + alt);
                    break;
            }

            netDirection.Set(direction);

            int altDirection = alt < 0 ? 3 : 1;

            netAlternative.Set(altDirection);

            targetVectors.Clear();

            targetVectors.Add(vector2);

        }

        public override void performTenMinuteUpdate(int timeOfDay, GameLocation l)
        {

        }

        public override void behaviorOnFarmerPushing()
        {

            if(netHaltActive.Value)
            {
                
                return;

            }

            DirectTarget(findPlayer().facingDirection, 2);

        }

        public virtual void ApplyTexture()
        {

            Sprite.spriteTexture = CharacterData.CharacterTexture(Name);

            Sprite.loadedTexture = Sprite.textureName.Value;

            Sprite.UpdateSourceRect();

            Portrait = CharacterData.CharacterPortrait(Name);

        }

        public virtual void normalUpdate(GameTime time, GameLocation location)
        {

            if (Sprite.loadedTexture != Sprite.textureName.Value)
            {
                
                ApplyTexture();
            
            }

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
                
                return;

            }

            UpdateBehaviour();

            if (targetVectors.Count <= 0 && !netHaltActive.Value && !netSpecialActive.Value)
            {

                UpdateTarget();

            }

            MoveTowardsTarget();

        }

        public virtual void UpdateBehaviour()
        {

            if(netHaltActive.Value && behaviourActive != behaviour.idle)
            {
                
                behaviourActive = behaviour.idle;
                
                idleTimer = 540;

            }

            UpdateIdle();

            UpdateMove();

            UpdateSpecial();

            cooldownTimer--;

            hitTimer--;

        }

        public virtual void UpdateIdle()
        {

            if (behaviourActive == behaviour.idle || netHaltActive.Value)
            {

                idleTimer--;

                if (idleTimer <= 0)
                {

                    netHaltActive.Set(false);

                    idleFrame.Set(0);

                    idleTimer = 540;

                }

                if (idleTimer % idleInterval == 0)
                {

                    int nextFrame = idleFrame.Value + 1;

                    idleFrame.Set(nextFrame);

                }

            }

        }

        public virtual void UpdateMove()
        {

            if (targetVectors.Count > 0)
            {

                if (moveTimer <= 0)
                {

                    moveTimer = moveInterval;

                    if(behaviourActive == behaviour.hurry)
                    {

                        moveTimer -= 3;

                    }

                    int nextFrame = moveFrame.Value + 1;

                    if(nextFrame >= moveLength)
                    {

                        nextFrame = 0;

                    }

                    moveFrame.Set(nextFrame);

                }

                moveTimer--;

            }

            if (netDashActive.Value && targetVectors.Count == 0)
            {

                netDashActive.Set(false);

            }

        }

        public virtual void UpdateSpecial()
        {

            if (netSpecialActive.Value)
            {
                
                specialTimer--;

                if (specialTimer % specialInterval == 0)
                {

                    int nextFrame = specialFrame.Value + 1;

                    specialFrame.Set(nextFrame);

                }

                if (specialTimer <= 0)
                {

                    netSpecialActive.Set(false);

                    specialFrame.Set(0);

                    behaviourActive = behaviour.idle;

                    cooldownTimer = 120;

                }

            }

            if (barrages.Count > 0)
            {

                UpdateBarrages();

            }

        }

        public virtual void UpdateTarget()
        {

            switch (modeActive)
            {

                case mode.scene:
                case mode.standby:

                    if (TargetEvent()) { 

                        return; 

                    };

                    if (TargetIdle()) { 

                        return; 

                    };

                    break;

                case mode.track:

                    if (TargetMonster())
                    {

                        return;

                    }

                    if (TargetTrack()) { 

                        return; 

                    };

                    break;

                case mode.roam:

                    if (TargetRoam()) { 

                        return; 

                    };

                    break;

            }

            TargetRandom();

        }

        public virtual bool TargetEvent()
        {
            if (eventVectors.Count <= 0)
            {
                return false;

            }

            Vector2 target = eventVectors.ElementAt(0);

            if (Vector2.Distance(target, Position) <= 32f)
            {
                
                ReachedEventPosition();

                eventVectors.RemoveAt(0);

                return true;
            
            }
            
            NextTarget(target, 4f);

            return true;
        
        }

        public virtual bool TargetIdle()
        {
            
            behaviourActive = behaviour.idle;

            idleTimer = 600;

            ResetAll();

            return true;
        
        }

        public virtual bool TargetMonster()
        {

            if(cooldownTimer > 0)
            {

                return false;

            }

            float monsterDistance;

            float closestDistance = 9999f;

            List<StardewValley.Monsters.Monster> targetMonsters = new();

            foreach (NPC nonPlayableCharacter in currentLocation.characters)
            {

                Microsoft.Xna.Framework.Rectangle boundingBox2 = nonPlayableCharacter.GetBoundingBox();

                if (nonPlayableCharacter is StardewValley.Monsters.Monster monsterCharacter)
                {

                    if (currentLocation is SlimeHutch)
                    {
                        continue;
                    }

                    if (monsterCharacter.Health > 0 && !monsterCharacter.IsInvisible)
                    {

                        monsterDistance = Vector2.Distance(Position, monsterCharacter.Position);

                        if (monsterDistance < opponentThreshold)
                        {

                            if (monsterDistance < closestDistance)
                            {

                                closestDistance = monsterDistance;

                                targetMonsters.Clear();

                                targetMonsters.Add(monsterCharacter);


                            }

                        }

                    }

                    continue;

                }

            }

            if(targetMonsters.Count > 0)
            {

                return MonsterAttack(targetMonsters.First());

            }

            return false;

        }

        public virtual bool MonsterAttack(StardewValley.Monsters.Monster monster)
        {

            float num = Vector2.Distance(Position, monster.Position);

            behaviourActive = behaviour.dash;

            moveTimer = moveInterval;

            netDashActive.Set(true);

            NextTarget(monster.Position, -1);

            return true;

        }

        public virtual bool TargetTrack()
        {
            if (!Mod.instance.trackRegister.ContainsKey(Name) || Mod.instance.trackRegister[Name].trackVectors.Count == 0)
            {
                return false;
            }

            float num = Vector2.Distance(Position, Mod.instance.trackRegister[Name].trackVectors.Last());
            
            if (num <= 180f && behaviourActive != behaviour.follow)
            {
            
                return false;
            
            }
            else if (num >= 512f)
            {

                behaviourActive = behaviour.dash;

                moveTimer = moveInterval;

                netDashActive.Set(true);

                NextTarget(Mod.instance.trackRegister[Name].trackVectors.Last(),-1);

                Mod.instance.trackRegister[Name].trackVectors.Clear();

                return true;

            }
            
            NextTarget(Mod.instance.trackRegister[Name].NextVector(), -1);

            behaviourActive = behaviour.follow;

            moveTimer = moveInterval;

            return true;

        }

        public virtual bool TargetRoam()
        {
            if (roamVectors.Count == 0)
            {
                
                roamLapse = Game1.currentGameTime.TotalGameTime.TotalMinutes + 1.0;
                
                roamVectors = RoamAnalysis();
            
            }
            
            Vector2 roamVector = roamVectors[roamIndex];
            
            if (roamVector == new Vector2(-1f))
            {
                
                ReachedIdlePosition();
                
                UpdateRoam(true);
                
                return true;
            
            }

            UpdateRoam();

            float num = Vector2.Distance(roamVector, Position);
            
            if ((double)num <= 120.0)
            {
                
                ReachedRoamPosition();
                
                UpdateRoam(true);
                
                return true;
            
            }
            
            behaviourActive = behaviour.follow;

            moveTimer = moveInterval;

            if ((double)num >= 1200.0)
            {

                behaviourActive = behaviour.hurry;

            }

            NextTarget(roamVectors[roamIndex], 4f);

            return true;
        
        }

        public void UpdateRoam(bool reset = false)
        {
            if (!(roamLapse < Game1.currentGameTime.TotalGameTime.TotalMinutes | reset))
            {
                return;
            }
            
            roamLapse = Game1.currentGameTime.TotalGameTime.TotalMinutes + 1.0;
            
            roamIndex++;
            
            if (roamIndex == roamVectors.Count)
            {
                roamIndex = 0;

            }

        }

        public virtual void TargetRandom()
        {
            
            targetVectors.Clear();
            
            Random random = new Random();

            int rand = netFollowActive.Value ? 3 : 5;

            int randomInt = random.Next(rand);

            switch(randomInt)
            {
                case 0:
                case 1:
                case 2:

                    DirectTarget(random.Next(4), random.Next(1, 4));

                    behaviourActive = behaviour.follow;

                    moveTimer = moveInterval;

                    break;

                case 3:
                case 4:

                    Halt();

                    behaviourActive = behaviour.idle;

                    idleTimer = 300;

                    ResetAll();

                    break;

            }
            
        }

        public virtual void MoveTowardsTarget()
        {

            if(targetVectors.Count == 0)
            {
                return;
            }

            //------------- Factors

            Vector2 nextPosition = targetVectors.First();

            Vector2 diffPosition = nextPosition - Position;

            float absX = Math.Abs(diffPosition.X); // x position

            float absY = Math.Abs(diffPosition.Y); // y position

            int signX = diffPosition.X < 0.001f ? -1 : 1; // x sign

            int signY = diffPosition.Y < 0.001f ? -1 : 1; // y sign

            float moveX = signX;

            float moveY = signY;

            if (absX > absY)
            {

                moveY = (absY < 0.05f) ? 0 : (int)(absY / absX * signY);

            }
            else
            {
                moveX = (absX < 0.05f) ? 0 : (int)(absX / absY * signX);

            }

            Vector2 factorVector = new(moveX, moveY);

            float moveSpeed = gait;

            if (behaviourActive == behaviour.dash)
            {
                moveSpeed = gait * 5;

            }
            else if (behaviourActive == behaviour.hurry)
            {

                moveSpeed = gait * 2;

            }

            Vector2 movement = factorVector * moveSpeed;

            if (Vector2.Distance(nextPosition, Position) <= (moveSpeed * 1.25))
            {

                movement = diffPosition;

                targetVectors.Clear();

            }

            //------------- Collision check

            Microsoft.Xna.Framework.Rectangle boundingBox = GetBoundingBox();


            boundingBox.X += (int)movement.X;
            boundingBox.Y += (int)movement.Y;

            Microsoft.Xna.Framework.Rectangle farmerBox = Game1.player.GetBoundingBox();

            bool collision = false;

            if (farmerBox.Intersects(boundingBox))
            {

                collision = true;

            }

            Rectangle hitBox = GetHitBox();
            
            hitBox.X += (int)movement.X;
            
            hitBox.Y += (int)movement.Y;

            List<StardewValley.Monsters.Monster> damageMonsters = new();

            foreach (NPC nonPlayableCharacter in currentLocation.characters)
            {

                Microsoft.Xna.Framework.Rectangle boundingBox2 = nonPlayableCharacter.GetBoundingBox();

                if (nonPlayableCharacter is StardewValley.Monsters.Monster monsterCharacter)
                {

                    if (currentLocation is SlimeHutch)
                    {
                        continue;
                    }

                    if (monsterCharacter.Health > 0 && !monsterCharacter.IsInvisible && hitTimer <= 0)
                    {

                        if (boundingBox2.Intersects(hitBox))
                        {

                            damageMonsters.Add(monsterCharacter);


                        }

                    }

                    continue;

                }

                if (nonPlayableCharacter != this)
                {

                    if (boundingBox2.Intersects(boundingBox))
                    {

                        collision = true;

                    }

                }

            }

            if (damageMonsters.Count > 0)
            {

                foreach (StardewValley.Monsters.Monster monsterCharacter in damageMonsters)
                {

                    HitMonster(monsterCharacter);

                }

                hitTimer = 120;

                return;

            }

            if (collision && behaviourActive == behaviour.idle)
            {

                TargetRandom();

                return;

            }

            //------------- Tile check

            Vector2 nextSpace = Position + (factorVector * 64);

            Vector2 thisTile = new((int)(Position.X / 64), (int)(Position.Y / 64));

            Vector2 nextTile = new((int)(nextSpace.X / 64), (int)(nextSpace.Y / 64));

            if (thisTile != nextTile)
            {

                if (ModUtility.GroundCheck(currentLocation, nextTile, npc:true) != "ground")
                {

                    TargetRandom();

                    return;

                }

            }

            //-------------------------- commit movement

            Position += movement;

        }

        public virtual void WarpToDefault()
        {
            
            Halt();
            
            if (currentLocation.Name != DefaultMap)
            {
                
                currentLocation.characters.Remove(this);
                
                currentLocation = Game1.getLocationFromName(DefaultMap);
                
                currentLocation.characters.Add(this);
            
            }
            
            Position = DefaultPosition;
            
            update(Game1.currentGameTime, currentLocation);
       
        }

        public virtual void SwitchSceneMode()
        {
            
            SwitchDefaultMode();

            modeActive = mode.scene;

        }

        public virtual void SwitchFollowMode(Farmer follow = null)
        { 

            SwitchDefaultMode();

            netFollowActive.Set(true);

            netStandbyActive.Set(false);

            Mod.instance.trackRegister.Add(Name, new TrackHandle(Name,follow));

            modeActive = mode.track;

        }

        public virtual void ActivateStandby()
        {
            Mod.instance.trackRegister[Name].standby = true;

            netStandbyActive.Set(true);

            modeActive = mode.standby;

        }

        public virtual void DeactivateStandby()
        {
            Mod.instance.trackRegister[Name].standby = false;

            netStandbyActive.Set(false);

            modeActive = mode.track;

        }

        public virtual void SwitchRoamMode()
        {
            
            SwitchDefaultMode();

            roamVectors.Clear();
            
            roamLapse = Game1.currentGameTime.TotalGameTime.TotalMinutes + 1.0;

            modeActive = mode.roam;

        }

        public virtual void SwitchDefaultMode()
        {
            modeActive = mode.random;

            Mod.instance.trackRegister.Remove(Name);

            netFollowActive.Set(false);

            netStandbyActive.Set(false);

        }

        public virtual List<Vector2> RoamAnalysis()
        {
            
            int layerWidth = currentLocation.map.Layers[0].LayerWidth;
            
            int layerHeight = currentLocation.map.Layers[0].LayerHeight;
            
            int num = layerWidth / 8;

            int midWidth = (layerWidth / 2) * 64;
            
            int eighthWidth = (layerWidth / 8) * 64;
            
            int nextWidth = (layerWidth * 64) - eighthWidth;
            
            int midHeight = (layerHeight / 2) * 64;
            
            int eighthHeight = (layerHeight / 8) * 64;
            
            int nextHeight = (layerHeight * 64) - eighthHeight;

            List<Vector2> roamList = new()
            {
                new(midWidth,midHeight),
                new(midWidth,midHeight),
                new(midWidth,midHeight),
                new(midWidth,midHeight),
                new(nextWidth,nextHeight),
                new(nextWidth,eighthHeight),
                new(eighthWidth,nextHeight),
                new(eighthWidth,eighthHeight),

            };

            if(currentLocation.IsOutdoors)
            {
                roamList = new()
                {
                    new(midWidth, midHeight),
                    new(midWidth,midHeight),
                    new(midWidth,midHeight),
                    new(midWidth,midHeight),
                    new(nextWidth,nextHeight),
                    new(nextWidth,eighthHeight),
                    new(eighthWidth,nextHeight),
                    new(eighthWidth,eighthHeight),
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

                randomList.Add(roamList[random.Next(randomList.Count)]);

            }

            return randomList;

        }

        public virtual void HitMonster(StardewValley.Monsters.Monster monsterCharacter)
        {

            DealDamageToMonster(monsterCharacter);

        }

        public virtual void DealDamageToMonster(StardewValley.Monsters.Monster monsterCharacter,bool kill = false,int damage = -1,bool push = true)
        {

            if (!ModUtility.MonsterVitals(monsterCharacter, currentLocation))
            {

                return;

            }

            if (damage == -1)
            {
                damage = Mod.instance.DamageLevel() / 2;

            }
                
            if (!kill)
            {

                damage = Math.Min(damage, monsterCharacter.Health - 1);

            }

            List<int> pushList = new() { 0, 0 };

            if (push)
            {

                pushList = ModUtility.CalculatePush(currentLocation, monsterCharacter, Position);

            }

            ModUtility.HitMonster(currentLocation, Game1.player, monsterCharacter, damage, false, diffX: pushList[0], diffY: pushList[1]);

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

        public virtual void ReachedEventPosition()
        {

            Halt();

            behaviourActive = behaviour.idle;

            idleTimer = 1200;

            ResetAll();

        }

        public virtual void ReachedRoamPosition()
        {
        }

        public virtual void ReachedIdlePosition()
        {
            
            Halt();

            behaviourActive = behaviour.idle;

            idleTimer = 1200;

            ResetAll();

        }

    }

}
