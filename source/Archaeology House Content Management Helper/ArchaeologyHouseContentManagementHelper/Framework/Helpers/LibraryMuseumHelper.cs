using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewMods.ArchaeologyHouseContentManagementHelper.Framework
{
    public class LibraryMuseumHelper
    {
        private static Vector2[] libraryCounterTiles = new Vector2[] { new Vector2(3f, 10f) };

        private IModHelper modHelper;
        private IMonitor monitor;
        private IReflectionHelper reflectionHelper;

        //private IReflectedMethod getLostBooksLocations;

        private LibraryMuseum museum;

        public int MuseumPieces => museum.museumPieces.Count();

        // TODO: What about Game1.stats.NotesFound as an alternative?
        public int LibraryBooks => Game1.player.archaeologyFound.ContainsKey(Constants.GAME_OBJECT_LOST_BOOK_ID) 
            ? Game1.player.archaeologyFound[Constants.GAME_OBJECT_LOST_BOOK_ID][0] 
            : 0;

        public int TotalMuseumPieces => LibraryMuseum.totalArtifacts;
        public int TotalLibraryBooks => LibraryMuseum.totalNotes;

        public bool HasDonatedAllMuseumPieces => MuseumPieces == TotalMuseumPieces;
        public bool HasCollectedAllBooks => LibraryBooks == TotalLibraryBooks;

        public LibraryMuseumHelper(IModHelper modHelper, IMonitor monitor, IReflectionHelper reflectionHelper)
        {
            this.modHelper = modHelper;
            this.monitor = monitor;
            this.reflectionHelper = reflectionHelper;

            museum = new LibraryMuseum("Maps\\ArchaeologyHouse", "ArchaeologyHouse");
        }

        public bool IsPlayerAtCounter(Farmer farmer)
        {
            if (farmer == null)
            {
                monitor.Log("Error: [farmer] cannot be [null]!", LogLevel.Error);
                throw new ArgumentNullException(nameof(farmer), "Error: [farmer] cannot be [null]!");
            }

            return farmer.currentLocation is LibraryMuseum && libraryCounterTiles.Contains(farmer.getTileLocation());
        }

        public bool DoesFarmerHaveAnythingToDonate(Farmer farmer)
        {
            if (farmer == null)
            {
                monitor.Log("Error: [farmer] cannot be [null]!", LogLevel.Error);
                throw new ArgumentNullException(nameof(farmer), "Error: [farmer] cannot be [null]!");
            }

            return museum.doesFarmerHaveAnythingToDonate(farmer);
        }

        public bool HasPlayerCollectibleRewards(Farmer farmer)
        {
            if (farmer == null)
            {
                monitor.Log("Error: [farmer] cannot be [null]!", LogLevel.Error);
                throw new ArgumentNullException(nameof(farmer), "Error: [farmer] cannot be [null]!");
            }

            return museum.getRewardsForPlayer(farmer).Count > 0;
        }

        public List<Item> GetRewardsForPlayer(Farmer farmer)
        {
            if (farmer == null)
            {
                monitor.Log("Error: [farmer] cannot be [null]!", LogLevel.Error);
                throw new ArgumentNullException(nameof(farmer), "Error: [farmer] cannot be [null]!");
            }

            return museum.getRewardsForPlayer(farmer);
        }

        public void CollectedReward(Item item, Farmer farmer)
        {
            museum.collectedReward(item, farmer);
        }

        // Copied from LibraryMuseum.cs
        //
        // Adds support for item swap
        public static bool IsTileSuitableForMuseumPiece(int x, int y)
        {
            LibraryMuseum museum = Game1.currentLocation as LibraryMuseum;

            // Allow item to be placed at spot with another item
            if (museum.museumPieces.ContainsKey(new Vector2((float)x, (float)y)))
            {
                return true;
            }

            // Allow item to be placed at empty spots
            switch (museum.getTileIndexAt(new Point(x, y), "Buildings"))
            {
                case 1072:
                case 1073:
                case 1074:
                case 1237:
                case 1238:
                    return true;
                default:
                    // Prevent items from being placed outside the designated area
                    return false;
            }
        }
    }
}
