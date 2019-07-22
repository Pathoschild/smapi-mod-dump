//Copyright (c) 2019 Jahangmar

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU Lesser General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//GNU Lesser General Public License for more details.

//You should have received a copy of the GNU Lesser General Public License
//along with this program. If not, see <https://www.gnu.org/licenses/>.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Objects;
using StardewValley.Projectiles;
using StardewValley.Locations;

using static PetInteraction.PetBehavior;
using StardewValley.Tools;

namespace PetInteraction
{
    public class ModEntry : Mod
    {
        private const bool DEBUG_MODE = false;

        private int catch_up_distance = 2;

        public static int PetBehaviour = -1;

        public static readonly Pet TempPet = new Cat()
        {
            Name ="PetInteractionTempCat",
            displayName = "TempCatDisplay",
        };

        public static bool IsTempPet(Pet pet)
        {
            return pet == TempPet || pet.Name == TempPet.Name;
        }

        public static Config config;

        public static bool debug()
        {
            return DEBUG_MODE;
        }

        public override void Entry(IModHelper helper)
        {
            _Monitor = Monitor;
            _Helper = helper;

            config = Helper.ReadConfig<Config>();

            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            helper.Events.GameLoop.OneSecondUpdateTicked += GameLoop_OneSecondUpdateTicked;
            helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
            if (debug())
                helper.Events.Display.RenderedWorld += Display_RenderedWorld;
            helper.Events.Player.Warped += Player_Warped;
            helper.Events.GameLoop.UpdateTicking += GameLoop_UpdateTicking;
            helper.Events.GameLoop.Saving += GameLoop_Saving;

            //helper.ConsoleCommands.Add("fix_cat", "")
            //helper.ConsoleCommands.Add("temppet","",HandleActionTempPet);
            helper.ConsoleCommands.Add("check_pet", "", Test);
        }

        void Test(string name, string[] args)
        {
            /*
            foreach (Character c in Game1.getFarm().characters)
            {
                if (c is Pet p)
                    Log("Found pet ("+p.Name+") in Farm");
            }

            foreach (Character c in Utility.getHomeOfFarmer(Game1.player).characters)
            {
                if (c is Pet p)
                    Log("Found pet ("+p.Name+") in FarmHouse");
            }
            */
            foreach (GameLocation location in Game1.locations)
                foreach (Character c in location.characters)
                {
                    if (c is Pet p)
                        Log("Found pet ("+p.Name+") in location " + location.Name);
                }
            /*
            Log("pet.IsWalkingInSquare" + pet.IsWalkingInSquare);
            Log("pet.ignoreScheduleToday" + pet.ignoreScheduleToday);
            Log("pet.IsWalkingTowardPlayer" + pet.IsWalkingTowardPlayer);
            Log("pet.followSchedule" + pet.followSchedule);
            Log("pet.DirectionsToNewLocation is null" + (pet.DirectionsToNewLocation == null));
            */
        }

        private void AddTempPetToFarm()
        {
            TempPet.currentLocation = Game1.getFarm();
            if (!Game1.getFarm().characters.Contains(TempPet))
                Game1.warpCharacter(TempPet, Game1.getFarm(), new Vector2(0, 0));
            Log("Adding TempPet");
        }

        private void RemoveTempPetFromFarm()
        {
            if (Game1.getFarm().characters.Contains(TempPet))
                Game1.getFarm().characters.Remove(TempPet);
            Log("Removing TempPet");

            foreach (GameLocation location in Game1.locations)
            {
                for (int i=location.characters.Count-1; i>=0; i--)
                {
                    if (location.characters[i] is Pet p && IsTempPet(p))
                    {
                        Monitor.Log("Found temporary pet that should not be there. Fixed it.", LogLevel.Error);
                        location.characters.RemoveAt(i);
                    }
                }
            }
        }

        private class Comparer : IComparer<Vector2>
        {
            public int Compare(Vector2 n1, Vector2 n2)
            {
                return (int)((n1.X - n2.X) + (n1.Y - n2.Y));
            }
        }

