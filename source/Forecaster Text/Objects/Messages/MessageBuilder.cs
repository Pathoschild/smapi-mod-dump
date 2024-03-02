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
using System.Linq;
using System.Text;
using ForecasterText.Objects.Addons;
using ForecasterText.Objects.Enums;
using StardewModdingAPI;
using StardewValley;

namespace ForecasterText.Objects.Messages {
    public class MessageBuilder : ISourceMessage {
        public readonly string T9N;
        private readonly IDictionary<string, IList<object>> Values = new Dictionary<string, IList<object>>();
        
        public MessageBuilder(string t9N) {
            this.T9N = t9N;
        }
        
        public MessageBuilder AddText(string key, string? text)
            => text is not null ? this.Add(key, text) : this;
        public MessageBuilder AddText(string key, char c)
            => this.Add(key, c);
        public MessageBuilder PadText(string key, char padding) {
            if (
                this.Values.TryGetValue(key, out IList<object>? list)
                && list.Count > 0
                && list.LastOrDefault() is not uint
            ) {
                list.Add(padding);
            }
            
            return this;
        }
        
        public MessageBuilder AddRecipe(string key, CraftingRecipe recipe)
            => this.AddText(key, recipe.DisplayName);
        
        public MessageBuilder AddTranslation(string key, Translation translation)
            => this.AddTranslation(key, translation, t9N => t9N.ToString());
        public MessageBuilder AddTranslation(string key, Translation path, Func<Translation, string> parser)
            => this.AddText(key, parser(path));
        
        public MessageBuilder AddEmoji(string key, uint? id)
            => id is uint i ? this.Add(key, i) : this;
        public MessageBuilder AddEmoji(string key, WeatherIcons icon)
            => this.Add(key, icon);
        public MessageBuilder AddEmoji(string key, SpiritMoods icon)
            => this.Add(key, icon);
        public MessageBuilder AddEmoji(string key, MiscEmoji icon)
            => this.Add(key, icon);
        public MessageBuilder AddEmoji(string key, Character character) => character switch {
            NPC npc => this.AddNpcEmoji(key, npc.getName()),
            _ => this
        };
        public MessageBuilder AddNpcEmoji(string key, string name) => this.AddEmoji(key, CharacterEmoji.GetEmoji(name));
        
        private MessageBuilder Add(string key, object? value) {
            if (!this.Values.TryGetValue(key, out IList<object>? list)) {
                list = new List<object>();
                this.Values[key] = list;
            }
            
            if (value is not null)
                list.Add(value);
            
            return this;
        }
        
        /// <inheritdoc/>
        public string Write(Farmer farmer, ITranslationHelper t9N, ForecasterConfig config)
            => t9N.Get(this.T9N, this.WriteVariables(config));
        
        private IDictionary<string, string> WriteVariables(ForecasterConfig config) {
            return this.Values.ToDictionary(pair => pair.Key, pair => {
                StringBuilder intern = new();
                
                foreach (object raw in pair.Value) {
                    object? cast = raw switch {
                        SpiritMoods mood => config.GetEmoji(mood),
                        WeatherIcons weather => config.GetEmoji(weather),
                        MiscEmoji emoji => config.GetEmoji(emoji),
                        _ => raw
                    };
                    
                    if (cast is null)
                        continue;
                    intern.Append(cast is uint i ? $"[{i}]" : cast);
                }
                
                return intern.ToString();
            });
        }
    }
}
