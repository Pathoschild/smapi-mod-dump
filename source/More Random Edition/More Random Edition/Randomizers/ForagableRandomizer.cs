/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using StardewValley.GameData.Objects;
using System.Collections.Generic;
using System.Linq;
using SVLocationData = StardewValley.GameData.Locations.LocationData;

namespace Randomizer
{
    /// <summary>
    /// This randomizes the Locations.xnb data
    /// </summary>
    public class ForagableRandomizer
	{
		private static RNG Rng { get; set; }

		private static List<Item> AllForagables { get; set; }

		public static List<Item> SpringForagables { get; } = new List<Item>();
		public static List<Item> SummerForagables { get; } = new List<Item>();
		public static List<Item> FallForagables { get; } = new List<Item>();
		public static List<Item> WinterForagables { get; } = new List<Item>();

		public static List<Item> BeachForagables { get; } = new List<Item>();
		public static List<Item> WoodsForagables { get; } = new List<Item>();
		public static List<Item> DesertForagables { get; } = new List<Item>();

		/// <summary>
		/// Randomizes all foragables to a random season and location
		/// </summary>
		/// <param name="objectReplacements">
		/// The in-progress objectInformation - it's important to NOT
		/// overwrite anything in this, since it contains already edited information already
		/// This is passed into edit the tooltip of the foragables to include its season
		/// </param>
		/// <returns>A dictionary of locations to replace</returns>
		public static Dictionary<string, SVLocationData> Randomize(
			Dictionary<string, ObjectData> objectReplacements)
		{
            ClearForagableLists();

            var locationsReplacements = new Dictionary<string, SVLocationData>();
            if (!Globals.Config.RandomizeForagables)
            {
				// Fill with defaults, since ItemList.GetForagables needs this data
				PopulateDefaultForagables();
                return locationsReplacements;
            }

            Rng = RNG.GetFarmRNG(nameof(ForagableRandomizer));
            AllForagables = ItemList.Items.Values.Where(x => x.ShouldBeForagable).ToList();

			GroupForagablesBySeason();

			List<LocationData> foragableLocationDataList = GetForagableLocationDataList();

            foreach (LocationData foragableLocationData in foragableLocationDataList)
			{
				locationsReplacements.Add(
					foragableLocationData.LocationName, 
					foragableLocationData.GetLocationDataWithModifiedForagableData());
			}

			AddTooltipToForagableItems(objectReplacements, foragableLocationDataList);
			WriteToSpoilerLog(foragableLocationDataList);

			return locationsReplacements;
		}

		/// <summary>
		/// Clears the lists of foragables
		/// </summary>
		public static void ClearForagableLists()
		{
            SpringForagables.Clear();
            SummerForagables.Clear();
            FallForagables.Clear();
            WinterForagables.Clear();
            BeachForagables.Clear();
            WoodsForagables.Clear();
            DesertForagables.Clear();
        }

        /// <summary>
        /// Sets the foragable arrays to contain the vanilla foragables
        /// </summary>
        private static void PopulateDefaultForagables()
        {
            SpringForagables.AddRange(new List<Item>()
            {
                ObjectIndexes.WildHorseradish.GetItem(),
                ObjectIndexes.Daffodil.GetItem(),
                ObjectIndexes.Leek.GetItem(),
                ObjectIndexes.Dandelion.GetItem()
            });

            SummerForagables.AddRange(new List<Item>()
            {
                ObjectIndexes.SpiceBerry.GetItem(),
                ObjectIndexes.Grape.GetItem(),
                ObjectIndexes.SweetPea.GetItem()
            });

            FallForagables.AddRange(new List<Item>()
            {
                ObjectIndexes.WildPlum.GetItem(),
                ObjectIndexes.Hazelnut.GetItem(),
                ObjectIndexes.Blackberry.GetItem(),
                ObjectIndexes.CommonMushroom.GetItem(),

            });

            WinterForagables.AddRange(new List<Item>()
            {
                ObjectIndexes.WinterRoot.GetItem(),
                ObjectIndexes.CrystalFruit.GetItem(),
                ObjectIndexes.SnowYam.GetItem(),
                ObjectIndexes.Crocus.GetItem(),
                ObjectIndexes.Holly.GetItem(),
            });

            BeachForagables.AddRange(new List<Item>()
            {
                ObjectIndexes.NautilusShell.GetItem(),
                ObjectIndexes.Coral.GetItem(),
                ObjectIndexes.SeaUrchin.GetItem(),
                ObjectIndexes.RainbowShell.GetItem(),
                ObjectIndexes.Clam.GetItem(),
                ObjectIndexes.Cockle.GetItem(),
                ObjectIndexes.Mussel.GetItem(),
                ObjectIndexes.Oyster.GetItem()
            });

            WoodsForagables.AddRange(new List<Item>()
            {
                ObjectIndexes.Morel.GetItem(),
                ObjectIndexes.CommonMushroom.GetItem(),
                ObjectIndexes.RedMushroom.GetItem(),
                ObjectIndexes.FiddleheadFern.GetItem(),
                ObjectIndexes.Chanterelle.GetItem(),
            });

            DesertForagables.AddRange(new List<Item>()
            {
                ObjectIndexes.CactusFruit.GetItem(),
                ObjectIndexes.Coconut.GetItem()
            });
        }

