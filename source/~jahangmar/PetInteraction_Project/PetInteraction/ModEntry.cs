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

        private const int catch_up_distance = 2;

        //private int next_path_pixel_distance = 4; //4

        public static int PetBehaviour = -1;

        public static readonly Pet TempPet = new Cat()
        {
            Name = "PetInteractionTempCat",
            displayName = "TempCatDisplay",
        };

        public static bool IsTempPet(Pet pet)
        {
            return pet == TempPet || pet.Name == TempPet.Name || pet.Name == "TempCat";
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
            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;

            helper.Events.World.BuildingListChanged += World_BuildingListChanged;
            helper.Events.World.LargeTerrainFeatureListChanged += World_LargeTerrainFeatureListChanged;
            helper.Events.World.ObjectListChanged += World_ObjectListChanged;
            helper.Events.World.TerrainFeatureListChanged += World_TerrainFeatureListChanged;

            //helper.ConsoleCommands.Add("add_pet", "", AddPet);
            //helper.ConsoleCommands.Add("setdis", "", (string arg1, string[] arg2) => next_path_pixel_distance = System.Convert.ToInt32(arg2[0]));
            helper.ConsoleCommands.Add("check_pets", "", Test);
        }

        void AddPet(string arg1, string[] arg2)
        {
            Game1.getFarm().characters.Add(new Dog() { Name = "Name", displayName = "displayName" });
        }


        void Test(string name, string[] args)
        {
            foreach (GameLocation location in Game1.locations)
                foreach (Character c in location.characters)
                {
                    if (c is Pet p)
                        Log("Found pet (" + p.Name + ") in location " + location.Name);
                }
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
                for (int i = location.characters.Count - 1; i >= 0; i--)
                {
                    if (location.characters[i] is Pet p && IsTempPet(p))
                    {
                        Monitor.Log("Found temporary pet that should not be there (" + location.Name + "). Fixed it.", LogLevel.Debug);
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
            if (!CanUpdatePet())
                return;

            foreach (Vector2 vec in CurrentPath)
                e.SpriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, vec * 64f), new Rectangle(194 + 0 * 16, 388, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.999f);
            foreach (Vector2 vec in NonPassables)
                e.SpriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, vec * 64f), new Rectangle(194 + 1 * 16, 388, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.999f);
            foreach (Vector2 vec in Passables)
                e.SpriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, vec * 64f), new Rectangle(194 + 0 * 16, 388, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.999f);
        }

        void World_BuildingListChanged(object sender, StardewModdingAPI.Events.BuildingListChangedEventArgs e)
        {
            if (e.Added.Any())
                PathFinder.ResetCachedTiles(e.Location);
        }

        void World_LargeTerrainFeatureListChanged(object sender, StardewModdingAPI.Events.LargeTerrainFeatureListChangedEventArgs e)
        {
            if (e.Added.Any())
                PathFinder.ResetCachedTiles(e.Location);
        }

        void World_ObjectListChanged(object sender, StardewModdingAPI.Events.ObjectListChangedEventArgs e)
        {
            if (e.Added.Any())
                PathFinder.ResetCachedTiles(e.Location);
        }

        void World_TerrainFeatureListChanged(object sender, StardewModdingAPI.Events.TerrainFeatureListChangedEventArgs e)
        {
            if (e.Added.Any())
                PathFinder.ResetCachedTiles(e.Location);
        }


        private void SafeState()
        {
            //make sure the TestPet was removed
            RemoveTempPetFromFarm();
            //make sure your pet is at the farmhouse
            if (GetPet() != null && !(Game1.getFarm().characters.Contains(pet) && !(Game1.getLocationFromName(Game1.player.homeLocation).characters.Contains(pet))))
            {
                WarpPetToFarmhouse(Game1.player);
            }
        }

        void GameLoop_Saving(object sender, StardewModdingAPI.Events.SavingEventArgs e)
        {
            SafeState();
        }

        void GameLoop_SaveLoaded(object sender, StardewModdingAPI.Events.SaveLoadedEventArgs e)
        {
            pet = null;
            SafeState();
            PathFinder.ResetCachedTiles();
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
            if (!CanUpdatePet())
                return;

            bool PetClicked(Pet p)
            {
                Vector2 grabTile = e.Cursor.GrabTile;
                return pet.currentLocation == Game1.currentLocation && p.yJumpOffset == 0 && p.GetBoundingBox().Intersects(new Rectangle((int)grabTile.X * Game1.tileSize, (int)grabTile.Y * Game1.tileSize, Game1.tileSize, Game1.tileSize));
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

                        foreach (Character c in loc.characters)
                        {
                            if (c is Pet p && PetClicked(p))
                            {
                                SetPet(p);
                                break;
                            }
                        }
                        //Log(pet.Name + " is selected");
                        if (PetClicked(pet) && NotGiftingTreat())
                        {
                            if (Helper.Reflection.GetField<bool>(pet, "wasPetToday").GetValue())
                            {
                                Helper.Input.Suppress(e.Button);
                                SetState(PetState.Waiting);
                                Jump();
                            }
                            else if (!Helper.Reflection.GetField<bool>(pet, "wasPetToday").GetValue())
                            {
                                Helper.Input.Suppress(e.Button);
                                Petting(pet);
                            }
                        }
                        break;
                    case PetState.CatchingUp:
                    case PetState.Waiting:
                    case PetState.Chasing:
                    case PetState.Fetching:
                    case PetState.Retrieve:
                        Pet newPet = null;
                        foreach (Character c in loc.characters)
                        {
                            if (c is Pet p && p != pet && PetClicked(p))
                            {
                                newPet = p;
                                break;
                            }
                        }

                        if ((loc is Farm || loc is FarmHouse) && NotGiftingTreat())
                        {
                            if (newPet != null && PetClicked(newPet) && Helper.Reflection.GetField<bool>(newPet, "wasPetToday").GetValue())
                            {
                                Helper.Input.Suppress(e.Button);
                                SetState(PetState.Waiting);
                                SetPet(newPet);
                                Jump();
                            }
                            else if (PetClicked(pet))
                            {
                                Helper.Input.Suppress(e.Button);
                                SetState(PetState.Vanilla);
                                Jump();
                            }
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

                    Passables.AddRange(PathFinder.GetPassableNeighbors(e.Cursor.Tile, pet));
                    if (PathFinder.IsPassable(e.Cursor.Tile, pet) && !Passables.Contains(e.Cursor.Tile))
                        Passables.Add(e.Cursor.Tile);
                    else if (!NonPassables.Contains(e.Cursor.Tile))
                        NonPassables.Add(e.Cursor.Tile);
                }


                if (CanThrow(Game1.player.ActiveObject))
                {
                    Throw(Game1.player.ActiveObject, e.Cursor.Tile);
                }
                else if (Game1.player.CurrentTool is Tool tool && tool != null && PetClicked(pet) && !Game1.player.usingTool)
                {
                    if (tool is Hoe || tool is Axe || tool is Pickaxe || tool is WateringCan)
                    {
                        GetHitByTool();
                    }
                }

            }
        }

        private Vector2 oldPetPos;

        void GameLoop_OneSecondUpdateTicked(object sender, StardewModdingAPI.Events.OneSecondUpdateTickedEventArgs e)
        {
            if (!CanUpdatePet())
                return;

            switch (petState)
            {
                case PetState.Vanilla:
                    break;
                case PetState.CatchingUp:
                case PetState.Waiting:
                case PetState.Retrieve:

                    if (petState == PetState.Waiting || !Compare(pet.Position, oldPetPos))
                    {
                        oldPetPos = pet.Position;
                        SetTimer();
                    }

                    if (PlayerPetDistance() > catch_up_distance && Game1.currentLocation == pet.currentLocation)// && (CurrentPath == null || CurrentPath.Count == 0))
                    {
                        var oldpath = CurrentPath;
                        var path = PathFinder.CalculatePath(pet, new Vector2(Game1.player.getTileX(), Game1.player.getTileY()));//TODO use player.Position instead?
                        CurrentPath = path;

                        if (CurrentPath.Count > 0)
                        {
                            if (oldpath == null || oldpath.Count == 0)
                            {
                                SetPetPositionFromTile(CurrentPath.Peek()); //this sets the pet on a proper tile. This is only done if the pet starts moving 
                                //Log("Snapped pet position to first tile of current path");
                            }
                            else
                            {
                                CurrentPath.Dequeue(); //if pet is already moving: remove first node. Otherwise the pet might be a few pixels ahead and tries to move back first.
                            }

                            if (petState == PetState.Waiting)
                                SetState(PetState.CatchingUp);
                        }
                        else
                        {
                            CannotReachPlayer();
                        }
                    }

                    TryChaseCritterInRange();

                    if (TimeOut(60))
                    {
                        Log("timeout during " + petState);
                        Confused();
                        SetState(PetState.Waiting);
                    }
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

        int FacingDirectionBeforeUpdate = 0;

        void GameLoop_UpdateTicking(object sender, StardewModdingAPI.Events.UpdateTickingEventArgs e)
        {
            if (!CanUpdatePet())
                return;

            if (petState != PetState.Vanilla)
                GetPet().CurrentBehavior = -1;

            FacingDirectionBeforeUpdate = GetPet().FacingDirection;
        }


        void GameLoop_UpdateTicked(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
        {
            if (!CanUpdatePet())
                return;

            //if (pet.FacingDirection != petFace)
            //    Monitor.Log("Game changed FacingDirection: " + petFace + " -> " + GetPet().FacingDirection);

            switch (petState)
            {
                case PetState.Vanilla:
                    break;
                case PetState.CatchingUp:
                case PetState.Chasing:
                case PetState.Fetching:
                case PetState.Retrieve:

                    if (CurrentPath.Count > 0)
                    {
                        CatchUp(FacingDirectionBeforeUpdate);
                    }

                    if (petState == PetState.Fetching && CurrentPath.Count == 4 && GetPet() is Cat cat)
                        cat.leap(null);

                    int check_distance = petState == PetState.CatchingUp || petState == PetState.Retrieve ? 4 : 6;

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
                    else if (PetCurrentCatchUpGoalDistance() <= check_distance)//next_path_pixel_distance) TODO
                    {
                        //Vector2 velocityTowardsGoal = GetVelocity();
                        Vector2 goalPos = CurrentPath.Dequeue() * Game1.tileSize;
                        pet.Position = goalPos;

                    }

                    if (petState == PetState.CatchingUp && PlayerPetDistance() <= 2)
                        SetState(PetState.Waiting);
                    if (petState == PetState.Retrieve && PlayerPetDistance() <= 1)
                    {
                        DropItem();
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

            Vector2 PlayerTile = new Vector2((int)(Game1.player.Position.X / Game1.tileSize), (int)(Game1.player.Position.Y / Game1.tileSize));


            bool EnteredLeftRight()
            {
                return PlayerTile.X < 2 || PlayerTile.X > e.NewLocation.map.GetLayer("Back").LayerWidth - 2;
            }

            bool EnteredTopBot()
            {
                return PlayerTile.Y < 2 || PlayerTile.Y > e.NewLocation.map.GetLayer("Back").LayerHeight - 2;
            }

            if (e.NewLocation.Name == "Temp")
            {
                Monitor.Log("Pet cannot follow on temporary map. Warping to Farm and unfollow.");
                WarpPet(Game1.getLocationFromName("Farm"), new Vector2(54f, 8f));
                SetState(PetState.Vanilla);
            }
            else if (e.NewLocation is Town
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
                    Utility.recursiveFindOpenTileForCharacter(pet, e.NewLocation, PlayerTile, 10)
                };

                if (e.NewLocation is CommunityCenter)
                {
                    tryTiles.Insert(0, PlayerTile - new Vector2(0, 5));
                }
                else if ((e.NewLocation is Beach || e.NewLocation is BeachNightMarket) && e.OldLocation is Town)
                {
                    tryTiles.Insert(0, PlayerTile + new Vector2(0, 5));
                }
                else if (e.NewLocation is Town && (e.OldLocation is Beach || e.OldLocation is BeachNightMarket))
                {
                    tryTiles.Insert(0, PlayerTile - new Vector2(0, 5));
                    tryTiles.Insert(0, PlayerTile - new Vector2(1, 5));
                    tryTiles.Insert(0, PlayerTile - new Vector2(-1, 5));
                }

                if (EnteredLeftRight())
                {
                    tryTiles.Insert(0, PlayerTile + new Vector2(0, 1));
                    tryTiles.Insert(0, PlayerTile - new Vector2(0, 1));
                }

                if (EnteredTopBot())
                {
                    tryTiles.Insert(0, PlayerTile + new Vector2(1, 0));
                    tryTiles.Insert(0, PlayerTile - new Vector2(1, 0));
                }

                Vector2 petTile = tryTiles.Find(tile => PathFinder.IsPassable(tile, pet));
                //Log("Player loc: " + new Vector2(Game1.player.getTileX(), Game1.player.getTileY()) + ", horse: " + (Game1.player.isRidingHorse() ? new Vector2(Game1.player.mount.getTileX(), Game1.player.mount.getTileY()).ToString() : ""));
                if (petTile != null)
                {
                    WarpPet(e.NewLocation, petTile);
                    Log("Warped pet to " + petTile);
                }

                else
                    Log("Could not find position for pet", LogLevel.Error);
            }
            else if (e.NewLocation is Farm)
            {
                WarpPet(Game1.getLocationFromName("Farm"), new Vector2(54f, 8f));
                pet.position.X -= 64f;
            }

            else if (e.NewLocation is FarmHouse farmHouse)
            {
                WarpPetToFarmhouse(farmHouse.owner);
            }
            else if (e.NewLocation is MineShaft && !(e.OldLocation is MineShaft) || e.NewLocation is Woods /*|| e.NewLocation is Sewer*/)
            {
                if (config.show_message_on_warp)
                    Game1.showGlobalMessage(Helper.Translation.Get("warp.todangerous", new { petname = GetPet().displayName }));
            }
            else if (!e.NewLocation.isOutdoors && e.OldLocation.isOutdoors)
            {
                if (config.show_message_on_warp && !e.NewLocation.isFarmBuildingInterior() && !(e.NewLocation is FarmCave))
                    Game1.showGlobalMessage(Helper.Translation.Get("warp.waitingoutside", new { petname = GetPet().displayName }));
            }
            else
            {
                Monitor.Log("warped to unknown location: " + Game1.currentLocation.Name, LogLevel.Trace);
            }
        }

        private void WarpPet(GameLocation location, Vector2 tile)
        {
            Pet pet = GetPet();
            if (pet == null)
            {
                Monitor.Log("Couldn't find pet to warp to "+location.Name, LogLevel.Trace);
                return;
            }

            if (pet.currentLocation == null)
                pet.currentLocation = location;
            Game1.warpCharacter(pet, location, tile);
        }

        private void WarpPetToFarmhouse(Farmer owner)
        {
            Pet pet = GetPet();
            if (pet == null)
            {
                Monitor.Log("Couldn't find pet to warp to farmhouse", LogLevel.Trace);
                return;
            }
            if (pet.currentLocation == null)
                pet.currentLocation = Game1.currentLocation;
            GetPet().warpToFarmHouse(owner);
        }

        private bool CanUpdatePet() => Context.IsWorldReady && Game1.currentLocation != null && Game1.player.hasPet() && GetPet() != null && Game1.activeClickableMenu == null && !Game1.eventUp;

        private static IMonitor _Monitor;
        public static void Log(string msg, LogLevel level = LogLevel.Trace) => _Monitor.Log(msg, level);

        private static IModHelper _Helper;
        public static IModHelper GetHelper() => _Helper;



    }
}