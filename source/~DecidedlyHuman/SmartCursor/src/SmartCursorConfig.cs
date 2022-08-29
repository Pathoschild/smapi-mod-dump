/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;

namespace SmartCursor
{
    public class SmartCursorConfig
    {
        // Keybinds
        public SButton SmartCursorHold = SButton.LeftControl;

        // Toggles
        public bool AllowTargetingBabyTrees = false;
        public bool AllowTargetingGiantCrops = false;
        public bool AllowTargetingTappedTrees = false;
    }
}
