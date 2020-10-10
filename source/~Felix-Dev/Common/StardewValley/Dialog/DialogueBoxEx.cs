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
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FelixDev.StardewMods.Common.StardewValley
{
    /// <summary>
    /// Adds support for a custom newline indicator (defined in DialogHelper) to the in-game 
    /// class [DialogueBox]. 
    /// </summary>
    public class DialogueBoxEx : DialogueBox
    {
        /// <summary>
        /// A reference to the private field [characterDialoguesBrokenUpRef]. This field contains
        /// the current dialog string to be printed, sliced up in multiple pages to fit inside the dialog box. Each entry
        /// stands for one page to be displayed. We will parse each entry for our custom newline indicators.
        /// </summary>
        private FieldInfo characterDialoguesBrokenUpRef;

        private FieldInfo characterDialogueRef;

        private string dialogueBefore;

        private Type dialogueBoxType;

        public DialogueBoxEx(Dialogue dialogue) : base(dialogue)
        {
            dialogueBoxType = typeof(DialogueBox);
            characterDialoguesBrokenUpRef = dialogueBoxType.GetField(
                "characterDialoguesBrokenUp",
                BindingFlags.NonPublic | BindingFlags.Instance)
                ?? throw new MissingFieldException(nameof(DialogueBox), "characterDialoguesBrokenUp");

            characterDialogueRef = dialogueBoxType.GetField(
                "characterDialogue",
                BindingFlags.NonPublic | BindingFlags.Instance)
                ?? throw new MissingFieldException(nameof(DialogueBox), "characterDialogue");

            var currentDialogPages = (Stack<string>)characterDialoguesBrokenUpRef.GetValue(this);
            dialogueBefore = currentDialogPages.Peek();

            BreakUpDialogInPages(dialogue);
        }

        /// <summary>
        /// Handles our custom newlines added to dialogs.
        /// </summary>
        /// <param name="dialogue">The dialog to parse.</param>
        private void BreakUpDialogInPages(Dialogue dialogue)
        {
            var currentDialogPages = (Stack<string>)characterDialoguesBrokenUpRef.GetValue(this);

            // Replace our custom newline indicators for the current dialog string with the newline indicator 
            // used by the in-game text parser (SpriteText.cs).
            currentDialogPages = new Stack<string>(currentDialogPages.Select(str => str.Replace(DialogHelper.GetNewlineIndicator(), SpriteText.newLine.ToString())).Reverse());

            List<string> fixedSplittedDialogPages = new List<string>();

            string currentDialogPage = currentDialogPages.Pop();
            while (!String.IsNullOrEmpty(currentDialogPage))
            {
                // Get the first part of the current dialog part to be displayed which fits inside the dialog box.
                string pageString = SpriteText.getStringPreviousToThisHeightCutoff(currentDialogPage, this.width - 460 - 20, this.height - 16);

                // Split the current dialog string into multiple pages if the string would be displayed
                // outside the bounds of the dialog box.
                //
                // The game already splits up a dialog into multiple pages, however, I have noticed
                // that sometimes it incorrectly splits the text so that a small part is still displayed
                // outside the dialog box.
                if (pageString.Length < currentDialogPage.Length)
                {
                    fixedSplittedDialogPages.Add(pageString.Trim());

                    string str2 = currentDialogPage.Replace(Environment.NewLine, "");

                    // Combine our splitted text for the next page with the splitted part by the game.
                    currentDialogPage = str2.Substring(pageString.Length).Trim() + ((currentDialogPages.Count > 0) ? currentDialogPages.Pop() : "");
                }
                else
                {
                    fixedSplittedDialogPages.Add(currentDialogPage);
                    currentDialogPage = (currentDialogPages.Count > 0) ? currentDialogPages.Pop() : "";
                }
            }

            // Update the page stack to display the splitted text.
            Stack<string> pageStack = new Stack<string>();
            for (int i = fixedSplittedDialogPages.Count - 1; i >= 0; i--)
            {
                pageStack.Push(fixedSplittedDialogPages[i]);
            }

            characterDialoguesBrokenUpRef.SetValue(this, pageStack);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);

            // Parse the next dialog string to be printed for our custom newlines
            var splitDialogsAfter = (Stack<string>) characterDialoguesBrokenUpRef.GetValue(this); 
            if (splitDialogsAfter.Count > 0 && !dialogueBefore.Equals(splitDialogsAfter.Peek()))
            {
                dialogueBefore = splitDialogsAfter.Peek();
                BreakUpDialogInPages((Dialogue)characterDialogueRef.GetValue(this));
            }
        }
    }
}
