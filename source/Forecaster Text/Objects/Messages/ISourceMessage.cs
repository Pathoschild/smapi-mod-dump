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

using System.Collections.Generic;
using ForecasterText.Objects.Addons;
using ForecasterText.Objects.Enums;
using StardewModdingAPI;
using StardewValley;

namespace ForecasterText.Objects.Messages {
    internal interface ISourceMessage {
        string Write(Farmer farmer, ITranslationHelper t9N, ForecasterConfig config);
        string Write(ForecasterConfig config, ITranslationHelper t9N)
            => this.Write(Game1.player, t9N, config);
        
        public static MessageSource GetDailyLuck(SpiritMoods mood)
            => MessageSource.TV(new MessageBuilder("tv.spirits")
                .AddEmoji("spirit", MiscEmoji.SPIRITS)
                .AddEmoji("mood", mood));
        
        public static MessageSource GetQueenOfSauce(string recipe, bool hasRecipe)
            => MessageSource.TV(new MessageBuilder("tv.recipe")
                .AddEmoji("icon", hasRecipe ? MiscEmoji.KNOWN_RECIPE : MiscEmoji.NEW_RECIPE)
                .AddTranslation("recipe", $@"Data\CookingRecipes:{recipe}", content => content?.Split('/') is {Length: >=5} split ? split[4] : recipe));
        
        public static MessageSource GetBirthdays(IEnumerable<object> characters, ForecasterConfig config) {
            MessageBuilder builder = null;
            
            foreach (object obj in characters) {
                // Create the build if it doesn't exist
                builder ??= new MessageBuilder("tv.birthday")
                    .AddEmoji("icon", MiscEmoji.BIRTHDAY);
                
                // If using Emojis
                if (!config.UseVillagerNames) {
                    // Try adding an emoji
                    if(obj switch {
                        Character character when character.HasEmoji()
                            => builder.AddEmoji("...", character),
                        string name when CharacterEmoji.HasEmoji(name)
                            => builder.AddNpcEmoji("...", name),
                        _ => null
                    } is not null)
                        continue; // Continue to next to skip fallback to name (For custom NPCs)
                }
                
                builder.PadText("...", ' ') // Add a space between names
                    .AddText("...", (obj as Character)?.GetName() ?? obj as string);
            }
            
            return MessageSource.Calendar(builder);
        }
    }
}
