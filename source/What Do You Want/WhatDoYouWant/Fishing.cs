/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/emurphy42/WhatDoYouWant
**
*************************************************/

using StardewValley;
using StardewValley.GameData.Objects;
using StardewValley.Extensions;
using StardewModdingAPI;

namespace WhatDoYouWant
{
    internal class FishData
    {
        public string? Seasons { get; set; }
        public int SeasonsSortOrder { get; set; }
        public string? FishName { get; set; }
        public string? TextureName { get; set; } // Collections Tab sort option
        public int SpriteIndex { get; set; } // Collections Tab sort option
    }

    internal class Fishing
    {
        public const string SortOrder_SeasonsSpringFirst = "SeasonsSpringFirst";
        public const string SortOrder_SeasonsCurrentFirst = "SeasonsCurrentFirst";
        public const string SortOrder_FishName = "FishName";
        public const string SortOrder_CollectionsTab = "CollectionsTab";

        // Hardcoded data for fish that GetSeasonsByLocation() would get wrong
        private static readonly Dictionary<string, List<Season>> FishSeasons = new()
        {
            // also winter, but rain in winter requires rain totem
            { "(O)140", new List<Season> { Season.Fall } }, // Walleye
            { "(O)143", new List<Season> { Season.Spring, Season.Fall } }, // Catfish
            { "(O)150", new List<Season> { Season.Summer, Season.Fall } }, // Red Snapper
            // probably confused by SquidFest
            { "(O)151", new List<Season> { Season.Winter } }, // Squid
            // any season
            { "(O)682", ModEntry.seasons }, // Mutant Carp
            // submarine only reachable in winter
            { "(O)798", new List<Season> { Season.Winter } }, // Midnight Squid
            { "(O)799", new List<Season> { Season.Winter } }, // Spook Fish
            { "(O)800", new List<Season> { Season.Winter } } // Blobfish
        };

        // Get list of season(s) in which a fish can be caught
        //   * May vary by location, weather, time of day, etc.
        //   * For simplicity, just get season(s) common to all locations where it can ever be caught
        //   * e.g. Tuna can be caught in any season on Ginger Island, but only in summer/winter at beach
        private static List<Season> GetSeasons(string fishId)
        {
            // Check hardcoded data
            if (FishSeasons.ContainsKey(fishId))
            {
                return FishSeasons[fishId];
            }

            // start with all seasons
            var seasonsCommonToAllLocations = new List<Season>();
            foreach (var season in ModEntry.seasons)
            {
                seasonsCommonToAllLocations.Add(season);
            }

            // mapping for parsing conditions            
            var seasonStrings = new Dictionary<string, Season>
            {
                { "spring", Season.Spring },
                { "summer", Season.Summer },
                { "fall", Season.Fall },
                { "winter", Season.Winter }
            };

            foreach (var location in Game1.locations)
            {
                var locationData = location.GetData();
                if (locationData == null)
                {
                    continue;
                }
                var locationFishList = locationData.Fish;
                foreach (var locationFish in locationFishList)
                {
                    if (locationFish.ItemId != fishId)
                    {
                        continue;
                    }

                    // season(s) during which it can be caught at this location
                    var locationSeasons = new List<Season>();
                    
                    if (locationFish.Season.HasValue)
                    {
                        // it can only be caught in one season in this location
                        locationSeasons.Add((Season)locationFish.Season);
                    }
                    else if (locationFish.Condition != null)
                    {
                        // parse relevant conditions
                        var conditionData = GameStateQuery.Parse(locationFish.Condition);
                        var seasonalConditions = conditionData.Where(condition => GameStateQuery.SeasonQueryKeys.Contains(condition.Query[0]));
                        foreach (var condition in seasonalConditions)
                        {
                            foreach (var season in seasonStrings)
                            {
                                if (!condition.Negated && condition.Query.Any(word => word.Equals(season.Key, StringComparison.OrdinalIgnoreCase)))
                                {
                                    locationSeasons.Add(season.Value);
                                }
                            }
                        }
                    }
                    else
                    {
                        // it can be caught in any season in this location
                        locationSeasons = ModEntry.seasons;
                    }

                    foreach (var season in ModEntry.seasons)
                    {
                        if (seasonsCommonToAllLocations.Contains(season) && !locationSeasons.Contains(season))
                        {
                            seasonsCommonToAllLocations.Remove(season);
                        }
                    }
                }
            }

            return seasonsCommonToAllLocations;
        }

