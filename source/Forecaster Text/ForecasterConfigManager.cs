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
using ForecasterText.Objects.Messages;
using GenericModConfigMenu;
using StardewModdingAPI;

namespace ForecasterText {
    public sealed class ForecasterConfigManager {
        private ForecasterConfig _CachedConfig;
        private readonly ModEntry Mod;
        private ITranslationHelper Translations => this.Mod.Helper.Translation;
        
        private List<ConfigEmojiMessage> Examples = new();
        
        public ForecasterConfig ModConfig {
            get => this._CachedConfig ??= this.Mod.Helper.ReadConfig<ForecasterConfig>();
            private set => this._CachedConfig = value;
        }
        public ForecasterConfig MultiplayerConfig {
            get => this.ModConfig.Child ??= new ForecasterConfig();
            private set => this.ModConfig.Child = value;
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
            
            // TODO: Re-Enable multiplayer config display once message format for sending is figured out
            //ForecasterConfig multiplayerConfig = this.MultiplayerConfig;
            this.InitializePage(this.ModConfig);
            //this.InitializePage(multiplayerConfig, "child");
        }
        
        private void InitializePage(ForecasterConfig config, string page = null) {
            if (page is "child") {
                this.ConfigMenu.AddPage(this.Manifest, page, () => "Multiplayer");
                this.AddParagraph("Here you can set the options that are used for sending the TV information to other players. They will only receive alerts if they do not already have this mod");
            }
            
            // When to display weather
            this.AddSectionTitle("weather");
            
            this.AddWeatherDropdown(
                () => config.StardewValleyWeather,
                display => config.StardewValleyWeather = display,
                "weather.pelican_town"
            );
            
            this.AddWeatherDropdown(
                () => config.GingerIslandWeather,
                display => config.GingerIslandWeather = display,
                "weather.ginger_island"
            );
            
            // When to display birthdays
            this.AddSectionTitle("birthdays");
            
            this.AddBoolOption(
                () => config.ShowBirthdays,
                value => config.ShowBirthdays = value,
                "birthdays",
                "birthdays.desc2"
            );
            this.AddBoolOption(
                () => config.UseVillagerNames,
                value => {
                    config.UseVillagerNames = value;
                    this.ReRender();
                },
                "birthdays.use_names"
            );
            
            // When to display luck
            this.AddSectionTitle("luck");
            
            this.AddBoolOption(
                () => config.ShowGoodLuck,
                value => config.ShowGoodLuck = value,
                "luck.good"
            );
            
            this.AddBoolOption(
                () => config.ShowNeutralLuck,
                value => config.ShowNeutralLuck = value,
                "luck.neutral"
            );
            
            this.AddBoolOption(
                () => config.ShowBadLuck,
                value => config.ShowBadLuck = value,
                "luck.bad"
            );
            
            // When to display recipes
            this.AddSectionTitle("recipes");
            
            this.AddBoolOption(
                () => config.ShowNewRecipes,
                value => config.ShowNewRecipes = value,
                "recipes.new"
            );
            
            this.AddBoolOption(
                () => config.ShowExistingRecipes,
                value => config.ShowExistingRecipes = value,
                "recipes.learned"
            );
            
            if (config.Child is not null) {
                // Multiplayer options
                this.AddPageLink(
                    "child",
                    "multiplayer"
                );
                
                this.AddBoolOption(
                    () => config.SendToOthers,
                    value => config.SendToOthers = value,
                    "multiplayer.share"
                );
                
                this.AddBoolOption(
                    () => config.UseSameForOthers,
                    value => config.UseSameForOthers = value,
                    "multiplayer.config"
                );
            }
            
            // The icons to use
            this.AddSectionTitle("icons.spirits");
            
            // Emoji to represent Spirits
            this.AddEmojiSelector("icons.spirit", null,
                () => config.SpiritsEmoji,
                i => config.SpiritsEmoji = i
            );
            this.AddEmojiSelector("icons.spirits.very_happy", null,
                () => config.VeryHappySpiritEmoji,
                i => config.VeryHappySpiritEmoji = i,
                message => this.SpiritExampleMessage(message, SpiritMoods.VERY_HAPPY)?.Write(config, this.Translations)
            );
            this.AddEmojiSelector("icons.spirits.good_humor", null,
                () => config.GoodHumorSpiritEmoji,
                i => config.GoodHumorSpiritEmoji = i,
                message => this.SpiritExampleMessage(message, SpiritMoods.GOOD_HUMOR)?.Write(config, this.Translations)
            );
            this.AddEmojiSelector("icons.spirits.neutral", null,
                () => config.NeutralSpiritEmoji,
                i => config.NeutralSpiritEmoji = i,
                message => this.SpiritExampleMessage(message, SpiritMoods.NEUTRAL)?.Write(config, this.Translations)
            );
            this.AddEmojiSelector("icons.spirits.somewhat_annoyed", null,
                () => config.SomewhatAnnoyedSpiritEmoji,
                i => config.SomewhatAnnoyedSpiritEmoji = i,
                message => this.SpiritExampleMessage(message, SpiritMoods.SOMEWHAT_ANNOYED)?.Write(config, this.Translations)
            );
            this.AddEmojiSelector("icons.spirits.mildly_perturbed", null,
                () => config.MildlyPerturbedSpiritEmoji,
                i => config.MildlyPerturbedSpiritEmoji = i,
                message => this.SpiritExampleMessage(message, SpiritMoods.MILDLY_PERTURBED)?.Write(config, this.Translations)
            );
            this.AddEmojiSelector("icons.spirits.very_displeased", null,
                () => config.VeryDispleasedSpiritEmoji,
                i => config.VeryDispleasedSpiritEmoji = i,
                message => this.SpiritExampleMessage(message, SpiritMoods.VERY_DISPLEASED)?.Write(config, this.Translations)
            );
            
            // Emoji for recipes
            this.AddSectionTitle("icons.recipes");
            
            this.AddEmojiSelector("icons.recipes.new", null,
                () => config.NewRecipeEmoji,
                i => config.NewRecipeEmoji = i,
                message => this.RecipeExampleMessage(message, false)?.Write(config, this.Translations)
            );
            this.AddEmojiSelector("icons.recipes.learned", null,
                () => config.KnownRecipeEmoji,
                i => config.KnownRecipeEmoji = i,
                message => this.RecipeExampleMessage(message, true)?.Write(config, this.Translations)
            );
            
            // Emoji for birthdays
            this.AddSectionTitle("icons.birthdays");
            
            this.AddEmojiSelector("icons.birthdays.today", null,
                () => config.BirthdayEmoji,
                i => config.BirthdayEmoji = i,
                message => this.BirthdayExampleMessage(message, new [] { "Shane", "Abigail" })?.Write(config, this.Translations)
            );
            
            // Emojis for weather
            this.AddSectionTitle("icons.weather");
            
            this.AddEmojiSelector("icons.weather.sunny",
                () => config.SunWeatherEmoji,
                i => config.SunWeatherEmoji = i,
                message => this.WeatherExampleMessage(message, WeatherIcons.SUN)?.Write(config, this.Translations)
            );
            this.AddEmojiSelector("icons.weather.rain",
                () => config.RainWeatherEmoji,
                i => config.RainWeatherEmoji = i,
                message => this.WeatherExampleMessage(message, WeatherIcons.RAIN)?.Write(config, this.Translations)
            );
            this.AddEmojiSelector("icons.weather.thunder",
                () => config.ThunderWeatherEmoji,
                i => config.ThunderWeatherEmoji = i,
                message => this.WeatherExampleMessage(message, WeatherIcons.LIGHTNING)?.Write(config, this.Translations)
            );
            this.AddEmojiSelector("icons.weather.snow",
                () => config.SnowWeatherEmoji,
                i => config.SnowWeatherEmoji = i,
                message => this.WeatherExampleMessage(message, WeatherIcons.SNOW)?.Write(config, this.Translations)
            );
            this.AddEmojiSelector("icons.weather.festival",
                () => config.FestivalWeatherEmoji,
                i => config.FestivalWeatherEmoji = i,
                message => this.WeatherExampleMessage(message, WeatherIcons.FESTIVAL)?.Write(config, this.Translations)
            );
            this.AddEmojiSelector("icons.weather.wedding",
                () => config.WeddingWeatherEmoji,
                i => config.WeddingWeatherEmoji = i,
                message => this.WeatherExampleMessage(message, WeatherIcons.WEDDING)?.Write(config, this.Translations)
            );
        }
        
