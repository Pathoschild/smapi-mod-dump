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
using ForecasterText.Objects;
using ForecasterText.Objects.Enums;
using GenericModConfigMenu;
using StardewModdingAPI;

namespace ForecasterText {
    public sealed class ForecasterConfigManager {
        private ForecasterConfig _CachedConfig;
        private readonly ModEntry Mod;
        
        private List<ConfigEmojiMessage> Examples = new();
        
        public ForecasterConfig ModConfig {
            get => this._CachedConfig ??= this.Mod.Helper.ReadConfig<ForecasterConfig>();
            private set => this._CachedConfig = value;
        }
        
        public ForecasterConfigManager(ModEntry mod) {
            this.Mod = mod;
        }
        
        #region Mod Config Menu
        
        private IGenericModConfigMenuApi ConfigMenu { get; set; }
        private IManifest Manifest => this.Mod.ModManifest;
        
        public void RegisterConfigManager(IGenericModConfigMenuApi configMenu) {
            this.ConfigMenu = configMenu;
            
            // Register our config callbacks
            this.Register(
                reset: () => this.ModConfig = new ForecasterConfig(),
                save: () => this.Mod.Helper.WriteConfig(this.ModConfig)
            );
            
            // When to display weather
            this.AddSectionTitle(
                "Show Weather",
                "When to show weather messages"
            );
            
            this.AddWeatherDropdown(
                () => this.ModConfig.StardewValleyWeather,
                display => this.ModConfig.StardewValleyWeather = display,
                "Stardew Valley",
                "When to show the weather for Stardew Valley"
            );
            
            this.AddWeatherDropdown(
                () => this.ModConfig.GingerIslandWeather,
                display => this.ModConfig.GingerIslandWeather = display,
                "Ginger Island",
                "When to show the weather for Ginger Island"
            );
            
            // When to display luck
            this.AddSectionTitle(
                "Show Luck",
                "When to show luck messages"
            );
            
            this.AddBoolOption(
                () => this.ModConfig.ShowGoodLuck,
                value => this.ModConfig.ShowGoodLuck = value,
                "Show Good Luck",
                "If good luck messages are displayed"
            );
            
            this.AddBoolOption(
                () => this.ModConfig.ShowNeutralLuck,
                value => this.ModConfig.ShowNeutralLuck = value,
                "Show Neutral Luck",
                "If neutral luck messages are displayed"
            );
            
            this.AddBoolOption(
                () => this.ModConfig.ShowBadLuck,
                value => this.ModConfig.ShowBadLuck = value,
                "Show Bad Luck",
                "If bad luck messages are displayed"
            );
            
            // When to display recipes
            this.AddSectionTitle(
                "Show Recipes",
                "When to show recipe messages"
            );
            
            this.AddBoolOption(
                () => this.ModConfig.ShowNewRecipes,
                value => this.ModConfig.ShowNewRecipes = value,
                "Show new recipes",
                "Show messages for recipes that are not known"
            );
            
            this.AddBoolOption(
                () => this.ModConfig.ShowExistingRecipes,
                value => this.ModConfig.ShowExistingRecipes = value,
                "Show learned recipes",
                "Show messages for recipes that are already known"
            );
            
            // The icons to use
            this.AddSectionTitle(
                "Event Icons",
                "The icons to use in messages for spirits"
            );
            
            // Emoji to represent Spirits
            this.AddEmojiSelector("Spirits", null,
                () => this.ModConfig.SpiritsEmoji,
                i => this.ModConfig.SpiritsEmoji = i
            );
            this.AddEmojiSelector("Very Happy", null,
                () => this.ModConfig.VeryHappySpiritEmoji,
                i => this.ModConfig.VeryHappySpiritEmoji = i,
                message => this.SpiritExampleMessage(message, SpiritMoods.VERY_HAPPY)
            );
            this.AddEmojiSelector("Good Humor", null,
                () => this.ModConfig.GoodHumorSpiritEmoji,
                i => this.ModConfig.GoodHumorSpiritEmoji = i,
                message => this.SpiritExampleMessage(message, SpiritMoods.GOOD_HUMOR)
            );
            this.AddEmojiSelector("Neutral", null,
                () => this.ModConfig.NeutralSpiritEmoji,
                i => this.ModConfig.NeutralSpiritEmoji = i,
                message => this.SpiritExampleMessage(message, SpiritMoods.NEUTRAL)
            );
            this.AddEmojiSelector("Somewhat Annoyed", null,
                () => this.ModConfig.SomewhatAnnoyedSpiritEmoji,
                i => this.ModConfig.SomewhatAnnoyedSpiritEmoji = i,
                message => this.SpiritExampleMessage(message, SpiritMoods.SOMEWHAT_ANNOYED)
            );
            this.AddEmojiSelector("Mildly Perturbed", null,
                () => this.ModConfig.MildlyPerturbedSpiritEmoji,
                i => this.ModConfig.MildlyPerturbedSpiritEmoji = i,
                message => this.SpiritExampleMessage(message, SpiritMoods.MILDLY_PERTURBED)
            );
            this.AddEmojiSelector("Very Displeased", null,
                () => this.ModConfig.VeryDispleasedSpiritEmoji,
                i => this.ModConfig.VeryDispleasedSpiritEmoji = i,
                message => this.SpiritExampleMessage(message, SpiritMoods.VERY_DISPLEASED)
            );
            
            // Emoji for recipes
            this.AddSectionTitle(
                "Recipe Icons",
                "The icons to use in messages for recipes"
            );
            this.AddEmojiSelector("New Recipe", null,
                () => this.ModConfig.NewRecipeEmoji,
                i => this.ModConfig.NewRecipeEmoji = i,
                message => this.RecipeExampleMessage(message, false)
            );
            this.AddEmojiSelector("Known Recipe", null,
                () => this.ModConfig.KnownRecipeEmoji,
                i => this.ModConfig.KnownRecipeEmoji = i,
                message => this.RecipeExampleMessage(message, true)
            );
            
            // Emojis for weather
            this.AddSectionTitle(
                "Weather Icons",
                "The icons to use in messages for weather"
            );
            this.AddEmojiSelector("Sunny", "Displays if it will be sunny the next day", 
                () => this.ModConfig.SunWeatherEmoji,
                i => this.ModConfig.SunWeatherEmoji = i,
                message => this.WeatherExampleMessage(message, WeatherIcons.SUN)
            );
            this.AddEmojiSelector("Rain", "Displays if it will rain the next day", 
                () => this.ModConfig.RainWeatherEmoji,
                i => this.ModConfig.RainWeatherEmoji = i,
                message => this.WeatherExampleMessage(message, WeatherIcons.RAIN)
            );
            this.AddEmojiSelector("Thunder", "Displays if it will thunder the next day",
                () => this.ModConfig.ThunderWeatherEmoji,
                i => this.ModConfig.ThunderWeatherEmoji = i,
                message => this.WeatherExampleMessage(message, WeatherIcons.LIGHTNING)
            );
            this.AddEmojiSelector("Snow", "Displays if it will snow the next day",
                () => this.ModConfig.SnowWeatherEmoji,
                i => this.ModConfig.SnowWeatherEmoji = i,
                message => this.WeatherExampleMessage(message, WeatherIcons.SNOW)
            );
            this.AddEmojiSelector("Festival", "Displays with the weather if a festival is the next day",
                () => this.ModConfig.FestivalWeatherEmoji,
                i => this.ModConfig.FestivalWeatherEmoji = i,
                message => this.WeatherExampleMessage(message, WeatherIcons.FESTIVAL)
            );
            this.AddEmojiSelector("Wedding", "Displays with the weather if a wedding is the next day",
                () => this.ModConfig.WeddingWeatherEmoji,
                i => this.ModConfig.WeddingWeatherEmoji = i,
                message => this.WeatherExampleMessage(message, WeatherIcons.WEDDING)
            );
        }
        
