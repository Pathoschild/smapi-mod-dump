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

using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace ForecasterText {
    public sealed class ModEntry : Mod {
        internal readonly TVEvents Events;
        private readonly ForecasterConfigManager ConfigManager;
        
        public string ModID => this.ModManifest.UniqueID;
        
        public ModEntry() {
            this.ConfigManager = new ForecasterConfigManager(this);
            this.Events = new TVEvents(this.ConfigManager);
        }
        
        /// <summary>
        /// Mod Initializer
        /// </summary>
        public override void Entry(IModHelper helper) {
            this.Helper.Events.GameLoop.DayStarted += this.Events.OnDayStart;
            this.Helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        }
        
        /// <summary>
        /// Register with the Config Mod when the game is launched
        /// </summary>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs args) {
            if (this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu") is IGenericModConfigMenuApi configMenu)
                this.ConfigManager.RegisterConfigManager(configMenu);
        }
        
        /// <summary>Check if the main player knows a recipe</summary>
        public static bool PlayerHasRecipe(string recipe)
            => ModEntry.PlayerHasRecipe(Game1.player, recipe);
        
        /// <summary>Check if a farmer knows a recipe</summary>
        public static bool PlayerHasRecipe(Farmer farmer, string recipe)
            => farmer.cookingRecipes.ContainsKey(recipe);
        
        /// <summary>Check if the main player has been to ginger island</summary>
        public static bool PlayerBeenToIsland()
            => ModEntry.PlayerBeenToIsland(Game1.player);
        
        /// <summary>Check if a farmer has been to ginger island</summary>
        public static bool PlayerBeenToIsland(Farmer farmer)
            => farmer.hasOrWillReceiveMail("Visited_Island");
    }
}
