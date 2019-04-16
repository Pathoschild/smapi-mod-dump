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

using StardewValley;
using StardewValley.Menus;

using StardewModdingAPI;

namespace InteractionTweaks
{
    public class CarpenterMenuFeature : ModFeature
    {
        public static new void Enable()
        {
            //enabled = true;
            Helper.Events.Display.MenuChanged += Display_MenuChanged;
        }

        public static new void Disable()
        {
            //enabled = false;
            Helper.Events.Display.MenuChanged -= Display_MenuChanged;
        }

        static void Display_MenuChanged(object sender, StardewModdingAPI.Events.MenuChangedEventArgs e)
        {
            if (e.NewMenu is CarpenterMenu carpenterMenu && !(e.NewMenu is ModCarpenterMenu))
            {
                Monitor.Log("Replacing carpenter menu with custom menu", LogLevel.Trace);
                bool magicalConstruction = Helper.Reflection.GetField<bool>(carpenterMenu, "magicalConstruction").GetValue();
                Game1.activeClickableMenu = new ModCarpenterMenu(Monitor, Helper, magicalConstruction);
            }
        }

    }
}
