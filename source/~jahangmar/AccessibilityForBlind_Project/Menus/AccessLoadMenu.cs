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
using System.Collections.Generic;
using StardewModdingAPI;
using StardewValley.Menus;

namespace AccessibilityForBlind.Menus
{
    public class AccessLoadMenu : AccessTitleSubMenu
    {
        private int itemIdx = 0;
        private int itemCount;
        private List<LoadGameMenu.MenuSlot> menuSlots;

        private MenuItem confirmDeleteButton;
        private MenuItem backButton;

        public AccessLoadMenu(LoadGameMenu menu) : base(menu)
        {
            menuSlots = ModEntry.GetHelper().Reflection.GetField<List<LoadGameMenu.MenuSlot>>(menu, "menuSlots").GetValue();
            itemCount = menuSlots.Count;
            //ModEntry.Log($"menuSlots.Count == " + menuSlots.Count);
            //ModEntry.Log($"slotButtons.Count == " + menu.slotButtons.Count);
            //AddItem(menuItem);
            //LoadGameMenu.itemsPerPage
            //menu.slotButtons
            //menuSlots
            //down arrow
            //up arrow
            //deleteButtons
            ResetLoadButtons();
            confirmDeleteButton = MenuItem.MenuItemFromComponent(menu.okDeleteButton, menu, "delete save file");
            confirmDeleteButton.speakOnClickAction -= confirmDeleteButton.DefaultSpeakOnClickAction;
            //menuslots: all save file slots
            //slotButtons: only 4 buttons
        }

        public override string GetTitle()
        {
            return Title();
        }

        public static string Title() => "Load Menu";

        public void ResetLoadButtons()
        {
            LoadGameMenu menu = stardewMenu as LoadGameMenu;
            int currentItemIndex = ModEntry.GetHelper().Reflection.GetField<int>(menu, "currentItemIndex").GetValue();
            ClearItems();

            string ShortDescr(int idx)
            {
                StardewValley.Farmer farmer = (menuSlots[idx] as LoadGameMenu.SaveFileSlot).Farmer;
                return $"slot {idx + 1}: {farmer.Name} on {farmer.farmName} farm";
            }

            string LongDescr(int idx)
            {
                StardewValley.Farmer farmer = (menuSlots[idx] as LoadGameMenu.SaveFileSlot).Farmer;
                string season = "";
                switch (farmer.seasonForSaveGame)
                {
                    case 0: season = "spring"; break;
                    case 1: season = "summer"; break;
                    case 2: season = "fall"; break;
                    case 3: season = "winter"; break;
                    default: season = ""; break;
                }
                string timePlayed = StardewValley.Utility.getHoursMinutesStringFromMilliseconds(farmer.millisecondsPlayed);
                return $"Day {farmer.dayOfMonthForSaveGame} of {season} in year {farmer.yearForSaveGame}. Playtime {timePlayed}. Press delete button to delete.";
            }

            for (int j=0; j<System.Math.Min(itemCount,LoadGameMenu.itemsPerPage); j++)
            {
                MenuItem menuItem = MenuItem.MenuItemFromComponent(menu.slotButtons[j], menu, ShortDescr(j));
                menuItem.Description = LongDescr(j);
                menuItem.TextOnAction = "Loading " + ShortDescr(j);
                AddItem(menuItem);
            }

            for (int j=LoadGameMenu.itemsPerPage; j<itemCount; j++)
            {
                MenuItem menuItem = MenuItem.MenuItemFromComponent(menu.slotButtons[LoadGameMenu.itemsPerPage-1], menu, ShortDescr(j));
                menuItem.Description = LongDescr(j);
                menuItem.TextOnAction = "Loading " + ShortDescr(j);
                AddItem(menuItem);
            }

            backButton = MenuItem.MenuItemFromComponent(menu.backButton, StardewValley.Game1.activeClickableMenu);
            backButton.Label = "back to title";
            backButton.TextOnAction = AccessTitleMenu.Title();
            AddItem(backButton);
        }