        private void Register(Action reset, Action save)
            => this.ConfigMenu?.Register(this.Manifest, reset, save);
        
        private void AddSectionTitle(string key) {
            IConfT9N t9N = this.Translation(key);
            this.AddSectionTitle(t9N.Get, t9N.GetDesc);
        }
        
        private void AddSectionTitle(Func<string> text, Func<string> tooltip = null)
            => this.ConfigMenu?.AddSectionTitle(this.Manifest, text, tooltip);
        
        private void AddBoolOption(Func<bool> getValue, Action<bool> setValue, string name) {
            IConfT9N t9N = this.Translation(name);
            this.AddBoolOption(getValue, setValue, t9N.Get, t9N.GetDesc);
        }
        private void AddBoolOption(Func<bool> getValue, Action<bool> setValue, string name, string tooltip) {
            IConfT9N t9N = this.Translation(name, tooltip);
            this.AddBoolOption(getValue, setValue, t9N.Get, t9N.GetDesc);
        }
        private void AddBoolOption(Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null)
            => this.ConfigMenu?.AddBoolOption(this.Manifest, getValue, setValue, name, tooltip);
        
        private void AddWeatherDropdown(Func<WeatherDisplay> getValue, Action<WeatherDisplay> setValue, string name) {
            IConfT9N t9N = this.Translation(name);
            this.AddWeatherDropdown(getValue, setValue, t9N.Get, t9N.GetDesc);
        }
        