        private void Register(Action reset, Action save)
            => this.ConfigMenu?.Register(this.Manifest, reset, save);
        
        private void AddSectionTitle(string text, string tooltip = null)
            => this.AddSectionTitle(() => text, () => tooltip);
        
        private void AddSectionTitle(Func<string> text, Func<string> tooltip = null)
            => this.ConfigMenu?.AddSectionTitle(this.Manifest, text, tooltip);
        
        private void AddBoolOption(Func<bool> getValue, Action<bool> setValue, string name, string tooltip = null)
            => this.AddBoolOption(getValue, setValue, () => name, () => tooltip);
        
        private void AddBoolOption(Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null)
            => this.ConfigMenu?.AddBoolOption(this.Manifest, getValue, setValue, name, tooltip);
        
        private void AddWeatherDropdown(Func<WeatherDisplay> getValue, Action<WeatherDisplay> setValue, string name, string tooltip = null)
            => this.AddWeatherDropdown(getValue, setValue, () => name, () => tooltip);
        
        private void AddWeatherDropdown(Func<WeatherDisplay> getValue, Action<WeatherDisplay> setValue, Func<string> name, Func<string> tooltip = null)
            => this.ConfigMenu?.AddTextOption(this.Manifest, () => Config.Normalize(getValue()), value => setValue(Config.FromInput<WeatherDisplay>(value)), name, tooltip, Config.Values<WeatherDisplay>());
        
