/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/yuri0r/toolbelt
**
*************************************************/

using StardewModdingAPI;
using System.Collections.Generic;

namespace ToolBelt
{
    class ModConfig
    {
        public bool UseBackdrop { get; set; } = true;
        public int AnimationMilliseconds { get; set; } = 150;
        public bool LeftStickSelection { get; set; } = false;
        public bool HoverSelects { get; set; } = false;
        public bool SwapTools { get; set; } = true;
        public bool HorizontalSelect { get; set; } = false;
        public bool ConsiderHorseFlutAsTool { get; set; } = false;
        public SButton ToggleKey { get; set; } = SButton.LeftAlt;
        public List<string> BlacklistNames { get; set; } = new List<string>();
        public List<int> BlacklistIds { get; set; } = new List<int>();

    }
}