		/// <summary>
		/// Writes out the results for the given season and foragable location data
		/// </summary>
		/// <param name="season">The season to write the results for</param>
		/// <param name="locationData">The data to write the results for</param>
		private static void WriteResultsForSeason(Seasons season, LocationData locationData)
		{
			List<ForagableData> dataToWrite = null;
			switch (season)
			{
				case Seasons.Spring:
					dataToWrite = locationData.SpringForagables;
					break;
				case Seasons.Summer:
					dataToWrite = locationData.SummerForagables;
					break;
				case Seasons.Fall:
					dataToWrite = locationData.FallForagables;
					break;
				case Seasons.Winter:
					dataToWrite = locationData.WinterForagables;
					break;
			}

			if (dataToWrite == null)
			{
				Globals.ConsoleError($"Could not find the foragable list for {season}");
				return;
			}

			Globals.SpoilerWrite("");
			Globals.SpoilerWrite(season.ToString());
			foreach (ForagableData foragableData in dataToWrite)
			{
				Globals.SpoilerWrite($"{foragableData.ItemId}: {ItemList.Items[foragableData.ItemId].Name} | {foragableData.ItemRarity}");
			}
		}

		/// <summary>
		/// Populates the list of foragables by season
		/// </summary>
		public static void GroupForagablesBySeason()
		{
			List<Item> foragableItems = ItemList.Items.Values.Where(x => x.ShouldBeForagable).ToList();

			// Initializes each unique area with their unique foragables
			AddMultipleToList(foragableItems, BeachForagables, 3);
			AddMultipleToList(foragableItems, WoodsForagables, 1);
			AddMultipleToList(foragableItems, DesertForagables, 2);

			// Initializes each season with an even number of foragables
			int numberToDistribute = foragableItems.Count / 4;
			AddMultipleToList(foragableItems, SpringForagables, numberToDistribute);
			AddMultipleToList(foragableItems, SummerForagables, numberToDistribute);
			AddMultipleToList(foragableItems, FallForagables, numberToDistribute);
			AddMultipleToList(foragableItems, WinterForagables, numberToDistribute);

			// Ensure the rest of the foragables get distributed
			DistributeRemainingForagables(foragableItems);
		}

		/// <summary>
		/// Populates the given list with five foragables
		/// </summary>
		/// <param name="foragableList">The list of all foragables to choose from</param>
		/// <param name="listToPopulate">The list to populate</param>
		private static void AddMultipleToList(List<Item> foragableList, List<Item> listToPopulate, int numberToAdd)
		{
			if (foragableList.Count < numberToAdd)
			{
				Globals.ConsoleError($"Not enough foragables to initialize everything - trying to add {numberToAdd} from a list of {foragableList.Count}.");
				return;
			}

			for (int i = 0; i < numberToAdd; i++)
			{
				AddToList(foragableList, listToPopulate);
			}
		}

		/// <summary>
		/// Distribute the rest of the foragable list
		/// </summary>
		/// <param name="foragableList">The list of all foragables to choose from</param>
		private static void DistributeRemainingForagables(List<Item> foragableList)
		{
			bool keepLooping = true;

			while (keepLooping)
			{
				int season = Rng.NextIntWithinRange(0, 3);
				switch (season)
				{
					case 0:
						keepLooping = AddToList(foragableList, SpringForagables);
						break;
					case 1:
						keepLooping = AddToList(foragableList, SummerForagables);
						break;
					case 2:
						keepLooping = AddToList(foragableList, FallForagables);
						break;
					case 3:
						keepLooping = AddToList(foragableList, WinterForagables);
						break;
					default:
						Globals.ConsoleError("Should not have generated a value above 3 for a season check!");
						keepLooping = false;
						break;
				}
			}
		}

