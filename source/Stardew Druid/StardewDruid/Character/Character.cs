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
using StardewDruid.Map;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewDruid.Character
{
    public class Character : NPC
    {
        public List<Vector2> moveVectors;
        public int moveDirection;
        public int altDirection;
        public Dictionary<string, int> timers;
        public List<string> priorities;
        public float gait;
        public List<StardewValley.Monsters.Monster> targetOpponents;
        public int opponentThreshold;
        public int roamIndex;
        public List<Vector2> roamVectors;
        public double roamLapse;
        public List<Vector2> eventVectors;

        public Character()
        {
        }

        public Character(Vector2 position, string map, string Name)
          : base(CharacterData.CharacterSprite(Name), position, map, 2, Name, new Dictionary<int, int[]>(), CharacterData.CharacterPortrait(Name), false, null)
        {
            willDestroyObjectsUnderfoot = false;
            priorities = new List<string>();
            timers = new Dictionary<string, int>();
            moveVectors = new List<Vector2>();
            roamVectors = new List<Vector2>();
            eventVectors = new List<Vector2>();
            moveDirection = 0;
            targetOpponents = new();
            opponentThreshold = 640;
            gait = 1.2f;
            DefaultMap = map;
            DefaultPosition = position;
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
                DefaultMap = "FarmCave";
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
                return false;

            }
                
            foreach (NPC character in currentLocation.characters)
            {
                
                if (character is StardewValley.Monsters.Monster monster && (double)Vector2.Distance(Position, monster.Position) <= 1280.0)
                {
                    
                    return false;
               
                }
                    
            }

            Halt();

            faceGeneralDirection(who.Position, 0, false);

            moveDirection = FacingDirection;

            switch (moveDirection)
            {
                case 0:
                    moveUp = true;
                    break;
                case 1:
                    moveRight = true;
                    break;
                case 2:
                    moveDown = true;
                    break;
                default:
                    moveLeft = true;
                    break;
            }

            return true;

        }

        public override void performTenMinuteUpdate(int timeOfDay, GameLocation l)
        {
        }

        public override void behaviorOnFarmerPushing()
        {
            if (!Context.IsMainPlayer || priorities.Contains("frozen"))
            {
                
                return;

            }
                
            if (timers.ContainsKey("push"))
            {
                timers["push"] += 2;

                if (timers["push"] <= 10)
                {
                    return;
                }
                    
                moveVectors.Clear();

                TargetDirection(findPlayer().facingDirection, 2);

                timers.Remove("Push");

            }
            else
            {

                timers["push"] = 2;

            }
                
        }

        public override void Halt()
        {
            if (Context.IsMainPlayer)
            {
                moveVectors.Clear();
                timers.Clear();
                timers["stop"] = 60;
            }
            moveDown = false;
            moveLeft = false;
            moveRight = false;
            moveUp = false;
            Sprite.currentFrame -= Sprite.currentFrame % Sprite.framesPerAnimation;
            Sprite.UpdateSourceRect();
        }

        public virtual void normalUpdate(GameTime time, GameLocation location)
        {
            if (Sprite.loadedTexture == null || Sprite.loadedTexture.Length == 0)
            {
                Sprite.spriteTexture = CharacterData.CharacterTexture(Name);
                Sprite.loadedTexture = Sprite.textureName.Value;
                Portrait = CharacterData.CharacterPortrait(Name);
            }

            if (!Context.IsMainPlayer)
            {
                updateSlaveAnimation(time);
            }
            else
            {
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
            }
        }

        public override void update(GameTime time, GameLocation location)
        {

            normalUpdate(time, location);
            if (!Context.IsMainPlayer)
                return;
            for (int index = timers.Count - 1; index >= 0; --index)
            {
                KeyValuePair<string, int> keyValuePair = timers.ElementAt(index);
                timers[keyValuePair.Key]--;
                if (timers[keyValuePair.Key] <= 0)
                    timers.Remove(keyValuePair.Key);
            }
            if (!timers.ContainsKey("stop"))
            {
                if (moveVectors.Count > 0 && (double)Vector2.Distance(moveVectors.First(), Position) <= 16.0)
                    moveVectors.RemoveAt(0);
                if (moveVectors.Count <= 0)
                    UpdateTarget();
                MoveTowardsTarget(time);
            }
            Sprite.animateOnce(time);

        }

        public virtual Rectangle GetHitBox() => GetBoundingBox();

        public virtual void UpdateTarget()
        {
            if (Game1.IsClient)
            {
                
                return;

            }

            foreach (string priority in priorities)
            {
                
                switch (priority)
                {
                    
                    case "event":
                        
                        if (TargetEvent())
                        {

                            return;

                        }
                            
                        break;

                    case "frozen":
                    case "idle":

                        timers["stop"] = 1000;

                        Sprite.CurrentFrame = 0;

                        if (new Random().Next(2) == 0 || priorities.Contains("idle"))
                        {
                            
                            timers["idle"] = 1000;

                        }

                        return;

                    case "attack":

                        if (TargetOpponent())
                        {
                            return;
                        }

                        break;

                    case "track":

                        if (TargetTrack())
                        {

                            return;

                        }

                        break;

                    case "roam":

                        if (TargetRoam())
                        {

                            return;

                        }

                        break;
                
                }
            
            }

            TargetRandom();
        
        }

        public virtual bool TargetEvent()
        {
            if (eventVectors.Count <= 0)
            {
                return false;

            }

            Vector2 target = eventVectors.First();

            if (Vector2.Distance(target, Position) <= 32f)
            {
                
                ReachedEventPosition();
                
                return true;
            
            }
            
            VectorForTarget(target, 4f);

            return true;
        
        }

        public virtual bool TargetOpponent()
        {
            
            if (timers.ContainsKey("cooldown"))
            {
                return false;
            }
                
            for (int index = targetOpponents.Count - 1; index >= 0; --index)
            {
                
                if (!ModUtility.MonsterVitals(targetOpponents[index], currentLocation))
                {
                    
                    targetOpponents.RemoveAt(index);
                
                }  
                else if ((double)Vector2.Distance(targetOpponents[index].Position, Position) >= opponentThreshold)
                {
                    
                    targetOpponents.RemoveAt(index);
                
                }
                    
            }
            if (targetOpponents.Count == 0)
            {
                return false;
            }
                
            VectorForTarget(targetOpponents.First().Position);

            timers["attack"] = 120;

            return true;

        }

        public virtual bool TargetTrack()
        {
            if (!Mod.instance.trackRegister.ContainsKey(Name) || Mod.instance.trackRegister[Name].trackVectors.Count == 0)
                return false;
            float num = Vector2.Distance(Position, Game1.player.Position);
            if ((double)num <= 180.0 && !timers.ContainsKey("track"))
                return false;
            if ((double)Vector2.Distance(Mod.instance.trackRegister[Name].trackVectors.First<Vector2>(), Position) >= 180.0)
                WarpToTarget();
            if (Mod.instance.trackRegister[Name].trackVectors.Count == 0)
            {
                if (new Random().Next(2) != 0)
                    return false;
                timers["stop"] = 300;
                timers["idle"] = 300;
                return true;
            }
            VectorForTarget(Mod.instance.trackRegister[Name].NextVector(), -1f, false);
            timers["track"] = 120;
            if ((double)num > 480.0)
                timers["sprint"] = 180;
            else
                timers["hurry"] = 120;
            return true;
        }

        public virtual void WarpToTarget()
        {
            if (currentLocation.Name != Game1.player.currentLocation.Name)
            {
                Halt();
                currentLocation.characters.Remove(this);
                currentLocation = Game1.player.currentLocation;
                currentLocation.characters.Add(this);
            }
            if (Mod.instance.trackRegister[Name].trackVectors.Count > 0)
            {
                Mod.instance.trackRegister[Name].TruncateTo(3);
                Position = Mod.instance.trackRegister[Name].NextVector();
            }
            else
            {

                Position = new Vector2(Position.X, Position.Y + 64f);//Vector2.op_Addition(((StardewValley.Character)Game1.player).Position, new Vector2(0.0f, 64f));

            }

            Vector2 warpPosition = new(Position.X, Position.Y + 32f);

            ModUtility.AnimateQuickWarp(currentLocation, warpPosition, "Solar");
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
            float num = Vector2.Distance(roamVector, Position);
            if ((double)num <= 120.0)
            {
                ReachedRoamPosition();
                UpdateRoam(true);
                return true;
            }
            if ((double)num >= 1200.0)
                timers["hurry"] = 300;
            UpdateRoam();
            VectorForTarget(roamVectors[roamIndex], 4f);
            return true;
        }

        public void UpdateRoam(bool reset = false)
        {
            if (!(roamLapse < Game1.currentGameTime.TotalGameTime.TotalMinutes | reset))
                return;
            roamLapse = Game1.currentGameTime.TotalGameTime.TotalMinutes + 1.0;
            ++roamIndex;
            if (roamIndex == roamVectors.Count)
                roamIndex = 0;
        }

        public void VectorForTarget(Vector2 target, float ahead = 1.5f, bool check = true)
        {

            Vector2 moveTarget = target;

            float distance = Vector2.Distance(Position, target);
            
            Vector2 difference = new(target.X - Position.X, target.Y - Position.Y);//Vector2.op_Subtraction(target, Position);
            
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
            
            if (distance >= 128.0 & check)
            {
                
                float checkAhead = Math.Min(64f * ahead, distance);
                
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
            
            moveVectors.Add(moveTarget);

        }

        public virtual void TargetRandom()
        {
            
            moveVectors.Clear();
            
            timers.Clear();
            
            Random random = new Random();

            int randomInt = random.Next(4);

            switch(randomInt)
            {
                case 0:
                case 1:
                case 2:

                    TargetDirection(random.Next(4), random.Next(1, 4));

                    break;

                case 3:
                case 4:

                    Halt();

                    break;

                case 5:

                    Halt();

                    timers["stop"] = 400;

                    timers["idle"] = 400;

                    break;

            }
            
        }

        public virtual void TargetDirection(int direction, int distance)
        {
            
            int num = 64 * distance;
            
            Vector2 vector2;
            
            switch (direction)
            {
                case 0:
                    vector2 = new(Position.X, Position.Y - num);
                    break;
                case 1:
                    vector2 = new(Position.X + num, Position.Y);
                    break;
                case 2:
                    vector2 = new(Position.X, Position.Y + num);
                    break;
                default:
                    vector2 = new(Position.X - num, Position.Y);
                    break;
            }
            
            moveDirection = direction;

            altDirection = moveDirection + 1;

            moveVectors.Add(vector2);

        }

        public virtual void MoveTowardsTarget(GameTime time)
        {
            if (Game1.IsClient)
            {
                return;
            }

            if (moveVectors.Count == 0)
            {
                Sprite.currentFrame -= Sprite.currentFrame % Sprite.framesPerAnimation;
                Sprite.UpdateSourceRect();

                return;

            }

            //------------- Factors

            Vector2 nextPosition = moveVectors.First();

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

            if (timers.ContainsKey("sprint") || timers.ContainsKey("attack"))
            {
                moveSpeed = gait * 5;

            }
            else if (timers.ContainsKey("hurry"))
            {

                moveSpeed = gait * 2;

            }

            Vector2 movement = factorVector * moveSpeed;

            if (Vector2.Distance(nextPosition, Position) <= (moveSpeed * 1.25))
            {

                movement = diffPosition;

            }

            if (timers.ContainsKey("force"))
            {

                Position += movement;

                AnimateMovement(time);

                return;

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

            float monsterDistance;

            float closestDistance = 9999f;

            foreach (NPC nonPlayableCharacter in currentLocation.characters)
            {

                Microsoft.Xna.Framework.Rectangle boundingBox2 = nonPlayableCharacter.GetBoundingBox();

                if (nonPlayableCharacter is StardewValley.Monsters.Monster monsterCharacter)
                {

                    if (monsterCharacter.Health > 0 && !monsterCharacter.IsInvisible)
                    {

                        if (boundingBox2.Intersects(hitBox))
                        {

                            damageMonsters.Add(monsterCharacter);


                        }

                        if (!timers.ContainsKey("attack")) {

                            monsterDistance = Vector2.Distance(Position, monsterCharacter.Position);

                            if (monsterDistance < opponentThreshold)
                            {

                                if (monsterDistance < closestDistance)
                                {

                                    closestDistance = monsterDistance;

                                    targetOpponents.Clear();

                                    targetOpponents.Add(monsterCharacter);

                                }

                            }
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

            if (damageMonsters.Count > 0 && !timers.ContainsKey("cooldown"))
            {

                foreach (StardewValley.Monsters.Monster monsterCharacter in damageMonsters)
                {

                    DealDamageToMonster(monsterCharacter);

                }

                timers["attack"] = 10;

                timers["cooldown"] = 120;

                return;

            }

            if (collision && !timers.ContainsKey("collide"))
            {

                TargetRandom();

                timers.Add("collide", 120);

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

            AnimateMovement(time);

        }

        public virtual void AnimateMovement(GameTime time)
        {
            moveDown = false;
            moveLeft = false;
            moveRight = false;
            moveUp = false;
            FacingDirection = moveDirection;
            switch (moveDirection)
            {
                case 0:
                    moveUp = true;
                    Sprite.AnimateUp(time, 0, "");
                    break;
                case 1:
                    moveRight = true;
                    Sprite.AnimateRight(time, 0, "");
                    break;
                case 2:
                    moveDown = true;
                    Sprite.AnimateDown(time, 0, "");
                    break;
                default:
                    moveLeft = true;
                    Sprite.AnimateLeft(time, 0, "");
                    break;
            }
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

        public virtual void SwitchEventMode()
        {
            
            SwitchDefaultMode();

            priorities = new List<string>()
            {
                "event",
                "frozen"
            };

        }

        public virtual void SwitchFrozenMode()
        {
            SwitchDefaultMode();
            priorities = new List<string>() { "frozen" };
        }

        public virtual void SwitchFollowMode()
        {
            SwitchDefaultMode();
            Mod.instance.trackRegister.Add(Name, new TrackHandle(Name));
            priorities = new List<string>()
              {
                "attack",
                "track"
              };
        }

        public virtual void SwitchRoamMode()
        {
            
            SwitchDefaultMode();

            roamVectors.Clear();
            
            roamLapse = Game1.currentGameTime.TotalGameTime.TotalMinutes + 1.0;
            
            priorities = new List<string>() { "roam" };
        
        }

        public virtual void SwitchDefaultMode()
        {
            Halt();
            priorities = new List<string>();
            Mod.instance.trackRegister.Remove(Name);
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

        public virtual void DealDamageToMonster(StardewValley.Monsters.Monster monsterCharacter,bool kill = false,int damage = -1,bool push = true)
        {
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

        public virtual void ReachedEventPosition()
        {

            Halt();
            
            eventVectors.RemoveAt(0);
        
        }

        public virtual void ReachedRoamPosition()
        {
        }

        public virtual void ReachedIdlePosition()
        {
            
            Halt();
            
            timers["stop"] = 1000;
            
            timers["idle"] = 1000;
        
        }

        public virtual void LeftClickAction(SButton Button)
        {
        }

        public virtual void RightClickAction(SButton Button)
        {
        }

        public virtual void ShutDown()
        {
        }

        public virtual void PlayerBusy()
        {
        }
    }
}
