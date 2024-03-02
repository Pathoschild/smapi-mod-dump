/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using StardewValley.GameData;
using System.Collections.Generic;
using System.Linq;

namespace Randomizer
{
    /// <summary>
    /// With the fish randomizer, certain special orders will become very difficult, as the
    /// accepted fish will not be as the game describes
    /// 
    /// This class aims to address these issues by adjusting the data based on the randomizer changes
    /// </summary>
    public class SpecialOrderAdjustments
    {
        private const string SeasonalFishOrderKey = "Demetrius";

        public static Dictionary<string, SpecialOrderData> GetSpecialOrderAdjustments()
        {
            Dictionary<string, SpecialOrderData> adjustments = new();
            if (!Globals.Config.Fish.Randomize)
            {
                return adjustments;
            }

            Dictionary<string, SpecialOrderData> specialOrderData = Globals.ModRef.Helper.GameContent
                .Load<Dictionary<string, SpecialOrderData>>("Data/SpecialOrders");

            FixSeasonalFishOrder(specialOrderData, adjustments);

            return adjustments;
        }

        /// <summary>
        /// Fixes the seasonal fish order so it's actually possible to complete in time
        /// </summary>
        /// <param name="specialOrderData">The data to fix</param>
        /// <param name="adjustments">The current list of SpecialOrder adjustments</param>
        private static void FixSeasonalFishOrder(
            Dictionary<string, SpecialOrderData> specialOrderData,
            Dictionary<string, SpecialOrderData> adjustments)
        {
            SpecialOrderData seasonalFishOrder = specialOrderData[SeasonalFishOrderKey];
            ReplaceSeasonalFishOrder(seasonalFishOrder, Seasons.Spring);
            ReplaceSeasonalFishOrder(seasonalFishOrder, Seasons.Summer);
            ReplaceSeasonalFishOrder(seasonalFishOrder, Seasons.Fall);
            ReplaceSeasonalFishOrder(seasonalFishOrder, Seasons.Winter);

            adjustments[SeasonalFishOrderKey] = seasonalFishOrder;
        }

        /// <summary>
        /// Replaces the value in the seasonal fish quest with ones that match the randomizer
        /// </summary>
        /// <param name="specialOrderData">The data to modify</param>
        /// <param name="season">The season to get values for</param>
        private static void ReplaceSeasonalFishOrder(
            SpecialOrderData specialOrderData, Seasons season)
        {
            // This quest always only has one RandomizedElement list, so just get it
            var fishElements = specialOrderData.RandomizedElements[0].Values
                .Where(el => el.RequiredTags == $"season_{season.ToString().ToLower()}")
                .First();

            // Get fish locations that would actually be "local"
            var localLocations = new List<Locations>()
            {
                Locations.Town,
                Locations.Mountain,
                Locations.Forest,
                Locations.Beach
            };

            // Include all fish from the above areas that are available this season
            // Also exlcude fish that only show up with it's raining, since that's too hard
            var allPossibleFish = FishItem.GetListAsFishItem().Where(item =>
                item.AvailableSeasons.Contains(season) &&
                item.AvailableLocations.Any(loc => localLocations.Contains(loc) &&
                (item.Weathers.Count != 1 || !item.IsRainFish))
            );

            string fishString = string.Join(", ", allPossibleFish.Select(fish => fish.EnglishName));
            fishElements.Value = $"PICK_ITEM {fishString}";
        }
    }
}
