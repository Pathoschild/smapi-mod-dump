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
using StardewValley.GameData.Locations;
using StardewValley.GameData.Objects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SVLocationData = StardewValley.GameData.Locations.LocationData;

namespace Randomizer
{
    public class FishRandomizer
	{
        private static RNG Rng { get; set; }

        /// <summary>
        /// Randomizes the fish by shuffling which fish ids appear in which location
        /// It also completely randomizes behaviors, dart chance, and names
        /// </summary>
        /// <param name="editedObjectInfo">Used to change the fish behaviors, dart chance, and names</param>
        /// <param name="locationReplacements">Used to change which fish id appears where</param>
        public static void Randomize(
            EditedObjects editedObjectInfo, 
            Dictionary<string, SVLocationData> locationReplacements)
		{
            if (!Globals.Config.Fish.Randomize) 
            {
                ComputeDefaultFishLocationChanges();
				return;
            }

            Rng = RNG.GetFarmRNG(nameof(FishRandomizer));

            List<FishItem> legendaryFish = FishItem.GetLegendaries().Cast<FishItem>().ToList();
			List<FishItem> normalFish = FishItem.Get().Cast<FishItem>().ToList();
			List<FishItem> normalFishCopy = new();

			// A map of the old qualified fish id to the one it will replace
			// Used to replace the location information's fish ids
			Dictionary<string, string> oldToNewFishIdMap = new();
            Dictionary<string, string> newToOldFishIdMap = new();

            foreach (FishItem fish in normalFish)
			{
				FishItem fishInfo = new(fish.ObjectIndex, true); // A constructor that does nothing
				CopyFishInfo(fish, fishInfo);
				normalFishCopy.Add(fishInfo);
            }

			List<string> fishNames = NameAndDescriptionRandomizer.GenerateFishNames(normalFish.Count + legendaryFish.Count);
			foreach (FishItem fish in normalFish)
			{
				FishItem fishToReplace = Rng.GetAndRemoveRandomValueFromList(normalFishCopy);
				int newDartChance = GenerateRandomFishDifficulty();
				FishBehaviorType newBehaviorType = Rng.GetRandomValueFromList(
					Enum.GetValues(typeof(FishBehaviorType)).Cast<FishBehaviorType>().ToList());
				string newName = Rng.GetAndRemoveRandomValueFromList(fishNames);

				CopyFishInfo(fishToReplace, fish);
				fish.DartChance = newDartChance;
				fish.BehaviorType = newBehaviorType;
				fish.OverrideName = newName;
                oldToNewFishIdMap.Add(fishToReplace.QualifiedId, fish.QualifiedId);
                newToOldFishIdMap.Add(fish.QualifiedId, fishToReplace.QualifiedId);

                // These fish are unfortunately hard-coded to spawn here
                if (fish.IsMinesFish)
                {
                    TryAddLocationToFishItem(fish, Locations.UndergroundMine);
                }

				fish.DifficultyToObtain = fish.IsSubmarineOnlyFish
					? ObtainingDifficulties.RareItem
					: ObtainingDifficulties.LargeTimeRequirements;
			}

			foreach (FishItem fish in legendaryFish)
			{
				FishBehaviorType newBehaviorType = Rng.GetRandomValueFromList(
					Enum.GetValues(typeof(FishBehaviorType)).Cast<FishBehaviorType>().ToList());

				string newName = Rng.GetAndRemoveRandomValueFromList(fishNames);
				fish.BehaviorType = newBehaviorType;
				fish.OverrideName = newName;
			}

            ComputeFishLocationChanges(locationReplacements, oldToNewFishIdMap);

            foreach(FishItem fish in normalFish.Concat(legendaryFish))
            {
                editedObjectInfo.FishReplacements.Add(fish.Id.ToString(), fish.ToString());
                editedObjectInfo.ObjectsReplacements.Add(fish.Id.ToString(), GetFishObjectData(fish));
            }

            // Keeping this here for debugging purposes
			// Uncomment to print out all randomized seasons and locations
            //List<int> sortedFishIds = ItemList.Items.Values
            //    .Where(x => x is FishItem)
            //    .Select(x => x.Id).ToList();
            //sortedFishIds.Sort();
            //foreach (int fishId in sortedFishIds)
            //{
            //    List<Seasons> seasons = (ItemList.Items[(ObjectIndexes)fishId] as FishItem).AvailableSeasons;
            //    List<Locations> locs = (ItemList.Items[(ObjectIndexes)fishId] as FishItem).AvailableLocations;
            //    string itemName = ItemList.Items[(ObjectIndexes)fishId].Name;
            //    Globals.ConsoleWarn($"{fishId} {itemName}: {string.Join(" ", seasons.Select(x => x.ToString().ToLower()))}");
            //    Globals.ConsoleWarn($"{fishId} {itemName}: {string.Join(" ", locs.Select(x => x.ToString().ToLower()))}");
            //}

            WriteToSpoilerLog(newToOldFishIdMap);
		}

