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
using StardewValley.GameData.Crops;

namespace WhatDoYouWant
{
    internal class PolycultureCropData
    {
        public string? Seasons { get; set; }
        public int SeasonsSortOrder { get; set; }
        public string? CropName { get; set; }
        public int NumberNeeded { get; set; }
    }

    internal class Polyculture
    {
        private const int NumberShippedForPolyculture = 15;

        public const string SortOrder_SeasonsSpringFirst = "SeasonsSpringFirst";
        public const string SortOrder_SeasonsCurrentFirst = "SeasonsCurrentFirst";
        public const string SortOrder_CropName = "CropName";
        public const string SortOrder_NumberNeeded = "NumberNeeded";

        public static void ShowPolycultureList(ModEntry modInstance)
        {
            var sortBySeasonsSpringFirst = (modInstance.Config.PolycultureSortOrder == SortOrder_SeasonsSpringFirst);
            var sortBySeasonsCurrentFirst = (modInstance.Config.PolycultureSortOrder == SortOrder_SeasonsCurrentFirst);
            var sortByCropName = (modInstance.Config.PolycultureSortOrder == SortOrder_CropName);
            var sortByNumberNeeded = (modInstance.Config.PolycultureSortOrder == SortOrder_NumberNeeded);

            var seasonsSortOrderList = ModEntry.GetSeasons(currentFirst: sortBySeasonsCurrentFirst);

            // adapted from base game logic to award Polyculure achievement
            var PolycultureCropList = new List<PolycultureCropData>();
            foreach (CropData cropData in (IEnumerable<CropData>)Game1.cropData.Values)
            {
                // Is it part of Polyculture?
                if (!cropData.CountForPolyculture)
                {
                    continue;
                }

                // Has enough of it already been shipped?
                Game1.player.basicShipped.TryGetValue(cropData.HarvestItemId, out int numberShipped);
                if (numberShipped >= NumberShippedForPolyculture)
                {
                    continue;
                }

                // Add it to the list

                var dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(cropData.HarvestItemId);

                PolycultureCropList.Add(new PolycultureCropData()
                {
                    Seasons = modInstance.GetSeasonsDescription(cropData.Seasons, seasonsSortOrderList: seasonsSortOrderList),
                    SeasonsSortOrder = ModEntry.GetSeasonsSortOrder(cropData.Seasons, seasonsSortOrderList: seasonsSortOrderList),
                    CropName = dataOrErrorItem.DisplayName,
                    NumberNeeded = NumberShippedForPolyculture - numberShipped
                });
            }

            if (PolycultureCropList.Count == 0)
            {
                var completeDescription = modInstance.Helper.Translation.Get("Polyculture_Complete", new { title = ModEntry.GetTitle_Polyculture() });
                Game1.drawDialogueNoTyping(completeDescription);
                return;
            }

            var linesToDisplay = new List<string>();
            foreach (var polycultureCrop in PolycultureCropList
                .OrderBy(entry => sortBySeasonsSpringFirst || sortBySeasonsCurrentFirst ? entry.SeasonsSortOrder : 0)
                .ThenBy(entry => sortByNumberNeeded ? entry.NumberNeeded : 0)
                .ThenBy(entry => entry.CropName)
            )
            {
                var cropDescription = (sortBySeasonsSpringFirst || sortBySeasonsCurrentFirst)
                    ? $"{polycultureCrop.Seasons} - {polycultureCrop.CropName}"
                    : $"{polycultureCrop.CropName} - {polycultureCrop.Seasons}";
                var numberNeeded = modInstance.Helper.Translation.Get("Polyculture_NumberNeeded", new { number = polycultureCrop.NumberNeeded });
                linesToDisplay.Add($"* {cropDescription} - {numberNeeded}{ModEntry.LineBreak}");
            }

            modInstance.ShowLines(linesToDisplay, title: ModEntry.GetTitle_Polyculture());
        }
    }
}
