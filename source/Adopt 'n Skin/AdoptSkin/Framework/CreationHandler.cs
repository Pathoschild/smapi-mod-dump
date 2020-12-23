/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Gathouria/Adopt-Skin
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using xTile.Layers;
using xTile.Dimensions;
using xTile.Tiles;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Menus;
using StardewValley.Characters;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Tools;

namespace AdoptSkin.Framework
{
    class CreationHandler
    {
        /************************
        ** Fields
        *************************/

        /// <summary>Randomizer for logic within CreationHandler instances.</summary>
        private readonly Random Randomizer = new Random();

        /// <summary>The string representation of a Horse instance without an associated Stable building.
        internal static readonly Guid ZeroHorseID = new Guid("00000000-0000-0000-0000-000000000000");

        /// <summary>Reference to Adopt & Skin's ModEntry. Used to access creature information and print information to the monitor when necessary.</summary>
        internal ModEntry Earth;

        /// <summary>The Stray and WildHorse on the map for the day. Null is there is none.</summary>
        internal Stray StrayInfo = null;
        internal WildHorse HorseInfo = null;

        internal bool FirstHorseReceived = false;
        internal bool FirstPetReceived = false;

        internal bool PetsPlacedForDay = false;
        internal bool PetsPlacedForNight = false;

        internal static readonly int AdoptPrice = ModEntry.Config.StrayAdoptionPrice;
        internal static readonly int WildHorseChance = ModEntry.Config.WildHorseChancePercentage;
        internal static readonly int StrayChance = ModEntry.Config.StrayChancePercentage;






        /// <summary>The handler and creator for potential pets and wild horses to adopt</summary>
        /// <param name="modentry"></param>
        internal CreationHandler(ModEntry modentry)
        {
            Earth = modentry;
        }

        /// <summary>Calculates variables that change each day</summary>
        internal void ProcessNewDay(object sender, DayStartedEventArgs e)
        {
            // Make the luck bonus into a percentage for our use
            int luckBonus = (int)(Game1.player.team.sharedDailyLuck.Value * 100);
            // Luck affect has been turned off in the Config
            if (!ModEntry.Config.ChanceAffectedByLuck)
                luckBonus = 0;

            // Only allow the host player to load in strays and wild horses
            if (!Context.IsMainPlayer)
                return;
            // Check chances for Stray and WildHorse to spawn, add creation to update loop if spawn should occur
            if (ModEntry.Config.StraySpawn && FirstPetReceived && Randomizer.Next(0, 100) - luckBonus < StrayChance)
                ModEntry.SHelper.Events.GameLoop.UpdateTicked += this.PlaceStray;
            if (ModEntry.Config.WildHorseSpawn && FirstHorseReceived && Randomizer.Next(0, 100) - luckBonus < WildHorseChance)
                ModEntry.SHelper.Events.GameLoop.UpdateTicked += this.PlaceWildHorse;

            // Spread out pets from around water dish
            if (ModEntry.Config.DisperseCuddlePuddle)
                ModEntry.SHelper.Events.Player.Warped += this.SpreadPets;
        }


        /// <summary>Removes Stray and WildHorse instances from the map</summary>
        internal void ProcessEndDay(object sender, DayEndingEventArgs e)
        {
            // Remove any Strays and WildHorses from the map
            if (StrayInfo != null && StrayInfo.PetInstance != null)
                StrayInfo.RemoveFromWorld();
            if (HorseInfo != null && HorseInfo.HorseInstance != null)
                HorseInfo.RemoveFromWorld();

            PetsPlacedForDay = false;
            PetsPlacedForNight = false;
        }


        /// <summary>Creates a Stray instance</summary>
        internal void PlaceStray(object sender, UpdateTickedEventArgs e)
        {
            if (!Game1.hasLoadedGame || !ModEntry.AssetsLoaded)
                return;
            StrayInfo = new Stray();
            ModEntry.SHelper.Events.GameLoop.UpdateTicked -= this.PlaceStray;
        }


        /// <summary>Creates a WildHorse instance</summary>
        internal void PlaceWildHorse(object sender, UpdateTickedEventArgs e)
        {
            if (!Game1.hasLoadedGame || !ModEntry.AssetsLoaded)
                return;

            HorseInfo = new WildHorse();
            ModEntry.SHelper.Events.GameLoop.UpdateTicked -= this.PlaceWildHorse;
        }


        internal void MoveStrayToSpawn()
        {
            if (StrayInfo != null && StrayInfo.PetInstance != null)
                Game1.warpCharacter(StrayInfo.PetInstance, Stray.Marnies, Stray.CreationLocation);
        }


