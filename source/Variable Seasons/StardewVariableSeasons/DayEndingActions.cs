/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/calebstein1/StardewVariableSeasons
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
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
            if (!Game1.IsMasterGame) return;
            
            ModEntry.ChangeDate = helper.Data.ReadSaveData<ModData>("next-season-change").NextSeasonChange;
            
            var changeDate = ModEntry.ChangeDate;
            
            monitor.Log($"Next season is {SeasonUtils.GetNextSeason(Game1.season).ToString()}");

            monitor.Log($"Current day is {Game1.Date.DayOfMonth.ToString()}");
            switch (Game1.dayOfMonth)
            {
                case 14:
                {
                    monitor.Log("Drawing new date...");
                    var nextSeasonChange = new ModData
                    {
                        NextSeasonChange = SeasonUtils.GenNextChangeDate()
                    };
                
                    helper.Data.WriteSaveData("next-season-change", nextSeasonChange);
                    break;
                }
                case 28:
                    Game1.dayOfMonth = 0;
                    if (SeasonUtils.GetNextSeason(ModEntry.SeasonByDay) == Season.Spring)
                    {
                        Game1.year++;
                    }
                    
                    ModEntry.SeasonByDay = SeasonUtils.GetNextSeason(ModEntry.SeasonByDay);
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

            monitor.Log($"Current actual season is {Game1.season.ToString()}");
            monitor.Log($"Current season by day is {ModEntry.CurrentSeason}");
            monitor.Log($"Next season change on {changeDate.ToString()}");
            monitor.Log($"Crop survival counter is {ModEntry.CropSurvivalCounter.ToString()}");

            if (Game1.Date.DayOfMonth != changeDate) return;
            monitor.Log("Change to next season");
            Game1.season = SeasonUtils.GetNextSeason(Game1.season);
            helper.Reflection.GetMethod(typeof(Game1), "OnNewSeason").Invoke();
            
            ModEntry.CropSurvivalCounter = 0;
            SaveCropSurvivalCounter(helper);
        }
    }
}