        public static void ShowFishingList(ModEntry modInstance, Farmer who)
        {
            var sortBySeasonsSpringFirst = (modInstance.Config.FishingSortOrder == SortOrder_SeasonsSpringFirst);
            var sortBySeasonsCurrentFirst = (modInstance.Config.FishingSortOrder == SortOrder_SeasonsCurrentFirst);
            var sortByFishName = (modInstance.Config.FishingSortOrder == SortOrder_FishName);
            var sortByCollectionsTab = (modInstance.Config.FishingSortOrder == SortOrder_CollectionsTab);

            var seasonsSortOrderList = ModEntry.GetSeasons(currentFirst: sortBySeasonsCurrentFirst);

            // adapted from base game logic to calculate fishing %
            var fishList = new List<FishData>();
            foreach (var parsedItemData in ItemRegistry.GetObjectTypeDefinition().GetAllData())
            {
                // Is it a fish?
                if (parsedItemData.ObjectType != "Fish")
                {
                    continue;
                }

                // Is it part of Master Angler?
                if (parsedItemData.RawData is ObjectData rawData && rawData.ExcludeFromFishingCollection)
                {
                    continue;
                }

                // Have they already caught it?
                if (who.fishCaught.ContainsKey(parsedItemData.QualifiedItemId))
                {
                    continue;
                }

                // Add it to the list
                var dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(parsedItemData.QualifiedItemId);

                var seasons = GetSeasons(parsedItemData.QualifiedItemId);

                fishList.Add(new FishData()
                {
                    Seasons = modInstance.GetSeasonsDescription(seasons, seasonsSortOrderList: seasonsSortOrderList),
                    SeasonsSortOrder = ModEntry.GetSeasonsSortOrder(seasons, seasonsSortOrderList: seasonsSortOrderList),
                    FishName = modInstance.Helper.Translation.Get("Fishing_Fish", new { fish = dataOrErrorItem.DisplayName }),
                    TextureName = dataOrErrorItem.TextureName,
                    SpriteIndex = dataOrErrorItem.SpriteIndex
                });
            }

            if (fishList.Count == 0)
            {
                var completeDescription = modInstance.Helper.Translation.Get("Fishing_Complete", new { title = ModEntry.GetTitle_Fishing() });
                Game1.drawDialogueNoTyping(completeDescription);
                return;
            }

            var linesToDisplay = new List<string>();

            foreach (var fish in fishList
                .OrderBy(entry => sortBySeasonsSpringFirst || sortBySeasonsCurrentFirst ? entry.SeasonsSortOrder : 0)
                .ThenBy(entry => sortByCollectionsTab ? entry.TextureName : "")
                .ThenBy(entry => sortByCollectionsTab ? entry.SpriteIndex : 0)
                .ThenBy(entry => entry.FishName)
            )
            {
                var seasonName = string.IsNullOrWhiteSpace(fish.Seasons) ? "???" : fish.Seasons;
                var fishName = (sortBySeasonsSpringFirst || sortBySeasonsCurrentFirst)
                    ? $"{seasonName} - {fish.FishName}"
                    : $"{fish.FishName} - {seasonName}";
                linesToDisplay.Add($"* {fishName}{ModEntry.LineBreak}");
            }

            modInstance.ShowLines(linesToDisplay, title: ModEntry.GetTitle_Fishing());
        }
    }
}