		/// <summary>
		/// Copies a select set of info from one fish to another
		/// </summary>
		/// <param name="fromFish">The fish to copy from</param>
		/// <param name="toFish">The fish to copy to</param>
		private static void CopyFishInfo(FishItem fromFish, FishItem toFish)
		{
			toFish.Times = new Range(fromFish.Times.MinValue, fromFish.Times.MaxValue);
			toFish.ExcludedTimes = new Range(fromFish.ExcludedTimes.MinValue, fromFish.ExcludedTimes.MaxValue);
			toFish.Weathers = new List<Weather>(fromFish.Weathers);
			toFish.MinWaterDepth = fromFish.MinWaterDepth;
			toFish.DepthMultiplier = fromFish.DepthMultiplier;
			toFish.IsValidTutorialFish = fromFish.IsValidTutorialFish;
		}

		/// <summary>
		/// Gets a random fish difficulty value
		/// </summary>
		/// <returns>
		/// 25% Easy: 15 - 30
		/// 45% Moderate: 31 - 50
		/// 25% Difficult: 51 - 75
		/// 5% WTF: 76 - 95
		/// 0% Legendary: 96 - 110
		/// </returns>
		private static int GenerateRandomFishDifficulty()
		{
			int difficultyRange = Rng.NextIntWithinRange(1, 100);
			if (difficultyRange < 26)
			{
				return Rng.NextIntWithinRange(15, 30);
			}
			else if (difficultyRange < 71)
			{
				return Rng.NextIntWithinRange(31, 50);
			}
			else if (difficultyRange < 96)
			{
				return Rng.NextIntWithinRange(51, 75);
			}
			else
			{
				return Rng.NextIntWithinRange(76, 95);
			}
		}

		/// <summary>
		/// Gets the fish object data with all the relevant modifications
		/// Currently this is the display name and description
		/// </summary>
		/// <param name="fish">The fish</param>
		/// <returns />
		private static ObjectData GetFishObjectData(FishItem fish)
		{
            ObjectData defaultObjectInfo = 
				EditedObjects.DefaultObjectInformation[fish.Id.ToString()];

			defaultObjectInfo.DisplayName = fish.OverrideName;
			defaultObjectInfo.Description = fish.Description;

			return defaultObjectInfo;
		}

        /// <summary>
        /// Used to populate the location changes if fish rando is off
        /// This will ensure that fish actually exist in our item list
        /// </summary>
        private static void ComputeDefaultFishLocationChanges()
        {
			foreach (var locData in DataLoader.Locations(Game1.content))
			{
                var locationData = locData.Value;
				string locationName = locData.Key;
				if (!Enum.TryParse(locationName, out Locations location))
				{
					// This is okay, we don't have every location mapped
					continue;
				}

				foreach (var spawnFishData in locationData.Fish)
				{
					if (spawnFishData.ItemId == null)
					{
						continue;
					}

					Item item = ItemList.GetItemFromStringId(spawnFishData.ItemId);
					if (item is not FishItem fishItem)
					{
						continue;
					}

					TryAddLocationToFishItem(fishItem, location);
					TryAddSeasonsToFishItem(fishItem, location, spawnFishData);
				}
			}
		}

		/// <summary>
		/// Go through the location data and modify which fish goes where based on the
		/// map we created earlier
		/// 
		/// This also populates the location/season data of each fish
		/// </summary>
		/// <param name="locationDataReplacements">The location data to pase and modify</param>
		/// <param name="oldToNewFishIdMap">A map of the old fish ids to the new ones</param>
		private static void ComputeFishLocationChanges(
            Dictionary<string, SVLocationData> locationDataReplacements,
            Dictionary<string, string> oldToNewFishIdMap)
        {
            // Go through each location and add to the location dictionary
            foreach (var locData in DataLoader.Locations(Game1.content))
            {
                string locationName = locData.Key;
                if (!Enum.TryParse(locationName, out Locations location))
                {
                    // This is okay, we don't have every location mapped
                    continue;
                }

                // Use the location data in the replacements if it's there
                // Otherwise, use the one we're looping through, but add it to the replacements
                // USE THIS VALUE AND NOT locData.Value NOW!
                SVLocationData locationData = LocationData.TrySetLocationData(
                    locationName,
                    locData.Value,
                    locationDataReplacements);

                foreach (var spawnFishData in locationData.Fish)
                {
                    if (spawnFishData.ItemId == null)
                    {
                        continue;
                    }

                    Item item = ItemList.GetItemFromStringId(spawnFishData.ItemId);
                    if (item is not FishItem fishItem)
                    {
                        continue;
                    }

                    // Set the fish item to the one we should modify, if applicable
                    // We also set the ids on the location data to whatever fish will now be there
                    if (oldToNewFishIdMap.TryGetValue(fishItem.QualifiedId, out string newFishId))
                    {
                        fishItem = ItemList.GetItemFromStringId(newFishId) as FishItem;
                        spawnFishData.Id = newFishId;
                        spawnFishData.ItemId = newFishId;

                        // This will get it to behave like the old randomizer
                        // We may want to revisit this and add approriate things to the
                        // tooltip if we want to actually re-instate fish areas
                        spawnFishData.FishAreaId = null;
                    }

                    TryAddLocationToFishItem(fishItem, location);
                    TryAddSeasonsToFishItem(fishItem, location, spawnFishData);
                }
            }
        }