        private void AddEmojiSelector(string text, string tooltip = null, Func<uint> get = null, Action<uint> set = null, ConfigMessageParsingRenderer parser = null) {
            // Unlike other types check if the config exists before constructing types
            if (this.ConfigMenu is {} config) {
                ConfigEmojiMenu menu = new(this.Mod, text, tooltip, get, i => {
                    this.Examples.ForEach(message => message.Dirty = true);
                    set?.Invoke(i);
                });
                
                if (parser is not null) {
                    ConfigEmojiMessage display = new(this.Mod, menu, parser);
                    this.Examples.Add(display);
                    
                    config.AddComplexOption(
                        this.Manifest,
                        () => string.Empty,
                        display.OnDraw,
                        () => null,
                        height: () => 0
                    );
                }
                
                config.AddComplexOption(
                    this.Manifest,
                    () => menu.Name,
                    menu.OnDraw,
                    () => menu.Tooltip,
                    height: () => menu.Height,
                    afterReset: () => menu.ResetView(),
                    beforeMenuOpened: () => menu.ResetView()
                );
            }
        }
        
        private void AddParagraph(string text) => 
            this.AddParagraph(() => text);
        
        private void AddParagraph(Func<string> text) => 
            this.ConfigMenu.AddParagraph(this.Manifest, text);
        
        #endregion
        #region Getters
        
        public uint GetEmoji(WeatherIcons icon) => icon switch {
            WeatherIcons.SUN => this.ModConfig.SunWeatherEmoji,
            WeatherIcons.RAIN => this.ModConfig.RainWeatherEmoji,
            WeatherIcons.LIGHTNING => this.ModConfig.ThunderWeatherEmoji,
            WeatherIcons.FESTIVAL => this.ModConfig.FestivalWeatherEmoji,
            WeatherIcons.SNOW => this.ModConfig.SnowWeatherEmoji,
            WeatherIcons.WEDDING => this.ModConfig.WeddingWeatherEmoji,
            _ => 0u
        };
        public uint GetEmoji(SpiritMoods icon) => icon switch {
            SpiritMoods.VERY_HAPPY => this.ModConfig.VeryHappySpiritEmoji,
            SpiritMoods.GOOD_HUMOR => this.ModConfig.GoodHumorSpiritEmoji,
            SpiritMoods.NEUTRAL => this.ModConfig.NeutralSpiritEmoji,
            SpiritMoods.SOMEWHAT_ANNOYED => this.ModConfig.SomewhatAnnoyedSpiritEmoji,
            SpiritMoods.MILDLY_PERTURBED => this.ModConfig.MildlyPerturbedSpiritEmoji,
            SpiritMoods.VERY_DISPLEASED => this.ModConfig.VeryDispleasedSpiritEmoji,
            _ => 0u
        };
        
        #endregion
        #region Examples
        
        internal string SpiritExampleMessage(ConfigEmojiMessage message, SpiritMoods mood)
            => this.Mod.Events.GetDailyLuck(mood);
        
        internal string WeatherExampleMessage(ConfigEmojiMessage message, WeatherIcons weatherDisplay)
            => this.Mod.Events.GetTownForecast((int)weatherDisplay);
        
        internal string RecipeExampleMessage(ConfigEmojiMessage message, bool hasRecipe)
            => this.Mod.Events.GetQueenOfSauce("Trout Soup", hasRecipe);
        
        #endregion
    }
}
