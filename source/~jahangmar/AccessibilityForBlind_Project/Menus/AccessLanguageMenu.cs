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
using StardewValley.Menus;

namespace AccessibilityForBlind.Menus
{
    public class AccessLanguageMenu : AccessTitleSubMenu
    {
        public AccessLanguageMenu(LanguageSelectionMenu menu, TitleMenu titleMenu) : base(menu)
        {
            foreach (ClickableComponent comp in menu.languages)
            {
                AddItem(MenuItem.MenuItemFromComponent(comp, menu));
            }
            MenuItem menuItem = MenuItem.MenuItemFromComponent(titleMenu.backButton, StardewValley.Game1.activeClickableMenu);
            menuItem.Label = "back to title";
            menuItem.TextOnAction = AccessTitleMenu.Title();
            AddItem(menuItem);

        }

        public override string GetTitle()
        {
            return Title();
        }

        public static string Title() => "Languange Menu";

    }
}