        private void AddWeatherDropdown(Func<WeatherDisplay> getValue, Action<WeatherDisplay> setValue, Func<string> name, Func<string> tooltip = null) => this.ConfigMenu?.AddTextOption(
            this.Manifest,
            () => getValue().ToString(),
            value => setValue(Enum.TryParse(value, true, out WeatherDisplay display) ? display : WeatherDisplay.ALWAYS),
            name,
            tooltip,
            allowedValues: Config.Values<WeatherDisplay>(),
            formatAllowedValue: value => this.Translations.Get($"config.weather.show.{value.ToLowerInvariant()}")
        );
        
        private void AddEmojiSelector(string text, Func<uint> get = null, Action<uint> set = null, ConfigMessageParsingRenderer parser = null)
            => this.AddEmojiSelector(text, text is null ? null : $"{text}.desc", get, set, parser);
        
        private void AddEmojiSelector(string text, string tooltip, Func<uint> get = null, Action<uint> set = null, ConfigMessageParsingRenderer parser = null) {
            // Unlike other types check if the config exists before constructing types
            if (this.ConfigMenu is {} config) {
                ConfigEmojiMenu menu = new(this.Mod, this.Translation(text, tooltip), get, i => {
                    set?.Invoke(i);
                    this.ReRender();
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
                    menu.T9N.Get,
                    menu.OnDraw,
                    menu.T9N.GetDesc,
                    height: () => menu.Height,
                    afterReset: () => menu.ResetView(),
                    beforeMenuOpened: () => menu.ResetView()
                );
            }
        }
        
        private void AddPageLink(string page, string text) {
            IConfT9N t9N = this.Translation(text);
            this.AddPageLink(page, t9N.Get, t9N.GetDesc);
        }

        private void AddPageLink(string page, Func<string> text, Func<string> tooltip)
            => this.ConfigMenu?.AddPageLink(this.Manifest, page, text, tooltip);
        
        private void AddParagraph(string text) => 
            this.AddParagraph(() => text);
        
        private void AddParagraph(Func<string> text) => 
            this.ConfigMenu?.AddParagraph(this.Manifest, text);
        
        #endregion
        #region Examples
        
        internal ISourceMessage SpiritExampleMessage(ConfigEmojiMessage message, SpiritMoods mood)
            => ISourceMessage.GetDailyLuck(mood);
        
        internal ISourceMessage RecipeExampleMessage(ConfigEmojiMessage message, bool hasRecipe)
            => ISourceMessage.GetQueenOfSauce("Trout Soup", hasRecipe);
        
        internal ISourceMessage BirthdayExampleMessage(ConfigEmojiMessage message, IEnumerable<object> names)
            => ISourceMessage.GetBirthdays(names, this.ModConfig);
        
        internal ISourceMessage WeatherExampleMessage(ConfigEmojiMessage message, WeatherIcons weatherDisplay)
            => new WeatherMessage.TestDisplay(weatherDisplay);
        
        #endregion
        #region Rendering
        
        public void ReRender<T>(object sender, T e)
            => this.ReRender();
        
        private void ReRender()
            => this.Examples.ForEach(message => message.Dirty = true);
        
        #endregion
        #region Translation Keys
        
        private IConfT9N Translation(string key)
            => new ConfigurationTranslation(this.Translations, key);
        private IConfT9N Translation(string mainKey, string descKey)
            => new ConfigurationTranslation(this.Translations, mainKey, descKey);
        
        private sealed class ConfigurationTranslation : IConfT9N {
            private readonly ITranslationHelper Translations;
            private readonly string MainKey;
            private readonly string DescKey;
            
            public ConfigurationTranslation(
                ITranslationHelper translations,
                string key
            ): this(translations, key, key is null ? null : $"{key}.desc") {}
            public ConfigurationTranslation(ITranslationHelper translations, string mainKey, string descKey) {
                this.Translations = translations;
                this.MainKey = mainKey is null ? null : $"config.{mainKey}";
                this.DescKey = descKey is null ? null : $"config.{descKey}";
            }
            
            /// <inheritdoc/>
            public string Get()
                => this.MainKey is null ? null : this.Translations.Get(this.MainKey);
            
            /// <inheritdoc/>
            public string GetDesc()
                => this.DescKey is null ? null : this.Translations.Get(this.DescKey);
        }
        
        #endregion
    }
}
