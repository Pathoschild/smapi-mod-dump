/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/calebstein1/StardewVariableSeasons
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace StardewVariableSeasons
{
    public static class DayEndingActions
    {
        private static void SaveCropSurvivalCounter(IModHelper helper)
        {
            var cropSurvivalCounter = new ModData
            {
                CropSurvivalCounter = ModEntry.CropSurvivalCounter
                
            };
            
            helper.Data.WriteSaveData("crop-survival-counter", cropSurvivalCounter);
        }
        public static void OnDayEnding(IMonitor monitor, IModHelper helper, object sender, DayEndingEventArgs e)
        {
            ModEntry.ChangeDate = helper.Data.ReadSaveData<ModData>("next-season-change").NextSeasonChange;
            
            var season = new Seasons();
            var changeDate = ModEntry.ChangeDate;
            
            monitor.Log($"Next season is {season.Next(Game1.currentSeason)}", LogLevel.Debug);
            monitor.Log($"Previous season was {season.Prev(Game1.currentSeason)}", LogLevel.Debug);

            monitor.Log($"Current day is {Game1.Date.DayOfMonth.ToString()}", LogLevel.Debug);
            switch (Game1.dayOfMonth)
            {
                case 14:
                {
                    monitor.Log("Drawing new date...", LogLevel.Debug);
                    var nextSeasonChange = new ModData
                    {
                        NextSeasonChange = season.GenNextChangeDate()
                    };
                
                    helper.Data.WriteSaveData("next-season-change", nextSeasonChange);
                    break;
                }
                case 28:
                    Game1.dayOfMonth = 0;
                    if (season.Next(ModEntry.SeasonByDay) == "spring")
                    {
                        Game1.year++;
                        if (Game1.year == 2)
                            Game1.addKentIfNecessary();
                    }
                    
                    ModEntry.SeasonByDay = season.Next(ModEntry.SeasonByDay);
                    var seasonByDay = new ModData
                    {
                        SeasonByDay = ModEntry.SeasonByDay
                    };
                        
                    helper.Data.WriteSaveData("season-by-day", seasonByDay);
                    break;
            }

            if (ModEntry.CropSurvivalCounter < 5)
            {
                ModEntry.CropSurvivalCounter++;
                SaveCropSurvivalCounter(helper);
            }

            monitor.Log($"Current actual season is {Game1.currentSeason}", LogLevel.Debug);
            monitor.Log($"Current season by day is {ModEntry.SeasonByDay}", LogLevel.Debug);
            monitor.Log($"Next season change on {changeDate.ToString()}", LogLevel.Debug);
            monitor.Log($"Crop survival counter is {ModEntry.CropSurvivalCounter.ToString()}", LogLevel.Debug);

            if (Game1.Date.DayOfMonth != changeDate) return;
            monitor.Log("Change to next season", LogLevel.Debug);
            ModEntry.CropSurvivalCounter = 0;
            SaveCropSurvivalCounter(helper);
            
            Game1.currentSeason = season.Next(Game1.currentSeason);
            Game1.setGraphicsForSeason();
            Utility.ForAllLocations(delegate(GameLocation l)
            {
                l.seasonUpdate(Game1.GetSeasonForLocation(l));
            });
        }
    }
}