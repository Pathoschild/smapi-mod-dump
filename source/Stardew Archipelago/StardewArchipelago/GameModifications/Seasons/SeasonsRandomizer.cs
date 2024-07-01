/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Serialization;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;

namespace StardewArchipelago.GameModifications.Seasons
{
    public class SeasonsRandomizer
    {
        private const string _nextSeasonDialogKey = "NextSeason";
        public static readonly string[] ValidSeasons = new[] { "Spring", "Summer", "Fall", "Winter" };
        private const string PROGRESSIVE_SEASON = "Progressive Season";

        private static IMonitor _monitor;
        private static IModHelper _helper;
        private static ArchipelagoClient _archipelago;
        private static ArchipelagoStateDto _state;

        public SeasonsRandomizer(IMonitor monitor, IModHelper helper, ArchipelagoClient archipelago, ArchipelagoStateDto state)
        {
            _monitor = monitor;
            _helper = helper;
            _archipelago = archipelago;
            _state = state;
        }

        public string GetFirstSeason()
        {
            return GetUnlockedSeasons()[0];
        }

        public static List<string> GetUnlockedSeasons()
        {
            if (_archipelago.SlotData.SeasonRandomization == SeasonRandomization.Progressive)
            {
                var progressiveSeasonsNumber = _archipelago.GetReceivedItemCount(PROGRESSIVE_SEASON);
                return ValidSeasons.Take(progressiveSeasonsNumber + 1).ToList();
            }

            if (_archipelago.SlotData.SeasonRandomization == SeasonRandomization.Disabled)
            {
                return ValidSeasons.ToList();
            }

            var receivedSeasons = _archipelago.GetAllReceivedItems().Select(x => x.ItemName).Where(x => ValidSeasons.Contains(x)).ToList();
            return receivedSeasons;
        }

        public static void SetNextSeason(string season)
        {
            var currentSeasonNumber = (int)Game1.stats.DaysPlayed / 28;
            if (_state.SeasonsOrder.Count <= currentSeasonNumber)
            {
                _state.SeasonsOrder.Add(season);
            }
            else
            {
                _state.SeasonsOrder[currentSeasonNumber] = season;
            }
        }

        public static void SetSeason(string season)
        {
            Game1.currentSeason = season.ToLower();
            Game1.setGraphicsForSeason();
            Utility.ForEachLocation(location =>
            {
                location.seasonUpdate();
                return true;
            });
        }

