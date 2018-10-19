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

using Constants = StardewMods.Common.StardewValley.Constants;

namespace StardewMods.ArchaeologyHouseContentManagementHelper.Framework
{
    internal class LibraryMuseumHelper
    {
        private static readonly Vector2[] LibraryCounterTiles = new Vector2[] { new Vector2(3f, 10f) };

        private static readonly Lazy<LibraryMuseum> lazy = new Lazy<LibraryMuseum>(() => Game1.getLocationFromName(Constants.GAME_LOCATION_LIBRARY_MUSEUM_NAME) as LibraryMuseum);

        private static LibraryMuseum Museum => lazy.Value;

        public static int MuseumPieces => Museum.museumPieces.Count();

        // TODO: What about Game1.stats.NotesFound as an alternative?
        public static int LibraryBooks => Game1.player.archaeologyFound.ContainsKey(Constants.ID_GAME_OBJECT_LOST_BOOK) 
            ? Game1.player.archaeologyFound[Constants.ID_GAME_OBJECT_LOST_BOOK][0] 
            : 0;

        public static int TotalMuseumPieces => LibraryMuseum.totalArtifacts;
        public static int TotalLibraryBooks => LibraryMuseum.totalNotes;

        public static bool HasDonatedAllMuseumPieces => MuseumPieces == TotalMuseumPieces;
        public static bool HasCollectedAllBooks => LibraryBooks == TotalLibraryBooks;

        private static IReflectedMethod getLostBooksLocationsRef = ModEntry.CommonServices.ReflectionHelper.GetMethod(Museum, "getLostBooksLocations");

        public static bool IsPlayerAtCounter(Farmer farmer)
        {
            if (farmer == null)
            {
                ModEntry.CommonServices.Monitor.Log("Error: [farmer] cannot be [null]!", LogLevel.Error);

                throw new ArgumentNullException(nameof(farmer), "Error: [farmer] cannot be [null]!");
            }

            return farmer.currentLocation is LibraryMuseum && LibraryCounterTiles.Contains(farmer.getTileLocation());
        }

        public static int[] GetLostBookIndexList()
        {
            return getLostBooksLocationsRef.Invoke<Dictionary<int, Vector2>>().Select(e => e.Key).ToArray();
        }

        public static bool HasPlayerCollectibleRewards(Farmer farmer)
        {
            if (farmer == null)
            {
                ModEntry.CommonServices.Monitor.Log("Error: [farmer] cannot be [null]!", LogLevel.Error);
                throw new ArgumentNullException(nameof(farmer), "Error: [farmer] cannot be [null]!");
            }

            return Museum.getRewardsForPlayer(farmer).Count > 0;
        }

        public static List<Item> GetRewardsForPlayer(Farmer farmer)
        {
            if (farmer == null)
            {
                ModEntry.CommonServices.Monitor.Log("Error: [farmer] cannot be [null]!", LogLevel.Error);
                throw new ArgumentNullException(nameof(farmer), "Error: [farmer] cannot be [null]!");
            }

            return Museum.getRewardsForPlayer(farmer);
        }

        public static void CollectedReward(Item item, Farmer farmer)
        {
            Museum.collectedReward(item, farmer);
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

        public static MuseumTileClassification GetTileMuseumClassification(int x, int y, bool showSwapVisualIndicator)
        {
            // If items cannot be swapped, don't set visual "can place" indicator
            if (!showSwapVisualIndicator && Museum.museumPieces.ContainsKey(new Vector2((float)x, (float)y)))
            {
                return MuseumTileClassification.Invalid;
            }

            // Only indicate tiles belonging to the designated areas for the museum pieces
            switch (Museum.getTileIndexAt(new Point(x, y), "Buildings"))
            {
                case 1072:
                    return MuseumTileClassification.Valid;
                case 1073:
                    // https://stardewvalleywiki.com/Museum (see "unreachable tiles")
                    if (x >= 31 && x <= 33 && y >= 14 && y <= 15)
                    {
                        return MuseumTileClassification.Limited;
                    }

                    return MuseumTileClassification.Valid;

                case 1074:
                case 1237:
                case 1238:
                    return MuseumTileClassification.Valid;
                default:
                    return MuseumTileClassification.Invalid;
            }
        }
    }
}
