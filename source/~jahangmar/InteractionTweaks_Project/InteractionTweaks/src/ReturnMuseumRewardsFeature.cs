// Copyright (c) 2019 Jahangmar
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.
using StardewValley;
using StardewValley.Menus;
using StardewValley.Locations;
namespace InteractionTweaks
{
    public class ReturnMuseumRewardsFeature : ModFeature
    {
        public static new void Enable()
        {
            Helper.Events.Display.MenuChanged += Display_MenuChanged;
        }

        public static new void Disable()
        {
            Helper.Events.Display.MenuChanged -= Display_MenuChanged;
        }

        static void Display_MenuChanged(object sender, StardewModdingAPI.Events.MenuChangedEventArgs e)
        {
            if (Game1.currentLocation is LibraryMuseum && e.NewMenu is ItemGrabMenu grabMenu)
            {
                grabMenu.reverseGrab = true;
            }
        }

    }
    /*

    class ReturnItemGrabMenu : ItemGrabMenu
    {
        public ReturnItemGrabMenu(ItemGrabMenu oldMenu) : base(inventory, reverseGrab, showReceivingMenu, highlightFunction, behaviorOnItemSelectFunction, message, behaviorOnItemGrab, snapToBottom, canBeExitedWithKey, playRightClickSound, allowRightClick, showOrganizeButton, source, sourceItem, whichSpecialButton, context)

        {

        }
    }
    */
}
