/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using xTile.Dimensions;
using System.Xml.Serialization;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewModdingAPI;
using System.IO;
using xTile;
using System.Threading;
using StardewValley.Locations;
using static System.Net.Mime.MediaTypeNames;
using System.Net.WebSockets;
using StardewDruid.Map;
using Microsoft.VisualBasic;
using static StardewValley.Minigames.TargetGame;
using StardewValley.Network;

namespace StardewDruid.Character
{
    public class Character : StardewValley.NPC
    {

        //---------------------------------------------------------
        // Behaviour
        //---------------------------------------------------------

        public List<Vector2> moveVectors;

        //public Dictionary<int, float> moveData;

        public int moveDirection;

        public int altDirection;

        public Dictionary<string, int> timers;

        public List<string> priorities;

        public float gait;

        //---------------------------------------------------------
        // Opponent
        //---------------------------------------------------------

        public List<StardewValley.Monsters.Monster> targetOpponents;

        public int opponentThreshold;

        //---------------------------------------------------------
        // Roam Mode
        //---------------------------------------------------------

        public int roamIndex;

        public List<Vector2> roamVectors;

        public double roamLapse;

        public Character()
        {

        }

        public Character(Vector2 position, string map, string Name)
            : base(
            CharacterData.CharacterSprite(Name),
            position,
            map,
            2,
            Name,
            new(),
            CharacterData.CharacterPortrait(Name),
            false
            )
        {

            willDestroyObjectsUnderfoot = false;

            roamVectors = new();

            priorities = new();

            timers = new();

            moveVectors = new();

            moveDirection = 0;

            targetOpponents = new();

            opponentThreshold = 640;

            roamLapse = Game1.currentGameTime.TotalGameTime.TotalMinutes;

            gait = 1.2f;

        }

        // =================================================
        // Base Overrides
        // =================================================

        public override void reloadSprite()
        {
            
            Sprite = CharacterData.CharacterSprite(Name);

            Portrait = CharacterData.CharacterPortrait(Name);

        }

        public override void reloadData()
        {

            CharacterDisposition disposition = CharacterData.CharacterDisposition(name);

            Age = disposition.Age;
            
            Manners = disposition.Manners;
            
            SocialAnxiety = disposition.SocialAnxiety;
            
            Optimism = disposition.Optimism;
            
            Gender = disposition.Gender;
            
            datable.Value = disposition.datable;
            
            Birthday_Season = disposition.Birthday_Season;
            
            Birthday_Day = disposition.Birthday_Day;
            
            id = disposition.id;

        }

        public override void reloadDefaultLocation()
        {

            DefaultMap = Mod.instance.CharacterMap(name);

            if(DefaultMap == null)
            {

                DefaultMap = "Farm";

            }

            DefaultPosition = Mod.instance.CharacterPosition(name);

        }

        protected override string translateName(string name)
        { 
            return name;
        
        }

        public override void tryToReceiveActiveObject(Farmer who)
        {
            return;
        }

        public override bool checkAction(Farmer who, GameLocation l)
        {

            if (timers.ContainsKey("busy"))
            {

                return false;

            }

            Halt();

            faceGeneralDirection(who.Position);

            return true;

        }

        public override void performTenMinuteUpdate(int timeOfDay, GameLocation l)
        {

        }

        public override void behaviorOnFarmerPushing()
        {

            //if (behaviour.ContainsKey("frozen"))
            if(priorities.Contains("frozen"))
            {

                return;

            }

            if (timers.ContainsKey("push"))
            {

                timers["push"] += 2;

                if (timers["push"] > 10)
                {

                    moveVectors.Clear();

                    TargetDirection(findPlayer().facingDirection, 2);

                    timers.Remove("Push");

                }

            }
            else
            {

                timers["push"] = 2;

            }

        }

        public override void Halt()
        {

            moveVectors.Clear();

            timers.Clear();

            timers["stop"] = 60;

            //Sprite.StopAnimation();

            int frameDiff = Sprite.currentFrame % 4;

            Sprite.currentFrame -= frameDiff;

            Sprite.UpdateSourceRect();

        }