        // private static void OnNewSeason()
        public static bool OnNewSeason_UsePredefinedChoice_Prefix()
        {
            try
            {
                SetSeason(_state.SeasonsOrder.Last());
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(OnNewSeason_UsePredefinedChoice_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public static WorldDate Date => Game1.netWorldState.Value.Date;
        public static bool Date_UseTotalDaysStats_Prefix(ref WorldDate __result)
        {
            try
            {
                GetVanillaValues(out var totalDays, out var year, out var seasonNumber, out var seasonName);
                var dayOfMonth = (totalDays % 28) + 1;
                __result = new WorldDate(year, seasonName.ToLower(), dayOfMonth);
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(Date_UseTotalDaysStats_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        public static bool NewDay_SeasonChoice_Prefix(float timeToPause)
        {
            try
            {
                if (Game1.dayOfMonth != 28)
                {
                    return true; // run original logic
                }

                var possibleResponses = new List<Response>();

                foreach (var season in GetUnlockedSeasons())
                {
                    possibleResponses.Add(new Response(season, season).SetHotKey(Keys.None));
                }

                var seasonName = Utility.capitalizeFirstLetter(Game1.CurrentSeasonDisplayName);
                Game1.currentLocation.createQuestionDialogue($"{seasonName} has come to an end. What season is next?", possibleResponses.ToArray(), _nextSeasonDialogKey, null);
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(NewDay_SeasonChoice_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        public static bool AnswerDialogueAction_SeasonChoice_Prefix(GameLocation __instance, string questionAndAnswer, string[] questionParams, ref bool __result)
        {
            try
            {
                if (!questionAndAnswer.StartsWith(_nextSeasonDialogKey))
                {
                    return true; // run original logic
                }

                var parts = questionAndAnswer.Split("_");
                var chosenSeason = parts[1];
                SetNextSeason(chosenSeason);

                __result = true;
                NewDayOriginal(0);
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(AnswerDialogueAction_SeasonChoice_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static void NewDayOriginal(float timeToPause)
        {
            Game1.currentMinigame = null;
            Game1.newDay = true;
            Game1.newDaySync = new NewDaySynchronizer();
            if (Game1.player.isInBed.Value || Game1.player.passedOut)
            {
                Game1.nonWarpFade = true;
                var screenFadeField = _helper.Reflection.GetField<ScreenFade>(typeof(Game1), "screenFade");
                screenFadeField.GetValue().FadeScreenToBlack(Game1.player.passedOut ? 1.1f : 0.0f);
                Game1.player.Halt();
                Game1.player.currentEyes = 1;
                Game1.player.blinkTimer = -4000;
                Game1.player.CanMove = false;
                Game1.player.passedOut = false;
                Game1.pauseTime = timeToPause;
            }
            if (Game1.activeClickableMenu == null || Game1.dialogueUp)
                return;
            Game1.activeClickableMenu.emergencyShutDown();
            Game1.exitActiveMenu();
        }

        // public int CountdownToWedding
        public static bool CountdownToWedding_Add1_Prefix(Friendship __instance, ref int __result)
        {
            try
            {
                if (__instance.WeddingDate == null || __instance.WeddingDate.TotalDays < Game1.Date.TotalDays)
                    __result = 0;
                else
                    __result = __instance.WeddingDate.TotalDays - Game1.Date.TotalDays + 1;

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CountdownToWedding_Add1_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public static string getWeatherModificationsForDate(WorldDate date, string default_weather)
        public static bool GetWeatherModificationsForDate_UseCorrectDates_Prefix(WorldDate date, string default_weather, ref string __result)
        {
            try
            {
                var chosenWeather = default_weather;
                var num = date.TotalDays - Game1.Date.TotalDays;
                var currentSeason = Game1.season;
                if (date.DayOfMonth == 1 || Game1.stats.DaysPlayed + num <= 4L)
                {
                    chosenWeather = Weather.Sun;
                }

                if (Game1.stats.DaysPlayed + num == 3L)
                {
                    chosenWeather = Weather.Rain;
                }

                if (currentSeason == Season.Summer && date.DayOfMonth % 13 == 0)
                {
                    chosenWeather = Weather.Storm;
                }

                if (Utility.isFestivalDay(date.DayOfMonth, currentSeason))
                {
                    chosenWeather = Weather.Festival;
                }

                foreach (var passiveFestivalData in DataLoader.PassiveFestivals(Game1.content).Values)
                {
                    if (date.DayOfMonth < passiveFestivalData.StartDay || date.DayOfMonth > passiveFestivalData.EndDay || date.Season != passiveFestivalData.Season ||
                        !GameStateQuery.CheckConditions(passiveFestivalData.Condition) || passiveFestivalData.MapReplacements == null)
                    {
                        continue;
                    }
                    foreach (var key in passiveFestivalData.MapReplacements.Keys)
                    {
                        var locationFromName = Game1.getLocationFromName(key);
                        if (locationFromName != null && locationFromName.InValleyContext())
                        {
                            chosenWeather = Weather.Sun;
                            break;
                        }
                    }
                }

                __result = chosenWeather;
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GetWeatherModificationsForDate_UseCorrectDates_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static readonly Dictionary<string, string> _alternateMailKeys = new()
        {
            { "spring_2_1", "year_1_day_2" },
            { "spring_6_2", "spring_6_1" },
            { "spring_15_2", "spring_16_1" },
            { "spring_21_2", "spring_21_1" },
            { "summer_3_1", "year_1_day_31" },
            { "summer_6_2", "summer_6_1" },
            { "summer_21_2", "summer_21_1" },
            { "fall_6_2", "fall_6_1" },
            { "fall_19_2", "fall_20_1" },
            { "winter_5_2", "winter_5_1" },
            { "winter_13_2", "winter_13_1" },
            { "winter_19_2", "winter_19_1" },
        };

        public static void ChangeMailKeysBasedOnSeasonsToDaysElapsed()
        {
            var mailData = DataLoader.Mail(Game1.content);
            foreach (var originalKey in _alternateMailKeys.Keys)
            {
                if (mailData.ContainsKey(originalKey))
                {
                    mailData.Add(_alternateMailKeys[originalKey], mailData[originalKey]);
                    mailData.Remove(originalKey);
                }
            }
        }

        public static void ResetMailKeys()
        {
            var mailData = DataLoader.Mail(Game1.content);
            foreach (var modifiedKey in _alternateMailKeys.Values)
            {
                if (mailData.ContainsKey(modifiedKey))
                {
                    var originalKey = _alternateMailKeys.Keys.First(x => _alternateMailKeys[x] == modifiedKey);
                    mailData.Add(originalKey, mailData[modifiedKey]);
                    mailData.Remove(modifiedKey);
                }
            }
        }

        public static void SendMailHardcodedForToday()
        {
            GetVanillaValues(out var totalDays, out var year, out var seasonNumber, out var _);
            var mailData = DataLoader.Mail(Game1.content);
            SendMailForCurrentDateSpecificYear(year, Game1.currentSeason, mailData);
            SendMailForCurrentTotalDaysElapsed(year, mailData);
        }

        private static void SendMailForCurrentDateSpecificYear(int year, string seasonName, Dictionary<string, string> mailData)
        {
            for (var i = 1; i <= year; i++)
            {
                var key = seasonName + "_" + Game1.dayOfMonth + "_" + i;
                if (key.Equals("spring_1_2"))
                {
                    continue;
                }

                SendMailIfNeverReceivedBefore(mailData, key);
            }
        }

        private static void SendMailForCurrentTotalDaysElapsed(int year, Dictionary<string, string> mailData)
        {
            var totalDays = Game1.stats.DaysPlayed;
            var daysThisYear = totalDays - (112 * (year - 1));
            for (var i = 1; i <= year; i++)
            {
                var keyToday = $"year_{i}_day_{daysThisYear}";
                SendMailIfNeverReceivedBefore(mailData, keyToday);
            }
        }

        private static void SendMailIfNeverReceivedBefore(Dictionary<string, string> mailData, string key)
        {
            if (!mailData.ContainsKey(key))
            {
                return;
            }

            var originalKey = _alternateMailKeys.Keys.FirstOrDefault(x => _alternateMailKeys[x] == key);
            if (originalKey != null && !Game1.player.mailReceived.Contains(originalKey))
            {
                Game1.player.mailReceived.Add(originalKey);
            }

            if (Game1.player.hasOrWillReceiveMail(key))
            {
                if (Game1.player.mailReceived.Contains(key) && Game1.player.mailbox.Contains(key))
                {
                    Game1.player.mailbox.Remove(key);
                }

                return;
            }

            Game1.mailbox.Add(key);
        }

        public static void PrepareDateForSaveGame()
        {
            GetVanillaValues(out var totalDays, out var year, out var seasonNumber, out _);
            Game1.year = year;
            Game1.player.dayOfMonthForSaveGame = Game1.dayOfMonth;
            Game1.player.seasonForSaveGame = seasonNumber;
            Game1.player.yearForSaveGame = Game1.year;
        }

        private static void GetVanillaValues(out int totalDays, out int year, out int seasonNumber, out string seasonName)
        {
            totalDays = (int)Game1.stats.DaysPlayed - 1;
            year = (totalDays / 112);
            var daysThisYear = totalDays - (year * 112);
            seasonNumber = daysThisYear / 28;
            seasonName = ValidSeasons[seasonNumber];
            year = year + 1;
        }

        private static class Weather
        {
            public const string Sun = "Sun";
            public const string Rain = "Rain";
            public const string Storm = "Storm";
            public const string Festival = "Festival";
        }
    }
}
