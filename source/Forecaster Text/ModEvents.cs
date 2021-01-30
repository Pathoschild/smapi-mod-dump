/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GStefanowich/SDV-Forecaster
**
*************************************************/

/*
 * This software is licensed under the MIT License
 * https://github.com/GStefanowich/SDV-Forecaster
 *
 * Copyright (c) 2019 Gregory Stefanowich
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace ForecasterText {
    public static class ModEvents {
        private static readonly VirtualTV TELEVISION = new VirtualTV();
        
        public static void OnDayStart(object sender, DayStartedEventArgs e) {
            if (!Context.IsWorldReady)
                return;
            
            // Send messages for each event
            ModEvents.SendChatMessage(new [] {
                ModEvents.GetTownForecast(),
                ModEvents.GetIslandForecast(),
                ModEvents.GetDailyLuck(),
                ModEvents.GetQueenOfSauce()
            });
        }
        
        /*
         * Show messages in chat
         */
        
        private static void SendChatMessage( IEnumerable<string> messages ) {
            foreach (string message in messages) ModEvents.SendChatMessage(message);
        }
        private static void SendChatMessage( string message ) {
            if (message != null) Game1.chatBox.addInfoMessage($"TV: {message}");
        }
        
        /*
         * Weather
         */
        
        private static string GetTownForecast() {
            WorldDate date = new WorldDate(Game1.Date);
            ++date.TotalDays;
            return ModEvents.GetWeatherInformation("Pelican Town forecast", !Game1.IsMasterGame ? Game1.getWeatherModificationsForDate(date, Game1.netWorldState.Value.WeatherForTomorrow) : Game1.getWeatherModificationsForDate(date, Game1.weatherForTomorrow));
        }
        
        private static string GetIslandForecast() {
            if (!ModEntry.PlayerBeenToIsland())
                return null;
            return ModEvents.GetWeatherInformation("Ginger Island forecast", Game1.netWorldState.Value.GetWeatherForLocation(
                Game1.getLocationFromName("IslandSouth").GetLocationContext()
            ).weatherForTomorrow.Value);
        }
        
        private static string GetWeatherInformation( string prefix, int num ) {
            switch (num) {
                case 0:
                case 4:
                case 6:
                    return $"{prefix} [99]";
                case 1:
                    return $"{prefix} [101][100]";
                case 2:
                    if (Game1.currentSeason.Equals("winter"))
                        return $"{prefix} [103]";
                    return $"{prefix} [99]";
                case 3:
                    return $"{prefix} [102][100]";
                case 5:
                    return $"{prefix} [103]";
            }
            return $"{prefix}: ???";
        }
        
        /*
         * Luck
         */
        
        private static string GetDailyLuck() {
            Farmer who = Game1.player;
            if (who.team.sharedDailyLuck.Value == -0.12)
                return "[119]are[15]today"; // Furious (TV.cs.13191)
            if (who.DailyLuck == 0.0)
                return "[119]are[16]today"; // Neutral (TV.cs.13201)
            if (who.DailyLuck >= -0.07 && who.DailyLuck < -0.02) {
                Random random = new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame / 2);
                if (random.NextDouble() < 0.5)
                    return "[119]are[18]today"; // Somewhat Annoyed (TV.cs.13193)
                return "[119]are[11]today"; // Mildly Perturbed (TV.cs.13195)
            }
            if (who.DailyLuck >= -0.07 && who.team.sharedDailyLuck.Value != 0.12) {
                if (who.DailyLuck > 0.07)
                    return "[119]are[43]today"; // Very Happy (TV.cs.13198)
                if (who.DailyLuck <= 0.02)
                    return "[119]are[16]today"; // Neutral (TV.cs.13200)
                return "[119]are[2]today"; // Good Humor (TV.cs.13199)
            }
            if (who.DailyLuck >= -0.07)
                return "[119]are[1]today"; // Joyous (TV.cs.13197)
            return "[119]are[14]today"; // Very Displeased (TV.cs.13192)
        }
        
        /*
         * Recipes
         */
        
        private static string GetQueenOfSauce() {
            int num = (int)(Game1.stats.DaysPlayed % 224U / 7U);
            if (Game1.stats.DaysPlayed % 224U == 0U)
                num = 32;
            FarmerTeam team = Game1.player.team;
            switch (Game1.dayOfMonth % 7) {
                case 0: // Sunday
                    break;
                case 3: // Wednesday
                    if (team.lastDayQueenOfSauceRerunUpdated.Value != Game1.Date.TotalDays) {
                        team.lastDayQueenOfSauceRerunUpdated.Set(Game1.Date.TotalDays);
                        team.queenOfSauceRerunWeek.Set(ModEvents.TELEVISION.GetRerunWeek());
                    }
                    num = team.queenOfSauceRerunWeek.Value;
                    break;
                default: return null;
            }
            
            // Dictionary of recipes
            Dictionary<string, string> dictionary = Game1.temporaryContent.Load<Dictionary<string, string>>("Data\\TV\\CookingChannel");
            if (!dictionary.TryGetValue($"{num}", out string translation))
                return null;
            
            // Split the translation info
            string[] recipeInfo = translation.Split('/');
            if (recipeInfo.Length <= 0)
                return null;
            
            // Get the recipe name
            string recipeName = recipeInfo[0];
            return $"[{(ModEntry.PlayerHasRecipe(recipeName) ? "135" : "132")}]Learn to make \"{recipeName}\"";
        }
        
    }
}