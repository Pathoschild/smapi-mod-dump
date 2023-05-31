/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace Omegasis.Revitalize.Framework.HUD
{
    public static class HudUtilities
    {

        public static Queue<string> DialogueBoxMessages = new Queue<string>();


        public static void OnGameUpdateTicked()
        {
            TryToShowNextDialogueBoxMessage();
        }

        /// <summary>
        /// Shows the error message for the game's hud to say that the player's inventory is full.
        /// </summary>
        public static void ShowInventoryFullErrorMessage()
        {
            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
        }

        public static void AddDialogueBoxMessagesToShow(params string[] messages)
        {
            foreach (string message in messages)
            {
                DialogueBoxMessages.Enqueue(message);
            }
        }

        public static void TryToShowNextDialogueBoxMessage()
        {
            if(DialogueBoxMessages.Count > 0 && Game1.activeClickableMenu==null)
            {
                Game1.drawObjectDialogue(DialogueBoxMessages.Dequeue());
            }
        }
    }
}