        public static List<Vector2> NonPassables = new List<Vector2>();
        public static List<Vector2> Passables = new List<Vector2>();

        void Display_RenderedWorld(object sender, StardewModdingAPI.Events.RenderedWorldEventArgs e)
        {
            foreach (Vector2 vec in CurrentPath)
                e.SpriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, vec * 64f), new Rectangle(194 + 0 * 16, 388, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.999f);
            foreach (Vector2 vec in NonPassables)
                e.SpriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, vec * 64f), new Rectangle(194 + 1 * 16, 388, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.999f);
            foreach (Vector2 vec in Passables)
                e.SpriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, vec * 64f), new Rectangle(194 + 0 * 16, 388, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.999f);
        }

        private void SafeState()
        {
            //make sure the TestPet was removed
            RemoveTempPetFromFarm();
            //make sure your pet is at the farmhouse
            if (GetPet() != null && !(Game1.getFarm().characters.Contains(pet) && !(Game1.getLocationFromName(Game1.player.homeLocation).characters.Contains(pet))))
            {
                pet.warpToFarmHouse(Game1.player);
            }


        }

        void GameLoop_Saving(object sender, StardewModdingAPI.Events.SavingEventArgs e)
        {
            SafeState();
        }


        void GameLoop_DayStarted(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            pet = null;
            SetState(PetState.Vanilla);
            hasFetchedToday = false;
            if (GetPet() != null && !(Game1.getFarm().characters.Contains(pet) && !(Game1.getLocationFromName(Game1.player.homeLocation).characters.Contains(pet))))
            {
                pet.warpToFarmHouse(Game1.player);
            }
        }


        void Input_ButtonPressed(object sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            if (Game1.currentLocation == null || !Game1.player.hasPet() || GetPet() == null || Game1.activeClickableMenu != null || Game1.eventUp)
                return;

            bool PetClicked()
            {
                Vector2 grabTile = e.Cursor.GrabTile;
                return pet.yJumpOffset == 0 && pet.GetBoundingBox().Intersects(new Rectangle((int)grabTile.X * Game1.tileSize, (int)grabTile.Y * Game1.tileSize, Game1.tileSize, Game1.tileSize));
            }

            bool NotGiftingTreat()
            {
                return !Helper.ModRegistry.IsLoaded("Paritee.TreatYourAnimals") || Game1.player.ActiveObject == null || Game1.player.ActiveObject.Edibility == -300;
            }

            GameLocation loc = Game1.currentLocation;
            if (e.Button.IsActionButton())
            {
                switch (petState)
                {
                    case PetState.Vanilla:
                        if (PetClicked() && Helper.Reflection.GetField<bool>(GetPet(), "wasPetToday").GetValue() && NotGiftingTreat())
                        {
                            Helper.Input.Suppress(e.Button);
                            SetState(PetState.Waiting);
                            Jump();
                        }
                        break;
                    case PetState.CatchingUp:
                    case PetState.Waiting:
                    case PetState.Chasing:
                    case PetState.Fetching:
                    case PetState.Retrieve:
                        if ((loc is Farm || loc is FarmHouse) && PetClicked() && NotGiftingTreat())
                        {
                            Helper.Input.Suppress(e.Button);
                            SetState(PetState.Vanilla);
                            Jump();
                        }
                        break;
                }
            }
            else if (e.Button.IsUseToolButton())
            {
                if (debug())
                {
                    if (NonPassables.Contains(e.Cursor.Tile))
                    {
                        Monitor.Log("NONPASSABLE: " + e.Cursor.Tile);
                    }

                    Passables.AddRange(PathFinder.GetPassableNeighbors(e.Cursor.Tile));
                    if (PathFinder.IsPassable(e.Cursor.Tile) && !Passables.Contains(e.Cursor.Tile))
                        Passables.Add(e.Cursor.Tile);
                    else if (!NonPassables.Contains(e.Cursor.Tile))
                        NonPassables.Add(e.Cursor.Tile);
                }


                if (CanThrow(Game1.player.ActiveObject))
                {
                    Throw(Game1.player.ActiveObject, e.Cursor.Tile);
                }
                else if (Game1.player.CurrentTool is Tool tool && tool != null && PetClicked() && !Game1.player.usingTool)
                {
                    if (tool is Hoe || tool is Axe || tool is Pickaxe || tool is WateringCan)
                    {
                            GetHitByTool();
                    }
                }
                               
            }
        }

        void GameLoop_OneSecondUpdateTicked(object sender, StardewModdingAPI.Events.OneSecondUpdateTickedEventArgs e)
        {
            if (Game1.currentLocation == null || !Game1.player.hasPet() || GetPet() == null)
                return;

            switch (petState)
            {
                case PetState.Vanilla:
                    break;
                case PetState.CatchingUp:
                case PetState.Waiting:
                case PetState.Retrieve:
                    if (PlayerPetDistance() > catch_up_distance && Game1.currentLocation == pet.currentLocation)
                    {
                        CurrentPath = PathFinder.CalculatePath(pet, new Vector2(Game1.player.getTileX(), Game1.player.getTileY()));

                        if (CurrentPath.Count > 0)
                        {
                            SetPetPositionFromTile(CurrentPath.Peek());
                            if (petState == PetState.Waiting)
                                SetState(PetState.CatchingUp);
                        }
                        else
                        {
                            CannotReachPlayer();
                        }
                    }

                    TryChaseCritterInRange();

                    break;
                case PetState.Chasing:
                case PetState.Fetching:
                case PetState.WaitingToFetch:
                    if (TimeOut())
                    {
                        Log("timeout during " + petState);
                        Confused();
                        SetState(PetState.Waiting);
                    }
                    break;
            }
            if (petState != PetState.Vanilla)
                GetPet().CurrentBehavior = PetBehaviour;
        }

        void GameLoop_UpdateTicking(object sender, StardewModdingAPI.Events.UpdateTickingEventArgs e)
        {
            if (Game1.currentLocation == null || !Game1.player.hasPet() || GetPet() == null)
                return;

            if (petState != PetState.Vanilla)    
                GetPet().CurrentBehavior = -1;
        }


        void GameLoop_UpdateTicked(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
        {
            if (Game1.currentLocation == null || !Game1.player.hasPet() || GetPet() == null)
                return;

            switch (petState)
            {
                case PetState.Vanilla:
                    break;
                case PetState.CatchingUp:
                case PetState.Chasing:
                case PetState.Fetching:
                case PetState.Retrieve:

                    CatchUp();

                    if (petState == PetState.Fetching && CurrentPath.Count == 4 && GetPet() is Cat cat)
                        cat.leap(null);

                    if (CurrentPath.Count == 0)
                    {
                        if (petState == PetState.Retrieve)
                        {
                            DropItem();
                        }
                        else if (petState == PetState.Fetching)
                        {
                            PickUpItem();
                        }
                        else
                        {
                            if (petState == PetState.Chasing)
                                Jump();
                            SetState(PetState.Waiting);
                        }
                    }

                    else if (PetCurrentCatchUpGoalDistance() <= 4)
                    {
                        Vector2 pos = CurrentPath.Dequeue();
                        SetPetPositionFromTile(pos);
                    }
                    break;
                case PetState.Waiting:
                    break;
            }

            if (petState != PetState.Vanilla)
                GetPet().CurrentBehavior = PetBehaviour;
        }

        void Player_Warped(object sender, StardewModdingAPI.Events.WarpedEventArgs e)
        {
            if (GetPet() == null)
                return;

            if (e.NewLocation is Farm)
            {
                RemoveTempPetFromFarm();
            }
            else
            {
                AddTempPetToFarm();
            }

            if (petState == PetState.Vanilla)
                return;

            if (debug())
            {
                Passables.Clear();
                NonPassables.Clear();
            }

            bool EnteredLeftRight()
            {
                //Log("Layerwidth " + e.NewLocation.map.GetLayer("Back").LayerWidth);
                return Game1.player.getTileX() < 2 || Game1.player.getTileX() > e.NewLocation.map.GetLayer("Back").LayerWidth-2;
            }

            bool EnteredTopBot()
            {
                return Game1.player.getTileY() < 2 || Game1.player.getTileY() > e.NewLocation.map.GetLayer("Back").LayerHeight-2;
            }

            if (e.NewLocation is Town
                || e.NewLocation is Forest
                || e.NewLocation is Desert
                || e.NewLocation is BusStop
                || e.NewLocation is Beach
                || e.NewLocation is BeachNightMarket
                || e.NewLocation is Mountain
                || e.NewLocation is Summit
                //|| e.NewLocation is CommunityCenter
                || e.NewLocation is Railroad
                || e.NewLocation.Name == "Backwoods")
            {
                List<Vector2> tryTiles = new List<Vector2>()
                {
                    Utility.recursiveFindOpenTileForCharacter(pet, e.NewLocation, Game1.player.getTileLocation(), 10)
                };

                if (e.NewLocation is CommunityCenter)
                {
                    tryTiles.Insert(0, Game1.player.getTileLocation() - new Vector2(0, 5));
                }
                else if ((e.NewLocation is Beach || e.NewLocation is BeachNightMarket) && e.OldLocation is Town)
                {
                    tryTiles.Insert(0, Game1.player.getTileLocation() + new Vector2(0, 5));
                }
                else if (e.NewLocation is Town && (e.OldLocation is Beach || e.OldLocation is BeachNightMarket))
                {
                    tryTiles.Insert(0, Game1.player.getTileLocation() - new Vector2(0, 5));
                    tryTiles.Insert(0, Game1.player.getTileLocation() - new Vector2(1, 5));
                    tryTiles.Insert(0, Game1.player.getTileLocation() - new Vector2(-1, 5));
                }

                if (EnteredLeftRight())
                {
                    tryTiles.Insert(0, Game1.player.getTileLocation() + new Vector2(0, 1));
                    tryTiles.Insert(0, Game1.player.getTileLocation() - new Vector2(0, 1));
                }

                if (EnteredTopBot())
                {
                    tryTiles.Insert(0, Game1.player.getTileLocation() + new Vector2(1, 0));
                    tryTiles.Insert(0, Game1.player.getTileLocation() - new Vector2(1, 0));
                }

                Vector2 petTile = tryTiles.Find(PathFinder.IsPassable);
                if (petTile != null)
                {
                    Game1.warpCharacter(GetPet(), e.NewLocation, petTile);
                    Log("Warped pet to " + petTile);
                }

                else
                    Log("Could not find position for pet", LogLevel.Error);
            }
            else if (e.NewLocation is Farm)
            {
                Game1.warpCharacter(GetPet(), "Farm", new Vector2(54f, 8f));
                pet.position.X -= 64f;
            }
            
            else if (e.NewLocation is FarmHouse farmHouse)
            {
                GetPet().warpToFarmHouse(farmHouse.owner);
            }
            else if (e.NewLocation is MineShaft && !(e.OldLocation is MineShaft) || e.NewLocation is Woods /*|| e.NewLocation is Sewer*/)
            {
                if (config.show_message_on_warp)
                    Game1.showGlobalMessage(Helper.Translation.Get("warp.todangerous", new { petname = GetPet().displayName }));
            }
            else if (!e.NewLocation.isOutdoors && e.OldLocation.isOutdoors)
            {
                if (config.show_message_on_warp)
                    Game1.showGlobalMessage(Helper.Translation.Get("warp.waitingoutside", new { petname = GetPet().displayName }));
            }
            else
            {
                Monitor.Log("warped to unknown location: "+Game1.currentLocation.Name, LogLevel.Trace);
            }
        }

        private static IMonitor _Monitor;
        public static void Log(string msg, LogLevel level = LogLevel.Trace) => _Monitor.Log(msg, level);

        private static IModHelper _Helper;
        public static IModHelper GetHelper() => _Helper;

    }
}