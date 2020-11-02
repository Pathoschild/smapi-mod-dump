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
using System.Collections.Generic;
using StardewValley;
using StardewValley.Menus;

namespace AccessibilityForBlind.Menus
{
    public class AccessDialogBox : AccessMenu
    {
        public AccessDialogBox(IClickableMenu menu) : base(menu)
        {
            DialogueBox dialogueBox = (menu as DialogueBox);
            List<Response> responses = ModEntry.GetHelper().Reflection.GetField<List<Response>>(dialogueBox, "responses").GetValue();

            for (int i=0; i<dialogueBox.allClickableComponents.Count; i++)
            {
                AddItem(MenuItem.MenuItemFromComponent(dialogueBox.allClickableComponents[i], dialogueBox, responses[i].responseText));
            }
            SpeakCurrentText();
        }

        private void SpeakCurrentText()
        {
            TextToSpeech.Speak((stardewMenu as DialogueBox).getCurrentString());
        }

        public override string GetTitle()
        {
            return "";
        }
    }
}
