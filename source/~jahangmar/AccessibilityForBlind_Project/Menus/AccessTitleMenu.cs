/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jahangmar/StardewValleyMods
**
*************************************************/

// Copyright (c) 2020 Jahangmar
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
using System;
using StardewModdingAPI;

namespace AccessibilityForBlind.Menus
{
    public class AccessTitleMenu : AccessMenu
    {
        MenuItem muteButton;
        //private StardewValley.Menus.TitleMenu titleMenu;

        public AccessTitleMenu(StardewValley.Menus.TitleMenu titleMenu) : base(titleMenu)
        {
            MenuItem menuItem;
            menuItem = MenuItem.MenuItemFromComponent(titleMenu.buttons[0], titleMenu);
            menuItem.TextOnAction = AccessCharacterCreationMenu.Title();
            AddItem(menuItem);
            menuItem = MenuItem.MenuItemFromComponent(titleMenu.buttons[1], titleMenu);
            menuItem.TextOnAction = AccessLoadMenu.Title();
            AddItem(menuItem);
            menuItem = MenuItem.MenuItemFromComponent(titleMenu.buttons[2], titleMenu);
            AddItem(menuItem);
            menuItem = MenuItem.MenuItemFromComponent(titleMenu.buttons[3], titleMenu);
            menuItem.TextOnAction =  "Closing game";
            AddItem(menuItem);
            AddItem(MenuItem.MenuItemFromComponent(titleMenu.languageButton, titleMenu, "language"));
            muteButton = MenuItem.MenuItemFromComponent(titleMenu.muteMusicButton, titleMenu);
        }

        public override string GetTitle()
        {
            return Title();
        }

        public static string Title() => "Title Menu";

        public override void ButtonPressed(SButton button)
        {
            if (!ModEntry.GetHelper().Reflection.GetField<bool>(base.stardewMenu, "titleInPosition").GetValue())
                return;

            if (StardewValley.Menus.TitleMenu.subMenu != null)
            {
                AccessMenu menu = ModEntry.GetInstance().SelectMenu(StardewValley.Menus.TitleMenu.subMenu);
                if (menu != null)
                    menu.ButtonPressed(button);
                return;
            }

            base.ButtonPressed(button);
            if (Inputs.IsMenuTitleMuteButton(button))
            {
                muteButton.Activate();
            }
        }
    }
}
