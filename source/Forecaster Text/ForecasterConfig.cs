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

using ForecasterText.Objects.Enums;

namespace ForecasterText {
    /// <summary>
    /// The mods main config for storing/reading from disc
    /// </summary>
    public sealed class ForecasterConfig {
        #region Showing Weather
        
        /// <summary>When to show the weather for Stardew Valley</summary>
        public WeatherDisplay StardewValleyWeather { get; set; } = WeatherDisplay.ALWAYS;
        
        /// <summary>When to show the weather for Ginger Island</summary>
        public WeatherDisplay GingerIslandWeather { get; set; } = WeatherDisplay.ALWAYS;
        
        #endregion
        #region Luck Emoji
        
        public bool ShowGoodLuck { get; set; } = true;
        public bool ShowNeutralLuck { get; set; } = true;
        public bool ShowBadLuck { get; set; } = true;
        
        #endregion
        #region Recipe Emoji
        
        public bool ShowNewRecipes { get; set; } = true;
        public bool ShowExistingRecipes { get; set; } = false;
        
        #endregion
        #region Spirits Emoji
        
        public uint SpiritsEmoji { get; set; } = 119u;
        public uint VeryHappySpiritEmoji { get; set; } = 43u;
        public uint GoodHumorSpiritEmoji { get; set; } = 2u;
        public uint NeutralSpiritEmoji { get; set; } = 16u;
        public uint SomewhatAnnoyedSpiritEmoji { get; set; } = 18u;
        public uint MildlyPerturbedSpiritEmoji { get; set; } = 11u;
        public uint VeryDispleasedSpiritEmoji { get; set; } = 14u;
        
        #endregion
        #region Recipe Emoji
        
        public uint NewRecipeEmoji { get; set; } = 132u;
        public uint KnownRecipeEmoji { get; set; } = 135u;
        
        #endregion
        #region Weather Emoji
        
        public uint SunWeatherEmoji { get; set; } = 99u;
        public uint RainWeatherEmoji { get; set; } = 100u;
        public uint ThunderWeatherEmoji { get; set; } = 102u;
        public uint SnowWeatherEmoji { get; set; } = 103u;
        public uint FestivalWeatherEmoji { get; set; } = 151u;
        public uint WeddingWeatherEmoji { get; set; } = 46u;
        
        #endregion
    }
}