        /// <summary>
        /// Adds the loctaion to the fish item if it doesn't already have it
        /// </summary>
        /// <param name="fishItem">The fish item</param>
        /// <param name="location">The location to add</param>
        private static void TryAddLocationToFishItem(FishItem fishItem, Locations location)
        {
            // For some reason, Legend is marked as backwoods, and
            // the rando won't work properly if it's NOT assigned to Mountain
            // In all other cases, backwoods is equivalent, so this logic should be fine
            Locations locToUse = location == Locations.Backwoods
                ? Locations.Mountain
                : location;

            if (!fishItem.AvailableLocations.Contains(locToUse))
            {
                fishItem.AvailableLocations.Add(locToUse);
            }
        }

        /// <summary>
        /// Adds the seasons to the fish item if they don't already exist
        /// 
        /// If the condition is a LOCATION_SEASON condition, adds all the ones present there
        /// Else, if Season exists, add that one
        /// Else, this fish belongs to add seasons, so add all of them
        /// </summary>
        /// <param name="fishItem">The fish item</param>
        /// <param name="location">The location - Submarine should only add winter</param>
        /// <param name="spawnFishData">the spawn data, containing the season and condition</param>
        private static void TryAddSeasonsToFishItem(
            FishItem fishItem,
            Locations location,
            SpawnFishData spawnFishData)
        {
            // Special case - the submarine is always during winter only!
            if (location == Locations.Submarine)
            {
                TryAddSeasonToFishItem(fishItem, Seasons.Winter);
                return;
            }

            // Get the seasons to add from the condition, if relevant
            string condition = spawnFishData.Condition;
            if (!string.IsNullOrWhiteSpace(condition) && 
                condition.StartsWith("LOCATION_SEASON Here"))
            {
                TryAddSeasonToFishItem(fishItem, Seasons.Spring, condition);
                TryAddSeasonToFishItem(fishItem, Seasons.Summer, condition);
                TryAddSeasonToFishItem(fishItem, Seasons.Fall, condition);
                TryAddSeasonToFishItem(fishItem, Seasons.Winter, condition);
            }

            // Add the season if it exists
            else if (spawnFishData.Season != null)
            {
                TryAddSeasonToFishItem(fishItem, (Seasons)spawnFishData.Season);
            }

            // Otherwise - it is null, so this is for all the seasons
            else
            {
                TryAddSeasonToFishItem(fishItem, Seasons.Spring);
                TryAddSeasonToFishItem(fishItem, Seasons.Summer);
                TryAddSeasonToFishItem(fishItem, Seasons.Fall);
                TryAddSeasonToFishItem(fishItem, Seasons.Winter);
            }
        }

        /// <summary>
        /// Try to add the season to the fish item
        /// Will check that the season is within the condition, if applicable
        /// If nothing is passed in, then it will pass the condition
        /// </summary>
        /// <param name="fishItem">The fish item</param>
        /// <param name="season">The season</param>
        /// <param name="condition">The condition</param>
        private static void TryAddSeasonToFishItem(
            FishItem fishItem,
            Seasons season,
            string condition = null)
        {
            if (!fishItem.AvailableSeasons.Contains(season) &&
                (condition == null || condition.Contains(season.ToString().ToLower())))
            {
                fishItem.AvailableSeasons.Add(season);
            }
        }

        /// <summary>
        /// Writes the relevant changes to the spoiler log
        /// </summary>
        /// <param name="oldToNewFishIdMap">Which fish ids were remapped where</param>
        private static void WriteToSpoilerLog(Dictionary<string, string> newToOldFishIdMap)
		{
			List<FishItem> allRandomizedFish = FishItem.GetListAsFishItem(true);

			Globals.SpoilerWrite("==== FISH ====");
			foreach (FishItem fish in allRandomizedFish)
			{
                Globals.SpoilerWrite($"{fish.Id}: {fish.Name}");

                if (newToOldFishIdMap.TryGetValue(fish.QualifiedId, out string newFishId))
                {
                    string newName = ItemRegistry.GetData(fish.QualifiedId).InternalName;
                    string oldName = ItemRegistry.GetData(newFishId).InternalName;
                    Globals.SpoilerWrite($"Original fish: {oldName}; Current fish: {newName}");
                }

				Globals.SpoilerWrite($"Difficulty: {fish.DartChance} - Level Req: {fish.MinFishingLevel} - Water depth: {fish.MinWaterDepth}");
				Globals.SpoilerWrite(fish.Description);
				Globals.SpoilerWrite("---");
			}
			Globals.SpoilerWrite("");
		}
	}
}
