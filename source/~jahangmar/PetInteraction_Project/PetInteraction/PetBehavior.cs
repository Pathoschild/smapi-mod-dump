/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jahangmar/StardewValleyMods
**
*************************************************/

// Copyright (c) 2019 Jahangmar
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Characters;
using StardewValley.Objects;
using StardewValley.Projectiles;

namespace PetInteraction
{
    public class PetBehavior
    {
        public static Pet pet;

        public enum PetState
        {
            Vanilla,
            CatchingUp,
            Waiting,
            Chasing,
            Fetching,
            Retrieve,
            WaitingToFetch
        }

        //game constants
        private const int pet_max_friendship = 1000;
        private const int driftwood_index = 169;

        //1 out of x
        private const int complain_not_reaching_critter = 20;

        //1 out of x
        private const int complain_not_reaching_player = 10;

        private const int throw_speed = 10;

        private const int timeout_after = 500;



        public static bool throwing = false;

        public static bool hasFetchedToday;

        public static PetState petState = PetState.Vanilla;

        //public static Stack<Vector2> FetchPath = new Stack<Vector2>();

        public static Queue<Vector2> CurrentPath = new Queue<Vector2>();

        private static Item Stick;

        private static int timer;

        public static void SetState(PetState state)
        {
            if (petState == state)
                return;

            petState = state;
            //ModEntry.Log("Set state to " + petState);

            switch (state)
            {
                case PetState.Vanilla:
                    throwing = false;
                    break;
                case PetState.CatchingUp:
                    break;
                case PetState.Waiting:
                    Sit();
                    CurrentPath.Clear();
                    break;
                case PetState.Chasing:
                    SetTimer();
                    break;
                case PetState.WaitingToFetch:
                    SetTimer();
                    break;
                case PetState.Fetching:
                    SetTimer();
                    break;
                case PetState.Retrieve:
                    throwing = false;
                    break;
            }

            if (state != PetState.Retrieve && state != PetState.Waiting && Stick != null)
            {
                Game1.createItemDebris(Stick, GetPet().Position, 5, Game1.currentLocation);
                Stick = null;
            }
        }

        public static void SetTimer()
        {
            timer = Game1.ticks;
        }

        public static bool TimeOut(int after = timeout_after)
        {
            return Game1.ticks > timer + after;
        }

        private static Pet FindPet(string name = null)
        {
            bool check(Character c) => c is Pet p && !ModEntry.IsTempPet(p) && (name == null || p.displayName == name);

            foreach (Character c in Game1.getFarm().characters)
            {
                if (check(c))
                    return c as Pet;
            }

            foreach (Character c in Utility.getHomeOfFarmer(Game1.player).characters)
            {
                if (check(c))
                    return c as Pet;
            }

            foreach (GameLocation location in Game1.locations)
                foreach (Character c in location.characters)
                {
                    if (check(c))
                        return c as Pet;
                }

            if (name == null)
                return null;
            else
                return FindPet(null);
        }

        public static Pet GetPet()
        {
            if (pet == null)
                pet = FindPet();

            if (pet != null)
                ModEntry.TempPet.displayName = pet.displayName;

            return pet;
        }

        public static void SetPet(Pet pet)
        {
            PetBehavior.pet = pet;
            if (pet != null)
                ModEntry.TempPet.displayName = pet.displayName;
        }

        public static bool WasPetToday(Pet pet) => pet.lastPetDay.ContainsKey(Game1.player.UniqueMultiplayerID) && pet.lastPetDay[Game1.player.UniqueMultiplayerID] == Game1.Date.TotalDays;
        

        public static void SetWasPetToday(Pet pet, bool value)
        {
            int days = value ? Game1.Date.TotalDays : Game1.Date.TotalDays - 1;

            if (!pet.lastPetDay.ContainsKey(Game1.player.UniqueMultiplayerID))
                pet.lastPetDay.Add(Game1.player.UniqueMultiplayerID, days);
            else
                pet.lastPetDay[Game1.player.UniqueMultiplayerID] = days;
        }

        public static bool Compare(double a, double b, double epsilon = 0.01) => System.Math.Abs(a - b) < epsilon;
        public static bool Compare(Vector2 v1, Vector2 v2, double epsilon = 0.01) => Compare(v1.X, v2.X, epsilon) && Compare(v1.Y, v2.Y, epsilon);

