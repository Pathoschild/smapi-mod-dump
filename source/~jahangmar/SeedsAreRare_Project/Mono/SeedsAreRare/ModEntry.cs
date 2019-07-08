//Copyright (c) 2019 Jahangmar

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU Lesser General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//GNU Lesser General Public License for more details.

//You should have received a copy of the GNU Lesser General Public License
//along with this program. If not, see <https://www.gnu.org/licenses/>.


using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

using StardewValley.Menus;
using System.Collections.Generic;

namespace SeedsAreRare
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {

        private SeedsAreRareConfig config;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            config = helper.ReadConfig<SeedsAreRareConfig>();

            helper.Events.Display.MenuChanged += this.MenuChanged;
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
        }

        void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            if(!Game1.player.craftingRecipes.Keys.Contains("Seed Maker") && Game1.player.farmingLevel >= 1)
            {
                Monitor.Log("Adding Seed Maker recipe", LogLevel.Trace);
                Game1.player.craftingRecipes.Add("Seed Maker", 0);
            }

            if (Game1.player.farmingLevel >= 1)
                Helper.Events.GameLoop.DayStarted -= GameLoop_DayStarted;
        }


        /// <summary>Raised after a menu was opened.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        void MenuChanged(object sender, MenuChangedEventArgs e)
        {

            //check if we are in a shop menu
            if (e.NewMenu is ShopMenu shopMenu)
            {
                if (config.exclude_pierre && Game1.currentLocation is StardewValley.Locations.SeedShop)
                    return;
                if (config.exclude_oasis && shopMenu.portraitPerson != null && shopMenu.portraitPerson.Name.Equals("Sandy"))
                    return;
                if (config.exclude_traveling_merchant && Game1.currentLocation is StardewValley.Locations.Forest)
                    return;
                if (config.exclude_night_market && Game1.currentLocation is StardewValley.Locations.BeachNightMarket)
                    return;

                List<Item> shopInventory = this.Helper.Reflection.GetField<List<Item>>(shopMenu, "forSale").GetValue();

                //remove all seeds but do not remove the tree saplings (those are also categorized as seeds)
                shopInventory.RemoveAll((Item item) => item.Category == StardewValley.Object.SeedsCategory && !(config.exclude_rare_seed && item.Name.Equals("Rare Seed")) && !item.Name.EndsWith("Sapling", StringComparison.Ordinal));
                    
            }           
        }

    }
}