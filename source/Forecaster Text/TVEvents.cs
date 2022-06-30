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
using System.Text;
using ForecasterText.Objects.Enums;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace ForecasterText {
    internal class TVEvents {
        private readonly VirtualTV Television = new();
        private readonly ForecasterConfigManager ConfigManager;
        private ForecasterConfig Config => this.ConfigManager.ModConfig;
        
        public TVEvents(ForecasterConfigManager config) {
            this.ConfigManager = config;
        }
        
        public void OnDayStart(object sender, DayStartedEventArgs e) {
            if (!Context.IsWorldReady)
                return;
            
            // Get the player
            Farmer player = Game1.player;
            
            // Send messages for each event
            this.SendChatMessage(new [] {
                this.GetTownForecast(),
                this.GetIslandForecast(),
                this.GetDailyLuck(player),
                this.GetQueenOfSauce(player)
            });
        }
        
        #region Show messages in chat
        
        private void SendChatMessage( IEnumerable<string> messages ) {
            foreach (string message in messages)
                this.SendChatMessage(message);
        }
        private void SendChatMessage( string message ) {
            if (message != null)
                Game1.chatBox.addInfoMessage($"TV: {message}");
        }
        
        #endregion
        #region Weather
        
        private string GetTownForecast() {
            WorldDate date = new(Game1.Date);
            ++date.TotalDays;
            return this.GetTownForecast(!Game1.IsMasterGame ? Game1.getWeatherModificationsForDate(date, Game1.netWorldState.Value.WeatherForTomorrow) : Game1.getWeatherModificationsForDate(date, Game1.weatherForTomorrow));
        }
        
        public string GetTownForecast(int weather) {
            return this.GetWeatherInformation(this.Config.StardewValleyWeather, "Pelican Town forecast", weather);
        }
        
        private string GetIslandForecast() {
            if (!ModEntry.PlayerBeenToIsland())
                return null;
            return this.GetIslandForecast(Game1.netWorldState.Value.GetWeatherForLocation(
                Game1.getLocationFromName("IslandSouth").GetLocationContext()
            ).weatherForTomorrow.Value);
        }
        
        public string GetIslandForecast(int weather) {
            return this.GetWeatherInformation(this.Config.GingerIslandWeather, "Ginger Island forecast", weather);
        }
        
        private string GetWeatherInformation(WeatherDisplay config, string prefix, int weather) {
            if (!this.ShowWeather(config, weather))
                return null;
            
            uint[] emojis = weather switch {
                Game1.weather_sunny
                    => new[] { this.ConfigManager.GetEmoji(WeatherIcons.SUN) },
                Game1.weather_festival
                    => new[] { this.ConfigManager.GetEmoji(WeatherIcons.SUN), this.ConfigManager.GetEmoji(WeatherIcons.FESTIVAL) },
                Game1.weather_wedding
                    => new[] { this.ConfigManager.GetEmoji(WeatherIcons.SUN), this.ConfigManager.GetEmoji(WeatherIcons.WEDDING) },
                Game1.weather_rain
                    => new[] { this.ConfigManager.GetEmoji(WeatherIcons.RAIN) },
                Game1.weather_debris when Game1.currentSeason.Equals("winter")
                    => new[] { this.ConfigManager.GetEmoji(WeatherIcons.SNOW) },
                Game1.weather_debris
                    => new[] { this.ConfigManager.GetEmoji(WeatherIcons.SUN) },
                Game1.weather_lightning
                    => new[] { this.ConfigManager.GetEmoji(WeatherIcons.LIGHTNING), this.ConfigManager.GetEmoji(WeatherIcons.RAIN) },
                Game1.weather_snow
                    => new[] { this.ConfigManager.GetEmoji(WeatherIcons.SNOW) },
                _ => null
            };
            
            // If no icons, or no results
            if (emojis is not {Length: >0})
                return null;
            
            StringBuilder builder = new($"{prefix} ");
            foreach (uint emoji in emojis)
                builder.Append($"[{emoji}]");
            
            return builder.ToString();
        }
        
        private bool ShowWeather(WeatherDisplay display, int weather) {
            return display switch {
                WeatherDisplay.NEVER => false,
                WeatherDisplay.ALWAYS => true,
                _ => weather switch {
                    Game1.weather_sunny => display is WeatherDisplay.NOT_RAINING,
                    Game1.weather_festival => display is WeatherDisplay.NOT_RAINING,
                    Game1.weather_wedding => display is WeatherDisplay.NOT_RAINING,
                    Game1.weather_rain => display is WeatherDisplay.RAINING,
                    Game1.weather_debris => display is WeatherDisplay.NOT_RAINING,
                    Game1.weather_lightning => display is WeatherDisplay.RAINING,
                    Game1.weather_snow => display is WeatherDisplay.NOT_RAINING,
                    _ => false
                }
            };
        }
        
        #endregion
        #region Luck
        
        private string GetDailyLuck(Farmer who) {
            SpiritMoods mood = this.GetSpiritMood(who);
            
            if ( // If any of the "Show Luck" options is turned off
                (mood is SpiritMoods.GOOD_HUMOR or SpiritMoods.VERY_HAPPY && !this.Config.ShowGoodLuck)
                || (mood is SpiritMoods.NEUTRAL && !this.Config.ShowNeutralLuck)
                || (mood is SpiritMoods.SOMEWHAT_ANNOYED or SpiritMoods.MILDLY_PERTURBED or SpiritMoods.VERY_DISPLEASED && !this.Config.ShowBadLuck)
            ) return null;
            
            return this.GetDailyLuck(mood);
        }
        
        public string GetDailyLuck(SpiritMoods mood)
            => $"[{this.Config.SpiritsEmoji}]spirits are[{this.ConfigManager.GetEmoji(mood)}]today";
        
        private SpiritMoods GetSpiritMood(Farmer who) {
            if (who.team.sharedDailyLuck.Value == -0.12)
                return SpiritMoods.VERY_DISPLEASED; // Furious (TV.cs.13191)
            if (who.DailyLuck == 0.0)
                return SpiritMoods.NEUTRAL; // Neutral (TV.cs.13201)
            if (who.DailyLuck >= -0.07 && who.DailyLuck < -0.02) {
                Random random = new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame / 2);
                if (random.NextDouble() < 0.5)
                    return SpiritMoods.SOMEWHAT_ANNOYED; // Somewhat Annoyed (TV.cs.13193)
                return SpiritMoods.MILDLY_PERTURBED; // Mildly Perturbed (TV.cs.13195)
            }
            if (who.DailyLuck >= -0.07 && who.team.sharedDailyLuck.Value != 0.12) {
                if (who.DailyLuck > 0.07)
                    return SpiritMoods.VERY_HAPPY; // Very Happy (TV.cs.13198)
                if (who.DailyLuck <= 0.02)
                    return SpiritMoods.NEUTRAL; // Neutral (TV.cs.13200)
                return SpiritMoods.GOOD_HUMOR; // Good Humor (TV.cs.13199)
            }
            if (who.DailyLuck >= -0.07)
                return SpiritMoods.GOOD_HUMOR; // Joyous (TV.cs.13197)
            return SpiritMoods.VERY_DISPLEASED; // Very Displeased (TV.cs.13192)
        }
        
        #endregion
        #region Recipes
        
        private string GetQueenOfSauce(Farmer farmer) {
            int num = (int)(Game1.stats.DaysPlayed % 224U / 7U);
            if (Game1.stats.DaysPlayed % 224U == 0U)
                num = 32;
            FarmerTeam team = farmer.team;
            switch (Game1.dayOfMonth % 7) {
                case 0: // Sunday
                    break;
                case 3: // Wednesday
                    if (team.lastDayQueenOfSauceRerunUpdated.Value != Game1.Date.TotalDays) {
                        team.lastDayQueenOfSauceRerunUpdated.Set(Game1.Date.TotalDays);
                        team.queenOfSauceRerunWeek.Set(this.Television.GetRerunWeek());
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
            bool hasRecipe = ModEntry.PlayerHasRecipe(farmer, recipeName);
            if ((hasRecipe && !this.Config.ShowExistingRecipes) || (!hasRecipe && !this.Config.ShowNewRecipes))
                return null;
            return this.GetQueenOfSauce(recipeName, hasRecipe);
        }
        
        public string GetQueenOfSauce(string recipe, bool hasRecipe)
            => $"[{(hasRecipe ? this.Config.KnownRecipeEmoji : this.Config.NewRecipeEmoji)}]Learn to make \"{recipe}\"";
        
        #endregion
    }
}