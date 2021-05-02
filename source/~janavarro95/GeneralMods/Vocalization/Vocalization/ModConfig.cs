/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System.Collections.Generic;
using Vocalization.Framework;

namespace Vocalization
{
    /// <summary>The configuration file for the mod.</summary>
    public class ModConfig
    {

        /// <summary>The volume at which the sound for voices is played at.</summary>
        public decimal voiceVolume = 1;

        /// <summary>
        /// Should the mod automatically mute the game's npc dialogue typing sound?
        /// </summary>
        public bool muteDialogueTyping = true;

        public string menuHotkey = "Y";

        /// <summary>Handles all of the translation information and parsing.</summary>
        public TranslationInfo translationInfo = new TranslationInfo();

        /// <summary>Keeps track of the voice modes for determining how much audio is played.</summary>
        public List<string> modes = new List<string> { "Simple", "Full", "HeartEvents", "SimpleAndHeartEvents" };

        /// <summary>The current mode for the mod.</summary>
        public string currentMode = "Full";

        public bool ShopDialogueEnabled=true;
        public bool TVDialogueEnabled = true;
        public bool LetterDialogueEnabled = true;

        public bool Developer_ScrapeOnlyEnglishDialogue=true;

        /// <summary>Validates</summary>
        public void verifyValidMode()
        {
            if (!this.modes.Contains(this.currentMode))
            {
                Vocalization.ModMonitor.Log("Invalid configuration: " + this.currentMode + ". Changing to Full voiced mode.");
                this.currentMode = "Full";
            }
        }
    }
}
