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

using System.Linq;
using StardewModdingAPI;
using StardewValley;

namespace ForecasterText.Objects.Messages {
    public sealed class BirthdaysMessage : ISourceMessage {
        /// <inheritdoc />
        public string Write(Farmer farmer, ITranslationHelper t9N, ForecasterConfig config) {
            // If not showing birthdays
            if (!config.ShowBirthdays)
                return null;
            
            // Get a list of todays birthdays
            MessageSource birthdays = ISourceMessage.GetBirthdays(farmer.friendshipData.FieldDict.Keys
                .Select(name => Game1.getCharacterFromName(name, true))
                .Where(npc => npc is not null && npc.isBirthday(Game1.currentSeason, Game1.dayOfMonth)), config);
            
            return birthdays is null ? null : birthdays.Write(farmer, t9N, config);
        }
    }
}