		/// <summary>
		/// Adds a foragable to the given list
		/// </summary>
		/// <param name="foragableList">The list of all foragables to choose from</param>
		/// <param name="listToPopulate">The list to populate</param>
		/// <returns>Whether there's more in the list to add after the call</returns>
		private static bool AddToList(List<Item> foragableList, List<Item> listToPopulate)
		{
			if (foragableList.Count == 0) { return false; }
			listToPopulate.Add(Rng.GetAndRemoveRandomValueFromList(foragableList));
			return foragableList.Count > 0;
		}

		/// <summary>
		/// Gets the list for foragable location data - one per location
		/// </summary>
		/// <returns></returns>
		private static List<LocationData> GetForagableLocationDataList()
		{
			var forgableLocationDataList = new List<LocationData>();
			foreach (Locations location in LocationData.ForagableLocations)
			{
				// Add any item to the desert
				if (location == Locations.Desert)
				{
					AddUniqueNewForagable(DesertForagables);
				}

				LocationData foragableLocationData = new(location);

				PopulateLocationBySeason(foragableLocationData, Seasons.Spring);
				PopulateLocationBySeason(foragableLocationData, Seasons.Summer);
				PopulateLocationBySeason(foragableLocationData, Seasons.Fall);
				PopulateLocationBySeason(foragableLocationData, Seasons.Winter);

                forgableLocationDataList.Add(foragableLocationData);
			}

			return forgableLocationDataList;
		}

		/// <summary>
		/// Populate the location data's season arrays
		/// </summary>
		/// <param name="foragableLocationData">The location data</param>
		/// <param name="season">The season</param>
		private static void PopulateLocationBySeason(LocationData foragableLocationData, Seasons season)
		{
			List<ForagableData> foragableDataList = null;
			List<Item> foragableItemList = null;

			switch (season)
			{
				case Seasons.Spring:
					foragableDataList = foragableLocationData.SpringForagables;
					foragableItemList = SpringForagables;
					break;
				case Seasons.Summer:
					foragableDataList = foragableLocationData.SummerForagables;
					foragableItemList = SummerForagables;
					break;
				case Seasons.Fall:
					foragableDataList = foragableLocationData.FallForagables;
					foragableItemList = FallForagables;
					break;
				case Seasons.Winter:
					foragableDataList = foragableLocationData.WinterForagables;
					foragableItemList = WinterForagables;
					break;
			}

			if (foragableDataList == null || foragableItemList == null)
			{
				Globals.ConsoleError($"Could not get foragable list for season: {season}");
			}

			if (foragableLocationData.Location == Locations.Desert)
			{
				foragableItemList = DesertForagables;
			}

			// Give the beach a random item from the season, then only assign the beach items after that
			if (foragableLocationData.Location == Locations.Beach)
			{
				Item randomSeasonItem = Rng.GetRandomValueFromList(foragableItemList);
				foragableDataList.Add(new ForagableData(randomSeasonItem, Rng));
				foragableItemList = BeachForagables;
			}

			// Give the woods a random item from ANY season, then only assign the woods items after that
			if (foragableLocationData.LocationName == "Woods")
			{
				AddUniqueNewForagable(WoodsForagables);
				foragableItemList = WoodsForagables;
			}

			foreach (Item item in foragableItemList)
			{
				foragableDataList.Add(new ForagableData(item, Rng));
			}

			// Remove the item that was added to the woods
			if (foragableLocationData.LocationName == "Woods" && WoodsForagables.Count > 1)
			{
				WoodsForagables.RemoveAt(WoodsForagables.Count - 1);
			}

			// Add a random item that's really rare to see
			Item randomItem = Rng.GetRandomValueFromList(
				ItemList.Items.Values.Where(x =>
					x.DifficultyToObtain >= ObtainingDifficulties.MediumTimeRequirements &&
					x.DifficultyToObtain < ObtainingDifficulties.Impossible).ToList()
			);
			foragableDataList.Add(new ForagableData(randomItem, Rng) { ItemRarity = 0.001 });
		}

		/// <summary>
		/// Adds a random new item to a list
		/// </summary>
		/// <param name="listToPopulate">The list to populate</param>
		private static void AddUniqueNewForagable(List<Item> listToPopulate)
		{
			if (listToPopulate.Count >= AllForagables.Count)
			{
				Globals.ConsoleWarn("Tried to add a unique foragable when the given list was full!");
				return;
			}

			int itemIndex;
			do
			{
				itemIndex = Rng.Next(0, AllForagables.Count);
			} while (listToPopulate.Contains(AllForagables[itemIndex]));

			listToPopulate.Add(AllForagables[itemIndex]);
		}

