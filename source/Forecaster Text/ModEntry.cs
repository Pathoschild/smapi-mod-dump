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
using System.Reflection;
using ForecasterText.Objects;
using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace ForecasterText {
    public sealed class ModEntry : Mod {
        internal readonly TVEvents Events;
        internal readonly ForecasterConfigManager ConfigManager;
        private readonly VirtualTV Television = new();
        
        public string ModID => this.ModManifest.UniqueID;
        
        public ModEntry() {
            this.ConfigManager = new ForecasterConfigManager(this);
            this.Events = new TVEvents(
                this,
                this.ConfigManager
            );
        }
        
        /// <summary>
        /// Mod Initializer
        /// </summary>
        public override void Entry(IModHelper helper) {
            this.Helper.Events.GameLoop.DayStarted += this.Events.OnDayStart;
            this.Helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            this.Helper.Events.Multiplayer.PeerConnected += this.OnPeerJoin;
            
            // Reload example content when the language is changed
            this.Helper.Events.Content.LocaleChanged += this.ConfigManager.ReRender;
        }
        
        /// <summary>
        /// Register with the Config Mod when the game is launched
        /// </summary>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs args) {
            if (this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu") is IGenericModConfigMenuApi configMenu)
                this.ConfigManager.RegisterConfigManager(configMenu);
        }
        
        /// <summary>
        /// When a player joins the game, make sure they know the details of the day
        /// </summary>
        private void OnPeerJoin(object sender, PeerConnectedEventArgs peerConnectedEventArgs) {
            Farmer farmer = Game1.getOnlineFarmers()
                .FirstOrDefault(farmer => farmer.UniqueMultiplayerID == peerConnectedEventArgs.Peer.PlayerID && !this.PlayerHasMod(farmer));
            if (farmer is not null)
                this.Events.OnFarmerJoin(sender, farmer);
        }
        
        /// <summary>Check if another farmer has the ForecasterTexts mod</summary>
        public bool PlayerHasMod(Farmer farmer)
            => this.PlayerHasMod(farmer, this.ModID);
        
        /// <summary>Check if another farmer has a specific mod</summary>
        public bool PlayerHasMod(Farmer farmer, string modId) {
            if (Game1.player == farmer)
                return this.Helper.ModRegistry.Get(modId) is not null;
            return this.Helper.Multiplayer.GetConnectedPlayer(farmer.UniqueMultiplayerID) is { HasSmapi: true } peer
                && peer.GetMod(modId) is not null;
        }
        
        public IRecipeFinder GetRecipeFinder(Farmer farmer)
            => new RecipeFinder(this, farmer);
        
        private sealed class RecipeFinder : IRecipeFinder {
            private readonly ModEntry Mod;
            private readonly DayOfWeek DayOfWeek;
            private readonly Farmer Farmer;
            private FarmerTeam Team => this.Farmer.team;
            private VirtualTV Television => this.Mod.Television;
            
            public RecipeFinder(ModEntry mod, Farmer farmer) {
                this.Mod = mod;
                this.DayOfWeek = (DayOfWeek)(Game1.dayOfMonth % 7);
                this.Farmer = farmer;
            }
            
            /// <inheritdoc/>
            public string GetAnyRecipe() => this.DayOfWeek switch {
                DayOfWeek.Sunday or DayOfWeek.Wednesday => this.GetRegularRecipes(),
                DayOfWeek.Friday => this.GetAnimalHusbandryRecipes(),
                _ => null
            };
            
            private string GetRegularRecipes() {
                uint played = Game1.stats.DaysPlayed;
                if (played < 5)
                    return null;
                
                int num = (int)(Game1.stats.DaysPlayed % 224U / 7U);
                if (played % 224U == 0U)
                    num = 32;
                
                switch (this.DayOfWeek) {
                    case DayOfWeek.Sunday:
                        break;
                    case DayOfWeek.Wednesday:
                        if (this.Team.lastDayQueenOfSauceRerunUpdated.Value != Game1.Date.TotalDays) {
                            this.Team.lastDayQueenOfSauceRerunUpdated.Set(Game1.Date.TotalDays);
                            this.Team.queenOfSauceRerunWeek.Set(this.Television.GetRerunWeek());
                        }
                        num = this.Team.queenOfSauceRerunWeek.Value;
                        break;
                    default: return null;
                }
                
                // Dictionary of recipes
                Dictionary<string, string> dictionary = Game1.temporaryContent.Load<Dictionary<string, string>>("Data\\TV\\CookingChannel");
                if (!dictionary.TryGetValue($"{num}", out string translation))
                    return null;
                
                // Split the translation info
                string[] recipeInfo = translation.Split('/');
                
                // Get the recipe name
                return recipeInfo.Length <= 0 ? null : recipeInfo[0];
            }
            
            private string GetAnimalHusbandryRecipes() {
                if (!this.Mod.PlayerHasMod(this.Farmer, "Digus.AnimalHusbandryMod"))
                    return null;
                
                if (
                    Type.GetType("AnimalHusbandryMod.recipes.MeatFridayChannel, AnimalHusbandryMod") is not Type type
                    || Activator.CreateInstance(type) is not object fridayChannel
                    || type.GetMethod("GetRecipeNumber", BindingFlags.NonPublic | BindingFlags.Static) is not MethodInfo getRecipeNumber
                    || getRecipeNumber.Invoke(null, Array.Empty<object>()) is not int recipeNumber
                    || type.GetField("_recipes", BindingFlags.NonPublic | BindingFlags.Instance) is not FieldInfo recipeField
                    || recipeField.GetValue(fridayChannel) is not Dictionary<int, string> recipes
                ) {
                    this.Mod.Monitor.Log("Reflection failed", LogLevel.Error);
                    return null;
                }
                
                // Split the translation info
                string[] recipeInfo = recipes[recipeNumber].Split('/');
                
                // Get the recipe name
                return recipeInfo.Length <= 0 ? null : recipeInfo[0];
            }
        }
    }
}