        /// <summary>
        /// Returns distance (in pixels) from pet position to current goal of the path. 
        /// </summary>
        public static int PetCurrentCatchUpGoalDistance() => CurrentPath.Count == 0 ? 0 : (int)Utility.distance(GetPet().Position.X, CurrentPath.Peek().X * Game1.tileSize, GetPet().Position.Y, CurrentPath.Peek().Y * Game1.tileSize);

        /// <summary>
        /// Returns distance between pet and player in tiles.
        /// </summary>
        public static int PlayerPetDistance() => (int)Utility.distance(GetPet().getTileX(), Game1.player.getTileX(), GetPet().getTileY(), Game1.player.getTileY());

        public static int PetDistance(Vector2 tile) => (int)Utility.distance(GetPet().getTileX(), tile.X, GetPet().getTileY(), tile.Y);

        private static int Distance(Vector2 vec1, Vector2 vec2) => (int)Utility.distance(vec1.X, vec2.X, vec1.Y, vec2.Y);

        public static void SetPetPositionFromTile(Vector2 tile)
        {
            GetPet().Position = new Vector2(tile.X * Game1.tileSize, tile.Y * Game1.tileSize);
        }

        public static bool CanThrow(Item item)
        {
            return !throwing && petState != PetState.Vanilla && petState != PetState.Retrieve && item != null
                    && (item.ParentSheetIndex == Object.wood || item.ParentSheetIndex == driftwood_index);
                    
        }

        public static void Throw(Item item, Vector2 cursorTile)
        {
            if (throwing)
                return;

            Vector2 velocity = Utility.getVelocityTowardPoint(Game1.player.getTileLocation(), cursorTile, throw_speed);

            StickProjectile proj = new StickProjectile(item.getOne(), velocity);
            Game1.currentLocation.projectiles.Add(proj);

            Game1.player.reduceActiveItemByOne();
            throwing = true;

            SetState(PetState.WaitingToFetch);
        }

        class StickProjectile : BasicProjectile
        {
            public static Item item;
            public static Vector2 destination; //pixels

            public StickProjectile(Item item, Vector2 velocity) : base(0, item.ParentSheetIndex, 0, 0, 1, velocity.X, velocity.Y, (Game1.player.getTileLocation() - new Vector2(0, 0)) * Game1.tileSize, "shwip", "throw", false, false, Game1.currentLocation, Game1.player, true, HandleonCollisionBehavior)
            {
                StickProjectile.item = item;
            }
            public override bool isColliding(GameLocation location)
            {
                Rectangle rect = new Rectangle((int)position.Value.X + 1, (int)position.Value.Y + 1, Game1.tileSize - 2, Game1.tileSize - 2);
                return travelDistance > ModEntry.config.stick_range * Game1.tileSize || !Game1.player.GetBoundingBox().Intersects(rect) && location.isCollidingPosition(rect, Game1.viewport, false, 0, false, null, false, false, true);
            }

            static void HandleonCollisionBehavior(GameLocation location, int xPosition, int yPosition, Character who)
            {
                destination = new Vector2(xPosition, yPosition);
                Game1.createItemDebris(item, destination, 4, Game1.currentLocation);


                Vector2 pathdest = new Vector2((int)(destination.X / Game1.tileSize), (int)(destination.Y / Game1.tileSize));
                RemovePlayerFromPassableCache();
                CurrentPath = PathFinder.CalculatePath(GetPet(), pathdest);
                if (CurrentPath.Count > 0)
                {
                    SetPetPositionFromTile(CurrentPath.Peek());
                    SetState(PetState.Fetching);
                }
                else
                {
                    CannotFetch(pathdest);
                    SetState(PetState.Waiting);
                }

                throwing = false;
            }
        }

        public static void PickUpItem()
        {
            Debris d = null;
            foreach (Debris debris in Game1.currentLocation.debris)
            {
                if (debris.item?.ParentSheetIndex == StickProjectile.item.ParentSheetIndex)
                {
                    foreach (Chunk chunk in debris.Chunks)
                    {
                        if (Distance(chunk.position.Value, StickProjectile.destination) < Game1.tileSize*3)
                        {
                            d = debris;
                            break;
                        }
                    }
                }
                if (d != null)
                    break;
            }

            if (d != null)
            {
                Stick = d.item;
                Game1.currentLocation.debris.Remove(d);

                CurrentPath = PathFinder.CalculatePath(pet, new Vector2(Game1.player.getTileX(), Game1.player.getTileY()));

                if (CurrentPath.Count > 0)
                {
                    SetPetPositionFromTile(CurrentPath.Peek());
                    SetState(PetState.Retrieve);
                }
                else
                {
                    CannotReachPlayer();
                    SetState(PetState.Waiting);
                }

            }
            else
            {
                CannotPickUp();
                SetState(PetState.Waiting);
            }

        }