		/// <summary>
		/// Adds tooltips to foragable items to help the player know when
		/// they're available
		/// </summary>
		/// <param name="objectReplacements">
		/// The object information replacements dictionary from EditedObjectInfo
		/// It is important to NOT modify this more than we intend to
		/// </param>
		/// <param name="foragableLocationDataList">The list of foragable data that we constructed</param>
        private static void AddTooltipToForagableItems(
			Dictionary<string, ObjectData> objectReplacements,
			List<LocationData> foragableLocationDataList)
		{
			Dictionary<string, List<Seasons>> foragableIdToSeasons = new();

			foreach (LocationData foragableLocationData in foragableLocationDataList)
			{
                FindForagableSeasons(foragableIdToSeasons, foragableLocationData.SpringForagables, Seasons.Spring);
                FindForagableSeasons(foragableIdToSeasons, foragableLocationData.SummerForagables, Seasons.Summer);
                FindForagableSeasons(foragableIdToSeasons, foragableLocationData.FallForagables, Seasons.Fall);
                FindForagableSeasons(foragableIdToSeasons, foragableLocationData.WinterForagables, Seasons.Winter);
            }

			foreach (var foragableData in foragableIdToSeasons)
			{
				string itemId = foragableData.Key;
				var seasonList = foragableData.Value.Distinct();
                if (!seasonList.Any())
                {
                    Globals.ConsoleWarn($"Foragable with no season: ${itemId}");
                }
				
                // Insert the data into the object replacements if it isn't there already
                // This is expected for Grapes and Cactus fruit, where we append to the description
                if (!objectReplacements.ContainsKey(itemId))
                {
					objectReplacements[itemId] = EditedObjects.DefaultObjectInformation[itemId];
                }

				// Now append the season string to the description in the object replacements
				// Also yes, we're using the fish translations here since it's item-agnostic
				string tooltip = seasonList.Count() >= 4
					? Globals.GetTranslation("fish-tooltip-seasons-all")
					: Globals.GetTranslation("fish-tooltip-seasons", new
					{
						seasons = string.Join(", ", seasonList.Select(season =>
							Globals.GetTranslation($"seasons-{season.ToString().ToLower()}")))
					});

				var currentDescription = objectReplacements[itemId].Description;
				objectReplacements[itemId].Description = $"{currentDescription} {tooltip}";
            }
        }

		/// <summary>
		/// Fills a dictionary with a list of seasons that a foragable can be collected in
		/// </summary>
		/// <param name="foragableIdToSeasons">The dictionary to fill</param>
		/// <param name="foragableList">Fhe list of foragables to loop through</param>
		/// <param name="season">The season the foragable list is for</param>
		private static void FindForagableSeasons(
			Dictionary<string, List<Seasons>> foragableIdToSeasons, 
			List<ForagableData> foragableList, 
			Seasons season)
		{
			foreach(ForagableData foragableData in foragableList)
			{
				// Skip over non-foragables - they are the 1 in 1k drops
				// Also skip over 1 in 1k drops in case they are foragables!
				// We only want the tooltip for when the player can reasonably get them
                string itemId = foragableData.ItemId.ToString();
                if (!ItemList.GetItemFromStringId(itemId).IsForagable ||
                    foragableData.ItemRarity == 0.001)
                {
                    continue;
                }

                if (!foragableIdToSeasons.ContainsKey(itemId))
				{
                    foragableIdToSeasons[itemId] = new List<Seasons>();
                }
				foragableIdToSeasons[itemId].Add(season);
            }
		}

        /// <summary>
        /// Writes the results to the spoiler log
        /// </summary>
        /// <param name="foragableLocationDataList">The list of location data that was randomized</param>
        private static void WriteToSpoilerLog(List<LocationData> foragableLocationDataList)
        {
            Globals.SpoilerWrite("==== Foragables ===");
            foreach (LocationData foragableLocationData in foragableLocationDataList)
            {
                Globals.SpoilerWrite("");
                Globals.SpoilerWrite($">> {foragableLocationData.LocationName} <<");

                WriteResultsForSeason(Seasons.Spring, foragableLocationData);
                WriteResultsForSeason(Seasons.Summer, foragableLocationData);
                WriteResultsForSeason(Seasons.Fall, foragableLocationData);
                WriteResultsForSeason(Seasons.Winter, foragableLocationData);
            }
            Globals.SpoilerWrite("");
        }
    }
}
