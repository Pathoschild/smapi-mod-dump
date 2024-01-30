/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jaredtjahjadi/LogMenu
**
*************************************************/

using StardewModdingAPI;

namespace LogMenu
{
    internal class ModConfig
    {
        public bool StartFromBottom { get; set; } = true; // Option to have menu start from bottom or top. Default = true
        public bool OldestToNewest { get; set; } = true; // Option to have oldest messages at top of menu. Default = true
        public bool NonNPCDialogue { get; set; } = true; // Option to log non-NPC dialogue (e.g., when interacting with objects). Default = true
        public bool ToggleHUDMessages { get; set; } // Option to log HUD messages. Default = false
        public int LogLimit { get; set; } = 30; // Desired number of logged messages. Default = 30
        public SButton LogButton { get; set; } = SButton.L; // Desired key to open the dialogue list. Default = L
    }
}
