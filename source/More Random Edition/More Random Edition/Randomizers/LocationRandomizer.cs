/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using StardewValley;
using StardewValley.GameData.Movies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Randomizer
{
	/// <summary>
	/// This randomizes the Locations.xnb data
	/// </summary>
	public class LocationRandomizer
	{
		private static List<Item> AllForagables { get; set; }

		public static List<Item> SpringForagables { get; } = new List<Item>();
		public static List<Item> SummerForagables { get; } = new List<Item>();
		public static List<Item> FallForagables { get; } = new List<Item>();
		public static List<Item> WinterForagables { get; } = new List<Item>();

		public static List<Item> BeachForagables { get; } = new List<Item>();
		public static List<Item> WoodsForagables { get; } = new List<Item>();
		public static List<Item> DesertForagables { get; } = new List<Item>();

		/// <summary>
		/// Randomizes all foragables to a random season and location - does not yet handle fishing or dirt items
		/// </summary>
		/// <param name="objectInformationReplacements">
		/// The in-progress objectInformationReplacements - it's important to NOT
		/// overwrite anything in this, since it contains already edited information already
		/// This is passed into edit the tooltip of the foragables to include its season
		/// </param>
		/// <returns>A dictionary of locations to replace</returns>
		public static Dictionary<string, string> Randomize(
			Dictionary<int, string> objectInformationReplacements)
		{
			AllForagables = ItemList.Items.Values.Where(x => x.ShouldBeForagable).ToList();

			SpringForagables.Clear();
			SummerForagables.Clear();
			FallForagables.Clear();
			WinterForagables.Clear();
			BeachForagables.Clear();
			WoodsForagables.Clear();
			DesertForagables.Clear();

			var locationsReplacements = new Dictionary<string, string>();
			GroupForagablesBySeason();

			List<LocationData> foragableLocationDataList = GetForagableLocationDataList();
			foreach (LocationData foragableLocationData in foragableLocationDataList)
			{
				locationsReplacements.Add(foragableLocationData.LocationName, foragableLocationData.ToString());
			}

			WriteToSpoilerLog(foragableLocationDataList);

			if (!Globals.Config.RandomizeForagables)
			{
				PopulateDefaultForagables();
			}

			AddTooltipToForagableItems(objectInformationReplacements, foragableLocationDataList);

			return locationsReplacements;
		}

		/// <summary>
		/// Writes the results to the spoiler log
		/// </summary>
		/// <param name="foragableLocationDataList">The list of location data that was randomized</param>
		public static void WriteToSpoilerLog(List<LocationData> foragableLocationDataList)
		{
			if (!Globals.Config.RandomizeForagables && !Globals.Config.AddRandomArtifactItem) { return; }

			Globals.SpoilerWrite("==== Foragables and Artifact Spots ====");
			foreach (LocationData foragableLocationData in foragableLocationDataList)
			{
				Globals.SpoilerWrite("");
				Globals.SpoilerWrite($">> {foragableLocationData.LocationName} <<");

				if (Globals.Config.RandomizeForagables)
				{
					WriteResultsForSeason(Seasons.Spring, foragableLocationData);
					WriteResultsForSeason(Seasons.Summer, foragableLocationData);
					WriteResultsForSeason(Seasons.Fall, foragableLocationData);
					WriteResultsForSeason(Seasons.Winter, foragableLocationData);
				}

				if (Globals.Config.AddRandomArtifactItem)
				{
					Globals.SpoilerWrite("");
					Globals.SpoilerWrite($"Extra digging item and rarity: {foragableLocationData.ExtraDiggingItem.Name} | {foragableLocationData.ExtraDiggingItemRarity}");
				}
			}
			Globals.SpoilerWrite("");
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
				Globals.SpoilerWrite($"{foragableData.ItemId}: {ItemList.Items[(ObjectIndexes)foragableData.ItemId].Name} | {foragableData.ItemRarity}");
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
				int season = Globals.RNG.Next(0, 4);
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
			listToPopulate.Add(Globals.RNGGetAndRemoveRandomValueFromList(foragableList));
			return foragableList.Count > 0;
		}

		/// <summary>
		/// Gets the list for foragable location data - one per location
		/// </summary>
		/// <returns></returns>
		private static List<LocationData> GetForagableLocationDataList()
		{
			var foragableLocations = new List<Locations>
			{
				Locations.Desert,
				Locations.BusStop,
				Locations.Forest,
				Locations.Town,
				Locations.Mountain,
				Locations.Backwoods,
				Locations.Railroad,
				Locations.Beach,
				Locations.Woods
			};

			var forgableLocationDataList = new List<LocationData>();
			foreach (Locations location in foragableLocations)
			{
				// Add any item to the desert
				if (location == Locations.Desert)
				{
					AddUniqueNewForagable(DesertForagables);
				}

				LocationData foragableLocationData = new()
				{
					Location = location
				};

				PopulateLocationBySeason(foragableLocationData, Seasons.Spring);
				PopulateLocationBySeason(foragableLocationData, Seasons.Summer);
				PopulateLocationBySeason(foragableLocationData, Seasons.Fall);
				PopulateLocationBySeason(foragableLocationData, Seasons.Winter);
				SetExtraDiggableItemInfo(foragableLocationData);

				forgableLocationDataList.Add(foragableLocationData);
			}

			LocationData mineLocationData = new() { Location = Locations.UndergroundMine };
			SetExtraDiggableItemInfo(mineLocationData);
			forgableLocationDataList.Add(mineLocationData);
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
				Item randomSeasonItem = foragableItemList[Globals.RNG.Next(0, foragableItemList.Count)];
				foragableDataList.Add(new ForagableData(randomSeasonItem.Id));
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
				foragableDataList.Add(new ForagableData(item.Id));
			}

			// Remove the item that was added to the woods
			if (foragableLocationData.LocationName == "Woods" && WoodsForagables.Count > 1)
			{
				WoodsForagables.RemoveAt(WoodsForagables.Count - 1);
			}

			// Add a random item that's really rare to see
			Item randomItem = Globals.RNGGetRandomValueFromList(
				ItemList.Items.Values.Where(x =>
					x.DifficultyToObtain >= ObtainingDifficulties.MediumTimeRequirements &&
					x.DifficultyToObtain < ObtainingDifficulties.Impossible).ToList()
			);
			foragableDataList.Add(new ForagableData(randomItem.Id) { ItemRarity = 0.001 });
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
				itemIndex = Globals.RNG.Next(0, AllForagables.Count);
			} while (listToPopulate.Contains(AllForagables[itemIndex]));

			listToPopulate.Add(AllForagables[itemIndex]);
		}

		/// <summary>
		/// Sets the diggable item info on the given data
		/// </summary>
		/// <param name="locationData">The location data</param>
		private static void SetExtraDiggableItemInfo(LocationData locationData)
		{
			ObtainingDifficulties difficulty = GetRandomItemDifficulty();
			double probability;
			switch (difficulty)
			{
				case ObtainingDifficulties.NoRequirements:
					probability = (double)Range.GetRandomValue(30, 60) / 100;
					break;
				case ObtainingDifficulties.SmallTimeRequirements:
					probability = (double)Range.GetRandomValue(30, 40) / 100;
					break;
				case ObtainingDifficulties.MediumTimeRequirements:
					probability = (double)Range.GetRandomValue(20, 30) / 100;
					break;
				case ObtainingDifficulties.LargeTimeRequirements:
					probability = (double)Range.GetRandomValue(10, 20) / 100;
					break;
				case ObtainingDifficulties.UncommonItem:
					probability = (double)Range.GetRandomValue(5, 15) / 100;
					break;
				case ObtainingDifficulties.RareItem:
					probability = (double)Range.GetRandomValue(1, 5) / 100;
					break;
				default:
					Globals.ConsoleError($"Attempting to get a diggable item with invalid difficulty: {difficulty}");
					difficulty = ObtainingDifficulties.NoRequirements;
					probability = (double)Range.GetRandomValue(30, 60) / 100;
					break;
			}

			locationData.ExtraDiggingItem = ItemList.GetRandomItemAtDifficulty(difficulty);
			locationData.ExtraDiggingItemRarity = probability;
		}

		/// <summary>
		/// Gets a random item difficulty
		/// - 1/2 = no req
		/// - 1/4 = small time req
		/// - 1/8 = medium time req
		/// - 1/16 = large time req
		/// - 1/32 = uncommon
		/// - 1/64 = rare
		/// </summary>
		/// <returns></returns>
		private static ObtainingDifficulties GetRandomItemDifficulty()
		{
			if (Globals.RNGGetNextBoolean())
			{
				return ObtainingDifficulties.NoRequirements;
			}

			else if (Globals.RNGGetNextBoolean())
			{
				return ObtainingDifficulties.SmallTimeRequirements;
			}

			else if (Globals.RNGGetNextBoolean())
			{
				return ObtainingDifficulties.MediumTimeRequirements;
			}

			else if (Globals.RNGGetNextBoolean())
			{
				return ObtainingDifficulties.LargeTimeRequirements;
			}

			else if (Globals.RNGGetNextBoolean())
			{
				return ObtainingDifficulties.UncommonItem;
			}

			else
			{
				return ObtainingDifficulties.RareItem;
			}
		}

		/// <summary>
		/// Sets the foragable arrays to contain the vanilla foragables
		/// </summary>
		private static void PopulateDefaultForagables()
		{
			SpringForagables.Clear();
			SummerForagables.Clear();
			FallForagables.Clear();
			WinterForagables.Clear();
			BeachForagables.Clear();
			WoodsForagables.Clear();
			DesertForagables.Clear();

			SpringForagables.AddRange(new List<Item>()
			{
				ItemList.Items[ObjectIndexes.WildHorseradish],
				ItemList.Items[ObjectIndexes.Daffodil],
				ItemList.Items[ObjectIndexes.Leek],
				ItemList.Items[ObjectIndexes.Dandelion]
			});

			SummerForagables.AddRange(new List<Item>()
			{
				ItemList.Items[ObjectIndexes.SpiceBerry],
				ItemList.Items[ObjectIndexes.Grape],
				ItemList.Items[ObjectIndexes.SweetPea]
			});

			FallForagables.AddRange(new List<Item>()
			{
				ItemList.Items[ObjectIndexes.WildPlum],
				ItemList.Items[ObjectIndexes.Hazelnut],
				ItemList.Items[ObjectIndexes.Blackberry],
				ItemList.Items[ObjectIndexes.CommonMushroom],

			});

			WinterForagables.AddRange(new List<Item>()
			{
				ItemList.Items[ObjectIndexes.WinterRoot],
				ItemList.Items[ObjectIndexes.CrystalFruit],
				ItemList.Items[ObjectIndexes.SnowYam],
				ItemList.Items[ObjectIndexes.Crocus],
				ItemList.Items[ObjectIndexes.Holly],
			});

			BeachForagables.AddRange(new List<Item>()
			{
				ItemList.Items[ObjectIndexes.NautilusShell],
				ItemList.Items[ObjectIndexes.Coral],
				ItemList.Items[ObjectIndexes.SeaUrchin],
				ItemList.Items[ObjectIndexes.RainbowShell],
				ItemList.Items[ObjectIndexes.Clam],
				ItemList.Items[ObjectIndexes.Cockle],
				ItemList.Items[ObjectIndexes.Mussel],
				ItemList.Items[ObjectIndexes.Oyster]
			});

			WoodsForagables.AddRange(new List<Item>()
			{
				ItemList.Items[ObjectIndexes.Morel],
				ItemList.Items[ObjectIndexes.CommonMushroom],
				ItemList.Items[ObjectIndexes.RedMushroom],
				ItemList.Items[ObjectIndexes.FiddleheadFern],
				ItemList.Items[ObjectIndexes.Chanterelle],
			});

			DesertForagables.AddRange(new List<Item>()
			{
				ItemList.Items[ObjectIndexes.CactusFruit],
				ItemList.Items[ObjectIndexes.Coconut]
			});
		}

		/// <summary>
		/// Adds tooltips to foragable items to help the player know when
		/// they're available
		/// </summary>
		/// <param name="objectInformationReplacements">
		/// The object information replacements dictionary from EditedObjectInfo
		/// It is important to NOT modify this more than we intend to
		/// </param>
		/// <param name="foragableLocationDataList">The list of foragable data that we constructed</param>
        private static void AddTooltipToForagableItems(
			Dictionary<int, string> objectInformationReplacements,
			List<LocationData> foragableLocationDataList)
		{
			Dictionary<int, List<Seasons>> foragableIdToSeasons = new();

			foreach (LocationData foragableLocationData in foragableLocationDataList)
			{
                FindForagableSeasons(foragableIdToSeasons, foragableLocationData.SpringForagables, Seasons.Spring);
                FindForagableSeasons(foragableIdToSeasons, foragableLocationData.SummerForagables, Seasons.Summer);
                FindForagableSeasons(foragableIdToSeasons, foragableLocationData.FallForagables, Seasons.Fall);
                FindForagableSeasons(foragableIdToSeasons, foragableLocationData.WinterForagables, Seasons.Winter);
            }

			foreach (var foragableData in foragableIdToSeasons)
			{
				int itemId = foragableData.Key;
				var seasonList = foragableData.Value.Distinct();
                if (!seasonList.Any())
                {
                    Globals.ConsoleWarn($"Foragable with no season: ${itemId}");
                }

                // Insert the data into objectInformation if it isn't there already
                // This is expected for Grapes and Cactus fruit, where we append to the description
                if (!objectInformationReplacements.ContainsKey(itemId))
                {
                    objectInformationReplacements[itemId] = ItemList.OriginalItemList[itemId];
                }

                // Now append the season string to the description in objectInformation
				// Also yes, we're using the fish translations here since it's item-agnostic
                string tooltip = seasonList.Count() >= 4
					? Globals.GetTranslation("fish-tooltip-seasons-all")
					: Globals.GetTranslation("fish-tooltip-seasons", new { seasons = string.Join(", ", seasonList) });

                string[] objectInfoData = objectInformationReplacements[itemId].Split("/");
                objectInfoData[(int)ObjectInformationIndexes.Description] += $" {tooltip}";

                objectInformationReplacements[itemId] = string.Join("/", objectInfoData);
            }
        }

		/// <summary>
		/// Fills a dictionary with a list of seasons that a foragable can be collected in
		/// </summary>
		/// <param name="foragableIdToSeasons">The dictionary to fill</param>
		/// <param name="foragableList">Fhe list of foragables to loop through</param>
		/// <param name="season">The season the foragable list is for</param>
		private static void FindForagableSeasons(
			Dictionary<int, List<Seasons>> foragableIdToSeasons, 
			List<ForagableData> foragableList, 
			Seasons season)
		{
			foreach(ForagableData foragableData in foragableList)
			{
				// Skip over non-foragables - they are the 1 in 1k drops
				// Also skip over 1 in 1k drops in case they are foragables!
				// We only want the tooltip for when the player can reasonably get them
                int itemId = foragableData.ItemId;
                if (!ItemList.Items[(ObjectIndexes)itemId].IsForagable ||
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
    }
}
