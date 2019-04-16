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

using System.Collections.Generic;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;

namespace InteractionTweaks
{
    public class AdventurersGuildFeature : ModFeature
    {
        public static new void Enable()
        {
            Helper.Events.Display.MenuChanged += Display_MenuChanged;
        }

        public static new void Disable()
        {
            Helper.Events.Display.MenuChanged -= Display_MenuChanged;
        }

        public static void Display_MenuChanged(object sender, StardewModdingAPI.Events.MenuChangedEventArgs e)
        {
            if (Game1.activeClickableMenu is ShopMenu menu && !(menu is AdventurersGuildShopMenu) && Game1.currentLocation is AdventureGuild)
            {
                Monitor.Log("Replacing Adventure Guild ShopMenu with custom ShopMenu", LogLevel.Trace);
                Dictionary<Item, int[]> itemPriceAndStock = Helper.Reflection.GetField<Dictionary<Item, int[]>>(menu, "itemPriceAndStock").GetValue();
                Game1.activeClickableMenu = new AdventurersGuildShopMenu(Helper, Monitor, itemPriceAndStock, Config.SlingshotFeature);
            }
        }
    }
}