        public override void update(GameTime time, GameLocation location)
        {

            if (!Context.IsMainPlayer)
            {

                if (Sprite.loadedTexture == null || Sprite.loadedTexture.Length == 0)
                {

                    Sprite.spriteTexture = CharacterData.CharacterTexture(name);

                    Sprite.loadedTexture = Sprite.textureName.Value;

                    Portrait = CharacterData.CharacterPortrait("Effigy");

                }

                updateSlaveAnimation(time);

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
                        textAboveHeadAlpha = Math.Max(0f, textAboveHeadAlpha - 0.04f);
                    }
                }
            }

            updateEmote(time);

            foreach(KeyValuePair<string,int> timer in timers)
            {
                timers[timer.Key]--;

                if (timers[timer.Key] <= 0)
                {

                    timers.Remove(timer.Key);

                }

            }

            if (!timers.ContainsKey("stop"))
            {

                if(moveVectors.Count > 0)
                {
                    
                    Vector2 nextPosition = moveVectors.First();

                    float moveDistance = Vector2.Distance(nextPosition, Position);

                    if (moveDistance <= 16f)
                    {

                        moveVectors.RemoveAt(0);

                    }

                }

                if (moveVectors.Count <= 0)
                {

                    UpdateTarget();

                }

                MoveTowardsTarget(time);

            }