        internal void SpreadPets(object sender, WarpedEventArgs e)
        {
            // ** TODO: Only one pet on farmer's bed
            // TODO: If no pets, handle error
            List<Pet> pets = ModApi.GetPets().ToList();

            // No pets are in the game
            if (pets.Count == 0)
                return;

            // Ensure Stray isn't moved around by vanilla
            if (typeof(Farm).IsAssignableFrom(e.NewLocation.GetType()) ||
                typeof(FarmHouse).IsAssignableFrom(e.NewLocation.GetType()) ||
                Stray.Marnies != e.NewLocation)
                ModEntry.Creator.MoveStrayToSpawn();

            // Only move pets once at beginning of day and once at night
            if ((!Game1.isDarkOut() && PetsPlacedForDay) ||
                (Game1.isDarkOut() && PetsPlacedForNight))
                return;


            if (IsIndoorWeather())
            {
                IndoorWeatherPetSpawn();
                PetsPlacedForDay = true;
                PetsPlacedForNight = true;
                return;
            }
            else
            {
                // Place everyone at the correct starting point, at the water dish
                foreach (Pet pet in ModApi.GetPets())
                    pet.setAtFarmPosition();

                // Find area to warp pets to
                Farm farm = Game1.getFarm();
                int initX = (int)pets[0].getTileLocation().X;
                int initY = (int)pets[0].getTileLocation().Y;
                List<Vector2> warpableTiles = new List<Vector2>();
                int cer = ModEntry.Config.CuddleExplosionRadius;

                // Collect a set of potential tiles to warp a pet to
                for (int i = -cer; i < cer; i++)
                {
                    for (int j = -cer; j < cer; j++)
                    {
                        int warpX = initX + i;
                        int warpY = initY + j;
                        if (warpX < 0)
                            warpX = 0;
                        if (warpY < 0)
                            warpY = 0;

                        Vector2 tile = new Vector2(warpX, warpY);
                        if (IsTileAccessible(farm, tile))
                            warpableTiles.Add(tile);
                    }
                }

                // No placeable tiles found within the range given in the Config
                if (warpableTiles.Count == 0)
                {
                    ModEntry.SMonitor.Log($"Pets cannot be spread within the given radius: {cer}", LogLevel.Debug);
                    return;
                }

                // Spread pets
                foreach (Pet pet in ModApi.GetPets())
                {
                    Vector2 ranTile = warpableTiles[Randomizer.Next(0, warpableTiles.Count)];
                    Game1.warpCharacter(pet, farm, ranTile);
                }

                PetsPlacedForDay = true;
            }
        }


        /// <summary>Spawns all owned pets into the FarmHouse</summary>
        internal void IndoorWeatherPetSpawn()
        {
            foreach (Pet pet in ModApi.GetPets())
                pet.warpToFarmHouse(Game1.player);
        }


        internal bool IsIndoorWeather() { return (Game1.isRaining || Game1.isSnowing || Game1.isLightning || Game1.isDarkOut()); }


        internal static bool IsTileAccessible(GameLocation map, Vector2 tile)
        {
            if (!map.isTileOnMap(tile) ||
                map.isOpenWater((int)tile.X, (int)tile.Y) ||
                map.isBehindTree(tile) ||
                map.isBehindBush(tile) ||
                map.isCollidingWithWarpOrDoor(new Microsoft.Xna.Framework.Rectangle((int)tile.X, (int)tile.Y, 1, 1)) != null ||
                IsBuildingInCreatureSpace(map, tile) ||
                !map.isTileLocationTotallyClearAndPlaceableIgnoreFloors(tile) ||
                !map.isTileLocationTotallyClearAndPlaceableIgnoreFloors(new Vector2(tile.X + 1, tile.Y)))
            {
                return false;
            }

            return true;
        }


        /// <summary>Returns true if a building collides with the 2x2 space that a creature would occupy if placed on the given tile</summary>
        internal static bool IsBuildingInCreatureSpace(GameLocation map, Vector2 tile)
        {
            if (map is BuildableGameLocation buildableLocation)
            {
                foreach (Building building in buildableLocation.buildings)
                    if (building.occupiesTile(tile) ||
                        building.occupiesTile(new Vector2(tile.X + 1, tile.Y)) ||
                        building.occupiesTile(new Vector2(tile.X + 1, tile.Y + 1)) ||
                        building.occupiesTile(new Vector2(tile.X, tile.Y + 1)))
                        return true;
            }
            return false;
        }


        /// <summary>Returns the first Stable instance found on the farm.</summary>
        internal static Guid GetStableID()
        {
            Guid stableID = ZeroHorseID;

            foreach (Horse horse in ModApi.GetHorses())
                if (horse.HorseId != ZeroHorseID)
                {
                    stableID = horse.HorseId;
                    break;
                }

            return stableID;
        }


