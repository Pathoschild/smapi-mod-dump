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

namespace StardewDruid.Character
{
    public class Effigy : StardewValley.NPC
    {

        private Mod mod;

        public int attendPlayer;

        public int wanderAway;

        public int movementChange;

        public bool moveToPlayer;

        public Effigy(Mod Mod, AnimatedSprite sprite, Vector2 position, string defaultMap, int facingDir, string name, Dictionary<int, int[]> schedule, Texture2D portrait) 
           : base(sprite, position, defaultMap, facingDir, name, schedule, portrait, false)
        {
            mod = Mod;

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

            speed = disposition.speed;

        }

        public override void reloadDefaultLocation()
        {
        
            DefaultMap = CharacterData.CharacterDefaultMap(name);

            DefaultPosition = CharacterData.CharacterPosition(name);
        
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
           
            Halt();

            attendPlayer = 120;

            faceGeneralDirection(who.Position);

            mod.druidEffigy.DialogueApproach();

            return true;

        }

        public override void performTenMinuteUpdate(int timeOfDay, GameLocation l)
        {
            if(wanderAway > 0)
            {

                wanderAway--;

            }

        }

        public override void Halt()
        {
            
            moveUp = false;

            moveDown = false;

            moveRight = false;

            moveLeft = false;

            Sprite.StopAnimation();


        }

        public Dictionary<int,float> PlayerProximity()
        {

            Dictionary<int, float> proximity = new() { [0] = -1f };

            if (Game1.player.currentLocation != currentLocation)
            {
                return proximity;
            }

            if (currentLocation.map.GetLayer("Back").Tiles[(int)Game1.player.getTileLocation().X, (int)Game1.player.getTileLocation().Y] == null)
            {
                return proximity;
            }

            if (currentLocation.map.GetLayer("Back").Tiles[(int)Game1.player.getTileLocation().X, (int)Game1.player.getTileLocation().Y].Properties.ContainsKey("NPCBarrier"))
            {
                return proximity;
            }

            Vector2 targetPosition = Game1.player.GetBoundingBox().Center.ToVector2();

            Vector2 currentPosition = GetBoundingBox().Center.ToVector2();

            float targetDistance = Vector2.Distance(targetPosition, currentPosition);

            if (targetDistance > 480)
            {

                return proximity;

            }

            float differentialX = targetPosition.X - currentPosition.X;

            float differentialY = targetPosition.Y - currentPosition.Y;

            if (Math.Abs(differentialX) < Math.Abs(differentialY))
            {

                if (differentialY < 0)
                {

                    proximity[0] = 0f;


                }
                else
                {

                    proximity[0] = 2f;

                }

            }
            else
            {

                if (differentialX < 0)
                {

                    proximity[0] = 3f;

                }
                else
                {

                    proximity[0] = 1f;

                }

            }

            proximity[1] = targetDistance;

            proximity[2] = differentialX;

            proximity[3] = differentialY;

            return proximity;

        }

        public override void update(GameTime time, GameLocation location)
        {

            if (attendPlayer > 0)
            {

                attendPlayer--;

                if(attendPlayer == 0)
                {

                    wanderAway = 3;

                }

                return;

            }

            updateMovement(location, time);

            MovePosition(time, Game1.viewport, location);

        }

        public override void updateMovement(GameLocation location, GameTime time)
        {

            if (Game1.IsClient)
            {
                return;
            }

            if (movementChange > 0)
            {

                movementChange--;

                return;

            }

            MoveTowardsPlayer(location);

            MoveRandomDirection(location);

            movementChange = 60;

        }

        public void MoveTowardsPlayer(GameLocation location)
        {
            
            if (wanderAway > 0)
            {
                return;
            }

            Dictionary<int, float> proximity = PlayerProximity();

            if (proximity[0] < 0f)
            {

                return;

            }

            if (proximity[1] <= 80f)
            {

                Halt();

                attendPlayer = 120;

                faceDirection((int)proximity[0]);

                return;

            }

            switch ((int)proximity[0])
            {
                case 0:

                    SetMovingOnlyUp();

                    break;

                case 1: 
                    
                    SetMovingOnlyRight();
                    
                    break;
                case 2:
                    
                    SetMovingOnlyDown();
                    
                    break;
                
                default:
                    
                    SetMovingOnlyLeft();
                    
                    break;

            }

            return;

        }

