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
    public static class SaveLoadedActions
    {
        private static ModData _nextSeasonChange;
        private static ModData _seasonByDay;
        private static ModData _cropSurvivalCounter;
        
        public static void OnSaveLoaded(IMonitor monitor, IModHelper helper, object sender, SaveLoadedEventArgs e)
        {
            _nextSeasonChange = helper.Data.ReadSaveData<ModData>("next-season-change");
            _seasonByDay = helper.Data.ReadSaveData<ModData>("season-by-day");
            _cropSurvivalCounter = helper.Data.ReadSaveData<ModData>("crop-survival-counter");
            
            try
            {
                ModEntry.ChangeDate = _nextSeasonChange.NextSeasonChange;
            }
            catch
            {
                var nextSeasonChange = new ModData
                {
                    NextSeasonChange = 28
                };
            
                helper.Data.WriteSaveData("next-season-change", nextSeasonChange);
            }
            
            try
            {
                ModEntry.SeasonByDay = _seasonByDay.SeasonByDay;
            }
            catch
            {
                ModEntry.SeasonByDay = "spring";
                var seasonByDay = new ModData
                {
                    SeasonByDay = ModEntry.SeasonByDay
                };
                    
                helper.Data.WriteSaveData("season-by-day", seasonByDay);
            }
            
            try
            {
                ModEntry.CropSurvivalCounter = _cropSurvivalCounter.CropSurvivalCounter;
            }
            catch
            {
                ModEntry.CropSurvivalCounter = 5;
                var cropSurvivalCounter = new ModData
                {
                    CropSurvivalCounter = ModEntry.CropSurvivalCounter
                };
                    
                helper.Data.WriteSaveData("crop-survival-counter", cropSurvivalCounter);
            }
        }
    }
}