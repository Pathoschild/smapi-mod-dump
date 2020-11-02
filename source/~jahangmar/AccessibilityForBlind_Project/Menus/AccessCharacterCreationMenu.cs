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
using StardewValley.Menus;
using StardewValley;

namespace AccessibilityForBlind.Menus
{
    public class AccessCharacterCreationMenu : AccessTitleSubMenu
    {
        public AccessCharacterCreationMenu(StardewValley.Menus.CharacterCustomization characterCustomization) : base(characterCustomization)
        {
            TextBox GetTextBox(string name) =>
                ModEntry.GetHelper().Reflection.GetField<TextBox>(characterCustomization, name).GetValue();

            MenuItem menuItem;
            foreach (var button in characterCustomization.genderButtons)
            {
                menuItem = MenuItem.MenuItemFromComponent(button, characterCustomization);
                menuItem.Label = button.name;
                menuItem.TextOnAction = "Gender set to " + menuItem.Label;
                AddItem(menuItem);
            }

            menuItem = MenuTextBox.MenuTextBoxFromComponent(characterCustomization.nameBoxCC, GetTextBox("nameBox"), characterCustomization);
            menuItem.Label = "Character name";
            AddItem(menuItem);
            menuItem = MenuTextBox.MenuTextBoxFromComponent(characterCustomization.farmnameBoxCC, GetTextBox("farmnameBox"), characterCustomization);
            menuItem.Label = "Farm name";
            menuItem.TextOnAction = "farm";
            AddItem(menuItem);
            menuItem = MenuTextBox.MenuTextBoxFromComponent(characterCustomization.favThingBoxCC, GetTextBox("favThingBox"), characterCustomization);
            menuItem.Label = "Favourite thing";
            AddItem(menuItem);

            ClickableComponent catButton = null;
            foreach(ClickableComponent comp in characterCustomization.rightSelectionButtons)
            {
                if (comp.name.Equals("Pet"))
                {
                    catButton = comp;
                }
            }
            if (catButton != null)
            {
                MenuItem petButton = MenuItem.MenuItemFromComponent(catButton, characterCustomization);
                petButton.Label = "change pet";
                petButton.speakOnClickAction -= petButton.DefaultSpeakOnClickAction;
                string breedToString()
                {
                    if (Game1.player.catPerson)
                    {
                        switch (Game1.player.whichPetBreed)
                        {
                            case 0: return "orange cat";
                            case 1: return "grey cat";
                            case 2: return "yellow cat";
                            default: return "unknown";
                        }
                    }
                    else
                    {
                        switch (Game1.player.whichPetBreed)
                        {
                            case 0: return "Laprador Retriever";
                            case 1: return "German Shepherd";
                            case 2: return "Bloodhound";
                            default: return "unknown";
                        }
                    }
                }
                petButton.speakOnClickAction += () => { TextToSpeech.Speak("selected " + breedToString() + " as pet"); };
                AddItem(petButton);
            }
            else
                ModEntry.Log("couldn't find pet button", LogLevel.Error);


            menuItem = MenuItem.MenuItemFromComponent(characterCustomization.skipIntroButton, characterCustomization);
            menuItem.Label = "skip intro";
            menuItem.speakOnClickAction -= menuItem.DefaultSpeakOnClickAction;
            menuItem.speakOnClickAction += () =>
            {
                bool skipIntro = ModEntry.GetHelper().Reflection.GetField<bool>(characterCustomization, "skipIntro").GetValue();
                TextToSpeech.Speak(skipIntro ? "skipping intro" : "playing intro");
            };
            AddItem(menuItem);

            foreach (ClickableTextureComponent comp in characterCustomization.farmTypeButtons)
            {
                menuItem = MenuItem.MenuItemFromComponent(comp, characterCustomization);
                menuItem.Label = comp.name + " farm type";
                menuItem.TextOnAction = "selected "+comp.name + " farm type";
                menuItem.Description = comp.hoverText.Split('_')[1];
                AddItem(menuItem);
            }

            menuItem = MenuItem.MenuItemFromComponent(characterCustomization.okButton, characterCustomization);
            menuItem.Label = "start game";
            menuItem.speakOnClickAction -= menuItem.DefaultSpeakOnClickAction;
            menuItem.speakOnClickAction += () =>
            {
                if (characterCustomization.canLeaveMenu())
                    TextToSpeech.Speak("starting new game");
                else
                    TextToSpeech.Speak("enter character name, farm name, and favourite thing first");
            };
            AddItem(menuItem);

            menuItem = MenuItem.MenuItemFromComponent(characterCustomization.backButton, StardewValley.Game1.activeClickableMenu);
            menuItem.Label = "back to title";
            menuItem.TextOnAction = AccessTitleMenu.Title();
            AddItem(menuItem);
        }

        public override string GetTitle()
        {
            return Title();
        }

        public static string Title() => "Character creation";

    }
}
