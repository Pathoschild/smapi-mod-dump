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

namespace AdoptSkin.Framework
{
    class CreationHandler
    {
        /************************
        ** Fields
        *************************/

        /// <summary>Randomizer for logic within CreationHandler instances.</summary>
        private readonly Random Randomizer = new Random();

        /// <summary>Reference to Adopt & Skin's ModEntry. Used to access creature information and print information to the monitor when necessary.</summary>
        internal ModEntry Earth;

        /// <summary>The Stray and WildHorse on the map for the day. Null is there is none.</summary>
        internal Stray StrayInfo = null;
        internal WildHorse HorseInfo = null;

        internal bool FirstHorseReceived = false;
        internal bool FirstPetReceived = false;

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
            // Positive luck will give a negative modifier, making it more likely that the random number is in the success range
            int luckBonus = -(int)(Game1.dailyLuck * 100);

            // Luck affect has been turned off in the Config
            if (!ModEntry.Config.ChanceAffectedByLuck)
                luckBonus = 0;

            // Check chances for Stray and WildHorse to spawn, add creation to update loop if spawn should occur
            if (ModEntry.Config.StraySpawn && FirstPetReceived && Randomizer.Next(0, 100) + luckBonus < StrayChance)
                ModEntry.SHelper.Events.GameLoop.UpdateTicked += this.PlaceStray;
            if (ModEntry.Config.WildHorseSpawn && FirstHorseReceived && Randomizer.Next(0, 100) + luckBonus < WildHorseChance)
                ModEntry.SHelper.Events.GameLoop.UpdateTicked += this.PlaceWildHorse;

            // Spread out pets from around water dish
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


        internal void SpreadPets(object sender, WarpedEventArgs e)
        {
            // Only warp pets on return to farm, and when the weather is proper for pets being outside
            if (!(e.NewLocation is Farm) || Game1.isRaining || Game1.isLightning || Game1.isSnowing)
                return;
            // Make sure pets are not otherwise in the FarmHouse
            List<Pet> pets = ModEntry.GetPets().ToList();
            if (!ModEntry.PetSkinMap.ContainsKey(pets[0].Manners) || !(pets[0].currentLocation is Farm))
                return;


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

            // Can't warp in the range specified in the Config
            if (warpableTiles.Count == 0)
            {
                ModEntry.SMonitor.Log($"Pets cannot be spread within the given radius: {cer}", LogLevel.Error);
                return;
            }

            // Warp pets
            foreach (Pet pet in ModEntry.GetPets())
                if (ModEntry.PetSkinMap.ContainsKey(pet.Manners))
                {
                    Vector2 ranTile = warpableTiles[Randomizer.Next(0, warpableTiles.Count)];
                    Game1.warpCharacter(pet, farm, ranTile);
                }

        }


        internal static bool IsTileAccessible(GameLocation map, Vector2 tile)
        {
            if (!map.isTileOnMap(tile) ||
                map.isOpenWater((int)tile.X, (int)tile.Y) ||
                map.isBehindTree(tile) ||
                map.isBehindBush(tile) ||
                map.isCollidingWithWarpOrDoor(new Microsoft.Xna.Framework.Rectangle((int)tile.X, (int)tile.Y, 1, 1)) != null ||
                !map.isTileLocationTotallyClearAndPlaceableIgnoreFloors(tile) ||
                !map.isTileLocationTotallyClearAndPlaceableIgnoreFloors(new Vector2(tile.X + 1, tile.Y)))
            {
                return false;
            }

            return true;
        }






        /*****************************
         ** P E T   A D O P T I O N **
         *****************************/

        /// <summary>Places the pet bed in Marnie's</summary>
        internal void PlaceBetBed()
        {
            GameLocation marnies = Game1.getLocationFromName("AnimalShop");
            TileSheet tileSheet = new xTile.Tiles.TileSheet("PetBed", marnies.map, Earth.Helper.Content.GetActualAssetKey("assets/petbed.png"), new xTile.Dimensions.Size(1, 1), new xTile.Dimensions.Size(16, 15));
            marnies.map.AddTileSheet(tileSheet);
            Layer buildingLayer = marnies.map.GetLayer("Buildings");
            buildingLayer.Tiles[17, 15] = new StaticTile(buildingLayer, tileSheet, BlendMode.Alpha, 0);
            marnies.updateMap();
        }
        

        /// <summary>Check to see if the player is attempting to interact with the stray</summary>
        internal void StrayInteractionCheck(object sender, ButtonPressedEventArgs e)
        {
            if (StrayInfo != null &&
                e.Button.Equals(SButton.MouseRight) &&
                StrayInfo.PetInstance.withinPlayerThreshold(3))
            {
                if ((int)e.Cursor.Tile.X >= StrayInfo.PetInstance.getLeftMostTileX().X && (int)e.Cursor.Tile.X <= StrayInfo.PetInstance.getRightMostTileX().X &&
                    (int)e.Cursor.Tile.Y >= StrayInfo.PetInstance.getTileY() - 1 && (int)e.Cursor.Tile.Y <= StrayInfo.PetInstance.getTileY() + 1)
                {

                    Game1.activeClickableMenu = new ConfirmationDialog("This is one of the strays that Marnie has taken in. \n\n" +
                        $"The animal is wary, but curious. Will you adopt this {ModEntry.Sanitize(StrayInfo.PetInstance.GetType().Name)} for {AdoptPrice}G?", (who) =>
                    {
                        if (Game1.activeClickableMenu is StardewValley.Menus.ConfirmationDialog cd)
                            cd.cancel();

                        if (Game1.player.Money >= AdoptPrice)
                        {
                            Game1.player.Money -= AdoptPrice;
                            Game1.activeClickableMenu = new NamingMenu(PetNamer, $"What will you name it?");
                        }
                        else
                        {
                            // Exit the naming menu
                            Game1.drawObjectDialogue($"You don't have {AdoptPrice}G..");
                        }
                    });
                }
            }
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
        }






        /*********************************
         ** H O R S E   A D O P T I O N **
         *********************************/

        /// <summary>Check to see if the player is attempting to interact with the wild horse</summary>
        internal void WildHorseInteractionCheck(object sender, ButtonPressedEventArgs e)
        {
            if (HorseInfo != null && 
                e.Button.Equals(SButton.MouseRight) &&
                HorseInfo.HorseInstance.withinPlayerThreshold(3))
            {
                if ((int)e.Cursor.Tile.X >= HorseInfo.HorseInstance.getLeftMostTileX().X && (int)e.Cursor.Tile.X <= HorseInfo.HorseInstance.getRightMostTileX().X &&
                    (int)e.Cursor.Tile.Y >= HorseInfo.HorseInstance.getTileY() - 1 && (int)e.Cursor.Tile.Y <= HorseInfo.HorseInstance.getTileY() + 1)
                {

                    Game1.activeClickableMenu = new ConfirmationDialog("This appears to be an escaped horse from a neighboring town. \n\nIt looks tired, but friendly. Will you adopt this horse?", (who) =>
                    {
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


        /// <summary>Adopts and names the wild horse being interacted with. Called in the CheckHorse event handler.</summary>
        internal void HorseNamer(string horseName)
        {
            // Name Horse and add to Adopt & Skin database
            HorseInfo.HorseInstance.Name = horseName;
            HorseInfo.HorseInstance.displayName = horseName;
            Earth.AddCreature(HorseInfo.HorseInstance, HorseInfo.SkinID);

            // Horse is no longer a WildHorse to keep track of
            HorseInfo = null;

            // Exit the naming menu
            Game1.drawObjectDialogue($"Adopted {horseName}.");
        }
    }
}