        public static void DropItem()
        {
            Game1.createItemDebris(Stick, Game1.player.Position, 5, Game1.currentLocation);

            if (!hasFetchedToday && new System.Random().Next(100)+1 <= ModEntry.config.pet_fetch_friendship_chance)
            {
                GetPet().doEmote(Character.heartEmote);
                pet.playContentSound();
                pet.friendshipTowardFarmer.Value = System.Math.Min(pet_max_friendship, pet.friendshipTowardFarmer.Value + ModEntry.config.pet_fetch_friendship_increase);
                hasFetchedToday = true;
            }

            if (GetPet() is Dog dog)
                dog.pantSound(null);

            Stick = null;

            SetState(PetState.Waiting);
        }

        /// <summary>
        /// Returns velocity of pet towards current goal of the path.
        /// </summary>
        public static Vector2 GetVelocity()
        {
            if (CurrentPath.Count == 0)
                return new Vector2(0, 0);

            Vector2 pathPosition = CurrentPath.Peek() * Game1.tileSize;
            if ((int)pathPosition.X == (int)pet.Position.X && (int)pathPosition.Y == (int)pet.Position.Y)
            {
                return new Vector2(0, 0);
            }
            else
            {
                int speed = ModEntry.config.pet_speed;
                switch (petState)
                {
                    case PetState.CatchingUp:
                        speed = ModEntry.config.pet_speed;
                        break;
                    case PetState.Chasing:
                    case PetState.Fetching:
                        speed = ModEntry.config.pet_fast_speed;
                        break;                    
                }

                Vector2 vel = Utility.getVelocityTowardPoint(pet.Position, pathPosition, speed);
                          
                return vel; 
            }
        }

        //static int last_dir = 0;
        //public static int PetBehaviorFacingDir = 0;
        public static int PetBeahviorCurrentBehavior = 0;
        public static void CatchUp(int oldFacingDir)
        {
            GetPet();
            if (pet == null)
                return;
            if (pet is Dog dog && (petState == PetState.Chasing || petState == PetState.Fetching))
                SetPetBehavior(Dog.behavior_Sprint);
            else
                SetPetBehavior(Pet.behavior_Walk);

            pet.FacingDirection = oldFacingDir;

            Vector2 velocity = GetVelocity();

            velocity = new Vector2((int)System.Math.Round(velocity.X), (int)System.Math.Round(velocity.Y));

            if (!Compare(velocity, new Vector2(0, 0)))
            {
                if (System.Math.Abs(velocity.X) > System.Math.Abs(velocity.Y))
                    pet.FacingDirection = velocity.X >= 0 ? 1 : 3;
                else
                    pet.FacingDirection = velocity.Y >= 0 ? 2 : 0;

                //ModEntry.Log("Dir changed by mod to "+pet.FacingDirection);
                //PetBehaviorFacingDir = pet.FacingDirection;
            }
           // else
           //     pet.FacingDirection = PetBehaviorFacingDir;
            /*
            if (pet.FacingDirection != last_dir)
            {
                ModEntry.Log("Dir changed: " + pet.FacingDirection);
                ModEntry.Log("velocity: " + velocity);
                ModEntry.Log("pet.Pos: " + pet.Position);
                ModEntry.Log("next pos: " + CurrentPath.Peek() * Game1.tileSize);
                last_dir = pet.FacingDirection;
            }
            */

            pet.xVelocity = velocity.X;
            pet.yVelocity = -velocity.Y;


            ModEntry.GetHelper().Reflection.GetField<bool>(pet, "moveUp").SetValue(false);
            ModEntry.GetHelper().Reflection.GetField<bool>(pet, "moveRight").SetValue(false);
            ModEntry.GetHelper().Reflection.GetField<bool>(pet, "moveDown").SetValue(false);
            ModEntry.GetHelper().Reflection.GetField<bool>(pet, "moveLeft").SetValue(false);
            pet.animateInFacingDirection(Game1.currentGameTime);
        }

        public static void Sit()
        {
            GetPet();
            pet.SetMovingUp(false);
            pet.SetMovingDown(false);
            pet.SetMovingLeft(false);
            pet.SetMovingRight(false);
            SetPetBehavior(Pet.behavior_SitDown);
        }

