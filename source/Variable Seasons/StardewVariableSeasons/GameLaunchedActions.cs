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
    internal record ModConfig
    {
        internal Season Season;
        internal Season SeasonByDay;
        internal int Day;
        internal int ChangeDate;
    }
    public static class GameLaunchedActions
    {
        private static void UpdateState(ModConfig config, IModHelper helper)
        {
            if (!config.Day.Equals(Game1.dayOfMonth))
            {
                Game1.dayOfMonth = config.Day;
            }

            if (!config.ChangeDate.Equals(ModEntry.ChangeDate))
            {
                ModEntry.ChangeDate = config.ChangeDate;

                var changeDate = new ModData
                {
                    NextSeasonChange = ModEntry.ChangeDate
                };
                
                helper.Data.WriteSaveData("next-season-change", changeDate);
            }
            
            Game1.season = config.Season;
            ModEntry.SeasonByDay = config.SeasonByDay;
            Game1.setGraphicsForSeason();
            Game1.netWorldState.Value.UpdateFromGame1();
            
            var seasonByDay = new ModData
            {
                SeasonByDay = ModEntry.SeasonByDay
            };
                
            helper.Data.WriteSaveData("season-by-day", seasonByDay);
        }
        public static void OnGameLaunched(IMonitor monitor, IModHelper helper, IManifest manifest, object sender, GameLaunchedEventArgs e)
        {
            var configMenu = helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null) return;

            var config = new ModConfig
            {
                Season = Game1.season,
                SeasonByDay = ModEntry.SeasonByDay,
                Day = Game1.dayOfMonth,
                ChangeDate = ModEntry.ChangeDate
            };
            configMenu.Register(
                mod: manifest,
                reset: () => config = new ModConfig
                {
                    Season = Game1.season,
                    SeasonByDay = ModEntry.SeasonByDay,
                    Day = Game1.dayOfMonth,
                    ChangeDate = ModEntry.ChangeDate
                },
                save: () => UpdateState(config, helper)
            );
            
            configMenu.AddParagraph(
                mod: manifest,
                text: () => "These options can be used to correct the in-game and calendar seasons should they become incorrect for any reason"
            );

            configMenu.AddTextOption(
                mod: manifest,
                name: () => "In-Game Season",
                getValue: () => Game1.season.ToString(),
                setValue: value => config.Season = SeasonUtils.StrToSeason(value.ToLower()),
                allowedValues: new[] { "Spring", "Summer", "Fall", "Winter" }
            );

            configMenu.AddTextOption(
                mod: manifest,
                name: () => "Calendar Season",
                getValue: () => ModEntry.SeasonByDay.ToString(),
                setValue: value => config.SeasonByDay = SeasonUtils.StrToSeason(value.ToLower()),
                allowedValues: new[] { "Spring", "Summer", "Fall", "Winter" }
            );
            
            configMenu.AddParagraph(
                mod: manifest,
                text: () => "Developer settings below this point... proceed with caution!"
            );
            
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => "Day",
                getValue: () => Game1.dayOfMonth,
                setValue: value => config.Day = value,
                min: 1,
                max: 28
            );
            
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => "Season Change Day",
                getValue: () => ModEntry.ChangeDate + 1,
                setValue: value => config.ChangeDate = value - 1,
                min: 1,
                max: 28
            );
        }
    }
}