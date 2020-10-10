/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Felix-Dev/StardewMods
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Text;

namespace FelixDev.StardewMods.Common.StardewValley
{
    /// <summary>
    /// Wraps the in-game draw dialogue method to support additional features, 
    /// such as custom newline indicators which are unavailable otherwise.
    /// </summary>
    public class DialogHelper
    {
        /// <summary> The newline indicator the mods can use for dialogs. </summary>
        public const string DIALOG_NEWLINE = "\n";

        /// <summary> Alternatve newline indicator. Internal use only. </summary>
        private const string DIALOG_NEWLINE_ALTERNATIVE = "\r\n";

        /// <summary>
        /// Get the newline indicator used internally by the [DialogHelper].
        /// </summary>
        /// <returns>The used newline indicator. </returns>
        public static string GetNewlineIndicator()
        {
            return Environment.NewLine.Equals(DIALOG_NEWLINE) ? DIALOG_NEWLINE_ALTERNATIVE : DIALOG_NEWLINE;
        }

        /// <summary>
        /// Draw a dialog with support for custom newlines.
        /// </summary>
        /// <param name="text">The dialog to print.</param>
        /// <param name="speaker">The speaker of the dialog. </param>
        public static void DrawDialog(string text, NPC speaker)
        {
            if (text == null || speaker == null)
            {
                return;
            }

            // Convert newline character, because the game removes newline characters without processing them
            if (text.Contains(DIALOG_NEWLINE) && Environment.NewLine.Equals(DIALOG_NEWLINE))
            {
                text.Replace(DIALOG_NEWLINE, DIALOG_NEWLINE_ALTERNATIVE);
            }

            var dialogBox = new DialogueBoxEx(new Dialogue(text, speaker));
            Game1.activeClickableMenu = dialogBox;
        }
    }
}
