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
using ForecasterText.Objects.Enums;
using StardewModdingAPI;
using StardewValley;

namespace ForecasterText.Objects.Messages {
    public sealed class SpiritMoodMessage : ISourceMessage {
        /// <inheritdoc />
        public string Write(Farmer farmer, ITranslationHelper t9N, ForecasterConfig config) {
            SpiritMoods mood = this.GetSpiritMood(farmer);
            
            if ( // If any of the "Show Luck" options is turned off
                (mood is SpiritMoods.GOOD_HUMOR or SpiritMoods.VERY_HAPPY && !config.ShowGoodLuck)
                || (mood is SpiritMoods.NEUTRAL && !config.ShowNeutralLuck)
                || (mood is SpiritMoods.SOMEWHAT_ANNOYED or SpiritMoods.MILDLY_PERTURBED or SpiritMoods.VERY_DISPLEASED && !config.ShowBadLuck)
            ) return null;
            
            return ISourceMessage.GetDailyLuck(mood)
                .Write(farmer, t9N, config);
        }
        
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
    }
}
