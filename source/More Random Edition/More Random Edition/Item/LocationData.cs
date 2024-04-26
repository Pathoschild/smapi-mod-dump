/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using Force.DeepCloner;
using StardewValley;
using StardewValley.GameData.Locations;
using System.Collections.Generic;
using static StardewValley.GameData.QuantityModifier;
using SVLocationData = StardewValley.GameData.Locations.LocationData;

namespace Randomizer
{
    /// <summary>
    /// Contains information about foragable items and helper functions to help
    /// with fish and artifact spot locations
    /// </summary>
    public class LocationData
	{
		public Locations Location { get; set; }
        public string LocationName { get { return Location.ToString(); } }
        public SVLocationData StardewLocationData { get; private set; }
		public List<ForagableData> SpringForagables { get; } = new List<ForagableData>();
		public List<ForagableData> SummerForagables { get; } = new List<ForagableData>();
		public List<ForagableData> FallForagables { get; } = new List<ForagableData>();
		public List<ForagableData> WinterForagables { get; } = new List<ForagableData>();

        public static List<Locations> ForagableLocations { 
            get => new()
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
        }
        public static List<Locations> ArtifactSpotLocations
        {
            get => new()
            {
                Locations.Desert,
                Locations.BusStop,
                Locations.Forest,
                Locations.Town,
                Locations.Mountain,
                Locations.Backwoods,
                Locations.Railroad,
                Locations.Beach,
                Locations.Woods,
                Locations.UndergroundMine
            };
        }

		public LocationData(Locations location) 
		{ 
			Location = location;
			StardewLocationData = DataLoader.Locations(Game1.content)[LocationName];
		}

        /// <summary>
        /// Gets the location data populated with the foragable info in this class
        /// </summary>
        /// <returns>The modified data</returns>
		public SVLocationData GetLocationDataWithModifiedForagableData()
		{
            var modifiedData = StardewLocationData.DeepClone();
            modifiedData.Forage.Clear();

            AddForagableDataForSeason(modifiedData, Season.Spring, SpringForagables);
            AddForagableDataForSeason(modifiedData, Season.Summer, SummerForagables);
            AddForagableDataForSeason(modifiedData, Season.Fall, FallForagables);
            AddForagableDataForSeason(modifiedData, Season.Winter, WinterForagables);

            return modifiedData;
        }

        /// <summary>
        /// Adds the foragable data for the given season
        /// </summary>
        /// <param name="locationData">The data to add the foragables to</param>
        /// <param name="season">The season</param>
        /// <param name="data">The list of foragables to add</param>
        private static void AddForagableDataForSeason(
            SVLocationData locationData,
            Season season, 
            List<ForagableData> data)
        {
            data.ForEach(foragableData =>
                locationData.Forage.Add(GetSpawnForageData(foragableData, season)));
        }

        /// <summary>
        /// Gets the spawn forage data to set in the location
        /// this is mostly default values, but with the season and our own
        /// foragable data values added on
        /// </summary>
        /// <param name="foragableData">Our foragable data to include</param>
        /// <param name="season">The season of the foragable</param>
        /// <returns>The created spawn forage data</returns>
		private static SpawnForageData GetSpawnForageData(ForagableData foragableData, Season season)
		{
            return new SpawnForageData
            {
                Chance = foragableData.ItemRarity,
                Season = season,
                Condition = null,
                Id = foragableData.QualifiedItemId,
                ItemId = foragableData.QualifiedItemId,
                RandomItemId = null,
                MaxItems = null,
                MinStack = -1,
                MaxStack = -1,
                Quality = -1,
                ObjectInternalName = null,
                ObjectDisplayName = null,
                ToolUpgradeLevel = -1,
                IsRecipe = false,
                StackModifiers = null,
                StackModifierMode = QuantityModifierMode.Stack,
                QualityModifiers = null,
                QualityModifierMode = QuantityModifierMode.Stack,
                PerItemCondition = null
            };
		}

        /// <summary>
        /// Try to set the key-value pair into the given dictionary of location data
        /// - If the entry does not exist, adds it to the dictionary
        /// - In either case, returns back the location data that's in the dictionary
        /// </summary>
        /// <param name="locationName">The location name - the key to the replacement dictionary</param>
        /// <param name="locationDataToSet">The location data to set into the dictionary</param>
        /// <param name="locationDataReplacements">The replacements dictionary</param>
        /// <returns>The location data that's present, or that became present in the dictionary</returns>
        public static SVLocationData TrySetLocationData(
            string locationName,
            SVLocationData locationDataToSet,
            Dictionary<string, SVLocationData> locationDataReplacements)
        {
            // Grab the location from the dictionary if it exists, otherwise use the given value
            bool locationAlreadyExists = locationDataReplacements.ContainsKey(locationName);
            SVLocationData locationData = locationAlreadyExists
                ? locationDataReplacements[locationName]
                : locationDataToSet;

            // Set the value if it's not already set
            if (!locationAlreadyExists)
            {
                locationDataReplacements[locationName] = locationData;
            }

            return locationData;
        }

        /// <summary>
        /// Try to set the key-value pair into the given dictionary of location data
        /// - If the entry does not exist, adds it to the dictionary
        /// - In either case, returns back the location data that's in the dictionary
        /// </summary>
        /// <param name="location">The location to set the data for</param>
        /// <param name="locationDataToSet">The location data to set into the dictionary</param>
        /// <param name="locationDataReplacements">The replacements dictionary</param>
        /// <returns>The location data that's present, or that became present in the dictionary</returns>
        public static SVLocationData TrySetLocationData(
            Locations location,
            SVLocationData locationDataToSet,
            Dictionary<string, SVLocationData> locationDataReplacements)
        {
            return TrySetLocationData(location.ToString(), locationDataToSet, locationDataReplacements);
        }
    }
}