        public void MoveRandomDirection(GameLocation location)
        {

            if (isMoving())
            {
                return;

            }
            int num = Game1.random.Next(5);

            if (num != (FacingDirection + 2) % 4)
            {
                if (num < 4)
                {
                        
                    int direction = FacingDirection;
                        
                    faceDirection(num);
                        
                    if (currentLocation.isCollidingPosition(nextPosition(num), Game1.viewport, this))
                    {
                            
                        faceDirection(direction);
                            
                        return;
                        
                    }
                    
                }

                switch (num)
                {
                    case 0:
                        SetMovingUp(b: true);
                        break;
                    case 1:
                        SetMovingRight(b: true);
                        break;
                    case 2:
                        SetMovingDown(b: true);
                        break;
                    case 3:
                        SetMovingLeft(b: true);
                        break;
                    default:
                        Halt();
                        Sprite.StopAnimation();
                        break;
                }
            }

            return;

        }


        public override void MovePosition(GameTime time, xTile.Dimensions.Rectangle viewport, GameLocation currentLocation)
        {
            
            if (Game1.IsClient)
            {
                return;
            }

            Location location = nextPositionTile();

            Microsoft.Xna.Framework.Rectangle next;

            if (moveUp)
            {
                next = nextPosition(0);

                if (!currentLocation.isCollidingPosition(next, Game1.viewport, isFarmer: false, 0, glider: false, this, pathfinding: false))
                {
                    position.Y -= base.speed;
                    Sprite.AnimateUp(time);
                }
                else if (!HandleCollision(next, 0))
                {
                    Halt();
  
                    if (Game1.random.NextDouble() < 0.6)
                    {
                        SetMovingDown(b: true);
                    }
                }

                faceDirection(0);
            }
            else if (moveRight)
            {

                next = nextPosition(1);

                if (!currentLocation.isCollidingPosition(next, Game1.viewport, isFarmer: false, 0, glider: false, this))
                {
                    position.X += base.speed;
                    Sprite.AnimateRight(time);
                }
                else if (!HandleCollision(next, 1))
                {
                    Halt();

                    if (Game1.random.NextDouble() < 0.6)
                    {
                        SetMovingLeft(b: true);
                    }
                }

                faceDirection(1);
            }
            else if (moveDown)
            {

                next = nextPosition(2);

                if (!currentLocation.isCollidingPosition(next, Game1.viewport, isFarmer: false, 0, glider: false, this))
                {
                    position.Y += base.speed;
                    Sprite.AnimateDown(time);
                }
                else if (!HandleCollision(next, 2))
                {
                    Halt();

                    if (Game1.random.NextDouble() < 0.6)
                    {
                        SetMovingUp(b: true);
                    }
                }

                faceDirection(2);
            }
            else
            {
                if (!moveLeft)
                {
                    return;
                }
                next = nextPosition(3);

                if (!currentLocation.isCollidingPosition(next, Game1.viewport, isFarmer: false, 0, glider: false, this))
                {
                    position.X -= base.speed;
                    Sprite.AnimateRight(time);
                }
                else if (!HandleCollision(next, 3))
                {
                    Halt();

                    if (Game1.random.NextDouble() < 0.6)
                    {
                        SetMovingRight(b: true);
                    }
                }

                faceDirection(3);

            }
        }

        public virtual bool HandleCollision(Microsoft.Xna.Framework.Rectangle next, int direction = 0)
        {

            Dictionary<int, float> proximity = PlayerProximity();

            if (proximity[0] < 0f)
            {

                return false;

            }

            if (proximity[1] <= 80f && (int)proximity[0] == direction)
            {

                Halt();

                attendPlayer = 120;

                return true;

            }

            return false;

        }


    }
}