        public override void NextItem()
        {
            LoadGameMenu menu = stardewMenu as LoadGameMenu;
            int currentItemIndex = ModEntry.GetHelper().Reflection.GetField<int>(menu, "currentItemIndex").GetValue();

            if (current != null && current != backButton)
                itemIdx += 1;
            else
                itemIdx = 0;

            if (itemIdx < itemCount)
            {
                if (itemIdx >= LoadGameMenu.itemsPerPage)
                {
                    ModEntry.GetHelper().Reflection.GetMethod(menu, "downArrowPressed").Invoke();
                }
            }
            else
            {
                itemIdx = 0;
                ModEntry.GetHelper().Reflection.GetField<int>(menu, "currentItemIndex").SetValue(0);
            }
            base.NextItem();
        }

        public override void PrevItem()
        {
            LoadGameMenu menu = stardewMenu as LoadGameMenu;
            int currentItemIndex = ModEntry.GetHelper().Reflection.GetField<int>(menu, "currentItemIndex").GetValue();

            if (current != null && current != backButton)
                itemIdx -= 1;
            else if (current == null)
            {
                itemIdx = 0;
                ModEntry.GetHelper().Reflection.GetField<int>(menu, "currentItemIndex").SetValue(0);
            }
            else if (current == backButton)
            {
                if (ModEntry.GetHelper().Reflection.GetField<int>(menu, "currentItemIndex").GetValue() == 0)//not scrolled
                {
                    for (int i=0; i<=itemCount-LoadGameMenu.itemsPerPage; i++)
                        ModEntry.GetHelper().Reflection.GetMethod(menu, "downArrowPressed").Invoke();
                }
                itemIdx = itemCount - 1;
            }

            if (itemIdx >= LoadGameMenu.itemsPerPage-1)
            {
                ModEntry.GetHelper().Reflection.GetMethod(menu, "upArrowPressed").Invoke();
            }
            else
            {
                 
            }
            base.PrevItem();
        }

        private bool reset = false;

        public override void ButtonPressed(SButton button)
        {
            if (reset)
            {
                reset = false;
                ModEntry.GetInstance().SelectMenu(stardewMenu as LoadGameMenu).ButtonPressed(button);
                return;
            }

            if (Inputs.IsMenuDeleteButton(button))
            {
                if (current != null && current != backButton)
                {
                    (stardewMenu as LoadGameMenu).deleteConfirmationScreen = true;
                    ModEntry.GetHelper().Reflection.GetField<int>((stardewMenu as LoadGameMenu), "selectedForDelete").SetValue(itemIdx);
                    string name = (menuSlots[itemIdx] as LoadGameMenu.SaveFileSlot).Farmer.Name;
                    TextToSpeech.Speak("Press enter to confirm to delete " + name + " save file, press escape to cancel");
                }
            }
            else if (Inputs.IsMenuEscapeButton(button))
            {
                if (!TextToSpeech.Speaking() && (stardewMenu as LoadGameMenu).deleteConfirmationScreen)
                {
                    (stardewMenu as LoadGameMenu).deleteConfirmationScreen = false;
                    ModEntry.GetHelper().Reflection.GetField<int>((stardewMenu as LoadGameMenu), "selectedForDelete").SetValue(-1);
                    TextToSpeech.Speak("canceled deleting");
                }
            }
            else if (Inputs.IsTTSInfoButton(button) || Inputs.IsTTSStopButton(button) || Inputs.IsTTSRepeatButton(button))
            {
                base.ButtonPressed(button);
            }
            else
            {
                if (!(stardewMenu as LoadGameMenu).deleteConfirmationScreen)
                    base.ButtonPressed(button);
                else
                {
                    if (Inputs.IsMenuActivateButton(button))
                    {
                        TextToSpeech.Speak("deleting save file");
                        confirmDeleteButton.Activate();
                        while (ModEntry.GetHelper().Reflection.GetField<int>((stardewMenu as LoadGameMenu), "currentItemIndex").GetValue() > 0)
                            ModEntry.GetHelper().Reflection.GetMethod((stardewMenu as LoadGameMenu), "upArrowPressed").Invoke();
                        reset = true;
                        TextToSpeech.Speak("finished deleting");
                    }
                }
            }

        }
    }
}
