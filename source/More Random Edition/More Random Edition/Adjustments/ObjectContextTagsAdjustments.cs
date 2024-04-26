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

namespace Randomizer
{
    /// <summary>
    /// This will adjust context tags to match the randomizer's values
    /// Mainly meant for quests that use them
    /// 
    /// Currently only adjusts fish, as it's a problem for the biome fish quest
    /// </summary>
    public class ObjectContextTagsAdjustments
    {
        /// <summary>
        /// A list of all the season tags
        /// </summary>
        private readonly static List<string> SeasonTags = new()
        {
            "season_spring",
            "season_summer",
            "season_fall",
            "season_winter",
            "season_all"
        };

        /// <summary>
        /// A list of the fish habitat tags we care about
        /// Note that fish_night_market and fish_legendary are there but
        /// we don't need to adjust them, since we don't change their locations
        /// </summary>
        private readonly static List<string> FishHabitatTags = new()
        {
            "fish_ocean", // ocean
            "fish_river", // town
            "fish_lake", // mountain
            "fish_pond", // forest
            "fish_secret_pond", // secret pond
            "fish_swamp", // swamp
            "fish_mines", // mines
            "fish_desert", // desert
            "fish_night_market", // submarine
            "fish_sewers" // sewers
        };

        /// <summary>
        /// Adjusts the context tags of all relevant objects
        /// Currently, this only adjusts fish context tags, 
        /// as they're needed for the Biome Balance special order
        /// </summary>
        /// <returns>The object data replacements, containing the ContextTag info</returns>
        public static void AdjustContextTags(Dictionary<string, ObjectData> objectDataReplacements)
        {
            if (!Globals.Config.Fish.Randomize)
            {
                return;
            }

            FishItem.GetListAsFishItem(includeLegendaries: true).ForEach(fishItem =>
            {
                ObjectData objectDataToEdit = GetAndSetObjectDataToEdit(
                    objectDataReplacements,
                    fishItem.Id.ToString());

                AdjustFishItemTags(fishItem, objectDataToEdit);
            });
        }

        /// <summary>
        /// Gets the object data to edit, and sets it to our object data replacements
        /// if it's not in there already
        /// </summary>
        /// <param name="objectDataReplacements">The replacement dictionary - grabs from here if present</param>
        /// <param name="id">The fish id</param>
        /// <returns>The data to edit, either from the dictionar if found, or from the default info</returns>
        private static ObjectData GetAndSetObjectDataToEdit(
            Dictionary<string, ObjectData> objectDataReplacements,
            string id)
        {
            if (objectDataReplacements.ContainsKey(id))
            {
                return objectDataReplacements[id];
            }

            ObjectData dataToEdit = EditedObjects.DefaultObjectInformation[id];
            objectDataReplacements[id] = dataToEdit;
            return dataToEdit;
        }

        /// <summary>
        /// Adjusts the given fish's tags
        /// - First, removes the tags that we're modifying
        /// - Then adds the tags back on (season and habitat)
        /// </summary>
        /// <param name="fishItem">The fish item</param>
        /// <param name="objectData">The object to adjust</param>
        private static void AdjustFishItemTags(FishItem fishItem, ObjectData objectData)
        {
            var tags = objectData.ContextTags;
            List<string> tagsWithoutFishInfo = tags
                .Except(SeasonTags.Concat(FishHabitatTags))
                .ToList();

            List<string> seasonTags = GetSeasonTags(fishItem);
            List<string> habitatTags = GetHabitatTags(fishItem);

            objectData.ContextTags = tagsWithoutFishInfo
                .Concat(seasonTags)
                .Concat(habitatTags)
                .ToList();
        }

        /// <summary>
        /// Gets a list of all the season tags that should be added to a fish
        /// </summary>
        /// <param name="fishItem">The fish</param>
        /// <returns>The list of season tags</returns>
        private static List<string> GetSeasonTags(FishItem fishItem)
        {
            if (fishItem.AvailableSeasons.Count == 4)
            {
                return new List<string>() { "season_all" };
            }

            var seasonStrings = fishItem.AvailableSeasons
                .Select(season => $"season_{season.ToString().ToLower()}");
            return new List<string>(seasonStrings);
        }

        /// <summary>
        /// Gets a list of all the habitat tags that should be added to a fish
        /// </summary>
        /// <param name="fishItem">The fish</param>
        /// <returns>The list of habitat tags</returns>
        private static List<string> GetHabitatTags(FishItem fishItem)
        {
            List<string> habitatTags = new();
            fishItem.AvailableLocations.ForEach(location =>
            {
                switch(location)
                {
                    case Locations.Beach:
                        habitatTags.Add("fish_ocean");
                        break;
                    case Locations.Town:
                        habitatTags.Add("fish_river");
                        break;
                    case Locations.Mountain:
                        habitatTags.Add("fish_lake");
                        break;
                    case Locations.Forest:
                        habitatTags.Add("fish_pond");
                        break;
                    case Locations.Woods:
                        habitatTags.Add("fish_secret_pond");
                        break;
                    case Locations.WitchSwamp:
                        habitatTags.Add("fish_swamp");
                        break;
                    case Locations.UndergroundMine:
                        habitatTags.Add("fish_mines");
                        break;
                    case Locations.Desert:
                        habitatTags.Add("fish_desert");
                        break;
                    case Locations.Submarine:
                        habitatTags.Add("fish_night_market");
                        break;
                    case Locations.Sewer:
                        habitatTags.Add("fish_sewers");
                        break;
                }
            });
            return habitatTags;
        }
    }
}