            Sprite.animateOnce(time);

        }

        public virtual void WarpToDefault()
        {

            string defaultMap = Mod.instance.CharacterMap(Name);

            Vector2 startPosition = CharacterData.CharacterPosition(defaultMap);

            DefaultMap = defaultMap;

            DefaultPosition = startPosition;

            Position = startPosition;

            currentLocation.characters.Remove(this);

            currentLocation = Game1.getLocationFromName(defaultMap);

            currentLocation.characters.Add(this);

            update(Game1.currentGameTime, currentLocation);

        }

        public virtual void SwitchFrozenMode()
        {

            SwitchDefaultMode();

            priorities = new()
            {

                "frozen",

            };

        }

        public virtual void SwitchFollowMode()
        {

            SwitchDefaultMode();

            Mod.instance.trackRegister.Add(name, new(name));

            priorities = new()
            {

                "attack",
                "track",

            };

        }

        public virtual void SwitchRoamMode()
        {

            SwitchDefaultMode();

            priorities = new()
            {

                "roam",

            };

        }

        public virtual void SwitchDefaultMode()
        {
            
            Halt();

            Mod.instance.trackRegister.Remove(name);

        }

        // =================================================
        // Movement Behaviour
        // =================================================

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

                    case "frozen":

                        timers["stop"] = 9999;

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

        public virtual bool TargetOpponent()
        {
            
            if (timers.ContainsKey("cooldown"))
            {
                
                return false;
            
            }

            for(int i = targetOpponents.Count - 1; i >= 0; i--)
            {

                if (!ModUtility.MonsterVitals(targetOpponents[i], currentLocation))
                {

                    targetOpponents.RemoveAt(i);

                }
                else if (Vector2.Distance(targetOpponents[i].Position,Position) >= opponentThreshold)
                {

                    targetOpponents.RemoveAt(i);

                }

            }

            if(targetOpponents.Count == 0)
            {

                return false;
            
            }

            VectorForTarget(targetOpponents.First().Position);

            timers["busy"] = 180;

            timers["attack"] = 120;

            return true;

        }

        public virtual bool TargetTrack()
        {

            if (Mod.instance.trackRegister[name].trackVectors.Count == 0)
            {

                return false;

            }

            float separation = Vector2.Distance(Position, Game1.player.Position);

            if (separation <= 180f && !timers.ContainsKey("track"))
            {

                return false;

            }

            if (Vector2.Distance(Mod.instance.trackRegister[name].trackVectors.First(),Position) >= 180f)
            {

                WarpToTarget();
            
            }


            if (Mod.instance.trackRegister[name].trackVectors.Count == 0)
            {

                return false;

            }

            Vector2 trackVector = Mod.instance.trackRegister[name].NextVector();

            VectorForTarget(trackVector, -1, false);

            //moveVectors.Add(trackVector);

            timers["track"] = 120;

            if (separation > 480f)
            {

                timers["sprint"] = 180;

            }
            else
            {

                timers["hurry"] = 120;

            }

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

            if (Mod.instance.trackRegister[name].trackVectors.Count > 0)
            {

                Mod.instance.trackRegister[name].TruncateTo(3);

                Position = Mod.instance.trackRegister[name].NextVector();

            }
            else
            {

                Position = Game1.player.Position + new Vector2(0, 64);

            }

            ModUtility.AnimateQuickWarp(currentLocation, position - new Vector2(0, 32), "Solar");

        }

        public virtual bool TargetRoam()
        {

            if (roamVectors.Count == 0)
            {

                roamVectors = RoamAnalysis();

            }

            float roamDistance = Vector2.Distance(roamVectors[roamIndex], Position);

            if (roamDistance <= 120f)
            {

                ReachedRoamPosition();

                UpdateRoam(true);

                return true;

            }
            else if(roamDistance >= 1200f)
            {

                timers["hurry"] = 300;

            }

            UpdateRoam();

            VectorForTarget(roamVectors[roamIndex], 4);

            return true;

        }
        
        public void UpdateRoam(bool reset = false)
        {
            //Mod.instance.Monitor.Log("Roam: " + name + " " + roamLapse + " " + Game1.currentGameTime.TotalGameTime.TotalMinutes, LogLevel.Debug);
            if (roamLapse < Game1.currentGameTime.TotalGameTime.TotalMinutes || reset)
            {

                roamLapse = Game1.currentGameTime.TotalGameTime.TotalMinutes + 1;

                roamIndex++;

                if (roamIndex == roamVectors.Count)
                {

                    roamIndex = 0;

                }

            }
            
        }

        public void VectorForTarget(Vector2 target, float ahead = 1.5f, bool check = true)
        {

            Vector2 targetVector = target;

            float distance = Vector2.Distance(Position, target);

            Vector2 diffPosition = target - Position;

            float absX = Math.Abs(diffPosition.X); // x position

            float absY = Math.Abs(diffPosition.Y); // y position

            int signX = diffPosition.X < 0.01f ? -1 : 1; // x sign

            int signY = diffPosition.Y < 0.01f ? -1 : 1; // y sign

            if (absX > absY)
            {

                moveDirection = 2 - signX;

                altDirection = 1 + signY;

            }
            else
            {

                moveDirection = 1 + signY;

                altDirection = 2 - signX;

            }

            if (distance >= 128f && check)
            {

                Vector2 moveVector;

                float far = Math.Min((64 * ahead), distance);

                if (absX > absY)
                {

                    moveDirection = 2 - signX;

                    altDirection = 1 + signY;

                    moveVector = new Vector2(signX, (int)(absY / absX * signY)) * far;

                }
                else
                {

                    moveDirection = 1 + signY;

                    altDirection = 2 - signX;

                    moveVector = new Vector2((int)(absX / absY * signX), signY) * far;

                }

                targetVector = Position + moveVector;

            }

            moveVectors.Add(targetVector);

            return;

        }

        public virtual void TargetRandom()
        {

            moveVectors.Clear();

            timers.Clear();

            Random random = new();

            if(random.Next(2) == 0)
            {
                
                TargetDirection(random.Next(4), random.Next(1, 4));

            }
            else
            {

                Halt();

            }

        }

        public virtual void TargetDirection(int direction, int distance)
        {
            
            int offset = 64 * distance;

            Vector2 target;

            switch (direction)
            {
                case 0:

                    target = Position - new Vector2(0, offset);

                    break;

                case 1:

                    target = Position + new Vector2(offset, 0);
                    
                    break;

                case 2:

                    target = Position + new Vector2(0, offset);

                    break;

                default:

                    target = Position - new Vector2(offset, 0);

                    break;
            }

            moveDirection = direction;

            altDirection = moveDirection + 1 % 4;

            moveVectors.Add(target);

        }

        public virtual void MoveTowardsTarget(GameTime time)
        {
            if (Game1.IsClient)
            {
                return;
            }

            if(moveVectors.Count == 0)
            {
                Sprite.StopAnimation();

                return;

            }

            //------------- Factors

            Vector2 nextPosition = moveVectors.First();

            Vector2 diffPosition = nextPosition - Position;

            float absX = Math.Abs(diffPosition.X); // x position

            float absY = Math.Abs(diffPosition.Y); // y position

            int signX  = diffPosition.X < 0.001f ? -1 : 1; // x sign

            int signY = diffPosition.Y < 0.001f ? -1 : 1; // y sign

            float moveX = signX;

            float moveY = signY;

            if (absX > absY)
            {

                /*if (!timers.ContainsKey("direction"))
                {

                    moveDirection = 2 - signX;

                    altDirection = 1 + signY;

                    timers.Add("direction", 40);
                }*/

                moveY = (absY < 0.05f) ? 0 : (int)(absY / absX * signY);

            }
            else
            {

                /*if (!timers.ContainsKey("direction"))
                {

                    moveDirection = 1 + signY;

                    altDirection = 2 - signX;

                    timers.Add("direction", 40);

                }*/

                moveX = (absX < 0.05f) ? 0 : (int)(absX / absY * signX);

            }

            Vector2 factorVector = new(moveX, moveY);

            float moveSpeed = gait;

            if (timers.ContainsKey("sprint") || timers.ContainsKey("attack"))
            {
                moveSpeed = gait * 5;

            }
            else if(timers.ContainsKey("hurry"))
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

                        if (boundingBox2.Intersects(boundingBox))
                        {

                            damageMonsters.Add(monsterCharacter);

                    
                        }

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

                    continue;

                }

                if(nonPlayableCharacter != this)
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

            if(thisTile != nextTile)
            {

                if(!ModUtility.GroundCheck(currentLocation, nextTile, true))
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

            switch(moveDirection)
            {
                case 0:
                    Sprite.AnimateUp(time);
                    break;

                case 1:
                    Sprite.AnimateRight(time);
                    break;

                case 2:
                    Sprite.AnimateDown(time);
                    break;

                default:
                    Sprite.AnimateLeft(time);
                    break;

            }

        }

        // =================================================
        // Priority Presets
        // =================================================

        public virtual List<Vector2> RoamAnalysis()
        {

            int mapWidth = currentLocation.map.Layers[0].LayerWidth;

            int mapHeight = currentLocation.map.Layers[0].LayerHeight;

            int mapDivision = mapWidth / 8;

            List<Vector2> roamIndexes = new()
            {

                new Vector2(mapWidth / 2, mapHeight / 2) * 64,

                new Vector2(mapWidth - mapDivision, mapDivision) * 64,

                new Vector2(mapWidth / 2, mapHeight / 2) * 64,

                new Vector2(mapWidth - mapDivision, mapHeight - mapDivision) * 64,

                new Vector2(mapWidth / 2, mapHeight / 2) * 64,

                new Vector2(mapDivision, mapHeight - mapDivision) * 64,

                new Vector2(mapWidth / 2, mapHeight / 2) * 64,

                new Vector2(mapDivision, mapDivision) * 64,

            };

            return roamIndexes;

        }


        // =================================================
        // Destination Behaviour
        // =================================================

        public virtual void DealDamageToMonster(StardewValley.Monsters.Monster monsterCharacter)
        {

            int damage = Math.Min(Mod.instance.DamageLevel() / 2, (monsterCharacter.Health - 1));

            int diffX = 0;

            int diffY = 0;

            if (!monsterCharacter.isGlider.Value)
            {

                float differentialX = monsterCharacter.Position.X - Position.X;

                float differentialY = monsterCharacter.Position.Y - Position.Y;

                int signY = 1;

                int signX = 1;

                float differentialFactor;

                if (differentialY < 0)
                {

                    signY = -1;

                }

                if (differentialX < 0)
                {

                    signX = -1;

                }

                if (Math.Abs(differentialX) < Math.Abs(differentialY))
                {
                    differentialFactor = Math.Abs(differentialX) / Math.Abs(differentialY);

                    diffX = (int)(128 * signX * differentialFactor);

                    diffY = 128 * signY;

                }
                else
                {
                    differentialFactor = Math.Abs(differentialY) / Math.Abs(differentialX);

                    diffX = 128 * signX;

                    diffY = (int)(128 * signY * differentialFactor);

                }
            }

            monsterCharacter.takeDamage(damage, diffX, diffY, false, 1.00, Game1.player);

            currentLocation.removeDamageDebris(monsterCharacter);

            Microsoft.Xna.Framework.Rectangle boundingBox = monsterCharacter.GetBoundingBox();

            currentLocation.debris.Add(new Debris(damage, new Vector2(boundingBox.Center.X + 16, boundingBox.Center.Y), new Color(255, 130, 0), 1f, monsterCharacter));

        }

        public virtual void ReachedRoamPosition()
        {

        }


    }

}
