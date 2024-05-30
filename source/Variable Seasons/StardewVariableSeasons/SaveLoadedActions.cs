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
    public static class SaveLoadedActions
    {
        private static ModData _nextSeasonChange;
        private static ModData _seasonByDay;
        private static ModData _cropSurvivalCounter;
        private static ModDataLegacy _nextSeasonChangeLegacy;
        private static ModDataLegacy _seasonByDayLegacy;
        private static ModDataLegacy _cropSurvivalCounterLegacy;
        
        public static void OnSaveLoaded(IMonitor monitor, IModHelper helper, object sender, SaveLoadedEventArgs e)
        {
            try
            {
                _nextSeasonChange = helper.Data.ReadSaveData<ModData>("next-season-change");
                ModEntry.ChangeDate = _nextSeasonChange.NextSeasonChange;
            }
            catch
            {
                try
                {
                    _nextSeasonChangeLegacy = helper.Data.ReadSaveData<ModDataLegacy>("next-season-change");
                    ModEntry.ChangeDate = _nextSeasonChangeLegacy.NextSeasonChange;
                }
                catch
                {
                    ModEntry.ChangeDate = 0;
                }
                
                var nextSeasonChange = new ModData
                {
                    NextSeasonChange = ModEntry.ChangeDate
                };
            
                helper.Data.WriteSaveData("next-season-change", nextSeasonChange);
            }
            
            try
            {
                _seasonByDay = helper.Data.ReadSaveData<ModData>("season-by-day");
                ModEntry.SeasonByDay = _seasonByDay.SeasonByDay;
            }
            catch
            {
                try
                {
                    _seasonByDayLegacy = helper.Data.ReadSaveData<ModDataLegacy>("season-by-day");
                    ModEntry.SeasonByDay = SeasonUtils.StrToSeason(_seasonByDayLegacy.SeasonByDay);
                }
                catch
                {
                    ModEntry.SeasonByDay = Game1.season;
                }
                    
                var seasonByDay = new ModData
                {
                    SeasonByDay = ModEntry.SeasonByDay
                };
                
                helper.Data.WriteSaveData("season-by-day", seasonByDay);
            }
            
            try
            {
                _cropSurvivalCounter = helper.Data.ReadSaveData<ModData>("crop-survival-counter");
                ModEntry.CropSurvivalCounter = _cropSurvivalCounter.CropSurvivalCounter;
            }
            catch
            {
                try
                {
                    _cropSurvivalCounterLegacy = helper.Data.ReadSaveData<ModDataLegacy>("crop-survival-counter");
                    ModEntry.CropSurvivalCounter = _cropSurvivalCounterLegacy.CropSurvivalCounter;
                }
                catch
                {
                    ModEntry.CropSurvivalCounter = 5;
                }

                var cropSurvivalCounter = new ModData
                {
                    CropSurvivalCounter = ModEntry.CropSurvivalCounter
                };
                    
                helper.Data.WriteSaveData("crop-survival-counter", cropSurvivalCounter);
            }
        }
    }
}