        private static void SetPetBehavior(int behavior)
        {
            ModEntry.PetBehaviour = behavior;
        }

        public static void Jump()
        {
            GetPet().jump();
        }

        public static void GetHitByTool()
        {
            if (Game1.currentLocation is Farm || Game1.currentLocation is StardewValley.Locations.FarmHouse)
                SetState(PetState.Vanilla);
            GetPet().doEmote(Character.angryEmote);
            Jump();
            pet.friendshipTowardFarmer.Value = System.Math.Max(0, pet.friendshipTowardFarmer.Value - ModEntry.config.pet_friendship_decrease_onhit ); // Values from StardewValley.Characters.Pet
            SetWasPetToday(pet, false);
        }

        public static void Confused()
        {
            GetPet().doEmote(Character.questionMarkEmote);
            GetPet().playContentSound();
        }

        public static void TryChaseCritterInRange()
        {
            var critters = ModEntry.GetHelper().Reflection.GetField<List<Critter>>(Game1.currentLocation, "critters").GetValue();
            if (critters == null)
                return;

            bool chasable(Critter critter)
            {
                if (critter is Birdie birdie && ModEntry.GetHelper().Reflection.GetField<int>(birdie, "state").GetValue() != Birdie.flyingAway)
                {
                    return true;
                }
                else if (critter is Seagull sg && ModEntry.GetHelper().Reflection.GetField<int>(sg, "state").GetValue() != Seagull.flyingAway)
                {
                    return true;
                }
                else if (critter is Rabbit)
                {
                    return true;
                }
                else if (critter is Squirrel)
                {
                    return true;
                }
                return false;
            }

            foreach (Critter critter in critters)
            {
                Vector2 position = new Vector2((int)critter.position.X, (int)critter.position.Y);

                if (chasable(critter) && PetDistance(position / Game1.tileSize) < 20)
                {
                    RemovePlayerFromPassableCache();
                    Queue<Vector2> path = PathFinder.CalculatePath(GetPet(), position / Game1.tileSize);
                    if (path.Count > 0)
                    {
                        CurrentPath = path;
                        SetState(PetState.Chasing);
                        if (pet is Dog)
                            Game1.playSound("dog_bark");
                    }
                    else
                    {
                        var random = new System.Random();
                        if (random.Next(complain_not_reaching_critter) == 0)
                        {
                            Confused();
                        }
                        ModEntry.Log("Cannot reach critter at "+position+" ("+ position / Game1.tileSize+")");
                    }

                }
            }
        }

        public static void CannotReachPlayer()
        {
            var random = new System.Random();
            if (random.Next(complain_not_reaching_player) == 0)
            {
                Confused();
            }
            ModEntry.Log("Cannot reach player at "+ new Vector2(Game1.player.getTileX(), Game1.player.getTileY()));
        }

        public static void CannotFetch(Vector2 stickPos)
        {
            Confused();
            ModEntry.Log("Cannot reach stick at "+stickPos);
        }

        public static void CannotPickUp()
        {
            Confused();
            ModEntry.Log("Cannot find stick");
        }

        public static void Petting(Pet pet)
        {
            //ModEntry.GetHelper().Reflection.GetField<bool>(pet, "wasPetToday").SetValue(true);
            SetWasPetToday(GetPet(), true);
            pet.friendshipTowardFarmer.Value = System.Math.Min(1000, pet.friendshipTowardFarmer.Value + ModEntry.config.pet_petting_friendship_increase);
            if (ModEntry.config.unconditional_love || ModEntry.config.love_everytime_at_max_friendship && pet.friendshipTowardFarmer >= 1000 && Game1.player != null)
            {
                Game1.showGlobalMessage(Game1.content.LoadString("Strings\\Characters:PetLovesYou", pet.displayName));
                if (!Game1.player.mailReceived.Contains("petLoveMessage"))
                    Game1.player.mailReceived.Add("petLoveMessage");
            }
            pet.doEmote(20, true);
            pet.playContentSound();
        }

        public static void RemovePlayerFromPassableCache()
        {
            Vector2 playerTile = new Vector2((int) (Game1.player.GetBoundingBox().X / Game1.tileSize), (int) (Game1.player.GetBoundingBox().Y / Game1.tileSize));
            List<Vector2> tiles = Utility.getAdjacentTileLocations(playerTile);
            tiles.Add(playerTile);
            foreach (Vector2 tile in tiles)
                PathFinder.RemoveCachedPassableTile(tile);
        }
    }
}
