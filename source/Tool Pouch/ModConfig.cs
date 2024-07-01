/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Brandon22Adams/ToolPouch
**
*************************************************/

using StardewModdingAPI;
using System.Collections.Generic;

namespace ToolPouch
{
    class ModConfig
    {
        public int BagCapacity { get; set; } = 9;
        public bool UseBackdrop { get; set; } = true;
        public int AnimationMilliseconds { get; set; } = 150;
        public bool LeftStickSelection { get; set; } = false;
        public bool HoverSelects { get; set; } = false;
        public SButton ToggleKey { get; set; } = SButton.LeftAlt;

        public SButton ControllerToggleKey { get; set; } = SButton.LeftStick;

        public bool DisableOpeningPouchOutsideOfInventory = false;

    }
}