        /// <summary>Check to see if the player is attempting to interact with a Stray or WildHorse</summary>
        internal void AdoptableInteractionCheck(object sender, ButtonPressedEventArgs e)
        {
            if (StrayInfo != null)
            {
                // Check for mouse, keyboard, and controller versions of interaction
                if ((e.Button.Equals(SButton.MouseRight) || (e.Button.Equals(SButton.ControllerA)) || e.Button.IsActionButton()) &&
                    StrayInfo.PetInstance.withinPlayerThreshold(2))
                {
                    Game1.activeClickableMenu = new ConfirmationDialog("This is one of the strays that Marnie has taken in. \n\n" +
                        $"The animal is wary, but curious. Will you adopt this {ModEntry.Sanitize(StrayInfo.PetInstance.GetType().Name)} for {AdoptPrice}G?", (who) =>
                        {
                            if (ModEntry.Config.DebuggingMode)
                                ModEntry.SMonitor.Log("Confirmation dialog for stray adoption opened", LogLevel.Debug);

                            if (Game1.activeClickableMenu is StardewValley.Menus.ConfirmationDialog cd)
                                cd.cancel();

                            if (Game1.player.Money >= AdoptPrice)
                                Game1.activeClickableMenu = new NamingMenu(PetNamer, $"What will you name it?");
                            else
                                // Exit the naming menu
                                Game1.drawObjectDialogue($"You don't have {AdoptPrice}G..");
                        });
                }
            }
            if (HorseInfo != null)
            {
                // Check for mouse, keyboard, and controller versions of interaction
                if ((e.Button.Equals(SButton.MouseRight) || (e.Button.Equals(SButton.ControllerA)) || e.Button.IsActionButton()) &&
                    HorseInfo.HorseInstance.withinPlayerThreshold(2))
                {
                    
                    Game1.activeClickableMenu = new ConfirmationDialog("This appears to be an escaped horse from a neighboring town. \n\nIt looks tired, but friendly. Will you adopt this horse?", (who) =>
                    {
                        if (ModEntry.Config.DebuggingMode)
                            ModEntry.SMonitor.Log("Confirmation dialog for wild horse adoption opened", LogLevel.Debug);

                        if (Game1.activeClickableMenu is StardewValley.Menus.ConfirmationDialog cd)
                            cd.cancel();

                        Game1.activeClickableMenu = new NamingMenu(HorseNamer, "What will you name this horse?");
                    }, (who) =>
                    {
                        // Exit the naming menu
                        Game1.drawObjectDialogue($"You leave the creature to rest for now. It's got a big, bright world ahead of it.");
                    });
                }
            }
        }






        /*****************************
         ** P E T   A D O P T I O N **
         *****************************/

        /// <summary>Places the pet bed in Marnie's</summary>
        internal void PlacePetBed()
        {
            GameLocation marnies = Game1.getLocationFromName("AnimalShop");
            TileSheet tileSheet = new xTile.Tiles.TileSheet("PetBed", marnies.map, Earth.Helper.Content.GetActualAssetKey("assets/petbed.png"), new xTile.Dimensions.Size(1, 1), new xTile.Dimensions.Size(16, 15));
            marnies.map.AddTileSheet(tileSheet);
            Layer buildingLayer = marnies.map.GetLayer("Buildings");
            buildingLayer.Tiles[17, 15] = new StaticTile(buildingLayer, tileSheet, BlendMode.Alpha, 0);
            marnies.updateMap();
        }


        /// <summary>Places the pet bed in Marnie's on the next day, rather than immediately.</summary>
        internal void PlacePetBedTomorrow(object sender, DayStartedEventArgs e)
        {
            PlacePetBed();
            ModEntry.SHelper.Events.GameLoop.DayStarted -= PlacePetBedTomorrow;
        }
        

        /// <summary>Adopts and names the stray being interacted with. Called in the CheckStray event handler.</summary>
        internal void PetNamer(string petName)
        {
            // Name Pet and add to Adopt & Skin database
            StrayInfo.PetInstance.Name = petName;
            StrayInfo.PetInstance.displayName = petName;
            Earth.AddCreature(StrayInfo.PetInstance, StrayInfo.SkinID);

            // Warp the new Pet to the farmhouse
            StrayInfo.PetInstance.warpToFarmHouse(Game1.player);

            // Set walk-through pet configuration
            if (!ModEntry.Config.WalkThroughPets)
                StrayInfo.PetInstance.farmerPassesThrough = false;

            // Pet is no longer a Stray to keep track of
            StrayInfo = null;

            // Exit the naming menu
            Game1.drawObjectDialogue($"{petName} was brought home.");

            Game1.player.Money -= AdoptPrice;
        }






        /*********************************
         ** H O R S E   A D O P T I O N **
         *********************************/

        /// <summary>Adopts and names the wild horse being interacted with. Called in the CheckHorse event handler.</summary>
        internal void HorseNamer(string horseName)
        {
            // Name Horse and add to Adopt & Skin database
            HorseInfo.HorseInstance.Name = horseName;
            HorseInfo.HorseInstance.displayName = horseName;
            HorseInfo.HorseInstance.HorseId = ModApi.GetRandomStableID();
            Earth.AddCreature(HorseInfo.HorseInstance, HorseInfo.SkinID, Game1.player);

            // Horse is no longer a WildHorse to keep track of
            HorseInfo = null;

            // Exit the naming menu
            Game1.drawObjectDialogue($"Adopted {horseName}.");
        }
    }
}
